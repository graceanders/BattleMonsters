using System;
using System.Collections.Generic;
using System.Text;

namespace BattleMonsters
{
    interface IAttack
    {
        int ATKScore { get; set; }

        double Attack(int AttackerATK, int DefenderDEF);
    }
}
