using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace SiFirstMonoGame.Solver
{
    class GraphBuilder
    {

        // The original maze tilebitmap.. where 1 is a wall and 0 is path.
        internal Tilemapper.TileBitmap Maze
        {
            get;
            private set;
        }

        // this is an array of where there are nodes in the maze. 1 is a node, 0 is not.
        internal Tilemapper.TileBitmap MazeNodes
        {
            get;
            private set;
        }

        internal GraphBuilder(Tilemapper.TileBitmap maze, Point startPoint, Point endPoint)
        {
            this.Maze = maze;
            this.MazeNodes = new Tilemapper.TileBitmap(Maze);
            buildGraph(startPoint,endPoint);

        }

        internal List<MazeNode> Graph
        {
            get;
            private set;
        }


        void processNode(int col, int row)
        {
            if (Maze.getIsSet(col, row) == false)
            {
                // we're on a bit of the maze which isn't a wall.
                Point pt = new Point(col, row);

                if (canMoveUp(pt) || canMoveDown(pt))
                {
                    // we're on a bit of a maze which allows us to move up or down
                    if (canMoveRight(pt) && !canMoveLeft(pt))
                    {
                        MazeNodes.setIsSet(pt.X, pt.Y, true);
                    }
                    else if (canMoveLeft(pt) && !canMoveRight(pt))
                    {
                        MazeNodes.setIsSet(pt.X, pt.Y, true);
                    }
                    else if (canMoveUp(pt) && !canMoveDown(pt))
                    {
                        MazeNodes.setIsSet(pt.X, pt.Y, true);
                    }
                    else if (canMoveDown(pt) && !canMoveUp(pt))
                    {
                        MazeNodes.setIsSet(pt.X, pt.Y, true);
                    }
                    else if (canMoveLeft(pt) && canMoveRight(pt))
                    {
                        MazeNodes.setIsSet(pt.X, pt.Y, true);
                    }

                }
                else if (canMoveLeft(pt) || canMoveRight(pt))
                {
                    // we're on a bit of the maze which allows us to move left or right
                    if (canMoveUp(pt) && !canMoveDown(pt))
                    {
                        MazeNodes.setIsSet(pt.X, pt.Y, true);
                    }
                    else if (canMoveDown(pt) && !canMoveUp(pt))
                    {
                        MazeNodes.setIsSet(pt.X, pt.Y, true);
                    }
                    else if (canMoveLeft(pt) && !canMoveRight(pt))
                    {
                        MazeNodes.setIsSet(pt.X, pt.Y, true);
                    }
                    else if (canMoveRight(pt) && !canMoveLeft(pt))
                    {
                        MazeNodes.setIsSet(pt.X, pt.Y, true);
                    }
                    else if (canMoveUp(pt) && canMoveUp(pt))
                    {
                        MazeNodes.setIsSet(pt.X, pt.Y, true);
                    }
                }
            }
        }

        void buildGraph(Point startPoint, Point endPoint)
        {
            for (int row = 0; row < Maze.rowCount; row++)
            {
                for (int col = 0; col < Maze.colCount; col++)
                {
                    MazeNodes.setIsSet(col, row, false);                    
                }
            }

            // add the start and end points (in case thee are in positions with no nodes
            MazeNodes.setIsSet(startPoint.X, startPoint.Y,true);
            MazeNodes.setIsSet(endPoint.X, endPoint.Y,true);

            // 1st go through the maze and mark any junctions (nodes)
            for (int row = 0; row < Maze.rowCount; row++)
            {
                for (int col=0; col < Maze.colCount; col++)
                {
                    processNode(col, row);
                }
            }




            // using the Tilebitmap for the nodes  create an of the nodes
            MazeNode[,] allNodes = new MazeNode[Maze.colCount, Maze.rowCount];
            for (int row=0; row < Maze.rowCount; row++)
            {
                for (int col=0; col < Maze.colCount; col++)
                {                    
                    if (MazeNodes.getIsSet(col,row))
                    {
                        MazeNode thisNode = new MazeNode(new Point(col, row));
                        allNodes[col, row] = thisNode;
                    } else
                    {
                        allNodes[col, row] = null;
                    }
                }
            }


            // once we have all the nodes in an array we can make links out of all of them
            Graph = new List<MazeNode>();
            for (int row = 0; row < Maze.rowCount; row++)
            {
                for (int col = 0; col < Maze.colCount; col++)
                {
                    MazeNode thisNode = allNodes[col, row];
                    if (thisNode != null)
                    {
                        int edgeLength;
                        MazeNode edgeEndNode;
                        if (canMoveLeft(thisNode.Position))
                        {
                            edgeLength = distanceToNodeLeft(thisNode.Position);
                            edgeEndNode = allNodes[col - edgeLength, row];
                            thisNode.addLink(edgeEndNode, edgeLength);
                        }

                        if (canMoveRight(thisNode.Position))
                        {
                            edgeLength = distanceToNodeRight(thisNode.Position);
                            edgeEndNode = allNodes[col + edgeLength, row];
                            thisNode.addLink(edgeEndNode, edgeLength);
                        }

                        if (canMoveUp(thisNode.Position))
                        {
                            edgeLength = distanceToNodeUp(thisNode.Position);
                            edgeEndNode = allNodes[col, row- edgeLength];
                            thisNode.addLink(edgeEndNode, edgeLength);
                        }


                        if (canMoveDown(thisNode.Position))
                        {
                            edgeLength = distanceToNodeDown(thisNode.Position);
                            edgeEndNode = allNodes[col, row + edgeLength];
                            thisNode.addLink(edgeEndNode, edgeLength);
                        }

                        Graph.Add(thisNode);

                    }
                }
            }
        }

        internal MazeNode getNodeForPosition(Point pt)
        {
            // list will be sorted as added in order within loop (row...,col...)
            return Graph.Find(x => x.Position.Equals(pt));
        }

        bool canMoveUp(Point pt)
        {
            return !Maze.getIsSet(pt.X, pt.Y - 1);
        }

        bool canMoveDown(Point pt)
        {
            return !Maze.getIsSet(pt.X, pt.Y + 1);
        }

        bool canMoveLeft(Point pt)
        {
            return !Maze.getIsSet(pt.X-1, pt.Y);
        }

        bool canMoveRight(Point pt)
        {
            return !Maze.getIsSet(pt.X+1, pt.Y);
        }

        int distanceToNodeRight(Point pt)
        {
            int len = 0;
            Point p = new Point(pt.X, pt.Y);
            while (canMoveRight(p))
            {
                len++;
                p.X++;
                if (MazeNodes.getIsSet(p.X,p.Y))
                {
                    return len;
                }
            }
            return len;
        }

        int distanceToNodeLeft(Point pt)
        {
            int len = 0;
            Point p = new Point(pt.X, pt.Y);
            while (canMoveLeft(p))
            {
                len++;
                p.X--;
                if (MazeNodes.getIsSet(p.X, p.Y))
                {
                    return len;
                }
            }
            return len;
        }


        int distanceToNodeDown(Point pt)
        {
            int len = 0;
            Point p = new Point(pt.X, pt.Y);
            while (canMoveDown(p))
            {
                len++;
                p.Y++;
                if (MazeNodes.getIsSet(p.X, p.Y))
                {
                    return len;
                }
            }
            return len;
        }


        int distanceToNodeUp(Point pt)
        {
            int len = 0;
            Point p = new Point(pt.X, pt.Y);
            while (canMoveUp(p))
            {
                len++;
                p.Y--;
                if (MazeNodes.getIsSet(p.X, p.Y))
                {
                    return len;
                }
            }
            return len;
        }


        internal byte linkCount(Point pt)
        {
            byte count = 0;
            if (canMoveUp(pt)) count++;
            if (canMoveDown(pt)) count++;
            if (canMoveRight(pt)) count++;
            if (canMoveLeft(pt)) count++;
            return count;
        }
    }
}
