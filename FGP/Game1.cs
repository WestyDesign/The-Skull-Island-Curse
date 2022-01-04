#region 'Using' information
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using FGP.States;
#endregion

namespace FGP
{ 
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        private State currentState;
        private State nextState;

        Player player = new Player();

        public void ChangeState(State state)
        { nextState = state; }

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            IsMouseVisible = true;
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 800; // A bit of an awkward size, but fits the main menu image well.
            graphics.ApplyChanges();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            currentState = new MenuState(this, graphics.GraphicsDevice, Content);
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            { Exit(); } // Close the game when the player presses the escape key.

            if (nextState != null)
            {
                currentState = nextState;
                nextState = null;
            }                     

            currentState.Update(gameTime); // Runs the update method inside the current state's class.
        }

        protected override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();               
            spriteBatch.End();

            GraphicsDevice.Clear(Color.LightSkyBlue);
            currentState.Draw(gameTime, spriteBatch);                   
            base.Draw(gameTime);
        }
    }
}