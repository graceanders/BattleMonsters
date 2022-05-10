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
            this.Coins = 20;
        }

        public void CalculateLevelAndCoins(int Round)
        {
            //this.Level = CalculateLevel();
            Coins *= Round;
        }
    }
}
