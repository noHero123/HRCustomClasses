using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HREngine.Bots
{

  public class PenalityManager
    {
        //todo acolyteofpain
        //todo better aoe-penality

        public Dictionary<string, int> priorityDatabase = new Dictionary<string, int>();
        Dictionary<string, int> HealTargetDatabase= new Dictionary<string,int>();
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
        }

        public int getAttackWithMininonPenality(Minion m, Playfield p, int target, bool lethal)
        {
            int pen = 0;
            pen = getAttackSecretPenality(m,p,target);
            if (!lethal && m.name == "bloodimp") pen = 50;
            return pen;
        }

        public int getAttackWithHeroPenality(int target, Playfield p)
        {
            int retval = 0;

            //no penality, but a bonus, if he has weapon on hand!
            if (p.ownWeaponDurability == 1)
            {
                bool hasweapon = false;
                foreach (Handmanager.Handcard c in p.owncards)
                {
                    if (c.card.type == CardDB.cardtype.WEAPON) hasweapon = true;
                }
                if (hasweapon) retval = -p.ownWeaponAttack - 1; // so he doesnt "lose" the weapon in evaluation :D
            }
            return retval;
        }

        public int getPlayCardPenality(CardDB.Card card, int target, Playfield p, int choice, bool lethal)
        {
            int retval = 0;
            string name = card.name;
            //there is no reason to buff HP of minon (because it is not healed)

            int abuff = getAttackBuffPenality(card, target, p, choice);
            int tbuff = getTauntBuffPenality(name, target, p, choice);
            if (name == "markofthewild" && ( (abuff >= 500 && tbuff == 0) || (abuff == 0 && tbuff >= 500)) )
            {
                retval = 0;
            }
            else
            {
                retval += abuff + tbuff;
            }
            retval += getHPBuffPenality(card, target, p, choice);
            retval += getSilencePenality( name,  target,  p,  choice, lethal);
            retval += getDamagePenality( name,  target,  p,  choice);
            retval += getHealPenality( name,  target,  p,  choice, lethal);
            retval += getCardDrawPenality( name,  target,  p,  choice);
            retval += getCardDrawofEffectMinions( card,  p);
            retval += getCardDiscardPenality( name,  p);
            retval += getDestroyOwnPenality(name, target, p);
            
            retval += getDestroyPenality( name,  target,  p);
            retval += getSpecialCardComboPenalitys( card,  target,  p, lethal);
            retval += playSecretPenality( card,  p);
            retval += getPlayCardSecretPenality(card, p);

            return retval;
        }

        private int getAttackBuffPenality(CardDB.Card card, int target, Playfield p, int choice)
        {
            string name = card.name;
            int pen = 0;
            //buff enemy?
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

                pen = 500;
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
                bool enemyhasTaunts=false;
                foreach (Minion mnn in p.enemyMinions)
                {
                    if (mnn.taunt)
                    {
                        enemyhasTaunts = true;
                        break;
                    }
                }
                Minion m= p.enemyMinions[target-10];
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
            if (name == "earthshock") return 0;//earthshock is handled in damage stuff
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

                    if ( !m.silenced && (m.name == "ancientwatcher" || m.name == "ragnarosthefirelord") )
                    {
                        return 500;
                    }

                    if (lethal)
                    {
                        //during lethal we only silence taunt, or if its a mob (owl/spellbreaker) + we can give him charge
                        if (m.taunt || (name == "ironbeakowl" && (p.ownMinions.Find(x => x.name == "tundrarhino") != null || p.ownMinions.Find(x => x.name == "warsongcommander") != null || p.owncards.Find(x => x.card.name == "charge") != null)) || (name == "spellbreaker" && p.owncards.Find(x => x.card.name == "charge") != null)) return 0;
                       
                        return 500;
                    }

                    if (priorityDatabase.ContainsKey(m.name) && !m.silenced)
                    {
                        return -10;
                    }
                    //silence nothing
                    if ((m.Angr < m.handcard.card.Attack || m.maxHp < m.handcard.card.Health) || !(m.taunt || m.windfury || m.divineshild || m.enchantments.Count >= 1))
                    {
                        return 30;
                    }

                    

                    pen = 0;
                }
            }

            return pen;

        }

        private int getDamagePenality(string name, int target, Playfield p, int choice)
        {
            int pen = 0;

            if (name == "shieldslam" && p.ownHeroDefence == 0) return 500;
            if (name == "savagery" && p.ownheroAngr == 0) return 500;
            if (name == "keeperofthegrove" && choice != 1) return 0; // look at silence penality

            if (this.DamageAllDatabase.ContainsKey(name) || (p.auchenaiseelenpriesterin && HealAllDatabase.ContainsKey(name))) // aoe penality
            {
                foreach (Handmanager.Handcard hc in p.owncards)
                {
                    if (hc.card.name == "execute") return 0;
                }

                if( p.enemyMinions.Count <=1 || p.enemyMinions.Count +1 <= p.ownMinions.Count || p.ownMinions.Count >=3)
                {
                    return 30;
                }
            }

            if (this.DamageAllEnemysDatabase.ContainsKey(name)) // aoe penality
            {
                foreach (Handmanager.Handcard hc in p.owncards)
                {
                    if (hc.card.name == "execute") return 0;
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
                if (DamageTargetSpecialDatabase.ContainsKey(name) )
                {
                    int dmg  = DamageTargetSpecialDatabase[name];
                    Minion m = p.ownMinions[target];

                    if (name == "demonfire" && (TAG_RACE)m.handcard.card.race == TAG_RACE.DEMON) return 0;
                    if (name == "earthshock" && m.Hp >= 2 )
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
                if ((target == 100) && p.ownHeroHp == 30) return 500;
                if ((target == 100) && p.ownHeroHp + heal > 30) pen = p.ownHeroHp + heal - 30;
                Minion m = new Minion();

                if (target >= 0 && target < 10)
                {
                    m = p.ownMinions[target];
                    int wasted=0;
                    if (m.Hp == m.maxHp) return 500;
                    if (m.Hp + heal > m.maxHp) wasted = m.Hp + heal - m.maxHp;
                    pen=wasted;
                    if (m.taunt && wasted <= 2 && m.Hp < m.maxHp) pen -= 5; // if we heal a taunt, its good :D
                }

                if (target >= 10 && target < 20)
                {
                    m = p.enemyMinions[target-10];
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
                        if (hc.card.name == "slam") return pen;
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
            if (name == "harrisonjones") carddraw = p.enemyWeaponDurability;
            if (name == "divinefavor") carddraw =  p.enemyAnzCards + p.enemycarddraw - (p.owncards.Count);
            if (name == "battlerage")
            {
                carddraw = 0;
                foreach (Minion mnn in p.ownMinions)
                {
                    if (mnn.wounded) carddraw++;
                }
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
                    m = p.enemyMinions[target-10];
                }
                carddraw=0;
                if (m.Hp >= 3) carddraw = 1;
                if (carddraw == 0) return 2;
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
                if (carddraw == 0) return 2;
            }

            if (p.owncards.Count >= 5) return 0;
            pen = -carddraw + p.ownMaxMana - p.mana;
            return pen;
        }

        private int getCardDrawofEffectMinions(CardDB.Card card, Playfield p)
        {
            int pen = 0;
            int carddraw=0;
            if (card.type == CardDB.cardtype.SPELL)
            {
                foreach (Minion mnn in p.ownMinions)
                {
                    if(mnn.name == "gadgetzanauctioneer" ) carddraw++;
                }
            }

            if (card.type == CardDB.cardtype.MOB && (TAG_RACE)card.race == TAG_RACE.PET)
            {
                foreach (Minion mnn in p.ownMinions)
                {
                    if (mnn.name == "starvingbuzzard") carddraw++;
                }
            }

            if (carddraw==0) return 0;

            if (p.owncards.Count >= 5) return 0;
            pen = -carddraw + p.ownMaxMana - p.mana;

            return pen;
        }

        private int getCardDiscardPenality(string name, Playfield p)
        {
            if (p.owncards.Count <= 1) return 0;
            int pen = 0;
            if (this.cardDiscardDatabase.ContainsKey(name))
            {
                int newmana=p.mana-cardDiscardDatabase[name];
                bool canplayanothercard = false;
                foreach (Handmanager.Handcard hc in p.owncards)
                {
                    if (this.cardDiscardDatabase.ContainsKey(hc.card.name)) continue;
                    if (hc.card.getManaCost(p,hc.manacost) <= newmana)
                    {
                        canplayanothercard = true;
                    }
                }
                if (canplayanothercard) pen += 10;

            }

            return pen;
        }

        private int getDestroyOwnPenality(string name, int target, Playfield p)
        {
            if (!this.destroyOwnDatabase.ContainsKey(name)) return 0;
            int pen = 0;
            if ((name == "brawl" || name == "deathwing" || name == "twistingnether") && p.mobsplayedThisTurn >= 1) return 500;

            if (target >= 0 && target <= 9)
            {
                // dont destroy owns ;_; (except mins with deathrattle effects)

                Minion m = p.ownMinions[target];
                if (m.handcard.card.deathrattle) return 10;

                return 500;
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

                Minion m = p.enemyMinions[target-10];

                if (m.Angr <= 4)
                {
                    pen += 25; // so we dont destroy cheap ones :D
                }

            }

            return pen;
        }

        private int getSpecialCardComboPenalitys(CardDB.Card card, int target, Playfield p, bool lethal)
        {
            string name = card.name;

            if (lethal && card.type == CardDB.cardtype.MOB)
            {
                if (!(name == "nightblade" || card.Charge || this.silenceDatabase.ContainsKey(name)|| ((TAG_RACE)card.race == TAG_RACE.PET && p.ownMinions.Find(x => x.name == "tundrarhino") != null) || (p.ownMinions.Find(x => x.name == "warsongcommander") != null && card.Attack <= 3) || p.owncards.Find(x => x.card.name == "charge") != null))
                {
                    return 500;
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
                m = p.enemyMinions[target-10];
            }

            if ( name == "divinespirit")
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
                            if (p.owncards.Find(x => x.card.specialMin == CardDB.specialMinions.biggamehunter) != null && p.owncards.Find(x => x.card.name == "innerfire") != null && (m.Hp >= 4 || (p.owncards.Find(x => x.card.name == "divinespirit")!=null &&  m.Hp >=2)))
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

            if (name == "facelessmanipulator" )
            {
                if (target == -1 ) 
                {
                    return 21;
                }
                if (m.Angr>=5 || m.handcard.card.cost>=5)
                {
                    return 0;
                }
                return 20;
            }

            if (name == "knifejuggler")
            {
                if (p.mobsplayedThisTurn>=1)
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
                    if (frog.Angr >= 4 && frog.Hp >=4) return 0;
                    return 30;
                }
               
            }

            if ((card.specialMin == CardDB.specialMinions.biggamehunter) && target == -1)
            {
                return 19;
            }

            if ((name == "defenderofargus" || name == "sunfuryprotector") && p.ownMinions.Count == 0)
            {
                return 10;
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
                if (p.enemyMinions.Count <= 2 || (p.ownMinions.Count - p.enemyMinions.Count  >= 1))
                {
                    return 20;
                }
            }

            if (name == "bloodsailraider" && p.ownWeaponDurability==0)
            {
                //if you have bloodsailraider and no weapon equiped, but own a weapon:
                foreach (Handmanager.Handcard hc in p.owncards)
                {
                    if (hc.card.type == CardDB.cardtype.WEAPON) return 10;
                }
            }

            if (name == "theblackknight")
            {

                foreach (Minion mnn in p.enemyMinions)
                {
                    if (mnn.taunt && (m.Angr >= 3 || m.Hp >= 3)) return 0;
                }
                return 10;
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
            if ((name == "aldorpeacekeeper" || name == "humility" ) && target >= 0 && target <= 19)
            {
                if (target >= 0 && target <= 9) pen = 500; // dont use on own minions
                if (target >= 10 && target <= 19 && p.enemyMinions[target - 10].Angr <= 3) // only use on strong minions
                {
                    pen = 30;
                }
                if (m.name == "lightspawn") pen = 500;
            }

            if (name == "shatteredsuncleric" && target == -1) {pen = 10;}
            if (name == "argentprotector" && target == -1){pen = 10;}

            if (name == "defiasringleader" && p.cardsPlayedThisTurn == 0)
            {pen = 10;}
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
                {ready++;}
            }
                if (ready == 0)
                {pen = 5;}
            }
            if (name == "abusivesergeant")
            {
                int ready = 0;
                foreach (Minion min in p.ownMinions)
                {
                    if (min.Ready)
                    {ready++;}
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
                    foreach(Minion mins in p.ownMinions)
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
                if (mnn.numAttacksThisTurn >= 1) attackedbefore ++;
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

            if (p.enemyHeroName == HeroEnum.mage )
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
                    if ((!isOwnLowestInHand(m, p)&& p.mobsplayedThisTurn == 0) || (p.mobsplayedThisTurn ==0 && attackedbefore>=1) ) pen += 10;
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
                    if ((!isOwnLowestInHand(m, p) && p.mobsplayedThisTurn == 0 )|| attackedbefore == 0) pen += 10;
                }


            }

            

            return pen;
        }

        private int getAttackSecretPenality(Minion m , Playfield p, int target)
        {
            if (p.enemySecretCount == 0)
            {
                return 0;
            }

            int pen = 0;

            int attackedbefore = 0;

            foreach (Minion mnn in p.ownMinions)
            {
                if (mnn.numAttacksThisTurn >= 1) attackedbefore ++;
            }

            if (p.enemyHeroName == HeroEnum.hunter)
            {
                bool islow = isOwnLowest(m, p);
                if (attackedbefore == 0 && islow) pen -= 20;
                if (attackedbefore == 0 && !islow) pen += 10;

                if (target == 200 && p.enemyMinions.Count >=1)
                {
                    //penality if we doestn attacked before
                    if(hasMinionsWithLowHeal(p)) pen += 10; //penality if we doestn attacked minions before
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
                if (target == 200 && islow && p.mobsplayedThisTurn>=1)
                {
                    pen -= 20;
                }
                
            }

            if (p.enemyHeroName == HeroEnum.pala)
            {

                bool islow = isOwnLowest(m, p);

                if (target >= 10 && target <= 20  && attackedbefore==0)
                {
                    Minion enem = p.enemyMinions[target - 10];
                    if (!isEnemyLowest(enem, p) || m.Hp <= 2) pen += 5;
                }

                if (target == 200 && !islow)
                {
                    pen += 5;
                }

                if (target == 200 && p.enemyMinions.Count >=1 && attackedbefore == 0)
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
            List<targett> litt= p.getAttackTargets();
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
            //HealTargetDatabase.Add("divinespirit", 2);
        }

        private void setupDamageDatabase()
        {

            DamageHeroDatabase.Add("headcrack", 2);

            DamageAllDatabase.Add("abomination",2);
            DamageAllDatabase.Add("dreadinfernal", 1);
            DamageAllDatabase.Add("hellfire", 3);
            DamageAllDatabase.Add("whirlwind", 1);
            DamageAllDatabase.Add("yseraawakens", 5);

            DamageAllEnemysDatabase.Add("arcaneexplosion",1);
            DamageAllEnemysDatabase.Add("consecration", 1);
            DamageAllEnemysDatabase.Add("fanofknives", 1);
            DamageAllEnemysDatabase.Add("flamestrike", 4);
            DamageAllEnemysDatabase.Add("holynova", 2);
            DamageAllEnemysDatabase.Add("lightningstorm", 2);
            DamageAllEnemysDatabase.Add("stomp", 1);
            DamageAllEnemysDatabase.Add("madbomber", 1);
            DamageAllEnemysDatabase.Add("swipe", 4);//1 to others
            
            DamageRandomDatabase.Add("arcanemissiles",1);
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
            this.priorityDatabase.Add("raidleader", 5 );
            this.priorityDatabase.Add("grimscaleoracle", 5);
            this.priorityDatabase.Add("direwolfalpha", 6 );
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
            heroAttackBuffDatabase.Add("bite",4);
            heroAttackBuffDatabase.Add("claw",2);
            heroAttackBuffDatabase.Add("heroicstrike",2);

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

            this.healthBuffDatabase.Add("ancientofwar",5);//choice2
            this.healthBuffDatabase.Add("bananas",1);
            this.healthBuffDatabase.Add("blessingofkings",4);
            this.healthBuffDatabase.Add("markofnature",4);//choice2
            this.healthBuffDatabase.Add("markofthewild",2);
            this.healthBuffDatabase.Add("nightmare",5);
            this.healthBuffDatabase.Add("powerwordshield",2);
            this.healthBuffDatabase.Add("rampage",3);
            this.healthBuffDatabase.Add("rooted",5);

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
            cardDiscardDatabase.Add("doomguard",5);
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
            returnHandDatabase.Add("ancientbrewmaster",0);
            returnHandDatabase.Add("dream",0);
            returnHandDatabase.Add("kidnapper",0);//if combo
            returnHandDatabase.Add("shadowstep",0);
            returnHandDatabase.Add("vanish",0);
            returnHandDatabase.Add("youthfulbrewmaster",0);
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
            this.specialMinions.Add("venturecomercenary", 0);
            this.specialMinions.Add("violentteacher", 0);
            this.specialMinions.Add("warsongcommander", 0);
            this.specialMinions.Add("waterelemental", 0);
        }

        private void setupEnemyTargetPriority()
        {
            priorityTargets.Add("angrychicken", 10);
            priorityTargets.Add("lightwarden", 10);
            priorityTargets.Add("secretkeeper",10);
            priorityTargets.Add("youngdragonhawk", 10);
            priorityTargets.Add("bloodmagethalnos", 10);
            priorityTargets.Add("direwolfalpha", 10);
            priorityTargets.Add("doomsayer", 10);
            priorityTargets.Add("knifejuggler", 10);
            priorityTargets.Add("koboldgeomancer", 10);
            priorityTargets.Add("manaaddict", 10);
            priorityTargets.Add("masterswordsmith",10);
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

}
