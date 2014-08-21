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
    public class LevelSelectPage : DrawableGameComponent
    {
        SpriteBatch spriteBatch;
        List<Level> Levels;

        public LevelSelectPage(Game game)
            : base(game)
        {
            this.Enabled = false;
            this.Visible = false;
            this.VisibleChanged += (s, e) =>
            {
                if (this.Visible)
                {
                    byte offset = ((Game1)Game).World;
                    string str = StorageHelper.QueryFinishLevel(offset);
                    offset = (byte)(offset * 20);
                    foreach (var l in Levels)
                    {
                        if (str.Contains("," + (l.Number + offset) + ","))
                            l.Focused = true;
                        else
                        {
                            l.Focused = false;
                        }
                    }
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
            Levels = new List<Level>(20);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(Game.GraphicsDevice);

            for (byte i = 0; i < Levels.Capacity; i++)
            {
                int x = (i % 4) * 120 + 5;
                int y = (i / 4) * 165 + 5;
                Levels.Add(new Level(Game.Content, i, x, y, 110, 130));
            }

            base.LoadContent();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here

            if (TouchPanel.IsGestureAvailable)
            {
                var ges = TouchPanel.ReadGesture();
                Point pt = new Point((int)ges.Position.X, (int)ges.Position.Y);
                foreach (Level lv in Levels)
                {
                    if (lv.Contains(pt))
                    {
                        Game1 game = (Game1)Game;
                        game.Level = (byte)(lv.Number + game.World * 20);
                        game.GoForward(PageType.GamePage);
                        break;
                    }
                }
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);

            foreach (Level lv in Levels)
                lv.Draw(spriteBatch);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
