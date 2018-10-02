using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace CPI311.GameEngine
{
    public class EventTimer {
        public float ticks;
        public Action action;

        public EventTimer(Action a, float t) {
            action = a;
            ticks = t;
        }
    }

    public static class Time {
        public static float ElapsedGameTime { get; private set; }
        public static TimeSpan TotalGameTime { get; private set; }
        public static float TotalGameTimeMilli { get => TotalGameTime.Milliseconds; }

        public static List<EventTimer> timers = new List<EventTimer>();
        
        public static void Initialize()
        {
            ElapsedGameTime = 0;
            TotalGameTime = new TimeSpan(0);
        }

        public static void Update(GameTime gameTime)
        {
            ElapsedGameTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            TotalGameTime = gameTime.TotalGameTime;

            // Update timers
            foreach (EventTimer timer in timers.ToList()) {
                timer.ticks -= ElapsedGameTime;
                if(timer.ticks <= 0) timer.action();
            }
            timers.RemoveAll(x => x.ticks <= 0);
        }
    }

}
