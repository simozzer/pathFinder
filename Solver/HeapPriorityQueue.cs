using System;
using System.Collections.Generic;
using System.Text;

namespace SiFirstMonoGame.Solver
{
    // Priority Queue using min  priority
    class HeapPriorityQueue<T> where T:IComparable
    {
        internal List<T> Items
        {
            get;
            private set;
        }
        
        internal HeapPriorityQueue()
        {
            this.Items = new List<T>();        
        }

        ~HeapPriorityQueue()
        {
            Clear();
        }

        void Clear()
        {
            this.Items.Clear();
        }

        internal T Dequeue()
        {
            if (Count == 0)
            {
                throw new Exception("empty queue");
            }

            // return the item at the root
            T result = Items[0];

            // if there was only 1 item in the queue it's now empty
            if (Count == 1)
            {
                Items.Clear();
            }
            // if there were two, just replace the root with the one remaining child. The heap property is obviously satisfied
            else if (Count == 2)
            {
                Items[0] = Items[1];
                Items.RemoveAt(1);
            } // otherwise we have to repair the heap property
            else
            {
                Items[0] = Items[Count - 1];
                Items.RemoveAt(Count - 1);
                trickleDown();
            }

            return result;

        }

        internal void Enqueue(T item)
        {
            // add the item to the end of the list and bubble up as fast as it will go
            Items.Add(item);
            bubbleUp(Count - 1);
        }

        internal T Examine()
        {
            // TODO
            return default(T);
        }

        internal bool IsEmpty()
        {
            return Items.Count == 0;
        }

        internal bool Contains(T item)
        {
            return Items.Contains(item);
        }

        internal int IndexOf(T item)
        {
            return Items.IndexOf(item);
        }

        internal int Count
        {
            get
            {
                return Items.Count;
            }
        }

        internal void bubbleUp(int aFromInx)
        {
            T item = Items[aFromInx];
            /* While the item is under constructrion is smaller than it's parent, swap it with the parent (min priority)
             * and continue from its new position.
             * NB: THe parent for the child at index n in at n-1 /2 
             */
            int parentInx = (aFromInx - 1) / 2;
            // While our item has a parent and it' less that the parent...
            while ((aFromInx > 0) && (item.CompareTo(Items[parentInx])) < 0)
            {
                Items[aFromInx] = Items[parentInx];
                aFromInx = parentInx;
                parentInx = (aFromInx - 1) / 2;
            }
            Items[aFromInx] = item;
        }

        void trickleDown()
        {
            int fromInx = 0;
            T item = Items[0];
            int maxInx = Count - 1;
            /* swap the item under consideration with it's smallest child until it has no children.
             * NB the children for the parent an index n are 2n+ and 2n+2. */
            int childInx = (fromInx * 2) + 1;
            // while there is at least a left child
            while (childInx <= maxInx)
            {
                // if there is a right child as well, calculate the index of the smaller child
                if (((childInx + 1) <= maxInx) && (Items[childInx].CompareTo(Items[childInx + 1]) > 0))
                {
                    childInx++;
                }

                // move the smaller child up the tree, and move our item down the tree and repeat
                Items[fromInx] = Items[childInx];
                fromInx = childInx;
                childInx = (fromInx * 2) + 1;
            }
            // store our item where we end up
            Items[fromInx] = item;
            // now bubble this up the tree
            bubbleUp(fromInx);
        }

    }
}
