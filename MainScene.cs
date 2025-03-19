using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using FinalProject.GameObject;
using System.Collections.Generic;

namespace FinalProject;

public class MainScene : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Player player;
    private Camera camera;
    private MapManager mapManager; // Add this field

    public MainScene()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        _graphics.PreferredBackBufferWidth = Singleton.Instance.ScreenWidth;
        _graphics.PreferredBackBufferHeight = Singleton.Instance.ScreenHeight;
        _graphics.ApplyChanges();
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // Initialize map manager
        mapManager = new MapManager();

        // Load map textures
        Texture2D map1Texture = Content.Load<Texture2D>("Textures/level1");
        Texture2D map2Texture = Content.Load<Texture2D>("Textures/level2");

        // Add maps with their collision data
        mapManager.AddMap("Map 1", map1Texture, new Vector2(0, 500), "Content/Maps/level1_collision.txt");
        mapManager.AddMap("Map 2", map2Texture, new Vector2(0, 1200), "Content/Maps/level2_collision.txt");


        // Create player texture
        //Texture2D playerTexture = new Texture2D(GraphicsDevice, 32, 32);
        //Color[] playerData = new Color[32 * 32];
        //for (int i = 0; i < playerData.Length; i++)
        //    playerData[i] = Color.Red;
        //playerTexture.SetData(playerData);
        Texture2D idleTexture = Content.Load<Texture2D>("_Idle");
        Texture2D runTexture = Content.Load<Texture2D>("_Run");
        Texture2D jumpTexture = Content.Load<Texture2D>("_Jump");

        Dictionary<string, Animation> animations = new Dictionary<string, Animation> {
            { "Idle", new Animation(idleTexture, 120, 80, 10, 0.1f) },
            { "Run", new Animation(runTexture, 120, 80, 10, 0.08f) },
            { "Jump", new Animation(jumpTexture, 120, 80, 3, 0.15f) }
        };

        // Initialize systems
        player = new Player(animations, new Vector2(50, 700));
        camera = new Camera(Singleton.Instance.ScreenWidth, Singleton.Instance.ScreenHeight);

        Singleton.Instance.Font = Content.Load<SpriteFont>("GameFont");
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        Singleton.Instance.UpdateKeyboardState();

        // Update camera to follow player
        camera.Follow(player.Position, mapManager.GetWorldBounds());

        // Update player with collision tiles
        player.Update(gameTime, mapManager.GetAllSolidTiles());

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin(transformMatrix: camera?.Transform);

        // Get camera bounds for visibility checking
        Rectangle cameraBounds = new Rectangle(
            (int)camera.Position.X,
            (int)camera.Position.Y,
            Singleton.Instance.ScreenWidth,
            Singleton.Instance.ScreenHeight
        );

        // Draw map first (before player)
        mapManager.Draw(_spriteBatch, cameraBounds);

        // Draw player
        player.Draw(_spriteBatch);

        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
