using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace SiFirstMonoGame.Tilemapper
{
    class TileMapCollision
    {
        /**
         * Returns a list of cell co-ordinates that are intersected by the player
         **/
        public  static List<Point> getCollisions(TileBitmap map, int tileSize, Point mapPoint, int playerSize)
        {
            List<Point> collisions = new List<Point>();
            int cellX = mapPoint.X / tileSize;
            int cellY = mapPoint.Y / tileSize;
            if (map.getIsSet(cellX,cellY))
            {
                collisions.Add(new Point(cellX, cellY));                   
            }

            int cellX2 = (mapPoint.X + playerSize) / tileSize;
            if ((cellX2 != cellX) && map.getIsSet(cellX2, cellY))             
            {                
                collisions.Add(new Point(cellX2, cellY));
            }
            
            int cellY2 = (mapPoint.Y + playerSize) / tileSize;
            if (cellY2 != cellY)
            {
                if (map.getIsSet(cellX,cellY2))
                {
                    collisions.Add(new Point(cellX, cellY2));
                }

                if ((cellX2 != cellX) && map.getIsSet(cellX2, cellY2))
                {
                    collisions.Add(new Point(cellX2, cellY2));
                }
            }

            return collisions;

        }
    }
}
