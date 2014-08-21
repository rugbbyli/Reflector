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
    public class MenuPage : DrawableGameComponent
    {
        SpriteBatch spriteBatch;

        SpriteFont font;

        Vector2 VecMirror, VecPrism;

        Texture2D Background, Top, Bottom, Mirror, Prism, World, Level;
        Rectangle RecMirror, RecPrism, RecTop, RecBottom, RecWorld, RecLevel;

        Button menu, next, dismiss;
        
        Color ForeColor = new Color(0, 0, 0, 0);

        string LevelWay, StrMirror, StrPrism;

        public string Message
        {
            set
            {
                var message = value.Split(',');
                if (message[4][0] == 'T')
                {
                    Top = Game.Content.Load<Texture2D>("Images/Menu/finish");
                    LevelWay = "complete_";

                }
                else
                {
                    Top = Game.Content.Load<Texture2D>("Images/Menu/pause");
                    LevelWay = "notcomplete_";
                }
                StrMirror = "  :  " + message[0].PadLeft(2) + "  /  " + message[1].PadLeft(2);
                StrPrism = "  :  " + message[2].PadLeft(2) + "  /  " + message[3].PadLeft(2);
            }
        }

        public MenuPage(Game game)
            : base(game)
        {
            this.Enabled = false;
            this.Visible = false;
            
            this.VisibleChanged += (s, e) =>
            {
                if (this.Visible == true)
                {
                    Game1 gm = this.Game as Game1;
                    World = Game.Content.Load<Texture2D>("Images/Level/w" + (gm.Level / 20 + 1));
                    int l = gm.Level % 20 + 1;
                    Level = Game.Content.Load<Texture2D>("Images/Level/" + LevelWay + l);
                    ForeColor.R = ForeColor.G = ForeColor.B = ForeColor.A = 0;
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

            Background = Game.Content.Load<Texture2D>("Images/Menu/bg");
            Bottom = Game.Content.Load<Texture2D>("Images/Menu/bottom");
            font = Game.Content.Load<SpriteFont>("Fonts/number");
            Mirror = Game.Content.Load<Texture2D>("Images/MirrorMenu/mirror");
            Prism = Game.Content.Load<Texture2D>("Images/MirrorMenu/prism");

            RecTop = new Rectangle(0, 70, 480, 160);
            RecWorld = new Rectangle(0, 250, 400, 150);
            RecLevel = new Rectangle(370, 295, 50, 50);
            RecMirror = new Rectangle(80, 390, 50, 50);
            VecMirror = new Vector2(130, 390);
            RecPrism = new Rectangle(80, 460, 50, 50);
            VecPrism = new Vector2(130, 460);
            RecBottom = new Rectangle(0, 700, 480, 100);

            menu = new Button(Game.Content, Colors.red, 10, 705, 150, 85);
            dismiss = new Button(Game.Content, Colors.blue, 165, 705, 150, 85);
            next = new Button(Game.Content, Colors.green, 320, 705, 150, 85);

            base.LoadContent();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here
            #region 处理用户输入
            if (TouchPanel.IsGestureAvailable)
            {
                var ges = TouchPanel.ReadGesture();
                Point pt = new Point((int)ges.Position.X, (int)ges.Position.Y);
                if (menu.Contains(pt))
                {
                    this.Visible = false;
                    this.Enabled = false;
                    ((Game1)Game).GoBack();
                }
                else if (dismiss.Contains(pt))
                {
                    this.Visible = false;
                    this.Enabled = false;
                    ((Game1)Game).State = PageType.GamePage;
                }
                else if (next.Contains(pt))
                {
                    this.Enabled = false;
                    this.Visible = false;

                    //此处应该判断是否可以开始下一关，或者在菜单显示的时候进行判断，并隐藏此按钮（必要时）
                    ((Game1)Game).Level += 1;
                    ((Game1)Game).State = PageType.GamePage;
                }
            }
            #endregion

            if (ForeColor.A < 220)
            {
                ForeColor.A += 11;
                ForeColor.R = ForeColor.G = ForeColor.B += 1;
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);

            spriteBatch.Draw(Background, Game.Window.ClientBounds, null, ForeColor, 0, Vector2.Zero, SpriteEffects.None, 0.1f);
            
            spriteBatch.Draw(Top, RecTop, null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0);
            
            spriteBatch.Draw(World, RecWorld, null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0);
            spriteBatch.Draw(Level, RecLevel, null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0);
            spriteBatch.Draw(Mirror, RecMirror, null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0);
            spriteBatch.Draw(Prism, RecPrism, null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0);
            spriteBatch.DrawString(font, StrMirror, VecMirror, Color.White, 0, Vector2.Zero, 0.8f, SpriteEffects.None, 0);
            spriteBatch.DrawString(font, StrPrism, VecPrism, Color.White, 0, Vector2.Zero, 0.8f, SpriteEffects.None, 0);
            spriteBatch.Draw(Bottom, RecBottom, null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.05f);

            menu.Draw(spriteBatch);
            dismiss.Draw(spriteBatch);
            next.Draw(spriteBatch);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
