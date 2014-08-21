using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reflector
{
    class Button
    {
        Rectangle rec1, rec2, rec3, rec4;
        Texture2D Border;
        Texture2D Foreground;
        Rectangle Rec;

        Color ForeColor = Color.White;
        bool m_enable;
        public bool Enable
        {
            get
            {
                return m_enable;
            }
            set
            {
                m_enable = value;
                if (value == true)
                    ForeColor = Color.White;
                else
                {
                    ForeColor = new Color(50, 50, 50,255);
                }
            }
        }

        public Button(ContentManager content, Colors borderbrush, string image, int x, int y, int width, int height)
            : this(content, borderbrush, x, y, width, height)
        {
            Foreground = content.Load<Texture2D>(image);
        }

        public Button(ContentManager content, Colors borderbrush, int x, int y, int width, int height)
        {
            Border = content.Load<Texture2D>("Images/Laser/" + borderbrush.ToString());

            rec1 = new Rectangle(x, y, width, 6);
            rec2 = new Rectangle(x, y + height - 6, width, 6);
            rec3 = new Rectangle(x + 6, y + 3, height - 6, 6);
            rec4 = new Rectangle(x + width, y + 3, height - 6, 6);
            Rec = new Rectangle(x, y, width, height);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Border, rec1, null, ForeColor, 0, Vector2.Zero, SpriteEffects.None, 0);
            spriteBatch.Draw(Border, rec2, null, ForeColor, 0, Vector2.Zero, SpriteEffects.None, 0);
            spriteBatch.Draw(Border, rec3, null, ForeColor, MathHelper.PiOver2, Vector2.Zero, SpriteEffects.None, 0);
            spriteBatch.Draw(Border, rec4, null, ForeColor, MathHelper.PiOver2, Vector2.Zero, SpriteEffects.None, 0);
            if(Foreground != null)
                spriteBatch.Draw(Foreground, Rec, null, ForeColor, 0, Vector2.Zero, SpriteEffects.None, 0);
        }

        public bool Contains(Point point)
        {
            return Rec.Contains(point);
        }
    }

    class WorldButton : Button
    {
        public WorldButton(ContentManager content, Colors borderBrush, int world, int x, int y, int width, int height)
            : base(content, borderBrush, "Images/Level/w" + world, x, y, width, height)
        {

        }
    }
}
