using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HREngine.Bots
{

    public class BehaviorControl : Behavior
    {
        PenalityManager penman = PenalityManager.Instance;

        public override int getPlayfieldValue(Playfield p)
        {
            if (p.value >= -2000000) return p.value;
            int retval = 0;
            int hpboarder = 10;
            if (p.ownHeroName == HeroEnum.warlock && p.enemyHeroName != HeroEnum.mage) hpboarder = 6;
            int aggroboarder = 11;

            retval -= p.evaluatePenality;
            retval += p.owncards.Count * 1;

            retval += p.ownMaxMana;
            retval -= p.enemyMaxMana;

            retval += p.ownMinions.Count * 10;

            retval += p.ownMaxMana * 20 - p.enemyMaxMana * 20;

            if (p.ownHeroHp + p.ownHeroDefence > hpboarder)
            {
                retval += p.ownHeroHp + p.ownHeroDefence;
            }
            else
            {
                retval -= (hpboarder + 1 - p.ownHeroHp - p.ownHeroDefence) * (hpboarder + 1 - p.ownHeroHp - p.ownHeroDefence);
            }


            if (p.enemyHeroHp + p.enemyHeroDefence > aggroboarder)
            {
                retval += -p.enemyHeroHp - p.enemyHeroDefence;
            }
            else
            {
                retval += (int)Math.Pow((aggroboarder + 1 - p.enemyHeroHp - p.enemyHeroDefence), 2);
            }

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
            retval -= p.enemycarddraw * 15;

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
                if (m.taunt && m.handcard.card.name == CardDB.cardName.frog) owntaunt++;
                if (m.Angr > 1 || m.Hp > 1) ownMinionsCount++;
                if (m.handcard.card.hasEffect) retval += 1;
                if (m.handcard.card.name == CardDB.cardName.silverhandrecruit && m.Angr == 1 && m.Hp == 1) retval -= 5;
                if (m.handcard.card.name == CardDB.cardName.direwolfalpha || m.handcard.card.name == CardDB.cardName.flametonguetotem || m.handcard.card.name == CardDB.cardName.stormwindchampion || m.handcard.card.name == CardDB.cardName.raidleader) retval += 10;
                if (m.handcard.card.name == CardDB.cardName.bloodmagethalnos) retval += 10;
            }

            /*if (p.enemyMinions.Count >= 0)
            {
                int anz = p.enemyMinions.Count;
                if (owntaunt == 0) retval -= 10 * anz;
                retval += owntaunt * 10 - 11 * anz;
            }*/

            int playmobs = 0;
            bool useAbili = false;
            bool usecoin = false;
            foreach (Action a in p.playactions)
            {
                if (a.heroattack && p.enemyHeroHp <= p.attackFaceHP) retval++;
                if (a.useability) useAbili = true;
                if (p.ownHeroName == HeroEnum.warrior && a.heroattack && useAbili) retval -= 1; 
                if (a.useability && a.handcard.card.name == CardDB.cardName.lesserheal && ((a.enemytarget >= 10 && a.enemytarget <= 20) || a.enemytarget == 200)) retval -= 5;
                if (!a.cardplay) continue;
                if ((a.handcard.card.name == CardDB.cardName.thecoin || a.handcard.card.name == CardDB.cardName.innervate)) usecoin = true;
                if (a.handcard.card.type == CardDB.cardtype.MOB) playmobs++;
                //if (a.handcard.card.name == "arcanemissiles" && a.numEnemysBeforePlayed == 0) retval -= 10; // arkane missles on enemy hero is bad :D

                if (a.handcard.card.name == CardDB.cardName.flamestrike && a.numEnemysBeforePlayed <= 2) retval -= 20;
                //save spell for all classes: (except for rouge if he has no combo)
                if (p.ownHeroName != HeroEnum.thief && a.handcard.card.type == CardDB.cardtype.SPELL && (a.numEnemysBeforePlayed == 0 || a.enemytarget == 200) && a.handcard.card.name != CardDB.cardName.shieldblock) retval -= 11;
                if (p.ownHeroName == HeroEnum.thief && a.handcard.card.type == CardDB.cardtype.SPELL && (a.enemytarget == 200)) retval -= 11;
            }
            if (usecoin && useAbili && p.ownMaxMana <= 2) retval -= 40;
            //if (usecoin && p.mana >= 1) retval -= 20;

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
                retval -= this.getEnemyMinionValue(m, p);
            }

            retval -= p.enemySecretCount;
            retval -= p.lostDamage;//damage which was to high (like killing a 2/1 with an 3/3 -> => lostdamage =2
            retval -= p.lostWeaponDamage;
            //if (p.ownMinions.Count == 0) retval -= 20;
            //if (p.enemyMinions.Count == 0) retval += 20;
            if (p.enemyHeroHp <= 0) retval = 10000;
            //soulfire etc
            int deletecardsAtLast = 0;
            foreach (Action a in p.playactions)
            {
                if (!a.cardplay) continue;
                if (a.handcard.card.name == CardDB.cardName.soulfire || a.handcard.card.name == CardDB.cardName.doomguard || a.handcard.card.name == CardDB.cardName.succubus) deletecardsAtLast = 1;
                if (deletecardsAtLast == 1 && !(a.handcard.card.name == CardDB.cardName.soulfire || a.handcard.card.name == CardDB.cardName.doomguard || a.handcard.card.name == CardDB.cardName.succubus)) retval -= 20;
            }
            if (p.enemyHeroHp >= 1 && p.guessingHeroHP <= 0) retval -= 1000;
            if (p.ownHeroHp <= 0) retval = -10000;

            p.value = retval;
            return retval;
        }



        public override int getEnemyMinionValue(Minion m, Playfield p)
        {
            int retval = 10;
            retval += m.Hp * 2;
            if (!m.frozen && !(m.handcard.card.name == CardDB.cardName.ancientwatcher && !m.silenced))
            {
                retval += m.Angr * 2;
                if (m.windfury) retval += m.Angr * 2;
                if (m.Angr >= 4) retval += 10;
                if (m.Angr >= 7) retval += 50;
            }

            if (m.Angr == 0) retval -= 7;

            retval += m.handcard.card.rarity;
            if (m.taunt) retval += 5;
            if (m.divineshild) retval += m.Angr;
            if (m.divineshild && m.taunt) retval += 5;
            if (m.stealth) retval += 1;

            if (m.poisonous) retval += 4;

            if (penman.priorityTargets.ContainsKey(m.name) && !m.silenced) retval += penman.priorityTargets[m.name];
            return retval;
        }
   
    }

}
