using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HREngine.Bots
{
    public class MiniSimulator
    {
        //#####################################################################################################################
        private int maxdeep = 6;
        private int maxwide = 10;
        private int totalboards = 50;
        private bool usePenalityManager = true;
        private bool useCutingTargets = true;
        private bool dontRecalc = true;
        private bool useLethalCheck = true;
        private bool useComparison = true;

        private bool printNormalstuff = false;

        List<Playfield> posmoves = new List<Playfield>(7000);

        public Action bestmove = new Action();
        public int bestmoveValue = 0;
        public Playfield bestboard = new Playfield();

        public Bot botBase = null;
        private int calculated = 0;

        private bool simulateSecondTurn = false;

        PenalityManager pen = PenalityManager.Instance;

        public MiniSimulator()
        {
        }
        public MiniSimulator(int deep, int wide, int ttlboards)
        {
            this.maxdeep = deep;
            this.maxwide = wide;
            this.totalboards = ttlboards;
        }

        public void updateParams(int deep, int wide, int ttlboards)
        {
            this.maxdeep = deep;
            this.maxwide = wide;
            this.totalboards = ttlboards;
        }

        public void setPrintingstuff(bool sp)
        {
            this.printNormalstuff = sp;
        }

        public void setSecondTurnSimu(bool sts)
        {
            this.simulateSecondTurn = sts;
        }

        private void addToPosmoves(Playfield pf)
        {
            if (pf.ownHeroHp <= 0) return;
            /*foreach (Playfield p in this.posmoves)
            {
                if (pf.isEqual(p, false)) return;
            }*/
            this.posmoves.Add(pf);
            //posmoves.Sort((a, b) => -(botBase.getPlayfieldValue(a)).CompareTo(botBase.getPlayfieldValue(b)));//want to keep the best
            //if (posmoves.Count > this.maxwide) posmoves.RemoveAt(this.maxwide);
            if (this.totalboards >= 1)
            {
                this.calculated++;
            }
        }

        private bool doAllChoices(Playfield p, Handmanager.Handcard hc, bool lethalcheck)
        {
            bool havedonesomething = false;

            for (int i = 1; i < 3; i++)
            {
                CardDB.Card c = hc.card;
                if (c.name == "keeperofthegrove")
                {
                    if (i == 1)
                    {
                        c = CardDB.Instance.getCardDataFromID("EX1_166a");
                    }
                    if (i == 2)
                    {
                        c = CardDB.Instance.getCardDataFromID("EX1_166b");
                    }
                }

                if (c.name == "starfall")
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

                if (c.name == "ancientoflore")
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

                if (c.name == "powerofthewild")
                {
                    if (i == 1)
                    {
                        c = CardDB.Instance.getCardDataFromID("EX1_160b");
                    }
                    if (i == 2)
                    {
                        c = CardDB.Instance.getCardDataFromID("EX1_160a");
                    }
                }
                if (c.name == "ancientofwar")
                {
                    if (i == 1)
                    {
                        c = CardDB.Instance.getCardDataFromID("EX1_178a");
                    }
                    if (i == 2)
                    {
                        c = CardDB.Instance.getCardDataFromID("EX1_178b");
                    }
                }
                if (c.name == "druidoftheclaw")
                {
                    if (i == 1)
                    {
                        c = CardDB.Instance.getCardDataFromID("EX1_165t1");
                    }
                    if (i == 2)
                    {
                        c = CardDB.Instance.getCardDataFromID("EX1_165t2");
                    }
                }
                //cenarius dont need
                if (c.name == "keeperofthegrove")//keeper of the grove
                {
                    if (i == 1)
                    {
                        c = CardDB.Instance.getCardDataFromID("EX1_166a");
                    }
                    if (i == 2)
                    {
                        c = CardDB.Instance.getCardDataFromID("EX1_166b");
                    }
                }
                if (c.name == "markofnature")
                {
                    if (i == 1)
                    {
                        c = CardDB.Instance.getCardDataFromID("EX1_155a");
                    }
                    if (i == 2)
                    {
                        c = CardDB.Instance.getCardDataFromID("EX1_155b");
                    }
                }
                if (c.name == "nourish")
                {
                    if (i == 1)
                    {
                        c = CardDB.Instance.getCardDataFromID("EX1_164a");
                    }
                    if (i == 2)
                    {
                        c = CardDB.Instance.getCardDataFromID("EX1_164b");
                    }
                }
                if (c.name == "wrath")
                {
                    if (i == 1)
                    {
                        c = CardDB.Instance.getCardDataFromID("EX1_154a");
                    }
                    if (i == 2)
                    {
                        c = CardDB.Instance.getCardDataFromID("EX1_154b");
                    }
                }

                if (c.canplayCard(p, hc.manacost))
                {
                    havedonesomething = true;



                    int bestplace = p.getBestPlace(c, lethalcheck);
                    List<targett> trgts = c.getTargetsForCard(p);
                    int cardplayPenality = 0;
                    if (trgts.Count == 0)
                    {


                        if (usePenalityManager)
                        {
                            cardplayPenality = pen.getPlayCardPenality(hc.card, -1, p, i, lethalcheck);
                            if (cardplayPenality <= 499)
                            {
                                //help.logg(hc.card.name + " is played");
                                Playfield pf = new Playfield(p);
                                pf.playCard(hc, hc.position - 1, hc.entity, -1, -1, i, bestplace, cardplayPenality);
                                addToPosmoves(pf);
                            }
                        }
                        else
                        {
                            Playfield pf = new Playfield(p);
                            pf.playCard(hc, hc.position - 1, hc.entity, -1, -1, i, bestplace, cardplayPenality);
                            addToPosmoves(pf);
                        }

                    }
                    else
                    {
                        foreach (targett trgt in trgts)
                        {

                            if (usePenalityManager)
                            {
                                cardplayPenality = pen.getPlayCardPenality(hc.card, trgt.target, p, 0, lethalcheck);
                                if (cardplayPenality <= 499)
                                {
                                    //help.logg(hc.card.name + " is played");
                                    Playfield pf = new Playfield(p);
                                    pf.playCard(hc, hc.position - 1, hc.entity, trgt.target, trgt.targetEntity, i, bestplace, cardplayPenality);
                                    addToPosmoves(pf);
                                }
                            }
                            else
                            {
                                Playfield pf = new Playfield(p);
                                pf.playCard(hc, hc.position - 1, hc.entity, trgt.target, trgt.targetEntity, i, bestplace, cardplayPenality);
                                addToPosmoves(pf);
                            }

                        }
                    }

                }

            }


            return havedonesomething;
        }

        public int doallmoves(Playfield playf, bool isLethalCheck)
        {
            //Helpfunctions.Instance.logg("NXTTRN" + playf.mana);
            if (botBase == null) botBase = Ai.Instance.botBase;
            bool test = false;
            this.posmoves.Clear();
            this.addToPosmoves(playf);
            bool havedonesomething = true;
            List<Playfield> temp = new List<Playfield>();
            int deep = 0;
            //Helpfunctions.Instance.logg("NXTTRN" + playf.mana + " " + posmoves.Count);
            this.calculated = 0;
            while (havedonesomething)
            {
                if (this.printNormalstuff) Helpfunctions.Instance.logg("ailoop");
                GC.Collect();
                temp.Clear();
                temp.AddRange(this.posmoves);
                havedonesomething = false;
                Playfield bestold = null;
                int bestoldval = -20000000;
                foreach (Playfield p in temp)
                {

                    if (p.complete || p.ownHeroHp <= 0)
                    {
                        continue;
                    }

                    //take a card and play it
                    List<string> playedcards = new List<string>();

                    foreach (Handmanager.Handcard hc in p.owncards)
                    {
                        if (this.calculated > this.totalboards) continue;
                        CardDB.Card c = hc.card;
                        //help.logg("try play crd" + c.name + " " + c.getManaCost(p) + " " + c.canplayCard(p));
                        if (playedcards.Contains(c.name)) continue; // dont play the same card in one loop
                        playedcards.Add(c.name);
                        if (c.choice)
                        {
                            if (doAllChoices(p, hc, isLethalCheck))
                            {
                                havedonesomething = true;
                            }
                        }
                        else
                        {
                            int bestplace = p.getBestPlace(c, isLethalCheck);
                            if (hc.canplayCard(p))
                            {
                                havedonesomething = true;
                                List<targett> trgts = c.getTargetsForCard(p);

                                if (isLethalCheck && (pen.DamageTargetDatabase.ContainsKey(c.name) || pen.DamageTargetSpecialDatabase.ContainsKey(c.name)))// only target enemy hero during Lethal check!
                                {
                                    targett trg = trgts.Find(x => x.target == 200);
                                    if (trg != null)
                                    {
                                        trgts.Clear();
                                        trgts.Add(trg);
                                    }
                                    else
                                    {
                                        // no enemy hero -> enemy have taunts ->kill the taunts from left to right
                                        if (trgts.Count >= 1)
                                        {
                                            trg = trgts[0];
                                            trgts.Clear();
                                            trgts.Add(trg);
                                        }
                                    }
                                }


                                int cardplayPenality = 0;

                                if (trgts.Count == 0)
                                {


                                    if (usePenalityManager)
                                    {
                                        cardplayPenality = pen.getPlayCardPenality(c, -1, p, 0, isLethalCheck);
                                        if (cardplayPenality <= 499)
                                        {
                                            Playfield pf = new Playfield(p);
                                            havedonesomething = true;
                                            pf.playCard(hc, hc.position - 1, hc.entity, -1, -1, 0, bestplace, cardplayPenality);
                                            addToPosmoves(pf);
                                        }
                                    }
                                    else
                                    {
                                        Playfield pf = new Playfield(p);
                                        havedonesomething = true;
                                        pf.playCard(hc, hc.position - 1, hc.entity, -1, -1, 0, bestplace, cardplayPenality);
                                        addToPosmoves(pf);
                                    }


                                }
                                else
                                {
                                    if (isLethalCheck)// only target enemy hero during Lethal check!
                                    {
                                        targett trg = trgts.Find(x => x.target == 200);
                                        if (trg != null)
                                        {
                                            trgts.Clear();
                                            trgts.Add(trg);
                                        }
                                    }

                                    foreach (targett trgt in trgts)
                                    {


                                        if (usePenalityManager)
                                        {
                                            cardplayPenality = pen.getPlayCardPenality(c, trgt.target, p, 0, isLethalCheck);
                                            if (cardplayPenality <= 499)
                                            {
                                                Playfield pf = new Playfield(p);
                                                havedonesomething = true;
                                                pf.playCard(hc, hc.position - 1, hc.entity, trgt.target, trgt.targetEntity, 0, bestplace, cardplayPenality);
                                                addToPosmoves(pf);
                                            }
                                        }
                                        else
                                        {
                                            Playfield pf = new Playfield(p);
                                            havedonesomething = true;
                                            pf.playCard(hc, hc.position - 1, hc.entity, trgt.target, trgt.targetEntity, 0, bestplace, cardplayPenality);
                                            addToPosmoves(pf);
                                        }

                                    }

                                }


                            }
                        }
                    }

                    //attack with a minion

                    List<Minion> playedMinions = new List<Minion>(8);

                    foreach (Minion m in p.ownMinions)
                    {
                        if (this.calculated > this.totalboards) continue;

                        if (m.Ready && m.Angr >= 1 && !m.frozen)
                        {
                            //BEGIN:cut (double/similar) attacking minions out#####################################
                            // DONT LET SIMMILAR MINIONS ATTACK IN ONE TURN (example 3 unlesh the hounds-hounds doesnt need to simulated hole)
                            List<Minion> tempoo = new List<Minion>(playedMinions);
                            bool dontattacked = true;
                            bool isSpecial = pen.specialMinions.ContainsKey(m.name);
                            foreach (Minion mnn in tempoo)
                            {
                                // special minions are allowed to attack in silended and unsilenced state!
                                //help.logg(mnn.silenced + " " + m.silenced + " " + mnn.name + " " + m.name + " " + penman.specialMinions.ContainsKey(m.name));

                                bool otherisSpecial = pen.specialMinions.ContainsKey(mnn.name);

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
                            List<targett> trgts = p.getAttackTargets(true);


                            if (isLethalCheck)// only target enemy hero during Lethal check!
                            {
                                targett trg = trgts.Find(x => x.target == 200);
                                if (trg != null)
                                {
                                    trgts.Clear();
                                    trgts.Add(trg);
                                }
                                else
                                {
                                    // no enemy hero -> enemy have taunts ->kill the taunts from left to right
                                    if (trgts.Count >= 1)
                                    {
                                        trg = trgts[0];
                                        trgts.Clear();
                                        trgts.Add(trg);
                                    }
                                }
                            }
                            else
                            {
                                if (this.useCutingTargets) trgts = this.cutAttackTargets(trgts, p, true);
                            }

                            foreach (targett trgt in trgts)
                            {


                                int attackPenality = 0;

                                if (usePenalityManager)
                                {
                                    attackPenality = pen.getAttackWithMininonPenality(m, p, trgt.target, isLethalCheck);
                                    if (attackPenality <= 499)
                                    {
                                        Playfield pf = new Playfield(p);
                                        havedonesomething = true;
                                        pf.attackWithMinion(m, trgt.target, trgt.targetEntity, attackPenality);
                                        addToPosmoves(pf);
                                    }
                                }
                                else
                                {
                                    Playfield pf = new Playfield(p);
                                    havedonesomething = true;
                                    pf.attackWithMinion(m, trgt.target, trgt.targetEntity, attackPenality);
                                    addToPosmoves(pf);
                                }


                            }
                            if (trgts.Count == 1 && trgts[0].target == 200)//only enemy hero is available als attack
                            {
                                break;
                            }
                        }

                    }

                    // attack with hero
                    if (p.ownHeroReady)
                    {
                        if (this.calculated > this.totalboards) continue;
                        List<targett> trgts = p.getAttackTargets(true);

                        havedonesomething = true;

                        if (isLethalCheck)// only target enemy hero during Lethal check!
                        {
                            targett trg = trgts.Find(x => x.target == 200);
                            if (trg != null)
                            {
                                trgts.Clear();
                                trgts.Add(trg);
                            }
                            else
                            {
                                // no enemy hero -> enemy have taunts ->kill the taunts from left to right
                                if (trgts.Count >= 1)
                                {
                                    trg = trgts[0];
                                    trgts.Clear();
                                    trgts.Add(trg);
                                }
                            }
                        }
                        else
                        {
                            if (this.useCutingTargets) trgts = this.cutAttackTargets(trgts, p, true);
                        }

                        foreach (targett trgt in trgts)
                        {
                            Playfield pf = new Playfield(p);
                            int heroAttackPen = 0;
                            if (usePenalityManager)
                            {
                                heroAttackPen = pen.getAttackWithHeroPenality(trgt.target, p);
                            }
                            pf.attackWithWeapon(trgt.target, trgt.targetEntity, heroAttackPen);
                            addToPosmoves(pf);
                        }
                    }

                    // use ability
                    /// TODO check if ready after manaup
                    if (p.ownAbilityReady && p.mana >= 2 && p.ownHeroAblility.canplayCard(p, 2))
                    {
                        if (this.calculated > this.totalboards) continue;
                        int abilityPenality = 0;

                        havedonesomething = true;
                        // if we have mage or priest, we have to target something####################################################
                        if (p.ownHeroName == HeroEnum.mage || p.ownHeroName == HeroEnum.priest)
                        {

                            List<targett> trgts = p.ownHeroAblility.getTargetsForCard(p);

                            if (isLethalCheck && (p.ownHeroName == HeroEnum.mage || (p.ownHeroName == HeroEnum.priest && p.ownHeroAblility.name != "lesserheal")))// only target enemy hero during Lethal check!
                            {
                                targett trg = trgts.Find(x => x.target == 200);
                                if (trg != null)
                                {
                                    trgts.Clear();
                                    trgts.Add(trg);
                                }
                                else
                                {
                                    // no enemy hero -> enemy have taunts ->kill the taunts from left to right
                                    if (trgts.Count >= 1)
                                    {
                                        trg = trgts[0];
                                        trgts.Clear();
                                        trgts.Add(trg);
                                    }
                                }
                            }

                            foreach (targett trgt in trgts)
                            {



                                if (usePenalityManager)
                                {
                                    abilityPenality = pen.getPlayCardPenality(p.ownHeroAblility, trgt.target, p, 0, isLethalCheck);
                                    if (abilityPenality <= 499)
                                    {
                                        Playfield pf = new Playfield(p);
                                        havedonesomething = true;
                                        pf.activateAbility(p.ownHeroAblility, trgt.target, trgt.targetEntity, abilityPenality);
                                        addToPosmoves(pf);
                                    }
                                }
                                else
                                {
                                    Playfield pf = new Playfield(p);
                                    havedonesomething = true;
                                    pf.activateAbility(p.ownHeroAblility, trgt.target, trgt.targetEntity, abilityPenality);
                                    addToPosmoves(pf);
                                }

                            }
                        }
                        else
                        {
                            // the other classes dont have to target####################################################
                            Playfield pf = new Playfield(p);

                            if (usePenalityManager)
                            {
                                abilityPenality = pen.getPlayCardPenality(p.ownHeroAblility, -1, pf, 0, isLethalCheck);
                                if (abilityPenality <= 499)
                                {
                                    havedonesomething = true;
                                    pf.activateAbility(p.ownHeroAblility, -1, -1, abilityPenality);
                                    addToPosmoves(pf);
                                }
                            }
                            else
                            {
                                havedonesomething = true;
                                pf.activateAbility(p.ownHeroAblility, -1, -1, abilityPenality);
                                addToPosmoves(pf);
                            }

                        }

                    }


                    if (isLethalCheck)
                    {
                        p.complete = true;
                    }
                    else
                    {
                        p.endTurn(this.simulateSecondTurn);
                    }

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

                    if (this.calculated > this.totalboards) break;
                }

                if (!test && bestoldval >= -10000 && bestold != null)
                {
                    this.posmoves.Add(bestold);
                }

                //Helpfunctions.Instance.loggonoff(true);
                if (this.printNormalstuff)
                {
                    int donec = 0;
                    foreach (Playfield p in posmoves)
                    {
                        if (p.complete) donec++;
                    }
                    Helpfunctions.Instance.logg("deep " + deep + " len " + this.posmoves.Count + " dones " + donec);
                }

                if (!test)
                {
                    cuttingposibilities();
                }

                if (this.printNormalstuff)
                {
                    Helpfunctions.Instance.logg("cut to len " + this.posmoves.Count);
                }
                //Helpfunctions.Instance.loggonoff(false);
                deep++;

                if (this.calculated > this.totalboards) break;
                if (deep >= this.maxdeep) break;//remove this?
            }

            foreach (Playfield p in posmoves)//temp
            {
                if (!p.complete)
                {
                    if (isLethalCheck)
                    {
                        p.complete = true;
                    }
                    else
                    {
                        p.endTurn(this.simulateSecondTurn);
                    }
                }
            }
            // Helpfunctions.Instance.logg("find best ");
            if (posmoves.Count >= 1)
            {
                int bestval = int.MinValue;
                int bestanzactions = 1000;
                Playfield bestplay = posmoves[0];//temp[0]
                foreach (Playfield p in posmoves)//temp
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

                this.bestmove = bestplay.getNextAction();
                this.bestmoveValue = bestval;
                this.bestboard = new Playfield(bestplay);
                //Helpfunctions.Instance.logg("return");
                return bestval;
            }
            //Helpfunctions.Instance.logg("return");
            this.bestmove = null;
            this.bestmoveValue = -100000;
            this.bestboard = playf;
            return -10000;
        }

        public void cuttingposibilities()
        {
            // take the x best values
            int takenumber = this.maxwide;
            List<Playfield> temp = new List<Playfield>();
            posmoves.Sort((a, b) => -(botBase.getPlayfieldValue(a)).CompareTo(botBase.getPlayfieldValue(b)));//want to keep the best

            if (this.useComparison)
            {
                int i = 0;
                foreach (Playfield p in posmoves)
                {
                    bool found = false;
                    foreach (Playfield pp in temp)
                    {
                        if (pp.isEqual(p, false))
                        {
                            found = true;
                            break;
                        }
                    }
                    if (!found) temp.Add(p);
                    i++;
                    if (i >= this.maxwide) break;

                }
            }
            else
            {
                temp.AddRange(posmoves);
            }

            posmoves.Clear();
            posmoves.AddRange(temp.GetRange(0, Math.Min(takenumber, temp.Count)));
            //posmoves.Clear();
            //posmoves.AddRange(Helpfunctions.TakeList(temp, takenumber));

        }

        public List<targett> cutAttackTargets(List<targett> oldlist, Playfield p, bool own)
        {
            List<targett> retvalues = new List<targett>();
            List<Minion> addedmins = new List<Minion>(8);

            bool priomins = false;
            List<targett> retvaluesPrio = new List<targett>();
            foreach (targett t in oldlist)
            {
                if ((own && t.target == 200) || (!own && t.target == 100))
                {
                    retvalues.Add(t);
                    continue;
                }
                if ((own && t.target >= 10 && t.target <= 19) || (!own && t.target >= 0 && t.target <= 9))
                {
                    Minion m = null;
                    if (own) m = p.enemyMinions[t.target - 10];
                    if (!own) m = p.ownMinions[t.target];
                    /*if (penman.priorityDatabase.ContainsKey(m.name))
                    {
                        //retvalues.Add(t);
                        retvaluesPrio.Add(t);
                        priomins = true;
                        //help.logg(m.name + " is added to targetlist");
                        continue;
                    }*/


                    bool goingtoadd = true;
                    List<Minion> temp = new List<Minion>(addedmins);
                    bool isSpecial = pen.specialMinions.ContainsKey(m.name);
                    foreach (Minion mnn in temp)
                    {
                        // special minions are allowed to attack in silended and unsilenced state!
                        //help.logg(mnn.silenced + " " + m.silenced + " " + mnn.name + " " + m.name + " " + penman.specialMinions.ContainsKey(m.name));

                        bool otherisSpecial = pen.specialMinions.ContainsKey(mnn.name);

                        if ((!isSpecial || (isSpecial && m.silenced)) && (!otherisSpecial || (otherisSpecial && mnn.silenced))) // both are not special, if they are the same, dont add
                        {
                            if (mnn.Angr == m.Angr && mnn.Hp == m.Hp && mnn.divineshild == m.divineshild && mnn.taunt == m.taunt && mnn.poisonous == m.poisonous) goingtoadd = false;
                            continue;
                        }

                        if (isSpecial == otherisSpecial && !m.silenced && !mnn.silenced) // same are special
                        {
                            if (m.name != mnn.name) // different name -> take it
                            {
                                continue;
                            }
                            // same name -> test whether they are equal
                            if (mnn.Angr == m.Angr && mnn.Hp == m.Hp && mnn.divineshild == m.divineshild && mnn.taunt == m.taunt && mnn.poisonous == m.poisonous) goingtoadd = false;
                            continue;
                        }

                    }

                    if (goingtoadd)
                    {
                        addedmins.Add(m);
                        retvalues.Add(t);
                        //help.logg(m.name + " " + m.id +" is added to targetlist");
                    }
                    else
                    {
                        //help.logg(m.name + " is not needed to attack");
                        continue;
                    }

                }
            }
            //help.logg("end targetcutting");
            if (priomins) return retvaluesPrio;

            return retvalues;
        }

        public void printPosmoves()
        {
            foreach (Playfield p in this.posmoves)
            {
                p.printBoard();
            }
        }

    }

}
