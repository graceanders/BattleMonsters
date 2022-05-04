using System;
using System.Collections.Generic;
using System.Text;

namespace BattleMonsters
{
    public class Player : Character
    {
        int StartingLevel = 1;
        int StartingCoins = 5;

        //public Team Team;

        public List<Creature> AllMonsters = new List<Creature>(); //All Monsters the have

        public List<Creature> DeadMonsters = new List<Creature>();//Can be healed for coins


        public Player() : this("Ace") { }
        public Player(string name)
        {
            this.Name = name;
            //this.CombindedATK = CalculateCombindedATK();
            this.Level = StartingLevel;
            this.Coins = StartingCoins;
        }

        public void AddMonsterToAllMonsters(Creature c)
        {
            if (CheckIfTeamIsFull() == true)
            {
                GamePrintout.TxtPrintOut += "\nYour Team is full the Monster has been added to your collection of Monsters";
                this.AllMonsters.Add(c);
            }
            else
            {
                this.Team.Add(c);
                GamePrintout.TxtPrintOut += "\nThis Monster has been added to your Team!";
            }
            
        }

        public void AddMonsterToDeadMonsters(Creature c)
        {
            this.DeadMonsters.Add(c);
            this.Team.Remove(c);
        }

        public bool CheckIfTeamIsFull()
        {
            if(this.Team.Count == 3)
            {
                return true;
            }
            return false;
        }

        public int GetDeadMonsterCount()
        {
            return this.DeadMonsters.Count;
        }
    }
}
