using HREngine.API;
using HREngine.API.Actions;
using HREngine.API.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
//using System.Linq;

 

namespace HREngine.Bots
{
    

    public class Bot : IBot
   {
       private int concedeLvl = 5; // the rank, till you want to concede
       private int dirtytarget = -1;
       private int dirtychoice = -1;
       private string choiceCardId = "";
       PenalityManager penman = PenalityManager.Instance;
       Silverfish sf;

       public Bot()
       {
           OnBattleStateUpdate = HandleOnBattleStateUpdate;
           OnMulliganStateUpdate = HandleBattleMulliganPhase;
           bool concede = false;
           bool writeToSingleFile = false;
           try
           {
               concede = (HRSettings.Get.ReadSetting("silverfish.xml", "uai.autoconcede") == "true") ? true : false;
               writeToSingleFile = (HRSettings.Get.ReadSetting("silverfish.xml", "uai.singleLog") == "true") ? true : false;
           }
           catch
           {
               Helpfunctions.Instance.ErrorLog("a wild error occurrs! cant read the settings...");
           }
           try
           {
               this.concedeLvl = Convert.ToInt32((HRSettings.Get.ReadSetting("silverfish.xml", "uai.concedelvl")));
               if (this.concedeLvl >= 20) this.concedeLvl = 20;
               if (concede)
               {
                   Helpfunctions.Instance.ErrorLog("concede till rank " + concedeLvl);
               }
           }
           catch
           {
               Helpfunctions.Instance.ErrorLog("cant read your concede-Lvl");
           }

           this.sf = new Silverfish(writeToSingleFile);





           Mulligan.Instance.setAutoConcede(concede);

           sf.setnewLoggFile();

           try
           {
               int enfacehp = Convert.ToInt32((HRSettings.Get.ReadSetting("silverfish.xml", "uai.enemyfacehp")));
               Helpfunctions.Instance.ErrorLog("set enemy-face-hp to: " + enfacehp);
               ComboBreaker.Instance.attackFaceHP = enfacehp;
           }
           catch
           {
               Helpfunctions.Instance.ErrorLog("error in reading enemy-face-hp");
           }

           try
           {
               int mxwde = Convert.ToInt32((HRSettings.Get.ReadSetting("silverfish.xml", "uai.maxwide")));
               if (mxwde != 3000)
               {
                   Ai.Instance.setMaxWide(mxwde);
                   Helpfunctions.Instance.ErrorLog("set maxwide to: " + mxwde);
               }
           }
           catch
           {
               Helpfunctions.Instance.ErrorLog("error in reading Maxwide from settings, please recheck the entry");
           }

           try
           {
               bool twots = (HRSettings.Get.ReadSetting("silverfish.xml", "uai.simulateTwoTurns") == "true") ? true : false;
               if (twots)
               {
                   Ai.Instance.setTwoTurnSimulation(twots);
                   Helpfunctions.Instance.ErrorLog("activated two turn simulation");
               }



           }
           catch
           {
               Helpfunctions.Instance.ErrorLog("error in reading two-turn-simulation from settings");
           }

           try
           {
               bool playaround = (HRSettings.Get.ReadSetting("silverfish.xml", "uai.playAround") == "true") ? true : false;

               if (playaround)
               {
                   Ai.Instance.setPlayAround(playaround);
                   Helpfunctions.Instance.ErrorLog("activated playaround");
               }

           }
           catch
           {
               Helpfunctions.Instance.ErrorLog("error in reading two-turn-simulation from settings");
           }

           Helpfunctions.Instance.ErrorLog("write to single log file is: " + writeToSingleFile);

           bool teststuff = false;
           bool printstuff = false;
           try
           {

               printstuff = (HRSettings.Get.ReadSetting("silverfish.xml", "uai.longteststuff") == "true") ? true : false;
               teststuff = (HRSettings.Get.ReadSetting("silverfish.xml", "uai.teststuff") == "true") ? true : false;
           }
           catch
           {
               Helpfunctions.Instance.ErrorLog("something went wrong with simulating stuff!");
           }

           if (teststuff)
           {
               Ai.Instance.autoTester(this, printstuff);
           }
       }

      public int getPlayfieldValue(Playfield p)
      {
          if (p.value >= -2000000) return p.value;
          int retval = 0;
          retval -= p.evaluatePenality;
          retval += p.owncards.Count * 1;

          retval += p.ownMaxMana;
          retval -= p.enemyMaxMana;

          retval += p.ownMinions.Count * 10;
          retval -= p.enemyMinions.Count * 10;

          retval += p.ownMaxMana * 20 - p.enemyMaxMana * 20;

          if (p.ownHeroHp + p.ownHeroDefence > 10)
          {
              retval += p.ownHeroHp + p.ownHeroDefence;
          }
          else
          {
              retval -= (11 - p.ownHeroHp - p.ownHeroDefence) * (11 - p.ownHeroHp - p.ownHeroDefence);
          }

          retval += -p.enemyHeroHp - p.enemyHeroDefence;

          if (p.ownWeaponAttack >= 1)
          {
              retval += p.ownWeaponAttack * p.ownWeaponDurability;
          }

          if (!p.enemyHeroFrozen)
          {
              retval -= p.enemyWeaponDurability * p.enemyWeaponAttack;
          }
          else
          {
              if (p.enemyWeaponDurability >= 1)
              {
                  retval += 12;
              }
          }

          retval += p.owncarddraw * 5;
          retval -= p.enemycarddraw * 5;

          int owntaunt = 0;
          int ownMinionsCount = 0;
          foreach (Minion m in p.ownMinions)
          {
              retval += m.Hp * 1;
              retval += m.Angr * 2;
              retval += m.handcard.card.rarity;
              if (m.windfury) retval += m.Angr;
              if (m.divineshild) retval += 1;
              if (m.stealth) retval += 1;
              if (!m.taunt && m.stealth && penman.specialMinions.ContainsKey(m.name)) retval += 20;
              //if (m.poisonous) retval += 1;
              if (m.divineshild && m.taunt) retval += 4;
              if (m.taunt && m.handcard.card.specialMin == CardDB.specialMinions.frog) owntaunt++;
              if (m.Angr > 1 || m.Hp > 1) ownMinionsCount++;
              if (m.handcard.card.hasEffect) retval += 1;
              if (m.handcard.card.specialMin == CardDB.specialMinions.silverhandrecruit && m.Angr == 1 && m.Hp == 1) retval -= 5;
              if (m.handcard.card.specialMin == CardDB.specialMinions.direwolfalpha || m.handcard.card.specialMin == CardDB.specialMinions.flametonguetotem || m.handcard.card.specialMin == CardDB.specialMinions.stormwindchampion || m.handcard.card.specialMin == CardDB.specialMinions.raidleader) retval += 10;
              if (m.handcard.card.specialMin == CardDB.specialMinions.bloodmagethalnos) retval += 10;
          }

          if (p.enemyMinions.Count >= 0)
          {
              int anz = p.enemyMinions.Count;
              if (owntaunt == 0) retval -= 10 * anz;
              retval += owntaunt * 10 - 11 * anz;
          }

          int playmobs = 0;
          bool useAbili = false;
          bool usecoin = false;
          foreach (Action a in p.playactions)
          {
              if (a.heroattack && p.enemyHeroHp <= p.attackFaceHP) retval++;
              if (a.useability) useAbili = true;
              if (a.useability && a.handcard.card.specialMin == CardDB.specialMinions.lesserheal && ((a.enemytarget >= 10 && a.enemytarget <= 20) || a.enemytarget == 200)) retval -= 5;
              if (!a.cardplay) continue;
              if ((a.handcard.card.specialMin == CardDB.specialMinions.thecoin || a.handcard.card.specialMin == CardDB.specialMinions.innervate)) usecoin = true;
              if (a.handcard.card.type == CardDB.cardtype.MOB) playmobs++;
              //if (a.handcard.card.name == "arcanemissiles" && a.numEnemysBeforePlayed == 0) retval -= 10; // arkane missles on enemy hero is bad :D

              if (a.handcard.card.specialMin == CardDB.specialMinions.flamestrike && a.numEnemysBeforePlayed <= 2) retval -= 20;
              //save spell for all classes: (except for rouge if he has no combo)
              if (p.ownHeroName != HeroEnum.thief && a.handcard.card.type == CardDB.cardtype.SPELL && (a.numEnemysBeforePlayed == 0 || a.enemytarget == 200) && a.handcard.card.specialMin != CardDB.specialMinions.shieldblock) retval -= 11;
              if (p.ownHeroName == HeroEnum.thief && a.handcard.card.type == CardDB.cardtype.SPELL && (a.enemytarget == 200)) retval -= 11;
          }
          if (usecoin && useAbili && p.ownMaxMana <= 2) retval -= 20;
          if (usecoin && p.mana >= 1) retval -= 20;

          int mobsInHand = 0;
          foreach (Handmanager.Handcard hc in p.owncards)
          {
              if (hc.card.type == CardDB.cardtype.MOB && hc.card.Attack >= 3) mobsInHand++;
          }

          if (ownMinionsCount - p.enemyMinions.Count >= 4 && mobsInHand >= 1)
          {
              retval += mobsInHand * 25;
          }



          foreach (Minion m in p.enemyMinions)
          {
              retval -= m.Hp * 2;
              if (!m.frozen && !(m.handcard.card.specialMin == CardDB.specialMinions.ancientwatcher && !m.silenced))
              {
                  retval -= m.Angr * 2;
                  if (m.windfury) retval -= m.Angr;
                  if (m.Angr >= 4) retval -= 20;
                  if (m.Angr >= 7) retval -= 50;
              }
              if (m.Angr == 0) retval += 5;
              retval -= m.handcard.card.rarity;
              if (m.taunt) retval -= 5;
              if (m.divineshild) retval -= m.Angr;
              if (m.divineshild && m.taunt) retval -= 5;
              if (m.stealth) retval -= 1;

              if (m.poisonous) retval -= 4;

              if (penman.priorityTargets.ContainsKey(m.name) && !m.silenced) retval -= penman.priorityTargets[m.name];


          }

          retval -= p.enemySecretCount;
          retval -= p.lostDamage;//damage which was to high (like killing a 2/1 with an 3/3 -> => lostdamage =2
          retval -= p.lostWeaponDamage;
          if (p.ownMinions.Count == 0) retval -= 20;
          if (p.enemyMinions.Count == 0) retval += 20;
          if (p.enemyHeroHp <= 0) retval = 10000;
          //soulfire etc
          int deletecardsAtLast = 0;
          foreach (Action a in p.playactions)
          {
              if (!a.cardplay) continue;
              if (a.handcard.card.specialMin == CardDB.specialMinions.soulfire || a.handcard.card.specialMin == CardDB.specialMinions.doomguard || a.handcard.card.specialMin == CardDB.specialMinions.succubus) deletecardsAtLast = 1;
              if (deletecardsAtLast == 1 && !(a.handcard.card.specialMin == CardDB.specialMinions.soulfire || a.handcard.card.specialMin == CardDB.specialMinions.doomguard || a.handcard.card.specialMin == CardDB.specialMinions.succubus)) retval -= 20;
          }
          if (p.enemyHeroHp >= 1 && p.guessingHeroHP <= 0) retval -= 1000;
          if (p.ownHeroHp <= 0) retval = -10000;

          p.value = retval;
          return retval;
      }

      private void concede()
      {
          /*int totalwin = 0;
          int totallose = 0;
          string[] lines = new string[0] { };
          try
          {
              string path = (HRSettings.Get.CustomRuleFilePath).Remove(HRSettings.Get.CustomRuleFilePath.Length - 13) + "Common" + System.IO.Path.DirectorySeparatorChar;
              lines = System.IO.File.ReadAllLines(path + "Settings.ini");
          }
          catch
          {
              Helpfunctions.Instance.logg("cant find Settings.ini");
          }
          foreach (string s in lines)
          {
              if (s.Contains("bot.stats.victory"))
              {
                  int val1 = s.Length;
                  string temp1 = s.Substring(18, (val1 - 18));
                  Helpfunctions.Instance.ErrorLog(temp1);
                  totalwin = int.Parse(temp1);
              }
              else if (s.Contains("bot.stats.defeat"))
              {
                  int val2 = s.Length;
                  string temp2 = s.Substring(17, (val2 - 17));
                  Helpfunctions.Instance.ErrorLog(temp2);
                  totallose = int.Parse(temp2);
              }
          }
          if (totalwin > totallose)
          {
              Helpfunctions.Instance.ErrorLog("not today!");
              HRGame.ConcedeGame();
          }*/
          int curlvl = HRPlayer.GetLocalPlayer().GetRank();
          if (HREngine.API.Utilities.HRSettings.Get.SelectedGameMode != HRGameMode.RANKED_PLAY) return;
          if (curlvl < this.concedeLvl)
          {
              Helpfunctions.Instance.ErrorLog("not today!");
              HRGame.ConcedeGame();
          }
      }


      private HREngine.API.Actions.ActionBase HandleBattleMulliganPhase()
      {
          Helpfunctions.Instance.ErrorLog("handle mulligan");

          if ((TAG_MULLIGAN)HRPlayer.GetLocalPlayer().GetTag(HRGameTag.MULLIGAN_STATE) != TAG_MULLIGAN.INPUT)
          {
              Helpfunctions.Instance.ErrorLog("but we have to wait :D");
              return null;
          }

          if (HRMulligan.IsMulliganActive())
          {
              var list = HRCard.GetCards(HRPlayer.GetLocalPlayer(), HRCardZone.HAND);
              if (Mulligan.Instance.hasmulliganrules())
              {
                  HRPlayer enemyPlayer = HRPlayer.GetEnemyPlayer();
                  string enemName = Hrtprozis.Instance.heroIDtoName(enemyPlayer.GetHeroCard().GetEntity().GetCardId());
                  List<Mulligan.CardIDEntity> celist = new List<Mulligan.CardIDEntity>();
                  foreach (var item in list)
                  {
                      if (item.GetEntity().GetCardId() != "GAME_005")// dont mulligan coin
                      {
                          celist.Add(new Mulligan.CardIDEntity(item.GetEntity().GetCardId(), item.GetEntity().GetEntityId()));
                      }
                  }
                  List<int> mullientitys = Mulligan.Instance.whatShouldIMulligan(celist, enemName);
                  foreach (var item in list)
                  {
                      if (mullientitys.Contains(item.GetEntity().GetEntityId()))
                      {
                          Helpfunctions.Instance.ErrorLog("Rejecting Mulligan Card " + item.GetEntity().GetName() + " because of your rules");
                          HRMulligan.ToggleCard(item);
                      }
                  }


              }
              else
              {
                  foreach (var item in list)
                  {
                      if (item.GetEntity().GetCost() >= 4)
                      {
                          Helpfunctions.Instance.ErrorLog("Rejecting Mulligan Card " + item.GetEntity().GetName() + " because it cost is >= 4.");
                          HRMulligan.ToggleCard(item);
                      }
                      if (item.GetEntity().GetCardId() == "EX1_308" || item.GetEntity().GetCardId() == "EX1_622" || item.GetEntity().GetCardId() == "EX1_005")
                      {
                          Helpfunctions.Instance.ErrorLog("Rejecting Mulligan Card " + item.GetEntity().GetName() + " because it is soulfire or shadow word: death");
                          HRMulligan.ToggleCard(item);
                      }
                  }
              }


              sf.setnewLoggFile();

              if (Mulligan.Instance.loserLoserLoser)
              {
                  concede();
              }
              return null;
              //HRMulligan.EndMulligan();
          }
          return null;
      }

      /// <summary>
      /// [EN]
      /// This handler is executed when the local player turn is active.
      ///
      /// [DE]
      /// Dieses Event wird ausgelöst wenn der Spieler am Zug ist.
      /// </summary>
      private HREngine.API.Actions.ActionBase HandleOnBattleStateUpdate()
      {

          try
          {
              if (HRBattle.IsInTargetMode() && dirtytarget >= 0)
              {
                  Helpfunctions.Instance.ErrorLog("dirty targeting...");
                  HREntity target = getEntityWithNumber(dirtytarget);

                  dirtytarget = -1;

                  return new HREngine.API.Actions.TargetAction(target);
              }
              if (HRChoice.IsChoiceActive())
              {
                  if (this.dirtychoice >= 1)
                  {
                      List<HREntity> choices = HRChoice.GetChoiceCards();
                      int choice = this.dirtychoice;
                      this.dirtychoice = -1;
                      string ccId = this.choiceCardId;
                      this.choiceCardId = "";
                      HREntity target = choices[choice - 1];
                      if (ccId == "EX1_160")
                      {
                          foreach (HREntity hre in choices)
                          {
                              if (choice == 1 && hre.GetCardId() == "EX1_160b") target = hre;
                              if (choice == 2 && hre.GetCardId() == "EX1_160a") target = hre;
                          }
                      }
                      if (ccId == "NEW1_008")
                      {
                          foreach (HREntity hre in choices)
                          {
                              if (choice == 1 && hre.GetCardId() == "NEW1_008a") target = hre;
                              if (choice == 2 && hre.GetCardId() == "NEW1_008b") target = hre;
                          }
                      }
                      if (ccId == "EX1_178")
                      {
                          foreach (HREntity hre in choices)
                          {
                              if (choice == 1 && hre.GetCardId() == "EX1_178a") target = hre;
                              if (choice == 2 && hre.GetCardId() == "EX1_178b") target = hre;
                          }
                      }
                      if (ccId == "EX1_573")
                      {
                          foreach (HREntity hre in choices)
                          {
                              if (choice == 1 && hre.GetCardId() == "EX1_573a") target = hre;
                              if (choice == 2 && hre.GetCardId() == "EX1_573b") target = hre;
                          }
                      }
                      if (ccId == "EX1_165")//druid of the claw
                      {
                          foreach (HREntity hre in choices)
                          {
                              if (choice == 1 && hre.GetCardId() == "EX1_165t1") target = hre;
                              if (choice == 2 && hre.GetCardId() == "EX1_165t2") target = hre;
                          }
                      }
                      if (ccId == "EX1_166")//keeper of the grove
                      {
                          foreach (HREntity hre in choices)
                          {
                              if (choice == 1 && hre.GetCardId() == "EX1_166a") target = hre;
                              if (choice == 2 && hre.GetCardId() == "EX1_166b") target = hre;
                          }
                      }
                      if (ccId == "EX1_155")
                      {
                          foreach (HREntity hre in choices)
                          {
                              if (choice == 1 && hre.GetCardId() == "EX1_155a") target = hre;
                              if (choice == 2 && hre.GetCardId() == "EX1_155b") target = hre;
                          }
                      }
                      if (ccId == "EX1_164")
                      {
                          foreach (HREntity hre in choices)
                          {
                              if (choice == 1 && hre.GetCardId() == "EX1_164a") target = hre;
                              if (choice == 2 && hre.GetCardId() == "EX1_164b") target = hre;
                          }
                      }
                      if (ccId == "New1_007")//starfall
                      {
                          foreach (HREntity hre in choices)
                          {
                              if (choice == 1 && hre.GetCardId() == "New1_007b") target = hre;
                              if (choice == 2 && hre.GetCardId() == "New1_007a") target = hre;
                          }
                      }
                      if (ccId == "EX1_154")//warth
                      {
                          foreach (HREntity hre in choices)
                          {
                              if (choice == 1 && hre.GetCardId() == "EX1_154a") target = hre;
                              if (choice == 2 && hre.GetCardId() == "EX1_154b") target = hre;
                          }
                      }
                      Helpfunctions.Instance.logg("chooses the card: " + target.GetCardId());
                      return new HREngine.API.Actions.ChoiceAction(target);
                  }
                  else
                  {
                      //Todo: ultimate tracking-simulation!
                      List<HREntity> choices = HRChoice.GetChoiceCards();
                      Random r = new Random();
                      int choice = r.Next(0, choices.Count);
                      Helpfunctions.Instance.logg("chooses a random card");
                      return new HREngine.API.Actions.ChoiceAction(choices[choice]);
                  }
              }


              //SafeHandleBattleLocalPlayerTurnHandler();


              sf.updateEverything(this);
              Action moveTodo = Ai.Instance.bestmove;
              if (moveTodo == null)
              {
                  Helpfunctions.Instance.ErrorLog("end turn");
                  return null;
              }
              Helpfunctions.Instance.ErrorLog("play action");
              moveTodo.print();
              if (moveTodo.cardplay)
              {
                  HRCard cardtoplay = getCardWithNumber(moveTodo.cardEntitiy);
                  if (moveTodo.enemytarget >= 0)
                  {
                      HREntity target = getEntityWithNumber(moveTodo.enemyEntitiy);
                      Helpfunctions.Instance.ErrorLog("play: " + cardtoplay.GetEntity().GetName() + " target: " + target.GetName());
                      Helpfunctions.Instance.logg("play: " + cardtoplay.GetEntity().GetName() + " target: " + target.GetName() + " choice: " + moveTodo.druidchoice);
                      if (moveTodo.druidchoice >= 1)
                      {
                          if (moveTodo.enemyEntitiy >= 0) this.dirtytarget = moveTodo.enemyEntitiy;
                          this.dirtychoice = moveTodo.druidchoice; //1=leftcard, 2= rightcard
                          this.choiceCardId = moveTodo.handcard.card.CardID;

                      }
                      if (moveTodo.handcard.card.type == CardDB.cardtype.MOB)
                      {
                          return new HREngine.API.Actions.PlayCardAction(cardtoplay, target, moveTodo.owntarget + 1);
                      }

                      return new HREngine.API.Actions.PlayCardAction(cardtoplay, target);

                  }
                  else
                  {
                      Helpfunctions.Instance.ErrorLog("play: " + cardtoplay.GetEntity().GetName() + " target nothing");
                      Helpfunctions.Instance.logg("play: " + cardtoplay.GetEntity().GetName() + " choice: " + moveTodo.druidchoice);
                      if (moveTodo.druidchoice >= 1)
                      {
                          this.dirtychoice = moveTodo.druidchoice; //1=leftcard, 2= rightcard
                          this.choiceCardId = moveTodo.handcard.card.CardID;

                      }
                      if (moveTodo.handcard.card.type == CardDB.cardtype.MOB)
                      {
                          return new HREngine.API.Actions.PlayCardAction(cardtoplay, null, moveTodo.owntarget + 1);
                      }
                      return new HREngine.API.Actions.PlayCardAction(cardtoplay);
                  }

              }

              if (moveTodo.minionplay)
              {
                  HREntity attacker = getEntityWithNumber(moveTodo.ownEntitiy);
                  HREntity target = getEntityWithNumber(moveTodo.enemyEntitiy);
                  Helpfunctions.Instance.ErrorLog("minion attack: " + attacker.GetName() + " target: " + target.GetName());
                  Helpfunctions.Instance.logg("minion attack: " + attacker.GetName() + " target: " + target.GetName());
                  return new HREngine.API.Actions.AttackAction(attacker, target);

              }

              if (moveTodo.heroattack)
              {
                  HREntity attacker = getEntityWithNumber(moveTodo.ownEntitiy);
                  HREntity target = getEntityWithNumber(moveTodo.enemyEntitiy);
                  this.dirtytarget = moveTodo.enemyEntitiy;
                  //Helpfunctions.Instance.ErrorLog("heroattack: attkr:" + moveTodo.ownEntitiy + " defender: " + moveTodo.enemyEntitiy);
                  Helpfunctions.Instance.ErrorLog("heroattack: " + attacker.GetName() + " target: " + target.GetName());
                  Helpfunctions.Instance.logg("heroattack: " + attacker.GetName() + " target: " + target.GetName());
                  if (HRPlayer.GetLocalPlayer().HasWeapon())
                  {
                      Helpfunctions.Instance.ErrorLog("hero attack with weapon");
                      return new HREngine.API.Actions.AttackAction(HRPlayer.GetLocalPlayer().GetWeaponCard().GetEntity(), target);
                  }
                  Helpfunctions.Instance.ErrorLog("hero attack without weapon");
                  //Helpfunctions.Instance.ErrorLog("attacker entity: " + HRPlayer.GetLocalPlayer().GetHero().GetEntityId());
                  return new HREngine.API.Actions.AttackAction(HRPlayer.GetLocalPlayer().GetHero(), target);

              }

              if (moveTodo.useability)
              {
                  HRCard cardtoplay = HRPlayer.GetLocalPlayer().GetHeroPower().GetCard();

                  if (moveTodo.enemytarget >= 0)
                  {
                      HREntity target = getEntityWithNumber(moveTodo.enemyEntitiy);
                      Helpfunctions.Instance.ErrorLog("use ablitiy: " + cardtoplay.GetEntity().GetName() + " target " + target.GetName());
                      Helpfunctions.Instance.logg("use ablitiy: " + cardtoplay.GetEntity().GetName() + " target " + target.GetName());
                      return new HREngine.API.Actions.PlayCardAction(cardtoplay, target);

                  }
                  else
                  {
                      Helpfunctions.Instance.ErrorLog("use ablitiy: " + cardtoplay.GetEntity().GetName() + " target nothing");
                      Helpfunctions.Instance.logg("use ablitiy: " + cardtoplay.GetEntity().GetName() + " target nothing");
                      return new HREngine.API.Actions.PlayCardAction(cardtoplay);
                  }
              }

          }
          catch (Exception Exception)
          {
              Helpfunctions.Instance.ErrorLog(Exception.Message);
              Helpfunctions.Instance.ErrorLog(Environment.StackTrace);
          }
          return null;
          //HRBattle.FinishRound();
      }

      private HREntity getEntityWithNumber(int number)
      {
          foreach (HREntity e in this.getallEntitys())
          {
              if (number == e.GetEntityId()) return e;
          }
          return null;
      }

      private HRCard getCardWithNumber(int number)
      {
          foreach (HRCard e in this.getallHandCards())
          {
              if (number == e.GetEntity().GetEntityId()) return e;
          }
          return null;
      }

      private List<HREntity> getallEntitys()
      {
          List<HREntity> result = new List<HREntity>();
          HREntity ownhero = HRPlayer.GetLocalPlayer().GetHero();
          HREntity enemyhero = HRPlayer.GetEnemyPlayer().GetHero();
          HREntity ownHeroAbility = HRPlayer.GetLocalPlayer().GetHeroPower();
          List<HRCard> list2 = HRCard.GetCards(HRPlayer.GetLocalPlayer(), HRCardZone.PLAY);
          List<HRCard> list3 = HRCard.GetCards(HRPlayer.GetEnemyPlayer(), HRCardZone.PLAY);

          result.Add(ownhero);
          result.Add(enemyhero);
          result.Add(ownHeroAbility);

          foreach (HRCard item in list2)
          {
              result.Add(item.GetEntity());
          }
          foreach (HRCard item in list3)
          {
              result.Add(item.GetEntity());
          }
          

          

          return result;
      }

      private List<HRCard> getallHandCards()
      {
          List<HRCard> list = HRCard.GetCards(HRPlayer.GetLocalPlayer(), HRCardZone.HAND);
          return list;
      }

      protected virtual void SafeHandleBattleLocalPlayerTurnHandler()
      {
          
      }

      protected virtual HRCard GetMinionByPriority(HRCard lastMinion = null)
      {
         return null;
      }

      
   }

    public class Silverfish
    {
        private int versionnumber = 64;
        private bool singleLog = false;


        Settings sttngs = Settings.Instance;

        List<Minion> ownMinions = new List<Minion>();
        List<Minion> enemyMinions = new List<Minion>();
        List<Handmanager.Handcard> handCards = new List<Handmanager.Handcard>();
        int ownPlayerController = 0;
        List<string> ownSecretList = new List<string>();
        int enemySecretCount = 0;

        int currentMana = 0;
        int ownMaxMana = 0;
        int numMinionsPlayedThisTurn = 0;
        int cardsPlayedThisTurn = 0;
        int ueberladung = 0;

        int enemyMaxMana = 0;


        string ownHeroWeapon = "";
        int heroWeaponAttack = 0;
        int heroWeaponDurability = 0;
        bool heroImmuneToDamageWhileAttacking = false;
        bool heroImmune = false;
        bool enemyHeroImmune = false;

        string enemyHeroWeapon = "";
        int enemyWeaponAttack = 0;
        int enemyWeaponDurability = 0;

        int heroAtk = 0;
        int heroHp = 30;
        int heroDefence = 0;
        string heroname = "";
        bool ownheroisread = false;
        int heroNumAttacksThisTurn = 0;
        bool heroHasWindfury = false;
        bool herofrozen = false;

        int enemyAtk = 0;
        int enemyHp = 30;
        string enemyHeroname = "";
        int enemyDefence = 0;
        bool enemyfrozen = false;

        CardDB.Card heroAbility = new CardDB.Card();
        bool ownAbilityisReady = false;
        CardDB.Card enemyAbility = new CardDB.Card();

        int anzcards = 0;
        int enemyAnzCards = 0;

        int ownHeroFatigue = 0;
        int enemyHeroFatigue = 0;
        int ownDecksize = 0;
        int enemyDecksize = 0;

        public Silverfish(bool snglLg)
        {
            this.singleLog = snglLg;
            Helpfunctions.Instance.ErrorLog("init Silverfish");
            string path = (HRSettings.Get.CustomRuleFilePath).Remove(HRSettings.Get.CustomRuleFilePath.Length - 13) + "UltimateLogs" + System.IO.Path.DirectorySeparatorChar;
            System.IO.Directory.CreateDirectory(path);
            sttngs.setFilePath((HRSettings.Get.CustomRuleFilePath).Remove(HRSettings.Get.CustomRuleFilePath.Length - 13) + "Bots" + System.IO.Path.DirectorySeparatorChar + "silver" + System.IO.Path.DirectorySeparatorChar);

            if (!singleLog)
            {
                sttngs.setLoggPath(path);
            }
            else
            {
                sttngs.setLoggPath((HRSettings.Get.CustomRuleFilePath).Remove(HRSettings.Get.CustomRuleFilePath.Length - 13));
                sttngs.setLoggFile("UILogg.txt");
                Helpfunctions.Instance.createNewLoggfile();
            }
            PenalityManager.Instance.setCombos();
            Mulligan m = Mulligan.Instance; // read the mulligan list
        }

        public void setnewLoggFile()
        {
            if (!singleLog)
            {
                sttngs.setLoggFile("UILogg" + DateTime.Now.ToString("_yyyy-MM-dd_HH-mm-ss") + ".txt");
                Helpfunctions.Instance.createNewLoggfile();
            }
            else
            {
                sttngs.setLoggFile("UILogg.txt");
            }
        }

        public void updateEverything(Bot botbase)
        {

            HRPlayer ownPlayer = HRPlayer.GetLocalPlayer();
            HRPlayer enemyPlayer = HRPlayer.GetEnemyPlayer();
            ownPlayerController = ownPlayer.GetHero().GetControllerId();//ownPlayer.GetHero().GetControllerId()


            // create hero + minion data
            getHerostuff();
            getMinions();
            getHandcards();

            // send ai the data:
            Hrtprozis.Instance.clearAll();
            Handmanager.Instance.clearAll();

            Hrtprozis.Instance.setOwnPlayer(ownPlayerController);
            Handmanager.Instance.setOwnPlayer(ownPlayerController);

            Hrtprozis.Instance.updatePlayer(this.ownMaxMana, this.currentMana, this.cardsPlayedThisTurn, this.numMinionsPlayedThisTurn, this.ueberladung, ownPlayer.GetHero().GetEntityId(), enemyPlayer.GetHero().GetEntityId());
            Hrtprozis.Instance.updateSecretStuff(this.ownSecretList, this.enemySecretCount);

            Hrtprozis.Instance.updateOwnHero(this.ownHeroWeapon, this.heroWeaponAttack, this.heroWeaponDurability, this.heroImmuneToDamageWhileAttacking, this.heroAtk, this.heroHp, this.heroDefence, this.heroname, this.ownheroisread, this.herofrozen, this.heroAbility, this.ownAbilityisReady, this.heroNumAttacksThisTurn, this.heroHasWindfury, this.heroImmune);
            Hrtprozis.Instance.updateEnemyHero(this.enemyHeroWeapon, this.enemyWeaponAttack, this.enemyWeaponDurability, this.enemyAtk, this.enemyHp, this.enemyDefence, this.enemyHeroname, this.enemyfrozen, this.enemyAbility, this.enemyHeroImmune, this.enemyMaxMana);

            Hrtprozis.Instance.updateMinions(this.ownMinions, this.enemyMinions);
            Handmanager.Instance.setHandcards(this.handCards, this.anzcards, this.enemyAnzCards);

            Hrtprozis.Instance.updateFatigueStats(this.ownDecksize, this.ownHeroFatigue, this.enemyDecksize, this.enemyHeroFatigue);

            // print data
            Hrtprozis.Instance.printHero();
            Hrtprozis.Instance.printOwnMinions();
            Hrtprozis.Instance.printEnemyMinions();
            Handmanager.Instance.printcards();

            // calculate stuff
            Helpfunctions.Instance.ErrorLog("calculating stuff... " + DateTime.Now.ToString("HH:mm:ss.ffff"));
            Ai.Instance.dosomethingclever(botbase);
            Helpfunctions.Instance.ErrorLog("calculating ended! " + DateTime.Now.ToString("HH:mm:ss.ffff"));

        }

        private void getHerostuff()
        {


            HRPlayer ownPlayer = HRPlayer.GetLocalPlayer();
            HRPlayer enemyPlayer = HRPlayer.GetEnemyPlayer();

            HREntity ownhero = ownPlayer.GetHero();
            HREntity enemyhero = enemyPlayer.GetHero();
            HREntity ownHeroAbility = ownPlayer.GetHeroPower();

            //player stuff#########################
            //this.currentMana =ownPlayer.GetTag(HRGameTag.RESOURCES) - ownPlayer.GetTag(HRGameTag.RESOURCES_USED) + ownPlayer.GetTag(HRGameTag.TEMP_RESOURCES);
            this.currentMana = ownPlayer.GetNumAvailableResources();
            this.ownMaxMana = ownPlayer.GetTag(HRGameTag.RESOURCES);
            this.enemyMaxMana = enemyPlayer.GetTag(HRGameTag.RESOURCES);
            Helpfunctions.Instance.logg("#######################################################################");
            Helpfunctions.Instance.logg("#######################################################################");
            Helpfunctions.Instance.logg("start calculations, current time: " + DateTime.Now.ToString("HH:mm:ss") + " V" + this.versionnumber);
            Helpfunctions.Instance.logg("#######################################################################");
            Helpfunctions.Instance.logg("mana " + currentMana + "/" + ownMaxMana);
            Helpfunctions.Instance.logg("emana " + enemyMaxMana);
            Helpfunctions.Instance.logg("own secretsCount: " + ownPlayer.GetSecretDefinitions().Count);
            enemySecretCount = HRCard.GetCards(enemyPlayer, HRCardZone.SECRET).Count;
            enemySecretCount = 0;
            Helpfunctions.Instance.logg("enemy secretsCount: " + enemySecretCount);
            this.ownSecretList = ownPlayer.GetSecretDefinitions();
            this.numMinionsPlayedThisTurn = ownPlayer.GetTag(HRGameTag.NUM_MINIONS_PLAYED_THIS_TURN);
            this.cardsPlayedThisTurn = ownPlayer.GetTag(HRGameTag.NUM_CARDS_PLAYED_THIS_TURN);
            //if (ownPlayer.HasCombo()) this.cardsPlayedThisTurn = 1;
            this.ueberladung = ownPlayer.GetTag(HRGameTag.RECALL_OWED);

            //get weapon stuff
            this.ownHeroWeapon = "";
            this.heroWeaponAttack = 0;
            this.heroWeaponDurability = 0;

            this.ownHeroFatigue = ownhero.GetFatigue();
            this.enemyHeroFatigue = enemyhero.GetFatigue();
            //this.ownDecksize = HRCard.GetCards(ownPlayer, HRCardZone.DECK).Count;
            //this.enemyDecksize = HRCard.GetCards(enemyPlayer, HRCardZone.DECK).Count;

            this.heroImmune = ownhero.IsImmune();
            this.enemyHeroImmune = enemyhero.IsImmune();

            this.enemyHeroWeapon = "";
            this.enemyWeaponAttack = 0;
            this.enemyWeaponDurability = 0;
            if (enemyPlayer.HasWeapon())
            {
                HREntity weapon = enemyPlayer.GetWeaponCard().GetEntity();
                this.enemyHeroWeapon = CardDB.Instance.getCardDataFromID(weapon.GetCardId()).name;
                this.enemyWeaponAttack = weapon.GetATK();
                this.enemyWeaponDurability = weapon.GetDurability();

            }


            //own hero stuff###########################
            this.heroAtk = ownhero.GetATK();
            this.heroHp = ownhero.GetHealth() - ownhero.GetDamage();
            this.heroDefence = ownhero.GetArmor();
            this.heroname = Hrtprozis.Instance.heroIDtoName(ownhero.GetCardId());
            bool exausted = false;
            exausted = ownhero.IsExhausted();
            this.ownheroisread = true;

            this.heroImmuneToDamageWhileAttacking = (ownhero.IsImmune()) ? true : false;
            this.herofrozen = ownhero.IsFrozen();
            this.heroNumAttacksThisTurn = ownhero.GetNumAttacksThisTurn();
            this.heroHasWindfury = ownhero.HasWindfury();
            //int numberofattacks = ownhero.GetNumAttacksThisTurn();

            //Helpfunctions.Instance.ErrorLog(ownhero.GetName() + " ready params ex: " + exausted + " " + heroAtk + " " + numberofattacks + " " + herofrozen);

            if (exausted == true)
            {
                this.ownheroisread = false;
            }
            if (exausted == false && this.heroAtk == 0)
            {
                this.ownheroisread = false;
            }
            if (herofrozen) ownheroisread = false;


            if (ownPlayer.HasWeapon())
            {
                HREntity weapon = ownPlayer.GetWeaponCard().GetEntity();
                this.ownHeroWeapon = CardDB.Instance.getCardDataFromID(weapon.GetCardId()).name;
                this.heroWeaponAttack = weapon.GetATK();
                this.heroWeaponDurability = weapon.GetTag(HRGameTag.DURABILITY) - weapon.GetTag(HRGameTag.DAMAGE);//weapon.GetDurability();
                this.heroImmuneToDamageWhileAttacking = false;
                if (this.ownHeroWeapon == "gladiatorslongbow")
                {
                    this.heroImmuneToDamageWhileAttacking = true;
                }

                //Helpfunctions.Instance.ErrorLog("weapon: " + ownHeroWeapon + " " + heroWeaponAttack + " " + heroWeaponDurability);

            }

            //enemy hero stuff###############################################################
            this.enemyAtk = enemyhero.GetATK();

            this.enemyHp = enemyhero.GetHealth() - enemyhero.GetDamage();

            this.enemyHeroname = Hrtprozis.Instance.heroIDtoName(enemyhero.GetCardId());

            this.enemyDefence = enemyhero.GetArmor();

            this.enemyfrozen = enemyhero.IsFrozen();






            //own hero ablity stuff###########################################################

            this.heroAbility = CardDB.Instance.getCardDataFromID(ownHeroAbility.GetCardId());
            this.ownAbilityisReady = (ownHeroAbility.IsExhausted()) ? false : true; // if exhausted, ability is NOT ready

            this.enemyAbility = CardDB.Instance.getCardDataFromID(enemyhero.GetHeroPower().GetCardId());



        }

        private void getMinions()
        {
            ownMinions.Clear();
            enemyMinions.Clear();
            HRPlayer ownPlayer = HRPlayer.GetLocalPlayer();
            HRPlayer enemyPlayer = HRPlayer.GetEnemyPlayer();

            // ALL minions on Playfield:
            List<HRCard> list = HRCard.GetCards(ownPlayer, HRCardZone.PLAY);
            list.AddRange(HRCard.GetCards(enemyPlayer, HRCardZone.PLAY));

            List<HREntity> enchantments = new List<HREntity>();


            foreach (HRCard item in list)
            {
                HREntity entitiy = item.GetEntity();
                int zp = entitiy.GetZonePosition();

                if (entitiy.GetCardType() == HRCardType.MINION && zp >= 1)
                {
                    //Helpfunctions.Instance.ErrorLog("zonepos " + zp);
                    CardDB.Card c = CardDB.Instance.getCardDataFromID(entitiy.GetCardId());
                    Minion m = new Minion();
                    m.name = c.name;
                    m.handcard.card = c;
                    m.Angr = entitiy.GetATK();
                    m.maxHp = entitiy.GetHealth();
                    m.Hp = m.maxHp - entitiy.GetDamage();
                    m.wounded = false;
                    if (m.maxHp > m.Hp) m.wounded = true;


                    m.exhausted = entitiy.IsExhausted();

                    m.taunt = (entitiy.HasTaunt()) ? true : false;

                    m.charge = (entitiy.HasCharge()) ? true : false;

                    m.numAttacksThisTurn = entitiy.GetNumAttacksThisTurn();

                    int temp = entitiy.GetNumTurnsInPlay();
                    m.playedThisTurn = (temp == 0) ? true : false;

                    m.windfury = (entitiy.HasWindfury()) ? true : false;

                    m.frozen = (entitiy.IsFrozen()) ? true : false;

                    m.divineshild = (entitiy.HasDivineShield()) ? true : false;

                    m.stealth = (entitiy.IsStealthed()) ? true : false;

                    m.poisonous = (entitiy.IsPoisonous()) ? true : false;

                    m.immune = (entitiy.IsImmune()) ? true : false;

                    m.silenced = (entitiy.GetTag(HRGameTag.SILENCED) >= 1) ? true : false;


                    m.zonepos = zp;
                    m.id = m.zonepos - 1;

                    m.entitiyID = entitiy.GetEntityId();

                    m.enchantments.Clear();

                    //Helpfunctions.Instance.ErrorLog(  m.name + " ready params ex: " + m.exhausted + " charge: " +m.charge + " attcksthisturn: " + m.numAttacksThisTurn + " playedthisturn " + m.playedThisTurn );

                    m.Ready = false; // if exhausted, he is NOT ready

                    if (!m.playedThisTurn && !m.exhausted && !m.frozen && (m.numAttacksThisTurn == 0 || (m.numAttacksThisTurn == 1 && m.windfury)))
                    {
                        m.Ready = true;
                    }

                    if (m.playedThisTurn && m.charge && (m.numAttacksThisTurn == 0 || (m.numAttacksThisTurn == 1 && m.windfury)))
                    {
                        //m.exhausted = false;
                        m.Ready = true;
                    }

                    if (!m.silenced && (m.name == "ancientwatcher" || m.name == "ragnarosthefirelord"))
                    {
                        m.Ready = false;
                    }


                    if (entitiy.GetControllerId() == this.ownPlayerController) // OWN minion
                    {

                        this.ownMinions.Add(m);
                    }
                    else
                    {
                        this.enemyMinions.Add(m);
                    }

                }
                // minions added

                if (entitiy.GetCardType() == HRCardType.WEAPON)
                {
                    //Helpfunctions.Instance.ErrorLog("found weapon!");
                    if (entitiy.GetControllerId() == this.ownPlayerController) // OWN weapon
                    {
                        this.ownHeroWeapon = CardDB.Instance.getCardDataFromID(entitiy.GetCardId()).name;
                        this.heroWeaponAttack = entitiy.GetATK();
                        this.heroWeaponDurability = entitiy.GetDurability();
                        //this.heroImmuneToDamageWhileAttacking = false;


                    }
                    else
                    {
                        this.enemyHeroWeapon = CardDB.Instance.getCardDataFromID(entitiy.GetCardId()).name;
                        this.enemyWeaponAttack = entitiy.GetATK();
                        this.enemyWeaponDurability = entitiy.GetDurability();
                    }
                }

                if (entitiy.GetCardType() == HRCardType.ENCHANTMENT)
                {

                    enchantments.Add(entitiy);
                }


            }

            foreach (HRCard item in list)
            {
                foreach (HREntity e in item.GetEntity().GetEnchantments())
                {
                    enchantments.Add(e);
                }
            }


            // add enchantments to minions
            setEnchantments(enchantments);
        }

        private void setEnchantments(List<HREntity> enchantments)
        {
            foreach (HREntity bhu in enchantments)
            {
                //create enchantment
                Enchantment ench = CardDB.getEnchantmentFromCardID(bhu.GetCardId());
                ench.creator = bhu.GetCreatorId();
                ench.controllerOfCreator = bhu.GetControllerId();
                ench.cantBeDispelled = false;
                //if (bhu.c) ench.cantBeDispelled = true;

                foreach (Minion m in this.ownMinions)
                {
                    if (m.entitiyID == bhu.GetAttached())
                    {
                        m.enchantments.Add(ench);
                        //Helpfunctions.Instance.ErrorLog("add enchantment " +bhu.GetCardId()+" to: " + m.entitiyID);
                    }

                }

                foreach (Minion m in this.enemyMinions)
                {
                    if (m.entitiyID == bhu.GetAttached())
                    {
                        m.enchantments.Add(ench);
                    }

                }

            }

        }

        private void getHandcards()
        {
            handCards.Clear();
            this.anzcards = 0;
            this.enemyAnzCards = 0;
            List<HRCard> list = HRCard.GetCards(HRPlayer.GetLocalPlayer(), HRCardZone.HAND);
            list.AddRange(HRCard.GetCards(HRPlayer.GetEnemyPlayer(), HRCardZone.HAND));

            foreach (HRCard item in list)
            {

                HREntity entitiy = item.GetEntity();

                if (entitiy.GetControllerId() == this.ownPlayerController && entitiy.GetZonePosition() >= 1) // own handcard
                {
                    CardDB.Card c = CardDB.Instance.getCardDataFromID(entitiy.GetCardId());
                    //c.cost = entitiy.GetCost();
                    //c.entityID = entitiy.GetEntityId();

                    Handmanager.Handcard hc = new Handmanager.Handcard();
                    hc.card = c;
                    hc.position = entitiy.GetZonePosition();
                    hc.entity = entitiy.GetEntityId();
                    hc.manacost = entitiy.GetCost();
                    handCards.Add(hc);
                    this.anzcards++;
                }

                if (entitiy.GetControllerId() != this.ownPlayerController && entitiy.GetZonePosition() >= 1) // enemy handcard
                {
                    this.enemyAnzCards++;
                }
            }

        }



    }

    // the ai :D
//please ask/write me if you use this in your project

    public class Action
    {
        public bool cardplay = false;
        public bool heroattack = false;
        public bool useability = false;
        public bool minionplay = false;
        public Handmanager.Handcard handcard;
        public int cardEntitiy = -1;
        public int owntarget = -1; //= target where card/minion is placed
        public int ownEntitiy = -1;
        public int enemytarget = -1; // target where red arrow is placed
        public int enemyEntitiy = -1;
        public int druidchoice = 0; // 1 left card, 2 right card
        public int numEnemysBeforePlayed = 0;
        public bool comboBeforePlayed = false;

        public void print()
        {
            Helpfunctions help = Helpfunctions.Instance;
            help.logg("current Action: ");
            if (this.cardplay)
            {
                help.logg("play " + this.handcard.card.name);
                if (this.druidchoice >= 1) help.logg("choose choise " + this.druidchoice);
                help.logg("with entityid " + this.cardEntitiy);
                if (this.owntarget >= 0)
                {
                    help.logg("on position " + this.owntarget);
                }
                if (this.enemytarget >= 0)
                {
                    help.logg("and target to " + this.enemytarget + " " + this.enemyEntitiy);
                }
            }
            if (this.minionplay)
            {
                help.logg("attacker: " + this.owntarget + " enemy: " + this.enemytarget);
                help.logg("targetplace " + this.enemyEntitiy);
            }
            if (this.heroattack)
            {
                help.logg("attack with hero, enemy: " + this.enemytarget);
                help.logg("targetplace " + this.enemyEntitiy);
            }
            if (this.useability)
            {
                help.logg("useability ");
                if (this.enemytarget >= 0)
                {
                    help.logg("on enemy: " + this.enemytarget + "targetplace " + this.enemyEntitiy);
                }
            }
            help.logg("");
        }

    }

    public class Playfield
    {
        public bool logging = false;
        public bool sEnemTurn = false;

        public int attackFaceHP = 15;

        public int evaluatePenality = 0;
        public int ownController = 0;

        public int ownHeroEntity = -1;
        public int enemyHeroEntity = -1;

        public int value = Int32.MinValue;
        public int guessingHeroHP = 30;

        public int mana = 0;
        public int enemyHeroHp = 30;
        public HeroEnum ownHeroName = HeroEnum.druid;
        public HeroEnum enemyHeroName = HeroEnum.druid;
        public bool ownHeroReady = false;
        public bool enemyHeroReady = false;
        public int ownHeroNumAttackThisTurn = 0;
        public int enemyHeroNumAttackThisTurn = 0;
        public bool ownHeroWindfury = false;
        public bool enemyHeroWindfury = false;

        public List<string> ownSecretsIDList = new List<string>();
        public int enemySecretCount = 0;

        public int ownHeroHp = 30;
        public int ownheroAngr = 0;
        public int enemyheroAngr = 0;
        public bool ownHeroFrozen = false;
        public bool enemyHeroFrozen = false;
        public bool heroImmuneWhileAttacking = false;
        public bool enemyheroImmuneWhileAttacking = false;
        public bool heroImmune = false;
        public bool enemyHeroImmune = false;
        public int ownWeaponDurability = 0;
        public int ownWeaponAttack = 0;
        public string ownWeaponName = "";
        public string enemyWeaponName = "";

        public int enemyWeaponAttack = 0;
        public int enemyWeaponDurability = 0;
        public List<Minion> ownMinions = new List<Minion>();
        public List<Minion> enemyMinions = new List<Minion>();
        public List<Handmanager.Handcard> owncards = new List<Handmanager.Handcard>();
        public List<Action> playactions = new List<Action>();
        public bool complete = false;
        public int owncarddraw = 0;
        public int ownHeroDefence = 0;
        public int enemycarddraw = 0;
        public int enemyAnzCards = 0;
        public int enemyHeroDefence = 0;

        public int doublepriest = 0;
        public int enemydoublepriest = 0;
        public int spellpower = 0;

        public int enemyspellpower = 0;


        public bool auchenaiseelenpriesterin = false;
        public bool ownBaronRivendare = false;
        public bool enemyBaronRivendare = false;

        public bool playedmagierinderkirintor = false;
        public bool playedPreparation = false;

        public int winzigebeschwoererin = 0;
        public int startedWithWinzigebeschwoererin = 0;
        public int zauberlehrling = 0;
        public int startedWithZauberlehrling = 0;
        public int managespenst = 0;
        public int startedWithManagespenst = 0;
        public int soeldnerDerVenture = 0;
        public int startedWithsoeldnerDerVenture = 0;
        public int beschwoerungsportal = 0;
        public int startedWithbeschwoerungsportal = 0;

        public int ownWeaponAttackStarted = 0;
        public int ownMobsCountStarted = 0;
        public int ownCardsCountStarted = 0;
        public int ownHeroHpStarted = 30;
        public int enemyHeroHpStarted = 30;

        public int mobsplayedThisTurn = 0;
        public int startedWithMobsPlayedThisTurn = 0;

        public int cardsPlayedThisTurn = 0;
        public int ueberladung = 0; //=recall

        public int ownMaxMana = 0;
        public int enemyMaxMana = 0;

        public int lostDamage = 0;
        public int lostHeal = 0;
        public int lostWeaponDamage = 0;

        public int ownDeckSize = 30;
        public int enemyDeckSize = 30;
        public int ownHeroFatigue = 0;
        public int enemyHeroFatigue = 0;

        public bool ownAbilityReady = false;
        public CardDB.Card ownHeroAblility;
        public bool enemyAbilityReady = false;
        public CardDB.Card enemyHeroAblility;

        //Helpfunctions help = Helpfunctions.Instance;

        private void addMinionsReal(List<Minion> source, List<Minion> trgt)
        {
            foreach (Minion m in source)
            {
                trgt.Add(new Minion(m));
            }

        }

        private void addCardsReal(List<Handmanager.Handcard> source)
        {

            foreach (Handmanager.Handcard m in source)
            {
                this.owncards.Add(new Handmanager.Handcard(m));
            }

        }

        public Playfield()
        {
            //this.simulateEnemyTurn = Ai.Instance.simulateEnemyTurn;
            this.ownController = Hrtprozis.Instance.getOwnController();
            this.ownHeroEntity = Hrtprozis.Instance.ownHeroEntity;
            this.enemyHeroEntity = Hrtprozis.Instance.enemyHeroEntitiy;
            this.mana = Hrtprozis.Instance.currentMana;
            this.ownMaxMana = Hrtprozis.Instance.ownMaxMana;
            this.enemyMaxMana = Hrtprozis.Instance.enemyMaxMana;
            this.evaluatePenality = 0;
            this.ownSecretsIDList = Hrtprozis.Instance.ownSecretList;
            this.enemySecretCount = Hrtprozis.Instance.enemySecretCount;

            this.heroImmune = Hrtprozis.Instance.heroImmune;
            this.enemyHeroImmune = Hrtprozis.Instance.enemyHeroImmune;

            this.attackFaceHP = Hrtprozis.Instance.attackFaceHp;

            addMinionsReal(Hrtprozis.Instance.ownMinions, ownMinions);
            addMinionsReal(Hrtprozis.Instance.enemyMinions, enemyMinions);
            addCardsReal(Handmanager.Instance.handCards);
            this.enemyHeroHp = Hrtprozis.Instance.enemyHp;
            this.ownHeroName = Hrtprozis.Instance.heroname;
            this.enemyHeroName = Hrtprozis.Instance.enemyHeroname;
            this.ownHeroHp = Hrtprozis.Instance.heroHp;
            this.complete = false;
            this.ownHeroReady = Hrtprozis.Instance.ownheroisread;
            this.ownHeroWindfury = Hrtprozis.Instance.ownHeroWindfury;
            this.ownHeroNumAttackThisTurn = Hrtprozis.Instance.ownHeroNumAttacksThisTurn;

            this.ownHeroFrozen = Hrtprozis.Instance.herofrozen;
            this.enemyHeroFrozen = Hrtprozis.Instance.enemyfrozen;
            this.ownheroAngr = Hrtprozis.Instance.heroAtk;
            this.heroImmuneWhileAttacking = Hrtprozis.Instance.heroImmuneToDamageWhileAttacking;
            this.ownWeaponDurability = Hrtprozis.Instance.heroWeaponDurability;
            this.ownWeaponAttack = Hrtprozis.Instance.heroWeaponAttack;
            this.ownWeaponName = Hrtprozis.Instance.ownHeroWeapon;
            this.owncarddraw = 0;
            this.ownHeroDefence = Hrtprozis.Instance.heroDefence;
            this.enemyHeroDefence = Hrtprozis.Instance.enemyDefence;
            this.enemyWeaponAttack = Hrtprozis.Instance.enemyWeaponAttack;//dont know jet
            this.enemyWeaponName = Hrtprozis.Instance.enemyHeroWeapon;
            this.enemyWeaponDurability = Hrtprozis.Instance.enemyWeaponDurability;
            this.enemycarddraw = 0;
            this.enemyAnzCards = Handmanager.Instance.enemyAnzCards;
            this.ownAbilityReady = Hrtprozis.Instance.ownAbilityisReady;
            this.ownHeroAblility = Hrtprozis.Instance.heroAbility;
            this.enemyHeroAblility = Hrtprozis.Instance.enemyAbility;
            this.doublepriest = 0;
            this.spellpower = 0;
            this.mobsplayedThisTurn = Hrtprozis.Instance.numMinionsPlayedThisTurn;
            this.startedWithMobsPlayedThisTurn = Hrtprozis.Instance.numMinionsPlayedThisTurn;// only change mobsplayedthisturm
            this.cardsPlayedThisTurn = Hrtprozis.Instance.cardsPlayedThisTurn;
            this.ueberladung = Hrtprozis.Instance.ueberladung;

            this.ownHeroFatigue = Hrtprozis.Instance.ownHeroFatigue;
            this.enemyHeroFatigue = Hrtprozis.Instance.enemyHeroFatigue;
            this.ownDeckSize = Hrtprozis.Instance.ownDeckSize;
            this.enemyDeckSize = Hrtprozis.Instance.enemyDeckSize;

            //need the following for manacost-calculation
            this.ownHeroHpStarted = this.ownHeroHp;
            this.enemyHeroHpStarted = this.enemyHeroHp;
            this.ownWeaponAttackStarted = this.ownWeaponAttack;
            this.ownCardsCountStarted = this.owncards.Count;
            this.ownMobsCountStarted = this.ownMinions.Count + this.enemyMinions.Count;


            this.playedmagierinderkirintor = false;
            this.playedPreparation = false;

            this.zauberlehrling = 0;
            this.winzigebeschwoererin = 0;
            this.managespenst = 0;
            this.soeldnerDerVenture = 0;
            this.beschwoerungsportal = 0;

            this.startedWithbeschwoerungsportal = 0;
            this.startedWithManagespenst = 0;
            this.startedWithWinzigebeschwoererin = 0;
            this.startedWithZauberlehrling = 0;
            this.startedWithsoeldnerDerVenture = 0;

            this.ownBaronRivendare = false;
            this.enemyBaronRivendare = false;

            foreach (Minion m in this.ownMinions)
            {
                if (m.silenced) continue;

                if (m.name == "prophetvelen") this.doublepriest++;
                spellpower = spellpower + m.handcard.card.spellpowervalue;
                if (m.name == "auchenaisoulpriest") this.auchenaiseelenpriesterin = true;

                if (m.name == "pint-sizedsummoner")
                {
                    this.winzigebeschwoererin++;
                    this.startedWithWinzigebeschwoererin++;
                }
                if (m.name == "sorcerersapprentice")
                {
                    this.zauberlehrling++;
                    this.startedWithZauberlehrling++;
                }
                if (m.name == "manawraith")
                {
                    this.managespenst++;
                    this.startedWithManagespenst++;
                }
                if (m.name == "venturecomercenary")
                {
                    this.soeldnerDerVenture++;
                    this.startedWithsoeldnerDerVenture++;
                }
                if (m.name == "summoningportal")
                {
                    this.beschwoerungsportal++;
                    this.startedWithbeschwoerungsportal++;
                }

                if (m.handcard.card.specialMin == CardDB.specialMinions.baronrivendare)
                {
                    this.ownBaronRivendare = true;
                }

                foreach (Enchantment e in m.enchantments)// only at first init needed, after that its copied
                {
                    if (e.CARDID == "NEW1_036e" || e.CARDID == "NEW1_036e2") m.cantLowerHPbelowONE = true;
                }
            }

            foreach (Minion m in this.enemyMinions)
            {
                if (m.silenced) continue;
                this.enemyspellpower = this.enemyspellpower + m.handcard.card.spellpowervalue;
                if (m.name == "prophetvelen") this.enemydoublepriest++;
                if (m.name == "manawraith")
                {
                    this.managespenst++;
                    this.startedWithManagespenst++;
                }
                if (m.handcard.card.specialMin == CardDB.specialMinions.baronrivendare)
                {
                    this.enemyBaronRivendare = true;
                }
            }


        }

        public Playfield(Playfield p)
        {
            this.sEnemTurn = p.sEnemTurn;
            this.ownController = p.ownController;
            this.ownHeroEntity = p.ownHeroEntity;
            this.enemyHeroEntity = p.enemyHeroEntity;

            this.evaluatePenality = p.evaluatePenality;

            foreach (string s in p.ownSecretsIDList)
            { this.ownSecretsIDList.Add(s); }
            this.enemySecretCount = p.enemySecretCount;
            this.mana = p.mana;
            this.ownMaxMana = p.ownMaxMana;
            this.enemyMaxMana = p.enemyMaxMana;
            addMinionsReal(p.ownMinions, ownMinions);
            addMinionsReal(p.enemyMinions, enemyMinions);
            addCardsReal(p.owncards);
            this.enemyHeroHp = p.enemyHeroHp;
            this.ownHeroName = p.ownHeroName;
            this.enemyHeroName = p.enemyHeroName;
            this.ownHeroHp = p.ownHeroHp;
            this.playactions.AddRange(p.playactions);
            this.complete = false;
            this.ownHeroReady = p.ownHeroReady;
            this.enemyHeroReady = p.enemyHeroReady;
            this.ownHeroNumAttackThisTurn = p.ownHeroNumAttackThisTurn;
            this.enemyHeroNumAttackThisTurn = p.enemyHeroNumAttackThisTurn;
            this.ownHeroWindfury = p.ownHeroWindfury;

            this.attackFaceHP = p.attackFaceHP;

            this.heroImmune = p.heroImmune;
            this.enemyHeroImmune = p.enemyHeroImmune;

            this.ownheroAngr = p.ownheroAngr;
            this.enemyheroAngr = p.enemyheroAngr;
            this.ownHeroFrozen = p.ownHeroFrozen;
            this.enemyHeroFrozen = p.enemyHeroFrozen;
            this.heroImmuneWhileAttacking = p.heroImmuneWhileAttacking;
            this.enemyheroImmuneWhileAttacking = p.enemyheroImmuneWhileAttacking;
            this.owncarddraw = p.owncarddraw;
            this.ownHeroDefence = p.ownHeroDefence;
            this.enemyWeaponAttack = p.enemyWeaponAttack;
            this.enemyWeaponDurability = p.enemyWeaponDurability;
            this.enemyWeaponName = p.enemyWeaponName;
            this.enemycarddraw = p.enemycarddraw;
            this.enemyAnzCards = p.enemyAnzCards;
            this.enemyHeroDefence = p.enemyHeroDefence;
            this.ownWeaponDurability = p.ownWeaponDurability;
            this.ownWeaponAttack = p.ownWeaponAttack;
            this.ownWeaponName = p.ownWeaponName;

            this.lostDamage = p.lostDamage;
            this.lostWeaponDamage = p.lostWeaponDamage;
            this.lostHeal = p.lostHeal;

            this.ownAbilityReady = p.ownAbilityReady;
            this.enemyAbilityReady = p.enemyAbilityReady;
            this.ownHeroAblility = p.ownHeroAblility;
            this.enemyHeroAblility = p.enemyHeroAblility;
            this.doublepriest = 0;
            this.spellpower = 0;
            this.mobsplayedThisTurn = p.mobsplayedThisTurn;
            this.startedWithMobsPlayedThisTurn = p.startedWithMobsPlayedThisTurn;
            this.cardsPlayedThisTurn = p.cardsPlayedThisTurn;
            this.ueberladung = p.ueberladung;

            this.ownDeckSize = p.ownDeckSize;
            this.enemyDeckSize = p.enemyDeckSize;
            this.ownHeroFatigue = p.ownHeroFatigue;
            this.enemyHeroFatigue = p.enemyHeroFatigue;

            //need the following for manacost-calculation
            this.ownHeroHpStarted = p.ownHeroHpStarted;
            this.enemyHeroHp = p.enemyHeroHp;
            this.ownWeaponAttackStarted = p.ownWeaponAttackStarted;
            this.ownCardsCountStarted = p.ownCardsCountStarted;
            this.ownMobsCountStarted = p.ownMobsCountStarted;

            this.startedWithWinzigebeschwoererin = p.startedWithWinzigebeschwoererin;
            this.playedmagierinderkirintor = p.playedmagierinderkirintor;

            this.startedWithZauberlehrling = p.startedWithZauberlehrling;
            this.startedWithWinzigebeschwoererin = p.startedWithWinzigebeschwoererin;
            this.startedWithManagespenst = p.startedWithManagespenst;
            this.startedWithsoeldnerDerVenture = p.startedWithsoeldnerDerVenture;
            this.startedWithbeschwoerungsportal = p.startedWithbeschwoerungsportal;

            this.ownBaronRivendare = false;
            this.enemyBaronRivendare = false;

            this.zauberlehrling = 0;
            this.winzigebeschwoererin = 0;
            this.managespenst = 0;
            this.soeldnerDerVenture = 0;
            foreach (Minion m in this.ownMinions)
            {
                if (m.silenced) continue;

                if (m.handcard.card.specialMin == CardDB.specialMinions.prophetvelen) this.doublepriest++;
                spellpower = spellpower + m.handcard.card.spellpowervalue;
                if (m.handcard.card.specialMin == CardDB.specialMinions.auchenaisoulpriest) this.auchenaiseelenpriesterin = true;
                if (m.handcard.card.specialMin == CardDB.specialMinions.pintsizedsummoner) this.winzigebeschwoererin++;
                if (m.handcard.card.specialMin == CardDB.specialMinions.sorcerersapprentice) this.zauberlehrling++;
                if (m.handcard.card.specialMin == CardDB.specialMinions.manawraith) this.managespenst++;
                if (m.handcard.card.specialMin == CardDB.specialMinions.venturecomercenary) this.soeldnerDerVenture++;
                if (m.handcard.card.specialMin == CardDB.specialMinions.summoningportal) this.beschwoerungsportal++;
                if (m.handcard.card.specialMin == CardDB.specialMinions.baronrivendare)
                {
                    this.ownBaronRivendare = true;
                }


            }

            foreach (Minion m in this.enemyMinions)
            {
                if (m.silenced) continue;
                this.enemyspellpower = this.enemyspellpower + m.handcard.card.spellpowervalue;
                if (m.handcard.card.specialMin == CardDB.specialMinions.prophetvelen) this.enemydoublepriest++;
                if (m.handcard.card.specialMin == CardDB.specialMinions.manawraith) this.managespenst++;
                if (m.handcard.card.specialMin == CardDB.specialMinions.baronrivendare)
                {
                    this.enemyBaronRivendare = true;
                }
            }

        }

        public bool isEqual(Playfield p, bool logg)
        {
            if (logg)
            {
                if (this.value != p.value) return false;
            }
            if (this.enemySecretCount != p.enemySecretCount)
            {

                if (logg) Helpfunctions.Instance.logg("enemy secrets changed ");
                return false;
            }

            if (this.mana != p.mana || this.enemyMaxMana != p.enemyMaxMana || this.ownMaxMana != p.ownMaxMana)
            {
                if (logg) Helpfunctions.Instance.logg("mana changed " + this.mana + " " + p.mana + " " + this.enemyMaxMana + " " + p.enemyMaxMana + " " + this.ownMaxMana + " " + p.ownMaxMana);
                return false;
            }

            if (this.ownDeckSize != p.ownDeckSize || this.enemyDeckSize != p.enemyDeckSize || this.ownHeroFatigue != p.ownHeroFatigue || this.enemyHeroFatigue != p.enemyHeroFatigue)
            {
                if (logg) Helpfunctions.Instance.logg("deck/fatigue changed " + this.ownDeckSize + " " + p.ownDeckSize + " " + this.enemyDeckSize + " " + p.enemyDeckSize + " " + this.ownHeroFatigue + " " + p.ownHeroFatigue + " " + this.enemyHeroFatigue + " " + p.enemyHeroFatigue);
            }

            if (this.cardsPlayedThisTurn != p.cardsPlayedThisTurn || this.mobsplayedThisTurn != p.mobsplayedThisTurn || this.ueberladung != p.ueberladung)
            {
                if (logg) Helpfunctions.Instance.logg("stuff changed " + this.cardsPlayedThisTurn + " " + p.cardsPlayedThisTurn + " " + this.mobsplayedThisTurn + " " + p.mobsplayedThisTurn + " " + this.ueberladung + " " + p.ueberladung);
                return false;
            }

            if (this.ownHeroName != p.ownHeroName || this.enemyHeroName != p.enemyHeroName)
            {
                if (logg) Helpfunctions.Instance.logg("hero name changed ");
                return false;
            }

            if (this.ownHeroHp != p.ownHeroHp || this.ownheroAngr != p.ownheroAngr || this.ownHeroDefence != p.ownHeroDefence || this.ownHeroFrozen != p.ownHeroFrozen || this.heroImmuneWhileAttacking != p.heroImmuneWhileAttacking || this.heroImmune != p.heroImmune)
            {
                if (logg) Helpfunctions.Instance.logg("ownhero changed " + this.ownHeroHp + " " + p.ownHeroHp + " " + this.ownheroAngr + " " + p.ownheroAngr + " " + this.ownHeroDefence + " " + p.ownHeroDefence + " " + this.ownHeroFrozen + " " + p.ownHeroFrozen + " " + this.heroImmuneWhileAttacking + " " + p.heroImmuneWhileAttacking + " " + this.heroImmune + " " + p.heroImmune);
                return false;
            }
            if (this.ownHeroReady != p.ownHeroReady || this.ownWeaponAttack != p.ownWeaponAttack || this.ownWeaponDurability != p.ownWeaponDurability || this.ownHeroNumAttackThisTurn != p.ownHeroNumAttackThisTurn || this.ownHeroWindfury != p.ownHeroWindfury)
            {
                if (logg) Helpfunctions.Instance.logg("weapon changed " + this.ownHeroReady + " " + p.ownHeroReady + " " + this.ownWeaponAttack + " " + p.ownWeaponAttack + " " + this.ownWeaponDurability + " " + p.ownWeaponDurability + " " + this.ownHeroNumAttackThisTurn + " " + p.ownHeroNumAttackThisTurn + " " + this.ownHeroWindfury + " " + p.ownHeroWindfury);
                return false;
            }
            if (this.enemyHeroHp != p.enemyHeroHp || this.enemyWeaponAttack != p.enemyWeaponAttack || this.enemyHeroDefence != p.enemyHeroDefence || this.enemyWeaponDurability != p.enemyWeaponDurability || this.enemyHeroFrozen != p.enemyHeroFrozen || this.enemyHeroImmune != p.enemyHeroImmune)
            {
                if (logg) Helpfunctions.Instance.logg("enemyhero changed " + this.enemyHeroHp + " " + p.enemyHeroHp + " " + this.enemyWeaponAttack + " " + p.enemyWeaponAttack + " " + this.enemyHeroDefence + " " + p.enemyHeroDefence + " " + this.enemyWeaponDurability + " " + p.enemyWeaponDurability + " " + this.enemyHeroFrozen + " " + p.enemyHeroFrozen + " " + this.enemyHeroImmune + " " + p.enemyHeroImmune);
                return false;
            }

            /*if (this.auchenaiseelenpriesterin != p.auchenaiseelenpriesterin || this.winzigebeschwoererin != p.winzigebeschwoererin || this.zauberlehrling != p.zauberlehrling || this.managespenst != p.managespenst || this.soeldnerDerVenture != p.soeldnerDerVenture || this.beschwoerungsportal != p.beschwoerungsportal || this.doublepriest != p.doublepriest)
            {
                Helpfunctions.Instance.logg("special minions changed " + this.auchenaiseelenpriesterin + " " + p.auchenaiseelenpriesterin + " " + this.winzigebeschwoererin + " " + p.winzigebeschwoererin + " " + this.zauberlehrling + " " + p.zauberlehrling + " " + this.managespenst + " " + p.managespenst + " " + this.soeldnerDerVenture + " " + p.soeldnerDerVenture + " " + this.beschwoerungsportal + " " + p.beschwoerungsportal + " " + this.doublepriest + " " + p.doublepriest);
                return false;
            }*/

            if (this.ownHeroAblility.name != p.ownHeroAblility.name)
            {
                if (logg) Helpfunctions.Instance.logg("hero ability changed ");
                return false;
            }

            if (this.spellpower != p.spellpower)
            {
                if (logg) Helpfunctions.Instance.logg("spellpower changed");
                return false;
            }

            if (this.ownMinions.Count != p.ownMinions.Count || this.enemyMinions.Count != p.enemyMinions.Count || this.owncards.Count != p.owncards.Count)
            {
                if (logg) Helpfunctions.Instance.logg("minions count or hand changed");
                return false;
            }

            bool minionbool = true;
            for (int i = 0; i < this.ownMinions.Count; i++)
            {
                Minion dis = this.ownMinions[i]; Minion pis = p.ownMinions[i];
                //if (dis.entitiyID == 0) dis.entitiyID = pis.entitiyID;
                //if (pis.entitiyID == 0) pis.entitiyID = dis.entitiyID;
                if (dis.entitiyID != pis.entitiyID) minionbool = false;
                if (dis.Angr != pis.Angr || dis.Hp != pis.Hp || dis.maxHp != pis.maxHp || dis.numAttacksThisTurn != pis.numAttacksThisTurn) minionbool = false;
                if (dis.Ready != pis.Ready) minionbool = false; // includes frozen, exhaunted
                if (dis.playedThisTurn != pis.playedThisTurn || dis.numAttacksThisTurn != pis.numAttacksThisTurn) minionbool = false;
                if (dis.silenced != pis.silenced || dis.stealth != pis.stealth || dis.taunt != pis.taunt || dis.windfury != pis.windfury || dis.wounded != pis.wounded || dis.zonepos != pis.zonepos) minionbool = false;
                if (dis.divineshild != pis.divineshild || dis.cantLowerHPbelowONE != pis.cantLowerHPbelowONE || dis.immune != pis.immune) minionbool = false;

            }
            if (minionbool == false)
            {
                if (logg) Helpfunctions.Instance.logg("ownminions changed");
                return false;
            }

            for (int i = 0; i < this.enemyMinions.Count; i++)
            {
                Minion dis = this.enemyMinions[i]; Minion pis = p.enemyMinions[i];
                //if (dis.entitiyID == 0) dis.entitiyID = pis.entitiyID;
                //if (pis.entitiyID == 0) pis.entitiyID = dis.entitiyID;
                if (dis.entitiyID != pis.entitiyID) minionbool = false;
                if (dis.Angr != pis.Angr || dis.Hp != pis.Hp || dis.maxHp != pis.maxHp || dis.numAttacksThisTurn != pis.numAttacksThisTurn) minionbool = false;
                if (dis.Ready != pis.Ready) minionbool = false; // includes frozen, exhaunted
                if (dis.playedThisTurn != pis.playedThisTurn || dis.numAttacksThisTurn != pis.numAttacksThisTurn) minionbool = false;
                if (dis.silenced != pis.silenced || dis.stealth != pis.stealth || dis.taunt != pis.taunt || dis.windfury != pis.windfury || dis.wounded != pis.wounded || dis.zonepos != pis.zonepos) minionbool = false;
                if (dis.divineshild != pis.divineshild || dis.cantLowerHPbelowONE != pis.cantLowerHPbelowONE || dis.immune != pis.immune) minionbool = false;
            }
            if (minionbool == false)
            {
                if (logg) Helpfunctions.Instance.logg("enemyminions changed");
                return false;
            }

            for (int i = 0; i < this.owncards.Count; i++)
            {
                Handmanager.Handcard dishc = this.owncards[i]; Handmanager.Handcard pishc = p.owncards[i];
                if (dishc.position != pishc.position || dishc.entity != pishc.entity || dishc.getManaCost(this) != pishc.getManaCost(p))
                {
                    if (logg) Helpfunctions.Instance.logg("handcard changed: " + dishc.card.name);
                    return false;
                }
            }

            return true;
        }

        public void simulateEnemysTurn(bool simulateTwoTurns, bool playaround, bool print)
        {
            int maxwide = 20;

            this.enemyAbilityReady = true;
            this.enemyHeroNumAttackThisTurn = 0;
            this.enemyHeroWindfury = false;
            if (this.enemyWeaponName == "doomhammer") this.enemyHeroWindfury = true;
            this.enemyheroImmuneWhileAttacking = false;
            if (this.enemyWeaponName == "gladiatorslongbow") this.enemyheroImmuneWhileAttacking = true;
            if (!this.enemyHeroFrozen && this.enemyWeaponDurability > 0) this.enemyHeroReady = true;
            this.enemyheroAngr = this.enemyWeaponAttack;
            bool havedonesomething = true;
            List<Playfield> posmoves = new List<Playfield>();
            posmoves.Add(new Playfield(this));
            List<Playfield> temp = new List<Playfield>();
            int deep = 0;
            int enemMana = Math.Min(this.enemyMaxMana + 1, 10);
            if (playaround) posmoves[0].EnemyCardPlaying(this.enemyHeroName, enemMana, this.enemycarddraw + this.enemyAnzCards);

            while (havedonesomething)
            {

                temp.Clear();
                temp.AddRange(posmoves);
                havedonesomething = false;
                Playfield bestold = null;
                int bestoldval = 20000000;
                foreach (Playfield p in temp)
                {

                    if (p.complete)
                    {
                        continue;
                    }
                    List<Minion> playedMinions = new List<Minion>(8);

                    foreach (Minion m in p.enemyMinions)
                    {

                        if (m.Ready && m.Angr >= 1 && !m.frozen)
                        {
                            //BEGIN:cut (double/similar) attacking minions out#####################################
                            // DONT LET SIMMILAR MINIONS ATTACK IN ONE TURN (example 3 unlesh the hounds-hounds doesnt need to simulated hole)
                            List<Minion> tempoo = new List<Minion>(playedMinions);
                            bool dontattacked = true;
                            bool isSpecial = PenalityManager.Instance.specialMinions.ContainsKey(m.name);
                            foreach (Minion mnn in tempoo)
                            {
                                // special minions are allowed to attack in silended and unsilenced state!
                                //help.logg(mnn.silenced + " " + m.silenced + " " + mnn.name + " " + m.name + " " + penman.specialMinions.ContainsKey(m.name));

                                bool otherisSpecial = PenalityManager.Instance.specialMinions.ContainsKey(mnn.name);

                                if ((!isSpecial || (isSpecial && m.silenced)) && (!otherisSpecial || (otherisSpecial && mnn.silenced))) // both are not special, if they are the same, dont add
                                {
                                    if (mnn.Angr == m.Angr && mnn.Hp == m.Hp && mnn.divineshild == m.divineshild && mnn.taunt == m.taunt && mnn.poisonous == m.poisonous) dontattacked = false;
                                    continue;
                                }

                                if (isSpecial == otherisSpecial && !m.silenced && !mnn.silenced) // same are special
                                {
                                    if (m.name != mnn.name) // different name -> take it
                                    {
                                        continue;
                                    }
                                    // same name -> test whether they are equal
                                    if (mnn.Angr == m.Angr && mnn.Hp == m.Hp && mnn.divineshild == m.divineshild && mnn.taunt == m.taunt && mnn.poisonous == m.poisonous) dontattacked = false;
                                    continue;
                                }

                            }

                            if (dontattacked)
                            {
                                playedMinions.Add(m);
                            }
                            else
                            {
                                //help.logg(m.name + " doesnt need to attack!");
                                continue;
                            }
                            //END: cut (double/similar) attacking minions out#####################################

                            //help.logg(m.name + " is going to attack!");
                            List<targett> trgts = p.getAttackTargets(false);



                            if (true)//(this.useCutingTargets)
                            {
                                trgts = Ai.Instance.nextTurnSimulator.cutAttackTargets(trgts, p, false);
                            }

                            foreach (targett trgt in trgts)
                            {

                                Playfield pf = new Playfield(p);
                                havedonesomething = true;
                                pf.ENEMYattackWithMinion(m, trgt.target, trgt.targetEntity);
                                posmoves.Add(pf);


                            }
                            if (trgts.Count == 1 && trgts[0].target == 100)//only enemy hero is available als attack
                            {
                                break;
                            }
                        }

                    }
                    // attacked with minions done
                    // attack with hero
                    if (p.enemyHeroReady)
                    {
                        List<targett> trgts = p.getAttackTargets(false);

                        havedonesomething = true;


                        if (true)//(this.useCutingTargets)
                        {
                            trgts = Ai.Instance.nextTurnSimulator.cutAttackTargets(trgts, p, false);
                        }

                        foreach (targett trgt in trgts)
                        {
                            Playfield pf = new Playfield(p);
                            pf.ENEMYattackWithWeapon(trgt.target, trgt.targetEntity, 0);
                            posmoves.Add(pf);
                        }
                    }

                    // use ability
                    /// TODO check if ready after manaup

                    if (p.enemyAbilityReady && p.enemyHeroAblility.canplayCard(p, 0))
                    {
                        int abilityPenality = 0;

                        havedonesomething = true;
                        // if we have mage or priest, we have to target something####################################################
                        if (p.enemyHeroName == HeroEnum.mage || p.enemyHeroName == HeroEnum.priest)
                        {

                            List<targett> trgts = p.enemyHeroAblility.getTargetsForCard(p);
                            foreach (targett trgt in trgts)
                            {
                                Playfield pf = new Playfield(p);
                                havedonesomething = true;
                                pf.ENEMYactivateAbility(p.enemyHeroAblility, trgt.target, trgt.targetEntity);
                                posmoves.Add(pf);
                            }
                        }
                        else
                        {
                            // the other classes dont have to target####################################################
                            Playfield pf = new Playfield(p);

                            havedonesomething = true;
                            pf.ENEMYactivateAbility(p.enemyHeroAblility, -1, -1);
                            posmoves.Add(pf);
                        }

                    }

                    p.endEnemyTurn();

                    if (Ai.Instance.botBase.getPlayfieldValue(p) < bestoldval) // want the best enemy-play-> worst for us
                    {
                        bestoldval = Ai.Instance.botBase.getPlayfieldValue(p);
                        bestold = p;
                    }
                    posmoves.Remove(p);

                    if (posmoves.Count >= maxwide) break;
                }

                if (bestoldval <= 10000 && bestold != null)
                {
                    posmoves.Add(bestold);
                }

                deep++;
                if (posmoves.Count >= maxwide) break;
            }

            foreach (Playfield p in posmoves)
            {
                if (!p.complete) p.endEnemyTurn();
            }

            int bestval = int.MaxValue;
            Playfield bestplay = posmoves[0];
            if (print) bestplay.printBoard();
            foreach (Playfield p in posmoves)
            {
                int val = Ai.Instance.botBase.getPlayfieldValue(p);
                if (bestval > val)// we search the worst value
                {
                    bestplay = p;
                    bestval = val;
                }
            }

            this.value = bestplay.value;
            if (simulateTwoTurns)
            {
                bestplay.prepareNextTurn();
                this.value = (int)(0.5 * bestval + 0.5 * Ai.Instance.nextTurnSimulator.doallmoves(bestplay, false));
            }



        }


        private int EnemyCardPlaying(HeroEnum enemyHeroNamee, int currmana, int cardcount)
        {
            int mana = currmana;
            if (cardcount == 0) return currmana;
            if (enemyHeroNamee == HeroEnum.mage)
            {
                mana = EnemyPlaysACard("flamestrike", mana);
                mana = EnemyPlaysACard("blizzard", mana);
            }

            if (enemyHeroNamee == HeroEnum.hunter)
            {
                mana = EnemyPlaysACard("unleashthehounds", mana);
            }

            if (enemyHeroNamee == HeroEnum.priest)
            {
                mana = EnemyPlaysACard("holynova", mana);
            }

            if (enemyHeroNamee == HeroEnum.shaman)
            {
                mana = EnemyPlaysACard("lightningstorm", mana);
            }

            if (enemyHeroNamee == HeroEnum.warrior)
            {
                mana = EnemyPlaysACard("whirlwind", mana);
            }

            if (enemyHeroNamee == HeroEnum.pala)
            {
                mana = EnemyPlaysACard("consecration", mana);
            }

            if (enemyHeroNamee == HeroEnum.druid)
            {
                mana = EnemyPlaysACard("swipe", mana);
            }



            return mana;
        }

        private int EnemyPlaysACard(string cardname, int currmana)
        {

            //todo manacosts

            if (cardname == "flamestrike" && currmana >= 7)
            {
                List<Minion> temp = new List<Minion>(this.ownMinions);
                int damage = getEnemySpellDamageDamage(4);
                foreach (Minion enemy in temp)
                {
                    minionGetDamagedOrHealed(enemy, damage, 0, true, true);
                }

                currmana -= 7;
                return currmana;
            }

            if (cardname == "blizzard" && currmana >= 6)
            {
                List<Minion> temp = new List<Minion>(this.ownMinions);
                int damage = getEnemySpellDamageDamage(2);
                foreach (Minion enemy in temp)
                {
                    enemy.frozen = true;
                    minionGetDamagedOrHealed(enemy, damage, 0, true, true);
                }

                currmana -= 6;
                return currmana;
            }


            if (cardname == "unleashthehounds" && currmana >= 5)
            {
                int anz = this.ownMinions.Count;
                int posi = this.enemyMinions.Count - 1;
                CardDB.Card kid = CardDB.Instance.getCardDataFromID("EX1_538t");//hound
                for (int i = 0; i < anz; i++)
                {
                    callKid(kid, posi, false);
                }
                currmana -= 5;
                return currmana;
            }





            if (cardname == "holynova" && currmana >= 5)
            {
                List<Minion> temp = new List<Minion>(this.enemyMinions);
                int heal = 2;
                int damage = getEnemySpellDamageDamage(2);
                foreach (Minion enemy in temp)
                {
                    minionGetDamagedOrHealed(enemy, 0, heal, false, true);
                }
                attackOrHealHero(-heal, false);
                temp.Clear();
                temp.AddRange(this.ownMinions);
                foreach (Minion enemy in temp)
                {
                    minionGetDamagedOrHealed(enemy, damage, 0, true, true);
                }
                attackOrHealHero(damage, true);
                currmana -= 5;
                return currmana;
            }




            if (cardname == "lightningstorm" && currmana >= 3)
            {
                List<Minion> temp = new List<Minion>(this.ownMinions);
                int damage = getEnemySpellDamageDamage(2);
                foreach (Minion enemy in temp)
                {
                    minionGetDamagedOrHealed(enemy, damage, 0, true, true);
                }
                currmana -= 3;
                return currmana;
            }



            if (cardname == "whirlwind" && currmana >= 1)
            {
                List<Minion> temp = new List<Minion>(this.enemyMinions);
                int damage = getEnemySpellDamageDamage(1);
                foreach (Minion enemy in temp)
                {
                    minionGetDamagedOrHealed(enemy, damage, 0, false, true);
                }
                temp.Clear();
                temp = new List<Minion>(this.ownMinions);
                foreach (Minion enemy in temp)
                {
                    minionGetDamagedOrHealed(enemy, damage, 0, true, true);
                }
                currmana -= 1;
                return currmana;
            }



            if (cardname == "consecration" && currmana >= 4)
            {
                List<Minion> temp = new List<Minion>(this.ownMinions);
                int damage = getEnemySpellDamageDamage(2);
                foreach (Minion enemy in temp)
                {
                    minionGetDamagedOrHealed(enemy, damage, 0, true, true);
                }

                attackOrHealHero(damage, true);
                currmana -= 4;
                return currmana;
            }



            if (cardname == "swipe" && currmana >= 4)
            {
                int damage = getEnemySpellDamageDamage(4);
                // all others get 1 spelldamage
                int damage1 = getEnemySpellDamageDamage(1);

                List<Minion> temp = new List<Minion>(this.ownMinions);
                int target = 10;
                foreach (Minion mnn in temp)
                {
                    if (mnn.Hp <= damage || PenalityManager.Instance.specialMinions.ContainsKey(mnn.name))
                    {
                        target = mnn.id;
                    }
                }
                foreach (Minion mnn in temp)
                {
                    if (mnn.id + 10 != target)
                    {
                        minionGetDamagedOrHealed(mnn, damage1, 0, false);
                    }
                }
                currmana -= 4;
                return currmana;
            }





            return currmana;
        }

        private int getEnemySpellDamageDamage(int dmg)
        {
            int retval = dmg;
            retval += this.enemyspellpower;
            if (this.enemydoublepriest >= 1) retval *= (2 * this.enemydoublepriest);
            return retval;
        }

        public void prepareNextTurn()
        {
            this.ownMaxMana = Math.Min(10, this.ownMaxMana + 1);
            this.mana = this.ownMaxMana - this.ueberladung;
            foreach (Minion m in ownMinions)
            {
                m.Ready = true;
                m.numAttacksThisTurn = 0;
                m.playedThisTurn = false;
            }

            if (this.ownWeaponName != "") this.ownHeroReady = true;
            this.ownheroAngr = this.ownWeaponAttack;
            this.ownHeroFrozen = false;
            this.ownAbilityReady = true;
            this.complete = false;
            this.sEnemTurn = false;
            this.value = int.MinValue;
        }

        public List<targett> getAttackTargets(bool own)
        {
            List<targett> trgts = new List<targett>();
            List<targett> trgts2 = new List<targett>();
            bool hastanks = false;
            if (own)
            {
                trgts2.Add(new targett(200, this.enemyHeroEntity));
                foreach (Minion m in this.enemyMinions)
                {
                    if (m.stealth) continue; // cant target stealth

                    if (m.taunt)
                    {
                        hastanks = true;
                        trgts.Add(new targett(m.id + 10, m.entitiyID));
                    }
                    else
                    {
                        trgts2.Add(new targett(m.id + 10, m.entitiyID));
                    }
                }
            }
            else
            {

                foreach (Minion m in this.ownMinions)
                {
                    if (m.stealth) continue; // cant target stealth

                    if (m.taunt)
                    {
                        hastanks = true;
                        trgts.Add(new targett(m.id, m.entitiyID));
                    }
                    else
                    {
                        trgts2.Add(new targett(m.id, m.entitiyID));
                    }
                }

                if (trgts2.Count == 0) trgts2.Add(new targett(100, this.ownHeroEntity));
            }

            if (hastanks) return trgts;

            return trgts2;


        }

        public int getBestPlace(CardDB.Card card, bool lethal)
        {
            if (card.type != CardDB.cardtype.MOB) return 0;
            if (this.ownMinions.Count == 0) return 0;
            if (this.ownMinions.Count == 1) return 1;

            int[] places = new int[this.ownMinions.Count];
            int i = 0;
            int tempval = 0;
            if (lethal && card.specialMin == CardDB.specialMinions.defenderofargus)
            {
                i = 0;
                foreach (Minion m in this.ownMinions)
                {

                    places[i] = 0;
                    tempval = 0;
                    if (m.Ready)
                    {
                        tempval -= m.Angr - 1;
                        if (m.windfury) tempval -= m.Angr - 1;
                    }
                    places[i] = tempval;

                    i++;
                }


                i = 0;
                int bestpl = 7;
                int bestval = 10000;
                foreach (Minion m in this.ownMinions)
                {
                    int prev = 0;
                    int next = 0;
                    if (i >= 1) prev = places[i - 1];
                    next = places[i];
                    if (bestval > prev + next)
                    {
                        bestval = prev + next;
                        bestpl = i;
                    }
                    i++;
                }
                return bestpl;
            }
            if (card.specialMin == CardDB.specialMinions.sunfuryprotector || card.specialMin == CardDB.specialMinions.defenderofargus) // bestplace, if right and left minions have no taunt + lots of hp, dont make priority-minions to taunt
            {
                i = 0;
                foreach (Minion m in this.ownMinions)
                {

                    places[i] = 0;
                    tempval = 0;
                    if (!m.taunt)
                    {
                        tempval -= m.Hp;
                    }
                    else
                    {
                        tempval -= m.Hp + 2;
                    }

                    if (m.handcard.card.specialMin == CardDB.specialMinions.flametonguetotem) tempval += 50;
                    if (m.handcard.card.specialMin == CardDB.specialMinions.raidleader) tempval += 10;
                    if (m.handcard.card.specialMin == CardDB.specialMinions.grimscaleoracle) tempval += 10;
                    if (m.handcard.card.specialMin == CardDB.specialMinions.direwolfalpha) tempval += 50;
                    if (m.handcard.card.specialMin == CardDB.specialMinions.murlocwarleader) tempval += 10;
                    if (m.handcard.card.specialMin == CardDB.specialMinions.southseacaptain) tempval += 10;
                    if (m.handcard.card.specialMin == CardDB.specialMinions.stormwindchampion) tempval += 10;
                    if (m.handcard.card.specialMin == CardDB.specialMinions.timberwolf) tempval += 10;
                    if (m.handcard.card.specialMin == CardDB.specialMinions.leokk) tempval += 10;
                    if (m.handcard.card.specialMin == CardDB.specialMinions.northshirecleric) tempval += 10;
                    if (m.handcard.card.specialMin == CardDB.specialMinions.sorcerersapprentice) tempval += 10;
                    if (m.handcard.card.specialMin == CardDB.specialMinions.pintsizedsummoner) tempval += 10;
                    if (m.handcard.card.specialMin == CardDB.specialMinions.summoningportal) tempval += 10;
                    if (m.handcard.card.specialMin == CardDB.specialMinions.scavenginghyena) tempval += 10;

                    places[i] = tempval;

                    i++;
                }


                i = 0;
                int bestpl = 7;
                int bestval = 10000;
                foreach (Minion m in this.ownMinions)
                {
                    int prev = 0;
                    int next = 0;
                    if (i >= 1) prev = places[i - 1];
                    next = places[i];
                    if (bestval > prev + next)
                    {
                        bestval = prev + next;
                        bestpl = i;
                    }
                    i++;
                }
                return bestpl;
            }

            int cardIsBuffer = 0;
            bool placebuff = false;
            if (card.specialMin == CardDB.specialMinions.flametonguetotem || card.specialMin == CardDB.specialMinions.direwolfalpha)
            {
                placebuff = true;
                if (card.specialMin == CardDB.specialMinions.flametonguetotem) cardIsBuffer = 2;
                if (card.specialMin == CardDB.specialMinions.direwolfalpha) cardIsBuffer = 1;
            }
            bool commander = false;
            foreach (Minion m in this.ownMinions)
            {
                if (m.handcard.card.specialMin == CardDB.specialMinions.warsongcommander) commander = true;
            }
            foreach (Minion m in this.ownMinions)
            {
                if (m.handcard.card.specialMin == CardDB.specialMinions.flametonguetotem || m.handcard.card.specialMin == CardDB.specialMinions.direwolfalpha) placebuff = true;
            }
            //attackmaxing :D
            if (placebuff)
            {


                int cval = 0;
                if (card.Charge || (card.Attack <= 3 && commander))
                {
                    cval = card.Attack;
                    if (card.windfury) cval = card.Attack;
                }
                i = 0;
                int[] buffplaces = new int[this.ownMinions.Count];
                int gesval = 0;
                foreach (Minion m in this.ownMinions)
                {
                    buffplaces[i] = 0;
                    places[i] = 0;
                    if (m.Ready)
                    {
                        tempval = m.Angr;
                        if (m.windfury && m.numAttacksThisTurn == 0) tempval += m.Angr;

                    }
                    if (m.handcard.card.specialMin == CardDB.specialMinions.flametonguetotem)
                    {
                        buffplaces[i] = 2;
                    }
                    if (m.handcard.card.specialMin == CardDB.specialMinions.direwolfalpha)
                    {
                        buffplaces[i] = 1;
                    }
                    places[i] = tempval;
                    gesval += tempval;
                    i++;
                }

                int bplace = 0;
                int bvale = 0;
                tempval = 0;
                i = 0;
                for (int j = 0; j <= this.ownMinions.Count; j++)
                {
                    tempval = gesval;
                    int current = cval;
                    int prev = 0;
                    int next = 0;
                    if (i >= 1)
                    {
                        tempval -= places[i - 1];
                        prev = places[i - 1];
                        prev += cardIsBuffer;
                        current += buffplaces[i - 1];
                        if (i < this.ownMinions.Count)
                        {
                            prev -= buffplaces[i];
                        }
                    }
                    if (i < this.ownMinions.Count)
                    {
                        tempval -= places[i];
                        next = places[i];
                        next += cardIsBuffer;
                        current += buffplaces[i];
                        if (i >= 1)
                        {
                            next -= buffplaces[i - 1];
                        }
                    }
                    tempval += current + prev + next;
                    if (tempval > bvale)
                    {
                        bplace = i;
                        bvale = tempval;
                    }
                    i++;
                }
                return bplace;

            }

            // normal placement
            int cardvalue = card.Attack * 2 + card.Health;
            if (card.tank)
            {
                cardvalue += 5;
                cardvalue += card.Health;
            }

            if (card.specialMin == CardDB.specialMinions.flametonguetotem) cardvalue += 90;
            if (card.specialMin == CardDB.specialMinions.raidleader) cardvalue += 10;
            if (card.specialMin == CardDB.specialMinions.grimscaleoracle) cardvalue += 10;
            if (card.specialMin == CardDB.specialMinions.direwolfalpha) cardvalue += 90;
            if (card.specialMin == CardDB.specialMinions.murlocwarleader) cardvalue += 10;
            if (card.specialMin == CardDB.specialMinions.southseacaptain) cardvalue += 10;
            if (card.specialMin == CardDB.specialMinions.stormwindchampion) cardvalue += 10;
            if (card.specialMin == CardDB.specialMinions.timberwolf) cardvalue += 10;
            if (card.specialMin == CardDB.specialMinions.leokk) cardvalue += 10;
            if (card.specialMin == CardDB.specialMinions.northshirecleric) cardvalue += 10;
            if (card.specialMin == CardDB.specialMinions.sorcerersapprentice) cardvalue += 10;
            if (card.specialMin == CardDB.specialMinions.pintsizedsummoner) cardvalue += 10;
            if (card.specialMin == CardDB.specialMinions.summoningportal) cardvalue += 10;
            if (card.specialMin == CardDB.specialMinions.scavenginghyena) cardvalue += 10;
            if (card.specialMin == CardDB.specialMinions.faeriedragon) cardvalue += 40;
            cardvalue += 1;

            i = 0;
            foreach (Minion m in this.ownMinions)
            {
                places[i] = 0;
                tempval = m.Angr * 2 + m.maxHp;
                if (m.taunt)
                {
                    tempval += 6;
                    tempval += m.maxHp;
                }
                if (!m.silenced)
                {
                    if (m.handcard.card.specialMin == CardDB.specialMinions.flametonguetotem) tempval += 90;
                    if (m.handcard.card.specialMin == CardDB.specialMinions.raidleader) tempval += 10;
                    if (m.handcard.card.specialMin == CardDB.specialMinions.grimscaleoracle) tempval += 10;
                    if (m.handcard.card.specialMin == CardDB.specialMinions.direwolfalpha) tempval += 90;
                    if (m.handcard.card.specialMin == CardDB.specialMinions.murlocwarleader) tempval += 10;
                    if (m.handcard.card.specialMin == CardDB.specialMinions.southseacaptain) tempval += 10;
                    if (m.handcard.card.specialMin == CardDB.specialMinions.stormwindchampion) tempval += 10;
                    if (m.handcard.card.specialMin == CardDB.specialMinions.timberwolf) tempval += 10;
                    if (m.handcard.card.specialMin == CardDB.specialMinions.leokk) tempval += 10;
                    if (m.handcard.card.specialMin == CardDB.specialMinions.northshirecleric) tempval += 10;
                    if (m.handcard.card.specialMin == CardDB.specialMinions.sorcerersapprentice) tempval += 10;
                    if (m.handcard.card.specialMin == CardDB.specialMinions.pintsizedsummoner) tempval += 10;
                    if (m.handcard.card.specialMin == CardDB.specialMinions.summoningportal) tempval += 10;
                    if (m.handcard.card.specialMin == CardDB.specialMinions.scavenginghyena) tempval += 10;
                    if (m.handcard.card.specialMin == CardDB.specialMinions.faeriedragon) tempval += 40;
                    if (m.stealth) tempval += 40;
                }
                places[i] = tempval;

                i++;
            }

            //bigminion if >=10
            int bestplace = 0;
            int bestvale = 0;
            tempval = 0;
            i = 0;
            for (int j = 0; j <= this.ownMinions.Count; j++)
            {
                int prev = cardvalue;
                int next = cardvalue;
                if (i >= 1) prev = places[i - 1];
                if (i < this.ownMinions.Count) next = places[i];


                if (cardvalue >= prev && cardvalue >= next)
                {
                    tempval = 2 * cardvalue - prev - next;
                    if (tempval > bestvale)
                    {
                        bestplace = i;
                        bestvale = tempval;
                    }
                }
                if (cardvalue <= prev && cardvalue <= next)
                {
                    tempval = -2 * cardvalue + prev + next;
                    if (tempval > bestvale)
                    {
                        bestplace = i;
                        bestvale = tempval;
                    }
                }

                i++;
            }

            return bestplace;
        }

        public int getBestPlacePrint(CardDB.Card card)
        {
            if (card.type != CardDB.cardtype.MOB) return 0;
            if (this.ownMinions.Count == 0) return 0;
            if (this.ownMinions.Count == 1) return 1;

            int[] places = new int[this.ownMinions.Count];
            int i = 0;
            int tempval = 0;
            if (card.name == "sunfuryprotector" || card.name == "defenderofargus") // bestplace, if right and left minions have no taunt + lots of hp, dont make priority-minions to taunt
            {
                i = 0;
                foreach (Minion m in this.ownMinions)
                {

                    places[i] = 0;
                    tempval = 0;
                    if (!m.taunt)
                    {
                        tempval -= m.Hp;
                    }
                    else
                    {
                        tempval = 30;
                    }

                    if (m.name == "flametonguetotem") tempval += 50;
                    if (m.name == "raidleader") tempval += 10;
                    if (m.name == "grimscaleoracle") tempval += 10;
                    if (m.name == "direwolfalpha") tempval += 50;
                    if (m.name == "murlocwarleader") tempval += 10;
                    if (m.name == "southseacaptain") tempval += 10;
                    if (m.name == "stormwindchampion") tempval += 10;
                    if (m.name == "timberwolf") tempval += 10;
                    if (m.name == "leokk") tempval += 10;
                    if (m.name == "northshirecleric") tempval += 10;
                    if (m.name == "sorcerersapprentice") tempval += 10;
                    if (m.name == "pint-sizedsummoner") tempval += 10;
                    if (m.name == "summoningportal") tempval += 10;
                    if (m.name == "scavenginghyena") tempval += 10;

                    places[i] = tempval;

                    i++;
                }


                i = 0;
                int bestpl = 7;
                int bestval = 10000;
                foreach (Minion m in this.ownMinions)
                {
                    Helpfunctions.Instance.logg(places[i] + "");
                    int prev = 0;
                    int next = 0;
                    if (i >= 1) prev = places[i - 1];
                    next = places[i];
                    if (bestval > prev + next)
                    {
                        bestval = prev + next;
                        bestpl = i;
                    }
                    i++;
                }
                return bestpl;
            }

            // normal placement
            int cardvalue = card.Attack * 2 + card.Health;
            if (card.tank)
            {
                cardvalue += 5;
                cardvalue += card.Health;
            }

            if (card.name == "flametonguetotem") cardvalue += 50;
            if (card.name == "raidleader") cardvalue += 10;
            if (card.name == "grimscaleoracle") cardvalue += 10;
            if (card.name == "direwolfalpha") cardvalue += 50;
            if (card.name == "murlocwarleader") cardvalue += 10;
            if (card.name == "southseacaptain") cardvalue += 10;
            if (card.name == "stormwindchampion") cardvalue += 10;
            if (card.name == "timberwolf") cardvalue += 10;
            if (card.name == "leokk") cardvalue += 10;
            if (card.name == "northshirecleric") cardvalue += 10;
            if (card.name == "sorcerersapprentice") cardvalue += 10;
            if (card.name == "pint-sizedsummoner") cardvalue += 10;
            if (card.name == "summoningportal") cardvalue += 10;
            if (card.name == "scavenginghyena") cardvalue += 10;
            cardvalue += 1;

            i = 0;
            foreach (Minion m in this.ownMinions)
            {
                places[i] = 0;
                tempval = m.Angr * 2 + m.maxHp;
                if (m.taunt)
                {
                    tempval += 6;
                    tempval += m.maxHp;
                }

                if (m.name == "flametonguetotem") tempval += 50;
                if (m.name == "raidleader") tempval += 10;
                if (m.name == "grimscaleoracle") tempval += 10;
                if (m.name == "direwolfalpha") tempval += 50;
                if (m.name == "murlocwarleader") tempval += 10;
                if (m.name == "southseacaptain") tempval += 10;
                if (m.name == "stormwindchampion") tempval += 10;
                if (m.name == "timberwolf") tempval += 10;
                if (m.name == "leokk") tempval += 10;
                if (m.name == "northshirecleric") tempval += 10;
                if (m.name == "sorcerersapprentice") tempval += 10;
                if (m.name == "pint-sizedsummoner") tempval += 10;
                if (m.name == "summoningportal") tempval += 10;
                if (m.name == "scavenginghyena") tempval += 10;

                places[i] = tempval;
                Helpfunctions.Instance.logg(places[i] + "");

                i++;
            }

            //bigminion if >=10
            int bestplace = 0;
            int bestvale = 0;
            tempval = 0;
            i = 0;
            Helpfunctions.Instance.logg(cardvalue + " (own)");
            i = 0;
            for (int j = 0; j <= this.ownMinions.Count; j++)
            {
                int prev = cardvalue;
                int next = cardvalue;
                if (i >= 1) prev = places[i - 1];
                if (i < this.ownMinions.Count)
                {
                    next = places[i];
                }


                if (cardvalue >= prev && cardvalue >= next)
                {
                    tempval = 2 * cardvalue - prev - next;
                    if (tempval > bestvale)
                    {
                        bestplace = i;
                        bestvale = tempval;
                    }
                }
                if (cardvalue <= prev && cardvalue <= next)
                {
                    tempval = -2 * cardvalue + prev + next;
                    if (tempval > bestvale)
                    {
                        bestplace = i;
                        bestvale = tempval;
                    }
                }

                i++;
            }
            Helpfunctions.Instance.logg(bestplace + " (best)");
            return bestplace;
        }

        private void endEnemyTurn()
        {
            endTurnEffect(false);//own turn ends
            endTurnBuffs(false);//end enemy turn
            startTurnEffect(true);//start your turn
            this.complete = true;
            //Ai.Instance.botBase.getPlayfieldValue(this);

        }

        public void endTurn(bool simulateTwoTurns, bool playaround, bool print = false)
        {
            this.value = int.MinValue;

            //penalty for destroying combo

            this.evaluatePenality += ComboBreaker.Instance.checkIfComboWasPlayed(this.playactions, this.ownWeaponName);

            if (this.complete) return;
            endTurnEffect(true);//own turn ends
            endTurnBuffs(true);//end own buffs 
            startTurnEffect(false);//enemy turn begins
            simulateTraps();
            if (!sEnemTurn)
            {
                guessHeroDamage();
                endTurnEffect(false);//own turn ends
                endTurnBuffs(false);//end enemy turn
                startTurnEffect(true);//start your turn
                this.complete = true;
            }
            else
            {
                guessHeroDamage();
                simulateEnemysTurn(simulateTwoTurns, playaround, print);
                this.complete = true;
            }

        }

        private void guessHeroDamage()
        {
            int ghd = 0;
            foreach (Minion m in this.enemyMinions)
            {
                if (m.frozen) continue;
                ghd += m.Angr;
                if (m.windfury) ghd += m.Angr;
            }

            if (this.enemyHeroName == HeroEnum.druid) ghd++;
            if (this.enemyHeroName == HeroEnum.mage) ghd++;
            if (this.enemyHeroName == HeroEnum.thief) ghd++;
            if (this.enemyHeroName == HeroEnum.hunter) ghd += 2;
            ghd += enemyWeaponAttack;

            foreach (Minion m in this.ownMinions)
            {
                if (m.frozen) continue;
                if (m.taunt) ghd -= m.Hp;
                if (m.taunt && m.divineshild) ghd -= 1;
            }

            int guessingHeroDamage = Math.Max(0, ghd);
            this.guessingHeroHP = this.ownHeroHp + this.ownHeroDefence - guessingHeroDamage;
        }

        private void simulateTraps()
        {
            // DONT KILL ENEMY HERO (cause its only guessing)
            foreach (string secretID in this.ownSecretsIDList)
            {
                //hunter secrets############
                if (secretID == "EX1_554") //snaketrap
                {

                    //call 3 snakes (if possible)
                    int posi = this.ownMinions.Count - 1;
                    CardDB.Card kid = CardDB.Instance.getCardDataFromID("EX1_554t");//snake
                    callKid(kid, posi, true);
                    callKid(kid, posi, true);
                    callKid(kid, posi, true);
                }
                if (secretID == "EX1_609") //snipe
                {
                    //kill weakest minion of enemy
                    List<Minion> temp = new List<Minion>(this.enemyMinions);
                    temp.Sort((a, b) => a.Angr.CompareTo(b.Angr));//take the weakest
                    if (temp.Count == 0) continue;
                    Minion m = temp[0];
                    minionGetDamagedOrHealed(m, 4, 0, false);
                }
                if (secretID == "EX1_610") //explosive trap
                {
                    //take 2 damage to each enemy
                    List<Minion> temp = new List<Minion>(this.enemyMinions);
                    foreach (Minion m in temp)
                    {
                        minionGetDamagedOrHealed(m, 2, 0, false);
                    }
                    attackEnemyHeroWithoutKill(2);
                }
                if (secretID == "EX1_611") //freezing trap
                {
                    //return weakest enemy minion to hand
                    List<Minion> temp = new List<Minion>(this.enemyMinions);
                    temp.Sort((a, b) => a.Angr.CompareTo(b.Angr));//take the weakest
                    if (temp.Count == 0) continue;
                    Minion m = temp[0];
                    minionReturnToHand(m, false, 0);
                }
                if (secretID == "EX1_533") // missdirection
                {
                    // first damage to your hero is nulled -> lower guessingHeroDamage
                    List<Minion> temp = new List<Minion>(this.enemyMinions);
                    temp.Sort((a, b) => a.Angr.CompareTo(b.Angr));//take the weakest
                    if (temp.Count == 0) continue;
                    Minion m = temp[0];
                    m.Angr = 0;
                    this.evaluatePenality -= this.enemyMinions.Count;// the more the enemy minions has on board, the more the posibility to destroy something other :D
                }

                //mage secrets############
                if (secretID == "EX1_287") //counterspell
                {
                    // what should we do?
                    this.evaluatePenality -= 8;
                }

                if (secretID == "EX1_289") //ice barrier
                {
                    this.ownHeroDefence += 8;
                }

                if (secretID == "EX1_295") //ice barrier
                {
                    //set the guessed Damage to zero
                    foreach (Minion m in this.enemyMinions)
                    {
                        m.Angr = 0;
                    }
                }

                if (secretID == "EX1_294") //mirror entity
                {
                    //summon snake ( a weak minion)
                    int posi = this.ownMinions.Count - 1;
                    CardDB.Card kid = CardDB.Instance.getCardDataFromID("EX1_554t");//snake
                    callKid(kid, posi, true);
                }
                if (secretID == "tt_010") //spellbender
                {
                    //whut???
                    // add 2 to your defence (most attack-buffs give +2, lots of damage spells too)
                    this.evaluatePenality -= 4;
                }
                if (secretID == "EX1_594") // vaporize
                {
                    // first damage to your hero is nulled -> lower guessingHeroDamage and destroy weakest minion
                    List<Minion> temp = new List<Minion>(this.enemyMinions);
                    temp.Sort((a, b) => a.Angr.CompareTo(b.Angr));//take the weakest
                    if (temp.Count == 0) continue;
                    Minion m = temp[0];
                    minionGetDestroyed(m, false);
                }
                if (secretID == "EX1_duplicate") // duplicate
                {
                    // first damage to your hero is nulled -> lower guessingHeroDamage and destroy weakest minion
                    List<Minion> temp = new List<Minion>(this.ownMinions);
                    temp.Sort((a, b) => a.Angr.CompareTo(b.Angr));//take the weakest
                    if (temp.Count == 0) continue;
                    Minion m = temp[0];
                    drawACard(m.name, true);
                    drawACard(m.name, true);
                }

                //pala secrets############
                if (secretID == "EX1_132") // eye for an eye
                {
                    // enemy takes one damage
                    List<Minion> temp = new List<Minion>(this.enemyMinions);
                    temp.Sort((a, b) => a.Angr.CompareTo(b.Angr));//take the weakest
                    if (temp.Count == 0) continue;
                    Minion m = temp[0];
                    attackEnemyHeroWithoutKill(m.Angr);
                }
                if (secretID == "EX1_130") // noble sacrifice
                {
                    //spawn a 2/1 taunt!
                    int posi = this.ownMinions.Count - 1;
                    CardDB.Card kid = CardDB.Instance.getCardDataFromID("CS2_121");//frostwolfgrunt
                    callKid(kid, posi, true);
                    this.ownMinions[this.ownMinions.Count - 1].maxHp = 1;
                    this.ownMinions[this.ownMinions.Count - 1].Hp = 1;

                }

                if (secretID == "EX1_136") // redemption
                {
                    // we give our weakest minion a divine shield :D
                    List<Minion> temp = new List<Minion>(this.ownMinions);
                    temp.Sort((a, b) => a.Hp.CompareTo(b.Hp));//take the weakest
                    if (temp.Count == 0) continue;
                    foreach (Minion m in temp)
                    {
                        if (m.divineshild) continue;
                        m.divineshild = true;
                        break;
                    }
                }

                if (secretID == "EX1_379") // repentance
                {
                    // set his current lowest hp minion to x/1
                    List<Minion> temp = new List<Minion>(this.enemyMinions);
                    temp.Sort((a, b) => a.Hp.CompareTo(b.Hp));//take the weakest
                    if (temp.Count == 0) continue;
                    Minion m = temp[0];
                    m.Hp = 1;
                    m.maxHp = 1;
                }

                if (secretID == "EX1_Avenge") // avenge
                {
                    // we give our weakest minion +3/+2 :D
                    List<Minion> temp = new List<Minion>(this.ownMinions);
                    temp.Sort((a, b) => a.Hp.CompareTo(b.Hp));//take the weakest
                    if (temp.Count == 0) continue;
                    foreach (Minion m in temp)
                    {
                        minionGetBuffed(m, 3, 2, true);
                        break;
                    }
                }
            }


        }

        private void endTurnBuffs(bool own)
        {

            List<Minion> temp = new List<Minion>();

            if (own)
            {
                temp.AddRange(this.ownMinions);
            }
            else
            {
                temp.AddRange(this.enemyMinions);
            }
            // end buffs
            foreach (Minion m in temp)
            {
                m.cantLowerHPbelowONE = false;
                m.immune = false;
                List<Enchantment> tempench = new List<Enchantment>(m.enchantments);
                foreach (Enchantment e in tempench)
                {
                    if (e.CARDID == "EX1_316e")//ueberwaeltigende macht
                    {
                        minionGetDestroyed(m, own);
                    }

                    if (e.CARDID == "CS2_046e")//kampfrausch
                    {
                        debuff(m, e, own);
                    }

                    if (e.CARDID == "CS2_045e")// waffe felsbeiser
                    {
                        debuff(m, e, own);
                    }

                    if (e.CARDID == "EX1_046e")// dunkeleisenzwerg
                    {
                        debuff(m, e, own);
                    }
                    if (e.CARDID == "CS2_188o")// ruchloserunteroffizier
                    {
                        debuff(m, e, own);
                    }
                    if (e.CARDID == "EX1_055o")//  manasuechtige
                    {
                        debuff(m, e, own);
                    }
                    if (e.CARDID == "EX1_549o")//zorn des wildtiers
                    {
                        debuff(m, e, own);
                    }
                    if (e.CARDID == "EX1_334e")// dunkler wahnsin (control minion till end of turn)
                    {
                        //"uncontrol minion"
                        minionGetControlled(m, !own, true);
                    }

                }
            }

            temp.Clear();
            if (own)
            {
                temp.AddRange(this.enemyMinions);

            }
            else
            {
                temp.AddRange(this.ownMinions);
            }

            foreach (Minion m in temp)
            {
                m.cantLowerHPbelowONE = false;
                m.immune = false;
                List<Enchantment> tempench = new List<Enchantment>(m.enchantments);
                foreach (Enchantment e in tempench)
                {

                    if (e.CARDID == "EX1_046e")// dunkeleisenzwerg
                    {
                        debuff(m, e, !own);
                    }
                    if (e.CARDID == "CS2_188o")// ruchloserunteroffizier
                    {
                        debuff(m, e, !own);
                    }
                    if (e.CARDID == "EX1_549o")//zorn des wildtiers
                    {
                        debuff(m, e, !own);
                    }

                }
            }

        }


        private void endTurnEffect(bool own)
        {

            List<Minion> temp = new List<Minion>();
            List<Minion> ownmins = new List<Minion>();
            List<Minion> enemymins = new List<Minion>();
            if (own)
            {
                temp.AddRange(this.ownMinions);
                ownmins.AddRange(this.ownMinions);
                enemymins.AddRange(this.enemyMinions);
            }
            else
            {
                temp.AddRange(this.enemyMinions);
                ownmins.AddRange(this.enemyMinions);
                enemymins.AddRange(this.ownMinions);
            }



            foreach (Minion m in temp)
            {
                if (m.silenced) continue;

                if (m.name == "barongeddon") // all other chards get dmg get 2 dmg
                {
                    List<Minion> temp2 = new List<Minion>(this.ownMinions);
                    foreach (Minion mm in temp2)
                    {
                        if (mm.entitiyID != m.entitiyID)
                        {
                            minionGetDamagedOrHealed(mm, 2, 0, true);
                        }
                    }
                    temp2.Clear();
                    temp2.AddRange(this.enemyMinions);
                    foreach (Minion mm in temp2)
                    {
                        if (mm.entitiyID != m.entitiyID)
                        {
                            minionGetDamagedOrHealed(mm, 2, 0, false);
                        }
                    }
                    attackOrHealHero(2, true);
                    attackOrHealHero(2, false);

                }

                if (m.name == "bloodimp" || m.name == "youngpriestess") // buff a minion
                {
                    List<Minion> temp2 = new List<Minion>(ownmins);
                    temp2.Sort((a, b) => a.Hp.CompareTo(b.Hp));//buff the weakest
                    foreach (Minion mins in Helpfunctions.TakeList(temp2, 1))
                    {
                        minionGetBuffed(mins, 0, 1, own);
                    }
                }

                if (m.name == "masterswordsmith") // buff a minion
                {
                    List<Minion> temp2 = new List<Minion>(ownmins);
                    temp2.Sort((a, b) => a.Angr.CompareTo(b.Angr));//buff the weakest
                    foreach (Minion mins in Helpfunctions.TakeList(temp2, 1))
                    {
                        minionGetBuffed(mins, 1, 0, own);
                    }
                }

                if (m.name == "emboldener3000") // buff a minion
                {
                    List<Minion> temp2 = new List<Minion>(this.enemyMinions);
                    temp2.Sort((a, b) => -a.Angr.CompareTo(b.Angr));//buff the strongest enemy
                    foreach (Minion mins in Helpfunctions.TakeList(temp2, 1))
                    {
                        minionGetBuffed(mins, 1, 0, false);//buff alyways enemy :D
                    }
                }

                if (m.name == "gruul") // gain +1/+1
                {
                    minionGetBuffed(m, 1, 1, own);
                }

                if (m.name == "etherealarcanist") // gain +2/+2
                {
                    if (own && this.ownSecretsIDList.Count >= 1)
                    {
                        minionGetBuffed(m, 2, 2, own);
                    }
                    if (!own && this.enemySecretCount >= 1)
                    {
                        minionGetBuffed(m, 2, 2, own);
                    }
                }


                if (m.name == "manatidetotem") // draw card
                {
                    if (own)
                    {
                        //this.owncarddraw++;
                        this.drawACard("", own);
                    }
                    else
                    {
                        //this.enemycarddraw++;
                        this.drawACard("", own);
                    }
                }

                if (m.name == "healingtotem") // heal
                {
                    List<Minion> temp2 = new List<Minion>(ownmins);
                    foreach (Minion mins in temp2)
                    {
                        minionGetDamagedOrHealed(mins, 0, 1, own);
                    }
                }

                if (m.name == "hogger") // summon
                {
                    int posi = m.id;
                    CardDB.Card kid = CardDB.Instance.getCardDataFromID("NEW1_040t");//gnoll
                    callKid(kid, posi, own);
                }

                if (m.name == "impmaster") // damage itself and summon 
                {
                    int posi = m.id;
                    if (m.Hp == 1) posi--;
                    minionGetDamagedOrHealed(m, 1, 0, own);

                    CardDB.Card kid = CardDB.Instance.getCardDataFromID("EX1_598");//imp
                    callKid(kid, posi, own);
                }

                if (m.name == "natpagle") // draw card
                {
                    if (own)
                    {
                        //this.owncarddraw++;
                        this.drawACard("", own);
                    }
                    else
                    {
                        //this.enemycarddraw++;
                        this.drawACard("", own);
                    }
                }

                if (m.name == "ragnarosthefirelord") // summon
                {
                    if (this.enemyMinions.Count >= 1)
                    {
                        List<Minion> temp2 = new List<Minion>(enemymins);
                        temp2.Sort((a, b) => -a.Hp.CompareTo(b.Hp));//damage the stronges
                        foreach (Minion mins in Helpfunctions.TakeList(temp2, 1))
                        {
                            minionGetDamagedOrHealed(mins, 8, 0, !own);
                        }
                    }
                    else
                    {
                        attackOrHealHero(8, !own);
                    }
                }


                if (m.name == "repairbot") // heal damaged char
                {

                    attackOrHealHero(-6, false);
                }
                if (m.handcard.card.CardID == "EX1_tk9") //treant which is destroyed
                {
                    minionGetDestroyed(m, own);
                }

                if (m.name == "ysera") // draw card
                {
                    if (own)
                    {
                        //this.owncarddraw++;
                        this.drawACard("yseraawakens", own);
                    }
                    else
                    {
                        //this.enemycarddraw++;
                        this.drawACard("yseraawakens", own);
                    }
                }
            }

            foreach (Minion m in enemymins)
            {
                if (m.name == "gruul") // gain +1/+1
                {
                    minionGetBuffed(m, 1, 1, !own);
                }
            }

        }

        private void startTurnEffect(bool own)
        {
            List<Minion> temp = new List<Minion>();
            List<Minion> ownmins = new List<Minion>();
            List<Minion> enemymins = new List<Minion>();
            if (own)
            {
                temp.AddRange(this.ownMinions);
                ownmins.AddRange(this.ownMinions);
                enemymins.AddRange(this.enemyMinions);
            }
            else
            {
                temp.AddRange(this.enemyMinions);
                ownmins.AddRange(this.enemyMinions);
                enemymins.AddRange(this.ownMinions);
            }

            bool untergang = false;
            foreach (Minion m in temp)
            {
                if (m.silenced) continue;

                if (m.name == "demolisher") // deal 2 dmg
                {
                    List<Minion> temp2 = new List<Minion>(enemymins);
                    foreach (Minion mins in temp2)
                    {
                        minionGetDamagedOrHealed(mins, 2, 0, !own);
                    }
                }

                if (m.name == "shadeofnaxxramas") // buff itself
                {
                    minionGetBuffed(m, 1, 1, own);
                }

                if (m.name == "doomsayer") // destroy
                {
                    untergang = true;
                }

                if (m.name == "homingchicken") // ok
                {
                    minionGetDestroyed(m, own);
                    if (own)
                    {
                        //this.owncarddraw += 3;
                        this.drawACard("", own);
                        this.drawACard("", own);
                        this.drawACard("", own);
                    }
                    else
                    {
                        //this.enemycarddraw += 3 ;
                        this.drawACard("", own);
                        this.drawACard("", own);
                        this.drawACard("", own);
                    }
                }

                if (m.name == "lightwell") // heal
                {
                    if (ownmins.Count >= 1)
                    {
                        List<Minion> temp2 = new List<Minion>(ownmins);
                        bool healed = false;
                        foreach (Minion mins in temp2)
                        {
                            if (mins.wounded)
                            {
                                minionGetDamagedOrHealed(mins, 0, 3, own);
                                healed = true;
                                break;
                            }
                        }

                        if (!healed) attackOrHealHero(-3, own);
                    }
                    else
                    {
                        attackOrHealHero(-3, own);
                    }
                }

                if (m.name == "poultryizer") // 
                {
                    if (own)
                    {
                        if (this.enemyMinions.Count >= 1)
                        {
                            List<Minion> temp2 = new List<Minion>(this.enemyMinions);
                            temp2.Sort((a, b) => a.Hp.CompareTo(b.Hp));//damage the lowest
                            foreach (Minion mins in temp2)
                            {
                                CardDB.Card c = CardDB.Instance.getCardDataFromID("Mekka4t");
                                minionTransform(mins, c, false);
                                break;
                            }

                        }
                        else
                        {

                            List<Minion> temp2 = new List<Minion>(this.ownMinions);
                            temp2.Sort((a, b) => -a.Hp.CompareTo(b.Hp));//damage the stronges
                            foreach (Minion mins in temp2)
                            {
                                CardDB.Card c = CardDB.Instance.getCardDataFromID("Mekka4t");
                                minionTransform(mins, c, true);
                                break;
                            }
                        }
                    }
                    else
                    {
                        if (this.ownMinions.Count >= 1)
                        {
                            List<Minion> temp2 = new List<Minion>(this.ownMinions);
                            temp2.Sort((a, b) => -a.Hp.CompareTo(b.Hp));//damage the stronges
                            foreach (Minion mins in temp2)
                            {
                                CardDB.Card c = CardDB.Instance.getCardDataFromID("Mekka4t");
                                minionTransform(mins, c, true);
                                break;
                            }
                        }
                    }
                }

                if (m.name == "alarm-o-bot") // 
                {
                    if (own)
                    {
                        List<Handmanager.Handcard> temp2 = new List<Handmanager.Handcard>();
                        foreach (Handmanager.Handcard hc in this.owncards)
                        {
                            if (hc.card.type == CardDB.cardtype.MOB) temp2.Add(hc);
                        }
                        temp2.Sort((a, b) => -a.card.Attack.CompareTo(b.card.Attack));//damage the stronges
                        foreach (Handmanager.Handcard mins in temp2)
                        {
                            CardDB.Card c = CardDB.Instance.getCardDataFromID(mins.card.CardID);
                            minionTransform(m, c, true);
                            this.removeCard(mins);
                            this.drawACard("alarm-o-bot", true);
                            break;
                        }
                    }
                    else
                    {
                        minionGetBuffed(m, 4, 4, false);
                        m.Hp = m.maxHp;
                    }

                }


            }


            foreach (Minion m in enemymins) // search for corruption in other minions
            {
                List<Enchantment> elist = new List<Enchantment>(m.enchantments);
                foreach (Enchantment e in elist)
                {

                    if (e.CARDID == "CS2_063e")//corruption
                    {
                        if (own && e.controllerOfCreator == this.ownController) // own turn + we owner of curruption
                        {
                            minionGetDestroyed(m, false);
                        }
                        if (!own && e.controllerOfCreator != this.ownController)
                        {
                            minionGetDestroyed(m, true);
                        }
                    }
                }
            }

            if (untergang)
            {
                foreach (Minion mins in ownmins)
                {
                    minionGetDestroyed(mins, own);

                }
                foreach (Minion mins in enemymins)
                {
                    minionGetDestroyed(mins, !own);
                }
            }

            this.drawACard("", own);
        }

        private int getSpellDamageDamage(int dmg)
        {
            int retval = dmg;
            retval += this.spellpower;
            if (this.doublepriest >= 1) retval *= (2 * this.doublepriest);
            return retval;
        }



        private int getSpellHeal(int heal)
        {
            int retval = heal;
            retval += this.spellpower;
            if (this.auchenaiseelenpriesterin) retval *= -1;
            if (this.doublepriest >= 1) retval *= (2 * this.doublepriest);
            return retval;
        }

        private void attackEnemyHeroWithoutKill(int dmg)
        {
            if (this.enemyHeroImmune && dmg > 0) return;
            int oldHp = this.enemyHeroHp;
            if (dmg < 0 && this.enemyHeroHp <= 0) return;
            if (this.enemyHeroDefence <= 0)
            {
                this.enemyHeroHp = Math.Min(30, this.enemyHeroHp - dmg);
            }
            else
            {
                if (this.enemyHeroDefence > 0)
                {

                    int rest = enemyHeroDefence - dmg;
                    if (rest < 0)
                    {
                        this.enemyHeroHp += rest;
                    }
                    this.enemyHeroDefence = Math.Max(0, this.enemyHeroDefence - dmg);

                }
            }

            if (oldHp >= 1 && this.enemyHeroHp == 0) this.enemyHeroHp = 1;
        }

        private void attackOrHealHero(int dmg, bool own) // negative damage is heal
        {
            if (own)
            {
                if (this.heroImmune && dmg > 0) return;
                if (dmg < 0 || this.ownHeroDefence <= 0)
                {
                    if (dmg < 0 && this.ownHeroHp <= 0) return;
                    //heal
                    int copy = this.ownHeroHp;

                    if (dmg < 0 && this.ownHeroHp - dmg > 30) this.lostHeal += this.ownHeroHp - dmg - 30;

                    this.ownHeroHp = Math.Min(30, this.ownHeroHp - dmg);
                    if (copy < this.ownHeroHp)
                    {
                        triggerAHeroGetHealed(own);
                    }
                }
                else
                {
                    if (this.ownHeroDefence > 0 && dmg > 0)
                    {

                        int rest = this.ownHeroDefence - dmg;
                        if (rest < 0)
                        {
                            this.ownHeroHp += rest;
                        }
                        this.ownHeroDefence = Math.Max(0, this.ownHeroDefence - dmg);

                    }
                }


            }
            else
            {
                if (this.enemyHeroImmune && dmg > 0) return;
                if (dmg < 0 || this.enemyHeroDefence <= 0)
                {
                    if (dmg < 0 && this.enemyHeroHp <= 0) return;
                    int copy = this.enemyHeroHp;
                    if (dmg < 0 && this.enemyHeroHp - dmg > 30) this.lostHeal += this.enemyHeroHp - dmg - 30;
                    this.enemyHeroHp = Math.Min(30, this.enemyHeroHp - dmg);
                    if (copy < this.enemyHeroHp)
                    {
                        triggerAHeroGetHealed(own);
                    }
                }
                else
                {
                    if (this.enemyHeroDefence > 0 && dmg > 0)
                    {

                        int rest = enemyHeroDefence - dmg;
                        if (rest < 0)
                        {
                            this.enemyHeroHp += rest;
                        }
                        this.enemyHeroDefence = Math.Max(0, this.enemyHeroDefence - dmg);

                    }
                }

            }

        }

        private void debuff(Minion m, Enchantment e, bool own)
        {
            int anz = m.enchantments.RemoveAll(x => x.creator == e.creator && x.CARDID == e.CARDID);
            if (anz >= 1)
            {
                for (int i = 0; i < anz; i++)
                {

                    if (e.charge && !m.handcard.card.Charge && m.enchantments.FindAll(x => x.charge == true).Count == 0)
                    {
                        m.charge = false;
                    }
                    if (e.taunt && !m.handcard.card.tank && m.enchantments.FindAll(x => x.taunt == true).Count == 0)
                    {
                        m.taunt = false;
                    }
                    if (e.divineshild && m.enchantments.FindAll(x => x.divineshild == true).Count == 0)
                    {
                        m.divineshild = false;
                    }
                    if (e.windfury && !m.handcard.card.windfury && m.enchantments.FindAll(x => x.windfury == true).Count == 0)
                    {
                        m.divineshild = false;
                    }
                    if (e.imune && m.enchantments.FindAll(x => x.imune == true).Count == 0)
                    {
                        m.immune = false;
                    }
                    minionGetBuffed(m, -e.angrbuff, -e.hpbuff, own);
                }
            }
        }

        private void deleteEffectOf(string CardID, int creator)
        {
            // deletes the effect of the cardID with creator from all minions 
            Enchantment e = CardDB.getEnchantmentFromCardID(CardID);
            e.creator = creator;
            List<Minion> temp = new List<Minion>(this.ownMinions);
            foreach (Minion m in temp)
            {
                debuff(m, e, true);
            }
            temp.Clear();
            temp.AddRange(this.enemyMinions);
            foreach (Minion m in temp)
            {
                debuff(m, e, false);
            }
        }

        private void deleteEffectOfWithExceptions(string CardID, int creator, List<int> exeptions)
        {
            // deletes the effect of the cardID with creator from all minions 
            Enchantment e = CardDB.getEnchantmentFromCardID(CardID);
            e.creator = creator;
            foreach (Minion m in this.ownMinions)
            {
                if (!exeptions.Contains(m.id))
                {
                    debuff(m, e, true);
                }
            }

            foreach (Minion m in this.enemyMinions)
            {
                if (!exeptions.Contains(m.id))
                {
                    debuff(m, e, false);
                }
            }
        }

        private void addEffectToMinionNoDoubles(Minion m, Enchantment e, bool own)
        {
            foreach (Enchantment es in m.enchantments)
            {
                if (es.CARDID == e.CARDID && es.creator == e.creator) return;
            }
            m.enchantments.Add(e);
            if (e.angrbuff >= 1 || e.hpbuff >= 1)
            {
                minionGetBuffed(m, e.angrbuff, e.hpbuff, own);
            }
            if (e.charge) minionGetCharge(m);
            if (e.divineshild) m.divineshild = true;
            if (e.taunt) m.taunt = true;
            if (e.windfury) minionGetWindfurry(m);
            if (e.imune) m.immune = true;
        }

        private void adjacentBuffer(Minion m, string enchantment, int before, int after, bool own)
        {
            List<Minion> lm = new List<Minion>();
            if (own)
            {
                lm.AddRange(this.ownMinions);
            }
            else
            {
                lm.AddRange(this.enemyMinions);
            }
            List<int> exeptions = new List<int>();
            exeptions.Add(before);
            exeptions.Add(after);
            deleteEffectOfWithExceptions(enchantment, m.entitiyID, exeptions);
            Enchantment e = CardDB.getEnchantmentFromCardID(enchantment);
            e.creator = m.entitiyID;
            e.controllerOfCreator = this.ownController;
            if (before >= 0)
            {
                Minion bef = lm[before];
                addEffectToMinionNoDoubles(bef, e, own);
            }
            if (after < lm.Count)
            {
                Minion bef = lm[after];
                addEffectToMinionNoDoubles(bef, e, own);
            }
        }

        private void adjacentBuffUpdate(bool own)
        {
            int before = -1;
            int after = 1;
            List<Minion> lm = new List<Minion>();
            if (own)
            {
                lm.AddRange(this.ownMinions);
            }
            else
            {
                lm.AddRange(this.enemyMinions);
            }
            foreach (Minion m in lm)
            {
                /*
                if (m.name == "direwolfalpha")
                {
                    string enchantment = "EX1_162o";
                    //help.logg("buffupdate " + m.entitiyID);
                    adjacentBuffer(m, enchantment, before, after, own);
                }
                if (m.name == "flametonguetotem")
                {
                    string enchantment = "EX1_565o";
                    adjacentBuffer(m, enchantment, before, after, own);
                }
                before++;
                after++;
                */
                getNewEffects(m, own, m.id, false);


            }

        }

        private void endEffectsDueToDeath(Minion m, bool own)
        { // minion which grants effect died
            if (m.handcard.card.specialMin == CardDB.specialMinions.raidleader) // if he dies, lower attack of all minions of his side
            {
                deleteEffectOf("CS2_122e", m.entitiyID);
            }

            if (m.handcard.card.specialMin == CardDB.specialMinions.grimscaleoracle)
            {
                deleteEffectOf("EX1_508o", m.entitiyID);
            }

            if (m.handcard.card.specialMin == CardDB.specialMinions.direwolfalpha)
            {
                deleteEffectOf("EX1_162o", m.entitiyID);
            }
            if (m.handcard.card.specialMin == CardDB.specialMinions.murlocwarleader)
            {
                deleteEffectOf("EX1_507e", m.entitiyID);
            }
            if (m.handcard.card.specialMin == CardDB.specialMinions.southseacaptain)
            {
                deleteEffectOf("NEW1_027e", m.entitiyID);
            }
            if (m.handcard.card.specialMin == CardDB.specialMinions.stormwindchampion)
            {
                deleteEffectOf("CS2_222o", m.entitiyID);
            }
            if (m.handcard.card.specialMin == CardDB.specialMinions.timberwolf)
            {
                deleteEffectOf("DS1_175o", m.entitiyID);
            }
            if (m.handcard.card.specialMin == CardDB.specialMinions.leokk)
            {
                deleteEffectOf("NEW1_033o", m.entitiyID);
            }

            //lowering truebaugederalte

            foreach (Minion mnn in this.ownMinions)
            {
                if (mnn.handcard.card.specialMin == CardDB.specialMinions.oldmurkeye && m.handcard.card.race == 14)
                {
                    minionGetBuffed(mnn, -1, 0, true);
                }
            }
            foreach (Minion mnn in this.enemyMinions)
            {
                if (mnn.handcard.card.specialMin == CardDB.specialMinions.oldmurkeye && m.handcard.card.race == 14)
                {
                    minionGetBuffed(mnn, -1, 0, false);
                }
            }

            //no deathrattle, but lowering the weapon
            if (m.handcard.card.specialMin == CardDB.specialMinions.spitefulsmith && m.wounded)// remove weapon changes form hasserfuelleschmiedin
            {
                if (own && this.ownWeaponDurability >= 1)
                {
                    this.ownWeaponAttack -= 2;
                    this.ownheroAngr -= 2;
                }
                if (!own && this.enemyWeaponDurability >= 1)
                {
                    this.enemyWeaponAttack -= 2;
                    this.enemyheroAngr -= 2;
                }
            }
        }

        private void getNewEffects(Minion m, bool own, int placeOfNewMob, bool isSummon)
        {
            bool havekriegshymnenanfuehrerin = false;
            List<Minion> temp = new List<Minion>(this.ownMinions);
            int controller = this.ownController;
            if (!own)
            {
                temp.Clear();
                temp.AddRange(this.enemyMinions);
                controller = 0;
            }
            int ownanz = temp.Count;

            if (own && isSummon && this.ownWeaponName == "swordofjustice")
            {
                minionGetBuffed(m, 1, 1, own);
                this.lowerWeaponDurability(1, true);
            }

            int adjacentplace = 1;
            if (isSummon) adjacentplace = 0;

            foreach (Minion ownm in temp)
            {
                if (ownm.silenced) continue; // silenced minions dont buff

                if (isSummon && ownm.handcard.card.specialMin == CardDB.specialMinions.warsongcommander)
                {
                    havekriegshymnenanfuehrerin = true;
                }

                if (ownm.handcard.card.specialMin == CardDB.specialMinions.raidleader && ownm.entitiyID != m.entitiyID)
                {
                    Enchantment e = CardDB.getEnchantmentFromCardID("CS2_122e");
                    e.creator = ownm.entitiyID;
                    e.controllerOfCreator = controller;
                    addEffectToMinionNoDoubles(m, e, own);

                }
                if (ownm.handcard.card.specialMin == CardDB.specialMinions.leokk && ownm.entitiyID != m.entitiyID)
                {
                    Enchantment e = CardDB.getEnchantmentFromCardID("NEW1_033o");
                    e.creator = ownm.entitiyID;
                    e.controllerOfCreator = controller;
                    addEffectToMinionNoDoubles(m, e, own);

                }
                if (ownm.handcard.card.specialMin == CardDB.specialMinions.stormwindchampion && ownm.entitiyID != m.entitiyID)
                {
                    Enchantment e = CardDB.getEnchantmentFromCardID("CS2_222o");
                    e.creator = ownm.entitiyID;
                    e.controllerOfCreator = controller;
                    addEffectToMinionNoDoubles(m, e, own);
                }
                if (ownm.handcard.card.specialMin == CardDB.specialMinions.grimscaleoracle && m.handcard.card.race == 14 && ownm.entitiyID != m.entitiyID)
                {
                    Enchantment e = CardDB.getEnchantmentFromCardID("EX1_508o");
                    e.creator = ownm.entitiyID;
                    e.controllerOfCreator = controller;
                    addEffectToMinionNoDoubles(m, e, own);
                }
                if (ownm.handcard.card.specialMin == CardDB.specialMinions.murlocwarleader && m.handcard.card.race == 14 && ownm.entitiyID != m.entitiyID)
                {
                    Enchantment e = CardDB.getEnchantmentFromCardID("EX1_507e");
                    e.creator = ownm.entitiyID;
                    e.controllerOfCreator = controller;
                    addEffectToMinionNoDoubles(m, e, own);
                }
                if (ownm.handcard.card.specialMin == CardDB.specialMinions.southseacaptain && m.handcard.card.race == 23)
                {
                    Enchantment e = CardDB.getEnchantmentFromCardID("NEW1_027e");
                    e.creator = ownm.entitiyID;
                    e.controllerOfCreator = controller;
                    addEffectToMinionNoDoubles(m, e, own);
                }


                if (ownm.handcard.card.specialMin == CardDB.specialMinions.timberwolf && (TAG_RACE)m.handcard.card.race == TAG_RACE.PET)
                {
                    Enchantment e = CardDB.getEnchantmentFromCardID("DS1_175o");
                    e.creator = ownm.entitiyID;
                    e.controllerOfCreator = controller;
                    addEffectToMinionNoDoubles(m, e, own);
                }

                if (isSummon && ownm.handcard.card.specialMin == CardDB.specialMinions.tundrarhino && (TAG_RACE)m.handcard.card.race == TAG_RACE.PET)
                {
                    minionGetCharge(m);
                }

                if (ownm.handcard.card.specialMin == CardDB.specialMinions.direwolfalpha)
                {
                    if (ownm.id == placeOfNewMob + 1 || ownm.id == placeOfNewMob - adjacentplace)
                    {
                        Enchantment e = CardDB.getEnchantmentFromCardID("EX1_162o");
                        e.creator = ownm.entitiyID;
                        e.controllerOfCreator = controller;
                        addEffectToMinionNoDoubles(m, e, own);
                    }
                    else
                    {
                        //remove effect!!
                        Enchantment e = CardDB.getEnchantmentFromCardID("EX1_162o");
                        e.creator = ownm.entitiyID;
                        e.controllerOfCreator = controller;
                        debuff(m, e, own);
                    }
                }
                if (ownm.handcard.card.specialMin == CardDB.specialMinions.flametonguetotem)
                {
                    if (ownm.id == placeOfNewMob + 1 || ownm.id == placeOfNewMob - adjacentplace)
                    {
                        Enchantment e = CardDB.getEnchantmentFromCardID("EX1_565o");
                        e.creator = ownm.entitiyID;
                        e.controllerOfCreator = controller;
                        addEffectToMinionNoDoubles(m, e, own);
                    }
                    else
                    {
                        //remove effect!!
                        Enchantment e = CardDB.getEnchantmentFromCardID("EX1_565o");
                        e.creator = ownm.entitiyID;
                        e.controllerOfCreator = controller;
                        debuff(m, e, own);
                    }

                }
            }
            //buff oldmurk
            if (isSummon && m.handcard.card.specialMin == CardDB.specialMinions.oldmurkeye && own)
            {
                int murlocs = 0;
                foreach (Minion mnn in this.ownMinions)
                {
                    if (mnn.handcard.card.race == 14) murlocs++;
                }
                foreach (Minion mnn in this.enemyMinions)
                {
                    if (mnn.handcard.card.race == 14) murlocs++;
                }

                minionGetBuffed(m, murlocs, 0, true);
            }

            // minions that gave ALL minions buffs
            temp.Clear();
            if (own)
            {
                temp.AddRange(this.enemyMinions);
                controller = 0;
            }
            else
            {
                temp.AddRange(this.ownMinions);
                controller = this.ownController;
            }

            foreach (Minion ownm in temp) // the enemy grimmschuppenorakel!
            {
                if (ownm.silenced) continue; // silenced minions dont buff

                if (ownm.handcard.card.specialMin == CardDB.specialMinions.grimscaleoracle && m.handcard.card.race == 14 && ownm.entitiyID != m.entitiyID)
                {
                    Enchantment e = CardDB.getEnchantmentFromCardID("EX1_508o");
                    e.creator = ownm.entitiyID;
                    e.controllerOfCreator = controller;
                    addEffectToMinionNoDoubles(m, e, own);
                }
                if (ownm.handcard.card.specialMin == CardDB.specialMinions.murlocwarleader && m.handcard.card.race == 14 && ownm.entitiyID != m.entitiyID)
                {
                    Enchantment e = CardDB.getEnchantmentFromCardID("EX1_507e");
                    e.creator = ownm.entitiyID;
                    e.controllerOfCreator = controller;
                    addEffectToMinionNoDoubles(m, e, own);
                }

            }

            if (isSummon && havekriegshymnenanfuehrerin && m.Angr <= 3)
            {
                minionGetCharge(m);
            }

        }

        private void deathrattle(Minion m, bool own)
        {

            if (!m.silenced)
            {

                //real deathrattles
                if (m.handcard.card.CardID == "EX1_534")//m.name == "savannenhochmaehne"
                {
                    CardDB.Card c = CardDB.Instance.getCardDataFromID("EX1_534t");//hyena
                    callKid(c, m.id - 1, own);
                    callKid(c, m.id - 1, own);
                }

                if (m.name == "harvestgolem")
                {
                    CardDB.Card c = CardDB.Instance.getCardDataFromID("skele21");//damagedgolem
                    callKid(c, m.id - 1, own);
                }

                if (m.name == "cairnebloodhoof")
                {
                    CardDB.Card c = CardDB.Instance.getCardDataFromID("EX1_110");//bainebloodhoof
                    callKid(c, m.id - 1, own);
                    //penaltity for summon this thing :D (so we dont kill it only to have a new minion)
                    this.evaluatePenality += 5;


                }

                if (m.name == "thebeast")
                {
                    CardDB.Card c = CardDB.Instance.getCardDataFromID("EX1_finkle");//finkleeinhorn
                    callKid(c, m.id - 1, own);

                }

                if (m.name == "lepergnome")
                {
                    attackOrHealHero(2, !own);
                }

                if (m.name == "loothoarder")
                {
                    if (own)
                    {
                        //this.owncarddraw++;
                        this.drawACard("", own);
                    }
                    else
                    {
                        this.drawACard("", own);
                        //this.enemycarddraw++;
                    }
                }




                if (m.name == "bloodmagethalnos")
                {
                    if (own)
                    {
                        //this.owncarddraw++;
                        this.drawACard("", own);
                    }
                    else
                    {
                        //this.enemycarddraw++;
                        this.drawACard("", own);
                    }
                }

                if (m.name == "abomination")
                {
                    if (logging) Helpfunctions.Instance.logg("deathrattle monstrositaet:");
                    attackOrHealHero(2, false);
                    attackOrHealHero(2, true);
                    List<Minion> temp = new List<Minion>(this.ownMinions);
                    foreach (Minion mnn in temp)
                    {
                        minionGetDamagedOrHealed(mnn, 2, 0, true);
                    }
                    temp.Clear();
                    temp.AddRange(this.enemyMinions);
                    foreach (Minion mnn in temp)
                    {
                        minionGetDamagedOrHealed(mnn, 2, 0, false);
                    }

                }


                if (m.name == "tirionfordring")
                {
                    if (own)
                    {
                        CardDB.Card c = CardDB.Instance.getCardDataFromID("EX1_383t");//ashbringer
                        this.equipWeapon(c);
                    }
                    else
                    {
                        this.enemyWeaponAttack = 5;
                        this.enemyWeaponDurability = 3;
                    }
                }

                if (m.name == "sylvanaswindrunner")
                {
                    List<Minion> temp = new List<Minion>();
                    if (own)
                    {
                        List<Minion> temp2 = new List<Minion>(this.enemyMinions);
                        temp2.Sort((a, b) => a.Angr.CompareTo(b.Angr));
                        temp.AddRange(Helpfunctions.TakeList(temp2, Math.Min(2, this.enemyMinions.Count)));
                    }
                    else
                    {
                        List<Minion> temp2 = new List<Minion>(this.ownMinions);
                        temp2.Sort((a, b) => -a.Angr.CompareTo(b.Angr));
                        temp.AddRange(temp2);
                    }
                    if (temp.Count >= 1)
                    {
                        if (own)
                        {
                            Minion target = new Minion();
                            target = temp[0];
                            if (temp.Count >= 2 && target.taunt && !temp[1].taunt) target = temp[1];
                            minionGetControlled(target, true, false);
                        }
                        else
                        {
                            Minion target = new Minion();

                            target = temp[0];
                            foreach (Minion mnn in temp)
                            {
                                if (mnn.Ready)
                                {
                                    target = mnn;
                                    break;
                                }
                            }
                            minionGetControlled(target, false, false);
                        }
                    }
                }

                if (m.handcard.card.specialMin == CardDB.specialMinions.nerubianegg)
                {
                    CardDB.Card c = CardDB.Instance.getCardData("nerubian");//nerubian
                    callKid(c, m.id - 1, own);
                }
                if (m.handcard.card.specialMin == CardDB.specialMinions.dancingswords)
                {
                    this.drawACard("", !own);
                }

                if (m.handcard.card.specialMin == CardDB.specialMinions.voidcaller)
                {
                    if (own)
                    {
                        List<Handmanager.Handcard> temp = new List<Handmanager.Handcard>();
                        foreach (Handmanager.Handcard hc in this.owncards)
                        {
                            if ((TAG_RACE)hc.card.race == TAG_RACE.DEMON)
                            {
                                temp.Add(hc);
                            }
                        }

                        temp.Sort((x, y) => x.card.Attack.CompareTo(y.card.Attack));

                        foreach (Handmanager.Handcard mnn in temp)
                        {
                            callKid(mnn.card, this.ownMinions.Count - 1, true);
                            removeCard(mnn);
                            break;
                        }

                    }
                    else
                    {
                        if (enemyAnzCards + enemycarddraw >= 1)
                        {
                            CardDB.Card c = CardDB.Instance.getCardDataFromID("EX1_301");//felguard
                            callKid(c, this.ownMinions.Count - 1, true);
                        }
                    }
                }

                if (m.handcard.card.specialMin == CardDB.specialMinions.anubarambusher)
                {
                    if (own)
                    {
                        List<Minion> temp = new List<Minion>();

                        if (own)
                        {
                            List<Minion> temp2 = new List<Minion>(this.ownMinions);
                            temp2.Sort((a, b) => -a.Angr.CompareTo(b.Angr));
                            temp.AddRange(temp2);
                        }
                        else
                        {
                            List<Minion> temp2 = new List<Minion>(this.enemyMinions);
                            temp2.Sort((a, b) => a.Angr.CompareTo(b.Angr));
                            temp.AddRange(temp2);
                        }

                        if (temp.Count >= 1)
                        {
                            if (own)
                            {
                                Minion target = new Minion();
                                target = temp[0];
                                if (temp.Count >= 2 && !target.taunt && temp[1].taunt) target = temp[1];
                                minionReturnToHand(target, own, 0);
                            }
                            else
                            {
                                Minion target = new Minion();

                                target = temp[0];
                                if (temp.Count >= 2 && target.taunt && !temp[1].taunt) target = temp[1];
                                minionReturnToHand(target, own, 0);
                            }
                        }
                    }
                }

                if (m.handcard.card.specialMin == CardDB.specialMinions.darkcultist)
                {
                    if (own)
                    {
                        List<Minion> temp = new List<Minion>();

                        if (own)
                        {
                            List<Minion> temp2 = new List<Minion>(this.ownMinions);
                            temp2.Sort((a, b) => -a.Angr.CompareTo(b.Angr));
                            temp.AddRange(temp2);
                        }
                        else
                        {
                            List<Minion> temp2 = new List<Minion>(this.enemyMinions);
                            temp2.Sort((a, b) => a.Angr.CompareTo(b.Angr));
                            temp.AddRange(temp2);
                        }

                        if (temp.Count >= 1)
                        {
                            if (own)
                            {
                                Minion target = new Minion();
                                target = temp[0];
                                if (temp.Count >= 2 && target.taunt && !temp[1].taunt) target = temp[1];
                                minionGetBuffed(target, 0, 3, own);
                            }
                            else
                            {
                                Minion target = new Minion();

                                target = temp[0];
                                if (temp.Count >= 2 && !target.taunt && temp[1].taunt) target = temp[1];
                                minionGetBuffed(target, 0, 3, own);
                            }
                        }
                    }
                }

                if (m.handcard.card.specialMin == CardDB.specialMinions.webspinner)
                {
                    if (own)
                    {
                        this.drawACard("rivercrocolisk", true);
                    }
                    else
                    {
                        this.drawACard("rivercrocolisk", false);
                    }
                }



            }

            //deathrattle enchantments // these can be triggered after an silence (if they are casted after the silence)
            bool geistderahnen = false;
            foreach (Enchantment e in m.enchantments)
            {
                if (e.CARDID == "CS2_038e" && !geistderahnen)
                {
                    //revive minion due to "geist der ahnen"
                    CardDB.Card kid = m.handcard.card;
                    int pos = this.ownMinions.Count - 1;
                    if (!own) pos = this.enemyMinions.Count - 1;
                    callKid(kid, pos, own);
                    geistderahnen = true;
                }
                //Seele des Waldes
                if (e.CARDID == "EX1_158e")
                {
                    //revive minion due to "geist der ahnen"
                    CardDB.Card kid = CardDB.Instance.getCardDataFromID("EX1_158t");//Treant
                    int pos = this.ownMinions.Count - 1;
                    if (!own) pos = this.enemyMinions.Count - 1;
                    callKid(kid, pos, own);
                }
            }


        }

        private void triggerAMinionDied(Minion m, bool own)
        {
            List<Minion> temp = new List<Minion>();
            List<Minion> temp2 = new List<Minion>();
            if (own)
            {
                temp.AddRange(this.ownMinions);
                temp2.AddRange(this.enemyMinions);
            }
            else
            {
                temp.AddRange(this.enemyMinions);
                temp2.AddRange(this.ownMinions);
            }

            foreach (Minion mnn in temp)
            {
                if (mnn.silenced) continue;

                if (mnn.handcard.card.specialMin == CardDB.specialMinions.scavenginghyena && m.handcard.card.race == 20)
                {
                    mnn.Angr += 2; mnn.Hp += 1;
                }
                if (mnn.handcard.card.specialMin == CardDB.specialMinions.flesheatingghoul)
                {
                    mnn.Angr += 1;
                }
                if (mnn.handcard.card.specialMin == CardDB.specialMinions.cultmaster)
                {
                    if (own)
                    {
                        //this.owncarddraw++;
                        this.drawACard("", own);
                    }
                    else
                    {
                        //this.enemycarddraw++;
                        this.drawACard("", own);
                    }
                }
            }

            foreach (Minion mnn in temp2)
            {
                if (mnn.silenced) continue;
                if (mnn.handcard.card.specialMin == CardDB.specialMinions.flesheatingghoul)
                {
                    mnn.Angr += 1;
                }
            }

        }

        private void minionGetDestroyed(Minion m, bool own)
        {

            if (own)
            {
                removeMinionFromList(m, this.ownMinions, true);

            }
            else
            {
                removeMinionFromList(m, this.enemyMinions, false);
            }

        }

        private void minionReturnToHand(Minion m, bool own, int manachange)
        {

            if (own)
            {
                removeMinionFromListNoDeath(m, this.ownMinions, true);
                CardDB.Card c = m.handcard.card;
                Handmanager.Handcard hc = new Handmanager.Handcard();
                hc.card = c;
                hc.position = this.owncards.Count + 1;
                hc.entity = m.entitiyID;
                hc.manacost = c.calculateManaCost(this) + manachange;
                this.owncards.Add(hc);
            }
            else
            {
                removeMinionFromListNoDeath(m, this.enemyMinions, false);
            }

        }

        private void minionTransform(Minion m, CardDB.Card c, bool own)
        {
            Handmanager.Handcard hc = new Handmanager.Handcard(c);
            hc.entity = m.entitiyID;
            Minion tranform = createNewMinion(hc, m.id, own);
            Minion temp = new Minion();
            temp.setMinionTominion(m);
            m.setMinionTominion(tranform);
            m.entitiyID = -2;
            this.endEffectsDueToDeath(temp, own);
            adjacentBuffUpdate(own);
            if (logging) Helpfunctions.Instance.logg("minion got sheep" + m.name + " " + m.Angr);
        }


        private void minionGetSilenced(Minion m, bool own)
        {
            //TODO

            m.taunt = false;
            m.stealth = false;
            m.charge = false;

            m.divineshild = false;
            m.poisonous = false;

            //delete enrage (if minion is silenced the first time)
            if (m.wounded && m.handcard.card.Enrage && !m.silenced)
            {
                deleteWutanfall(m, own);
            }

            //delete enrage (if minion is silenced the first time)

            if (m.frozen && m.numAttacksThisTurn == 0 && !(m.handcard.card.specialMin == CardDB.specialMinions.ancientwatcher || m.name == "ragnarosthefirelord") && !m.playedThisTurn)
            {
                m.Ready = true;
            }


            m.frozen = false;

            if (!m.silenced && (m.handcard.card.specialMin == CardDB.specialMinions.ancientwatcher || m.name == "ragnarosthefirelord") && !m.playedThisTurn && m.numAttacksThisTurn == 0)
            {
                m.Ready = true;
            }

            endEffectsDueToDeath(m, own);//the minion doesnt die, but its effect is ending

            m.enchantments.Clear();

            m.Angr = m.handcard.card.Attack;
            if (m.maxHp < m.handcard.card.Health)//minion has lower maxHp as his card -> heal his hp
            {
                m.Hp += m.handcard.card.Health - m.maxHp; //heal minion

            }
            m.maxHp = m.handcard.card.Health;
            if (m.Hp > m.maxHp) m.Hp = m.maxHp;

            getNewEffects(m, own, m.id, false);// minion get effects of others 

            m.silenced = true;
        }

        private void minionGetControlled(Minion m, bool newOwner, bool canAttack)
        {
            List<Minion> newOwnerList = new List<Minion>();

            if (newOwner) { newOwnerList = new List<Minion>(this.ownMinions); }
            else { newOwnerList.AddRange(this.enemyMinions); }

            if (newOwnerList.Count >= 7) return;

            if (newOwner)
            {
                removeMinionFromListNoDeath(m, this.enemyMinions, !newOwner);
                m.Ready = false;
                m.playedThisTurn = true;
                this.getNewEffects(m, newOwner, newOwnerList.Count, false);

                addMiniontoList(m, this.ownMinions, newOwnerList.Count, newOwner);
                if (m.charge || canAttack)
                {
                    m.charge = false;
                    minionGetCharge(m);
                }

            }
            else
            {
                removeMinionFromListNoDeath(m, this.ownMinions, !newOwner);
                //m.Ready=false;
                addMiniontoList(m, this.enemyMinions, newOwnerList.Count, newOwner);
                //if (m.charge) minionGetCharge(m);
            }

        }


        private void minionGetWindfurry(Minion m)
        {
            if (m.windfury) return;
            m.windfury = true;
            if (m.frozen) return;
            if (!m.playedThisTurn && m.numAttacksThisTurn <= 1)
            {
                m.Ready = true;
            }
            if (!m.charge && m.numAttacksThisTurn <= 1)
            {
                m.Ready = true;
            }
        }

        private void minionGetCharge(Minion m)
        {
            if (m.charge) return;
            m.charge = true;
            if (m.playedThisTurn && (m.numAttacksThisTurn == 0 || (m.numAttacksThisTurn == 1 && m.windfury)))
            {
                m.Ready = true;
            }
        }

        private void minionGetReady(Minion m) // minion get ready due to attack-buff
        {
            if (!m.silenced && (m.handcard.card.specialMin == CardDB.specialMinions.ancientwatcher || m.name == "ragnarosthefirelord")) return;

            if (!m.playedThisTurn && !m.frozen && (m.numAttacksThisTurn == 0 || (m.numAttacksThisTurn == 1 && m.windfury)))
            {
                m.Ready = true;
            }
        }

        private void minionGetBuffed(Minion m, int attackbuff, int hpbuff, bool own)
        {
            if (m.Angr == 0 && attackbuff >= 1) minionGetReady(m);

            m.Angr = Math.Max(0, m.Angr + attackbuff);

            if (hpbuff >= 1)
            {
                m.Hp = m.Hp + hpbuff;
                m.maxHp = m.maxHp + hpbuff;
            }
            else
            {
                //debuffing hp, lower only maxhp (unless maxhp < hp)
                m.maxHp = m.maxHp + hpbuff;
                if (m.maxHp < m.Hp)
                {
                    m.Hp = m.maxHp;
                }
            }


            if (m.maxHp == m.Hp)
            {
                m.wounded = false;
            }
            else
            {
                m.wounded = true;
            }

            if (m.name == "lightspawn" && !m.silenced)
            {
                m.Angr = m.Hp;
            }

            if (m.Hp <= 0)
            {
                if (own)
                {
                    this.removeMinionFromList(m, this.ownMinions, true);
                    if (logging) Helpfunctions.Instance.logg("own " + m.name + " died");
                }
                else
                {
                    this.removeMinionFromList(m, this.enemyMinions, false);
                    if (logging) Helpfunctions.Instance.logg("enemy " + m.name + " died");
                }
            }
        }


        private void deleteWutanfall(Minion m, bool own)
        {
            if (m.name == "angrychicken")
            {
                minionGetBuffed(m, -5, 0, own);
            }
            if (m.name == "amaniberserker")
            {
                minionGetBuffed(m, -3, 0, own);
            }
            if (m.name == "taurenwarrior")
            {
                minionGetBuffed(m, -3, 0, own);
            }
            if (m.name == "grommashhellscream")
            {
                minionGetBuffed(m, -6, 0, own);
            }
            if (m.name == "ragingworgen")
            {
                minionGetBuffed(m, -1, 0, own);
                minionGetWindfurry(m);
            }
            if (m.name == "spitefulsmith")
            {
                if (own && this.ownWeaponDurability >= 1)
                {
                    this.ownWeaponAttack -= 2;
                    this.ownheroAngr -= 2;
                }
                if (!own && this.enemyWeaponDurability >= 1)
                {
                    this.enemyWeaponAttack -= 2;
                    this.enemyheroAngr -= 2;
                }
            }
        }

        private void wutanfall(Minion m, bool woundedBefore, bool own) // = enrange effects
        {
            if (!m.handcard.card.Enrage) return; // if minion has no enrange, do nothing
            if (woundedBefore == m.wounded || m.silenced) return; // if he was wounded, and still is (or was unwounded) do nothing

            if (m.wounded && m.Hp >= 1) //is wounded, wasnt wounded before, grant wutanfall
            {
                if (m.name == "angrychicken")
                {
                    minionGetBuffed(m, 5, 0, own);
                }
                if (m.name == "amaniberserker")
                {
                    minionGetBuffed(m, 3, 0, own);
                }
                if (m.name == "taurenwarrior")
                {
                    minionGetBuffed(m, 3, 0, own);
                }
                if (m.name == "grommashhellscream")
                {
                    minionGetBuffed(m, 6, 0, own);
                }
                if (m.name == "ragingworgen")
                {
                    minionGetBuffed(m, 1, 0, own);
                    minionGetWindfurry(m);
                }
                if (m.name == "spitefulsmith")
                {
                    if (own && this.ownWeaponDurability >= 1)
                    {
                        this.ownWeaponAttack += 2;
                        this.ownheroAngr += 2;
                    }
                    if (!own && this.enemyWeaponDurability >= 1)
                    {
                        this.enemyWeaponAttack += 2;
                        this.enemyheroAngr += 2;
                    }
                }

            }

            if (!m.wounded) // reverse buffs
            {
                deleteWutanfall(m, own);
            }
        }

        private void triggerAHeroGetHealed(bool own)
        {
            foreach (Minion mnn in this.ownMinions)
            {
                if (mnn.silenced) continue;
                if (mnn.handcard.card.specialMin == CardDB.specialMinions.lightwarden)
                {
                    minionGetBuffed(mnn, 2, 0, true);
                }
            }
            foreach (Minion mnn in this.enemyMinions)
            {
                if (mnn.silenced) continue;
                if (mnn.handcard.card.specialMin == CardDB.specialMinions.lightwarden)
                {
                    minionGetBuffed(mnn, 2, 0, false);
                }
            }
        }

        private void triggerAMinionGetHealed(Minion m, bool own)
        {
            foreach (Minion mnn in this.ownMinions)
            {
                if (mnn.silenced) continue;
                if (mnn.handcard.card.specialMin == CardDB.specialMinions.northshirecleric)
                {
                    //this.owncarddraw++;
                    this.drawACard("", true);
                }
                if (mnn.handcard.card.specialMin == CardDB.specialMinions.lightwarden)
                {
                    minionGetBuffed(mnn, 2, 0, true);
                }
            }
            foreach (Minion mnn in this.enemyMinions)
            {
                if (mnn.silenced) continue;
                if (mnn.handcard.card.specialMin == CardDB.specialMinions.northshirecleric)
                {
                    //this.enemycarddraw++;
                    this.drawACard("", false);
                }
                if (mnn.handcard.card.specialMin == CardDB.specialMinions.lightwarden)
                {
                    minionGetBuffed(mnn, 2, 0, false);
                }
            }

        }

        private void triggerAMinionGetDamage(Minion m, bool own)
        {
            //minion take dmg
            if (m.handcard.card.specialMin == CardDB.specialMinions.acolyteofpain && !m.silenced)
            {
                if (own)
                {
                    //this.owncarddraw++;
                    this.drawACard("", own);
                }
                else
                {
                    //this.enemycarddraw++;
                    this.drawACard("", own);
                }
            }
            if (m.handcard.card.specialMin == CardDB.specialMinions.gurubashiberserker && !m.silenced)
            {
                minionGetBuffed(m, 3, 0, own);
            }
            foreach (Minion mnn in this.ownMinions)
            {
                if (mnn.silenced) continue;
                if (mnn.handcard.card.specialMin == CardDB.specialMinions.frothingberserker)
                {
                    mnn.Angr++;
                }
                if (own)
                {
                    if (mnn.handcard.card.specialMin == CardDB.specialMinions.armorsmith)
                    {
                        this.ownHeroDefence++;
                    }
                }
            }
            foreach (Minion mnn in this.enemyMinions)
            {
                if (mnn.silenced) continue;
                if (mnn.handcard.card.specialMin == CardDB.specialMinions.frothingberserker)
                {
                    mnn.Angr++;
                }
                if (!own)
                {
                    if (mnn.handcard.card.specialMin == CardDB.specialMinions.armorsmith)
                    {
                        this.enemyHeroDefence++;
                    }
                }
            }
        }

        /*private void minionGetDamagedOrHealed(Minion m, int damages, int heals, bool own)
        {
            minionGetDamagedOrHealed(m, damages, heals, own, false);
        }*/

        private void minionGetDamagedOrHealed(Minion m, int damages, int heals, bool own, bool dontCalcLostDmg = false, bool isMinionattack = false)
        {
            int damage = damages;
            int heal = heals;

            bool woundedbefore = m.wounded;
            if (heal < 0) // heal was shifted in damage
            {
                damage = -1 * heal;
                heal = 0;
            }

            if (damage >= 1 && m.divineshild)
            {
                m.divineshild = false;
                if (!own && !dontCalcLostDmg)
                {
                    if (isMinionattack)
                    {
                        this.lostDamage += damage;
                    }
                    else
                    {
                        this.lostDamage += damage * damage;
                    }
                }
                return;
            }

            if (m.cantLowerHPbelowONE && damage >= 1 && damage >= m.Hp) damage = m.Hp - 1;

            if (!own && !dontCalcLostDmg && m.Hp < damage)
            {
                if (isMinionattack)
                {
                    this.lostDamage += (damage - m.Hp);
                }
                else
                {
                    this.lostDamage += (damage - m.Hp) * (damage - m.Hp);
                }
            }

            int hpcopy = m.Hp;

            if (damage >= 1)
            {
                m.Hp = m.Hp - damage;
            }

            if (heal >= 1)
            {
                if (own && !dontCalcLostDmg && heal <= 999 && m.Hp + heal > m.maxHp) this.lostHeal += m.Hp + heal - m.maxHp;

                m.Hp = m.Hp + Math.Min(heal, m.maxHp - m.Hp);
            }



            if (m.Hp > hpcopy)
            {
                //minionWasHealed
                triggerAMinionGetHealed(m, own);
            }

            if (m.Hp < hpcopy)
            {
                triggerAMinionGetDamage(m, own);
            }

            if (m.maxHp == m.Hp)
            {
                m.wounded = false;
            }
            else
            {
                m.wounded = true;
            }

            this.wutanfall(m, woundedbefore, own);

            if (m.name == "lightspawn" && !m.silenced)
            {
                m.Angr = m.Hp;
            }


            if (m.Hp <= 0)
            {
                if (own)
                {
                    this.removeMinionFromList(m, this.ownMinions, true);
                    if (logging) Helpfunctions.Instance.logg("own " + m.name + " died");
                }
                else
                {
                    this.removeMinionFromList(m, this.enemyMinions, false);
                    if (logging) Helpfunctions.Instance.logg("enemy " + m.name + " died");
                }
            }
        }

        private void copyMinion(Minion target, Minion source)
        {
            target.name = source.name;
            target.Angr = source.Angr;
            target.handcard.card = CardDB.Instance.getCardDataFromID(source.handcard.card.CardID);
            target.charge = source.charge;
            target.divineshild = source.divineshild;
            target.exhausted = source.exhausted;
            target.frozen = source.frozen;
            target.Hp = source.Hp;
            target.immune = source.immune;
            target.maxHp = source.maxHp;
            target.playedThisTurn = source.playedThisTurn;
            target.poisonous = source.poisonous;
            target.silenced = source.silenced;
            target.stealth = source.stealth;
            target.taunt = source.taunt;
            target.windfury = source.windfury;
            target.wounded = source.wounded;
            target.Ready = false;
            if (target.charge) target.Ready = true;
            foreach (Enchantment e in source.enchantments)
            {
                Enchantment ne = CardDB.getEnchantmentFromCardID(e.CARDID);
                target.enchantments.Add(ne);
            }
        }

        private void removeMinionFromListNoDeath(Minion m, List<Minion> l, bool own)
        {
            l.Remove(m);
            int i = 0;
            foreach (Minion mnn in l)
            {
                mnn.id = i;
                mnn.zonepos = i + 1;
                i++;
            }
            this.endEffectsDueToDeath(m, own);
            adjacentBuffUpdate(own);
        }

        private void removeMinionFromList(Minion m, List<Minion> l, bool own)
        {
            l.Remove(m);
            int i = 0;
            foreach (Minion mnn in l)
            {
                mnn.id = i;
                mnn.zonepos = i + 1;
                i++;
            }

            this.endEffectsDueToDeath(m, own);
            this.deathrattle(m, own);
            if (own)
            {
                if (this.ownBaronRivendare) this.deathrattle(m, own);
            }
            else
            {
                if (this.enemyBaronRivendare) this.deathrattle(m, own);
            }
            this.triggerAMinionDied(m, own);
            adjacentBuffUpdate(own);

        }

        private void attack(int attacker, int target, bool dontcount)
        {
            Minion m = new Minion();
            bool attackOwn = true;
            if (attacker < 10)
            {
                m = this.ownMinions[attacker];
                attackOwn = true;
            }
            if (attacker >= 10 && attacker < 20)
            {
                m = this.enemyMinions[attacker - 10];
                attackOwn = false;
            }

            if (!dontcount)
            {
                m.numAttacksThisTurn++;
                if (m.windfury && m.numAttacksThisTurn == 2)
                {
                    m.Ready = false;
                }
                if (!m.windfury)
                {
                    m.Ready = false;
                }
            }

            if (logging) Helpfunctions.Instance.logg(".attck with" + m.name + " A " + m.Angr + " H " + m.Hp);

            if (target == 200)//target is hero
            {
                attackOrHealHero(m.Angr, false);
                return;
            }

            bool enemyOwn = false;
            Minion enemy = new Minion();
            if (target < 10)
            {
                enemy = this.ownMinions[target];
                enemyOwn = true;
            }

            if (target >= 10 && target < 20)
            {
                enemy = this.enemyMinions[target - 10];
                enemyOwn = false;
            }




            int ownAttack = m.Angr;
            int enemyAttack = enemy.Angr;
            // defender take damage
            if (m.handcard.card.poisionous)
            {
                minionGetDestroyed(enemy, enemyOwn);
            }
            else
            {
                int oldHP = enemy.Hp;
                minionGetDamagedOrHealed(enemy, ownAttack, 0, enemyOwn, false, true);
                if (!m.silenced && oldHP > enemy.Hp && m.handcard.card.specialMin == CardDB.specialMinions.waterelemental) enemy.frozen = true;
            }


            //attacker take damage
            if (!m.immune && !dontcount)
            {
                if (enemy.handcard.card.poisionous)
                {
                    minionGetDestroyed(m, attackOwn);
                }
                else
                {
                    int oldHP = m.Hp;
                    minionGetDamagedOrHealed(m, enemyAttack, 0, attackOwn, false, true);
                    if (!enemy.silenced && oldHP > m.Hp && enemy.handcard.card.specialMin == CardDB.specialMinions.waterelemental) m.frozen = true;
                }
            }
        }

        public void attackWithMinion(Minion ownMinion, int target, int targetEntity, int penality)
        {
            this.evaluatePenality += penality;
            Action a = new Action();
            a.minionplay = true;
            a.owntarget = ownMinion.id;
            a.ownEntitiy = ownMinion.entitiyID;
            a.enemytarget = target;
            a.enemyEntitiy = targetEntity;
            a.numEnemysBeforePlayed = this.enemyMinions.Count;
            a.comboBeforePlayed = (this.cardsPlayedThisTurn >= 1) ? true : false;
            this.playactions.Add(a);
            if (logging) Helpfunctions.Instance.logg("attck with" + ownMinion.name + " " + ownMinion.id + " trgt " + target + " A " + ownMinion.Angr + " H " + ownMinion.Hp);


            attack(ownMinion.id, target, false);

            //draw a card if the minion has enchantment from: Segen der weisheit 
            int segenderweisheitAnz = 0;
            foreach (Enchantment e in ownMinion.enchantments)
            {
                if (e.CARDID == "EX1_363e2" && e.controllerOfCreator == this.ownController)
                {
                    segenderweisheitAnz++;
                }
            }
            this.owncarddraw += segenderweisheitAnz;
            for (int i = 0; i < segenderweisheitAnz; i++)
            {
                this.drawACard("", true);
            }
        }

        public void ENEMYattackWithMinion(Minion ownMinion, int target, int targetEntity)
        {

            if (logging) Helpfunctions.Instance.logg("ennemy attck with" + ownMinion.name + " " + ownMinion.id + " trgt " + target + " A " + ownMinion.Angr + " H " + ownMinion.Hp);
            attack(ownMinion.id + 10, target, false);
            //draw a card if the minion has enchantment from: Segen der weisheit 
            int segenderweisheitAnz = 0;
            foreach (Enchantment e in ownMinion.enchantments)
            {
                if (e.CARDID == "EX1_363e2" && e.controllerOfCreator != this.ownController)
                {
                    segenderweisheitAnz++;
                }
            }
            this.enemycarddraw += segenderweisheitAnz;
            for (int i = 0; i < segenderweisheitAnz; i++)
            {
                this.drawACard("", false);
            }
        }

        private void addMiniontoList(Minion m, List<Minion> l, int pos, bool own)
        {
            List<Minion> newmins = new List<Minion>(l);
            l.Clear();

            int i = 0;
            foreach (Minion mnn in newmins)
            {

                if (pos == i)
                {
                    m.id = i;
                    m.zonepos = i + 1;
                    l.Add(m);
                    i++;
                }
                mnn.id = i;
                mnn.zonepos = i + 1;
                l.Add(mnn);
                i++;
            }
            // maybe he is last mob
            if (pos == i)
            {
                m.id = i;
                m.zonepos = i + 1;
                l.Add(m);
                i++;
            }
            adjacentBuffUpdate(own);
            triggerPlayedAMinion(m.handcard, own);

        }

        private Minion createNewMinion(Handmanager.Handcard hc, int placeOfNewMob, bool own)
        {
            Minion m = new Minion();
            m.handcard = new Handmanager.Handcard(hc);
            m.entitiyID = hc.entity;
            m.Posix = 0;
            m.Posiy = 0;
            m.Angr = hc.card.Attack;
            m.Hp = hc.card.Health;
            m.maxHp = hc.card.Health;
            m.name = hc.card.name;
            m.playedThisTurn = true;
            m.numAttacksThisTurn = 0;
            m.id = placeOfNewMob;
            m.zonepos = placeOfNewMob + 1;


            if (hc.card.windfury) m.windfury = true;
            if (hc.card.tank) m.taunt = true;
            if (hc.card.Charge)
            {
                m.Ready = true;
                m.charge = true;
            }
            if (hc.card.Shield) m.divineshild = true;
            if (hc.card.poisionous) m.poisonous = true;

            if (hc.card.Stealth) m.stealth = true;

            if (m.name == "lightspawn" && !m.silenced)
            {
                m.Angr = m.Hp;
            }

            this.getNewEffects(m, own, placeOfNewMob, true);

            return m;
        }

        private void doBattleCryWithTargeting(Minion c, int target, int choice)
        {

            //target is the target AFTER spawning mobs
            int attackbuff = 0;
            int hpbuff = 0;
            int heal = 0;
            int damage = 0;
            bool spott = false;
            bool divineshild = false;
            bool windfury = false;
            bool silence = false;
            bool destroy = false;
            bool frozen = false;
            bool stealth = false;
            bool backtohand = false;
            int backtoHandManaChange = 0;

            bool own = true;

            if (target >= 10 && target < 20)
            {
                own = false;
            }
            Minion m = new Minion();
            if (target < 10)
            {
                m = this.ownMinions[target];
            }
            if (target >= 10 && target < 20)
            {
                m = this.enemyMinions[target - 10];
            }


            if (c.name == "ancientoflore")
            {
                if (choice == 2)
                {
                    heal = 5;
                }
            }


            if (c.name == "keeperofthegrove")
            {
                if (choice == 1)
                {
                    damage = 2;
                }

                if (choice == 2)
                {
                    silence = true;
                }
            }

            if (c.name == "crazedalchemist")
            {
                if (target < 10)
                {
                    bool woundedbef = m.wounded;
                    int temp = m.Angr;
                    m.Angr = m.Hp;
                    m.Hp = temp;
                    m.maxHp = temp;
                    m.wounded = false;
                    wutanfall(m, woundedbef, true);
                    if (m.Hp <= 0) minionGetDestroyed(m, true);
                }

                if (target >= 10 && target < 20)
                {
                    bool woundedbef = m.wounded;
                    int temp = m.Angr;
                    m.Angr = m.Hp;
                    m.Hp = temp;
                    m.maxHp = temp;
                    m.wounded = false;
                    wutanfall(m, woundedbef, false);
                    if (m.Hp <= 0) minionGetDestroyed(m, false);
                }

            }

            if (c.name == "si7agent" && this.cardsPlayedThisTurn >= 1)
            {
                damage = 2;
            }
            if (c.name == "kidnapper" && this.cardsPlayedThisTurn >= 1)
            {
                backtohand = true;
                backtoHandManaChange = 0;
            }
            if (c.name == "masterofdisguise")
            {
                stealth = true;
            }

            if (c.name == "cabalshadowpriest")
            {
                minionGetControlled(m, true, false);
            }


            if (c.name == "ironbeakowl" || c.name == "spellbreaker") //eisenschnabeleule, zauberbrecher
            {
                silence = true;
            }

            if (c.name == "shatteredsuncleric")
            {
                attackbuff = 1;
                hpbuff = 1;
            }

            if (c.name == "ancientbrewmaster")
            {
                backtohand = true;
                backtoHandManaChange = 0;
            }
            if (c.name == "youthfulbrewmaster")
            {
                backtohand = true;
                backtoHandManaChange = 0;
            }

            if (c.name == "darkirondwarf")
            {
                //attackbuff = 2;
                Enchantment e = CardDB.getEnchantmentFromCardID("EX1_046e");
                e.creator = c.entitiyID;
                e.controllerOfCreator = this.ownController;
                addEffectToMinionNoDoubles(m, e, own);
            }

            if (c.name == "hungrycrab")
            {
                destroy = true;
                /*Enchantment e = CardDB.getEnchantmentFromCardID("NEW1_017e");
                e.creator = c.entitiyID;
                e.controllerOfCreator = this.ownController;
                addEffectToMinionNoDoubles(c, e, true);//buff own hungrige krabbe*/
                minionGetBuffed(c, 2, 2, true);
            }

            if (c.name == "abusivesergeant")
            {
                Enchantment e = CardDB.getEnchantmentFromCardID("CS2_188o");
                e.creator = c.entitiyID;
                e.controllerOfCreator = this.ownController;
                addEffectToMinionNoDoubles(m, e, own);
            }
            if (c.name == "crueltaskmaster")
            {
                attackbuff = 2;
                damage = 1;
            }

            if (c.name == "frostelemental")
            {
                frozen = true;
            }

            if (c.name == "elvenarcher")
            {
                damage = 1;
            }
            if (c.name == "voodoodoctor")
            {
                if (this.auchenaiseelenpriesterin)
                { damage = 2; }
                else { heal = 2; }
            }
            if (c.name == "templeenforcer")
            {
                hpbuff = 3;
            }
            if (c.name == "ironforgerifleman")
            {
                damage = 1;
            }
            if (c.name == "stormpikecommando")
            {
                damage = 2;
            }
            if (c.name == "houndmaster")
            {
                attackbuff = 2;
                hpbuff = 2;
                spott = true;
            }

            if (c.name == "aldorpeacekeeper")
            {
                attackbuff = 1 - m.Angr;
            }

            if (c.name == "theblackknight")
            {
                destroy = true;
            }

            if (c.name == "argentprotector")
            {
                divineshild = true; // Grants NO buff
            }

            if (c.name == "windspeaker")
            {
                windfury = true;
            }
            if (c.name == "fireelemental")
            {
                damage = 3;
            }
            if (c.name == "earthenringfarseer")
            {
                if (this.auchenaiseelenpriesterin)
                { damage = 3; }
                else { heal = 3; }
            }
            if (c.name == "biggamehunter")
            {
                destroy = true;
            }

            if (c.name == "alexstrasza")
            {
                if (target == 100)
                {
                    this.ownHeroHp = 15;

                }
                if (target == 200)
                {
                    this.enemyHeroHp = 15;
                }
            }

            if (c.name == "facelessmanipulator")
            {//todo, test this :D

                copyMinion(c, m);
            }

            //make effect on target
            //ownminion
            if (target < 10)
            {
                if (attackbuff != 0 || hpbuff != 0)
                {
                    minionGetBuffed(m, attackbuff, hpbuff, true);
                }
                if (damage != 0 || heal != 0)
                {
                    minionGetDamagedOrHealed(m, damage, heal, true);
                }
                if (spott) m.taunt = true;
                if (windfury) minionGetWindfurry(m);
                if (divineshild) m.divineshild = true;
                if (destroy) minionGetDestroyed(m, true);
                if (frozen) m.frozen = true;
                if (stealth) m.stealth = true;
                if (backtohand) minionReturnToHand(m, true, backtoHandManaChange);
                if (silence) minionGetSilenced(m, true);

            }
            //enemyminion
            if (target >= 10 && target < 20)
            {
                if (attackbuff != 0 || hpbuff != 0)
                {
                    minionGetBuffed(m, attackbuff, hpbuff, false);
                }
                if (damage != 0 || heal != 0)
                {
                    minionGetDamagedOrHealed(m, damage, heal, false);
                }
                if (spott) m.taunt = true;
                if (windfury) minionGetWindfurry(m);
                if (divineshild) m.divineshild = true;
                if (destroy) minionGetDestroyed(m, false);
                if (frozen) m.frozen = true;
                if (stealth) m.stealth = true;
                if (backtohand) minionReturnToHand(m, false, backtoHandManaChange);
                if (silence) minionGetSilenced(m, false);
            }
            if (target == 100)
            {
                if (frozen) this.ownHeroFrozen = true;
                if (damage >= 1) attackOrHealHero(damage, true);
                if (heal >= 1) attackOrHealHero(-heal, true);
            }
            if (target == 200)
            {
                if (frozen) this.enemyHeroFrozen = true;
                if (damage >= 1) attackOrHealHero(damage, false);
                if (heal >= 1) attackOrHealHero(-heal, false);
            }

        }

        private void doBattleCryWithoutTargeting(Minion c, int position, bool own, int choice)
        {
            //only nontargetable battlecrys!

            //druid choices

            //urtum des krieges:
            if (c.name == "ancientofwar")
            {
                if (choice == 1)
                {
                    minionGetBuffed(c, 5, 0, true);
                }
                if (choice == 2)
                {
                    minionGetBuffed(c, 0, 5, true);
                    c.taunt = true;
                }
            }

            if (c.name == "ancientoflore")
            {
                if (choice == 1)
                {
                    //this.owncarddraw += 2;
                    this.drawACard("", own);
                    this.drawACard("", own);
                }

            }

            if (c.name == "druidoftheclaw")
            {
                if (choice == 1)
                {
                    minionGetCharge(c);
                }
                if (choice == 2)
                {
                    minionGetBuffed(c, 0, 2, true);
                    c.taunt = true;
                }
            }

            if (c.name == "cenarius")
            {
                if (choice == 1)
                {
                    foreach (Minion m in this.ownMinions)
                    {
                        minionGetBuffed(m, 2, 2, true);
                    }
                }
                //choice 2 = spawn 2 kids
            }

            //normal ones

            if (c.name == "mindcontroltech")
            {
                if (this.enemyMinions.Count >= 4)
                {
                    List<Minion> temp = new List<Minion>();

                    List<Minion> temp2 = new List<Minion>(this.enemyMinions);
                    temp2.Sort((a, b) => a.Angr.CompareTo(b.Angr));//we take the weekest

                    temp.AddRange(Helpfunctions.TakeList(temp2, 2));
                    Minion target = new Minion();
                    target = temp[0];
                    if (target.taunt && !temp[1].taunt) target = temp[1];
                    minionGetControlled(target, true, false);

                }
            }

            if (c.name == "felguard")
            {
                this.ownMaxMana--;
            }
            if (c.name == "arcanegolem")
            {
                this.enemyMaxMana++;
            }

            if (c.name == "edwinvancleef" && this.cardsPlayedThisTurn >= 1)
            {
                minionGetBuffed(c, this.cardsPlayedThisTurn * 2, this.cardsPlayedThisTurn * 2, own);
            }

            if (c.name == "doomguard")
            {
                this.owncarddraw -= Math.Min(2, this.owncards.Count);
                this.owncards.RemoveRange(0, Math.Min(2, this.owncards.Count));
            }

            if (c.name == "succubus")
            {
                this.owncarddraw -= Math.Min(1, this.owncards.Count);
                this.owncards.RemoveRange(0, Math.Min(1, this.owncards.Count));
            }

            if (c.name == "lordjaraxxus")
            {
                this.ownHeroAblility = CardDB.Instance.getCardDataFromID("EX1_tk33");
                this.ownHeroName = HeroEnum.lordjaraxxus;
                this.ownHeroHp = c.Hp;
            }

            if (c.name == "flameimp")
            {
                attackOrHealHero(3, own);
            }
            if (c.name == "pitlord")
            {
                attackOrHealHero(5, own);
            }

            if (c.name == "voidterror")
            {
                List<Minion> temp = new List<Minion>();
                if (own)
                {
                    temp.AddRange(this.ownMinions);
                }
                else
                {
                    temp.AddRange(this.enemyMinions);
                }

                int angr = 0;
                int hp = 0;
                foreach (Minion m in temp)
                {
                    if (m.id == position || m.id == position - 1)
                    {
                        angr += m.Angr;
                        hp += m.Hp;
                    }
                }
                foreach (Minion m in temp)
                {
                    if (m.id == position || m.id == position - 1)
                    {
                        minionGetDestroyed(m, own);
                    }
                }
                minionGetBuffed(c, angr, hp, own);

            }

            if (c.name == "frostwolfwarlord")
            {
                minionGetBuffed(c, this.ownMinions.Count, this.ownMinions.Count, own);
            }
            if (c.name == "bloodsailraider")
            {
                c.Angr += this.ownWeaponAttack;
            }

            if (c.name == "southseadeckhand" && this.ownWeaponDurability >= 1)
            {
                minionGetCharge(c);
            }



            if (c.name == "bloodknight")
            {
                int shilds = 0;
                foreach (Minion m in this.ownMinions)
                {
                    if (m.divineshild)
                    {
                        m.divineshild = false;
                        shilds++;
                    }
                }
                foreach (Minion m in this.enemyMinions)
                {
                    if (m.divineshild)
                    {
                        m.divineshild = false;
                        shilds++;
                    }
                }
                minionGetBuffed(c, 3 * shilds, 3 * shilds, own);

            }

            if (c.name == "kingmukla")
            {
                this.enemycarddraw += 2;
            }

            if (c.name == "coldlightoracle")
            {
                //this.enemycarddraw += 2;
                //this.owncarddraw += 2;
                this.drawACard("", true);
                this.drawACard("", true);
                this.drawACard("", false);
                this.drawACard("", false);
            }

            if (c.name == "arathiweaponsmith")
            {
                CardDB.Card wcard = CardDB.Instance.getCardDataFromID("EX1_398t");//battleaxe
                this.equipWeapon(wcard);


            }
            if (c.name == "bloodsailcorsair")
            {
                this.lowerWeaponDurability(1, false);
            }

            if (c.name == "acidicswampooze")
            {
                this.lowerWeaponDurability(1000, false);
            }
            if (c.name == "noviceengineer")
            {
                //this.owncarddraw++;
                drawACard("", true);
            }
            if (c.name == "gnomishinventor")
            {
                //this.owncarddraw++;
                drawACard("", true);
            }

            if (c.name == "darkscalehealer")
            {
                List<Minion> temp = new List<Minion>(this.ownMinions);
                foreach (Minion m in temp)
                {

                    if (this.auchenaiseelenpriesterin)
                    { minionGetDamagedOrHealed(m, 2, 0, true); }
                    else { minionGetDamagedOrHealed(m, 0, 2, true); }

                }
                if (this.auchenaiseelenpriesterin)
                { attackOrHealHero(2, true); }
                else { attackOrHealHero(-2, true); }

            }
            if (c.name == "nightblade")
            {
                attackOrHealHero(3, !own);
            }

            if (c.name == "twilightdrake")
            {
                minionGetBuffed(c, 0, this.owncards.Count, true);
            }

            if (c.name == "azuredrake")
            {
                //this.owncarddraw++;
                drawACard("", true);
            }

            if (c.name == "harrisonjones")
            {
                this.enemyWeaponAttack = 0;
                //this.owncarddraw += enemyWeaponDurability;
                for (int i = 0; i < enemyWeaponDurability; i++)
                {
                    drawACard("", true);
                }
                this.enemyWeaponDurability = 0;
            }

            if (c.name == "guardianofkings")
            {
                attackOrHealHero(-6, true);
            }

            if (c.name == "captaingreenskin")
            {
                if (this.ownWeaponName != "")
                {
                    this.ownheroAngr += 1;
                    this.ownWeaponAttack++;
                    this.ownWeaponDurability++;
                }
            }

            if (c.name == "priestessofelune")
            {
                attackOrHealHero(-4, true);
            }
            if (c.name == "injuredblademaster")
            {
                minionGetDamagedOrHealed(c, 4, 0, true);
            }

            if (c.name == "dreadinfernal")
            {
                List<Minion> temp = new List<Minion>(this.ownMinions);
                foreach (Minion m in temp)
                {
                    minionGetDamagedOrHealed(m, 1, 0, true);
                }
                temp.Clear();
                temp.AddRange(this.enemyMinions);
                foreach (Minion m in temp)
                {
                    minionGetDamagedOrHealed(m, 1, 0, false);
                }
                attackOrHealHero(1, false);
                attackOrHealHero(1, true);
            }

            if (c.name == "tundrarhino")
            {
                minionGetCharge(c);
                List<Minion> temp = new List<Minion>(this.ownMinions);
                foreach (Minion m in temp)
                {
                    if ((TAG_RACE)m.handcard.card.race == TAG_RACE.PET)
                    {
                        minionGetCharge(m);
                    }
                }
            }

            if (c.name == "stampedingkodo")
            {
                List<Minion> temp = new List<Minion>();
                List<Minion> temp2 = new List<Minion>(this.enemyMinions);
                temp2.Sort((a, b) => a.Hp.CompareTo(b.Hp));//destroys the weakest
                temp.AddRange(temp2);
                foreach (Minion enemy in temp)
                {
                    if (enemy.Angr <= 2)
                    {
                        minionGetDestroyed(enemy, false);
                        break;
                    }
                }
            }

            if (c.name == "sunfuryprotector")
            {
                List<Minion> temp = new List<Minion>(this.ownMinions);
                foreach (Minion m in temp)
                {
                    if (m.id == position - 1 || m.id == position)
                    {
                        m.taunt = true;
                    }
                }
            }

            if (c.name == "ancientmage")
            {
                List<Minion> temp = new List<Minion>(this.ownMinions);
                foreach (Minion m in temp)
                {
                    if (m.id == position - 1 || m.id == position)
                    {
                        m.handcard.card.spellpowervalue++;
                    }
                }
            }

            if (c.name == "defenderofargus")
            {
                List<Minion> temp = new List<Minion>(this.ownMinions);
                foreach (Minion m in temp)
                {
                    if (m.id == position - 1 || m.id == position)//position and position -1 because its not placed jet
                    {
                        Enchantment e = CardDB.getEnchantmentFromCardID("EX1_093e");
                        e.creator = c.entitiyID;
                        e.controllerOfCreator = this.ownController;
                        addEffectToMinionNoDoubles(m, e, own);
                    }
                }
            }

            if (c.name == "coldlightseer")
            {
                List<Minion> temp = new List<Minion>(this.ownMinions);
                foreach (Minion m in temp)
                {
                    if ((TAG_RACE)m.handcard.card.race == TAG_RACE.MURLOC)
                    {
                        minionGetBuffed(m, 0, 2, true);
                    }
                }
                temp.Clear();
                temp.AddRange(this.enemyMinions);
                foreach (Minion m in temp)
                {
                    if ((TAG_RACE)m.handcard.card.race == TAG_RACE.MURLOC)
                    {
                        minionGetBuffed(m, 0, 2, false);
                    }
                }
            }

            if (c.name == "deathwing")
            {
                List<Minion> temp = new List<Minion>(this.ownMinions);
                foreach (Minion enemy in temp)
                {
                    minionGetDestroyed(enemy, true);
                }
                temp.Clear();
                temp.AddRange(this.enemyMinions);
                foreach (Minion enemy in temp)
                {
                    minionGetDestroyed(enemy, false);
                }
                this.owncards.Clear();
                this.owncarddraw = 0;
                this.enemyAnzCards = 0;
                this.enemycarddraw = 0;

            }

            if (c.name == "captainsparrot")
            {
                //this.owncarddraw++;
                drawACard("", true);

            }



        }

        private int spawnKids(CardDB.Card c, int position, bool own, int choice)
        {
            int kids = 0;
            if (c.name == "murloctidehunter")
            {
                kids = 1;
                CardDB.Card kid = CardDB.Instance.getCardDataFromID("EX1_506a");//murlocscout
                callKid(kid, position, own);

            }
            if (c.name == "razorfenhunter")
            {
                kids = 1;
                CardDB.Card kid = CardDB.Instance.getCardDataFromID("CS2_boar");//boar
                callKid(kid, position, own);

            }
            if (c.name == "dragonlingmechanic")
            {
                kids = 1;
                CardDB.Card kid = CardDB.Instance.getCardDataFromID("EX1_025t");//mechanicaldragonling
                callKid(kid, position, own);

            }
            if (c.name == "leeroyjenkins")
            {
                kids = 2;
                CardDB.Card kid = CardDB.Instance.getCardDataFromID("EX1_116t");//whelp
                int pos = this.ownMinions.Count - 1;
                if (own) pos = this.enemyMinions.Count - 1;
                callKid(kid, pos, !own);
                callKid(kid, pos, !own);

            }

            if (c.name == "cenarius" && choice == 2)
            {
                kids = 2;
                CardDB.Card kid = CardDB.Instance.getCardDataFromID("EX1_573t"); //special treant
                int pos = this.ownMinions.Count - 1;
                if (!own) pos = this.enemyMinions.Count - 1;
                callKid(kid, pos, own);
                callKid(kid, pos, own);

            }
            if (c.name == "silverhandknight")
            {
                kids = 1;
                CardDB.Card kid = CardDB.Instance.getCardDataFromID("CS2_152");//squire
                callKid(kid, position, own);

            }
            if (c.name == "gelbinmekkatorque")
            {
                kids = 1;
                CardDB.Card kid = CardDB.Instance.getCardDataFromID("Mekka1");//homingchicken
                callKid(kid, position, own);

            }

            if (c.name == "defiasringleader" && this.cardsPlayedThisTurn >= 1) //needs combo for spawn
            {
                kids = 1;
                CardDB.Card kid = CardDB.Instance.getCardDataFromID("EX1_131t");//defiasbandit
                callKid(kid, position, own);

            }
            if (c.name == "onyxia")
            {
                kids = 7 - this.ownMinions.Count;
                CardDB.Card kid = CardDB.Instance.getCardDataFromID("EX1_116t");//whelp
                for (int i = 0; i < kids; i++)
                {
                    callKid(kid, position, own);
                }

            }
            return kids;
        }

        private void callKid(CardDB.Card c, int placeoffather, bool own)
        {
            if (own && this.ownMinions.Count >= 7) return;
            if (!own && this.enemyMinions.Count >= 7) return;
            int mobplace = placeoffather + 1;
            /*if (own && this.ownMinions.Count >= 1)
            {
                retval.X = ownMinions[mobplace - 1].Posix + 85;
                retval.Y = ownMinions[mobplace - 1].Posiy;
            }
            if (!own && this.enemyMinions.Count >= 1)
            {
                retval.X = enemyMinions[mobplace - 1].Posix + 85;
                retval.Y = enemyMinions[mobplace - 1].Posiy;
            }*/

            Minion m = createNewMinion(new Handmanager.Handcard(c), mobplace, own);

            if (own)
            {
                addMiniontoList(m, this.ownMinions, mobplace, own);// additional minions span next to it!
            }
            else
            {
                addMiniontoList(m, this.enemyMinions, mobplace, own);// additional minions span next to it!
            }

        }

        private Action placeAmobSomewhere(Handmanager.Handcard hc, int cardpos, int target, int choice, int placepos)
        {

            Action a = new Action();
            a.cardplay = true;
            //a.card = new CardDB.Card(c);
            a.numEnemysBeforePlayed = this.enemyMinions.Count;
            a.comboBeforePlayed = (this.cardsPlayedThisTurn >= 1) ? true : false;

            //we place him on the right!
            int mobplace = placepos;


            //create the minion out of the card + effects from other minions, which higher his hp/angr


            // but before additional minions span next to it! (because we buff the minion in createNewMinion and swordofjustice gives summeond minons his buff first!
            int spawnkids = spawnKids(hc.card, mobplace - 1, true, choice); //  if a mob targets something, it doesnt spawn minions!?


            //create the new minion
            Minion m = createNewMinion(hc, mobplace, true);




            //do the battlecry (where you dont need a target)
            doBattleCryWithoutTargeting(m, mobplace, true, choice);
            if (target >= 0)
            {
                doBattleCryWithTargeting(m, target, choice);

            }


            addMiniontoList(m, this.ownMinions, mobplace, true);
            if (logging) Helpfunctions.Instance.logg("added " + m.handcard.card.name);

            //only for fun :D
            if (target >= 0)
            {
                // the OWNtargets right of the placed mobs are going up :D
                if (target < 10 && target > mobplace + spawnkids) target++;
            }

            a.enemytarget = target;
            a.owntarget = mobplace + 1; //1==before the 1.minion on board , 2 ==before the 2. minion o board (from left)
            return a;
        }

        private void lowerWeaponDurability(int value, bool own)
        {
            if (own)
            {
                this.ownWeaponDurability -= value;
                if (this.ownWeaponDurability <= 0)
                {
                    //deathrattle deathsbite
                    if (this.ownWeaponName == "deathsbite")
                    {
                        int anz = 1;
                        if (this.ownBaronRivendare) anz = 2;
                        for (int i = 0; i < anz; i++)
                        {
                            int dmg = getSpellDamageDamage(1);
                            List<Minion> temp = new List<Minion>(this.ownMinions);
                            foreach (Minion m in temp)
                            {
                                minionGetDamagedOrHealed(m, 1, 0, true);
                            }
                            temp.Clear();
                            temp.AddRange(this.enemyMinions);
                            foreach (Minion m in temp)
                            {
                                minionGetDamagedOrHealed(m, 1, 0, false);
                            }
                        }

                    }

                    this.ownheroAngr -= this.ownWeaponAttack;
                    this.ownWeaponDurability = 0;
                    this.ownWeaponAttack = 0;
                    this.ownWeaponName = "";

                    foreach (Minion m in this.ownMinions)
                    {
                        if (m.playedThisTurn && m.name == "southseadeckhand")
                        {
                            m.Ready = false;
                            m.charge = false;
                        }
                    }
                }
            }
            else
            {
                this.enemyWeaponDurability -= value;
                if (this.enemyWeaponDurability <= 0)
                {
                    //deathrattle deathsbite
                    if (this.enemyWeaponName == "deathsbite")
                    {
                        int anz = 1;
                        if (this.enemyBaronRivendare) anz = 2;
                        for (int i = 0; i < anz; i++)
                        {
                            int dmg = getSpellDamageDamage(1);
                            List<Minion> temp = new List<Minion>(this.ownMinions);
                            foreach (Minion m in temp)
                            {
                                minionGetDamagedOrHealed(m, 1, 0, true);
                            }
                            temp.Clear();
                            temp.AddRange(this.enemyMinions);
                            foreach (Minion m in temp)
                            {
                                minionGetDamagedOrHealed(m, 1, 0, false);
                            }
                        }

                    }

                    this.enemyheroAngr -= this.enemyWeaponAttack;
                    this.enemyWeaponDurability = 0;
                    this.enemyWeaponAttack = 0;
                    this.enemyWeaponName = "";
                }
            }
        }


        private void equipWeapon(CardDB.Card c)
        {
            if (this.ownWeaponDurability >= 1)
            {
                this.lostWeaponDamage += this.ownWeaponDurability * this.ownWeaponAttack * this.ownWeaponAttack;
                this.lowerWeaponDurability(1000, true);
            }

            this.ownheroAngr = c.Attack;
            this.ownWeaponAttack = c.Attack;
            this.ownWeaponDurability = c.Durability;
            this.ownWeaponName = c.name;
            if (c.name == "doomhammer")
            {
                this.ownHeroWindfury = true;
            }
            else
            {
                this.ownHeroWindfury = false;
            }
            if ((this.ownHeroNumAttackThisTurn == 0 || (this.ownHeroWindfury && this.ownHeroNumAttackThisTurn == 1)) && !this.ownHeroFrozen)
            {
                this.ownHeroReady = true;
            }
            if (c.name == "gladiatorslongbow")
            {
                this.heroImmuneWhileAttacking = true;
            }
            else
            {
                this.heroImmuneWhileAttacking = false;
            }

            foreach (Minion m in this.ownMinions)
            {
                if (m.playedThisTurn && m.name == "southseadeckhand")
                {
                    minionGetCharge(m);
                }
            }

        }

        private void playCardWithTarget(Handmanager.Handcard hc, int target, int choice)
        {
            CardDB.Card c = hc.card;
            //play card with target
            int attackbuff = 0;
            int hpbuff = 0;
            int heal = 0;
            int damage = 0;
            bool spott = false;
            bool divineshild = false;
            bool windfury = false;
            bool silence = false;
            bool destroy = false;
            bool frozen = false;
            bool stealth = false;
            bool backtohand = false;
            int backtoHandManaChange = 0;
            bool charge = false;
            bool setHPtoONE = false;
            bool immune = false;
            int adjacentDamage = 0;
            bool sheep = false;
            bool frogg = false;
            //special
            bool geistderahnen = false;
            bool ueberwaeltigendemacht = false;

            bool own = true;

            if (target >= 10 && target < 20)
            {
                own = false;
            }
            Minion m = new Minion();
            if (target < 10)
            {
                m = this.ownMinions[target];
            }
            if (target >= 10 && target < 20)
            {
                m = this.enemyMinions[target - 10];
            }


            //warrior###########################################################################

            if (c.name == "execute")
            {
                destroy = true;
            }

            if (c.name == "innerrage")
            {
                damage = 1;
                attackbuff = 2;
            }

            if (c.name == "slam")
            {
                damage = 2;
                if (m.Hp >= 3)
                {
                    //this.owncarddraw++;
                    drawACard("", true);
                }
            }

            if (c.name == "mortalstrike")
            {
                damage = 4;
                if (ownHeroHp <= 12) damage = 6;
            }

            if (c.name == "shieldslam")
            {
                damage = this.ownHeroDefence;
            }

            if (c.name == "charge")
            {
                charge = true;
                attackbuff = 2;
            }

            if (c.name == "rampage")
            {
                attackbuff = 3;
                hpbuff = 3;
            }

            //hunter#################################################################################

            if (c.name == "huntersmark")
            {
                setHPtoONE = true;
            }
            if (c.name == "arcaneshot")
            {
                damage = 2;
            }
            if (c.name == "killcommand")
            {
                damage = 3;
                foreach (Minion mnn in this.ownMinions)
                {
                    if ((TAG_RACE)mnn.handcard.card.race == TAG_RACE.PET)
                    {
                        damage = 5;
                    }
                }
            }
            if (c.name == "bestialwrath")
            {

                Enchantment e = CardDB.getEnchantmentFromCardID("EX1_549o");
                e.creator = hc.entity;
                e.controllerOfCreator = this.ownController;
                addEffectToMinionNoDoubles(m, e, own);
            }

            if (c.name == "explosiveshot")
            {
                damage = 5;
                adjacentDamage = 2;
            }

            //mage###############################################################################

            if (c.name == "icelance")
            {
                if (target >= 0 && target <= 19)
                {
                    if (m.frozen)
                    {
                        damage = 4;
                    }
                    else { frozen = true; }
                }
                else
                {
                    if (target == 100)
                    {
                        if (this.ownHeroFrozen)
                        {
                            damage = 4;
                        }
                        else
                        {
                            frozen = true;
                        }
                    }
                    if (target == 200)
                    {
                        if (this.enemyHeroFrozen)
                        {
                            damage = 4;
                        }
                        else
                        {
                            frozen = true;
                        }
                    }
                }
            }

            if (c.name == "coneofcold")
            {
                damage = 1;
                adjacentDamage = 1;
                frozen = true;
            }
            if (c.name == "fireball")
            {
                damage = 6;
            }
            if (c.name == "polymorph")
            {
                sheep = true;
            }

            if (c.name == "pyroblast")
            {
                damage = 10;
            }

            if (c.name == "frostbolt")
            {
                damage = 3;
                frozen = true;
            }

            //pala######################################################################

            if (c.name == "humility")
            {
                m.Angr = 1;
            }
            if (c.name == "handofprotection")
            {
                divineshild = true;
            }
            if (c.name == "blessingofmight")
            {
                attackbuff = 3;
            }
            if (c.name == "holylight")
            {
                heal = 6;
            }

            if (c.name == "hammerofwrath")
            {
                damage = 3;
                //this.owncarddraw++;
                drawACard("", true);
            }

            if (c.name == "blessingofkings")
            {
                attackbuff = 4;
                hpbuff = 4;
            }

            if (c.name == "blessingofwisdom")
            {
                Enchantment e = CardDB.getEnchantmentFromCardID("EX1_363e2");
                e.creator = hc.entity;
                e.controllerOfCreator = this.ownController;
                m.enchantments.Add(e);
            }

            if (c.name == "blessedchampion")
            {
                m.Angr *= 2;
            }
            if (c.name == "holywrath")
            {
                damage = 3;
                //this.owncarddraw++;
                drawACard("", true);
            }
            if (c.name == "layonhands")
            {
                for (int i = 0; i < 3; i++)
                {
                    //this.owncarddraw++;
                    drawACard("", true);
                }
                heal = 8;
            }

            //priest ##########################################

            if (c.name == "shadowmadness")
            {

                Enchantment e = CardDB.getEnchantmentFromCardID("EX1_334e");
                e.creator = hc.entity;
                e.controllerOfCreator = this.ownController;
                addEffectToMinionNoDoubles(m, e, own);
                this.minionGetControlled(m, true, true);
            }

            if (c.name == "mindcontrol")
            {
                this.minionGetControlled(m, true, false);
            }

            if (c.name == "holysmite")
            {
                damage = 2;
            }
            if (c.name == "powerwordshield")
            {
                hpbuff = 2;
                //this.owncarddraw++;
                drawACard("", true);
            }
            if (c.name == "silence")
            {
                silence = true;
            }
            if (c.name == "divinespirit")
            {
                hpbuff = m.Hp;
            }
            if (c.name == "innerfire")
            {
                m.Angr = m.Hp;
            }
            if (c.name == "holyfire")
            {
                damage = 5;
                int ownheal = getSpellHeal(5);
                attackOrHealHero(-ownheal, true);
            }
            if (c.name == "shadowwordpain")
            {
                destroy = true;
            }
            if (c.name == "shadowworddeath")
            {
                destroy = true;
            }
            //rogue ##########################################
            if (c.name == "shadowstep")
            {
                backtohand = true;
                backtoHandManaChange = -2;
                //m.handcard.card.cost = Math.Max(0, m.handcard.card.cost -= 2);
            }
            if (c.name == "sap")
            {
                backtohand = true;
                backtoHandManaChange = 0;
            }
            if (c.name == "shiv")
            {
                damage = 1;
                //this.owncarddraw++;
                drawACard("", true);
            }
            if (c.name == "coldblood")
            {
                attackbuff = 2;
                if (this.cardsPlayedThisTurn >= 1) attackbuff = 4;
            }
            if (c.name == "conceal")
            {
                stealth = true;
            }
            if (c.name == "eviscerate")
            {
                damage = 2;
                if (this.cardsPlayedThisTurn >= 1) damage = 4;
            }
            if (c.name == "betrayal")
            {
                //attack right neightbor
                if (target >= 10 && target < 20 && target < this.enemyMinions.Count + 10 - 1)
                {
                    attack(target, target + 1, true);
                }
                if (target < 10 && target < this.ownMinions.Count - 1)
                {
                    attack(target, target + 1, true);
                }

                //attack left neightbor
                if (target >= 11 || (target < 10 && target >= 1))
                {
                    attack(target, target - 1, true);
                }

            }

            if (c.name == "perditionsblade")
            {
                damage = 1;
                if (this.cardsPlayedThisTurn >= 1) damage = 2;
            }

            if (c.name == "backstab")
            {
                damage = 2;
            }

            if (c.name == "assassinate")
            {
                destroy = true;
            }
            //shaman ##########################################
            if (c.name == "lightningbolt")
            {
                damage = 3;
            }
            if (c.name == "frostshock")
            {
                frozen = true;
                damage = 1;
            }
            if (c.name == "rockbiterweapon")
            {
                if (target <= 20)
                {
                    Enchantment e = CardDB.getEnchantmentFromCardID("CS2_045e");
                    e.creator = hc.entity;
                    e.controllerOfCreator = this.ownController;
                    addEffectToMinionNoDoubles(m, e, own);
                }
                else
                {
                    if (target == 100)
                    {
                        this.ownheroAngr += 3;
                        if ((this.ownHeroNumAttackThisTurn == 0 || (this.ownHeroWindfury && this.ownHeroNumAttackThisTurn == 1)) && !this.ownHeroFrozen)
                        {
                            this.ownHeroReady = true;
                        }
                    }
                }
            }
            if (c.name == "windfury")
            {
                windfury = true;
            }
            if (c.name == "hex")
            {
                frogg = true;
            }
            if (c.name == "earthshock")
            {
                silence = true;
                damage = 1;
            }
            if (c.name == "ancestralspirit")
            {
                geistderahnen = true;
            }
            if (c.name == "lavaburst")
            {
                damage = 5;
            }

            if (c.name == "ancestralhealing")
            {
                heal = 1000;
                spott = true;
            }

            //hexenmeister ##########################################

            if (c.name == "sacrificialpact")
            {
                destroy = true;
                this.attackOrHealHero(getSpellHeal(5), true); // heal own hero
            }

            if (c.name == "soulfire")
            {
                damage = 4;
                this.owncarddraw--;
                this.owncards.RemoveRange(0, Math.Min(1, this.owncards.Count));


            }
            if (c.name == "poweroverwhelming")
            {
                //only to own mininos
                Enchantment e = CardDB.getEnchantmentFromCardID("EX1_316e");
                e.creator = hc.entity;
                e.controllerOfCreator = this.ownController;
                addEffectToMinionNoDoubles(m, e, true);
            }
            if (c.name == "corruption")
            {
                //only to enemy mininos
                Enchantment e = CardDB.getEnchantmentFromCardID("CS2_063e");
                e.creator = hc.entity;
                e.controllerOfCreator = this.ownController;
                addEffectToMinionNoDoubles(m, e, false);
            }
            if (c.name == "mortalcoil")
            {
                damage = 1;
                if (getSpellDamageDamage(1) >= m.Hp && !m.divineshild && !m.immune)
                {
                    //this.owncarddraw++;
                    drawACard("", true);
                }
            }
            if (c.name == "drainlife")
            {
                damage = 2;
                attackOrHealHero(2, true);
            }
            if (c.name == "shadowbolt")
            {
                damage = 4;
            }
            if (c.name == "shadowflame")
            {
                int damage1 = getSpellDamageDamage(m.Angr);
                List<Minion> temp = new List<Minion>(this.enemyMinions);
                foreach (Minion mnn in temp)
                {
                    minionGetDamagedOrHealed(mnn, damage1, 0, false);
                }
                //destroy own mininon
                destroy = true;
            }

            if (c.name == "demonfire")
            {
                if (m.handcard.card.race == 15 && own)
                {
                    attackbuff = 2;
                    hpbuff = 2;
                }
                else
                {
                    damage = 2;
                }
            }
            if (c.name == "baneofdoom")
            {
                damage = 2;
                if (getSpellDamageDamage(2) >= m.Hp && !m.divineshild && !m.immune)
                {
                    int posi = this.ownMinions.Count - 1;
                    CardDB.Card kid = CardDB.Instance.getCardDataFromID("CS2_059");//bloodimp
                    callKid(kid, posi, true);
                }
            }

            if (c.name == "siphonsoul")
            {
                destroy = true;
                attackOrHealHero(3, true);

            }


            //druid #######################################################################

            if (c.name == "moonfire" && c.CardID == "CS2_008")// nicht zu verwechseln mit cenarius choice nummer 1
            {
                damage = 1;
            }

            if (c.name == "markofthewild")
            {
                spott = true;
                attackbuff = 2;
                hpbuff = 2;
            }

            if (c.name == "healingtouch")
            {
                heal = 8;
            }

            if (c.name == "starfire")
            {
                damage = 5;
                //this.owncarddraw++;
                drawACard("", true);
            }

            if (c.name == "naturalize")
            {
                destroy = true;
                this.enemycarddraw += 2;
            }

            if (c.name == "savagery")
            {
                damage = this.ownheroAngr;
            }

            if (c.name == "swipe")
            {
                damage = 4;
                // all others get 1 spelldamage
                int damage1 = getSpellDamageDamage(1);
                if (target != 200)
                {
                    attackOrHealHero(damage1, false);
                }
                List<Minion> temp = new List<Minion>(this.enemyMinions);
                foreach (Minion mnn in temp)
                {
                    if (mnn.id + 10 != target)
                    {
                        minionGetDamagedOrHealed(mnn, damage1, 0, false);
                    }
                }
            }

            //druid choices##################################################################################
            if (c.name == "wrath")
            {
                if (choice == 1)
                {
                    damage = 3;
                }
                if (choice == 2)
                {
                    damage = 1;
                    //this.owncarddraw++;
                    drawACard("", true);
                }
            }

            if (c.name == "markofnature")
            {
                if (choice == 1)
                {
                    attackbuff = 4;
                }
                if (choice == 2)
                {
                    spott = true;
                    hpbuff = 4;
                }
            }

            if (c.name == "starfall")
            {
                if (choice == 1)
                {
                    damage = 5;
                }

            }


            //special cards#########################################################################################

            if (c.name == "nightmare")
            {
                //only to own mininos
                Enchantment e = CardDB.getEnchantmentFromCardID("EX1_316e");
                e.creator = hc.entity;
                e.controllerOfCreator = this.ownController;
                addEffectToMinionNoDoubles(m, e, true);
            }

            if (c.name == "dream")
            {
                backtohand = true;
                backtoHandManaChange = 0;
            }

            if (c.name == "bananas")
            {
                attackbuff = 1;
                hpbuff = 1;
            }

            if (c.name == "barreltoss")
            {
                damage = 2;
            }

            if (c.CardID == "PRO_001b")// i am murloc
            {
                damage = 4;
                //this.owncarddraw++;
                drawACard("", true);

            } if (c.name == "willofmukla")
            {
                heal = 6;
            }

            //NaxxCards###################################################################################
            if (c.name == "reincarnation")
            {
                int place = m.id;
                if (place >= 10) place -= 10;
                CardDB.Card d = m.handcard.card;
                minionGetDestroyed(m, own);
                callKid(d, place, own);
            }

            //make effect on target
            //ownminion

            if (damage >= 1) damage = getSpellDamageDamage(damage);
            if (adjacentDamage >= 1) adjacentDamage = getSpellDamageDamage(adjacentDamage);
            if (heal >= 1 && heal < 1000) heal = getSpellHeal(heal);

            if (target < 10)
            {
                if (silence) minionGetSilenced(m, true);
                minionGetBuffed(m, attackbuff, hpbuff, true);
                minionGetDamagedOrHealed(m, damage, heal, true);
                if (spott) m.taunt = true;
                if (charge) minionGetCharge(m);
                if (windfury) minionGetWindfurry(m);
                if (divineshild) m.divineshild = true;
                if (destroy) minionGetDestroyed(m, true);
                if (frozen) m.frozen = true;
                if (stealth) m.stealth = true;
                if (backtohand) minionReturnToHand(m, true, backtoHandManaChange);
                if (immune) m.immune = true;
                if (adjacentDamage >= 1)
                {
                    List<Minion> tempolist = new List<Minion>(this.ownMinions);
                    foreach (Minion mnn in tempolist)
                    {
                        if (mnn.id == target + 1 || mnn.id == target - 1)
                        {
                            minionGetDamagedOrHealed(m, adjacentDamage, 0, own);
                            if (frozen) mnn.frozen = true;
                        }
                    }
                }
                if (sheep) minionTransform(m, CardDB.Instance.getCardDataFromID("CS2_tk1"), own);
                if (frogg) minionTransform(m, CardDB.Instance.getCardDataFromID("hexfrog"), own);
                if (setHPtoONE)
                {
                    m.Hp = 1; m.maxHp = 1;
                }

                if (geistderahnen)
                {
                    Enchantment e = CardDB.getEnchantmentFromCardID("CS2_038e");
                    e.creator = hc.entity;
                    e.controllerOfCreator = this.ownController;
                    addEffectToMinionNoDoubles(m, e, true);
                }


            }
            //enemyminion
            if (target >= 10 && target < 20)
            {
                if (silence) minionGetSilenced(m, false);
                minionGetBuffed(m, attackbuff, hpbuff, false);
                minionGetDamagedOrHealed(m, damage, heal, false);
                if (spott) m.taunt = true;
                if (charge) minionGetCharge(m);
                if (windfury) minionGetWindfurry(m);
                if (divineshild) m.divineshild = true;
                if (destroy) minionGetDestroyed(m, false);
                if (frozen) m.frozen = true;
                if (stealth) m.stealth = true;
                if (backtohand) minionReturnToHand(m, false, backtoHandManaChange);
                if (immune) m.immune = true;
                if (adjacentDamage >= 1)
                {
                    List<Minion> tempolist = new List<Minion>(this.enemyMinions);
                    foreach (Minion mnn in tempolist)
                    {
                        if (mnn.id + 10 == target + 1 || mnn.id + 10 == target - 1)
                        {
                            minionGetDamagedOrHealed(m, adjacentDamage, 0, own);
                            if (frozen) mnn.frozen = true;
                        }
                    }
                }
                if (sheep) minionTransform(m, CardDB.Instance.getCardDataFromID("CS2_tk1"), own);
                if (frogg) minionTransform(m, CardDB.Instance.getCardDataFromID("hexfrog"), own);
                if (setHPtoONE)
                {
                    m.Hp = 1; m.maxHp = 1;
                }
                if (geistderahnen)
                {
                    Enchantment e = CardDB.getEnchantmentFromCardID("CS2_038e");
                    e.creator = hc.entity;
                    e.controllerOfCreator = this.ownController;
                    addEffectToMinionNoDoubles(m, e, false);
                }

            }
            if (target == 100)
            {
                if (frozen) this.ownHeroFrozen = true;
                if (damage >= 1) attackOrHealHero(damage, true);
                if (heal >= 1) attackOrHealHero(-heal, true);
            }
            if (target == 200)
            {
                if (frozen) this.enemyHeroFrozen = true;
                if (damage >= 1) attackOrHealHero(damage, false);
                if (heal >= 1) attackOrHealHero(-heal, false);
            }

        }

        private void playCardWithoutTarget(Handmanager.Handcard hc, int choice)
        {
            CardDB.Card c = hc.card;
            //todo faehrtenlesen!

            //play card without target
            if (c.name == "thecoin")
            {
                this.mana++;

            }
            //hunter#########################################################################
            if (c.name == "multi-shot" && this.enemyMinions.Count >= 2)
            {
                List<Minion> temp = new List<Minion>();
                int damage = getSpellDamageDamage(3);
                List<Minion> temp2 = new List<Minion>(this.enemyMinions);
                temp2.Sort((a, b) => -a.Hp.CompareTo(b.Hp));//damage the strongest
                temp.AddRange(Helpfunctions.TakeList(temp2, 2));
                foreach (Minion enemy in temp)
                {
                    minionGetDamagedOrHealed(enemy, damage, 0, false);
                }

            }
            if (c.name == "animalcompanion")
            {
                CardDB.Card c2 = CardDB.Instance.getCardDataFromID("NEW1_032");//misha
                int placeoffather = this.ownMinions.Count - 1;
                callKid(c2, placeoffather, true);
            }

            if (c.name == "flare")
            {
                foreach (Minion m in this.ownMinions)
                {
                    m.stealth = false;
                }
                foreach (Minion m in this.enemyMinions)
                {
                    m.stealth = false;
                }
                //this.owncarddraw++;
                drawACard("", true);
                this.enemySecretCount = 0;
            }

            if (c.name == "unleashthehounds")
            {
                int anz = this.enemyMinions.Count;
                int posi = this.ownMinions.Count - 1;
                CardDB.Card kid = CardDB.Instance.getCardDataFromID("EX1_538t");//hound
                for (int i = 0; i < anz; i++)
                {
                    callKid(kid, posi, true);
                }
            }

            if (c.name == "deadlyshot" && this.enemyMinions.Count >= 1)
            {
                List<Minion> temp = new List<Minion>();
                List<Minion> temp2 = new List<Minion>(this.enemyMinions);
                temp2.Sort((a, b) => a.Angr.CompareTo(b.Angr));
                temp.AddRange(Helpfunctions.TakeList(temp2, 1));
                foreach (Minion enemy in temp)
                {
                    minionGetDestroyed(enemy, false);
                }

            }

            //warrior#########################################################################
            if (c.name == "commandingshout")
            {
                List<Minion> temp = new List<Minion>(this.ownMinions);
                Enchantment e1 = CardDB.getEnchantmentFromCardID("NEW1_036e");
                e1.creator = hc.entity;
                e1.controllerOfCreator = this.ownController;
                Enchantment e2 = CardDB.getEnchantmentFromCardID("NEW1_036e2");
                e2.creator = hc.entity;
                e2.controllerOfCreator = this.ownController;
                foreach (Minion mnn in temp)
                {//cantLowerHPbelowONE
                    addEffectToMinionNoDoubles(mnn, e1, true);
                    addEffectToMinionNoDoubles(mnn, e2, true);
                    mnn.cantLowerHPbelowONE = true;
                }

            }

            if (c.name == "battlerage")
            {
                foreach (Minion mnn in this.ownMinions)
                {
                    if (mnn.wounded)
                    {
                        //this.owncarddraw++;
                        drawACard("", true);
                    }
                }

            }

            if (c.name == "brawl")
            {
                List<Minion> temp = new List<Minion>(this.ownMinions);
                foreach (Minion mnn in temp)
                {
                    minionGetDestroyed(mnn, true);
                }
                temp.Clear();
                temp.AddRange(this.enemyMinions);
                foreach (Minion mnn in temp)
                {
                    minionGetDestroyed(mnn, false);
                }

            }


            if (c.name == "cleave" && this.enemyMinions.Count >= 2)
            {
                List<Minion> temp = new List<Minion>();
                int damage = getSpellDamageDamage(2);
                List<Minion> temp2 = new List<Minion>(this.enemyMinions);
                temp2.Sort((a, b) => -a.Hp.CompareTo(b.Hp));
                temp.AddRange(Helpfunctions.TakeList(temp2, 2));
                foreach (Minion enemy in temp)
                {
                    minionGetDamagedOrHealed(enemy, damage, 0, false);
                }

            }

            if (c.name == "upgrade")
            {
                if (this.ownWeaponName != "")
                {
                    this.ownWeaponAttack++;
                    this.ownheroAngr++;
                    this.ownWeaponDurability++;
                }
                else
                {
                    CardDB.Card wcard = CardDB.Instance.getCardDataFromID("EX1_409t");//heavyaxe
                    this.equipWeapon(wcard);
                }

            }



            if (c.name == "whirlwind")
            {
                List<Minion> temp = new List<Minion>(this.enemyMinions);
                int damage = getSpellDamageDamage(1);
                foreach (Minion enemy in temp)
                {
                    minionGetDamagedOrHealed(enemy, damage, 0, false);
                }
                temp.Clear();
                temp = new List<Minion>(this.ownMinions);
                foreach (Minion enemy in temp)
                {
                    minionGetDamagedOrHealed(enemy, damage, 0, true);
                }
            }

            if (c.name == "heroicstrike")
            {
                this.ownheroAngr = this.ownheroAngr + 4;
                if ((this.ownHeroNumAttackThisTurn == 0 || (this.ownHeroWindfury && this.ownHeroNumAttackThisTurn == 1)) && !this.ownHeroFrozen)
                {
                    this.ownHeroReady = true;
                }
            }

            if (c.name == "shieldblock")
            {
                this.ownHeroDefence = this.ownHeroDefence + 5;
                //this.owncarddraw++;
                drawACard("", true);
            }



            //mage#########################################################################################

            if (c.name == "blizzard")
            {
                int damage = getSpellDamageDamage(2);
                List<Minion> temp = new List<Minion>(this.enemyMinions);
                int maxHp = 0;
                foreach (Minion enemy in temp)
                {
                    enemy.frozen = true;
                    if (maxHp < enemy.Hp) maxHp = enemy.Hp;

                    minionGetDamagedOrHealed(enemy, damage, 0, false, true);
                }

                this.lostDamage += Math.Max(0, damage - maxHp);

            }

            if (c.name == "arcanemissiles")
            {
                /*List<Minion> temp = new List<Minion>(this.enemyMinions);
                temp.Sort((a, b) => -a.Hp.CompareTo(b.Hp));
                int damage = 1;
                int ammount = getSpellDamageDamage(3);
                int i = 0;
                int hp = 0;
                foreach (Minion enemy in temp)
                {
                    if (enemy.Hp >= 2)
                    {
                        minionGetDamagedOrHealed(enemy, damage, 0, false);
                        i++;
                        hp += enemy.Hp;
                        if (i == ammount) break;
                    }
                    
                }
                if (i < ammount) attackOrHealHero(ammount - i, false);*/

                int damage = 1;
                int i = 0;
                List<Minion> temp = new List<Minion>(this.enemyMinions);
                int times = this.getSpellDamageDamage(3);
                while (i < times)
                {
                    if (temp.Count >= 1)
                    {
                        temp.Sort((a, b) => -a.Hp.CompareTo(b.Hp));
                        if (temp[0].Hp == 1 && this.enemyHeroHp >= 2)
                        {
                            attackOrHealHero(damage, false);
                        }
                        else
                        {
                            minionGetDamagedOrHealed(temp[0], damage, 0, false);
                        }
                    }
                    else
                    {
                        attackOrHealHero(damage, false);
                    }
                    i++;
                }



            }
            if (c.name == "arcaneintellect")
            {
                //this.owncarddraw++;
                drawACard("", true);
                drawACard("", true);
            }

            if (c.name == "mirrorimage")
            {
                int posi = this.ownMinions.Count - 1;
                CardDB.Card kid = CardDB.Instance.getCardDataFromID("CS2_mirror");
                callKid(kid, posi, true);
                callKid(kid, posi, true);
            }

            if (c.name == "arcaneexplosion")
            {
                List<Minion> temp = new List<Minion>(this.enemyMinions);
                int damage = getSpellDamageDamage(1);
                foreach (Minion enemy in temp)
                {
                    minionGetDamagedOrHealed(enemy, damage, 0, false);
                }
            }
            if (c.name == "frostnova")
            {
                List<Minion> temp = new List<Minion>(this.enemyMinions);
                foreach (Minion enemy in temp)
                {
                    enemy.frozen = true;
                }

            }
            if (c.name == "flamestrike")
            {
                List<Minion> temp = new List<Minion>(this.enemyMinions);
                int damage = getSpellDamageDamage(4);
                int maxHp = 0;
                foreach (Minion enemy in temp)
                {
                    if (maxHp < enemy.Hp) maxHp = enemy.Hp;

                    minionGetDamagedOrHealed(enemy, damage, 0, false, true);
                }
                this.lostDamage += Math.Max(0, damage - maxHp);

            }

            //pala#################################################################
            if (c.name == "consecration")
            {
                List<Minion> temp = new List<Minion>(this.enemyMinions);
                int damage = getSpellDamageDamage(2);
                foreach (Minion enemy in temp)
                {
                    minionGetDamagedOrHealed(enemy, damage, 0, false);
                }

                attackOrHealHero(damage, false);
            }

            if (c.name == "equality")
            {
                foreach (Minion m in this.ownMinions)
                {
                    m.Hp = 1;
                    m.maxHp = 1;
                }
                foreach (Minion m in this.enemyMinions)
                {
                    m.Hp = 1;
                    m.maxHp = 1;
                }

            }
            if (c.name == "divinefavor")
            {
                int enemcardsanz = this.enemyAnzCards + this.enemycarddraw;
                int diff = enemcardsanz - this.owncards.Count;
                if (diff >= 1)
                {
                    for (int i = 0; i < diff; i++)
                    {
                        //this.owncarddraw++;
                        drawACard("", true);
                    }
                }
            }

            if (c.name == "avengingwrath")
            {

                int damage = 1;
                int i = 0;
                List<Minion> temp = new List<Minion>(this.enemyMinions);
                int times = this.getSpellDamageDamage(8);
                while (i < times)
                {
                    if (temp.Count >= 1)
                    {
                        temp.Sort((a, b) => -a.Hp.CompareTo(b.Hp));
                        if (temp[0].Hp == 1 && this.enemyHeroHp >= 2)
                        {
                            attackOrHealHero(damage, false);
                        }
                        else
                        {
                            minionGetDamagedOrHealed(temp[0], damage, 0, false);
                        }
                    }
                    else
                    {
                        attackOrHealHero(damage, false);
                    }
                    i++;
                }

            }


            //priest ####################################################
            if (c.name == "circleofhealing")
            {
                List<Minion> temp = new List<Minion>(this.enemyMinions);
                int heal = getSpellHeal(4);
                foreach (Minion enemy in temp)
                {
                    minionGetDamagedOrHealed(enemy, 0, heal, false);
                }
                temp.Clear();
                temp.AddRange(this.ownMinions);
                foreach (Minion enemy in temp)
                {
                    minionGetDamagedOrHealed(enemy, 0, heal, true);
                }

            }
            if (c.name == "thoughtsteal")
            {
                //this.owncarddraw++;
                this.drawACard("enemycard", true);
                //this.owncarddraw++;
                this.drawACard("enemycard", true);
            }
            if (c.name == "mindvision")
            {
                if (this.enemyAnzCards + this.enemycarddraw >= 1)
                {
                    //this.owncarddraw++;
                    this.drawACard("enemycard", true);
                }
            }

            if (c.name == "shadowform")
            {
                if (this.ownHeroAblility.CardID == "CS1h_001") // lesser heal becomes mind spike
                {
                    this.ownHeroAblility = CardDB.Instance.getCardDataFromID("EX1_625t");
                    this.ownAbilityReady = true;
                }
                else
                {
                    this.ownHeroAblility = CardDB.Instance.getCardDataFromID("EX1_625t2");  // mindspike becomes mind shatter
                    this.ownAbilityReady = true;
                }
            }

            if (c.name == "mindgames")
            {
                CardDB.Card copymin = CardDB.Instance.getCardDataFromID("CS2_152"); //we draw a knappe :D (worst case)
                callKid(copymin, this.ownMinions.Count - 1, true);
            }

            if (c.name == "massdispel")
            {
                foreach (Minion m in this.enemyMinions)
                {
                    minionGetSilenced(m, false);
                }
            }
            if (c.name == "mindblast")
            {
                int damage = getSpellDamageDamage(5);
                attackOrHealHero(damage, false);
            }

            if (c.name == "holynova")
            {
                List<Minion> temp = new List<Minion>(this.ownMinions);
                int heal = getSpellHeal(2);
                int damage = getSpellDamageDamage(2);
                foreach (Minion enemy in temp)
                {
                    minionGetDamagedOrHealed(enemy, 0, heal, true, true);
                }
                attackOrHealHero(-heal, true);
                temp.Clear();
                temp.AddRange(this.enemyMinions);
                foreach (Minion enemy in temp)
                {
                    minionGetDamagedOrHealed(enemy, damage, 0, false, true);
                }
                attackOrHealHero(damage, false);

            }
            //rogue #################################################
            if (c.name == "preparation")
            {
                this.playedPreparation = true;
            }
            if (c.name == "bladeflurry")
            {
                List<Minion> temp = new List<Minion>(this.enemyMinions);
                int damage = this.getSpellDamageDamage(this.ownWeaponAttack);
                foreach (Minion enemy in temp)
                {
                    minionGetDamagedOrHealed(enemy, damage, 0, false);
                }
                attackOrHealHero(damage, false);

                //destroy own weapon
                this.lowerWeaponDurability(1000, true);
            }
            if (c.name == "headcrack")
            {
                int damage = getSpellDamageDamage(2);
                attackOrHealHero(damage, false);
                if (this.cardsPlayedThisTurn >= 1) this.owncarddraw++; // DONT DRAW A CARD WITH (drawAcard()) because we get this NEXT turn 
            }
            if (c.name == "sinisterstrike")
            {
                int damage = getSpellDamageDamage(3);
                attackOrHealHero(damage, false);
            }
            if (c.name == "deadlypoison")
            {
                if (this.ownWeaponName != "")
                {
                    this.ownWeaponAttack += 2;
                    this.ownheroAngr += 2;

                }
            }
            if (c.name == "fanofknives")
            {
                List<Minion> temp = new List<Minion>(this.enemyMinions);
                int damage = getSpellDamageDamage(1);
                foreach (Minion enemy in temp)
                {
                    minionGetDamagedOrHealed(enemy, damage, 0, false);
                }
                drawACard("", true);
            }

            if (c.name == "sprint")
            {
                for (int i = 0; i < 4; i++)
                {
                    //this.owncarddraw++;
                    drawACard("", true);
                }

            }

            if (c.name == "vanish")
            {
                List<Minion> temp = new List<Minion>(this.enemyMinions);
                int heal = getSpellHeal(4);
                foreach (Minion enemy in temp)
                {
                    minionReturnToHand(enemy, false, 0);
                }
                temp.Clear();
                temp.AddRange(this.ownMinions);
                foreach (Minion enemy in temp)
                {
                    minionReturnToHand(enemy, true, 0);
                }

            }

            //shaman #################################################
            if (c.name == "forkedlightning" && this.enemyMinions.Count >= 2)
            {
                List<Minion> temp = new List<Minion>();
                int damage = getSpellDamageDamage(2);
                List<Minion> temp2 = new List<Minion>(this.enemyMinions);
                temp2.Sort((a, b) => -a.Hp.CompareTo(b.Hp));
                temp.AddRange(Helpfunctions.TakeList(temp2, 2));
                foreach (Minion enemy in temp)
                {
                    minionGetDamagedOrHealed(enemy, damage, 0, false);
                }

            }

            if (c.name == "farsight")
            {
                //this.owncarddraw++;
                drawACard("", true);

            }

            if (c.name == "lightningstorm")
            {
                List<Minion> temp = new List<Minion>(this.enemyMinions);
                int damage = getSpellDamageDamage(2);

                int maxHp = 0;
                foreach (Minion enemy in temp)
                {
                    if (maxHp < enemy.Hp) maxHp = enemy.Hp;

                    minionGetDamagedOrHealed(enemy, damage, 0, false, true);
                }
                this.lostDamage += Math.Max(0, damage - maxHp);

            }

            if (c.name == "feralspirit")
            {
                int posi = this.ownMinions.Count - 1;
                CardDB.Card kid = CardDB.Instance.getCardDataFromID("EX1_tk11");//spiritwolf
                callKid(kid, posi, true);
                callKid(kid, posi, true);
            }

            if (c.name == "totemicmight")
            {
                List<Minion> temp = new List<Minion>(this.ownMinions);
                foreach (Minion m in temp)
                {
                    if (m.handcard.card.race == 21) // if minion is a totem, buff it
                    {
                        minionGetBuffed(m, 0, 2, true);
                    }
                }

            }

            if (c.name == "bloodlust")
            {
                List<Minion> temp = new List<Minion>(this.ownMinions);
                foreach (Minion m in temp)
                {
                    Enchantment e = CardDB.getEnchantmentFromCardID("CS2_046e");
                    e.creator = this.ownController;
                    e.controllerOfCreator = this.ownController;
                    addEffectToMinionNoDoubles(m, e, true);
                }
            }


            //hexenmeister #################################################
            if (c.name == "sensedemons")
            {
                //this.owncarddraw += 2;
                this.drawACard("", true);
                this.drawACard("", true);


            }
            if (c.name == "twistingnether")
            {
                List<Minion> temp = new List<Minion>(this.enemyMinions);
                foreach (Minion enemy in temp)
                {
                    minionGetDestroyed(enemy, false);
                }
                temp.Clear();
                temp.AddRange(this.ownMinions);
                foreach (Minion enemy in temp)
                {
                    minionGetDestroyed(enemy, true);
                }

            }

            if (c.name == "hellfire")
            {
                List<Minion> temp = new List<Minion>(this.enemyMinions);
                int damage = getSpellDamageDamage(3);
                foreach (Minion enemy in temp)
                {
                    minionGetDamagedOrHealed(enemy, damage, 0, false);
                }
                temp.Clear();
                temp.AddRange(this.ownMinions);
                foreach (Minion enemy in temp)
                {
                    minionGetDamagedOrHealed(enemy, damage, 0, true);
                }
                attackOrHealHero(damage, true);
                attackOrHealHero(damage, false);

            }


            //druid #################################################
            if (c.name == "souloftheforest")
            {
                List<Minion> temp = new List<Minion>(this.ownMinions);
                Enchantment e = CardDB.getEnchantmentFromCardID("EX1_158e");
                e.creator = hc.entity;
                e.controllerOfCreator = this.ownController;
                foreach (Minion enemy in temp)
                {
                    addEffectToMinionNoDoubles(enemy, e, true);
                }
            }

            if (c.name == "innervate")
            {
                this.mana = Math.Min(this.mana + 2, 10);

            }

            if (c.name == "bite")
            {
                this.ownheroAngr += 4;
                this.ownHeroDefence += 4;
                if ((this.ownHeroNumAttackThisTurn == 0 || (this.ownHeroWindfury && this.ownHeroNumAttackThisTurn == 1)) && !this.ownHeroFrozen)
                {
                    this.ownHeroReady = true;
                }

            }

            if (c.name == "claw")
            {
                this.ownheroAngr += 2;
                this.ownHeroDefence += 2;
                if ((this.ownHeroNumAttackThisTurn == 0 || (this.ownHeroWindfury && this.ownHeroNumAttackThisTurn == 1)) && !this.ownHeroFrozen)
                {
                    this.ownHeroReady = true;
                }

            }

            if (c.name == "forceofnature")
            {
                int posi = this.ownMinions.Count - 1;
                CardDB.Card kid = CardDB.Instance.getCardDataFromID("EX1_tk9");//Treant
                callKid(kid, posi, true);
                callKid(kid, posi, true);
                callKid(kid, posi, true);
            }

            if (c.name == "powerofthewild")// macht der wildnis with summoning
            {
                if (choice == 1)
                {
                    foreach (Minion m in this.ownMinions)
                    {
                        minionGetBuffed(m, 1, 1, true);
                    }
                }
                if (choice == 2)
                {
                    int posi = this.ownMinions.Count - 1;
                    CardDB.Card kid = CardDB.Instance.getCardDataFromID("EX1_160t");//panther
                    callKid(kid, posi, true);
                }
            }

            if (c.name == "starfall")
            {
                if (choice == 2)
                {
                    List<Minion> temp = new List<Minion>(this.enemyMinions);
                    int damage = getSpellDamageDamage(2);
                    foreach (Minion enemy in temp)
                    {
                        minionGetDamagedOrHealed(enemy, damage, 0, false);
                    }
                }

            }

            if (c.name == "nourish")
            {
                if (choice == 1)
                {
                    if (this.ownMaxMana == 10)
                    {
                        //this.owncarddraw++;
                        this.drawACard("excessmana", true);
                    }
                    else
                    {
                        this.ownMaxMana++;
                        this.mana++;
                    }
                    if (this.ownMaxMana == 10)
                    {
                        //this.owncarddraw++;
                        this.drawACard("excessmana", true);
                    }
                    else
                    {
                        this.ownMaxMana++;
                        this.mana++;
                    }
                }
                if (choice == 2)
                {
                    //this.owncarddraw+=3;
                    this.drawACard("", true);
                    this.drawACard("", true);
                    this.drawACard("", true);
                }
            }

            if (c.name == "savageroar")
            {
                List<Minion> temp = new List<Minion>(this.ownMinions);
                Enchantment e = CardDB.getEnchantmentFromCardID("CS2_011o");
                e.creator = hc.entity;
                e.controllerOfCreator = this.ownController;
                foreach (Minion m in temp)
                {
                    addEffectToMinionNoDoubles(m, e, true);
                }
                this.ownheroAngr += 2;
            }

            //special cards#######################

            if (c.CardID == "PRO_001a")// i am murloc
            {
                int posi = this.ownMinions.Count - 1;
                CardDB.Card kid = CardDB.Instance.getCardDataFromID("PRO_001at");//panther
                callKid(kid, posi, true);
                callKid(kid, posi, true);
                callKid(kid, posi, true);

            }

            if (c.CardID == "PRO_001c")// i am murloc
            {
                int posi = this.ownMinions.Count - 1;
                CardDB.Card kid = CardDB.Instance.getCardDataFromID("EX1_021");//scharfseher
                callKid(kid, posi, true);

            }

            if (c.name == "wildgrowth")
            {
                if (this.ownMaxMana == 10)
                {
                    //this.owncarddraw++;
                    this.drawACard("excessmana", true);
                }
                else
                {
                    this.ownMaxMana++;
                }

            }

            if (c.name == "excessmana")
            {
                //this.owncarddraw++;
                this.drawACard("", true);
            }

            if (c.name == "yseraawakens")
            {
                List<Minion> temp = new List<Minion>(this.enemyMinions);
                int damage = getSpellDamageDamage(5);
                foreach (Minion enemy in temp)
                {
                    if (enemy.name != "ysera")// dont attack ysera
                    {
                        minionGetDamagedOrHealed(enemy, damage, 0, false);
                    }
                }
                temp.Clear();
                temp.AddRange(this.ownMinions);
                foreach (Minion enemy in temp)
                {
                    if (enemy.name != "ysera")//dont attack ysera
                    {
                        minionGetDamagedOrHealed(enemy, damage, 0, false);
                    }
                }
                attackOrHealHero(damage, true);
                attackOrHealHero(damage, false);

            }

            if (c.name == "stomp")
            {
                List<Minion> temp = new List<Minion>(this.enemyMinions);
                int damage = getSpellDamageDamage(2);
                foreach (Minion enemy in temp)
                {
                    minionGetDamagedOrHealed(enemy, damage, 0, false);
                }

            }

            //NaxxCards###################################################################################

            if (c.name == "poisonseeds")
            {
                int ownanz = this.ownMinions.Count;
                int enemanz = this.enemyMinions.Count;
                foreach (Minion mnn in this.ownMinions)
                {
                    minionGetDestroyed(mnn, true);
                }
                foreach (Minion mnn in this.enemyMinions)
                {
                    minionGetDestroyed(mnn, false);
                }
                for (int i = 0; i < ownanz; i++)
                {
                    CardDB.Card d = CardDB.Instance.getCardDataFromID("EX1_158t");
                    callKid(d, 0, true);
                }
                for (int i = 0; i < enemanz; i++)
                {
                    CardDB.Card d = CardDB.Instance.getCardDataFromID("EX1_158t");
                    callKid(d, 0, false);
                }
            }

        }

        private void drawACard(string ss, bool own)
        {
            string s = ss;
            if (s == "") s = "unknown";
            if (s == "enemycard") s = "unknown"; // NO PENALITY FOR DRAWING TO MUCH CARDS

            // cant hold more than 10 cards

            if (own)
            {
                if (s == "unknown") // draw a card from deck :D
                {
                    if (ownDeckSize == 0)
                    {
                        this.ownHeroFatigue++;
                        this.attackOrHealHero(this.ownHeroFatigue, true);
                    }
                    else
                    {
                        this.ownDeckSize--;
                        if (this.owncards.Count >= 10)
                        {
                            this.evaluatePenality += 5;
                            return;
                        }
                        this.owncarddraw++;
                    }

                }
                else
                {
                    if (this.owncards.Count >= 10)
                    {
                        this.evaluatePenality += 5;
                        return;
                    }
                    this.owncarddraw++;

                }


            }
            else
            {
                if (s == "unknown") // draw a card from deck :D
                {
                    if (enemyDeckSize == 0)
                    {
                        this.enemyHeroFatigue++;
                        this.attackOrHealHero(this.enemyHeroFatigue, false);
                    }
                    else
                    {
                        this.enemyDeckSize--;
                        if (this.enemyAnzCards + this.enemycarddraw >= 10)
                        {
                            this.evaluatePenality += 5;
                            return;
                        }
                        this.enemycarddraw++;
                    }

                }
                else
                {
                    if (this.enemyAnzCards + this.enemycarddraw >= 10)
                    {
                        this.evaluatePenality += 5;
                        return;
                    }
                    this.enemycarddraw++;

                }
                return;
            }

            if (s == "unknown")
            {
                CardDB.Card plchldr = new CardDB.Card();
                plchldr.name = "unknown";
                Handmanager.Handcard hc = new Handmanager.Handcard();
                hc.card = plchldr;
                hc.position = this.owncards.Count + 1;
                hc.manacost = 1000;
                this.owncards.Add(hc);
            }
            else
            {
                CardDB.Card c = CardDB.Instance.getCardData(s);
                Handmanager.Handcard hc = new Handmanager.Handcard();
                hc.card = c;
                hc.position = this.owncards.Count + 1;
                hc.manacost = c.calculateManaCost(this);
                this.owncards.Add(hc);
            }

        }

        private void triggerPlayedAMinion(Handmanager.Handcard hc, bool own)
        {
            if (own) // effects only for OWN minons
            {
                List<Minion> tempo = new List<Minion>(this.ownMinions);
                foreach (Minion m in tempo)
                {
                    if (m.silenced) continue;

                    if (m.handcard.card.specialMin == CardDB.specialMinions.knifejuggler && m.entitiyID != hc.entity)
                    {
                        if (this.enemyMinions.Count >= 1)
                        {
                            List<Minion> temp = new List<Minion>();
                            int damage = 1;
                            List<Minion> temp2 = new List<Minion>(this.enemyMinions);
                            temp2.Sort((a, b) => -a.Hp.CompareTo(b.Hp));
                            bool dmgdone = false;
                            foreach (Minion enemy in temp2)
                            {
                                if (enemy.Hp > 1)
                                {
                                    minionGetDamagedOrHealed(enemy, damage, 0, false);
                                    dmgdone = true;
                                    break;
                                }
                                if (!dmgdone) this.attackOrHealHero(1, false);
                            }


                        }
                        else
                        {
                            this.attackOrHealHero(1, false);
                        }
                    }

                    if (own && m.handcard.card.specialMin == CardDB.specialMinions.starvingbuzzard && (TAG_RACE)hc.card.race == TAG_RACE.PET && m.entitiyID != hc.entity)
                    {
                        //this.owncarddraw++;
                        this.drawACard("", true);
                    }

                    if (own && m.handcard.card.specialMin == CardDB.specialMinions.undertaker && hc.card.deathrattle)
                    {
                        minionGetBuffed(m, 1, 1, own);
                    }

                }


            }


            //effects for ALL minons
            List<Minion> tempoo = new List<Minion>(this.ownMinions);
            foreach (Minion m in tempoo)
            {
                if (m.silenced) continue;
                if (m.handcard.card.specialMin == CardDB.specialMinions.murloctidecaller && hc.card.race == 14 && m.entitiyID != hc.entity)
                {
                    minionGetBuffed(m, 1, 0, true);
                }
                if (m.handcard.card.specialMin == CardDB.specialMinions.oldmurkeye && hc.card.race == 14 && m.entitiyID != hc.entity)
                {
                    minionGetBuffed(m, 1, 0, true);
                }
            }
            tempoo.Clear();
            tempoo.AddRange(this.enemyMinions);
            foreach (Minion m in tempoo)
            {
                if (m.silenced) continue;
                //truebaugederalte
                if (m.handcard.card.specialMin == CardDB.specialMinions.murloctidecaller && hc.card.race == 14 && m.entitiyID != hc.entity)
                {
                    minionGetBuffed(m, 1, 0, false);
                }
                if (m.handcard.card.specialMin == CardDB.specialMinions.oldmurkeye && hc.card.race == 14 && m.entitiyID != hc.entity)
                {
                    minionGetBuffed(m, 1, 0, false);
                }
            }


        }

        private void triggerPlayedASpell(CardDB.Card c)
        {

            bool wilderpyro = false;
            List<Minion> temp = new List<Minion>(this.ownMinions);
            foreach (Minion m in temp)
            {
                if (m.silenced) continue;

                if (m.handcard.card.specialMin == CardDB.specialMinions.manawyrm)
                {
                    minionGetBuffed(m, 1, 0, true);
                }

                if (m.handcard.card.specialMin == CardDB.specialMinions.manaaddict)
                {
                    minionGetBuffed(m, 2, 0, true);
                }

                if (m.handcard.card.specialMin == CardDB.specialMinions.secretkeeper && c.Secret)
                {
                    minionGetBuffed(m, 1, 1, true);
                }

                if (m.handcard.card.specialMin == CardDB.specialMinions.archmageantonidas)
                {
                    drawACard("fireball", true);
                }

                if (m.handcard.card.specialMin == CardDB.specialMinions.violetteacher)
                {

                    CardDB.Card d = CardDB.Instance.getCardDataFromID("NEW1_026t");//violetapprentice
                    callKid(d, m.id, true);
                }

                if (m.handcard.card.specialMin == CardDB.specialMinions.gadgetzanauctioneer)
                {
                    //this.owncarddraw++;
                    drawACard("", true);
                }
                if (m.handcard.card.specialMin == CardDB.specialMinions.wildpyromancer)
                {
                    wilderpyro = true;
                }
            }

            foreach (Minion m in this.enemyMinions)
            {

                if (m.handcard.card.specialMin == CardDB.specialMinions.secretkeeper && c.Secret)
                {
                    minionGetBuffed(m, 1, 1, true);
                }
            }

            if (wilderpyro)
            {
                temp.Clear();
                temp.AddRange(this.ownMinions);
                foreach (Minion m in temp)
                {
                    if (m.silenced) continue;

                    if (m.handcard.card.specialMin == CardDB.specialMinions.wildpyromancer)
                    {
                        List<Minion> temp2 = new List<Minion>(this.ownMinions);
                        foreach (Minion mnn in temp2)
                        {
                            minionGetDamagedOrHealed(mnn, 1, 0, true);
                        }
                        temp2.Clear();
                        temp2.AddRange(this.enemyMinions);
                        foreach (Minion mnn in temp2)
                        {
                            minionGetDamagedOrHealed(mnn, 1, 0, false);
                        }
                    }
                }
            }

        }

        public void removeCard(Handmanager.Handcard hcc)
        {

            this.owncards.RemoveAll(x => x.entity == hcc.entity);
            int i = 1;
            foreach (Handmanager.Handcard hc in this.owncards)
            {
                hc.position = i;
                i++;
            }

        }

        public void playCard(Handmanager.Handcard hc, int cardpos, int cardEntity, int target, int targetEntity, int choice, int placepos, int penality)
        {
            CardDB.Card c = hc.card;
            this.evaluatePenality += penality;
            // lock at frostnova (click) / frostblitz (no click)
            this.mana = this.mana - hc.getManaCost(this);

            removeCard(hc);// remove card

            if (c.Secret)
            {
                this.ownSecretsIDList.Add(c.CardID);
                this.playedmagierinderkirintor = false;
            }
            if (c.type == CardDB.cardtype.SPELL) this.playedPreparation = false;

            //Helpfunctions.Instance.logg("play crd " + c.name + " entitiy# " + cardEntity + " mana " + hc.getManaCost(this) + " trgt " + target);
            if (logging) Helpfunctions.Instance.logg("play crd " + c.name + " entitiy# " + cardEntity + " mana " + hc.getManaCost(this) + " trgt " + target);

            if (c.type == CardDB.cardtype.MOB)
            {
                Action b = this.placeAmobSomewhere(hc, cardpos, target, choice, placepos);
                b.handcard = new Handmanager.Handcard(hc);
                b.druidchoice = choice;
                b.owntarget = placepos;
                b.enemyEntitiy = targetEntity;
                b.cardEntitiy = cardEntity;
                this.playactions.Add(b);
                this.mobsplayedThisTurn++;
                if (c.name == "kirintormage") this.playedmagierinderkirintor = true;

            }
            else
            {
                Action a = new Action();
                a.cardplay = true;
                a.handcard = new Handmanager.Handcard(hc);
                a.cardEntitiy = cardEntity;
                a.numEnemysBeforePlayed = this.enemyMinions.Count;
                a.comboBeforePlayed = (this.cardsPlayedThisTurn >= 1) ? true : false;
                a.owntarget = 0;
                if (target >= 0)
                {
                    a.owntarget = -1;
                }
                a.enemytarget = target;
                a.enemyEntitiy = targetEntity;
                a.druidchoice = choice;

                if (target == -1)
                {
                    //card with no target
                    if (c.type == CardDB.cardtype.WEAPON)
                    {
                        equipWeapon(c);
                    }
                    playCardWithoutTarget(hc, choice);
                }
                else //before : if(target >=0 && target < 20)
                {
                    if (c.type == CardDB.cardtype.WEAPON)
                    {
                        equipWeapon(c);
                    }
                    playCardWithTarget(hc, target, choice);
                }

                this.playactions.Add(a);

                if (c.type == CardDB.cardtype.SPELL)
                {
                    this.triggerPlayedASpell(c);
                }
            }

            triggerACardGetPlayed(c);

            this.ueberladung += c.recallValue;

            this.cardsPlayedThisTurn++;

        }

        private void triggerACardGetPlayed(CardDB.Card c)
        {
            List<Minion> temp = new List<Minion>(this.ownMinions);
            foreach (Minion mnn in temp)
            {
                if (mnn.silenced) continue;
                if (mnn.handcard.card.specialMin == CardDB.specialMinions.illidanstormrage)
                {
                    CardDB.Card d = CardDB.Instance.getCardDataFromID("EX1_614t");//flameofazzinoth
                    callKid(d, mnn.id, true);
                }
                if (mnn.handcard.card.specialMin == CardDB.specialMinions.questingadventurer)
                {
                    minionGetBuffed(mnn, 1, 1, true);
                }
                if (mnn.handcard.card.specialMin == CardDB.specialMinions.unboundelemental && c.recallValue >= 1)
                {
                    minionGetBuffed(mnn, 1, 1, true);
                }
            }
        }

        public void attackWithWeapon(int target, int targetEntity, int penality)
        {
            //this.ownHeroAttackedInRound = true;
            this.ownHeroNumAttackThisTurn++;
            if ((this.ownHeroWindfury && this.ownHeroNumAttackThisTurn == 2) || (!this.ownHeroWindfury && this.ownHeroNumAttackThisTurn == 1))
            {
                this.ownHeroReady = false;
            }
            Action a = new Action();
            a.heroattack = true;
            a.enemytarget = target;
            a.enemyEntitiy = targetEntity;
            a.owntarget = 100;
            a.ownEntitiy = this.ownHeroEntity;
            a.numEnemysBeforePlayed = this.enemyMinions.Count;
            a.comboBeforePlayed = (this.cardsPlayedThisTurn >= 1) ? true : false;
            this.playactions.Add(a);

            if (this.ownWeaponName == "truesilverchampion")
            {
                this.attackOrHealHero(-2, true);
            }

            if (logging) Helpfunctions.Instance.logg("attck with weapon " + a.owntarget + " " + a.ownEntitiy + " trgt: " + a.enemytarget + " " + a.enemyEntitiy);

            if (target == 200)
            {
                attackOrHealHero(this.ownheroAngr, false);
            }
            else
            {

                Minion enemy = this.enemyMinions[target - 10];

                int enem_attack = enemy.Angr;

                minionGetDamagedOrHealed(enemy, this.ownheroAngr, 0, false);

                if (!this.heroImmuneWhileAttacking)
                {
                    attackOrHealHero(enem_attack, true);
                    if (!enemy.silenced && enemy.handcard.card.specialMin == CardDB.specialMinions.waterelemental)
                    {
                        this.ownHeroFrozen = true;
                    }
                }
            }

            //todo
            if (ownWeaponName == "gorehowl" && target != 200)
            {
                this.ownWeaponAttack--;
                this.ownheroAngr--;
            }
            else
            {
                this.lowerWeaponDurability(1, true);
            }

        }

        public void ENEMYattackWithWeapon(int target, int targetEntity, int penality)
        {
            //this.ownHeroAttackedInRound = true;
            this.enemyHeroNumAttackThisTurn++;
            if ((this.enemyHeroWindfury && this.enemyHeroNumAttackThisTurn == 2) || (!this.enemyHeroWindfury && this.enemyHeroNumAttackThisTurn == 1))
            {
                this.enemyHeroReady = false;
            }

            if (this.enemyWeaponName == "truesilverchampion")
            {
                this.attackOrHealHero(-2, false);
            }

            if (logging) Helpfunctions.Instance.logg("enemy attck with weapon trgt: " + target + " " + targetEntity);

            if (target == 100)
            {
                attackOrHealHero(this.enemyheroAngr, true);
            }
            else
            {

                Minion enemy = this.ownMinions[target];
                minionGetDamagedOrHealed(enemy, this.enemyheroAngr, 0, true);

                if (!this.enemyheroImmuneWhileAttacking)
                {
                    attackOrHealHero(enemy.Angr, false);
                    if (!enemy.silenced && enemy.handcard.card.specialMin == CardDB.specialMinions.waterelemental)
                    {
                        this.enemyHeroFrozen = true;
                    }
                }
            }

            //todo
            if (enemyWeaponName == "gorehowl" && target != 100)
            {
                this.enemyWeaponAttack--;
                this.enemyheroAngr--;
            }
            else
            {
                this.lowerWeaponDurability(1, false);
            }

        }

        public void activateAbility(CardDB.Card c, int target, int targetEntity, int penality)
        {
            this.evaluatePenality += penality;
            HeroEnum heroname = this.ownHeroName;
            this.ownAbilityReady = false;
            this.mana -= 2;
            Action a = new Action();
            a.useability = true;
            a.handcard = new Handmanager.Handcard(c);
            a.enemytarget = target;
            a.enemyEntitiy = targetEntity;
            a.numEnemysBeforePlayed = this.enemyMinions.Count;
            a.comboBeforePlayed = (this.cardsPlayedThisTurn >= 1) ? true : false;
            this.playactions.Add(a);
            if (logging) Helpfunctions.Instance.logg("play ability on target " + target);

            if (heroname == HeroEnum.mage)
            {
                int damage = 1;
                if (target == 100)
                {
                    attackOrHealHero(damage, true);
                }
                else
                {
                    if (target == 200)
                    {
                        attackOrHealHero(damage, false);
                    }
                    else
                    {
                        if (target < 10)
                        {
                            Minion m = this.ownMinions[target];
                            this.minionGetDamagedOrHealed(m, damage, 0, true);
                        }

                        if (target >= 10 && target < 20)
                        {
                            Minion m = this.enemyMinions[target - 10];
                            this.minionGetDamagedOrHealed(m, damage, 0, false);
                        }
                    }
                }

            }

            if (heroname == HeroEnum.priest)
            {
                int heal = 2;
                if (this.auchenaiseelenpriesterin) heal = -2;

                if (c.name == "mindspike")
                {
                    heal = -1 * 2;
                }
                if (c.name == "mindshatter")
                {
                    heal = -1 * 3;
                }

                if (target == 100)
                {
                    attackOrHealHero(-1 * heal, true);
                }
                else
                {
                    if (target == 200)
                    {
                        attackOrHealHero(-1 * heal, false);
                    }
                    else
                    {
                        if (target < 10)
                        {
                            Minion m = this.ownMinions[target];
                            this.minionGetDamagedOrHealed(m, 0, heal, true);
                        }

                        if (target >= 10 && target < 20)
                        {
                            Minion m = this.enemyMinions[target - 10];
                            this.minionGetDamagedOrHealed(m, 0, heal, false);
                        }
                    }
                }

            }

            if (heroname == HeroEnum.warrior)
            {
                this.ownHeroDefence += 2;
            }

            if (heroname == HeroEnum.warlock)
            {
                //this.owncarddraw++;
                this.drawACard("", true);
                this.attackOrHealHero(2, true);
            }


            if (heroname == HeroEnum.thief)
            {

                CardDB.Card wcard = CardDB.Instance.getCardDataFromID("CS2_082");
                this.equipWeapon(wcard);
            }

            if (heroname == HeroEnum.druid)
            {
                this.ownheroAngr += 1;
                if ((this.ownHeroNumAttackThisTurn == 0 || (this.ownHeroWindfury && this.ownHeroNumAttackThisTurn == 1)) && !this.ownHeroFrozen)
                {
                    this.ownHeroReady = true;
                }
                this.ownHeroDefence += 1;
            }


            if (heroname == HeroEnum.hunter)
            {
                this.attackOrHealHero(2, false);
            }

            if (heroname == HeroEnum.pala)
            {
                int posi = this.ownMinions.Count - 1;
                CardDB.Card kid = CardDB.Instance.getCardDataFromID("CS2_101t");//silverhandrecruit
                callKid(kid, posi, true);
            }

            if (heroname == HeroEnum.shaman)
            {
                int posi = this.ownMinions.Count - 1;
                CardDB.Card kid = CardDB.Instance.getCardDataFromID("NEW1_009");//healingtotem
                callKid(kid, posi, true);
            }

            if (heroname == HeroEnum.lordjaraxxus)
            {
                int posi = this.ownMinions.Count - 1;
                CardDB.Card kid = CardDB.Instance.getCardDataFromID("EX1_tk34");//infernal
                callKid(kid, posi, true);
            }


        }

        public void ENEMYactivateAbility(CardDB.Card c, int target, int targetEntity)
        {
            HeroEnum heroname = this.enemyHeroName;
            this.enemyAbilityReady = false;
            if (logging) Helpfunctions.Instance.logg("enemy play ability on target " + target);

            if (heroname == HeroEnum.mage)
            {
                int damage = 1;
                if (target == 100)
                {
                    attackOrHealHero(damage, true);
                }
                else
                {
                    if (target == 200)
                    {
                        attackOrHealHero(damage, false);
                    }
                    else
                    {
                        if (target < 10)
                        {
                            Minion m = this.ownMinions[target];
                            this.minionGetDamagedOrHealed(m, damage, 0, true);
                        }

                        if (target >= 10 && target < 20)
                        {
                            Minion m = this.enemyMinions[target - 10];
                            this.minionGetDamagedOrHealed(m, damage, 0, false);
                        }
                    }
                }

            }

            if (heroname == HeroEnum.priest)
            {
                int heal = 2;
                if (this.auchenaiseelenpriesterin) heal = -2;

                if (c.name == "mindspike")
                {
                    heal = -1 * 2;
                }
                if (c.name == "mindshatter")
                {
                    heal = -1 * 3;
                }

                if (target == 100)
                {
                    if (heal >= 1) return;
                    attackOrHealHero(-1 * heal, true);
                }
                else
                {
                    if (target == 200)
                    {
                        if (heal >= 1)
                        {
                            bool haslightwarden = false;
                            foreach (Minion mnn in this.enemyMinions)
                            {
                                if (mnn.handcard.card.specialMin == CardDB.specialMinions.lightwarden)
                                {
                                    haslightwarden = true;
                                    break;
                                }
                            }
                            if (!haslightwarden) return;
                        }
                        attackOrHealHero(-1 * heal, false);
                    }
                    else
                    {
                        if (target < 10)
                        {
                            Minion m = this.ownMinions[target];
                            this.minionGetDamagedOrHealed(m, 0, heal, true);
                        }

                        if (target >= 10 && target < 20)
                        {
                            Minion m = this.enemyMinions[target - 10];
                            this.minionGetDamagedOrHealed(m, 0, heal, false);
                        }
                    }
                }

            }

            if (heroname == HeroEnum.warrior)
            {
                this.enemyHeroDefence += 2;
            }

            if (heroname == HeroEnum.warlock)
            {
                //this.owncarddraw++;
                this.drawACard("", false);
                this.attackOrHealHero(2, false);
            }


            if (heroname == HeroEnum.thief)
            {

                CardDB.Card wcard = CardDB.Instance.getCardDataFromID("CS2_082");//wickedknife
                this.enemyheroAngr = wcard.Attack;
                this.enemyWeaponAttack = wcard.Attack;
                this.enemyWeaponDurability = wcard.Durability;
                this.enemyHeroWindfury = false;
                if ((this.enemyHeroNumAttackThisTurn == 0 || (this.enemyHeroWindfury && this.enemyHeroNumAttackThisTurn == 1)) && !this.enemyHeroFrozen)
                {
                    this.enemyHeroReady = true;
                }
            }

            if (heroname == HeroEnum.druid)
            {
                this.enemyheroAngr += 1;
                if ((this.enemyHeroNumAttackThisTurn == 0 || (this.enemyHeroWindfury && this.enemyHeroNumAttackThisTurn == 1)) && !this.enemyHeroFrozen)
                {
                    this.enemyHeroReady = true;
                }
                this.enemyHeroDefence += 1;
            }


            if (heroname == HeroEnum.hunter)
            {
                this.attackOrHealHero(2, true);
            }

            if (heroname == HeroEnum.pala)
            {
                int posi = this.enemyMinions.Count - 1;
                CardDB.Card kid = CardDB.Instance.getCardDataFromID("CS2_101t");//silverhandrecruit
                callKid(kid, posi, false);
            }

            if (heroname == HeroEnum.shaman)
            {
                int posi = this.enemyMinions.Count - 1;
                CardDB.Card kid = CardDB.Instance.getCardDataFromID("CS2_050");//searingtotem
                callKid(kid, posi, false);
            }

            if (heroname == HeroEnum.lordjaraxxus)
            {
                int posi = this.enemyMinions.Count - 1;
                CardDB.Card kid = CardDB.Instance.getCardDataFromID("EX1_tk34");//infernal
                callKid(kid, posi, false);
            }


        }

        public void doAction()
        {
            /*if (this.playactions.Count >= 1)
            {
                Action a = this.playactions[0];

                if (a.cardplay)
                {
                    if (logging) help.logg("play " + a.handcard.card.name);
                    if (logging) help.logg("with position " + a.cardplace.X + "," + a.cardplace.Y);
                    help.clicklauf(a.cardplace.X, a.cardplace.Y);
                    if (a.owntarget >= 0)
                    {
                        if (logging) help.logg("on position " + a.ownplace.X + "," + a.ownplace.Y);
                        help.clicklauf(a.ownplace.X, a.ownplace.Y);
                    }
                    if (a.enemytarget >= 0)
                    {
                        if (logging) help.logg("and target to " + a.enemytarget + ": on " + a.targetplace.X + ", " + a.targetplace.Y);
                        help.clicklauf(a.targetplace.X, a.targetplace.Y);
                    }
                }
                if (a.minionplay)
                {
                    if (logging) help.logg("attacker: " + a.owntarget + " enemy: " + a.enemytarget);
                    help.clicklauf(a.ownplace.X, a.ownplace.Y);
                    System.Threading.Thread.Sleep(500);
                    if (logging) help.logg("targetplace " + a.targetplace.X + ", " + a.targetplace.Y);
                    help.clicklauf(a.targetplace.X, a.targetplace.Y);
                }
                if (a.heroattack)
                {
                    if (logging) help.logg("attack with hero, enemy: " + a.enemytarget);
                    help.clicklauf(a.ownplace.X, a.ownplace.Y);
                    if (logging) help.logg("targetplace " + a.targetplace.X + ", " + a.targetplace.Y);
                    help.clicklauf(a.targetplace.X, a.targetplace.Y);
                }
                if (a.useability)
                {
                    if (logging) help.logg("useability ");
                    help.clicklauf(a.ownplace.X, a.ownplace.Y);
                    if (a.enemytarget >= 0)
                    {
                        if (logging) help.logg("on enemy: " + a.enemytarget + "targetplace " + a.targetplace.X + ", " + a.targetplace.Y);
                        help.clicklauf(a.targetplace.X, a.targetplace.Y);
                    }
                }

            }
            else
            {
                // click endturnbutton
                help.clicklauf(939, 353);
            }
            help.laufmaus(915, 400, 6);
             */
        }


        private void debugMinions()
        {
            Helpfunctions.Instance.logg("OWN MINIONS################");

            foreach (Minion m in this.ownMinions)
            {
                Helpfunctions.Instance.logg("name,ang, hp, maxhp: " + m.name + ", " + m.Angr + ", " + m.Hp + ", " + m.maxHp);
                foreach (Enchantment e in m.enchantments)
                {
                    Helpfunctions.Instance.logg("enchment: " + e.CARDID + " " + e.creator + " " + e.controllerOfCreator);
                }
            }

            Helpfunctions.Instance.logg("ENEMY MINIONS############");
            foreach (Minion m in this.enemyMinions)
            {
                Helpfunctions.Instance.logg("name,ang, hp: " + m.name + ", " + m.Angr + ", " + m.Hp);
            }
        }

        public void printBoard()
        {
            Helpfunctions.Instance.logg("board: " + value);
            Helpfunctions.Instance.logg("pen " + this.evaluatePenality);
            Helpfunctions.Instance.logg("cardsplayed: " + this.cardsPlayedThisTurn + " handsize: " + this.owncards.Count);
            Helpfunctions.Instance.logg("ownhero: ");
            Helpfunctions.Instance.logg("ownherohp: " + this.ownHeroHp + " + " + this.ownHeroDefence);
            Helpfunctions.Instance.logg("ownheroattac: " + this.ownheroAngr);
            Helpfunctions.Instance.logg("ownheroweapon: " + this.ownWeaponAttack + " " + this.ownWeaponDurability + " " + this.ownWeaponName);
            Helpfunctions.Instance.logg("ownherostatus: frozen" + this.ownHeroFrozen + " ");
            Helpfunctions.Instance.logg("enemyherohp: " + this.enemyHeroHp + " + " + this.enemyHeroDefence);
            Helpfunctions.Instance.logg("OWN MINIONS################");

            foreach (Minion m in this.ownMinions)
            {
                Helpfunctions.Instance.logg("name,ang, hp: " + m.name + ", " + m.Angr + ", " + m.Hp);
                foreach (Enchantment e in m.enchantments)
                {
                    Helpfunctions.Instance.logg("enchment " + e.CARDID + " " + e.creator + " " + e.controllerOfCreator);
                }
            }

            Helpfunctions.Instance.logg("ENEMY MINIONS############");
            foreach (Minion m in this.enemyMinions)
            {
                Helpfunctions.Instance.logg("name,ang, hp: " + m.name + ", " + m.Angr + ", " + m.Hp);
            }


            Helpfunctions.Instance.logg("");
        }

        public Action getNextAction()
        {
            if (this.playactions.Count >= 1) return this.playactions[0];
            return null;
        }

        public void printActions()
        {
            foreach (Action a in this.playactions)
            {
                if (a.cardplay)
                {
                    Helpfunctions.Instance.logg("play " + a.handcard.card.name);
                    if (a.druidchoice >= 1) Helpfunctions.Instance.logg("choose choise " + a.druidchoice);
                    Helpfunctions.Instance.logg("with entity " + a.cardEntitiy);
                    if (a.owntarget >= 0)
                    {
                        Helpfunctions.Instance.logg("on position " + a.owntarget);
                    }
                    if (a.enemytarget >= 0)
                    {
                        Helpfunctions.Instance.logg("and target to " + a.enemytarget + " " + a.enemyEntitiy);
                    }
                }
                if (a.minionplay)
                {
                    Helpfunctions.Instance.logg("attacker: " + a.owntarget + " enemy: " + a.enemytarget);
                    Helpfunctions.Instance.logg("targetplace " + a.enemyEntitiy);
                }
                if (a.heroattack)
                {
                    Helpfunctions.Instance.logg("attack with hero, enemy: " + a.enemytarget);
                    Helpfunctions.Instance.logg("targetplace " + a.enemyEntitiy);
                }
                if (a.useability)
                {
                    Helpfunctions.Instance.logg("useability ");
                    if (a.enemytarget >= 0)
                    {
                        Helpfunctions.Instance.logg("on enemy: " + a.enemytarget + "targetplace " + a.enemyEntitiy);
                    }
                }
                Helpfunctions.Instance.logg("");
            }
        }

    }

    public class Ai
    {
        private int maxdeep = 12;
        private int maxwide = 3000;
        public bool simulateEnemyTurn = true;
        private bool usePenalityManager = true;
        private bool useCutingTargets = true;
        private bool dontRecalc = true;
        private bool useLethalCheck = true;
        private bool useComparison = true;

        public MiniSimulator nextTurnSimulator;
        MiniSimulator mainTurnSimulator;

        PenalityManager penman = PenalityManager.Instance;

        List<Playfield> posmoves = new List<Playfield>(7000);

        Hrtprozis hp = Hrtprozis.Instance;
        Handmanager hm = Handmanager.Instance;
        Helpfunctions help = Helpfunctions.Instance;

        public Action bestmove = new Action();
        public int bestmoveValue = 0;
        Playfield bestboard = new Playfield();
        Playfield nextMoveGuess = new Playfield();
        public Bot botBase = null;

        private bool secondturnsim = false;
        private bool playaround = false;

        private static Ai instance;

        public static Ai Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Ai();
                }
                return instance;
            }
        }

        private Ai()
        {
            this.nextMoveGuess = new Playfield();
            this.nextMoveGuess.mana = -1;
            this.nextTurnSimulator = new MiniSimulator();
            this.mainTurnSimulator = new MiniSimulator(maxdeep, maxwide, 0); // 0 for unlimited
            this.mainTurnSimulator.setPrintingstuff(true);
        }

        public void setMaxWide(int mw)
        {
            this.maxwide = mw;
            if (maxwide <= 100) this.maxwide = 100;
            this.mainTurnSimulator.updateParams(maxdeep, maxwide, 0);
        }

        public void setTwoTurnSimulation(bool stts)
        {
            this.mainTurnSimulator.setSecondTurnSimu(stts);
            this.secondturnsim = stts;
        }

        public void setPlayAround(bool spa)
        {
            this.mainTurnSimulator.setPlayAround(spa);
            this.playaround = spa;
        }

        private void doallmoves(bool test, bool isLethalCheck)
        {
            this.mainTurnSimulator.doallmoves(this.posmoves[0], isLethalCheck);

            Playfield bestplay = this.mainTurnSimulator.bestboard;
            int bestval = this.mainTurnSimulator.bestmoveValue;

            help.loggonoff(true);
            help.logg("-------------------------------------");
            help.logg("bestPlayvalue " + bestval);

            bestplay.printActions();
            this.bestmove = bestplay.getNextAction();
            this.bestmoveValue = bestval;
            this.bestboard = new Playfield(bestplay);

            if (bestmove != null) // save the guessed move, so we doesnt need to recalc!
            {
                this.nextMoveGuess = new Playfield();
                if (bestmove.cardplay)
                {
                    //pf.playCard(c, hc.position - 1, hc.entity, trgt.target, trgt.targetEntity, 0, bestplace, cardplayPenality);
                    Handmanager.Handcard hc = this.nextMoveGuess.owncards.Find(x => x.entity == bestmove.cardEntitiy);

                    if (bestmove.owntarget >= 0 && bestmove.enemytarget >= 0 && bestmove.enemytarget <= 9 && bestmove.owntarget < bestmove.enemytarget)
                    {
                        this.nextMoveGuess.playCard(bestmove.handcard, hc.position - 1, hc.entity, bestmove.enemytarget - 1, bestmove.enemyEntitiy, bestmove.druidchoice, bestmove.owntarget, 0);
                    }
                    else
                    {
                        this.nextMoveGuess.playCard(bestmove.handcard, hc.position - 1, hc.entity, bestmove.enemytarget, bestmove.enemyEntitiy, bestmove.druidchoice, bestmove.owntarget, 0);
                    }
                    //this.nextMoveGuess.playCard(bestmove.card, hc.position - 1, hc.entity, bestmove.enemytarget, bestmove.enemyEntitiy, bestmove.druidchoice, bestmove.owntarget, 0);

                }

                if (bestmove.minionplay)
                {
                    //.attackWithMinion(m, trgt.target, trgt.targetEntity, attackPenality);
                    Minion m = this.nextMoveGuess.ownMinions.Find(x => x.entitiyID == bestmove.ownEntitiy);
                    this.nextMoveGuess.attackWithMinion(m, bestmove.enemytarget, bestmove.enemyEntitiy, 0);

                }

                if (bestmove.heroattack)
                {
                    this.nextMoveGuess.attackWithWeapon(bestmove.enemytarget, bestmove.enemyEntitiy, 0);
                }

                if (bestmove.useability)
                {
                    //.activateAbility(p.ownHeroAblility, trgt.target, trgt.targetEntity, abilityPenality);
                    this.nextMoveGuess.activateAbility(this.nextMoveGuess.ownHeroAblility, bestmove.enemytarget, bestmove.enemyEntitiy, 0);
                }

                this.bestboard.playactions.RemoveAt(0);
            }
            else
            {
                nextMoveGuess.mana = -1;
            }

        }

        private void doNextCalcedMove()
        {
            help.logg("noRecalcNeeded!!!-----------------------------------");
            this.bestboard.printActions();
            this.bestmove = this.bestboard.getNextAction();

            if (bestmove != null) // save the guessed move, so we doesnt need to recalc!
            {
                this.nextMoveGuess = new Playfield();
                if (bestmove.cardplay)
                {
                    //pf.playCard(c, hc.position - 1, hc.entity, trgt.target, trgt.targetEntity, 0, bestplace, cardplayPenality);
                    Handmanager.Handcard hc = this.nextMoveGuess.owncards.Find(x => x.entity == bestmove.cardEntitiy);
                    if (bestmove.owntarget >= 0 && bestmove.enemytarget >= 0 && bestmove.enemytarget <= 9 && bestmove.owntarget < bestmove.enemytarget)
                    {
                        this.nextMoveGuess.playCard(bestmove.handcard, hc.position - 1, hc.entity, bestmove.enemytarget - 1, bestmove.enemyEntitiy, bestmove.druidchoice, bestmove.owntarget, 0);
                    }
                    else
                    {
                        this.nextMoveGuess.playCard(bestmove.handcard, hc.position - 1, hc.entity, bestmove.enemytarget, bestmove.enemyEntitiy, bestmove.druidchoice, bestmove.owntarget, 0);
                    }


                }

                if (bestmove.minionplay)
                {
                    //.attackWithMinion(m, trgt.target, trgt.targetEntity, attackPenality);
                    Minion m = this.nextMoveGuess.ownMinions.Find(x => x.entitiyID == bestmove.ownEntitiy);
                    this.nextMoveGuess.attackWithMinion(m, bestmove.enemytarget, bestmove.enemyEntitiy, 0);

                }

                if (bestmove.heroattack)
                {
                    this.nextMoveGuess.attackWithWeapon(bestmove.enemytarget, bestmove.enemyEntitiy, 0);
                }

                if (bestmove.useability)
                {
                    //.activateAbility(p.ownHeroAblility, trgt.target, trgt.targetEntity, abilityPenality);
                    this.nextMoveGuess.activateAbility(this.nextMoveGuess.ownHeroAblility, bestmove.enemytarget, bestmove.enemyEntitiy, 0);
                }

                this.bestboard.playactions.RemoveAt(0);
            }
            else
            {
                nextMoveGuess.mana = -1;
            }

        }

        public void dosomethingclever(Bot bbase)
        {
            //return;
            //turncheck
            //help.moveMouse(950,750);
            //help.Screenshot();
            this.botBase = bbase;
            hp.updatePositions();

            posmoves.Clear();
            posmoves.Add(new Playfield());
            posmoves[0].sEnemTurn = this.simulateEnemyTurn;
            /* foreach (var item in this.posmoves[0].owncards)
             {
                 help.logg("card " + item.handcard.card.name + " is playable :" + item.handcard.card.canplayCard(posmoves[0]) + " cost/mana: " + item.handcard.card.cost + "/" + posmoves[0].mana);
             }
             */
            //help.logg("is hero ready?" + posmoves[0].ownHeroReady);

            help.loggonoff(false);
            //do we need to recalc?
            help.logg("recalc-check###########");
            if (this.dontRecalc && posmoves[0].isEqual(this.nextMoveGuess, true))
            {
                doNextCalcedMove();
            }
            else
            {
                help.logg("Leathal-check###########");
                bestmoveValue = -1000000;
                DateTime strt = DateTime.Now;
                if (useLethalCheck)
                {
                    strt = DateTime.Now;
                    doallmoves(false, true);
                    help.logg("calculated " + (DateTime.Now - strt).TotalSeconds);
                }

                if (bestmoveValue < 10000)
                {
                    posmoves.Clear();
                    posmoves.Add(new Playfield());
                    posmoves[0].sEnemTurn = this.simulateEnemyTurn;
                    help.logg("no lethal, do something random######");
                    strt = DateTime.Now;
                    doallmoves(false, false);
                    help.logg("calculated " + (DateTime.Now - strt).TotalSeconds);

                }
            }


            //help.logging(true);

        }

        public void autoTester(Bot bbase, bool printstuff)
        {
            help.logg("simulating board ");

            BoardTester bt = new BoardTester();
            this.botBase = bbase;
            hp.printHero();
            hp.printOwnMinions();
            hp.printEnemyMinions();
            hm.printcards();
            //calculate the stuff
            posmoves.Clear();
            posmoves.Add(new Playfield());
            posmoves[0].sEnemTurn = this.simulateEnemyTurn;
            foreach (Playfield p in this.posmoves)
            {
                p.printBoard();
            }
            help.logg("ownminionscount " + posmoves[0].ownMinions.Count);
            help.logg("owncardscount " + posmoves[0].owncards.Count);

            foreach (var item in this.posmoves[0].owncards)
            {
                help.logg("card " + item.card.name + " is playable :" + item.canplayCard(posmoves[0]) + " cost/mana: " + item.manacost + "/" + posmoves[0].mana);
            }
            help.logg("ability " + posmoves[0].ownHeroAblility.name + " is playable :" + posmoves[0].ownHeroAblility.canplayCard(posmoves[0], 2) + " cost/mana: " + posmoves[0].ownHeroAblility.getManaCost(posmoves[0], 2) + "/" + posmoves[0].mana);

            // lethalcheck + normal
            DateTime strt = DateTime.Now;
            doallmoves(false, true);
            help.logg("calculated " + (DateTime.Now - strt).TotalSeconds);
            if (bestmoveValue < 10000)
            {
                posmoves.Clear();
                posmoves.Add(new Playfield());
                posmoves[0].sEnemTurn = this.simulateEnemyTurn;
                strt = DateTime.Now;
                doallmoves(false, false);
                help.logg("calculated " + (DateTime.Now - strt).TotalSeconds);
            }

            this.mainTurnSimulator.printPosmoves();

            help.logg("bestfield");
            bestboard.printBoard();
            if (printstuff) simmulateWholeTurn();
        }

        public void simmulateWholeTurn()
        {
            help.logg("simulate best board");
            //this.bestboard.printActions();

            Playfield tempbestboard = new Playfield();

            if (bestmove != null) // save the guessed move, so we doesnt need to recalc!
            {
                bestmove.print();
                if (bestmove.cardplay)
                {
                    help.logg("card");
                    //pf.playCard(c, hc.position - 1, hc.entity, trgt.target, trgt.targetEntity, 0, bestplace, cardplayPenality);
                    Handmanager.Handcard hc = tempbestboard.owncards.Find(x => x.entity == bestmove.cardEntitiy);
                    if (bestmove.owntarget >= 0 && bestmove.enemytarget >= 0 && bestmove.enemytarget <= 9 && bestmove.owntarget < bestmove.enemytarget)
                    {
                        tempbestboard.playCard(bestmove.handcard, hc.position - 1, hc.entity, bestmove.enemytarget - 1, bestmove.enemyEntitiy, bestmove.druidchoice, bestmove.owntarget, 0);
                    }
                    else
                    {
                        tempbestboard.playCard(bestmove.handcard, hc.position - 1, hc.entity, bestmove.enemytarget, bestmove.enemyEntitiy, bestmove.druidchoice, bestmove.owntarget, 0);
                    }
                }

                if (bestmove.minionplay)
                {
                    help.logg("min");
                    //.attackWithMinion(m, trgt.target, trgt.targetEntity, attackPenality);
                    Minion mm = tempbestboard.ownMinions.Find(x => x.entitiyID == bestmove.ownEntitiy);
                    help.logg("min");
                    tempbestboard.attackWithMinion(mm, bestmove.enemytarget, bestmove.enemyEntitiy, 0);
                    help.logg("min");
                }

                if (bestmove.heroattack)
                {
                    help.logg("hero");
                    tempbestboard.attackWithWeapon(bestmove.enemytarget, bestmove.enemyEntitiy, 0);
                }

                if (bestmove.useability)
                {
                    help.logg("abi");
                    //.activateAbility(p.ownHeroAblility, trgt.target, trgt.targetEntity, abilityPenality);
                    tempbestboard.activateAbility(this.nextMoveGuess.ownHeroAblility, bestmove.enemytarget, bestmove.enemyEntitiy, 0);
                }

            }
            else
            {
                tempbestboard.mana = -1;
            }
            help.logg("-------------");
            help.logg("OwnMinions:");


            foreach (Minion m in tempbestboard.ownMinions)
            {
                help.logg(m.name + " id " + m.id + " zp " + m.zonepos + " " + " e:" + m.entitiyID + " " + " A:" + m.Angr + " H:" + m.Hp + " mH:" + m.maxHp + " rdy:" + m.Ready + " tnt:" + m.taunt + " frz:" + m.frozen + " silenced:" + m.silenced + " divshield:" + m.divineshild + " ptt:" + m.playedThisTurn + " wndfr:" + m.windfury + " natt:" + m.numAttacksThisTurn + " sil:" + m.silenced + " stl:" + m.stealth + " poi:" + m.poisonous + " imm:" + m.immune + " ex:" + m.exhausted + " chrg:" + m.charge);
                foreach (Enchantment e in m.enchantments)
                {
                    help.logg(e.CARDID + " " + e.creator + " " + e.controllerOfCreator);
                }
            }
            help.logg("EnemyMinions:");
            foreach (Minion m in tempbestboard.enemyMinions)
            {
                help.logg(m.name + " id " + m.id + " zp " + m.zonepos + " " + " e:" + m.entitiyID + " " + " A:" + m.Angr + " H:" + m.Hp + " mH:" + m.maxHp + " rdy:" + m.Ready + " tnt:" + m.taunt + " frz:" + m.frozen + " silenced:" + m.silenced + " divshield:" + m.divineshild + " wndfr:" + m.windfury + " sil:" + m.silenced + " stl:" + m.stealth + " poi:" + m.poisonous + " imm:" + m.immune + " ex:" + m.exhausted);
                foreach (Enchantment e in m.enchantments)
                {
                    help.logg(e.CARDID + " " + e.creator + " " + e.controllerOfCreator);
                }
            }


            foreach (Action bestmovee in bestboard.playactions)
            {

                help.logg("stepp");


                if (bestmovee != null) // save the guessed move, so we doesnt need to recalc!
                {
                    bestmovee.print();
                    if (bestmovee.cardplay)
                    {
                        help.logg("card");
                        //pf.playCard(c, hc.position - 1, hc.entity, trgt.target, trgt.targetEntity, 0, bestplace, cardplayPenality);
                        Handmanager.Handcard hc = tempbestboard.owncards.Find(x => x.entity == bestmovee.cardEntitiy);
                        if (bestmovee.owntarget >= 0 && bestmovee.enemytarget >= 0 && bestmovee.enemytarget <= 9 && bestmovee.owntarget < bestmovee.enemytarget)
                        {
                            tempbestboard.playCard(bestmovee.handcard, hc.position - 1, hc.entity, bestmovee.enemytarget - 1, bestmovee.enemyEntitiy, bestmovee.druidchoice, bestmovee.owntarget, 0);
                        }
                        else
                        {
                            tempbestboard.playCard(bestmovee.handcard, hc.position - 1, hc.entity, bestmovee.enemytarget, bestmovee.enemyEntitiy, bestmovee.druidchoice, bestmovee.owntarget, 0);
                        }
                    }

                    if (bestmovee.minionplay)
                    {
                        help.logg("min");
                        //.attackWithMinion(m, trgt.target, trgt.targetEntity, attackPenality);
                        Minion mm = tempbestboard.ownMinions.Find(x => x.entitiyID == bestmovee.ownEntitiy);
                        help.logg("min");
                        tempbestboard.attackWithMinion(mm, bestmovee.enemytarget, bestmovee.enemyEntitiy, 0);
                        help.logg("min");
                    }

                    if (bestmovee.heroattack)
                    {
                        help.logg("hero");
                        tempbestboard.attackWithWeapon(bestmovee.enemytarget, bestmovee.enemyEntitiy, 0);
                    }

                    if (bestmovee.useability)
                    {
                        help.logg("abi");
                        //.activateAbility(p.ownHeroAblility, trgt.target, trgt.targetEntity, abilityPenality);
                        tempbestboard.activateAbility(this.nextMoveGuess.ownHeroAblility, bestmovee.enemytarget, bestmovee.enemyEntitiy, 0);
                    }

                }
                else
                {
                    tempbestboard.mana = -1;
                }
                help.logg("-------------");
                help.logg("OwnMinions:");
                foreach (Minion m in tempbestboard.ownMinions)
                {
                    help.logg(m.name + " id " + m.id + " zp " + m.zonepos + " " + " e:" + m.entitiyID + " " + " A:" + m.Angr + " H:" + m.Hp + " mH:" + m.maxHp + " rdy:" + m.Ready + " tnt:" + m.taunt + " frz:" + m.frozen + " silenced:" + m.silenced + " divshield:" + m.divineshild + " ptt:" + m.playedThisTurn + " wndfr:" + m.windfury + " natt:" + m.numAttacksThisTurn + " sil:" + m.silenced + " stl:" + m.stealth + " poi:" + m.poisonous + " imm:" + m.immune + " ex:" + m.exhausted + " chrg:" + m.charge);
                    foreach (Enchantment e in m.enchantments)
                    {
                        help.logg(e.CARDID + " " + e.creator + " " + e.controllerOfCreator);
                    }
                }
                help.logg("EnemyMinions:");
                foreach (Minion m in tempbestboard.enemyMinions)
                {
                    help.logg(m.name + " id " + m.id + " zp " + m.zonepos + " " + " e:" + m.entitiyID + " " + " A:" + m.Angr + " H:" + m.Hp + " mH:" + m.maxHp + " rdy:" + m.Ready + " tnt:" + m.taunt + " frz:" + m.frozen + " silenced:" + m.silenced + " divshield:" + m.divineshild + " wndfr:" + m.windfury + " sil:" + m.silenced + " stl:" + m.stealth + " poi:" + m.poisonous + " imm:" + m.immune + " ex:" + m.exhausted);
                    foreach (Enchantment e in m.enchantments)
                    {
                        help.logg(e.CARDID + " " + e.creator + " " + e.controllerOfCreator);
                    }
                }
            }

            help.logg("AFTER ENEMY TURN:");
            tempbestboard.sEnemTurn = this.simulateEnemyTurn;
            tempbestboard.endTurn(this.secondturnsim, this.playaround, true);
        }

    }

    public class MiniSimulator
    {
        //#####################################################################################################################
        private int maxdeep = 6;
        private int maxwide = 10;
        private int totalboards = 50;
        private bool usePenalityManager = true;
        private bool useCutingTargets = true;
        private bool dontRecalc = true;
        private bool useLethalCheck = true;
        private bool useComparison = true;

        private bool printNormalstuff = false;

        List<Playfield> posmoves = new List<Playfield>(7000);

        public Action bestmove = new Action();
        public int bestmoveValue = 0;
        public Playfield bestboard = new Playfield();

        public Bot botBase = null;
        private int calculated = 0;

        private bool simulateSecondTurn = false;
        private bool playaround = false;

        PenalityManager pen = PenalityManager.Instance;

        public MiniSimulator()
        {
        }
        public MiniSimulator(int deep, int wide, int ttlboards)
        {
            this.maxdeep = deep;
            this.maxwide = wide;
            this.totalboards = ttlboards;
        }

        public void updateParams(int deep, int wide, int ttlboards)
        {
            this.maxdeep = deep;
            this.maxwide = wide;
            this.totalboards = ttlboards;
        }

        public void setPrintingstuff(bool sp)
        {
            this.printNormalstuff = sp;
        }

        public void setSecondTurnSimu(bool sts)
        {
            this.simulateSecondTurn = sts;
        }

        public void setPlayAround(bool spa)
        {
            this.playaround = spa;
        }

        private void addToPosmoves(Playfield pf)
        {
            if (pf.ownHeroHp <= 0) return;
            /*foreach (Playfield p in this.posmoves)
            {
                if (pf.isEqual(p, false)) return;
            }*/
            this.posmoves.Add(pf);
            //posmoves.Sort((a, b) => -(botBase.getPlayfieldValue(a)).CompareTo(botBase.getPlayfieldValue(b)));//want to keep the best
            //if (posmoves.Count > this.maxwide) posmoves.RemoveAt(this.maxwide);
            if (this.totalboards >= 1)
            {
                this.calculated++;
            }
        }

        private bool doAllChoices(Playfield p, Handmanager.Handcard hc, bool lethalcheck)
        {
            bool havedonesomething = false;

            for (int i = 1; i < 3; i++)
            {
                CardDB.Card c = hc.card;
                if (c.name == "keeperofthegrove")
                {
                    if (i == 1)
                    {
                        c = CardDB.Instance.getCardDataFromID("EX1_166a");
                    }
                    if (i == 2)
                    {
                        c = CardDB.Instance.getCardDataFromID("EX1_166b");
                    }
                }

                if (c.name == "starfall")
                {
                    if (i == 1)
                    {
                        c = CardDB.Instance.getCardDataFromID("NEW1_007b");
                    }
                    if (i == 2)
                    {
                        c = CardDB.Instance.getCardDataFromID("NEW1_007a");
                    }
                }

                if (c.name == "ancientoflore")
                {
                    if (i == 1)
                    {
                        c = CardDB.Instance.getCardDataFromID("NEW1_008a");
                    }
                    if (i == 2)
                    {
                        c = CardDB.Instance.getCardDataFromID("NEW1_008b");
                    }
                }

                if (c.name == "powerofthewild")
                {
                    if (i == 1)
                    {
                        c = CardDB.Instance.getCardDataFromID("EX1_160b");
                    }
                    if (i == 2)
                    {
                        c = CardDB.Instance.getCardDataFromID("EX1_160a");
                    }
                }
                if (c.name == "ancientofwar")
                {
                    if (i == 1)
                    {
                        c = CardDB.Instance.getCardDataFromID("EX1_178a");
                    }
                    if (i == 2)
                    {
                        c = CardDB.Instance.getCardDataFromID("EX1_178b");
                    }
                }
                if (c.name == "druidoftheclaw")
                {
                    if (i == 1)
                    {
                        c = CardDB.Instance.getCardDataFromID("EX1_165t1");
                    }
                    if (i == 2)
                    {
                        c = CardDB.Instance.getCardDataFromID("EX1_165t2");
                    }
                }
                //cenarius dont need
                if (c.name == "keeperofthegrove")//keeper of the grove
                {
                    if (i == 1)
                    {
                        c = CardDB.Instance.getCardDataFromID("EX1_166a");
                    }
                    if (i == 2)
                    {
                        c = CardDB.Instance.getCardDataFromID("EX1_166b");
                    }
                }
                if (c.name == "markofnature")
                {
                    if (i == 1)
                    {
                        c = CardDB.Instance.getCardDataFromID("EX1_155a");
                    }
                    if (i == 2)
                    {
                        c = CardDB.Instance.getCardDataFromID("EX1_155b");
                    }
                }
                if (c.name == "nourish")
                {
                    if (i == 1)
                    {
                        c = CardDB.Instance.getCardDataFromID("EX1_164a");
                    }
                    if (i == 2)
                    {
                        c = CardDB.Instance.getCardDataFromID("EX1_164b");
                    }
                }
                if (c.name == "wrath")
                {
                    if (i == 1)
                    {
                        c = CardDB.Instance.getCardDataFromID("EX1_154a");
                    }
                    if (i == 2)
                    {
                        c = CardDB.Instance.getCardDataFromID("EX1_154b");
                    }
                }

                if (c.canplayCard(p, hc.manacost))
                {
                    havedonesomething = true;



                    int bestplace = p.getBestPlace(c, lethalcheck);
                    List<targett> trgts = c.getTargetsForCard(p);
                    int cardplayPenality = 0;
                    if (trgts.Count == 0)
                    {


                        if (usePenalityManager)
                        {
                            cardplayPenality = pen.getPlayCardPenality(hc.card, -1, p, i, lethalcheck);
                            if (cardplayPenality <= 499)
                            {
                                //help.logg(hc.card.name + " is played");
                                Playfield pf = new Playfield(p);
                                pf.playCard(hc, hc.position - 1, hc.entity, -1, -1, i, bestplace, cardplayPenality);
                                addToPosmoves(pf);
                            }
                        }
                        else
                        {
                            Playfield pf = new Playfield(p);
                            pf.playCard(hc, hc.position - 1, hc.entity, -1, -1, i, bestplace, cardplayPenality);
                            addToPosmoves(pf);
                        }

                    }
                    else
                    {
                        foreach (targett trgt in trgts)
                        {

                            if (usePenalityManager)
                            {
                                cardplayPenality = pen.getPlayCardPenality(hc.card, trgt.target, p, 0, lethalcheck);
                                if (cardplayPenality <= 499)
                                {
                                    //help.logg(hc.card.name + " is played");
                                    Playfield pf = new Playfield(p);
                                    pf.playCard(hc, hc.position - 1, hc.entity, trgt.target, trgt.targetEntity, i, bestplace, cardplayPenality);
                                    addToPosmoves(pf);
                                }
                            }
                            else
                            {
                                Playfield pf = new Playfield(p);
                                pf.playCard(hc, hc.position - 1, hc.entity, trgt.target, trgt.targetEntity, i, bestplace, cardplayPenality);
                                addToPosmoves(pf);
                            }

                        }
                    }

                }

            }


            return havedonesomething;
        }

        public int doallmoves(Playfield playf, bool isLethalCheck)
        {
            //Helpfunctions.Instance.logg("NXTTRN" + playf.mana);
            if (botBase == null) botBase = Ai.Instance.botBase;
            bool test = false;
            this.posmoves.Clear();
            this.addToPosmoves(playf);
            bool havedonesomething = true;
            List<Playfield> temp = new List<Playfield>();
            int deep = 0;
            //Helpfunctions.Instance.logg("NXTTRN" + playf.mana + " " + posmoves.Count);
            this.calculated = 0;
            while (havedonesomething)
            {
                if (this.printNormalstuff) Helpfunctions.Instance.logg("ailoop");
                GC.Collect();
                temp.Clear();
                temp.AddRange(this.posmoves);
                havedonesomething = false;
                Playfield bestold = null;
                int bestoldval = -20000000;
                foreach (Playfield p in temp)
                {

                    if (p.complete || p.ownHeroHp <= 0)
                    {
                        continue;
                    }

                    //take a card and play it
                    List<string> playedcards = new List<string>();

                    foreach (Handmanager.Handcard hc in p.owncards)
                    {
                        if (this.calculated > this.totalboards) continue;
                        CardDB.Card c = hc.card;
                        //help.logg("try play crd" + c.name + " " + c.getManaCost(p) + " " + c.canplayCard(p));
                        if (playedcards.Contains(c.name)) continue; // dont play the same card in one loop
                        playedcards.Add(c.name);
                        if (c.choice)
                        {
                            if (doAllChoices(p, hc, isLethalCheck))
                            {
                                havedonesomething = true;
                            }
                        }
                        else
                        {
                            int bestplace = p.getBestPlace(c, isLethalCheck);
                            if (hc.canplayCard(p))
                            {
                                havedonesomething = true;
                                List<targett> trgts = c.getTargetsForCard(p);

                                if (isLethalCheck && (pen.DamageTargetDatabase.ContainsKey(c.name) || pen.DamageTargetSpecialDatabase.ContainsKey(c.name)))// only target enemy hero during Lethal check!
                                {
                                    targett trg = trgts.Find(x => x.target == 200);
                                    if (trg != null)
                                    {
                                        trgts.Clear();
                                        trgts.Add(trg);
                                    }
                                    else
                                    {
                                        // no enemy hero -> enemy have taunts ->kill the taunts from left to right
                                        if (trgts.Count >= 1)
                                        {
                                            trg = trgts[0];
                                            trgts.Clear();
                                            trgts.Add(trg);
                                        }
                                    }
                                }


                                int cardplayPenality = 0;

                                if (trgts.Count == 0)
                                {


                                    if (usePenalityManager)
                                    {
                                        cardplayPenality = pen.getPlayCardPenality(c, -1, p, 0, isLethalCheck);
                                        if (cardplayPenality <= 499)
                                        {
                                            Playfield pf = new Playfield(p);
                                            havedonesomething = true;
                                            pf.playCard(hc, hc.position - 1, hc.entity, -1, -1, 0, bestplace, cardplayPenality);
                                            addToPosmoves(pf);
                                        }
                                    }
                                    else
                                    {
                                        Playfield pf = new Playfield(p);
                                        havedonesomething = true;
                                        pf.playCard(hc, hc.position - 1, hc.entity, -1, -1, 0, bestplace, cardplayPenality);
                                        addToPosmoves(pf);
                                    }


                                }
                                else
                                {
                                    if (isLethalCheck)// only target enemy hero during Lethal check!
                                    {
                                        targett trg = trgts.Find(x => x.target == 200);
                                        if (trg != null)
                                        {
                                            trgts.Clear();
                                            trgts.Add(trg);
                                        }
                                    }

                                    foreach (targett trgt in trgts)
                                    {


                                        if (usePenalityManager)
                                        {
                                            cardplayPenality = pen.getPlayCardPenality(c, trgt.target, p, 0, isLethalCheck);
                                            if (cardplayPenality <= 499)
                                            {
                                                Playfield pf = new Playfield(p);
                                                havedonesomething = true;
                                                pf.playCard(hc, hc.position - 1, hc.entity, trgt.target, trgt.targetEntity, 0, bestplace, cardplayPenality);
                                                addToPosmoves(pf);
                                            }
                                        }
                                        else
                                        {
                                            Playfield pf = new Playfield(p);
                                            havedonesomething = true;
                                            pf.playCard(hc, hc.position - 1, hc.entity, trgt.target, trgt.targetEntity, 0, bestplace, cardplayPenality);
                                            addToPosmoves(pf);
                                        }

                                    }

                                }


                            }
                        }
                    }

                    //attack with a minion

                    List<Minion> playedMinions = new List<Minion>(8);

                    foreach (Minion m in p.ownMinions)
                    {
                        if (this.calculated > this.totalboards) continue;

                        if (m.Ready && m.Angr >= 1 && !m.frozen)
                        {
                            //BEGIN:cut (double/similar) attacking minions out#####################################
                            // DONT LET SIMMILAR MINIONS ATTACK IN ONE TURN (example 3 unlesh the hounds-hounds doesnt need to simulated hole)
                            List<Minion> tempoo = new List<Minion>(playedMinions);
                            bool dontattacked = true;
                            bool isSpecial = pen.specialMinions.ContainsKey(m.name);
                            foreach (Minion mnn in tempoo)
                            {
                                // special minions are allowed to attack in silended and unsilenced state!
                                //help.logg(mnn.silenced + " " + m.silenced + " " + mnn.name + " " + m.name + " " + penman.specialMinions.ContainsKey(m.name));

                                bool otherisSpecial = pen.specialMinions.ContainsKey(mnn.name);

                                if ((!isSpecial || (isSpecial && m.silenced)) && (!otherisSpecial || (otherisSpecial && mnn.silenced))) // both are not special, if they are the same, dont add
                                {
                                    if (mnn.Angr == m.Angr && mnn.Hp == m.Hp && mnn.divineshild == m.divineshild && mnn.taunt == m.taunt && mnn.poisonous == m.poisonous) dontattacked = false;
                                    continue;
                                }

                                if (isSpecial == otherisSpecial && !m.silenced && !mnn.silenced) // same are special
                                {
                                    if (m.name != mnn.name) // different name -> take it
                                    {
                                        continue;
                                    }
                                    // same name -> test whether they are equal
                                    if (mnn.Angr == m.Angr && mnn.Hp == m.Hp && mnn.divineshild == m.divineshild && mnn.taunt == m.taunt && mnn.poisonous == m.poisonous) dontattacked = false;
                                    continue;
                                }

                            }

                            if (dontattacked)
                            {
                                playedMinions.Add(m);
                            }
                            else
                            {
                                //help.logg(m.name + " doesnt need to attack!");
                                continue;
                            }
                            //END: cut (double/similar) attacking minions out#####################################

                            //help.logg(m.name + " is going to attack!");
                            List<targett> trgts = p.getAttackTargets(true);


                            if (isLethalCheck)// only target enemy hero during Lethal check!
                            {
                                targett trg = trgts.Find(x => x.target == 200);
                                if (trg != null)
                                {
                                    trgts.Clear();
                                    trgts.Add(trg);
                                }
                                else
                                {
                                    // no enemy hero -> enemy have taunts ->kill the taunts from left to right
                                    if (trgts.Count >= 1)
                                    {
                                        trg = trgts[0];
                                        trgts.Clear();
                                        trgts.Add(trg);
                                    }
                                }
                            }
                            else
                            {
                                if (this.useCutingTargets) trgts = this.cutAttackTargets(trgts, p, true);
                            }

                            foreach (targett trgt in trgts)
                            {


                                int attackPenality = 0;

                                if (usePenalityManager)
                                {
                                    attackPenality = pen.getAttackWithMininonPenality(m, p, trgt.target, isLethalCheck);
                                    if (attackPenality <= 499)
                                    {
                                        Playfield pf = new Playfield(p);
                                        havedonesomething = true;
                                        pf.attackWithMinion(m, trgt.target, trgt.targetEntity, attackPenality);
                                        addToPosmoves(pf);
                                    }
                                }
                                else
                                {
                                    Playfield pf = new Playfield(p);
                                    havedonesomething = true;
                                    pf.attackWithMinion(m, trgt.target, trgt.targetEntity, attackPenality);
                                    addToPosmoves(pf);
                                }


                            }
                            if (trgts.Count == 1 && trgts[0].target == 200)//only enemy hero is available als attack
                            {
                                break;
                            }
                        }

                    }

                    // attack with hero
                    if (p.ownHeroReady)
                    {
                        if (this.calculated > this.totalboards) continue;
                        List<targett> trgts = p.getAttackTargets(true);

                        havedonesomething = true;

                        if (isLethalCheck)// only target enemy hero during Lethal check!
                        {
                            targett trg = trgts.Find(x => x.target == 200);
                            if (trg != null)
                            {
                                trgts.Clear();
                                trgts.Add(trg);
                            }
                            else
                            {
                                // no enemy hero -> enemy have taunts ->kill the taunts from left to right
                                if (trgts.Count >= 1)
                                {
                                    trg = trgts[0];
                                    trgts.Clear();
                                    trgts.Add(trg);
                                }
                            }
                        }
                        else
                        {
                            if (this.useCutingTargets) trgts = this.cutAttackTargets(trgts, p, true);
                        }

                        foreach (targett trgt in trgts)
                        {
                            Playfield pf = new Playfield(p);
                            int heroAttackPen = 0;
                            if (usePenalityManager)
                            {
                                heroAttackPen = pen.getAttackWithHeroPenality(trgt.target, p);
                            }
                            pf.attackWithWeapon(trgt.target, trgt.targetEntity, heroAttackPen);
                            addToPosmoves(pf);
                        }
                    }

                    // use ability
                    /// TODO check if ready after manaup
                    if (p.ownAbilityReady && p.mana >= 2 && p.ownHeroAblility.canplayCard(p, 2))
                    {
                        if (this.calculated > this.totalboards) continue;
                        int abilityPenality = 0;

                        havedonesomething = true;
                        // if we have mage or priest, we have to target something####################################################
                        if (p.ownHeroName == HeroEnum.mage || p.ownHeroName == HeroEnum.priest)
                        {

                            List<targett> trgts = p.ownHeroAblility.getTargetsForCard(p);

                            if (isLethalCheck && (p.ownHeroName == HeroEnum.mage || (p.ownHeroName == HeroEnum.priest && p.ownHeroAblility.name != "lesserheal")))// only target enemy hero during Lethal check!
                            {
                                targett trg = trgts.Find(x => x.target == 200);
                                if (trg != null)
                                {
                                    trgts.Clear();
                                    trgts.Add(trg);
                                }
                                else
                                {
                                    // no enemy hero -> enemy have taunts ->kill the taunts from left to right
                                    if (trgts.Count >= 1)
                                    {
                                        trg = trgts[0];
                                        trgts.Clear();
                                        trgts.Add(trg);
                                    }
                                }
                            }

                            foreach (targett trgt in trgts)
                            {



                                if (usePenalityManager)
                                {
                                    abilityPenality = pen.getPlayCardPenality(p.ownHeroAblility, trgt.target, p, 0, isLethalCheck);
                                    if (abilityPenality <= 499)
                                    {
                                        Playfield pf = new Playfield(p);
                                        havedonesomething = true;
                                        pf.activateAbility(p.ownHeroAblility, trgt.target, trgt.targetEntity, abilityPenality);
                                        addToPosmoves(pf);
                                    }
                                }
                                else
                                {
                                    Playfield pf = new Playfield(p);
                                    havedonesomething = true;
                                    pf.activateAbility(p.ownHeroAblility, trgt.target, trgt.targetEntity, abilityPenality);
                                    addToPosmoves(pf);
                                }

                            }
                        }
                        else
                        {
                            // the other classes dont have to target####################################################
                            Playfield pf = new Playfield(p);

                            if (usePenalityManager)
                            {
                                abilityPenality = pen.getPlayCardPenality(p.ownHeroAblility, -1, pf, 0, isLethalCheck);
                                if (abilityPenality <= 499)
                                {
                                    havedonesomething = true;
                                    pf.activateAbility(p.ownHeroAblility, -1, -1, abilityPenality);
                                    addToPosmoves(pf);
                                }
                            }
                            else
                            {
                                havedonesomething = true;
                                pf.activateAbility(p.ownHeroAblility, -1, -1, abilityPenality);
                                addToPosmoves(pf);
                            }

                        }

                    }


                    if (isLethalCheck)
                    {
                        p.complete = true;
                    }
                    else
                    {
                        p.endTurn(this.simulateSecondTurn, this.playaround);
                    }

                    //sort stupid stuff ouf

                    if (botBase.getPlayfieldValue(p) > bestoldval)
                    {
                        bestoldval = botBase.getPlayfieldValue(p);
                        bestold = p;
                    }
                    if (!test)
                    {
                        posmoves.Remove(p);
                    }

                    if (this.calculated > this.totalboards) break;
                }

                if (!test && bestoldval >= -10000 && bestold != null)
                {
                    this.posmoves.Add(bestold);
                }

                //Helpfunctions.Instance.loggonoff(true);
                if (this.printNormalstuff)
                {
                    int donec = 0;
                    foreach (Playfield p in posmoves)
                    {
                        if (p.complete) donec++;
                    }
                    Helpfunctions.Instance.logg("deep " + deep + " len " + this.posmoves.Count + " dones " + donec);
                }

                if (!test)
                {
                    cuttingposibilities();
                }

                if (this.printNormalstuff)
                {
                    Helpfunctions.Instance.logg("cut to len " + this.posmoves.Count);
                }
                //Helpfunctions.Instance.loggonoff(false);
                deep++;

                if (this.calculated > this.totalboards) break;
                if (deep >= this.maxdeep) break;//remove this?
            }

            foreach (Playfield p in posmoves)//temp
            {
                if (!p.complete)
                {
                    if (isLethalCheck)
                    {
                        p.complete = true;
                    }
                    else
                    {
                        p.endTurn(this.simulateSecondTurn, this.playaround);
                    }
                }
            }
            // Helpfunctions.Instance.logg("find best ");
            if (posmoves.Count >= 1)
            {
                int bestval = int.MinValue;
                int bestanzactions = 1000;
                Playfield bestplay = posmoves[0];//temp[0]
                foreach (Playfield p in posmoves)//temp
                {
                    int val = botBase.getPlayfieldValue(p);
                    if (bestval <= val)
                    {
                        if (bestval == val && bestanzactions < p.playactions.Count) continue;
                        bestplay = p;
                        bestval = val;
                        bestanzactions = p.playactions.Count;
                    }

                }

                this.bestmove = bestplay.getNextAction();
                this.bestmoveValue = bestval;
                this.bestboard = new Playfield(bestplay);
                //Helpfunctions.Instance.logg("return");
                return bestval;
            }
            //Helpfunctions.Instance.logg("return");
            this.bestmove = null;
            this.bestmoveValue = -100000;
            this.bestboard = playf;
            return -10000;
        }

        public void cuttingposibilities()
        {
            // take the x best values
            int takenumber = this.maxwide;
            List<Playfield> temp = new List<Playfield>();
            posmoves.Sort((a, b) => -(botBase.getPlayfieldValue(a)).CompareTo(botBase.getPlayfieldValue(b)));//want to keep the best

            if (this.useComparison)
            {
                int i = 0;
                foreach (Playfield p in posmoves)
                {
                    bool found = false;
                    foreach (Playfield pp in temp)
                    {
                        if (pp.isEqual(p, false))
                        {
                            found = true;
                            break;
                        }
                    }
                    if (!found) temp.Add(p);
                    i++;
                    if (i >= this.maxwide) break;

                }
            }
            else
            {
                temp.AddRange(posmoves);
            }

            posmoves.Clear();
            posmoves.AddRange(temp.GetRange(0, Math.Min(takenumber, temp.Count)));
            //posmoves.Clear();
            //posmoves.AddRange(Helpfunctions.TakeList(temp, takenumber));

        }

        public List<targett> cutAttackTargets(List<targett> oldlist, Playfield p, bool own)
        {
            List<targett> retvalues = new List<targett>();
            List<Minion> addedmins = new List<Minion>(8);

            bool priomins = false;
            List<targett> retvaluesPrio = new List<targett>();
            foreach (targett t in oldlist)
            {
                if ((own && t.target == 200) || (!own && t.target == 100))
                {
                    retvalues.Add(t);
                    continue;
                }
                if ((own && t.target >= 10 && t.target <= 19) || (!own && t.target >= 0 && t.target <= 9))
                {
                    Minion m = null;
                    if (own) m = p.enemyMinions[t.target - 10];
                    if (!own) m = p.ownMinions[t.target];
                    /*if (penman.priorityDatabase.ContainsKey(m.name))
                    {
                        //retvalues.Add(t);
                        retvaluesPrio.Add(t);
                        priomins = true;
                        //help.logg(m.name + " is added to targetlist");
                        continue;
                    }*/


                    bool goingtoadd = true;
                    List<Minion> temp = new List<Minion>(addedmins);
                    bool isSpecial = pen.specialMinions.ContainsKey(m.name);
                    foreach (Minion mnn in temp)
                    {
                        // special minions are allowed to attack in silended and unsilenced state!
                        //help.logg(mnn.silenced + " " + m.silenced + " " + mnn.name + " " + m.name + " " + penman.specialMinions.ContainsKey(m.name));

                        bool otherisSpecial = pen.specialMinions.ContainsKey(mnn.name);

                        if ((!isSpecial || (isSpecial && m.silenced)) && (!otherisSpecial || (otherisSpecial && mnn.silenced))) // both are not special, if they are the same, dont add
                        {
                            if (mnn.Angr == m.Angr && mnn.Hp == m.Hp && mnn.divineshild == m.divineshild && mnn.taunt == m.taunt && mnn.poisonous == m.poisonous) goingtoadd = false;
                            continue;
                        }

                        if (isSpecial == otherisSpecial && !m.silenced && !mnn.silenced) // same are special
                        {
                            if (m.name != mnn.name) // different name -> take it
                            {
                                continue;
                            }
                            // same name -> test whether they are equal
                            if (mnn.Angr == m.Angr && mnn.Hp == m.Hp && mnn.divineshild == m.divineshild && mnn.taunt == m.taunt && mnn.poisonous == m.poisonous) goingtoadd = false;
                            continue;
                        }

                    }

                    if (goingtoadd)
                    {
                        addedmins.Add(m);
                        retvalues.Add(t);
                        //help.logg(m.name + " " + m.id +" is added to targetlist");
                    }
                    else
                    {
                        //help.logg(m.name + " is not needed to attack");
                        continue;
                    }

                }
            }
            //help.logg("end targetcutting");
            if (priomins) return retvaluesPrio;

            return retvalues;
        }

        public void printPosmoves()
        {
            foreach (Playfield p in this.posmoves)
            {
                p.printBoard();
            }
        }

    }

    public class Handmanager
    {
        public class Cardsposi
        {
            public int Amount = 0;
            public int DetectPosix = 0;
            public int DetectPosiy = 0;
            public int cardsHooverPosx = 0;
            public int cardsHooverdiff = 0;
            public int cardsBigPosx = 0;
            public int cardsBigdiff = 0;
            public int hoovery = 750;
            public int bigreadydetecty = 510;

        }

        public class Handcard
        {
            public int position = 0;
            public int entity = -1;
            public int manacost = 1000;
            public CardDB.Card card;

            public Handcard()
            {
                card = new CardDB.Card();
            }
            public Handcard(Handcard hc)
            {
                this.position = hc.position;
                this.entity = hc.entity;
                this.manacost = hc.manacost;
                this.card = hc.card;
            }
            public Handcard(CardDB.Card c)
            {
                this.position = 0;
                this.entity = -1;
                this.card = c;
            }
            public int getManaCost(Playfield p)
            {
                return this.card.getManaCost(p, this.manacost);
            }
            public bool canplayCard(Playfield p)
            {
                return this.card.canplayCard(p, this.manacost);
            }
        }

        private BattleField bf = BattleField.Instance;

        public List<Cardsposi> cardsdata = new List<Cardsposi>();

        public List<Handcard> handCards = new List<Handcard>();

        public int anzcards = 0;

        public int enemyAnzCards = 0;

        private int ownPlayerController = 0;

        Helpfunctions help;
        Cardsposi currentCarddata = new Cardsposi();
        CardDB cdb = CardDB.Instance;

        private static Handmanager instance;

        public static Handmanager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Handmanager();
                }
                return instance;
            }
        }


        private Handmanager()
        {
            this.help = Helpfunctions.Instance;

            int i = 0;
            Cardsposi c = new Cardsposi();
            c.Amount = 1;
            c.DetectPosix = 450;
            c.DetectPosiy = 675;
            c.cardsHooverPosx = 490;
            c.cardsHooverdiff = 1;
            c.cardsBigPosx = 366;
            c.cardsBigdiff = 1;
            this.cardsdata.Add(c);

            i = i + 1;
            c = new Cardsposi();
            c.Amount = 2;
            c.DetectPosix = 403;
            c.DetectPosiy = 674;
            c.cardsHooverPosx = 438;
            c.cardsHooverdiff = 100;
            c.cardsBigPosx = 317;
            c.cardsBigdiff = 99;
            this.cardsdata.Add(c);

            i = i + 1;
            c = new Cardsposi();
            c.Amount = 3;
            c.DetectPosix = 356;
            c.DetectPosiy = 675;
            c.cardsHooverPosx = 390;
            c.cardsHooverdiff = 100;
            c.cardsBigPosx = 267;
            c.cardsBigdiff = 99;
            this.cardsdata.Add(c);

            i = i + 1;
            c = new Cardsposi();
            c.Amount = 4;
            c.DetectPosix = 291;
            c.DetectPosiy = 715;
            c.cardsHooverPosx = 350;
            c.cardsHooverdiff = 90;
            c.cardsBigPosx = 220;
            c.cardsBigdiff = 97;
            this.cardsdata.Add(c);

            i = i + 1;
            c = new Cardsposi();
            c.Amount = 5;
            c.DetectPosix = 280;
            c.DetectPosiy = 712;
            c.cardsHooverPosx = 340;
            c.cardsHooverdiff = 75;
            c.cardsBigPosx = 210;
            c.cardsBigdiff = 78;
            this.cardsdata.Add(c);

            i = i + 1;
            c = new Cardsposi();
            c.Amount = 6;
            c.DetectPosix = 273;
            c.DetectPosiy = 726;
            c.cardsHooverPosx = 323;
            c.cardsHooverdiff = 65;
            c.cardsBigPosx = 204;
            c.cardsBigdiff = 65;
            this.cardsdata.Add(c);

            i = i + 1;
            c = new Cardsposi();
            c.Amount = 7;
            c.DetectPosix = 267;
            c.DetectPosiy = 724;
            c.cardsHooverPosx = 314;
            c.cardsHooverdiff = 56;
            c.cardsBigPosx = 199;
            c.cardsBigdiff = 56;
            this.cardsdata.Add(c);

            i = i + 1;
            c = new Cardsposi();
            c.Amount = 8;
            c.DetectPosix = 262;
            c.DetectPosiy = 740;
            c.cardsHooverPosx = 300;
            c.cardsHooverdiff = 47;
            c.cardsBigPosx = 196;
            c.cardsBigdiff = 49;
            this.cardsdata.Add(c);

            i = i + 1;
            c = new Cardsposi();
            c.Amount = 9;
            c.DetectPosix = 260;
            c.DetectPosiy = 738;
            c.cardsHooverPosx = 295;
            c.cardsHooverdiff = 42;
            c.cardsBigPosx = 193;
            c.cardsBigdiff = 43;
            this.cardsdata.Add(c);

            i = i + 1;
            c = new Cardsposi();
            c.Amount = 10;
            c.DetectPosix = 257;
            c.DetectPosiy = 752;
            c.cardsHooverPosx = 286;
            c.cardsHooverdiff = 38;
            c.cardsBigPosx = 191;
            c.cardsBigdiff = 39;
            this.cardsdata.Add(c);

        }

        public void clearAll()
        {
            this.handCards.Clear();
            this.anzcards = 0;
            this.enemyAnzCards = 0;
            this.ownPlayerController = 0;
        }

        public void setOwnPlayer(int player)
        {
            this.ownPlayerController = player;
        }



        public Cardsposi getCardposi(int anzcard)
        {
            if (anzcard == 0) return new Cardsposi();
            int k = anzcard - 1;
            Cardsposi returnval = new Cardsposi();
            returnval.Amount = this.cardsdata[k].Amount;
            returnval.cardsBigdiff = this.cardsdata[k].cardsBigdiff;
            returnval.cardsBigPosx = this.cardsdata[k].cardsBigPosx;
            returnval.cardsHooverdiff = this.cardsdata[k].cardsHooverdiff;
            returnval.cardsHooverPosx = this.cardsdata[k].cardsHooverPosx;
            returnval.DetectPosix = this.cardsdata[k].DetectPosix;
            returnval.DetectPosiy = this.cardsdata[k].DetectPosiy;
            return returnval;
        }

        public void setHandcards(List<Handcard> hc, int anzown, int anzenemy)
        {
            this.handCards.Clear();
            foreach (Handcard h in hc)
            {
                this.handCards.Add(new Handcard(h));
            }
            //this.handCards.AddRange(hc);
            this.handCards.Sort((a, b) => a.position.CompareTo(b.position));
            this.anzcards = anzown;
            this.enemyAnzCards = anzenemy;
            this.currentCarddata = this.getCardposi(this.anzcards);
        }


        public void printcards()
        {
            help.logg("Own Handcards: ");
            foreach (Handmanager.Handcard c in this.handCards)
            {
                help.logg("pos " + c.position + " " + c.card.name + " " + c.manacost + " entity " + c.entity + " " + c.card.CardID);
            }
        }


    }

    public enum HeroEnum
    {
        druid,
        hogger,
        hunter,
        priest,
        warlock,
        thief,
        pala,
        warrior,
        shaman,
        mage,
        lordjaraxxus
    }

    public class Hrtprozis
    {


        public int attackFaceHp = 15;
        public int ownHeroFatigue = 0;
        public int ownDeckSize = 30;
        public int enemyDeckSize = 30;
        public int enemyHeroFatigue = 0;

        public int ownHeroEntity = -1;
        public int enemyHeroEntitiy = -1;
        public DateTime roundstart = DateTime.Now;
        BattleField bf = BattleField.Instance;
        bool tempwounded = false;
        public int currentMana = 0;
        public int heroHp = 30, enemyHp = 30;
        public int heroAtk = 0, enemyAtk = 0;
        public int heroDefence = 0, enemyDefence = 0;
        public bool ownheroisread = false;
        public bool ownAbilityisReady = false;
        public int ownHeroNumAttacksThisTurn = 0;
        public bool ownHeroWindfury = false;

        public List<string> ownSecretList = new List<string>();
        public int enemySecretCount = 0;



        public HeroEnum heroname = HeroEnum.druid, enemyHeroname = HeroEnum.druid;
        public CardDB.Card heroAbility;
        public CardDB.Card enemyAbility;
        public int anzEnemys = 0;
        public int anzOwn = 0;
        public bool herofrozen = false;
        public bool enemyfrozen = false;
        public int numMinionsPlayedThisTurn = 0;
        public int cardsPlayedThisTurn = 0;
        public int ueberladung = 0;

        public int ownMaxMana = 0;
        public int enemyMaxMana = 0;

        public int enemyWeaponDurability = 0;
        public int enemyWeaponAttack = 0;
        public string enemyHeroWeapon = "";

        public int heroWeaponDurability = 0;
        public int heroWeaponAttack = 0;
        public string ownHeroWeapon = "";
        public bool heroImmuneToDamageWhileAttacking = false;
        public bool heroImmune = false;
        public bool enemyHeroImmune = false;

        public bool minionsFailure = false;

        public List<Minion> ownMinions = new List<Minion>();
        public List<Minion> enemyMinions = new List<Minion>();

        int manadiff = 23;

        int detectmin = 30, detectmax = 130;

        Helpfunctions help = Helpfunctions.Instance;
        //Imagecomparer icom = Imagecomparer.Instance;
        //HrtNumbers hrtnumbers = HrtNumbers.Instance;
        CardDB cdb = CardDB.Instance;

        private int ownPlayerController = 0;

        private static Hrtprozis instance;

        public static Hrtprozis Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Hrtprozis();
                }
                return instance;
            }
        }



        private Hrtprozis()
        {
        }

        public void setAttackFaceHP(int hp)
        {
            this.attackFaceHp = hp;
        }

        public void clearAll()
        {
            ownHeroEntity = -1;
            enemyHeroEntitiy = -1;
            tempwounded = false;
            currentMana = 0;
            heroHp = 30;
            enemyHp = 30;
            heroAtk = 0;
            enemyAtk = 0;
            heroDefence = 0; enemyDefence = 0;
            ownheroisread = false;
            ownAbilityisReady = false;
            ownHeroNumAttacksThisTurn = 0;
            ownHeroWindfury = false;
            ownSecretList.Clear();
            enemySecretCount = 0;
            heroname = HeroEnum.druid;
            enemyHeroname = HeroEnum.druid;
            heroAbility = new CardDB.Card();
            enemyAbility = new CardDB.Card();
            anzEnemys = 0;
            anzOwn = 0;
            herofrozen = false;
            enemyfrozen = false;
            numMinionsPlayedThisTurn = 0;
            cardsPlayedThisTurn = 0;
            ueberladung = 0;
            ownMaxMana = 0;
            enemyMaxMana = 0;
            enemyWeaponDurability = 0;
            enemyWeaponAttack = 0;
            heroWeaponDurability = 0;
            heroWeaponAttack = 0;
            heroImmuneToDamageWhileAttacking = false;
            ownMinions.Clear();
            enemyMinions.Clear();
            heroImmune = false;
            enemyHeroImmune = false;
            this.ownHeroWeapon = "";
            this.enemyHeroWeapon = "";
        }


        public void setOwnPlayer(int player)
        {
            this.ownPlayerController = player;
        }

        public int getOwnController()
        {
            return this.ownPlayerController;
        }

        public string heroIDtoName(string s)
        {
            string retval = "druid";

            if (s == "XXX_040")
            {
                retval = "hogger";
            }
            if (s == "HERO_05")
            {
                retval = "hunter";
            }
            if (s == "HERO_09")
            {
                retval = "priest";
            }
            if (s == "HERO_06")
            {
                retval = "druid";
            }
            if (s == "HERO_07")
            {
                retval = "warlock";
            }
            if (s == "HERO_03")
            {
                retval = "thief";
            }
            if (s == "HERO_04")
            {
                retval = "pala";
            }
            if (s == "HERO_01")
            {
                retval = "warrior";
            }
            if (s == "HERO_02")
            {
                retval = "shaman";
            }
            if (s == "HERO_08")
            {
                retval = "mage";
            }
            if (s == "EX1_323h")
            {
                retval = "lordjaraxxus";
            }

            return retval;
        }

        public HeroEnum heroNametoEnum(string s)
        {
            HeroEnum retval = HeroEnum.druid;

            if (s == "hogger")
            {
                retval = HeroEnum.hogger;
            }
            if (s == "hunter")
            {
                retval = HeroEnum.hunter;
            }
            if (s == "priest")
            {
                retval = HeroEnum.priest;
            }
            if (s == "druid")
            {
                retval = HeroEnum.druid;
            }
            if (s == "warlock")
            {
                retval = HeroEnum.warlock;
            }
            if (s == "thief")
            {
                retval = HeroEnum.thief;
            }
            if (s == "pala")
            {
                retval = HeroEnum.pala;
            }
            if (s == "warrior")
            {
                retval = HeroEnum.warrior;
            }
            if (s == "shaman")
            {
                retval = HeroEnum.shaman;
            }
            if (s == "mage")
            {
                retval = HeroEnum.mage;
            }
            if (s == "lordjaraxxus")
            {
                retval = HeroEnum.lordjaraxxus;
            }

            return retval;
        }


        public void updateMinions(List<Minion> om, List<Minion> em)
        {
            this.ownMinions.Clear();
            this.enemyMinions.Clear();
            foreach (var item in om)
            {
                this.ownMinions.Add(new Minion(item));
            }
            //this.ownMinions.AddRange(om);
            foreach (var item in em)
            {
                this.enemyMinions.Add(new Minion(item));
            }
            //this.enemyMinions.AddRange(em);

            //sort them 
            updatePositions();
        }

        public void updateSecretStuff(List<string> ownsecs, int numEnemSec)
        {
            this.ownSecretList.Clear();
            foreach (string s in ownsecs)
            {
                this.ownSecretList.Add(s);
            }
            this.enemySecretCount = numEnemSec;
        }

        public void updatePlayer(int maxmana, int currentmana, int cardsplayedthisturn, int numMinionsplayed, int recall, int heroentity, int enemyentity)
        {
            this.currentMana = currentmana;
            this.ownMaxMana = maxmana;
            this.cardsPlayedThisTurn = cardsplayedthisturn;
            this.numMinionsPlayedThisTurn = numMinionsplayed;
            this.ueberladung = recall;
            this.ownHeroEntity = heroentity;
            this.enemyHeroEntitiy = enemyentity;


        }

        public void updateOwnHero(string weapon, int watt, int wdur, bool heroimunewhileattack, int heroatt, int herohp, int herodef, string heron, bool heroready, bool frozen, CardDB.Card hab, bool habrdy, int numAttacksTTurn, bool windfury, bool hisim)
        {
            this.ownHeroWeapon = weapon;
            this.heroWeaponAttack = watt;
            this.heroWeaponDurability = wdur;
            this.heroImmuneToDamageWhileAttacking = heroimunewhileattack;
            this.heroAtk = heroatt;
            this.heroHp = herohp;
            this.heroDefence = herodef;
            this.heroname = this.heroNametoEnum(heron);
            this.ownheroisread = heroready;
            this.herofrozen = frozen;
            this.heroAbility = hab;
            this.ownAbilityisReady = habrdy;
            this.ownHeroWindfury = windfury;
            this.ownHeroNumAttacksThisTurn = numAttacksTTurn;
            this.heroImmune = hisim;
        }

        public void updateEnemyHero(string weapon, int watt, int wdur, int heroatt, int herohp, int herodef, string heron, bool frozen, CardDB.Card eab, bool ehisim, int enemMM)
        {
            this.enemyHeroWeapon = weapon;
            this.enemyWeaponAttack = watt;
            this.enemyWeaponDurability = wdur;
            this.enemyAtk = heroatt;
            this.enemyHp = herohp;
            this.enemyHeroname = this.heroNametoEnum(heron);
            this.enemyDefence = herodef;
            this.enemyfrozen = frozen;
            this.enemyAbility = eab;
            this.enemyHeroImmune = ehisim;
            this.enemyMaxMana = enemMM;
        }

        public void updateFatigueStats(int ods, int ohf, int eds, int ehf)
        {
            this.ownDeckSize = 30;// ods;
            this.ownHeroFatigue = ohf;
            this.enemyDeckSize = 30;// eds;
            this.enemyHeroFatigue = ehf;
        }

        public void setEnchantments(List<BattleField.HrtUnit> enchantments)
        {
            foreach (BattleField.HrtUnit bhu in enchantments)
            {
                //create enchantment
                Enchantment ench = CardDB.getEnchantmentFromCardID(bhu.CardID);
                ench.creator = bhu.getTag(GAME_TAG.CREATOR);
                ench.cantBeDispelled = false;
                if (bhu.getTag(GAME_TAG.CANT_BE_DISPELLED) == 1) ench.cantBeDispelled = true;

                foreach (Minion m in this.ownMinions)
                {
                    if (m.entitiyID == bhu.getTag(GAME_TAG.ATTACHED))
                    {
                        m.enchantments.Add(ench);
                    }

                }

                foreach (Minion m in this.enemyMinions)
                {
                    if (m.entitiyID == bhu.getTag(GAME_TAG.ATTACHED))
                    {
                        m.enchantments.Add(ench);
                    }

                }

            }

        }

        public void updatePositions()
        {
            this.ownMinions.Sort((a, b) => a.zonepos.CompareTo(b.zonepos));
            this.enemyMinions.Sort((a, b) => a.zonepos.CompareTo(b.zonepos));
            int i = 0;
            foreach (Minion m in this.ownMinions)
            {
                m.id = i;
                i++;
                m.zonepos = i;

            }
            i = 0;
            foreach (Minion m in this.enemyMinions)
            {
                m.id = i;
                i++;
                m.zonepos = i;
            }

            /*List<Minion> temp = new List<Minion>();
            temp.AddRange(ownMinions);
            this.ownMinions.Clear();
            this.ownMinions.AddRange(temp.OrderBy(x => x.zonepos).ToList());
            temp.Clear();
            temp.AddRange(enemyMinions);
            this.enemyMinions.Clear();
            this.enemyMinions.AddRange(temp.OrderBy(x => x.zonepos).ToList());*/

        }

        private Minion createNewMinion(Handmanager.Handcard hc, int id)
        {
            Minion m = new Minion();
            m.handcard = new Handmanager.Handcard(hc);
            m.id = id;
            m.zonepos = id + 1;
            m.entitiyID = hc.entity;
            m.Posix = 0;
            m.Posiy = 0;
            m.Angr = hc.card.Attack;
            m.Hp = hc.card.Health;
            m.maxHp = hc.card.Health;
            m.name = hc.card.name;
            m.playedThisTurn = true;
            m.numAttacksThisTurn = 0;


            if (hc.card.windfury) m.windfury = true;
            if (hc.card.tank) m.taunt = true;
            if (hc.card.Charge)
            {
                m.Ready = true;
                m.charge = true;
            }
            if (hc.card.Shield) m.divineshild = true;
            if (hc.card.poisionous) m.poisonous = true;

            if (hc.card.Stealth) m.stealth = true;

            if (m.name == "lightspawn" && !m.silenced)
            {
                m.Angr = m.Hp;
            }


            return m;
        }


        public void printHero()
        {
            help.logg("player:");
            help.logg(this.numMinionsPlayedThisTurn + " " + this.cardsPlayedThisTurn + " " + this.ueberladung + " " + this.ownPlayerController);

            help.logg("ownhero:");
            help.logg(this.heroname + " " + heroHp + " " + heroDefence + " " + this.heroImmuneToDamageWhileAttacking + " " + this.heroImmune);
            help.logg("ready: " + this.ownheroisread + " alreadyattacked: " + this.ownHeroNumAttacksThisTurn + " frzn: " + this.herofrozen + " attack: " + heroAtk + " " + heroWeaponAttack + " " + heroWeaponDurability + " " + ownHeroWeapon);
            help.logg("ability: " + this.ownAbilityisReady + " " + this.heroAbility.CardID);
            string secs = "";
            foreach (string sec in this.ownSecretList)
            {
                secs += sec + " ";
            }
            help.logg("osecrets: " + secs);
            help.logg("enemyhero:");
            help.logg(this.enemyHeroname + " " + enemyHp + " " + enemyDefence + " " + this.enemyfrozen + " " + this.enemyHeroImmune);
            help.logg(this.enemyWeaponAttack + " " + this.enemyWeaponDurability + " " + this.enemyHeroWeapon);
            help.logg("ability: " + "true" + " " + this.enemyAbility.CardID);
            help.logg("fatigue: " + this.ownDeckSize + " " + this.ownHeroFatigue + " " + this.enemyDeckSize + " " + this.enemyHeroFatigue);

        }


        public void printOwnMinions()
        {
            help.logg("OwnMinions:");
            foreach (Minion m in this.ownMinions)
            {
                help.logg(m.name + " " + m.handcard.card.CardID + " id:" + m.id + " zp:" + m.zonepos + " e:" + m.entitiyID + " A:" + m.Angr + " H:" + m.Hp + " mH:" + m.maxHp + " rdy:" + m.Ready + " tnt:" + m.taunt + " frz:" + m.frozen + " silenced:" + m.silenced + " divshield:" + m.divineshild + " ptt:" + m.playedThisTurn + " wndfr:" + m.windfury + " natt:" + m.numAttacksThisTurn + " stl:" + m.stealth + " poi:" + m.poisonous + " imm:" + m.immune + " ex:" + m.exhausted + " chrg:" + m.charge);
                foreach (Enchantment e in m.enchantments)
                {
                    help.logg(e.CARDID + " " + e.creator + " " + e.controllerOfCreator);
                }
            }

        }

        public void printEnemyMinions()
        {
            help.logg("EnemyMinions:");
            foreach (Minion m in this.enemyMinions)
            {
                help.logg(m.name + " " + m.handcard.card.CardID + " id:" + m.id + " zp:" + m.zonepos + " e:" + m.entitiyID + " A:" + m.Angr + " H:" + m.Hp + " mH:" + m.maxHp + " rdy:" + m.Ready + " tnt:" + m.taunt + " frz:" + m.frozen + " silenced:" + m.silenced + " divshield:" + m.divineshild + " wndfr:" + m.windfury + " stl:" + m.stealth + " poi:" + m.poisonous + " imm:" + m.immune + " ex:" + m.exhausted + " chrg:" + m.charge);
                foreach (Enchantment e in m.enchantments)
                {
                    help.logg(e.CARDID + " " + e.creator + " " + e.controllerOfCreator);
                }
            }

        }


    }

    public class PenalityManager
    {
        //todo acolyteofpain
        //todo better aoe-penality

        ComboBreaker cb;

        public Dictionary<string, int> priorityDatabase = new Dictionary<string, int>();
        Dictionary<string, int> HealTargetDatabase = new Dictionary<string, int>();
        Dictionary<string, int> HealHeroDatabase = new Dictionary<string, int>();
        Dictionary<string, int> HealAllDatabase = new Dictionary<string, int>();

        public Dictionary<string, int> DamageTargetDatabase = new Dictionary<string, int>();
        public Dictionary<string, int> DamageTargetSpecialDatabase = new Dictionary<string, int>();
        Dictionary<string, int> DamageAllDatabase = new Dictionary<string, int>();
        Dictionary<string, int> DamageHeroDatabase = new Dictionary<string, int>();
        Dictionary<string, int> DamageRandomDatabase = new Dictionary<string, int>();
        Dictionary<string, int> DamageAllEnemysDatabase = new Dictionary<string, int>();

        Dictionary<string, int> enrageDatabase = new Dictionary<string, int>();
        Dictionary<string, int> silenceDatabase = new Dictionary<string, int>();

        Dictionary<string, int> heroAttackBuffDatabase = new Dictionary<string, int>();
        Dictionary<string, int> attackBuffDatabase = new Dictionary<string, int>();
        Dictionary<string, int> healthBuffDatabase = new Dictionary<string, int>();
        Dictionary<string, int> tauntBuffDatabase = new Dictionary<string, int>();

        Dictionary<string, int> cardDrawBattleCryDatabase = new Dictionary<string, int>();
        Dictionary<string, int> cardDiscardDatabase = new Dictionary<string, int>();
        Dictionary<string, int> destroyOwnDatabase = new Dictionary<string, int>();
        Dictionary<string, int> destroyDatabase = new Dictionary<string, int>();
        Dictionary<string, int> buffingMinionsDatabase = new Dictionary<string, int>();
        Dictionary<string, int> buffing1TurnDatabase = new Dictionary<string, int>();
        Dictionary<string, int> heroDamagingAoeDatabase = new Dictionary<string, int>();

        Dictionary<string, int> returnHandDatabase = new Dictionary<string, int>();
        public Dictionary<string, int> priorityTargets = new Dictionary<string, int>();

        public Dictionary<string, int> specialMinions = new Dictionary<string, int>(); //minions with cardtext, but no battlecry


        private static PenalityManager instance;

        public static PenalityManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new PenalityManager();
                }
                return instance;
            }
        }

        private PenalityManager()
        {
            setupHealDatabase();
            setupEnrageDatabase();
            setupDamageDatabase();
            setupPriorityList();
            setupsilenceDatabase();
            setupAttackBuff();
            setupHealthBuff();
            setupCardDrawBattlecry();
            setupDiscardCards();
            setupDestroyOwnCards();
            setupSpecialMins();
            setupEnemyTargetPriority();
            setupHeroDamagingAOE();
            setupBuffingMinions();
        }

        public void setCombos()
        {
            this.cb = ComboBreaker.Instance;
        }

        public int getAttackWithMininonPenality(Minion m, Playfield p, int target, bool lethal)
        {
            int pen = 0;
            pen = getAttackSecretPenality(m, p, target);
            if (!lethal && m.name == "bloodimp") pen = 50;
            if (m.name == "leeroyjenkins")
            {
                if (target >= 10 && target <= 19)
                {
                    Minion t = p.enemyMinions[target - 10];
                    if (t.name == "whelp") return 500;
                }

            }
            return pen;
        }

        public int getAttackWithHeroPenality(int target, Playfield p)
        {
            int retval = 0;

            //no penality, but a bonus, if he has weapon on hand!
            if (p.ownWeaponDurability >= 1)
            {
                bool hasweapon = false;
                foreach (Handmanager.Handcard c in p.owncards)
                {
                    if (c.card.type == CardDB.cardtype.WEAPON) hasweapon = true;
                }
                if (p.ownWeaponAttack == 1 && p.ownHeroName == HeroEnum.thief) hasweapon = true;
                if (hasweapon) retval = -p.ownWeaponAttack - 1; // so he doesnt "lose" the weapon in evaluation :D
            }
            if (p.ownWeaponAttack == 1 && p.ownHeroName == HeroEnum.thief) retval += -1;
            return retval;
        }

        public int getPlayCardPenality(CardDB.Card card, int target, Playfield p, int choice, bool lethal)
        {
            int retval = 0;
            string name = card.name;
            //there is no reason to buff HP of minon (because it is not healed)

            int abuff = getAttackBuffPenality(card, target, p, choice, lethal);
            int tbuff = getTauntBuffPenality(name, target, p, choice);
            if (name == "markofthewild" && ((abuff >= 500 && tbuff == 0) || (abuff == 0 && tbuff >= 500)))
            {
                retval = 0;
            }
            else
            {
                retval += abuff + tbuff;
            }
            retval += getHPBuffPenality(card, target, p, choice);
            retval += getSilencePenality(name, target, p, choice, lethal);
            retval += getDamagePenality(name, target, p, choice, lethal);
            retval += getHealPenality(name, target, p, choice, lethal);
            retval += getCardDrawPenality(name, target, p, choice);
            retval += getCardDrawofEffectMinions(card, p);
            retval += getCardDiscardPenality(name, p);
            retval += getDestroyOwnPenality(name, target, p, lethal);

            retval += getDestroyPenality(name, target, p);
            retval += getSpecialCardComboPenalitys(card, target, p, lethal, choice);
            retval += playSecretPenality(card, p);
            retval += getPlayCardSecretPenality(card, p);
            if (!lethal) retval += cb.getPenalityForDestroyingCombo(card, p);

            return retval;
        }

        private int getAttackBuffPenality(CardDB.Card card, int target, Playfield p, int choice, bool lethal)
        {
            string name = card.name;
            int pen = 0;
            //buff enemy?

            if (!lethal && (card.name == "savageroar" || card.name == "bloodlust"))
            {
                int targets = 0;
                foreach (Minion m in p.ownMinions)
                {
                    if (m.Ready) targets++;
                }
                if ((p.ownHeroReady || p.ownHeroNumAttackThisTurn == 0) && card.name == "savageroar") targets++;

                if (targets <= 2)
                {
                    return 20;
                }
            }

            if (!this.attackBuffDatabase.ContainsKey(name)) return 0;
            if (target >= 10 && target <= 19)
            {
                if (card.type == CardDB.cardtype.MOB && p.ownMinions.Count == 0) return 0;
                //allow it if you have biggamehunter
                foreach (Handmanager.Handcard hc in p.owncards)
                {
                    if (hc.card.specialMin == CardDB.specialMinions.biggamehunter) return pen;
                    if (hc.card.specialMin == CardDB.specialMinions.shadowworddeath) return pen;
                }
                if (card.name == "crueltaskmaster")
                {
                    int maxhp = 0;
                    Minion m = p.enemyMinions[target - 10];

                    if (m.Hp == 1)
                    {
                        return 0;
                    }

                    if (m.Angr >= 4 || m.Hp >= 5)
                    {
                        maxhp++;
                    }
                    if (maxhp >= 1)
                    {
                        foreach (Handmanager.Handcard hc in p.owncards)
                        {
                            if (hc.card.name == "execute") return 0;
                        }
                    }
                    pen = 30;
                }
                else
                {
                    pen = 500;
                }
            }
            if (target >= 0 && target <= 9)
            {
                Minion m = p.ownMinions[target];
                if (!m.Ready)
                {
                    return 20;
                }
                if (m.Hp == 1 && !m.divineshild && !this.buffing1TurnDatabase.ContainsKey(name))
                {
                    return 10;
                }
            }
            return pen;
        }

        private int getHPBuffPenality(CardDB.Card card, int target, Playfield p, int choice)
        {
            string name = card.name;
            int pen = 0;
            //buff enemy?
            if (!this.healthBuffDatabase.ContainsKey(name)) return 0;
            if (target >= 10 && target <= 19 && !this.tauntBuffDatabase.ContainsKey(name))
            {
                pen = 500;
            }

            return pen;
        }


        private int getTauntBuffPenality(string name, int target, Playfield p, int choice)
        {
            int pen = 0;
            //buff enemy?
            if (!this.tauntBuffDatabase.ContainsKey(name)) return 0;
            if (name == "markofnature" && choice != 2) return 0;

            if (target >= 10 && target <= 19)
            {
                //allow it if you have black knight
                foreach (Handmanager.Handcard hc in p.owncards)
                {
                    if (hc.card.name == "theblackknight") return 0;
                }

                // allow taunting if target is priority and others have taunt
                bool enemyhasTaunts = false;
                foreach (Minion mnn in p.enemyMinions)
                {
                    if (mnn.taunt)
                    {
                        enemyhasTaunts = true;
                        break;
                    }
                }
                Minion m = p.enemyMinions[target - 10];
                if (enemyhasTaunts && this.priorityDatabase.ContainsKey(m.name))
                {
                    return 0;
                }

                pen = 500;
            }

            return pen;
        }

        private int getSilencePenality(string name, int target, Playfield p, int choice, bool lethal)
        {
            int pen = 0;
            if (name == "keeperofthegrove" && choice != 2) return 0; // look at damage penality in this case

            if (target >= 0 && target <= 9)
            {
                if (this.silenceDatabase.ContainsKey(name))
                {
                    // no pen if own is enrage
                    Minion m = p.ownMinions[target];

                    if ((!m.silenced && (m.name == "ancientwatcher" || m.name == "ragnarosthefirelord")) || m.Angr < m.handcard.card.Attack || m.maxHp < m.handcard.card.Health || (m.frozen && !m.playedThisTurn && m.numAttacksThisTurn == 0))
                    {
                        return 0;
                    }


                    pen += 500;
                }
            }

            if (target == -1)
            {
                if (name == "ironbeakowl" || name == "spellbreaker")
                {

                    return 20;
                }
            }

            if (target >= 10 && target <= 19)
            {
                if (this.silenceDatabase.ContainsKey(name))
                {
                    // no pen if own is enrage
                    Minion m = p.enemyMinions[target - 10];//

                    if (!m.silenced && (m.name == "ancientwatcher" || m.name == "ragnarosthefirelord"))
                    {
                        return 500;
                    }

                    if (lethal)
                    {
                        //during lethal we only silence taunt, or if its a mob (owl/spellbreaker) + we can give him charge
                        if (m.taunt || (name == "ironbeakowl" && (p.ownMinions.Find(x => x.name == "tundrarhino") != null || p.ownMinions.Find(x => x.name == "warsongcommander") != null || p.owncards.Find(x => x.card.name == "charge") != null)) || (name == "spellbreaker" && p.owncards.Find(x => x.card.name == "charge") != null)) return 0;

                        return 500;
                    }
                    if (m.handcard.card.specialMin == CardDB.specialMinions.venturecomercenary && !m.silenced && (m.Angr <= m.handcard.card.Attack && m.maxHp <= m.handcard.card.Health))
                    {
                        return 30;
                    }
                    if (priorityDatabase.ContainsKey(m.name) && !m.silenced)
                    {
                        return -10;
                    }

                    //silence nothing
                    if (m.Angr <= m.handcard.card.Attack && m.maxHp <= m.handcard.card.Health && !m.taunt && !m.windfury && !m.divineshild && !m.poisonous && m.enchantments.Count == 0 && !this.specialMinions.ContainsKey(name))
                    {
                        return 30;
                    }



                    pen = 0;
                }
            }

            return pen;

        }

        private int getDamagePenality(string name, int target, Playfield p, int choice, bool lethal)
        {
            int pen = 0;

            if (name == "shieldslam" && p.ownHeroDefence == 0) return 500;
            if (name == "savagery" && p.ownheroAngr == 0) return 500;
            if (name == "keeperofthegrove" && choice != 1) return 0; // look at silence penality

            if (this.DamageAllDatabase.ContainsKey(name) || (p.auchenaiseelenpriesterin && HealAllDatabase.ContainsKey(name))) // aoe penality
            {
                int maxhp = 0;
                foreach (Minion m in p.enemyMinions)
                {
                    if (m.Angr >= 4 || m.Hp >= 5)
                    {
                        maxhp++;
                    }
                }
                if (maxhp >= 1)
                {
                    foreach (Handmanager.Handcard hc in p.owncards)
                    {
                        if (hc.card.name == "execute") return 0;
                    }
                }

                if (p.enemyMinions.Count <= 1 || p.enemyMinions.Count + 1 <= p.ownMinions.Count || p.ownMinions.Count >= 3)
                {
                    return 30;
                }
            }

            if (this.DamageAllEnemysDatabase.ContainsKey(name)) // aoe penality
            {
                int maxhp = 0;
                foreach (Minion m in p.enemyMinions)
                {
                    if (m.Angr >= 4 || m.Hp >= 5)
                    {
                        maxhp++;
                    }
                }
                if (maxhp >= 4)
                {
                    foreach (Handmanager.Handcard hc in p.owncards)
                    {
                        if (hc.card.name == "execute") return 0;
                    }
                }

                if (name == "holynova")
                {
                    int targets = p.enemyMinions.Count;
                    foreach (Minion m in p.ownMinions)
                    {
                        if (m.wounded) targets++;
                    }
                    if (targets <= 2)
                    {
                        return 20;
                    }

                }
                if (p.enemyMinions.Count <= 2)
                {
                    return 20;
                }
            }

            if (target == 100)
            {
                if (DamageTargetDatabase.ContainsKey(name) || DamageTargetSpecialDatabase.ContainsKey(name) || (p.auchenaiseelenpriesterin && HealTargetDatabase.ContainsKey(name)))
                {
                    pen = 500;
                }
            }

            if (!lethal && target == 200)
            {
                if (name == "baneofdoom")
                {
                    pen = 500;
                }
            }

            if (target >= 0 && target <= 9)
            {
                if (DamageTargetDatabase.ContainsKey(name) || (p.auchenaiseelenpriesterin && HealTargetDatabase.ContainsKey(name)))
                {
                    // no pen if own is enrage
                    Minion m = p.ownMinions[target];

                    //standard ones :D (mostly carddraw
                    if (enrageDatabase.ContainsKey(m.name) && !m.wounded)
                    {
                        return pen;
                    }

                    // no pen if we have battlerage for example
                    int dmg = 0;
                    if (DamageTargetDatabase.ContainsKey(name))
                    {
                        dmg = DamageTargetDatabase[name];
                    }
                    else
                    {
                        dmg = HealTargetDatabase[name];
                    }
                    if (m.handcard.card.deathrattle) return 10;
                    if (m.Hp > dmg)
                    {
                        if (m.name == "acolyteofpain" && p.owncards.Count <= 3) return 0;
                        foreach (Handmanager.Handcard hc in p.owncards)
                        {
                            if (hc.card.name == "battlerage") return pen;
                            if (hc.card.name == "rampage") return pen;
                        }
                    }


                    pen = 500;
                }

                //special cards
                if (DamageTargetSpecialDatabase.ContainsKey(name))
                {
                    int dmg = DamageTargetSpecialDatabase[name];
                    Minion m = p.ownMinions[target];
                    if (name == "crueltaskmaster" && m.Hp >= 2) return 0;
                    if (name == "demonfire" && (TAG_RACE)m.handcard.card.race == TAG_RACE.DEMON) return 0;
                    if (name == "earthshock" && m.Hp >= 2)
                    {
                        if (priorityDatabase.ContainsKey(m.name) && !m.silenced)
                        {
                            return 500;
                        }

                        if ((!m.silenced && (m.name == "ancientwatcher" || m.name == "ragnarosthefirelord")) || m.Angr < m.handcard.card.Attack || m.maxHp < m.handcard.card.Health || (m.frozen && !m.playedThisTurn && m.numAttacksThisTurn == 0))
                            return 0;
                    }
                    if (name == "earthshock")//dont silence other own minions
                    {
                        return 500;
                    }

                    // no pen if own is enrage
                    if (enrageDatabase.ContainsKey(m.name) && !m.wounded)
                    {
                        return pen;
                    }

                    // no pen if we have battlerage for example

                    if (m.Hp > dmg)
                    {
                        foreach (Handmanager.Handcard hc in p.owncards)
                        {
                            if (hc.card.name == "battlerage") return pen;
                            if (hc.card.name == "rampage") return pen;
                        }
                    }

                    pen = 500;
                }
            }
            if (target >= 10 && target <= 19)
            {
                if (DamageTargetSpecialDatabase.ContainsKey(name) || DamageTargetDatabase.ContainsKey(name))
                {
                    Minion m = p.enemyMinions[target - 10];
                    if (name == "soulfire" && m.maxHp <= 2) pen = 10;

                    if (name == "baneofdoom" && m.Hp >= 3) pen = 10;

                    if (name == "shieldslam" && (m.Hp <= 4 || m.Angr <= 4)) pen = 20;
                }
            }

            return pen;
        }

        private int getHealPenality(string name, int target, Playfield p, int choice, bool lethal)
        {
            ///Todo healpenality for aoe heal
            ///todo auchenai soulpriest
            if (p.auchenaiseelenpriesterin) return 0;
            if (name == "ancientoflore" && choice != 2) return 0;
            int pen = 0;
            int heal = 0;
            /*if (HealHeroDatabase.ContainsKey(name))
            {
                heal = HealHeroDatabase[name];
                if (target == 200) pen = 500; // dont heal enemy
                if ((target == 100 || target == -1) && p.ownHeroHp + heal > 30) pen = p.ownHeroHp + heal - 30;
            }*/

            if (HealTargetDatabase.ContainsKey(name))
            {
                heal = HealTargetDatabase[name];
                if (target == 200) return 500; // dont heal enemy
                if ((target == 100) && p.ownHeroHp == 30) return 150;
                if ((target == 100) && p.ownHeroHp + heal > 30) pen = p.ownHeroHp + heal - 30;
                Minion m = new Minion();

                if (target >= 0 && target < 10)
                {
                    m = p.ownMinions[target];
                    int wasted = 0;
                    if (m.Hp == m.maxHp) return 500;
                    if (m.Hp + heal - 1 > m.maxHp) wasted = m.Hp + heal - m.maxHp;
                    pen = wasted;

                    if (m.taunt && wasted <= 2 && m.Hp < m.maxHp) pen -= 5; // if we heal a taunt, its good :D

                    if (m.Hp + heal <= m.maxHp) pen = -1;
                }

                if (target >= 10 && target < 20)
                {
                    m = p.enemyMinions[target - 10];
                    if (m.Hp == m.maxHp) return 500;
                    // no penality if we heal enrage enemy
                    if (enrageDatabase.ContainsKey(m.name))
                    {
                        return pen;
                    }
                    // no penality if we have heal-trigger :D
                    int i = 0;
                    foreach (Minion mnn in p.ownMinions)
                    {
                        if (mnn.name == "northshirecleric") i++;
                        if (mnn.name == "lightwarden") i++;
                    }
                    foreach (Minion mnn in p.enemyMinions)
                    {
                        if (mnn.name == "northshirecleric") i--;
                        if (mnn.name == "lightwarden") i--;
                    }
                    if (i >= 1) return pen;

                    // no pen if we have slam

                    foreach (Handmanager.Handcard hc in p.owncards)
                    {
                        if (hc.card.name == "slam" && m.Hp < 2) return pen;
                        if (hc.card.name == "backstab") return pen;
                    }

                    pen = 500;
                }


            }

            return pen;
        }

        private int getCardDrawPenality(string name, int target, Playfield p, int choice)
        {
            // penality if carddraw is late or you have enough cards
            int pen = 0;
            if (!cardDrawBattleCryDatabase.ContainsKey(name)) return 0;
            if (name == "ancientoflore" && choice != 1) return 0;
            if (name == "wrath" && choice != 2) return 0;
            if (name == "nourish" && choice != 2) return 0;
            int carddraw = cardDrawBattleCryDatabase[name];
            if (name == "harrisonjones")
            {
                carddraw = p.enemyWeaponDurability;
                if (carddraw == 0 && (p.enemyHeroName != HeroEnum.mage && p.enemyHeroName != HeroEnum.warlock && p.enemyHeroName != HeroEnum.priest)) return 5;
            }
            if (name == "divinefavor")
            {
                carddraw = p.enemyAnzCards + p.enemycarddraw - (p.owncards.Count);
                if (carddraw == 0) return 500;
            }

            if (name == "battlerage")
            {
                carddraw = 0;
                foreach (Minion mnn in p.ownMinions)
                {
                    if (mnn.wounded) carddraw++;
                }
                if (carddraw == 0) return 500;
            }

            if (name == "slam")
            {
                Minion m = new Minion();
                if (target >= 0 && target <= 9)
                {
                    m = p.ownMinions[target];
                }
                if (target >= 10 && target <= 19)
                {
                    m = p.enemyMinions[target - 10];
                }
                carddraw = 0;
                if (m.Hp >= 3) carddraw = 1;
                if (carddraw == 0) return 4;
            }

            if (name == "mortalcoil")
            {
                Minion m = new Minion();
                if (target >= 0 && target <= 9)
                {
                    m = p.ownMinions[target];
                }
                if (target >= 10 && target <= 19)
                {
                    m = p.enemyMinions[target - 10];
                }
                carddraw = 0;
                if (m.Hp == 1) carddraw = 1;
                if (carddraw == 0) return 3;
            }

            if (p.owncards.Count + carddraw > 10) return 15 * (p.owncarddraw + p.owncards.Count - 10);
            if (p.owncards.Count + p.cardsPlayedThisTurn > 5) return 5;

            return -carddraw + p.playactions.Count + p.ownMaxMana - p.mana;
            /*pen = -carddraw + p.ownMaxMana - p.mana;
            return pen;*/
        }

        private int getCardDrawofEffectMinions(CardDB.Card card, Playfield p)
        {
            int pen = 0;
            int carddraw = 0;
            if (card.type == CardDB.cardtype.SPELL)
            {
                foreach (Minion mnn in p.ownMinions)
                {
                    if (mnn.name == "gadgetzanauctioneer") carddraw++;
                }
            }

            if (card.type == CardDB.cardtype.MOB && (TAG_RACE)card.race == TAG_RACE.PET)
            {
                foreach (Minion mnn in p.ownMinions)
                {
                    if (mnn.name == "starvingbuzzard") carddraw++;
                }
            }

            if (carddraw == 0) return 0;

            if (p.owncards.Count >= 5) return 0;
            pen = -carddraw + p.ownMaxMana - p.mana + p.playactions.Count;

            return pen;
        }

        private int getCardDiscardPenality(string name, Playfield p)
        {
            if (p.owncards.Count <= 1) return 0;
            int pen = 0;
            if (this.cardDiscardDatabase.ContainsKey(name))
            {
                int newmana = p.mana - cardDiscardDatabase[name];
                bool canplayanothercard = false;
                foreach (Handmanager.Handcard hc in p.owncards)
                {
                    if (this.cardDiscardDatabase.ContainsKey(hc.card.name)) continue;
                    if (hc.card.getManaCost(p, hc.manacost) <= newmana)
                    {
                        canplayanothercard = true;
                    }
                }
                if (canplayanothercard) pen += 10;

            }

            return pen;
        }

        private int getDestroyOwnPenality(string name, int target, Playfield p, bool lethal)
        {
            if (!this.destroyOwnDatabase.ContainsKey(name)) return 0;
            int pen = 0;
            if ((name == "brawl" || name == "deathwing" || name == "twistingnether") && p.mobsplayedThisTurn >= 1) return 500;

            if (name == "brawl" || name == "twistingnether")
            {
                int highminion = 0;
                int veryhighminion = 0;
                foreach (Minion m in p.enemyMinions)
                {
                    if (m.Angr >= 5 || m.Hp >= 5) highminion++;
                    if (m.Angr >= 8 || m.Hp >= 8) veryhighminion++;
                }

                if (highminion >= 2 || veryhighminion >= 1)
                {
                    return 0;
                }

                if (p.enemyMinions.Count <= 2 || p.enemyMinions.Count + 2 <= p.ownMinions.Count || p.ownMinions.Count >= 3)
                {
                    return 30;
                }
            }

            if (target >= 0 && target <= 9)
            {
                // dont destroy owns ;_; (except mins with deathrattle effects)

                Minion m = p.ownMinions[target];
                if (m.handcard.card.deathrattle) return 10;
                if (lethal && name == "sacrificialpact")
                {
                    int beasts = 0;
                    foreach (Minion mm in p.ownMinions)
                    {
                        if (mm.Ready && mm.handcard.card.specialMin == CardDB.specialMinions.lightwarden) beasts++;
                    }
                    if (beasts == 0) return 500;
                }
                else
                {

                    return 500;
                }
            }

            return pen;
        }

        private int getDestroyPenality(string name, int target, Playfield p)
        {
            if (!this.destroyDatabase.ContainsKey(name)) return 0;
            int pen = 0;

            if (target >= 10 && target <= 19)
            {
                // dont destroy owns ;_; (except mins with deathrattle effects)

                Minion m = p.enemyMinions[target - 10];

                if (m.Angr >= 4 || m.Hp >= 5)
                {
                    pen = 0; // so we dont destroy cheap ones :D
                }
                else
                {
                    pen = 25;
                }

            }

            return pen;
        }

        private int getSpecialCardComboPenalitys(CardDB.Card card, int target, Playfield p, bool lethal, int choice)
        {
            string name = card.name;

            if (lethal && card.type == CardDB.cardtype.MOB)
            {

                if (this.buffingMinionsDatabase.ContainsKey(name))
                {
                    if (name == "timberwolf" || name == "houndmaster")
                    {
                        int beasts = 0;
                        foreach (Minion mm in p.ownMinions)
                        {
                            if ((TAG_RACE)mm.handcard.card.race == TAG_RACE.PET) beasts++;
                        }
                        if (beasts == 0) return 500;
                    }
                    if (name == "southseacaptain")
                    {
                        int beasts = 0;
                        foreach (Minion mm in p.ownMinions)
                        {
                            if ((TAG_RACE)mm.handcard.card.race == TAG_RACE.PIRATE) beasts++;
                        }
                        if (beasts == 0) return 500;
                    }
                    if (name == "murlocwarleader" || name == "grimscaleoracle" || name == "coldlightseer")
                    {
                        int beasts = 0;
                        foreach (Minion mm in p.ownMinions)
                        {
                            if ((TAG_RACE)mm.handcard.card.race == TAG_RACE.MURLOC) beasts++;
                        }
                        if (beasts == 0) return 500;
                    }
                }
                else
                {
                    if (name == "theblackknight")
                    {
                        int beasts = 0;
                        foreach (Minion mm in p.enemyMinions)
                        {
                            if (mm.taunt) beasts++;
                        }
                        if (beasts == 0) return 500;
                    }
                    else
                    {
                        if ((this.HealTargetDatabase.ContainsKey(name) || this.HealHeroDatabase.ContainsKey(name) || this.HealAllDatabase.ContainsKey(name)))
                        {
                            int beasts = 0;
                            foreach (Minion mm in p.ownMinions)
                            {
                                if (mm.Ready && mm.handcard.card.specialMin == CardDB.specialMinions.lightwarden) beasts++;
                            }
                            if (beasts == 0) return 500;
                        }
                        else
                        {
                            if (!(name == "nightblade" || card.Charge || this.silenceDatabase.ContainsKey(name) || ((TAG_RACE)card.race == TAG_RACE.PET && p.ownMinions.Find(x => x.name == "tundrarhino") != null) || (p.ownMinions.Find(x => x.name == "warsongcommander") != null && card.Attack <= 3) || p.owncards.Find(x => x.card.name == "charge") != null))
                            {
                                return 500;
                            }
                        }
                    }
                }
            }
            //some effects, which are bad :D
            int pen = 0;
            Minion m = new Minion();
            if (target >= 0 && target <= 9)
            {
                m = p.ownMinions[target];
            }
            if (target >= 10 && target <= 19)
            {
                m = p.enemyMinions[target - 10];
            }

            if (card.specialMin == CardDB.specialMinions.flametonguetotem && p.ownMinions.Count == 0)
            {
                return 100;
            }

            if (name == "sylvanaswindrunner")
            {
                if (p.enemyMinions.Count == 0)
                {
                    return 10;
                }
            }

            if (name == "houndmaster")
            {
                if (target == -1) return 50;
            }

            if (name == "bite")
            {
                if ((p.ownHeroNumAttackThisTurn == 0 || (p.ownHeroWindfury && p.ownHeroNumAttackThisTurn == 1)) && !p.ownHeroFrozen)
                {

                }
                else
                {
                    return 20;
                }
            }

            if (name == "deadlypoison")
            {
                return p.ownWeaponDurability * 2;
            }

            if (name == "coldblood")
            {
                if (lethal) return 0;
                return 25;
            }

            if (name == "bloodmagethalnos")
            {
                return 10;
            }

            if (name == "frostbolt")
            {
                if (target >= 10 && target <= 19)
                {
                    if (m.handcard.card.cost <= 2)
                        return 15;
                }
                return 15;
            }

            if (!lethal && choice == 1 && name == "druidoftheclaw")
            {
                return 20;
            }


            if (name == "poweroverwhelming")
            {
                if (target >= 0 && target <= 9 && !m.Ready)
                {
                    return 500;
                }
            }

            if (name == "frothingberserker")
            {
                if (p.cardsPlayedThisTurn >= 1) pen = 5;
            }

            if (name == "handofprotection")
            {
                if (m.Hp == 1) pen = 15;
            }

            if (lethal)
            {
                if (name == "corruption")
                {
                    int beasts = 0;
                    foreach (Minion mm in p.ownMinions)
                    {
                        if (mm.Ready && (mm.handcard.card.specialMin == CardDB.specialMinions.questingadventurer || mm.handcard.card.specialMin == CardDB.specialMinions.archmageantonidas || mm.handcard.card.specialMin == CardDB.specialMinions.manaaddict || mm.handcard.card.specialMin == CardDB.specialMinions.manawyrm || mm.handcard.card.specialMin == CardDB.specialMinions.wildpyromancer)) beasts++;
                    }
                    if (beasts == 0) return 500;
                }
            }

            if (name == "divinespirit")
            {
                if (lethal)
                {
                    if (target >= 10 && target <= 19)
                    {
                        if (!m.taunt)
                        {
                            return 500;
                        }
                        else
                        {
                            // combo for killing with innerfire and biggamehunter
                            if (p.owncards.Find(x => x.card.specialMin == CardDB.specialMinions.biggamehunter) != null && p.owncards.Find(x => x.card.name == "innerfire") != null && (m.Hp >= 4 || (p.owncards.Find(x => x.card.name == "divinespirit") != null && m.Hp >= 2)))
                            {
                                return 0;
                            }
                            return 500;
                        }
                    }
                }
                else
                {
                    if (target >= 10 && target <= 19)
                    {

                        // combo for killing with innerfire and biggamehunter
                        if (p.owncards.Find(x => x.card.specialMin == CardDB.specialMinions.biggamehunter) != null && p.owncards.Find(x => x.card.name == "innerfire") != null && m.Hp >= 4)
                        {
                            return 0;
                        }
                        return 500;
                    }

                }

                if (target >= 0 && target <= 9)
                {

                    if (m.Hp >= 4)
                    {
                        return 0;
                    }
                    return 15;
                }

            }

            if (name == "facelessmanipulator")
            {
                if (target == -1)
                {
                    return 50;
                }
                if (m.Angr >= 5 || m.handcard.card.cost >= 5 || (m.handcard.card.rarity == 5 || m.handcard.card.cost >= 3))
                {
                    return 0;
                }
                return 49;
            }

            if (name == "knifejuggler")
            {
                if (p.mobsplayedThisTurn >= 1)
                {
                    return 10;
                }
            }

            if ((name == "polymorph" || name == "hex"))
            {

                if (target >= 0 && target <= 9)
                {
                    return 500;
                }

                if (target >= 10 && target <= 19)
                {
                    Minion frog = p.enemyMinions[target - 10];
                    if (this.priorityTargets.ContainsKey(frog.name)) return 0;
                    if (frog.Angr >= 4 && frog.Hp >= 4) return 0;
                    return 30;
                }

            }

            if ((card.specialMin == CardDB.specialMinions.biggamehunter) && (target == -1 || target <= 9))
            {
                return 40;
            }

            if ((name == "defenderofargus" || name == "sunfuryprotector") && p.ownMinions.Count == 1)
            {
                return 40;
            }
            if ((name == "defenderofargus" || name == "sunfuryprotector") && p.ownMinions.Count == 0)
            {
                return 50;
            }

            if (name == "unleashthehounds")
            {
                if (p.enemyMinions.Count <= 1)
                {
                    return 20;
                }
            }

            if (name == "equality") // aoe penality
            {
                if (p.enemyMinions.Count <= 2 || (p.ownMinions.Count - p.enemyMinions.Count >= 1))
                {
                    return 20;
                }
            }

            if (name == "bloodsailraider" && p.ownWeaponDurability == 0)
            {
                //if you have bloodsailraider and no weapon equiped, but own a weapon:
                foreach (Handmanager.Handcard hc in p.owncards)
                {
                    if (hc.card.type == CardDB.cardtype.WEAPON) return 10;
                }
            }

            if (name == "theblackknight")
            {
                if (target == -1)
                {
                    return 50;
                }

                foreach (Minion mnn in p.enemyMinions)
                {
                    if (mnn.taunt && (m.Angr >= 3 || m.Hp >= 3)) return 0;
                }
                return 20;
            }

            if (name == "innerfire")
            {
                if (m.name == "lightspawn") pen = 500;
            }

            if (name == "huntersmark")
            {
                if (target >= 0 && target <= 9) pen = 500; // dont use on own minions
                if (target >= 10 && target <= 19 && (p.enemyMinions[target - 10].Hp <= 4) && p.enemyMinions[target - 10].Angr <= 4) // only use on strong minions
                {
                    pen = 20;
                }
            }
            if (name == "aldorpeacekeeper" && target == -1)
            {
                pen = 30;
            }
            if ((name == "aldorpeacekeeper" || name == "humility") && target >= 0 && target <= 19)
            {
                if (target >= 0 && target <= 9) pen = 500; // dont use on own minions
                if (target >= 10 && target <= 19 && p.enemyMinions[target - 10].Angr <= 3) // only use on strong minions
                {
                    pen = 30;
                }
                if (m.name == "lightspawn") pen = 500;
            }

            if (name == "shatteredsuncleric" && target == -1) { pen = 10; }
            if (name == "argentprotector" && target == -1) { pen = 10; }

            if (name == "defiasringleader" && p.cardsPlayedThisTurn == 0)
            { pen = 10; }
            if (name == "bloodknight")
            {
                int shilds = 0;
                foreach (Minion min in p.ownMinions)
                {
                    if (min.divineshild)
                    {
                        shilds++;
                    }
                }
                foreach (Minion min in p.enemyMinions)
                {
                    if (min.divineshild)
                    {
                        shilds++;
                    }
                }
                if (shilds == 0)
                {
                    pen = 10;
                }
            }
            if (name == "direwolfalpha")
            {
                int ready = 0;
                foreach (Minion min in p.ownMinions)
                {
                    if (min.Ready)
                    { ready++; }
                }
                if (ready == 0)
                { pen = 5; }
            }
            if (name == "abusivesergeant")
            {
                int ready = 0;
                foreach (Minion min in p.ownMinions)
                {
                    if (min.Ready)
                    { ready++; }
                }
                if (ready == 0)
                {
                    pen = 5;
                }
            }


            if (returnHandDatabase.ContainsKey(name))
            {
                if (name == "vanish")
                {
                    //dont vanish if we have minons on board wich are ready
                    bool haveready = false;
                    foreach (Minion mins in p.ownMinions)
                    {
                        if (mins.Ready) haveready = true;
                    }
                    if (haveready) pen += 10;
                }

                if (target >= 0 && target <= 9)
                {
                    Minion mnn = p.ownMinions[target];
                    if (mnn.Ready) pen += 10;
                }
            }

            return pen;
        }

        private int playSecretPenality(CardDB.Card card, Playfield p)
        {
            //penality if we play secret and have playable kirintormage
            int pen = 0;
            if (card.Secret)
            {
                foreach (Handmanager.Handcard hc in p.owncards)
                {
                    if (hc.card.name == "kirintormage" && p.mana >= hc.getManaCost(p))
                    {
                        pen = 500;
                    }
                }
            }

            return pen;
        }

        ///secret strategys pala
        /// -Attack lowest enemy. If you can’t, use noncombat means to kill it. 
        /// -attack with something able to withstand 2 damage. 
        /// -Then play something that had low health to begin with to dodge Repentance. 
        /// 
        ///secret strategys hunter
        /// - kill enemys with your minions with 2 or less heal.
        ///  - Use the smallest minion available for the first attack 
        ///  - Then smack them in the face with whatever’s left. 
        ///  - If nothing triggered until then, it’s a Snipe, so throw something in front of it that won’t die or is expendable.
        /// 
        ///secret strategys mage
        /// - Play a small minion to trigger Mirror Entity.
        /// Then attack the mage directly with the smallest minion on your side. 
        /// If nothing triggered by that point, it’s either Spellbender or Counterspell, so hold your spells until you can (and have to!) deal with either. 

        private int getPlayCardSecretPenality(CardDB.Card c, Playfield p)
        {
            int pen = 0;
            if (p.enemySecretCount == 0)
            {
                return 0;
            }

            int attackedbefore = 0;

            foreach (Minion mnn in p.ownMinions)
            {
                if (mnn.numAttacksThisTurn >= 1) attackedbefore++;
            }

            if (c.name == "acidicswampooze" && (p.enemyHeroName == HeroEnum.warrior || p.enemyHeroName == HeroEnum.thief || p.enemyHeroName == HeroEnum.pala))
            {
                if (p.enemyHeroName == HeroEnum.thief && p.enemyWeaponAttack <= 2)
                {
                    pen += 100;
                }
                else
                {
                    if (p.enemyWeaponAttack <= 1)
                    {
                        pen += 100;
                    }
                }
            }

            if (p.enemyHeroName == HeroEnum.hunter)
            {
                if (c.type == CardDB.cardtype.MOB && (attackedbefore == 0 || c.Health <= 4 || (p.enemyHeroHp >= p.enemyHeroHpStarted && attackedbefore >= 1)))
                {
                    pen += 10;
                }
            }

            if (p.enemyHeroName == HeroEnum.mage)
            {
                if (c.type == CardDB.cardtype.MOB)
                {
                    Minion m = new Minion();
                    m.Hp = c.Health;
                    m.maxHp = c.Health;
                    m.Angr = c.Attack;
                    m.taunt = c.tank;
                    m.name = c.name;
                    //play first the small minion:
                    if ((!isOwnLowestInHand(m, p) && p.mobsplayedThisTurn == 0) || (p.mobsplayedThisTurn == 0 && attackedbefore >= 1)) pen += 10;
                }

                if (c.type == CardDB.cardtype.SPELL && p.cardsPlayedThisTurn == p.mobsplayedThisTurn)
                {
                    pen += 10;
                }

            }

            if (p.enemyHeroName == HeroEnum.pala)
            {
                if (c.type == CardDB.cardtype.MOB)
                {
                    Minion m = new Minion();
                    m.Hp = c.Health;
                    m.maxHp = c.Health;
                    m.Angr = c.Attack;
                    m.taunt = c.tank;
                    m.name = c.name;
                    if ((!isOwnLowestInHand(m, p) && p.mobsplayedThisTurn == 0) || attackedbefore == 0) pen += 10;
                }


            }



            return pen;
        }

        private int getAttackSecretPenality(Minion m, Playfield p, int target)
        {
            if (p.enemySecretCount == 0)
            {
                return 0;
            }

            int pen = 0;

            int attackedbefore = 0;

            foreach (Minion mnn in p.ownMinions)
            {
                if (mnn.numAttacksThisTurn >= 1) attackedbefore++;
            }

            if (p.enemyHeroName == HeroEnum.hunter)
            {
                bool islow = isOwnLowest(m, p);
                if (attackedbefore == 0 && islow) pen -= 20;
                if (attackedbefore == 0 && !islow) pen += 10;

                if (target == 200 && p.enemyMinions.Count >= 1)
                {
                    //penality if we doestn attacked before
                    if (hasMinionsWithLowHeal(p)) pen += 10; //penality if we doestn attacked minions before
                }
            }

            if (p.enemyHeroName == HeroEnum.mage)
            {
                if (p.mobsplayedThisTurn == 0) pen += 10;

                bool islow = isOwnLowest(m, p);

                if (target == 200 && !islow)
                {
                    pen += 10;
                }
                if (target == 200 && islow && p.mobsplayedThisTurn >= 1)
                {
                    pen -= 20;
                }

            }

            if (p.enemyHeroName == HeroEnum.pala)
            {

                bool islow = isOwnLowest(m, p);

                if (target >= 10 && target <= 20 && attackedbefore == 0)
                {
                    Minion enem = p.enemyMinions[target - 10];
                    if (!isEnemyLowest(enem, p) || m.Hp <= 2) pen += 5;
                }

                if (target == 200 && !islow)
                {
                    pen += 5;
                }

                if (target == 200 && p.enemyMinions.Count >= 1 && attackedbefore == 0)
                {
                    pen += 5;
                }

            }


            return pen;
        }






        private int getValueOfMinion(Minion m)
        {
            int ret = 0;
            ret += 2 * m.Angr + m.Hp;
            if (m.taunt) ret += 2;
            if (this.priorityDatabase.ContainsKey(m.name)) ret += 20 + priorityDatabase[m.name];
            return ret;
        }

        private bool isOwnLowest(Minion mnn, Playfield p)
        {
            bool ret = true;
            int val = getValueOfMinion(mnn);
            foreach (Minion m in p.ownMinions)
            {
                if (!m.Ready) continue;
                if (getValueOfMinion(m) < val) ret = false;
            }
            return ret;
        }

        private bool isOwnLowestInHand(Minion mnn, Playfield p)
        {
            bool ret = true;
            Minion m = new Minion();
            int val = getValueOfMinion(mnn);
            foreach (Handmanager.Handcard card in p.owncards)
            {
                if (card.card.type != CardDB.cardtype.MOB) continue;
                CardDB.Card c = card.card;
                m.Hp = c.Health;
                m.maxHp = c.Health;
                m.Angr = c.Attack;
                m.taunt = c.tank;
                m.name = c.name;
                if (getValueOfMinion(m) < val) ret = false;
            }
            return ret;
        }

        private int getValueOfEnemyMinion(Minion m)
        {
            int ret = 0;
            ret += m.Hp;
            if (m.taunt) ret -= 2;
            return ret;
        }

        private bool isEnemyLowest(Minion mnn, Playfield p)
        {
            bool ret = true;
            List<targett> litt = p.getAttackTargets(true);
            int val = getValueOfEnemyMinion(mnn);
            foreach (Minion m in p.enemyMinions)
            {
                if (litt.Find(x => x.target == m.id) == null) continue;
                if (getValueOfEnemyMinion(m) < val) ret = false;
            }
            return ret;
        }

        private bool hasMinionsWithLowHeal(Playfield p)
        {
            bool ret = false;
            foreach (Minion m in p.ownMinions)
            {
                if (m.Hp <= 2 && (m.Ready || this.priorityDatabase.ContainsKey(m.name))) ret = true;
            }
            return ret;
        }



        private void setupEnrageDatabase()
        {
            enrageDatabase.Add("amaniberserker", 0);
            enrageDatabase.Add("angrychicken", 0);
            enrageDatabase.Add("grommashhellscream", 0);
            enrageDatabase.Add("ragingworgen", 0);
            enrageDatabase.Add("spitefulsmith", 0);
            enrageDatabase.Add("taurenwarrior", 0);
        }

        private void setupHealDatabase()
        {
            HealAllDatabase.Add("holynova", 2);//to all own minions
            HealAllDatabase.Add("circleofhealing", 4);//allminions
            HealAllDatabase.Add("darkscalehealer", 2);//all friends

            HealHeroDatabase.Add("drainlife", 2);//tohero
            HealHeroDatabase.Add("guardianofkings", 6);//tohero
            HealHeroDatabase.Add("holyfire", 5);//tohero
            HealHeroDatabase.Add("priestessofelune", 4);//tohero
            HealHeroDatabase.Add("sacrificialpact", 5);//tohero
            HealHeroDatabase.Add("siphonsoul", 3); //tohero

            HealTargetDatabase.Add("ancestralhealing", 1000);
            HealTargetDatabase.Add("ancientsecrets", 5);
            HealTargetDatabase.Add("holylight", 6);
            HealTargetDatabase.Add("earthenringfarseer", 3);
            HealTargetDatabase.Add("healingtouch", 8);
            HealTargetDatabase.Add("layonhands", 8);
            HealTargetDatabase.Add("lesserheal", 2);
            HealTargetDatabase.Add("voodoodoctor", 2);
            HealTargetDatabase.Add("willofmukla", 8);
            HealTargetDatabase.Add("ancientoflore", 5);
            //HealTargetDatabase.Add("divinespirit", 2);
        }

        private void setupDamageDatabase()
        {

            DamageHeroDatabase.Add("headcrack", 2);

            DamageAllDatabase.Add("abomination", 2);
            DamageAllDatabase.Add("dreadinfernal", 1);
            DamageAllDatabase.Add("hellfire", 3);
            DamageAllDatabase.Add("whirlwind", 1);
            DamageAllDatabase.Add("yseraawakens", 5);

            DamageAllEnemysDatabase.Add("arcaneexplosion", 1);
            DamageAllEnemysDatabase.Add("consecration", 1);
            DamageAllEnemysDatabase.Add("fanofknives", 1);
            DamageAllEnemysDatabase.Add("flamestrike", 4);
            DamageAllEnemysDatabase.Add("holynova", 2);
            DamageAllEnemysDatabase.Add("lightningstorm", 2);
            DamageAllEnemysDatabase.Add("stomp", 1);
            DamageAllEnemysDatabase.Add("madbomber", 1);
            DamageAllEnemysDatabase.Add("swipe", 4);//1 to others

            DamageRandomDatabase.Add("arcanemissiles", 1);
            DamageRandomDatabase.Add("avengingwrath", 1);
            DamageRandomDatabase.Add("cleave", 2);
            DamageRandomDatabase.Add("forkedlightning", 2);
            DamageRandomDatabase.Add("multi-shot", 3);

            DamageTargetSpecialDatabase.Add("crueltaskmaster", 1); // gives 2 attack
            DamageTargetSpecialDatabase.Add("innerrage", 1); // gives 2 attack

            DamageTargetSpecialDatabase.Add("demonfire", 2); // friendly demon get +2/+2
            DamageTargetSpecialDatabase.Add("earthshock", 1); //SILENCE /good for raggy etc or iced
            DamageTargetSpecialDatabase.Add("hammerofwrath", 3); //draw a card
            DamageTargetSpecialDatabase.Add("holywrath", 2);//draw a card
            DamageTargetSpecialDatabase.Add("roguesdoit...", 4);//draw a card
            DamageTargetSpecialDatabase.Add("shiv", 1);//draw a card
            DamageTargetSpecialDatabase.Add("savagery", 1);//dmg=herodamage
            DamageTargetSpecialDatabase.Add("shieldslam", 1);//dmg=armor
            DamageTargetSpecialDatabase.Add("slam", 2);//draw card if it survives
            DamageTargetSpecialDatabase.Add("soulfire", 4);//delete a card


            DamageTargetDatabase.Add("keeperofthegrove", 2); // or silence
            DamageTargetDatabase.Add("wrath", 3);//or 1 + card

            DamageTargetDatabase.Add("coneofcold", 1);
            DamageTargetDatabase.Add("arcaneshot", 2);
            DamageTargetDatabase.Add("backstab", 2);
            DamageTargetDatabase.Add("baneofdoom", 2);
            DamageTargetDatabase.Add("barreltoss", 2);
            DamageTargetDatabase.Add("blizzard", 2);
            DamageTargetDatabase.Add("drainlife", 2);
            DamageTargetDatabase.Add("elvenarcher", 1);
            DamageTargetDatabase.Add("eviscerate", 3);
            DamageTargetDatabase.Add("explosiveshot", 5);
            DamageTargetDatabase.Add("fireelemental", 3);
            DamageTargetDatabase.Add("fireball", 6);
            DamageTargetDatabase.Add("fireblast", 1);
            DamageTargetDatabase.Add("frostshock", 1);
            DamageTargetDatabase.Add("frostbolt", 1);
            DamageTargetDatabase.Add("hoggersmash", 4);
            DamageTargetDatabase.Add("holyfire", 5);
            DamageTargetDatabase.Add("holysmite", 2);
            DamageTargetDatabase.Add("icelance", 4);//only if iced
            DamageTargetDatabase.Add("ironforgerifleman", 1);
            DamageTargetDatabase.Add("killcommand", 3);//or 5
            DamageTargetDatabase.Add("lavaburst", 5);
            DamageTargetDatabase.Add("lightningbolt", 2);
            DamageTargetDatabase.Add("mindshatter", 3);
            DamageTargetDatabase.Add("mindspike", 2);
            DamageTargetDatabase.Add("moonfire", 1);
            DamageTargetDatabase.Add("mortalcoil", 1);
            DamageTargetDatabase.Add("mortalstrike", 4);
            DamageTargetDatabase.Add("perditionsblade", 1);
            DamageTargetDatabase.Add("pyroblast", 10);
            DamageTargetDatabase.Add("shadowbolt", 4);
            DamageTargetDatabase.Add("shotgunblast", 1);
            DamageTargetDatabase.Add("si7agent", 2);
            DamageTargetDatabase.Add("starfall", 5);
            DamageTargetDatabase.Add("starfire", 5);//draw a card, but its to strong
            DamageTargetDatabase.Add("stormpikecommando", 5);






        }

        private void setupsilenceDatabase()
        {
            this.silenceDatabase.Add("dispel", 1);
            this.silenceDatabase.Add("earthshock", 1);
            this.silenceDatabase.Add("massdispel", 1);
            this.silenceDatabase.Add("silence", 1);
            this.silenceDatabase.Add("keeperofthegrove", 1);
            this.silenceDatabase.Add("ironbeakowl", 1);
            this.silenceDatabase.Add("spellbreaker", 1);
        }

        private void setupPriorityList()
        {
            this.priorityDatabase.Add("prophetvelen", 5);
            this.priorityDatabase.Add("archmageantonidas", 5);
            this.priorityDatabase.Add("flametonguetotem", 6);
            this.priorityDatabase.Add("raidleader", 5);
            this.priorityDatabase.Add("grimscaleoracle", 5);
            this.priorityDatabase.Add("direwolfalpha", 6);
            this.priorityDatabase.Add("murlocwarleader", 5);
            this.priorityDatabase.Add("southseacaptain", 5);
            this.priorityDatabase.Add("stormwindchampion", 5);
            this.priorityDatabase.Add("timberwolf", 5);
            this.priorityDatabase.Add("leokk", 5);
            this.priorityDatabase.Add("northshirecleric", 5);
            this.priorityDatabase.Add("sorcerersapprentice", 3);
            this.priorityDatabase.Add("summoningportal", 5);
            this.priorityDatabase.Add("pint-sizedsummoner", 3);
            this.priorityDatabase.Add("scavenginghyena", 5);
            this.priorityDatabase.Add("manatidetotem ", 5);
        }

        private void setupAttackBuff()
        {
            heroAttackBuffDatabase.Add("bite", 4);
            heroAttackBuffDatabase.Add("claw", 2);
            heroAttackBuffDatabase.Add("heroicstrike", 2);

            this.attackBuffDatabase.Add("abusivesergeant", 2);
            this.attackBuffDatabase.Add("ancientofwar", 5); //choice1
            this.attackBuffDatabase.Add("bananas", 1);
            this.attackBuffDatabase.Add("bestialwrath", 2); // NEVER ON enemy MINION
            this.attackBuffDatabase.Add("blessingofkings", 4);
            this.attackBuffDatabase.Add("blessingofmight", 3);
            this.attackBuffDatabase.Add("coldblood", 2);
            this.attackBuffDatabase.Add("crueltaskmaster", 2);
            this.attackBuffDatabase.Add("darkirondwarf", 2);
            this.attackBuffDatabase.Add("innerrage", 2);
            this.attackBuffDatabase.Add("markofnature", 4);//choice1 
            this.attackBuffDatabase.Add("markofthewild", 2);
            this.attackBuffDatabase.Add("nightmare", 5); //destroy minion on next turn
            this.attackBuffDatabase.Add("rampage", 3);//only damaged minion 
            this.attackBuffDatabase.Add("uproot", 5);

        }

        private void setupHealthBuff()
        {

            this.healthBuffDatabase.Add("ancientofwar", 5);//choice2
            this.healthBuffDatabase.Add("bananas", 1);
            this.healthBuffDatabase.Add("blessingofkings", 4);
            this.healthBuffDatabase.Add("markofnature", 4);//choice2
            this.healthBuffDatabase.Add("markofthewild", 2);
            this.healthBuffDatabase.Add("nightmare", 5);
            this.healthBuffDatabase.Add("powerwordshield", 2);
            this.healthBuffDatabase.Add("rampage", 3);
            this.healthBuffDatabase.Add("rooted", 5);

            this.tauntBuffDatabase.Add("markofnature", 1);
            this.tauntBuffDatabase.Add("markofthewild", 1);
            this.tauntBuffDatabase.Add("rooted", 1);


        }

        private void setupCardDrawBattlecry()
        {
            cardDrawBattleCryDatabase.Add("wrath", 1); //choice=2
            cardDrawBattleCryDatabase.Add("ancientoflore", 2);// choice =1
            cardDrawBattleCryDatabase.Add("nourish", 3); //choice = 2
            cardDrawBattleCryDatabase.Add("ancientteachings", 2);
            cardDrawBattleCryDatabase.Add("excessmana", 1);
            cardDrawBattleCryDatabase.Add("starfire", 1);
            cardDrawBattleCryDatabase.Add("azuredrake", 1);
            cardDrawBattleCryDatabase.Add("coldlightoracle", 2);
            cardDrawBattleCryDatabase.Add("gnomishinventor", 1);
            cardDrawBattleCryDatabase.Add("harrisonjones", 0);
            cardDrawBattleCryDatabase.Add("noviceengineer", 1);
            cardDrawBattleCryDatabase.Add("roguesdoit...", 1);
            cardDrawBattleCryDatabase.Add("arcaneintellect", 1);
            cardDrawBattleCryDatabase.Add("hammerofwrath", 1);
            cardDrawBattleCryDatabase.Add("holywrath", 1);
            cardDrawBattleCryDatabase.Add("layonhands", 3);
            cardDrawBattleCryDatabase.Add("massdispel", 1);
            cardDrawBattleCryDatabase.Add("powerwordshield", 1);
            cardDrawBattleCryDatabase.Add("fanofknives", 1);
            cardDrawBattleCryDatabase.Add("shiv", 1);
            cardDrawBattleCryDatabase.Add("sprint", 4);
            cardDrawBattleCryDatabase.Add("farsight", 1);
            cardDrawBattleCryDatabase.Add("lifetap", 1);
            cardDrawBattleCryDatabase.Add("commandingshout", 1);
            cardDrawBattleCryDatabase.Add("shieldblock", 1);
            cardDrawBattleCryDatabase.Add("slam", 1); //if survives
            cardDrawBattleCryDatabase.Add("mortalcoil", 1);//only if kills
            cardDrawBattleCryDatabase.Add("battlerage", 1);//only if wounded own minions
            cardDrawBattleCryDatabase.Add("divinefavor", 1);//only if enemy has more cards than you
        }

        private void setupDiscardCards()
        {
            cardDiscardDatabase.Add("doomguard", 5);
            cardDiscardDatabase.Add("soulfire", 0);
            cardDiscardDatabase.Add("succubus", 2);
        }

        private void setupDestroyOwnCards()
        {
            this.destroyOwnDatabase.Add("brawl", 0);
            this.destroyOwnDatabase.Add("deathwing", 0);
            this.destroyOwnDatabase.Add("twistingnether", 0);
            this.destroyOwnDatabase.Add("naturalize", 0);//not own mins
            this.destroyOwnDatabase.Add("shadowworddeath", 0);//not own mins
            this.destroyOwnDatabase.Add("shadowwordpain", 0);//not own mins
            this.destroyOwnDatabase.Add("siphonsoul", 0);//not own mins
            this.destroyOwnDatabase.Add("biggamehunter", 0);//not own mins
            this.destroyOwnDatabase.Add("hungrycrab", 0);//not own mins
            this.destroyOwnDatabase.Add("sacrificialpact", 0);//not own mins


            this.destroyDatabase.Add("assassinate", 0);//not own mins
            this.destroyDatabase.Add("corruption", 0);//not own mins
            this.destroyDatabase.Add("execute", 0);//not own mins
            this.destroyDatabase.Add("naturalize", 0);//not own mins
            this.destroyDatabase.Add("siphonsoul", 0);//not own mins
            this.destroyDatabase.Add("mindcontrol", 0);//not own mins

        }

        private void setupReturnBackToHandCards()
        {
            returnHandDatabase.Add("ancientbrewmaster", 0);
            returnHandDatabase.Add("dream", 0);
            returnHandDatabase.Add("kidnapper", 0);//if combo
            returnHandDatabase.Add("shadowstep", 0);
            returnHandDatabase.Add("vanish", 0);
            returnHandDatabase.Add("youthfulbrewmaster", 0);
        }

        private void setupHeroDamagingAOE()
        {
            this.heroDamagingAoeDatabase.Add("", 0);
        }

        private void setupSpecialMins()
        {
            this.specialMinions.Add("amaniberserker", 0);
            this.specialMinions.Add("angrychicken", 0);
            this.specialMinions.Add("abomination", 0);
            this.specialMinions.Add("acolyteofpain", 0);
            this.specialMinions.Add("alarm-o-bot", 0);
            this.specialMinions.Add("archmage", 0);
            this.specialMinions.Add("archmageantonidas", 0);
            this.specialMinions.Add("armorsmith", 0);
            this.specialMinions.Add("auchenaisoulpriest", 0);
            this.specialMinions.Add("azuredrake", 0);
            this.specialMinions.Add("barongeddon", 0);
            this.specialMinions.Add("bloodimp", 0);
            this.specialMinions.Add("bloodmagethalnos", 0);
            this.specialMinions.Add("cairnebloodhoof", 0);
            this.specialMinions.Add("cultmaster", 0);
            this.specialMinions.Add("dalaranmage", 0);
            this.specialMinions.Add("demolisher", 0);
            this.specialMinions.Add("direwolfalpha", 0);
            this.specialMinions.Add("doomsayer", 0);
            this.specialMinions.Add("emperorcobra", 0);
            this.specialMinions.Add("etherealarcanist", 0);
            this.specialMinions.Add("flametonguetotem", 0);
            this.specialMinions.Add("flesheatingghoul", 0);
            this.specialMinions.Add("gadgetzanauctioneer", 0);
            this.specialMinions.Add("grimscaleoracle", 0);
            this.specialMinions.Add("grommashhellscream", 0);
            this.specialMinions.Add("gruul", 0);
            this.specialMinions.Add("gurubashiberserker", 0);
            this.specialMinions.Add("harvestgolem", 0);
            this.specialMinions.Add("hogger", 0);
            this.specialMinions.Add("illidanstormrage", 0);
            this.specialMinions.Add("impmaster", 0);
            this.specialMinions.Add("knifejuggler", 0);
            this.specialMinions.Add("koboldgeomancer", 0);
            this.specialMinions.Add("lepergnome", 0);
            this.specialMinions.Add("lightspawn", 0);
            this.specialMinions.Add("lighwarden", 0);
            this.specialMinions.Add("lightwell", 0);
            this.specialMinions.Add("loothoader", 0);
            this.specialMinions.Add("lorewalkercho", 0);
            this.specialMinions.Add("malygos", 0);
            this.specialMinions.Add("manaaddict", 0);
            this.specialMinions.Add("manatidetotem", 0);
            this.specialMinions.Add("manawraith", 0);
            this.specialMinions.Add("manawyrm", 0);
            this.specialMinions.Add("masterswordsmith", 0);
            this.specialMinions.Add("murloctidecaller", 0);
            this.specialMinions.Add("murlocwarleader", 0);
            this.specialMinions.Add("natpagle", 0);
            this.specialMinions.Add("northshirecleric", 0);
            this.specialMinions.Add("ogremagi", 0);
            this.specialMinions.Add("oldmurk-eye", 0);
            this.specialMinions.Add("patientassasin", 0);
            this.specialMinions.Add("pint-sizedsummoner", 0);
            this.specialMinions.Add("prophetvelen", 0);
            this.specialMinions.Add("questingadventurer", 0);
            this.specialMinions.Add("ragingworgen", 0);
            this.specialMinions.Add("raidleader", 0);
            this.specialMinions.Add("savannahhighmane", 0);
            this.specialMinions.Add("scavenginghyena", 0);
            this.specialMinions.Add("secretkeeper", 0);
            this.specialMinions.Add("sorcerersapprentice", 0);
            this.specialMinions.Add("southseacaptain", 0);
            this.specialMinions.Add("spitefulsmith", 0);
            this.specialMinions.Add("starvingbuzzard", 0);
            this.specialMinions.Add("stormwindchampion", 0);
            this.specialMinions.Add("summoningportal", 0);
            this.specialMinions.Add("sylvanaswindrunner", 0);
            this.specialMinions.Add("taurenwarrior", 0);
            this.specialMinions.Add("thebeast", 0);
            this.specialMinions.Add("timberwolf", 0);
            this.specialMinions.Add("tirionfordring", 0);
            this.specialMinions.Add("tundrarhino", 0);
            this.specialMinions.Add("unboundelemental", 0);
            //this.specialMinions.Add("venturecomercenary", 0);
            this.specialMinions.Add("violentteacher", 0);
            this.specialMinions.Add("warsongcommander", 0);
            this.specialMinions.Add("waterelemental", 0);

            // naxx cards
            this.specialMinions.Add("baronrivendare", 0);
            this.specialMinions.Add("nerubianegg", 0);
            this.specialMinions.Add("undertaker", 0);
            this.specialMinions.Add("dancingswords", 0);
            this.specialMinions.Add("voidcaller", 0);
            this.specialMinions.Add("anubarambusher", 0);
            this.specialMinions.Add("darkcultist", 0);
            this.specialMinions.Add("webspinner", 0);

        }

        private void setupBuffingMinions()
        {
            buffingMinionsDatabase.Add("abusivesergeant", 0);
            buffingMinionsDatabase.Add("captaingreenskin", 0);
            buffingMinionsDatabase.Add("cenarius", 0);
            buffingMinionsDatabase.Add("coldlightseer", 0);
            buffingMinionsDatabase.Add("crueltaskmaster", 0);
            buffingMinionsDatabase.Add("darkirondwarf", 0);
            buffingMinionsDatabase.Add("defenderofargus", 0);
            buffingMinionsDatabase.Add("direwolfalpha", 0);
            buffingMinionsDatabase.Add("flametonguetotem", 0);
            buffingMinionsDatabase.Add("grimscaleoracle", 0);
            buffingMinionsDatabase.Add("houndmaster", 0);
            buffingMinionsDatabase.Add("leokk", 0);
            buffingMinionsDatabase.Add("murlocwarleader", 0);
            buffingMinionsDatabase.Add("raidleader", 0);
            buffingMinionsDatabase.Add("shatteredsuncleric", 0);
            buffingMinionsDatabase.Add("southseacaptain", 0);
            buffingMinionsDatabase.Add("spitefulsmith", 0);
            buffingMinionsDatabase.Add("stormwindchampion", 0);
            buffingMinionsDatabase.Add("templeenforcer", 0);
            buffingMinionsDatabase.Add("timberwolf", 0);

            buffing1TurnDatabase.Add("abusivesergeant", 0);
            buffing1TurnDatabase.Add("darkirondwarf", 0);

        }
        private void setupEnemyTargetPriority()
        {
            priorityTargets.Add("angrychicken", 10);
            priorityTargets.Add("lightwarden", 10);
            priorityTargets.Add("secretkeeper", 10);
            priorityTargets.Add("youngdragonhawk", 10);
            priorityTargets.Add("bloodmagethalnos", 10);
            priorityTargets.Add("direwolfalpha", 10);
            priorityTargets.Add("doomsayer", 10);
            priorityTargets.Add("knifejuggler", 10);
            priorityTargets.Add("koboldgeomancer", 10);
            priorityTargets.Add("manaaddict", 10);
            priorityTargets.Add("masterswordsmith", 10);
            priorityTargets.Add("natpagle", 10);
            priorityTargets.Add("murloctidehunter", 10);
            priorityTargets.Add("pint-sizedsummoner", 10);
            priorityTargets.Add("wildpyromancer", 10);
            priorityTargets.Add("alarm-o-bot", 10);
            priorityTargets.Add("acolyteofpain", 10);
            priorityTargets.Add("demolisher", 10);
            priorityTargets.Add("flesheatingghoul", 10);
            priorityTargets.Add("impmaster", 10);
            priorityTargets.Add("questingadventurer", 10);
            priorityTargets.Add("raidleader", 10);
            priorityTargets.Add("thrallmarfarseer", 10);
            priorityTargets.Add("cultmaster", 10);
            priorityTargets.Add("leeroyjenkins", 10);
            priorityTargets.Add("violetteacher", 10);
            priorityTargets.Add("gadgetzanauctioneer", 10);
            priorityTargets.Add("hogger", 10);
            priorityTargets.Add("illidanstormrage", 10);
            priorityTargets.Add("barongeddon", 10);
            priorityTargets.Add("stormwindchampion", 10);
            priorityTargets.Add("gurubashiberserker", 10);
            priorityTargets.Add("cairnebloodhoof", 19);
            priorityTargets.Add("harvestgolem", 16);

            //warrior cards
            priorityTargets.Add("frothingberserker", 10);
            priorityTargets.Add("warsongcommander", 10);

            //warlock cards
            priorityTargets.Add("summoningportal", 10);

            //shaman cards
            priorityTargets.Add("dustdevil", 10);
            priorityTargets.Add("wrathofairtotem", 1);
            priorityTargets.Add("flametonguetotem", 10);
            priorityTargets.Add("manatidetotem", 10);
            priorityTargets.Add("unboundelemental", 10);

            //rogue cards

            //priest cards
            priorityTargets.Add("northshirecleric", 10);
            priorityTargets.Add("lightwell", 10);
            priorityTargets.Add("auchenaisoulpriest", 10);
            priorityTargets.Add("prophetvelen", 10);

            //paladin cards

            //mage cards
            priorityTargets.Add("manawyrm", 10);
            priorityTargets.Add("sorcererapprentice", 10);
            priorityTargets.Add("etherealarcanist", 10);
            priorityTargets.Add("archmageantonidas", 10);

            //hunter cards
            priorityTargets.Add("timberwolf", 10);
            priorityTargets.Add("scavenginghyena", 10);
            priorityTargets.Add("starvingbuzzard", 10);
            priorityTargets.Add("leokk", 10);
            priorityTargets.Add("tundrarhino", 10);
        }


    }

    public class Helpfunctions
    {

        public static List<T> TakeList<T>(IEnumerable<T> source, int limit)
        {
            List<T> retlist = new List<T>();
            int i = 0;

            foreach (T item in source)
            {
                retlist.Add(item);
                i++;

                if (i >= limit) break;
            }
            return retlist;
        }


        public bool runningbot = false;

        private static Helpfunctions instance;

        public static Helpfunctions Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Helpfunctions();
                }
                return instance;
            }
        }

        private Helpfunctions()
        {

            System.IO.File.WriteAllText(Settings.Instance.logpath + Settings.Instance.logfile, "");
        }

        private bool writelogg = true;
        public void loggonoff(bool onoff)
        {
            //writelogg = onoff;
        }

        public void createNewLoggfile()
        {
            System.IO.File.WriteAllText(Settings.Instance.logpath + Settings.Instance.logfile, "");
        }

        public void logg(string s)
        {


            if (!writelogg) return;
            try
            {
                using (StreamWriter sw = File.AppendText(Settings.Instance.logpath + Settings.Instance.logfile))
                {
                    sw.WriteLine(s);
                }
            }
            catch { }
        }

        public DateTime UnixTimeStampToDateTime(int unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        public void ErrorLog(string s)
        {
            HREngine.API.Utilities.HRLog.Write(s);
        }

    }

    public class ComboBreaker
    {

        enum combotype
        {
            combo,
            target,
            weaponuse
        }


        private List<combo> combos = new List<combo>();
        private static ComboBreaker instance;

        Handmanager hm = Handmanager.Instance;
        Hrtprozis hp = Hrtprozis.Instance;

        public int attackFaceHP = -1;

        class combo
        {
            public combotype type = combotype.combo;
            public int neededMana = 0;
            public Dictionary<string, int> combocards = new Dictionary<string, int>();
            public Dictionary<string, int> cardspen = new Dictionary<string, int>();
            public Dictionary<string, int> combocardsTurn0Mobs = new Dictionary<string, int>();
            public Dictionary<string, int> combocardsTurn0All = new Dictionary<string, int>();
            public Dictionary<string, int> combocardsTurn1 = new Dictionary<string, int>();
            public int penality = 0;
            public int combolength = 0;
            public int combot0len = 0;
            public int combot1len = 0;
            public int combot0lenAll = 0;
            public bool twoTurnCombo = false;
            public int bonusForPlaying = 0;
            public int bonusForPlayingT0 = 0;
            public int bonusForPlayingT1 = 0;
            public string requiredWeapon = "";

            public combo(string s)
            {
                int i = 0;
                this.neededMana = 0;
                requiredWeapon = "";
                this.type = combotype.combo;
                this.twoTurnCombo = false;
                bool fixmana = false;
                if (s.Contains("nxttrn")) this.twoTurnCombo = true;
                if (s.Contains("mana:")) fixmana = true;
                /*foreach (string ding in s.Split(':'))
                {
                    if (i == 0)
                    {
                        if (ding == "c") this.type = combotype.combo;
                        if (ding == "t") this.type = combotype.target;
                        if (ding == "w") this.type = combotype.weaponuse;
                    }
                    if (ding == "" || ding == string.Empty) continue;

                    if (i == 1 && type == combotype.combo)
                    {
                        int m = Convert.ToInt32(ding);
                        neededMana = -1;
                        if (m >= 1) neededMana = m;
                    }
                */
                if (type == combotype.combo)
                {
                    this.combolength = 0;
                    this.combot0len = 0;
                    this.combot1len = 0;
                    this.combot0lenAll = 0;
                    int manat0 = 0;
                    int manat1 = -1;
                    bool t1 = false;
                    foreach (string crdl in s.Split(';')) //ding.Split
                    {
                        if (crdl == "" || crdl == string.Empty) continue;
                        if (crdl == "nxttrn")
                        {
                            t1 = true;
                            continue;
                        }
                        if (crdl.StartsWith("mana:"))
                        {
                            this.neededMana = Convert.ToInt32(crdl.Replace("mana:", ""));
                            continue;
                        }
                        if (crdl.StartsWith("bonus:"))
                        {
                            this.bonusForPlaying = Convert.ToInt32(crdl.Replace("bonus:", ""));
                            continue;
                        }
                        if (crdl.StartsWith("bonusfirst:"))
                        {
                            this.bonusForPlayingT0 = Convert.ToInt32(crdl.Replace("bonusfirst:", ""));
                            continue;
                        }
                        if (crdl.StartsWith("bonussecond:"))
                        {
                            this.bonusForPlayingT1 = Convert.ToInt32(crdl.Replace("bonussecond:", ""));
                            continue;
                        }
                        string crd = crdl.Split(',')[0];
                        if (t1)
                        {
                            manat1 += CardDB.Instance.getCardDataFromID(crd).cost;
                        }
                        else
                        {
                            manat0 += CardDB.Instance.getCardDataFromID(crd).cost;
                        }
                        this.combolength++;

                        if (combocards.ContainsKey(crd))
                        {
                            combocards[crd]++;
                        }
                        else
                        {
                            combocards.Add(crd, 1);
                            cardspen.Add(crd, Convert.ToInt32(crdl.Split(',')[1]));
                        }

                        if (this.twoTurnCombo)
                        {

                            if (t1)
                            {
                                if (this.combocardsTurn1.ContainsKey(crd))
                                {
                                    combocardsTurn1[crd]++;
                                }
                                else
                                {
                                    combocardsTurn1.Add(crd, 1);
                                }
                                this.combot1len++;
                            }
                            else
                            {
                                CardDB.Card lolcrd = CardDB.Instance.getCardDataFromID(crd);
                                if (lolcrd.type == CardDB.cardtype.MOB)
                                {
                                    if (this.combocardsTurn0Mobs.ContainsKey(crd))
                                    {
                                        combocardsTurn0Mobs[crd]++;
                                    }
                                    else
                                    {
                                        combocardsTurn0Mobs.Add(crd, 1);
                                    }
                                    this.combot0len++;
                                }
                                if (lolcrd.type == CardDB.cardtype.WEAPON)
                                {
                                    this.requiredWeapon = lolcrd.name;
                                }
                                if (this.combocardsTurn0All.ContainsKey(crd))
                                {
                                    combocardsTurn0All[crd]++;
                                }
                                else
                                {
                                    combocardsTurn0All.Add(crd, 1);
                                }
                                this.combot0lenAll++;
                            }
                        }


                    }
                    if (!fixmana)
                    {
                        this.neededMana = Math.Max(manat1, manat0);
                    }
                }

                /*if (i == 2 && type == combotype.combo)
                {
                    int m = Convert.ToInt32(ding);
                    penality = 0;
                    if (m >= 1) penality = m;
                }

                i++;
            }*/
            }

            public int isInCombo(List<Handmanager.Handcard> hand, int omm)
            {
                int cardsincombo = 0;
                Dictionary<string, int> combocardscopy = new Dictionary<string, int>(this.combocards);
                foreach (Handmanager.Handcard hc in hand)
                {
                    if (combocardscopy.ContainsKey(hc.card.CardID) && combocardscopy[hc.card.CardID] >= 1)
                    {
                        cardsincombo++;
                        combocardscopy[hc.card.CardID]--;
                    }
                }
                if (cardsincombo == this.combolength && omm < this.neededMana) return 1;
                if (cardsincombo == this.combolength) return 2;
                if (cardsincombo >= 1) return 1;
                return 0;
            }

            public int isMultiTurnComboTurn1(List<Handmanager.Handcard> hand, int omm, List<Minion> ownmins, string weapon)
            {
                if (!twoTurnCombo) return 0;
                int cardsincombo = 0;
                Dictionary<string, int> combocardscopy = new Dictionary<string, int>(this.combocardsTurn1);
                foreach (Handmanager.Handcard hc in hand)
                {
                    if (combocardscopy.ContainsKey(hc.card.CardID) && combocardscopy[hc.card.CardID] >= 1)
                    {
                        cardsincombo++;
                        combocardscopy[hc.card.CardID]--;
                    }
                }
                if (cardsincombo == this.combot1len && omm < this.neededMana) return 1;

                if (cardsincombo == this.combot1len)
                {
                    //search for required minions on field
                    int turn0requires = 0;
                    foreach (string s in combocardsTurn0Mobs.Keys)
                    {
                        foreach (Minion m in ownmins)
                        {
                            if (!m.playedThisTurn && m.handcard.card.CardID == s)
                            {
                                turn0requires++;
                                break;
                            }
                        }
                    }

                    if (requiredWeapon != "" && requiredWeapon != weapon) return 1;

                    if (turn0requires >= combot0len) return 2;

                    return 1;
                }
                if (cardsincombo >= 1) return 1;
                return 0;
            }

            public int isMultiTurnComboTurn0(List<Handmanager.Handcard> hand, int omm)
            {
                if (!twoTurnCombo) return 0;
                int cardsincombo = 0;
                Dictionary<string, int> combocardscopy = new Dictionary<string, int>(this.combocardsTurn0All);
                foreach (Handmanager.Handcard hc in hand)
                {
                    if (combocardscopy.ContainsKey(hc.card.CardID) && combocardscopy[hc.card.CardID] >= 1)
                    {
                        cardsincombo++;
                        combocardscopy[hc.card.CardID]--;
                    }
                }
                if (cardsincombo == this.combot0lenAll && omm < this.neededMana) return 1;

                if (cardsincombo == this.combot0lenAll)
                {
                    return 2;
                }
                if (cardsincombo >= 1) return 1;
                return 0;
            }


            public bool isMultiTurn1Card(CardDB.Card card)
            {
                if (this.combocardsTurn1.ContainsKey(card.CardID))
                {
                    return true;
                }
                return false;
            }

            public bool isCardInCombo(CardDB.Card card)
            {
                if (this.combocards.ContainsKey(card.CardID))
                {
                    return true;
                }
                return false;
            }

            public int hasPlayedCombo(List<Handmanager.Handcard> hand)
            {
                int cardsincombo = 0;
                Dictionary<string, int> combocardscopy = new Dictionary<string, int>(this.combocards);
                foreach (Handmanager.Handcard hc in hand)
                {
                    if (combocardscopy.ContainsKey(hc.card.CardID) && combocardscopy[hc.card.CardID] >= 1)
                    {
                        cardsincombo++;
                        combocardscopy[hc.card.CardID]--;
                    }
                }

                if (cardsincombo >= this.combolength) return this.bonusForPlaying;
                return 0;
            }

            public int hasPlayedTurn0Combo(List<Handmanager.Handcard> hand)
            {
                int cardsincombo = 0;
                Dictionary<string, int> combocardscopy = new Dictionary<string, int>(this.combocardsTurn0All);
                foreach (Handmanager.Handcard hc in hand)
                {
                    if (combocardscopy.ContainsKey(hc.card.CardID) && combocardscopy[hc.card.CardID] >= 1)
                    {
                        cardsincombo++;
                        combocardscopy[hc.card.CardID]--;
                    }
                }

                if (cardsincombo >= this.combot0lenAll) return this.bonusForPlayingT0;
                return 0;
            }

            public int hasPlayedTurn1Combo(List<Handmanager.Handcard> hand)
            {
                int cardsincombo = 0;
                Dictionary<string, int> combocardscopy = new Dictionary<string, int>(this.combocardsTurn1);
                foreach (Handmanager.Handcard hc in hand)
                {
                    if (combocardscopy.ContainsKey(hc.card.CardID) && combocardscopy[hc.card.CardID] >= 1)
                    {
                        cardsincombo++;
                        combocardscopy[hc.card.CardID]--;
                    }
                }

                if (cardsincombo >= this.combot1len) return this.bonusForPlayingT1;
                return 0;
            }

        }

        public static ComboBreaker Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ComboBreaker();
                }
                return instance;
            }
        }

        private ComboBreaker()
        {
            readCombos();
            if (attackFaceHP != -1)
            {
                hp.setAttackFaceHP(attackFaceHP);
            }
        }

        private void readCombos()
        {
            string[] lines = new string[0] { };
            combos.Clear();
            try
            {
                string path = Settings.Instance.path;
                lines = System.IO.File.ReadAllLines(path + "_combo.txt");
            }
            catch
            {
                Helpfunctions.Instance.logg("cant find _combo.txt");
                Helpfunctions.Instance.ErrorLog("cant find _combo.txt (if you dont created your own combos, ignore this message)");
                return;
            }
            Helpfunctions.Instance.logg("read _combo.txt...");
            Helpfunctions.Instance.ErrorLog("read _combo.txt...");
            foreach (string line in lines)
            {

                if (line.Contains("weapon:"))
                {
                    try
                    {
                        this.attackFaceHP = Convert.ToInt32(line.Replace("weapon:", ""));
                    }
                    catch
                    {
                        Helpfunctions.Instance.logg("combomaker cant read: " + line);
                        Helpfunctions.Instance.ErrorLog("combomaker cant read: " + line);
                    }
                }
                else
                {
                    try
                    {
                        combo c = new combo(line);
                        this.combos.Add(c);
                    }
                    catch
                    {
                        Helpfunctions.Instance.logg("combomaker cant read: " + line);
                        Helpfunctions.Instance.ErrorLog("combomaker cant read: " + line);
                    }
                }

            }

        }

        public int getPenalityForDestroyingCombo(CardDB.Card crd, Playfield p)
        {
            if (this.combos.Count == 0) return 0;
            int pen = int.MaxValue;
            bool found = false;
            int mana = Math.Max(hp.ownMaxMana, hp.currentMana);
            foreach (combo c in this.combos)
            {
                if (c.isCardInCombo(crd))
                {
                    int iia = c.isInCombo(hm.handCards, hp.ownMaxMana);//check if we have all cards for a combo, and if the choosen card is one
                    int iib = c.isMultiTurnComboTurn1(hm.handCards, mana, p.ownMinions, p.ownWeaponName);

                    int iic = Math.Max(iia, iib);
                    if (iia == 2 && iib != 2 && c.isMultiTurn1Card(crd))// it is a card of the combo, is a turn 1 card, but turn 1 is not possible -> we have to play turn 0 cards first
                    {
                        iic = 1;
                    }
                    if (iic == 1) found = true;
                    if (iic == 1 && pen > c.cardspen[crd.CardID]) pen = c.cardspen[crd.CardID];//iic==1 will destroy combo
                    if (iic == 2) pen = 0;//card is ok to play
                }

            }
            if (found) { return pen; }
            return 0;

        }

        public int checkIfComboWasPlayed(List<Action> alist, string weapon)
        {
            if (this.combos.Count == 0) return 0;
            //returns a penalty only if the combo could be played, but is not played completely
            List<Handmanager.Handcard> playedcards = new List<Handmanager.Handcard>();
            List<combo> searchingCombo = new List<combo>();
            // only check the cards, that are in a combo that can be played:
            int mana = Math.Max(hp.ownMaxMana, hp.currentMana);
            foreach (Action a in alist)
            {
                if (!a.cardplay) continue;
                CardDB.Card crd = a.handcard.card;
                //playedcards.Add(a.handcard);
                foreach (combo c in this.combos)
                {
                    if (c.isCardInCombo(crd))
                    {
                        int iia = c.isInCombo(hm.handCards, hp.ownMaxMana);
                        int iib = c.isMultiTurnComboTurn1(hm.handCards, mana, hp.ownMinions, weapon);
                        int iic = Math.Max(iia, iib);
                        if (iia == 2 && iib != 2 && c.isMultiTurn1Card(crd))
                        {
                            iic = 1;
                        }
                        if (iic == 2)
                        {
                            playedcards.Add(a.handcard); // add only the cards, which dont get a penalty
                        }
                    }

                }
            }

            if (playedcards.Count == 0) return 0;

            bool wholeComboPlayed = false;

            int bonus = 0;
            foreach (combo c in this.combos)
            {
                int iia = c.hasPlayedCombo(playedcards);
                int iib = c.hasPlayedTurn0Combo(playedcards);
                int iic = c.hasPlayedTurn1Combo(playedcards);
                int iie = iia + iib + iic;
                if (iie >= 1)
                {
                    wholeComboPlayed = true;
                    bonus -= iie;
                }
            }

            if (wholeComboPlayed) return bonus;
            return 250;

        }


    }

    public class Mulligan
    {
        public class CardIDEntity
        {
            public string id = "";
            public int entitiy = 0;
            public CardIDEntity(string id, int entt)
            {
                this.id = id;
                this.entitiy = entt;
            }
        }

        class mulliitem
        {
            public string cardid = "";
            public string enemyclass = "";
            public int howmuch = 2;
            public string[] requiresCard = null;
            public int manarule = -1;

            public mulliitem(string id, string enemy, int number, string[] req = null, int mrule = -1)
            {
                this.cardid = id;
                this.enemyclass = enemy;
                this.howmuch = number;
                this.requiresCard = req;
                this.manarule = mrule;
            }
        }

        List<mulliitem> holdlist = new List<mulliitem>();
        List<mulliitem> deletelist = new List<mulliitem>();
        public bool loserLoserLoser = false;

        private static Mulligan instance;

        public static Mulligan Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Mulligan();
                }
                return instance;
            }
        }

        private Mulligan()
        {
            readCombos();
        }

        private void readCombos()
        {
            string[] lines = new string[0] { };
            this.holdlist.Clear();
            this.deletelist.Clear();
            try
            {
                string path = Settings.Instance.path;
                lines = System.IO.File.ReadAllLines(path + "_mulligan.txt");
            }
            catch
            {
                Helpfunctions.Instance.logg("cant find _mulligan.txt");
                Helpfunctions.Instance.ErrorLog("cant find _mulligan.txt (if you dont created your own mulliganfile, ignore this message)");
                return;
            }
            Helpfunctions.Instance.logg("read _mulligan.txt...");
            Helpfunctions.Instance.ErrorLog("read _mulligan.txt...");
            foreach (string line in lines)
            {
                if (line.StartsWith("loser"))
                {
                    this.loserLoserLoser = true;
                    continue;
                }

                if (line.StartsWith("hold;"))
                {
                    try
                    {
                        string enemyclass = line.Split(';')[1];
                        string cardlist = line.Split(';')[2];
                        foreach (string crd in cardlist.Split(','))
                        {
                            if (crd.Contains(":"))
                            {
                                if ((crd.Split(':')).Length == 3)
                                {
                                    this.holdlist.Add(new mulliitem(crd.Split(':')[0], enemyclass, Convert.ToInt32(crd.Split(':')[1]), crd.Split(':')[2].Split('/')));
                                }
                                else
                                {
                                    this.holdlist.Add(new mulliitem(crd.Split(':')[0], enemyclass, Convert.ToInt32(crd.Split(':')[1])));
                                }

                            }
                            else
                            {
                                this.holdlist.Add(new mulliitem(crd, enemyclass, 2));
                            }
                        }

                        if (line.Split(';').Length == 4)
                        {
                            int manarule = Convert.ToInt32(line.Split(';')[3]);
                            this.holdlist.Add(new mulliitem("#MANARULE", enemyclass, 2, null, manarule));
                        }

                    }
                    catch
                    {
                        Helpfunctions.Instance.logg("mullimaker cant read: " + line);
                        Helpfunctions.Instance.ErrorLog("mullimaker cant read: " + line);
                    }
                }
                else
                {
                    if (line.StartsWith("discard;"))
                    {
                        try
                        {
                            string enemyclass = line.Split(';')[1];
                            string cardlist = line.Split(';')[2];
                            foreach (string crd in cardlist.Split(','))
                            {
                                if (crd == null || crd == "") continue;
                                this.deletelist.Add(new mulliitem(crd, enemyclass, 2));
                            }

                            if (line.Split(';').Length == 4)
                            {
                                int manarule = Convert.ToInt32(line.Split(';')[3]);
                                this.deletelist.Add(new mulliitem("#MANARULE", enemyclass, 2, null, manarule));
                            }

                        }
                        catch
                        {
                            Helpfunctions.Instance.logg("mullimaker cant read: " + line);
                            Helpfunctions.Instance.ErrorLog("mullimaker cant read: " + line);
                        }
                    }
                    else
                    {

                    }
                }

            }

        }

        public bool hasmulliganrules()
        {
            if (this.holdlist.Count == 0 && this.deletelist.Count == 0) return false;
            return true;
        }

        public List<int> whatShouldIMulligan(List<CardIDEntity> cards, string enemclass)
        {
            List<int> discarditems = new List<int>();

            foreach (mulliitem mi in this.deletelist)
            {
                foreach (CardIDEntity c in cards)
                {
                    if (mi.cardid == "#MANARULE" && (mi.enemyclass == "all" || mi.enemyclass == enemclass))
                    {
                        if (CardDB.Instance.getCardDataFromID(c.id).cost >= mi.manarule)
                        {
                            if (discarditems.Contains(c.entitiy)) continue;
                            discarditems.Add(c.entitiy);
                        }
                        continue;
                    }

                    if (c.id == mi.cardid && (mi.enemyclass == "all" || mi.enemyclass == enemclass))
                    {
                        if (discarditems.Contains(c.entitiy)) continue;
                        discarditems.Add(c.entitiy);
                    }
                }
            }

            if (holdlist.Count == 0) return discarditems;

            Dictionary<string, int> holddic = new Dictionary<string, int>();
            foreach (CardIDEntity c in cards)
            {
                bool delete = true;
                foreach (mulliitem mi in this.holdlist)
                {

                    if (mi.cardid == "#MANARULE" && (mi.enemyclass == "all" || mi.enemyclass == enemclass))
                    {
                        if (CardDB.Instance.getCardDataFromID(c.id).cost <= mi.manarule)
                        {
                            delete = false;
                        }
                        continue;
                    }

                    if (c.id == mi.cardid && (mi.enemyclass == "all" || mi.enemyclass == enemclass))
                    {

                        if (mi.requiresCard == null)
                        {

                            if (holddic.ContainsKey(c.id)) // we are holding one of the cards
                            {
                                if (mi.howmuch == 2)
                                {
                                    delete = false;
                                }
                            }
                            else
                            {
                                delete = false;
                            }
                        }
                        else
                        {
                            bool hasRequirements = false;
                            foreach (CardIDEntity reqs in cards)
                            {
                                foreach (string s in mi.requiresCard)
                                {
                                    if (s == reqs.id)
                                    {
                                        hasRequirements = true;
                                        break;
                                    }
                                }
                            }
                            if (hasRequirements)
                            {
                                if (holddic.ContainsKey(c.id)) // we are holding one of the cards
                                {
                                    if (mi.howmuch == 2)
                                    {
                                        delete = false;
                                    }
                                }
                                else
                                {
                                    delete = false;
                                }
                            }

                        }
                    }
                }

                if (delete)
                {
                    if (discarditems.Contains(c.entitiy)) continue;
                    discarditems.Add(c.entitiy);
                }
                else
                {
                    discarditems.RemoveAll(x => x == c.entitiy);

                    if (holddic.ContainsKey(c.id))
                    {
                        holddic[c.id]++;
                    }
                    else
                    {
                        holddic.Add(c.id, 1);
                    }
                }

            }

            return discarditems;

        }

        public void setAutoConcede(bool mode)
        {
            this.loserLoserLoser = mode;
        }

    }

    public class CardDB
    {
        // Data is stored in hearthstone-folder -> data->win cardxml0
        //(data-> cardxml0 seems outdated (blutelfkleriker has 3hp there >_>)
        public enum cardtype
        {
            NONE,
            MOB,
            SPELL,
            WEAPON,
            HEROPWR,
            ENCHANTMENT,

        }

        public enum specialMinions // and cards :D
        {
            None,
            raidleader,
            leokk,
            stormwindchampion,
            grimscaleoracle,
            murlocwarleader,
            southseacaptain,
            timberwolf,
            tundrarhino,
            direwolfalpha,
            flametonguetotem,
            swordofjustice,
            warsongcommander,
            oldmurkeye,
            acolyteofpain,
            gurubashiberserker,
            frothingberserker,
            armorsmith,
            lightwarden,
            northshirecleric,
            scavenginghyena,
            flesheatingghoul,
            cultmaster,
            illidanstormrage,
            questingadventurer,
            unboundelemental,
            waterelemental,
            knifejuggler,
            starvingbuzzard,
            murloctidecaller,
            manawyrm,
            manaaddict,
            secretkeeper,
            archmageantonidas,
            violetteacher,
            gadgetzanauctioneer,
            wildpyromancer,
            frog,
            prophetvelen,
            auchenaisoulpriest,
            pintsizedsummoner,
            sorcerersapprentice,
            manawraith,
            venturecomercenary,
            summoningportal,
            spitefulsmith,
            defenderofargus,
            sunfuryprotector,
            biggamehunter,
            shadowworddeath,
            lesserheal,
            flamestrike,
            shieldblock,
            soulfire,
            doomguard,
            succubus,
            silverhandrecruit,
            thecoin,
            innervate,
            ancientwatcher,
            webspinner,
            darkcultist,
            anubarambusher,
            voidcaller,
            dancingswords,
            undertaker,
            nerubianegg,
            shadeofnaxxramas,
            baronrivendare,
            faeriedragon,
            bloodmagethalnos
        }

        public enum ErrorType2
        {
            NONE,//=0
            REQ_MINION_TARGET,//=1
            REQ_FRIENDLY_TARGET,//=2
            REQ_ENEMY_TARGET,//=3
            REQ_DAMAGED_TARGET,//=4
            REQ_ENCHANTED_TARGET,
            REQ_FROZEN_TARGET,
            REQ_CHARGE_TARGET,
            REQ_TARGET_MAX_ATTACK,//=8
            REQ_NONSELF_TARGET,//=9
            REQ_TARGET_WITH_RACE,//=10
            REQ_TARGET_TO_PLAY,//=11 
            REQ_NUM_MINION_SLOTS,//=12 
            REQ_WEAPON_EQUIPPED,//=13
            REQ_ENOUGH_MANA,//=14
            REQ_YOUR_TURN,
            REQ_NONSTEALTH_ENEMY_TARGET,
            REQ_HERO_TARGET,//17
            REQ_SECRET_CAP,
            REQ_MINION_CAP_IF_TARGET_AVAILABLE,//19
            REQ_MINION_CAP,
            REQ_TARGET_ATTACKED_THIS_TURN,
            REQ_TARGET_IF_AVAILABLE,//=22
            REQ_MINIMUM_ENEMY_MINIONS,//=23 /like spalen :D
            REQ_TARGET_FOR_COMBO,//=24
            REQ_NOT_EXHAUSTED_ACTIVATE,
            REQ_UNIQUE_SECRET,
            REQ_TARGET_TAUNTER,
            REQ_CAN_BE_ATTACKED,
            REQ_ACTION_PWR_IS_MASTER_PWR,
            REQ_TARGET_MAGNET,
            REQ_ATTACK_GREATER_THAN_0,
            REQ_ATTACKER_NOT_FROZEN,
            REQ_HERO_OR_MINION_TARGET,
            REQ_CAN_BE_TARGETED_BY_SPELLS,
            REQ_SUBCARD_IS_PLAYABLE,
            REQ_TARGET_FOR_NO_COMBO,
            REQ_NOT_MINION_JUST_PLAYED,
            REQ_NOT_EXHAUSTED_HERO_POWER,
            REQ_CAN_BE_TARGETED_BY_OPPONENTS,
            REQ_ATTACKER_CAN_ATTACK,
            REQ_TARGET_MIN_ATTACK,//=41
            REQ_CAN_BE_TARGETED_BY_HERO_POWERS,
            REQ_ENEMY_TARGET_NOT_IMMUNE,
            REQ_ENTIRE_ENTOURAGE_NOT_IN_PLAY,//44 (totemic call)
            REQ_MINIMUM_TOTAL_MINIONS,//45 (scharmuetzel)
            REQ_MUST_TARGET_TAUNTER,//=46
            REQ_UNDAMAGED_TARGET//=47
        }

        public class Card
        {
            public string CardID = "";
            public string name = "";
            public int race = 0;
            public int rarity = 0;
            public int cost = 0;
            public int crdtype = 0;
            public cardtype type = CardDB.cardtype.NONE;
            public string description = "";
            public int carddraw = 0;

            public bool hasEffect = false;// has the minion an effect, but not battlecry

            public int Attack = 0;
            public int Health = 0;
            public int Durability = 0;//for weapons
            public bool target = false;
            public string targettext = "";
            public bool tank = false;
            public bool Silence = false;
            public bool choice = false;
            public bool windfury = false;
            public bool poisionous = false;
            public bool deathrattle = false;
            public bool battlecry = false;
            public bool oneTurnEffect = false;
            public bool Enrage = false;
            public bool Aura = false;
            public bool Elite = false;
            public bool Combo = false;
            public bool Recall = false;
            public int recallValue = 0;
            public bool immuneWhileAttacking = false;
            public bool immuneToSpellpowerg = false;
            public bool Stealth = false;
            public bool Freeze = false;
            public bool AdjacentBuff = false;
            public bool Shield = false;
            public bool Charge = false;
            public bool Secret = false;
            public bool Morph = false;
            public bool Spellpower = false;
            public bool GrantCharge = false;
            public bool HealTarget = false;
            //playRequirements, reqID= siehe PlayErrors->ErrorType
            public int needEmptyPlacesForPlaying = 0;
            public int needWithMinAttackValueOf = 0;
            public int needWithMaxAttackValueOf = 0;
            public int needRaceForPlaying = 0;
            public int needMinNumberOfEnemy = 0;
            public int needMinTotalMinions = 0;
            public int needMinionsCapIfAvailable = 0;

            public int spellpowervalue = 0;
            public specialMinions specialMin = specialMinions.None;
            public List<ErrorType2> playrequires;

            public Card()
            {
                playrequires = new List<ErrorType2>();
            }

            public Card(Card c)
            {
                //this.entityID = c.entityID;
                this.hasEffect = c.hasEffect;
                this.rarity = c.rarity;
                this.AdjacentBuff = c.AdjacentBuff;
                this.Attack = c.Attack;
                this.Aura = c.Aura;
                this.battlecry = c.battlecry;
                this.carddraw = c.carddraw;
                this.CardID = c.CardID;
                this.Charge = c.Charge;
                this.choice = c.choice;
                this.Combo = c.Combo;
                this.cost = c.cost;
                this.crdtype = c.crdtype;
                this.deathrattle = c.deathrattle;
                this.description = c.description;
                this.Durability = c.Durability;
                this.Elite = c.Elite;
                this.Enrage = c.Enrage;
                this.Freeze = c.Freeze;
                this.GrantCharge = c.GrantCharge;
                this.HealTarget = c.HealTarget;
                this.Health = c.Health;
                this.immuneToSpellpowerg = c.immuneToSpellpowerg;
                this.immuneWhileAttacking = c.immuneWhileAttacking;
                this.Morph = c.Morph;
                this.name = c.name;
                this.needEmptyPlacesForPlaying = c.needEmptyPlacesForPlaying;
                this.needMinionsCapIfAvailable = c.needMinionsCapIfAvailable;
                this.needMinNumberOfEnemy = c.needMinNumberOfEnemy;
                this.needMinTotalMinions = c.needMinTotalMinions;
                this.needRaceForPlaying = c.needRaceForPlaying;
                this.needWithMaxAttackValueOf = c.needWithMaxAttackValueOf;
                this.needWithMinAttackValueOf = c.needWithMinAttackValueOf;
                this.oneTurnEffect = c.oneTurnEffect;
                this.playrequires = new List<ErrorType2>(c.playrequires);
                this.poisionous = c.poisionous;
                this.race = c.race;
                this.Recall = c.Recall;
                this.recallValue = c.recallValue;
                this.Secret = c.Secret;
                this.Shield = c.Shield;
                this.Silence = c.Silence;
                this.Spellpower = c.Spellpower;
                this.spellpowervalue = c.spellpowervalue;
                this.Stealth = c.Stealth;
                this.tank = c.tank;
                this.target = c.target;
                this.targettext = c.targettext;
                this.type = c.type;
                this.windfury = c.windfury;
                this.specialMin = c.specialMin;
            }

            public bool isRequirementInList(CardDB.ErrorType2 et)
            {
                if (this.playrequires.Contains(et)) return true;
                return false;
            }

            public List<targett> getTargetsForCard(Playfield p)
            {
                List<targett> retval = new List<targett>();

                if (isRequirementInList(CardDB.ErrorType2.REQ_TARGET_FOR_COMBO) && p.cardsPlayedThisTurn == 0) return retval;

                if (isRequirementInList(CardDB.ErrorType2.REQ_TARGET_TO_PLAY) || isRequirementInList(CardDB.ErrorType2.REQ_NONSELF_TARGET) || isRequirementInList(CardDB.ErrorType2.REQ_TARGET_IF_AVAILABLE) || isRequirementInList(CardDB.ErrorType2.REQ_TARGET_FOR_COMBO))
                {
                    retval.Add(new targett(100, p.ownHeroEntity));//ownhero
                    retval.Add(new targett(200, p.enemyHeroEntity));//enemyhero
                    foreach (Minion m in p.ownMinions)
                    {
                        if ((this.type == cardtype.SPELL || this.type == cardtype.HEROPWR) && (m.name == "faeriedragon" || m.name == "laughingsister")) continue;
                        retval.Add(new targett(m.id, m.entitiyID));
                    }
                    foreach (Minion m in p.enemyMinions)
                    {
                        if (((this.type == cardtype.SPELL || this.type == cardtype.HEROPWR) && (m.name == "faeriedragon" || m.name == "laughingsister")) || m.stealth) continue;
                        retval.Add(new targett(m.id + 10, m.entitiyID));
                    }

                }

                if (isRequirementInList(CardDB.ErrorType2.REQ_HERO_TARGET))
                {
                    retval.RemoveAll(x => (x.target <= 30));
                }

                if (isRequirementInList(CardDB.ErrorType2.REQ_MINION_TARGET))
                {
                    retval.RemoveAll(x => (x.target == 100) || (x.target == 200));
                }

                if (isRequirementInList(CardDB.ErrorType2.REQ_FRIENDLY_TARGET))
                {
                    retval.RemoveAll(x => (x.target >= 10 && x.target <= 20) || (x.target == 200));
                }

                if (isRequirementInList(CardDB.ErrorType2.REQ_ENEMY_TARGET))
                {
                    retval.RemoveAll(x => (x.target <= 9 || (x.target == 100)));
                }

                if (isRequirementInList(CardDB.ErrorType2.REQ_DAMAGED_TARGET))
                {
                    foreach (Minion m in p.ownMinions)
                    {
                        if (!m.wounded)
                        {
                            retval.RemoveAll(x => x.targetEntity == m.entitiyID);
                        }
                    }
                    foreach (Minion m in p.enemyMinions)
                    {
                        if (!m.wounded)
                        {
                            retval.RemoveAll(x => x.targetEntity == m.entitiyID);
                        }
                    }
                }

                if (isRequirementInList(CardDB.ErrorType2.REQ_UNDAMAGED_TARGET))
                {
                    foreach (Minion m in p.ownMinions)
                    {
                        if (m.wounded)
                        {
                            retval.RemoveAll(x => x.targetEntity == m.entitiyID);
                        }
                    }
                    foreach (Minion m in p.enemyMinions)
                    {
                        if (m.wounded)
                        {
                            retval.RemoveAll(x => x.targetEntity == m.entitiyID);
                        }
                    }
                }

                if (isRequirementInList(CardDB.ErrorType2.REQ_TARGET_MAX_ATTACK))
                {
                    foreach (Minion m in p.ownMinions)
                    {
                        if (m.Angr > this.needWithMaxAttackValueOf)
                        {
                            retval.RemoveAll(x => x.targetEntity == m.entitiyID);
                        }
                    }
                    foreach (Minion m in p.enemyMinions)
                    {
                        if (m.Angr > this.needWithMaxAttackValueOf)
                        {
                            retval.RemoveAll(x => x.targetEntity == m.entitiyID);
                        }
                    }
                }

                if (isRequirementInList(CardDB.ErrorType2.REQ_TARGET_MIN_ATTACK))
                {
                    foreach (Minion m in p.ownMinions)
                    {
                        if (m.Angr < this.needWithMinAttackValueOf)
                        {
                            retval.RemoveAll(x => x.targetEntity == m.entitiyID);
                        }
                    }
                    foreach (Minion m in p.enemyMinions)
                    {
                        if (m.Angr < this.needWithMinAttackValueOf)
                        {
                            retval.RemoveAll(x => x.targetEntity == m.entitiyID);
                        }
                    }
                }

                if (isRequirementInList(CardDB.ErrorType2.REQ_TARGET_WITH_RACE))
                {
                    foreach (Minion m in p.ownMinions)
                    {
                        if (!(m.handcard.card.race == this.needRaceForPlaying))
                        {
                            retval.RemoveAll(x => x.targetEntity == m.entitiyID);
                        }
                    }
                    foreach (Minion m in p.enemyMinions)
                    {
                        if (!(m.handcard.card.race == this.needRaceForPlaying))
                        {
                            retval.RemoveAll(x => x.targetEntity == m.entitiyID);
                        }
                    }
                }

                if (isRequirementInList(CardDB.ErrorType2.REQ_MUST_TARGET_TAUNTER))
                {
                    foreach (Minion m in p.ownMinions)
                    {
                        if (!m.taunt)
                        {
                            retval.RemoveAll(x => x.targetEntity == m.entitiyID);
                        }
                    }
                    foreach (Minion m in p.enemyMinions)
                    {
                        if (!m.taunt)
                        {
                            retval.RemoveAll(x => x.targetEntity == m.entitiyID);
                        }
                    }
                }
                return retval;

            }

            public int calculateManaCost(Playfield p)//calculates the mana from orginal mana, needed for back-to hand effects
            {
                int retval = this.cost;
                int offset = 0;

                if (this.type == cardtype.MOB)
                {
                    offset += (p.soeldnerDerVenture) * 3;

                    offset += (p.managespenst);

                    int temp = -(p.startedWithbeschwoerungsportal) * 2;
                    if (retval + temp <= 0) temp = -retval + 1;
                    offset = offset + temp;

                    if (p.mobsplayedThisTurn == 0)
                    {
                        offset -= p.winzigebeschwoererin;
                    }

                }

                if (this.type == cardtype.SPELL)
                { //if the number of zauberlehrlings change
                    offset -= (p.zauberlehrling);
                    if (p.playedPreparation)
                    { //if the number of zauberlehrlings change
                        offset -= 3;
                    }

                }

                switch (this.name)
                {
                    case "dreadcorsair":
                        retval = retval + offset - p.ownWeaponAttack;
                        break;
                    case "seagiant":
                        retval = retval + offset - p.ownMinions.Count - p.enemyMinions.Count;
                        break;
                    case "mountaingiant":
                        retval = retval + offset - p.owncards.Count;
                        break;
                    case "moltengiant":
                        retval = retval + offset - p.ownHeroHp;
                        break;
                    default:
                        retval = retval + offset;
                        break;
                }

                if (this.Secret && p.playedmagierinderkirintor)
                {
                    retval = 0;
                }

                retval = Math.Max(0, retval);

                return retval;
            }

            public int getManaCost(Playfield p, int currentcost)//calculates mana from current mana
            {
                int retval = currentcost;


                int offset = 0; // if offset < 0 costs become lower, if >0 costs are higher at the end

                // CARDS that increase the manacosts of others ##############################
                //Manacosts changes with soeldner der venture co.
                if (p.soeldnerDerVenture != p.startedWithsoeldnerDerVenture && this.type == cardtype.MOB)
                {
                    offset += (p.soeldnerDerVenture - p.startedWithsoeldnerDerVenture) * 3;
                }

                //Manacosts changes with mana-ghost
                if (p.managespenst != p.startedWithManagespenst && this.type == cardtype.MOB)
                {
                    offset += (p.managespenst - p.startedWithManagespenst);
                }


                // CARDS that decrease the manacosts of others ##############################

                //Manacosts changes with the summoning-portal >_>
                if (p.startedWithbeschwoerungsportal != p.beschwoerungsportal && this.type == cardtype.MOB)
                { //cant lower the mana to 0
                    int temp = (p.startedWithbeschwoerungsportal - p.beschwoerungsportal) * 2;
                    if (retval + temp <= 0) temp = -retval + 1;
                    offset = offset + temp;
                }

                //Manacosts changes with the pint-sized summoner
                if (p.winzigebeschwoererin >= 1 && p.mobsplayedThisTurn >= 1 && p.startedWithMobsPlayedThisTurn == 0 && this.type == cardtype.MOB)
                { // if we start oure calculations with 0 mobs played, then the cardcost are 1 mana to low in the further calculations (with the little summoner on field)
                    offset += p.winzigebeschwoererin;
                }
                if (p.mobsplayedThisTurn == 0 && p.winzigebeschwoererin <= p.startedWithWinzigebeschwoererin && this.type == cardtype.MOB)
                { // one pint-sized summoner got killed, before we played the first mob -> the manacost are higher of all mobs
                    offset += (p.startedWithWinzigebeschwoererin - p.winzigebeschwoererin);
                }

                //Manacosts changes with the zauberlehrling summoner
                if (p.zauberlehrling != p.startedWithZauberlehrling && this.type == cardtype.SPELL)
                { //if the number of zauberlehrlings change
                    offset += (p.startedWithZauberlehrling - p.zauberlehrling);
                }



                //manacosts are lowered, after we played preparation
                if (p.playedPreparation && this.type == cardtype.SPELL)
                { //if the number of zauberlehrlings change
                    offset -= 3;
                }





                switch (this.name)
                {
                    case "dreadcorsair":
                        retval = retval + offset - p.ownWeaponAttack + p.ownWeaponAttackStarted; // if weapon attack change we change manacost
                        break;
                    case "seagiant":
                        retval = retval + offset - p.ownMinions.Count - p.enemyMinions.Count + p.ownMobsCountStarted;
                        break;
                    case "mountaingiant":
                        retval = retval + offset - p.owncards.Count + p.ownCardsCountStarted;
                        break;
                    case "moltengiant":
                        retval = retval + offset - p.ownHeroHp + p.ownHeroHpStarted;
                        break;
                    default:
                        retval = retval + offset;
                        break;
                }

                if (this.Secret && p.playedmagierinderkirintor)
                {
                    retval = 0;
                }

                retval = Math.Max(0, retval);

                return retval;
            }

            public bool canplayCard(Playfield p, int manacost)
            {
                //is playrequirement?
                bool haveToDoRequires = isRequirementInList(CardDB.ErrorType2.REQ_TARGET_TO_PLAY);
                bool retval = true;
                // cant play if i have to few mana

                if (p.mana < this.getManaCost(p, manacost)) return false;

                // cant play mob, if i have allready 7 mininos
                if (this.type == CardDB.cardtype.MOB && p.ownMinions.Count >= 7) return false;

                if (isRequirementInList(CardDB.ErrorType2.REQ_MINIMUM_ENEMY_MINIONS))
                {
                    if (p.enemyMinions.Count < this.needMinNumberOfEnemy) return false;
                }
                if (isRequirementInList(CardDB.ErrorType2.REQ_NUM_MINION_SLOTS))
                {
                    if (p.ownMinions.Count > 7 - this.needEmptyPlacesForPlaying) return false;
                }

                if (isRequirementInList(CardDB.ErrorType2.REQ_WEAPON_EQUIPPED))
                {
                    if (p.ownWeaponName == "") return false;
                }

                if (isRequirementInList(CardDB.ErrorType2.REQ_MINIMUM_TOTAL_MINIONS))
                {
                    if (this.needMinTotalMinions > p.ownMinions.Count + p.enemyMinions.Count) return false;
                }

                if (haveToDoRequires)
                {
                    if (this.getTargetsForCard(p).Count == 0) return false;

                    //it requires a target-> return false if 
                }

                if (isRequirementInList(CardDB.ErrorType2.REQ_TARGET_IF_AVAILABLE) && isRequirementInList(CardDB.ErrorType2.REQ_MINION_CAP_IF_TARGET_AVAILABLE))
                {
                    if (this.getTargetsForCard(p).Count >= 1 && p.ownMinions.Count > 7 - this.needMinionsCapIfAvailable) return false;
                }

                if (isRequirementInList(CardDB.ErrorType2.REQ_ENTIRE_ENTOURAGE_NOT_IN_PLAY))
                {
                    int difftotem = 0;
                    foreach (Minion m in p.ownMinions)
                    {
                        if (m.name == "healingtotem" || m.name == "wrathofairtotem" || m.name == "searingtotem" || m.name == "stoneclawtotem") difftotem++;
                    }
                    if (difftotem == 4) return false;
                }


                if (this.Secret)
                {
                    if (p.ownSecretsIDList.Contains(this.CardID)) return false;
                    if (p.ownSecretsIDList.Count >= 5) return false;
                }


                return true;
            }



        }

        List<Card> cardlist = new List<Card>();
        Dictionary<string, Card> cardidToCardList = new Dictionary<string, Card>();

        private static CardDB instance;

        public static CardDB Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new CardDB();
                }
                return instance;
            }
        }

        private CardDB()
        {
            string[] lines = new string[0] { };
            try
            {
                string path = Settings.Instance.path;
                lines = System.IO.File.ReadAllLines(path + "_carddb.txt");
            }
            catch
            {
                Helpfunctions.Instance.logg("cant find _carddb.txt");
                Helpfunctions.Instance.ErrorLog("ERROR#################################################");
                Helpfunctions.Instance.ErrorLog("cant find _carddb.txt");
            }
            cardlist.Clear();
            this.cardidToCardList.Clear();
            Card c = new Card();
            int de = 0;
            bool targettext = false;
            //placeholdercard
            Card plchldr = new Card();
            plchldr.name = "unknown";
            plchldr.cost = 1000;
            this.cardlist.Add(plchldr);

            foreach (string s in lines)
            {
                if (s.Contains("/Entity"))
                {
                    if (c.type == cardtype.ENCHANTMENT)
                    {
                        //Helpfunctions.Instance.logg(c.CardID);
                        //Helpfunctions.Instance.logg(c.name);
                        //Helpfunctions.Instance.logg(c.description);
                        continue;
                    }

                    if (c.name != "")
                    {
                        //Helpfunctions.Instance.logg(c.name);
                        this.cardlist.Add(c);
                        if (!this.cardidToCardList.ContainsKey(c.CardID))
                        {
                            this.cardidToCardList.Add(c.CardID, c);
                        }
                    }

                }
                if (s.Contains("<Entity version=\"2\" CardID=\""))
                {
                    c = new Card();
                    de = 0;
                    targettext = false;
                    string temp = s.Replace("<Entity version=\"2\" CardID=\"", "");
                    temp = temp.Replace("\">", "");
                    c.CardID = temp;
                    continue;
                }
                if (s.Contains("<Entity version=\"1\" CardID=\""))
                {
                    c = new Card();
                    de = 0;
                    targettext = false;
                    string temp = s.Replace("<Entity version=\"1\" CardID=\"", "");
                    temp = temp.Replace("\">", "");
                    c.CardID = temp;
                    continue;
                }

                if (s.Contains("<Tag name=\"Health\""))
                {
                    string temp = s.Split(new string[] { "value=\"" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    temp = temp.Split('\"')[0];
                    c.Health = Convert.ToInt32(temp);
                    continue;
                }
                if (s.Contains("<Tag name=\"Atk\""))
                {
                    string temp = s.Split(new string[] { "value=\"" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    temp = temp.Split('\"')[0];
                    c.Attack = Convert.ToInt32(temp);
                    continue;
                }
                if (s.Contains("<Tag name=\"Race\""))
                {
                    string temp = s.Split(new string[] { "value=\"" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    temp = temp.Split('\"')[0];
                    c.race = Convert.ToInt32(temp);
                    continue;
                }
                if (s.Contains("<Tag name=\"Rarity\""))
                {
                    string temp = s.Split(new string[] { "value=\"" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    temp = temp.Split('\"')[0];
                    c.rarity = Convert.ToInt32(temp);
                    continue;
                }
                if (s.Contains("<Tag name=\"Cost\""))
                {
                    string temp = s.Split(new string[] { "value=\"" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    temp = temp.Split('\"')[0];
                    c.cost = Convert.ToInt32(temp);
                    continue;
                }

                if (s.Contains("<Tag name=\"CardType\""))
                {
                    string temp = s.Split(new string[] { "value=\"" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    temp = temp.Split('\"')[0];
                    if (c.name != "")
                    {
                        //Helpfunctions.Instance.logg(temp);
                    }

                    c.crdtype = Convert.ToInt32(temp);
                    if (c.crdtype == 10)
                    {
                        c.type = CardDB.cardtype.HEROPWR;
                    }
                    if (c.crdtype == 4)
                    {
                        c.type = CardDB.cardtype.MOB;
                    }
                    if (c.crdtype == 5)
                    {
                        c.type = CardDB.cardtype.SPELL;
                    }
                    if (c.crdtype == 6)
                    {
                        c.type = CardDB.cardtype.ENCHANTMENT;
                    }
                    if (c.crdtype == 7)
                    {
                        c.type = CardDB.cardtype.WEAPON;
                    }
                    continue;
                }

                if (s.Contains("<enUS>"))
                {
                    string temp = s.Replace("<enUS>", "");

                    temp = temp.Replace("</enUS>", "");
                    temp = temp.Replace("&lt;", "");
                    temp = temp.Replace("b&gt;", "");
                    temp = temp.Replace("/b&gt;", "");
                    temp = temp.ToLower();
                    if (de == 0)
                    {
                        temp = temp.Replace("'", "");
                        temp = temp.Replace(" ", "");
                        temp = temp.Replace(":", "");
                        temp = temp.Replace(".", "");
                        temp = temp.Replace("!", "");
                        //temp = temp.Replace("ß", "ss");
                        //temp = temp.Replace("ü", "ue");
                        //temp = temp.Replace("ä", "ae");
                        //temp = temp.Replace("ö", "oe");

                        //Helpfunctions.Instance.logg(temp);
                        c.name = temp;
                        c.specialMin = specialMinions.None;
                        if (temp == "direwolfalpha") c.specialMin = specialMinions.direwolfalpha;
                        if (temp == "flametonguetotem") c.specialMin = specialMinions.flametonguetotem;
                        if (temp == "grimscaleoracle") c.specialMin = specialMinions.grimscaleoracle;
                        if (temp == "leokk") c.specialMin = specialMinions.leokk;
                        if (temp == "murlocwarleader") c.specialMin = specialMinions.murlocwarleader;
                        if (temp == "raidleader") c.specialMin = specialMinions.raidleader;
                        if (temp == "southseacaptain") c.specialMin = specialMinions.southseacaptain;
                        if (temp == "stormwindchampion") c.specialMin = specialMinions.stormwindchampion;
                        if (temp == "timberwolf") c.specialMin = specialMinions.timberwolf;
                        if (temp == "tundrarhino") c.specialMin = specialMinions.tundrarhino;
                        if (temp == "swordofjustice") c.specialMin = specialMinions.swordofjustice;
                        if (temp == "warsongcommander") c.specialMin = specialMinions.warsongcommander;
                        if (temp == "oldmurk-eye") c.specialMin = specialMinions.oldmurkeye;
                        if (temp == "acolyteofpain") c.specialMin = specialMinions.acolyteofpain;
                        if (temp == "gurubashiberserker") c.specialMin = specialMinions.gurubashiberserker;
                        if (temp == "frothingberserker") c.specialMin = specialMinions.frothingberserker;
                        if (temp == "armorsmith") c.specialMin = specialMinions.armorsmith;
                        if (temp == "northshirecleric") c.specialMin = specialMinions.northshirecleric;
                        if (temp == "lightwarden") c.specialMin = specialMinions.lightwarden;
                        if (temp == "scavenginghyena") c.specialMin = specialMinions.scavenginghyena;
                        if (temp == "flesheatingghoul") c.specialMin = specialMinions.flesheatingghoul;
                        if (temp == "cultmaster") c.specialMin = specialMinions.cultmaster;
                        if (temp == "illidanstormrage") c.specialMin = specialMinions.illidanstormrage;
                        if (temp == "questingadventurer") c.specialMin = specialMinions.questingadventurer;
                        if (temp == "unboundelemental") c.specialMin = specialMinions.unboundelemental;
                        if (temp == "waterelemental") c.specialMin = specialMinions.waterelemental;
                        if (temp == "knifejuggler") c.specialMin = specialMinions.knifejuggler;
                        if (temp == "starvingbuzzard") c.specialMin = specialMinions.starvingbuzzard;
                        if (temp == "murloctidecaller") c.specialMin = specialMinions.murloctidecaller;
                        if (temp == "manawyrm") c.specialMin = specialMinions.manawyrm;
                        if (temp == "manaaddict") c.specialMin = specialMinions.manaaddict;
                        if (temp == "secretkeeper") c.specialMin = specialMinions.secretkeeper;
                        if (temp == "archmageantonidas") c.specialMin = specialMinions.archmageantonidas;
                        if (temp == "violetteacher") c.specialMin = specialMinions.violetteacher;
                        if (temp == "gadgetzanauctioneer") c.specialMin = specialMinions.gadgetzanauctioneer;
                        if (temp == "wildpyromancer") c.specialMin = specialMinions.wildpyromancer;
                        if (temp == "frog") c.specialMin = specialMinions.frog;
                        if (temp == "prophetvelen") c.specialMin = specialMinions.prophetvelen;
                        if (temp == "auchenaisoulpriest") c.specialMin = specialMinions.auchenaisoulpriest;
                        if (temp == "pint-sizedsummoner") c.specialMin = specialMinions.pintsizedsummoner;
                        if (temp == "sorcerersapprentice") c.specialMin = specialMinions.sorcerersapprentice;
                        if (temp == "manawraith") c.specialMin = specialMinions.manawraith;
                        if (temp == "venturecomercenary") c.specialMin = specialMinions.venturecomercenary;
                        if (temp == "summoningportal") c.specialMin = specialMinions.summoningportal;
                        if (temp == "spitefulsmith") c.specialMin = specialMinions.spitefulsmith;
                        if (temp == "defenderofargus") c.specialMin = specialMinions.defenderofargus;
                        if (temp == "sunfuryprotector") c.specialMin = specialMinions.sunfuryprotector;
                        if (temp == "biggamehunter") c.specialMin = specialMinions.biggamehunter;
                        if (temp == "shadowworddeath") c.specialMin = specialMinions.shadowworddeath;
                        if (temp == "lesserheal") c.specialMin = specialMinions.lesserheal;
                        if (temp == "flamestrike") c.specialMin = specialMinions.flamestrike;
                        if (temp == "shieldblock") c.specialMin = specialMinions.shieldblock;
                        if (temp == "soulfire") c.specialMin = specialMinions.soulfire;
                        if (temp == "doomguard") c.specialMin = specialMinions.doomguard;
                        if (temp == "succubus") c.specialMin = specialMinions.succubus;
                        if (temp == "silverhandrecruit") c.specialMin = specialMinions.silverhandrecruit;
                        if (temp == "thecoin") c.specialMin = specialMinions.thecoin;
                        if (temp == "innervate") c.specialMin = specialMinions.innervate;
                        if (temp == "ancientwatcher") c.specialMin = specialMinions.ancientwatcher;
                        if (temp == "faeriedragon") c.specialMin = specialMinions.faeriedragon;
                        if (temp == "bloodmagethalnos") c.specialMin = specialMinions.bloodmagethalnos;
                        //naxx update
                        if (temp == "webspinner") c.specialMin = specialMinions.webspinner;
                        if (temp == "darkcultist") c.specialMin = specialMinions.darkcultist;
                        if (temp == "anubarambusher") c.specialMin = specialMinions.anubarambusher;
                        if (temp == "voidcaller") c.specialMin = specialMinions.voidcaller;
                        if (temp == "dancingswords") c.specialMin = specialMinions.dancingswords;
                        if (temp == "undertaker") c.specialMin = specialMinions.undertaker;
                        if (temp == "nerubianegg") c.specialMin = specialMinions.nerubianegg;
                        if (temp == "shadeofnaxxramas") c.specialMin = specialMinions.shadeofnaxxramas;
                        if (temp == "baronrivendare") c.specialMin = specialMinions.baronrivendare;


                        if (PenalityManager.Instance.specialMinions.ContainsKey(temp)) c.hasEffect = true;

                    }
                    if (de == 1)
                    {
                        c.description = temp;
                        if (c.description.Contains("choose one"))
                        {
                            c.choice = true;
                            //Helpfunctions.Instance.logg(c.name + " is choice");
                        }
                    }
                    if (targettext)
                    {
                        c.targettext = temp;
                        targettext = false;
                    }

                    de++;
                    continue;
                }
                if (s.Contains("<Tag name=\"Poisonous\""))
                {
                    string temp = s.Split(new string[] { "value=\"" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    temp = temp.Split('\"')[0];
                    int ti = Convert.ToInt32(temp);
                    if (ti == 1) c.poisionous = true;
                    continue;
                }
                if (s.Contains("<Tag name=\"Enrage\""))
                {
                    string temp = s.Split(new string[] { "value=\"" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    temp = temp.Split('\"')[0];
                    int ti = Convert.ToInt32(temp);
                    if (ti == 1) c.Enrage = true;
                    continue;
                }

                if (s.Contains("<Tag name=\"OneTurnEffect\""))
                {
                    string temp = s.Split(new string[] { "value=\"" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    temp = temp.Split('\"')[0];
                    int ti = Convert.ToInt32(temp);
                    if (ti == 1) c.oneTurnEffect = true;
                    continue;
                }
                if (s.Contains("<Tag name=\"Aura\""))
                {
                    string temp = s.Split(new string[] { "value=\"" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    temp = temp.Split('\"')[0];
                    int ti = Convert.ToInt32(temp);
                    if (ti == 1) c.Aura = true;
                    continue;
                }


                if (s.Contains("<Tag name=\"Taunt\""))
                {
                    string temp = s.Split(new string[] { "value=\"" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    temp = temp.Split('\"')[0];
                    int ti = Convert.ToInt32(temp);
                    if (ti == 1) c.tank = true;
                    continue;
                }
                if (s.Contains("<Tag name=\"Battlecry\""))
                {
                    string temp = s.Split(new string[] { "value=\"" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    temp = temp.Split('\"')[0];
                    int ti = Convert.ToInt32(temp);
                    if (ti == 1) c.battlecry = true;
                    continue;
                }
                if (s.Contains("<Tag name=\"Windfury\""))
                {
                    string temp = s.Split(new string[] { "value=\"" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    temp = temp.Split('\"')[0];
                    int ti = Convert.ToInt32(temp);
                    if (ti == 1) c.windfury = true;
                    continue;
                }

                if (s.Contains("<Tag name=\"Deathrattle\""))
                {
                    string temp = s.Split(new string[] { "value=\"" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    temp = temp.Split('\"')[0];
                    int ti = Convert.ToInt32(temp);
                    if (ti == 1) c.deathrattle = true;
                    continue;
                }
                if (s.Contains("<Tag name=\"Durability\""))
                {
                    string temp = s.Split(new string[] { "value=\"" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    temp = temp.Split('\"')[0];
                    c.Durability = Convert.ToInt32(temp);
                    continue;
                }
                if (s.Contains("<Tag name=\"Elite\""))
                {
                    string temp = s.Split(new string[] { "value=\"" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    temp = temp.Split('\"')[0];
                    int ti = Convert.ToInt32(temp);
                    if (ti == 1) c.Elite = true;
                    continue;
                }
                if (s.Contains("<Tag name=\"Combo\""))
                {
                    string temp = s.Split(new string[] { "value=\"" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    temp = temp.Split('\"')[0];
                    int ti = Convert.ToInt32(temp);
                    if (ti == 1) c.Combo = true;
                    continue;
                }
                if (s.Contains("<Tag name=\"Recall\""))
                {
                    string temp = s.Split(new string[] { "value=\"" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    temp = temp.Split('\"')[0];
                    int ti = Convert.ToInt32(temp);
                    if (ti == 1) c.Recall = true;
                    c.recallValue = 1;
                    if (c.name == "forkedlightning") c.recallValue = 2;
                    if (c.name == "dustdevil") c.recallValue = 2;
                    if (c.name == "lightningstorm") c.recallValue = 2;
                    if (c.name == "lavaburst") c.recallValue = 2;
                    if (c.name == "feralspirit") c.recallValue = 2;
                    if (c.name == "doomhammer") c.recallValue = 2;
                    if (c.name == "earthelemental") c.recallValue = 3;
                    continue;
                }

                if (s.Contains("<Tag name=\"ImmuneToSpellpower\""))
                {
                    string temp = s.Split(new string[] { "value=\"" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    temp = temp.Split('\"')[0];
                    int ti = Convert.ToInt32(temp);
                    if (ti == 1) c.immuneToSpellpowerg = true;
                    continue;
                }
                if (s.Contains("<Tag name=\"Stealth\""))
                {
                    string temp = s.Split(new string[] { "value=\"" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    temp = temp.Split('\"')[0];
                    int ti = Convert.ToInt32(temp);
                    if (ti == 1) c.Stealth = true;
                    continue;
                }
                if (s.Contains("<Tag name=\"Secret\""))
                {
                    string temp = s.Split(new string[] { "value=\"" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    temp = temp.Split('\"')[0];
                    int ti = Convert.ToInt32(temp);
                    if (ti == 1) c.Secret = true;
                    continue;
                }
                if (s.Contains("<Tag name=\"Freeze\""))
                {
                    string temp = s.Split(new string[] { "value=\"" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    temp = temp.Split('\"')[0];
                    int ti = Convert.ToInt32(temp);
                    if (ti == 1) c.Freeze = true;
                    continue;
                }
                if (s.Contains("<Tag name=\"AdjacentBuff\""))
                {
                    string temp = s.Split(new string[] { "value=\"" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    temp = temp.Split('\"')[0];
                    int ti = Convert.ToInt32(temp);
                    if (ti == 1) c.AdjacentBuff = true;
                    continue;
                }
                if (s.Contains("<Tag name=\"Divine Shield\""))
                {
                    string temp = s.Split(new string[] { "value=\"" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    temp = temp.Split('\"')[0];
                    int ti = Convert.ToInt32(temp);
                    if (ti == 1) c.Shield = true;
                    continue;
                }
                if (s.Contains("<Tag name=\"Charge\""))
                {
                    string temp = s.Split(new string[] { "value=\"" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    temp = temp.Split('\"')[0];
                    int ti = Convert.ToInt32(temp);
                    if (ti == 1) c.Charge = true;
                    continue;
                }
                if (s.Contains("<Tag name=\"Silence\""))
                {
                    string temp = s.Split(new string[] { "value=\"" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    temp = temp.Split('\"')[0];
                    int ti = Convert.ToInt32(temp);
                    if (ti == 1) c.Silence = true;
                    continue;
                }
                if (s.Contains("<Tag name=\"Morph\""))
                {
                    string temp = s.Split(new string[] { "value=\"" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    temp = temp.Split('\"')[0];
                    int ti = Convert.ToInt32(temp);
                    if (ti == 1) c.Morph = true;
                    continue;
                }
                if (s.Contains("<Tag name=\"Spellpower\""))
                {
                    string temp = s.Split(new string[] { "value=\"" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    temp = temp.Split('\"')[0];
                    int ti = Convert.ToInt32(temp);
                    if (ti == 1) c.Spellpower = true;
                    c.spellpowervalue = 1;
                    if (c.name == "ancientmage") c.spellpowervalue = 0;
                    if (c.name == "malygos") c.spellpowervalue = 5;
                    continue;
                }
                if (s.Contains("<Tag name=\"GrantCharge\""))
                {
                    string temp = s.Split(new string[] { "value=\"" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    temp = temp.Split('\"')[0];
                    int ti = Convert.ToInt32(temp);
                    if (ti == 1) c.GrantCharge = true;
                    continue;
                }
                if (s.Contains("<Tag name=\"HealTarget\""))
                {
                    string temp = s.Split(new string[] { "value=\"" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    temp = temp.Split('\"')[0];
                    int ti = Convert.ToInt32(temp);
                    if (ti == 1) c.HealTarget = true;
                    continue;
                }

                if (s.Contains("TargetingArrowText"))
                {
                    c.target = true;
                    targettext = true;
                    continue;
                }

                if (s.Contains("<PlayRequirement"))
                {
                    string temp = s.Split(new string[] { "reqID=\"" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    temp = temp.Split('\"')[0];
                    ErrorType2 et2 = (ErrorType2)Convert.ToInt32(temp);
                    c.playrequires.Add(et2);
                }


                if (s.Contains("<PlayRequirement reqID=\"12\" param=\""))
                {
                    string temp = s.Split(new string[] { "param=\"" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    temp = temp.Split('\"')[0];
                    c.needEmptyPlacesForPlaying = Convert.ToInt32(temp);
                    continue;
                }
                if (s.Contains("PlayRequirement reqID=\"41\" param=\""))
                {
                    string temp = s.Split(new string[] { "param=\"" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    temp = temp.Split('\"')[0];
                    c.needWithMinAttackValueOf = Convert.ToInt32(temp);
                    continue;
                }
                if (s.Contains("PlayRequirement reqID=\"8\" param=\""))
                {
                    string temp = s.Split(new string[] { "param=\"" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    temp = temp.Split('\"')[0];
                    c.needWithMaxAttackValueOf = Convert.ToInt32(temp);
                    continue;
                }
                if (s.Contains("PlayRequirement reqID=\"10\" param=\""))
                {
                    string temp = s.Split(new string[] { "param=\"" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    temp = temp.Split('\"')[0];
                    c.needRaceForPlaying = Convert.ToInt32(temp);
                    continue;
                }
                if (s.Contains("PlayRequirement reqID=\"23\" param=\""))
                {
                    string temp = s.Split(new string[] { "param=\"" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    temp = temp.Split('\"')[0];
                    c.needMinNumberOfEnemy = Convert.ToInt32(temp);
                    continue;
                }
                if (s.Contains("PlayRequirement reqID=\"45\" param=\""))
                {
                    string temp = s.Split(new string[] { "param=\"" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    temp = temp.Split('\"')[0];
                    c.needMinTotalMinions = Convert.ToInt32(temp);
                    continue;
                }
                if (s.Contains("PlayRequirement reqID=\"19\" param=\""))
                {
                    string temp = s.Split(new string[] { "param=\"" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    temp = temp.Split('\"')[0];
                    c.needMinionsCapIfAvailable = Convert.ToInt32(temp);
                    continue;
                }



                if (s.Contains("<Tag name="))
                {
                    string temp = s.Split(new string[] { "<Tag name=\"" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    temp = temp.Split('\"')[0];
                    /*
                    if (temp != "DevState" && temp != "FlavorText" && temp != "ArtistName" && temp != "Cost" && temp != "EnchantmentIdleVisual" && temp != "EnchantmentBirthVisual" && temp != "Collectible" && temp != "CardSet" && temp != "AttackVisualType" && temp != "CardName" && temp != "Class" && temp != "CardTextInHand" && temp != "Rarity" && temp != "TriggerVisual" && temp != "Faction" && temp != "HowToGetThisGoldCard" && temp != "HowToGetThisCard" && temp != "CardTextInPlay")
                        Helpfunctions.Instance.logg(s);*/
                }


            }


        }


        public Card getCardData(string cardname)
        {
            string target = cardname.ToLower();
            Card c = new Card();

            foreach (Card ca in this.cardlist)
            {
                if (ca.name == target)
                {
                    return ca;
                }
            }

            return new Card(c);
        }

        public Card getCardDataFromID(string id)
        {
            if (this.cardidToCardList.ContainsKey(id))
            {
                return new Card(cardidToCardList[id]);
            }

            return new Card();
        }

        public static Enchantment getEnchantmentFromCardID(string cardID)
        {
            Enchantment retval = new Enchantment();
            retval.CARDID = cardID;

            if (cardID == "CS2_188o")//insiriert  dieser diener hat +2 angriff in diesem zug. (ruchloser unteroffizier)
            {
                retval.angrbuff = 2;
            }

            if (cardID == "CS2_059o")//blutpakt (blutwichtel)
            {
                retval.hpbuff = 1;
            }
            if (cardID == "EX1_019e")//Segen der Klerikerin (blutelfenklerikerin)
            {
                retval.angrbuff = 1;
                retval.hpbuff = 1;
            }

            if (cardID == "CS2_045e")//waffedesfelsbeissers
            {
                retval.angrbuff = 3;
            }
            if (cardID == "EX1_587e")//windfury
            {
                retval.windfury = true;
            }
            if (cardID == "EX1_355e")//urteildestemplers   granted by blessed champion
            {
                retval.angrfaktor = 2;
            }
            if (cardID == "NEW1_036e")//befehlsruf
            {
                retval.cantLowerHPbelowONE = true;
            }
            if (cardID == "CS2_046e")// kampfrausch
            {
                retval.angrbuff = 3;
            }

            if (cardID == "CS2_104e")// toben
            {
                retval.angrbuff = 3;
                retval.hpbuff = 3;
            }
            if (cardID == "DREAM_05e")// alptraum
            {
                retval.angrbuff = 5;
                retval.hpbuff = 5;
            }
            if (cardID == "CS2_022e")// verwandlung
            {
                retval.angrbuff = 3;
            }
            if (cardID == "EX1_611e")// gefangen
            {
                //icetrap?
            }

            if (cardID == "EX1_014te")// banane
            {
                retval.angrbuff = 1;
                retval.hpbuff = 1;
            }
            if (cardID == "EX1_178ae")// festgewurzelt
            {
                retval.hpbuff = 5;
                retval.taunt = true;
            }
            if (cardID == "CS2_011o")// wildesbruellen
            {
                retval.angrbuff = 2;
            }
            if (cardID == "EX1_366e")// rechtschaffen
            {
                retval.angrbuff = 1;
                retval.hpbuff = 1;
            }
            if (cardID == "CS2_017o")// klauen (ownhero +1angr)
            {
            }
            if (cardID == "EX1_604o")// rasend
            {
                retval.angrbuff = 1;
            }
            if (cardID == "EX1_084e")// sturmangriff
            {
                retval.charge = true;
            }
            if (cardID == "CS1_129e")// inneresfeuer // angr = live
            {
                retval.angrEqualLife = true;
            }
            if (cardID == "EX1_603e")// aufzackgebracht (fieser zuchtmeister)
            {
                retval.angrbuff = 2;
            }
            if (cardID == "EX1_507e")// mrgglaargl! der murlocanführer verleiht +2/+1.
            {
                retval.angrbuff = 2;
                retval.hpbuff = 1;
            }
            if (cardID == "CS2_038e")// geistderahnen : todesröcheln: dieser diener kehrt aufs schlachtfeld zurück.
            {

            }
            if (cardID == "NEW1_024o")// gruenhauts befehl +1/+1.
            {
                retval.angrbuff = 1;
                retval.hpbuff = 1;
            }
            if (cardID == "EX1_590e")// schattenvonmuru : angriff und leben durch aufgezehrte gottesschilde erhöht. (blutritter)
            {
                retval.angrbuff = 3;
                retval.hpbuff = 3;
            }
            if (cardID == "CS2_074e")// toedlichesgift
            {
            }
            if (cardID == "EX1_258e")// ueberladen von entfesselnder elementar
            {
                retval.angrbuff = 1;
                retval.hpbuff = 1;
            }
            if (cardID == "TU4f_004o")// vermaechtnisdeskaisers von cho
            {
                retval.angrbuff = 2;
                retval.hpbuff = 2;
            }

            if (cardID == "NEW1_017e")// gefuellterbauch randvoll mit murloc. (hungrigekrabbe)
            {
                retval.angrbuff = 2;
                retval.hpbuff = 2;
            }

            if (cardID == "EX1_334e")// dunklerbefehl von dunkler Wahnsin
            {
            }

            if (cardID == "CS2_087e")// segendermacht von segendermacht
            {
                retval.angrbuff = 3;
            }
            if (cardID == "EX1_613e")// vancleefsrache dieser diener hat erhöhten angriff und erhöhtes leben.
            {
                retval.angrbuff = 2;
                retval.hpbuff = 2;
            }
            if (cardID == "EX1_623e")// infusion
            {
                retval.hpbuff = 3;
            }
            if (cardID == "CS2_073e2")// kaltbluetigkeit +4
            {
                retval.angrbuff = 4;
            }
            if (cardID == "EX1_162o")// staerkedesrudels der terrorwolfalpha verleiht diesem diener +1 angriff.
            {
                retval.angrbuff = 1;
            }
            if (cardID == "EX1_549o")// zorndeswildtiers +2 angriff und immun/ in diesem zug.
            {
                retval.angrbuff = 2;
                retval.imune = true;
            }

            if (cardID == "EX1_091o")//  kontrollederkabale  dieser diener wurde von einer kabaleschattenpriesterin gestohlen.
            {
            }

            if (cardID == "CS2_084e")//  maldesjaegers
            {
                retval.setHPtoOne = true;
            }
            if (cardID == "NEW1_036e2")//  befehlsruf2 ? das leben eurer diener kann in diesem zug nicht unter 1 fallen.
            {
                retval.cantLowerHPbelowONE = true;
            }
            if (cardID == "CS2_122e")// angespornt der schlachtzugsleiter verleiht diesem diener +1 angriff. (schlachtzugsleiter)
            {
                retval.angrbuff = 1;
            }
            if (cardID == "CS2_103e")// charge
            {
                retval.charge = true;
            }
            if (cardID == "EX1_080o")// geheimnissebewahren    erhöhte werte.
            {
                retval.angrbuff = 1;
                retval.hpbuff = 1;
            }
            if (cardID == "CS2_005o")// klaue +2 angriff in diesem zug.
            {
                retval.angrbuff = 2;
            }
            if (cardID == "EX1_363e2")// segenderweisheit
            {
                retval.cardDrawOnAngr = true;
            }
            if (cardID == "EX1_178be")//  entwurzelt +5 angr
            {
                retval.angrbuff = 5;
            }
            if (cardID == "CS2_222o")//  diemachtsturmwinds +1+1 (von champ of sturmwind)
            {
                retval.angrbuff = 1;
                retval.hpbuff = 1;
            }
            if (cardID == "EX1_399e")// amoklauf von gurubashi berserker
            {
                retval.angrbuff = 3;
            }
            if (cardID == "CS2_041e")// machtderahnen
            {
                retval.taunt = true;
            }
            if (cardID == "EX1_612o")//  machtderkirintor
            {

            }
            if (cardID == "EX1_004e")// elunesanmut erhöhtes leben. von junger priesterin
            {
                retval.hpbuff = 1;
            }
            if (cardID == "EX1_246e")// verhext dieser diener wurde verwandelt.
            {

            }
            if (cardID == "EX1_244e")// machtdertotems (card that buffs hp of totems)
            {
                retval.hpbuff = 2;
            }
            if (cardID == "EX1_607e")// innerewut (innere wut)
            {
                retval.angrbuff = 2;
            }
            if (cardID == "EX1_573ae")// gunstdeshalbgotts (cenarius?)
            {
                retval.angrbuff = 2;
                retval.hpbuff = 2;
            }
            if (cardID == "EX1_411e2")// schliffbenoetigt angriff verringert.  von waffe blutschrei
            {
                retval.angrbuff = -1;
            }
            if (cardID == "CS2_063e")// verderbnis  wird zu beginn des zuges des verderbenden spielers vernichtet.
            {

            }
            if (cardID == "CS2_181e")// vollekraft +2 angr ka von wem
            {
                retval.angrbuff = 2;
            }
            if (cardID == "EX1_508o")// mlarggragllabl! dieser murloc hat +1 angriff. (grimmschuppenorakel)
            {
                retval.angrbuff = 1;
            }
            if (cardID == "CS2_073e")// kaltbluetigkeit +2 angriff.
            {
                retval.angrbuff = 2;
            }
            if (cardID == "NEW1_018e")// goldrausch von blutsegelraeuberin
            {

            }
            if (cardID == "EX1_059e2")// experimente! der verrückte alchemist hat angriff und leben vertauscht.
            {

            }
            if (cardID == "EX1_570e")// biss (only hero)
            {
                retval.angrbuff = 4;
            }
            if (cardID == "EX1_360e")//  demut  angriff wurde auf 1 gesetzt.
            {
                retval.setANGRtoOne = true;
            }
            if (cardID == "DS1_175o")// wutgeheul durch waldwolf
            {
                retval.angrbuff = 1;
            }
            if (cardID == "EX1_596e")// daemonenfeuer
            {
                retval.angrbuff = 2;
                retval.hpbuff = 2;
            }

            if (cardID == "EX1_158e")// seeledeswaldes todesröcheln: ruft einen treant (2/2) herbei.
            {

            }
            if (cardID == "EX1_316e")// ueberwaeltigendemacht
            {
                retval.angrbuff = 4;
                retval.hpbuff = 4;
            }
            if (cardID == "EX1_044e")// stufenaufstieg erhöhter angriff und erhöhtes leben. (rastloser abenteuer)
            {

            }
            if (cardID == "EX1_304e")// verzehren  erhöhte werte. (hexer)
            {

            }
            if (cardID == "EX1_363e")// segenderweisheit der segnende spieler zieht eine karte, wenn dieser diener angreift.
            {

            }
            if (cardID == "CS2_105e")// heldenhafterstoss
            {

            }
            if (cardID == "EX1_128e")// verhuellt bleibt bis zu eurem nächsten zug verstohlen.
            {

            }
            if (cardID == "NEW1_033o")// himmelsauge leokk verleiht diesem diener +1 angriff.
            {
                retval.angrbuff = 1;
            }
            if (cardID == "CS2_004e")// machtwortschild
            {
                retval.hpbuff = 2;
            }
            if (cardID == "EX1_382e")// waffenniederlegen! angriff auf 1 gesetzt.
            {
                retval.setANGRtoOne = true;
            }
            if (cardID == "CS2_092e")// segenderkoenige
            {
                retval.angrbuff = 4;
                retval.hpbuff = 4;
            }
            if (cardID == "NEW1_012o")// manasaettigung  erhöhter angriff.
            {

            }
            if (cardID == "EX1_619e")//  gleichheit  leben auf 1 gesetzt.
            {
                retval.setHPtoOne = true;
            }
            if (cardID == "EX1_509e")// blarghghl    erhöhter angriff.
            {
                retval.angrbuff = 1;
            }
            if (cardID == "CS2_009e")// malderwildnis
            {
                retval.angrbuff = 2;
                retval.hpbuff = 2;
                retval.taunt = true;
            }
            if (cardID == "EX1_103e")// mrghlglhal +2 leben.
            {
                retval.hpbuff = 2;
            }
            if (cardID == "NEW1_038o")// wachstum  gruul wächst ...
            {

            }
            if (cardID == "CS1_113e")//  gedankenkontrolle
            {

            }
            if (cardID == "CS2_236e")//  goettlicherwille  dieser diener hat doppeltes leben.
            {

            }
            if (cardID == "CS2_083e")// geschaerft +1 angriff in diesem zug.
            {
                retval.angrbuff = 1;
            }
            if (cardID == "TU4c_008e")// diemachtmuklas
            {
                retval.angrbuff = 8;
            }
            if (cardID == "EX1_379e")//  busse 
            {
                retval.setHPtoOne = true;
            }
            if (cardID == "EX1_274e")// puremacht! (astraler arkanist)
            {
                retval.angrbuff = 2;
                retval.hpbuff = 2;
            }
            if (cardID == "CS2_221e")// vorsicht!scharf! +2 angriff von hasserfüllte schmiedin. 
            {
                retval.weaponAttack = 2;
            }
            if (cardID == "EX1_409e")// aufgewertet +1 angriff und +1 haltbarkeit.
            {
                retval.weaponAttack = 1;
                retval.weapondurability = 1;
            }
            if (cardID == "tt_004o")//kannibalismus (fleischfressender ghul)
            {
                retval.angrbuff = 1;
            }
            if (cardID == "EX1_155ae")// maldernatur
            {
                retval.angrbuff = 4;
            }
            if (cardID == "NEW1_025e")// verstaerkt (by emboldener 3000)
            {
                retval.angrbuff = 1;
                retval.hpbuff = 1;
            }
            if (cardID == "EX1_584e")// lehrenderkirintor zauberschaden+1 (by uralter magier)
            {
                retval.zauberschaden = 1;
            }
            if (cardID == "EX1_160be")// rudelfuehrer +1/+1. (macht der wildnis)
            {
                retval.angrbuff = 1;
                retval.hpbuff = 1;
            }
            if (cardID == "TU4c_006e")//  banane
            {
                retval.angrbuff = 1;
                retval.hpbuff = 1;
            }
            if (cardID == "NEW1_027e")// yarrr!   der südmeerkapitän verleiht +1/+1.
            {
                retval.angrbuff = 1;
                retval.hpbuff = 1;
            }
            if (cardID == "DS1_070o")// praesenzdesmeisters +2/+2 und spott/. (hundemeister)
            {
                retval.angrbuff = 2;
                retval.hpbuff = 2;
                retval.taunt = true;
            }
            if (cardID == "EX1_046e")// gehaertet +2 angriff in diesem zug. (dunkeleisenzwerg)
            {
                retval.angrbuff = 2;
            }
            if (cardID == "EX1_531e")// satt    erhöhter angriff und erhöhtes leben. (aasfressende Hyaene)
            {
                retval.angrbuff = 2;
                retval.hpbuff = 1;
            }
            if (cardID == "CS2_226e")// bannerderfrostwoelfe    erhöhte werte. (frostwolfkriegsfuerst)
            {
                retval.angrbuff = 1;
                retval.hpbuff = 1;
            }
            if (cardID == "DS1_178e")//  sturmangriff tundranashorn verleiht ansturm.
            {
                retval.charge = true;
            }
            if (cardID == "CS2_226o")//befehlsgewalt der kriegsfürst der frostwölfe hat erhöhten angriff und erhöhtes leben.
            {
                retval.angrbuff = 1;
                retval.hpbuff = 1;
            }
            if (cardID == "Mekka4e")// verwandelt wurde in ein huhn verwandelt!
            {

            }
            if (cardID == "EX1_411e")// blutrausch kein haltbarkeitsverlust. (blutschrei)
            {

            }
            if (cardID == "EX1_145o")// vorbereitung    der nächste zauber, den ihr in diesem zug wirkt, kostet (3) weniger.
            {

            }
            if (cardID == "EX1_055o")// gestaerkt    die manasüchtige hat erhöhten angriff.
            {
                retval.angrbuff = 2;
            }
            if (cardID == "CS2_053e")// fernsicht   eine eurer karten kostet (3) weniger.
            {

            }
            if (cardID == "CS2_146o")//  geschaerft +1 haltbarkeit.
            {
                retval.weapondurability = 1;
            }
            if (cardID == "EX1_059e")//  experimente! der verrückte alchemist hat angriff und leben vertauscht.
            {

            }
            if (cardID == "EX1_565o")// flammenzunge +2 angriff von totem der flammenzunge.
            {
                retval.angrbuff = 2;
            }
            if (cardID == "EX1_001e")// wachsam    erhöhter angriff. (lichtwaechterin)
            {
                retval.angrbuff = 2;
            }
            if (cardID == "EX1_536e")// aufgewertet   erhöhte haltbarkeit.
            {
                retval.weaponAttack = 1;
                retval.weapondurability = 1;
            }
            if (cardID == "EX1_155be")// maldernatur   dieser diener hat +4 leben und spott/.
            {
                retval.hpbuff = 4;
                retval.taunt = true;
            }
            if (cardID == "CS2_103e2")// sturmangriff    +2 angriff und ansturm/.
            {
                retval.angrbuff = 2;
                retval.charge = true;
            }
            if (cardID == "TU4f_006o")// transzendenz    cho kann nicht angegriffen werden, bevor ihr seine diener erledigt habt.
            {

            }
            if (cardID == "EX1_043e")// stundedeszwielichts    erhöhtes leben. (zwielichtdrache)
            {
                retval.hpbuff = 1;
            }
            if (cardID == "NEW1_037e")// bewaffnet   erhöhter angriff. meisterschwertschmied
            {
                retval.angrbuff = 1;
            }
            if (cardID == "EX1_161o")// demoralisierendesgebruell    dieser diener hat -3 angriff in diesem zug.
            {

            }
            if (cardID == "EX1_093e")// handvonargus
            {
                retval.angrbuff = 1;
                retval.hpbuff = 1;
                retval.taunt = true;
            }


            return retval;
        }



    }

    public class BoardTester
    {
        int ownPlayer = 1;

        int enemmaxman = 0;

        int mana = 0;
        int maxmana = 0;
        string ownheroname = "";
        int ownherohp = 0;
        int ownherodefence = 0;
        bool ownheroready = false;
        bool ownHeroimmunewhileattacking = false;
        int ownheroattacksThisRound = 0;
        int ownHeroAttack = 0;
        string ownHeroWeapon = "";
        int ownHeroWeaponAttack = 0;
        int ownHeroWeaponDurability = 0;
        int numMinionsPlayedThisTurn = 0;
        int cardsPlayedThisTurn = 0;
        int overdrive = 0;

        int ownDecksize = 30;
        int enemyDecksize = 30;
        int ownFatigue = 0;
        int enemyFatigue = 0;

        bool heroImmune = false;
        bool enemyHeroImmune = false;

        int enemySecrets = 0;

        bool ownHeroFrozen = false;

        List<string> ownsecretlist = new List<string>();
        string enemyheroname = "";
        int enemyherohp = 0;
        int enemyherodefence = 0;
        bool enemyFrozen = false;
        int enemyWeaponAttack = 0;
        int enemyWeaponDur = 0;
        string enemyWeapon = "";

        List<Minion> ownminions = new List<Minion>();
        List<Minion> enemyminions = new List<Minion>();
        List<Handmanager.Handcard> handcards = new List<Handmanager.Handcard>();

        public BoardTester()
        {
            string[] lines = new string[0] { };
            try
            {
                string path = Settings.Instance.path;
                lines = System.IO.File.ReadAllLines(path + "test.txt");
            }
            catch
            {
                Helpfunctions.Instance.logg("cant find test.txt");
                Helpfunctions.Instance.ErrorLog("cant find test.txt");
                return;
            }

            CardDB.Card heroability = CardDB.Instance.getCardDataFromID("CS2_034");
            CardDB.Card enemyability = CardDB.Instance.getCardDataFromID("CS2_034");
            bool abilityReady = false;

            int readstate = 0;
            int counter = 0;

            Minion tempminion = new Minion();
            int j = 0;
            foreach (string sss in lines)
            {
                string s = sss + " ";
                Helpfunctions.Instance.logg(s);

                if (s.StartsWith("ailoop"))
                {
                    break;
                }
                if (s.StartsWith("####"))
                {
                    continue;
                }
                if (s.StartsWith("start calculations"))
                {
                    continue;
                }

                if (s.StartsWith("enemy secretsCount:"))
                {
                    this.enemySecrets = Convert.ToInt32(s.Split(' ')[2]);
                    continue;
                }

                if (s.StartsWith("mana "))
                {
                    string ss = s.Replace("mana ", "");
                    mana = Convert.ToInt32(ss.Split('/')[0]);
                    maxmana = Convert.ToInt32(ss.Split('/')[1]);
                }

                if (s.StartsWith("emana "))
                {
                    string ss = s.Replace("emana ", "");
                    enemmaxman = Convert.ToInt32(ss);
                }

                if (readstate == 42 && counter == 1) // player
                {
                    this.overdrive = Convert.ToInt32(s.Split(' ')[2]);
                    this.numMinionsPlayedThisTurn = Convert.ToInt32(s.Split(' ')[0]);
                    this.cardsPlayedThisTurn = Convert.ToInt32(s.Split(' ')[1]);
                    this.ownPlayer = Convert.ToInt32(s.Split(' ')[3]);
                }

                if (readstate == 1 && counter == 1) // class + hp + defence + immunewhile attacking + immune
                {
                    ownheroname = s.Split(' ')[0];
                    ownherohp = Convert.ToInt32(s.Split(' ')[1]);
                    ownherodefence = Convert.ToInt32(s.Split(' ')[2]);
                    this.ownHeroimmunewhileattacking = (s.Split(' ')[3] == "True") ? true : false;
                    this.heroImmune = (s.Split(' ')[4] == "True") ? true : false;

                }

                if (readstate == 1 && counter == 2) // ready, num attacks this turn, frozen
                {
                    string readystate = s.Split(' ')[1];
                    this.ownheroready = (readystate == "True") ? true : false;
                    this.ownheroattacksThisRound = Convert.ToInt32(s.Split(' ')[3]);

                    this.ownHeroFrozen = (s.Split(' ')[5] == "True") ? true : false;

                    ownHeroAttack = Convert.ToInt32(s.Split(' ')[7]);
                    ownHeroWeaponAttack = Convert.ToInt32(s.Split(' ')[8]);
                    this.ownHeroWeaponDurability = Convert.ToInt32(s.Split(' ')[9]);
                    if (ownHeroWeaponAttack == 0)
                    {
                        ownHeroWeapon = ""; //:D
                    }
                    else
                    {
                        ownHeroWeapon = s.Split(' ')[10];
                    }
                }

                if (readstate == 1 && counter == 3) // ability + abilityready
                {
                    abilityReady = (s.Split(' ')[1] == "True") ? true : false;
                    heroability = CardDB.Instance.getCardDataFromID(s.Split(' ')[2]);
                }

                if (readstate == 1 && counter >= 5) // secrets
                {
                    if (!s.StartsWith("enemyhero:"))
                    {
                        ownsecretlist.Add(s.Replace(" ", ""));
                    }
                }

                if (readstate == 2 && counter == 1) // class + hp + defence + frozen + immune
                {
                    enemyheroname = s.Split(' ')[0];
                    enemyherohp = Convert.ToInt32(s.Split(' ')[1]);
                    enemyherodefence = Convert.ToInt32(s.Split(' ')[2]);
                    enemyFrozen = (s.Split(' ')[3] == "True") ? true : false;
                    enemyHeroImmune = (s.Split(' ')[4] == "True") ? true : false;
                }

                if (readstate == 2 && counter == 2) // wepon + stuff
                {
                    this.enemyWeaponAttack = Convert.ToInt32(s.Split(' ')[0]);
                    this.enemyWeaponDur = Convert.ToInt32(s.Split(' ')[1]);
                    if (enemyWeaponDur == 0)
                    {
                        this.enemyWeapon = "";
                    }
                    else
                    {
                        this.enemyWeapon = s.Split(' ')[2];
                    }

                }
                if (readstate == 2 && counter == 3) // ability
                {
                    enemyability = CardDB.Instance.getCardDataFromID(s.Split(' ')[2]);
                }
                if (readstate == 2 && counter == 4) // fatigue
                {
                    this.ownDecksize = Convert.ToInt32(s.Split(' ')[1]);
                    this.enemyDecksize = Convert.ToInt32(s.Split(' ')[3]);
                    this.ownFatigue = Convert.ToInt32(s.Split(' ')[2]);
                    this.enemyFatigue = Convert.ToInt32(s.Split(' ')[4]);
                }

                if (readstate == 3) // minion or enchantment
                {
                    if (s.Contains(" id:"))
                    {
                        if (counter >= 2) this.ownminions.Add(tempminion);

                        string minionname = s.Split(' ')[0];
                        string minionid = s.Split(' ')[1];
                        int attack = Convert.ToInt32(s.Split(new string[] { " A:" }, StringSplitOptions.RemoveEmptyEntries)[1].Split(' ')[0]);
                        int hp = Convert.ToInt32(s.Split(new string[] { " H:" }, StringSplitOptions.RemoveEmptyEntries)[1].Split(' ')[0]);
                        int maxhp = Convert.ToInt32(s.Split(new string[] { " mH:" }, StringSplitOptions.RemoveEmptyEntries)[1].Split(' ')[0]);
                        bool ready = s.Split(new string[] { " rdy:" }, StringSplitOptions.RemoveEmptyEntries)[1].Split(' ')[0] == "True" ? true : false;
                        bool taunt = s.Split(new string[] { " tnt:" }, StringSplitOptions.RemoveEmptyEntries)[1].Split(' ')[0] == "True" ? true : false;
                        bool frzn = false;
                        if (s.Contains(" frz:")) frzn = s.Split(new string[] { " frz:" }, StringSplitOptions.RemoveEmptyEntries)[1].Split(' ')[0] == "True" ? true : false;
                        bool silenced = false;
                        if (s.Contains(" silenced:")) silenced = s.Split(new string[] { " silenced:" }, StringSplitOptions.RemoveEmptyEntries)[1].Split(' ')[0] == "True" ? true : false;
                        bool divshield = false;
                        if (s.Contains(" divshield:")) divshield = s.Split(new string[] { " divshield:" }, StringSplitOptions.RemoveEmptyEntries)[1].Split(' ')[0] == "True" ? true : false;
                        bool ptt = false;//played this turn
                        if (s.Contains(" ptt:")) ptt = s.Split(new string[] { " ptt:" }, StringSplitOptions.RemoveEmptyEntries)[1].Split(' ')[0] == "True" ? true : false;
                        bool wndfry = false;//windfurry
                        if (s.Contains(" wndfr:")) wndfry = s.Split(new string[] { " wndfr:" }, StringSplitOptions.RemoveEmptyEntries)[1].Split(' ')[0] == "True" ? true : false;
                        int natt = 0;
                        if (s.Contains(" natt:")) natt = Convert.ToInt32(s.Split(new string[] { " natt:" }, StringSplitOptions.RemoveEmptyEntries)[1].Split(' ')[0]);

                        int ent = 1000 + j;
                        if (s.Contains(" e:")) ent = Convert.ToInt32(s.Split(new string[] { " e:" }, StringSplitOptions.RemoveEmptyEntries)[1].Split(' ')[0]);

                        bool pois = false;//poision
                        if (s.Contains(" poi:")) pois = s.Split(new string[] { " poi:" }, StringSplitOptions.RemoveEmptyEntries)[1].Split(' ')[0] == "True" ? true : false;
                        bool stl = false;//stealth
                        if (s.Contains(" stl:")) stl = s.Split(new string[] { " stl:" }, StringSplitOptions.RemoveEmptyEntries)[1].Split(' ')[0] == "True" ? true : false;
                        bool immn = false;//immune
                        if (s.Contains(" imm:")) immn = s.Split(new string[] { " imm:" }, StringSplitOptions.RemoveEmptyEntries)[1].Split(' ')[0] == "True" ? true : false;
                        bool chrg = false;//charge
                        if (s.Contains(" chrg:")) chrg = s.Split(new string[] { " chrg:" }, StringSplitOptions.RemoveEmptyEntries)[1].Split(' ')[0] == "True" ? true : false;
                        bool ex = false;//exhausted
                        if (s.Contains(" ex:")) ex = s.Split(new string[] { " ex:" }, StringSplitOptions.RemoveEmptyEntries)[1].Split(' ')[0] == "True" ? true : false;


                        int id = Convert.ToInt32(s.Split(new string[] { " id:" }, StringSplitOptions.RemoveEmptyEntries)[1].Split(' ')[0]);
                        tempminion = createNewMinion(new Handmanager.Handcard(CardDB.Instance.getCardDataFromID(minionid)), id);
                        tempminion.Angr = attack;
                        tempminion.Hp = hp;
                        tempminion.maxHp = maxhp;
                        tempminion.Ready = ready;
                        tempminion.taunt = taunt;
                        tempminion.divineshild = divshield;
                        tempminion.playedThisTurn = ptt;
                        tempminion.windfury = wndfry;
                        tempminion.numAttacksThisTurn = natt;
                        tempminion.entitiyID = ent;
                        tempminion.handcard.entity = ent;
                        tempminion.silenced = silenced;
                        tempminion.exhausted = ex;
                        tempminion.poisonous = pois;
                        tempminion.stealth = stl;
                        tempminion.immune = immn;
                        tempminion.charge = chrg;
                        tempminion.frozen = frzn;
                        if (maxhp > hp) tempminion.wounded = true;





                    }
                    else
                    {
                        try
                        {
                            Enchantment e = CardDB.getEnchantmentFromCardID(s.Split(' ')[0]);
                            e.controllerOfCreator = Convert.ToInt32(s.Split(' ')[2]);
                            e.creator = Convert.ToInt32(s.Split(' ')[1]);
                            tempminion.enchantments.Add(e);
                        }
                        catch
                        {
                        }
                    }

                }

                if (readstate == 4) // minion or enchantment
                {
                    if (s.Contains(" id:"))
                    {
                        if (counter >= 2) this.enemyminions.Add(tempminion);

                        string minionname = s.Split(' ')[0];
                        string minionid = s.Split(' ')[1];
                        int attack = Convert.ToInt32(s.Split(new string[] { " A:" }, StringSplitOptions.RemoveEmptyEntries)[1].Split(' ')[0]);
                        int hp = Convert.ToInt32(s.Split(new string[] { " H:" }, StringSplitOptions.RemoveEmptyEntries)[1].Split(' ')[0]);
                        int maxhp = Convert.ToInt32(s.Split(new string[] { " mH:" }, StringSplitOptions.RemoveEmptyEntries)[1].Split(' ')[0]);
                        bool ready = s.Split(new string[] { " rdy:" }, StringSplitOptions.RemoveEmptyEntries)[1].Split(' ')[0] == "True" ? true : false;
                        bool taunt = s.Split(new string[] { " tnt:" }, StringSplitOptions.RemoveEmptyEntries)[1].Split(' ')[0] == "True" ? true : false;
                        bool frzn = false;
                        if (s.Contains(" frz:")) frzn = s.Split(new string[] { " frz:" }, StringSplitOptions.RemoveEmptyEntries)[1].Split(' ')[0] == "True" ? true : false;

                        bool silenced = false;
                        if (s.Contains(" silenced:")) silenced = s.Split(new string[] { " silenced:" }, StringSplitOptions.RemoveEmptyEntries)[1].Split(' ')[0] == "True" ? true : false;
                        bool divshield = false;
                        if (s.Contains(" divshield:")) divshield = s.Split(new string[] { " divshield:" }, StringSplitOptions.RemoveEmptyEntries)[1].Split(' ')[0] == "True" ? true : false;
                        bool ptt = false;//played this turn
                        if (s.Contains(" ptt:")) ptt = s.Split(new string[] { " ptt:" }, StringSplitOptions.RemoveEmptyEntries)[1].Split(' ')[0] == "True" ? true : false;
                        bool wndfry = false;//windfurry
                        if (s.Contains(" wndfr:")) wndfry = s.Split(new string[] { " wndfr:" }, StringSplitOptions.RemoveEmptyEntries)[1].Split(' ')[0] == "True" ? true : false;
                        int natt = 0;
                        if (s.Contains(" natt:")) natt = Convert.ToInt32(s.Split(new string[] { " natt:" }, StringSplitOptions.RemoveEmptyEntries)[1].Split(' ')[0]);

                        int ent = 1000 + j;
                        if (s.Contains(" e:")) ent = Convert.ToInt32(s.Split(new string[] { " e:" }, StringSplitOptions.RemoveEmptyEntries)[1].Split(' ')[0]);

                        bool pois = false;//poision
                        if (s.Contains(" poi:")) pois = s.Split(new string[] { " poi:" }, StringSplitOptions.RemoveEmptyEntries)[1].Split(' ')[0] == "True" ? true : false;
                        bool stl = false;//stealth
                        if (s.Contains(" stl:")) stl = s.Split(new string[] { " stl:" }, StringSplitOptions.RemoveEmptyEntries)[1].Split(' ')[0] == "True" ? true : false;
                        bool immn = false;//immune
                        if (s.Contains(" imm:")) immn = s.Split(new string[] { " imm:" }, StringSplitOptions.RemoveEmptyEntries)[1].Split(' ')[0] == "True" ? true : false;
                        bool chrg = false;//charge
                        if (s.Contains(" chrg:")) chrg = s.Split(new string[] { " chrg:" }, StringSplitOptions.RemoveEmptyEntries)[1].Split(' ')[0] == "True" ? true : false;
                        bool ex = false;//exhausted
                        if (s.Contains(" ex:")) ex = s.Split(new string[] { " ex:" }, StringSplitOptions.RemoveEmptyEntries)[1].Split(' ')[0] == "True" ? true : false;

                        int id = Convert.ToInt32(s.Split(new string[] { " id:" }, StringSplitOptions.RemoveEmptyEntries)[1].Split(' ')[0]);
                        tempminion = createNewMinion(new Handmanager.Handcard(CardDB.Instance.getCardDataFromID(minionid)), id);
                        tempminion.Angr = attack;
                        tempminion.Hp = hp;
                        tempminion.maxHp = maxhp;
                        tempminion.Ready = ready;
                        tempminion.taunt = taunt;
                        tempminion.divineshild = divshield;
                        tempminion.playedThisTurn = ptt;
                        tempminion.windfury = wndfry;
                        tempminion.numAttacksThisTurn = natt;
                        tempminion.entitiyID = ent;
                        tempminion.silenced = silenced;
                        tempminion.exhausted = ex;
                        tempminion.poisonous = pois;
                        tempminion.stealth = stl;
                        tempminion.immune = immn;
                        tempminion.charge = chrg;
                        tempminion.frozen = frzn;
                        if (maxhp > hp) tempminion.wounded = true;


                    }
                    else
                    {
                        try
                        {
                            Enchantment e = CardDB.getEnchantmentFromCardID(s.Split(' ')[0]);
                            e.controllerOfCreator = Convert.ToInt32(s.Split(' ')[2]);
                            e.creator = Convert.ToInt32(s.Split(' ')[1]);
                            tempminion.enchantments.Add(e);
                        }
                        catch
                        {
                        }
                    }

                }

                if (readstate == 5) // minion or enchantment
                {

                    Handmanager.Handcard card = new Handmanager.Handcard();

                    string minionname = s.Split(' ')[2];
                    string minionid = s.Split(' ')[6];
                    int pos = Convert.ToInt32(s.Split(' ')[1]);
                    int mana = Convert.ToInt32(s.Split(' ')[3]);
                    card.card = CardDB.Instance.getCardDataFromID(minionid);
                    card.entity = Convert.ToInt32(s.Split(' ')[5]);
                    card.manacost = mana;
                    card.position = pos;
                    handcards.Add(card);

                }


                if (s.StartsWith("ownhero:"))
                {
                    readstate = 1;
                    counter = 0;
                }

                if (s.StartsWith("enemyhero:"))
                {
                    readstate = 2;
                    counter = 0;
                }

                if (s.StartsWith("OwnMinions:"))
                {
                    readstate = 3;
                    counter = 0;
                }

                if (s.StartsWith("EnemyMinions:"))
                {
                    if (counter >= 2) this.ownminions.Add(tempminion);

                    readstate = 4;
                    counter = 0;
                }

                if (s.StartsWith("Own Handcards:"))
                {
                    if (counter >= 2) this.enemyminions.Add(tempminion);

                    readstate = 5;
                    counter = 0;
                }

                if (s.StartsWith("player:"))
                {
                    readstate = 42;
                    counter = 0;
                }



                counter++;
                j++;
            }
            Helpfunctions.Instance.logg("rdy");


            Hrtprozis.Instance.setOwnPlayer(ownPlayer);
            Handmanager.Instance.setOwnPlayer(ownPlayer);

            Hrtprozis.Instance.updatePlayer(this.maxmana, this.mana, this.cardsPlayedThisTurn, this.numMinionsPlayedThisTurn, this.overdrive, 100, 200);
            Hrtprozis.Instance.updateSecretStuff(this.ownsecretlist, enemySecrets);

            int numattttHero = 0;
            bool herowindfury = false;
            Hrtprozis.Instance.updateOwnHero(this.ownHeroWeapon, this.ownHeroWeaponAttack, this.ownHeroWeaponDurability, ownHeroimmunewhileattacking, this.ownHeroAttack, this.ownherohp, this.ownherodefence, this.ownheroname, this.ownheroready, this.ownHeroFrozen, heroability, abilityReady, numattttHero, herowindfury, this.heroImmune);
            Hrtprozis.Instance.updateEnemyHero(this.enemyWeapon, this.enemyWeaponAttack, this.enemyWeaponDur, this.enemyWeaponAttack, this.enemyherohp, this.enemyherodefence, this.enemyheroname, this.enemyFrozen, enemyability, enemyHeroImmune, enemmaxman);

            Hrtprozis.Instance.updateMinions(this.ownminions, this.enemyminions);

            Hrtprozis.Instance.updateFatigueStats(this.ownDecksize, this.ownFatigue, this.enemyDecksize, this.enemyFatigue);

            Handmanager.Instance.setHandcards(this.handcards, this.handcards.Count, 5);


        }



        private Minion createNewMinion(Handmanager.Handcard hc, int id)
        {
            Minion m = new Minion();
            m.handcard = new Handmanager.Handcard(hc);
            m.id = id;
            m.zonepos = id + 1;
            m.entitiyID = hc.entity;
            m.Posix = 0;
            m.Posiy = 0;
            m.Angr = hc.card.Attack;
            m.Hp = hc.card.Health;
            m.maxHp = hc.card.Health;
            m.name = hc.card.name;
            m.playedThisTurn = true;
            m.numAttacksThisTurn = 0;


            if (hc.card.windfury) m.windfury = true;
            if (hc.card.tank) m.taunt = true;
            if (hc.card.Charge)
            {
                m.Ready = true;
                m.charge = true;
            }
            if (hc.card.Shield) m.divineshild = true;
            if (hc.card.poisionous) m.poisonous = true;

            if (hc.card.Stealth) m.stealth = true;

            if (m.name == "lightspawn" && !m.silenced)
            {
                m.Angr = m.Hp;
            }


            return m;
        }



    }

    public class Enchantment
    {
        public bool cantBeDispelled = false;
        public string CARDID = "";
        public int creator = 0;
        public int angrbuff = 0;
        public int hpbuff = 0;
        public int weaponAttack = 0;
        public int weapondurability = 0;
        public int angrfaktor = 1;
        public int hpfaktor = 1;
        public bool charge = false;
        public bool divineshild = false;
        public bool taunt = false;

        public bool cantLowerHPbelowONE = false;
        public bool angrEqualLife = false;

        public bool imune = false;
        public bool setHPtoOne = false;
        public bool setANGRtoOne = false;
        public bool cardDrawOnAngr = false;
        public bool windfury = false;
        public int zauberschaden = 0;
        public int controllerOfCreator = 0;
    }

    public class Minion
    {
        public int id = -1;
        public int Posix = 0;
        public int Posiy = 0;
        public int Hp = 0;
        public int maxHp = 0;
        public int Angr = 0;
        public bool Ready = false;
        public bool taunt = false;
        public bool wounded = false;//hp red?
        public string name = "";
        public Handmanager.Handcard handcard;
        public bool divineshild = false;
        public bool windfury = false;
        public bool frozen = false;
        public int zonepos = 0;
        public bool stealth = false;
        public bool immune = false;
        public bool exhausted = false;
        public int numAttacksThisTurn = 0;
        public bool playedThisTurn = false;
        public bool charge = false;
        public bool poisonous = false;
        public bool silenced = false;
        public int entitiyID = -1;
        public bool cantLowerHPbelowONE = false;
        public List<Enchantment> enchantments;

        public Minion()
        {
            enchantments = new List<Enchantment>();
            this.handcard = new Handmanager.Handcard();
        }

        public Minion(Minion m)
        {
            this.id = m.id;
            this.Posix = m.Posix;
            this.Posiy = m.Posiy;
            this.Hp = m.Hp;
            this.maxHp = m.maxHp;
            this.Angr = m.Angr;
            this.Ready = m.Ready;
            this.taunt = m.taunt;
            this.wounded = m.wounded;
            this.name = m.name;
            this.handcard = m.handcard;
            this.divineshild = m.divineshild;
            this.windfury = m.windfury;
            this.frozen = m.frozen;
            this.zonepos = m.zonepos;
            this.stealth = m.stealth;
            this.immune = m.immune;
            this.exhausted = m.exhausted;
            this.numAttacksThisTurn = m.numAttacksThisTurn;
            this.playedThisTurn = m.playedThisTurn;
            this.charge = m.charge;
            this.poisonous = m.poisonous;
            this.silenced = m.silenced;
            this.entitiyID = m.entitiyID;
            this.enchantments = new List<Enchantment>(m.enchantments);
            this.cantLowerHPbelowONE = m.cantLowerHPbelowONE;
        }

        public void setMinionTominion(Minion m)
        {
            this.id = m.id;
            this.Posix = m.Posix;
            this.Posiy = m.Posiy;
            this.Hp = m.Hp;
            this.maxHp = m.maxHp;
            this.Angr = m.Angr;
            this.Ready = m.Ready;
            this.taunt = m.taunt;
            this.wounded = m.wounded;
            this.name = m.name;
            this.handcard = m.handcard;
            this.divineshild = m.divineshild;
            this.windfury = m.windfury;
            this.frozen = m.frozen;
            this.zonepos = m.zonepos;
            this.stealth = m.stealth;
            this.immune = m.immune;
            this.exhausted = m.exhausted;
            this.numAttacksThisTurn = m.numAttacksThisTurn;
            this.playedThisTurn = m.playedThisTurn;
            this.charge = m.charge;
            this.poisonous = m.poisonous;
            this.silenced = m.silenced;
            this.entitiyID = m.entitiyID;
            this.enchantments.Clear();
            this.enchantments.AddRange(m.enchantments);
        }
    }


    public class BattleField
    {

        public class tagpair
        {
            public int Name = 0;
            public int Value = 0;
        }


        public class HrtUnit
        {

            public string CardID = "";

            public int entitiyID = 0;

            public List<tagpair> tags = new List<tagpair>();

            public int getTag(GAME_TAG gt)
            {
                foreach (tagpair t in tags)
                {
                    if ((GAME_TAG)t.Name == gt)
                    {
                        return t.Value;
                    }
                }
                return 0;
            }

        }

        private static BattleField instance;

        public static BattleField Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new BattleField();
                }
                return instance;
            }
        }

        private BattleField()
        {
        }
    }

    public enum Side
    {
        NEUTRAL,
        FRIENDLY,
        OPPOSING
    }

    public enum GAME_TAG
    {
        STATE = 204,
        TURN = 20,
        STEP = 19,
        NEXT_STEP = 198,
        TEAM_ID = 31,
        PLAYER_ID = 30,
        STARTHANDSIZE = 29,
        MAXHANDSIZE = 28,
        MAXRESOURCES = 176,
        TIMEOUT = 7,
        TURN_START,
        TURN_TIMER_SLUSH,
        GOLD_REWARD_STATE = 13,
        FIRST_PLAYER = 24,
        CURRENT_PLAYER = 23,
        HERO_ENTITY = 27,
        RESOURCES = 26,
        RESOURCES_USED = 25,
        FATIGUE = 22,
        PLAYSTATE = 17,
        CURRENT_SPELLPOWER = 291,
        MULLIGAN_STATE = 305,
        HAND_REVEALED = 348,
        CARDNAME = 185,
        CARDTEXT_INHAND = 184,
        CARDRACE = 200,
        CARDTYPE = 202,
        COST = 48,
        HEALTH = 45,
        ATK = 47,
        DURABILITY = 187,
        ARMOR = 292,
        PREDAMAGE = 318,
        TARGETING_ARROW_TEXT = 325,
        LAST_AFFECTED_BY = 18,
        ENCHANTMENT_BIRTH_VISUAL = 330,
        ENCHANTMENT_IDLE_VISUAL,
        PREMIUM = 12,
        IGNORE_DAMAGE = 1,
        IGNORE_DAMAGE_OFF = 354,
        ENTITY_ID = 53,
        DEFINITION = 52,
        OWNER = 51,
        CONTROLLER = 50,
        ZONE = 49,
        EXHAUSTED = 43,
        ATTACHED = 40,
        PROPOSED_ATTACKER = 39,
        ATTACKING = 38,
        PROPOSED_DEFENDER = 37,
        DEFENDING = 36,
        PROTECTED = 35,
        PROTECTING = 34,
        RECENTLY_ARRIVED = 33,
        DAMAGE = 44,
        TRIGGER_VISUAL = 32,
        TAUNT = 190,
        SPELLPOWER = 192,
        DIVINE_SHIELD = 194,
        CHARGE = 197,
        SECRET = 219,
        MORPH = 293,
        DIVINE_SHIELD_READY = 314,
        TAUNT_READY = 306,
        STEALTH_READY,
        CHARGE_READY,
        CREATOR = 313,
        CANT_DRAW = 232,
        CANT_PLAY = 231,
        CANT_DISCARD = 230,
        CANT_DESTROY = 229,
        CANT_TARGET = 228,
        CANT_ATTACK = 227,
        CANT_EXHAUST = 226,
        CANT_READY = 225,
        CANT_REMOVE_FROM_GAME = 224,
        CANT_SET_ASIDE = 223,
        CANT_DAMAGE = 222,
        CANT_HEAL = 221,
        CANT_BE_DESTROYED = 247,
        CANT_BE_TARGETED = 246,
        CANT_BE_ATTACKED = 245,
        CANT_BE_EXHAUSTED = 244,
        CANT_BE_READIED = 243,
        CANT_BE_REMOVED_FROM_GAME = 242,
        CANT_BE_SET_ASIDE = 241,
        CANT_BE_DAMAGED = 240,
        CANT_BE_HEALED = 239,
        CANT_BE_SUMMONING_SICK = 253,
        CANT_BE_DISPELLED = 314,
        INCOMING_DAMAGE_CAP = 238,
        INCOMING_DAMAGE_ADJUSTMENT = 237,
        INCOMING_DAMAGE_MULTIPLIER = 236,
        INCOMING_HEALING_CAP = 235,
        INCOMING_HEALING_ADJUSTMENT = 234,
        INCOMING_HEALING_MULTIPLIER = 233,
        FROZEN = 260,
        JUST_PLAYED,
        LINKEDCARD,
        ZONE_POSITION,
        CANT_BE_FROZEN,
        COMBO_ACTIVE = 266,
        CARD_TARGET,
        NUM_CARDS_PLAYED_THIS_TURN = 269,
        CANT_BE_TARGETED_BY_OPPONENTS,
        NUM_TURNS_IN_PLAY,
        SUMMONED = 205,
        ENRAGED = 212,
        SILENCED = 188,
        WINDFURY,
        LOYALTY = 216,
        DEATHRATTLE,
        ADJACENT_BUFF = 350,
        STEALTH = 191,
        BATTLECRY = 218,
        NUM_TURNS_LEFT = 272,
        OUTGOING_DAMAGE_CAP,
        OUTGOING_DAMAGE_ADJUSTMENT,
        OUTGOING_DAMAGE_MULTIPLIER,
        OUTGOING_HEALING_CAP,
        OUTGOING_HEALING_ADJUSTMENT,
        OUTGOING_HEALING_MULTIPLIER,
        INCOMING_ABILITY_DAMAGE_ADJUSTMENT,
        INCOMING_COMBAT_DAMAGE_ADJUSTMENT,
        OUTGOING_ABILITY_DAMAGE_ADJUSTMENT,
        OUTGOING_COMBAT_DAMAGE_ADJUSTMENT,
        OUTGOING_ABILITY_DAMAGE_MULTIPLIER,
        OUTGOING_ABILITY_DAMAGE_CAP,
        INCOMING_ABILITY_DAMAGE_MULTIPLIER,
        INCOMING_ABILITY_DAMAGE_CAP,
        OUTGOING_COMBAT_DAMAGE_MULTIPLIER,
        OUTGOING_COMBAT_DAMAGE_CAP,
        INCOMING_COMBAT_DAMAGE_MULTIPLIER,
        INCOMING_COMBAT_DAMAGE_CAP,
        IS_MORPHED = 294,
        TEMP_RESOURCES,
        RECALL_OWED,
        NUM_ATTACKS_THIS_TURN,
        NEXT_ALLY_BUFF = 302,
        MAGNET,
        FIRST_CARD_PLAYED_THIS_TURN,
        CARD_ID = 186,
        CANT_BE_TARGETED_BY_ABILITIES = 311,
        SHOULDEXITCOMBAT,
        PARENT_CARD = 316,
        NUM_MINIONS_PLAYED_THIS_TURN,
        CANT_BE_TARGETED_BY_HERO_POWERS = 332,
        COMBO = 220,
        ELITE = 114,
        CARD_SET = 183,
        FACTION = 201,
        RARITY = 203,
        CLASS = 199,
        MISSION_EVENT = 6,
        FREEZE = 208,
        RECALL = 215,
        SILENCE = 339,
        COUNTER,
        ARTISTNAME = 342,
        FLAVORTEXT = 351,
        FORCED_PLAY,
        LOW_HEALTH_THRESHOLD,
        SPELLPOWER_DOUBLE = 356,
        HEALING_DOUBLE,
        NUM_OPTIONS_PLAYED_THIS_TURN,
        NUM_OPTIONS,
        TO_BE_DESTROYED,
        HEALTH_MINIMUM = 337,
        AURA = 362,
        POISONOUS,
        HOW_TO_EARN,
        HOW_TO_EARN_GOLDEN,
        AFFECTED_BY_SPELL_POWER = 370,
        IMMUNE_WHILE_ATTACKING = 373
    }

    public enum TAG_ZONE
    {
        INVALID,
        PLAY,
        DECK,
        HAND,
        GRAVEYARD,
        REMOVEDFROMGAME,
        SETASIDE,
        SECRET
    }

    public enum TAG_MULLIGAN
    {
        INVALID,
        INPUT,
        DEALING,
        WAITING,
        DONE
    }

    public enum TAG_CLASS
    {
        INVALID,
        DEATHKNIGHT,
        DRUID,
        HUNTER,
        MAGE,
        PALADIN,
        PRIEST,
        ROGUE,
        SHAMAN,
        WARLOCK,
        WARRIOR,
        DREAM

    }

    public enum TAG_CARDTYPE
    {
        INVALID,
        GAME,
        PLAYER,
        HERO,
        MINION,
        ABILITY,
        ENCHANTMENT,
        WEAPON,
        ITEM,
        TOKEN,
        HERO_POWER
    }


    public enum AttackType
    {
        INVALID,
        REGULAR,
        PROPOSED,
        CANCELED,
        ONLY_ATTACKER,
        ONLY_DEFENDER,
        ONLY_PROPOSED_ATTACKER,
        ONLY_PROPOSED_DEFENDER,
        WAITING_ON_PROPOSED_ATTACKER,
        WAITING_ON_PROPOSED_DEFENDER,
        WAITING_ON_ATTACKER,
        WAITING_ON_DEFENDER
    }

    public enum TAG_PLAYSTATE
    {
        INVALID,
        PLAYING,
        WINNING,
        LOSING,
        WON,
        LOST,
        TIED,
        DISCONNECTED,
        QUIT
    }


    public enum TAG_RACE
    {
        INVALID,
        BLOODELF,
        DRAENEI,
        DWARF,
        GNOME,
        GOBLIN,
        HUMAN,
        NIGHTELF,
        ORC,
        TAUREN,
        TROLL,
        UNDEAD,
        WORGEN,
        GOBLIN2,
        MURLOC,
        DEMON,
        SCOURGE,
        MECHANICAL,
        ELEMENTAL,
        OGRE,
        PET,
        TOTEM,
        NERUBIAN,
        PIRATE,
        DRAGON
    }

    public enum TAG_STATE
    {
        INVALID,
        LOADING,
        RUNNING,
        COMPLETE
    }

    public enum TAG_STEP
    {
        INVALID,
        BEGIN_FIRST,
        BEGIN_SHUFFLE,
        BEGIN_DRAW,
        BEGIN_MULLIGAN,
        MAIN_BEGIN,
        MAIN_READY,
        MAIN_RESOURCE,
        MAIN_DRAW,
        MAIN_START,
        MAIN_ACTION,
        MAIN_COMBAT,
        MAIN_END,
        MAIN_NEXT,
        FINAL_WRAPUP,
        FINAL_GAMEOVER,
        MAIN_CLEANUP,
        MAIN_START_TRIGGERS
    }

    public enum CHOICE_TYPE
    {
        INVALID,
        MULLIGAN,
        GENERAL
    }


    class Settings
    {

        public string path = "";
        public string logpath = "";
        public string logfile = "Logg.txt";
        private static Settings instance;

        public static Settings Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Settings();
                }
                return instance;
            }
        }


        private Settings()
        {
        }

        public void setFilePath(string path)
        {
            this.path = path;
        }
        public void setLoggPath(string path)
        {
            this.logpath = path;
        }

        public void setLoggFile(string path)
        {
            this.logfile = path;
        }
    }

    public class targett
    {
        public int target = -1;
        public int targetEntity = -1;

        public targett(int targ, int ent)
        {
            this.target = targ;
            this.targetEntity = ent;
        }
    }

   
}

