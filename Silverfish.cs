using HREngine.API;
using HREngine.API.Actions;
using HREngine.API.Utilities;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{

    public class Silverfish
    {
        public int versionnumber = 97;
        private bool singleLog = false;
        private string botbehave = "rush";

        Settings sttngs = Settings.Instance;

        List<Minion> ownMinions = new List<Minion>();
        List<Minion> enemyMinions = new List<Minion>();
        List<Handmanager.Handcard> handCards = new List<Handmanager.Handcard>();
        int ownPlayerController = 0;
        List<string> ownSecretList = new List<string>();
        int enemySecretCount = 0;

        int currentMana = 0;
        int ownMaxMana = 0;
        int numMinionsPlayedThisTurn = 0;
        int cardsPlayedThisTurn = 0;
        int ueberladung = 0;

        int enemyMaxMana = 0;


        string ownHeroWeapon = "";
        int heroWeaponAttack = 0;
        int heroWeaponDurability = 0;
        bool heroImmuneToDamageWhileAttacking = false;
        bool heroImmune = false;
        bool enemyHeroImmune = false;

        string enemyHeroWeapon = "";
        int enemyWeaponAttack = 0;
        int enemyWeaponDurability = 0;

        int heroAtk = 0;
        int heroHp = 30;
        int heroDefence = 0;
        string heroname = "";
        bool ownheroisread = false;
        int heroNumAttacksThisTurn = 0;
        bool heroHasWindfury = false;
        bool herofrozen = false;

        int enemyAtk = 0;
        int enemyHp = 30;
        string enemyHeroname = "";
        int enemyDefence = 0;
        bool enemyfrozen = false;

        CardDB.Card heroAbility = new CardDB.Card();
        bool ownAbilityisReady = false;
        CardDB.Card enemyAbility = new CardDB.Card();

        int anzcards = 0;
        int enemyAnzCards = 0;

        int ownHeroFatigue = 0;
        int enemyHeroFatigue = 0;
        int ownDecksize = 0;
        int enemyDecksize = 0;

        public Silverfish( bool snglLg)
        {
            this.singleLog = snglLg;
            Helpfunctions.Instance.ErrorLog("init Silverfish");
            string path = (HRSettings.Get.CustomRuleFilePath).Remove(HRSettings.Get.CustomRuleFilePath.Length - 13) + "UltimateLogs" + System.IO.Path.DirectorySeparatorChar;
            System.IO.Directory.CreateDirectory(path);
            sttngs.setFilePath((HRSettings.Get.CustomRuleFilePath).Remove(HRSettings.Get.CustomRuleFilePath.Length - 13) + "Bots" + System.IO.Path.DirectorySeparatorChar + "silver" + System.IO.Path.DirectorySeparatorChar);

            if (!singleLog)
            {
                sttngs.setLoggPath(path);
            }
            else
            {
                sttngs.setLoggPath((HRSettings.Get.CustomRuleFilePath).Remove(HRSettings.Get.CustomRuleFilePath.Length - 13));
                sttngs.setLoggFile("UILogg.txt");
                Helpfunctions.Instance.createNewLoggfile();
            }
            PenalityManager.Instance.setCombos();
            Mulligan m = Mulligan.Instance; // read the mulligan list
        }

        public void setnewLoggFile()
        {
            if (!singleLog)
            {
                sttngs.setLoggFile("UILogg" + DateTime.Now.ToString("_yyyy-MM-dd_HH-mm-ss") + ".txt");
                Helpfunctions.Instance.createNewLoggfile();
                Helpfunctions.Instance.ErrorLog("#######################################################");
                Helpfunctions.Instance.ErrorLog("fight is logged in: " + sttngs.logpath + sttngs.logfile);
                Helpfunctions.Instance.ErrorLog("#######################################################");
            }
            else
            {
                sttngs.setLoggFile("UILogg.txt");
            }
        }

        public void updateEverything(Behavior botbase)
        {
            this.botbehave = "rush";
            if (botbase is BehaviorControl) this.botbehave = "control";
            if (Ai.Instance.secondturnsim) this.botbehave += " twoturnsim";
            if (Ai.Instance.playaround) this.botbehave += " playaround";
            HRPlayer ownPlayer = HRPlayer.GetLocalPlayer();
            HRPlayer enemyPlayer = HRPlayer.GetEnemyPlayer();
            ownPlayerController = ownPlayer.GetHero().GetControllerId();//ownPlayer.GetHero().GetControllerId()


            // create hero + minion data
            getHerostuff();
            getMinions();
            getHandcards();
            getDecks();

            // send ai the data:
            Hrtprozis.Instance.clearAll();
            Handmanager.Instance.clearAll();

            Hrtprozis.Instance.setOwnPlayer(ownPlayerController);
            Handmanager.Instance.setOwnPlayer(ownPlayerController);

            Hrtprozis.Instance.updatePlayer(this.ownMaxMana, this.currentMana, this.cardsPlayedThisTurn, this.numMinionsPlayedThisTurn, this.ueberladung, ownPlayer.GetHero().GetEntityId(), enemyPlayer.GetHero().GetEntityId());
            Hrtprozis.Instance.updateSecretStuff(this.ownSecretList, this.enemySecretCount);

            Hrtprozis.Instance.updateOwnHero(this.ownHeroWeapon, this.heroWeaponAttack, this.heroWeaponDurability, this.heroImmuneToDamageWhileAttacking, this.heroAtk, this.heroHp, this.heroDefence, this.heroname, this.ownheroisread, this.herofrozen, this.heroAbility, this.ownAbilityisReady, this.heroNumAttacksThisTurn, this.heroHasWindfury, this.heroImmune);
            Hrtprozis.Instance.updateEnemyHero(this.enemyHeroWeapon, this.enemyWeaponAttack, this.enemyWeaponDurability, this.enemyAtk, this.enemyHp, this.enemyDefence, this.enemyHeroname, this.enemyfrozen, this.enemyAbility, this.enemyHeroImmune, this.enemyMaxMana);

            Hrtprozis.Instance.updateMinions(this.ownMinions, this.enemyMinions);
            Handmanager.Instance.setHandcards(this.handCards, this.anzcards, this.enemyAnzCards);

            Hrtprozis.Instance.updateFatigueStats(this.ownDecksize, this.ownHeroFatigue, this.enemyDecksize, this.enemyHeroFatigue);

            // print data
            Hrtprozis.Instance.printHero();
            Hrtprozis.Instance.printOwnMinions();
            Hrtprozis.Instance.printEnemyMinions();
            Handmanager.Instance.printcards();

            // calculate stuff
            Helpfunctions.Instance.ErrorLog("calculating stuff... " + DateTime.Now.ToString("HH:mm:ss.ffff"));
            Ai.Instance.dosomethingclever(botbase);
            Helpfunctions.Instance.ErrorLog("calculating ended! " + DateTime.Now.ToString("HH:mm:ss.ffff"));

        }

        private void getHerostuff()
        {
            Dictionary<int, HREntity> allEntitys = HRGame.GetEntityMap();

            HRPlayer ownPlayer = HRPlayer.GetLocalPlayer();
            HRPlayer enemyPlayer = HRPlayer.GetEnemyPlayer();

            HREntity ownhero = ownPlayer.GetHero();
            HREntity enemyhero = enemyPlayer.GetHero();
            HREntity ownHeroAbility = ownPlayer.GetHeroPower();

            //player stuff#########################
            //this.currentMana =ownPlayer.GetTag(HRGameTag.RESOURCES) - ownPlayer.GetTag(HRGameTag.RESOURCES_USED) + ownPlayer.GetTag(HRGameTag.TEMP_RESOURCES);
            this.currentMana = ownPlayer.GetNumAvailableResources();
            this.ownMaxMana = ownPlayer.GetTag(HRGameTag.RESOURCES);
            this.enemyMaxMana = enemyPlayer.GetTag(HRGameTag.RESOURCES);
            Helpfunctions.Instance.logg("#######################################################################");
            Helpfunctions.Instance.logg("#######################################################################");
            Helpfunctions.Instance.logg("start calculations, current time: " + DateTime.Now.ToString("HH:mm:ss") + " V" + this.versionnumber + " " + this.botbehave);
            Helpfunctions.Instance.logg("#######################################################################");
            Helpfunctions.Instance.logg("mana " + currentMana + "/" + ownMaxMana);
            Helpfunctions.Instance.logg("emana " + enemyMaxMana);
            Helpfunctions.Instance.logg("own secretsCount: " + ownPlayer.GetSecretDefinitions().Count);
            enemySecretCount = HRCard.GetCards(enemyPlayer, HRCardZone.SECRET).Count;
            enemySecretCount = 0;
            //count enemy secrets
            foreach (HREntity ent in allEntitys.Values)
            {
                if (ent.IsSecret() && ent.GetControllerId() == enemyPlayer.GetControllerId() && ent.GetZone() == HRCardZone.SECRET) enemySecretCount++;
            }
            

            
            Helpfunctions.Instance.logg("enemy secretsCount: " + enemySecretCount);
            this.ownSecretList = ownPlayer.GetSecretDefinitions();
            this.numMinionsPlayedThisTurn = ownPlayer.GetTag(HRGameTag.NUM_MINIONS_PLAYED_THIS_TURN);
            this.cardsPlayedThisTurn = ownPlayer.GetTag(HRGameTag.NUM_CARDS_PLAYED_THIS_TURN);
            //if (ownPlayer.HasCombo()) this.cardsPlayedThisTurn = 1;
            this.ueberladung = ownPlayer.GetTag(HRGameTag.RECALL_OWED);

            //get weapon stuff
            this.ownHeroWeapon = "";
            this.heroWeaponAttack = 0;
            this.heroWeaponDurability = 0;

            this.ownHeroFatigue = ownhero.GetFatigue();
            this.enemyHeroFatigue = enemyhero.GetFatigue();

            this.ownDecksize = 0;
            this.enemyDecksize = 0;
            //count decksize
            foreach (HREntity ent in allEntitys.Values)
            {
                if (ent.GetControllerId() == ownPlayer.GetControllerId() && ent.GetZone() == HRCardZone.DECK) ownDecksize++;
                if (ent.GetControllerId() == enemyPlayer.GetControllerId() && ent.GetZone() == HRCardZone.DECK) enemyDecksize++;
            }

            this.heroImmune = (ownhero.GetTag(HRGameTag.CANT_BE_DAMAGED) == 1) ? true : false;
            this.enemyHeroImmune = (enemyhero.GetTag(HRGameTag.CANT_BE_DAMAGED) == 1) ? true : false;

            this.enemyHeroWeapon = "";
            this.enemyWeaponAttack = 0;
            this.enemyWeaponDurability = 0;
            if (enemyPlayer.HasWeapon())
            {
                HREntity weapon = enemyPlayer.GetWeaponCard().GetEntity();
                this.enemyHeroWeapon = CardDB.Instance.getCardDataFromID(CardDB.Instance.cardIdstringToEnum(weapon.GetCardId())).name.ToString();
                this.enemyWeaponAttack = weapon.GetATK();
                this.enemyWeaponDurability = weapon.GetDurability();

            }


            //own hero stuff###########################
            this.heroAtk = ownhero.GetATK();
            this.heroHp = ownhero.GetHealth() - ownhero.GetDamage();
            this.heroDefence = ownhero.GetArmor();
            this.heroname = Hrtprozis.Instance.heroIDtoName(ownhero.GetCardId());
            bool exausted = false;
            exausted = ownhero.IsExhausted();
            this.ownheroisread = true;

            this.heroImmuneToDamageWhileAttacking = false;
            this.herofrozen = ownhero.IsFrozen();
            this.heroNumAttacksThisTurn = ownhero.GetNumAttacksThisTurn();
            this.heroHasWindfury = ownhero.HasWindfury();
            //int numberofattacks = ownhero.GetNumAttacksThisTurn();

            //Helpfunctions.Instance.ErrorLog(ownhero.GetName() + " ready params ex: " + exausted + " " + heroAtk + " " + numberofattacks + " " + herofrozen);


            if (ownPlayer.HasWeapon())
            {
                HREntity weapon = ownPlayer.GetWeaponCard().GetEntity();
                this.ownHeroWeapon = CardDB.Instance.getCardDataFromID(CardDB.Instance.cardIdstringToEnum(weapon.GetCardId())).name.ToString();
                this.heroWeaponAttack = weapon.GetATK();
                this.heroWeaponDurability = weapon.GetTag(HRGameTag.DURABILITY) - weapon.GetTag(HRGameTag.DAMAGE);//weapon.GetDurability();
                this.heroImmuneToDamageWhileAttacking = false;
                if (this.ownHeroWeapon == "gladiatorslongbow")
                {
                    this.heroImmuneToDamageWhileAttacking = true;
                }
                if (this.ownHeroWeapon == "doomhammer")
                {
                    this.heroHasWindfury = true;
                }

                //Helpfunctions.Instance.ErrorLog("weapon: " + ownHeroWeapon + " " + heroWeaponAttack + " " + heroWeaponDurability);

            }
            if(exausted) this.ownheroisread = false;

            if (this.heroNumAttacksThisTurn ==  1 && this.heroHasWindfury )
            {
                this.ownheroisread = true;
            }
            if (exausted == false && this.heroAtk == 0)
            {
                this.ownheroisread = false;
            }
            if (herofrozen) ownheroisread = false;


            //enemy hero stuff###############################################################
            this.enemyAtk = enemyhero.GetATK();

            this.enemyHp = enemyhero.GetHealth() - enemyhero.GetDamage();

            this.enemyHeroname = Hrtprozis.Instance.heroIDtoName(enemyhero.GetCardId());

            this.enemyDefence = enemyhero.GetArmor();

            this.enemyfrozen = enemyhero.IsFrozen();






            //own hero ablity stuff###########################################################

            this.heroAbility = CardDB.Instance.getCardDataFromID(CardDB.Instance.cardIdstringToEnum(ownHeroAbility.GetCardId()));
            this.ownAbilityisReady = (ownHeroAbility.IsExhausted()) ? false : true; // if exhausted, ability is NOT ready

            this.enemyAbility = CardDB.Instance.getCardDataFromID(CardDB.Instance.cardIdstringToEnum(enemyhero.GetHeroPower().GetCardId()));



        }

        private void getMinions()
        {
            ownMinions.Clear();
            enemyMinions.Clear();
            HRPlayer ownPlayer = HRPlayer.GetLocalPlayer();
            HRPlayer enemyPlayer = HRPlayer.GetEnemyPlayer();

            // ALL minions on Playfield:
            List<HRCard> list = HRCard.GetCards(ownPlayer, HRCardZone.PLAY);
            list.AddRange(HRCard.GetCards(enemyPlayer, HRCardZone.PLAY));

            List<HREntity> enchantments = new List<HREntity>();


            foreach (HRCard item in list)
            {
                HREntity entitiy = item.GetEntity();
                int zp = entitiy.GetZonePosition();

                if (entitiy.GetCardType() == HRCardType.MINION && zp >= 1)
                {
                    //Helpfunctions.Instance.ErrorLog("zonepos " + zp);
                    CardDB.Card c = CardDB.Instance.getCardDataFromID(CardDB.Instance.cardIdstringToEnum(entitiy.GetCardId()));
                    Minion m = new Minion();
                    m.name = c.name;
                    m.handcard.card = c;
                    m.Angr = entitiy.GetATK();
                    m.maxHp = entitiy.GetHealth();
                    m.Hp = m.maxHp - entitiy.GetDamage();
                    m.wounded = false;
                    if (m.maxHp > m.Hp) m.wounded = true;


                    m.exhausted = entitiy.IsExhausted();

                    m.taunt = (entitiy.HasTaunt()) ? true : false;

                    m.charge = (entitiy.HasCharge()) ? true : false;

                    m.numAttacksThisTurn = entitiy.GetNumAttacksThisTurn();

                    int temp = entitiy.GetNumTurnsInPlay();
                    m.playedThisTurn = (temp == 0) ? true : false;

                    m.windfury = (entitiy.HasWindfury()) ? true : false;

                    m.frozen = (entitiy.IsFrozen()) ? true : false;

                    m.divineshild = (entitiy.HasDivineShield()) ? true : false;

                    m.stealth = (entitiy.IsStealthed()) ? true : false;

                    m.poisonous = (entitiy.IsPoisonous()) ? true : false;

                    m.immune = (entitiy.IsImmune()) ? true : false;

                    m.silenced = (entitiy.GetTag(HRGameTag.SILENCED) >= 1) ? true : false;


                    m.zonepos = zp;
                    m.id = m.zonepos - 1;

                    m.entitiyID = entitiy.GetEntityId();

                    m.enchantments.Clear();

                    //Helpfunctions.Instance.ErrorLog(  m.name + " ready params ex: " + m.exhausted + " charge: " +m.charge + " attcksthisturn: " + m.numAttacksThisTurn + " playedthisturn " + m.playedThisTurn );

                    m.Ready = false; // if exhausted, he is NOT ready

                    if (!m.playedThisTurn && !m.frozen && (m.numAttacksThisTurn == 0 || (m.numAttacksThisTurn == 1 && m.windfury)))
                    {
                        m.Ready = true;
                    }

                    if (m.playedThisTurn && m.charge && (m.numAttacksThisTurn == 0 || (m.numAttacksThisTurn == 1 && m.windfury)))
                    {
                        //m.exhausted = false;
                        m.Ready = true;
                    }

                    if (!m.silenced && (m.name == CardDB.cardName.ancientwatcher || m.name == CardDB.cardName.ragnarosthefirelord))
                    {
                        m.Ready = false;
                    }


                    if (entitiy.GetControllerId() == this.ownPlayerController) // OWN minion
                    {

                        this.ownMinions.Add(m);
                    }
                    else
                    {
                        this.enemyMinions.Add(m);
                    }

                }
                // minions added

                if (entitiy.GetCardType() == HRCardType.WEAPON)
                {
                    //Helpfunctions.Instance.ErrorLog("found weapon!");
                    if (entitiy.GetControllerId() == this.ownPlayerController) // OWN weapon
                    {
                        this.ownHeroWeapon = CardDB.Instance.getCardDataFromID(CardDB.Instance.cardIdstringToEnum(entitiy.GetCardId())).name.ToString();
                        this.heroWeaponAttack = entitiy.GetATK();
                        this.heroWeaponDurability = entitiy.GetDurability();
                        //this.heroImmuneToDamageWhileAttacking = false;


                    }
                    else
                    {
                        this.enemyHeroWeapon = CardDB.Instance.getCardDataFromID(CardDB.Instance.cardIdstringToEnum(entitiy.GetCardId())).name.ToString();
                        this.enemyWeaponAttack = entitiy.GetATK();
                        this.enemyWeaponDurability = entitiy.GetDurability();
                    }
                }

                if (entitiy.GetCardType() == HRCardType.ENCHANTMENT)
                {

                    enchantments.Add(entitiy);
                }


            }

            foreach (HRCard item in list)
            {
                foreach (HREntity e in item.GetEntity().GetEnchantments())
                {
                    enchantments.Add(e);
                }
            }


            // add enchantments to minions
            setEnchantments(enchantments);
        }

        private void setEnchantments(List<HREntity> enchantments)
        {
            foreach (HREntity bhu in enchantments)
            {
                //create enchantment
                Enchantment ench = CardDB.getEnchantmentFromCardID(CardDB.Instance.cardIdstringToEnum(bhu.GetCardId()));
                ench.creator = bhu.GetCreatorId();
                ench.controllerOfCreator = bhu.GetControllerId();
                ench.cantBeDispelled = false;
                //if (bhu.c) ench.cantBeDispelled = true;

                foreach (Minion m in this.ownMinions)
                {
                    if (m.entitiyID == bhu.GetAttached())
                    {
                        m.enchantments.Add(ench);
                        //Helpfunctions.Instance.ErrorLog("add enchantment " +bhu.GetCardId()+" to: " + m.entitiyID);
                    }

                }

                foreach (Minion m in this.enemyMinions)
                {
                    if (m.entitiyID == bhu.GetAttached())
                    {
                        m.enchantments.Add(ench);
                    }

                }

            }

        }

        private void getHandcards()
        {
            handCards.Clear();
            this.anzcards = 0;
            this.enemyAnzCards = 0;
            List<HRCard> list = HRCard.GetCards(HRPlayer.GetLocalPlayer(), HRCardZone.HAND);

            foreach (HRCard item in list)
            {

                HREntity entitiy = item.GetEntity();

                if (entitiy.GetControllerId() == this.ownPlayerController && entitiy.GetZonePosition() >= 1) // own handcard
                {
                    CardDB.Card c = CardDB.Instance.getCardDataFromID(CardDB.Instance.cardIdstringToEnum(entitiy.GetCardId()));
                    //c.cost = entitiy.GetCost();
                    //c.entityID = entitiy.GetEntityId();

                    Handmanager.Handcard hc = new Handmanager.Handcard();
                    hc.card = c;
                    hc.position = entitiy.GetZonePosition();
                    hc.entity = entitiy.GetEntityId();
                    hc.manacost = entitiy.GetCost();
                    handCards.Add(hc);
                    this.anzcards++;
                }

                
            }

            Dictionary<int, HREntity> allEntitys = HRGame.GetEntityMap();

            foreach (HREntity ent in allEntitys.Values)
            {
                if (ent.GetControllerId() != this.ownPlayerController && ent.GetZonePosition() >= 1 && ent.GetZone() == HRCardZone.HAND) // enemy handcard
                {
                    this.enemyAnzCards++;
                }
            }

        }

        private void getDecks()
        {
            Dictionary<int, HREntity> allEntitys = HRGame.GetEntityMap();

            int owncontroler = HRPlayer.GetLocalPlayer().GetControllerId();
            int enemycontroler = HRPlayer.GetEnemyPlayer().GetControllerId();
            List<CardDB.cardIDEnum> ownCards = new List<CardDB.cardIDEnum>();
            List<CardDB.cardIDEnum> enemyCards = new List<CardDB.cardIDEnum>();

            foreach (HREntity ent in allEntitys.Values)
            {
                if (ent.GetZone() == HRCardZone.SECRET && ent.GetControllerId() == enemycontroler) continue; // cant know enemy secrets :D

                if (ent.GetCardType() == HRCardType.MINION || ent.GetCardType() == HRCardType.WEAPON || ent.GetCardType() == HRCardType.ABILITY)
                {
                    CardDB.cardIDEnum cardid = CardDB.Instance.cardIdstringToEnum(ent.GetCardId());
                    //string owner = "own";
                    //if (ent.GetControllerId() == enemycontroler) owner = "enemy";
                    //if (ent.GetControllerId() == enemycontroler && ent.GetZone() == HRCardZone.HAND) Helpfunctions.Instance.logg("enemy card in hand: " + "cardindeck: " + cardid + " " + ent.GetName());
                    //if (cardid != CardDB.cardIDEnum.None) Helpfunctions.Instance.logg("cardindeck: " + cardid + " " + ent.GetName() + " " + ent.GetZone() + " " + owner + " " + ent.GetCardType());
                    if (cardid != CardDB.cardIDEnum.None)
                    {
                        if (ent.GetControllerId() == owncontroler)
                        {
                            ownCards.Add(cardid);
                        }
                        else
                        {
                            enemyCards.Add(cardid);
                        }
                    }
                }

            }

            Probabilitymaker.Instance.setOwnCards(ownCards);
            Probabilitymaker.Instance.setEnemyCards(enemyCards);

        }

    }

}

