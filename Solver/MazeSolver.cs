using System;
using System.Collections.Generic;
using System.Text;
using SiFirstMonoGame.Tilemapper;
using Microsoft.Xna.Framework;

namespace SiFirstMonoGame
{
    class MazeSolver
    {
        internal TileBitmap maze;
        internal TileBitmap wasHere;
        internal TileBitmap correctPath;
        Point startPoint;
        Point endPoint;

        void initializePath()
        {
            for (int row = 0; row < maze.rowCount; row++)
            {
                for (int col = 0; col < maze.colCount; col++)
                {
                    wasHere.setIsSet(col, row, false);
                    correctPath.setIsSet(col, row, false);
                }
            }

        }
        internal MazeSolver(Tilemapper.TileBitmap maze, Point startPoint, Point endPoint)
        {
            this.maze = maze;
            wasHere = new TileBitmap(maze.colCount, maze.rowCount);
            correctPath = new TileBitmap(maze.colCount, maze.rowCount);
            this.startPoint = startPoint;
            this.endPoint = endPoint;

            initializePath();
            recursiveSolveLeft(startPoint);
            int leftPathLength = correctPath.numberSet();

            initializePath();
            recursiveSolveRight(startPoint);
            int rightPathLength = correctPath.numberSet();

            if (rightPathLength > leftPathLength)
            {

                initializePath();
                findBestRandom(startPoint, 8);

                int randomPathLength = correctPath.numberSet();

                if (randomPathLength > leftPathLength)
                {
                    recursiveSolveLeft(startPoint);
                }
                else
                {
                    // random path was correct.
                }

            }
            else
            {
                initializePath();
                findBestRandom(startPoint, 100);

                int randomPathLength = correctPath.numberSet();

                if (randomPathLength > rightPathLength)
                {
                    recursiveSolveRight(startPoint);
                }
                else
                {
                    // random path was correct.
                }
            }

        }


        internal TileBitmap CorrectPath
        {
            get
            {
                return this.correctPath;
            }
        }

        bool recursiveSolveLeft(Point startPoint)
        {
            if (startPoint.Equals(endPoint)) return true;

            if (maze.getIsSet(startPoint.X, startPoint.Y) || wasHere.getIsSet(startPoint.X, startPoint.Y)) return false;

            wasHere.setIsSet(startPoint.X, startPoint.Y, true);


            if (startPoint.X != 0) // Checks if not on left edge
                if (recursiveSolveLeft(new Point(startPoint.X - 1, startPoint.Y)))
                { // Recalls method one to the left
                    correctPath.setIsSet(startPoint.X, startPoint.Y, true); // Sets that path value to true;
                    return true;
                }
            if (startPoint.X != maze.colCount - 1) // Checks if not on right edge
                if (recursiveSolveLeft(new Point(startPoint.X + 1, startPoint.Y)))
                { // Recalls method one to the right
                    correctPath.setIsSet(startPoint.X, startPoint.Y, true);
                    return true;
                }
            if (startPoint.Y != 0)  // Checks if not on top edge
                if (recursiveSolveLeft(new Point(startPoint.X, startPoint.Y - 1)))
                { // Recalls method one up
                    correctPath.setIsSet(startPoint.X, startPoint.Y, true);
                    return true;
                }
            if (startPoint.Y != maze.rowCount - 1) // Checks if not on bottom edge
                if (recursiveSolveLeft(new Point(startPoint.X, startPoint.Y + 1)))
                { // Recalls method one down
                    correctPath.setIsSet(startPoint.X, startPoint.Y, true);
                    return true;
                }
            return false;
        }

        bool recursiveSolveRight(Point startPoint)
        {
            if (startPoint.Equals(endPoint)) return true;

            if (maze.getIsSet(startPoint.X, startPoint.Y) || wasHere.getIsSet(startPoint.X, startPoint.Y)) return false;

            wasHere.setIsSet(startPoint.X, startPoint.Y, true);


            if (startPoint.Y != maze.rowCount - 1) // Checks if not on bottom edge
                if (recursiveSolveRight(new Point(startPoint.X, startPoint.Y + 1)))
                { // Recalls method one down
                    correctPath.setIsSet(startPoint.X, startPoint.Y, true);
                    return true;
                }

            if (startPoint.Y != 0)  // Checks if not on top edge
                if (recursiveSolveRight(new Point(startPoint.X, startPoint.Y - 1)))
                { // Recalls method one up
                    correctPath.setIsSet(startPoint.X, startPoint.Y, true);
                    return true;
                }


            if (startPoint.X != maze.colCount - 1) // Checks if not on right edge
                if (recursiveSolveRight(new Point(startPoint.X + 1, startPoint.Y)))
                { // Recalls method one to the right
                    correctPath.setIsSet(startPoint.X, startPoint.Y, true);
                    return true;
                }

            if (startPoint.X != 0) // Checks if not on left edge
                if (recursiveSolveRight(new Point(startPoint.X - 1, startPoint.Y)))
                { // Recalls method one to the left
                    correctPath.setIsSet(startPoint.X, startPoint.Y, true); // Sets that path value to true;
                    return true;
                }


            return false;
        }

        void findBestRandom(Point startPoint, int tries)
        {
            List<TileBitmap> solutions = new List<TileBitmap>();
            for (int i=0; i < tries; i++)
            {
                initializePath();
                recursiveSolveRandom(startPoint);
                solutions.Add(new TileBitmap(correctPath));
            }

            solutions.Sort((a, b) => a.numberSet().CompareTo(b.numberSet()));
            correctPath = solutions[0];
        }

        bool recursiveSolveRandom(Point startPoint)
        {
            

            if (startPoint.Equals(endPoint)) return true;

            if (maze.getIsSet(startPoint.X, startPoint.Y) || wasHere.getIsSet(startPoint.X, startPoint.Y)) return false;

            wasHere.setIsSet(startPoint.X, startPoint.Y, true);

            Random rnd = new Random((int)DateTime.Now.Ticks);
            switch (rnd.Next(4))
            {
                case 0:
                    if (startPoint.X != 0) // Checks if not on left edge
                        if (recursiveSolveRandom(new Point(startPoint.X - 1, startPoint.Y)))
                        { // Recalls method one to the left
                            correctPath.setIsSet(startPoint.X, startPoint.Y, true); // Sets that path value to true;
                            return true;
                        }
                    if (startPoint.X != maze.colCount - 1) // Checks if not on right edge
                        if (recursiveSolveRandom(new Point(startPoint.X + 1, startPoint.Y)))
                        { // Recalls method one to the right
                            correctPath.setIsSet(startPoint.X, startPoint.Y, true);
                            return true;
                        }
                    if (startPoint.Y != 0)  // Checks if not on top edge
                        if (recursiveSolveRandom(new Point(startPoint.X, startPoint.Y - 1)))
                        { // Recalls method one up
                            correctPath.setIsSet(startPoint.X, startPoint.Y, true);
                            return true;
                        }
                    if (startPoint.Y != maze.rowCount - 1) // Checks if not on bottom edge
                        if (recursiveSolveRandom(new Point(startPoint.X, startPoint.Y + 1)))
                        { // Recalls method one down
                            correctPath.setIsSet(startPoint.X, startPoint.Y, true);
                            return true;
                        }
                    return false;
                case 1:
                    // swap left right
                    if (startPoint.X != maze.colCount - 1) // Checks if not on right edge
                        if (recursiveSolveRandom(new Point(startPoint.X + 1, startPoint.Y)))
                        { // Recalls method one to the right
                            correctPath.setIsSet(startPoint.X, startPoint.Y, true);
                            return true;
                        }
                    if (startPoint.X != 0) // Checks if not on left edge
                        if (recursiveSolveRandom(new Point(startPoint.X - 1, startPoint.Y)))
                        { // Recalls method one to the left
                            correctPath.setIsSet(startPoint.X, startPoint.Y, true); // Sets that path value to true;
                            return true;
                        }
                    if (startPoint.Y != 0)  // Checks if not on top edge
                        if (recursiveSolveRandom(new Point(startPoint.X, startPoint.Y - 1)))
                        { // Recalls method one up
                            correctPath.setIsSet(startPoint.X, startPoint.Y, true);
                            return true;
                        }
                    if (startPoint.Y != maze.rowCount - 1) // Checks if not on bottom edge
                        if (recursiveSolveRandom(new Point(startPoint.X, startPoint.Y + 1)))
                        { // Recalls method one down
                            correctPath.setIsSet(startPoint.X, startPoint.Y, true);
                            return true;
                        }
                    return false;
                case 2:
                    // updown first
                    if (startPoint.Y != 0)  // Checks if not on top edge
                        if (recursiveSolveRandom(new Point(startPoint.X, startPoint.Y - 1)))
                        { // Recalls method one up
                            correctPath.setIsSet(startPoint.X, startPoint.Y, true);
                            return true;
                        }
                    if (startPoint.Y != maze.rowCount - 1) // Checks if not on bottom edge
                        if (recursiveSolveRandom(new Point(startPoint.X, startPoint.Y + 1)))
                        { // Recalls method one down
                            correctPath.setIsSet(startPoint.X, startPoint.Y, true);
                            return true;                            
                        }
                    if (startPoint.X != 0) // Checks if not on left edge
                        if (recursiveSolveRandom(new Point(startPoint.X - 1, startPoint.Y)))
                        { // Recalls method one to the left
                            correctPath.setIsSet(startPoint.X, startPoint.Y, true); // Sets that path value to true;
                            return true;
                        }
                    if (startPoint.X != maze.colCount - 1) // Checks if not on right edge
                        if (recursiveSolveRandom(new Point(startPoint.X + 1, startPoint.Y)))
                        { // Recalls method one to the right
                            correctPath.setIsSet(startPoint.X, startPoint.Y, true);
                            return true;
                        }
                    return false;
                case 3:
                    // down up first
                    if (startPoint.Y != maze.rowCount - 1) // Checks if not on bottom edge
                        if (recursiveSolveRandom(new Point(startPoint.X, startPoint.Y + 1)))
                        { // Recalls method one down
                            correctPath.setIsSet(startPoint.X, startPoint.Y, true);
                            return true;                            
                        }
                    if (startPoint.Y != 0)  // Checks if not on top edge
                        if (recursiveSolveRandom(new Point(startPoint.X, startPoint.Y - 1)))
                        { // Recalls method one up
                            correctPath.setIsSet(startPoint.X, startPoint.Y, true);
                            return true;
                        }
                    if (startPoint.X != 0) // Checks if not on left edge
                        if (recursiveSolveRandom(new Point(startPoint.X - 1, startPoint.Y)))
                        { // Recalls method one to the left
                            correctPath.setIsSet(startPoint.X, startPoint.Y, true); // Sets that path value to true;
                            return true;
                        }
                    if (startPoint.X != maze.colCount - 1) // Checks if not on right edge
                        if (recursiveSolveRandom(new Point(startPoint.X + 1, startPoint.Y)))
                        { // Recalls method one to the right
                            correctPath.setIsSet(startPoint.X, startPoint.Y, true);
                            return true;
                        }
                    return false;
                default:
                    throw new Exception("ouch!");

            }

        }
    }
}
