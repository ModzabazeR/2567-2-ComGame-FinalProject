using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using FinalProject.GameObject;
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
        
        // โหลดเท็กซ์เจอร์ของศัตรู
        Texture2D enemyTexture = Content.Load<Texture2D>("_Jump");

        // สุ่มสร้างศัตรู
        enemies = new List<Enemy>();
        for (int i = 0; i < 1; i++) // สร้าง 5 ตัว
        {
            Vector2 spawnPosition = new Vector2(
                Singleton.Instance.Random.Next(100, 800), // X แบบสุ่ม
                600 // Y ให้อยู่บนพื้น
            );
            enemies.Add(new Enemy(enemyTexture, spawnPosition));
        }
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        Singleton.Instance.UpdateKeyboardState();

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

        // วาดศัตรู
        foreach (var enemy in enemies)
        {
            enemy.Draw(_spriteBatch);
        }
        
        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
