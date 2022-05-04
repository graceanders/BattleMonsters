using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace BattleMonsters
{
    //Maybe add visual that shows the monster your healing?
    public class HealManager : DrawableGameComponent
    {
        Player P;
        InputHandler input;

        public bool ExitHealing;
        int NeedsHeal;
        public bool MonsterHealed;

        public HealManager(Game game, Player player) : base(game)
        {
            input = (InputHandler)game.Services.GetService(typeof(IInputHandler));
            P = player;

        }


        #region Healing

        int HealCost = 10;
        
        Creature HealedMonster;
        
        public void HealMonster()//I'd like to have the player pick which monster they heal but I'm not sure how to get the input to corispond
        {
            if (!MonsterHealed)
            {
                GamePrintout.TxtPrintOut = "Welcome to the Healing Station!\n";

                NeedsHeal = P.GetDeadMonsterCount();

                if (NeedsHeal == 0)
                {
                    GamePrintout.TxtPrintOut += "You have 0 Monsters that need healing";
                }
                else
                {
                    if (P.Coins >= HealCost)//Can heal at least one Monster
                    {
                        PickWhichToHeal();
                    }
                    else
                    {
                        GamePrintout.TxtPrintOut += $"You only have {P.Coins} Coins\nYou need {HealCost} Coins to heal a Monster";
                    }
                }
            }

        }

        public void Heal(Creature c)
        {
            P.Coins -= HealCost;
            
            P.DeadMonsters.Remove(c);//Remove the first monster from DeadMonsters
            HealedMonster = c;//Set it locally
            P.AllMonsters.Add(HealedMonster);//Add to All Monsters
            GamePrintout.TxtPrintOut = $"{HealedMonster.Name} has been Healed, and added back to your colection of Monsters";
            MonsterHealed = true;
        }

        int WhichMonster = 0;
        void PickWhichToHeal()
        {
            if(NeedsHeal == 1) { GamePrintout.TxtPrintOut += $"Only {P.DeadMonsters[WhichMonster].Name} needs healing\n{P.DeadMonsters[WhichMonster].DisplayStats()}"; }
            else
            {
                if (WhichMonster > NeedsHeal - 1) { WhichMonster = 0; }//Cycle Through
                GamePrintout.TxtPrintOut += $"Would you like to heal {P.DeadMonsters[WhichMonster].Name}\n{P.DeadMonsters[WhichMonster].DisplayStats()}";
            }
            
        }
        #endregion

        //TODO: have arrows swap through all that need heal
        
        public void HealInput()
        {
            if (input.KeyboardState.WasKeyPressed(Keys.T))
            {
                Heal(P.DeadMonsters[WhichMonster]);
            }
            if (input.KeyboardState.WasKeyPressed(Keys.Right))
            {
                WhichMonster++;
                //Goes to next monster
            }
            if (input.KeyboardState.WasKeyPressed(Keys.E))
            {
                ExitHealing = true;
            }
        }

        public bool GetExitHealing() { return ExitHealing; }

        //Should it handel drawing its own vlaues?
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }
    }
}
