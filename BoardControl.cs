using HREngine.API;
using HREngine.API.Utilities;
using HREngine.Bots;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HREngine.Bots
{
   public class BoardControli : Bot
   {
      protected override API.HRCard GetMinionByPriority(HRCard lastMinion)
      {
         var result = HRBattle.GetNextMinionByPriority(MinionPriority.LowestHealth);
         if (result != null && (lastMinion == null || lastMinion != null && lastMinion.GetEntity().GetCardId() != result.GetCardId()))
            return result.GetCard();

         return null;
      }
      
       protected override int evaluatePlayfield(Playfield p)
      {
          int retval = 0;
          retval -= p.evaluatePenality;
          retval += p.owncards.Count * 1;

          retval += p.ownMaxMana;
          retval -= p.enemyMaxMana;

          retval += p.ownMinions.Count * 10;
          retval -= p.enemyMinions.Count * 10;

          retval += p.ownHeroHp + p.ownHeroDefence;
          retval += -p.enemyHeroHp - p.enemyHeroDefence;

          retval += p.ownWeaponAttack; ;// +ownWeaponDurability;
          if (!p.enemyHeroFrozen)
          {
              retval -= p.enemyWeaponDurability*p.enemyWeaponAttack;
          }
          else
          {
              if (p.enemyHeroName != "mage" && p.enemyHeroName != "priest")
              {
                  retval += 11;
              }
          }

          retval += p.owncarddraw * 5;
          retval -= p.enemycarddraw * 5;

          retval += p.ownMaxMana;

          if (p.enemyMinions.Count >= 0)
          {
              int anz = p.enemyMinions.Count;
              int owntaunt = p.ownMinions.FindAll(x => x.taunt == true).Count;
              int froggs = p.ownMinions.FindAll(x => x.name == "frog").Count;
              owntaunt -= froggs;
              if (owntaunt == 0) retval -= 10 * anz;
              retval += owntaunt * 10 - 11 * anz;
          }

          foreach (Action a in p.playactions)
          {
              if (a.useability && a.card.name == "lesserheal" && ((a.enemytarget >= 10 && a.enemytarget <= 20) || a.enemytarget == 200)) retval -= 5;
              if (!a.cardplay) continue;
              if (a.card.name == "arcanemissiles" && a.numEnemysBeforePlayed == 0) retval -= 10; // arkane missles on enemy hero is bad :D
              if (a.card.name == "execute") retval -= 18; // a enemy minion make -10 for only being there, so + 10 for being eliminated 
              if (a.card.name == "flamestrike" && a.numEnemysBeforePlayed <= 2) retval -= 20;
              //save spell for mage:
              if (p.ownHeroName == "mage" && a.card.type == CardDB.cardtype.SPELL && (a.numEnemysBeforePlayed == 0 || a.enemytarget == 200)) retval -= 11;
          }


          foreach (Minion m in p.ownMinions)
          {
              retval += m.Hp * 1;
              retval += m.Angr * 2;
              retval += m.card.rarity;
              if (m.windfury) retval += m.Angr;
              //if (m.divineshild) retval += 1;
              if (m.stealth) retval += 1;
              //if (m.poisonous) retval += 1;
              if (m.divineshild && m.taunt) retval += 4;
          }

          foreach (Minion m in p.enemyMinions)
          {
              retval -= m.Hp*2;
              if (!m.frozen)
              {
                  retval -= m.Angr * 2;
                  if (m.windfury) retval -= m.Angr;
              }
              retval -= m.card.rarity;
              if (m.taunt) retval -= 5;
              if (m.divineshild) retval -= 1;
              if (m.stealth) retval -= 1;
              
              if (m.poisonous) retval -= 4;
              if (m.name == "prophetvelen") retval -= 5;
              if (m.name == "archmageantonidas") retval -= 5;
              if (m.name == "flametonguetotem") retval -= 5;
              if (m.name == "raidleader") retval -= 5;
              if (m.name == "grimscaleoracle") retval -= 5;
              if (m.name == "direwolfalpha") retval -= 2;
              if (m.name == "murlocwarleader") retval -= 5;
              if (m.name == "southseacaptain") retval -= 5;
              if (m.name == "stormwindchampion") retval -= 10;
              if (m.name == "timberwolf") retval -= 5;
              if (m.name == "leokk") retval -= 5;
              if (m.name == "northshirecleric") retval -= 5;
              if (m.name == "sorcerersapprentice") retval -= 3;
              if (m.name == "summoningportal") retval -= 5;
              if (m.name == "pint-sizedsummoner") retval -= 3;
              if (m.name == "scavenginghyena") retval -= 20;
              if (m.Angr >= 4) retval -= 20;
              if (m.Angr >= 7) retval -= 50;
          }

          retval -= p.enemySecretCount;
          retval -= p.lostDamage;//damage which was to high (like killing a 2/1 with an 3/3 -> => lostdamage =2
          retval -= p.lostWeaponDamage;
          if (p.ownMinions.Count == 0) retval -= 20;
          if (p.enemyMinions.Count == 0) retval += 20;
          if (p.enemyHeroHp <= 0) retval = 10000;
          if (p.enemyHeroHp >=1 && p.ownHeroHp + p.ownHeroDefence - p.guessingHeroDamage <= 0) retval -= 1000;
          if (p.ownHeroHp <= 0) retval = -10000;

          p.value = retval;
          return retval;
      }
   
   }
}
