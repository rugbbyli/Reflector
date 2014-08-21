using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reflector
{
    class Hole:SpriteBase, LightableObject
    {
        #region 黑洞的组成单位，单个洞
        class m_Hole:Sprite
        {
            int LastTime = 0;
            int RefreshInterval = 100;

            Vector2 ForeOrigin;

            Dictionary<Laser, Laser> m_lasers = new Dictionary<Laser, Laser>();
            /// <summary>
            /// 集合中，键是另一个洞的入射光，值是对应的自身的出射光
            /// </summary>
            public Dictionary<Laser, Laser> Lasers
            {
                get
                {
                    return m_lasers;
                }
            }

            public m_Hole(ContentManager content, int x, int y)
                : base(x, y, 60, 60)
            {
                Background = content.Load<Texture2D>("Images/hole");
                Foreground = content.Load<Texture2D>("Images/holefg");
                Depth = 0.7f;
                Origin = new Vector2(30, 30);
                ForeOrigin = new Vector2(40, 40);
            }

            public void Update(GameTime gameTime)
            {
                LastTime += gameTime.ElapsedGameTime.Milliseconds;

                if (LastTime >= RefreshInterval)
                {
                    LastTime -= RefreshInterval;
                    Angle += 0.05f;
                }
            }

            public override void Draw(SpriteBatch spriteBatch)
            {
                spriteBatch.Draw(Foreground, Rectangle, null, Microsoft.Xna.Framework.Color.White, 0, ForeOrigin, SpriteEffects.None, 0.5f);
                spriteBatch.Draw(Background, Rectangle, null, Microsoft.Xna.Framework.Color.White, Angle, Origin, SpriteEffects.None, Depth);
                foreach (var v in m_lasers)
                    v.Value.Draw(spriteBatch);
            }
        }
        #endregion

        WorkingArea Father;
        m_Hole hole1, hole2;

        public Hole(WorkingArea father, int x1, int y1, int x2, int y2)
        {
            Father = father;
            hole1 = new m_Hole(Father.Content, x1, y1);
            hole2 = new m_Hole(Father.Content, x2, y2);
        }

        public IEnumerable<Laser> In
        {
            get
            {
                return Enumerable.Concat<Laser>(hole1.Lasers.Keys, hole2.Lasers.Keys);
            }
        }

        public override void Update(Laser laser)
        {
            if (this.In.Contains(laser))
            {
                foreach (var l in hole1.Lasers.Values)
                    Father.UpdateLaser(l);
                foreach (var l in hole2.Lasers.Values)
                    Father.UpdateLaser(l);
            }
            else
            {
                Laser added = null;
                if (hole1.Contains(laser.EndPoint))
                {
                    if (!hole2.Lasers.Keys.Contains(laser))
                    {
                        added = new Laser(this, Father.Content, laser.Color, hole2.X, hole2.Y - 10, laser.Angle);
                        hole2.Lasers.Add(laser, added);
                    }
                }
                else
                {
                    if (!hole1.Lasers.Keys.Contains(laser))
                    {
                        added = new Laser(this, Father.Content, laser.Color, hole1.X, hole1.Y - 10, laser.Angle);
                        hole1.Lasers.Add(laser, added);
                    }
                }
                Father.UpdateLaser(added);
            }
            
        }

        public void Update(GameTime gameTime)
        {
            hole1.Update(gameTime);
            hole2.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            hole1.Draw(spriteBatch);
            hole2.Draw(spriteBatch);
        }

        public void CheckLaser()
        {
            var list = hole1.Lasers.Keys.ToList();
            foreach (Laser l in list)
            {
                if (l.Father == null || !hole2.Contains(l.EndPoint))
                {
                    hole1.Lasers[l].Father = null;
                    hole1.Lasers.Remove(l);
                }
            }

            list = hole2.Lasers.Keys.ToList();
            foreach (Laser l in list)
            {
                if (l.Father == null || !hole1.Contains(l.EndPoint))
                {
                    hole2.Lasers[l].Father = null;
                    hole2.Lasers.Remove(l);
                }
            }
        }

        public override bool Contains(Point pt)
        {
            return hole1.Contains(pt) || hole2.Contains(pt);
        }
    }
}
