using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HREngine.Bots
{
    // reads the board and simulates it
    public class BoardTester
    {
        int ownPlayer = 1;

        int mana = 0;
        int maxmana = 0;
        string ownheroname = "";
        int ownherohp = 0;
        int ownherodefence = 0;
        bool ownheroready = false;
        bool ownHeroimmunewhileattacking = false;
        int ownheroattacksThisRound = 0;
        int ownHeroAttack = 0;
        string ownHeroWeapon="";
        int ownHeroWeaponAttack = 0;
        int ownHeroWeaponDurability = 0;
        int numMinionsPlayedThisTurn =0;
        int cardsPlayedThisTurn = 0;
        int overdrive = 0;

        int enemySecrets = 0;

        bool ownHeroFrozen = false;

        List<string> ownsecretlist = new List<string>();
        string enemyheroname = "";
        int enemyherohp = 0;
        int enemyherodefence = 0;
        bool enemyFrozen = false;
        int enemyWeaponAttack = 0;
        int enemyWeaponDur = 0;
        string enemyWeapon = "";

        List<Minion> ownminions = new List<Minion>();
        List<Minion> enemyminions = new List<Minion>();
        List<Handmanager.Handcard> handcards = new List<Handmanager.Handcard>();

        public BoardTester()
        {
            string[] lines = new string[0] { };
            try
            {
                string path = Settings.Instance.path;
                lines = System.IO.File.ReadAllLines(path + "test.txt");
            }
            catch
            {
                Helpfunctions.Instance.logg("cant find test.txt");
                return;
            }

            CardDB.Card heroability = CardDB.Instance.getCardDataFromID("CS2_034");
            bool abilityReady = false;

            int readstate = 0;
            int counter = 0;

            Minion tempminion = new Minion();
            int j = 0;
            foreach (string sss in lines)
            {
                string s = sss + " ";
                Helpfunctions.Instance.logg(s);

                if (s.StartsWith("ailoop"))
                {
                    break;
                }
                if (s.StartsWith("####"))
                {
                    continue;
                }
                if (s.StartsWith("start calculations"))
                {
                    continue;
                }

                if (s.StartsWith("enemy secretsCount:"))
                {
                    this.enemySecrets = Convert.ToInt32(s.Split(' ')[2]);
                    continue;
                }

                if (s.StartsWith("mana "))
                {
                    string ss = s.Replace("mana ", "");
                    mana = Convert.ToInt32(ss.Split('/')[0]);
                    maxmana = Convert.ToInt32(ss.Split('/')[1]);
                }

                if (readstate == 42 && counter == 1) // player
                {
                    this.overdrive = Convert.ToInt32(s.Split(' ')[2]);
                    this.numMinionsPlayedThisTurn = Convert.ToInt32(s.Split(' ')[0]);
                    this.cardsPlayedThisTurn = Convert.ToInt32(s.Split(' ')[1]);
                    this.ownPlayer = Convert.ToInt32(s.Split(' ')[3]);
                }

                if (readstate == 1 && counter == 1) // class + hp + defence + immune
                {
                    ownheroname = s.Split(' ')[0];
                    ownherohp = Convert.ToInt32(s.Split(' ')[1]);
                    ownherodefence = Convert.ToInt32(s.Split(' ')[2]);
                    string boolim = s.Split(' ')[4];
                    this.ownHeroimmunewhileattacking = (boolim == "True") ? true : false;

                }

                if (readstate == 1 && counter == 2) // ready, num attacks this turn, frozen
                {
                    string readystate = s.Split(' ')[1];
                    this.ownheroready = (readystate == "True") ? true : false;
                    this.ownheroattacksThisRound = Convert.ToInt32(s.Split(' ')[3]);

                    this.ownHeroFrozen = (s.Split(' ')[5]=="True")? true:false;

                    ownHeroAttack = Convert.ToInt32(s.Split(' ')[7]);
                    ownHeroWeaponAttack = Convert.ToInt32(s.Split(' ')[8]);
                    this.ownHeroWeaponDurability = Convert.ToInt32(s.Split(' ')[9]);
                    if (ownHeroWeaponAttack == 0)
                    {
                        ownHeroWeapon = ""; //:D
                    }
                    else
                    {
                        ownHeroWeapon = s.Split(' ')[10];
                    }
                }

                if (readstate == 1 && counter == 3) // ability + abilityready
                {
                    abilityReady = (s.Split(' ')[1] == "True") ? true : false;
                    heroability = CardDB.Instance.getCardDataFromID(s.Split(' ')[2]);
                }

                if (readstate == 1 && counter >= 5) // secrets
                {
                    if (!s.StartsWith("enemyhero:"))
                    {
                        ownsecretlist.Add(s.Replace(" ", ""));
                    }
                }
                
                if (readstate == 2 && counter == 1) // class + hp + defence + frozen
                {
                    enemyheroname = s.Split(' ')[0];
                    enemyherohp = Convert.ToInt32(s.Split(' ')[1]);
                    enemyherodefence = Convert.ToInt32(s.Split(' ')[2]);
                    enemyFrozen = (s.Split(' ')[3] == "True") ? true : false;
                }

                if (readstate == 2 && counter == 2) // wepon + stuff
                {
                    this.enemyWeaponAttack = Convert.ToInt32(s.Split(' ')[0]);
                    this.enemyWeaponDur = Convert.ToInt32(s.Split(' ')[1]);
                    if (enemyWeaponDur == 0)
                    {
                        this.enemyWeapon = "";
                    }
                    else
                    {
                        this.enemyWeapon = s.Split(' ')[2] ;
                    }
                    
                }

                if (readstate == 3) // minion or enchantment
                {
                    if (s.Contains(" id:"))
                    {
                        if (counter >= 2) this.ownminions.Add(tempminion);

                        string minionname = s.Split(' ')[0];
                        int attack = Convert.ToInt32(s.Split(new string[] { " A:" }, StringSplitOptions.RemoveEmptyEntries)[1].Split(' ')[0]);
                        int hp = Convert.ToInt32(s.Split(new string[] { " H:" }, StringSplitOptions.RemoveEmptyEntries)[1].Split(' ')[0]);
                        int maxhp = Convert.ToInt32(s.Split(new string[] { " mH:" }, StringSplitOptions.RemoveEmptyEntries)[1].Split(' ')[0]);
                        bool ready = s.Split(new string[] { " rdy:" }, StringSplitOptions.RemoveEmptyEntries)[1].Split(' ')[0] == "True" ? true : false;
                        bool taunt = s.Split(new string[] { " tnt:" }, StringSplitOptions.RemoveEmptyEntries)[1].Split(' ')[0] == "True" ? true : false;
                        bool silenced = false;
                        if (s.Contains(" silenced:")) silenced = s.Split(new string[] { " silenced:" }, StringSplitOptions.RemoveEmptyEntries)[1].Split(' ')[0] == "True" ? true : false;
                        bool divshield = false;
                        if (s.Contains(" divshield:")) divshield = s.Split(new string[] { " divshield:" }, StringSplitOptions.RemoveEmptyEntries)[1].Split(' ')[0] == "True" ? true : false;
                        bool ptt = false;//played this turn
                        if (s.Contains(" ptt:")) ptt = s.Split(new string[] { " ptt:" }, StringSplitOptions.RemoveEmptyEntries)[1].Split(' ')[0] == "True" ? true : false;
                        bool wndfry = false;//windfurry
                        if (s.Contains(" wndfr:")) wndfry = s.Split(new string[] { " wndfr:" }, StringSplitOptions.RemoveEmptyEntries)[1].Split(' ')[0] == "True" ? true : false;
                        int natt = 0;
                        if (s.Contains(" natt:")) natt = Convert.ToInt32(s.Split(new string[] { " natt:" }, StringSplitOptions.RemoveEmptyEntries)[1].Split(' ')[0]);

                        int ent = 1000 + j;
                        if (s.Contains(" e:")) ent = Convert.ToInt32(s.Split(new string[] { " e:" }, StringSplitOptions.RemoveEmptyEntries)[1].Split(' ')[0]);

                        bool pois = false;//poision
                        if (s.Contains(" poi:")) pois = s.Split(new string[] { " poi:" }, StringSplitOptions.RemoveEmptyEntries)[1].Split(' ')[0] == "True" ? true : false;
                        bool stl = false;//stealth
                        if (s.Contains(" stl:")) stl = s.Split(new string[] { " stl:" }, StringSplitOptions.RemoveEmptyEntries)[1].Split(' ')[0] == "True" ? true : false;
                        bool immn = false;//immune
                        if (s.Contains(" imm:")) immn = s.Split(new string[] { " imm:" }, StringSplitOptions.RemoveEmptyEntries)[1].Split(' ')[0] == "True" ? true : false;
                        bool chrg = false;//charge
                        if (s.Contains(" chrg:")) chrg = s.Split(new string[] { " chrg:" }, StringSplitOptions.RemoveEmptyEntries)[1].Split(' ')[0] == "True" ? true : false;
                        bool ex = false;//exhausted
                        if (s.Contains(" ex:")) ex = s.Split(new string[] { " ex:" }, StringSplitOptions.RemoveEmptyEntries)[1].Split(' ')[0] == "True" ? true : false;


                        int id = Convert.ToInt32(s.Split(new string[] { " id:" }, StringSplitOptions.RemoveEmptyEntries)[1].Split(' ')[0]);
                        tempminion = createNewMinion(new Handmanager.Handcard(CardDB.Instance.getCardData(minionname)), id);
                        tempminion.Angr = attack;
                        tempminion.Hp = hp;
                        tempminion.maxHp = maxhp;
                        tempminion.Ready = ready;
                        tempminion.taunt = taunt;
                        tempminion.divineshild = divshield;
                        tempminion.playedThisTurn = ptt;
                        tempminion.windfury = wndfry;
                        tempminion.numAttacksThisTurn = natt;
                        tempminion.entitiyID = ent;
                        tempminion.handcard.entity = ent;
                        tempminion.silenced = silenced;
                        tempminion.exhausted = ex;
                        tempminion.poisonous = pois;
                        tempminion.stealth = stl;
                        tempminion.immune = immn;
                        tempminion.charge = chrg;
                        if (maxhp > hp) tempminion.wounded = true;





                    }
                    else
                    {
                        try
                        {
                            Enchantment e = CardDB.getEnchantmentFromCardID(s.Split(' ')[0]);
                            e.controllerOfCreator = Convert.ToInt32(s.Split(' ')[2]);
                            e.creator = Convert.ToInt32(s.Split(' ')[1]);
                            tempminion.enchantments.Add(e);
                        }
                        catch
                        {
                        }
                    }

                }

                if (readstate == 4) // minion or enchantment
                {
                    if (s.Contains(" id:"))
                    {
                        if (counter >= 2) this.enemyminions.Add(tempminion);

                        string minionname = s.Split(' ')[0];
                        int attack = Convert.ToInt32(s.Split(new string[] { " A:" }, StringSplitOptions.RemoveEmptyEntries)[1].Split(' ')[0]);
                        int hp = Convert.ToInt32(s.Split(new string[] { " H:" }, StringSplitOptions.RemoveEmptyEntries)[1].Split(' ')[0]);
                        int maxhp = Convert.ToInt32(s.Split(new string[] { " mH:" }, StringSplitOptions.RemoveEmptyEntries)[1].Split(' ')[0]);
                        bool ready = s.Split(new string[] { " rdy:" }, StringSplitOptions.RemoveEmptyEntries)[1].Split(' ')[0] == "True" ? true : false;
                        bool taunt = s.Split(new string[] { " tnt:" }, StringSplitOptions.RemoveEmptyEntries)[1].Split(' ')[0] == "True" ? true : false;
                        bool silenced = false;
                        if (s.Contains(" silenced:")) silenced = s.Split(new string[] { " silenced:" }, StringSplitOptions.RemoveEmptyEntries)[1].Split(' ')[0] == "True" ? true : false;
                        bool divshield = false;
                        if (s.Contains(" divshield:")) divshield = s.Split(new string[] { " divshield:" }, StringSplitOptions.RemoveEmptyEntries)[1].Split(' ')[0] == "True" ? true : false;
                        bool ptt = false;//played this turn
                        if (s.Contains(" ptt:")) ptt = s.Split(new string[] { " ptt:" }, StringSplitOptions.RemoveEmptyEntries)[1].Split(' ')[0] == "True" ? true : false;
                        bool wndfry = false;//windfurry
                        if (s.Contains(" wndfr:")) wndfry = s.Split(new string[] { " wndfr:" }, StringSplitOptions.RemoveEmptyEntries)[1].Split(' ')[0] == "True" ? true : false;
                        int natt = 0;
                        if (s.Contains(" natt:")) natt = Convert.ToInt32(s.Split(new string[] { " natt:" }, StringSplitOptions.RemoveEmptyEntries)[1].Split(' ')[0]);

                        int ent = 1000 + j;
                        if (s.Contains(" e:")) ent = Convert.ToInt32(s.Split(new string[] { " e:" }, StringSplitOptions.RemoveEmptyEntries)[1].Split(' ')[0]);

                        bool pois = false;//poision
                        if (s.Contains(" poi:")) pois = s.Split(new string[] { " poi:" }, StringSplitOptions.RemoveEmptyEntries)[1].Split(' ')[0] == "True" ? true : false;
                        bool stl = false;//stealth
                        if (s.Contains(" stl:")) stl = s.Split(new string[] { " stl:" }, StringSplitOptions.RemoveEmptyEntries)[1].Split(' ')[0] == "True" ? true : false;
                        bool immn = false;//immune
                        if (s.Contains(" imm:")) immn = s.Split(new string[] { " imm:" }, StringSplitOptions.RemoveEmptyEntries)[1].Split(' ')[0] == "True" ? true : false;
                        bool chrg = false;//charge
                        if (s.Contains(" chrg:")) chrg = s.Split(new string[] { " chrg:" }, StringSplitOptions.RemoveEmptyEntries)[1].Split(' ')[0] == "True" ? true : false;
                        bool ex = false;//exhausted
                        if (s.Contains(" ex:")) ex = s.Split(new string[] { " ex:" }, StringSplitOptions.RemoveEmptyEntries)[1].Split(' ')[0] == "True" ? true : false;
                        
                        int id = Convert.ToInt32(s.Split(new string[] { " id:" }, StringSplitOptions.RemoveEmptyEntries)[1].Split(' ')[0]);
                        tempminion = createNewMinion(new Handmanager.Handcard(CardDB.Instance.getCardData(minionname)), id);
                        tempminion.Angr = attack;
                        tempminion.Hp = hp;
                        tempminion.maxHp = maxhp;
                        tempminion.Ready = ready;
                        tempminion.taunt = taunt;
                        tempminion.divineshild = divshield;
                        tempminion.playedThisTurn = ptt;
                        tempminion.windfury = wndfry;
                        tempminion.numAttacksThisTurn = natt;
                        tempminion.entitiyID = ent;
                        tempminion.silenced = silenced;
                        tempminion.exhausted = ex;
                        tempminion.poisonous = pois;
                        tempminion.stealth = stl;
                        tempminion.immune = immn;
                        tempminion.charge = chrg;
                        if (maxhp > hp) tempminion.wounded = true;


                    }
                    else
                    {
                        try
                        {
                            Enchantment e = CardDB.getEnchantmentFromCardID(s.Split(' ')[0]);
                            e.controllerOfCreator = Convert.ToInt32(s.Split(' ')[2]);
                            e.creator = Convert.ToInt32(s.Split(' ')[1]);
                            tempminion.enchantments.Add(e);
                        }
                        catch
                        { 
                        }
                    }

                }

                if (readstate == 5) // minion or enchantment
                {

                    Handmanager.Handcard card = new Handmanager.Handcard();

                    string minionname = s.Split(' ')[2];
                    int pos = Convert.ToInt32(s.Split(' ')[1]);
                    int mana = Convert.ToInt32(s.Split(' ')[3]);
                    card.card = CardDB.Instance.getCardData(minionname);
                    card.entity = Convert.ToInt32(s.Split(' ')[5]);
                    card.manacost = mana;
                    card.position = pos;
                    handcards.Add(card);

                }


                if (s.StartsWith("ownhero:"))
                {
                    readstate = 1;
                    counter = 0;
                }

                if (s.StartsWith("enemyhero:"))
                {
                    readstate = 2;
                    counter = 0;
                }

                if (s.StartsWith("OwnMinions:"))
                {
                    readstate = 3;
                    counter = 0;
                }

                if (s.StartsWith("EnemyMinions:"))
                {
                    if (counter >= 2) this.ownminions.Add(tempminion);

                    readstate = 4;
                    counter = 0;
                }

                if (s.StartsWith("Own Handcards:"))
                {
                    if (counter >= 2) this.enemyminions.Add(tempminion);

                    readstate = 5;
                    counter = 0;
                }

                if (s.StartsWith("player:"))
                {
                    readstate = 42;
                    counter = 0;
                }



                counter++;
                j++;
            }
            Helpfunctions.Instance.logg("rdy");


            Hrtprozis.Instance.setOwnPlayer(ownPlayer);
            Handmanager.Instance.setOwnPlayer(ownPlayer);

            Hrtprozis.Instance.updatePlayer(this.maxmana, this.mana, this.cardsPlayedThisTurn, this.numMinionsPlayedThisTurn,this.overdrive, 100, 200);
            Hrtprozis.Instance.updateSecretStuff(this.ownsecretlist, enemySecrets);

            int numattttHero = 0;
            bool herowindfury = false;
            Hrtprozis.Instance.updateOwnHero(this.ownHeroWeapon, this.ownHeroWeaponAttack, this.ownHeroWeaponDurability, ownHeroimmunewhileattacking, this.ownHeroAttack, this.ownherohp, this.ownherodefence, this.ownheroname, this.ownheroready, this.ownHeroFrozen, heroability, abilityReady, numattttHero, herowindfury);
            Hrtprozis.Instance.updateEnemyHero(this.enemyWeapon,this.enemyWeaponAttack, this.enemyWeaponDur, this.enemyWeaponAttack, this.enemyherohp, this.enemyherodefence, this.enemyheroname,this.enemyFrozen);

            Hrtprozis.Instance.updateMinions(this.ownminions, this.enemyminions);
            Handmanager.Instance.setHandcards(this.handcards, this.handcards.Count, 5);


        }



        private Minion createNewMinion(Handmanager.Handcard hc, int id)
        {
            Minion m = new Minion();
            m.handcard = hc;
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

            if (hc.card.poisionous) m.poisonous = true;

            if (hc.card.Stealth) m.stealth = true;

            if (m.name == "lightspawn" && !m.silenced)
            {
                m.Angr = m.Hp;
            }


            return m;
        }



    }

}
