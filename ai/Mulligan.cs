using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HREngine.Bots
{

    public class Mulligan
    {
        public class CardIDEntity
        {
            public string id = "";
            public int entitiy = 0;
            public CardIDEntity(string id, int entt)
            {
                this.id = id;
                this.entitiy = entt;
            }
        }

        class mulliitem
        {
            public string cardid = "";
            public string enemyclass = "";
            public int howmuch = 2;

            public mulliitem(string id, string enemy, int number)
            {
                this.cardid = id;
                this.enemyclass = enemy;
                this.howmuch=number;
            }
        }

        List<mulliitem> holdlist = new List<mulliitem>();
        List<mulliitem> deletelist = new List<mulliitem>();
        private static Mulligan instance;

        public static Mulligan Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Mulligan();
                }
                return instance;
            }
        }

        private Mulligan()
        {
            readCombos();
        }

        private void readCombos()
        {
            string[] lines = new string[0] { };
            this.holdlist.Clear();
            this.deletelist.Clear();
            try
            {
                string path = Settings.Instance.path;
                lines = System.IO.File.ReadAllLines(path + "_mulligan.txt");
            }
            catch
            {
                Helpfunctions.Instance.logg("cant find _mulligan.txt");
                return;
            }
            Helpfunctions.Instance.logg("read _mulligan.txt...");
            foreach (string line in lines)
            {

                if (line.StartsWith("hold;"))
                {
                    try
                    {
                        string enemyclass = line.Split(';')[1];
                        string cardlist = line.Split(';')[2];
                        foreach (string crd in cardlist.Split(','))
                        {
                            if (crd.Contains(":"))
                            {
                                this.holdlist.Add(new mulliitem(crd.Split(':')[0], enemyclass, Convert.ToInt32(crd.Split(':')[1])));
                            }
                            else
                            {
                                this.holdlist.Add(new mulliitem(crd, enemyclass, 2));
                            }
                        }
                    }
                    catch
                    {
                        Helpfunctions.Instance.logg("mullimaker cant read: " + line);
                    }
                }
                else
                {
                    if (line.StartsWith("discard;"))
                    {
                        try
                        {
                            string enemyclass = line.Split(';')[1];
                            string cardlist = line.Split(';')[2];
                            foreach (string crd in cardlist.Split(','))
                            {
                                this.deletelist.Add(new mulliitem(crd, enemyclass,2));
                            }
                        }
                        catch
                        {
                            Helpfunctions.Instance.logg("mullimaker cant read: " + line);
                        }
                    }
                    else
                    {

                    }
                }

            }

        }

        public bool hasmulliganrules()
        {
            if (this.holdlist.Count == 0 && this.deletelist.Count == 0) return false;
            return true;
        }

        public List<int> whatShouldIMulligan(List<CardIDEntity> cards, string enemclass)
        {
            List<int> discarditems = new List<int>();

            foreach (mulliitem mi in this.deletelist)
            {
                foreach (CardIDEntity c in cards)
                {
                    if (c.id == mi.cardid && (mi.enemyclass == "all" || mi.enemyclass == enemclass))
                    {
                        if(discarditems.Contains(c.entitiy)) continue;
                        discarditems.Add(c.entitiy);
                    }
                }
            }

            if (holdlist.Count == 0) return discarditems;

            Dictionary<string, int> holddic = new Dictionary<string, int>();
            foreach (CardIDEntity c in cards)
            {
                bool delete = true;
                foreach (mulliitem mi in this.holdlist)
                {
                    if (c.id == mi.cardid && (mi.enemyclass == "all" || mi.enemyclass == enemclass))
                    {
                        if (holddic.ContainsKey(c.id)) // we are holding one of the cards
                        {
                            if (mi.howmuch == 2)
                            {
                                delete = false;
                            }
                        }
                        else
                        {
                            delete = false;
                        }
                    }
                }

                if (delete)
                {
                    if (discarditems.Contains(c.entitiy)) continue;
                    discarditems.Add(c.entitiy);
                }
                else 
                {
                    if (holddic.ContainsKey(c.id))
                    {
                        holddic[c.id]++;
                    }
                    else
                    {
                        holddic.Add(c.id, 1);
                    }
                }
                
            }

            return discarditems;

        }
    
    }

}
