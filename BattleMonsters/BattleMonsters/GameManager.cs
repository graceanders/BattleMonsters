using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace BattleMonsters
{
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

        BattleManager CurrentBattle;

        Random random;

        bool PickedStarter;

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

            mm = new MonsterManager(game);
            hm = new HealManager(game, P);
            game.Components.Add(mm);
            game.Components.Add(hm);


            g = game;
        }

        //TODO: Move Starter values and responsobilites to MonsterManager
        //TODO: Vecor Values should be bassed off of screen width and height
        Creature Starter1, Starter2, Starter3;
        protected override void LoadContent()
        {
            sb = new SpriteBatch(this.Game.GraphicsDevice);
            font = this.Game.Content.Load<SpriteFont>("Font");
            bigfont = this.Game.Content.Load<SpriteFont>("BigFont");

            GamePrintout.TxtPrintOut = "Welcome to Battle Monsters!";
            

            AlwaysShow = Color.White;

            //Starter
            Starter1 = mm.FireStarter;
            Starter1.DrawColor = PickStarterElement;
            Starter1.Location = new Vector2(((g.GraphicsDevice.Viewport.Width / 3) - (Starter1.spriteTexture.Width / 2)), 550);

            Starter2 = mm.WaterStarter;
            Starter2.DrawColor = PickStarterElement;
            Starter2.Location = new Vector2(((g.GraphicsDevice.Viewport.Width / 2) - (Starter2.spriteTexture.Width / 2)), 550);

            Starter3 = mm.EarthStarter;
            Starter3.DrawColor = PickStarterElement;
            Starter3.Location = new Vector2(1040, 550);


            //TODO: Dont have these by componenets, just draw out 3 images of the starters
            //^ this is necessary so that CurrentMonster can be Components which need to be
            g.Components.Add(Starter1);
            g.Components.Add(Starter2);
            g.Components.Add(Starter3);

            PMLocation = new Vector2(0, 550);
            EMLocation = new Vector2(1450, 550);

            Round = 1;

            PickedStarter = false;
            GameMode = GameMode.PickStarter;

            random = new Random();

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
                    ButtonGuideTxt = "B: Battle | H: Heal | T: Manage Team";
                    break;
                case GameMode.MonsterSwap:
                    BattleElement = Color.White;
                    OutOfBattleElement = Color.Transparent;
                    ButtonGuideTxt = "L: Lock in Monster 1: Swap to First | 2: Swap to Second | 3: Swap to Third";
                    break;
                case GameMode.Healing:
                    HealElement = Color.White;
                    hm.HealMonster();
                    ButtonGuideTxt = "T: Heal This Monster | ->: Next Monster | E: Exit";

                    if (hm.GetExitHealing()) 
                    { 
                        GameMode = GameMode.OutBattle;
                        HealElement = Color.Transparent;
                        GamePrintout.TxtPrintOut = "";//Clear
                    }
                    break;
                case GameMode.Raffel:

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

        //TODO: will need testing but I think Raffle is pretty solid!!
        #region Raffle

        int RaffleCost = 20;
        bool PullFromRaffel;
        Creature MonsterPulled;
        public void MonsterRaffle()
        {
            GamePrintout.TxtPrintOut = "Welcome to the Raffle!\n";
            if (P.Coins >= RaffleCost)
            {
                GamePrintout.TxtPrintOut += "Would you like to pull from the Raffle?\n1: Yes or 2: No";
                if (PullFromRaffel)
                {
                    PullMonster();
                }
                else { GamePrintout.TxtPrintOut = "Not a problem come again soon!"; }
            }
            else { GamePrintout.TxtPrintOut += $"You do not have enough to participate in the Raffle come again when you have {RaffleCost} Coins"; }
        }

        void PullMonster()
        {
            P.Coins -= RaffleCost;
            int WhichMonster = random.Next(0, mm.Monsters.Count);//Sets a Random between the bounds of the Monters one can get
            MonsterPulled = mm.Monsters[WhichMonster];//Sets the Monster gotten locally
            MonsterPulled.GetStats(Round);//Gets there Stats

            GamePrintout.TxtPrintOut = $"You pulled a {nameof(MonsterPulled)} named {MonsterPulled.Name}\nStats:\nHP: {MonsterPulled.HP} | ATK Score: {MonsterPulled.ATKScore} | DEF: {MonsterPulled.DEFScore}";

            P.AddMonsterToAllMonsters(MonsterPulled);
        }

        #endregion


        #region Input

        #region Pick Starter

        /* Starter Guide
              Starter 1 = Fire
              Starter 2 = Water
              Starter 3 = Earth
             */

        public void PickStarter()
        {
            if (input.KeyboardState.WasKeyPressed(Keys.D1))
            {
                ThisStarter(mm.FireStarter);
            }
            if (input.KeyboardState.WasKeyPressed(Keys.D2))
            {
                ThisStarter(mm.WaterStarter);
            }
            if (input.KeyboardState.WasKeyPressed(Keys.D3))
            {
                ThisStarter(mm.EarthStarter);
            }
        }

        void ThisStarter(Creature c)
        {
            P.CurrentMonster = c;
            GamePrintout.TxtPrintOut = $"You have chosen {P.CurrentMonster.Name} as your starter!";

            //Enemy is at a type disadvantage
            if ( c == mm.FireStarter) 
            {
                E.CurrentMonster = mm.EarthStarter;
                Starter2.DrawColor = Color.Transparent;//If this is not here the unpicked stays visible
            }
            if (c == mm.WaterStarter) 
            {
                E.CurrentMonster = mm.FireStarter;
                Starter3.DrawColor = Color.Transparent;
            }
            if (c == mm.EarthStarter) 
            {
                E.CurrentMonster = mm.WaterStarter;
                Starter1.DrawColor = Color.Transparent;
            }

            PickedStarter = true;
            PickStarterElement = Color.Transparent;
            GameMode = GameMode.OutBattle;
            BattleReady();
        }

        void BattleReady()
        {
            P.CurrentMonster.Location = PMLocation;
            P.CurrentMonster.DrawColor = BattleElement;
            P.CurrentMonster.GetStats(Round);

            E.CurrentMonster.Location = EMLocation;
            E.CurrentMonster.DrawColor = BattleElement;
            E.CurrentMonster.GetStats(Round);

        }
        #endregion

        public void HandleInput(GameTime gameTime)
        {
            //Controls differ whether the player is in battle
            //Don't want controls to work in the other game mode
            if(PickedStarter == false) { PickStarter(); }

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
                MonsterSwapInput();
            }
            if(GameMode == GameMode.Healing)
            {
                hm.HealInput();
            }
        }


        //TODO: Move to InBattle and MonsterSwap t0 Battle Manager
        

        void MonsterSwapInput()
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
                GameMode = GameMode.InBattle;
                GamePrintout.TxtPrintOut = $"You have selected {P.CurrentMonster.Name}\nThe Battle will commense!";
            }
        }
        //------------------------

        

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

            if(P.CurrentMonster != null)
            {
                //Player
                sb.DrawString(bigfont, $"Player: {P.Name}", P_TextLocation, BattleElement);
                sb.DrawString(font, $"{P.CurrentMonster.Name}'s HP: {P.CurrentMonster.HP}/{P.CurrentMonster.HPMax}\n\nStats:\nATK Score: {P.CurrentMonster.ATKScore}\nDEF Score: {P.CurrentMonster.DEFScore}", PM_HPLocation, BattleElement);

                //Enemy
                sb.DrawString(bigfont, $"Enemy: {E.Name}", E_TextLocation, BattleElement);
                sb.DrawString(font, $"{E.CurrentMonster.Name}'s HP: {E.CurrentMonster.HP}/{E.CurrentMonster.HPMax}\n\nStats:\nATK Score: {E.CurrentMonster.ATKScore}\nDEF Score: {E.CurrentMonster.DEFScore}", EM_HPLocation, BattleElement);
            }

            if(CurrentBattle != null)
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
            #endregion

            sb.End();

            base.Draw(gameTime);
        }

    }
    #endregion
}
