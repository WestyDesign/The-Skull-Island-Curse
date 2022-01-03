#region 'Using' information
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace FGP.Controls
{
    public class Button : Component
    {
        #region Privates
        private MouseState _currentMouse;
        private MouseState _previousMouse; // Checks what the mouse was previously doing so that a click & release starts the game, not just a click and hold.
        private bool _isHovering; // Checks if the mouse is hovering over a button
        private SpriteFont _font; // The font to be used on the buttons
        private Texture2D _texture;
        #endregion

        #region Publics
        public event EventHandler Click;
        public bool Clicked { get; private set; }
        public Color PenColour { get; set; }
        public Vector2 Position { get; set; }

        public Rectangle Rectangle
        { get { return new Rectangle((int)Position.X, (int)Position.Y, _texture.Width, _texture.Height); } }

        public string Text { get; set; } // The text inside of the button - editable further down.
        #endregion

        #region Methods
        public Button(Texture2D texture, SpriteFont font)
        {
            _texture = texture;
            _font = font;
            PenColour = Color.LightBlue; // The colour of the text inside the button.
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            var colour = Color.CornflowerBlue; // The colour of the button when nothing in particular is happening to it.

            if (_isHovering) // The colour the button becomes when moused over.
                colour = Color.DarkBlue;

            spriteBatch.Draw(_texture, Rectangle, colour);

            if (!string.IsNullOrEmpty(Text))
            {
                var x = (Rectangle.X + (Rectangle.Width / 2)) - (_font.MeasureString(Text).X / 2);
                var y = (Rectangle.Y + (Rectangle.Height / 2)) - (_font.MeasureString(Text).Y / 2);
                spriteBatch.DrawString(_font, Text, new Vector2(x, y), PenColour);
            }
        }

        public override void Update(GameTime gameTime)
        {
            _previousMouse = _currentMouse;
            _currentMouse = Mouse.GetState();

            var mouseRectangle = new Rectangle(_currentMouse.X, _currentMouse.Y, 1, 1);

            _isHovering = false;

            if (mouseRectangle.Intersects(Rectangle))
            {
                _isHovering = true;

                if (_currentMouse.LeftButton == ButtonState.Released && _previousMouse.LeftButton == ButtonState.Pressed)
                { Click?.Invoke(this, new EventArgs()); }
            }
        }
        #endregion
    }
}