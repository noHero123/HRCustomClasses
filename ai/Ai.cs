using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{

    public class Ai
    {

        private int maxdeep = 12;
        private int maxwide = 7000;
        List<Playfield> posmoves = new List<Playfield>();

        Hrtprozis hp = Hrtprozis.Instance;
        Handmanager hm = Handmanager.Instance;
        Helpfunctions help = Helpfunctions.Instance;

        public Action bestmove = new Action();
        public int bestmoveValue = 0;

        private static Ai instance;

        public static Ai Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Ai();
                }
                return instance;
            }
        }

        private Ai()
        {
        }

        private bool doAllChoices(CardDB.Card card, Playfield p, Handmanager.Handcard hc)
        {
            bool havedonesomething = false;

            for (int i = 1; i < 3; i++)
            {
                CardDB.Card c = card;
                if (card.name == "sternenregen")
                {
                    if (i == 1)
                    {
                        c = CardDB.Instance.getCardDataFromID("NEW1_007b");
                    }
                    if (i == 2)
                    {
                        c = CardDB.Instance.getCardDataFromID("NEW1_007a");
                    }
                }

                if (card.name == "urtumderlehren")
                {
                    if (i == 1)
                    {
                        c = CardDB.Instance.getCardDataFromID("NEW1_008a");
                    }
                    if (i == 2)
                    {
                        c = CardDB.Instance.getCardDataFromID("NEW1_008b");
                    }
                }

                if (c.canplayCard(p))
                {
                    havedonesomething = true;



                    int bestplace = p.getBestPlace(c);
                    List<targett> trgts = c.getTargetsForCard(p);

                    if (trgts.Count == 0)
                    {
                        Playfield pf = new Playfield(p);

                        pf.playCard(card, hc.position - 1, hc.entity, -1, -1, i, bestplace);
                        this.posmoves.Add(pf);
                    }
                    else
                    {
                        foreach (targett trgt in trgts)
                        {
                            Playfield pf = new Playfield(p);
                            pf.playCard(card, hc.position - 1, hc.entity, trgt.target, trgt.targetEntity, i, bestplace);
                            this.posmoves.Add(pf);
                        }
                    }

                }

            }


            return havedonesomething;
        }



        private void doallmoves(bool test, Bot botBase)
        {

            bool havedonesomething = true;
            List<Playfield> temp = new List<Playfield>();
            int deep = 0;
            while (havedonesomething)
            {
                help.logg("ailoop");
                GC.Collect();
                temp.Clear();
                temp.AddRange(this.posmoves);
                havedonesomething = false;
                Playfield bestold = null;
                int bestoldval = -20000000;
                foreach (Playfield p in temp)
                {

                    if (p.complete)
                    {
                        continue;
                    }

                    //take a card and play it
                    List<string> playedcards = new List<string>();
                    foreach (Handmanager.Handcard hc in p.owncards)
                    {
                        CardDB.Card c = hc.card;
                        //help.logg("try play crd" + c.name + " " + c.getManaCost(p) + " " + c.canplayCard(p));
                        if (playedcards.Contains(c.name)) continue; // dont play the same card in one loop
                        playedcards.Add(c.name);
                        if (c.choice)
                        {
                            if (doAllChoices(c, p, hc))
                            {
                                havedonesomething = true;
                            }
                        }
                        else
                        {
                            int bestplace = p.getBestPlace(c);
                            if (c.canplayCard(p))
                            {
                                havedonesomething = true;
                                List<targett> trgts = c.getTargetsForCard(p);

                                if (trgts.Count == 0)
                                {
                                    Playfield pf = new Playfield(p);
                                    pf.playCard(c, hc.position - 1, hc.entity, -1, -1, 0,bestplace);
                                    this.posmoves.Add(pf);
                                }
                                else
                                {
                                    foreach (targett trgt in trgts)
                                    {
                                        Playfield pf = new Playfield(p);
                                        pf.playCard(c, hc.position - 1, hc.entity, trgt.target, trgt.targetEntity, 0,bestplace);
                                        this.posmoves.Add(pf);
                                    }

                                }


                            }
                        }
                    }

                    //attack with a minion
                    foreach (Minion m in p.ownMinions)
                    {

                        if (m.Ready && m.Angr >= 1 && !m.frozen)
                        {
                            List<targett> trgts = p.getAttackTargets();
                            havedonesomething = true;
                            foreach (targett trgt in trgts)
                            {
                                Playfield pf = new Playfield(p);
                                pf.attackWithMinion(m, trgt.target, trgt.targetEntity);
                                this.posmoves.Add(pf);
                            }

                        }

                    }

                    // attack with hero
                    if (p.ownHeroReady)
                    {
                        List<targett> trgts = p.getAttackTargets();
                        havedonesomething = true;
                        foreach (targett trgt in trgts)
                        {
                            Playfield pf = new Playfield(p);
                            pf.attackWithWeapon(trgt.target, trgt.targetEntity);
                            this.posmoves.Add(pf);
                        }
                    }

                    // use ability
                    /// TODO check if ready after manaup
                    if (p.ownAbilityReady && p.mana >= 2)
                    {
                        havedonesomething = true;
                        if (this.hp.heroname == "mage" || this.hp.heroname == "priest")
                        {

                            List<targett> trgts = p.ownHeroAblility.getTargetsForCard(p);
                            foreach (targett trgt in trgts)
                            {
                                //if (this.hp.heroname == "priest" && trgt == 200) continue;
                                havedonesomething = true;
                                Playfield pf = new Playfield(p);
                                pf.activateAbility(p.ownHeroAblility, trgt.target, trgt.targetEntity);
                                this.posmoves.Add(pf);
                            }
                        }
                        else
                        {
                            havedonesomething = true;
                            Playfield pf = new Playfield(p);
                            pf.activateAbility(p.ownHeroAblility, -1, -1);
                            this.posmoves.Add(pf);
                        }

                    }


                    p.endTurn();

                    //sort stupid stuff ouf

                    if (botBase.getPlayfieldValue(p) > bestoldval)
                    {
                        bestoldval = botBase.getPlayfieldValue(p);
                        bestold = p;
                    }
                    if (!test)
                    {
                        posmoves.Remove(p);
                    }

                }

                if (!test && bestoldval >= -10000 && bestold != null)
                {
                    this.posmoves.Add(bestold);
                }

                help.loggonoff(true);
                int donec = 0;
                foreach (Playfield p in posmoves)
                {
                    if (p.complete) donec++;
                }
                help.logg("deep " + deep + " len " + this.posmoves.Count + " dones " + donec);

                if (!test)
                {
                    cuttingposibilities(botBase);
                }
                help.logg("cut to len " + this.posmoves.Count);
                /*if ((deep + 1) % 4 == 0)
                {
                    help.logg("cut");
                }*/
                help.loggonoff(false);
                deep++;

                if (deep >= this.maxdeep) break;//remove this?
            }

            int bestval = int.MinValue;
            int bestanzactions = 1000;
            Playfield bestplay = temp[0];
            foreach (Playfield p in temp)
            {
                int val = botBase.getPlayfieldValue(p);
                if (bestval <= val)
                {
                    if (bestval == val && bestanzactions < p.playactions.Count) continue;
                    bestplay = p;
                    bestval = val;
                    bestanzactions = p.playactions.Count;
                }

            }
            help.loggonoff(true);
            help.logg("-------------------------------------");
            help.logg("bestPlayvalue " + bestval);

            bestplay.printActions();
            this.bestmove = bestplay.getNextAction();
            this.bestmoveValue = bestval;
            if (bestmove.cardplay && bestmove.card.type == CardDB.cardtype.MOB)
            {
                Playfield pf = new Playfield();
                help.logg("bestplaces:");
                pf.getBestPlacePrint(bestmove.card);
            }

        }


        private void cuttingposibilities(Bot botBase)
        {
            // take the x best values
            int takenumber = this.maxwide;
            List<Playfield> temp = new List<Playfield>();
            posmoves.Sort((a, b) => -(botBase.getPlayfieldValue(a)).CompareTo(botBase.getPlayfieldValue(b)));//want to keep the best
            temp.AddRange(posmoves);
            posmoves.Clear();
            posmoves.AddRange(Helpfunctions.TakeList(temp, takenumber));

        }



        public void dosomethingclever(Bot botbase)
        {
            //return;
            //turncheck
            //help.moveMouse(950,750);
            //help.Screenshot();
            posmoves.Clear();
            hp.updatePositions();
            posmoves.Add(new Playfield());

           /* foreach (var item in this.posmoves[0].owncards)
            {
                help.logg("card " + item.card.name + " is playable :" + item.card.canplayCard(posmoves[0]) + " cost/mana: " + item.card.cost + "/" + posmoves[0].mana);
            }
            */
            //help.logg("is hero ready?" + posmoves[0].ownHeroReady);

            help.loggonoff(false);
            doallmoves(false, botbase);
            //help.logging(true);

        }

        public void doBenchmark(Bot botbase)
        {
            help.logg("do benchmark, dont cry");
            //setup cards in hand
            this.hm.loadPreparedBattlefield(10);


            this.hp.loadPreparedHeros(0);//setup hero hp, weapons and stuff
            //setup minions on field
            this.hp.loadPreparedBattlefield(10);

            //calculate the stuff
            posmoves.Clear();
            posmoves.Add(new Playfield());
            foreach (Playfield p in this.posmoves)
            {
                p.printBoard();
            }
            help.logg("ownminionscount " + posmoves[0].ownMinions.Count);
            help.logg("owncardscount " + posmoves[0].owncards.Count);

            foreach (var item in this.posmoves[0].owncards)
            {
                help.logg("card " + item.card.name + " is playable :" + item.card.canplayCard(posmoves[0]) + " cost/mana: " + item.card.cost + "/" + posmoves[0].mana);
            }

            doallmoves(true, botbase);
        }

        public void simulatorTester(Bot botbase)
        {
            help.logg("simulating board ");
            //setup cards in hand
            this.hm.loadPreparedBattlefield(5);


            this.hp.loadPreparedHeros(0);//setup hero hp, weapons and stuff
            //setup minions on field
            this.hp.loadPreparedBattlefield(5);

            //calculate the stuff
            posmoves.Clear();
            posmoves.Add(new Playfield());
            foreach (Playfield p in this.posmoves)
            {
                p.printBoard();
            }
            help.logg("ownminionscount " + posmoves[0].ownMinions.Count);
            help.logg("owncardscount " + posmoves[0].owncards.Count);

            foreach (var item in this.posmoves[0].owncards)
            {
                help.logg("card " + item.card.name + " is playable :" + item.card.canplayCard(posmoves[0]) +" cost/mana: " + item.card.cost +"/"+ posmoves[0].mana);
            }

            doallmoves(true, botbase);
            foreach (Playfield p in this.posmoves)
            {
                p.printBoard();
            }
        }

        public void autoTester(Bot botbase)
        {
            help.logg("simulating board ");

            BoardTester bt = new BoardTester();

            //calculate the stuff
            posmoves.Clear();
            posmoves.Add(new Playfield());
            foreach (Playfield p in this.posmoves)
            {
                p.printBoard();
            }
            help.logg("ownminionscount " + posmoves[0].ownMinions.Count);
            help.logg("owncardscount " + posmoves[0].owncards.Count);

            foreach (var item in this.posmoves[0].owncards)
            {
                help.logg("card " + item.card.name + " is playable :" + item.card.canplayCard(posmoves[0]) + " cost/mana: " + item.card.cost + "/" + posmoves[0].mana);
            }

            doallmoves(true, botbase);
            foreach (Playfield p in this.posmoves)
            {
                p.printBoard();
            }
        }

    }


}
