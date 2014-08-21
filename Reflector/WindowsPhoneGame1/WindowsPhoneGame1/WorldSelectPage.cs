using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Threading;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Input;


namespace Reflector
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class WorldSelectPage : DrawableGameComponent
    {
        SpriteBatch spriteBatch;
        WorldButton[] world = new WorldButton[6];

        public WorldSelectPage(Game game)
            : base(game)
        {
            this.Enabled = false;
            this.Visible = false;
            this.VisibleChanged += (s, e) =>
            {
                if (this.Visible == true)
                {
                    int w = StorageHelper.GetUnlockWorld();
                    int i = 0;
                    for (; i <= w; i++)
                        world[i].Enable = true;
                    for (; i < 6; i++)
                        world[i].Enable = false;
                }
            };
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

            world[0] = new WorldButton(Game.Content, Colors.blue, 1, 50, 10, 380, 120);
            world[1] = new WorldButton(Game.Content, Colors.red, 2, 50, 142, 380, 120);
            world[2] = new WorldButton(Game.Content, Colors.yellow, 3, 50, 274, 380, 120);
            world[3] = new WorldButton(Game.Content, Colors.green, 4, 50, 406, 380, 120);
            world[4] = new WorldButton(Game.Content, Colors.orange, 5, 50, 538, 380, 120);
            world[5] = new WorldButton(Game.Content, Colors.purple, 6, 50, 670, 380, 120);

            base.LoadContent();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here
            base.Update(gameTime);

            if (TouchPanel.IsGestureAvailable)
            {
                var ges = TouchPanel.ReadGesture();
                if (ges.GestureType != GestureType.Tap) return;
                Point pt = new Point((int)ges.Position.X, (int)ges.Position.Y);
                for (byte i = 0; i < 6; i++)
                {
                    if (world[i].Contains(pt))
                    {
                        if (!world[i].Enable) return;
                        ((Game1)Game).World = i;
                        ((Game1)Game).GoForward(PageType.LevelSelectPage);
                        return;
                    }
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);

            foreach (Button b in world)
                b.Draw(spriteBatch);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
