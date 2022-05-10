using MonoGameLibrary.Sprite;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace BattleMonsters
{
    public class Creature : DrawableSprite, IAttack, IDefend
    {
        //TODO: Build Up Type Advantage
        public string Name { get; set; }
        public int HP { get; set; }
        public int HPMax { get; set; }
        public int ATKScore { get; set; }
        public int DEFScore { get; set; }

        Random stats;

        public bool Dead, Ran;

        public Creature(Game game) : base(game) { }

        public Creature (Game game, string name) : base(game)
        {
            this.Name = name;
        }


        int MinHP = 10;
        int MaxHP = 20;

        int MinATKScore = 5;
        int MaxATKScore = 15;

        int MinDEFScore = 5;
        int MaxDEFScore = 15;

        public void GetStats(int Round)
        {
            stats = new Random();

            int hp = stats.Next((MinHP * Round), (MaxHP * Round));
            this.HP = hp;
            this.HPMax = hp;

            int atk = stats.Next((MinATKScore * Round), (MaxATKScore * Round));
            this.ATKScore = atk;

            int def = stats.Next((MinDEFScore * Round), (MaxDEFScore * Round));
            this.DEFScore = def;
        }

        double Damage;
        public double Attack(int AttackerATK, int DefenderDEF)
        {
            Damage = AttackerATK * Defence(DefenderDEF, AttackerATK);

            return Damage;
        }

        double DamagePercentage;
        public double Defence(int DefenderDEF, int AtackerATK)
        {
            if (DefenderDEF > AtackerATK)
            {
                //Midigates a percentage of damage
                DamagePercentage = .9;
            }
            else if (DefenderDEF == AtackerATK)
            {
                //Normal Damage
                DamagePercentage = 1;
            }
            else if (DefenderDEF < AtackerATK)
            {
                //Attacker dose more damage
                DamagePercentage = 1.1;
            }

            return DamagePercentage;
        }


        public string DisplayStats()
        {
            return $"Stats:\nHp: {this.HP}/{this.HPMax}\nATK Score: {this.ATKScore}\nDEF Score: {this.DEFScore}";
        }
    }
}
