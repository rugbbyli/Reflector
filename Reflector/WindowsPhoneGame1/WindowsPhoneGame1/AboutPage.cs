using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Threading;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Phone.Tasks;


namespace Reflector
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class AboutPage : DrawableGameComponent
    {
        SpriteBatch spriteBatch;
        
        Texture2D Logo;
        Rectangle Rec_Logo;

        public AboutPage(Game game)
            : base(game)
        {
            this.Enabled = false;
            this.Visible = false;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(Game.GraphicsDevice);

            Logo = Game.Content.Load<Texture2D>("Images/aboutpage");
            Rec_Logo = Game.Window.ClientBounds;

            base.LoadContent();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            if (TouchPanel.IsGestureAvailable)
            {
                var ges = TouchPanel.ReadGesture();
                if (ges.GestureType == GestureType.Tap)
                {
                    if (ges.Position.Y > 467 && ges.Position.Y < 600)
                        new MarketplaceReviewTask().Show();
                    else
                        ((Game1)Game).GoBack();
                }
            }
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);

            spriteBatch.Draw(Logo, Rec_Logo, null, Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
