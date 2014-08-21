using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Reflector
{

    public enum Colors
    {
        red = 0,
        blue = 1,
        yellow = 2,
        green = 3,
        orange,
        purple,
        white,
        black,
    }

    abstract class SpriteBase
    {
        public virtual void Update(Laser laser)
        {

        }

        public abstract bool Contains(Point point);
    }

    class Sprite:SpriteBase
    {
        protected Texture2D Foreground;//前景，无焦点时显示
        protected Texture2D Background;//背景，获得焦点时显示

        public bool Visible = true;

        Vector2 m_origin;
        public Vector2 Origin
        {
            get { return m_origin; }
            protected set
            {
                m_origin = value;
                X += (int)m_origin.X;
                Y += (int)m_origin.Y;
            }
        }
        Rectangle m_destRectangle;
        public Rectangle Rectangle
        {
            get { return m_destRectangle; }
            //private set { m_destRectangle = value; }
        }

        public int X
        {
            get { return m_destRectangle.X; }
            protected set 
            { 
                m_destRectangle.X = value;
            }
        }
        public int Y
        {
            get { return m_destRectangle.Y; }
            protected set 
            { 
                m_destRectangle.Y = value;
            }
        }
        public int Width
        {
            get { return m_destRectangle.Width; }
            protected set 
            { 
                m_destRectangle.Width = value;
            }
        }
        public int Height
        {
            get { return m_destRectangle.Height; }
            protected set 
            { 
                m_destRectangle.Height = value;
            }
        }
        float m_angle;
        /// <summary>
        /// Angle一定是一个介于0到360之间的角度值
        /// </summary>
        public float Angle
        {
            get { return m_angle; }
            set
            {
                m_angle = (value + 360) % 360;
            }
        }

        Colors m_color;
        public virtual Colors Color
        {
            get { return m_color; }
            protected set { m_color = value; }
        }

        bool m_focused;
        public virtual bool Focused
        {
            get { return m_focused; }
            set { m_focused = value; }
        }
        
        protected float Depth = 0.5f;

        public Sprite(int x ,int y, int width, int height, Colors color):this(x,y,width,height)
        {
            m_color = color;
        }

        public Sprite(int x, int y, int width, int height)
        {
            Focused = false;
            Angle = 0;
            m_destRectangle = new Rectangle(x, y, width, height);
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (Focused) spriteBatch.Draw(Background, m_destRectangle, null, Microsoft.Xna.Framework.Color.White, MathHelper.ToRadians(Angle), m_origin, SpriteEffects.None, Depth);
            else spriteBatch.Draw(Foreground, m_destRectangle, null, Microsoft.Xna.Framework.Color.White, MathHelper.ToRadians(Angle), m_origin, SpriteEffects.None, Depth);
        }

        public override bool Contains(Point point)
        {
            point.X += (int)this.Origin.X;
            point.Y += (int)this.Origin.Y;
            return Rectangle.Contains(point);
        }
    }

    /// <summary>
    /// 激光
    /// </summary>
    class Laser : Sprite
    {
        ContentManager Content;

        public void SetLength(int length)
        {
            this.Width = length;
            foreRectangle.Width = length;
        }

        public void SetColor(Colors color)
        {
            this.Color = color;
            this.Foreground = Content.Load<Texture2D>(@"Images/Laser/" + color);
            this.Background = Content.Load<Texture2D>(@"Images/Laser/selected_" + color);
        }

        public object Father;

        Point End;
        public Point EndPoint
        {
            get
            {
                return End;
            }
            set
            {
                End = value;
                double length = Math.Pow(End.X - this.X, 2) + Math.Pow(End.Y - this.Y, 2);
                this.SetLength((int)Math.Sqrt(length));
            }
        }

        Rectangle foreRectangle;
        Vector2 foreOrigin;

        public Laser(object father, ContentManager content, Colors color, Point start, Point end)
            : this(father, content, color, (int)start.X, (int)start.Y, 800, 20)
        {
            this.End = end;

            Angle = (float)Math.Asin((end.Y - start.Y) / this.Width);
            if (start.X > end.X) Angle = 180 - Angle;

        }

        public Laser(object father, ContentManager content, Colors color, int x, int y, float angle)
            : this(father, content, color,x,y,800,20)
        {
            Angle = angle;
        }

        private Laser(object father, ContentManager content, Colors color, int x, int y, int width, int height)
            :base(x,y,width,height,color)
        {
            this.Content = content;
            this.Foreground = content.Load<Texture2D>(@"Images/Laser/" + color);
            this.Background = content.Load<Texture2D>(@"Images/Laser/selected_" + color);
            this.Father = father;
            this.Depth = 0.6f;
            Origin = new Vector2(0, 20);
            this.Y -= 10;
            foreOrigin = new Vector2(0, 10);
            foreRectangle = new Rectangle(this.X,this.Y,this.Width,10);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!Focused)
            {
                spriteBatch.Draw(Foreground, foreRectangle, null, Microsoft.Xna.Framework.Color.White, MathHelper.ToRadians(Angle), foreOrigin, SpriteEffects.None, Depth);
            }
            else
                base.Draw(spriteBatch);
        }
    }

    /// <summary>
    /// 激光发射器
    /// </summary>
    class Emitter : Sprite
    {
        public Laser Laser { get; private set; }

        public Emitter(ContentManager content, Colors color, int x, int y, float angle)
            :base(x,y,60,60)
        {
            this.Foreground = content.Load<Texture2D>(@"Images/Emitter/" + color.ToString());
            this.Origin = new Vector2(Width / 2, Height / 2);

            //0向上/90向右/180向下/270向左
            this.Angle = angle;

            this.Laser = new Laser(this, content, color, this.X, this.Y - 10, angle - 90);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Laser.Draw(spriteBatch);
            base.Draw(spriteBatch);
        }
    }

    /// <summary>
    /// 接收器
    /// </summary>
    class Receiver : Sprite
    {
        public Receiver(ContentManager content , Colors color, int x ,int y):base(x,y,60,60,color)
        {
            this.Foreground = content.Load<Texture2D>(@"Images/Receiver/" + color.ToString());
            this.Background = content.Load<Texture2D>(@"Images/Receiver/selected_" + color.ToString());
        }

        public override void Update(Laser laser)
        {
            if (laser.Color == this.Color)
            {
                this.In = laser;
                this.Focused = true;
            }
        }

        Laser In;
        public void CheckState()
        {
            if (In == null || In.Father == null || !this.Contains(In.EndPoint))
            {
                this.In = null;
                this.Focused = false;
            }
        }
    }

    /// <summary>
    /// 开关
    /// </summary>
    class Switch : Sprite
    {
        public Switch(ContentManager content, Colors color, int x,int y):base(x,y,60,60,color)
        {
            this.Foreground = content.Load<Texture2D>(@"Images/Switch/" + color.ToString() + "_off");
            this.Background = content.Load<Texture2D>(@"Images/Switch/" + color.ToString() + "_on");
        }

        public override void Update(Laser laser)
        {
            if (laser.Color == this.Color)
            {
                this.In = laser;
                this.Focused = true;
            }
        }

        public Laser In { get; private set; }
        public void CheckState()
        {
            if (In == null || In.Father == null || !this.Contains(In.EndPoint))
            {
                this.In = null;
                this.Focused = false;
            }
        }
    }

    /// <summary>
    /// 墙和门
    /// </summary>
    class Wall : Sprite
    {
        public Wall(ContentManager content, Colors color, int x,int y):base(x,y,60,60,color)
        {
            Foreground = content.Load<Texture2D>(@"Images/Wall/" + color.ToString());
            sourceRectangle = new Rectangle(5, 5, 60, 60);
        }

        Rectangle sourceRectangle;
        public override void Draw(SpriteBatch spriteBatch)
        {
            if(this.Visible)
                spriteBatch.Draw(Foreground, Rectangle, sourceRectangle, Microsoft.Xna.Framework.Color.White, MathHelper.ToRadians(Angle), Origin, SpriteEffects.None, Depth);
        }
    }

    class Level : Sprite
    {
        public byte Number { get; private set; }

        public Level(ContentManager content, byte level, int x,int y,int width,int height):base(x,y,width,height)
        {
            this.Number = level;
            this.Foreground = content.Load<Texture2D>(@"Images/Level/notcomplete_" + (level + 1));
            this.Background = content.Load<Texture2D>(@"Images/Level/complete_" + (level + 1));
        }
    }
}
