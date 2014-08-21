using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reflector
{
    class Star
    {
        Texture2D Image;
        int LastTime = 0;
        int RefreshInterval = 100;
        Vector2 Speed;
        Vector2 Position;

        public Star(Texture2D image, Vector2 position, Vector2 speed)
        {
            this.Image = image;
            this.Position = position;
            this.Speed = speed;
        }

        public virtual void Update(GameTime gameTime)
        {
            LastTime += gameTime.ElapsedGameTime.Milliseconds;

            if (LastTime >= RefreshInterval)
            {
                LastTime -= RefreshInterval;
                Position += Speed;
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Image, Position, null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 1);
        }

        public bool IsOutOfBounds(Rectangle bounds)
        {
            return (
                Position.X + Image.Width < 0
                || Position.Y + Image.Height < 0
                || Position.X > bounds.Width
                || Position.Y > bounds.Height);
        }
    }
}
