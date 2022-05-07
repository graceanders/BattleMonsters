using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace BattleMonsters
{
    //TODO: CurrentMonsters Sprite dosne't adjust with Swaps

    public enum GameMode { PickStarter, InBattle, OutBattle, MonsterSwap, Healing, Raffel, ManageTeam }
    public enum GameState { Won, Playing, Lost }

    public enum GameDifficulty { Easy, Medium, Hard }

    public static class GamePrintout
    {
        public static string TxtPrintOut;

        public static void PrintToGameOutput(string output)
        {
            TxtPrintOut = output;
        }
    }

    public class GameManager : DrawableGameComponent
    {
        //Service Dependencies
        public static GameConsole console;

        Game g;
        InputHandler input;

        public GameMode GameMode { get; set; }
        public GameState GameState { get; set; }

        public GameDifficulty GameDifficulty { get; set; }

        public Player P;
        public Enemy E;
        //public Creature CPM, CEM;


        public Vector2 PMLocation, EMLocation;

        //Text Draw Locations
        Vector2 P_TextLocation, PM_HPLocation, E_TextLocation, EM_HPLocation;
        Vector2 RoundtxtLoc, TurntxtLoc, CointxtLoc;


        SpriteFont font;
        SpriteFont bigfont;

        SpriteBatch sb;

        Color PickStarterElement, BattleElement, OutOfBattleElement, HealElement, AlwaysShow;

        public int Round;

        MonsterManager mm;
        HealManager hm;
        RaffleManager rm;

        BattleManager CurrentBattle;

        //bool PickedStarter;

        public GameManager(Game game) : base(game)
        {
            input = (InputHandler)game.Services.GetService(typeof(IInputHandler));

            //Lazy load GameConsole
            console = (GameConsole)this.Game.Services.GetService(typeof(IGameConsole));
            if (console == null) //ohh no no console make a new one and add it to the game
            {
                console = new GameConsole(this.Game);
                this.Game.Components.Add(console);  //add a new game console to Game
            }

            GameState = GameState.Playing;

            P = new Player();
            E = new Enemy();


            mm = new MonsterManager(game, P, E);
            hm = new HealManager(game, P);
            game.Components.Add(mm);
            game.Components.Add(hm);


            g = game;
        }

        //TODO: Vecor Values should be bassed off of screen width and height

        int margin = 50;
        protected override void LoadContent()
        {
            sb = new SpriteBatch(this.Game.GraphicsDevice);
            font = this.Game.Content.Load<SpriteFont>("Font");
            bigfont = this.Game.Content.Load<SpriteFont>("BigFont");

            GamePrintout.TxtPrintOut = "Welcome to Battle Monsters!";

            AlwaysShow = Color.White;

            PMLocation = new Vector2(margin, g.GraphicsDevice.Viewport.Height - 400);
            EMLocation = new Vector2((g.GraphicsDevice.Viewport.Width - mm.Starter1Texture.Width) - margin, g.GraphicsDevice.Viewport.Height - 400);

            Round = 1;
            
            GameMode = GameMode.PickStarter;

            #region Text Draw Set
            P_TextLocation = new Vector2(20, 20);
            PM_HPLocation = new Vector2(20, 80);
            E_TextLocation = new Vector2(g.GraphicsDevice.Viewport.Width - 250, 20);
            EM_HPLocation = new Vector2(g.GraphicsDevice.Viewport.Width - 250, 80);

            RoundtxtLoc = new Vector2((g.GraphicsDevice.Viewport.Width / 2) - 50, 20);
            TurntxtLoc = new Vector2(RoundtxtLoc.X, (RoundtxtLoc.Y + 50));
            CointxtLoc = new Vector2((g.GraphicsDevice.Viewport.Width - 150), 1025);
            #endregion

#if TESTING
            SetUpTestingValues();
#endif

            base.LoadContent();
        }

        void SetUpTestingValues()
        {
            P.Coins = 100;

            //Healing
            Creature HealableMonster;
            HealableMonster = mm.Monsters[7];
            HealableMonster.GetStats(Round);
            P.DeadMonsters.Add(HealableMonster);

            HealableMonster = mm.Monsters[12];
            HealableMonster.GetStats(Round);
            P.DeadMonsters.Add(HealableMonster);

            //Full Team
            Creature AddToTeam;
            AddToTeam = mm.WaterJelly;
            AddToTeam.GetStats(Round);
            P.Team.Add(AddToTeam);

            AddToTeam = mm.EarthBird;
            AddToTeam.GetStats(Round);
            P.Team.Add(AddToTeam);
        }

        int CalculateHeightMargine(Creature C) { return (C.spriteTexture.Height / 2); }
        int CalculateWidthMargine(Creature C) { return (C.spriteTexture.Width / 2); }

        public override void Update(GameTime gameTime)
        {
            CheckGameMode();
            CheckGameState();
            CheckGameDifficulty();
            if(CurrentBattle != null) { CheckBattleState(); }

            UpdateGameInfoTxt();

            HandleInput(gameTime);

            base.Update(gameTime);
        }


        #region State Changes
        public void CheckGameMode()
        {
            switch (this.GameMode)
            {
                case GameMode.PickStarter:
                    PickStarterElement = Color.White;
                    ButtonGuideTxt = "1: Fire Starter | 2: Water Starter | 3: Earth Starter";//Not showing up
                    GamePrintout.TxtPrintOut = "Which Starter would you like to pick?";
                    if(mm.PickedStarter == true)
                    {
                        PickStarterElement = Color.Transparent;
                        GameMode = GameMode.OutBattle;
                        BattleReady();
                    }
                    break;
                case GameMode.InBattle:
                    BattleElement = Color.White;
                    OutOfBattleElement = Color.Transparent;
                    ButtonGuideTxt = "A: Attack | R: Run";
                    WhichBattle();
                    break;
                case GameMode.OutBattle:
                    OutOfBattleElement = Color.White;
                    BattleElement = Color.Transparent;
                    ButtonGuideTxt = "B: Battle | H: Heal | R: Raffle | T: Manage Team";
                    break;
                case GameMode.MonsterSwap:
                    //WhichBattle();
                    BattleElement = Color.White;
                    OutOfBattleElement = Color.Transparent;
                    ButtonGuideTxt = "L: Lock in Monster 1: Swap to First | 2: Swap to Second | 3: Swap to Third";
                    if (CurrentBattle.LockedIn) { GameMode = GameMode.InBattle;}
                    break;
                case GameMode.Healing:
                    HealElement = Color.White;
                    hm.HealMonster();
                    ButtonGuideTxt = "T: Heal This Monster | ->: Next Monster | E: Exit";

                    if (hm.GetExitHealing()) 
                    {
                        ExitMode(HealElement);
                    }
                    break;
                case GameMode.Raffel:
                    rm.RaffleElement = Color.White;
                    rm.MonsterRaffle();
                    ButtonGuideTxt = "P: Pull from Raffle | E: Exit";

                    if (rm.GetExitRaffle())
                    {
                        ExitMode(rm.RaffleElement);
                    }
                    break;
                case GameMode.ManageTeam:

                    break;
            }
        }

        public void CheckGameState()
        {
            switch (this.GameState)
            {
                case GameState.Won:
                    break;
                case GameState.Playing:
                    break;
                case GameState.Lost:
                    break;
            }
        }

        public void CheckGameDifficulty()
        {
            //How dose this change the game / dose it?
            switch (this.GameDifficulty)
            {
                case GameDifficulty.Easy:
                    break;
                case GameDifficulty.Medium:
                    break;
                case GameDifficulty.Hard:
                    break;
            }
        }
        #endregion

        public void WhichBattle()
        {
            if (Round == 1)
            {
                CurrentBattle = new BattleManager(g, P, E);
            }
            if (Round == 2)
            {
                //Set up other necesarry values
                CurrentBattle = new BattleManager(this.Game, this.P, this.E);
            }
            if (Round == 3)
            {
                //Set up other necesarry values
                CurrentBattle = new BattleManager(this.Game, this.P, this.E);
            }
            if (Round == 4)
            {
                //Set up other necesarry values
                CurrentBattle = new BattleManager(this.Game, this.P, this.E);
            }
            if (Round == 5)
            {
                //Set up other necesarry values
                CurrentBattle = new BattleManager(this.Game, this.P, this.E);
            }
            if (Round == 6)
            {
                //Set up other necesarry values
                CurrentBattle = new BattleManager(this.Game, this.P, this.E);
            }
            //is the game won id round = 7?
        }


        //TODO Should be in Battle Manager
        public void CheckBattleState()
        {
            switch (CurrentBattle.BattleState)
            {
                case BattleState.Playing:

                    break;
                case BattleState.Won:
                    Round++;
                    break;
                case BattleState.Lost:

                    break;
                case BattleState.Forfit:

                    break;
            }
        }

        int EasyDifficultyThreshold = 2;
        int MediumDifficultyThreshold = 4;
        int HardDifficultyThreshold = 6;
        public void UpdateDifficulty()
        {
            if (Round <= EasyDifficultyThreshold)
            {
                GameDifficulty = GameDifficulty.Easy;
            }
            else if (Round <= MediumDifficultyThreshold)
            {
                GameDifficulty = GameDifficulty.Medium;
            }
            else if (Round <= HardDifficultyThreshold)
            {
                GameDifficulty = GameDifficulty.Hard;
            }
        }

        void ExitMode(Color color)
        {
            GameMode = GameMode.OutBattle;
            color = Color.Transparent;
            GamePrintout.TxtPrintOut = "";
        }

        //TODO: will need testing but I think Raffle is pretty solid!!
        #region Raffle

        

        #endregion
        void BattleReady()
        {
            P.CurrentMonster.Location = PMLocation;
            P.CurrentMonster.DrawColor = BattleElement;
            P.CurrentMonster.GetStats(Round);
            g.Components.Add(P.CurrentMonster);

            E.CurrentMonster.Location = EMLocation;
            E.CurrentMonster.DrawColor = BattleElement;
            E.CurrentMonster.GetStats(Round);
            g.Components.Add(E.CurrentMonster);

        }

        #region Input

        public void HandleInput(GameTime gameTime)
        {
            //Controls differ whether the player is in battle
            //Don't want controls to work in the other game mode
            if(mm.PickedStarter == false) { mm.PickStarter(); }

            if (GameMode == GameMode.OutBattle)
            {
                if (input.KeyboardState.WasKeyPressed(Keys.B))
                {
                    //Battle
                    GamePrintout.TxtPrintOut = $"Round {Round} shall commense!";
                    if(P.Team.Count == 0)
                    {
                        P.Team.Add(P.CurrentMonster);
                        E.Team.Add(E.CurrentMonster);
                    }
                    GameMode = GameMode.MonsterSwap;
                }
                if (input.KeyboardState.WasKeyPressed(Keys.H))
                {
                    hm.MonsterHealed = false;
                    hm.ExitHealing = false;
                    GamePrintout.TxtPrintOut = "You have select to Heal your Monsters!";
                    GameMode = GameMode.Healing;
                }
                if (input.KeyboardState.WasKeyPressed(Keys.R))
                {
                    rm = new RaffleManager(g, P, mm, Round);
                    rm.PullFromRaffel = false;
                    rm.ExitRaffle = false;
                    GameMode = GameMode.Raffel;
                }
                if (input.KeyboardState.WasKeyPressed(Keys.T))
                {
                    //Manage team
                    GamePrintout.TxtPrintOut = "Lets manage your Team!";
                    GameMode = GameMode.ManageTeam;
                }

            }
            if (GameMode == GameMode.InBattle)
            {
                CurrentBattle.InBattleInput();
            }
            if(GameMode == GameMode.MonsterSwap)
            {
                //Dont love this 
                if(CurrentBattle == null) { WhichBattle(); }
                CurrentBattle.MonsterSwapInput();
            }
            if(GameMode == GameMode.Healing)
            {
                hm.HealInput();
            }
            if(GameMode == GameMode.Raffel)
            {
                rm.RaffleInput();
            }
        }

        string GameUpdateTxt;
        void UpdateGameInfoTxt()
        {
            GameUpdateTxt = GamePrintout.TxtPrintOut;
        }

        int CalculateStringWidth(string s)//From what I read this is a very taxing function but I was unable to find a better alternative
        {
            if(s != null)
            {
                Vector2 size = font.MeasureString(s);
                return (int)size.X;
            }

            return 0;
            
        }

        Vector2 ButtonGuidLoc = new Vector2(20, 1025);
        Vector2 GameInfoLoc;
        string ButtonGuideTxt;
        public override void Draw(GameTime gameTime)
        {
            sb.Begin();

            sb.DrawString(bigfont, $"Round: {Round}", RoundtxtLoc, AlwaysShow);
            sb.DrawString(bigfont, $"Coins: {P.Coins}", CointxtLoc, AlwaysShow);
            sb.DrawString(bigfont, $"Player: {P.Name}", P_TextLocation, AlwaysShow);

            #region Display Team
            if (P.Team.Count == 0) 
            { sb.Draw(mm.EmptySpot, mm.TeamOneLoc, AlwaysShow); }
            else if(P.Team.Count >= 1) 
            { sb.Draw(P.Team[0].spriteTexture, mm.TeamOneLoc, AlwaysShow); }

            if(P.Team.Count < 2) 
            { sb.Draw(mm.EmptySpot, mm.TeamTwoLoc, AlwaysShow); }
            else if(P.Team.Count >= 2)
            { sb.Draw(P.Team[1].spriteTexture, mm.TeamTwoLoc, AlwaysShow); }

            if(P.Team.Count < 3) 
            { sb.Draw(mm.EmptySpot, mm.TeamThreeLoc, AlwaysShow); }
            else { sb.Draw(P.Team[2].spriteTexture, mm.TeamThreeLoc, AlwaysShow); }
            #endregion

            if (P.CurrentMonster != null)
            {
                //Player
                sb.DrawString(font, $"{P.CurrentMonster.Name}'s HP: {P.CurrentMonster.HP}/{P.CurrentMonster.HPMax}\n\nStats:\nATK Score: {P.CurrentMonster.ATKScore}\nDEF Score: {P.CurrentMonster.DEFScore}", PM_HPLocation, BattleElement);

                //Enemy
                sb.DrawString(bigfont, $"Enemy: {E.Name}", E_TextLocation, BattleElement);
                sb.DrawString(font, $"{E.CurrentMonster.Name}'s HP: {E.CurrentMonster.HP}/{E.CurrentMonster.HPMax}\n\nStats:\nATK Score: {E.CurrentMonster.ATKScore}\nDEF Score: {E.CurrentMonster.DEFScore}", EM_HPLocation, BattleElement);
            }

            //Starter
            sb.Draw(mm.Starter1Texture, new Rectangle((int)mm.Starter1.Location.X, (int)mm.Starter1.Location.Y, mm.Starter1.spriteTexture.Width, mm.Starter1.spriteTexture.Height), PickStarterElement);
            sb.Draw(mm.Starter2Texture, new Rectangle((int)mm.Starter2.Location.X, (int)mm.Starter2.Location.Y, mm.Starter2.spriteTexture.Width, mm.Starter2.spriteTexture.Height), PickStarterElement);
            sb.Draw(mm.Starter3Texture, new Rectangle((int)mm.Starter3.Location.X, (int)mm.Starter3.Location.Y, mm.Starter3.spriteTexture.Width, mm.Starter3.spriteTexture.Height), PickStarterElement);

            if (CurrentBattle != null)
            {
                sb.DrawString(font, $"Turn: {CurrentBattle.Turn}", TurntxtLoc, BattleElement);
            }

            #region Game Info Text Info
            if(GameUpdateTxt != null)
            {
                GameInfoLoc = new Vector2((g.GraphicsDevice.Viewport.Width / 2) - (CalculateStringWidth(GameUpdateTxt)/ 2), 100);
                sb.DrawString(font, GameUpdateTxt, GameInfoLoc, Color.LightGray);
            }
            #endregion

            #region ButtonGuide
            if(GameMode == GameMode.PickStarter) { sb.DrawString(font, ButtonGuideTxt, ButtonGuidLoc, PickStarterElement); }
            if (GameMode == GameMode.OutBattle) { sb.DrawString(font, ButtonGuideTxt, ButtonGuidLoc, OutOfBattleElement); }
            if (GameMode == GameMode.InBattle) { sb.DrawString(font, ButtonGuideTxt, ButtonGuidLoc, BattleElement); }
            if(GameMode == GameMode.MonsterSwap) { sb.DrawString(font, ButtonGuideTxt, ButtonGuidLoc, BattleElement); }
            if(GameMode == GameMode.Healing) { sb.DrawString(font, ButtonGuideTxt, ButtonGuidLoc, HealElement); }
            if (GameMode == GameMode.Raffel) 
            { 
                sb.DrawString(font, ButtonGuideTxt, ButtonGuidLoc, rm.RaffleElement);
                if (rm.PulledMonsterSprite == null) { sb.Draw(rm.WhatMonster, rm.MonsterPulledLoc, rm.RaffleElement); }
                else { sb.Draw(rm.PulledMonsterSprite, rm.MonsterPulledLoc, rm.RaffleElement); }

            }
            #endregion

            sb.End();

            base.Draw(gameTime);
        }

    }
    #endregion
}
