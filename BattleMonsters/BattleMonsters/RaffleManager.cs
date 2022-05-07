using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace BattleMonsters
{
    public class RaffleManager : DrawableGameComponent
    {
        InputHandler input;
        Player P;

        MonsterManager mm;
        int Round;

        Random random;

        public Texture2D PulledMonsterSprite, WhatMonster;
        public Rectangle MonsterPulledLoc;

        public RaffleManager(Game game, Player p, MonsterManager MM, int round) : base(game)
        {
            input = (InputHandler)game.Services.GetService(typeof(IInputHandler));
            P = p;

            mm = MM;
            Round = round;

            random = new Random();
            WhatMonster = game.Content.Load<Texture2D>("What");
            MonsterPulledLoc = new Rectangle(game.GraphicsDevice.Viewport.Width / 4, 100, 250, 250);
        }

        protected override void LoadContent()
        {
            base.LoadContent();
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        int RaffleCost = 20;
        public bool PullFromRaffel;
        Creature MonsterPulled;
        public void MonsterRaffle()
        {
            if (!PullFromRaffel)
            {
                GamePrintout.TxtPrintOut = "Welcome to the Raffle!\n";
                if (P.Coins >= RaffleCost)
                {
                    GamePrintout.TxtPrintOut += "You have enough Coins to pull from the Raffle";
                }
                else { GamePrintout.TxtPrintOut += $"You do not have enough to participate in the Raffle come again when you have {RaffleCost} Coins"; }
            }

        }

        void PullMonster()
        {
            PullFromRaffel = true;

            P.Coins -= RaffleCost;
            int WhichMonster = random.Next(0, mm.Monsters.Count);//Sets a Random between the bounds of the Monters one can get
            MonsterPulled = mm.Monsters[WhichMonster];//Sets the Monster gotten locally
            MonsterPulled.GetStats(Round);//Gets there Stats

            PulledMonsterSprite = MonsterPulled.spriteTexture;

            GamePrintout.TxtPrintOut = $"You pulled {MonsterPulled.Name}\nStats:\nHP: {MonsterPulled.HP} | ATK Score: {MonsterPulled.ATKScore} | DEF: {MonsterPulled.DEFScore}";

            P.AddMonsterToAllMonsters(MonsterPulled);
        }

        public void RaffleInput()
        {
            if (input.KeyboardState.WasKeyPressed(Keys.P))
            {
                PullMonster();
            }
            if (input.KeyboardState.WasKeyPressed(Keys.E))
            {
                ExitRaffle = true;
            }
        }

        public bool ExitRaffle = false;
        public bool GetExitRaffle() { return ExitRaffle; }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }
    }
}
