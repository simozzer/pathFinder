using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace SiFirstMonoGame.Solver
{
    class Dijktra
    {
        
        GraphBuilder GraphBuidler
        {
            get;
            set;
        }

        internal List<MazeNode> Graph
        {
            get;
            private set;
        }

        internal Tilemapper.TileBitmap Path
        {
            get;
            private set;
        }

        internal Stack<Point> PathToTarget
        {
            get;
            private set;
        }


        internal MazeNode getLowestCostLinkedNode(MazeNode nodeUnderConsideration)
        {
            // update distance of adjacent nodes (vertices)
            MazeNode lowestNode = null;
            foreach (MazeLink link in nodeUnderConsideration.Links)
            {
                MazeNode targetNode = link.TargetNode;
                if (!targetNode.Visited)
                {
                    int targetCost = nodeUnderConsideration.Cost + link.LinkLength;
                    if (targetCost < targetNode.Cost)
                    {
                        targetNode.Cost = targetCost;
                    }
                    if ((lowestNode == null) || (targetCost < lowestNode.Cost))
                    {
                        lowestNode = targetNode;
                    }
                }
            }
            return lowestNode;
        }

        internal Dijktra(SiFirstMonoGame.Tilemapper.TileBitmap maze, Point startPoint, Point endPoint)
        {
            GraphBuidler = new GraphBuilder(maze, startPoint, endPoint);
            Graph = GraphBuidler.Graph;


            HeapPriorityQueue<MazeNode> minHeap = new HeapPriorityQueue<MazeNode>();
            MazeNode startNode = null;
            foreach(MazeNode node in Graph) { 
                if (node.Position.Equals(startPoint))
                {
                    node.Cost = 0;
                    startNode = node;
                } else
                {
                    node.Cost = int.MaxValue;
                }
                node.Visited = false;
                minHeap.Enqueue(node);
            }


                
            while (!minHeap.IsEmpty())
            {

                MazeNode thisNode = minHeap.Dequeue();
                foreach (MazeLink link in thisNode.Links)
                {
                    MazeNode linkedNode = link.TargetNode;
                    
                    if (!linkedNode.Visited)
                    {
                        if (thisNode.Cost + link.LinkLength < linkedNode.Cost)
                        {
                            linkedNode.Cost = thisNode.Cost + link.LinkLength;
                            linkedNode.ParentPath = thisNode;
                            minHeap.bubbleUp(minHeap.IndexOf(linkedNode));
                        }
                    }
                }
                thisNode.Visited = true;

            }

            MazeNode targetNode = Graph.Find(x => x.Position.Equals(endPoint));
            startNode = Graph.Find(x => x.Position.Equals(startPoint));


            Path = new Tilemapper.TileBitmap(maze.colCount, maze.rowCount);
            MazeNode currentNode = targetNode;
            // TODO.. fill in the blanks in the path
            while (currentNode != null)
            {
                Path.setIsSet(currentNode.Position.X, currentNode.Position.Y, true);
                if (currentNode.ParentPath != null)
                {
                    Point parentPathPosition = currentNode.ParentPath.Position;
                    if (currentNode.Position.X == parentPathPosition.X)
                    {
                        // vertical movement
                        if (parentPathPosition.Y > currentNode.Position.Y)
                        {
                            // moving down
                            for (int y = currentNode.Position.Y; y < parentPathPosition.Y; y++)
                            {
                                Path.setIsSet(currentNode.Position.X, y, true);
                            }

                        } 
                        else
                        {
                            // moving up
                            for (int y = currentNode.Position.Y; y > parentPathPosition.Y; y--)
                            {
                                Path.setIsSet(currentNode.Position.X, y, true);
                            }

                        }

                    } else
                    {
                        // horizontal movement
                        if (parentPathPosition.X > currentNode.Position.X)
                        {
                            // moving right
                            for (int x = currentNode.Position.X; x < parentPathPosition.X; x++)
                            {
                                Path.setIsSet(x,currentNode.Position.Y,true);
                            }
                        }
                        else
                        {
                            // moving left
                            for (int x = currentNode.Position.X; x > parentPathPosition.X; x--)
                            {
                                Path.setIsSet(x, currentNode.Position.Y, true);
                            }
                        }
                    }
                        
                }
                
                currentNode = currentNode.ParentPath;
            }


            currentNode = targetNode;
            // TODO.. fill in the blanks in the path
            PathToTarget = new Stack<Point>();
            while (currentNode != null)
            {                     
                PathToTarget.Push(currentNode.Position);                
                currentNode = currentNode.ParentPath;
            }

        }

    }
}
