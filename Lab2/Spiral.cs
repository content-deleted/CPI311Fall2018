using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CPI311.GameEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Lab2
{
    public class Spiral : Sprite
    {
        public float radius = 50;
        float angle = 0;
        public float speed = 0.1f;
        float frequency = 20;
        float amplitude = 0;
        float phase = 0;

        Vector2 anchor; 

        public Spiral(Texture2D texture, Vector2 pos) : base(texture) => anchor = pos;

        public override void Update() {
            angle += speed;

            amplitude = (float) ( ( Math.Cos(angle/frequency) ));

            Position = anchor +  amplitude * new Vector2( (float) (Math.Cos(angle) * radius), (float) (Math.Sin(angle) * radius) );

            Rotation = angle;
        }
    }
}
