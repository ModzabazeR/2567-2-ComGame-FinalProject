using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using FinalProject.GameObject;
using System.Collections.Generic;
using System;
using System.Linq;

using DragonBones;
using Animation = FinalProject.GameObject.Animation;
using DBAnimation = DragonBones.Animation;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using System.IO;
using Newtonsoft.Json;
using GameEnemy = FinalProject.GameObject.Enemy;

namespace FinalProject;

public class MainScene : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Player player;
    private Camera camera;
    private MapManager mapManager; // Add this field

    private DragonBones.DragonBones _dragonBones;
    private MonoGameDragonBonesFactory _factory;

    // Dictionary to store character armatures by type
    private Dictionary<string, DragonBonesCharacter> _characterTemplates = new Dictionary<string, DragonBonesCharacter>();

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

        // --- STEP 1: Initialize DragonBones animation system FIRST ---
        Console.WriteLine("Initializing DragonBones system...");
        _dragonBones = new DragonBones.DragonBones(new MonoGameEventDispatcher());
        _factory = new MonoGameDragonBonesFactory(GraphicsDevice, Content, _dragonBones);

        // --- STEP 2: Create character templates for all characters ---
        Console.WriteLine("Creating DragonBones character templates...");
        CreateCharacterTemplate(
            "player",
            "Content/DragonBones_resource/Player_ske.json",
            "Content/DragonBones_resource/Player_tex.json",
            "DragonBones_resource/Player_tex"
        );

        // Add animation mappings to the template
        if (_characterTemplates.ContainsKey("player"))
        {
            // Only add mappings for animations that actually exist
            _characterTemplates["player"].AddAnimationMapping("IDLE", "idle");
            _characterTemplates["player"].AddAnimationMapping("WALK", "walk");
            // Don't add JUMP if it doesn't exist
            Console.WriteLine("Animation mappings added to player template");
        }
        else
        {
            Console.WriteLine("Failed to create player template, loading fallback animations");
        }

        // --- STEP 3: Load fallback textures in case DragonBones fails ---
        Console.WriteLine("Loading fallback sprite textures...");
        Texture2D idleTexture = null;
        Texture2D runTexture = null;
        Texture2D jumpTexture = null;

        try {
            idleTexture = Content.Load<Texture2D>("_Idle");
            runTexture = Content.Load<Texture2D>("_Run");
            jumpTexture = Content.Load<Texture2D>("_Jump");
        } catch (Exception ex) {
            Console.WriteLine($"Error loading fallback textures: {ex.Message}");
        }

        Dictionary<string, Animation> animations = new Dictionary<string, Animation>();
        if (idleTexture != null) 
            animations.Add("Idle", new Animation(idleTexture, idleTexture.Width / 10, idleTexture.Height, 10, 0.1f));
        if (runTexture != null) 
            animations.Add("Run", new Animation(runTexture, runTexture.Width / 10, runTexture.Height, 10, 0.08f));
        if (jumpTexture != null) 
            animations.Add("Jump", new Animation(jumpTexture, jumpTexture.Width, jumpTexture.Height, 1, 0.1f));

        // --- STEP 4: Create player AFTER templates are loaded ---
        Vector2 playerStartPosition = new Vector2(500, 1000); // More visible position
        Console.WriteLine($"Creating player at position: {playerStartPosition}");
        player = new Player(animations, playerStartPosition);

        // Create player debug visual and make it bigger/more visible
        Texture2D debugTexture = new Texture2D(GraphicsDevice, 1, 1);
        debugTexture.SetData(new[] { Color.Red });
        player.SetDebugTexture(debugTexture);

        // --- STEP 5: Assign DragonBones character to player ---
        if (_characterTemplates.ContainsKey("player"))
        {
            player.DragonBonesCharacter = CreateCharacterInstance(_characterTemplates["player"], player.Position);
            
            if (player.DragonBonesCharacter != null)
            {
                // Start with idle animation
                player.DragonBonesCharacter.PlayAnimation("IDLE");
                Console.WriteLine("DragonBones character assigned to player successfully");
            }
            else
            {
                Console.WriteLine("Failed to create DragonBones character instance");
            }
        }
        else
        {
            Console.WriteLine("No DragonBones template found for player");
        }

        // Create camera and load font
        camera = new Camera(Singleton.Instance.ScreenWidth, Singleton.Instance.ScreenHeight);
        camera.SetPosition(new Vector2(0, 0));
        Singleton.Instance.Font = Content.Load<SpriteFont>("GameFont");

        // Force the player to be at a visible position
        player.Position = new Vector2(400, 400);  // Center of screen
    }

    // Helper method to create a character template
    private void CreateCharacterTemplate(string name, string skeletonJsonPath, string atlasJsonPath, string texturePath)
    {
        try {
            // Create factory if not already created
            if (_factory == null)
            {
                _dragonBones = new DragonBones.DragonBones(new MonoGameEventDispatcher());
                _factory = new MonoGameDragonBonesFactory(GraphicsDevice, Content, _dragonBones);
            }
            
            // Load the texture
            Texture2D atlasTexture = Content.Load<Texture2D>(texturePath);
            
            // Parse the data directly
            string skeletonJson = File.ReadAllText(skeletonJsonPath);
            string atlasJson = File.ReadAllText(atlasJsonPath);
            
            // Parse DragonBones data first
            var dragonBonesData = _factory.ParseDragonBonesData(skeletonJson);
            if (dragonBonesData != null)
            {
                _factory.AddDragonBonesData(dragonBonesData);
                
                // Parse texture atlas data
                var textureAtlasData = JsonHelper.ParseTextureAtlasData(atlasJson);
                if (textureAtlasData != null && textureAtlasData is MonoGameTextureAtlasData monoAtlasData)
                {
                    // Assign the texture to the atlas data
                    monoAtlasData.texture = atlasTexture;
                    
                    // Add to factory
                    _factory.AddTextureAtlasData(textureAtlasData);
                    
                    // Get first armature name if available
                    string armatureName = dragonBonesData.armatureNames.Count > 0 ? 
                                        dragonBonesData.armatureNames[0] : 
                                        "Armature";
                    
                    // Build the armature
                    var armature = _factory.BuildArmature(armatureName);
                    
                    // Create display object
                    var display = new MonoGameArmatureDisplay(armature, atlasTexture);
                    
                    // Add to clock
                    _dragonBones.clock.Add(armature);
                    
                    // Create character
                    var character = new DragonBonesCharacter(armature, display, Vector2.Zero);
                    
                    // Add default mappings
                    foreach (var animName in armature.animation.animationNames)
                    {
                        character.AddAnimationMapping(animName.ToUpper(), animName);
                    }
                    
                    _characterTemplates[name] = character;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating character template: {ex.Message}");
        }
    }
    
    // Helper method to find an animation name that contains a specific term
    private string FindAnimationWithName(List<string> animations, string nameContains)
    {
        return animations.FirstOrDefault(a => a.ToLower().Contains(nameContains.ToLower()));
    }

    // Helper method to create an instance of a character from a template
    private DragonBonesCharacter CreateCharacterInstance(DragonBonesCharacter template, Vector2 position)
    {
        var armature = _factory.BuildArmature(template.Armature.name);
        var display = new MonoGameArmatureDisplay(armature, template.ArmatureDisplay.Texture);
        
        // Add to world clock
        _dragonBones.clock.Add(armature);
        
        // Create new character
        var character = new DragonBonesCharacter(armature, display, position);
        
        // Copy animation mappings
        foreach (var mapping in template._animationMappings)
        {
            character.AddAnimationMapping(mapping.Key, mapping.Value);
        }
        
        // Play default animation
        character.PlayAnimation("IDLE");
        
        return character;
    }

    protected override void Update(GameTime gameTime)
    {
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        _dragonBones.clock.AdvanceTime(deltaTime);
        
        base.Update(gameTime);
        
        // Update input state
        Singleton.Instance.PreviousKey = Singleton.Instance.CurrentKey;
        Singleton.Instance.CurrentKey = Keyboard.GetState();
        
        // Update player
        if (player != null)
        {
            // Debug output to verify player state
            if (Singleton.Instance.ShowDebugInfo)
            {
                Console.WriteLine($"Player position: {player.Position}, Velocity: {player.Velocity}");
                Console.WriteLine($"Player has DragonBones: {(player.DragonBonesCharacter != null ? "Yes" : "No")}");
            }
            
            player.Update(gameTime, mapManager.GetAllSolidTiles());
            
            // Update camera to follow player
            if (camera != null)
            {
                Rectangle mapBounds = mapManager.GetWorldBounds();
                camera.Follow(player.Position, mapBounds);
            }
        }
        
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
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        // Begin sprite batch with camera transform
        _spriteBatch.Begin(SpriteSortMode.Deferred, Microsoft.Xna.Framework.Graphics.BlendState.AlphaBlend, SamplerState.PointClamp, 
                         null, null, null, camera?.Transform);

        // Draw map
        Rectangle cameraBounds = new Rectangle(
            (int)camera.Position.X,
            (int)camera.Position.Y,
            Singleton.Instance.ScreenWidth,
            Singleton.Instance.ScreenHeight
        );
        mapManager.Draw(_spriteBatch, cameraBounds);

        // Only draw debug origin marker if debug is enabled
        if (Singleton.Instance.ShowDebugInfo)
        {
            Texture2D pixel = new Texture2D(GraphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.Yellow });
            
            // Draw camera corners
            _spriteBatch.Draw(pixel, new Rectangle((int)camera.Position.X, (int)camera.Position.Y, 20, 20), Color.Red);
            _spriteBatch.Draw(pixel, new Rectangle((int)camera.Position.X + Singleton.Instance.ScreenWidth - 20, 
                                                  (int)camera.Position.Y, 20, 20), Color.Blue);
            _spriteBatch.Draw(pixel, new Rectangle((int)camera.Position.X, 
                                                  (int)camera.Position.Y + Singleton.Instance.ScreenHeight - 20, 
                                                  20, 20), Color.Green);
            
            // Draw player position indicator - a bright cross that's easy to see
            int crossSize = 20;
            _spriteBatch.Draw(pixel, new Rectangle((int)player.Position.X - crossSize, (int)player.Position.Y, crossSize * 2, 5), Color.Magenta);
            _spriteBatch.Draw(pixel, new Rectangle((int)player.Position.X, (int)player.Position.Y - crossSize, 5, crossSize * 2), Color.Magenta);
        }

        // Draw player
        if (player != null)
        {
            try
            {
                player.Draw(_spriteBatch);
                
                // Only log position during startup
                if (gameTime.TotalGameTime.TotalSeconds < 2)
                {
                    Console.WriteLine($"Camera: {camera.Position}, Player: {player.Position}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error drawing player: {ex.Message}");
            }
        }

        // Draw enemies
        // foreach (var enemy in enemies)
        // {
        //     enemy.Draw(_spriteBatch);
        // }

        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
