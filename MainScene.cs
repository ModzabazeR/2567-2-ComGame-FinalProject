using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using FinalProject.GameObject;
using FinalProject.GameObject.Entity;
using FinalProject.GameObject.Entity.Enemy;

using FinalProject.Utils.MapManager;
using FinalProject.Utils.SplashScreen;

using System.Collections.Generic;
using System;

namespace FinalProject;

public class MainScene : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Player player;
    private Camera camera;
    private MapManager mapManager; // Add this field

    private SplashScreenSequence _currentSequence;

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

        StartIntroSequence();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // Load the font first since splash screens need it
        Singleton.Instance.Font = Content.Load<SpriteFont>("GameFont");

        // Start the intro sequence
        StartIntroSequence();

        Texture2D idleTexture = Content.Load<Texture2D>("_Idle");
        Texture2D runTexture = Content.Load<Texture2D>("_Run");
        Texture2D jumpTexture = Content.Load<Texture2D>("_Jump");

        Singleton.Instance.EntityAnimations["Player"] = new Dictionary<string, Animation> {
            { "Idle", new Animation(idleTexture, 120, 80, 10, 0.1f) },
            { "Run", new Animation(runTexture, 120, 80, 10, 0.08f) },
            { "Jump", new Animation(jumpTexture, 120, 80, 3, 0.15f) }
        };

        // Initialize map manager
        mapManager = new MapManager();

        // Load map textures
        Texture2D map1Texture = Content.Load<Texture2D>("Textures/level1");
        Texture2D map2Texture = Content.Load<Texture2D>("Textures/level2");
        Texture2D map3Texture = Content.Load<Texture2D>("Textures/level3");

        // Add maps with their collision data
        mapManager.AddMap("Map 1", map1Texture, new Vector2(0, 500), "Content/Maps/level1_collision.lcm");
        mapManager.AddMap("Map 2", map2Texture, new Vector2(0, 1200), "Content/Maps/level2_collision.lcm");
        mapManager.AddMap("Map 3", map3Texture, new Vector2(0, 2000), "Content/Maps/level3_collision.lcm");

        // Initialize systems
        player = new Player(Singleton.Instance.EntityAnimations["Player"], new Vector2(50, 700));
        camera = new Camera(Singleton.Instance.ScreenWidth, Singleton.Instance.ScreenHeight);
    }

    protected override void Update(GameTime gameTime)
    {
        Singleton.Instance.UpdateKeyboardState();

        // Handle splash screen and cutscene states
        if (Singleton.Instance.CurrentGameState == GameState.Splash ||
            Singleton.Instance.CurrentGameState == GameState.Cutscene)
        {
            // Add this check for Escape key to skip intro
            if (Singleton.Instance.CurrentKey.IsKeyDown(Keys.Escape) &&
                Singleton.Instance.PreviousKey.IsKeyUp(Keys.Escape))
            {
                _currentSequence?.Skip();
                return;
            }

            if (_currentSequence != null)
            {
                _currentSequence.Update(gameTime);
                if (_currentSequence.IsComplete)
                {
                    _currentSequence = null;
                    Singleton.Instance.CurrentGameState = GameState.Playing;
                }
                return;
            }
        }

        // Only update game logic if in Playing state
        if (Singleton.Instance.CurrentGameState == GameState.Playing)
        {
            // Get camera bounds for visibility checking
            Rectangle cameraBounds = new Rectangle(
                (int)camera.Position.X,
                (int)camera.Position.Y,
                Singleton.Instance.ScreenWidth,
                Singleton.Instance.ScreenHeight
            );

            // Update camera to follow player
            camera.Follow(player.Position, mapManager.GetWorldBounds());

            // Update player with collision tiles
            player.Update(gameTime, mapManager.GetAllSolidTiles());

            foreach (var map in mapManager.GetMaps())
            {
                map.Update(gameTime, player.Position);
                if (map.IsVisible(cameraBounds))
                {
                    foreach (var enemy in map.GetEnemies())
                    {
                        enemy.Update(gameTime, mapManager.GetAllSolidTiles());
                    }
                }
            }

        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        if (Singleton.Instance.CurrentGameState == GameState.Splash ||
            Singleton.Instance.CurrentGameState == GameState.Cutscene)
        {
            GraphicsDevice.Clear(Color.Black);
            _spriteBatch.Begin();
            _currentSequence?.Draw(_spriteBatch, Singleton.Instance.Font);
            _spriteBatch.End();
        }
        else if (Singleton.Instance.CurrentGameState == GameState.Playing)
        {
            GraphicsDevice.Clear(Color.White);
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

            // Draw enemies from each map
            foreach (var map in mapManager.GetMaps())
            {
                if (map.IsVisible(cameraBounds))
                {
                    foreach (var enemy in map.GetEnemies())
                    {
                        enemy.Draw(_spriteBatch);
                    }
                }
            }

            // Draw player
            player.Draw(_spriteBatch);

            _spriteBatch.End();
        }

        base.Draw(gameTime);
    }

    private void StartIntroSequence()
    {
        var introScreens = new List<SplashScreenData>
        {
            // The legend
            new SplashScreenData(
                [
                    "The Queue",
                    "",
                    "Some gods want fancy temples.",
                    "Others rule through fear.",
                    "",
                    "But The Queue is different.",
                    "It controls people by making them wait... forever."
                ], fadeSpeed: 0.3f, displayTime: 6f),

            // The protagonist
            new SplashScreenData(
                [
                    "I never liked standing still.",
                    "",
                    "While others waited in endless lines,",
                    "I chose to keep moving, to stay active,",
                    "To live life the way I wanted.",
                    "",
                    "That's why they came after me."
                ], fadeSpeed: 0.4f, displayTime: 6f),

            // The capture
            new SplashScreenData(
                [
                    "They dragged me to their temple,",
                    "Said I was the perfect sacrifice",
                    "Because I refused to stand in line.",
                    "",
                    "But during their ceremony,",
                    "Something went terribly wrong.",
                    "The whole temple fell into darkness."
                ], fadeSpeed: 0.4f, displayTime: 7f),

            // The stakes
            new SplashScreenData(
                [
                    "Now we're trapped in The Queue's world,",
                    "Where everyone stands frozen in lines.",
                    "",
                    "Its power is growing stronger,",
                    "Spreading across our world.",
                    "",
                    "I have to escape. I have to run."
                ], fadeSpeed: 0.5f, displayTime: 6f)
        };

        _currentSequence = new SplashScreenSequence(introScreens);
        Singleton.Instance.CurrentGameState = GameState.Splash;
    }

    public void ShowCutscene(params SplashScreenData[] screens)
    {
        _currentSequence = new SplashScreenSequence(screens);
        Singleton.Instance.CurrentGameState = GameState.Cutscene;
    }

    public void ShowSimpleCutscene(string[] text, float fadeSpeed = 1f, float displayTime = 2f)
    {
        ShowCutscene(new SplashScreenData(text, fadeSpeed, displayTime));
    }
}
