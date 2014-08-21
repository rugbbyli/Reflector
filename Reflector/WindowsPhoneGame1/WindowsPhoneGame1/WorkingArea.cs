using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;

namespace Reflector
{
    public class WorkingArea
    {
        public bool Finished { get; private set; }
        public event EventHandler LevelFinish;
        void OnLevelFinish()
        {
            Finished = true;
            if (Current != null)
            {
                Current.Focused = false;
                Current = null;
            }
            if (LevelFinish != null)
                this.LevelFinish(this, EventArgs.Empty);
        }

        /// <summary>
        /// 获取当前游戏信息，格式为：[mirror used],[mirror total],[prism used],[prism total],[level finished?]
        /// </summary>
        public string GameMessage
        {
            get
            {
                int mirrorused = 0, prismused;
                foreach (var v in Lenses)
                    if (v is Mirror) mirrorused++;
                prismused = Lenses.Count - mirrorused;
                return mirrorused + "," + (mirrorused + ControlPanel.MirrorCount) + "," + prismused + "," + (prismused + ControlPanel.PrismCount) + "," + Finished.ToString();

            }
        }
        public ContentManager Content { get; private set; }
        ControlPanel ControlPanel;
        int X;
        int Y;
        List<Lens> Lenses = new List<Lens>();
        List<Emitter> Emitters = new List<Emitter>();
        List<Receiver> Receivers = new List<Receiver>();
        List<Wall> Walls = new List<Wall>();
        List<Switch> Switches = new List<Switch>();

        Hole Hole1;
        
        Lens current;
        Lens Current
        {
            get
            {
                return current;
            }
            set
            {
                current = value;
                if (current == null)
                    ControlPanel.Style = ControlPanelStyle.Panel;
                else
                    ControlPanel.Style = ControlPanelStyle.Control;
            }
        }

        public WorkingArea(ContentManager content)
        {
            X = 0; Y = 10;
            Content = content;
            Finished = false;
        }

        /// <summary>
        /// 检查指定位置是否可以放置新的元素
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public bool CanPutIn(Point pt)
        {
            if (pt.Y > this.Y + 600) return false;

            foreach (var v in Emitters)
                if (v.Contains(pt)) return false;
            foreach (var v in Receivers)
                if (v.Contains(pt)) return false;
            foreach (var v in Walls)
                if (v.Contains(pt) && v.Visible == true) return false;
            foreach (var v in Lenses)
                if (v.Contains(pt) && v != Current) return false;
            foreach (var v in Switches)
                if (v.Contains(pt)) return false;

            if (Hole1 != null)
            {
                if (Hole1.Contains(pt)) return false;
            }

            return true;
        }

        /// <summary>
        /// 返回指定位置的元素，如果不存在，返回Null
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        SpriteBase FindChildWithPoint(Point pt)
        {
            if (pt.Y > this.Y + 600) return null;

            foreach (var v in Emitters)
                if (v.Contains(pt)) return v;
            foreach (var v in Receivers)
                if (v.Contains(pt)) return v;
            foreach (var v in Walls)
                if (v.Contains(pt) && v.Visible == true) return v;
            foreach (var v in Lenses)
                if (v.Contains(pt)) return v;
            foreach (var v in Switches)
                if (v.Contains(pt)) return v;

            if (Hole1!= null)
            {
                if(Hole1.Contains(pt)) return Hole1;
            }

            return null;
        }

        public void Update(GameTime gameTime)
        {
            HandleUserInput();

            if (Hole1 != null)
            {
                Hole1.Update(gameTime);
            }

            return;
        }

        /// <summary>
        /// 沿光线的路径找到它的终点，并通知终点物体
        /// </summary>
        internal void UpdateLaser(Laser laser)
        {
            if (laser == null) return;
            Point offset;

            if (laser.Angle < 90 || laser.Angle > 270) offset.X = 60;
            else if (laser.Angle > 90 && laser.Angle < 270) offset.X = -60;
            else offset.X = 0;
            if (laser.Angle > 0 && laser.Angle < 180) offset.Y = 60;
            else if (laser.Angle > 180 && laser.Angle < 360) offset.Y = -60;
            else offset.Y = 0;

            Point position = new Point(laser.X + offset.X, laser.Y + offset.Y);
            SpriteBase sprite;
            while (position.X > 0 && position.X < 480 && position.Y > this.Y && position.Y < this.Y + 600)
            {
                if ((sprite = this.FindChildWithPoint(position)) != null)
                {
                    laser.EndPoint = position;
                    if (sprite is Wall) return;
                    sprite.Update(laser);
                    return;
                }
                position.X += offset.X;
                position.Y += offset.Y;
            }
            position.X -= offset.X / 2;
            position.Y -= offset.Y / 2;
            laser.EndPoint = position;
        }

        /// <summary>
        /// 刷新从发射器发出的光线
        /// </summary>
        private void UpdateAllEmitter()
        {
            foreach (var v in Emitters)
                UpdateLaser(v.Laser);
        }

        /// <summary>
        /// 遍历所有元素，检查他们的入射光线的有效性
        /// </summary>
        public void CheckState()
        {
            #region 检查开关
            if (Switches.Count > 0)
            {
                foreach (var swi in Switches)
                {
                    Stack<LightableObject> fathers = new Stack<LightableObject>();
                    if (swi.In != null)
                    {
                        var f = swi.In.Father as LightableObject;
                        while (f != null)
                        {
                            fathers.Push(f);
                            if (f is Lens)
                                f = (f as Lens).In.Father as LightableObject;
                            else
                            {
                                f = null;
                            }
                        }
                        while (fathers.Count > 0)
                            fathers.Pop().CheckLaser();

                        swi.CheckState();
                        #region 开门和关门
                        if (swi.Focused)
                        {
                            foreach (Wall w in Walls)
                            {
                                if (w.Color == swi.Color)
                                {
                                    w.Visible = false;
                                    //openDoor = true;
                                }
                            }
                        }
                        else
                        {
                            foreach (Wall w in Walls)
                                if (w.Color == swi.Color)
                                {
                                    var obj = this.FindChildWithPoint(new Point(w.X, w.Y));
                                    if (obj != null)
                                    {
                                        var lens = obj as Lens;
                                        lens.ClearLaser();
                                        this.Lenses.Remove(lens);

                                        if (lens is Prism) ControlPanel.PrismCount += 1;
                                        else if (lens is Mirror) ControlPanel.MirrorCount += 1;

                                        w.Visible = true;

                                        UpdateLaser(lens.In);

                                        var prism = lens as Prism;
                                        if (prism != null && prism.Type == Prism.PrismType.Blend)
                                        {
                                            UpdateLaser(prism.Extra);
                                        }
                                    }
                                    else
                                    {
                                        w.Visible = true;
                                    }
                                }
                        }
                        #endregion
                    }
                }

                //暂时先刷新全部发射器
                UpdateAllEmitter();
            }
            #endregion

            #region 检查镜片和洞
            List<LightableObject> Checked = new List<LightableObject>();
            foreach (Lens le in Lenses)
            {
                m_CheckLH(le, Checked);
            }
            m_CheckLH(Hole1, Checked);
            #endregion

            #region 最后再检查接收器
            bool finish = true;
            foreach (var rec in Receivers)
            {
                rec.CheckState();
                if (!rec.Focused) finish = false; 
            }
            //关卡完成
            if (finish == true)
            {
                OnLevelFinish();
            }
            #endregion
        }

        private void m_CheckLH(LightableObject LH, List<LightableObject> list)
        {
            if (LH == null) return;
            //if (!(LH is Lens) && !(LH is Hole)) return;
            if (list.Contains(LH)) return;

            list.Add(LH);

            if (LH is Lens)
            {
                var lens = LH as Lens;
                if (lens.In != null)
                {
                    var father = lens.In.Father as LightableObject;
                    m_CheckLH(father, list);
                }
                if (lens.Extra != null && (lens as Prism).Type == Prism.PrismType.Blend)
                {
                    var father = lens.Extra.Father as LightableObject;
                    m_CheckLH(father, list);
                }
                
                lens.CheckLaser();

                //if (lens.Out != null)
                //{
                //    var child = this.FindChildWithPoint(lens.Out.EndPoint) as LightableObject;
                //    m_CheckLH(child, list);
                //}
                //if (lens.Extra != null && (lens as Prism).Style == Prism.PrismStyle.分解)
                //{
                //    var child = this.FindChildWithPoint(lens.Extra.EndPoint) as LightableObject;
                //    m_CheckLH(child, list);
                //}
            }
            else if (LH is Hole)
            {
                var hole = LH as Hole;
                foreach (Laser l in hole.In)
                {
                    m_CheckLH(l.Father as LightableObject, list);
                }
                hole.CheckLaser();
            }

        }

        bool MouseDown = false;
        Point FirstPoint;
        bool IsDraging = false;
        Point Offset;
        /// <summary>
        /// 处理用户输入
        /// </summary>
        private void HandleUserInput()
        {
            MouseState state = Mouse.GetState();

            #region 按下操作，需要找到按下内容，可以为空
            if (state.LeftButton == ButtonState.Pressed && MouseDown == false)
            {
                MouseDown = true;
                FirstPoint = new Point(state.X, state.Y);

                if (Current != null)
                {
                    if (state.Y > ControlPanel.Position.Y)
                    {
                        Offset.Y = 1000;
                        return;
                    }
                    //触摸点不是当前点，取消当前焦点
                    if (!Current.Contains(FirstPoint))
                    {
                        Current.Focused = false;
                        Current = null;
                    }
                    else
                    {
                        if (Current is Prism) (Current as Prism).ChangeSelect();
                    }
                }

                #region 如果当前点为空，寻找新的当前点
                if (Current == null)
                {
                    //在已有控件找
                    if (FirstPoint.Y < this.Y + 600)
                    {
                        foreach(var le in Lenses)
                        {
                            if(le.Contains(FirstPoint))
                            {
                                Current = le;
                                le.Focused = true;
                                break;
                            }
                        }
                    }
                    //触摸点落在控制面板中
                    else if (FirstPoint.Y > ControlPanel.Position.Y)
                    {
                        int where = ControlPanel.PointInWhere(FirstPoint);
                        switch (where)
                        {
                            case 1:
                                if (ControlPanel.PrismCount > 0)
                                {
                                    Current = new Prism(this, ControlPanel.Rec1.X, ControlPanel.Rec1.Y);
                                    ControlPanel.PrismCount -= 1;
                                }
                                break;
                            case 2:
                                if (ControlPanel.MirrorCount > 0)
                                {
                                    Current = new Mirror(this, ControlPanel.Rec2.X, ControlPanel.Rec2.Y);
                                    ControlPanel.MirrorCount -= 1;
                                }
                                break;

                        }
                        if (Current != null)
                        {
                            Current.Focused = true;
                            this.Lenses.Add(Current);
                            Current.CanInput = false;
                        }
                    }
                }
                #endregion

                if (Current != null)
                {
                    Offset.X = FirstPoint.X - Current.X;
                    Offset.Y = FirstPoint.Y - Current.Y;
                }
            }
            #endregion

            #region 释放操作，需要判断是单击还是拖拽结束
            else if (state.LeftButton == ButtonState.Released)
            {
                if (MouseDown == true)
                {
                    if (Current != null && !Current.CanInput)
                    {
                        this.Lenses.Remove(Current);
                        if (Current is Prism) ControlPanel.PrismCount += 1;
                        else if (Current is Mirror) ControlPanel.MirrorCount += 1;

                        UpdateLaser(Current.In);
                        UpdateLaser(Current.Extra);
                        CheckState();

                        Current = null;
                        return;
                    }
                    //判断是否单击了控制面板控制按钮
                    if (!IsDraging)
                    {
                        
                        int where = ControlPanel.PointInWhere(FirstPoint);
                        if (where != 0)
                        {
                            if (ControlPanel.Style == ControlPanelStyle.Control)
                            {
                                if (where == 1)
                                {
                                    Current.RotateLeft();
                                    
                                }
                                else if (where == 2)
                                {
                                    Current.RotateRight();
                                }
                                else if (where == 3)
                                {
                                    this.Lenses.Remove(Current);
                                    if (Current is Prism) ControlPanel.PrismCount += 1;
                                    else if (Current is Mirror) ControlPanel.MirrorCount += 1;

                                    UpdateLaser(Current.In);
                                    UpdateLaser(Current.Extra);
                                    
                                    Current = null;
                                }
                            }
                            if (where == 4)
                            {
                                Guide.BeginShowMessageBox("提示", "确定要重置吗？",
                                    new string[] { "确定", "取消" }, 1, MessageBoxIcon.None, new AsyncCallback((s) =>
                                    {
                                        var r = Guide.EndShowMessageBox(s);
                                        //确认
                                        if (r == 0)
                                        {
                                            foreach (var v in this.Lenses)
                                            {
                                                if (v is Prism) ControlPanel.PrismCount += 1;
                                                else ControlPanel.MirrorCount += 1;
                                            }
                                            this.Lenses.Clear();
                                            UpdateAllEmitter();
                                            Current = null;
                                        }
                                    }), null);
                                
                            }
                            CheckState();
                        }
                    }
                    //拖拽结束，把控件放好位置
                    else if (IsDraging && Current != null)
                    {
                        Current.In = null;
                        Current.Extra = null;
                        Current.SetPosition(Current.X / 60 * 60 + this.X + 30, (Current.Y - this.Y)/ 60 * 60 + 30 + this.Y);
                        UpdateAllEmitter();
                        CheckState();
                    }

                    MouseDown = false;
                    IsDraging = false;
                }
            }
            #endregion

            #region 在按下状态下，判断是否拖拽
            if (MouseDown)
            {
                if (IsDraging == false && (state.X != FirstPoint.X || state.Y != FirstPoint.Y))
                {
                    IsDraging = true;
                    if (Current != null)
                    {
                        Current.ClearLaser();
                    }
                }

                if (IsDraging && Current != null)
                {
                    if (Offset.Y > 100) return;
                    Current.SetPosition(state.X - Offset.X, state.Y - Offset.Y);
                    Current.CanInput = this.CanPutIn(new Point(Current.X,Current.Y));
                }
            }
            #endregion
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var v in Emitters)
                v.Draw(spriteBatch);
            foreach (var v in Receivers)
                v.Draw(spriteBatch);
            foreach (var v in Walls)
                v.Draw(spriteBatch);
            foreach (var v in Switches)
                v.Draw(spriteBatch);
            foreach (var v in Lenses)
                v.Draw(spriteBatch);

            if (Hole1 != null)
            {
                Hole1.Draw(spriteBatch);
            }

            ControlPanel.Draw(spriteBatch);
        }

        public static WorkingArea Create(string level, ContentManager content)
        {
            string data = content.Load<string>("Level/" + level);
            try
            {
                WorkingArea area = new WorkingArea(content);
                Point pt1 = new Point(0,0), pt2 = new Point(0,0);

                string[] items = data.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                string[] sp = items[0].Split(',');

                area.ControlPanel = new ControlPanel(content, 0, 700, int.Parse(sp[0]), int.Parse(sp[1]));
                
                for(int i = 1;i<items.Length;i++)
                {
                    sp = items[i].Split(',');

                    int x = (int.Parse(sp[0]) - 1) * 60 + area.X;
                    int y = (int.Parse(sp[1]) - 1) * 60 + area.Y;
                    if (sp.Length < 5)
                    {
                        if(sp[2] == "8")
                            area.Walls.Add(new Wall(content, Colors.black, x, y));
                        else if(sp[2] == "6")
                        {
                            if (pt1.Y == 0)
                            {
                                pt1.X = x;
                                pt1.Y = y;
                            }
                            else if(pt2.Y == 0)
                            {
                                pt2.X = x;
                                pt2.Y = y;
                                area.Hole1 = new Hole(area, pt1.X, pt1.Y, pt2.X, pt2.Y);
                            }
                            else
                            {
                                throw new Exception("洞的数量大于2,错误发生在关卡文件" + level + "中");
                            }
                        }
                    }
                    else
                    {
                        Colors color = Colors.blue;
                        switch (sp[4])
                        {
                            case "1": color = Colors.blue; break;
                            case "2": color = Colors.yellow; break;
                            case "3": color = Colors.red; break;
                            case "4": color = Colors.green; break;
                            case "5": color = Colors.orange; break;
                            case "6": color = Colors.purple; break;
                        }

                        switch (sp[2])
                        {
                            case "7":
                                int angle = int.Parse(sp[3]) - 90;
                                area.Emitters.Add(new Emitter(content, color, x, y, -angle));
                                break;
                            case "3":
                                area.Receivers.Add(new Receiver(content, color, x, y));
                                break;
                            case "4":
                                area.Switches.Add(new Switch(content, color, x, y));
                                break;
                            case "5":
                                area.Walls.Add(new Wall(content, color, x, y));
                                break;
                        }
                        
                    }
                }
                area.UpdateAllEmitter();
                area.CheckState();
                return area;
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}
