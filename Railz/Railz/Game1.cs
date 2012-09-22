using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Railz
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {

        #region Variables

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Background background;
        AnimatedSprite Explosion;

        Player player;
        public int iPlayAreaTop = 30;
        public int iPlayAreaBottom = 630;
        int iMaxHorizontalSpeed = 8;
        float fBoardUpdateDelay = 0f;
        float fBoardUpdateInterval = 0.01f;

        int iBulletVerticalOffset = 12;
        int[] iBulletFacingOffsets = new int[2] { 70, 0 };
        static int iMaxBullets = 40;
        Bullet[] bullets = new Bullet[iMaxBullets];
        float fBulletDelayTimer = 0.0f;
        float fFireDelay = 0.15f;

        int speedMultiplier = 2;

        #endregion

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        #region Initialization

        
        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            graphics.PreferredBackBufferHeight = 720;
            graphics.PreferredBackBufferWidth = 1280;
            graphics.ApplyChanges();
            base.Initialize();
        }

        #endregion

        #region LoadContent

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            //
            // Explosion
            Explosion = new AnimatedSprite(
                Content.Load<Texture2D>(@"Textures\Explosions"),
                0, 0, 64, 64, 16);
            Explosion.X = 0;
            Explosion.Y = 0;            
            //
            // Background
            background = new Background(
                Content,
                @"Textures\PrimaryBackground",
                @"Textures\ParallaxStars");
            //
            // Player
            player = new Player(Content.Load<Texture2D>(@"Textures\PlayerShip"));
            //
            // Bullets
            bullets[0] = new Bullet(Content.Load<Texture2D>(@"Textures\PlayerBullet"));

            for (int x = 1; x < iMaxBullets; x++)
                bullets[x] = new Bullet();

            // TODO: use this.Content to load your game content here
        }

        #endregion

        #region UnloadContent

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        /// 
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        #endregion

        #region Update

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
            
            fBulletDelayTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            player.SpeedChangeCount += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (player.SpeedChangeCount > player.SpeedChangeDelay)
            {
                CheckHorizontalMovementKeys(Keyboard.GetState(),
                                            GamePad.GetState(PlayerIndex.One));
            }

            player.VerticalChangeCount += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (player.VerticalChangeCount > player.VerticalChangeDelay)
            {
                CheckVerticalMovementKeys(Keyboard.GetState(),
                                          GamePad.GetState(PlayerIndex.One));
            }
            player.Update(gameTime);

            fBoardUpdateDelay += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (fBoardUpdateDelay > fBoardUpdateInterval)
            {
                fBoardUpdateDelay = 0f;
                UpdateBoard();
            }

            // TODO: Add your update logic here
            Explosion.Update(gameTime);
            UpdateBullets(gameTime);
            base.Update(gameTime);
        }

        #endregion

        #region Draw

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin();
            background.Draw(spriteBatch);
            player.Draw(spriteBatch);

            // Draw any active player bullets on the screen
            for (int i = 0; i < iMaxBullets; i++)
            {
                // Only draw active bullets
                if (bullets[i].IsActive)
                {
                    bullets[i].Draw(spriteBatch);
                }
            }

            Explosion.Draw(spriteBatch, 0, 0, false);
            spriteBatch.End();

            base.Draw(gameTime);
        }
        
        #endregion

        #region Methods

        //
        // Horizontal Movement
        protected void CheckHorizontalMovementKeys(KeyboardState ksKeys,
                                           GamePadState gsPad)
        {
            bool bResetTimer = false;

            player.Thrusting = false;
            if ((ksKeys.IsKeyDown(Keys.Right)) || (gsPad.ThumbSticks.Left.X > 0))
            {
                if (player.ScrollRate < iMaxHorizontalSpeed)
                {
                    player.ScrollRate += player.AccelerationRate;
                    if (player.ScrollRate > iMaxHorizontalSpeed)
                        player.ScrollRate = iMaxHorizontalSpeed;
                    bResetTimer = true;
                }
                player.Thrusting = true;
                player.Facing = 0;
            }

            if ((ksKeys.IsKeyDown(Keys.Left)) || (gsPad.ThumbSticks.Left.X < 0))
            {
                if (player.ScrollRate > -iMaxHorizontalSpeed)
                {
                    player.ScrollRate -= player.AccelerationRate;
                    if (player.ScrollRate < -iMaxHorizontalSpeed)
                        player.ScrollRate = -iMaxHorizontalSpeed;
                    bResetTimer = true;
                }
                player.Thrusting = true;
                player.Facing = 1;
            }

            if (bResetTimer)
                player.SpeedChangeCount = 0.0f;
        }
        //
        // VerticalMovement
        protected void CheckVerticalMovementKeys(KeyboardState ksKeys, GamePadState gsPad)
        {

            bool bResetTimer = false;

            if ((ksKeys.IsKeyDown(Keys.Up)) || (gsPad.ThumbSticks.Left.Y > 0))
            {
                if (player.Y > iPlayAreaTop)
                {
                    player.Y -= player.VerticalMovementRate;
                    bResetTimer = true;
                }
            }

            if ((ksKeys.IsKeyDown(Keys.Down)) || (gsPad.ThumbSticks.Left.Y < 0))
            {
                if (player.Y < iPlayAreaBottom)
                {
                    player.Y += player.VerticalMovementRate;
                    bResetTimer = true;
                }
            }

            // TODO Add a timeframe for the button being held down, for every 5 or 10 seconds drop a bar on a meter that shows how much more the engines can take before overheating.
            // if the engines overheat then the ship drops out of warp (cannot use a) for some specified amount of time.
            // This feature could also have the ship stranded in space until repairs can be made; again determined by some specified amount of time.

            // Ion-Engines; Uses A on the keyboard & X on the 
            if ((ksKeys.IsKeyDown(Keys.A)) || (gsPad.Buttons.X == ButtonState.Pressed))
            {
                speedMultiplier = 3;
            }
            else { speedMultiplier = 2; }

            if (bResetTimer)
                player.VerticalChangeCount = 0f;
            CheckOtherKeys(Keyboard.GetState(), GamePad.GetState(PlayerIndex.One));
        }      

        //
        // Game board Update
        public void UpdateBoard()
        {
            background.BackgroundOffset += player.ScrollRate;
            background.ParallaxOffset += player.ScrollRate * speedMultiplier;
        }
        //
        // Helper Function for firing bullets (when the player presses the fire button)
        protected void FireBullet(int iVerticalOffset)
        {
            // Find and fire a free bullet
            for (int x = 0; x < iMaxBullets; x++)
            {
                if (!bullets[x].IsActive)
                {
                    bullets[x].Fire(player.X + iBulletFacingOffsets[player.Facing],
                                             player.Y + iBulletVerticalOffset + iVerticalOffset,
                                             player.Facing);
                    break;

                }
            }
        }
        protected void CheckOtherKeys(KeyboardState ksKeys, GamePadState gsPad)
        {

            // Space Bar or Game Pad A button fire the 
            // player's weapon.  The weapon has it's
            // own regulating delay (fBulletDelayTimer) 
            // to pace the firing of the player's weapon.
            if ((ksKeys.IsKeyDown(Keys.Space)) || (gsPad.Buttons.A == ButtonState.Pressed))
            {
                if (fBulletDelayTimer >= fFireDelay)
                {
                    FireBullet(0);
                    fBulletDelayTimer = 0.0f;
                }
            }
        }

        protected void UpdateBullets(GameTime gameTime)
        {
            // Updates the location of all the active player bullets.
            for (int x = 0; x < iMaxBullets; x++)
            {
                if (bullets[x].IsActive)
                    bullets[x].Update(gameTime);
            }
        }

        #endregion

    }
}
