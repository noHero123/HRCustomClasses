// the ai :D
//please ask/write me if you use this in your project

using System;
using System.Collections.Generic;
using System.Text;



//TODO:

//cardids of duplicate + avenge

//tueftlermeisteroberfunks
//verrückter bomber ( 3 damage to random chars)
//nozdormu (for computing time :D)
//faehrtenlesen
// lehrensucher cho
//scharmuetzel kills all :D
//hoggersmash




namespace HREngine.Bots
{

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
        public int penalty = 0;

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
                    if (this.penalty > 0)
                    {
                        help.logg("penality for playing " + this.penalty);
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

        public List<CardDB.cardIDEnum> ownSecretsIDList = new List<CardDB.cardIDEnum>();
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
        public CardDB.cardName ownWeaponName = CardDB.cardName.unknown;
        public CardDB.cardName enemyWeaponName = CardDB.cardName.unknown;
        
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

                if (m.name == CardDB.cardName.prophetvelen) this.doublepriest++;
                spellpower = spellpower + m.handcard.card.spellpowervalue;
                if (m.name == CardDB.cardName.auchenaisoulpriest) this.auchenaiseelenpriesterin = true;

                if (m.name == CardDB.cardName.pintsizedsummoner)
                {
                    this.winzigebeschwoererin++;
                    this.startedWithWinzigebeschwoererin++;
                }
                if (m.name == CardDB.cardName.sorcerersapprentice)
                {
                    this.zauberlehrling++;
                    this.startedWithZauberlehrling++;
                }
                if (m.name == CardDB.cardName.manawraith)
                {
                    this.managespenst++;
                    this.startedWithManagespenst++;
                }
                if (m.name == CardDB.cardName.venturecomercenary)
                {
                    this.soeldnerDerVenture++;
                    this.startedWithsoeldnerDerVenture++;
                }
                if (m.name == CardDB.cardName.summoningportal)
                {
                    this.beschwoerungsportal++;
                    this.startedWithbeschwoerungsportal++;
                }

                if (m.handcard.card.name == CardDB.cardName.baronrivendare)
                {
                    this.ownBaronRivendare = true;
                }

                foreach (Enchantment e in m.enchantments)// only at first init needed, after that its copied
                {
                    if (e.CARDID == CardDB.cardIDEnum.NEW1_036e || e.CARDID == CardDB.cardIDEnum.NEW1_036e2) m.cantLowerHPbelowONE = true;
                }
            }

            foreach (Minion m in this.enemyMinions)
            {
                if (m.silenced) continue;
                this.enemyspellpower = this.enemyspellpower + m.handcard.card.spellpowervalue;
                if (m.name == CardDB.cardName.prophetvelen) this.enemydoublepriest++;
                if (m.name == CardDB.cardName.manawraith)
                {
                    this.managespenst++;
                    this.startedWithManagespenst++;
                }
                if (m.handcard.card.name == CardDB.cardName.baronrivendare)
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

            foreach (CardDB.cardIDEnum s in p.ownSecretsIDList)
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

                if (m.handcard.card.name == CardDB.cardName.prophetvelen) this.doublepriest++;
                spellpower = spellpower + m.handcard.card.spellpowervalue;
                if (m.handcard.card.name == CardDB.cardName.auchenaisoulpriest) this.auchenaiseelenpriesterin = true;
                if (m.handcard.card.name == CardDB.cardName.pintsizedsummoner) this.winzigebeschwoererin++;
                if (m.handcard.card.name == CardDB.cardName.sorcerersapprentice) this.zauberlehrling++;
                if (m.handcard.card.name == CardDB.cardName.manawraith) this.managespenst++;
                if (m.handcard.card.name == CardDB.cardName.venturecomercenary) this.soeldnerDerVenture++;
                if (m.handcard.card.name == CardDB.cardName.summoningportal) this.beschwoerungsportal++;
                if (m.handcard.card.name == CardDB.cardName.baronrivendare)
                {
                    this.ownBaronRivendare = true;
                }


            }

            foreach (Minion m in this.enemyMinions)
            {
                if (m.silenced) continue;
                this.enemyspellpower = this.enemyspellpower + m.handcard.card.spellpowervalue;
                if (m.handcard.card.name == CardDB.cardName.prophetvelen) this.enemydoublepriest++;
                if (m.handcard.card.name == CardDB.cardName.manawraith) this.managespenst++;
                if (m.handcard.card.name == CardDB.cardName.baronrivendare)
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
                
                if(logg) Helpfunctions.Instance.logg("enemy secrets changed ");
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

            if (this.ownHeroHp != p.ownHeroHp || this.ownheroAngr != p.ownheroAngr || this.ownHeroDefence != p.ownHeroDefence || this.ownHeroFrozen != p.ownHeroFrozen || this.heroImmuneWhileAttacking != p.heroImmuneWhileAttacking || this.heroImmune!=p.heroImmune)
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
                if (dis.entitiyID != pis.entitiyID) minionbool= false;
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
                if ( dishc.position != pishc.position || dishc.entity != pishc.entity || dishc.getManaCost(this) != pishc.getManaCost(p))
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
            if (this.enemyWeaponName == CardDB.cardName.doomhammer) this.enemyHeroWindfury = true;
            this.enemyheroImmuneWhileAttacking = false;
            if (this.enemyWeaponName == CardDB.cardName.gladiatorslongbow) this.enemyheroImmuneWhileAttacking = true;
            if (!this.enemyHeroFrozen && this.enemyWeaponDurability > 0) this.enemyHeroReady = true;
            this.enemyheroAngr = this.enemyWeaponAttack;
            bool havedonesomething = true;
            List<Playfield> posmoves = new List<Playfield>();
            posmoves.Add(new Playfield(this));
            List<Playfield> temp = new List<Playfield>();
            int deep = 0;
            int enemMana = Math.Min(this.enemyMaxMana + 1, 10);

            if (playaround)
            {
                int oldval = Ai.Instance.botBase.getPlayfieldValue(posmoves[0]);
                posmoves[0].value = int.MinValue;
                enemMana = posmoves[0].EnemyCardPlaying(this.enemyHeroName, enemMana, this.enemycarddraw + this.enemyAnzCards);
                int newval = Ai.Instance.botBase.getPlayfieldValue(posmoves[0]);
                posmoves[0].value = int.MinValue;
                if (oldval < newval)
                {
                    posmoves.Clear();
                    posmoves.Add(new Playfield(this));
                }
            }

            foreach (Minion m in posmoves[0].enemyMinions)
            {
                if (m.frozen || (m.handcard.card.name == CardDB.cardName.ancientwatcher && !m.silenced))
                {
                    m.Ready = false;
                }
                m.Ready = true;
                m.numAttacksThisTurn = 0;
            }

            //play ability!
            if (posmoves[0].enemyAbilityReady && enemMana >= 2 && posmoves[0].enemyHeroAblility.canplayCard(posmoves[0], 0))
            {
                int abilityPenality = 0;

                havedonesomething = true;
                // if we have mage or priest, we have to target something####################################################
                if (posmoves[0].enemyHeroName == HeroEnum.mage || posmoves[0].enemyHeroName == HeroEnum.priest)
                {

                    List<targett> trgts = posmoves[0].enemyHeroAblility.getTargetsForCardEnemy(posmoves[0]);
                    foreach (targett trgt in trgts)
                    {
                        if (trgt.target >= 100) continue;
                        Playfield pf = new Playfield(posmoves[0]);
                        //havedonesomething = true;
                        //Helpfunctions.Instance.logg("use ability on " + trgt.target + " " + trgt.targetEntity);
                        pf.ENEMYactivateAbility(posmoves[0].enemyHeroAblility, trgt.target, trgt.targetEntity);
                        posmoves.Add(pf);
                    }
                }
                else
                {
                    // the other classes dont have to target####################################################
                    Playfield pf = new Playfield(posmoves[0]);

                    //havedonesomething = true;
                    posmoves[0].ENEMYactivateAbility(posmoves[0].enemyHeroAblility, -1, -1);
                    posmoves.Add(pf);
                }

            }

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
                                //help.logg(mnn.silenced + " " + m.silenced + " " + mnn.name + " " + m.name + " " + penman.cardName.ContainsKey(m.name));

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

                    p.endEnemyTurn();
                    p.guessingHeroHP = this.guessingHeroHP;
                    if (Ai.Instance.botBase.getPlayfieldValue(p) < bestoldval) // want the best enemy-play-> worst for us
                    {
                        bestoldval = Ai.Instance.botBase.getPlayfieldValue(p);
                        bestold = p;
                    }
                    posmoves.Remove(p);

                    if (posmoves.Count >= maxwide) break;
                }

                if ( bestoldval <= 10000 && bestold != null)
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
                p.guessingHeroHP = this.guessingHeroHP;
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

            bool useAOE = false;
            int mobscount = 0;
            foreach (Minion min in this.ownMinions)
            {
                if (min.maxHp >= 2 && min.Angr >= 2) mobscount++;
            }

            if (mobscount >= 3) useAOE = true;

            if (enemyHeroNamee == HeroEnum.warrior)
            {
                bool usewhirlwind = true;
                foreach (Minion m in this.enemyMinions)
                {
                    if (m.Hp == 1) usewhirlwind = false;
                }
                if (this.ownMinions.Count <= 3) usewhirlwind = false;

                if (usewhirlwind)
                {
                    mana = EnemyPlaysACard(CardDB.cardName.whirlwind, mana);
                }
            }

            if (!useAOE) return mana;

            if (enemyHeroNamee == HeroEnum.mage)
            {
                    mana = EnemyPlaysACard(CardDB.cardName.flamestrike, mana);
                    mana = EnemyPlaysACard(CardDB.cardName.blizzard, mana);
            }

            if (enemyHeroNamee == HeroEnum.hunter)
            {
                mana = EnemyPlaysACard(CardDB.cardName.unleashthehounds, mana);
            }

            if (enemyHeroNamee == HeroEnum.priest)
            {
                mana = EnemyPlaysACard(CardDB.cardName.holynova, mana);
            }

            if (enemyHeroNamee == HeroEnum.shaman)
            {
                mana = EnemyPlaysACard(CardDB.cardName.lightningstorm, mana);
            }

            if (enemyHeroNamee == HeroEnum.pala)
            {
                mana = EnemyPlaysACard(CardDB.cardName.consecration, mana);
            }

            if (enemyHeroNamee == HeroEnum.druid)
            {
                mana = EnemyPlaysACard(CardDB.cardName.swipe, mana);
            }
            


            return mana;
        }

        private int EnemyPlaysACard(CardDB.cardName cardname, int currmana)
        {
            
            //todo manacosts

                if (cardname == CardDB.cardName.flamestrike && currmana >= 7)
                {
                    List<Minion> temp = new List<Minion>(this.ownMinions);
                    int damage = getEnemySpellDamageDamage(4);
                    foreach (Minion enemy in temp)
                    {
                        minionGetDamagedOrHealed(enemy, damage, 0, true,true);
                    }

                    currmana -= 7;
                    return currmana;
                }

                if (cardname == CardDB.cardName.blizzard && currmana >= 6)
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


                if (cardname == CardDB.cardName.unleashthehounds && currmana >= 5)
                {
                    int anz = this.ownMinions.Count;
                    int posi = this.enemyMinions.Count - 1;
                    CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.EX1_538t);//hound
                    for (int i = 0; i < anz; i++)
                    {
                        callKid(kid, posi, false);
                    }
                    currmana -= 5;
                    return currmana;
                }

                

            

                if (cardname == CardDB.cardName.holynova && currmana >= 5)
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
                



                if (cardname == CardDB.cardName.lightningstorm && currmana >= 4)//3
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
                


                if (cardname == CardDB.cardName.whirlwind && currmana >= 3 )//1
                {
                    List<Minion> temp = new List<Minion>(this.enemyMinions);
                    int damage = getEnemySpellDamageDamage(1);
                    foreach (Minion enemy in temp)
                    {
                        minionGetDamagedOrHealed(enemy, damage, 0, false,true);
                    }
                    temp.Clear();
                    temp = new List<Minion>(this.ownMinions);
                    foreach (Minion enemy in temp)
                    {
                        minionGetDamagedOrHealed(enemy, damage, 0, true,true);
                    }
                    currmana -= 1;
                    return currmana;
                }
                

           
                if (cardname == CardDB.cardName.consecration && currmana >= 4)
                {
                    List<Minion> temp = new List<Minion>(this.ownMinions);
                    int damage = getEnemySpellDamageDamage(2);
                    foreach (Minion enemy in temp)
                    {
                        minionGetDamagedOrHealed(enemy, damage, 0, true,true);
                    }

                    attackOrHealHero(damage, true);
                    currmana -= 4;
                    return currmana;
                }
                

            
                if (cardname == CardDB.cardName.swipe && currmana >= 4)
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

            if (this.ownWeaponName != CardDB.cardName.unknown) this.ownHeroReady = true;
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

                //if (trgts2.Count == 0) trgts2.Add(new targett(100, this.ownHeroEntity));
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
            if (lethal && card.name == CardDB.cardName.defenderofargus)
            {
                i = 0;
                foreach (Minion m in this.ownMinions)
                {

                    places[i] = 0;
                    tempval = 0;
                    if (m.Ready)
                    {
                        tempval -= m.Angr -1;
                        if(m.windfury) tempval-=m.Angr -1;
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
            if (card.name == CardDB.cardName.sunfuryprotector || card.name == CardDB.cardName.defenderofargus) // bestplace, if right and left minions have no taunt + lots of hp, dont make priority-minions to taunt
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
                        tempval -= m.Hp+2;
                    }

                    if (m.handcard.card.name == CardDB.cardName.flametonguetotem) tempval += 50;
                    if (m.handcard.card.name == CardDB.cardName.raidleader) tempval += 10;
                    if (m.handcard.card.name == CardDB.cardName.grimscaleoracle) tempval += 10;
                    if (m.handcard.card.name == CardDB.cardName.direwolfalpha) tempval += 50;
                    if (m.handcard.card.name == CardDB.cardName.murlocwarleader) tempval += 10;
                    if (m.handcard.card.name == CardDB.cardName.southseacaptain) tempval += 10;
                    if (m.handcard.card.name == CardDB.cardName.stormwindchampion) tempval += 10;
                    if (m.handcard.card.name == CardDB.cardName.timberwolf) tempval += 10;
                    if (m.handcard.card.name == CardDB.cardName.leokk) tempval += 10;
                    if (m.handcard.card.name == CardDB.cardName.northshirecleric) tempval += 10;
                    if (m.handcard.card.name == CardDB.cardName.sorcerersapprentice) tempval += 10;
                    if (m.handcard.card.name == CardDB.cardName.pintsizedsummoner) tempval += 10;
                    if (m.handcard.card.name == CardDB.cardName.summoningportal) tempval += 10;
                    if (m.handcard.card.name == CardDB.cardName.scavenginghyena) tempval += 10;

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
                    if(bestval > prev + next) 
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
            if (card.name == CardDB.cardName.flametonguetotem || card.name == CardDB.cardName.direwolfalpha)
            {
                placebuff = true;
                if (card.name == CardDB.cardName.flametonguetotem) cardIsBuffer = 2;
                if (card.name == CardDB.cardName.direwolfalpha) cardIsBuffer = 1;
            }
            bool commander = false;
            foreach (Minion m in this.ownMinions)
            {
                if (m.handcard.card.name == CardDB.cardName.warsongcommander) commander = true;
            }
            foreach (Minion m in this.ownMinions)
            {
                if (m.handcard.card.name == CardDB.cardName.flametonguetotem || m.handcard.card.name == CardDB.cardName.direwolfalpha) placebuff = true;
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
                    if (m.handcard.card.name == CardDB.cardName.flametonguetotem )
                    {
                        buffplaces[i] = 2;
                    }
                    if (m.handcard.card.name == CardDB.cardName.direwolfalpha)
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
                        prev+=cardIsBuffer;
                        current+=buffplaces[i-1];
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
                            next -= buffplaces[i-1];
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

            if (card.name == CardDB.cardName.flametonguetotem) cardvalue += 90;
            if (card.name == CardDB.cardName.raidleader) cardvalue += 10;
            if (card.name == CardDB.cardName.grimscaleoracle) cardvalue += 10;
            if (card.name == CardDB.cardName.direwolfalpha) cardvalue += 90;
            if (card.name == CardDB.cardName.murlocwarleader) cardvalue += 10;
            if (card.name == CardDB.cardName.southseacaptain) cardvalue += 10;
            if (card.name == CardDB.cardName.stormwindchampion) cardvalue += 10;
            if (card.name == CardDB.cardName.timberwolf) cardvalue += 10;
            if (card.name == CardDB.cardName.leokk) cardvalue += 10;
            if (card.name == CardDB.cardName.northshirecleric) cardvalue += 10;
            if (card.name == CardDB.cardName.sorcerersapprentice) cardvalue += 10;
            if (card.name == CardDB.cardName.pintsizedsummoner) cardvalue += 10;
            if (card.name == CardDB.cardName.summoningportal) cardvalue += 10;
            if (card.name == CardDB.cardName.scavenginghyena) cardvalue += 10;
            if (card.name == CardDB.cardName.faeriedragon) cardvalue += 40;
            cardvalue += 1;

            i = 0;
            foreach(Minion m in this.ownMinions)
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
                    if (m.handcard.card.name == CardDB.cardName.flametonguetotem) tempval += 90;
                    if (m.handcard.card.name == CardDB.cardName.raidleader) tempval += 10;
                    if (m.handcard.card.name == CardDB.cardName.grimscaleoracle) tempval += 10;
                    if (m.handcard.card.name == CardDB.cardName.direwolfalpha) tempval += 90;
                    if (m.handcard.card.name == CardDB.cardName.murlocwarleader) tempval += 10;
                    if (m.handcard.card.name == CardDB.cardName.southseacaptain) tempval += 10;
                    if (m.handcard.card.name == CardDB.cardName.stormwindchampion) tempval += 10;
                    if (m.handcard.card.name == CardDB.cardName.timberwolf) tempval += 10;
                    if (m.handcard.card.name == CardDB.cardName.leokk) tempval += 10;
                    if (m.handcard.card.name == CardDB.cardName.northshirecleric) tempval += 10;
                    if (m.handcard.card.name == CardDB.cardName.sorcerersapprentice) tempval += 10;
                    if (m.handcard.card.name == CardDB.cardName.pintsizedsummoner) tempval += 10;
                    if (m.handcard.card.name == CardDB.cardName.summoningportal) tempval += 10;
                    if (m.handcard.card.name == CardDB.cardName.scavenginghyena) tempval += 10;
                    if (m.handcard.card.name == CardDB.cardName.faeriedragon) tempval += 40;
                    if (m.stealth) tempval += 40;
                }
                places[i] = tempval;

                i++;
            }

            //bigminion if >=10
            int bestplace = 0;
            int bestvale = 0;
            tempval=0;
            i=0;
            for (int j = 0; j <= this.ownMinions.Count; j++ )
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
                        bestplace = i ;
                        bestvale = tempval;
                    }
                }

                i++;
            }

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

            this.evaluatePenality += ComboBreaker.Instance.checkIfComboWasPlayed(this.playactions, this.ownWeaponName, this.ownHeroName);

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
                if (this.guessingHeroHP >= 1)
                {
                    simulateEnemysTurn(simulateTwoTurns, playaround, print);
                }
                this.complete = true;
            }
            
        }

        private void guessHeroDamage()
        {
            int ghd = 0;
            foreach (Minion m in this.enemyMinions)
            {
                if (m.frozen) continue;
                if (m.name == CardDB.cardName.ancientwatcher && !m.silenced)
                {
                    continue;
                }
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
            foreach (CardDB.cardIDEnum secretID in this.ownSecretsIDList)
            {
                //hunter secrets############
                if (secretID == CardDB.cardIDEnum.EX1_554) //snaketrap
                {

                    //call 3 snakes (if possible)
                    int posi = this.ownMinions.Count - 1;
                    CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.EX1_554t);//snake
                    callKid(kid, posi, true);
                    callKid(kid, posi, true);
                    callKid(kid, posi, true);
                }
                if (secretID == CardDB.cardIDEnum.EX1_609) //snipe
                {
                    //kill weakest minion of enemy
                    List<Minion> temp = new List<Minion>(this.enemyMinions);
                    temp.Sort((a, b) => a.Angr.CompareTo(b.Angr));//take the weakest
                    if (temp.Count == 0) continue;
                    Minion m = temp[0];
                    minionGetDamagedOrHealed(m, 4, 0, false);
                }
                if (secretID == CardDB.cardIDEnum.EX1_610) //explosive trap
                {
                    //take 2 damage to each enemy
                    List<Minion> temp = new List<Minion>(this.enemyMinions);
                    foreach (Minion m in temp)
                    {
                        minionGetDamagedOrHealed(m, 2, 0,false);
                    }
                    attackEnemyHeroWithoutKill(2);
                }
                if (secretID == CardDB.cardIDEnum.EX1_611) //freezing trap
                {
                    //return weakest enemy minion to hand
                    List<Minion> temp = new List<Minion>(this.enemyMinions);
                    temp.Sort((a, b) => a.Angr.CompareTo(b.Angr));//take the weakest
                    if (temp.Count == 0) continue;
                    Minion m = temp[0];
                    minionReturnToHand(m, false,0);
                }
                if (secretID == CardDB.cardIDEnum.EX1_533) // missdirection
                {
                    // first damage to your hero is nulled -> lower guessingHeroDamage
                    List<Minion> temp = new List<Minion>(this.enemyMinions);
                    temp.Sort((a, b) => -a.Angr.CompareTo(b.Angr));//take the strongest
                    if (temp.Count == 0) continue;
                    Minion m = temp[0];
                    m.Angr = 0;
                    this.evaluatePenality -= this.enemyMinions.Count;// the more the enemy minions has on board, the more the posibility to destroy something other :D
                }

                //mage secrets############
                if (secretID == CardDB.cardIDEnum.EX1_287) //counterspell
                {
                    // what should we do?
                    this.evaluatePenality -= 8;
                }

                if (secretID == CardDB.cardIDEnum.EX1_289) //ice barrier
                {
                    this.ownHeroDefence += 8;
                }

                if (secretID == CardDB.cardIDEnum.EX1_295) //ice barrier
                {
                    //set the guessed Damage to zero
                    foreach (Minion m in this.enemyMinions)
                    {
                        m.Angr = 0;
                    }
                }

                if (secretID == CardDB.cardIDEnum.EX1_294) //mirror entity
                {
                    //summon snake ( a weak minion)
                    int posi = this.ownMinions.Count - 1;
                    CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.EX1_554t);//snake
                    callKid(kid, posi, true);
                }
                if (secretID == CardDB.cardIDEnum.tt_010) //spellbender
                {
                    //whut???
                    // add 2 to your defence (most attack-buffs give +2, lots of damage spells too)
                    this.evaluatePenality -= 4;
                }
                if (secretID == CardDB.cardIDEnum.EX1_594) // vaporize
                {
                    // first damage to your hero is nulled -> lower guessingHeroDamage and destroy weakest minion
                    List<Minion> temp = new List<Minion>(this.enemyMinions);
                    temp.Sort((a, b) => a.Angr.CompareTo(b.Angr));//take the weakest
                    if (temp.Count == 0) continue;
                    Minion m = temp[0];
                    minionGetDestroyed(m, false);
                }
                if (secretID == CardDB.cardIDEnum.EX1_duplicate) // duplicate
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
                if (secretID == CardDB.cardIDEnum.EX1_132) // eye for an eye
                {
                    // enemy takes one damage
                    List<Minion> temp = new List<Minion>(this.enemyMinions);
                    temp.Sort((a, b) => a.Angr.CompareTo(b.Angr));//take the weakest
                    if (temp.Count == 0) continue;
                    Minion m = temp[0];
                    attackEnemyHeroWithoutKill(m.Angr);
                }
                if (secretID == CardDB.cardIDEnum.EX1_130) // noble sacrifice
                {
                    //spawn a 2/1 taunt!
                    int posi = this.ownMinions.Count - 1;
                    CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.CS2_121);//frostwolfgrunt
                    callKid(kid, posi, true);
                    this.ownMinions[this.ownMinions.Count - 1].maxHp = 1;
                    this.ownMinions[this.ownMinions.Count - 1].Hp = 1;

                }

                if (secretID == CardDB.cardIDEnum.EX1_136) // redemption
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

                if (secretID == CardDB.cardIDEnum.EX1_379) // repentance
                {
                    // set his current lowest hp minion to x/1
                    List<Minion> temp = new List<Minion>(this.enemyMinions);
                    temp.Sort((a, b) => a.Hp.CompareTo(b.Hp));//take the weakest
                    if (temp.Count == 0) continue;
                    Minion m = temp[0];
                    m.Hp = 1;
                    m.maxHp = 1;
                }

                if (secretID == CardDB.cardIDEnum.EX1_Avenge) // avenge
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

                    if (e.CARDID == CardDB.cardIDEnum.NEW1_036e || e.CARDID == CardDB.cardIDEnum.NEW1_036e2)//commanding shout
                    {
                        debuff(m, e, own);
                    }

                    if (e.CARDID == CardDB.cardIDEnum.EX1_316e)//ueberwaeltigende macht
                    {
                        minionGetDestroyed(m, own);
                    }

                    if (e.CARDID == CardDB.cardIDEnum.CS2_046e)//kampfrausch
                    {
                        debuff(m, e,own);
                    }

                    if (e.CARDID == CardDB.cardIDEnum.CS2_045e)// waffe felsbeiser
                    {
                        debuff(m, e, own);
                    }

                    if (e.CARDID == CardDB.cardIDEnum.EX1_046e)// dunkeleisenzwerg
                    {
                        debuff(m, e, own);
                    }
                    if (e.CARDID == CardDB.cardIDEnum.CS2_188o)// ruchloserunteroffizier
                    {
                        debuff(m, e, own);
                    }
                    if (e.CARDID == CardDB.cardIDEnum.EX1_055o)//  manasuechtige
                    {
                        debuff(m, e, own);
                    }
                    if (e.CARDID == CardDB.cardIDEnum.EX1_549o)//zorn des wildtiers
                    {
                        debuff(m, e, own);
                    }
                    if (e.CARDID == CardDB.cardIDEnum.EX1_334e)// dunkler wahnsin (control minion till end of turn)
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

                    if (e.CARDID == CardDB.cardIDEnum.EX1_046e)// dunkeleisenzwerg
                    {
                        debuff(m, e,!own);
                    }
                    if (e.CARDID == CardDB.cardIDEnum.CS2_188o)// ruchloserunteroffizier
                    {
                        debuff(m, e, !own);
                    }
                    if (e.CARDID == CardDB.cardIDEnum.EX1_549o)//zorn des wildtiers
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

                if (m.name == CardDB.cardName.barongeddon) // all other chards get dmg get 2 dmg
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

                if (m.name == CardDB.cardName.bloodimp || m.name == CardDB.cardName.youngpriestess) // buff a minion
                {
                    List<Minion> temp2 = new List<Minion>(ownmins);
                    temp2.Sort((a, b) => a.Hp.CompareTo(b.Hp));//buff the weakest
                    foreach (Minion mins in Helpfunctions.TakeList(temp2, 1))
                    {
                        minionGetBuffed(mins, 0, 1, own);
                    }
                }

                if (m.name == CardDB.cardName.masterswordsmith) // buff a minion
                {
                    List<Minion> temp2 = new List<Minion>(ownmins);
                    temp2.Sort((a, b) => a.Angr.CompareTo(b.Angr));//buff the weakest
                    foreach (Minion mins in Helpfunctions.TakeList(temp2, 1))
                    {
                        minionGetBuffed(mins, 1, 0, own);
                    }
                }

                if (m.name == CardDB.cardName.emboldener3000) // buff a minion
                {
                    List<Minion> temp2 = new List<Minion>(this.enemyMinions);
                    temp2.Sort((a, b) => -a.Angr.CompareTo(b.Angr));//buff the strongest enemy
                    foreach (Minion mins in Helpfunctions.TakeList(temp2, 1))
                    {
                        minionGetBuffed(mins, 1, 0, false);//buff alyways enemy :D
                    }
                }

                if (m.name == CardDB.cardName.gruul) // gain +1/+1
                {
                    minionGetBuffed(m, 1, 1, own);
                }

                if (m.name == CardDB.cardName.etherealarcanist) // gain +2/+2
                {
                    if (own && this.ownSecretsIDList.Count>=1)
                    {
                        minionGetBuffed(m, 2, 2, own);
                    }
                    if (!own && this.enemySecretCount >= 1)
                    {
                        minionGetBuffed(m, 2, 2, own);
                    }
                }


                if (m.name == CardDB.cardName.manatidetotem) // draw card
                {
                    if (own)
                    {
                        //this.owncarddraw++;
                        this.drawACard(CardDB.cardName.unknown, own);
                    }
                    else
                    {
                        //this.enemycarddraw++;
                        this.drawACard(CardDB.cardName.unknown, own);
                    }
                }

                if (m.name == CardDB.cardName.healingtotem) // heal
                {
                    List<Minion> temp2 = new List<Minion>(ownmins);
                    foreach (Minion mins in temp2)
                    {
                        minionGetDamagedOrHealed(mins, 0, 1, own);
                    }
                }

                if (m.name == CardDB.cardName.hogger) // summon
                {
                    int posi = m.id;
                    CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.NEW1_040t);//gnoll
                    callKid(kid, posi, own);
                }

                if (m.name == CardDB.cardName.impmaster) // damage itself and summon 
                {
                    int posi = m.id;
                    if (m.Hp == 1) posi--;
                    minionGetDamagedOrHealed(m, 1, 0, own);

                    CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.EX1_598);//imp
                    callKid(kid, posi, own);
                    m.stealth = false;
                }

                if (m.name == CardDB.cardName.natpagle) // draw card
                {
                    if (own)
                    {
                        //this.owncarddraw++;
                        this.drawACard(CardDB.cardName.unknown, own);
                    }
                    else
                    {
                        //this.enemycarddraw++;
                        this.drawACard(CardDB.cardName.unknown, own);
                    }
                }

                if (m.name == CardDB.cardName.ragnarosthefirelord) // summon
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
                    m.stealth = false;
                }


                if (m.name == CardDB.cardName.repairbot) // heal damaged char
                {

                    attackOrHealHero(-6, false);
                }
                if (m.handcard.card.cardIDenum == CardDB.cardIDEnum.EX1_tk9) //treant which is destroyed
                {
                    minionGetDestroyed(m, own);
                }

                if (m.name == CardDB.cardName.ysera) // draw card
                {
                    if (own)
                    {
                        //this.owncarddraw++;
                        this.drawACard(CardDB.cardName.yseraawakens,own);
                    }
                    else
                    {
                        //this.enemycarddraw++;
                        this.drawACard(CardDB.cardName.yseraawakens, own);
                    }
                }
            }

            foreach (Minion m in enemymins)
            {
                if (m.name == CardDB.cardName.gruul) // gain +1/+1
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

            bool untergang=false;
            foreach (Minion m in temp)
            {
                if (m.silenced) continue;

                if (m.name == CardDB.cardName.demolisher) // deal 2 dmg
                {
                    List<Minion> temp2 = new List<Minion>(enemymins);
                    foreach (Minion mins in temp2)
                    {
                        minionGetDamagedOrHealed(mins, 2, 0, !own);
                    }
                }

                if (m.name == CardDB.cardName.shadeofnaxxramas) // buff itself
                {
                    minionGetBuffed(m, 1, 1, own);
                }

                if (m.name == CardDB.cardName.doomsayer) // destroy
                {
                    untergang = true;
                }

                if (m.name == CardDB.cardName.homingchicken) // ok
                {
                    minionGetDestroyed(m, own);
                    if (own)
                    {
                        //this.owncarddraw += 3;
                        this.drawACard(CardDB.cardName.unknown, own);
                        this.drawACard(CardDB.cardName.unknown, own);
                        this.drawACard(CardDB.cardName.unknown, own);
                    }
                    else
                    {
                        //this.enemycarddraw += 3 ;
                        this.drawACard(CardDB.cardName.unknown, own);
                        this.drawACard(CardDB.cardName.unknown, own);
                        this.drawACard(CardDB.cardName.unknown, own);
                    }
                }

                if (m.name == CardDB.cardName.lightwell) // heal
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

                if (m.name == CardDB.cardName.poultryizer) // 
                {
                    if (own)
                    {
                        if (this.enemyMinions.Count >= 1)
                        {
                            List<Minion> temp2 = new List<Minion>(this.enemyMinions);
                            temp2.Sort((a, b) => a.Hp.CompareTo(b.Hp));//damage the lowest
                            foreach (Minion mins in temp2)
                            {
                                CardDB.Card c = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.Mekka4t);
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
                                CardDB.Card c = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.Mekka4t);
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
                                CardDB.Card c = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.Mekka4t);
                                minionTransform(mins, c, true);
                                break;
                            }
                        }
                    }
                }

                if (m.name == CardDB.cardName.alarmobot) // 
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
                            CardDB.Card c = CardDB.Instance.getCardDataFromID(mins.card.cardIDenum);
                            minionTransform(m, c, true);
                            this.removeCard(mins);
                            this.drawACard(CardDB.cardName.alarmobot, true);
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

                    if (e.CARDID == CardDB.cardIDEnum.CS2_063e)//corruption
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

            this.drawACard(CardDB.cardName.unknown, own);
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
                    minionGetBuffed(m, -e.angrbuff, -e.hpbuff,own);
                }
            }
        }

        private void deleteEffectOf(CardDB.cardIDEnum CardID, int creator)
        {
            // deletes the effect of the cardID with creator from all minions 
            Enchantment e = CardDB.getEnchantmentFromCardID(CardID);
            e.creator = creator;
            List<Minion> temp = new List<Minion>(this.ownMinions);
            foreach (Minion m in temp)
            {
                debuff(m, e,true);
            }
            temp.Clear();
            temp.AddRange(this.enemyMinions);
            foreach (Minion m in temp)
            {
                debuff(m, e,false);
            }
        }

        private void deleteEffectOfWithExceptions(CardDB.cardIDEnum CardID, int creator, List<int> exeptions)
        {
            // deletes the effect of the cardID with creator from all minions 
            Enchantment e = CardDB.getEnchantmentFromCardID(CardID);
            e.creator = creator;
            foreach (Minion m in this.ownMinions)
            {
                if (!exeptions.Contains(m.id))
                {
                    debuff(m, e,true);
                }
            }

            foreach (Minion m in this.enemyMinions)
            {
                if (!exeptions.Contains(m.id))
                {
                    debuff(m, e,false);
                }
            }
        }

        private void addEffectToMinionNoDoubles(Minion m, Enchantment e, bool own)
        {
            foreach (Enchantment es in m.enchantments)
            {
                if ( es.CARDID == e.CARDID && es.creator == e.creator) return;
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

        private void adjacentBuffer(Minion m, CardDB.cardIDEnum enchantment, int before, int after, bool own)
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
                getNewEffects(m, own, m.id, false);
            }

        }

        private void endEffectsDueToDeath(Minion m, bool own)
        { // minion which grants effect died
            if (m.handcard.card.name == CardDB.cardName.raidleader) // if he dies, lower attack of all minions of his side
            {
                deleteEffectOf(CardDB.cardIDEnum.CS2_122e, m.entitiyID);
            }

            if (m.handcard.card.name == CardDB.cardName.flametonguetotem)
            {
                deleteEffectOf(CardDB.cardIDEnum.EX1_565o, m.entitiyID);
            }

            if (m.handcard.card.name == CardDB.cardName.grimscaleoracle)
            {
                deleteEffectOf(CardDB.cardIDEnum.EX1_508o, m.entitiyID);
            }

            if (m.handcard.card.name == CardDB.cardName.direwolfalpha)
            {
                deleteEffectOf(CardDB.cardIDEnum.EX1_162o, m.entitiyID);
            }
            if (m.handcard.card.name == CardDB.cardName.murlocwarleader)
            {
                deleteEffectOf(CardDB.cardIDEnum.EX1_507e, m.entitiyID);
            }
            if (m.handcard.card.name == CardDB.cardName.southseacaptain)
            {
                deleteEffectOf(CardDB.cardIDEnum.NEW1_027e, m.entitiyID);
            }
            if (m.handcard.card.name == CardDB.cardName.stormwindchampion)
            {
                deleteEffectOf(CardDB.cardIDEnum.CS2_222o, m.entitiyID);
            }
            if (m.handcard.card.name == CardDB.cardName.timberwolf)
            {
                deleteEffectOf(CardDB.cardIDEnum.DS1_175o, m.entitiyID);
            }
            if (m.handcard.card.name == CardDB.cardName.leokk)
            {
                deleteEffectOf(CardDB.cardIDEnum.NEW1_033o, m.entitiyID);
            }

            //lowering truebaugederalte

            foreach (Minion mnn in this.ownMinions)
            {
                if (mnn.handcard.card.name == CardDB.cardName.oldmurkeye && m.handcard.card.race == 14)
                {
                    minionGetBuffed(mnn, -1, 0, true);
                }
            }
            foreach (Minion mnn in this.enemyMinions)
            {
                if (mnn.handcard.card.name == CardDB.cardName.oldmurkeye && m.handcard.card.race == 14)
                {
                    minionGetBuffed(mnn, -1, 0, false);
                }
            }

            //no deathrattle, but lowering the weapon
            if (m.handcard.card.name == CardDB.cardName.spitefulsmith && m.wounded)// remove weapon changes form hasserfuelleschmiedin
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

            if (own && isSummon && this.ownWeaponName == CardDB.cardName.swordofjustice)
            {
                minionGetBuffed(m, 1, 1, own);
                this.lowerWeaponDurability(1, true);
            }

            int adjacentplace = 1;
            if (isSummon) adjacentplace = 0;

            foreach (Minion ownm in temp)
            {
                if (ownm.silenced) continue; // silenced minions dont buff

                if (isSummon && ownm.handcard.card.name == CardDB.cardName.warsongcommander)
                {
                    havekriegshymnenanfuehrerin = true;
                }

                if (ownm.handcard.card.name == CardDB.cardName.raidleader && ownm.entitiyID != m.entitiyID)
                {
                    Enchantment e = CardDB.getEnchantmentFromCardID(CardDB.cardIDEnum.CS2_122e);
                    e.creator = ownm.entitiyID;
                    e.controllerOfCreator = controller;
                    addEffectToMinionNoDoubles(m, e, own);

                }
                if (ownm.handcard.card.name == CardDB.cardName.leokk && ownm.entitiyID != m.entitiyID)
                {
                    Enchantment e = CardDB.getEnchantmentFromCardID(CardDB.cardIDEnum.NEW1_033o);
                    e.creator = ownm.entitiyID;
                    e.controllerOfCreator = controller;
                    addEffectToMinionNoDoubles(m, e, own);

                }
                if (ownm.handcard.card.name == CardDB.cardName.stormwindchampion && ownm.entitiyID != m.entitiyID)
                {
                    Enchantment e = CardDB.getEnchantmentFromCardID(CardDB.cardIDEnum.CS2_222o);
                    e.creator = ownm.entitiyID;
                    e.controllerOfCreator = controller;
                    addEffectToMinionNoDoubles(m, e, own);
                }
                if (ownm.handcard.card.name == CardDB.cardName.grimscaleoracle && m.handcard.card.race == 14 && ownm.entitiyID != m.entitiyID)
                {
                    Enchantment e = CardDB.getEnchantmentFromCardID(CardDB.cardIDEnum.EX1_508o);
                    e.creator = ownm.entitiyID;
                    e.controllerOfCreator = controller;
                    addEffectToMinionNoDoubles(m, e, own);
                }
                if (ownm.handcard.card.name == CardDB.cardName.murlocwarleader && m.handcard.card.race == 14 && ownm.entitiyID != m.entitiyID)
                {
                    Enchantment e = CardDB.getEnchantmentFromCardID(CardDB.cardIDEnum.EX1_507e);
                    e.creator = ownm.entitiyID;
                    e.controllerOfCreator = controller;
                    addEffectToMinionNoDoubles(m, e, own);
                }
                if (ownm.handcard.card.name == CardDB.cardName.southseacaptain && m.handcard.card.race == 23)
                {
                    Enchantment e = CardDB.getEnchantmentFromCardID(CardDB.cardIDEnum.NEW1_027e);
                    e.creator = ownm.entitiyID;
                    e.controllerOfCreator = controller;
                    addEffectToMinionNoDoubles(m, e, own);
                }


                if (ownm.handcard.card.name == CardDB.cardName.timberwolf && (TAG_RACE)m.handcard.card.race == TAG_RACE.PET)
                {
                    Enchantment e = CardDB.getEnchantmentFromCardID(CardDB.cardIDEnum.DS1_175o);
                    e.creator = ownm.entitiyID;
                    e.controllerOfCreator = controller;
                    addEffectToMinionNoDoubles(m, e, own);
                }

                if (isSummon && ownm.handcard.card.name == CardDB.cardName.tundrarhino && (TAG_RACE)m.handcard.card.race == TAG_RACE.PET)
                {
                    minionGetCharge(m);
                }

                if (ownm.handcard.card.name == CardDB.cardName.direwolfalpha)
                {
                    if (ownm.id == placeOfNewMob + 1 || ownm.id == placeOfNewMob - adjacentplace)
                    {
                        Enchantment e = CardDB.getEnchantmentFromCardID(CardDB.cardIDEnum.EX1_162o);
                        e.creator = ownm.entitiyID;
                        e.controllerOfCreator = controller;
                        addEffectToMinionNoDoubles(m, e, own);
                    }
                    else
                    {
                        //remove effect!!
                        Enchantment e = CardDB.getEnchantmentFromCardID(CardDB.cardIDEnum.EX1_162o);
                        e.creator = ownm.entitiyID;
                        e.controllerOfCreator = controller;
                        debuff(m, e, own);
                    }
                }
                if (ownm.handcard.card.name == CardDB.cardName.flametonguetotem)
                {
                    if (ownm.id == placeOfNewMob + 1 || ownm.id == placeOfNewMob - adjacentplace)
                    {
                        Enchantment e = CardDB.getEnchantmentFromCardID(CardDB.cardIDEnum.EX1_565o);
                        e.creator = ownm.entitiyID;
                        e.controllerOfCreator = controller;
                        addEffectToMinionNoDoubles(m, e, own);
                    }
                    else 
                    {
                        //remove effect!!
                        Enchantment e = CardDB.getEnchantmentFromCardID(CardDB.cardIDEnum.EX1_565o);
                        e.creator = ownm.entitiyID;
                        e.controllerOfCreator = controller;
                        debuff(m, e, own);
                    }

                }
            }
            //buff oldmurk
            if (isSummon && m.handcard.card.name == CardDB.cardName.oldmurkeye && own)
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

                if (ownm.handcard.card.name == CardDB.cardName.grimscaleoracle && m.handcard.card.race == 14 && ownm.entitiyID != m.entitiyID)
                {
                    Enchantment e = CardDB.getEnchantmentFromCardID(CardDB.cardIDEnum.EX1_508o);
                    e.creator = ownm.entitiyID;
                    e.controllerOfCreator = controller;
                    addEffectToMinionNoDoubles(m, e, own);
                }
                if (ownm.handcard.card.name == CardDB.cardName.murlocwarleader && m.handcard.card.race == 14 && ownm.entitiyID != m.entitiyID)
                {
                    Enchantment e = CardDB.getEnchantmentFromCardID(CardDB.cardIDEnum.EX1_507e);
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
                if (m.handcard.card.cardIDenum == CardDB.cardIDEnum.EX1_534)//m.name == CardDB.cardName.savannenhochmaehne"
                {
                    CardDB.Card c = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.EX1_534t);//hyena
                    callKid(c, m.id - 1, own);
                    callKid(c, m.id - 1, own);
                }

                if (m.name == CardDB.cardName.harvestgolem)
                {
                    CardDB.Card c = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.skele21);//damagedgolem
                    callKid(c, m.id - 1, own);
                }

                if (m.name == CardDB.cardName.cairnebloodhoof)
                {
                    CardDB.Card c = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.EX1_110);//bainebloodhoof
                    callKid(c, m.id - 1, own);
                    //penaltity for summon this thing :D (so we dont kill it only to have a new minion)
                    this.evaluatePenality += 5;


                }

                if (m.name == CardDB.cardName.thebeast)
                {
                    CardDB.Card c = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.EX1_finkle);//finkleeinhorn
                    int place = this.enemyMinions.Count-1;
                    if(!own) place = this.ownMinions.Count-1;
                    callKid(c, place, !own);

                }

                if (m.name == CardDB.cardName.lepergnome)
                {
                    attackOrHealHero(2, !own);
                }

                if (m.name == CardDB.cardName.loothoarder)
                {
                    if (own)
                    {
                        //this.owncarddraw++;
                        this.drawACard(CardDB.cardName.unknown, own);
                    }
                    else
                    {
                        this.drawACard(CardDB.cardName.unknown, own);
                        //this.enemycarddraw++;
                    }
                }




                if (m.name == CardDB.cardName.bloodmagethalnos)
                {
                    if (own)
                    {
                        //this.owncarddraw++;
                        this.drawACard(CardDB.cardName.unknown, own);
                    }
                    else
                    {
                        //this.enemycarddraw++;
                        this.drawACard(CardDB.cardName.unknown, own);
                    }
                }

                if (m.name == CardDB.cardName.abomination)
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


                if (m.name == CardDB.cardName.tirionfordring)
                {
                    if (own)
                    {
                        CardDB.Card c = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.EX1_383t);//ashbringer
                        this.equipWeapon(c);
                    }
                    else
                    {
                        this.enemyWeaponAttack = 5;
                        this.enemyWeaponDurability = 3;
                    }
                }

                if (m.name == CardDB.cardName.sylvanaswindrunner)
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

                if (m.handcard.card.name == CardDB.cardName.nerubianegg)
                {
                    CardDB.Card c = CardDB.Instance.getCardData(CardDB.cardName.nerubian);//nerubian
                    callKid(c, m.id - 1, own);
                }
                if (m.handcard.card.name == CardDB.cardName.dancingswords)
                {
                    this.drawACard(CardDB.cardName.unknown, !own);
                }

                if (m.handcard.card.name == CardDB.cardName.voidcaller)
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
                            callKid(mnn.card, this.ownMinions.Count-1, true);
                            removeCard(mnn);
                            break;
                        }

                    }
                    else
                    {
                        if (enemyAnzCards + enemycarddraw >= 1)
                        {
                            CardDB.Card c = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.EX1_301);//felguard
                            callKid(c, this.ownMinions.Count - 1, true);
                        }
                    }
                }

                if (m.handcard.card.name == CardDB.cardName.anubarambusher)
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

                if (m.handcard.card.name == CardDB.cardName.darkcultist)
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

                if (m.handcard.card.name == CardDB.cardName.webspinner)
                {
                    if (own)
                    {
                        this.drawACard(CardDB.cardName.rivercrocolisk, true);
                    }
                    else
                    {
                        this.drawACard(CardDB.cardName.rivercrocolisk, false);
                    }
                }



            }

            //deathrattle enchantments // these can be triggered after an silence (if they are casted after the silence)
            bool geistderahnen = false;
            foreach (Enchantment e in m.enchantments)
            {
                if (e.CARDID == CardDB.cardIDEnum.CS2_038e && !geistderahnen)
                {
                    //revive minion due to "geist der ahnen"
                    CardDB.Card kid = m.handcard.card;
                    int pos = this.ownMinions.Count - 1;
                    if (!own) pos = this.enemyMinions.Count - 1;
                    callKid(kid, pos, own);
                    geistderahnen = true;
                }
                //Seele des Waldes
                if (e.CARDID == CardDB.cardIDEnum.EX1_158e)
                {
                    //revive minion due to "geist der ahnen"
                    CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.EX1_158t);//Treant
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

                bool ancestral = false;
                if (m.enchantments.Count >= 1)
                {
                    foreach (Enchantment e in m.enchantments)
                    {
                        if (e.CARDID == CardDB.cardIDEnum.CS2_038e)
                        {
                            ancestral = true;
                            break;
                        }
                    }
                }

                if (m.handcard.card.name == CardDB.cardName.cairnebloodhoof || m.handcard.card.name == CardDB.cardName.harvestgolem || ancestral)
                {
                    this.evaluatePenality -= Ai.Instance.botBase.getEnemyMinionValue(m, this) - 1;
                }
            }

            foreach (Minion mnn in temp)
            {
                if (mnn.silenced) continue;

                if (mnn.handcard.card.name == CardDB.cardName.scavenginghyena && m.handcard.card.race == 20)
                {
                    mnn.Angr += 2; mnn.Hp += 1;
                }
                if (mnn.handcard.card.name == CardDB.cardName.flesheatingghoul)
                {
                    mnn.Angr += 1;
                }
                if (mnn.handcard.card.name == CardDB.cardName.cultmaster)
                {
                    if (own)
                    {
                        //this.owncarddraw++;
                        this.drawACard(CardDB.cardName.unknown, own);
                    }
                    else
                    {
                        //this.enemycarddraw++;
                        this.drawACard(CardDB.cardName.unknown, own);
                    }
                }
            }

            foreach (Minion mnn in temp2)
            {
                if (mnn.silenced) continue;
                if (mnn.handcard.card.name == CardDB.cardName.flesheatingghoul)
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
                hc.manacost = c.calculateManaCost(this) + manachange ;
                this.owncards.Add(hc);
            }
            else
            {
                removeMinionFromListNoDeath(m, this.enemyMinions, false);
                this.enemycarddraw++;
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

            if (m.frozen && m.numAttacksThisTurn == 0 && !(m.handcard.card.name == CardDB.cardName.ancientwatcher || m.name == CardDB.cardName.ragnarosthefirelord) && !m.playedThisTurn)
            {
                m.Ready = true;
            }


            m.frozen = false;

            if (!m.silenced && (m.handcard.card.name == CardDB.cardName.ancientwatcher || m.name == CardDB.cardName.ragnarosthefirelord) && !m.playedThisTurn && m.numAttacksThisTurn == 0)
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
            if (!m.silenced && (m.handcard.card.name == CardDB.cardName.ancientwatcher || m.name == CardDB.cardName.ragnarosthefirelord)) return;

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

            if (m.name == CardDB.cardName.lightspawn && !m.silenced)
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
            if (m.name == CardDB.cardName.angrychicken)
            {
                minionGetBuffed(m, -5, 0, own);
            }
            if (m.name == CardDB.cardName.amaniberserker)
            {
                minionGetBuffed(m, -3, 0, own);
            }
            if (m.name == CardDB.cardName.taurenwarrior)
            {
                minionGetBuffed(m, -3, 0, own);
            }
            if (m.name == CardDB.cardName.grommashhellscream)
            {
                minionGetBuffed(m, -6, 0, own);
            }
            if (m.name == CardDB.cardName.ragingworgen)
            {
                minionGetBuffed(m, -1, 0, own);
                minionGetWindfurry(m);
            }
            if (m.name == CardDB.cardName.spitefulsmith)
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
                if (m.name == CardDB.cardName.angrychicken)
                {
                    minionGetBuffed(m, 5, 0, own);
                }
                if (m.name == CardDB.cardName.amaniberserker)
                {
                    minionGetBuffed(m, 3, 0, own);
                }
                if (m.name == CardDB.cardName.taurenwarrior)
                {
                    minionGetBuffed(m, 3, 0, own);
                }
                if (m.name == CardDB.cardName.grommashhellscream)
                {
                    minionGetBuffed(m, 6, 0, own);
                }
                if (m.name == CardDB.cardName.ragingworgen)
                {
                    minionGetBuffed(m, 1, 0, own);
                    minionGetWindfurry(m);
                }
                if (m.name == CardDB.cardName.spitefulsmith)
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
                if (mnn.handcard.card.name == CardDB.cardName.lightwarden)
                {
                    minionGetBuffed(mnn, 2, 0, true);
                }
            }
            foreach (Minion mnn in this.enemyMinions)
            {
                if (mnn.silenced) continue;
                if (mnn.handcard.card.name == CardDB.cardName.lightwarden)
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
                if (mnn.handcard.card.name == CardDB.cardName.northshirecleric)
                {
                    //this.owncarddraw++;
                    this.drawACard(CardDB.cardName.unknown, true);
                }
                if (mnn.handcard.card.name == CardDB.cardName.lightwarden)
                {
                    minionGetBuffed(mnn, 2, 0, true);
                }
            }
            foreach (Minion mnn in this.enemyMinions)
            {
                if (mnn.silenced) continue;
                if (mnn.handcard.card.name == CardDB.cardName.northshirecleric)
                {
                    //this.enemycarddraw++;
                    this.drawACard(CardDB.cardName.unknown, false);
                }
                if (mnn.handcard.card.name == CardDB.cardName.lightwarden)
                {
                    minionGetBuffed(mnn, 2, 0, false);
                }
            }

        }

        private void triggerAMinionGetDamage(Minion m, bool own)
        {
            //minion take dmg
            if (m.handcard.card.name == CardDB.cardName.acolyteofpain && !m.silenced)
            {
                if (own)
                {
                    //this.owncarddraw++;
                    this.drawACard(CardDB.cardName.unknown, own);
                }
                else
                {
                    //this.enemycarddraw++;
                    this.drawACard(CardDB.cardName.unknown, own);
                }
            }
            if (m.handcard.card.name == CardDB.cardName.gurubashiberserker && !m.silenced)
            {
                minionGetBuffed(m, 3, 0, own);
            }
            foreach (Minion mnn in this.ownMinions)
            {
                if (mnn.silenced) continue;
                if (mnn.handcard.card.name == CardDB.cardName.frothingberserker)
                {
                    mnn.Angr++;
                }
                if (own)
                {
                    if (mnn.handcard.card.name == CardDB.cardName.armorsmith)
                    {
                        this.ownHeroDefence++;
                    }
                }
            }
            foreach (Minion mnn in this.enemyMinions)
            {
                if (mnn.silenced) continue;
                if (mnn.handcard.card.name == CardDB.cardName.frothingberserker)
                {
                    mnn.Angr++;
                }
                if (!own)
                {
                    if (mnn.handcard.card.name == CardDB.cardName.armorsmith)
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

        private void minionGetDamagedOrHealed(Minion m, int damages, int heals, bool own ,  bool dontCalcLostDmg=false, bool isMinionattack=false)
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
                        this.lostDamage += damage-1;
                    }
                    else
                    {
                        this.lostDamage += (damage-1) * (damage-1);
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



            if (m.Hp >  hpcopy)
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

            if (m.name == CardDB.cardName.lightspawn && !m.silenced)
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
            target.handcard.card = CardDB.Instance.getCardDataFromID(source.handcard.card.cardIDenum);
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
                if(this.enemyBaronRivendare) this.deathrattle(m, own);
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
                m.stealth = false;
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
                int oldhp = this.ownHeroHp;
                attackOrHealHero(m.Angr, false);
                if (oldhp > this.ownHeroHp)
                {
                    if (!m.silenced && m.handcard.card.name == CardDB.cardName.waterelemental) this.ownHeroFrozen = true;
                }
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
                minionGetDamagedOrHealed(enemy, ownAttack, 0, enemyOwn,false,true);
                if (!m.silenced && oldHP > enemy.Hp && m.handcard.card.name == CardDB.cardName.waterelemental) enemy.frozen = true;
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
                    minionGetDamagedOrHealed(m, enemyAttack, 0, attackOwn,false,true);
                    if (!enemy.silenced && oldHP > m.Hp && enemy.handcard.card.name == CardDB.cardName.waterelemental) m.frozen = true;
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
            int segenderweisheitAnzEnemy = 0;
            foreach (Enchantment e in ownMinion.enchantments)
            {
                if (e.CARDID == CardDB.cardIDEnum.EX1_363e2 )
                {
                    if (e.controllerOfCreator == this.ownController)
                    {
                        segenderweisheitAnz++;
                    }
                    else 
                    {
                        segenderweisheitAnzEnemy++;
                    }
                }
            }
            for (int i = 0; i < segenderweisheitAnz; i++)
            {
                this.drawACard(CardDB.cardName.unknown, true);
            }
            for (int i = 0; i < segenderweisheitAnzEnemy; i++)
            {
                this.drawACard(CardDB.cardName.unknown, false);
            }
        }

        public void ENEMYattackWithMinion(Minion ownMinion, int target, int targetEntity)
        {
            
            if (logging) Helpfunctions.Instance.logg("ennemy attck with" + ownMinion.name + " " + ownMinion.id + " trgt " + target + " A " + ownMinion.Angr + " H " + ownMinion.Hp);
            attack(ownMinion.id+10, target, false);
            //draw a card if the minion has enchantment from: Segen der weisheit 
            int segenderweisheitAnz = 0;
            int segenderweisheitAnzEnemy = 0;
            foreach (Enchantment e in ownMinion.enchantments)
            {
                if (e.CARDID == CardDB.cardIDEnum.EX1_363e2)
                {
                    if (e.controllerOfCreator == this.ownController)
                    {
                        segenderweisheitAnz++;
                    }
                    else
                    {
                        segenderweisheitAnzEnemy++;
                    }
                }
            }
            for (int i = 0; i < segenderweisheitAnz; i++)
            {
                this.drawACard(CardDB.cardName.unknown, true);
            }
            for (int i = 0; i < segenderweisheitAnzEnemy; i++)
            {
                this.drawACard(CardDB.cardName.unknown, false);
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

            if (m.name == CardDB.cardName.lightspawn && !m.silenced)
            {
                m.Angr = m.Hp;
            }

            this.getNewEffects(m, own, placeOfNewMob,true);

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


            if (c.name == CardDB.cardName.ancientoflore)
            {
                if (choice == 2)
                {
                    heal = 5;
                }
            }


            if (c.name == CardDB.cardName.keeperofthegrove)
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

            if (c.name == CardDB.cardName.crazedalchemist)
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

            if (c.name == CardDB.cardName.si7agent && this.cardsPlayedThisTurn >= 1)
            {
                damage = 2;
            }
            if (c.name == CardDB.cardName.kidnapper && this.cardsPlayedThisTurn >= 1)
            {
                backtohand = true;
                backtoHandManaChange = 0;
            }
            if (c.name == CardDB.cardName.masterofdisguise)
            {
                stealth = true;
            }

            if (c.name == CardDB.cardName.cabalshadowpriest)
            {
                minionGetControlled(m, true, false);
            }


            if (c.name == CardDB.cardName.ironbeakowl || c.name == CardDB.cardName.spellbreaker) //eisenschnabeleule, zauberbrecher
            {
                silence = true;
            }

            if (c.name == CardDB.cardName.shatteredsuncleric)
            {
                attackbuff = 1;
                hpbuff = 1;
            }

            if (c.name == CardDB.cardName.ancientbrewmaster)
            {
                backtohand = true;
                backtoHandManaChange = 0;
            }
            if (c.name == CardDB.cardName.youthfulbrewmaster)
            {
                backtohand = true;
                backtoHandManaChange = 0;
            }

            if (c.name == CardDB.cardName.darkirondwarf)
            {
                //attackbuff = 2;
                Enchantment e = CardDB.getEnchantmentFromCardID(CardDB.cardIDEnum.EX1_046e);
                e.creator = c.entitiyID;
                e.controllerOfCreator = this.ownController;
                addEffectToMinionNoDoubles(m, e, own);
            }

            if (c.name == CardDB.cardName.hungrycrab)
            {
                destroy = true;
                /*Enchantment e = CardDB.getEnchantmentFromCardID("NEW1_017e");
                e.creator = c.entitiyID;
                e.controllerOfCreator = this.ownController;
                addEffectToMinionNoDoubles(c, e, true);//buff own hungrige krabbe*/
                minionGetBuffed(c, 2, 2, true);
            }

            if (c.name == CardDB.cardName.abusivesergeant)
            {
                Enchantment e = CardDB.getEnchantmentFromCardID(CardDB.cardIDEnum.CS2_188o);
                e.creator = c.entitiyID;
                e.controllerOfCreator = this.ownController;
                addEffectToMinionNoDoubles(m, e, own);
            }
            if (c.name == CardDB.cardName.crueltaskmaster)
            {
                attackbuff = 2;
                damage = 1;
            }

            if (c.name == CardDB.cardName.frostelemental)
            {
                frozen = true;
            }

            if (c.name == CardDB.cardName.elvenarcher)
            {
                damage = 1;
            }
            if (c.name == CardDB.cardName.voodoodoctor)
            {
                if (this.auchenaiseelenpriesterin)
                { damage = 2; }
                else { heal = 2; }
            }
            if (c.name == CardDB.cardName.templeenforcer)
            {
                hpbuff = 3;
            }
            if (c.name == CardDB.cardName.ironforgerifleman)
            {
                damage = 1;
            }
            if (c.name == CardDB.cardName.stormpikecommando)
            {
                damage = 2;
            }
            if (c.name == CardDB.cardName.houndmaster)
            {
                attackbuff = 2;
                hpbuff = 2;
                spott = true;
            }

            if (c.name == CardDB.cardName.aldorpeacekeeper)
            {
                attackbuff = 1 - m.Angr;
            }

            if (c.name == CardDB.cardName.theblackknight)
            {
                destroy = true;
            }

            if (c.name == CardDB.cardName.argentprotector)
            {
                divineshild = true; // Grants NO buff
            }

            if (c.name == CardDB.cardName.windspeaker)
            {
                windfury = true;
            }
            if (c.name == CardDB.cardName.fireelemental)
            {
                damage = 3;
            }
            if (c.name == CardDB.cardName.earthenringfarseer)
            {
                if (this.auchenaiseelenpriesterin)
                { damage = 3; }
                else { heal = 3; }
            }
            if (c.name == CardDB.cardName.biggamehunter)
            {
                destroy = true;
            }

            if (c.name == CardDB.cardName.alexstrasza)
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

            if (c.name == CardDB.cardName.facelessmanipulator)
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
                if (backtohand) minionReturnToHand(m, false,backtoHandManaChange);
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
            if (c.name == CardDB.cardName.ancientofwar)
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

            if (c.name == CardDB.cardName.ancientoflore)
            {
                if (choice == 1)
                {
                    //this.owncarddraw += 2;
                    this.drawACard(CardDB.cardName.unknown, own);
                    this.drawACard(CardDB.cardName.unknown, own);
                }

            }

            if (c.name == CardDB.cardName.druidoftheclaw)
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

            if (c.name == CardDB.cardName.cenarius)
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

            if (c.name == CardDB.cardName.mindcontroltech)
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

            if (c.name == CardDB.cardName.felguard)
            {
                this.ownMaxMana--;
            }
            if (c.name == CardDB.cardName.arcanegolem)
            {
                this.enemyMaxMana++;
            }

            if (c.name == CardDB.cardName.edwinvancleef && this.cardsPlayedThisTurn >= 1)
            {
                minionGetBuffed(c, this.cardsPlayedThisTurn * 2, this.cardsPlayedThisTurn * 2, own);
            }

            if (c.name == CardDB.cardName.doomguard)
            {
                this.owncarddraw -= Math.Min(2, this.owncards.Count);
                this.owncards.RemoveRange(0, Math.Min(2, this.owncards.Count));
            }

            if (c.name == CardDB.cardName.succubus)
            {
                this.owncarddraw -= Math.Min(1, this.owncards.Count);
                this.owncards.RemoveRange(0, Math.Min(1, this.owncards.Count));
            }

            if (c.name == CardDB.cardName.lordjaraxxus)
            {
                this.ownHeroAblility = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.EX1_tk33);
                this.ownHeroName = HeroEnum.lordjaraxxus;
                this.ownHeroHp = c.Hp;
            }

            if (c.name == CardDB.cardName.flameimp)
            {
                attackOrHealHero(3, own);
            }
            if (c.name == CardDB.cardName.pitlord)
            {
                attackOrHealHero(5, own);
            }

            if (c.name == CardDB.cardName.voidterror)
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

            if (c.name == CardDB.cardName.frostwolfwarlord)
            {
                minionGetBuffed(c, this.ownMinions.Count, this.ownMinions.Count, own);
            }
            if (c.name == CardDB.cardName.bloodsailraider)
            {
                c.Angr += this.ownWeaponAttack;
            }

            if (c.name == CardDB.cardName.southseadeckhand && this.ownWeaponDurability >= 1)
            {
                minionGetCharge(c);
            }



            if (c.name == CardDB.cardName.bloodknight)
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

            if (c.name == CardDB.cardName.kingmukla)
            {
                this.enemycarddraw += 2;
            }

            if (c.name == CardDB.cardName.coldlightoracle)
            {
                //this.enemycarddraw += 2;
                //this.owncarddraw += 2;
                this.drawACard(CardDB.cardName.unknown, true);
                this.drawACard(CardDB.cardName.unknown, true);
                this.drawACard(CardDB.cardName.unknown, false);
                this.drawACard(CardDB.cardName.unknown, false);
            }

            if (c.name == CardDB.cardName.arathiweaponsmith)
            {
                CardDB.Card wcard = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.EX1_398t);//battleaxe
                this.equipWeapon(wcard);
                

            }
            if (c.name == CardDB.cardName.bloodsailcorsair)
            {
                this.lowerWeaponDurability(1, false);
            }

            if (c.name == CardDB.cardName.acidicswampooze)
            {
                this.lowerWeaponDurability(1000, false);
            }
            if (c.name == CardDB.cardName.noviceengineer)
            {
                //this.owncarddraw++;
                drawACard(CardDB.cardName.unknown, true);
            }
            if (c.name == CardDB.cardName.gnomishinventor)
            {
                //this.owncarddraw++;
                drawACard(CardDB.cardName.unknown, true);
            }

            if (c.name == CardDB.cardName.darkscalehealer)
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
            if (c.name == CardDB.cardName.nightblade)
            {
                attackOrHealHero(3, !own);
            }

            if (c.name == CardDB.cardName.twilightdrake)
            {
                minionGetBuffed(c, 0, this.owncards.Count, true);
            }

            if (c.name == CardDB.cardName.azuredrake)
            {
                //this.owncarddraw++;
                drawACard(CardDB.cardName.unknown, true);
            }

            if (c.name == CardDB.cardName.harrisonjones)
            {
                this.enemyWeaponAttack = 0;
                //this.owncarddraw += enemyWeaponDurability;
                for (int i = 0; i < enemyWeaponDurability; i++)
                {
                    drawACard(CardDB.cardName.unknown, true);
                }
                this.enemyWeaponDurability = 0;
            }

            if (c.name == CardDB.cardName.guardianofkings)
            {
                attackOrHealHero(-6, true);
            }

            if (c.name == CardDB.cardName.captaingreenskin)
            {
                if (this.ownWeaponName != CardDB.cardName.unknown)
                {
                    this.ownheroAngr += 1;
                    this.ownWeaponAttack++;
                    this.ownWeaponDurability++;
                }
            }

            if (c.name == CardDB.cardName.priestessofelune)
            {
                attackOrHealHero(-4, true);
            }
            if (c.name == CardDB.cardName.injuredblademaster)
            {
                minionGetDamagedOrHealed(c, 4, 0, true);
            }

            if (c.name == CardDB.cardName.dreadinfernal)
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

            if (c.name == CardDB.cardName.tundrarhino)
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

            if (c.name == CardDB.cardName.stampedingkodo)
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

            if (c.name == CardDB.cardName.sunfuryprotector)
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

            if (c.name == CardDB.cardName.ancientmage)
            {
                List<Minion> temp = new List<Minion>(this.ownMinions);
                foreach (Minion m in temp)
                {
                    if (m.id == position - 1 || m.id == position )
                    {
                        m.handcard.card.spellpowervalue++;
                    }
                }
            }

            if (c.name == CardDB.cardName.defenderofargus)
            {
                List<Minion> temp = new List<Minion>(this.ownMinions);
                foreach (Minion m in temp)
                {
                    if (m.id == position - 1 || m.id == position)//position and position -1 because its not placed jet
                    {
                        Enchantment e = CardDB.getEnchantmentFromCardID(CardDB.cardIDEnum.EX1_093e);
                        e.creator = c.entitiyID;
                        e.controllerOfCreator = this.ownController;
                        addEffectToMinionNoDoubles(m, e, own);
                    }
                }
            }

            if (c.name == CardDB.cardName.coldlightseer)
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

            if (c.name == CardDB.cardName.deathwing)
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

            if (c.name == CardDB.cardName.captainsparrot)
            {
                //this.owncarddraw++;
                drawACard(CardDB.cardName.unknown, true);

            }



        }

        private int spawnKids(CardDB.Card c, int position, bool own, int choice)
        {
            int kids = 0;
            if (c.name == CardDB.cardName.murloctidehunter)
            {
                kids = 1;
                CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.EX1_506a);//murlocscout
                callKid(kid, position, own,true);

            }
            if (c.name == CardDB.cardName.razorfenhunter)
            {
                kids = 1;
                CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.CS2_boar);//boar
                callKid(kid, position, own, true);

            }
            if (c.name == CardDB.cardName.dragonlingmechanic)
            {
                kids = 1;
                CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.EX1_025t);//mechanicaldragonling
                callKid(kid, position, own, true);

            }
            if (c.name == CardDB.cardName.leeroyjenkins)
            {
                kids = 2;
                CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.EX1_116t);//whelp
                int pos = this.ownMinions.Count - 1;
                if (own) pos = this.enemyMinions.Count - 1;
                callKid(kid, pos, !own);
                callKid(kid, pos, !own);

            }

            if (c.name == CardDB.cardName.cenarius && choice == 2)
            {
                kids = 2;
                CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.EX1_573t); //special treant
                int pos = this.ownMinions.Count - 1;
                if (!own) pos = this.enemyMinions.Count - 1;
                callKid(kid, pos, own, true);
                callKid(kid, pos, own, true);

            }
            if (c.name == CardDB.cardName.silverhandknight)
            {
                kids = 1;
                CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.CS2_152);//squire
                callKid(kid, position, own, true);

            }
            if (c.name == CardDB.cardName.gelbinmekkatorque)
            {
                kids = 1;
                CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.Mekka1);//homingchicken
                callKid(kid, position, own, true);

            }

            if (c.name == CardDB.cardName.defiasringleader && this.cardsPlayedThisTurn >= 1) //needs combo for spawn
            {
                kids = 1;
                CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.EX1_131t);//defiasbandit
                callKid(kid, position, own, true);

            }
            if (c.name == CardDB.cardName.onyxia)
            {
                kids = 7 - this.ownMinions.Count;
                CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.EX1_116t);//whelp
                for (int i = 0; i < kids; i++)
                {
                        callKid(kid, position, own, true);
                }

            }
            return kids;
        }

        private void callKid(CardDB.Card c, int placeoffather, bool own, bool spawnKid = false)
        {
            if (own)
            {
                if(!spawnKid && this.ownMinions.Count >= 7) return;
                if (spawnKid && this.ownMinions.Count >= 6)
                {
                    this.evaluatePenality += 20;
                    return;
                }
            }
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
            int spawnkids = spawnKids(hc.card, mobplace-1, true, choice); //  if a mob targets something, it doesnt spawn minions!?
            

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
                    if (this.ownWeaponName == CardDB.cardName.deathsbite)
                    {
                        int anz =1;
                        if(this.ownBaronRivendare) anz=2;
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
                    this.ownWeaponName = CardDB.cardName.unknown;

                    foreach (Minion m in this.ownMinions)
                    {
                        if (m.playedThisTurn && m.name == CardDB.cardName.southseadeckhand)
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
                    if (this.enemyWeaponName == CardDB.cardName.deathsbite)
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
                    this.enemyWeaponName = CardDB.cardName.unknown;
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
            if (c.name == CardDB.cardName.doomhammer)
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
            if (c.name == CardDB.cardName.gladiatorslongbow)
            {
                this.heroImmuneWhileAttacking = true;
            }
            else
            {
                this.heroImmuneWhileAttacking = false;
            }

            foreach (Minion m in this.ownMinions)
            {
                if (m.playedThisTurn && m.name == CardDB.cardName.southseadeckhand)
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

            if (c.name == CardDB.cardName.execute)
            {
                destroy = true;
            }

            if (c.name == CardDB.cardName.innerrage)
            {
                damage = 1;
                attackbuff = 2;
            }

            if (c.name == CardDB.cardName.slam)
            {
                damage = 2;
                if (m.Hp >= 3)
                {
                    //this.owncarddraw++;
                    drawACard(CardDB.cardName.unknown, true);
                }
            }

            if (c.name == CardDB.cardName.mortalstrike)
            {
                damage = 4;
                if (ownHeroHp <= 12) damage = 6;
            }

            if (c.name == CardDB.cardName.shieldslam)
            {
                damage = this.ownHeroDefence;
            }

            if (c.name == CardDB.cardName.charge)
            {
                charge = true;
                attackbuff = 2;
            }

            if (c.name == CardDB.cardName.rampage)
            {
                attackbuff = 3;
                hpbuff = 3;
            }

            //hunter#################################################################################

            if (c.name == CardDB.cardName.huntersmark)
            {
                setHPtoONE = true;
            }
            if (c.name == CardDB.cardName.arcaneshot)
            {
                damage = 2;
            }
            if (c.name == CardDB.cardName.killcommand)
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
            if (c.name == CardDB.cardName.bestialwrath)
            {

                Enchantment e = CardDB.getEnchantmentFromCardID(CardDB.cardIDEnum.EX1_549o);
                e.creator = hc.entity;
                e.controllerOfCreator = this.ownController;
                addEffectToMinionNoDoubles(m, e, own);
            }

            if (c.name == CardDB.cardName.explosiveshot)
            {
                damage = 5;
                adjacentDamage = 2;
            }

            //mage###############################################################################

            if (c.name == CardDB.cardName.icelance)
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
                    if (target ==100)
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

            if (c.name == CardDB.cardName.coneofcold)
            {
                damage = 1;
                adjacentDamage = 1;
                frozen = true;
            }
            if (c.name == CardDB.cardName.fireball)
            {
                damage = 6;
            }
            if (c.name == CardDB.cardName.polymorph)
            {
                sheep = true;
            }

            if (c.name == CardDB.cardName.pyroblast)
            {
                damage = 10;
            }

            if (c.name == CardDB.cardName.frostbolt)
            {
                damage = 3;
                frozen = true;
            }

            //pala######################################################################

            if (c.name == CardDB.cardName.humility)
            {
                m.Angr = 1;
            }
            if (c.name == CardDB.cardName.handofprotection)
            {
                divineshild = true;
            }
            if (c.name == CardDB.cardName.blessingofmight)
            {
                attackbuff = 3;
            }
            if (c.name == CardDB.cardName.holylight)
            {
                heal = 6;
            }

            if (c.name == CardDB.cardName.hammerofwrath)
            {
                damage = 3;
                //this.owncarddraw++;
                drawACard(CardDB.cardName.unknown, true);
            }

            if (c.name == CardDB.cardName.blessingofkings)
            {
                attackbuff = 4;
                hpbuff = 4;
            }

            if (c.name == CardDB.cardName.blessingofwisdom)
            {
                Enchantment e = CardDB.getEnchantmentFromCardID(CardDB.cardIDEnum.EX1_363e2);
                e.creator = hc.entity;
                e.controllerOfCreator = this.ownController;
                m.enchantments.Add(e);
            }

            if (c.name == CardDB.cardName.blessedchampion)
            {
                m.Angr *= 2;
            }
            if (c.name == CardDB.cardName.holywrath)
            {
                damage = 3;
                //this.owncarddraw++;
                drawACard(CardDB.cardName.unknown, true);
            }
            if (c.name == CardDB.cardName.layonhands)
            {
                for (int i = 0; i < 3; i++)
                {
                    //this.owncarddraw++;
                    drawACard(CardDB.cardName.unknown, true);
                }
                heal = 8;
            }

            //priest ##########################################

            if (c.name == CardDB.cardName.shadowmadness)
            {

                Enchantment e = CardDB.getEnchantmentFromCardID(CardDB.cardIDEnum.EX1_334e);
                e.creator = hc.entity;
                e.controllerOfCreator = this.ownController;
                addEffectToMinionNoDoubles(m, e, own);
                this.minionGetControlled(m, true, true);
            }

            if (c.name == CardDB.cardName.mindcontrol)
            {
                this.minionGetControlled(m, true, false);
            }

            if (c.name == CardDB.cardName.holysmite)
            {
                damage = 2;
            }
            if (c.name == CardDB.cardName.powerwordshield)
            {
                hpbuff = 2;
                //this.owncarddraw++;
                drawACard(CardDB.cardName.unknown, true);
            }
            if (c.name == CardDB.cardName.silence)
            {
                silence = true;
            }
            if (c.name == CardDB.cardName.divinespirit)
            {
                hpbuff = m.Hp;
            }
            if (c.name == CardDB.cardName.innerfire)
            {
                m.Angr = m.Hp;
            }
            if (c.name == CardDB.cardName.holyfire)
            {
                damage = 5;
                int ownheal = getSpellHeal(5);
                attackOrHealHero(-ownheal, true);
            }
            if (c.name == CardDB.cardName.shadowwordpain)
            {
                destroy = true;
            }
            if (c.name == CardDB.cardName.shadowworddeath)
            {
                destroy = true;
            }
            //rogue ##########################################
            if (c.name == CardDB.cardName.shadowstep)
            {
                backtohand = true;
                backtoHandManaChange = -2;
                //m.handcard.card.cost = Math.Max(0, m.handcard.card.cost -= 2);
            }
            if (c.name == CardDB.cardName.sap)
            {
                backtohand = true;
                backtoHandManaChange = 0;
            }
            if (c.name == CardDB.cardName.shiv)
            {
                damage = 1;
                //this.owncarddraw++;
                drawACard(CardDB.cardName.unknown, true);
            }
            if (c.name == CardDB.cardName.coldblood)
            {
                attackbuff = 2;
                if (this.cardsPlayedThisTurn >= 1) attackbuff = 4;
            }
            if (c.name == CardDB.cardName.conceal)
            {
                stealth = true;
            }
            if (c.name == CardDB.cardName.eviscerate)
            {
                damage = 2;
                if (this.cardsPlayedThisTurn >= 1) damage = 4;
            }
            if (c.name == CardDB.cardName.betrayal)
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

            if (c.name == CardDB.cardName.perditionsblade)
            {
                damage = 1;
                if (this.cardsPlayedThisTurn >= 1) damage = 2;
            }

            if (c.name == CardDB.cardName.backstab)
            {
                damage = 2;
            }

            if (c.name == CardDB.cardName.assassinate)
            {
                destroy = true;
            }
            //shaman ##########################################
            if (c.name == CardDB.cardName.lightningbolt)
            {
                damage = 3;
            }
            if (c.name == CardDB.cardName.frostshock)
            {
                frozen = true;
                damage = 1;
            }
            if (c.name == CardDB.cardName.rockbiterweapon)
            {
                if (target <= 20)
                {
                    Enchantment e = CardDB.getEnchantmentFromCardID(CardDB.cardIDEnum.CS2_045e);
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
            if (c.name == CardDB.cardName.windfury)
            {
                windfury = true;
            }
            if (c.name == CardDB.cardName.hex)
            {
                frogg = true;
            }
            if (c.name == CardDB.cardName.earthshock)
            {
                silence = true;
                damage = 1;
            }
            if (c.name == CardDB.cardName.ancestralspirit)
            {
                geistderahnen = true;
            }
            if (c.name == CardDB.cardName.lavaburst)
            {
                damage = 5;
            }

            if (c.name == CardDB.cardName.ancestralhealing)
            {
                heal = 1000;
                spott = true;
            }

            //hexenmeister ##########################################

            if (c.name == CardDB.cardName.sacrificialpact)
            {
                destroy = true;
                this.attackOrHealHero(getSpellHeal(5), true); // heal own hero
            }

            if (c.name == CardDB.cardName.soulfire)
            {
                damage = 4;
                this.owncarddraw--;
                this.owncards.RemoveRange(0, Math.Min(1, this.owncards.Count));
                

            }
            if (c.name == CardDB.cardName.poweroverwhelming)
            {
                //only to own mininos
                Enchantment e = CardDB.getEnchantmentFromCardID(CardDB.cardIDEnum.EX1_316e);
                e.creator = hc.entity;
                e.controllerOfCreator = this.ownController;
                addEffectToMinionNoDoubles(m, e, true);
            }
            if (c.name == CardDB.cardName.corruption)
            {
                //only to enemy mininos
                Enchantment e = CardDB.getEnchantmentFromCardID(CardDB.cardIDEnum.CS2_063e);
                e.creator = hc.entity;
                e.controllerOfCreator = this.ownController;
                addEffectToMinionNoDoubles(m, e, false);
            }
            if (c.name == CardDB.cardName.mortalcoil)
            {
                damage = 1;
                if (getSpellDamageDamage(1) >= m.Hp && !m.divineshild && !m.immune)
                {
                    //this.owncarddraw++;
                    drawACard(CardDB.cardName.unknown, true);
                }
            }
            if (c.name == CardDB.cardName.drainlife)
            {
                damage = 2;
                attackOrHealHero(2, true);
            }
            if (c.name == CardDB.cardName.shadowbolt)
            {
                damage = 4;
            }
            if (c.name == CardDB.cardName.shadowflame)
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

            if (c.name == CardDB.cardName.demonfire)
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
            if (c.name == CardDB.cardName.baneofdoom)
            {
                damage = 2;
                if (getSpellDamageDamage(2) >= m.Hp && !m.divineshild && !m.immune)
                {
                    int posi = this.ownMinions.Count - 1;
                    CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.CS2_059);//bloodimp
                    callKid(kid, posi, true);
                }
            }

            if (c.name == CardDB.cardName.siphonsoul)
            {
                destroy = true;
                int h = getSpellHeal(3);
                attackOrHealHero(-h, true);

            }


            //druid #######################################################################

            if (c.name == CardDB.cardName.moonfire && c.cardIDenum == CardDB.cardIDEnum.CS2_008)// nicht zu verwechseln mit cenarius choice nummer 1
            {
                damage = 1;
            }

            if (c.name == CardDB.cardName.markofthewild)
            {
                spott = true;
                attackbuff = 2;
                hpbuff = 2;
            }

            if (c.name == CardDB.cardName.healingtouch)
            {
                heal = 8;
            }

            if (c.name == CardDB.cardName.starfire)
            {
                damage = 5;
                //this.owncarddraw++;
                drawACard(CardDB.cardName.unknown, true);
            }

            if (c.name == CardDB.cardName.naturalize)
            {
                destroy = true;
                this.enemycarddraw += 2;
            }

            if (c.name == CardDB.cardName.savagery)
            {
                damage = this.ownheroAngr;
            }

            if (c.name == CardDB.cardName.swipe)
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
            if (c.name == CardDB.cardName.wrath)
            {
                if (choice == 1)
                {
                    damage = 3;
                }
                if (choice == 2)
                {
                    damage = 1;
                    //this.owncarddraw++;
                    drawACard(CardDB.cardName.unknown, true);
                }
            }

            if (c.name == CardDB.cardName.markofnature)
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

            if (c.name == CardDB.cardName.starfall)
            {
                if (choice == 1)
                {
                    damage = 5;
                }

            }


            //special cards#########################################################################################

            if (c.name == CardDB.cardName.nightmare)
            {
                //only to own mininos
                Enchantment e = CardDB.getEnchantmentFromCardID(CardDB.cardIDEnum.EX1_316e);
                e.creator = hc.entity;
                e.controllerOfCreator = this.ownController;
                addEffectToMinionNoDoubles(m, e, true);
            }

            if (c.name == CardDB.cardName.dream)
            {
                backtohand = true;
                backtoHandManaChange = 0;
            }

            if (c.name == CardDB.cardName.bananas)
            {
                attackbuff = 1;
                hpbuff = 1;
            }

            if (c.name == CardDB.cardName.barreltoss)
            {
                damage = 2;
            }

            if (c.cardIDenum == CardDB.cardIDEnum.PRO_001b)// i am murloc
            {
                damage = 4;
                //this.owncarddraw++;
                drawACard(CardDB.cardName.unknown, true);

            } if (c.name == CardDB.cardName.willofmukla)
            {
                heal = 6 ;
            }

            //NaxxCards###################################################################################
            if (c.name == CardDB.cardName.reincarnation)
            {
                int place = m.id;
                if(place >=10) place -=10;
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
                if (backtohand) minionReturnToHand(m, true,backtoHandManaChange );
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
                if (sheep) minionTransform(m, CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.CS2_tk1), own);
                if (frogg) minionTransform(m, CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.hexfrog), own);
                if (setHPtoONE)
                {
                    m.Hp = 1; m.maxHp = 1;
                }

                if (geistderahnen)
                {
                    Enchantment e = CardDB.getEnchantmentFromCardID(CardDB.cardIDEnum.CS2_038e);
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
                if (backtohand) minionReturnToHand(m, false,backtoHandManaChange);
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
                if (sheep) minionTransform(m, CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.CS2_tk1), own);
                if (frogg) minionTransform(m, CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.hexfrog), own);
                if (setHPtoONE)
                {
                    m.Hp = 1; m.maxHp = 1;
                }
                if (geistderahnen)
                {
                    Enchantment e = CardDB.getEnchantmentFromCardID(CardDB.cardIDEnum.CS2_038e);
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
            if (c.name == CardDB.cardName.thecoin)
            {
                this.mana++;

            }
            //hunter#########################################################################
            if (c.name == CardDB.cardName.multishot && this.enemyMinions.Count >= 2)
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
            if (c.name == CardDB.cardName.animalcompanion)
            {
                CardDB.Card c2 = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.NEW1_032);//misha
                int placeoffather = this.ownMinions.Count - 1;
                callKid(c2, placeoffather, true);
            }

            if (c.name == CardDB.cardName.flare)
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
                drawACard(CardDB.cardName.unknown, true);
                this.enemySecretCount = 0;
            }

            if (c.name == CardDB.cardName.unleashthehounds)
            {
                int anz = this.enemyMinions.Count;
                int posi = this.ownMinions.Count - 1;
                CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.EX1_538t);//hound
                for (int i = 0; i < anz; i++)
                {
                    callKid(kid, posi, true);
                }
            }

            if (c.name == CardDB.cardName.deadlyshot && this.enemyMinions.Count >= 1)
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
            if (c.name == CardDB.cardName.commandingshout)
            {
                List<Minion> temp = new List<Minion>(this.ownMinions);
                Enchantment e1 = CardDB.getEnchantmentFromCardID(CardDB.cardIDEnum.NEW1_036e);
                e1.creator = hc.entity;
                e1.controllerOfCreator = this.ownController;
                Enchantment e2 = CardDB.getEnchantmentFromCardID(CardDB.cardIDEnum.NEW1_036e2);
                e2.creator = hc.entity;
                e2.controllerOfCreator = this.ownController;
                foreach (Minion mnn in temp)
                {//cantLowerHPbelowONE
                    addEffectToMinionNoDoubles(mnn, e1, true);
                    addEffectToMinionNoDoubles(mnn, e2, true);
                    mnn.cantLowerHPbelowONE = true;
                }

            }

            if (c.name == CardDB.cardName.battlerage)
            {
                foreach (Minion mnn in this.ownMinions)
                {
                    if (mnn.wounded)
                    {
                        //this.owncarddraw++;
                        drawACard(CardDB.cardName.unknown, true);
                    }
                }

            }

            if (c.name == CardDB.cardName.brawl)
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
                    minionGetDestroyed(mnn,false);
                }

            }


            if (c.name == CardDB.cardName.cleave && this.enemyMinions.Count >= 2)
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

            if (c.name == CardDB.cardName.upgrade)
            {
                if (this.ownWeaponName != CardDB.cardName.unknown)
                {
                    this.ownWeaponAttack++;
                    this.ownheroAngr++;
                    this.ownWeaponDurability++;
                }
                else
                {
                    CardDB.Card wcard = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.EX1_409t);//heavyaxe
                    this.equipWeapon(wcard);
                }

            }



            if (c.name == CardDB.cardName.whirlwind)
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

            if (c.name == CardDB.cardName.heroicstrike)
            {
                this.ownheroAngr = this.ownheroAngr + 4;
                if ((this.ownHeroNumAttackThisTurn == 0 || (this.ownHeroWindfury && this.ownHeroNumAttackThisTurn == 1)) && !this.ownHeroFrozen) 
                {
                    this.ownHeroReady = true;
                }
            }

            if (c.name == CardDB.cardName.shieldblock)
            {
                this.ownHeroDefence = this.ownHeroDefence + 5;
                //this.owncarddraw++;
                drawACard(CardDB.cardName.unknown, true);
            }



            //mage#########################################################################################

            if (c.name == CardDB.cardName.blizzard)
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

            if (c.name == CardDB.cardName.arcanemissiles)
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
            if (c.name == CardDB.cardName.arcaneintellect)
            {
                //this.owncarddraw++;
                drawACard(CardDB.cardName.unknown, true);
                drawACard(CardDB.cardName.unknown, true);
            }

            if (c.name == CardDB.cardName.mirrorimage)
            {
                int posi = this.ownMinions.Count - 1;
                CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.CS2_mirror);
                callKid(kid, posi, true);
                callKid(kid, posi, true);
            }

            if (c.name == CardDB.cardName.arcaneexplosion)
            {
                List<Minion> temp = new List<Minion>(this.enemyMinions);
                int damage = getSpellDamageDamage(1);
                foreach (Minion enemy in temp)
                {
                    minionGetDamagedOrHealed(enemy, damage, 0, false);
                }
            }
            if (c.name == CardDB.cardName.frostnova)
            {
                List<Minion> temp = new List<Minion>(this.enemyMinions);
                foreach (Minion enemy in temp)
                {
                    enemy.frozen = true;
                }

            }
            if (c.name == CardDB.cardName.flamestrike)
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
            if (c.name == CardDB.cardName.consecration)
            {
                List<Minion> temp = new List<Minion>(this.enemyMinions);
                int damage = getSpellDamageDamage(2);
                foreach (Minion enemy in temp)
                {
                    minionGetDamagedOrHealed(enemy, damage, 0, false);
                }

                attackOrHealHero(damage, false);
            }

            if (c.name == CardDB.cardName.equality)
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
            if (c.name == CardDB.cardName.divinefavor)
            {
                int enemcardsanz = this.enemyAnzCards + this.enemycarddraw;
                int diff = enemcardsanz - this.owncards.Count;
                if (diff >= 1)
                {
                    for (int i = 0; i < diff; i++)
                    {
                        //this.owncarddraw++;
                        drawACard(CardDB.cardName.unknown, true);
                    }
                }
            }

            if (c.name == CardDB.cardName.avengingwrath)
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
            if (c.name == CardDB.cardName.circleofhealing)
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
            if (c.name == CardDB.cardName.thoughtsteal)
            {
                //this.owncarddraw++;
                this.drawACard(CardDB.cardName.unknown, true, true);
                //this.owncarddraw++;
                this.drawACard(CardDB.cardName.unknown, true, true);
            }
            if (c.name == CardDB.cardName.mindvision)
            {
                if (this.enemyAnzCards+this.enemycarddraw >= 1)
                {
                    //this.owncarddraw++;
                    this.drawACard(CardDB.cardName.unknown, true, true);
                }
            }

            if (c.name == CardDB.cardName.shadowform)
            {
                if (this.ownHeroAblility.cardIDenum == CardDB.cardIDEnum.CS1h_001) // lesser heal becomes mind spike
                {
                    this.ownHeroAblility = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.EX1_625t);
                    this.ownAbilityReady = true;
                }
                else
                {
                    this.ownHeroAblility = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.EX1_625t2);  // mindspike becomes mind shatter
                    this.ownAbilityReady = true;
                }
            }

            if (c.name == CardDB.cardName.mindgames)
            {
                CardDB.Card copymin = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.CS2_152); //we draw a knappe :D (worst case)
                callKid(copymin, this.ownMinions.Count - 1, true);
            }

            if (c.name == CardDB.cardName.massdispel)
            {
                foreach (Minion m in this.enemyMinions)
                {
                    minionGetSilenced(m, false);
                }
            }
            if (c.name == CardDB.cardName.mindblast)
            {
                int damage = getSpellDamageDamage(5);
                attackOrHealHero(damage, false);
            }

            if (c.name == CardDB.cardName.holynova)
            {
                List<Minion> temp = new List<Minion>(this.ownMinions);
                int heal = getSpellHeal(2);
                int damage = getSpellDamageDamage(2);
                foreach (Minion enemy in temp)
                {
                    minionGetDamagedOrHealed(enemy, 0, heal,true,true);
                }
                attackOrHealHero(-heal, true);
                temp.Clear();
                temp.AddRange(this.enemyMinions);
                foreach (Minion enemy in temp)
                {
                    minionGetDamagedOrHealed(enemy, damage, 0, false,true);
                }
                attackOrHealHero(damage, false);

            }
            //rogue #################################################
            if (c.name == CardDB.cardName.preparation)
            {
                this.playedPreparation = true;
            }
            if (c.name == CardDB.cardName.bladeflurry)
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
            if (c.name == CardDB.cardName.headcrack)
            {
                int damage = getSpellDamageDamage(2);
                attackOrHealHero(damage, false);
                if (this.cardsPlayedThisTurn >= 1) this.owncarddraw++; // DONT DRAW A CARD WITH (drawAcard()) because we get this NEXT turn 
            }
            if (c.name == CardDB.cardName.sinisterstrike)
            {
                int damage = getSpellDamageDamage(3);
                attackOrHealHero(damage, false);
            }
            if (c.name == CardDB.cardName.deadlypoison)
            {
                if (this.ownWeaponName != CardDB.cardName.unknown)
                {
                    this.ownWeaponAttack += 2;
                    this.ownheroAngr += 2;

                }
            }
            if (c.name == CardDB.cardName.fanofknives)
            {
                List<Minion> temp = new List<Minion>(this.enemyMinions);
                int damage = getSpellDamageDamage(1);
                foreach (Minion enemy in temp)
                {
                    minionGetDamagedOrHealed(enemy, damage, 0, false);
                }
                drawACard(CardDB.cardName.unknown, true);
            }

            if (c.name == CardDB.cardName.sprint)
            {
                for (int i = 0; i < 4; i++)
                {
                    //this.owncarddraw++;
                    drawACard(CardDB.cardName.unknown, true);
                }

            }

            if (c.name == CardDB.cardName.vanish)
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
            if (c.name == CardDB.cardName.forkedlightning && this.enemyMinions.Count >= 2)
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

            if (c.name == CardDB.cardName.farsight)
            {
                //this.owncarddraw++;
                drawACard(CardDB.cardName.unknown, true);

            }

            if (c.name == CardDB.cardName.lightningstorm)
            {
                List<Minion> temp = new List<Minion>(this.enemyMinions);
                int damage = getSpellDamageDamage(2);

                int maxHp = 0;
                foreach (Minion enemy in temp)
                {
                    if (maxHp < enemy.Hp) maxHp = enemy.Hp;

                    minionGetDamagedOrHealed(enemy, damage, 0, false,true);
                }
                this.lostDamage += Math.Max(0, damage - maxHp); 

            }

            if (c.name == CardDB.cardName.feralspirit)
            {
                int posi = this.ownMinions.Count - 1;
                CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.EX1_tk11);//spiritwolf
                callKid(kid, posi, true);
                callKid(kid, posi, true);
            }

            if (c.name == CardDB.cardName.totemicmight)
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

            if (c.name == CardDB.cardName.bloodlust)
            {
                List<Minion> temp = new List<Minion>(this.ownMinions);
                foreach (Minion m in temp)
                {
                    Enchantment e = CardDB.getEnchantmentFromCardID(CardDB.cardIDEnum.CS2_046e);
                    e.creator = this.ownController;
                    e.controllerOfCreator = this.ownController;
                    addEffectToMinionNoDoubles(m, e, true);
                }
            }


            //hexenmeister #################################################
            if (c.name == CardDB.cardName.sensedemons)
            {
                //this.owncarddraw += 2;
                this.drawACard(CardDB.cardName.unknown, true);
                this.drawACard(CardDB.cardName.unknown, true);


            }
            if (c.name == CardDB.cardName.twistingnether)
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

            if (c.name == CardDB.cardName.hellfire)
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
            if (c.name == CardDB.cardName.souloftheforest)
            {
                List<Minion> temp = new List<Minion>(this.ownMinions);
                Enchantment e = CardDB.getEnchantmentFromCardID(CardDB.cardIDEnum.EX1_158e);
                e.creator = hc.entity;
                e.controllerOfCreator = this.ownController;
                foreach (Minion enemy in temp)
                {
                    addEffectToMinionNoDoubles(enemy, e, true);
                }
            }

            if (c.name == CardDB.cardName.innervate)
            {
                this.mana = Math.Min(this.mana + 2  ,10);

            }

            if (c.name == CardDB.cardName.bite)
            {
                this.ownheroAngr += 4;
                this.ownHeroDefence += 4;
                if ((this.ownHeroNumAttackThisTurn == 0 || (this.ownHeroWindfury && this.ownHeroNumAttackThisTurn == 1)) && !this.ownHeroFrozen)
                {
                    this.ownHeroReady = true;
                }

            }

            if (c.name == CardDB.cardName.claw)
            {
                this.ownheroAngr += 2;
                this.ownHeroDefence += 2;
                if ((this.ownHeroNumAttackThisTurn == 0 || (this.ownHeroWindfury && this.ownHeroNumAttackThisTurn == 1)) && !this.ownHeroFrozen)
                {
                    this.ownHeroReady = true;
                }

            }

            if (c.name == CardDB.cardName.forceofnature)
            {
                int posi = this.ownMinions.Count - 1;
                CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.EX1_tk9);//Treant
                callKid(kid, posi, true);
                callKid(kid, posi, true);
                callKid(kid, posi, true);
            }

            if (c.name == CardDB.cardName.powerofthewild)// macht der wildnis with summoning
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
                    CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.EX1_160t);//panther
                    callKid(kid, posi, true);
                }
            }

            if (c.name == CardDB.cardName.starfall)
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

            if (c.name == CardDB.cardName.nourish)
            {
                if (choice == 1)
                {
                    if (this.ownMaxMana == 10)
                    {
                        //this.owncarddraw++;
                        this.drawACard(CardDB.cardName.excessmana,true);
                    }
                    else
                    {
                        this.ownMaxMana++;
                        this.mana++;
                    }
                    if (this.ownMaxMana == 10)
                    {
                        //this.owncarddraw++;
                        this.drawACard(CardDB.cardName.excessmana,true);
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
                    this.drawACard(CardDB.cardName.unknown, true);
                    this.drawACard(CardDB.cardName.unknown, true);
                    this.drawACard(CardDB.cardName.unknown, true);
                }
            }

            if (c.name == CardDB.cardName.savageroar)
            {
                List<Minion> temp = new List<Minion>(this.ownMinions);
                Enchantment e = CardDB.getEnchantmentFromCardID(CardDB.cardIDEnum.CS2_011o);
                e.creator = hc.entity;
                e.controllerOfCreator = this.ownController;
                foreach (Minion m in temp)
                {
                    addEffectToMinionNoDoubles(m, e, true);
                }
                this.ownheroAngr += 2;
            }

            //special cards#######################

            if (c.cardIDenum == CardDB.cardIDEnum.PRO_001a)// i am murloc
            {
                int posi = this.ownMinions.Count - 1;
                CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.PRO_001at);//panther
                callKid(kid, posi, true);
                callKid(kid, posi, true);
                callKid(kid, posi, true);

            }

            if (c.cardIDenum == CardDB.cardIDEnum.PRO_001c)// i am murloc
            {
                int posi = this.ownMinions.Count - 1;
                CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.EX1_021);//scharfseher
                callKid(kid, posi, true);

            }

            if (c.name == CardDB.cardName.wildgrowth)
            {
                if (this.ownMaxMana == 10)
                {
                    //this.owncarddraw++;
                    this.drawACard(CardDB.cardName.excessmana,true);
                }
                else
                {
                    this.ownMaxMana++;
                }

            }

            if (c.name == CardDB.cardName.excessmana)
            {
                //this.owncarddraw++;
                this.drawACard(CardDB.cardName.unknown, true);
            }

            if (c.name == CardDB.cardName.yseraawakens)
            {
                List<Minion> temp = new List<Minion>(this.enemyMinions);
                int damage = getSpellDamageDamage(5);
                foreach (Minion enemy in temp)
                {
                    if (enemy.name != CardDB.cardName.ysera)// dont attack ysera
                    {
                        minionGetDamagedOrHealed(enemy, damage, 0, false);
                    }
                }
                temp.Clear();
                temp.AddRange(this.ownMinions);
                foreach (Minion enemy in temp)
                {
                    if (enemy.name != CardDB.cardName.ysera)//dont attack ysera
                    {
                        minionGetDamagedOrHealed(enemy, damage, 0, false);
                    }
                }
                attackOrHealHero(damage, true);
                attackOrHealHero(damage, false);

            }

            if (c.name == CardDB.cardName.stomp)
            {
                List<Minion> temp = new List<Minion>(this.enemyMinions);
                int damage = getSpellDamageDamage(2);
                foreach (Minion enemy in temp)
                {
                    minionGetDamagedOrHealed(enemy, damage, 0, false);
                }

            }

            //NaxxCards###################################################################################

            if (c.name == CardDB.cardName.poisonseeds)
            {
                int ownanz= this.ownMinions.Count;
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
                    CardDB.Card d = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.EX1_158t);
                    callKid(d, 0, true);
                }
                for (int i = 0; i < enemanz; i++)
                {
                    CardDB.Card d = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.EX1_158t);
                    callKid(d, 0, false);
                }
            }

        }

        private void drawACard(CardDB.cardName ss, bool own, bool nopen=false)
        {
            CardDB.cardName s = ss;

            // cant hold more than 10 cards

            if (own)
            {
                if (s == CardDB.cardName.unknown && !nopen) // draw a card from deck :D
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
                if (s == CardDB.cardName.unknown && !nopen) // draw a card from deck :D
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
                            this.evaluatePenality -= 50;
                            return;
                        }
                        this.enemycarddraw++;
                    }

                }
                else
                {
                    if (this.enemyAnzCards + this.enemycarddraw >= 10)
                    {
                        this.evaluatePenality -= 50;
                        return;
                    }
                    this.enemycarddraw++;

                }
                return;
            }

            if (s == CardDB.cardName.unknown)
            {
                CardDB.Card plchldr = new CardDB.Card();
                plchldr.name = CardDB.cardName.unknown;
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

                    if (m.handcard.card.name == CardDB.cardName.knifejuggler && m.entitiyID != hc.entity)
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
                            m.stealth = false;

                        }
                        else
                        {
                            this.attackOrHealHero(1, false);
                        }
                    }

                    if (own && m.handcard.card.name == CardDB.cardName.starvingbuzzard && (TAG_RACE)hc.card.race == TAG_RACE.PET && m.entitiyID != hc.entity)
                    {
                        //this.owncarddraw++;
                        this.drawACard(CardDB.cardName.unknown, true);
                    }

                    if (own && m.handcard.card.name == CardDB.cardName.undertaker && hc.card.deathrattle)
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
                if (m.handcard.card.name==  CardDB.cardName.murloctidecaller && hc.card.race == 14 && m.entitiyID != hc.entity)
                {
                    minionGetBuffed(m, 1, 0, true);
                }
                if (m.handcard.card.name == CardDB.cardName.oldmurkeye && hc.card.race == 14 && m.entitiyID != hc.entity)
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
                if (m.handcard.card.name == CardDB.cardName.murloctidecaller && hc.card.race == 14 && m.entitiyID != hc.entity)
                {
                    minionGetBuffed(m, 1, 0, false);
                }
                if (m.handcard.card.name == CardDB.cardName.oldmurkeye && hc.card.race == 14 && m.entitiyID != hc.entity)
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

                if (m.handcard.card.name == CardDB.cardName.manawyrm)
                {
                    minionGetBuffed(m, 1, 0, true);
                }

                if (m.handcard.card.name == CardDB.cardName.manaaddict)
                {
                    minionGetBuffed(m, 2, 0, true);
                }

                if (m.handcard.card.name == CardDB.cardName.secretkeeper && c.Secret)
                {
                    minionGetBuffed(m, 1, 1, true);
                }

                if (m.handcard.card.name == CardDB.cardName.archmageantonidas)
                {
                    drawACard(CardDB.cardName.fireball,true);
                }

                if (m.handcard.card.name == CardDB.cardName.violetteacher)
                {

                    CardDB.Card d = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.NEW1_026t);//violetapprentice
                    callKid(d, m.id, true);
                }

                if (m.handcard.card.name == CardDB.cardName.gadgetzanauctioneer)
                {
                    //this.owncarddraw++;
                    drawACard(CardDB.cardName.unknown, true);
                }
                if (m.handcard.card.name == CardDB.cardName.wildpyromancer)
                {
                    wilderpyro = true;
                }
            }

            foreach (Minion m in this.enemyMinions)
            {

                if (m.handcard.card.name == CardDB.cardName.secretkeeper && c.Secret)
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

                    if (m.handcard.card.name == CardDB.cardName.wildpyromancer)
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
                this.ownSecretsIDList.Add(c.cardIDenum);
                this.playedmagierinderkirintor = false;
            }
            if (c.type == CardDB.cardtype.SPELL) this.playedPreparation = false;

            //Helpfunctions.Instance.logg("play crd " + c.name + " entitiy# " + cardEntity + " mana " + hc.getManaCost(this) + " trgt " + target);
            if (logging) Helpfunctions.Instance.logg("play crd " + c.name + " entitiy# " + cardEntity + " mana " + hc.getManaCost(this) + " trgt " + target);

            if (c.type == CardDB.cardtype.MOB)
            {
                Action b = this.placeAmobSomewhere(hc, cardpos, target, choice,placepos);
                b.handcard = new Handmanager.Handcard(hc);
                b.druidchoice = choice;
                b.owntarget = placepos;
                b.enemyEntitiy = targetEntity;
                b.cardEntitiy = cardEntity;
                b.penalty = penality;
                this.playactions.Add(b);
                this.mobsplayedThisTurn++;
                if (c.name == CardDB.cardName.kirintormage) this.playedmagierinderkirintor = true;

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
                a.penalty = penality;
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
                if (mnn.handcard.card.name == CardDB.cardName.illidanstormrage)
                {
                    CardDB.Card d = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.EX1_614t);//flameofazzinoth
                    callKid(d, mnn.id, true);
                }
                if (mnn.handcard.card.name == CardDB.cardName.questingadventurer)
                {
                    minionGetBuffed(mnn, 1, 1, true);
                }
                if (mnn.handcard.card.name == CardDB.cardName.unboundelemental && c.recallValue >= 1)
                {
                    minionGetBuffed(mnn, 1, 1, true);
                }
            }
        }

        public void attackWithWeapon(int target, int targetEntity, int penality)
        {
            this.evaluatePenality += penality;
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

            if (this.ownWeaponName == CardDB.cardName.truesilverchampion)
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
                    int oldhp = this.ownHeroHp;
                    attackOrHealHero(enem_attack, true);
                    if (oldhp > this.ownHeroHp)
                    {
                        if (!enemy.silenced && enemy.handcard.card.name == CardDB.cardName.waterelemental)
                        {
                            this.ownHeroFrozen = true;
                        }
                    }
                }
            }

            //todo
            if (ownWeaponName == CardDB.cardName.gorehowl && target != 200)
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

            if (this.enemyWeaponName == CardDB.cardName.truesilverchampion)
            {
                this.attackOrHealHero(-2,false);
            }

            if (logging) Helpfunctions.Instance.logg("enemy attck with weapon trgt: " + target + " " + targetEntity);

            if (target == 100)
            {
                attackOrHealHero(this.enemyheroAngr,true);
            }
            else
            {

                Minion enemy = this.ownMinions[target];
                minionGetDamagedOrHealed(enemy, this.enemyheroAngr, 0,true);

                if (!this.enemyheroImmuneWhileAttacking)
                {
                    attackOrHealHero(enemy.Angr, false);
                    if (!enemy.silenced && enemy.handcard.card.name == CardDB.cardName.waterelemental)
                    {
                        this.enemyHeroFrozen = true;
                    }
                }
            }

            //todo
            if (enemyWeaponName == CardDB.cardName.gorehowl && target != 100)
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

                if (c.name == CardDB.cardName.mindspike)
                {
                    heal = -1 * 2;
                }
                if (c.name == CardDB.cardName.mindshatter)
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
                this.drawACard(CardDB.cardName.unknown, true);
                this.attackOrHealHero(2, true);
            }


            if (heroname == HeroEnum.thief)
            {

                CardDB.Card wcard = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.CS2_082);
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
                CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.CS2_101t);//silverhandrecruit
                callKid(kid, posi, true);
            }

            if (heroname == HeroEnum.shaman)
            {
                int posi = this.ownMinions.Count - 1;
                CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.NEW1_009);//healingtotem
                callKid(kid, posi, true);
            }

            if (heroname == HeroEnum.lordjaraxxus)
            {
                int posi = this.ownMinions.Count - 1;
                CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.EX1_tk34);//infernal
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

                if (c.name == CardDB.cardName.mindspike)
                {
                    heal = -1 * 2;
                }
                if (c.name == CardDB.cardName.mindshatter)
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
                                if (mnn.handcard.card.name == CardDB.cardName.lightwarden)
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
                this.drawACard(CardDB.cardName.unknown, false);
                this.attackOrHealHero(2, false);
            }


            if (heroname == HeroEnum.thief)
            {

                CardDB.Card wcard = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.CS2_082);//wickedknife
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
                CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.CS2_101t);//silverhandrecruit
                callKid(kid, posi, false);
            }

            if (heroname == HeroEnum.shaman)
            {
                int posi = this.enemyMinions.Count - 1;
                CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.CS2_050);//searingtotem
                callKid(kid, posi, false);
            }

            if (heroname == HeroEnum.lordjaraxxus)
            {
                int posi = this.enemyMinions.Count - 1;
                CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.EX1_tk34);//infernal
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
                    if (a.penalty != 0)
                    {
                        Helpfunctions.Instance.logg("penality for playing " + a.penalty);
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

}
