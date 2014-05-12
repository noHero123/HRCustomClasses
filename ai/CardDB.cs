using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace HREngine.Bots
{
    public class targett
    {
        public int target = -1;
        public int targetEntity = -1;

        public targett(int targ, int ent)
        {
            this.target = targ;
            this.targetEntity = ent;
        }
    }

    public class CardDB
    {
        // Data is stored in hearthstone-folder -> data->win cardxml0
        //(data-> cardxml0 seems outdated (blutelfkleriker has 3hp there >_>)
        public enum cardtype
        {
            NONE,
            MOB,
            SPELL,
            WEAPON,
            HEROPWR,
            ENCHANTMENT,

        }



        public enum ErrorType2
        {
            NONE,//=0
            REQ_MINION_TARGET,//=1
            REQ_FRIENDLY_TARGET,//=2
            REQ_ENEMY_TARGET,//=3
            REQ_DAMAGED_TARGET,//=4
            REQ_ENCHANTED_TARGET,
            REQ_FROZEN_TARGET,
            REQ_CHARGE_TARGET,
            REQ_TARGET_MAX_ATTACK,//=8
            REQ_NONSELF_TARGET,//=9
            REQ_TARGET_WITH_RACE,//=10
            REQ_TARGET_TO_PLAY,//=11 
            REQ_NUM_MINION_SLOTS,//=12 
            REQ_WEAPON_EQUIPPED,//=13
            REQ_ENOUGH_MANA,//=14
            REQ_YOUR_TURN,
            REQ_NONSTEALTH_ENEMY_TARGET,
            REQ_HERO_TARGET,//17
            REQ_SECRET_CAP,
            REQ_MINION_CAP_IF_TARGET_AVAILABLE,//19
            REQ_MINION_CAP,
            REQ_TARGET_ATTACKED_THIS_TURN,
            REQ_TARGET_IF_AVAILABLE,//=22
            REQ_MINIMUM_ENEMY_MINIONS,//=23 /like spalen :D
            REQ_TARGET_FOR_COMBO,//=24
            REQ_NOT_EXHAUSTED_ACTIVATE,
            REQ_UNIQUE_SECRET,
            REQ_TARGET_TAUNTER,
            REQ_CAN_BE_ATTACKED,
            REQ_ACTION_PWR_IS_MASTER_PWR,
            REQ_TARGET_MAGNET,
            REQ_ATTACK_GREATER_THAN_0,
            REQ_ATTACKER_NOT_FROZEN,
            REQ_HERO_OR_MINION_TARGET,
            REQ_CAN_BE_TARGETED_BY_SPELLS,
            REQ_SUBCARD_IS_PLAYABLE,
            REQ_TARGET_FOR_NO_COMBO,
            REQ_NOT_MINION_JUST_PLAYED,
            REQ_NOT_EXHAUSTED_HERO_POWER,
            REQ_CAN_BE_TARGETED_BY_OPPONENTS,
            REQ_ATTACKER_CAN_ATTACK,
            REQ_TARGET_MIN_ATTACK,//=41
            REQ_CAN_BE_TARGETED_BY_HERO_POWERS,
            REQ_ENEMY_TARGET_NOT_IMMUNE,
            REQ_ENTIRE_ENTOURAGE_NOT_IN_PLAY,//44 (totemic call)
            REQ_MINIMUM_TOTAL_MINIONS,//45 (scharmuetzel)
            REQ_MUST_TARGET_TAUNTER,//=46
            REQ_UNDAMAGED_TARGET//=47
        }

        public class Card
        {
            public string CardID = "";
            public int entityID = 0;
            public string name = "";
            public int race = 0;
            public int rarity = 0;
            public int cost = 0;
            public int crdtype = 0;
            public cardtype type = CardDB.cardtype.NONE;
            public string description = "";
            public int carddraw = 0;

            public int Attack = 0;
            public int Health = 0;
            public int Durability = 0;//for weapons
            public bool target = false;
            public string targettext = "";
            public bool tank = false;
            public bool Silence = false;
            public bool choice = false;
            public bool windfury = false;
            public bool poisionous = false;
            public bool deathrattle = false;
            public bool battlecry = false;
            public bool oneTurnEffect = false;
            public bool Enrage = false;
            public bool Aura = false;
            public bool Elite = false;
            public bool Combo = false;
            public bool Recall = false;
            public int recallValue = 0;
            public bool immuneWhileAttacking = false;
            public bool immuneToSpellpowerg = false;
            public bool Stealth = false;
            public bool Freeze = false;
            public bool AdjacentBuff = false;
            public bool Shield = false;
            public bool Charge = false;
            public bool Secret = false;
            public bool Morph = false;
            public bool Spellpower = false;
            public bool GrantCharge = false;
            public bool HealTarget = false;
            //playRequirements, reqID= siehe PlayErrors->ErrorType
            public int needEmptyPlacesForPlaying = 0;
            public int needWithMinAttackValueOf = 0;
            public int needWithMaxAttackValueOf = 0;
            public int needRaceForPlaying = 0;
            public int needMinNumberOfEnemy = 0;
            public int needMinTotalMinions = 0;
            public int needMinionsCapIfAvailable = 0;
            public List<ErrorType2> playrequires = new List<ErrorType2>();
            public int spellpowervalue = 0;

            public Card()
            { }

            public Card(Card c)
            {
                this.entityID = c.entityID;
                this.rarity = c.rarity;
                this.AdjacentBuff = c.AdjacentBuff;
                this.Attack = c.Attack;
                this.Aura = c.Aura;
                this.battlecry = c.battlecry;
                this.carddraw = c.carddraw;
                this.CardID = c.CardID;
                this.Charge = c.Charge;
                this.choice = c.choice;
                this.Combo = c.Combo;
                this.cost = c.cost;
                this.crdtype = c.crdtype;
                this.deathrattle = c.deathrattle;
                this.description = c.description;
                this.Durability = c.Durability;
                this.Elite = c.Elite;
                this.Enrage = c.Enrage;
                this.Freeze = c.Freeze;
                this.GrantCharge = c.GrantCharge;
                this.HealTarget = c.HealTarget;
                this.Health = c.Health;
                this.immuneToSpellpowerg = c.immuneToSpellpowerg;
                this.immuneWhileAttacking = c.immuneWhileAttacking;
                this.Morph = c.Morph;
                this.name = c.name;
                this.needEmptyPlacesForPlaying = c.needEmptyPlacesForPlaying;
                this.needMinionsCapIfAvailable = c.needMinionsCapIfAvailable;
                this.needMinNumberOfEnemy = c.needMinNumberOfEnemy;
                this.needMinTotalMinions = c.needMinTotalMinions;
                this.needRaceForPlaying = c.needRaceForPlaying;
                this.needWithMaxAttackValueOf = c.needWithMaxAttackValueOf;
                this.needWithMinAttackValueOf = c.needWithMinAttackValueOf;
                this.oneTurnEffect = c.oneTurnEffect;
                this.playrequires.AddRange(c.playrequires);
                this.poisionous = c.poisionous;
                this.race = c.race;
                this.Recall = c.Recall;
                this.recallValue = c.recallValue;
                this.Secret = c.Secret;
                this.Shield = c.Shield;
                this.Silence = c.Silence;
                this.Spellpower = c.Spellpower;
                this.spellpowervalue = c.spellpowervalue;
                this.Stealth = c.Stealth;
                this.tank = c.tank;
                this.target = c.target;
                this.targettext = c.targettext;
                this.type = c.type;
                this.windfury = c.windfury;
                this.playrequires.AddRange(c.playrequires);
            }

            public bool isRequirementInList(CardDB.ErrorType2 et)
            {
                foreach (CardDB.ErrorType2 et2 in this.playrequires)
                {
                    if (et == et2)
                    {
                        return true;
                    }
                }
                return false;
            }

            public List<targett> getTargetsForCard(Playfield p)
            {
                List<targett> retval = new List<targett>();

                if (isRequirementInList(CardDB.ErrorType2.REQ_TARGET_FOR_COMBO) && p.cardsPlayedThisTurn == 0) return retval;

                if (isRequirementInList(CardDB.ErrorType2.REQ_TARGET_TO_PLAY) || isRequirementInList(CardDB.ErrorType2.REQ_NONSELF_TARGET) || isRequirementInList(CardDB.ErrorType2.REQ_TARGET_IF_AVAILABLE))
                {
                    retval.Add(new targett(100, p.ownHeroEntity));//ownhero
                    retval.Add(new targett(200, p.enemyHeroEntity));//enemyhero
                    foreach (Minion m in p.ownMinions)
                    {
                        if ((this.type == cardtype.SPELL || this.type == cardtype.HEROPWR) && (m.name == "faeriedragon" || m.name == "laughingsister")) continue;
                        retval.Add(new targett(m.id, m.entitiyID));
                    }
                    foreach (Minion m in p.enemyMinions)
                    {
                        if (((this.type == cardtype.SPELL || this.type == cardtype.HEROPWR) && (m.name == "faeriedragon" || m.name == "laughingsister")) || m.stealth) continue;
                        retval.Add(new targett(m.id + 10, m.entitiyID));
                    }

                }

                if (isRequirementInList(CardDB.ErrorType2.REQ_HERO_TARGET))
                {
                    retval.RemoveAll(x => (x.target <= 30));
                }

                if (isRequirementInList(CardDB.ErrorType2.REQ_MINION_TARGET))
                {
                    retval.RemoveAll(x => (x.target == 100) || (x.target == 200));
                }

                if (isRequirementInList(CardDB.ErrorType2.REQ_FRIENDLY_TARGET))
                {
                    retval.RemoveAll(x => (x.target >= 10 && x.target <= 20) || (x.target == 200));
                }

                if (isRequirementInList(CardDB.ErrorType2.REQ_ENEMY_TARGET))
                {
                    retval.RemoveAll(x => (x.target <= 9 || (x.target == 100)));
                }

                if (isRequirementInList(CardDB.ErrorType2.REQ_DAMAGED_TARGET))
                {
                    foreach (Minion m in p.ownMinions)
                    {
                        if (!m.wounded)
                        {
                            retval.RemoveAll(x => x.targetEntity == m.entitiyID);
                        }
                    }
                    foreach (Minion m in p.enemyMinions)
                    {
                        if (!m.wounded)
                        {
                            retval.RemoveAll(x => x.targetEntity == m.entitiyID);
                        }
                    }
                }

                if (isRequirementInList(CardDB.ErrorType2.REQ_UNDAMAGED_TARGET))
                {
                    foreach (Minion m in p.ownMinions)
                    {
                        if (m.wounded)
                        {
                            retval.RemoveAll(x => x.targetEntity == m.entitiyID);
                        }
                    }
                    foreach (Minion m in p.enemyMinions)
                    {
                        if (m.wounded)
                        {
                            retval.RemoveAll(x => x.targetEntity == m.entitiyID);
                        }
                    }
                }

                if (isRequirementInList(CardDB.ErrorType2.REQ_TARGET_MAX_ATTACK))
                {
                    foreach (Minion m in p.ownMinions)
                    {
                        if (m.Angr > this.needWithMaxAttackValueOf)
                        {
                            retval.RemoveAll(x => x.targetEntity == m.entitiyID);
                        }
                    }
                    foreach (Minion m in p.enemyMinions)
                    {
                        if (m.Angr > this.needWithMaxAttackValueOf)
                        {
                            retval.RemoveAll(x => x.targetEntity == m.entitiyID);
                        }
                    }
                }

                if (isRequirementInList(CardDB.ErrorType2.REQ_TARGET_MIN_ATTACK))
                {
                    foreach (Minion m in p.ownMinions)
                    {
                        if (m.Angr < this.needWithMinAttackValueOf)
                        {
                            retval.RemoveAll(x => x.targetEntity == m.entitiyID);
                        }
                    }
                    foreach (Minion m in p.enemyMinions)
                    {
                        if (m.Angr < this.needWithMinAttackValueOf)
                        {
                            retval.RemoveAll(x => x.targetEntity == m.entitiyID);
                        }
                    }
                }

                if (isRequirementInList(CardDB.ErrorType2.REQ_TARGET_WITH_RACE))
                {
                    foreach (Minion m in p.ownMinions)
                    {
                        if (!(m.card.race == this.needRaceForPlaying))
                        {
                            retval.RemoveAll(x => x.targetEntity == m.entitiyID);
                        }
                    }
                    foreach (Minion m in p.enemyMinions)
                    {
                        if (!(m.card.race == this.needRaceForPlaying))
                        {
                            retval.RemoveAll(x => x.targetEntity == m.entitiyID);
                        }
                    }
                }

                if (isRequirementInList(CardDB.ErrorType2.REQ_MUST_TARGET_TAUNTER))
                {
                    foreach (Minion m in p.ownMinions)
                    {
                        if (!m.taunt)
                        {
                            retval.RemoveAll(x => x.targetEntity == m.entitiyID);
                        }
                    }
                    foreach (Minion m in p.enemyMinions)
                    {
                        if (!m.taunt)
                        {
                            retval.RemoveAll(x => x.targetEntity == m.entitiyID);
                        }
                    }
                }
                return retval;

            }

            public int getManaCost(Playfield p)
            {
                int retval = this.cost;


                int offset = 0; // if offset < 0 costs become lower, if >0 costs are higher at the end

                // CARDS that increase the manacosts of others ##############################
                //Manacosts changes with soeldner der venture co.
                if (p.soeldnerDerVenture != p.startedWithsoeldnerDerVenture && this.type == cardtype.MOB)
                {
                    offset += (p.soeldnerDerVenture - p.startedWithsoeldnerDerVenture) * 3;
                }

                //Manacosts changes with mana-ghost
                if (p.managespenst != p.startedWithManagespenst && this.type == cardtype.MOB)
                {
                    offset += (p.managespenst - p.startedWithManagespenst);
                }


                // CARDS that decrease the manacosts of others ##############################

                //Manacosts changes with the summoning-portal >_>
                if (p.startedWithbeschwoerungsportal != p.beschwoerungsportal && this.type == cardtype.MOB)
                { //cant lower the mana to 0
                    int temp = (p.startedWithbeschwoerungsportal - p.beschwoerungsportal) * 2;
                    if (retval + temp <= 0) temp = -retval + 1;
                    offset = offset + temp;
                }

                //Manacosts changes with the pint-sized summoner
                if (p.winzigebeschwoererin >= 1 && p.mobsplayedThisTurn >= 1 && p.startedWithMobsPlayedThisTurn == 0 && this.type == cardtype.MOB)
                { // if we start oure calculations with 0 mobs played, then the cardcost are 1 mana to low in the further calculations (with the little summoner on field)
                    offset += p.winzigebeschwoererin;
                }
                if (p.mobsplayedThisTurn == 0 && p.winzigebeschwoererin <= p.startedWithWinzigebeschwoererin && this.type == cardtype.MOB)
                { // one pint-sized summoner got killed, before we played the first mob -> the manacost are higher of all mobs
                    offset += (p.startedWithWinzigebeschwoererin - p.winzigebeschwoererin);
                }

                //Manacosts changes with the zauberlehrling summoner
                if (p.zauberlehrling != p.startedWithZauberlehrling && this.type == cardtype.SPELL)
                { //if the number of zauberlehrlings change
                    offset += (p.startedWithZauberlehrling - p.zauberlehrling);
                }



                //manacosts are lowered, after we played preparation
                if (p.playedPreparation && this.type == cardtype.SPELL)
                { //if the number of zauberlehrlings change
                    offset -= 3;
                }





                switch (this.name)
                {
                    case "dreadcorsair":
                        retval = retval + offset - p.ownWeaponAttack + p.ownWeaponAttackStarted; // if weapon attack change we change manacost
                        break;
                    case "seagiant":
                        retval = retval + offset - p.ownMinions.Count + p.ownMobsCountStarted;
                        break;
                    case "mountaingiant":
                        retval = retval + offset - p.owncards.Count + p.ownCardsCountStarted;
                        break;
                    case "moltengiant":
                        retval = retval + offset - p.ownHeroHp + p.ownHeroHpStarted;
                        break;
                    default:
                        retval = retval + offset;
                        break;
                }

                if (this.Secret && p.playedmagierinderkirintor)
                {
                    retval = 0;
                }

                retval = Math.Max(0, retval);

                return retval;
            }

            public bool canplayCard(Playfield p)
            {
                //is playrequirement?
                bool haveToDoRequires = isRequirementInList(CardDB.ErrorType2.REQ_TARGET_TO_PLAY);
                bool retval = true;
                // cant play if i have to few mana

                if (p.mana < this.getManaCost(p)) return false;

                // cant play mob, if i have allready 7 mininos
                if (this.type == CardDB.cardtype.MOB && p.ownMinions.Count >= 7) return false;

                if (isRequirementInList(CardDB.ErrorType2.REQ_MINIMUM_ENEMY_MINIONS))
                {
                    if (p.enemyMinions.Count < this.needMinNumberOfEnemy) return false;
                }
                if (isRequirementInList(CardDB.ErrorType2.REQ_NUM_MINION_SLOTS))
                {
                    if (p.ownMinions.Count > 7 - this.needEmptyPlacesForPlaying) return false;
                }
                
                if (isRequirementInList(CardDB.ErrorType2.REQ_WEAPON_EQUIPPED))
                {
                    if (p.ownWeaponName == "") return false;
                }

                if (isRequirementInList(CardDB.ErrorType2.REQ_MINIMUM_TOTAL_MINIONS))
                {
                    if (this.needMinTotalMinions > p.ownMinions.Count + p.enemyMinions.Count) return false;
                }

                if (haveToDoRequires)
                {
                    if (this.getTargetsForCard(p).Count == 0) return false;

                    //it requires a target-> return false if 
                }

                if (isRequirementInList(CardDB.ErrorType2.REQ_TARGET_IF_AVAILABLE) && isRequirementInList(CardDB.ErrorType2.REQ_MINION_CAP_IF_TARGET_AVAILABLE))
                {
                    if (this.getTargetsForCard(p).Count >= 1 && p.ownMinions.Count > 7-this.needMinionsCapIfAvailable ) return false;
                }

                if (isRequirementInList(CardDB.ErrorType2.REQ_ENTIRE_ENTOURAGE_NOT_IN_PLAY))
                {
                    int difftotem = 0;
                    foreach (Minion m in p.ownMinions)
                    {
                        if (m.name == "healingtotem" || m.name == "wrathofairtotem" || m.name == "searingtotem" || m.name == "stoneclawtotem") difftotem++;
                    }
                    if (difftotem == 4) return false;
                }
                

                if (this.Secret)
                {
                    if (p.ownSecretsIDList.Contains(this.CardID)) return false;
                    if (p.ownSecretsIDList.Count >= 5) return false;
                }


                return true;
            }



        }

        List<Card> cardlist = new List<Card>();

        private static CardDB instance;

        public static CardDB Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new CardDB();
                }
                return instance;
            }
        }

        private CardDB()
        {
            string[] lines = new string[0] { };
            try
            {
                string path = Settings.Instance.path;
                lines = System.IO.File.ReadAllLines(path + "_carddb.txt");
            }
            catch
            {
                Helpfunctions.Instance.logg("cant find carddb.txt");
            }
            cardlist.Clear();
            Card c = new Card();
            int de = 0;
            bool targettext = false;
            //placeholdercard
            Card plchldr = new Card();
            plchldr.name = "unknown";
            plchldr.cost = 1000;
            this.cardlist.Add(plchldr);

            foreach (string s in lines)
            {
                if (s.Contains("/Entity"))
                {
                    if (c.type == cardtype.ENCHANTMENT)
                    {
                        //Helpfunctions.Instance.logg(c.CardID);
                        //Helpfunctions.Instance.logg(c.name);
                        //Helpfunctions.Instance.logg(c.description);
                        continue;
                    }

                    if (c.name != "")
                    {
                        //Helpfunctions.Instance.logg(c.name);
                        this.cardlist.Add(c);
                    }

                }
                if (s.Contains("<Entity version=\"2\" CardID=\""))
                {
                    c = new Card();
                    de = 0;
                    targettext = false;
                    string temp = s.Replace("<Entity version=\"2\" CardID=\"", "");
                    temp = temp.Replace("\">", "");
                    c.CardID = temp;
                    continue;
                }
                if (s.Contains("<Entity version=\"1\" CardID=\""))
                {
                    c = new Card();
                    de = 0;
                    targettext = false;
                    string temp = s.Replace("<Entity version=\"1\" CardID=\"", "");
                    temp = temp.Replace("\">", "");
                    c.CardID = temp;
                    continue;
                }

                if (s.Contains("<Tag name=\"Health\""))
                {
                    string temp = s.Split(new string[] { "value=\"" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    temp = temp.Split('\"')[0];
                    c.Health = Convert.ToInt32(temp);
                    continue;
                }
                if (s.Contains("<Tag name=\"Atk\""))
                {
                    string temp = s.Split(new string[] { "value=\"" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    temp = temp.Split('\"')[0];
                    c.Attack = Convert.ToInt32(temp);
                    continue;
                }
                if (s.Contains("<Tag name=\"Race\""))
                {
                    string temp = s.Split(new string[] { "value=\"" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    temp = temp.Split('\"')[0];
                    c.race = Convert.ToInt32(temp);
                    continue;
                }
                if (s.Contains("<Tag name=\"Rarity\""))
                {
                    string temp = s.Split(new string[] { "value=\"" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    temp = temp.Split('\"')[0];
                    c.rarity = Convert.ToInt32(temp);
                    continue;
                }
                if (s.Contains("<Tag name=\"Cost\""))
                {
                    string temp = s.Split(new string[] { "value=\"" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    temp = temp.Split('\"')[0];
                    c.cost = Convert.ToInt32(temp);
                    continue;
                }

                if (s.Contains("<Tag name=\"CardType\""))
                {
                    string temp = s.Split(new string[] { "value=\"" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    temp = temp.Split('\"')[0];
                    if (c.name != "")
                    {
                        //Helpfunctions.Instance.logg(temp);
                    }

                    c.crdtype = Convert.ToInt32(temp);
                    if (c.crdtype == 10)
                    {
                        c.type = CardDB.cardtype.HEROPWR;
                    }
                    if (c.crdtype == 4)
                    {
                        c.type = CardDB.cardtype.MOB;
                    }
                    if (c.crdtype == 5)
                    {
                        c.type = CardDB.cardtype.SPELL;
                    }
                    if (c.crdtype == 6)
                    {
                        c.type = CardDB.cardtype.ENCHANTMENT;
                    }
                    if (c.crdtype == 7)
                    {
                        c.type = CardDB.cardtype.WEAPON;
                    }
                    continue;
                }

                if (s.Contains("<enUS>"))
                {
                    string temp = s.Replace("<enUS>", "");

                    temp = temp.Replace("</enUS>", "");
                    temp = temp.Replace("&lt;", "");
                    temp = temp.Replace("b&gt;", "");
                    temp = temp.Replace("/b&gt;", "");
                    temp = temp.ToLower();
                    if (de == 0)
                    {
                        temp = temp.Replace("'", "");
                        temp = temp.Replace(" ", "");
                        temp = temp.Replace(":", "");
                        temp = temp.Replace(".", "");
                        temp = temp.Replace("!", "");
                        //temp = temp.Replace("ß", "ss");
                        //temp = temp.Replace("ü", "ue");
                        //temp = temp.Replace("ä", "ae");
                        //temp = temp.Replace("ö", "oe");

                        //Helpfunctions.Instance.logg(temp);
                        c.name = temp;
                    }
                    if (de == 1)
                    {
                        c.description = temp;
                        if (c.description.Contains("choose one"))
                        {
                            c.choice = true;
                            //Helpfunctions.Instance.logg(c.name + " is choice");
                        }
                    }
                    if (targettext)
                    {
                        c.targettext = temp;
                        targettext = false;
                    }

                    de++;
                    continue;
                }
                if (s.Contains("<Tag name=\"Poisonous\""))
                {
                    string temp = s.Split(new string[] { "value=\"" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    temp = temp.Split('\"')[0];
                    int ti = Convert.ToInt32(temp);
                    if (ti == 1) c.poisionous = true;
                    continue;
                }
                if (s.Contains("<Tag name=\"Enrage\""))
                {
                    string temp = s.Split(new string[] { "value=\"" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    temp = temp.Split('\"')[0];
                    int ti = Convert.ToInt32(temp);
                    if (ti == 1) c.Enrage = true;
                    continue;
                }

                if (s.Contains("<Tag name=\"OneTurnEffect\""))
                {
                    string temp = s.Split(new string[] { "value=\"" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    temp = temp.Split('\"')[0];
                    int ti = Convert.ToInt32(temp);
                    if (ti == 1) c.oneTurnEffect = true;
                    continue;
                }
                if (s.Contains("<Tag name=\"Aura\""))
                {
                    string temp = s.Split(new string[] { "value=\"" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    temp = temp.Split('\"')[0];
                    int ti = Convert.ToInt32(temp);
                    if (ti == 1) c.Aura = true;
                    continue;
                }


                if (s.Contains("<Tag name=\"Taunt\""))
                {
                    string temp = s.Split(new string[] { "value=\"" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    temp = temp.Split('\"')[0];
                    int ti = Convert.ToInt32(temp);
                    if (ti == 1) c.tank = true;
                    continue;
                }
                if (s.Contains("<Tag name=\"Battlecry\""))
                {
                    string temp = s.Split(new string[] { "value=\"" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    temp = temp.Split('\"')[0];
                    int ti = Convert.ToInt32(temp);
                    if (ti == 1) c.battlecry = true;
                    continue;
                }
                if (s.Contains("<Tag name=\"Windfury\""))
                {
                    string temp = s.Split(new string[] { "value=\"" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    temp = temp.Split('\"')[0];
                    int ti = Convert.ToInt32(temp);
                    if (ti == 1) c.windfury = true;
                    continue;
                }

                if (s.Contains("<Tag name=\"Deathrattle\""))
                {
                    string temp = s.Split(new string[] { "value=\"" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    temp = temp.Split('\"')[0];
                    int ti = Convert.ToInt32(temp);
                    if (ti == 1) c.deathrattle = true;
                    continue;
                }
                if (s.Contains("<Tag name=\"Durability\""))
                {
                    string temp = s.Split(new string[] { "value=\"" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    temp = temp.Split('\"')[0];
                    c.Durability = Convert.ToInt32(temp);
                    continue;
                }
                if (s.Contains("<Tag name=\"Elite\""))
                {
                    string temp = s.Split(new string[] { "value=\"" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    temp = temp.Split('\"')[0];
                    int ti = Convert.ToInt32(temp);
                    if (ti == 1) c.Elite = true;
                    continue;
                }
                if (s.Contains("<Tag name=\"Combo\""))
                {
                    string temp = s.Split(new string[] { "value=\"" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    temp = temp.Split('\"')[0];
                    int ti = Convert.ToInt32(temp);
                    if (ti == 1) c.Combo = true;
                    continue;
                }
                if (s.Contains("<Tag name=\"Recall\""))
                {
                    string temp = s.Split(new string[] { "value=\"" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    temp = temp.Split('\"')[0];
                    int ti = Convert.ToInt32(temp);
                    if (ti == 1) c.Recall = true;
                    c.recallValue = 1;
                    if (c.name == "forkedlightning") c.recallValue = 2;
                    if (c.name == "dustdevil") c.recallValue = 2;
                    if (c.name == "lightningstorm") c.recallValue = 2;
                    if (c.name == "lavaburst") c.recallValue = 2;
                    if (c.name == "feralspirit") c.recallValue = 2;
                    if (c.name == "doomhammer") c.recallValue = 2;
                    if (c.name == "earthelemental") c.recallValue = 3;
                    continue;
                }

                if (s.Contains("<Tag name=\"ImmuneToSpellpower\""))
                {
                    string temp = s.Split(new string[] { "value=\"" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    temp = temp.Split('\"')[0];
                    int ti = Convert.ToInt32(temp);
                    if (ti == 1) c.immuneToSpellpowerg = true;
                    continue;
                }
                if (s.Contains("<Tag name=\"Stealth\""))
                {
                    string temp = s.Split(new string[] { "value=\"" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    temp = temp.Split('\"')[0];
                    int ti = Convert.ToInt32(temp);
                    if (ti == 1) c.Stealth = true;
                    continue;
                }
                if (s.Contains("<Tag name=\"Secret\""))
                {
                    string temp = s.Split(new string[] { "value=\"" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    temp = temp.Split('\"')[0];
                    int ti = Convert.ToInt32(temp);
                    if (ti == 1) c.Secret = true;
                    continue;
                }
                if (s.Contains("<Tag name=\"Freeze\""))
                {
                    string temp = s.Split(new string[] { "value=\"" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    temp = temp.Split('\"')[0];
                    int ti = Convert.ToInt32(temp);
                    if (ti == 1) c.Freeze = true;
                    continue;
                }
                if (s.Contains("<Tag name=\"AdjacentBuff\""))
                {
                    string temp = s.Split(new string[] { "value=\"" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    temp = temp.Split('\"')[0];
                    int ti = Convert.ToInt32(temp);
                    if (ti == 1) c.AdjacentBuff = true;
                    continue;
                }
                if (s.Contains("<Tag name=\"Divine Shield\""))
                {
                    string temp = s.Split(new string[] { "value=\"" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    temp = temp.Split('\"')[0];
                    int ti = Convert.ToInt32(temp);
                    if (ti == 1) c.Shield = true;
                    continue;
                }
                if (s.Contains("<Tag name=\"Charge\""))
                {
                    string temp = s.Split(new string[] { "value=\"" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    temp = temp.Split('\"')[0];
                    int ti = Convert.ToInt32(temp);
                    if (ti == 1) c.Charge = true;
                    continue;
                }
                if (s.Contains("<Tag name=\"Silence\""))
                {
                    string temp = s.Split(new string[] { "value=\"" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    temp = temp.Split('\"')[0];
                    int ti = Convert.ToInt32(temp);
                    if (ti == 1) c.Silence = true;
                    continue;
                }
                if (s.Contains("<Tag name=\"Morph\""))
                {
                    string temp = s.Split(new string[] { "value=\"" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    temp = temp.Split('\"')[0];
                    int ti = Convert.ToInt32(temp);
                    if (ti == 1) c.Morph = true;
                    continue;
                }
                if (s.Contains("<Tag name=\"Spellpower\""))
                {
                    string temp = s.Split(new string[] { "value=\"" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    temp = temp.Split('\"')[0];
                    int ti = Convert.ToInt32(temp);
                    if (ti == 1) c.Spellpower = true;
                    c.spellpowervalue = 1;
                    if (c.name == "ancientmage") c.spellpowervalue = 0;
                    if (c.name == "malygos") c.spellpowervalue = 5;
                    continue;
                }
                if (s.Contains("<Tag name=\"GrantCharge\""))
                {
                    string temp = s.Split(new string[] { "value=\"" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    temp = temp.Split('\"')[0];
                    int ti = Convert.ToInt32(temp);
                    if (ti == 1) c.GrantCharge = true;
                    continue;
                }
                if (s.Contains("<Tag name=\"HealTarget\""))
                {
                    string temp = s.Split(new string[] { "value=\"" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    temp = temp.Split('\"')[0];
                    int ti = Convert.ToInt32(temp);
                    if (ti == 1) c.HealTarget = true;
                    continue;
                }

                if (s.Contains("TargetingArrowText"))
                {
                    c.target = true;
                    targettext = true;
                    continue;
                }

                if (s.Contains("<PlayRequirement"))
                {
                    string temp = s.Split(new string[] { "reqID=\"" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    temp = temp.Split('\"')[0];
                    ErrorType2 et2 = (ErrorType2)Convert.ToInt32(temp);
                    c.playrequires.Add(et2);
                }


                if (s.Contains("<PlayRequirement reqID=\"12\" param=\""))
                {
                    string temp = s.Split(new string[] { "param=\"" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    temp = temp.Split('\"')[0];
                    c.needEmptyPlacesForPlaying = Convert.ToInt32(temp);
                    continue;
                }
                if (s.Contains("PlayRequirement reqID=\"41\" param=\""))
                {
                    string temp = s.Split(new string[] { "param=\"" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    temp = temp.Split('\"')[0];
                    c.needWithMinAttackValueOf = Convert.ToInt32(temp);
                    continue;
                }
                if (s.Contains("PlayRequirement reqID=\"8\" param=\""))
                {
                    string temp = s.Split(new string[] { "param=\"" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    temp = temp.Split('\"')[0];
                    c.needWithMaxAttackValueOf = Convert.ToInt32(temp);
                    continue;
                }
                if (s.Contains("PlayRequirement reqID=\"10\" param=\""))
                {
                    string temp = s.Split(new string[] { "param=\"" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    temp = temp.Split('\"')[0];
                    c.needRaceForPlaying = Convert.ToInt32(temp);
                    continue;
                }
                if (s.Contains("PlayRequirement reqID=\"23\" param=\""))
                {
                    string temp = s.Split(new string[] { "param=\"" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    temp = temp.Split('\"')[0];
                    c.needMinNumberOfEnemy = Convert.ToInt32(temp);
                    continue;
                }
                if (s.Contains("PlayRequirement reqID=\"45\" param=\""))
                {
                    string temp = s.Split(new string[] { "param=\"" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    temp = temp.Split('\"')[0];
                    c.needMinTotalMinions = Convert.ToInt32(temp);
                    continue;
                }
                if (s.Contains("PlayRequirement reqID=\"19\" param=\""))
                {
                    string temp = s.Split(new string[] { "param=\"" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    temp = temp.Split('\"')[0];
                    c.needMinionsCapIfAvailable = Convert.ToInt32(temp);
                    continue;
                }



                if (s.Contains("<Tag name="))
                {
                    string temp = s.Split(new string[] { "<Tag name=\"" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    temp = temp.Split('\"')[0];
                    /*
                    if (temp != "DevState" && temp != "FlavorText" && temp != "ArtistName" && temp != "Cost" && temp != "EnchantmentIdleVisual" && temp != "EnchantmentBirthVisual" && temp != "Collectible" && temp != "CardSet" && temp != "AttackVisualType" && temp != "CardName" && temp != "Class" && temp != "CardTextInHand" && temp != "Rarity" && temp != "TriggerVisual" && temp != "Faction" && temp != "HowToGetThisGoldCard" && temp != "HowToGetThisCard" && temp != "CardTextInPlay")
                        Helpfunctions.Instance.logg(s);*/
                }


            }


        }


        public Card getCardData(string cardname)
        {
            string target = cardname.ToLower();
            Card c = new Card();

            foreach (Card ca in this.cardlist)
            {
                if (ca.name == target)
                {
                    return ca;
                }
            }

            return new Card(c);
        }

        public Card getCardDataFromID(string id)
        {
            string target = id;
            Card c = new Card();

            foreach (Card ca in this.cardlist)
            {
                if (ca.CardID == target)
                {
                    return ca;
                }
            }

            return new Card(c);
        }

        private void rdtxt()
        {

            foreach (Card c in this.cardlist)
            {
                if (c.description.Contains("karte") && c.description.Contains("zieht"))
                {
                    c.carddraw = 1;
                }
                if (c.description.Contains("waehlt aus") && c.description.Contains("oder"))
                {
                    c.choice = true;
                }



            }
        }


        public static Enchantment getEnchantmentFromCardID(string cardID)
        {
            Enchantment retval = new Enchantment();
            retval.CARDID = cardID;

            if (cardID == "CS2_188o")//insiriert  dieser diener hat +2 angriff in diesem zug. (ruchloser unteroffizier)
            {
                retval.angrbuff = 2;
            }

            if (cardID == "CS2_059o")//blutpakt (blutwichtel)
            {
                retval.hpbuff = 1;
            }
            if (cardID == "EX1_019e")//Segen der Klerikerin (blutelfenklerikerin)
            {
                retval.angrbuff = 1;
                retval.hpbuff = 1;
            }

            if (cardID == "CS2_045e")//waffedesfelsbeissers
            {
                retval.angrbuff = 3;
            }
            if (cardID == "EX1_587e")//windfury
            {
                retval.windfury = true;
            }
            if (cardID == "EX1_355e")//urteildestemplers   granted by blessed champion
            {
                retval.angrfaktor = 2;
            }
            if (cardID == "NEW1_036e")//befehlsruf
            {
                retval.cantLowerHPbelowONE = true;
            }
            if (cardID == "CS2_046e")// kampfrausch
            {
                retval.angrbuff = 3;
            }

            if (cardID == "CS2_104e")// toben
            {
                retval.angrbuff = 3;
                retval.hpbuff = 3;
            }
            if (cardID == "DREAM_05e")// alptraum
            {
                retval.angrbuff = 5;
                retval.hpbuff = 5;
            }
            if (cardID == "CS2_022e")// verwandlung
            {
                retval.angrbuff = 3;
            }
            if (cardID == "EX1_611e")// gefangen
            {
                //icetrap?
            }

            if (cardID == "EX1_014te")// banane
            {
                retval.angrbuff = 1;
                retval.hpbuff = 1;
            }
            if (cardID == "EX1_178ae")// festgewurzelt
            {
                retval.hpbuff = 5;
                retval.taunt = true;
            }
            if (cardID == "CS2_011o")// wildesbruellen
            {
                retval.angrbuff = 2;
            }
            if (cardID == "EX1_366e")// rechtschaffen
            {
                retval.angrbuff = 1;
                retval.hpbuff = 1;
            }
            if (cardID == "CS2_017o")// klauen (ownhero +1angr)
            {
            }
            if (cardID == "EX1_604o")// rasend
            {
                retval.angrbuff = 1;
            }
            if (cardID == "EX1_084e")// sturmangriff
            {
                retval.charge = true;
            }
            if (cardID == "CS1_129e")// inneresfeuer // angr = live
            {
                retval.angrEqualLife = true;
            }
            if (cardID == "EX1_603e")// aufzackgebracht (fieser zuchtmeister)
            {
                retval.angrbuff = 2;
            }
            if (cardID == "EX1_507e")// mrgglaargl! der murlocanführer verleiht +2/+1.
            {
                retval.angrbuff = 2;
                retval.hpbuff = 1;
            }
            if (cardID == "CS2_038e")// geistderahnen : todesröcheln: dieser diener kehrt aufs schlachtfeld zurück.
            {

            }
            if (cardID == "NEW1_024o")// gruenhauts befehl +1/+1.
            {
                retval.angrbuff = 1;
                retval.hpbuff = 1;
            }
            if (cardID == "EX1_590e")// schattenvonmuru : angriff und leben durch aufgezehrte gottesschilde erhöht. (blutritter)
            {
                retval.angrbuff = 3;
                retval.hpbuff = 3;
            }
            if (cardID == "CS2_074e")// toedlichesgift
            {
            }
            if (cardID == "EX1_258e")// ueberladen von entfesselnder elementar
            {
                retval.angrbuff = 1;
                retval.hpbuff = 1;
            }
            if (cardID == "TU4f_004o")// vermaechtnisdeskaisers von cho
            {
                retval.angrbuff = 2;
                retval.hpbuff = 2;
            }

            if (cardID == "NEW1_017e")// gefuellterbauch randvoll mit murloc. (hungrigekrabbe)
            {
                retval.angrbuff = 2;
                retval.hpbuff = 2;
            }

            if (cardID == "EX1_334e")// dunklerbefehl von dunkler Wahnsin
            {
            }

            if (cardID == "CS2_087e")// segendermacht von segendermacht
            {
                retval.angrbuff = 3;
            }
            if (cardID == "EX1_613e")// vancleefsrache dieser diener hat erhöhten angriff und erhöhtes leben.
            {
                retval.angrbuff = 2;
                retval.hpbuff = 2;
            }
            if (cardID == "EX1_623e")// infusion
            {
                retval.hpbuff = 3;
            }
            if (cardID == "CS2_073e2")// kaltbluetigkeit +4
            {
                retval.angrbuff = 4;
            }
            if (cardID == "EX1_162o")// staerkedesrudels der terrorwolfalpha verleiht diesem diener +1 angriff.
            {
                retval.angrbuff = 1;
            }
            if (cardID == "EX1_549o")// zorndeswildtiers +2 angriff und immun/ in diesem zug.
            {
                retval.angrbuff = 2;
                retval.imune = true;
            }

            if (cardID == "EX1_091o")//  kontrollederkabale  dieser diener wurde von einer kabaleschattenpriesterin gestohlen.
            {
            }

            if (cardID == "CS2_084e")//  maldesjaegers
            {
                retval.setHPtoOne = true;
            }
            if (cardID == "NEW1_036e2")//  befehlsruf2 ? das leben eurer diener kann in diesem zug nicht unter 1 fallen.
            {
                retval.cantLowerHPbelowONE = true;
            }
            if (cardID == "CS2_122e")// angespornt der schlachtzugsleiter verleiht diesem diener +1 angriff. (schlachtzugsleiter)
            {
                retval.angrbuff = 1;
            }
            if (cardID == "CS2_103e")// charge
            {
                retval.charge = true;
            }
            if (cardID == "EX1_080o")// geheimnissebewahren    erhöhte werte.
            {
                retval.angrbuff = 1;
                retval.hpbuff = 1;
            }
            if (cardID == "CS2_005o")// klaue +2 angriff in diesem zug.
            {
                retval.angrbuff = 2;
            }
            if (cardID == "EX1_363e2")// segenderweisheit
            {
                retval.cardDrawOnAngr = true;
            }
            if (cardID == "EX1_178be")//  entwurzelt +5 angr
            {
                retval.angrbuff = 5;
            }
            if (cardID == "CS2_222o")//  diemachtsturmwinds +1+1 (von champ of sturmwind)
            {
                retval.angrbuff = 1;
                retval.hpbuff = 1;
            }
            if (cardID == "EX1_399e")// amoklauf von gurubashi berserker
            {
                retval.angrbuff = 3;
            }
            if (cardID == "CS2_041e")// machtderahnen
            {
                retval.taunt = true;
            }
            if (cardID == "EX1_612o")//  machtderkirintor
            {

            }
            if (cardID == "EX1_004e")// elunesanmut erhöhtes leben. von junger priesterin
            {
                retval.hpbuff = 1;
            }
            if (cardID == "EX1_246e")// verhext dieser diener wurde verwandelt.
            {

            }
            if (cardID == "EX1_244e")// machtdertotems (card that buffs hp of totems)
            {
                retval.hpbuff = 2;
            }
            if (cardID == "EX1_607e")// innerewut (innere wut)
            {
                retval.angrbuff = 2;
            }
            if (cardID == "EX1_573ae")// gunstdeshalbgotts (cenarius?)
            {
                retval.angrbuff = 2;
                retval.hpbuff = 2;
            }
            if (cardID == "EX1_411e2")// schliffbenoetigt angriff verringert.  von waffe blutschrei
            {
                retval.angrbuff = -1;
            }
            if (cardID == "CS2_063e")// verderbnis  wird zu beginn des zuges des verderbenden spielers vernichtet.
            {

            }
            if (cardID == "CS2_181e")// vollekraft +2 angr ka von wem
            {
                retval.angrbuff = 2;
            }
            if (cardID == "EX1_508o")// mlarggragllabl! dieser murloc hat +1 angriff. (grimmschuppenorakel)
            {
                retval.angrbuff = 1;
            }
            if (cardID == "CS2_073e")// kaltbluetigkeit +2 angriff.
            {
                retval.angrbuff = 2;
            }
            if (cardID == "NEW1_018e")// goldrausch von blutsegelraeuberin
            {

            }
            if (cardID == "EX1_059e2")// experimente! der verrückte alchemist hat angriff und leben vertauscht.
            {

            }
            if (cardID == "EX1_570e")// biss (only hero)
            {
                retval.angrbuff = 4;
            }
            if (cardID == "EX1_360e")//  demut  angriff wurde auf 1 gesetzt.
            {
                retval.setANGRtoOne = true;
            }
            if (cardID == "DS1_175o")// wutgeheul durch waldwolf
            {
                retval.angrbuff = 1;
            }
            if (cardID == "EX1_596e")// daemonenfeuer
            {
                retval.angrbuff = 2;
                retval.hpbuff = 2;
            }

            if (cardID == "EX1_158e")// seeledeswaldes todesröcheln: ruft einen treant (2/2) herbei.
            {

            }
            if (cardID == "EX1_316e")// ueberwaeltigendemacht
            {
                retval.angrbuff = 4;
                retval.hpbuff = 4;
            }
            if (cardID == "EX1_044e")// stufenaufstieg erhöhter angriff und erhöhtes leben. (rastloser abenteuer)
            {

            }
            if (cardID == "EX1_304e")// verzehren  erhöhte werte. (hexer)
            {

            }
            if (cardID == "EX1_363e")// segenderweisheit der segnende spieler zieht eine karte, wenn dieser diener angreift.
            {

            }
            if (cardID == "CS2_105e")// heldenhafterstoss
            {

            }
            if (cardID == "EX1_128e")// verhuellt bleibt bis zu eurem nächsten zug verstohlen.
            {

            }
            if (cardID == "NEW1_033o")// himmelsauge leokk verleiht diesem diener +1 angriff.
            {
                retval.angrbuff = 1;
            }
            if (cardID == "CS2_004e")// machtwortschild
            {
                retval.hpbuff = 2;
            }
            if (cardID == "EX1_382e")// waffenniederlegen! angriff auf 1 gesetzt.
            {
                retval.setANGRtoOne = true;
            }
            if (cardID == "CS2_092e")// segenderkoenige
            {
                retval.angrbuff = 4;
                retval.hpbuff = 4;
            }
            if (cardID == "NEW1_012o")// manasaettigung  erhöhter angriff.
            {

            }
            if (cardID == "EX1_619e")//  gleichheit  leben auf 1 gesetzt.
            {
                retval.setHPtoOne = true;
            }
            if (cardID == "EX1_509e")// blarghghl    erhöhter angriff.
            {
                retval.angrbuff = 1;
            }
            if (cardID == "CS2_009e")// malderwildnis
            {
                retval.angrbuff = 2;
                retval.hpbuff = 2;
                retval.taunt = true;
            }
            if (cardID == "EX1_103e")// mrghlglhal +2 leben.
            {
                retval.hpbuff = 2;
            }
            if (cardID == "NEW1_038o")// wachstum  gruul wächst ...
            {

            }
            if (cardID == "CS1_113e")//  gedankenkontrolle
            {

            }
            if (cardID == "CS2_236e")//  goettlicherwille  dieser diener hat doppeltes leben.
            {

            }
            if (cardID == "CS2_083e")// geschaerft +1 angriff in diesem zug.
            {
                retval.angrbuff = 1;
            }
            if (cardID == "TU4c_008e")// diemachtmuklas
            {
                retval.angrbuff = 8;
            }
            if (cardID == "EX1_379e")//  busse 
            {
                retval.setHPtoOne = true;
            }
            if (cardID == "EX1_274e")// puremacht! (astraler arkanist)
            {
                retval.angrbuff = 2;
                retval.hpbuff = 2;
            }
            if (cardID == "CS2_221e")// vorsicht!scharf! +2 angriff von hasserfüllte schmiedin. 
            {
                retval.weaponAttack = 2;
            }
            if (cardID == "EX1_409e")// aufgewertet +1 angriff und +1 haltbarkeit.
            {
                retval.weaponAttack = 1;
                retval.weapondurability = 1;
            }
            if (cardID == "tt_004o")//kannibalismus (fleischfressender ghul)
            {
                retval.angrbuff = 1;
            }
            if (cardID == "EX1_155ae")// maldernatur
            {
                retval.angrbuff = 4;
            }
            if (cardID == "NEW1_025e")// verstaerkt (by emboldener 3000)
            {
                retval.angrbuff = 1;
                retval.hpbuff = 1;
            }
            if (cardID == "EX1_584e")// lehrenderkirintor zauberschaden+1 (by uralter magier)
            {
                retval.zauberschaden = 1;
            }
            if (cardID == "EX1_160be")// rudelfuehrer +1/+1. (macht der wildnis)
            {
                retval.angrbuff = 1;
                retval.hpbuff = 1;
            }
            if (cardID == "TU4c_006e")//  banane
            {
                retval.angrbuff = 1;
                retval.hpbuff = 1;
            }
            if (cardID == "NEW1_027e")// yarrr!   der südmeerkapitän verleiht +1/+1.
            {
                retval.angrbuff = 1;
                retval.hpbuff = 1;
            }
            if (cardID == "DS1_070o")// praesenzdesmeisters +2/+2 und spott/. (hundemeister)
            {
                retval.angrbuff = 2;
                retval.hpbuff = 2;
                retval.taunt = true;
            }
            if (cardID == "EX1_046e")// gehaertet +2 angriff in diesem zug. (dunkeleisenzwerg)
            {
                retval.angrbuff = 2;
            }
            if (cardID == "EX1_531e")// satt    erhöhter angriff und erhöhtes leben. (aasfressende Hyaene)
            {
                retval.angrbuff = 2;
                retval.hpbuff = 1;
            }
            if (cardID == "CS2_226e")// bannerderfrostwoelfe    erhöhte werte. (frostwolfkriegsfuerst)
            {
                retval.angrbuff = 1;
                retval.hpbuff = 1;
            }
            if (cardID == "DS1_178e")//  sturmangriff tundranashorn verleiht ansturm.
            {
                retval.charge = true;
            }
            if (cardID == "CS2_226o")//befehlsgewalt der kriegsfürst der frostwölfe hat erhöhten angriff und erhöhtes leben.
            {
                retval.angrbuff = 1;
                retval.hpbuff = 1;
            }
            if (cardID == "Mekka4e")// verwandelt wurde in ein huhn verwandelt!
            {

            }
            if (cardID == "EX1_411e")// blutrausch kein haltbarkeitsverlust. (blutschrei)
            {

            }
            if (cardID == "EX1_145o")// vorbereitung    der nächste zauber, den ihr in diesem zug wirkt, kostet (3) weniger.
            {

            }
            if (cardID == "EX1_055o")// gestaerkt    die manasüchtige hat erhöhten angriff.
            {
                retval.angrbuff = 2;
            }
            if (cardID == "CS2_053e")// fernsicht   eine eurer karten kostet (3) weniger.
            {

            }
            if (cardID == "CS2_146o")//  geschaerft +1 haltbarkeit.
            {
                retval.weapondurability = 1;
            }
            if (cardID == "EX1_059e")//  experimente! der verrückte alchemist hat angriff und leben vertauscht.
            {

            }
            if (cardID == "EX1_565o")// flammenzunge +2 angriff von totem der flammenzunge.
            {
                retval.angrbuff = 2;
            }
            if (cardID == "EX1_001e")// wachsam    erhöhter angriff. (lichtwaechterin)
            {
                retval.angrbuff = 2;
            }
            if (cardID == "EX1_536e")// aufgewertet   erhöhte haltbarkeit.
            {
                retval.weaponAttack = 1;
                retval.weapondurability = 1;
            }
            if (cardID == "EX1_155be")// maldernatur   dieser diener hat +4 leben und spott/.
            {
                retval.hpbuff = 4;
                retval.taunt = true;
            }
            if (cardID == "CS2_103e2")// sturmangriff    +2 angriff und ansturm/.
            {
                retval.angrbuff = 2;
                retval.charge = true;
            }
            if (cardID == "TU4f_006o")// transzendenz    cho kann nicht angegriffen werden, bevor ihr seine diener erledigt habt.
            {

            }
            if (cardID == "EX1_043e")// stundedeszwielichts    erhöhtes leben. (zwielichtdrache)
            {
                retval.hpbuff = 1;
            }
            if (cardID == "NEW1_037e")// bewaffnet   erhöhter angriff. meisterschwertschmied
            {
                retval.angrbuff = 1;
            }
            if (cardID == "EX1_161o")// demoralisierendesgebruell    dieser diener hat -3 angriff in diesem zug.
            {

            }
            if (cardID == "EX1_093e")// handvonargus
            {
                retval.angrbuff = 1;
                retval.hpbuff = 1;
                retval.taunt = true;
            }


            return retval;
        }



    }

}
