using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HREngine.Bots
{

    public class EnemyTurnSimulator
    {

        private List<Playfield> posmoves = new List<Playfield>(7000);
        private int maxwide = 20;


        public void simulateEnemysTurn(Playfield rootfield, bool simulateTwoTurns, bool playaround, bool print, int pprob, int pprob2)
        {
            bool havedonesomething = true;
            posmoves.Clear();

            rootfield.enemyAbilityReady = true;
            rootfield.enemyHeroNumAttackThisTurn = 0;
            rootfield.enemyHeroWindfury = false;
            if (rootfield.enemyWeaponName == CardDB.cardName.doomhammer) rootfield.enemyHeroWindfury = true;
            rootfield.enemyheroImmuneWhileAttacking = false;
            if (rootfield.enemyWeaponName == CardDB.cardName.gladiatorslongbow) rootfield.enemyheroImmuneWhileAttacking = true;
            if (!rootfield.enemyHeroFrozen && rootfield.enemyWeaponDurability > 0) rootfield.enemyHeroReady = true;
            rootfield.enemyheroAngr = rootfield.enemyWeaponAttack;
            
            posmoves.Add(new Playfield(rootfield));
            List<Playfield> temp = new List<Playfield>();
            int deep = 0;
            int enemMana = Math.Min(rootfield.enemyMaxMana + 1, 10);

            if (playaround && !rootfield.loatheb)
            {
                int oldval = Ai.Instance.botBase.getPlayfieldValue(posmoves[0]);
                posmoves[0].value = int.MinValue;
                enemMana = posmoves[0].EnemyCardPlaying(rootfield.enemyHeroName, enemMana, rootfield.enemyAnzCards, pprob, pprob2);
                int newval = Ai.Instance.botBase.getPlayfieldValue(posmoves[0]);
                posmoves[0].value = int.MinValue;
                if (oldval < newval)
                {
                    posmoves.Clear();
                    posmoves.Add(new Playfield(rootfield));
                }
            }

            foreach (Minion m in posmoves[0].enemyMinions)
            {
                if (m.frozen || (m.handcard.card.name == CardDB.cardName.ancientwatcher && !m.silenced))
                {
                    m.Ready = false;
                    continue;
                }
                if (m.Angr == 0) continue;
                m.Ready = true;
                m.numAttacksThisTurn = 0;
            }

            //play ability!
            if (posmoves[0].enemyAbilityReady && enemMana >= 2 && posmoves[0].enemyHeroAblility.canplayCard(posmoves[0], 0) && !rootfield.loatheb)
            {
                int abilityPenality = 0;

                havedonesomething = true;
                // if we have mage or priest, we have to target something####################################################
                if (posmoves[0].enemyHeroName == HeroEnum.mage || posmoves[0].enemyHeroName == HeroEnum.priest)
                {

                    List<targett> trgts = posmoves[0].enemyHeroAblility.getTargetsForCardEnemy(posmoves[0]);
                    foreach (targett trgt in trgts)
                    {
                        if (trgt.target >= 100) continue;
                        Playfield pf = new Playfield(posmoves[0]);
                        //havedonesomething = true;
                        //Helpfunctions.Instance.logg("use ability on " + trgt.target + " " + trgt.targetEntity);
                        pf.ENEMYactivateAbility(posmoves[0].enemyHeroAblility, trgt.target, trgt.targetEntity);
                        posmoves.Add(pf);
                    }
                }
                else
                {
                    // the other classes dont have to target####################################################
                    Playfield pf = new Playfield(posmoves[0]);

                    //havedonesomething = true;
                    posmoves[0].ENEMYactivateAbility(posmoves[0].enemyHeroAblility, -1, -1);
                    posmoves.Add(pf);
                }

            }

            while (havedonesomething)
            {

                temp.Clear();
                temp.AddRange(posmoves);
                havedonesomething = false;
                Playfield bestold = null;
                int bestoldval = 20000000;
                foreach (Playfield p in temp)
                {

                    if (p.complete)
                    {
                        continue;
                    }
                    List<Minion> playedMinions = new List<Minion>(8);

                    foreach (Minion m in p.enemyMinions)
                    {

                        if (m.Ready && m.Angr >= 1 && !m.frozen)
                        {
                            //BEGIN:cut (double/similar) attacking minions out#####################################
                            // DONT LET SIMMILAR MINIONS ATTACK IN ONE TURN (example 3 unlesh the hounds-hounds doesnt need to simulated hole)
                            List<Minion> tempoo = new List<Minion>(playedMinions);
                            bool dontattacked = true;
                            bool isSpecial = PenalityManager.Instance.specialMinions.ContainsKey(m.name);
                            foreach (Minion mnn in tempoo)
                            {
                                // special minions are allowed to attack in silended and unsilenced state!
                                //help.logg(mnn.silenced + " " + m.silenced + " " + mnn.name + " " + m.name + " " + penman.cardName.ContainsKey(m.name));

                                bool otherisSpecial = PenalityManager.Instance.specialMinions.ContainsKey(mnn.name);

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
                            List<targett> trgts = p.getAttackTargets(false);



                            if (true)//(this.useCutingTargets)
                            {
                                trgts = Ai.Instance.nextTurnSimulator.cutAttackTargets(trgts, p, false);
                            }

                            foreach (targett trgt in trgts)
                            {

                                Playfield pf = new Playfield(p);
                                havedonesomething = true;
                                pf.ENEMYattackWithMinion(m, trgt.target, trgt.targetEntity);
                                posmoves.Add(pf);


                            }
                            if (trgts.Count == 1 && trgts[0].target == 100)//only enemy hero is available als attack
                            {
                                break;
                            }
                        }

                    }
                    // attacked with minions done
                    // attack with hero
                    if (p.enemyHeroReady)
                    {
                        List<targett> trgts = p.getAttackTargets(false);

                        havedonesomething = true;


                        if (true)//(this.useCutingTargets)
                        {
                            trgts = Ai.Instance.nextTurnSimulator.cutAttackTargets(trgts, p, false);
                        }

                        foreach (targett trgt in trgts)
                        {
                            Playfield pf = new Playfield(p);
                            pf.ENEMYattackWithWeapon(trgt.target, trgt.targetEntity, 0);
                            posmoves.Add(pf);
                        }
                    }

                    // use ability
                    /// TODO check if ready after manaup

                    p.endEnemyTurn();
                    p.guessingHeroHP = rootfield.guessingHeroHP;
                    if (Ai.Instance.botBase.getPlayfieldValue(p) < bestoldval) // want the best enemy-play-> worst for us
                    {
                        bestoldval = Ai.Instance.botBase.getPlayfieldValue(p);
                        bestold = p;
                    }
                    posmoves.Remove(p);

                    if (posmoves.Count >= maxwide) break;
                }

                if (bestoldval <= 10000 && bestold != null)
                {
                    posmoves.Add(bestold);
                }

                deep++;
                if (posmoves.Count >= maxwide) break;
            }

            foreach (Playfield p in posmoves)
            {
                if (!p.complete) p.endEnemyTurn();
            }

            int bestval = int.MaxValue;
            Playfield bestplay = posmoves[0];
            if (print) bestplay.printBoard();
            foreach (Playfield p in posmoves)
            {
                p.guessingHeroHP = rootfield.guessingHeroHP;
                int val = Ai.Instance.botBase.getPlayfieldValue(p);
                if (bestval > val)// we search the worst value
                {
                    bestplay = p;
                    bestval = val;
                }
            }

            rootfield.value = bestplay.value;
            if (simulateTwoTurns)
            {
                bestplay.prepareNextTurn();
                rootfield.value = (int)(0.5 * bestval + 0.5 * Ai.Instance.nextTurnSimulator.doallmoves(bestplay, false));
            }



        }




    }

}
