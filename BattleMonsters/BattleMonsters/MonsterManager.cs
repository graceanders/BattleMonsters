using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace BattleMonsters
{
    public class MonsterManager : DrawableGameComponent
    {
        Game g;
        InputHandler input;

        public Creature
            EarthStarter,
            FireStarter,
            WaterStarter,
            EarthJelly,
            FireJelly,
            WaterJelly,
            EarthScorpion,
            FireScorpion,
            WaterScorpion,
            EarthBird,
            FireBird,
            WaterBird,
            EarthDragon,
            FireDragon,
            WaterDragon;

        public List<Creature> Monsters;

        public bool PickedStarter;

        Player P;
        Enemy E;

        public MonsterManager (Game game, Player player, Enemy enemy) : base(game)
        {
            input = (InputHandler)game.Services.GetService(typeof(IInputHandler));

            P = player;
            E = enemy;

            Monsters = new List<Creature>();

            EarthStarter = new Creature(game, "Dust");
            FireStarter = new Creature(game, "Flick");
            WaterStarter = new Creature(game, "Drip");

            EarthJelly = new Creature(game, "Mud");
            FireJelly = new Creature(game, "Glow");
            WaterJelly = new Creature(game, "Droplet");

            EarthScorpion = new Creature(game, "Errupt");
            FireScorpion = new Creature(game, "Spark");
            WaterScorpion = new Creature(game, "Drown");

            EarthBird = new Creature(game, "Dust Storm");
            FireBird = new Creature(game, "Flame");
            WaterBird = new Creature(game, "Dive");

            EarthDragon = new Creature(game, "Earthquake");
            FireDragon = new Creature(game, "Wild Fire");
            WaterDragon = new Creature(game, "Tsunami");

            g = game;
        }

        public Creature Starter1, Starter2, Starter3;
        public Texture2D Starter1Texture, Starter2Texture, Starter3Texture;
        protected override void LoadContent()
        {

            #region Load All Monsters
            EarthStarter.spriteTexture = g.Content.Load<Texture2D>("EarthStarter");
            Monsters.Add(EarthStarter);
            FireStarter.spriteTexture = g.Content.Load<Texture2D>("FireStarter");
            Monsters.Add(FireStarter);
            WaterStarter.spriteTexture = g.Content.Load<Texture2D>("WaterStarter");
            Monsters.Add(WaterStarter);

            EarthJelly.spriteTexture = g.Content.Load<Texture2D>("EarthJelly");
            Monsters.Add(EarthJelly);
            FireJelly.spriteTexture = g.Content.Load<Texture2D>("FireJelly");
            Monsters.Add(FireJelly);
            WaterJelly.spriteTexture = g.Content.Load<Texture2D>("WaterJelly");
            Monsters.Add(WaterJelly);

            EarthScorpion.spriteTexture = g.Content.Load<Texture2D>("EarthScorpion");
            Monsters.Add(EarthScorpion);
            FireScorpion.spriteTexture = g.Content.Load<Texture2D>("FireScorpion");
            Monsters.Add(FireScorpion);
            WaterScorpion.spriteTexture = g.Content.Load<Texture2D>("WaterScorpion");
            Monsters.Add(WaterScorpion);

            EarthBird.spriteTexture = g.Content.Load<Texture2D>("EarthBird");
            Monsters.Add(EarthBird);
            FireBird.spriteTexture = g.Content.Load<Texture2D>("FireBird");
            Monsters.Add(FireBird);
            WaterBird.spriteTexture = g.Content.Load<Texture2D>("WaterBird");
            Monsters.Add(WaterBird);

            EarthDragon.spriteTexture = g.Content.Load<Texture2D>("EarthDragon");
            Monsters.Add(EarthDragon);
            FireDragon.spriteTexture = g.Content.Load<Texture2D>("FireDragon");
            Monsters.Add(FireDragon);
            WaterDragon.spriteTexture = g.Content.Load<Texture2D>("WaterDragon");
            Monsters.Add(WaterDragon);
            #endregion

            #region Load Starter
            Starter1 = FireStarter;
            Starter1.Location = new Vector2(((g.GraphicsDevice.Viewport.Width / 3) - (Starter1.spriteTexture.Width / 2)), 550);
            Starter1Texture = Starter1.spriteTexture;

            Starter2 = WaterStarter;
            Starter2.Location = new Vector2(((g.GraphicsDevice.Viewport.Width / 2) - (Starter2.spriteTexture.Width / 2)), 550);
            Starter2Texture = Starter2.spriteTexture;

            Starter3 = EarthStarter;
            Starter3.Location = new Vector2(1040, 550);
            Starter3Texture = Starter3.spriteTexture;

            PickedStarter = false;
            #endregion

            base.LoadContent();
        }


        /* 
           Starter 1 = Fire
           Starter 2 = Water
           Starter 3 = Earth
        */

        public void PickStarter()
        {
            if (input.KeyboardState.WasKeyPressed(Keys.D1))
            {
                ThisStarter(FireStarter);
            }
            if (input.KeyboardState.WasKeyPressed(Keys.D2))
            {
                ThisStarter(WaterStarter);
            }
            if (input.KeyboardState.WasKeyPressed(Keys.D3))
            {
                ThisStarter(EarthStarter);
            }
        }

        void ThisStarter(Creature c)
        {
            P.CurrentMonster = c;
            P.Team.Add(P.CurrentMonster);
            GamePrintout.TxtPrintOut = $"You have chosen {P.CurrentMonster.Name} as your starter!";

            //Enemy is at a type disadvantage
            if (c == FireStarter) { EnemyStarter(EarthStarter); }
            if (c == WaterStarter) { EnemyStarter(FireStarter); }
            if (c == EarthStarter) { EnemyStarter(WaterStarter); }

            PickedStarter = true;
        }

        void EnemyStarter(Creature c)
        {
            E.CurrentMonster = c;
            E.Team.Add(E.CurrentMonster);
            Starter2.DrawColor = Color.Transparent;//If this is not here the unpicked stays visible
        }

    }
}
