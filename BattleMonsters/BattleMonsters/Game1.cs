using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary.Util;

namespace BattleMonsters
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        GameManager gm;

        //Services
        InputHandler input;
        GameConsole console;

        Texture2D Background;
        Rectangle ScreenSize;

        public Game1() : base()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            //Services
            input = new InputHandler(this);
            console = new GameConsole(this);

            this.Components.Add(console);
            this.Components.Add(input);

            #region Code gotten from
            //https://community.monogame.net/t/get-the-actual-screen-width-and-height-on-windows-10-c-monogame/10006/3
            #endregion
            //Gets the current displays hight and width
            int w = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            int h = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

            ScreenSize = new Rectangle(0, 0, w, h);

            this._graphics.PreferredBackBufferWidth = w;
            this._graphics.PreferredBackBufferHeight = h;

            gm = new GameManager(this);
            this.Components.Add(gm);
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            //WARNING: Don't debug with this on, will soft lock
            #region Fullscreen
            //this._graphics.IsFullScreen = true;
            //this._graphics.ApplyChanges();
            #endregion

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            Background = Content.Load<Texture2D>("MarsBackground");

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }


        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DarkGray);

            _spriteBatch.Begin();

            _spriteBatch.Draw(Background, new Vector2(0, 0), ScreenSize, Color.White, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0f);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
