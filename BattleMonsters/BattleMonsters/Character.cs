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


        public void AddMonsterToTeam(Creature c)
        {
            this.Team.Add(c);
        }

        

    }
}
