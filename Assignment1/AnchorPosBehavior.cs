using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using CPI311.GameEngine;

namespace Assignment1 {
    /* Very simple class to anchor an object to another sprite */
    class AnchorPosBehavior : Behavior2d {
        Sprite anchor;
        public AnchorPosBehavior(Sprite s) => anchor = s;
        override public void Update() => objSprite.Position = anchor.Position;
    }

    class AnchorMouseBehavior : Behavior2d {
        override public void Update() => objSprite.Position = new Vector2 (Mouse.GetState().Position.X, Mouse.GetState().Position.Y);
    }
}
