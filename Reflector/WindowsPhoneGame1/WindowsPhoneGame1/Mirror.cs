using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reflector
{
    interface LightableObject
    {
        void CheckLaser();
    }

    /// <summary>
    /// 所有镜片的基类
    /// </summary>
    abstract class Lens:Sprite,LightableObject
    {
        protected WorkingArea Father;
        protected Texture2D Background2;

        private Texture2D ArrowIn;
        private Texture2D ArrowOut;
        private Texture2D ArrowExtra;
        Vector2 ArrowOrigin = new Vector2(16, 16);
        Rectangle RecArrowIn;
        Rectangle RecArrowOut;
        Rectangle RecArrowExtra;
        
        public bool CanInput = true;
        Laser m_in;
        Laser m_out;
        Laser m_extra;
        public Laser In
        {
            get
            {
                return m_in;
            }
            set
            {
                
                m_in = value;
                
            }
        }
        public Laser Out
        {
            get
            {
                return m_out;
            }
            protected set
            {
                
                m_out = value;

            }
        }
        public Laser Extra
        {
            get
            {
                return m_extra;
            }
            set
            {
                m_extra = value;
            }
        }
        Rectangle m_backRectangle;

        protected Lens(WorkingArea father,int x, int y):base(x,y,60,60)
        {
            this.Father = father;
            Background = father.Content.Load<Texture2D>("Images/MirrorMenu/circle");
            Background2 = father.Content.Load<Texture2D>("Images/MirrorMenu/circle_red");
            
            m_backRectangle = new Rectangle(X - 90, Y - 90, 180, 180);
            //Origin = new Vector2(30, 30);
        }

        public void SetPosition(int x, int y)
        {
            this.X = x;
            this.Y = y;
            this.m_backRectangle.X = x - 90;
            this.m_backRectangle.Y = y - 90;
        }

        public abstract void RotateLeft();
        public abstract void RotateRight();
        internal abstract void ClearLaser();

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Focused)
            {
                if (CanInput)
                    spriteBatch.Draw(Background, m_backRectangle, null, Microsoft.Xna.Framework.Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.3f);
                else
                    spriteBatch.Draw(Background2, m_backRectangle, null, Microsoft.Xna.Framework.Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.3f);
            }
            spriteBatch.Draw(Foreground, Rectangle, null, Microsoft.Xna.Framework.Color.White, MathHelper.ToRadians(Angle), Origin, SpriteEffects.None, 0.4f);
            if (In != null)
            {
                if(Focused) spriteBatch.Draw(ArrowIn, RecArrowIn, null, Microsoft.Xna.Framework.Color.White, MathHelper.ToRadians(In.Angle + 90), ArrowOrigin, SpriteEffects.None, 0.2f);

            }
            if (Out != null)
            {
                Out.Draw(spriteBatch);
                if(Focused) spriteBatch.Draw(ArrowOut, RecArrowOut, null, Microsoft.Xna.Framework.Color.White, MathHelper.ToRadians(Out.Angle + 90), ArrowOrigin, SpriteEffects.None, 0.2f);
            }
            if (Extra != null)
            {
                Extra.Draw(spriteBatch);
                if(Focused) spriteBatch.Draw(ArrowExtra, RecArrowExtra, null, Microsoft.Xna.Framework.Color.White, MathHelper.ToRadians(Extra.Angle + 90), ArrowOrigin, SpriteEffects.None, 0.2f);
            }

            
        }

        private Rectangle GetArrowPosition(float angle)
        {

            double y = 70 * Math.Sin(MathHelper.ToRadians(angle));
            double x = 70 * Math.Cos(MathHelper.ToRadians(angle));

            return new Rectangle(this.X + (int)x, this.Y + (int)y, 32, 32);
        }

        public virtual void CheckLaser()
        {
            if (In != null)
            {
                ArrowIn = Father.Content.Load<Texture2D>("Images/MirrorMenu/Arrow/" + In.Color.ToString());
                RecArrowIn = this.GetArrowPosition((In.Angle + 180) % 360);
            }

            if (Out != null)
            {
                ArrowOut = Father.Content.Load<Texture2D>("Images/MirrorMenu/Arrow/" + Out.Color.ToString());
                RecArrowOut = this.GetArrowPosition(Out.Angle);
            }

            if (Extra != null)
            {
                ArrowExtra = Father.Content.Load<Texture2D>("Images/MirrorMenu/Arrow/" + Extra.Color.ToString());
                float angle = Extra.Angle;
                if ((this as Prism).Type == Prism.PrismType.Blend) angle = (angle + 180) % 360;
                RecArrowExtra = this.GetArrowPosition(angle);
            }
        }
    }

    /// <summary>
    /// 平面反光镜
    /// </summary>
    class Mirror:Lens
    {
        public override void RotateLeft()
        {
            base.Angle -= 22.5f;
            Update(this.In);
        }

        public override void RotateRight()
        {
            base.Angle += 22.5f;
            Update(this.In);
        }

        public Mirror(WorkingArea father, int x, int y)
            : base(father, x, y)
        {
            Foreground = Father.Content.Load<Texture2D>("Images/MirrorMenu/mirror");
            this.Origin = new Vector2(Foreground.Width / 2, Foreground.Height / 2);
        }

        public override void CheckLaser()
        {
            if (In == null || In.Father == null || !this.Contains(In.EndPoint))
            {
                this.ClearLaser();
            }
            base.CheckLaser();
        }

        internal override void ClearLaser()
        {
            //this.In = null;
            if (this.Out != null)
            {
                this.Out.Father = null;
                this.Out = null;
            }
        }

        public override void Update(Laser laser)
        {
            if (laser == null) return;
            if (Out != null && In != laser) return;
            this.In = laser;
            if (//不能反射，Out置为Null
                //当镜子角度大于180度时，入射光线满足 M - 180 < L < M时反射
                (this.Angle >= 180 && (laser.Angle <= this.Angle - 180 || laser.Angle >= this.Angle))
                ||
                //当镜子角度小于180度时，入射光线满足 M <= L <= M + 180时不反射
                (this.Angle <= 180 && (laser.Angle >= this.Angle && laser.Angle <= this.Angle + 180))
                || Math.Abs(this.Angle - laser.Angle - 90) % 360 < 0.1
               )
            {
                ClearLaser();
            }
            else//可以反射，看Out是否为空，并改变角度
            {
                if (Out == null)
                {
                    this.Out = new Laser(this, this.Father.Content, laser.Color, laser.EndPoint.X, laser.EndPoint.Y - 10, 2 * this.Angle - laser.Angle);
                }
                else
                {
                    Out.Angle = 2 * this.Angle - laser.Angle;
                }
                Father.UpdateLaser(Out);
            }


        }
    }

    /// <summary>
    /// 分光镜
    /// </summary>
    class Prism : Lens
    {
        public enum PrismType
        {
            /// <summary>
            /// 合成
            /// </summary>
            Blend,
            /// <summary>
            /// 分解
            /// </summary>
            Resolve,
        }
        public PrismType Type;
        
        Laser Selected;

        public Prism(WorkingArea father, int x, int y)
            : base(father, x, y)
        {
            Foreground = Father.Content.Load<Texture2D>("Images/MirrorMenu/prism");
            this.Type = PrismType.Resolve;
            this.Origin = new Vector2(this.Foreground.Width / 2, this.Foreground.Height / 2);
        }

        public override void RotateLeft()
        {
            if (Selected != null)
            {
                float angle = (Selected.Angle - 45 + 360) % 360;
                while (this.HasAngle(angle))
                {
                    angle = (angle - 45 + 360) % 360;
                }
                Selected.Angle = angle;
                Father.UpdateLaser(Selected);
            }
        }

        public override void RotateRight()
        {
            if (Selected != null)
            {
                float angle = (Selected.Angle + 45) % 360;
                while (this.HasAngle(angle))
                {
                    angle = (angle + 45) % 360;
                }
                Selected.Angle = angle;
                Father.UpdateLaser(Selected);
            }
        }

        private bool HasAngle(float angle)
        {
            if (In != null && Math.Abs(Math.Abs(angle - In.Angle) - 180) < 0.1) return true;
            if (Selected == Extra)
            {
                if (Math.Abs(angle - Out.Angle) < 0.1) return true;
            }
            else
            {
                if (this.Type == PrismType.Blend)
                {
                    if (Math.Abs(Math.Abs(angle - Extra.Angle) - 180) < 0.1)  return true;
                }
                else
                {
                    if (Math.Abs(angle - Extra.Angle) < 0.1) return true;
                } 
            }
                
            return false;
        }

        public void ChangeSelect()
        {
            if (Type == PrismType.Resolve && Selected != null)
            {
                Selected.Focused = false;
                Selected = (Selected == Extra) ? Out : Extra;
                Selected.Focused = true;
            }
        }

        public override bool Focused
        {
            get
            {
                return base.Focused;
            }
            set
            {
                base.Focused = value;
                if (Selected != null) Selected.Focused = value;
            }
        }

        public override void Update(Laser laser)
        {
            //光线合成公式：
            //红黄蓝是单色/绿橙紫白是混合色
            //红+黄=橙/红+蓝=紫/蓝+黄=绿/三色合成白
            //白分解还是白

            if (this.Out == null || this.Type == PrismType.Resolve)
            {
                #region 一束光入射
                if (this.Out == null)
                {
                    this.Type = PrismType.Resolve;
                    In = laser;

                    switch (laser.Color)
                    {
                        case Colors.red:
                        case Colors.yellow:
                        case Colors.blue:
                        case Colors.white:
                            Out = new Laser(this, this.Father.Content, laser.Color, laser.EndPoint.X, laser.EndPoint.Y - 10, laser.Angle + 45);
                            Extra = new Laser(this, this.Father.Content, laser.Color, laser.EndPoint.X, laser.EndPoint.Y - 10, laser.Angle - 45);
                            break;
                        case Colors.orange:
                            Out = new Laser(this, this.Father.Content, Colors.red, laser.EndPoint.X, laser.EndPoint.Y - 10, laser.Angle + 45);
                            Extra = new Laser(this, this.Father.Content, Colors.yellow, laser.EndPoint.X, laser.EndPoint.Y - 10, laser.Angle - 45);
                            break;
                        case Colors.purple:
                            Out = new Laser(this, this.Father.Content, Colors.red, laser.EndPoint.X, laser.EndPoint.Y - 10, laser.Angle + 45);
                            Extra = new Laser(this, this.Father.Content, Colors.blue, laser.EndPoint.X, laser.EndPoint.Y - 10, laser.Angle - 45);
                            break;
                        case Colors.green:
                            Out = new Laser(this, this.Father.Content, Colors.blue, laser.EndPoint.X, laser.EndPoint.Y - 10, laser.Angle + 45);
                            Extra = new Laser(this, this.Father.Content, Colors.yellow, laser.EndPoint.X, laser.EndPoint.Y - 10, laser.Angle - 45);
                            break;

                    }
                    if(this.Focused) Out.Focused = true;
                    Selected = Out;
                }
                #endregion

                #region 两束光入射
                else
                {
                    if (laser == In) ;
                    else if (laser.Color == In.Color)
                    {
                        Extra.Father = null;
                        Extra = laser;
                        this.Type = PrismType.Blend;
                        Out.SetColor(In.Color);
                    }
                    else
                    {
                        this.Type = PrismType.Blend;
                        Extra.Father = null;
                        Extra = laser;
                        int lc = (int)laser.Color;
                        int ic = (int)In.Color;
                        Colors color;
                        if (lc < 3 && ic < 3)
                        {
                            if (lc + ic == 1) color = Colors.purple;
                            else if (lc + ic == 2) color = Colors.orange;
                            else color = Colors.green;
                        }
                        else color = Colors.white;

                        if (Math.Abs(Math.Abs(Out.Angle - Extra.Angle) - 180) < 1)
                        {
                            float angle = Math.Abs(this.In.Angle - this.Extra.Angle);
                            if (angle > 180)
                            {
                                angle = 360 - angle;
                                angle = angle / 2 + Math.Max(this.In.Angle, this.Extra.Angle);
                            }
                            else
                            {
                                angle = angle / 2 + Math.Min(this.In.Angle, this.Extra.Angle);
                            }

                            angle = (int)(angle / 45) * 45;
                            Out.Angle = angle;
                        }
                        Out.SetColor(color);
                        
                        //Out.Father = null;
                        //Out = new Laser(this, this.Father.Content, color, In.EndPoint.X, In.EndPoint.Y - 10, angle);
                        Out.Focused = this.Focused;
                        Selected = Out;
                    }
                }
                #endregion
            }
            Father.UpdateLaser(Out);
            if (this.Type == PrismType.Resolve) Father.UpdateLaser(Extra);
        }

        public override void CheckLaser()
        {
            if (this.Type == PrismType.Resolve)
            {
                if (In == null || In.Father == null || !this.Contains(In.EndPoint))
                {
                    this.ClearLaser();
                }
            }
        
            else
            {
                int exists = 2;
                if (Extra == null || Extra.Father == null || !this.Contains(Extra.EndPoint))
                {
                    Extra = null;
                    Selected = In;
                    exists -= 1;
                }
                if (In == null || In.Father == null || !this.Contains(In.EndPoint))
                {
                    In = null;
                    Selected = Extra;
                    exists -= 1;
                }
                //只剩一束光存在，状态由合成变成分解，Update
                if (exists == 1)
                {
                    Out.Father = null;
                    Out = null;
                    In = null;
                    this.Update(Selected);
                }
                //两束入射光都不存在了，执行ClearLaser
                else if (exists == 0)
                {
                    ClearLaser();
                }
            }

            base.CheckLaser();
        }

        internal override void ClearLaser()
        {
            this.Selected = null;
            if (this.Out != null)
            {
                this.Out.Father = null;
                this.Out = null;
            }
            if (this.Extra != null && this.Type == PrismType.Resolve)
            {
                this.Extra.Father = null;
                this.Extra = null;
            }
            this.Type = PrismType.Resolve;
        }
    }
}
