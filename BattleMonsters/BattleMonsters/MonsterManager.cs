using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace BattleMonsters
{
    class MonsterManager : DrawableGameComponent
    {
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

        Game g;

        public MonsterManager (Game game) : base(game)
        {
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

        protected override void LoadContent()
        {
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

            base.LoadContent();
        }
    }
}
