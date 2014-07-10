using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{


    public class Ai
    {
        private int maxdeep = 12;
        private int maxwide = 3000;
        public bool simulateEnemyTurn = true;
        private bool usePenalityManager = true;
        private bool useCutingTargets = true;
        private bool dontRecalc = true;
        private bool useLethalCheck = true;
        private bool useComparison = true;

        public MiniSimulator nextTurnSimulator;
        MiniSimulator mainTurnSimulator;

        PenalityManager penman = PenalityManager.Instance;

        List<Playfield> posmoves = new List<Playfield>(7000);

        Hrtprozis hp = Hrtprozis.Instance;
        Handmanager hm = Handmanager.Instance;
        Helpfunctions help = Helpfunctions.Instance;

        public Action bestmove = new Action();
        public int bestmoveValue = 0;
        Playfield bestboard = new Playfield();
        Playfield nextMoveGuess = new Playfield();
        public Behavior botBase = null;

        private bool secondturnsim = false;
        private bool playaround = false;

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
            this.nextTurnSimulator = new MiniSimulator();
            this.mainTurnSimulator = new MiniSimulator(maxdeep, maxwide, 0); // 0 for unlimited
            this.mainTurnSimulator.setPrintingstuff(true);
        }

        public void setMaxWide(int mw)
        {
            this.maxwide = mw;
            if (maxwide <= 100) this.maxwide = 100;
            this.mainTurnSimulator.updateParams(maxdeep, maxwide, 0);
        }

        public void setTwoTurnSimulation(bool stts)
        {
            this.mainTurnSimulator.setSecondTurnSimu(stts);
            this.secondturnsim = stts;
        }

        public void setPlayAround(bool spa)
        {
            this.mainTurnSimulator.setPlayAround(spa);
            this.playaround = spa;
        }

        private void doallmoves(bool test, bool isLethalCheck)
        {
            this.mainTurnSimulator.doallmoves(this.posmoves[0], isLethalCheck);

            Playfield bestplay = this.mainTurnSimulator.bestboard;
            int bestval = this.mainTurnSimulator.bestmoveValue;

            help.loggonoff(true);
            help.logg("-------------------------------------");
            help.logg("bestPlayvalue " + bestval);

            bestplay.printActions();
            this.bestmove = bestplay.getNextAction();
            this.bestmoveValue = bestval;
            this.bestboard = new Playfield(bestplay);

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

        public void dosomethingclever(Behavior bbase)
        {
            //return;
            //turncheck
            //help.moveMouse(950,750);
            //help.Screenshot();
            this.botBase = bbase;
            hp.updatePositions();

            posmoves.Clear();
            posmoves.Add(new Playfield());
            posmoves[0].sEnemTurn = this.simulateEnemyTurn;
            /* foreach (var item in this.posmoves[0].owncards)
             {
                 help.logg("card " + item.handcard.card.name + " is playable :" + item.handcard.card.canplayCard(posmoves[0]) + " cost/mana: " + item.handcard.card.cost + "/" + posmoves[0].mana);
             }
             */
            //help.logg("is hero ready?" + posmoves[0].ownHeroReady);

            help.loggonoff(false);
            //do we need to recalc?
            help.logg("recalc-check###########");
            if (this.dontRecalc && posmoves[0].isEqual(this.nextMoveGuess, true))
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
                    doallmoves(false, true);
                    help.logg("calculated " + (DateTime.Now - strt).TotalSeconds);
                }

                if (bestmoveValue < 10000)
                {
                    posmoves.Clear();
                    posmoves.Add(new Playfield());
                    posmoves[0].sEnemTurn = this.simulateEnemyTurn;
                    help.logg("no lethal, do something random######");
                    strt = DateTime.Now;
                    doallmoves(false, false);
                    help.logg("calculated " + (DateTime.Now - strt).TotalSeconds);

                }
            }


            //help.logging(true);

        }

        public void autoTester(Behavior bbase, bool printstuff)
        {
            help.logg("simulating board ");

            BoardTester bt = new BoardTester();
            this.botBase = bbase;
            hp.printHero();
            hp.printOwnMinions();
            hp.printEnemyMinions();
            hm.printcards();
            //calculate the stuff
            posmoves.Clear();
            posmoves.Add(new Playfield());
            posmoves[0].sEnemTurn = this.simulateEnemyTurn;
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
            help.logg("ability " + posmoves[0].ownHeroAblility.name + " is playable :" + posmoves[0].ownHeroAblility.canplayCard(posmoves[0], 2) + " cost/mana: " + posmoves[0].ownHeroAblility.getManaCost(posmoves[0], 2) + "/" + posmoves[0].mana);

            // lethalcheck + normal
            DateTime strt = DateTime.Now;
            doallmoves(false, true);
            help.logg("calculated " + (DateTime.Now - strt).TotalSeconds);
            if (bestmoveValue < 10000)
            {
                posmoves.Clear();
                posmoves.Add(new Playfield());
                posmoves[0].sEnemTurn = this.simulateEnemyTurn;
                strt = DateTime.Now;
                doallmoves(false, false);
                help.logg("calculated " + (DateTime.Now - strt).TotalSeconds);
            }

            this.mainTurnSimulator.printPosmoves();

            help.logg("bestfield");
            bestboard.printBoard();
            if(printstuff) simmulateWholeTurn();
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
                    tempbestboard.attackWithMinion(mm, bestmove.enemytarget, bestmove.enemyEntitiy, 0);
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
            tempbestboard.printBoard();

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
                        tempbestboard.attackWithMinion(mm, bestmovee.enemytarget, bestmovee.enemyEntitiy, 0);
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
                tempbestboard.printBoard();
            }

            help.logg("AFTER ENEMY TURN:");
            tempbestboard.sEnemTurn = this.simulateEnemyTurn;
            tempbestboard.endTurn(this.secondturnsim, this.playaround, true);
        }

    }


}
