using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace SiFirstMonoGame
{
    public class ParticleEngine
    {
        private Random random;
        private List<Particle> particles;
        private List<Texture2D> textures;
        
        public ParticleEngine(List<Texture2D> textures)
        {            
            this.textures = textures;
            this.particles = new List<Particle>();
            random = new Random();
        }
        
        
        public void AddParticles(Point screenPoint)
        {
            int total = 10;

            for (int i = 0; i < total; i++)
            {
                particles.Add(GenerateNewParticle(screenPoint));
            }
        }

        public void Update()
        {
            
            for (int particle = 0; particle < particles.Count; particle++)
            {
                particles[particle].Update();
                if (particles[particle].TTL <= 0)
                {
                    particles.RemoveAt(particle);
                    particle--;
                }
            }
        }

        private Particle GenerateNewParticle(Point screenPoint)
        {
            Texture2D texture = textures[random.Next(textures.Count)];
            Vector2 position = new Vector2(screenPoint.X, screenPoint.Y);
            Vector2 velocity = new Vector2(
                                    1f * (float)(random.NextDouble() * 2 - 1),
                                    1f * (float)(random.NextDouble() * 2 - 1));
            float angle = 0;
            float angularVelocity = 0.1f * (float)(random.NextDouble() * 2 - 1);
            Color color = new Color(
                        (float)random.NextDouble(),
                        (float)random.NextDouble(),
                        (float)random.NextDouble());
            float size = (float)random.NextDouble();
            int ttl = 20 + random.Next(40);

            return new Particle(texture, position, velocity, angle, angularVelocity, color, size, ttl);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
          //  spriteBatch.Begin();
            for (int index = 0; index < particles.Count; index++)
            {
                particles[index].Draw(spriteBatch);
            }
          //  spriteBatch.End();
        }
    }
}
