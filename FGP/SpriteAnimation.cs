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
        protected Texture2D Texture;
        private Vector2 position = Vector2.Zero;
        private Color colour = Color.White;
        private Vector2 origin;
        private float rotation = 0f;
        private float scale = 1f;
        private SpriteEffects spriteEffect;
        protected Rectangle[] Rectangles;
        protected int FrameIndex = 0;

        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        public Color Colour
        {
            get { return colour; }
            set { colour = value; }
        }

        public Vector2 Origin
        {
            get { return origin; }
            set { origin = value; }
        }

        public float Rotation
        {
            get { return rotation; }
            set { rotation = value; }
        }

        public float Scale
        {
            get { return scale; }
            set { scale = value; }
        }

        public SpriteEffects SpriteEffect
        {
            get { return spriteEffect; }
            set { spriteEffect = value; }
        }

        public SpriteManager(Texture2D Texture, int frames, Color color)
        {
            this.Texture = Texture;
            int width = Texture.Width / frames;
            Rectangles = new Rectangle[frames];
            Colour = color;

            for (int i = 0; i < frames; i++)
                Rectangles[i] = new Rectangle(i * width, 0, width, Texture.Height);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, Rectangles[FrameIndex], Colour, Rotation, Origin, Scale, SpriteEffect, 0f);
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