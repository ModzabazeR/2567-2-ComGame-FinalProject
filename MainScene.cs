using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using FinalProject.GameObject;
using System.Collections.Generic;
using FinalProject.Level;

namespace FinalProject;

public class MainScene : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Player player;
    private Camera camera;
    private LevelManager levelManager;

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

        // Create player texture
        Texture2D playerTexture = new Texture2D(GraphicsDevice, 32, 32);
        Color[] playerData = new Color[32 * 32];
        for (int i = 0; i < playerData.Length; i++)
            playerData[i] = Color.Red;
        playerTexture.SetData(playerData);

        // Initialize systems
        player = new Player(playerTexture, new Vector2(100, 100));
        camera = new Camera(Singleton.Instance.ScreenWidth, Singleton.Instance.ScreenHeight);
        levelManager = new LevelManager(GraphicsDevice);

        Singleton.Instance.Font = Content.Load<SpriteFont>("GameFont");
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

        // Update player with visible platforms
        var visiblePlatforms = levelManager.GetVisiblePlatforms(cameraBounds);
        player.Update(gameTime, visiblePlatforms);

        // Update camera to follow player
        camera.Follow(player.Position, new Rectangle(0, 0, 4000, 1200));

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        // Get camera bounds for visibility checking
        Rectangle cameraBounds = new Rectangle(
            (int)camera.Position.X,
            (int)camera.Position.Y,
            Singleton.Instance.ScreenWidth,
            Singleton.Instance.ScreenHeight
        );

        _spriteBatch.Begin(transformMatrix: camera.Transform);

        // Draw visible rooms
        levelManager.Draw(_spriteBatch, cameraBounds);

        // Draw player
        player.Draw(_spriteBatch);

        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
