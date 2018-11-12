using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameStateManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Assignment1 {
    public class PlayerSelectScreen : GameScreen {

        public static ContentManager content;

        Dictionary<PlayerIndex, PlayerEntry> playerEntries = new Dictionary<PlayerIndex, PlayerEntry>();
        string menuTitle = "Players";
        string subTitle = "Hit Start to join B to cancel";


        protected IList<PlayerEntry> PlayerEntries {
            get => playerEntries.Select(x => x.Value).ToList();
        }

        public PlayerSelectScreen() {
            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
        }
        
        
        public override void LoadContent() {
            if (content == null) {
                content = new ContentManager(ScreenManager.Game.Services, "Content");
                // Call this once the first time we move to select screen
                JobInfo.LoadContent(content);
            }
        }

        public override void HandleInput(InputState input) {
            PlayerIndex PlayerNum;

            if (input.IsMenuSelect(null, out PlayerNum)) {
                // Check if the index is in our list of joined players
                if (!playerEntries.ContainsKey(PlayerNum)) playerEntries.Add(PlayerNum, new PlayerEntry(PlayerNum, playerEntries.Count() + 1));
            }

            if(input.IsMenuCancel(null, out PlayerNum)) {
                if (playerEntries.ContainsKey(PlayerNum) && !playerEntries[PlayerNum].active) playerEntries.Remove(PlayerNum); //playerEntries[PlayerNum].active = false;
            }

            foreach (PlayerEntry player in PlayerEntries) 
                player.Update(this, input);

            if (PlayerEntries.Any() && PlayerEntries.All(p => p.active)) {
                SetupPlayers();
                ScreenManager.AddScreen(new MainGameplayScreen(), null);
                ScreenManager.RemoveScreen(this);
            }
        }

        private void SetupPlayers () {
            foreach (PlayerEntry p in PlayerEntries) {
                PlayerObject.CreatePlayer(JobInfo.Jobs[(int)p.job], p.controllingPlayer);
            }
            PlayerObject.players.Select(p => p.sprite.Position = new Vector2(1000, 1000));
        }
        
        protected virtual void UpdateMenuEntryLocations() {
            // Make the menu slide into place during transitions, using a
            // power curve to make things look more interesting (this makes
            // the movement slow down as it nears the end).
            float transitionOffset = (float)Math.Pow(TransitionPosition, 2);

            // start at Y = 175; each X value is generated per entry
            Vector2 position = new Vector2(0f, 175f);

            // update each menu entry's location in turn
            foreach(PlayerEntry player in PlayerEntries) { 
                position.X = ScreenManager.GraphicsDevice.Viewport.Width / 2 - player.GetWidth(this) / 2;

                if (ScreenState == ScreenState.TransitionOn)
                    position.X -= transitionOffset * 256;
                else
                    position.X += transitionOffset * 512;

                // set the entry's position
                player.Position = position;

                // move down for the next entry the size of this entry
                position.Y += player.GetHeight(this);
            }
        }

        /// <summary>
        /// Draws the menu.
        /// </summary>
        public override void Draw(GameTime gameTime) {
            // make sure our entries are in the right place before we draw them
            UpdateMenuEntryLocations();

            GraphicsDevice graphics = ScreenManager.GraphicsDevice;
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            SpriteFont font = ScreenManager.Font;

            spriteBatch.Begin();

            // Draw each menu entry in turn.
            foreach (PlayerEntry player in PlayerEntries)
                player.Draw(this, gameTime);

            // Make the menu slide into place during transitions, using a
            // power curve to make things look more interesting (this makes
            // the movement slow down as it nears the end).
            float transitionOffset = (float)Math.Pow(TransitionPosition, 2);

            // Draw the menu title centered on the screen
            Vector2 titlePosition = new Vector2(graphics.Viewport.Width / 2, 80);
            Vector2 titleOrigin = font.MeasureString(menuTitle) / 2;
            Color titleColor = new Color(192, 192, 192) * TransitionAlpha;
            float titleScale = 1.25f;

            titlePosition.Y -= transitionOffset * 100;

            spriteBatch.DrawString(font, menuTitle, titlePosition, titleColor, 0,
                                   titleOrigin, titleScale, SpriteEffects.None, 0);

            spriteBatch.End();
        }
    }
}
