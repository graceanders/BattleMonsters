﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace BattleMonsters
{
    public class TeamManager : DrawableGameComponent, IInteractable
    {
        Player P;
        Game g;

        InputHandler input;

        public bool ExitTeamManager;

        public string ButtonGuideTxt { get; set; }
        public Vector2 ButtonGuideLoc { get; set; }

        Rectangle TeamEditLoc;
        Texture2D TeamEditSprite;

        //string WhichTeamMember;
        int WhichTeamMember, WhichAllMonster;

        Creature ThisMonster;

        public Color TeamManageElement;

        MonsterManager mm;


        public TeamManager(Game game, Player p, MonsterManager MM): base(game)
        {
            input = (InputHandler)game.Services.GetService(typeof(IInputHandler));

            P = p;
            g = game;
            mm = MM;

            LoadContent();
        }

        protected override void LoadContent()
        {
            ButtonGuideTxt = "L: Lock in Monster | R: Remove Monster | ->: Next Monster | E: Exit";
            ButtonGuideLoc = new Vector2(20, g.GraphicsDevice.Viewport.Height - 50);

            TeamEditLoc = new Rectangle(g.GraphicsDevice.Viewport.Width / 4, 100, 250, 250);

            WhichAllMonster = WhichTeamMember = 0;
            TeamEditSprite = P.Team[0].spriteTexture;

            base.LoadContent();
        }

        bool Manageable;
        public void ManageTeam()
        {
            GamePrintout.TxtPrintOut = "Welcom to the Team Manager!";

            if (P.AllMonsters.Count != 0) 
            { 
                GamePrintout.TxtPrintOut += "\nWhen you are happy with a monster hit L to lock in";
                WhichTeamMember = 0;
                TeamEditSprite = P.Team[0].spriteTexture;
            }
            else 
            {
                GamePrintout.TxtPrintOut += "\nYou don't have any extra Monsters and cannot manage your Team\nCome back when you have more Monsters";
                Manageable = false; 
            }
            
        }

        int TeamBounds, AllMonsterBounds;
        Creature ReplacedMonster;
        public void TeamManageInput()
        {
            if (input.KeyboardState.WasKeyPressed(Keys.L))
            {
                if (Manageable)
                {
                    SaveReplacedMonster();

                    LockMonsterIn();

                    GamePrintout.TxtPrintOut = $"{ReplacedMonster.Name} has been swapped for {P.AllMonsters[WhichAllMonster].Name}\n{ReplacedMonster.Name} has been added to your other Monsters";

                    NextTeamMember();
                }

            }
            if (input.KeyboardState.WasKeyPressed(Keys.R))
            {
                if (Manageable) { RemoveCurrentMonster(); }
            }
            if (input.KeyboardState.WasKeyPressed(Keys.Right))
            {
                if (Manageable) { NextMonster(); }
            }
            if (input.KeyboardState.WasKeyPressed(Keys.E))
            {
                ExitTeamManager = true;
            }
        }

        public bool GetExitTeamManager() { return ExitTeamManager; }

        #region Lock In
        void SaveReplacedMonster()
        {
            if (WhichTeamMember == 0) { ReplacedMonster = P.Team[0]; }
            if (WhichTeamMember == 1) { ReplacedMonster = P.Team[1]; }
            if (WhichTeamMember == 2) { ReplacedMonster = P.Team[2]; }
        }

        void LockMonsterIn()
        {
            P.Team[WhichTeamMember] = P.AllMonsters[WhichAllMonster];
            P.AllMonsters.Add(ReplacedMonster);
            P.CurrentMonster = P.Team[0];
        }

        void NextTeamMember()
        {
            TeamBounds = P.Team.Count - 1;
            if (WhichTeamMember > TeamBounds) { WhichTeamMember = 0; }
            WhichTeamMember++;
            TeamEditSprite = P.Team[WhichTeamMember].spriteTexture;
        }
        #endregion

        void RemoveCurrentMonster()
        {
            P.Team.Remove(P.Team[WhichTeamMember]);
            GamePrintout.TxtPrintOut = $"You Removed {P.Team[WhichTeamMember].Name} from your Team";
            P.AllMonsters.Add(P.Team[WhichTeamMember]);
        }

        void NextMonster()
        {
            GamePrintout.TxtPrintOut += CompareStats();

            TeamEditSprite = P.AllMonsters[WhichAllMonster].spriteTexture;
            AllMonsterBounds = P.AllMonsters.Count - 1;
            if (WhichAllMonster > AllMonsterBounds) { WhichAllMonster = 0; }
            WhichAllMonster++;
        }

        string stats;
        string CompareStats()
        {
            stats = $"\nHP: {P.Team[WhichTeamMember].HP}/{P.Team[WhichTeamMember].HPMax} | ATK: {P.Team[WhichTeamMember].ATKScore} | DEF: {P.Team[WhichTeamMember].DEFScore}\n";
            stats += $"HP: {P.AllMonsters[WhichAllMonster].HP}/{P.AllMonsters[WhichAllMonster].HPMax} | ATK: {P.AllMonsters[WhichAllMonster].ATKScore} | DEF: {P.AllMonsters[WhichAllMonster].DEFScore}\n";
            return stats;
        }


        public void Draw(SpriteBatch sb)
        {
            sb.DrawString(GamePrintout.font, ButtonGuideTxt, ButtonGuideLoc, TeamManageElement);
            sb.Draw(TeamEditSprite, TeamEditLoc, TeamManageElement);
        }
    }
}
