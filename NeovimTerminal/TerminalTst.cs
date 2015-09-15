using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Interop;
using Brushes = System.Drawing.Brushes;
using Color = System.Drawing.Color;
using FontFamily = System.Drawing.FontFamily;
using FontStyle = System.Drawing.FontStyle;
using Image = System.Windows.Controls.Image;
using Point = System.Windows.Point;
using Size = System.Windows.Size;

namespace NeovimTerminal
{
    public class TerminalTst : Image
    {
        private Bitmap bmp;
        //private Bitmap _caret;

        private Graphics g;
        public Rectangle Caret;
        public Color Background { get; set; }
        public Color Foreground { get; set; }
        public Font Font { get; set; }

        public int CellWidth = 12;
        public int CellHeight = 16;

        private int _Rows;
        private int _Columns;

        public TerminalTst()
        {
            Font = new Font(new FontFamily("Arial"), 12);
            CellWidth = 12;
            CellHeight = 16;

            Caret = new Rectangle();
            Caret.Width = CellWidth;
            Caret.Height = CellHeight;

            Background = Color.DarkSlateGray;
            Foreground = Color.White;

            Resize(25, 80);
        }

        public void SwapBuffers()
        {
            //TODO: this will cause an outofmemory exception
            //      need to draw bitmaps and such
            var finalBmp = new Bitmap(bmp);
            //final = final == null ? new Bitmap(bmp) : final;
            var f = Graphics.FromImage(finalBmp);

            var attrs = new ImageAttributes();
            ColorMatrix m = new ColorMatrix(new float[][]
            {
                new float[] {-1, 0, 0, 0, 0},
                new float[] {0, -1, 0, 0, 0},
                new float[] {0, 0, -1, 0, 0},
                new float[] {0, 0, 0, 1, 0},
                new float[] {1, 1, 1, 0, 1}
            }); 
            attrs.SetColorMatrix(m);

            f.DrawImage(finalBmp, new Rectangle((int)Caret.X, (int)Caret.Y, CellWidth, CellHeight), Caret.X, Caret.Y, (float)CellWidth, (float)CellHeight, GraphicsUnit.Pixel, attrs);

            f.Dispose();

            var pointer = finalBmp.GetHbitmap();
            ImageSource src = Imaging.CreateBitmapSourceFromHBitmap(pointer, IntPtr.Zero, Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
            this.Source = src;

            finalBmp.Dispose();
            finalBmp = null;
        }

        public void MoveCaret(int row, int col)
        {
            Caret.X = col*CellWidth;
            Caret.Y = row*CellHeight;
        }

        public void PutText(string text)
        {
            var font = new Font(new System.Drawing.FontFamily("Arial"), 8);
            var brush = new SolidBrush(Foreground);
            for (int i = 0; i < text.Length; i++)
            {
                g.FillRectangle(new SolidBrush(Background), Caret.X, Caret.Y, CellWidth, CellHeight);
                g.DrawString(text[i].ToString(), font, brush, Caret.X, Caret.Y);
                Caret.X += CellWidth;
            }
        }

        public void Scroll(sbyte direction)
        {
            if (direction == -1)
            {
                g.DrawImage(bmp, 0, CellHeight);
                g.FillRectangle(new SolidBrush(Background), Caret.X, Caret.Y, (float)this.Width, CellHeight);
            }

            else if (direction == 1)
            {
                g.DrawImage(bmp, 0, -CellHeight);
                g.FillRectangle(new SolidBrush(Background), Caret.X, Caret.Y, (float)this.Width, CellHeight);
            }
        }

        public void Clear()
        {
            g.Clear(Background);
        }

        public void ClearToEnd()
        {
            g.FillRectangle(new SolidBrush(Background), Caret.X, Caret.Y, (float)this.Width, CellHeight);
        }

        public void Highlight(Color foreground, bool bold, bool italic)
        {
            Foreground = foreground;
        }

        public void Resize(int rows, int columns)
        {
            _Rows = rows;
            _Columns = columns;
            if (bmp == null)
            {
                bmp = new Bitmap(columns * CellWidth, rows * CellHeight);
            }
            else
            {
                
            }
            g = Graphics.FromImage(bmp);
            Width = CellWidth*columns;
            Height = CellHeight*rows;
        }
    }
}
