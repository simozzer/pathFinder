using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using System.IO;
using System.Collections.Generic;

using SiFirstMonoGame.Tilemapper;
using SiFirstMonoGame.Solver;

namespace SiFirstMonoGame
{
    enum GameStates
    {
        Dragging,
        Idle
    };

    enum PlayerStates
    {
        Vulnerable,
        Attacking,
        Dying
    }

    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        
        private SpriteBatch _spriteBatch;
        private Texture2D wall;
        private LoopedAnimation playerAnim;
        private Texture2D toolSquare;
        private Texture2D gridSquare, boundarySquare, pill;
        SpriteFont font, smallFont;
        private SoundEffect consumeBlip;
        private List<Texture2D> ghostTextures;
        private Texture2D ghostEyes;
        private Texture2D ghostPupils;
        List<Texture2D> particles;
        Texture2D redSquare;

        private GameStates state;
        float viewportLeft;
        float viewportTop;
        int viewportWidth;
        int viewportHeight;                
        Vector2 viewportCenter;
        private float VIEWPORT_MOVE_AMOUNT = 4.0f;
        private ButtonState oldLeftButtonState = ButtonState.Released;
        private int lastCellX, lastCellY;
        private TileBitmap tileLayer0;
        private TileBitmap pillLayer;
        private int maxCols = 50;
        private int maxRows = 50;

        private Keys saveKey = Keys.F2;
        private Keys loadKey = Keys.F1;
        private Keys leftKey = Keys.A;
        private Keys rightKey = Keys.D;
        private Keys upKey = Keys.W;
        private Keys downKey = Keys.S;

        private int tileSize = 32;
        private int playerSize = 28;
        private float playerLeft;// = 100.0f;
        private float playerTop;// = 100.0f;

        private float playerVelX = 0.0f;
        private float playerVelY = 0.0f;
        private float playerAccel = 2.0f;
        private float playerMaxVel;
        private int scrollRightInitiaite;
        private int scrollLeftInitiaite;
        private int scrollUpInitiaite;
        private int scrollDownInitiaite;

        Vector2 offSetToPlayer;
        private ParticleEngine engine;
        private MusicLooper looper;
        private int pillsRemaining;
        private Color backgroundColor;
        private int pillFlashCount;
        private int fillFlashTicks;

        private List<Vector2> ghostPositions;
        private float pillPitch;
        private float pillPitchDelta;

        private PlayerStates playerState;

        private List<Ghost> ghosts;

        private float deathAngle;

        private string filePath = "D:\\game.bin"; //"D:\\Smallgame.bin"; 
        //private MazeSolver solver;


        Dijktra solver;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            backgroundColor = Color.Black;
            pillFlashCount = 0;
            fillFlashTicks = 4;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
            viewportLeft = 0.0f;
            viewportTop = 0.0f;
            tileLayer0 = new TileBitmap(maxCols, maxRows);
            pillLayer = new TileBitmap(maxCols, maxRows);
            try
            {
                LoadMaze();
            } catch
            {

            }
            state = GameStates.Idle;
            lastCellX = -1;
            lastCellY = -1;
            playerMaxVel = VIEWPORT_MOVE_AMOUNT * 0.80f;
            tileSize = 32;
            this.ghosts = new List<Ghost>();
            for (int i=0; i < 25; i++)
            {
                Texture2D ghostBitmap = this.ghostTextures[i % ghostTextures.Count];
                ghosts.Add(new Ghost(tileLayer0, tileSize, 32, ghostBitmap ));
            }
            playerState = PlayerStates.Vulnerable;
            solver = null;
        }


        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            wall = Content.Load<Texture2D>("Wall");
            font = Content.Load<SpriteFont>("BasicFont");
            smallFont = Content.Load<SpriteFont>("SmallFont");
            toolSquare = Content.Load<Texture2D>("ToolSquare");
            gridSquare = Content.Load<Texture2D>("GridSquare");
            boundarySquare = Content.Load<Texture2D>("BoundarySquare");
            redSquare = Content.Load<Texture2D>("RedSquare");
            pill = Content.Load<Texture2D>("A16Pill");
            List<Texture2D> playerAnimImages = new List<Texture2D>();
            playerAnimImages.Add(Content.Load<Texture2D>("PillBoy/PillBoy0"));
            playerAnimImages.Add(Content.Load<Texture2D>("PillBoy/PillBoy1"));
            playerAnimImages.Add(Content.Load<Texture2D>("PillBoy/PillBoy2"));
            playerAnimImages.Add(Content.Load<Texture2D>("PillBoy/PillBoy3"));
            playerAnim = new UpDownLoopedAnimation(playerAnimImages, playerSize,0.3f);

            viewportWidth = GraphicsDevice.Viewport.Width;
            viewportHeight = GraphicsDevice.Viewport.Height;
            viewportCenter = new Vector2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);
            offSetToPlayer = new Vector2(viewportCenter.X - (playerSize / 2), viewportCenter.Y - (playerSize / 2));
            playerLeft = 80;// offSetToPlayer.X;
            playerTop = 80;//  offSetToPlayer.Y;
            scrollRightInitiaite = (int)(0.7f * GraphicsDevice.Viewport.Width);
            scrollLeftInitiaite = (int)(0.3f * GraphicsDevice.Viewport.Width);
            scrollUpInitiaite = (int)(0.3f * GraphicsDevice.Viewport.Height);
            scrollDownInitiaite = (int)(0.7f * GraphicsDevice.Viewport.Height);
            particles = new List<Texture2D>();
            particles.Add(Content.Load<Texture2D>("Particles/A16Blur"));
            particles.Add(Content.Load<Texture2D>("Particles/A16SQ"));
            particles.Add(Content.Load<Texture2D>("Particles/A16Trir"));

            ghostTextures = new List<Texture2D>();
            ghostTextures.Add(Content.Load<Texture2D>("Ghost"));
            ghostTextures.Add(Content.Load<Texture2D>("Ghost0"));
            ghostTextures.Add(Content.Load<Texture2D>("Ghost1"));
            ghostTextures.Add(Content.Load<Texture2D>("Ghost2"));
            ghostEyes = Content.Load<Texture2D>("GhostEyeHoles");
            ghostPupils = Content.Load<Texture2D>("GhostPupil");

            looper = new MusicLooper();
            List<SoundEffect> loops = new List<SoundEffect>();
            loops.Add(Content.Load<SoundEffect>("LoopsAt128BPM/Kick1"));
            loops.Add(Content.Load<SoundEffect>("LoopsAt128BPM/KickSnare"));
            loops.Add(Content.Load<SoundEffect>("LoopsAt128BPM/KickSnareHat"));
            //loops.Add(Content.Load<SoundEffect>("LoopsAt128BPM/KickSnareHats"));
            loops.Add(Content.Load<SoundEffect>("LoopsAt128BPM/KickSnareHatsChord"));

            looper.InitialLoops = loops;

            consumeBlip = Content.Load<SoundEffect>("Effects/blip");
            ghostPositions = new List<Vector2>();

            ghostPositions.Add(new Vector2(tileSize + (tileSize-ghostTextures[0].Width) /2));
            engine = new ParticleEngine(particles);            
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            ProcessManuallGameAreaScrolling();

            ProcessMouseClicks(gameTime);

            ProcessLoadAndSaveKeys();

            ProcessPlayerMovement();

            engine.Update();
            looper.Update(gameTime);

            playerAnim.UpdateImage();

            if (pillFlashCount > 0)
            {
                pillFlashCount--;
                float brightness = 1.0f - ((float)(fillFlashTicks - pillFlashCount) / (fillFlashTicks * 3));
                backgroundColor = new Color(0.0f, 0.0f, brightness);
            }
            else
            {
                backgroundColor = Color.Black;
            }
            Rectangle playerRect = new Rectangle((int)playerLeft, (int)playerTop, playerSize, playerSize);
            foreach (Ghost g in ghosts)
            {
                g.Update(gameTime);
                if (g.Rect.Intersects(playerRect))
                {
                    processGhostHit(g);
                }

                
            }

            switch (playerState)
            {
                case PlayerStates.Vulnerable:
                   
                    break;

                case PlayerStates.Attacking:

                    break;

                case PlayerStates.Dying:
                    deathAngle += 0.1f;
                    if (deathAngle >= 88.0/7.0)
                    {
                        deathAngle = 0.0f;
                        playerState = PlayerStates.Vulnerable;
                    }
                    break;
            }
            base.Update(gameTime);
        }

        void processGhostHit(Ghost g)
        {
            switch(playerState)
            {
                case PlayerStates.Vulnerable:
                    playerState = PlayerStates.Dying;
                    deathAngle = 0.0f;
                    break;

                case PlayerStates.Attacking:

                    break;

                case PlayerStates.Dying:

                    break;
            }
        }

        long lastTime;
        private void ProcessMouseClicks(GameTime gameTime)
        {
            MouseState mouseState = Mouse.GetState();
            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                int mouseX = mouseState.X;
                int mouseY = mouseState.Y;


                int gridX = pixelToTilemapCellX(mouseX);
                System.Diagnostics.Debug.WriteLine($"CellX: {gridX}");
                int gridY = pixelToTilemapCellY(mouseY);
                bool isSet = tileLayer0.getIsSet(gridX, gridY);


                // graphBuilder = new GraphBuilder(tileLayer0);

                /*
                if (gameTime.TotalGameTime.Ticks - lastTime > 50000)
                {
                    lastTime = gameTime.TotalGameTime.Ticks;
                    solver = new Dijktra(tileLayer0, new Point(1, 1), new Point(gridX, gridY));
                    
                }
                */
                foreach(Ghost g in ghosts)
                {
                    g.SendTo(new Point(gridX, gridY), tileLayer0, tileSize);
                }
                return;

                if (mouseState.LeftButton != oldLeftButtonState)
                {
                    oldLeftButtonState = mouseState.LeftButton;
                    tileLayer0.setIsSet(gridX, gridY, !isSet);
                    state = GameStates.Dragging;
                }
                else if ((state == GameStates.Dragging) && ((lastCellX != gridX) || (lastCellY != gridY)))
                {
                    tileLayer0.setIsSet(gridX, gridY, !isSet);
                }
                lastCellX = gridX;
                lastCellY = gridY;
            }
            else if (mouseState.LeftButton == ButtonState.Released)
            {
                oldLeftButtonState = ButtonState.Released;
                state = GameStates.Idle;
                lastCellX = -1;
                lastCellY = -1;

            }
        }

        private void ProcessManuallGameAreaScrolling()
        {
            KeyboardState state = Keyboard.GetState();
            if (state.IsKeyDown(Keys.Right))
            {
                viewportLeft += VIEWPORT_MOVE_AMOUNT;
            }
            else if (state.IsKeyDown(Keys.Left))
            {
                viewportLeft -= VIEWPORT_MOVE_AMOUNT;
            }

            if (state.IsKeyDown(Keys.Up))
            {
                viewportTop -= VIEWPORT_MOVE_AMOUNT;               
            }
            else if (state.IsKeyDown(Keys.Down))
            {
                viewportTop += VIEWPORT_MOVE_AMOUNT;
            }
        }

        private void ProcessPlayerMovement()
        {
            KeyboardState state = Keyboard.GetState();
            ProcessMovementX(state);
            ProcessMovementY(state);

            // check for pills
            List<Point> collissions = TileMapCollision.getCollisions(pillLayer, tileSize, new Point((int)(playerLeft), (int)(playerTop)), playerSize);
            if (collissions.Count >0)
            {
                Rectangle playerRect = new Rectangle((int)playerLeft, (int)playerTop, playerSize, playerSize);
                for (int i=0; i< collissions.Count; i++)
                {
                    Point collissionCellIndexes = collissions[i];
                    Point tileCenter = new Point(collissionCellIndexes.X * tileSize + (tileSize / 2),
                        collissionCellIndexes.Y * tileSize + (tileSize /2));
                    Rectangle pillRect = new Rectangle(tileCenter.X - (pill.Bounds.Width /2 ),
                        tileCenter.Y - (pill.Bounds.Height /2),
                        pill.Bounds.Width,
                        pill.Bounds.Height);
                    if (pillRect.Intersects(playerRect))
                    {
                        consumePill(collissionCellIndexes);
                    }
                    
                }
            }
            

        }


        private void consumePill(Point cellIndex)
        {
            pillLayer.setIsSet(cellIndex.X, cellIndex.Y, false);
            Point cellCenter = new Point((cellIndex.X * tileSize) + (tileSize / 2) - (int)viewportLeft, (cellIndex.Y * tileSize) + (tileSize / 2) - (int)viewportTop);
            engine.AddParticles(cellCenter);
            pillsRemaining--;
            pillFlashCount = fillFlashTicks;
            consumeBlip.Play(0.02f, pillPitch, 0.0f);
            pillPitch += pillPitchDelta;
        }

        private void ProcessMovementX(KeyboardState state)
        {
            bool leftOrRightPressed = false;
            if (state.IsKeyDown(rightKey))
            {
                if (playerVelX < (playerMaxVel - playerAccel))
                {
                    playerVelX += playerAccel;
                }
                else
                {
                    playerVelX = playerMaxVel;
                }
                leftOrRightPressed = true;

            }
            else if (state.IsKeyDown(leftKey))
            {
                if (playerVelX > -(playerMaxVel - playerAccel))
                {
                    playerVelX -= playerAccel;
                }
                else
                {
                    playerVelX = -playerMaxVel;
                }
                leftOrRightPressed = true;
            };

            if (!leftOrRightPressed)
            {
                if (playerVelX > 0)
                {
                    playerVelX -= playerAccel;
                    if (playerVelX < 0)
                    {
                        playerVelX = 0;
                    }

                }
                else if (playerVelX < 0)
                {
                    if (-playerVelX > playerAccel)
                    {
                        playerVelX += playerAccel;
                    }
                    else
                    {
                        playerVelX = 0;
                    }
                }
            }
            float newPlayerLeft = playerLeft + playerVelX;
            int cellX = pixelToTilemapCellX((int)(newPlayerLeft - viewportLeft));
            int cellX2 = pixelToTilemapCellX((int)(newPlayerLeft + playerSize - viewportLeft));
            int cellY = pixelToTilemapCellY((int)(playerTop - viewportTop));
            int cellY2 = pixelToTilemapCellY((int)(playerTop - viewportTop + playerSize ));

            if (newPlayerLeft < playerLeft)
            {
                // moving left;
                if (tileLayer0.getIsSet(cellX, cellY))
                {
                    playerLeft = tileMapCellToPixelLeft(cellX) + tileSize+1;
                    playerVelX = 0;
                }
                else if ((cellY2 != cellY) && (tileLayer0.getIsSet(cellX,cellY2)))
                {
                    playerLeft = tileMapCellToPixelLeft(cellX) + tileSize+1;
                    playerVelX = 0;
                }
                else
                {
                    float delta = playerLeft - newPlayerLeft;
                    playerLeft = newPlayerLeft;
                    if (playerLeft - viewportLeft < scrollLeftInitiaite)
                    {
                        viewportLeft -= delta;
                    }                    
                }
            }
            else if (newPlayerLeft > playerLeft)
            {
                if (tileLayer0.getIsSet(cellX + 1, cellY))
                {
                    playerLeft = tileMapCellToPixelLeft(cellX + 1) - (playerSize + 1);
                    playerVelX = 0;
                }
                else if ((cellY2 != cellY) && (tileLayer0.getIsSet(cellX+1,cellY2))) 
                {
                    playerLeft = tileMapCellToPixelLeft(cellX + 1) - (playerSize + 1);
                    playerVelX = 0;
                }
                else
                {
                    float delta = newPlayerLeft - playerLeft;
                    playerLeft = newPlayerLeft;
                    if (playerLeft -viewportLeft > scrollRightInitiaite)
                    {
                        viewportLeft += delta;
                    }
                }
            }
        }

        private void ProcessMovementY(KeyboardState state)
        {
            bool topOrDownPressed = false;
            if (state.IsKeyDown(downKey))
            {
                if (playerVelY < (playerMaxVel - playerAccel))
                {
                    playerVelY += playerAccel;
                }
                else
                {
                    playerVelY = playerMaxVel;
                }
                topOrDownPressed = true;

            }
            else if (state.IsKeyDown(upKey))
            {
                if (playerVelY> -(playerMaxVel - playerAccel))
                {
                    playerVelY -= playerAccel;
                }
                else
                {
                    playerVelY = -playerMaxVel;
                }
                topOrDownPressed = true;
            };

            if (!topOrDownPressed)
            {
                if (playerVelY > 0)
                {
                    playerVelY -= playerAccel;
                    if (playerVelY < 0)
                    {
                        playerVelY = 0;
                    }

                }
                else if (playerVelY < 0)
                {
                    if (-playerVelY > playerAccel)
                    {
                        playerVelY += playerAccel;
                    }
                    else
                    {
                        playerVelY = 0;
                    }
                }
            }
            float newPlayerTop = playerTop + playerVelY;
            int cellX = pixelToTilemapCellX((int)(playerLeft - viewportLeft));
            int cellX2 = pixelToTilemapCellX((int)(playerLeft - viewportLeft + playerSize));

            int cellY = pixelToTilemapCellY((int)(newPlayerTop - viewportTop));
            int cellY2 = pixelToTilemapCellY((int)(newPlayerTop - viewportTop + playerSize));

            if (newPlayerTop < playerTop)
            {
                // moving up
                if (tileLayer0.getIsSet(cellX, cellY))
                {
                    playerTop = tileMapCellToPixelTop(cellY) + tileSize + 1;
                    playerVelY = 0;
                }
                else if ((cellX2 != cellX) && (tileLayer0.getIsSet(cellX2,cellY)))
                {
                    playerTop = tileMapCellToPixelTop(cellY) + tileSize + 1;
                    playerVelY = 0;
                }
                else
                {
                    float delta = playerTop - newPlayerTop;
                    playerTop = newPlayerTop;
                    if (playerTop - viewportTop < scrollUpInitiaite)
                    {
                        viewportTop -= delta;
                    }
                }
            }
            else if (newPlayerTop > playerTop)
            {
                if (tileLayer0.getIsSet(cellX, cellY + 1))
                {
                    playerTop = tileMapCellToPixelTop(cellY + 1) - (playerSize + 1);
                    playerVelY = 0;
                }
                else if ((cellX2 != cellX) && (tileLayer0.getIsSet(cellX2, cellY + 1)))
                {
                    playerTop = tileMapCellToPixelTop(cellY + 1) - (playerSize + 1);
                    playerVelY = 0;
                }
                else
                {
                    
                    float delta = newPlayerTop - playerTop;
                    playerTop = newPlayerTop;
                    if (playerTop - viewportTop > scrollDownInitiaite)
                    {
                        viewportTop += delta;
                    }
                }
            }
        }

        private int pixelToTilemapCellX(float pixel)
        {   if (pixel + viewportLeft >= 0)
            {
                return (int)((pixel + viewportLeft) / (float)tileSize);
            } else
            {
                return -1;
            }

        }

        private int pixelToTilemapCellY(float pixel)
        {
            return (int)((pixel + viewportTop) / tileSize);
        }

        private int tileMapCellToPixelTop(int cellY)
        {
            return cellY * tileSize;
        }

        private int tileMapCellToPixelLeft(int cellX)
        {            
            return cellX * tileSize;
        }

        private void ProcessLoadAndSaveKeys()
        {
            KeyboardState state = Keyboard.GetState();
            if (state.IsKeyDown(saveKey))
            {
                FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate);
                try
                {
                    tileLayer0.writeToStream(fs);
                }
                finally
                {
                    fs.Close();
                }
            }

            if (state.IsKeyDown(loadKey))
            {
                LoadMaze();
            }
        }

        private void LoadMaze()
        {
            
            /*
            tileLayer0 = new TileBitmap(maxCols, maxRows);
            pillLayer = new TileBitmap(tileLayer0.colCount, tileLayer0.rowCount);
            for (int y = 0; y < tileLayer0.rowCount; y++)
            {
                for (int x = 0; x < tileLayer0.colCount; x++)
                {
                    pillLayer.setIsSet(x, y, !tileLayer0.getIsSet(x, y));

                }
            }
            pillsRemaining = pillLayer.numberSet();
            pillPitch = -1.0f;
            pillPitchDelta = 2.0f / (float)pillsRemaining;
            
            return;
            */

            if (File.Exists(filePath))
            {
                FileStream fs = new FileStream(filePath, FileMode.Open);
                try
                {
                    tileLayer0 = new TileBitmap(fs);
                    pillLayer = new TileBitmap(tileLayer0.colCount, tileLayer0.rowCount);
                    for (int y = 0; y < tileLayer0.rowCount; y++)
                    {
                        for (int x = 0; x < tileLayer0.colCount; x++)
                        {
                            pillLayer.setIsSet(x, y, !tileLayer0.getIsSet(x, y));

                        }
                    }
                    pillsRemaining = pillLayer.numberSet();
                    pillPitch = -1.0f;
                    pillPitchDelta = 2.0f / (float)pillsRemaining;

                }
                finally
                {
                    fs.Close();
                }
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            
            SpriteBatch b = new SpriteBatch(GraphicsDevice);
            b.Begin();

            TilemapRenderer r = new TilemapRenderer(GraphicsDevice, tileLayer0, new Vector2(viewportLeft, viewportTop), tileSize, wall, gridSquare);
            r.render(b, false, backgroundColor);
            
            TilemapRenderer pillRenderer = new TilemapRenderer(GraphicsDevice, pillLayer, new Vector2(viewportLeft, viewportTop), tileSize, pill, null);            
            pillRenderer.render(b, true, Color.Black);

            if (playerState == PlayerStates.Dying)
            {
                playerAnim.Draw(b, new Point((int)(playerLeft - viewportLeft), (int)(playerTop - viewportTop)), Color.White, new Vector2(playerVelX, playerVelY), deathAngle);
            }
            else
            {
                playerAnim.Draw(b, new Point((int)(playerLeft - viewportLeft), (int)(playerTop - viewportTop)), Color.White, new Vector2(playerVelX, playerVelY), 0.0f);
            }

            foreach(Ghost g in ghosts)
            { 
                g.Draw(b, new Rectangle((int)viewportLeft, (int)viewportTop, viewportWidth, viewportHeight), ghostEyes, ghostPupils, new Point((int)playerLeft,(int)playerTop));
            }

            
            if ((solver != null) && (solver.Path != null))
            {

                r = new TilemapRenderer(GraphicsDevice, solver.Path, new Vector2(viewportLeft, viewportTop), tileSize, redSquare, null);
                r.render(b, false, backgroundColor);
                
            }
            


            /*
            if (graphBuilder != null)
            {
                r = new TilemapRenderer(GraphicsDevice, graphBuilder.MazeNodes, new Vector2(viewportLeft, viewportTop), tileSize, redSquare, null);
                r.render(b, false, backgroundColor);
            }
            */

            engine.Draw(b);

            b.DrawString(smallFont, $"Pills To Go: {pillsRemaining}", new Vector2(0, 0), Color.White);

            b.End();
            base.Draw(gameTime);
        }
    }
}
