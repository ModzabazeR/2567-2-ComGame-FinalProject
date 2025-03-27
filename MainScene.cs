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

    private List<Enemy> enemies; // รายการศัตรู

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

        // Initialize map manager
        mapManager = new MapManager();

        // Load map textures
        Texture2D map1Texture = Content.Load<Texture2D>("Textures/level1");
        Texture2D map2Texture = Content.Load<Texture2D>("Textures/level2");

        // Add maps with their collision data
        mapManager.AddMap("Map 1", map1Texture, new Vector2(0, 500), "Content/Maps/level1_collision.lcm");
        mapManager.AddMap("Map 2", map2Texture, new Vector2(0, 1200), "Content/Maps/level2_collision.lcm");


        // Create player texture
        //Texture2D playerTexture = new Texture2D(GraphicsDevice, 32, 32);
        //Color[] playerData = new Color[32 * 32];
        //for (int i = 0; i < playerData.Length; i++)
        //    playerData[i] = Color.Red;
        //playerTexture.SetData(playerData);
        Texture2D idleTexture = Content.Load<Texture2D>("Player_Idle");
        Texture2D runTexture = Content.Load<Texture2D>("Player_Walk");


        Dictionary<string, Animation> animations = new Dictionary<string, Animation> {
            { "Idle", new Animation(idleTexture, 32, 75, 3, 0.33f) },
            { "Walk", new Animation(runTexture, 48, 75, 8, 0.125f) }
        };

        // Initialize systems
        player = new Player(animations, new Vector2(50, 700));
        camera = new Camera(Singleton.Instance.ScreenWidth, Singleton.Instance.ScreenHeight);

        // Spawn an enemy
        enemies = new List<Enemy>();
        enemies.Add(new SimpleEnemy(animations, new Vector2(200, 700)));
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

            // อัปเดตศัตรู
            foreach (var enemy in enemies)
            {
                enemy.Update(gameTime, mapManager.GetAllSolidTiles());
                // เช็คการชนระหว่าง Player กับ Enemy
                if (player.Bounds.Intersects(enemy.Bounds))
                {
                    Console.WriteLine("hit"); // แสดงข้อความเมื่อชนกัน
                }
            }

            // Update camera to follow player
            camera.Follow(player.Position, mapManager.GetWorldBounds());

            // Update player with collision tiles
            player.Update(gameTime, mapManager.GetAllSolidTiles());
        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

        if (Singleton.Instance.CurrentGameState == GameState.Splash ||
            Singleton.Instance.CurrentGameState == GameState.Cutscene)
        {
            _spriteBatch.Begin();
            _currentSequence?.Draw(_spriteBatch, Singleton.Instance.Font);
            _spriteBatch.End();
        }
        else if (Singleton.Instance.CurrentGameState == GameState.Playing)
        {
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

            // วาดศัตรู
            foreach (var enemy in enemies)
            {
                enemy.Draw(_spriteBatch);
            }

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
                new string[] {
                    "The Queue",
                    "",
                    "Some gods want fancy temples.",
                    "Others rule through fear.",
                    "",
                    "But The Queue is different.",
                    "It controls people by making them wait... forever."
                }, fadeSpeed: 0.3f, displayTime: 6f),

            // The protagonist
            new SplashScreenData(
                new string[] {
                    "I never liked standing still.",
                    "",
                    "While others waited in endless lines,",
                    "I chose to keep moving, to stay active,",
                    "To live life the way I wanted.",
                    "",
                    "That's why they came after me."
                }, fadeSpeed: 0.4f, displayTime: 6f),

            // The capture
            new SplashScreenData(
                new string[] {
                    "They dragged me to their temple,",
                    "Said I was the perfect sacrifice",
                    "Because I refused to stand in line.",
                    "",
                    "But during their ceremony,",
                    "Something went terribly wrong.",
                    "The whole temple fell into darkness."
                }, fadeSpeed: 0.4f, displayTime: 7f),

            // The stakes
            new SplashScreenData(
                new string[] {
                    "Now we're trapped in The Queue's world,",
                    "Where everyone stands frozen in lines.",
                    "",
                    "Its power is growing stronger,",
                    "Spreading across our world.",
                    "",
                    "I have to escape. I have to run."
                }, fadeSpeed: 0.5f, displayTime: 6f)
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
