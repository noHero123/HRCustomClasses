using HREngine.API;
using HREngine.API.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HREngine.Bots
{
   public class Rushi : Bot
   {
       PenalityManager penman = PenalityManager.Instance;

      protected override HRCard GetMinionByPriority(HRCard lastMinion)
      {
         HREntity result = null;
         if (HRPlayer.GetLocalPlayer().GetNumEnemyMinionsInPlay() <
            HRPlayer.GetLocalPlayer().GetNumFriendlyMinionsInPlay() ||
            HRPlayer.GetLocalPlayer().GetNumEnemyMinionsInPlay() < 4)
         {
            result = HRBattle.GetNextMinionByPriority(MinionPriority.Hero);
         }
         else
            result = HRBattle.GetNextMinionByPriority(MinionPriority.LowestHealth);

         if (result != null && (lastMinion == null || lastMinion != null && lastMinion.GetEntity().GetCardId() != result.GetCardId()))
            return result.GetCard();

         return null;
      }

      protected override int evaluatePlayfield(Playfield p)
      {
          if (p.value >= -2000000) return p.value;
          int retval = 0;
          retval -= p.evaluatePenality;
          retval += p.owncards.Count * 1;

          retval += p.ownHeroHp + p.ownHeroDefence;
          retval += -(p.enemyHeroHp + p.enemyHeroDefence);

          retval += p.ownMaxMana*10 - p.enemyMaxMana*10; 

          retval += p.ownWeaponAttack;// +ownWeaponDurability;
          if (!p.enemyHeroFrozen)
          {
              retval -= p.enemyWeaponDurability * p.enemyWeaponAttack;
          }
          else
          {
              if (p.enemyHeroName != HeroEnum.mage && p.enemyHeroName != HeroEnum.priest)
              {
                  retval += 11;
              }
          }

          retval += p.owncarddraw * 5;
          retval -= p.enemycarddraw * 5;

          retval += p.ownMaxMana;

          bool useAbili = false;
          bool usecoin = false;
          foreach (Action a in p.playactions)
          {
              if (a.useability) useAbili = true;
              if (a.useability && a.handcard.card.specialMin == CardDB.specialMinions.lesserheal && ((a.enemytarget >= 10 && a.enemytarget <= 20) || a.enemytarget == 200)) retval -= 5;
              if (!a.cardplay) continue;
              if ((a.handcard.card.specialMin == CardDB.specialMinions.thecoin || a.handcard.card.specialMin == CardDB.specialMinions.innervate)) usecoin = true;
              if (a.handcard.card.specialMin == CardDB.specialMinions.flamestrike && a.numEnemysBeforePlayed <= 2) retval -= 20;
          }
          if (usecoin && useAbili && p.ownMaxMana <= 2) retval -= 20;

          foreach (Minion m in p.ownMinions)
          {
              retval += m.Hp * 1;
              retval += m.Angr * 2;
              retval += m.handcard.card.rarity;
              if (m.windfury) retval += m.Angr;
              if (m.taunt) retval += 1;
              if (m.handcard.card.specialMin == CardDB.specialMinions.silverhandrecruit && m.Angr == 1 && m.Hp == 1) retval -= 5;
          }

          foreach (Minion m in p.enemyMinions)
          {
              if (p.enemyMinions.Count >= 4 || m.taunt || (penman.priorityTargets.ContainsKey(m.name) && !m.silenced) || m.Angr >= 5)
              {
                  retval -= m.Hp;
                  retval -= m.Angr * 2;
                  if (m.windfury) retval -= m.Angr;
                  if (m.taunt) retval -= 5;
                  if (m.divineshild) retval -= m.Angr;
                  if (m.frozen) retval += 1; // because its bad for enemy :D
                  if (m.poisonous) retval -= 4;
                  retval -= m.handcard.card.rarity;
              }


              if (penman.priorityTargets.ContainsKey(m.name) && !m.silenced) retval -= penman.priorityTargets[m.name];
              if (m.Angr >= 4) retval -= 20;
              if (m.Angr >= 7) retval -= 50;
          }

          retval -= p.enemySecretCount;
          retval -= p.lostDamage;//damage which was to high (like killing a 2/1 with an 3/3 -> => lostdamage =2
          retval -= p.lostWeaponDamage;
          if (p.ownMinions.Count == 0) retval -= 20;
          if (p.enemyMinions.Count >= 4) retval -= 200;
          if (p.enemyHeroHp <= 0) retval = 10000;
          //soulfire etc
          int deletecardsAtLast = 0;
          foreach (Action a in p.playactions)
          {
              if (!a.cardplay) continue;
              if (a.handcard.card.specialMin == CardDB.specialMinions.soulfire || a.handcard.card.specialMin == CardDB.specialMinions.doomguard || a.handcard.card.specialMin == CardDB.specialMinions.succubus) deletecardsAtLast = 1;
              if (deletecardsAtLast == 1 && !(a.handcard.card.specialMin == CardDB.specialMinions.soulfire || a.handcard.card.specialMin == CardDB.specialMinions.doomguard || a.handcard.card.specialMin == CardDB.specialMinions.succubus)) retval -= 20;
          }
          if (p.enemyHeroHp >= 1 && p.ownHeroHp + p.ownHeroDefence - p.guessingHeroDamage <= 0) retval -= 1000;
          if (p.ownHeroHp <= 0) retval = -10000;

          p.value = retval;
          return retval;
      }
   }
}
