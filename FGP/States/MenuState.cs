#region 'Using' information
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using FGP.Controls;
#endregion

namespace FGP.States
{
    public class MenuState : State
    {
        private List<Component> _components;
        private Texture2D menuBackground;
        private GraphicsDeviceManager graphics;

        public MenuState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content)
          : base(game, graphicsDevice, content)
        {
            var buttonTexture = _content.Load<Texture2D>("Menus/Button");
            var buttonFont = _content.Load<SpriteFont>("Fonts/timerFont");
            menuBackground = _content.Load<Texture2D>("Menus/MainMenu");

            var playGameButton = new Button(buttonTexture, buttonFont)
            {
                Position = new Vector2(400, 580),
                Text = "Play Game",
            };

            playGameButton.Click += PlayButton_Click;

            var quitGameButton = new Button(buttonTexture, buttonFont)
            {
                Position = new Vector2(700, 580),
                Text = "Quit Game",
            };

            quitGameButton.Click += QuitGameButton_Click;

            _components = new List<Component>()
            {
                playGameButton,
                quitGameButton,
            };
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            spriteBatch.Draw(menuBackground, new Vector2(0,-60), Color.White); // Draws the main menu background onto the screen

            foreach (var component in _components) // Draws the two buttons onto the screen.
                component.Draw(gameTime, spriteBatch);

            spriteBatch.End();
        }

        private void PlayButton_Click(object sender, EventArgs e) // Starts the game state when the 'Play' button is pressed.
        { _game.ChangeState(new GameState(_game, _graphicsDevice, _content)); }

        public override void Update(GameTime gameTime) // Adds the main menu buttons to the screen.
        {
            foreach (var component in _components)
                component.Update(gameTime);
        }

        private void QuitGameButton_Click(object sender, EventArgs e) // Closes the game when the quit button's pressed.
        { _game.Exit(); }
    }
}