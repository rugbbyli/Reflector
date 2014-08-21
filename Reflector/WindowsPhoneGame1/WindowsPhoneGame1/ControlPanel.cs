using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reflector
{
    enum ControlPanelStyle
    {
        Panel,
        Control,
    }

    class ControlPanel
    {
        Texture2D Background;
        Texture2D Mirror;
        public int MirrorCount;
        Texture2D Prism;
        public int PrismCount;
        Texture2D RotateLeft;
        Texture2D RotateRight;
        Texture2D Delete;
        Texture2D Reset;
        public ControlPanelStyle Style { get; set; }
        public Rectangle Position { get; set; }
        SpriteFont number, english;

        public Rectangle Rec1, Rec2;
        Rectangle Rec3, Rec4;
        Vector2 VecNum1, VecNum2, VecWord1, VecWord2, VecWord3, VecWord4;

        public ControlPanel(ContentManager content,int x,int y,int mirrors,int prisms)
        {
            Background = content.Load<Texture2D>(@"Images/ControlPanel/background");
            Mirror = content.Load<Texture2D>("Images/MirrorMenu/mirror");
            Prism = content.Load<Texture2D>("Images/MirrorMenu/prism");
            RotateLeft = content.Load<Texture2D>(@"Images/ControlPanel/left");
            RotateRight = content.Load<Texture2D>(@"Images/ControlPanel/right");
            Delete = content.Load<Texture2D>(@"Images/ControlPanel/delete");
            Reset = content.Load<Texture2D>(@"Images/ControlPanel/reset");

            Position = new Rectangle(x, y, Background.Width, Background.Height);
            MirrorCount = mirrors;
            PrismCount = prisms;

            number = content.Load<SpriteFont>("Fonts/number");
            english = content.Load<SpriteFont>("Fonts/english");
            this.Style = ControlPanelStyle.Panel;

            Rec1 = new Rectangle(x + 35, y + 10, 50, 50);
            Rec2 = new Rectangle(x + 155, y + 10, 50, 50);
            Rec3 = new Rectangle(x + 275, y + 10, 50, 50);
            Rec4 = new Rectangle(x + 395, y + 10, 50, 50);
            VecNum1 = new Vector2(x + 53, y + 65);
            VecNum2 = new Vector2(x + 173, y + 65);
            VecWord1 = new Vector2(Position.X + 43, Position.Y + 65);
            VecWord2 = new Vector2(Position.X + 155, Position.Y + 65);
            VecWord3 = new Vector2(Position.X + 270, Position.Y + 65);
            VecWord4 = new Vector2(Position.X + 395, Position.Y + 65);
        }

        public void Update()
        {
            
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Background, Position, null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.5f);
            //4个控件占200宽度，5个间距占280，可以分为50 60 60 60 50
            //每个控件左右各35长度
            if (Style == ControlPanelStyle.Panel)
            {
                spriteBatch.Draw(Prism, Rec1, null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.4f);
                spriteBatch.DrawString(number, PrismCount.ToString(), VecNum1, Color.White, 0, Vector2.Zero, 0.3f, SpriteEffects.None, 0.4f);
                spriteBatch.Draw(Mirror, Rec2, null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.4f);
                spriteBatch.DrawString(number, MirrorCount.ToString(), VecNum2, Color.White, 0, Vector2.Zero, 0.3f, SpriteEffects.None, 0.4f);
            }
            else
            {
                spriteBatch.Draw(RotateLeft, Rec1, null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.4f);
                spriteBatch.DrawString(english, "LEFT", VecWord1, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0.4f);

                spriteBatch.Draw(RotateRight, Rec2, null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.4f);
                spriteBatch.DrawString(english, "RIGHT", VecWord2, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0.4f);

                spriteBatch.Draw(Delete, Rec3, null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.4f);
                spriteBatch.DrawString(english, "DELETE", VecWord3, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0.4f);

            }
            spriteBatch.Draw(Reset, Rec4, null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.4f);
            spriteBatch.DrawString(english, "RESET", VecWord4, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0.4f);
        }

        public int PointInWhere(Point point)
        {
            //if (Rec1.Contains(point))
            //    return 1;
            //else if (Rec2.Contains(point))
            //    return 2;
            //else if (Rec3.Contains(point))
            //    return 3;
            //else if (Rec4.Contains(point))
            //    return 4;
            //else
            //    return 0;
            if (point.Y < this.Position.Y) return 0;
            return point.X / 120 + 1;
        }
    }
}
