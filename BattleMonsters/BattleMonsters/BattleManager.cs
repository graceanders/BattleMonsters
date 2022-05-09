using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using MonoGameLibrary.Util;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using System.Threading;

namespace BattleMonsters
{
    public enum BattleState { Playing, Won, Lost, Forfit}
    public class BattleManager : DrawableGameComponent, IInteractable
    {
        Game game;
        InputHandler input;

        private BattleState BattleState { get; set; }
        public GameDifficulty GameDifficulty { get; }

        public GameMode GameMode { get; }

        Player P;
        Enemy E;

        public int Turn;

        bool MoveMade = false;

        public string ButtonGuideTxt { get; set; }
        public Vector2 ButtonGuideLoc { get; set; }

        Color BattleElement;

        //Randoms:
        Random RunAttempt = new Random();
        Random WillRun = new Random();

        bool PlayerDid;

        public bool LockedIn;

        //Allow the enemy AI to check CPM's type and try to swith to a monster that beats it?


        public BattleManager(Game G, Player P, Enemy E) : base(G)
        {
            input = (InputHandler)G.Services.GetService(typeof(IInputHandler));

            game = G;
            this.P = P;
            this.E = E;

            Turn = 1;

            this.BattleState = BattleState.Playing;
        }

        public bool Won, BattleOver;
        public void CheckBattleState()
        {
            switch (BattleState)
            {
                case BattleState.Playing:
                    BattleOver = false;
                    break;
                case BattleState.Won:
                    Won = true;
                    BattleOver = true;
                    break;
                case BattleState.Lost:
                    Won = false;
                    BattleOver = true;
                    break;
                case BattleState.Forfit:
                    BattleOver = true;
                    Won = false;
                    break;
            }
        }

        string results;
        public string ReturnResults() 
        { 
            if(BattleState == BattleState.Won)
            {
                results = $"You defeated Enemy {E.Name}\nYou can now progress to the next Round!";
            }

            if(BattleState == BattleState.Lost)
            {
                results = $"Enemy {E.Name} defeated you!\nTo progress to the next Round you must defeat the Enemy!";
            }

            if (BattleState == BattleState.Forfit)
            {
                if (PlayerDid) { results = "You ran before defeating the Enemy\nTo progress to the next Round you must defeat the Enemy!"; }
                if (PlayerDid) { results = "You ran before defeating the Enemy\nTo progress to the next Round you must defeat the Enemy!"; }
            }

            return results;
        }


        void CallPause()
        {
            Pause = true;
            GamePrintout.TxtPrintOut += $"\n\nThe Enemy will go in 5 seconds";
        }

        bool Pause;
        bool RoundCompleted = false;
        public void FullTurn(bool Attack)
        {
            MoveMade = true;
            if (!RoundCompleted)
            {
                BattleState = BattleState.Playing;

                CheckLife();

                if (Attack)//If the player desides to attack
                {
                    GamePrintout.TxtPrintOut = "You have decided to Attack!";
                    PlayerTurn();
                }
                else
                {
                    GamePrintout.TxtPrintOut = "You have decided to Run!";
                    Run(P);
                }

                if (BattleState == BattleState.Playing) 
                {
                    //CallPause();
                    //while (!Pause)
                    //{
                    //    Thread.Sleep(5000);
                    //    Pause = true;
                    //}
                    EnemyTurn();
                    Turn++;

                }
                
            }
            else
            {
                TurnCompleted();
            }

        }


        double Damage;
        bool PlayerHasDamaged = false;
        bool EnemyHasDamaged = false;
        public void PlayerTurn()
        {
            Damage = P.CurrentMonster.Attack(P.CurrentMonster.ATKScore, E.CurrentMonster.DEFScore);

            DamageMonster(E.CurrentMonster, (int)Damage); ;
            GamePrintout.TxtPrintOut += $"\n{P.CurrentMonster.Name} Damaged {E.CurrentMonster.Name} for {(int)Damage} HP points\n{E.CurrentMonster.Name}'s HP is now at {E.CurrentMonster.HP}";
        }

        int Decision;
        public void EnemyTurn()
        {
            CallPause();
            Decision = WillRun.Next(0, 100);
            if(Decision > 50) { CheckProbablilityOfLoss(); }//Need to have the Enemy not forfit every time

            //OptomizeEnemySwap(); //Potentially build out
            if(BattleState == BattleState.Playing)
            {
                Damage = E.CurrentMonster.Attack(E.CurrentMonster.ATKScore, P.CurrentMonster.DEFScore);
                DamageMonster(P.CurrentMonster, (int)Damage);
                GamePrintout.TxtPrintOut += $"\n{E.CurrentMonster.Name} Damaged {P.CurrentMonster.Name} for {(int)Damage} HP points\n{P.CurrentMonster.Name}'s HP is now at {P.CurrentMonster.HP}";
            }
            RoundCompleted = true;
            GamePrintout.TxtPrintOut += "\nRound Completed";
        }

        public void TurnCompleted()
        {
            CheckLife();
            if (BattleState == BattleState.Playing)
            {
                MoveMade = false;
                RoundCompleted = false;
            }
        }



        void DamageMonster(Creature WhichMonster, int Damage)
        {
            if(WhichMonster == E.CurrentMonster)//Player Attacking
            {
                if (!PlayerHasDamaged) { WhichMonster.HP -= Damage; }
                PlayerHasDamaged = true;
            }
            if(WhichMonster == P.CurrentMonster)//Enemy Attacking
            {
                if (!EnemyHasDamaged) { WhichMonster.HP -= Damage; }
                EnemyHasDamaged = true;
            }
        }

        public void CheckProbablilityOfLoss()
        {
            if (P.CurrentMonster.ATKScore > E.CurrentMonster.HP)//if the enemy will lose on the next round
            {
                GamePrintout.TxtPrintOut += $"\nThe Enemy is attempting to Run!";
                //Attempt run
                Run(E);
            }
        }

        public void OptomizeEnemySwap()
        {
            if (GameDifficulty == GameDifficulty.Hard)
            {
                //allow the enemy to swap there monster for one that is better equipt at fighing the players monster
            }
        }

        #region Run

        int RunSucessRate;
        public void Run(Character WhichCharacter)
        {
            RunSucessRate = RunAttempt.Next(100);
            if (RunSucessRate < 60)
            {
                RunSucess(WhichCharacter);
            }
            else
            {
                GamePrintout.TxtPrintOut += $"\nThe {WhichCharacter.Name} was unsucesfull in their Run attempt";
            }
            
        }

        public bool RunSucess(Character WhichCharacter)
        {
            GamePrintout.TxtPrintOut += $"\nThe {WhichCharacter.Name} sucessfully Ran";
            BattleState = BattleState.Forfit;
            if (WhichCharacter == E)
            {
                BattleState = BattleState.Forfit;
                PlayerDid = false;
                
            }
            if (WhichCharacter == P)
            {
                BattleState = BattleState.Forfit;
                PlayerDid = true;
            }
            return PlayerDid;
        }

        #endregion

        int PlayerCombinedHP, EnemyCombinedHP;
        public void CheckLife()
        {
            PlayerCombinedHP = P.CalculateTeamHP();
            EnemyCombinedHP = E.CalculateTeamHP();

            if (PlayerCombinedHP <= 0)
            {
                this.BattleState = BattleState.Lost;
            }
            if (EnemyCombinedHP <= 0)
            {
                this.BattleState = BattleState.Won;
            }
        }

        #region Coins

        public void CheckCoinsLost()
        {
            //depends on difficulty
            if (GameDifficulty == GameDifficulty.Easy)
            {
                P.Coins -= (P.Coins / 3); //Looses a 1/3
                GamePrintout.TxtPrintOut += "\nYou lost a 1/3 of your Coins";
            }
            if (GameDifficulty == GameDifficulty.Medium)
            {
                P.Coins -= ((P.Coins / 3) * 2); //Looses 2/3
                GamePrintout.TxtPrintOut += "\nYou lost a 2/3 of your Coins";
            }
            if (GameDifficulty == GameDifficulty.Hard) //Looses all
            {
                P.Coins -= P.Coins;
                GamePrintout.TxtPrintOut += "\nYou lost all of your Coins";
            }
        }

        public void CheckCoinsGained()
        {
            //depends on difficulty
            if (GameDifficulty == GameDifficulty.Easy)
            {
                P.Coins += (E.Coins / 3); // Gains a 1/3 of the Enemies coins
            }
            if (GameDifficulty == GameDifficulty.Medium)
            {
                P.Coins += ((E.Coins / 3) * 2); //Gains 2/3 of the Enemies coins
            }
            if (GameDifficulty == GameDifficulty.Hard)
            {
                P.Coins += E.Coins; // Gains all of Enemies Coins
            }
            GamePrintout.TxtPrintOut += $"\nYou gaid {E.Coins / 3} Coins from the Enemy";

        }
        #endregion

        #region Input
        public void InBattleInput()
        {
            if (!MoveMade)
            {
                if (input.KeyboardState.WasKeyPressed(Keys.A))
                {
                    this.FullTurn(true);//Attack
                }
                if (input.KeyboardState.WasKeyPressed(Keys.R))
                {
                    this.FullTurn(false);//Run
                }
            }
        }

        public void MonsterSwapInput()
        {
            //Switches which monster the user is using at the moment
            //Probally create a bool value for is the player is attacking/ being attacked so they cant switch mid that
            if (input.KeyboardState.WasKeyPressed(Keys.D1))
            {
                P.CurrentMonster = P.Team[0];
                GamePrintout.TxtPrintOut = "You have Swapped to your 1st Monster!\nLock in to begin!";
            }
            if (input.KeyboardState.WasKeyPressed(Keys.D2))
            {
                if (P.Team.Count == 1)//No 2nd Monster
                {
                    GamePrintout.TxtPrintOut = "You only have 1 Monster in your team and cannot swap\nLock in to begin!";
                }
                else
                {
                    P.CurrentMonster = P.Team[1];
                    GamePrintout.TxtPrintOut = "You have Swapped to your 2st Monster!\nLock in to begin!";
                }
            }
            if (input.KeyboardState.WasKeyPressed(Keys.D3))
            {
                if (P.Team.Count < 3)//No 3rd
                {
                    if (P.Team.Count == 1)
                        GamePrintout.TxtPrintOut = "You only have 1 Monster in your Team and cannot Swap\nLock in to begin!";
                    else
                        GamePrintout.TxtPrintOut = "You only have 2 Monster in your Team and cannot Swap to your 3rd\nTry Swaping to the 1st or 2nd Monster";
                }
                else
                {
                    P.CurrentMonster = P.Team[2];
                    GamePrintout.TxtPrintOut = "You have Swapped to your 3st Monster!\nLock in to begin!";
                }
            }
            if (input.KeyboardState.WasKeyPressed(Keys.L))
            {
                
                LockedIn = true;
                GamePrintout.TxtPrintOut = $"You have selected {P.CurrentMonster.Name}\nThe Battle will commense!";
            }
        }
        #endregion

    }
}
