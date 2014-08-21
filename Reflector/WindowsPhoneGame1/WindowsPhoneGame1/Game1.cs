using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;

namespace Reflector
{
    /// <summary>
    /// 这是游戏的主类型
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        BackgroundPage m_backPage;
        StartPage m_startPage;

        WorldSelectPage m_worldPage;
        public WorldSelectPage WorldPage
        {
            get 
            {
                if (m_worldPage == null)
                {
                    m_worldPage = new WorldSelectPage(this);
                    this.Components.Add(m_worldPage);
                }
                return m_worldPage; 
            }
        }
        
        LevelSelectPage m_levelPage;
        public LevelSelectPage LevelPage
        {
            get
            {
                if (m_levelPage == null)
                {
                    m_levelPage = new LevelSelectPage(this);
                    this.Components.Add(m_levelPage);
                }
                return m_levelPage;
            }
        }

        GamePage m_gamePage;
        public GamePage GamePage
        {
            get
            {
                if (m_gamePage == null)
                {
                    m_gamePage = new GamePage(this);
                    this.Components.Add(m_gamePage);
                } 
                return m_gamePage;
            }
        }

        MenuPage m_menuPage;
        public MenuPage MenuPage
        {
            get
            {
                if (m_menuPage == null)
                {
                    m_menuPage = new MenuPage(this);
                    this.Components.Add(m_menuPage);
                } 
                return m_menuPage;
            }
        }

        HelpPage m_helpPage;
        public HelpPage HelpPage
        {
            get
            {
                if (m_helpPage == null)
                {
                    m_helpPage = new HelpPage(this);
                    this.Components.Add(m_helpPage);
                }
                return m_helpPage;
            }
        }

        AboutPage m_aboutPage;
        public AboutPage AboutPage
        {
            get
            {
                if (m_aboutPage == null)
                {
                    m_aboutPage = new AboutPage(this);
                    this.Components.Add(m_aboutPage);
                } 
                return m_aboutPage;
            }
        }

        DrawableGameComponent CurrentPage;

        byte m_level;
        public byte Level
        {
            get
            {
                return m_level;
            }
            set
            {
                m_level = value;
                this.GamePage.SetLevel((value + 1).ToString());
            }
        }
        public byte World { get; set; }
        
        PageType m_state;
        public PageType State
        {
            get{return m_state;}
            set
            {
                if(CurrentPage != null)
                {
                    CurrentPage.Enabled = false;
                    CurrentPage.Visible = false;
                }
                m_state = value;

                switch(m_state)
                {
                    case PageType.StartPage:
                        CurrentPage = m_startPage;break;
                    case PageType.GamePage:
                        CurrentPage = GamePage;break;
                    case PageType.LevelSelectPage:
                        CurrentPage = LevelPage;break;
                    case PageType.WorldSelectPage:
                        CurrentPage = WorldPage; break;
                    case PageType.AboutPage:
                        CurrentPage = AboutPage; break;
                    case PageType.HelpPage:
                        CurrentPage = HelpPage;break;

                }
                
                CurrentPage.Visible = true;
                CurrentPage.Enabled = true;                
            }
        }

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);

            graphics.PreferredBackBufferHeight = 800;
            graphics.PreferredBackBufferWidth = 480;
            graphics.IsFullScreen = true;
            graphics.SupportedOrientations = DisplayOrientation.Portrait;

            m_backPage = new BackgroundPage(this);
            m_startPage = new StartPage(this);


            Content.RootDirectory = "Content";

            TargetElapsedTime = TimeSpan.FromMilliseconds(33.333333);

            // 延长锁定时的电池寿命。
            InactiveSleepTime = TimeSpan.FromSeconds(1);
        }

        protected override void Initialize()
        {
            // TODO: 在此处添加初始化逻辑
            this.Components.Add(m_startPage);

            this.Components.Add(m_backPage);

            TouchPanel.EnabledGestures = GestureType.Hold | GestureType.Tap;

            GoForward(PageType.StartPage);
            
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void UnloadContent()
        {
            // TODO: 在此处取消加载任何非 ContentManager 内容
        }

        protected override void Update(GameTime gameTime)
        {
            // 允许游戏退出
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            {
                if (MenuPage.Visible)
                {
                    MenuPage.Enabled = false;
                    MenuPage.Visible = false;
                    GamePage.Enabled = true;
                }
                else
                    this.GoBack();
            }

            // TODO: 在此处添加更新逻辑
            
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            base.Draw(gameTime);
        }

        public void ShowMenu(string message)
        {
            GamePage.Enabled = false;
            MenuPage.Message = message;
            MenuPage.Visible = true;
            MenuPage.Enabled = true;
        }

        Stack<PageType> History = new Stack<PageType>();
        public void GoForward(PageType page)
        {
            History.Push(page);
            this.State = page;
        }

        public void GoBack()
        {
            if (History.Count > 0)
            {
                History.Pop();
                if (History.Count == 0)
                {
                    Guide.BeginShowMessageBox("提示", "确定要退出吗？",
                        new string[] { "确定", "取消" }, 1, MessageBoxIcon.None, new AsyncCallback((s) =>
                        {
                            var res  = Microsoft.Xna.Framework.GamerServices.Guide.EndShowMessageBox(s);
                            if (res == 0)
                            {
                                this.Exit();
                            }
                            else
                            {
                                History.Push(PageType.StartPage);
                            }
                        }), null);
                }
                else this.State = History.Peek();
            }
        }
    }

    public enum PageType
    {
        StartPage,
        GamePage,
        WorldSelectPage,
        LevelSelectPage,
        HelpPage,
        AboutPage,
        //MenuPage,
    }
}
