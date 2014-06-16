using HREngine.API;
using HREngine.API.Utilities;
using System;
using System.Collections.Generic;

namespace HREngine.Bots
{
   public class PossibleTurnAttack
   {
      public int Cost;
      public int Attack;
      public int NeededAttack;
      public List<HRCard> Cards;
   }




   public abstract class Bot : API.IBot
   {
       
       private int dirtytarget = -1;
       private int dirtychoice = -1;
       private string choiceCardId = "";
       Silverfish sf;

      public Bot()
      {
          OnBattleStateUpdate = HandleOnBattleStateUpdate;
          OnMulliganStateUpdate = HandleBattleMulliganPhase;
          this.sf = new Silverfish();
          sf.setnewLoggFile();
          //Ai.Instance.autoTester(this);
      }



      private HREngine.API.Actions.ActionBase HandleBattleMulliganPhase()
      {
          HRLog.Write("handle mulligan");
         if (HRMulligan.IsMulliganActive())
         {
            var list = HRCard.GetCards(HRPlayer.GetLocalPlayer(), HRCardZone.HAND);

            foreach (var item in list)
            {
               if (item.GetEntity().GetCost() >= 4)
               {
                  HRLog.Write("Rejecting Mulligan Card " + item.GetEntity().GetName() + " because it cost is >= 4.");
                  HRMulligan.ToggleCard(item);
               }
               if (item.GetEntity().GetCardId() == "EX1_308" || item.GetEntity().GetCardId() == "EX1_622" || item.GetEntity().GetCardId() == "EX1_005")
               {
                   HRLog.Write("Rejecting Mulligan Card " + item.GetEntity().GetName() + " because it is soulfire or shadow word: death");
                   HRMulligan.ToggleCard(item);
               }
            }

            sf.setnewLoggFile();
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
                 HRLog.Write("dirty targeting...");
                 HREntity target = getEntityWithNumber(dirtytarget);
                 
                 dirtytarget = -1;
                 
                 return new HREngine.API.Actions.TargetAction(target);
             }
             if (HRChoice.IsChoiceActive())
             {
                 if (this.dirtychoice >= 1)
                 {
                     List<HREntity> choices = HRChoice.GetChoiceCards();
                     int choice=this.dirtychoice;
                     this.dirtychoice=-1;
                     string ccId = this.choiceCardId;
                     this.choiceCardId = "";
                     HREntity target= choices[choice-1];
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
                     int choice = r.Next(0,choices.Count);
                     Helpfunctions.Instance.logg("chooses a random card");
                     return new HREngine.API.Actions.ChoiceAction(choices[choice]);
                 }
             }
              
           
            //SafeHandleBattleLocalPlayerTurnHandler();


             sf.updateEverything(this);
            Action moveTodo = Ai.Instance.bestmove;
            if (moveTodo == null)
            {
                HRLog.Write("end turn");
                return null;
            }
            HRLog.Write("play action");
            moveTodo.print();
            if (moveTodo.cardplay)
            {
                HRCard cardtoplay = getCardWithNumber(moveTodo.cardEntitiy);
                if (moveTodo.enemytarget >= 0)
                {
                    HREntity target = getEntityWithNumber(moveTodo.enemyEntitiy);
                    HRLog.Write("play: " + cardtoplay.GetEntity().GetName() + " target: " + target.GetName());
                    Helpfunctions.Instance.logg("play: " + cardtoplay.GetEntity().GetName() + " target: " + target.GetName() + " choice: " + moveTodo.druidchoice);
                    if (moveTodo.druidchoice >= 1)
                    {
                        if(moveTodo.enemyEntitiy>=0) this.dirtytarget = moveTodo.enemyEntitiy;
                        this.dirtychoice = moveTodo.druidchoice; //1=leftcard, 2= rightcard
                        this.choiceCardId = moveTodo.handcard.card.CardID;

                    }
                    if (moveTodo.handcard.card.type == CardDB.cardtype.MOB)
                    {
                        return new HREngine.API.Actions.PlayCardAction(cardtoplay, target, moveTodo.owntarget + 1);
                    }
                    
                    return new HREngine.API.Actions.PlayCardAction(cardtoplay,target);

                }
                else
                {
                    HRLog.Write("play: " + cardtoplay.GetEntity().GetName() + " target nothing");
                    if (moveTodo.handcard.card.type == CardDB.cardtype.MOB)
                    {
                        return new HREngine.API.Actions.PlayCardAction(cardtoplay, null, moveTodo.owntarget + 1);
                    }
                    return new HREngine.API.Actions.PlayCardAction(cardtoplay);
                }
                
            }

            if (moveTodo.minionplay )
            {
                HREntity attacker = getEntityWithNumber(moveTodo.ownEntitiy);
                HREntity target = getEntityWithNumber(moveTodo.enemyEntitiy);
                HRLog.Write("minion attack: " + attacker.GetName() + " target: " + target.GetName());
                Helpfunctions.Instance.logg("minion attack: " + attacker.GetName() + " target: " + target.GetName());
                return new HREngine.API.Actions.AttackAction(attacker,target);

            }

            if (moveTodo.heroattack)
            {
                HREntity attacker = getEntityWithNumber(moveTodo.ownEntitiy);
                HREntity target = getEntityWithNumber(moveTodo.enemyEntitiy);
                this.dirtytarget = moveTodo.enemyEntitiy;
                //HRLog.Write("heroattack: attkr:" + moveTodo.ownEntitiy + " defender: " + moveTodo.enemyEntitiy);
                HRLog.Write("heroattack: " + attacker.GetName() + " target: " + target.GetName());
                Helpfunctions.Instance.logg("heroattack: " + attacker.GetName() + " target: " + target.GetName());
                if (HRPlayer.GetLocalPlayer().HasWeapon() )
                {
                    HRLog.Write("hero attack with weapon");
                    return new HREngine.API.Actions.AttackAction(HRPlayer.GetLocalPlayer().GetWeaponCard().GetEntity(), target);
                }
                HRLog.Write("hero attack without weapon");
                //HRLog.Write("attacker entity: " + HRPlayer.GetLocalPlayer().GetHero().GetEntityId());
                return new HREngine.API.Actions.AttackAction(HRPlayer.GetLocalPlayer().GetHero(), target);

            }

            if (moveTodo.useability)
            {
                HRCard cardtoplay = HRPlayer.GetLocalPlayer().GetHeroPower().GetCard();

                if (moveTodo.enemytarget >= 0)
                {
                    HREntity target = getEntityWithNumber(moveTodo.enemyEntitiy);
                    HRLog.Write("use ablitiy: " + cardtoplay.GetEntity().GetName() + " target " + target.GetName());
                    Helpfunctions.Instance.logg("use ablitiy: " + cardtoplay.GetEntity().GetName() + " target " + target.GetName());
                    return new HREngine.API.Actions.PlayCardAction(cardtoplay, target);

                }
                else
                {
                    HRLog.Write("use ablitiy: " + cardtoplay.GetEntity().GetName() + " target nothing");
                    Helpfunctions.Instance.logg("use ablitiy: " + cardtoplay.GetEntity().GetName() + " target nothing");
                    return new HREngine.API.Actions.PlayCardAction(cardtoplay);
                }
            }

         }
         catch (Exception Exception)
         {
            HRLog.Write(Exception.Message);
            HRLog.Write(Environment.StackTrace);
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

      public int getPlayfieldValue(Playfield p)
      {
          return evaluatePlayfield(p);
      }

      public int getRulesEditorPenality(string cardId, Playfield p)
      {
          
          return 0;
      }

      protected virtual int evaluatePlayfield(Playfield p)
      {
          return 0;
      }
   
   }
}