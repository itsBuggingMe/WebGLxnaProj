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
        private Rectangle wndbounds;

        public WebGLxnaGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            //tests
            var wnd = Window;
            wndbounds = wnd.ClientBounds;
        }

        private Texture2D square;
        private float timeSinceLastPiped;

        protected override void Initialize()
        {
            _sb = new SpriteBatch(graphics.GraphicsDevice);
            square = generate();
            PlayerBounds = new Rectangle(0, 0, 64, 64);
            base.Initialize();
        }

        private Rectangle PlayerBounds;
        private Vector2 loc = new Vector2(400, 500);
        public List<Rectangle> Pipes = new List<Rectangle>();
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
        private Vector2 Velocity = new Vector2(0, -28);
        private Rectangle Score = new Rectangle(0,0, 0, 8);
        private float dtSinceLastPress;

        private KeyboardState pk;
        private int speed = 1;
        private int mode = 0;
        protected override void Update(GameTime gameTime)
        {
            

            wndbounds = Window.ClientBounds;
            timeSinceLastPiped++;
            MouseState ms = Mouse.GetState();
            KeyboardState ks = Keyboard.GetState();
            GamePadState ps = default;
            try { ps = GamePad.GetState(PlayerIndex.One); }
            catch (NotImplementedException) {  }

            if (ks.IsKeyDown(Keys.Escape) ||
                ks.IsKeyDown(Keys.Back) ||
                ps.Buttons.Back == ButtonState.Pressed)
            {
                try { Exit(); }
                catch (PlatformNotSupportedException) {  }
            }
            if(ks.IsKeyDown(Keys.Space) && !pk.IsKeyDown(Keys.Space))
            {
                mode = 0;
                dtSinceLastPress = 0;
                Velocity = new Vector2(0, -16);
            }
            Score.Width = Pipes.Count;
            if(timeSinceLastPiped > 60)
            {

                timeSinceLastPiped = 0;
                float height = (float)Random.Shared.NextDouble() * 400f;
                Pipes.Add(new Rectangle(1920, 0, 96, (int)height));
                float bottomY = height + 400;

                Pipes.Add(new Rectangle(1920, (int)bottomY, 96, 1920));
            }
            
            for(int i = 0; i < Pipes.Count; i++)
            {
                Pipes[i] = new Rectangle(Pipes[i].Location + new Point(-7 + speed, 0), Pipes[i].Size);
                if(Pipes[i].Intersects(PlayerBounds))
                {
                    Exit();
                }
            }
            if(ks.IsKeyDown(Keys.Left) && !pk.IsKeyDown(Keys.Left))
            {
                speed--;
            }
            if(ks.IsKeyDown(Keys.Right) && !pk.IsKeyDown(Keys.Right))
            {
                speed++;
            }
            if(ks.IsKeyDown(Keys.Up) && !pk.IsKeyDown(Keys.Up))
            {
                mode = 1;
            }
            if(ks.IsKeyDown(Keys.Up) && !pk.IsKeyDown(Keys.Up))
            {
                mode = 2;
            }
            if(!PlayerBounds.Intersects(wndbounds))
            {
                Exit();
            }

            if(mode == 0)
            {
                Velocity += new Vector2(0, 0.75f);
                Velocity *= 0.98f;
                loc += Velocity;
            }
            else if(mode == 1)
            {
                Velocity = Vector2.Zero;
                
            }
            else if(mode == 2)
            {
                Velocity = Vector2.Zero;
                loc.Y = wndbounds.Y - PlayerBounds.Y;
            }
            


            PlayerBounds.Location = loc.ToPoint();
            dtSinceLastPress++;

            pk = ks;

            base.Update(gameTime);
        }


        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(Color.SkyBlue);

            _sb.Begin();
            Pipes.ForEach(p => {_sb.Draw(square, p, Color.Blue); });

            _sb.Draw(square, PlayerBounds, Color.Red);
            _sb.Draw(square, Score, Color.Green);
            _sb.End();

            base.Draw(gameTime);
        }

        /*
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
            catch (NotImplementedException) {   }

            if (ks.IsKeyDown(Keys.Escape) ||
                ks.IsKeyDown(Keys.Back) ||
                ps.Buttons.Back == ButtonState.Pressed)
            {
                try { Exit(); }
                catch (PlatformNotSupportedException) { }
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
        }*/
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
