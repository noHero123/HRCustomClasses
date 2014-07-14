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

    public enum HeroEnum
    {
        None,
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


        public int attackFaceHp=15;
        public int ownHeroFatigue = 0;
        public int ownDeckSize = 30;
        public int enemyDeckSize = 30;
        public int enemyHeroFatigue = 0;

        public int ownHeroEntity = -1;
        public int enemyHeroEntitiy = -1;
        public DateTime roundstart = DateTime.Now;
        bool tempwounded = false;
        public int currentMana = 0;
        public int heroHp = 30, enemyHp = 30;
        public int heroAtk = 0, enemyAtk = 0;
        public int heroDefence = 0, enemyDefence = 0;
        public bool ownheroisread = false;
        public bool ownAbilityisReady = false;
        public int ownHeroNumAttacksThisTurn = 0;
        public bool ownHeroWindfury = false;

        public List<CardDB.cardIDEnum> ownSecretList = new List<CardDB.cardIDEnum>();
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
        public CardDB.cardName enemyHeroWeapon = CardDB.cardName.unknown;

        public int heroWeaponDurability = 0;
        public int heroWeaponAttack = 0;
        public CardDB.cardName ownHeroWeapon = CardDB.cardName.unknown;
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
            this.ownHeroWeapon = CardDB.cardName.unknown;
            this.enemyHeroWeapon = CardDB.cardName.unknown;
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

            if (s == "hogger")
            {
                return HeroEnum.hogger;
            }
            if (s == "hunter")
            {
                return HeroEnum.hunter;
            }
            if (s == "priest")
            {
                return HeroEnum.priest;
            }
            if (s == "druid")
            {
                return HeroEnum.druid;
            }
            if (s == "warlock")
            {
                return HeroEnum.warlock;
            }
            if (s == "thief")
            {
                return HeroEnum.thief;
            }
            if (s == "pala")
            {
                return HeroEnum.pala;
            }
            if (s == "warrior")
            {
                return HeroEnum.warrior;
            }
            if (s == "shaman")
            {
                return HeroEnum.shaman;
            }
            if (s == "mage")
            {
                return HeroEnum.mage;
            }
            if (s == "lordjaraxxus")
            {
                return HeroEnum.lordjaraxxus;
            }

            return HeroEnum.None;
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
                this.ownSecretList.Add(CardDB.Instance.cardIdstringToEnum(s));
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
            this.ownHeroWeapon = CardDB.Instance.cardNamestringToEnum(weapon);
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
            this.enemyHeroWeapon = CardDB.Instance.cardNamestringToEnum(weapon);
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

            if (m.name == CardDB.cardName.lightspawn && !m.silenced)
            {
                m.Angr = m.Hp;
            }


            return m;
        }


        public void printHero()
        {
            help.logg("player:");
            help.logg(this.numMinionsPlayedThisTurn + " " + this.cardsPlayedThisTurn + " " + this.ueberladung + " " + this.ownPlayerController );

            help.logg("ownhero:");
            help.logg(this.heroname + " " + heroHp + " " + heroDefence + " " + this.heroImmuneToDamageWhileAttacking + " " + this.heroImmune);
            help.logg("ready: " + this.ownheroisread + " alreadyattacked: " + this.ownHeroNumAttacksThisTurn + " frzn: " + this.herofrozen + " attack: " + heroAtk + " " + heroWeaponAttack + " " + heroWeaponDurability + " " + ownHeroWeapon);
            help.logg("ability: " + this.ownAbilityisReady + " " + this.heroAbility.cardIDenum);
            string secs = "";
            foreach (CardDB.cardIDEnum sec in this.ownSecretList)
            {
                secs += sec + " ";
            }
            help.logg("osecrets: " + secs);
            help.logg("enemyhero:");
            help.logg(this.enemyHeroname + " " + enemyHp + " " + enemyDefence + " " + this.enemyfrozen + " " + this.enemyHeroImmune);
            help.logg(this.enemyWeaponAttack + " " + this.enemyWeaponDurability +" " + this.enemyHeroWeapon);
            help.logg("ability: " + "true" + " " + this.enemyAbility.cardIDenum);
            help.logg("fatigue: " + this.ownDeckSize + " " +this.ownHeroFatigue + " "+ this.enemyDeckSize + " " + this.enemyHeroFatigue);

        }


        public void printOwnMinions()
        {
            help.logg("OwnMinions:");
            foreach (Minion m in this.ownMinions)
            {
                help.logg(m.name + " " + m.handcard.card.cardIDenum + " id:" + m.id + " zp:" + m.zonepos + " e:" + m.entitiyID + " A:" + m.Angr + " H:" + m.Hp + " mH:" + m.maxHp + " rdy:" + m.Ready + " tnt:" + m.taunt + " frz:" + m.frozen + " silenced:" + m.silenced + " divshield:" + m.divineshild + " ptt:" + m.playedThisTurn + " wndfr:" + m.windfury + " natt:" + m.numAttacksThisTurn + " stl:" + m.stealth + " poi:" + m.poisonous + " imm:" + m.immune + " ex:" + m.exhausted + " chrg:" + m.charge);
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
                help.logg(m.name + " " + m.handcard.card.cardIDenum + " id:" + m.id + " zp:" + m.zonepos + " e:" + m.entitiyID + " A:" + m.Angr + " H:" + m.Hp + " mH:" + m.maxHp + " rdy:" + m.Ready + " tnt:" + m.taunt + " frz:" + m.frozen + " silenced:" + m.silenced + " divshield:" + m.divineshild + " wndfr:" + m.windfury + " stl:" + m.stealth + " poi:" + m.poisonous + " imm:" + m.immune + " ex:" + m.exhausted + " chrg:" + m.charge);
                foreach (Enchantment e in m.enchantments)
                {
                    help.logg(e.CARDID + " " + e.creator + " " + e.controllerOfCreator);
                }
            }

        }


    }


}
