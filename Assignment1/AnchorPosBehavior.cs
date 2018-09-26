using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using CPI311.GameEngine;

namespace Assignment1 {
    /* Very simple class to anchor to the player object */
    class AnchorPosBehavior : Behavior2d {
        override public void Update() => objSprite.Position = PlayerObject.singleton.sprite.Position;
    }

    class AnchorMouseBehavior : Behavior2d {
        override public void Update() => objSprite.Position = new Vector2 (Mouse.GetState().Position.X, Mouse.GetState().Position.Y);
    }
}
