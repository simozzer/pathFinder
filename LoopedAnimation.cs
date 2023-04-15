using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;


namespace SiFirstMonoGame
{
    class LoopedAnimation
    {
        protected List<Texture2D> images;
        protected int imageIndex = 0;
        protected float fImage = 0.0f;
        protected int playerSize;
        protected float delta;
        protected float rotation;
        internal LoopedAnimation(List<Texture2D> images, int playerSize, float delta)
        {
            this.images = images;
            this.playerSize = playerSize;
            this.imageIndex = 0;
            this.delta = delta;
            this.fImage = 0.0f;
            this.rotation = 0;
        }

        internal virtual void UpdateImage()
        {
            this.fImage = this.fImage + delta;
            if (this.fImage >= 1.0f)
            {
                this.fImage = 0.0f;
                this.imageIndex++;
                if (this.imageIndex >= this.images.Count)
                {
                    this.imageIndex = 0;
                }
            }
        }

        internal void Draw(SpriteBatch b, Point origin, Color color, Vector2 velocities, float rotation)
        {

            //rotation += 0
            //            b.Draw(images[imageIndex], new Rectangle(origin, new Point(playerSize, playerSize)),color);
            if (Math.Abs(velocities.X) >= Math.Abs(velocities.Y))
            {
                // lateral movelement
                if (velocities.X >= 0)
                {
                    b.Draw(images[imageIndex], new Rectangle(new Point(origin.X + playerSize / 2, origin.Y + playerSize / 2), new Point(playerSize, playerSize)), null, color, 0.0f + rotation, new Vector2(playerSize / 2), SpriteEffects.None, 0.1f);
                }
                else
                {
                    b.Draw(images[imageIndex], new Rectangle(new Point(origin.X + playerSize / 2, origin.Y + playerSize / 2), new Point(playerSize, playerSize)), null, color, 0.0f + rotation, new Vector2(playerSize / 2), SpriteEffects.FlipHorizontally, 0.1f);
                }
            } else
            {
                // vertical movement
                if (velocities.Y >= 0)
                {
                    b.Draw(images[imageIndex], new Rectangle(new Point(origin.X + playerSize / 2, origin.Y + playerSize / 2), new Point(playerSize, playerSize)), null, color, (float)(Math.PI / 2.0f) + rotation, new Vector2(playerSize / 2), SpriteEffects.None, 0.1f);
                }
                else
                {
                    b.Draw(images[imageIndex], new Rectangle(new Point(origin.X + playerSize / 2, origin.Y + playerSize / 2), new Point(playerSize, playerSize)), null, color, (float)(Math.PI / 2.0f) + rotation, new Vector2(playerSize / 2), SpriteEffects.FlipHorizontally, 0.1f);
                }
            }
            
        }
    }

    class UpDownLoopedAnimation : LoopedAnimation {

        private bool inc;
        internal UpDownLoopedAnimation(List<Texture2D> images, int playerSize, float delta):base(images, playerSize, delta)
        {
            inc = true;
        }

        internal override void UpdateImage()
        {

            this.fImage = this.fImage + delta;
            if (this.fImage >= 1.0f)
            {
                this.fImage = 0.0f;


                if (inc)
                {
                    this.imageIndex++;
                    if (this.imageIndex >= this.images.Count)
                    {

                        this.imageIndex = this.images.Count - 1;
                        inc = false;
                    }
                }
                else
                {
                    this.imageIndex--;
                    if (this.imageIndex < 0)
                    {
                        this.imageIndex = 0;
                        inc = true;
                    }

                }
            }

        }
    }
}
