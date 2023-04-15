using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using SiFirstMonoGame.Tilemapper;
using SiFirstMonoGame.Solver;
  
namespace SiFirstMonoGame
{

    enum Directions  {
        Up, 
        Down,
        Left, 
        Right 
    }

    enum GhostState
    {
        RandomMovement,
        MappedMovement
    }

    class Ghost
    {
        Point origin;
        TileBitmap mazeMap;
        int tileSize;
        int ghostSize;
        Vector2 mazePositionPx;
        Directions direction;
        Texture2D bitmap;
        float velocity = 2.0f;
        Random rnd;
        GhostState state = GhostState.RandomMovement;
        internal Ghost(Point origin, TileBitmap maze, int tileSize, int ghostSize, Texture2D bitmap)
        {
            this.origin = origin;
            this.mazeMap = maze;
            this.tileSize = tileSize;
            this.ghostSize = ghostSize;

            this.mazePositionPx = new Vector2((origin.X * tileSize) + ((tileSize - ghostSize) / 2),
                (origin.Y * tileSize) + ((tileSize - ghostSize) / 2));
            this.direction = Directions.Right;
            rnd = new Random();
            this.bitmap = bitmap;
        }


        internal Vector2 GridPositionToMazePositionPx(Point origin, int tileSize, int ghostSize)
        {
            return new Vector2((origin.X * tileSize) + ((tileSize - ghostSize) / 2),
                  (origin.Y * tileSize) + ((tileSize - ghostSize) / 2));

        }

        internal Ghost(TileBitmap maze, int tileSize, int ghostSize, Texture2D bitmap)
        {
            rnd = new Random();
            bool isSet = false;
            int x = 0;
            int y = 0;
            while (!isSet)
            {
                x = rnd.Next(maze.colCount - 2) + 1;
                y = rnd.Next(maze.rowCount - 2) + 1;
                isSet = maze.getIsSet(x-1, y-1);
            }
            this.origin = new Point(x, y);
            this.mazeMap = maze;
            this.tileSize = tileSize;
            this.ghostSize = ghostSize;
            this.bitmap = bitmap;

            this.mazePositionPx = GridPositionToMazePositionPx(origin, tileSize, ghostSize);
            this.direction = Directions.Right;
        }



        internal Rectangle Rect
        {
            get
            {
                return new Rectangle(new Point((int)mazePositionPx.X, (int)mazePositionPx.Y), new Point(ghostSize));
            }
        }

        // the cell index in which to start (not the pixel coordinates) 
        internal Point Origin
        {
            get
            {
                return this.origin;
            }
        }
        internal bool canMoveLeft()
        {
            float newLeft = mazePositionPx.X - velocity;
            int cellX = (int)(newLeft / tileSize);
            int cellY = (int)(mazePositionPx.Y / tileSize);
            if (mazeMap.getIsSet(cellX, cellY))
            {
                return false;
            }
            int cellY2 = (int)((mazePositionPx.Y + (ghostSize - 1)) / tileSize);
            if ((cellY != cellY2) && (mazeMap.getIsSet(cellX, cellY2)))
            {
                return false;
            }
            return true;
        }

        internal bool canMoveRight()
        {
            float newRight = mazePositionPx.X + ghostSize - 1 + velocity;
            int cellX = (int)(newRight / tileSize);
            int cellY = (int)(mazePositionPx.Y / tileSize);
            if (mazeMap.getIsSet(cellX, cellY))
            {
                return false;
            }
            int cellY2 = (int)((mazePositionPx.Y + ghostSize - 1) / tileSize);
            if ((cellY != cellY2) && (mazeMap.getIsSet(cellX, cellY2)))
            {
                return false;
            }
            return true;
        }

        internal bool canMoveUp()
        {
            float newTop = mazePositionPx.Y - velocity;
            int cellY = (int)(newTop / tileSize);
            int cellX = (int)(mazePositionPx.X / tileSize);
            if (mazeMap.getIsSet(cellX, cellY))
            {
                return false;
            }
            int cellX2 = (int)((mazePositionPx.X + (ghostSize - 1)) / tileSize);
            if ((cellX != cellX2) && (mazeMap.getIsSet(cellX2, cellY)))
            {
                return false;
            }
            return true;
        }

        internal bool canMoveDown()
        {
            float newBottom = mazePositionPx.Y + ghostSize - 1 + velocity;
            int cellY = (int)(newBottom / tileSize);
            int cellX = (int)(mazePositionPx.X / tileSize);
            if (mazeMap.getIsSet(cellX, cellY))
            {
                return false;
            }
            int cellX2 = (int)((mazePositionPx.X + ghostSize - 1) / tileSize);
            if ((cellX != cellX2) && (mazeMap.getIsSet(cellX2, cellY)))
            {
                return false;
            }
            return true;
        }

        internal Directions getNewDirection(Directions lastDirection)
        {
            List<Directions> possibleDirections = new List<Directions>();
            if (canMoveUp()) { possibleDirections.Add(Directions.Up); }
            if (canMoveDown()) { possibleDirections.Add(Directions.Down); }
            if (canMoveLeft()) { possibleDirections.Add(Directions.Left); }
            if (canMoveRight()) { possibleDirections.Add(Directions.Right); }
            if (possibleDirections.Count == 1)
            {
                return possibleDirections[0];
            } else if (possibleDirections.Count > 0)
            {
                if (possibleDirections.Contains(lastDirection) && (possibleDirections.Count > 1))
                {
                    possibleDirections.Remove(lastDirection);
                }
                return possibleDirections[rnd.Next(possibleDirections.Count)];
            } else
            {
                return lastDirection;
            }
        }

        internal Directions getNewDirectionExclude(bool excludeVertical)
        {
            List<Directions> possibleDirections = new List<Directions>();
            if (canMoveUp() && !excludeVertical) { possibleDirections.Add(Directions.Up); }
            if (canMoveDown() && !excludeVertical) { possibleDirections.Add(Directions.Down); }
            if (canMoveLeft() && excludeVertical) { possibleDirections.Add(Directions.Left); }
            if (canMoveRight() && excludeVertical) { possibleDirections.Add(Directions.Right); }
            if (possibleDirections.Count == 1)
            {
                return possibleDirections[0];
            }
            else
            {
                return possibleDirections[rnd.Next(possibleDirections.Count)];
            }
        }

        internal void Update(GameTime gametime)
        {
            float newTop = mazePositionPx.Y;
            float newLeft = mazePositionPx.X;
            bool moved = false;
            Directions intendedDirection = direction;

            bool changeDir = false;
            bool isOnGrid = ((int)newTop % tileSize == 0) && ((int)newLeft % tileSize == 0);
            switch (state)
            {
                case GhostState.RandomMovement:
                    while (!moved)
                    {
                        changeDir = (!changeDir) && isOnGrid && (rnd.Next(2) == 1);
                        switch (direction)
                        {
                            case Directions.Up:
                                if (canMoveUp())
                                {
                                    if (changeDir && (canMoveLeft() || canMoveRight()))
                                    {
                                        direction = getNewDirectionExclude(true);
                                    }
                                    else
                                    {
                                        mazePositionPx.Y -= velocity;
                                        moved = true;
                                    }

                                }
                                else
                                {
                                    direction = getNewDirection(Directions.Down);
                                }

                                break;

                            case Directions.Down:
                                if (canMoveDown())
                                {
                                    if (changeDir && (canMoveLeft() || canMoveRight()))
                                    {
                                        direction = getNewDirectionExclude(true);
                                    }
                                    else
                                    {
                                        mazePositionPx.Y += velocity;
                                        moved = true;
                                    }
                                }
                                else
                                {
                                    direction = getNewDirection(Directions.Up);
                                }
                                break;

                            case Directions.Left:
                                if (canMoveLeft())
                                {
                                    if (changeDir && (canMoveUp() || canMoveDown()))
                                    {
                                        direction = getNewDirectionExclude(false);
                                    }
                                    else
                                    {
                                        mazePositionPx.X -= velocity;
                                        moved = true;
                                    }
                                }
                                else
                                {
                                    direction = getNewDirection(Directions.Right);
                                }
                                break;

                            case Directions.Right:
                                if (canMoveRight())
                                {
                                    if (changeDir && (canMoveUp() || canMoveDown()))
                                    {
                                        direction = getNewDirectionExclude(false);
                                    }
                                    else
                                    {
                                        mazePositionPx.X += velocity;
                                        moved = true;
                                    }
                                }
                                else
                                {
                                    direction = getNewDirection(Directions.Left);
                                }
                                break;

                        }

                    }
                    break;

                case GhostState.MappedMovement:


                    Point currentCoords = new Point((int)mazePositionPx.X / tileSize, (int)mazePositionPx.Y / tileSize);
                    if (PathToTarget.Count > 0)
                    {
                        Point destinationCoords = PathToTarget.Peek();
                        Point destinationCoordsPx = new Point(destinationCoords.X * tileSize, destinationCoords.Y * tileSize);
                        if (mazePositionPx.Equals(destinationCoordsPx))
                        {
                            destinationCoords = PathToTarget.Pop();
                            destinationCoordsPx = new Point(destinationCoords.X * tileSize, destinationCoords.Y * tileSize);
                        }

                        if (destinationCoordsPx.X == mazePositionPx.X)
                        {
                            if (destinationCoordsPx.Y < mazePositionPx.Y)
                            {
                                mazePositionPx.Y -= velocity;
                            }
                            else if (destinationCoordsPx.Y > mazePositionPx.Y)
                            {
                                mazePositionPx.Y += velocity;
                            }
                            else
                            {
                                PathToTarget.Pop();
                            }
                        } else if (destinationCoordsPx.Y == mazePositionPx.Y)
                        {
                            if (destinationCoordsPx.X < mazePositionPx.X)
                            {
                                mazePositionPx.X -= velocity;
                            }
                            else if (destinationCoordsPx.X > mazePositionPx.X)
                            {
                                mazePositionPx.X += velocity;
                            }
                            else
                            {
                                PathToTarget.Pop();
                            }
                        } else
                        {
                            PathToTarget.Pop();
                        }

                    } else
                    {
                        state = GhostState.RandomMovement;
                    }
                    break;
            }

        }


        internal Stack<Point> PathToTarget{
            get;
            private set;
           }

        internal void SendTo(Point gridCoordinates, TileBitmap maze, int tileSize)
        {
            Point currentCoords = new Point((int)mazePositionPx.X / tileSize, (int)mazePositionPx.Y / tileSize);
            Dijktra solver = new Dijktra(maze, currentCoords, gridCoordinates);
            PathToTarget = solver.PathToTarget;
            state = GhostState.MappedMovement;
        }

        internal void Draw(SpriteBatch b, Rectangle viewPort, Texture2D eyeHoles, Texture2D pupils, Point playerPosition)

        {
            Rectangle ghostRect = new Rectangle((int)mazePositionPx.X, (int)mazePositionPx.Y, ghostSize, ghostSize);
           
            if (ghostRect.Intersects(viewPort))
            {
                ghostRect.X -= viewPort.Left;
                ghostRect.Y -= viewPort.Top;
                b.Draw(bitmap, ghostRect, Color.White);
                b.Draw(eyeHoles, ghostRect, Color.White);
                if (playerPosition.X < ghostRect.X)
                {
                    ghostRect.X -= 2;
                } else if (playerPosition.X > ghostRect.X)
                {
                    ghostRect.X += 2;
                }

                if (playerPosition.Y < ghostRect.Y)
                {
                    ghostRect.Y -= 2;
                }
                else if (playerPosition.Y > ghostRect.Y)
                {
                    ghostRect.Y += 2;
                }


                b.Draw(pupils, ghostRect, Color.White);
            }
        }


    }
}
