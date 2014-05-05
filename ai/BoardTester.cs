using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HREngine.Bots
{
    // reads the board and simulates it
    class BoardTester
    {
        int mana = 0;
        int maxmana = 0;
        string ownheroname = "";
        int ownherohp = 0;
        int ownherodefence = 0;
        bool ownheroready = false;
        int ownheroattacksThisRound = 0;
        int ownHeroAttack = 0;
        string ownHeroWeapon="";
        int ownHeroWeaponAttack = 0;
        int ownHeroWeaponDurability = 0;
        int numMinionsPlayedThisTurn =0;
        int cardsPlayedThisTurn = 0;


        string enemyheroname = "";
        int enemyherohp = 0;
        int enemyherodefence = 0;

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

            int readstate = 0;
            int counter = 0;

            Minion tempminion = new Minion();
            foreach (string sss in lines)
            {
                string s = sss + " ";
                Helpfunctions.Instance.logg(s);

                if (s.StartsWith("ailoop"))
                {
                    break;
                }

                if (s.StartsWith("mana "))
                {
                    string ss = s.Replace("mana ", "");
                    mana = Convert.ToInt32(ss.Split('/')[0]);
                    maxmana = Convert.ToInt32(ss.Split('/')[1]);
                }

                if (readstate == 1 && counter == 1) // class + hp + defence
                {
                    ownheroname = s.Split(' ')[0];
                    ownherohp = Convert.ToInt32(s.Split(' ')[1]);
                    ownherodefence = Convert.ToInt32(s.Split(' ')[2]);

                }

                if (readstate == 1 && counter == 2) // ready
                {
                    string readystate = s.Split(' ')[1];
                    this.ownheroready = (readystate == "True") ? true : false;


                    this.ownheroattacksThisRound = Convert.ToInt32(s.Split(' ')[3]);
                    ownHeroAttack = Convert.ToInt32(s.Split(' ')[5]);
                    ownHeroWeaponAttack = Convert.ToInt32(s.Split(' ')[8]);
                    if (ownHeroWeaponAttack >= 1)
                    {
                        this.ownHeroWeaponDurability = 1;
                        ownHeroWeapon = "axt"; //:D
                    }
                }

                if (readstate == 2 && counter == 1) // class + hp + defence
                {
                    enemyheroname = s.Split(' ')[0];
                    enemyherohp = Convert.ToInt32(s.Split(' ')[1]);
                    enemyherodefence = Convert.ToInt32(s.Split(' ')[2]);

                }

                if (readstate == 3) // minion or enchantment
                {
                    if (s.Contains(" id "))
                    {
                        if (counter >= 2) this.ownminions.Add(tempminion);

                        string minionname = s.Split(' ')[0];
                        int attack = Convert.ToInt32(s.Split(new string[] { " A:" }, StringSplitOptions.RemoveEmptyEntries)[1].Split(' ')[0]);
                        int hp = Convert.ToInt32(s.Split(new string[] { " H:" }, StringSplitOptions.RemoveEmptyEntries)[1].Split(' ')[0]);
                        bool ready = s.Split(new string[] { " rdy:" }, StringSplitOptions.RemoveEmptyEntries)[1].Split(' ')[0] == "True" ? true : false;
                        bool taunt = s.Split(new string[] { " tnt:" }, StringSplitOptions.RemoveEmptyEntries)[1].Split(' ')[0] == "True" ? true : false;
                        bool silenced = false;
                        if (s.Contains(" silenced:")) silenced = s.Split(new string[] { " silenced:" }, StringSplitOptions.RemoveEmptyEntries)[1].Split(' ')[0] == "True" ? true : false;
                        bool divshield = false;
                        if (s.Contains(" divshield:")) divshield = s.Split(new string[] { " divshield:" }, StringSplitOptions.RemoveEmptyEntries)[1].Split(' ')[0] == "True" ? true : false;

                        tempminion = createNewMinion(CardDB.Instance.getCardData(minionname), 0);

                        tempminion.Angr = attack;
                        tempminion.Hp = hp;
                        tempminion.maxHp = hp;
                        tempminion.Ready = ready;
                        tempminion.taunt = taunt;
                        tempminion.divineshild = divshield;





                    }
                    else
                    {
                        Enchantment e = CardDB.getEnchantmentFromCardID(s);
                        e.controllerOfCreator = 1;
                        e.creator = 1;
                        tempminion.enchantments.Add(e);
                    }

                }

                if (readstate == 4) // minion or enchantment
                {
                    if (s.Contains(" id "))
                    {
                        if (counter >= 2) this.enemyminions.Add(tempminion);

                        string minionname = s.Split(' ')[0];
                        int attack = Convert.ToInt32(s.Split(new string[] { " A:" }, StringSplitOptions.RemoveEmptyEntries)[1].Split(' ')[0]);
                        int hp = Convert.ToInt32(s.Split(new string[] { " H:" }, StringSplitOptions.RemoveEmptyEntries)[1].Split(' ')[0]);
                        bool ready = s.Split(new string[] { " rdy:" }, StringSplitOptions.RemoveEmptyEntries)[1].Split(' ')[0] == "True" ? true : false;
                        bool taunt = s.Split(new string[] { " tnt:" }, StringSplitOptions.RemoveEmptyEntries)[1].Split(' ')[0] == "True" ? true : false;
                        bool silenced = false;
                        if (s.Contains(" silenced:")) silenced = s.Split(new string[] { " silenced:" }, StringSplitOptions.RemoveEmptyEntries)[1].Split(' ')[0] == "True" ? true : false;
                        bool divshield = false;
                        if (s.Contains(" divshield:")) divshield = s.Split(new string[] { " divshield:" }, StringSplitOptions.RemoveEmptyEntries)[1].Split(' ')[0] == "True" ? true : false;

                        tempminion = createNewMinion(CardDB.Instance.getCardData(minionname), 0);

                        tempminion.Angr = attack;
                        tempminion.Hp = hp;
                        tempminion.maxHp = hp;
                        tempminion.Ready = ready;
                        tempminion.taunt = taunt;
                        tempminion.divineshild = divshield;




                    }
                    else
                    {
                        Enchantment e = CardDB.getEnchantmentFromCardID(s);
                        e.controllerOfCreator = 1;
                        e.creator = 1;
                        tempminion.enchantments.Add(e);
                    }

                }

                if (readstate == 5) // minion or enchantment
                {

                    Handmanager.Handcard card = new Handmanager.Handcard();

                    string minionname = s.Split(' ')[2];
                    int pos = Convert.ToInt32(s.Split(' ')[1]);
                    int mana = Convert.ToInt32(s.Split(' ')[3]);
                    card.card = CardDB.Instance.getCardData(minionname);
                    card.entity = pos;
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


                counter++;
            }
            Helpfunctions.Instance.logg("rdy");
            CardDB.Card heroability = CardDB.Instance.getCardDataFromID("CS2_034");

            Hrtprozis.Instance.setOwnPlayer(1);
            Handmanager.Instance.setOwnPlayer(1);

            Hrtprozis.Instance.updatePlayer(this.maxmana, this.mana, this.cardsPlayedThisTurn, this.numMinionsPlayedThisTurn,0, 100, 200);
            Hrtprozis.Instance.updateSecretStuff(new List<string>(), 0);

            Hrtprozis.Instance.updateOwnHero(this.ownHeroWeapon, this.ownHeroWeaponAttack, this.ownHeroWeaponDurability,false, this.ownHeroAttack, this.ownherohp, this.ownherodefence, this.ownheroname, this.ownheroready, false, heroability, false, 0, false);
            Hrtprozis.Instance.updateEnemyHero("",0, 0, 0, this.enemyherohp, this.enemyherodefence, this.enemyheroname,false);

            Hrtprozis.Instance.updateMinions(this.ownminions, this.enemyminions);
            Handmanager.Instance.setHandcards(this.handcards, this.handcards.Count, 5);


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



    }

}
