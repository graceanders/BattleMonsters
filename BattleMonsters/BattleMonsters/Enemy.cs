using System;
using System.Collections.Generic;
using System.Text;

namespace BattleMonsters
{
    public class Enemy : Character
    {
        string[] EnemyNames = new string[] { "Kiara", "Alastor", "Cessair", "Snake", "Drift", "Apex", "Blitz", 
        "Pitfall", "Hex" };
        Random random;

        public Enemy()
        {
            random = new Random();
            int WhichName = random.Next(0, EnemyNames.Length);
            this.Name = EnemyNames[WhichName];
            this.CombindedATK = CalculateCombindedATK();
            this.Level = CalculateLevel();

            this.Coins = CalculateCoins(Level);
        }

        int CoinPerLevel = 20;
        int value;
        public int CalculateCoins(int level)
        {
            value = CoinPerLevel * level;
            return value;
        }
    }
}
