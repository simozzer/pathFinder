using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Microsoft.Xna.Framework;

namespace SiFirstMonoGame
{


    class MazeLink
    {
        internal MazeNode TargetNode
        {
            get;
            private set;
        }
        internal int LinkLength
        {
            get;
            private set;
        }

        internal MazeLink(MazeNode targetNode, int linkLength)
        {
            this.TargetNode = targetNode;
            this.LinkLength = linkLength;
        }
    }

    class MazeNode : IComparable<MazeNode>, IComparable
    {

        private List<MazeLink> links;

        Point position;


        internal int Cost
        {
            get;
            set;
        }

        internal bool Visited
        {
            get;
            set;
        }

        internal MazeNode ParentPath
        {
            get;
            set;
        }


        internal MazeNode(Point position )
        {            
            this.position = position;
            links = new List<MazeLink>();
            this.Cost = int.MaxValue;
            this.Visited = false;
            this.ParentPath = null;
        }

        internal List<MazeLink> Links
        {
            get
            {
                return links;
            }
        }

        internal Point Position
        {
            get
            {
                return position;
            }
        }

        public int CompareTo(object obj)
        {
           
            MazeNode node = (MazeNode)obj;
            return Cost.CompareTo(node.Cost);
        }



        internal MazeLink addLink(MazeNode target, int len)
        {
            if (target == null)
            { 
                throw new Exception("null target");
            }
            if (!containsLink(target))
            {
                MazeLink newLink = new MazeLink(target, len);
                links.Add(newLink);
                return newLink;
            } else
            {
                return null;
            }
        }

        internal bool containsLink(MazeNode target)
        {
            foreach(MazeLink link in links)
            {
                if (link.TargetNode == target)
                {
                    return true;
                }
            }
            return false;
        }

        public int CompareTo([AllowNull] MazeNode other)
        {
            return Cost.CompareTo(other.Cost);
        }
    }

    class DijktraNode: MazeNode
    {
        internal DijktraNode(Point position): base(position)
        {
            this.Cost = int.MaxValue;
            this.Visited = false;
        }
    }
}
