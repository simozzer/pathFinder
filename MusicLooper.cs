using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace SiFirstMonoGame
{
    internal class MusicLooper
    {
        List<SoundEffect> initialLoops;
        bool started = false;
        long startTicks;
        int loopIndex;
        internal MusicLooper()
        {
            started = false;
            loopIndex = 0;
        }
        long duration;

        internal List<SoundEffect> InitialLoops {            
            get
            {
                return this.initialLoops;
            }
            set {
                this.initialLoops = value;
                if (value != null)
                {
                     this.duration = value[0].Duration.Ticks;
                }
            }
        }

        internal void Update(GameTime gameTime)
        {
            return;
            if (!started)
            {
                startTicks = gameTime.TotalGameTime.Ticks;
                initialLoops[loopIndex].Play();
                started = true;
            } else if ((gameTime.TotalGameTime.Ticks - startTicks) > duration)
            {
                startTicks = gameTime.TotalGameTime.Ticks;
                loopIndex++;
                if (loopIndex >= initialLoops.Count)
                {
                    loopIndex = 0;
                }
                this.duration = initialLoops[loopIndex].Duration.Ticks;
                initialLoops[loopIndex].Play();
                
                   
            }
        }

        internal long Duration
        {
            get {
                return this.duration;
            }
        }
    }
}
