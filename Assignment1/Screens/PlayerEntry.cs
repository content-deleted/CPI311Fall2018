using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameStateManagement;

namespace Assignment1 {
    public class PlayerEntry {

        Vector2 position;

        public bool active = false;

        private bool buffer = false;

        public JobInfo.jobType job = JobInfo.jobType.Knight;

        public PlayerIndex controllingPlayer;
        public int PlayerNumber;

        public Vector2 Position {
            get { return position; }
            set { position = value; }
        }
        public event EventHandler<PlayerIndexEventArgs> Selected;

        public PlayerEntry(PlayerIndex controllingPlayer, int playerNumber) {
            this.controllingPlayer = controllingPlayer;
            PlayerNumber = playerNumber;
            position.X = 0;
        }

        public virtual void Update(PlayerSelectScreen screen, InputState input) {
            if(!buffer) { buffer = true; return; }
            // Dumb Handling for keyboard I'm dumb
            PlayerIndex? queryPlayer = null;
            if (controllingPlayer != (PlayerIndex)5) queryPlayer = controllingPlayer;


            

            PlayerIndex p;
            // Special handle for keyboard player
            if (queryPlayer == null) {
                if (input.IsNewKeyPress(Microsoft.Xna.Framework.Input.Keys.Space, null, out p))
                    active = true;
                if (input.IsNewKeyPress(Microsoft.Xna.Framework.Input.Keys.Escape, null, out p))
                    active = false;

                // Only let you change class if you haven't selected
                if (!active) {
                    if (input.IsNewKeyPress(Microsoft.Xna.Framework.Input.Keys.Right, null, out p))
                        job = ((int)job + 1 < JobInfo.JobCount) ? job + 1 : 0;

                    if (input.IsNewKeyPress(Microsoft.Xna.Framework.Input.Keys.Left, null, out p))
                        job = (JobInfo.jobType)((job > 0) ? (int)job - 1 : JobInfo.JobCount - 1);
                }
            }
            else {
                if (input.IsNewButtonPress(Microsoft.Xna.Framework.Input.Buttons.Start, queryPlayer, out p)) 
                    active = true;

                if (input.IsMenuCancel(queryPlayer, out p)) 
                    active = false;

                // Only let you change class if you haven't selected
                if (!active) {
                    if (input.IsMenuUp(queryPlayer)) 
                        job = ((int)job + 1 < JobInfo.JobCount) ? job + 1 : 0;
                    
                    if (input.IsMenuDown(queryPlayer)) 
                        job = (JobInfo.jobType)((job > 0) ? (int)job - 1 : JobInfo.JobCount - 1);
                }
            }

            /*
            float fadeSpeed = (float)gameTime.ElapsedGameTime.TotalSeconds * 4;

            if (isSelected)
                selectionFade = Math.Min(selectionFade + fadeSpeed, 1);
            else
                selectionFade = Math.Max(selectionFade - fadeSpeed, 0);
           */
        }
        
        public virtual void Draw(PlayerSelectScreen screen, GameTime gameTime) {
            // Draw text, centered on the middle of each line.
            ScreenManager screenManager = screen.ScreenManager;
            SpriteBatch spriteBatch = screenManager.SpriteBatch;
            SpriteFont font = screenManager.Font;

            Vector2 origin = new Vector2(0, font.LineSpacing / 2);

            spriteBatch.DrawString(font, "Player " + PlayerNumber, position, Color.White, 0,
                                    origin, 1f, SpriteEffects.None, 0);

            Point offset = new Point(68, 34);
            spriteBatch.Draw(JobInfo.portraitBG, new Rectangle(position.ToPoint() - offset, new Point(68, 68)), Color.White);
            offset -=  new Point(2, 2);
            spriteBatch.Draw(JobInfo.Jobs[(int)job].portrait, new Rectangle(position.ToPoint() - offset, new Point(64, 64)), Color.White);

            if(active) spriteBatch.Draw(JobInfo.ready, new Rectangle(position.ToPoint() - offset, new Point(22, 22)), Color.White);

        }


        /// <summary>
        /// Queries how much space this menu entry requires.
        /// </summary>
        public virtual int GetHeight(PlayerSelectScreen screen) {
            return screen.ScreenManager.Font.LineSpacing + 68;
        }


        /// <summary>
        /// Queries how wide the entry is, used for centering on the screen.
        /// </summary>
        public virtual int GetWidth(PlayerSelectScreen screen) {
            return (int)screen.ScreenManager.Font.MeasureString("Player " + PlayerNumber).X - 68 * 2;
        }

    }
}
