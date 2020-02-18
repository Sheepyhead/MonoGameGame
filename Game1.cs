using DefaultEcs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Rendering;
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
        private ResolutionIndependentRenderer _resolutionIndependence;
        private Camera2D _camera;

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

            _resolutionIndependence = new ResolutionIndependentRenderer(this);
            _camera = new Camera2D(_resolutionIndependence);
            InitializeResolutionIndependence(_graphics.GraphicsDevice.Viewport.Width, _graphics.GraphicsDevice.Viewport.Height);

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
                body.Mass = 1;
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
            var velocity = Vector2.Zero;
            var velocityScalar = 1000f;
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                velocity.X += velocityScalar;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                velocity.X -= velocityScalar;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                velocity.Y += velocityScalar;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                velocity.Y -= velocityScalar;
            }
            _player.Get<Transform>().Body.LinearVelocity = velocity;

            _physicalWorld.Step(gameTime.ElapsedGameTime);

            _camera.Position = _player.Get<Transform>().Position;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Magenta);

            // TODO: Add your drawing code here
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullNone, null, _camera.GetViewTransformationMatrix());
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

        private void InitializeResolutionIndependence(int realScreenWidth, int realScreenHeight)
        {
            _resolutionIndependence.VirtualWidth = _resolutionIndependence.ScreenWidth = realScreenWidth;
            _resolutionIndependence.VirtualHeight = _resolutionIndependence.ScreenHeight = realScreenHeight;
            _resolutionIndependence.Initialize();

            _camera.RecalculateTransformationMatrices();
        }
    }
}
