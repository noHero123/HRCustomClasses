using HREngine.API;
using HREngine.API.Utilities;
using HREngine.Bots;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HREngine.Bots
{
   public class BoardControli : BotBase
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
          retval += p.owncards.Count * 1;

          retval += p.ownMinions.Count * 10;
          retval -= p.enemyMinions.Count * 10;

          retval += p.ownHeroHp + p.ownHeroDefence;
          retval += -p.enemyHeroHp - p.enemyHeroDefence;

          retval += p.ownWeaponAttack; ;// +ownWeaponDurability;
          retval -= p.enemyWeaponDurability;

          retval += p.owncarddraw * 5;
          retval -= p.enemycarddraw * 5;

          retval += p.ownMaxMana;

          if (p.enemyMinions.Count >= 0)
          {
              int anz = p.enemyMinions.Count;
              int owntaunt = p.ownMinions.FindAll(x => x.taunt == true).Count;
              int froggs = p.ownMinions.FindAll(x => x.name == "frosch").Count;
              owntaunt -= froggs;
              if (owntaunt == 0) retval -= 10 * anz;
              retval += owntaunt * 10 - 11 * anz;
          }

          foreach (Minion m in p.ownMinions)
          {
              retval += m.Hp * 1;
              retval += m.Angr * 2;
              
              if (m.windfury) retval += m.Angr;
          }

          foreach (Minion m in p.enemyMinions)
          {

              retval -= m.Hp;
              retval -= m.Angr * 2;
              /*if (m.Angr >= m.maxHp + 1)
              {
                  //is a tanky minion
                  retval -= m.Hp;
              }*/

              if (m.windfury) retval -= m.Angr;
              if (m.taunt) retval -= 5;
              if (m.name == "schlachtzugsleiter") retval -= 5;
              if (m.name == "grimmschuppenorakel") retval -= 5;
              if (m.name == "terrorwolfalpha") retval -= 2;
              if (m.name == "murlocanfuehrer") retval -= 5;
              if (m.name == "suedmeerkapitaen") retval -= 5;
              if (m.name == "championvonsturmwind") retval -= 10;
              if (m.name == "waldwolf") retval -= 5;
              if (m.name == "leokk") retval -= 5;
              if (m.name == "klerikerinvonnordhain") retval -= 5;
              if (m.name == "zauberlehrling") retval -= 3;
              if (m.name == "winzigebeschwoererin") retval -= 3;
              if (m.Angr >= 4) retval -= 20;
              if (m.Angr >= 7) retval -= 50;
          }

          retval -= p.lostDamage;//damage which was to high (like killing a 2/1 with an 3/3 -> => lostdamage =2
          retval -= p.lostWeaponDamage;
          if (p.ownMinions.Count == 0) retval -= 20;
          if (p.enemyMinions.Count == 0) retval += 20;
          if (p.enemyHeroHp <= 0) retval = 10000;
          if (p.ownHeroHp <= 0) retval = -10000;

          p.value = retval;
          return retval;
      }
   
   }
}
