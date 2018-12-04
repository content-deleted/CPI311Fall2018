using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameStateManagement;
using Microsoft.Xna.Framework;

namespace Assignment1 {
    class MainMenu : MenuScreen {
        public MainMenu() : base("Main Menu") {
            MenuEntry startGame = new MenuEntry("Start");
            startGame.drawColor = Color.Gray;
            startGame.selectedColor = Color.White;
            
            startGame.Selected += StartGameSelected;

            MenuEntries.Add(startGame);
        }

        void StartGameSelected(object sender, PlayerIndexEventArgs e) {
            ScreenManager.AddScreen(new PlayerSelectScreen(), null);
            this.ExitScreen();
        }
    }
}
