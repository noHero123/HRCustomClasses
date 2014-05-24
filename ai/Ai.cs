﻿using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{


    public class Ai
    {
        private int maxdeep = 12;
        private int maxwide = 7000;
        private bool usePenalityManager = true;
        private bool useCutingTargets = true;
        private bool dontRecalc = true;
        private bool useLethalCheck = true;


        PenalityManager penman = PenalityManager.Instance;

        List<Playfield> posmoves = new List<Playfield>(7000);

        Hrtprozis hp = Hrtprozis.Instance;
        Handmanager hm = Handmanager.Instance;
        Helpfunctions help = Helpfunctions.Instance;

        public Action bestmove = new Action();
        public int bestmoveValue = 0;
        Playfield bestboard = new Playfield();
        Playfield nextMoveGuess = new Playfield();

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
            this.nextMoveGuess = new Playfield();
            this.nextMoveGuess.mana = -1;
        }

        private bool doAllChoices( Playfield p, Handmanager.Handcard hc, bool lethalcheck)
        {
            bool havedonesomething = false;

            for (int i = 1; i < 3; i++)
            {
                CardDB.Card c = hc.card;
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

                if (hc.canplayCard(p))
                {
                    havedonesomething = true;



                    int bestplace = p.getBestPlace(c);
                    List<targett> trgts = c.getTargetsForCard(p);
                    int cardplayPenality = 0;
                    if (trgts.Count == 0)
                    {
                        

                        if (usePenalityManager)
                        {
                            cardplayPenality = penman.getPlayCardPenality(c, -1, p, i, lethalcheck);
                            if (cardplayPenality <= 499)
                            {
                                Playfield pf = new Playfield(p);
                                pf.playCard(hc, hc.position - 1, hc.entity, -1, -1, i, bestplace, cardplayPenality);
                                this.posmoves.Add(pf);
                            }
                        }
                        else
                        {
                            Playfield pf = new Playfield(p);
                            pf.playCard(hc, hc.position - 1, hc.entity, -1, -1, i, bestplace, cardplayPenality);
                            this.posmoves.Add(pf);
                        }

                    }
                    else
                    {
                        foreach (targett trgt in trgts)
                        {
                            
                            if (usePenalityManager)
                            {
                                cardplayPenality = penman.getPlayCardPenality(c, -1, p, i, lethalcheck);
                                if (cardplayPenality <= 499)
                                {
                                    Playfield pf = new Playfield(p);
                                    pf.playCard(hc, hc.position - 1, hc.entity, trgt.target, trgt.targetEntity, i, bestplace, cardplayPenality);
                                    this.posmoves.Add(pf);
                                }
                            }
                            else
                            {
                                Playfield pf = new Playfield(p);
                                pf.playCard(hc, hc.position - 1, hc.entity, trgt.target, trgt.targetEntity, i, bestplace, cardplayPenality);
                                this.posmoves.Add(pf);
                            }

                        }
                    }

                }

            }


            return havedonesomething;
        }


        private void doallmoves(bool test, Bot botBase, bool isLethalCheck)
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
                            if (doAllChoices(p, hc, isLethalCheck))
                            {
                                havedonesomething = true;
                            }
                        }
                        else
                        {
                            int bestplace = p.getBestPlace(c);
                            if (hc.canplayCard(p))
                            {
                                havedonesomething = true;
                                List<targett> trgts = c.getTargetsForCard(p);

                                if (isLethalCheck && (penman.DamageTargetDatabase.ContainsKey(c.name) || penman.DamageTargetSpecialDatabase.ContainsKey(c.name)))// only target enemy hero during Lethal check!
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
                                        cardplayPenality = penman.getPlayCardPenality(c, -1, p, 0, isLethalCheck);
                                        if (cardplayPenality <= 499)
                                        {
                                            Playfield pf = new Playfield(p);
                                            havedonesomething = true;
                                            pf.playCard(hc, hc.position - 1, hc.entity, -1, -1, 0, bestplace, cardplayPenality);
                                            this.posmoves.Add(pf);
                                        }
                                    }
                                    else
                                    {
                                        Playfield pf = new Playfield(p);
                                        havedonesomething = true;
                                        pf.playCard(hc, hc.position - 1, hc.entity, -1, -1, 0, bestplace, cardplayPenality);
                                        this.posmoves.Add(pf);
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
                                            cardplayPenality = penman.getPlayCardPenality(c, trgt.target, p, 0, isLethalCheck);
                                            if (cardplayPenality <= 499)
                                            {
                                                Playfield pf = new Playfield(p);
                                                havedonesomething = true;
                                                pf.playCard(hc, hc.position - 1, hc.entity, trgt.target, trgt.targetEntity, 0, bestplace, cardplayPenality);
                                                this.posmoves.Add(pf);
                                            }
                                        }
                                        else
                                        {
                                            Playfield pf = new Playfield(p);
                                            havedonesomething = true;
                                            pf.playCard(hc, hc.position - 1, hc.entity, trgt.target, trgt.targetEntity, 0, bestplace, cardplayPenality);
                                            this.posmoves.Add(pf);
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

                        if (m.Ready && m.Angr >= 1 && !m.frozen)
                        {
                            //BEGIN:cut (double/similar) attacking minions out#####################################
                            // DONT LET SIMMILAR MINIONS ATTACK IN ONE TURN (example 3 unlesh the hounds-hounds doesnt need to simulated hole)
                            List<Minion> tempoo = new List<Minion>(playedMinions);
                            bool dontattacked = true;
                            bool isSpecial = penman.specialMinions.ContainsKey(m.name);
                            foreach (Minion mnn in tempoo)
                            {
                                // special minions are allowed to attack in silended and unsilenced state!
                                //help.logg(mnn.silenced + " " + m.silenced + " " + mnn.name + " " + m.name + " " + penman.specialMinions.ContainsKey(m.name));

                                bool otherisSpecial = penman.specialMinions.ContainsKey(mnn.name);

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
                            List<targett> trgts = p.getAttackTargets();


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
                                if (this.useCutingTargets) trgts = this.cutAttackTargets(trgts, p);
                            }

                            foreach (targett trgt in trgts)
                            {
                                

                                int attackPenality = 0;

                                if (usePenalityManager)
                                {
                                    attackPenality = penman.getAttackWithMininonPenality(m, p, trgt.target, isLethalCheck);
                                    if (attackPenality <= 499)
                                    {
                                        Playfield pf = new Playfield(p);
                                        havedonesomething = true;
                                        pf.attackWithMinion(m, trgt.target, trgt.targetEntity, attackPenality);
                                        this.posmoves.Add(pf);
                                    }
                                }
                                else
                                {
                                    Playfield pf = new Playfield(p);
                                    havedonesomething = true;
                                    pf.attackWithMinion(m, trgt.target, trgt.targetEntity, attackPenality);
                                    this.posmoves.Add(pf);
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
                        List<targett> trgts = p.getAttackTargets();

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
                            if (this.useCutingTargets) trgts = this.cutAttackTargets(trgts, p);
                        }

                        foreach (targett trgt in trgts)
                        {
                            Playfield pf = new Playfield(p);
                            int heroAttackPen = 0;
                            if (usePenalityManager)
                            {
                                heroAttackPen = penman.getAttackWithHeroPenality(trgt.target, p);
                            }
                            pf.attackWithWeapon(trgt.target, trgt.targetEntity, heroAttackPen);
                            this.posmoves.Add(pf);
                        }
                    }

                    // use ability
                    /// TODO check if ready after manaup
                    if (p.ownAbilityReady && p.mana >= 2 && p.ownHeroAblility.canplayCard(p,2))
                    {
                        int abilityPenality = 0;

                        havedonesomething = true;
                        // if we have mage or priest, we have to target something####################################################
                        if (this.hp.heroname == "mage" || this.hp.heroname == "priest")
                        {

                            List<targett> trgts = p.ownHeroAblility.getTargetsForCard(p);

                            if (isLethalCheck && (this.hp.heroname == "mage" || (this.hp.heroname == "priest" && p.ownHeroAblility.name != "lesserheal")))// only target enemy hero during Lethal check!
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
                                    abilityPenality = penman.getPlayCardPenality(p.ownHeroAblility, trgt.target, p, 0, isLethalCheck);
                                    if (abilityPenality <= 499)
                                    {
                                        Playfield pf = new Playfield(p);
                                        havedonesomething = true;
                                        pf.activateAbility(p.ownHeroAblility, trgt.target, trgt.targetEntity, abilityPenality);
                                        this.posmoves.Add(pf);
                                    }
                                }
                                else
                                {
                                    Playfield pf = new Playfield(p);
                                    havedonesomething = true;
                                    pf.activateAbility(p.ownHeroAblility, trgt.target, trgt.targetEntity, abilityPenality);
                                    this.posmoves.Add(pf);
                                }

                            }
                        }
                        else
                        {
                            // the other classes dont have to target####################################################
                            Playfield pf = new Playfield(p);

                            if (usePenalityManager)
                            {
                                abilityPenality = penman.getPlayCardPenality(p.ownHeroAblility, -1, pf, 0, isLethalCheck);
                                if (abilityPenality <= 499)
                                {
                                    havedonesomething = true;
                                    pf.activateAbility(p.ownHeroAblility, -1, -1, abilityPenality);
                                    this.posmoves.Add(pf);
                                }
                            }
                            else
                            {
                                havedonesomething = true;
                                pf.activateAbility(p.ownHeroAblility, -1, -1, abilityPenality);
                                this.posmoves.Add(pf);
                            }

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
            this.bestboard = new Playfield(bestplay);
            /*if (bestmove != null && bestmove.cardplay && bestmove.handcard.card.type == CardDB.cardtype.MOB)
            {
                Playfield pf = new Playfield();
                help.logg("bestplaces:");
                pf.getBestPlacePrint(bestmove.card);
            }*/

            if (bestmove != null) // save the guessed move, so we doesnt need to recalc!
            {
                this.nextMoveGuess = new Playfield();
                if (bestmove.cardplay)
                {
                    //pf.playCard(c, hc.position - 1, hc.entity, trgt.target, trgt.targetEntity, 0, bestplace, cardplayPenality);
                    Handmanager.Handcard hc = this.nextMoveGuess.owncards.Find(x => x.entity == bestmove.cardEntitiy);

                    if (bestmove.owntarget >= 0 && bestmove.enemytarget >= 0 && bestmove.enemytarget <= 9 && bestmove.owntarget < bestmove.enemytarget)
                    {
                        this.nextMoveGuess.playCard(bestmove.handcard, hc.position - 1, hc.entity, bestmove.enemytarget - 1, bestmove.enemyEntitiy, bestmove.druidchoice, bestmove.owntarget, 0);
                    }
                    else
                    {
                        this.nextMoveGuess.playCard(bestmove.handcard, hc.position - 1, hc.entity, bestmove.enemytarget, bestmove.enemyEntitiy, bestmove.druidchoice, bestmove.owntarget, 0);
                    }
                    //this.nextMoveGuess.playCard(bestmove.card, hc.position - 1, hc.entity, bestmove.enemytarget, bestmove.enemyEntitiy, bestmove.druidchoice, bestmove.owntarget, 0);

                }

                if (bestmove.minionplay)
                {
                    //.attackWithMinion(m, trgt.target, trgt.targetEntity, attackPenality);
                    Minion m = this.nextMoveGuess.ownMinions.Find(x => x.entitiyID == bestmove.ownEntitiy);
                    this.nextMoveGuess.attackWithMinion(m, bestmove.enemytarget, bestmove.enemyEntitiy, 0);

                }

                if (bestmove.heroattack)
                {
                    this.nextMoveGuess.attackWithWeapon(bestmove.enemytarget, bestmove.enemyEntitiy, 0);
                }

                if (bestmove.useability)
                {
                    //.activateAbility(p.ownHeroAblility, trgt.target, trgt.targetEntity, abilityPenality);
                    this.nextMoveGuess.activateAbility(this.nextMoveGuess.ownHeroAblility, bestmove.enemytarget, bestmove.enemyEntitiy, 0);
                }

                this.bestboard.playactions.RemoveAt(0);
            }
            else
            {
                nextMoveGuess.mana = -1;
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


        private List<targett> cutAttackTargets(List<targett> oldlist, Playfield p)
        {
            List<targett> retvalues = new List<targett>();
            List<Minion> addedmins = new List<Minion>(8);

            bool priomins = false;
            List<targett> retvaluesPrio = new List<targett>();
            foreach (targett t in oldlist)
            {
                if (t.target == 200)
                {
                    retvalues.Add(t);
                    continue;
                }
                if (t.target >= 10 && t.target <= 20)
                {
                    Minion m = p.enemyMinions[t.target - 10];
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
                    bool isSpecial = penman.specialMinions.ContainsKey(m.name);
                    foreach (Minion mnn in temp)
                    {
                        // special minions are allowed to attack in silended and unsilenced state!
                        //help.logg(mnn.silenced + " " + m.silenced + " " + mnn.name + " " + m.name + " " + penman.specialMinions.ContainsKey(m.name));

                        bool otherisSpecial = penman.specialMinions.ContainsKey(mnn.name);

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

        private void doNextCalcedMove()
        {
            help.logg("noRecalcNeeded!!!-----------------------------------");
            this.bestboard.printActions();
            this.bestmove = this.bestboard.getNextAction();

            if (bestmove != null) // save the guessed move, so we doesnt need to recalc!
            {
                this.nextMoveGuess = new Playfield();
                if (bestmove.cardplay)
                {
                    //pf.playCard(c, hc.position - 1, hc.entity, trgt.target, trgt.targetEntity, 0, bestplace, cardplayPenality);
                    Handmanager.Handcard hc = this.nextMoveGuess.owncards.Find(x => x.entity == bestmove.cardEntitiy);
                    if (bestmove.owntarget >= 0 && bestmove.enemytarget >= 0 && bestmove.enemytarget <= 9 && bestmove.owntarget < bestmove.enemytarget)
                    {
                        this.nextMoveGuess.playCard(bestmove.handcard, hc.position - 1, hc.entity, bestmove.enemytarget - 1, bestmove.enemyEntitiy, bestmove.druidchoice, bestmove.owntarget, 0);
                    }
                    else
                    {
                        this.nextMoveGuess.playCard(bestmove.handcard, hc.position - 1, hc.entity, bestmove.enemytarget, bestmove.enemyEntitiy, bestmove.druidchoice, bestmove.owntarget, 0);
                    }


                }

                if (bestmove.minionplay)
                {
                    //.attackWithMinion(m, trgt.target, trgt.targetEntity, attackPenality);
                    Minion m = this.nextMoveGuess.ownMinions.Find(x => x.entitiyID == bestmove.ownEntitiy);
                    this.nextMoveGuess.attackWithMinion(m, bestmove.enemytarget, bestmove.enemyEntitiy, 0);

                }

                if (bestmove.heroattack)
                {
                    this.nextMoveGuess.attackWithWeapon(bestmove.enemytarget, bestmove.enemyEntitiy, 0);
                }

                if (bestmove.useability)
                {
                    //.activateAbility(p.ownHeroAblility, trgt.target, trgt.targetEntity, abilityPenality);
                    this.nextMoveGuess.activateAbility(this.nextMoveGuess.ownHeroAblility, bestmove.enemytarget, bestmove.enemyEntitiy, 0);
                }

                this.bestboard.playactions.RemoveAt(0);
            }
            else
            {
                nextMoveGuess.mana = -1;
            }

        }

        public void dosomethingclever(Bot botbase)
        {
            //return;
            //turncheck
            //help.moveMouse(950,750);
            //help.Screenshot();

            hp.updatePositions();

            posmoves.Clear();
            posmoves.Add(new Playfield());

            /* foreach (var item in this.posmoves[0].owncards)
             {
                 help.logg("card " + item.handcard.card.name + " is playable :" + item.handcard.card.canplayCard(posmoves[0]) + " cost/mana: " + item.handcard.card.cost + "/" + posmoves[0].mana);
             }
             */
            //help.logg("is hero ready?" + posmoves[0].ownHeroReady);

            help.loggonoff(false);
            //do we need to recalc?
            help.logg("recalc-check###########");
            if (this.dontRecalc && posmoves[0].isEqual(this.nextMoveGuess))
            {
                doNextCalcedMove();
            }
            else
            {
                help.logg("Leathal-check###########");
                bestmoveValue = -1000000;
                DateTime strt = DateTime.Now;
                if (useLethalCheck)
                {
                    strt = DateTime.Now;
                    doallmoves(false, botbase, true);
                    help.logg("calculated " + (DateTime.Now - strt).TotalSeconds);
                }
                
                if (bestmoveValue < 10000)
                {
                    posmoves.Clear();
                    posmoves.Add(new Playfield());
                    help.logg("no lethal, do something random######");
                    strt = DateTime.Now;
                    doallmoves(false, botbase, false);
                    help.logg("calculated " + (DateTime.Now - strt).TotalSeconds);
                    
                }
            }


            //help.logging(true);

        }

        public void autoTester(Bot botbase)
        {
            help.logg("simulating board ");

            BoardTester bt = new BoardTester();

            hp.printHero();
            hp.printOwnMinions();
            hp.printEnemyMinions();
            hm.printcards();
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
                help.logg("card " + item.card.name + " is playable :" + item.canplayCard(posmoves[0]) + " cost/mana: " + item.manacost + "/" + posmoves[0].mana);
            }
            help.logg("ability " + posmoves[0].ownHeroAblility.name + " is playable :" + posmoves[0].ownHeroAblility.canplayCard(posmoves[0],2) + " cost/mana: " + posmoves[0].ownHeroAblility.getManaCost(posmoves[0],2) + "/" + posmoves[0].mana);

            // lethalcheck + normal
            DateTime strt = DateTime.Now;
            doallmoves(false, botbase, true);
            help.logg("calculated " + (DateTime.Now - strt).TotalSeconds);
            if (bestmoveValue < 10000)
            {
                posmoves.Clear();
                posmoves.Add(new Playfield());
                strt = DateTime.Now;
                doallmoves(false, botbase, false);
                help.logg("calculated " + (DateTime.Now - strt).TotalSeconds);
            }

            foreach (Playfield p in this.posmoves)
            {
                p.printBoard();
            }
            help.logg("bestfield");
            bestboard.printBoard();
            //simmulateWholeTurn();
        }

        public void simmulateWholeTurn()
        {
            help.logg("simulate best board");
            //this.bestboard.printActions();

            Playfield tempbestboard = new Playfield();

            if (bestmove != null) // save the guessed move, so we doesnt need to recalc!
            {
                bestmove.print();
                if (bestmove.cardplay)
                {
                    help.logg("card");
                    //pf.playCard(c, hc.position - 1, hc.entity, trgt.target, trgt.targetEntity, 0, bestplace, cardplayPenality);
                    Handmanager.Handcard hc = tempbestboard.owncards.Find(x => x.entity == bestmove.cardEntitiy);
                    if (bestmove.owntarget >= 0 && bestmove.enemytarget >= 0 && bestmove.enemytarget <= 9 && bestmove.owntarget < bestmove.enemytarget)
                    {
                        tempbestboard.playCard(bestmove.handcard, hc.position - 1, hc.entity, bestmove.enemytarget - 1, bestmove.enemyEntitiy, bestmove.druidchoice, bestmove.owntarget, 0);
                    }
                    else
                    {
                        tempbestboard.playCard(bestmove.handcard, hc.position - 1, hc.entity, bestmove.enemytarget, bestmove.enemyEntitiy, bestmove.druidchoice, bestmove.owntarget, 0);
                    }
                }

                if (bestmove.minionplay)
                {
                    help.logg("min");
                    //.attackWithMinion(m, trgt.target, trgt.targetEntity, attackPenality);
                    Minion mm = tempbestboard.ownMinions.Find(x => x.entitiyID == bestmove.ownEntitiy);
                    help.logg("min");
                    tempbestboard.attackWithMinion(mm, bestmove.enemytarget, bestmove.enemyEntitiy, 0);
                    help.logg("min");
                }

                if (bestmove.heroattack)
                {
                    help.logg("hero");
                    tempbestboard.attackWithWeapon(bestmove.enemytarget, bestmove.enemyEntitiy, 0);
                }

                if (bestmove.useability)
                {
                    help.logg("abi");
                    //.activateAbility(p.ownHeroAblility, trgt.target, trgt.targetEntity, abilityPenality);
                    tempbestboard.activateAbility(this.nextMoveGuess.ownHeroAblility, bestmove.enemytarget, bestmove.enemyEntitiy, 0);
                }

            }
            else
            {
                tempbestboard.mana = -1;
            }
            help.logg("-------------");
            help.logg("OwnMinions:");


            foreach (Minion m in tempbestboard.ownMinions)
            {
                help.logg(m.name + " id " + m.id + " zp " + m.zonepos + " " + " e:" + m.entitiyID + " " + " A:" + m.Angr + " H:" + m.Hp + " mH:" + m.maxHp + " rdy:" + m.Ready + " tnt:" + m.taunt + " frz:" + m.frozen + " silenced:" + m.silenced + " divshield:" + m.divineshild + " ptt:" + m.playedThisTurn + " wndfr:" + m.windfury + " natt:" + m.numAttacksThisTurn + " sil:" + m.silenced + " stl:" + m.stealth + " poi:" + m.poisonous + " imm:" + m.immune + " ex:" + m.exhausted + " chrg:" + m.charge);
                foreach (Enchantment e in m.enchantments)
                {
                    help.logg(e.CARDID + " " + e.creator + " " + e.controllerOfCreator);
                }
            }
            help.logg("EnemyMinions:");
            foreach (Minion m in tempbestboard.enemyMinions)
            {
                help.logg(m.name + " id " + m.id + " zp " + m.zonepos + " " + " e:" + m.entitiyID + " " + " A:" + m.Angr + " H:" + m.Hp + " mH:" + m.maxHp + " rdy:" + m.Ready + " tnt:" + m.taunt + " frz:" + m.frozen + " silenced:" + m.silenced + " divshield:" + m.divineshild + " wndfr:" + m.windfury + " sil:" + m.silenced + " stl:" + m.stealth + " poi:" + m.poisonous + " imm:" + m.immune + " ex:" + m.exhausted);
                foreach (Enchantment e in m.enchantments)
                {
                    help.logg(e.CARDID + " " + e.creator + " " + e.controllerOfCreator);
                }
            }


            foreach (Action bestmovee in bestboard.playactions)
            {

                help.logg("stepp");


                if (bestmovee != null) // save the guessed move, so we doesnt need to recalc!
                {
                    bestmovee.print();
                    if (bestmovee.cardplay)
                    {
                        help.logg("card");
                        //pf.playCard(c, hc.position - 1, hc.entity, trgt.target, trgt.targetEntity, 0, bestplace, cardplayPenality);
                        Handmanager.Handcard hc = tempbestboard.owncards.Find(x => x.entity == bestmovee.cardEntitiy);
                        if (bestmovee.owntarget >= 0 && bestmovee.enemytarget >= 0 && bestmovee.enemytarget <= 9 && bestmovee.owntarget < bestmovee.enemytarget)
                        {
                            tempbestboard.playCard(bestmovee.handcard, hc.position - 1, hc.entity, bestmovee.enemytarget - 1, bestmovee.enemyEntitiy, bestmovee.druidchoice, bestmovee.owntarget, 0);
                        }
                        else
                        {
                            tempbestboard.playCard(bestmovee.handcard, hc.position - 1, hc.entity, bestmovee.enemytarget, bestmovee.enemyEntitiy, bestmovee.druidchoice, bestmovee.owntarget, 0);
                        }
                    }

                    if (bestmovee.minionplay)
                    {
                        help.logg("min");
                        //.attackWithMinion(m, trgt.target, trgt.targetEntity, attackPenality);
                        Minion mm = tempbestboard.ownMinions.Find(x => x.entitiyID == bestmovee.ownEntitiy);
                        help.logg("min");
                        tempbestboard.attackWithMinion(mm, bestmovee.enemytarget, bestmovee.enemyEntitiy, 0);
                        help.logg("min");
                    }

                    if (bestmovee.heroattack)
                    {
                        help.logg("hero");
                        tempbestboard.attackWithWeapon(bestmovee.enemytarget, bestmovee.enemyEntitiy, 0);
                    }

                    if (bestmovee.useability)
                    {
                        help.logg("abi");
                        //.activateAbility(p.ownHeroAblility, trgt.target, trgt.targetEntity, abilityPenality);
                        tempbestboard.activateAbility(this.nextMoveGuess.ownHeroAblility, bestmovee.enemytarget, bestmovee.enemyEntitiy, 0);
                    }

                }
                else
                {
                    tempbestboard.mana = -1;
                }
                help.logg("-------------");
                help.logg("OwnMinions:");
                foreach (Minion m in tempbestboard.ownMinions)
                {
                    help.logg(m.name + " id " + m.id + " zp " + m.zonepos + " " + " e:" + m.entitiyID + " " + " A:" + m.Angr + " H:" + m.Hp + " mH:" + m.maxHp + " rdy:" + m.Ready + " tnt:" + m.taunt + " frz:" + m.frozen + " silenced:" + m.silenced + " divshield:" + m.divineshild + " ptt:" + m.playedThisTurn + " wndfr:" + m.windfury + " natt:" + m.numAttacksThisTurn + " sil:" + m.silenced + " stl:" + m.stealth + " poi:" + m.poisonous + " imm:" + m.immune + " ex:" + m.exhausted + " chrg:" + m.charge);
                    foreach (Enchantment e in m.enchantments)
                    {
                        help.logg(e.CARDID + " " + e.creator + " " + e.controllerOfCreator);
                    }
                }
                help.logg("EnemyMinions:");
                foreach (Minion m in tempbestboard.enemyMinions)
                {
                    help.logg(m.name + " id " + m.id + " zp " + m.zonepos + " " + " e:" + m.entitiyID + " " + " A:" + m.Angr + " H:" + m.Hp + " mH:" + m.maxHp + " rdy:" + m.Ready + " tnt:" + m.taunt + " frz:" + m.frozen + " silenced:" + m.silenced + " divshield:" + m.divineshild + " wndfr:" + m.windfury + " sil:" + m.silenced + " stl:" + m.stealth + " poi:" + m.poisonous + " imm:" + m.immune + " ex:" + m.exhausted);
                    foreach (Enchantment e in m.enchantments)
                    {
                        help.logg(e.CARDID + " " + e.creator + " " + e.controllerOfCreator);
                    }
                }
            }
        }

    }



}
