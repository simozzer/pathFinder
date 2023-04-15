
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace SiFirstMonoGame.Tilemapper
{
    class Bitline
    {

        private byte[] bytes;
        private int bytesInArray;
        private int len;
        public Bitline(int length)
        {
            if (length > 255)
            {
                throw new Exception("Max Bitline size exceeed");
            }
            len = length;
            bytesInArray = (length / 8) + 1;
            bytes = new byte[bytesInArray];
            for (int i=0; i < bytesInArray; i++)
            {
                bytes[i] = 0;
            }
        }

        public Bitline(Stream stream)
        {
            len = stream.ReadByte();
            bytesInArray = (len / 8) + 1;
            bytes = new byte[bytesInArray];
            stream.Read(bytes, 0, bytesInArray);

        }

        public bool getBit(int index)
        {
            if ((index <0 ) || (index >= len))
            {
                return false;
            }
            else
            {
                int byteIndex = index / 8;
                int bitIndex = index % 8;
                byte byteEntry = bytes[byteIndex];
                if ((byteEntry & (1 << bitIndex)) != 0)
                {
                    return true;
                } 
                else
                {
                    return false;
                }

            }
        }

        public void setBit(int index, bool bSet)
        {
            if ((index >= 0) || (index < len))
            {
                int byteIndex = index / 8;
                int bitIndex = index % 8;
                byte byteEntry = bytes[byteIndex];
                if (bSet)
                {
                    bytes[byteIndex] = (byte)(bytes[byteIndex] | (1 << bitIndex));
                } else
                {
                    bytes[byteIndex] = (byte)(bytes[byteIndex] & ~(1 << bitIndex));
                }             
            }
        }

        public void writeToStream(Stream strm)
        {
            strm.WriteByte((byte)len);           
            bytesInArray = (len / 8) + 1;
            strm.Write(bytes, 0, bytesInArray);            
        }

        public int numberSet()
        {
            int count = 0;
            for (int i=0; i < len; i++)
            {
                if (getBit(i) == true)
                {
                    count++;
                }
            }
            return count;
        }
    }


    class TileBitmap
    {
        int cols;
        int rows;

        Bitline[] lines;
        public TileBitmap(int colCount, int rowCount)
        {
            cols = colCount;
            rows = rowCount;
            lines = new Bitline[rowCount];
            for (int i=0; i < rowCount; i++)
            {
                lines[i] = new Bitline(colCount);
            }
        }
        public TileBitmap(TileBitmap source)
        {
            cols = source.colCount;
            rows = source.rowCount;
            lines = new Bitline[rowCount];
            for (int r = 0; r < rowCount; r++)
            {
                Bitline line = new Bitline(cols);
                lines[r] = line;
                for (int c = 0; c < colCount; c++)
                {
                    setIsSet(c, r, source.getIsSet(c, r));
                }
            }

        }

        public TileBitmap(Stream stream)
        {
            cols = stream.ReadByte();
            rows = stream.ReadByte();
            lines = new Bitline[rows];
            for (int i = 0; i < rows; i++)
            {
                lines[i] = new Bitline(stream);
            }

        }

        public bool getIsSet(int col, int row)
        {
            if ((col >= 0) && (row >= 0) && (col < cols) && (row < rows)) {
                return lines[row].getBit(col);
            } else
            {
                return true;
            }
        }

        public void setIsSet(int col, int row, bool bSet)
        {
            if ((col >= 0) && (row >= 0) && (col < cols) && (row < rows))
            {
                lines[row].setBit(col, bSet);
            }
        }

        public void writeToStream(Stream strm)
        {
            strm.WriteByte((byte)cols);
            strm.WriteByte((byte)rows);
            for (int row=0; row < rows; row++)
            {
                lines[row].writeToStream(strm);
            }
        }

        public int colCount
        {
            get
            {
                return cols;
            }
        }

        public int rowCount
        {
            get
            {
                return rows;
            }
            
        }

        public int numberSet()
        {
            int count = 0;
            for (int i=0; i < rowCount; i++) {
                count += lines[i].numberSet();
            }
            return count;
        }
    }
}
