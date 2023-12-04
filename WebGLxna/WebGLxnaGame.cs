using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using System.Security.Cryptography.X509Certificates;

namespace WebGLxna
{
    public class WebGLxnaGame : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch _sb;
        public WebGLxnaGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            //tests
            var wnd = Window;
            var wndbounds = wnd.ClientBounds;

            location = wndbounds.Size.ToVector2() * 0.5f;
        }

        private Texture2D square;
        private Paddle leftPaddle;
        private Paddle rightPaddle;
        protected override void Initialize()
        {
            _sb = new SpriteBatch(graphics.GraphicsDevice);
            square = generate();
            base.Initialize();

            leftPaddle = new Paddle(new Rectangle(72, 64, 32, 196), Color.Purple);
            rightPaddle = new Paddle(new Rectangle(1294, 64, 32, 196), Color.Yellow);
        }


        private Texture2D generate(int size = 32)
        {
            Texture2D t =  new Texture2D(graphics.GraphicsDevice, size, size);
            Color[] data = new Color[size * size];
            for(int i = 0; i < data.Length; i++)
            {
                data[i] = Color.White;
            }
            t.SetData(data);
            return t;
        }
        
        protected override void Update(GameTime gameTime)
        {
            MouseState ms = Mouse.GetState();
            KeyboardState ks = Keyboard.GetState();
            GamePadState ps = default;
            try { ps = GamePad.GetState(PlayerIndex.One); }
            catch (NotImplementedException) { /* ignore gamePadState */ }

            if (ks.IsKeyDown(Keys.Escape) ||
                ks.IsKeyDown(Keys.Back) ||
                ps.Buttons.Back == ButtonState.Pressed)
            {
                try { Exit(); }
                catch (PlatformNotSupportedException) { /* ignore */ }
            }

            leftPaddle.Update(Keys.W, Keys.S);
            rightPaddle.Update(Keys.I, Keys.K);
            Rectangle ballBound = square.Bounds;
            ballBound.Location += location.ToPoint();

            deltaV *= leftPaddle.Collison(ballBound);
            deltaV *= rightPaddle.Collison(ballBound);
            location += deltaV;
            if(location.X < 0 || (location.X + square.Width) >= graphics.PreferredBackBufferWidth)
            {
                deltaV *= Vector2.Zero;
            }
            if(location.Y < 0 || (location.Y + square.Height) >= graphics.PreferredBackBufferHeight)
            {
                deltaV *= new Vector2(1,-1);
            }
            deltaV.Normalize();
            deltaV *= (MathF.Sin((float)gameTime.TotalGameTime.TotalSeconds * 0.3f) + 2)*4;
            
            base.Update(gameTime);
        }

        Vector2 deltaV = Vector2.One * 4;
        Vector2 location = Vector2.Zero;

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(Color.CornflowerBlue);

            if(Keyboard.GetState().IsKeyDown(Keys.A))
            {
                GraphicsDevice.Clear(Color.Red);
            }

            _sb.Begin();
            _sb.Draw(square, location, Color.White);
            leftPaddle.Draw(_sb, square);
            rightPaddle.Draw(_sb, square);
            _sb.End();

            base.Draw(gameTime);
        }
    }

        internal class Paddle
    {
        private Rectangle Bounds;
        private Color color;
        public Paddle(Rectangle bounds, Color Color)
        {
            Bounds = bounds;
            color = Color;
        }

        public void Update(Keys up, Keys down)
        {
            KeyboardState keyState = Keyboard.GetState();
            if(keyState.IsKeyDown(up))
            {
                Bounds.Location += new Point(0, -6);
            }
            if(keyState.IsKeyDown(down))
            {
                Bounds.Location += new Point(0, 6);
            }
        }

        public void Draw(SpriteBatch sb, Texture2D texture2D)
        {
            sb.Draw(texture2D, Bounds, color);
        }

        public Vector2 Collison(Rectangle rectangle)
        {
            if(!rectangle.Intersects(Bounds))
            {
                return Vector2.One;
            }

            return new Vector2(-1, 1);
        }
    }
}
