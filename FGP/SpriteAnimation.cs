#region 'Using' information
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
#endregion

namespace FGP
{
    public class SpriteManager
    {
        /// <summary>
        ///  Try to make all of these private / protected
        /// </summary>
        protected Texture2D Texture;
        public Vector2 Position = Vector2.Zero;
        public Color Color = Color.White;
        public Vector2 Origin;
        public float Rotation = 0f;
        public float Scale = 1f;
        public SpriteEffects SpriteEffect;
        protected Rectangle[] Rectangles;
        protected int FrameIndex = 0;

        public SpriteManager(Texture2D Texture, int frames, Color color)
        {
            this.Texture = Texture;
            int width = Texture.Width / frames;
            Rectangles = new Rectangle[frames];
            Color = color;

            for (int i = 0; i < frames; i++)
                Rectangles[i] = new Rectangle(i * width, 0, width, Texture.Height);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, Rectangles[FrameIndex], Color, Rotation, Origin, Scale, SpriteEffect, 0f);
        }
    }

    public class SpriteAnimation : SpriteManager
    {
        private float timeElapsed;
        private bool IsLooping = true;
        private float timeToUpdate;
        public int FramesPerSecond { set { timeToUpdate = (1f / value); } }

        public SpriteAnimation(Texture2D Texture, int frames, Color color, int fps) : base(Texture, frames, color) 
        { FramesPerSecond = fps; }

        public void Update(GameTime gameTime)
        {
            timeElapsed += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (timeElapsed > timeToUpdate)
            {
                timeElapsed -= timeToUpdate;

                if (FrameIndex < Rectangles.Length - 1)
                { FrameIndex++; }

                else if (IsLooping)
                { FrameIndex = 0; }
            }
        }

        public void setFrame(int frame)
        { FrameIndex = frame; }
    }
}