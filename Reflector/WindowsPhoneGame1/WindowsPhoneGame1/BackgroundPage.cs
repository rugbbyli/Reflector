using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace Reflector
{
    /// <summary>
    /// 这是一个实现 IUpdateable 的游戏组件。
    /// </summary>
    public class BackgroundPage : DrawableGameComponent
    {
        SpriteBatch spriteBatch;
        Texture2D[] StarImage;
        List<Star> Stars;
        Random random;
        int StarCount = 150;

        public BackgroundPage(Game game)
            : base(game)
        {
            Stars = new List<Star>(StarCount);
            StarImage = new Texture2D[3];         
            random = new Random();
        }

        public override void Initialize()
        {
            
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(Game.GraphicsDevice);
            StarImage[0] = Game.Content.Load<Texture2D>(@"Images/star1px");
            StarImage[1] = Game.Content.Load<Texture2D>(@"Images/star3px");
            StarImage[2] = Game.Content.Load<Texture2D>(@"Images/star5px");
            InitStars();
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            for (int i = 0; i < Stars.Count; i++)
            {
                Stars[i].Update(gameTime);
                if (Stars[i].IsOutOfBounds(Game.Window.ClientBounds))
                    Stars.RemoveAt(i--);
            }

            if (Stars.Count < StarCount)
                Stars.Add(CreateStar());

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);

            foreach (Star s in Stars)
                s.Draw(spriteBatch);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        void InitStars()
        {
            for (int i = 0; i < StarCount; i++)
            {
                Vector2 speed = new Vector2(random.Next(1, 4), 0);
                Vector2 position = new Vector2(random.Next(0, 481), random.Next(0, 801));
                Star star = new Star(StarImage[random.Next(0, StarImage.Length)], position, speed);
                Stars.Add(star);
            }
        }

        Star CreateStar()
        {
            Vector2 speed = new Vector2(random.Next(1, 4), 0);
            Vector2 position = new Vector2(-1, random.Next(0, 801));
            Star star = new Star(StarImage[random.Next(0, StarImage.Length)], position, speed);
            return star;
        }
    }
}
