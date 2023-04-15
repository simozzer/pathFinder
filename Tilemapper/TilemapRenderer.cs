using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SiFirstMonoGame.Tilemapper
{
    class TilemapRenderer
    {
        TileBitmap tileMap;
        GraphicsDevice device;
        int width, height;
        Vector2 viewportOrigin;
        int tileSize;
        Texture2D texture;
        Texture2D emptySpace;

        public TilemapRenderer(GraphicsDevice device, TileBitmap tileMap, Vector2 viewportOrigin, int tileSize, Texture2D texture, Texture2D emptySpace)
        {
            this.device = device;
            this.width = device.Viewport.Width;
            this.height = device.Viewport.Height;
            this.viewportOrigin = viewportOrigin;
            this.tileSize = tileSize;
            this.tileMap = tileMap;
            this.texture = texture;
            this.emptySpace = emptySpace;
        }

        bool pointIsInTileMap(Point point)
        {
            return ((point.X >= 0)
                && (point.X < (tileSize * tileMap.colCount))
                && (point.Y >=0)
                && (point.Y < (tileSize * tileMap.rowCount)));
        }

        Point screenToMapPoint(Point point)
        {
            return new Point(point.X + (int)viewportOrigin.X, point.Y + (int)viewportOrigin.Y);
        }

        Point cellIndexesFromPixel(Point point)
        {
            if (pointIsInTileMap(point))
            {
                Point positionOnMap = screenToMapPoint(point);
                return new Point(positionOnMap.X / tileSize, positionOnMap.Y / tileSize);
            } else
            {
                return new Point(-1, -1);
            }
        }

        Point screenOriginForCellIndexes(Point point)
        {
            return new Point((int)((point.X * tileSize) - viewportOrigin.X), (int)((point.Y * tileSize) - viewportOrigin.Y));
        }

        public void render(SpriteBatch b, bool resize, Color color)
        {
            for (int x = -tileSize; x < width + tileSize; x += tileSize)
            {

                for (int y = -tileSize; y < height + tileSize; y += tileSize)
                {
                    Point screenPoint = new Point(x, y);
                    if (pointIsInTileMap(screenToMapPoint(screenPoint)))
                    {
                        Point cellIndexes = cellIndexesFromPixel(screenPoint);
                        if ((cellIndexes.X >=0) && (cellIndexes.Y >=0))
                        {                               
                            Point drawOrigin = screenOriginForCellIndexes(cellIndexes);
                            
                            if (resize)
                                {
                                    Rectangle tileRect = new Rectangle(new Point(drawOrigin.X + (tileSize / 2) - (texture.Bounds.Width / 2),
                                        drawOrigin.Y + (tileSize/2) - (texture.Bounds.Height/2)), 
                                        new Point(texture.Bounds.Width, texture.Bounds.Height));                                                                    
                                    if (tileMap.getIsSet(cellIndexes.X, cellIndexes.Y))
                                    {
                                        b.Draw(texture, tileRect, Color.White);
                                    } else
                                    {
                                        if (emptySpace != null)
                                        {
                                            Rectangle backTileRect = new Rectangle(drawOrigin, new Point(tileSize, tileSize));
                                            b.Draw(emptySpace, backTileRect, color);
                                        }
                                    }

                                } 
                                else
                                {
                                    Rectangle tileRect = new Rectangle(drawOrigin, new Point(tileSize, tileSize));
                                    if (tileMap.getIsSet(cellIndexes.X, cellIndexes.Y))
                                    {
                                        b.Draw(texture, tileRect, Color.White);
                                    } else
                                    {
                                        if (emptySpace != null)
                                        {
                                            Rectangle backTileRect = new Rectangle(drawOrigin, new Point(tileSize, tileSize));
                                            b.Draw(emptySpace, backTileRect, color);
                                        }
                                    }
                                }

                            }
                        }
                    }
                }

            }
        }
 
}
