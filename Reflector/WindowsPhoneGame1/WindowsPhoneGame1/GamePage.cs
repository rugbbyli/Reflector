using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Threading;
using Microsoft.Xna.Framework.Input.Touch;
using System.IO.IsolatedStorage;
using System.IO;
using System.Windows.Media.Imaging;
using Microsoft.Xna.Framework.GamerServices;

namespace Reflector
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class GamePage : DrawableGameComponent
    {
        SpriteBatch spriteBatch;
        Texture2D Background;
        WorkingArea workingArea;
        Vector2 VecBackground = new Vector2(0, 10);
        bool isTest = false;

        public void SetLevel(string level)
        {
            if (level == "reflector") isTest = true;
            else isTest = false;
            workingArea = WorkingArea.Create(level, Game.Content);
            workingArea.LevelFinish += workingArea_LevelFinish;
        }

        public void TakeScreenshot()
        {
            WriteableBitmap bmp = new WriteableBitmap(480, 800);
            
        }

        void workingArea_LevelFinish(object sender, EventArgs e)
        {
            if (isTest)
            {
                isTest = false;
                Guide.BeginShowMessageBox("提示", "过关了",
                        new string[] { "确定"}, 0, MessageBoxIcon.None, new AsyncCallback((s) =>
                        {
                            ((Game1)Game).State = PageType.StartPage;
                            return;
                        }), null);
                return;
            }
            ((Game1)Game).ShowMenu(workingArea.GameMessage);
            StorageHelper.AddFinishLevel(((Game1)Game).Level);
        }

        public GamePage(Game game)
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

            Background = Game.Content.Load<Texture2D>("Images/grid");
            
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
                if (ges.GestureType == GestureType.Hold)
                {
                    ((Game1)Game).ShowMenu(workingArea.GameMessage);
                    return;
                }
            }

            workingArea.Update(gameTime);

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);

            spriteBatch.Draw(Background, VecBackground, null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.6f);

            workingArea.Draw(spriteBatch);
            
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
