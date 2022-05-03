using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using MonoGameLibrary.Util;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace BattleMonsters
{
    //ERROR: Attack is attacking more then once

    public enum BattleState { Playing, Won, Lost, Forfit }
    public class BattleManager : DrawableGameComponent
    {
        Game game;
        InputHandler input;

        public BattleState BattleState { get; set; }
        public GameDifficulty GameDifficulty { get; }

        public GameMode GameMode { get; }

        Player P;
        Enemy E;

        bool MoveMade = false;


        //When player or enemy selects there monster it will be set to this
        //public Creature CPM; //Current Player Monster
        //public Creature CEM; //Current Enemy Monster


        //Randoms:
        Random RunAttempt = new Random();


        bool PlayerTurn, PlayerSwaped;

        bool PlayerDid;

        Vector2 CPMLocation;
        Vector2 CEMLocation;

        int margin = 10;

        //Allow the enemy AI to check CPM's type and try to swith to a monster that beats it?

        //this would requre a new Battle Manager to be created for every new enemy


        public BattleManager(Game G, Player P, Enemy E) : base(G)
        {
            input = (InputHandler)G.Services.GetService(typeof(IInputHandler));

            game = G;
            this.P = P;
            this.E = E;

            this.BattleState = BattleState.Playing;
        }

        protected override void LoadContent()
        {
            base.LoadContent();
        }

        public override void Initialize()
        {
            PlayerSwaped = false;
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
            GamePrintout.TxtPrintOut += $"\nThe {WhichCharacter.Name} was unsucesfull in their Run attempt";
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

        bool Attacked;
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

    }
}
