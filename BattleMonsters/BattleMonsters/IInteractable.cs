using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace BattleMonsters
{
    interface IInteractable
    {
        string ButtonGuideTxt { get; set; }

        Vector2 ButtonGuideLoc { get; set; }

    }
}
