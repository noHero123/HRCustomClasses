using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace HREngine.Bots
{

    public class Hrtprozis
    {

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

        public string heroname = "druid", enemyHeroname = "druid";
        public CardDB.Card heroAbility;
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

        public void updateOwnHero(string weapon, int watt, int wdur, bool heroimune, int heroatt, int herohp, int herodef, string heron, bool heroready, bool frozen, CardDB.Card hab, bool habrdy, int numAttacksTTurn, bool windfury)
        {
            this.ownHeroWeapon = weapon;
            this.heroWeaponAttack = watt;
            this.heroWeaponDurability = wdur;
            this.heroImmuneToDamageWhileAttacking = heroimune;
            this.heroAtk = heroatt;
            this.heroHp = herohp;
            this.heroDefence = herodef;
            this.heroname = heron;
            this.ownheroisread = heroready;
            this.herofrozen = frozen;
            this.heroAbility = hab;
            this.ownAbilityisReady = habrdy;
            this.ownHeroWindfury = windfury;
            this.ownHeroNumAttacksThisTurn = numAttacksTTurn;

        }

        public void updateEnemyHero(string weapon, int watt, int wdur, int heroatt, int herohp, int herodef, string heron, bool frozen)
        {
            this.enemyHeroWeapon = weapon;
            this.enemyWeaponAttack = watt;
            this.enemyWeaponDurability = wdur;
            this.enemyAtk = heroatt;
            this.enemyHp = herohp;
            this.enemyHeroname = heron;
            this.enemyDefence = herodef;
            this.enemyfrozen = frozen;
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

        private Minion createNewMinion(CardDB.Card c, int id)
        {
            Minion m = new Minion();
            m.card = c;
            m.id = id;
            m.zonepos = id + 1;
            m.entitiyID = c.entityID;
            m.Posix = 0;
            m.Posiy = 0;
            m.Angr = c.Attack;
            m.Hp = c.Health;
            m.maxHp = c.Health;
            m.name = c.name;
            m.playedThisTurn = true;
            m.numAttacksThisTurn = 0;


            if (c.windfury) m.windfury = true;
            if (c.tank) m.taunt = true;
            if (c.Charge)
            {
                m.Ready = true;
                m.charge = true;
            }

            if (c.poisionous) m.poisonous = true;

            if (c.Stealth) m.stealth = true;

            if (m.name == "lichtbrut" && !m.silenced)
            {
                m.Angr = m.Hp;
            }


            return m;
        }


        public void printHero()
        {
            help.logg("player:");
            help.logg(this.currentMana + " " + this.ownMaxMana + " " + this.numMinionsPlayedThisTurn + " " + this.cardsPlayedThisTurn + " " + this.ueberladung);

            help.logg("ownhero:");
            help.logg(this.heroname + " " + heroHp + " " + heroDefence);
            help.logg("ready: "+this.ownheroisread + " alreadyattacked: " + this.ownHeroNumAttacksThisTurn + " attack: " + heroAtk + " weapon attk: " + heroWeaponAttack);
            help.logg("enemyhero:");
            help.logg(this.enemyHeroname + " " + enemyHp + " " + heroAtk);

        }


        public void printOwnMinions()
        {
            help.logg("OwnMinions:");
            foreach (Minion m in this.ownMinions)
            {
                help.logg(m.name + " id " + m.id + " zp " + m.zonepos + " " + " A:" + m.Angr + " H:" + m.Hp + " rdy:" + m.Ready + " tnk:" + m.taunt + " frz:" + m.frozen);
                foreach (Enchantment e in m.enchantments)
                {
                    help.logg(e.CARDID +" "+ CardDB.Instance.getCardDataFromID(e.CARDID).name);
                }
            }

        }

        public void printEnemyMinions()
        {
            help.logg("EnemyMinions:");
            foreach (Minion m in this.enemyMinions)
            {
                help.logg(m.name + " id " + m.id + " zp " + m.zonepos + " " + " A:" + m.Angr + " H:" + m.Hp + " rdy:" + m.Ready + " tnk:" + m.taunt);
            }

        }


        public void loadPreparedHeros(int bfield)
        {
 
            if (bfield == 0)
            {
                
                currentMana = 10;
                heroHp = 30;
                enemyHp = 5;
                heroAtk = 0;
                enemyAtk = 0;
                heroDefence = 0;
                enemyDefence = 0;
                ownheroisread = false;
                ownAbilityisReady = false;
                heroname = "druid";
                enemyHeroname = "warrior";
                this.heroAbility = this.cdb.getCardDataFromID("CS2_017");
                anzEnemys = 0;
                anzOwn = 0;
                herofrozen = false;
                enemyfrozen = false;
                numMinionsPlayedThisTurn = 0;
                cardsPlayedThisTurn = 0;
                ueberladung = 0;
                ownMaxMana = 10;
                enemyMaxMana = 10;
                enemyWeaponDurability = 0;
                enemyWeaponAttack = 0;
                enemyHeroWeapon = "";

                heroWeaponDurability = 0;
                heroWeaponAttack = 0;
                ownHeroWeapon = "";
                heroImmuneToDamageWhileAttacking = false;

                ownPlayerController = 1;
            }
        }

        public void loadPreparedBattlefield(int bfield)
        {
            this.ownMinions.Clear();
            this.enemyMinions.Clear();


            if (bfield == 0)
            {

                Minion own1 = createNewMinion(cdb.getCardDataFromID("CS2_171"), 0); // steinhauereber
                own1.Ready = true;
                this.ownMinions.Add(own1);

                Minion enemy1 = createNewMinion(cdb.getCardDataFromID("CS2_222"), 0);// champion von sturmwind
                enemy1.Ready = true;
                this.enemyMinions.Add(enemy1);

            }

            if (bfield == 1)
            {

                Minion enemy1 = createNewMinion(cdb.getCardDataFromID("CS2_152"), 0);
                Minion enemy2 = createNewMinion(cdb.getCardDataFromID("CS2_152"), 1);
                Minion enemy3 = createNewMinion(cdb.getCardDataFromID("EX1_097"), 2);
                Minion enemy4 = createNewMinion(cdb.getCardDataFromID("CS2_152"), 3);
                Minion enemy5 = createNewMinion(cdb.getCardDataFromID("EX1_097"), 4);
                enemy1.stealth = true;
                enemy2.stealth = true;
                enemy4.stealth = true;
                enemy5.stealth = true;
                enemy5.Hp = 2; enemy5.maxHp = 4;


                this.enemyMinions.Add(enemy1);
                this.enemyMinions.Add(enemy2);
                this.enemyMinions.Add(enemy3);
                this.enemyMinions.Add(enemy4);
                this.enemyMinions.Add(enemy5);

            }

            if (bfield == 2)
            {

                Minion enemy1 = createNewMinion(cdb.getCardDataFromID("NEW1_011"), 0);
                Minion enemy2 = createNewMinion(cdb.getCardDataFromID("CS2_152"), 1);
                enemy2.stealth = true;


                this.enemyMinions.Add(enemy1);
                this.enemyMinions.Add(enemy2);

            }

            if (bfield == 3)
            {
                //wichtelmeisterin

                Minion own1 = createNewMinion(cdb.getCardDataFromID("EX1_597"), 0); // wichtelmeisterin
                own1.Hp = 2;
                own1.Angr = 6;
                Enchantment e = CardDB.getEnchantmentFromCardID("CS2_046e");
                e.creator=1;
                e.controllerOfCreator=1;
                own1.enchantments.Add(e);
                own1.Ready = false;
                this.ownMinions.Add(own1);

            }

            if(bfield ==10)
            {// benchmark
                Minion own1 = createNewMinion(cdb.getCardDataFromID("CS2_182"), 0); // jeti
                own1.Hp = 3;
                own1.maxHp = 3;
                own1.windfury = true;
                own1.Ready = true;
                this.ownMinions.Add(own1);
                 own1 = createNewMinion(cdb.getCardDataFromID("CS2_182"), 1); // jeti
                own1.Hp = 3;
                own1.maxHp = 3;
                own1.windfury = true;
                own1.Ready = true;
                this.ownMinions.Add(own1);
                 own1 = createNewMinion(cdb.getCardDataFromID("CS2_182"), 2); // jeti
                own1.Hp = 3;
                own1.maxHp = 3;
                own1.windfury = true;
                own1.Ready = true;
                this.ownMinions.Add(own1);
                 own1 = createNewMinion(cdb.getCardDataFromID("CS2_182"), 3); // jeti
                own1.Hp = 3;
                own1.maxHp = 3;
                own1.windfury = true;
                own1.Ready = true;
                this.ownMinions.Add(own1);
                 own1 = createNewMinion(cdb.getCardDataFromID("CS2_182"), 4); // jeti
                own1.Hp = 3;
                own1.maxHp = 3;
                own1.windfury = true;
                own1.Ready = true;
                this.ownMinions.Add(own1);
                 own1 = createNewMinion(cdb.getCardDataFromID("CS2_182"), 5); // jeti
                own1.Hp = 3;
                own1.maxHp = 3;
                own1.windfury = true;
                own1.Ready = true;
                this.ownMinions.Add(own1);
                 own1 = createNewMinion(cdb.getCardDataFromID("CS2_182"), 6); // jeti
                own1.Hp = 3;
                own1.maxHp = 3;
                own1.windfury = true;
                own1.Ready = true;
                this.ownMinions.Add(own1);

                // enemys

                own1 = createNewMinion(cdb.getCardDataFromID("CS2_182"), 0); // jeti
                this.enemyMinions.Add(own1);
                own1 = createNewMinion(cdb.getCardDataFromID("CS2_182"), 1); // jeti
                this.enemyMinions.Add(own1);
                own1 = createNewMinion(cdb.getCardDataFromID("CS2_182"), 2); // jeti
                this.enemyMinions.Add(own1);
                own1 = createNewMinion(cdb.getCardDataFromID("CS2_182"), 3); // jeti
                this.enemyMinions.Add(own1);
                own1 = createNewMinion(cdb.getCardDataFromID("CS2_182"), 4); // jeti
                this.enemyMinions.Add(own1);
                own1 = createNewMinion(cdb.getCardDataFromID("CS2_182"), 5); // jeti
                this.enemyMinions.Add(own1);
                own1 = createNewMinion(cdb.getCardDataFromID("CS2_182"), 6); // jeti
                this.enemyMinions.Add(own1);

            }


            updatePositions();
        }


    }


}
