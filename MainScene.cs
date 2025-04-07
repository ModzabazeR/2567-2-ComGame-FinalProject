using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using FinalProject.GameObject;
using FinalProject.GameObject.Entity;
using FinalProject.GameObject.Entity.Enemy;
using FinalProject.GameObject.Weapon;

using FinalProject.Utils.MapManager;
using FinalProject.Utils.SplashScreen;

using System.Collections.Generic;
using System;
using FinalProject.Utils.BGMManager;
using FinalProject.Utils.SFXManager;

namespace FinalProject;

public class MainScene : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Player player;
    private Camera camera;
    private MapManager mapManager; // Add this field

    private SplashScreenSequence _currentSequence;
    private bool _isMap3ClearedCutscene = false;
    private Texture2D hpTexture;

    private List<Bullet> bullets = new();
    private List<Bullet> enemyBullets = new();
    private bool _isIntroSongPlay = false; // Flag to check if song is requested
    private Rectangle restartButtonRect;
    private MouseState previousMouseState;
    private bool isEnteredMap2Cleared;

    // Add with other texture fields at the top
    private Texture2D _inventorySlotEmpty;
    private Texture2D _inventorySlotSelected;
    private Texture2D _crowbarIcon;
    private Texture2D _pistolIcon;
    private Texture2D _shotgunIcon;
    private Texture2D _grenadeIcon;

    private Crowbar crowbar;
    private Shotgun shotgun;
    private Pistol pistol;
    private Grenade grenade;

    private float timeRemaining = 60f * 10f; // 60 วินาที
    private SpriteFont timerFont;

    public static List<ExplosionZone> explosionZones = new();
    public bool isCameraZoomChanged = false; // Flag to check if camera zoom has changed


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

        BGMManager.Instance.Initialize(Content);
        SFXManager.Instance.Initialize(Content);

        StartIntroSequence();
        _isIntroSongPlay = true; // Request to play the song
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // Load the font first since splash screens need it
        Singleton.Instance.Font = Content.Load<SpriteFont>("GameFont");

        // bossbullet
        Singleton.Instance.EnemyBullets = enemyBullets;

        // Start the intro sequence
        StartIntroSequence();

        // Initialize map manager
        mapManager = new MapManager();

        // Create player texture
        //Texture2D playerTexture = new Texture2D(GraphicsDevice, 32, 32);
        //Color[] playerData = new Color[32 * 32];
        //for (int i = 0; i < playerData.Length; i++)
        //    playerData[i] = Color.Red;
        //playerTexture.SetData(playerData);
        Texture2D idleTexture = Content.Load<Texture2D>("Textures/player_movements/Player_Idle");
        Texture2D runTexture = Content.Load<Texture2D>("Textures/player_movements/Player_Walk");
        Texture2D jumpTexture = Content.Load<Texture2D>("Textures/player_movements/Player_Jump");

        Texture2D crowbarIdle = Content.Load<Texture2D>("Textures/player_weapons/crowbar/Crowbar_Idle");
        Texture2D crowbarWalk = Content.Load<Texture2D>("Textures/player_weapons/crowbar/Crowbar_Walk");
        Texture2D crowbarAtk = Content.Load<Texture2D>("Textures/player_weapons/crowbar/Crowbar_Attack");

        Texture2D pistolIdle = Content.Load<Texture2D>("Textures/player_weapons/pistol/Pistol_Idle");
        Texture2D pistolWalk = Content.Load<Texture2D>("Textures/player_weapons/pistol/Pistol_Walk");
        Texture2D pistolEmpty = Content.Load<Texture2D>("Textures/player_weapons/pistol/Pistol_Empty");
        Texture2D pistolReload = Content.Load<Texture2D>("Textures/player_weapons/pistol/Pistol_Reload");
        Texture2D pistolShoot = Content.Load<Texture2D>("Textures/player_weapons/pistol/Pistol_Shoot");

        Texture2D shotgunIdle = Content.Load<Texture2D>("Textures/player_weapons/shotgun/Shotgun_Idle");
        Texture2D shotgunWalk = Content.Load<Texture2D>("Textures/player_weapons/shotgun/Shotgun_Walk");
        Texture2D shotgunReload = Content.Load<Texture2D>("Textures/player_weapons/shotgun/Shotgun_Reload");
        Texture2D shotgunShoot = Content.Load<Texture2D>("Textures/player_weapons/shotgun/Shotgun_Shoot");

        Texture2D grenadeIdle = Content.Load<Texture2D>("Textures/player_weapons/grenade/Grenade_Idle");
        Texture2D grenadeWalk = Content.Load<Texture2D>("Textures/player_weapons/grenade/Grenade_Walk");
        Texture2D grenadeThrow = Content.Load<Texture2D>("Textures/player_weapons/grenade/Grenade_Throw");

        Singleton.Instance.CrowbarTexture = Content.Load<Texture2D>("Textures/player_weapons/crowbar/Crowbar");
        Singleton.Instance.PistolTexture = Content.Load<Texture2D>("Textures/player_weapons/pistol/Glock - P80 [64x48]");
        Singleton.Instance.GrenadeTexture = Content.Load<Texture2D>("Textures/player_weapons/grenade/Grenade-2");
        Singleton.Instance.ShotgunTexture = Content.Load<Texture2D>("Textures/player_weapons/shotgun/Shotgun");
        Singleton.Instance.PistolAmmoTexture = Content.Load<Texture2D>("Textures/player_weapons/pistol/Pistol_Ammo");
        Singleton.Instance.ShotgunAmmoTexture = Content.Load<Texture2D>("Textures/player_weapons/shotgun/Shotgun_Ammo");

        Texture2D zombieIdle = Content.Load<Texture2D>("Textures/zombie/Zombie_Idle");
        Texture2D zombieWalk = Content.Load<Texture2D>("Textures/zombie/Zombie_Walk");
        Texture2D zombieAttack = Content.Load<Texture2D>("Textures/zombie/Zombie_Attack");
        Texture2D zombieDeath = Content.Load<Texture2D>("Textures/zombie/Zombie_Death");

        Texture2D bossIdle = Content.Load<Texture2D>("Textures/boss/Boss_Idle");
        Texture2D bossSwordAtk = Content.Load<Texture2D>("Textures/boss/Boss_SwordATK");
        Texture2D bossGunAtk = Content.Load<Texture2D>("Textures/boss/Boss_GunATK");
        Texture2D bossPickupGrenade = Content.Load<Texture2D>("Textures/boss/Boss_PickupGrenade");
        Texture2D bossThrowGrenade = Content.Load<Texture2D>("Textures/boss/Boss_ThrowGrenade");
        Texture2D bossDeath = Content.Load<Texture2D>("Textures/boss/Boss_Death");

        _inventorySlotEmpty = Content.Load<Texture2D>("Textures/inventory/Inventory_Slot_Empty");
        _inventorySlotSelected = Content.Load<Texture2D>("Textures/inventory/Inventory_Slot_Selected");
        _crowbarIcon = Content.Load<Texture2D>("Textures/inventory/Crowbar_Icon");
        _pistolIcon = Content.Load<Texture2D>("Textures/inventory/Pistol_Icon");
        _shotgunIcon = Content.Load<Texture2D>("Textures/inventory/Shotgun_Icon");
        _grenadeIcon = Content.Load<Texture2D>("Textures/inventory/Grenade_Icon");


        Singleton.Instance.Animations["Player"] = new Dictionary<string, Animation> {
            { "Idle", new Animation(idleTexture, 32, 75, 3, 0.33f) },
            { "Walk", new Animation(runTexture, 48, 75, 8, 0.125f) },
            { "Jump", new Animation(jumpTexture, 75, 75, 12, 0.083f) },

            { "Crowbar_Idle", new Animation(crowbarIdle, 38, 75, 3, 0.33f) },
            {"Crowbar_Walk", new Animation(crowbarWalk, 48, 75, 8, 0.125f) },
            { "Crowbar_Attack", new Animation(crowbarAtk, 48, 75, 4, 0.25f) },

            { "Pistol_Idle", new Animation(pistolIdle, 32, 75, 3, 0.33f) },
            { "Pistol_Walk", new Animation(pistolWalk, 44, 75, 8, 0.125f) },
            { "Pistol_Empty", new Animation(pistolEmpty, 48, 75, 10, 0.1f) },
            { "Pistol_Reload", new Animation(pistolReload, 48, 75, 10, 0.1f) },
            { "Pistol_Shoot", new Animation(pistolShoot, 48, 75, 10, 0.1f) }, // Quicker shot

            { "Shotgun_Idle", new Animation(shotgunIdle, 48, 75, 3, 0.33f) },
            { "Shotgun_Walk", new Animation(shotgunWalk, 60, 75, 8, 0.125f) },
            { "Shotgun_Reload", new Animation(shotgunReload, 60, 75, 10, 0.1f) },
            { "Shotgun_Shoot", new Animation(shotgunShoot, 60, 75, 10, 0.1f) }, // Faster shot

            { "Grenade_Idle", new Animation(grenadeIdle, 30, 75, 3, 0.33f) },
            { "Grenade_Walk", new Animation(grenadeWalk, 48, 75, 8, 0.125f) },
            { "Grenade_Throw", new Animation(grenadeThrow, 48, 75, 8, 0.125f) }
        };

        Singleton.Instance.Animations["Zombie"] = new Dictionary<string, Animation>
        {
            { "Idle", new Animation(zombieIdle, 30, 75, 3, 0.33f) },
            { "Walk", new Animation(zombieWalk, 52, 75, 8, 0.125f) },
            { "Attack", new Animation(zombieAttack, 48, 75, 4, 0.25f) },
            { "Death", new Animation(zombieDeath, 96, 75, 5, 0.2f) }
        };

        Singleton.Instance.Animations["Boss"] = new Dictionary<string, Animation> {
            { "Idle", new Animation(zombieIdle, 552, 540, 3, 0.33f) },
            { "SwordAttack", new Animation(bossSwordAtk, 576, 540, 6, 0.168f) },
            { "GunAttack", new Animation(bossGunAtk, 576, 540, 6, 0.168f) },
            { "PickupGrenade", new Animation(bossPickupGrenade, 576, 540, 6, 0.168f) },
            { "ThrowGrenade", new Animation(bossThrowGrenade, 576, 540, 4, 0.25f) },
            { "Death", new Animation(bossDeath, 576, 540, 6, 0.168f) }
        };

        // Load map textures
        Texture2D map1Texture = Content.Load<Texture2D>("Textures/maps/level1");
        Texture2D map2Texture = Content.Load<Texture2D>("Textures/maps/level2");
        Texture2D map3Texture = Content.Load<Texture2D>("Textures/maps/level3");
        Texture2D map4Texture = Content.Load<Texture2D>("Textures/maps/level4");
        Texture2D map5Texture = Content.Load<Texture2D>("Textures/maps/level5");
        Texture2D map6Texture = Content.Load<Texture2D>("Textures/maps/level6");
        Texture2D map7Texture = Content.Load<Texture2D>("Textures/maps/level7");
        Texture2D map2ClearedTexture = Content.Load<Texture2D>("Textures/maps/level2_cleared");
        Texture2D map8Texture = Content.Load<Texture2D>("Textures/maps/level8");

        Texture2D map1Overlay = Content.Load<Texture2D>("Textures/maps/level1_overlay");
        Texture2D map3Overlay = Content.Load<Texture2D>("Textures/maps/level3_overlay");
        Texture2D map4Overlay = Content.Load<Texture2D>("Textures/maps/level4_overlay");
        Texture2D map6Overlay = Content.Load<Texture2D>("Textures/maps/level6_overlay");

        // Add maps with their collision data
        mapManager.AddMap("Map 1", map1Texture, new Vector2(0, 0), "Content/Maps/level1_collision.lcm");
        mapManager.AddMap("Map 2", map2Texture, new Vector2(2000, 0), "Content/Maps/level2_collision.lcm", 28, 50);
        mapManager.AddMap("Map 3", map3Texture, new Vector2(5000, 0), "Content/Maps/level3_collision.lcm");
        mapManager.AddMap("Map 4", map4Texture, new Vector2(8000, 0), "Content/Maps/level4_collision.lcm");
        mapManager.AddMap("Map 5", map5Texture, new Vector2(11000, 0), "Content/Maps/level5_collision.lcm");
        mapManager.AddMap("Map 6", map6Texture, new Vector2(14000, 0), "Content/Maps/level6_collision.lcm");
        mapManager.AddMap("Map 7", map7Texture, new Vector2(17000, 0), "Content/Maps/level7_collision.lcm");
        mapManager.AddMap("Map 2 Cleared", map2ClearedTexture, new Vector2(20000, 0), "Content/Maps/level2_cleared_collision.lcm", 28, 50);
        mapManager.AddMap("Boss", map8Texture, new Vector2(23000, 0), "Content/Maps/level8_collision.lcm");

        // Add overlays for specific maps
        mapManager.AddOverlay("Map 1", map1Overlay);
        mapManager.AddOverlay("Map 3", map3Overlay);
        mapManager.AddOverlay("Map 4", map4Overlay);
        mapManager.AddOverlay("Map 6", map6Overlay);

        // Set spawn points for each map (relative to map position)
        mapManager.SetMapSpawnPoint("Map 1", 3, 10);
        mapManager.SetMapSpawnPoint("Map 2", 2, 3);
        mapManager.SetMapSpawnPoint("Map 3", 35, 15);
        mapManager.SetMapSpawnPoint("Map 4", 2, 8);
        mapManager.SetMapSpawnPoint("Map 5", 2, 15);
        mapManager.SetMapSpawnPoint("Map 6", 2, 15);
        mapManager.SetMapSpawnPoint("Map 7", 2, 15);
        mapManager.SetMapSpawnPoint("Map 2 Cleared", 25, 30);
        mapManager.SetMapSpawnPoint("Boss", 2, 15);

        // Initialize camera
        camera = new Camera(Singleton.Instance.ScreenWidth, Singleton.Instance.ScreenHeight);

        var spawnroom = mapManager.GetMap("Boss");

        // Initialize player at Map 1's spawn point
        var map1 = mapManager.GetMap("Map 1");
        player = new Player(Singleton.Instance.Animations["Player"], spawnroom.SpawnPoint, mapManager);

        Singleton.Instance.Player = player;
        Singleton.Instance.MapManager = mapManager;

        mapManager.SetPlayer(player);

        // Add doors between maps (coordinates are relative to each map's position)
        mapManager.AddMapDoor("Map 1", 38, 17, "Map 2", 2, 3);
        mapManager.AddMapDoor("Map 2", 0, 7, "Map 1", 35, 15);

        mapManager.AddMapDoor("Map 2", 0, 29, "Map 3", 35, 15);
        mapManager.AddMapDoor("Map 3", 38, 17, "Map 2", 2, 28);

        mapManager.AddMapDoor("Map 4", 38, 17, "Map 5", 2, 15);
        mapManager.AddMapDoor("Map 5", 0, 17, "Map 4", 35, 15);

        mapManager.AddMapDoor("Map 5", 38, 17, "Map 6", 2, 15);
        mapManager.AddMapDoor("Map 6", 0, 17, "Map 5", 35, 15);

        mapManager.AddMapDoor("Map 6", 0, 10, "Map 7", 2, 15);
        mapManager.AddMapDoor("Map 7", 0, 17, "Map 6", 2, 7);

        mapManager.AddMapDoor("Map 7", 0, 6, "Map 2 Cleared", 25, 32);
        mapManager.AddMapDoor("Map 2 Cleared", 27, 33, "Map 7", 2, 5);

        mapManager.AddMapDoor("Map 2 Cleared", 0, 7, "Map 1", 35, 15);
        mapManager.AddMapDoor("Map 2 Cleared", 0, 29, "Map 3", 35, 15);
        mapManager.AddMapDoor("Map 2 Cleared", 27, 13, "Boss", 2, 15);

        mapManager.AddMapDeadZone("Map 2", 1, 38, 26, 11);
        mapManager.AddMapDeadZone("Map 2 Cleared", 1, 38, 26, 11);
        mapManager.AddMapDeadZone("Map 5", 14, 21, 3, 1);
        mapManager.AddMapDeadZone("Map 5", 25, 21, 3, 1);
        mapManager.AddMapDeadZone("Map 6", 29, 21, 9, 1);
        mapManager.AddMapDeadZone("Map 7", 9, 21, 5, 1);
        mapManager.AddMapDeadZone("Map 7", 32, 21, 6, 1);

        // Subscribe to Map 3's cleared event
        var map3 = mapManager.GetMap("Map 3");
        map3.OnMapCleared += () => ShowMap3ClearedCutscene();

        var bossMap = mapManager.GetMap("Boss");
        bossMap.OnMapCleared += () => StartOutroSequence();

        // สร้าง texture สีแดง 1x1 ใช้สำหรับวาด HP bar
        hpTexture = new Texture2D(GraphicsDevice, 1, 1);
        hpTexture.SetData(new[] { Color.Red });

        timerFont = Singleton.Instance.Font; // ใช้ font เดียวกัน

        // ตั้งปุ่ม Restart
        restartButtonRect = new Rectangle(
            Singleton.Instance.ScreenWidth / 2 - 100,
            Singleton.Instance.ScreenHeight / 2,
            200, 60
        );
    }

    protected override void Update(GameTime gameTime)
    {
        Singleton.Instance.UpdateKeyboardState();

        if (_isIntroSongPlay == true && BGMManager.Instance != null)
        {
            // Load background music after BGMManager is initialized
            BGMManager.Instance.PlayMainTheme1();
            _isIntroSongPlay = false;
        }

        for (int i = explosionZones.Count - 1; i >= 0; i--)
        {
            explosionZones[i].Update(gameTime, player);
            if (explosionZones[i].IsFinished())
                explosionZones.RemoveAt(i);
        }


        // Handle splash screen and cutscene states
        if (Singleton.Instance.CurrentGameState == GameState.Splash ||
            Singleton.Instance.CurrentGameState == GameState.Cutscene)
        {
            // Add this check for Escape key to skip intro
            if (Singleton.Instance.CurrentKey.IsKeyDown(Keys.Escape) &&
                Singleton.Instance.PreviousKey.IsKeyUp(Keys.Escape))
            {

                BGMManager.Instance.PlayMainTheme2();
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

                    // If this was the Map 3 cleared cutscene, move player to Map 4
                    if (_isMap3ClearedCutscene)
                    {
                        var map4 = mapManager.GetMap("Map 4");
                        player.Position = map4.SpawnPoint;
                        _isMap3ClearedCutscene = false;
                    }
                }
                return;
            }
        }

        // Only update game logic if in Playing state
        if (Singleton.Instance.CurrentGameState == GameState.Playing)
        {
            timeRemaining -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (timeRemaining <= 0f)
            {
                Singleton.Instance.CurrentGameState = GameState.GameOver;
            }
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

            if (!player.IsAlive)
            {
                Singleton.Instance.CurrentGameState = GameState.GameOver;

                // ตั้งตำแหน่งปุ่ม
                restartButtonRect = new Rectangle(
                    Singleton.Instance.ScreenWidth / 2 - 100,
                    Singleton.Instance.ScreenHeight / 2,
                    200, 60
                );
            }
            // ดึงกระสุนที่ Player ยิงมาจาก Player
            bullets.AddRange(player.CollectBullets());

            // กระสุน
            for (int i = bullets.Count - 1; i >= 0; i--)
            {
                var bullet = bullets[i];
                bullet.Update(gameTime, mapManager.GetAllSolidTiles());

                bool hitEnemy = false;

                foreach (var map in mapManager.GetMaps().Values)
                {
                    if (!map.IsVisible(cameraBounds)) continue;

                    foreach (var enemy in map.GetEnemiesCopy())
                    {
                        if (!enemy.IsSpawned || enemy.IsDefeated) continue;

                        if (bullet.Bounds.Intersects(enemy.Bounds))
                        {
                            enemy.hit(2);
                            hitEnemy = true;
                            break;
                        }
                    }
                }

                if (!bullet.IsActive() || hitEnemy)
                {
                    bullets.RemoveAt(i); // ลบกระสุนเมื่อชนกำแพงหรือศัตรู
                }
            }

            for (int i = enemyBullets.Count - 1; i >= 0; i--)
            {
                var bullet = enemyBullets[i];
                bullet.Update(gameTime, mapManager.GetAllSolidTiles());

                // ตรวจ collision กับ player
                if (bullet.Bounds.Intersects(player.Bounds))
                {
                    Vector2 knockback = Vector2.Normalize(player.Position - bullet.Position);
                    player.TakeDamage(1, knockback);
                    enemyBullets.RemoveAt(i);
                    continue;
                }

                if (!bullet.IsActive())
                {
                    enemyBullets.RemoveAt(i);
                }
            }



            // Check for map transitions
            mapManager.CheckMapTransitions(gameTime);

            // Update current map tracking
            mapManager.UpdateCurrentMap();


            if (!isEnteredMap2Cleared && mapManager.CurrentMap.Name == "Map 2 Cleared")
            {
                isEnteredMap2Cleared = true;
                mapManager.ReplaceMapDoor("Map 1", 0, "Map 2 Cleared");
                mapManager.ReplaceMapDoor("Map 3", 0, "Map 2 Cleared");
            }

            if (!isCameraZoomChanged && mapManager.CurrentMap.Name == "Boss")
            {
                camera.SetZoom(1f);
                isCameraZoomChanged = true;
            }

            foreach (var map in mapManager.GetMaps().Values)
            {
                map.Update(gameTime, player.Position);
                if (map.IsVisible(cameraBounds))
                {
                    foreach (var enemy in map.GetEnemiesCopy())
                    {
                        enemy.Update(gameTime, mapManager.GetAllSolidTiles());

                        if (enemy.Bounds.Intersects(player.Bounds) && enemy.IsSpawned && !enemy.IsDefeated)
                        {
                            Vector2 knockback = Vector2.Normalize(player.Position - enemy.Position);
                            player.TakeDamage(1, knockback);
                        }

                        // ตรวจสอบการโจมตี
                        if (player.IsAttacking && enemy.IsSpawned && !enemy.IsDefeated &&
                            enemy.Bounds.Intersects(player.GetAttackHitbox()))
                        {
                            if (!enemy.isInvincible)
                            {
                                if (player.CurrentWeapon == 0) {
                                    if (player.PrimaryWeapon is Crowbar)
                                    {
                                        enemy.hit(3);
                                    }
                                    else if (player.PrimaryWeapon is Shotgun)
                                    {
                                        enemy.hit(10);
                                    }
                                }
                                else if (player.CurrentWeapon == 1)
                                {
                                    if (player.SecondaryWeapon is Crowbar)
                                    {
                                        enemy.hit(3);
                                    }
                                    else if (player.SecondaryWeapon is Shotgun)
                                    {
                                        enemy.hit(10);
                                    }
                                }
                            }
                        }
                    }

                    // เพิ่ม:
                    for (int i = map.GetWeapons().Count - 1; i >= 0; i--)
                    {
                        var weapon = map.GetWeapons()[i];
                        weapon.Update(gameTime, mapManager.GetAllSolidTiles());

                        if (player.Bounds.Intersects(weapon.Bounds))
                        {
                            bool wasPickedUp = false;

                            if (weapon is Grenade)
                            {
                                player.PickupGrenade();
                                wasPickedUp = true;
                            }
                            else
                            {
                                wasPickedUp = player.PickupWeapon(weapon);
                            }

                            // Only remove if successfully picked up
                            if (wasPickedUp)
                            {
                                map.GetWeapons().RemoveAt(i);
                            }
                        }
                    }
                }
            }
        }

        if (Singleton.Instance.CurrentGameState == GameState.GameOver)
        {
            MouseState mouse = Mouse.GetState();

            if (mouse.LeftButton == ButtonState.Pressed && previousMouseState.LeftButton == ButtonState.Released)
            {
                if (restartButtonRect.Contains(mouse.Position))
                {
                    RestartGame(); // ⬅ เราจะเพิ่มฟังก์ชันนี้ด้านล่าง
                }
            }

            previousMouseState = mouse;
        }


        base.Update(gameTime);
    }

    private void DrawInventory(SpriteBatch spriteBatch)
    {
        const int slotSize = 64;
        const int padding = 10;
        Vector2 basePosition = new Vector2(padding, Singleton.Instance.ScreenHeight - slotSize - padding);

        // Draw weapon slots
        for (int i = 0; i < 2; i++)
        {
            Rectangle slotRect = new Rectangle(
                (int)basePosition.X + (slotSize + padding) * i,
                (int)basePosition.Y,
                slotSize,
                slotSize
            );

            // Draw slot background
            spriteBatch.Draw(_inventorySlotEmpty, slotRect, Color.White);

            // Draw active slot highlight
            if (player.CurrentWeapon == i)
            {
                spriteBatch.Draw(_inventorySlotSelected, slotRect, Color.White * 0.5f);
            }

            // Draw weapon icon
            // Get weapon and icon
            Weapon weapon = i == 0 ? player.PrimaryWeapon : player.SecondaryWeapon;
            Texture2D icon = GetWeaponIcon(weapon);
            if (icon != null)
            {
                spriteBatch.Draw(icon, slotRect, Color.White);

                // Draw ammo count for range weapons
                if (weapon is RangeWeapon rangeWeapon)
                {
                    string ammoText = $"{rangeWeapon.GetCurrentAmmo()}";
                    Vector2 textSize = Singleton.Instance.Font.MeasureString(ammoText);
                    Vector2 textPosition = new Vector2(
                        slotRect.Center.X - textSize.X / 2,
                        slotRect.Bottom - textSize.Y - 5
                    );

                    // Draw black outline for better visibility
                    spriteBatch.DrawString(Singleton.Instance.Font, ammoText, textPosition + new Vector2(-1, 0), Color.Black);
                    spriteBatch.DrawString(Singleton.Instance.Font, ammoText, textPosition + new Vector2(1, 0), Color.Black);
                    spriteBatch.DrawString(Singleton.Instance.Font, ammoText, textPosition + new Vector2(0, -1), Color.Black);
                    spriteBatch.DrawString(Singleton.Instance.Font, ammoText, textPosition + new Vector2(0, 1), Color.Black);

                    // Draw main text
                    spriteBatch.DrawString(Singleton.Instance.Font, ammoText, textPosition, Color.White);
                }
            }

            // Grenade slot
            Rectangle grenadeRect = new Rectangle(
                (int)basePosition.X + (slotSize + padding) * 2 + 30,
                (int)basePosition.Y,
                slotSize,
                slotSize
            );
            spriteBatch.Draw(_inventorySlotEmpty, grenadeRect, Color.White);
            if (player.GrenadeCount > 0)
                spriteBatch.Draw(_grenadeIcon, grenadeRect, Color.White);
        }
    }

    private Texture2D GetWeaponIcon(Weapon weapon)
    {
        // Use type checking with full namespace paths
        if (weapon is Crowbar)
            return _crowbarIcon;
        if (weapon is Pistol)
            return _pistolIcon;
        if (weapon is Shotgun)
            return _shotgunIcon;
        return null;
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
            foreach (var map in mapManager.GetMaps().Values)
            {
                if (map.IsVisible(cameraBounds))
                {
                    foreach (var enemy in map.GetEnemiesCopy())
                    {
                        enemy.Draw(_spriteBatch);
                    }
                    foreach (var weapon in map.GetWeapons())
                    {
                        weapon.Draw(_spriteBatch);
                    }
                }
            }
            foreach (var bullet in bullets)
            {
                bullet.Draw(_spriteBatch);
            }
            foreach (var bullet in enemyBullets)
            {
                bullet.Draw(_spriteBatch);
            }
            // พื้นระเบิด
            foreach (var zone in explosionZones)
            {
                zone.Draw(_spriteBatch);
            }


            // Draw player
            player.Draw(_spriteBatch);
            if (player.IsAttacking)
            {
                var attackBox = player.GetAttackHitbox();

                Texture2D redTexture = new Texture2D(GraphicsDevice, 1, 1);
                redTexture.SetData(new[] { Color.Red });

                _spriteBatch.Draw(
                    redTexture,
                    attackBox,
                    Color.Red * 0.5f // โปร่งใสหน่อย
                );
            }

            // Draw map overlays (if any)
            mapManager.DrawOverlays(_spriteBatch, cameraBounds);

            _spriteBatch.End();

            // ====== Draw UI (HP Bar) ======
            _spriteBatch.Begin(); // No transform for screen-space UI

            int barWidth = 200;
            int barHeight = 20;
            float hpRatio = (float)player.CurrentHP / player.MaxHP;

            Rectangle hpBack = new Rectangle(10, 10, barWidth, barHeight);
            Rectangle hpFront = new Rectangle(10, 10, (int)(barWidth * hpRatio), barHeight);

            _spriteBatch.Draw(hpTexture, hpBack, Color.DarkGray);
            _spriteBatch.Draw(hpTexture, hpFront, Color.Red);

            DrawInventory(_spriteBatch); // Draw inventory slots


            // Draw timer
            string timeText = $"Time: {Math.Ceiling(timeRemaining)}";
            Console.WriteLine(timeText);
            Vector2 timeSize = timerFont.MeasureString(timeText);

            _spriteBatch.DrawString(
                timerFont,
                timeText,
                new Vector2(Singleton.Instance.ScreenWidth - timeSize.X - 20, 10),
                Color.Red
            );


            _spriteBatch.End(); // ===== End UI rendering =====


        }


        else if (Singleton.Instance.CurrentGameState == GameState.GameOver)
        {
            GraphicsDevice.Clear(Color.Black);
            _spriteBatch.Begin();

            // ข้อความ Game Over
            string message = "Game Over";
            Vector2 textSize = Singleton.Instance.Font.MeasureString(message);
            _spriteBatch.DrawString(
                Singleton.Instance.Font,
                message,
                new Vector2(Singleton.Instance.ScreenWidth / 2 - textSize.X / 2, 200),
                Color.White
            );

            // ปุ่ม restart
            Texture2D buttonTex = new Texture2D(GraphicsDevice, 1, 1);
            buttonTex.SetData(new[] { Color.Gray });

            _spriteBatch.Draw(buttonTex, restartButtonRect, Color.White);
            _spriteBatch.DrawString(
                Singleton.Instance.Font,
                "Restart",
                new Vector2(restartButtonRect.X + 50, restartButtonRect.Y + 15),
                Color.Black
            );

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

    private void StartOutroSequence()
    {
        var outroScreens = new List<SplashScreenData>
        {
            // The escape
            new SplashScreenData(
                [
                    "I've done it.",
                    "",
                    "The Queue's temple lies in ruins behind me.",
                    "Its power is broken, its influence fading.",
                    "",
                    "The endless lines are dissolving into nothingness.",
                    "People are finally free to move as they choose."
                ], fadeSpeed: 0.4f, displayTime: 6f),

            // The aftermath
            new SplashScreenData(
                [
                    "But I know this isn't the end.",
                    "",
                    "Somewhere, another temple might be rising.",
                    "Another force trying to control through waiting.",
                    "",
                    "As long as there are those who resist standing still,",
                    "There will be those who try to make them."
                ], fadeSpeed: 0.4f, displayTime: 6f),

            // The resolution
            new SplashScreenData(
                [
                    "I'll keep moving forward.",
                    "Keep fighting against those who would trap us in lines.",
                    "",
                    "Because life isn't meant to be lived standing still.",
                    "It's meant to be lived in motion.",
                    "",
                    "And I intend to live it fully."
                ], fadeSpeed: 0.5f, displayTime: 7f),

            // The final message
            new SplashScreenData(
                [
                    "Thank you for playing",
                    "",
                    "Thesia"
                ], fadeSpeed: 0.6f, displayTime: 4f)
        };

        _currentSequence = new SplashScreenSequence(outroScreens);
        Singleton.Instance.CurrentGameState = GameState.Cutscene;
    }


    public void ShowCutscene(params SplashScreenData[] screens)
    {
        BGMManager.Instance.PlayMainTheme1();
        _currentSequence = new SplashScreenSequence(screens);
        Singleton.Instance.CurrentGameState = GameState.Cutscene;
    }

    public void ShowSimpleCutscene(string[] text, float fadeSpeed = 1f, float displayTime = 2f)
    {
        ShowCutscene(new SplashScreenData(text, fadeSpeed, displayTime));
    }

    private void RestartGame()
    {
        Exit(); // ปิดเกม instance นี้

        // เรียกตัวเองใหม่
        System.Diagnostics.Process.Start(Environment.ProcessPath);
    }


    private void ShowMap3ClearedCutscene()
    {
        _isMap3ClearedCutscene = true;
        ShowCutscene(new SplashScreenData(
            [
                "*Crack...*",
                "",
                "The ground beneath my feet begins to tremble.",
                "The air grows thick with an ancient power.",
                "",
                "*BOOM!*",
                "",
                "The floor gives way, and I'm falling...",
                "",
                "The Queue's influence grows stronger here.",
                "I can feel its weight pressing down on me.",
                "I must escape before it's too late."
            ], fadeSpeed: 0.4f, displayTime: 5f));
    }
}
