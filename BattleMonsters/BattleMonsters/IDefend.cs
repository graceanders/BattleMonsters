using System;
using System.Collections.Generic;
using System.Text;

namespace BattleMonsters
{
    interface IDefend
    {
        int DEFScore { get; set; }

        double Defence(int DefenderDEF, int AtackerATK);
    }
}
