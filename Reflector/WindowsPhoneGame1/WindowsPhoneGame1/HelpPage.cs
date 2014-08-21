using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;

namespace Reflector
{
    public class HelpPage : DrawableGameComponent
    {
        SpriteBatch spriteBatch;
        Texture2D Img1, Img2;
        Rectangle Rec1, Rec2;
        bool Moving = true;

        public HelpPage(Game game)
            : base(game)
        {
            this.Enabled = false;
            this.Visible = false;
            this.VisibleChanged += (s, e) =>
            {
                if (this.Visible)
                {
                    Rec1.Y = 400;
                    Rec2.Y = 1200;
                    Moving = true;
                }
            };
        }

        public override void Initialize()
        {
            // TODO: Add your initialization code here
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(Game.GraphicsDevice);

            Img1 = Game.Content.Load<Texture2D>("Images/helppage1");
            Img2 = Game.Content.Load<Texture2D>("Images/helppage2");
            Rec1 = new Rectangle(0, 400, 480, 800);
            Rec2 = new Rectangle(0, 1200, 480, 800);
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            if (TouchPanel.IsGestureAvailable)
            {
                var ges = TouchPanel.ReadGesture();
                if (ges.GestureType == GestureType.Tap)
                {
                    Moving = !Moving;
                }
            }

            if (Moving)
            {
                Rec1.Y-=2;
                Rec2.Y-=2;
                if (Rec2.Y < -700)
                {
                    Rec1.Y = 400;
                    Rec2.Y = 1200;
                }
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);

            if(Rec1.Y > -800)
                spriteBatch.Draw(Img1, Rec1, null, Color.White);
            if(Rec2.Y < 800)
            spriteBatch.Draw(Img2, Rec2, null, Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
