using DefaultEcs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using tainicom.Aether.Physics2D.Dynamics;
using GameWorld = DefaultEcs.World;
using PhysicalWorld = tainicom.Aether.Physics2D.Dynamics.World;

namespace MonoGameGame
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private GameWorld _gameWorld;
        private PhysicalWorld _physicalWorld;
        private Entity _player;
        private Entity _object;

#pragma warning disable CS8618 // Disabled since initialization is moved to Initialize
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }
#pragma warning restore CS8618

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            _gameWorld = new GameWorld();
            _player = _gameWorld.CreateEntity();
            _object = _gameWorld.CreateEntity();

            _physicalWorld = new PhysicalWorld();
            _physicalWorld.Gravity = Vector2.Zero;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            _player.Set(new SpriteRenderer { Texture = Content.Load<Texture2D>("face") });

            _object.Set(new SpriteRenderer { Texture = Content.Load<Texture2D>("face") });
            {
                var body = _physicalWorld.CreateBody(new Vector2(100, 100), 0, BodyType.Dynamic);
                var size = _player.Get<SpriteRenderer>().Texture.Bounds;
                body.CreateRectangle(size.Width, size.Height, 0, Vector2.Zero);
                body.LinearDamping = 20f;
                _player.Set(new Transform { Body = body });
            }
            {
                var size = _object.Get<SpriteRenderer>().Texture.Bounds;
                var body = _physicalWorld.CreateBody(new Vector2(400, 100), 0, BodyType.Static);
                body.CreateRectangle(size.Width, size.Height, 0, Vector2.Zero);
                _object.Set(new Transform { Body = body });
            }
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            var force = Vector2.Zero;
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                force.X += 100000;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                force.X -= 100000;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                force.Y += 100000;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                force.Y -= 100000;
            }
            _player.Get<Transform>().Body.ApplyForce(force);

            _physicalWorld.Step((float)gameTime.ElapsedGameTime.TotalSeconds);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            _spriteBatch.Begin();
            _spriteBatch.Draw(_player.Get<SpriteRenderer>().Texture, _player.Get<Transform>().Position, Color.White);
            _spriteBatch.Draw(_object.Get<SpriteRenderer>().Texture, _object.Get<Transform>().Position, Color.Red);
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        protected override void Dispose(bool disposing)
        {
            _player.Dispose();

            base.Dispose(disposing);
        }
    }
}
