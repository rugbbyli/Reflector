using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Threading;
using Microsoft.Xna.Framework.Input.Touch;


namespace Reflector
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class StartPage : DrawableGameComponent
    {
        SpriteBatch spriteBatch;
        List<Receiver> Receivers = new List<Receiver>();
        Texture2D Title;
        Rectangle RecTitle;
        Texture2D StartGame;
        Rectangle RecStart;
        Texture2D SelectLevel;
        Rectangle RecSelect;
        Texture2D Help;
        Rectangle RecHelp;
        Texture2D About;
        Rectangle RecAbout;

        Texture2D Laser;
        Rectangle rec1, rec2, rec3, rec4;

        public StartPage(Game game)  : base(game)
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

            Title = Game.Content.Load<Texture2D>("Images/title");
            RecTitle = new Rectangle(40, 40, 400, 180);

            StartGame = Game.Content.Load<Texture2D>("Images/start");
            RecStart = new Rectangle(130, 210, 300, 110);
            SelectLevel = Game.Content.Load<Texture2D>("Images/select");
            RecSelect = new Rectangle(130, 340, 300, 110);
            Help = Game.Content.Load<Texture2D>("Images/help");
            RecHelp = new Rectangle(130, 470, 300, 110);
            About = Game.Content.Load<Texture2D>("Images/about");
            RecAbout = new Rectangle(130, 600, 300, 110);

            Laser = Game.Content.Load<Texture2D>("Images/Laser/blue");
            rec1 = new Rectangle(40, 35, 400, 10);
            rec2 = new Rectangle(40,755,400,10);
            rec3 = new Rectangle(45, 35, 720, 10);
            rec4 = new Rectangle(445, 35, 720, 10);

            Receivers.Add(new Receiver(Game.Content, Colors.red, 10, 10) { Focused = true });
            Receivers.Add(new Receiver(Game.Content, Colors.green, 410, 10) { Focused = true });
            Receivers.Add(new Receiver(Game.Content, Colors.yellow, 10, 730) { Focused = true });
            Receivers.Add(new Receiver(Game.Content, Colors.purple, 410, 730) { Focused = true });

            Receivers.Add(new Receiver(Game.Content, Colors.blue, 70, 235) { Focused = true });
            Receivers.Add(new Receiver(Game.Content, Colors.blue, 70, 365) { Focused = true });
            Receivers.Add(new Receiver(Game.Content, Colors.blue, 70, 495) { Focused = true });
            Receivers.Add(new Receiver(Game.Content, Colors.blue, 70, 625) { Focused = true });

            base.LoadContent();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            if(TouchPanel.IsGestureAvailable)
            {
                var gesture = TouchPanel.ReadGesture();
                if (gesture.GestureType == GestureType.Tap)
                {
                    if (gesture.Position.Y >= RecStart.Y && gesture.Position.Y <= RecStart.Y + RecStart.Height)
                    {
                        ((Game1)Game).Level = StorageHelper.GetNotCompleteLevel();
                        ((Game1)Game).GoForward(PageType.GamePage);
                    }
                    else if (gesture.Position.Y >= RecSelect.Y && gesture.Position.Y <= RecSelect.Y + RecSelect.Height)
                    {
                        ((Game1)Game).GoForward(PageType.WorldSelectPage);
                    }
                    else if (gesture.Position.Y >= RecHelp.Y && gesture.Position.Y <= RecHelp.Y + RecHelp.Height)
                    {
                        ((Game1)Game).GoForward(PageType.HelpPage);
                        //StorageHelper.SetValue("world", "2");
                        //StorageHelper.SetValue("w0", ",0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,");
                    }
                    else if (gesture.Position.Y >= RecAbout.Y && gesture.Position.Y <= RecAbout.Y + RecAbout.Height)
                    {
                        ((Game1)Game).GoForward(PageType.AboutPage);
                        //StorageHelper.ClearData();
                    }
                    else if (gesture.Position.X < 20 && gesture.Position.Y < 20)
                    {
                        ((Game1)Game).GamePage.SetLevel("reflector");
                        ((Game1)Game).GoForward(PageType.GamePage);
                    }
                    else if (gesture.Position.X > 460 && gesture.Position.Y < 20)
                    {
                        StorageHelper.SetUnlockWorld(2);
                    }
                }
            }
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);

            spriteBatch.Draw(Laser, rec1, null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.6f);
            spriteBatch.Draw(Laser, rec2, null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.6f);
            spriteBatch.Draw(Laser, rec3, null, Color.White, MathHelper.PiOver2, Vector2.Zero, SpriteEffects.None, 0.6f);
            spriteBatch.Draw(Laser, rec4, null, Color.White, MathHelper.PiOver2, Vector2.Zero, SpriteEffects.None, 0.6f);

            foreach (Receiver s in Receivers)
                s.Draw(spriteBatch);
            
            spriteBatch.Draw(Title, RecTitle, Color.White);
            spriteBatch.Draw(StartGame, RecStart, Color.White);
            spriteBatch.Draw(SelectLevel, RecSelect, Color.White);
            spriteBatch.Draw(Help, RecHelp, Color.White);
            spriteBatch.Draw(About, RecAbout, Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
