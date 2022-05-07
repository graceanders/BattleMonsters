using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using MonoGameLibrary.Util;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace BattleMonsters
{
    public enum BattleState { Playing, Won, Lost, Forfit }
    public class BattleManager : DrawableGameComponent, IInteractable
    {
        Game game;
        InputHandler input;

        public BattleState BattleState { get; set; }
        public GameDifficulty GameDifficulty { get; }

        public GameMode GameMode { get; }

        Player P;
        Enemy E;

        Vector2 CPMLocation, CEMLocation;

        public int Turn;

        bool MoveMade = false;

        public string ButtonGuideTxt { get; set; }
        public Vector2 ButtonGuideLoc { get; set; }

        Color BattleElement;

        //Randoms:
        Random RunAttempt = new Random();

        bool PlayerDid;

        int margin = 10;

        public bool LockedIn;

        //Allow the enemy AI to check CPM's type and try to swith to a monster that beats it?

        //this would requre a new Battle Manager to be created for every new enemy


        public BattleManager(Game G, Player P, Enemy E) : base(G)
        {
            input = (InputHandler)G.Services.GetService(typeof(IInputHandler));

            game = G;
            this.P = P;
            this.E = E;

            Turn = 1;


            this.BattleState = BattleState.Playing;
        }

        protected override void LoadContent()
        {
            base.LoadContent();
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Update(GameTime gametime)
        {
            base.Update(gametime);
        }

        double Damage;
        bool PlayerHasDamaged = false;
        bool EnemyHasDamaged = false;
        public void PlayerRound()
        {
            Damage = P.CurrentMonster.Attack(P.CurrentMonster.ATKScore, E.CurrentMonster.DEFScore);

            DamageMonster(E.CurrentMonster, (int)Damage); ;
            GamePrintout.TxtPrintOut += $"\n{P.CurrentMonster.Name} Damaged {E.CurrentMonster.Name} for {(int)Damage} HP points\n{E.CurrentMonster.Name}'s HP is now at {E.CurrentMonster.HP}";
        }

        public void EnemyRound()
        {
            CheckProbablilityOfLoss();
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

        bool RoundCompleted = false;
        public void Round(bool Attack)
        {
            MoveMade = true;
            if (!RoundCompleted)
            {
                BattleState = BattleState.Playing;

                CheckLife();

                if (Attack)//If the player desides to attack
                {
                    GamePrintout.TxtPrintOut = "You have decided to Attack!";
                    PlayerRound();
                }
                else
                {
                    GamePrintout.TxtPrintOut = "You have decided to Run!";
                    Run(P);
                }

                if (BattleState == BattleState.Playing) { EnemyRound(); }
                Turn++;
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

        public void Lost(Character WhichCharacter)
        {
            if (WhichCharacter == E)
            {
                //Enemy lost next battle would be opened in the game
            }
            if (WhichCharacter == P)
            {
                //Player lost next battle would not be open
            }
        }

        public void CheckCoinsLost()
        {
            //depends on difficulty
            if (GameDifficulty == GameDifficulty.Easy)
            {
                P.Coins -= (P.Coins / 3); //Looses a 1/3
            }
            if (GameDifficulty == GameDifficulty.Medium)
            {
                P.Coins -= ((P.Coins / 3) * 2); //Looses 2/3
            }
            if (GameDifficulty == GameDifficulty.Hard) //Looses all
            {
                P.Coins -= P.Coins;
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

        }

        public void InBattleInput()
        {
            if (!MoveMade)
            {
                if (input.KeyboardState.WasKeyPressed(Keys.A))
                {
                    this.Round(true);//Attack
                }
                if (input.KeyboardState.WasKeyPressed(Keys.R))
                {
                    this.Round(false);//Run
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

    }
}
