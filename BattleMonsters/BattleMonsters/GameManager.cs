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
        Vector2 RoundtxtLoc;

        SpriteFont font;
        SpriteFont bigfont;

        SpriteBatch sb;

        Color PickStarterElement, BattleElement, OutOfBattleElement, AlwaysShow;

        public int Round;

        MonsterManager mm;

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

            mm = new MonsterManager(game);
            game.Components.Add(mm);

            g = game;
        }

        //TODO: Move Starter values and responsobilites to MonsterManager
        Creature Starter1, Starter2, Starter3;
        protected override void LoadContent()
        {
            sb = new SpriteBatch(this.Game.GraphicsDevice);
            font = this.Game.Content.Load<SpriteFont>("Font");
            bigfont = this.Game.Content.Load<SpriteFont>("BigFont");

            GamePrintout.TxtPrintOut = "Welcome to Battle Monsters!";

            P = new Player();
            E = new Enemy();

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
            E_TextLocation = new Vector2(g.GraphicsDevice.Viewport.Width - 150, 20);
            EM_HPLocation = new Vector2(g.GraphicsDevice.Viewport.Width - 150, 80);

            RoundtxtLoc = new Vector2((g.GraphicsDevice.Viewport.Width / 2) - 50, 20);
            #endregion

            base.LoadContent();
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

        bool AddMonstersToTeam = false;
        public void WhichBattle()
        {
            if (Round == 1)
            {
                AddMonstersToTeams();

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

        void AddMonstersToTeams()
        {
            if (!AddMonstersToTeam)
            {
                P.AddMonsterToTeam(P.CurrentMonster);
                E.AddMonsterToTeam(E.CurrentMonster);
            }
            AddMonstersToTeam = true;
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

        //TODO: will need texting but I think Raffle is pretty solid!!
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

        //TODO: Get Input Working for Healing

        #region Healing

        int HealCost = 10;
        int NeedHeal;
        Creature HealedMonster;
        public void HealMonster()//I'd like to have the player pick which monster they heal but I'm not sure how to get the input to corispond
        {
            GamePrintout.TxtPrintOut = "Welcome to the Healing Station!\n";

            foreach (Creature monster in P.DeadMonsters)
            {
                NeedHeal++;
            }

            if (NeedHeal == 0)
            {
                GamePrintout.TxtPrintOut += "You have 0 Monsters that need healing";
            }
            else
            {
                if(P.Coins >= HealCost)//Can heal at least one Monster
                {
                    PickWhichToHeal();
                }
                else
                {
                    GamePrintout.TxtPrintOut += $"You do not have enough to heal any of your Monsters\nYou have {P.Coins}, you need {HealCost}";
                }
            }

        }

        //TODO: Instead of displaying all show 5 and allow them to go to the next "row" or next Page
        bool Heal1, Heal2, Heal3, Heal4, Heal5;
        public void Heal(Creature c)
        {
            P.Coins -= HealCost;

            P.DeadMonsters.Remove(P.DeadMonsters[0]);//Remove the first monster from DeadMonsters
            HealedMonster = P.DeadMonsters[0];//Set it locally
            P.AllMonsters.Add(HealedMonster);//Add to AllMonsters
        }

        int ToChooseFrom;
        void PickWhichToHeal()
        {
            GamePrintout.TxtPrintOut += "Which Monster would you like the Heal?";
            //This is going to be complicated -__-

            //Caps at 5
            if (NeedHeal > 5) { ToChooseFrom = 5; }
            else { ToChooseFrom = P.DeadMonsters.Count; }

            for (int i = 0; i < ToChooseFrom; i++)
            {
                int Number = i++;
                GamePrintout.TxtPrintOut += $"\n{Number}: {P.DeadMonsters[i]}";
                //getting the input to work for this is gonna be a mess
            }
            GamePrintout.TxtPrintOut += "\nClick the number that corisponds with the Monster you'd like to heal";
        }
        #endregion

        #region Input

        //TODO Currently just giving the player a starter allow them to pick
        #region Pick Starter

        /* Starter Guide
              Starter 1 = Fire
              Starter 2 = Water
              Starter 3 = Earth
             */

        public void PickStarter()
        {
            if (input.KeyboardState.IsKeyDown(Keys.D1))
            {
                ThisStarter(mm.FireStarter);
            }
            if (input.KeyboardState.IsKeyDown(Keys.D2))
            {
                ThisStarter(mm.WaterStarter);
            }
            if (input.KeyboardState.IsKeyDown(Keys.D3))
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
                if (input.KeyboardState.IsKeyDown(Keys.B))
                {
                    //Battle
                    GamePrintout.TxtPrintOut = $"Round {Round} shall commense!";
                    GameMode = GameMode.MonsterSwap;
                }
                if (input.KeyboardState.IsKeyDown(Keys.H))
                {
                    GamePrintout.TxtPrintOut = "You have select to Heal your Monsters!";
                    GameMode = GameMode.Healing;
                }
                if (input.KeyboardState.IsKeyDown(Keys.T))
                {
                    //Manage team
                    GamePrintout.TxtPrintOut = "Lets manage your Team!";
                    GameMode = GameMode.ManageTeam;
                }

            }
            if (GameMode == GameMode.InBattle)
            {
                InBattleInput();
            }
            if(GameMode == GameMode.MonsterSwap)
            {
                MonsterSwapInput();
            }
            if(GameMode == GameMode.Healing)
            {
                HealInput();
            }
        }


        //TODO: Move to InBattle and MonsterSwap t0 Battle Manager
        void InBattleInput()
        {
            if (input.KeyboardState.IsKeyDown(Keys.A))
            {
                CurrentBattle.Round(true);
                //Attack
            }
            if (input.KeyboardState.IsKeyDown(Keys.R))
            {
                CurrentBattle.Round(false);
                //Retreat
            }
        }

        void MonsterSwapInput()
        {
            //Switches which monster the user is using at the moment
            //Probally create a bool value for is the player is attacking/ being attacked so they cant switch mid that
            if (input.KeyboardState.IsKeyDown(Keys.D1))
            {
                P.CurrentMonster = P.Team[0];
                GamePrintout.TxtPrintOut = "You have Swapped to your 1st Monster!\nLock in to begin!";
            }
            if (input.KeyboardState.IsKeyDown(Keys.D2))
            {
                if (P.Team[1] == null)//No 2nd Monster
                {
                    GamePrintout.TxtPrintOut = "You only have 1 Monster in your team and cannot swap\nLock in to begin!";
                }
                else
                {
                    P.CurrentMonster = P.Team[1];
                    GamePrintout.TxtPrintOut = "You have Swapped to your 2st Monster!\nLock in to begin!";
                }
            }
            if (input.KeyboardState.IsKeyDown(Keys.D3))
            {
                if (P.Team[2] == null)//No 3rd
                {
                    if (P.Team[1] == null)
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
            if (input.KeyboardState.IsKeyDown(Keys.L))
            {
                GameMode = GameMode.InBattle;
                GamePrintout.TxtPrintOut = $"You have selected {P.CurrentMonster.Name}\nThe Battle will commense!";
            }
        }
        //------------------------

        void HealInput()
        {
            //Currently only shows first 5

            //I hate this but i do not know a more efficiant way to do this
            if (NeedHeal == 1) { Heal1 = true; }
            if (NeedHeal == 2) { Heal2 = true; }
            if (NeedHeal == 3) { Heal3 = true; }
            if (NeedHeal == 4) { Heal4 = true; }
            if (NeedHeal == 5) { Heal5 = true; }

            if (Heal1)
            {
                if (input.KeyboardState.IsKeyDown(Keys.D1))
                {
                    Heal(P.DeadMonsters[0]);
                }
            }
            if (Heal2)
            {
                if (input.KeyboardState.IsKeyDown(Keys.D2))
                {
                    Heal(P.DeadMonsters[1]);
                }
            }
            if (Heal3)
            {
                if (input.KeyboardState.IsKeyDown(Keys.D3))
                {
                    Heal(P.DeadMonsters[2]);
                }
            }
            if (Heal4)
            {
                if (input.KeyboardState.IsKeyDown(Keys.D4))
                {
                    Heal(P.DeadMonsters[3]);
                }
            }
            if (Heal5)
            {
                if (input.KeyboardState.IsKeyDown(Keys.D5))
                {
                    Heal(P.DeadMonsters[4]);
                }
            }
        }
        #endregion

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

            if(P.CurrentMonster != null)
            {
                //Player
                sb.DrawString(bigfont, "Player", P_TextLocation, BattleElement);
                sb.DrawString(font, $"{P.CurrentMonster.Name}'s HP: {P.CurrentMonster.HP}\n\nStats:\nATK Score: {P.CurrentMonster.ATKScore}\nDEF Score: {P.CurrentMonster.DEFScore}", PM_HPLocation, BattleElement);

                //Enemy
                sb.DrawString(bigfont, "Enemy", E_TextLocation, BattleElement);
                sb.DrawString(font, $"{E.CurrentMonster.Name}'s HP: {E.CurrentMonster.HP}\n\nStats:\nATK Score: {E.CurrentMonster.ATKScore}\nDEF Score: {E.CurrentMonster.DEFScore}", EM_HPLocation, BattleElement);
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
            if(GameMode == GameMode.MonsterSwap) { sb.DrawString(font, ButtonGuideTxt, new Vector2((ButtonGuidLoc.X + 200), (ButtonGuidLoc.Y)), BattleElement); }
            #endregion

            sb.End();

            base.Draw(gameTime);
        }


    }

}
