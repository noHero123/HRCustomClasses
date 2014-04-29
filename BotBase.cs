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




   public abstract class BotBase : API.IBot
   {
       
       private int dirtytarget=-1;
       Silverfish sf;

      public BotBase()
      {
          OnBattleStateUpdate = HandleOnBattleStateUpdate;
          OnMulliganStateUpdate = HandleBattleMulliganPhase;
          this.sf = new Silverfish();
          Ai.Instance.simulatorTester(this);
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
                 HRLog.Write("dirty targeting...");
                 HREntity target = getEntityWithNumber(dirtytarget);
                 dirtytarget = -1;
                 return new HREngine.API.Actions.TargetAction(target);
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
                    Helpfunctions.Instance.logg("play: " + cardtoplay.GetEntity().GetName() + " target: " + target.GetName());
                    return new HREngine.API.Actions.PlayCardAction(cardtoplay,target);

                }
                else
                {
                    HRLog.Write("play: " + cardtoplay.GetEntity().GetName() + " target nothing");
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
                HRLog.Write("attacker entity: " + HRPlayer.GetLocalPlayer().GetHero().GetEntityId());
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

      protected virtual int evaluatePlayfield(Playfield p)
      {
          return 0;
      }
   
   }
}