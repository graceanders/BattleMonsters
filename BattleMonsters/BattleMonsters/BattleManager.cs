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
    public class BattleManager : DrawableGameComponent
    {
        Game game;
        InputHandler input;

        public BattleState BattleState { get; set; }
        public GameDifficulty GameDifficulty { get; }

        public GameMode GameMode { get; }

        Player player;
        Enemy enemy;


        //When player or enemy selects there monster it will be set to this
        public Creature CPM; //Current Player Monster
        public Creature CEM; //Current Enemy Monster


        //Randoms:
        Random RunAttempt = new Random();


        bool PlayerTurn, PlayerSwaped;

        bool PlayerDid;

        Vector2 CPMLocation;
        Vector2 CEMLocation;

        int margin = 10;


        SpriteBatch sb;
        //Allow the enemy AI to check CPM's type and try to swith to a monster that beats it?

        //this would requre a new Battle Manager to be created for every new enemy


        public BattleManager(Game G, Player P, Enemy E) : base(G)
        {
            game = G;
            player = P;
            enemy = E;

            //input = (InputHandler)G.Services.GetService(typeof(IInputHandler));

            ////Player
            //CPM = player.Team[0];
            //G.Components.Add(CPM);

            ////Enemy
            //CEM = enemy.Team[0];
            //G.Components.Add(CEM);


            this.BattleState = BattleState.Playing;
        }

        protected override void LoadContent()
        {
            base.LoadContent();
        }

        public override void Initialize()
        {
            PlayerTurn = true;
            PlayerSwaped = false;
            base.Initialize();
        }

        public override void Update(GameTime gametime)
        {

            base.Update(gametime);
        }

        public void PlayerRound()
        {
            CPM.Attack(CPM.ATKScore, CEM.DEFScore);
            //Report damage
            PlayerTurn = false;
        }

        public void EnemyRound()
        {
            CheckProbablilityOfLoss();
            //OptomizeEnemySwap(); //Potentially build out
            if(BattleState == BattleState.Playing)
            {
                CEM.Attack(CEM.ATKScore, CPM.DEFScore);
            }
            
            //Report Damage
            PlayerTurn = true;

        }

        public void Round(bool Attack)
        {
            BattleState = BattleState.Playing;

            CheckLife();

            if (Attack)//If the player desides to attack
            {
                PlayerRound();
            }
            else
            {
                Run(player);
            }

            if(BattleState == BattleState.Playing) { EnemyRound(); }
           

        }

        public void CheckProbablilityOfLoss()
        {
            if (CPM.ATKScore > CEM.HP)//if the enemy will lose on the next round
            {
                //Attempt run
                Run(enemy);
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
            if (RunSucessRate > 60)
            {
                RunSucess(WhichCharacter);
            }
            //Inform on some outup of unsucessfull run attempt
        }

        public bool RunSucess(Character WhichCharacter)
        {
            //Inform on some outup of sucessfull run
            BattleState = BattleState.Forfit;
            if (WhichCharacter == enemy)
            {
                BattleState = BattleState.Forfit;
                PlayerDid = false;
                
            }
            if (WhichCharacter == player)
            {
                BattleState = BattleState.Forfit;
                PlayerDid = true;
            }
            return PlayerDid;
        }

        int PlayerCombinedHP, EnemyCombinedHP;
        public void CheckLife()
        {
            PlayerCombinedHP = player.CalculateTeamHP();
            EnemyCombinedHP = enemy.CalculateTeamHP();

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
            if (WhichCharacter == enemy)
            {
                //Enemy lost next battle would be opened in the game
            }
            if (WhichCharacter == player)
            {
                //Player lost next battle would not be open
            }
        }

        public void CheckCoinsLost()
        {
            //depends on difficulty
            if (GameDifficulty == GameDifficulty.Easy)
            {
                player.Coins -= (player.Coins / 3); //Looses a 1/3
            }
            if (GameDifficulty == GameDifficulty.Medium)
            {
                player.Coins -= ((player.Coins / 3) * 2); //Looses 2/3
            }
            if (GameDifficulty == GameDifficulty.Hard) //Looses all
            {
                player.Coins -= player.Coins;
            }
        }

        public void CheckCoinsGained()
        {
            //depends on difficulty
            if (GameDifficulty == GameDifficulty.Easy)
            {
                player.Coins += (enemy.Coins / 3); // Gains a 1/3 of the Enemies coins
            }
            if (GameDifficulty == GameDifficulty.Medium)
            {
                player.Coins += ((enemy.Coins / 3) * 2); //Gains 2/3 of the Enemies coins
            }
            if (GameDifficulty == GameDifficulty.Hard)
            {
                player.Coins += enemy.Coins; // Gains all of Enemies Coins
            }

        }

        public void MonsterSwap()
        {
            if (player.Team.Count == 1)
            {
                //They can't swap, they have no monster to swap with
                PlayerSwaped = true;
            }
            else if (player.Team.Count == 2)
            {
                if (input.KeyboardState.IsKeyDown(Keys.D1))
                {
                    CPM = player.Team[0];
                }
                if (input.KeyboardState.IsKeyDown(Keys.D2))
                {
                    CPM = player.Team[1];
                }
            }
            else
            {
                if (input.KeyboardState.IsKeyDown(Keys.D1))
                {
                    CPM = player.Team[0];
                }
                if (input.KeyboardState.IsKeyDown(Keys.D2))
                {
                    CPM = player.Team[1];
                }
                if (input.KeyboardState.IsKeyDown(Keys.D3))
                {
                    CPM = player.Team[2];
                }

            }
        }
    

    }
}
