using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HREngine.Bots
{

    public class ComboBreaker
    {

        enum combotype
        {
            combo, 
            target,
            weaponuse
        }

        private List<combo> combos = new List<combo>();

         private static ComboBreaker instance;

         Handmanager hm = Handmanager.Instance;
         Hrtprozis hp = Hrtprozis.Instance;

         class combo 
         {
             public combotype type = combotype.combo;
             public int neededMana = -1;
             public Dictionary<string, int> combocards = new Dictionary<string, int>();
             public Dictionary<string, int> cardspen = new Dictionary<string, int>();
             public int penality = 0;
             public int combolength = 0;

             public combo(string s)
             {
                 int i = 0;
                 this.type = combotype.combo;
                foreach(string ding in s.Split(':'))
                {
                    /*if (i == 0)
                    {
                        if (ding == "c") this.type = combotype.combo;
                        if (ding == "t") this.type = combotype.target;
                        if (ding == "w") this.type = combotype.weaponuse;
                    }*/
                    if (ding == "" || ding == string.Empty) continue;

                    if (i == 1 && type==combotype.combo)
                    {
                        int m = Convert.ToInt32(ding);
                        neededMana = -1;
                        if (m >= 1) neededMana = m;
                    }

                    if (i == 0 && type == combotype.combo)
                    {
                        this.combolength = 0;
                        foreach (string crdl in ding.Split(';'))
                        {
                            if (crdl == "" || crdl == string.Empty) continue;
                            string crd = crdl.Split(',')[0];
                            this.combolength++;
                            if (combocards.ContainsKey(crd))
                            {
                                combocards[crd]++;
                                continue;
                            }
                            combocards.Add(crd,1);
                            cardspen.Add(crd, Convert.ToInt32(crdl.Split(',')[1]));

                        }
                    }

                    /*if (i == 2 && type == combotype.combo)
                    {
                        int m = Convert.ToInt32(ding);
                        penality = 0;
                        if (m >= 1) penality = m;
                    }*/


                        i++;
                }
             }

             public int isInCombo(List<Handmanager.Handcard> hand, int omm)
             {
                 int cardsincombo = 0;
                 Dictionary<string, int> combocardscopy = new Dictionary<string, int>(this.combocards);
                 foreach (Handmanager.Handcard hc in hand)
                 {
                     if (combocardscopy.ContainsKey(hc.card.CardID) && combocardscopy[hc.card.CardID] >=1)
                     {
                         cardsincombo++;
                         combocardscopy[hc.card.CardID]--;
                     }
                 }
                 if (cardsincombo == this.combolength && omm < this.neededMana) return 1;
                 if (cardsincombo == this.combolength) return 2;
                 if (cardsincombo >= 1) return 1;
                 return 0;
             }

             public bool isCardInCombo(CardDB.Card card)
             {
                     if (this.combocards.ContainsKey(card.CardID))
                     {
                         return true;
                     }
                 return false;
             }

         }

        public static ComboBreaker Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ComboBreaker();
                }
                return instance;
            }
        }

        private ComboBreaker()
        {
            readCombos();
        }

        private void readCombos()
        {
            string[] lines = new string[0] { };
            combos.Clear();
            try
            {
                string path = Settings.Instance.path;
                lines = System.IO.File.ReadAllLines(path + "_combo.txt");
            }
            catch
            {
                Helpfunctions.Instance.logg("cant find _combo.txt");
                return;
            }
            Helpfunctions.Instance.logg("read _combo.txt...");
            foreach (string line in lines)
            {
                combo c = new combo(line);
                this.combos.Add(c);
            }
            
        }

        public int getPenalityForDestroyingCombo(CardDB.Card crd)
        {
            int pen=int.MaxValue;
            bool found = false;
            foreach (combo c in this.combos)
                {
                    if (c.isCardInCombo(crd))
                    {
                        int iic = c.isInCombo(hm.handCards, hp.ownMaxMana);
                        if (iic == 1) found = true;
                        if (iic == 1 && pen > c.cardspen[crd.CardID]) pen = c.cardspen[crd.CardID];//iic==1 will destroy combo
                        if (iic == 2) pen = 0;
                    }
 
                }
            if (found) { return pen; }
            return 0;
            
        }



    }

}
