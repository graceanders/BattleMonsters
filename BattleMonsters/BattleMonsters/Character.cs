using System;
using System.Collections.Generic;
using System.Text;

namespace BattleMonsters
{
    public class Character
    {
        public string Name;
        public int Coins;//Player colects coins from Enemy
        public int Level;

        public List<Creature> Team = new List<Creature>();

        public int CombindedATK;
        public int TeamHP;

        int Calculated;

        public Creature CurrentMonster;

        public int CalculateCombindedATK()
        {
            foreach (Creature monster in Team)
            {
                Calculated += monster.ATKScore;
            }

            if (Team.Count == 0) { Calculated = 0; }

            return Calculated;
        }

        public int CalculateTeamHP()
        {
            TeamHP = 0;
            foreach(Creature monster in Team)
            {
                TeamHP += monster.HP;
            }

            return TeamHP;
        }

        //TBD
        int Lvl1Threshold = 10;
        int Lvl2Threshold = 20;
        int Lvl3Threshold = 30;
        int Lvl4Threshold = 40;
        int Lvl5Threshold = 50;

        public int CalculateLevel()//Current Max Level 5
        {
            //Level increases as combined Team ATK score crosses certin thresholds
            if (this.CombindedATK >= Lvl1Threshold) 
            {
                this.Level = 1;
                Calculated = 1; 
            }
            if (this.CombindedATK >= Lvl2Threshold) 
            {
                this.Level = 1;
                Calculated = 2; 
            }
            if (this.CombindedATK >= Lvl3Threshold) 
            { 
                Calculated = 3;
                this.Level = 3;
            }
            if (this.CombindedATK >= Lvl4Threshold) 
            { 
                Calculated = 4;
                this.Level = 4;
            }
            if (this.CombindedATK >= Lvl5Threshold) 
            { 
                Calculated = 5;
                this.Level = 5;
            }

            return Calculated;
        }

        public void AddMonsterToTeam(Creature c)
        {
            this.Team.Add(c);
        }

        

    }
}
