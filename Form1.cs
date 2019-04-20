using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Block_Game
{
    public partial class Form1 : Form
    {
        public int refreshRate = 5;

        public bool gameActive = false;
        public List<List<bool>> filledSpace = new List<List<bool>> { };

        public Point spaceDimensions = new Point(1000, 600);
        public int spaceSize = 20;

        public Point ballPosition = new Point(100, 60);
        public Point ballVelocity = new Point(0, 3);

        public int prevAX = 0;
        public int prevBX = 0;
        public int prevAY = 0;
        public int prevBY = 0;

        public Form1()
        {
            this.KeyPreview = false;
            //List<List<bool>> filledSpace = new List<List<bool>> { };
            filledSpace = GenerateInitialSpace(spaceDimensions, spaceSize);
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.DoubleBuffered = true;

            this.Paint += new PaintEventHandler(Form1_Paint);

        }

        public void Form1_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            DrawBorder(e, spaceDimensions.X, spaceDimensions.Y, spaceSize);
            e.Graphics.FillRectangle(new SolidBrush(Color.Red), new Rectangle(spaceDimensions.X - spaceSize, spaceDimensions.Y - spaceSize, spaceSize, spaceSize));

            e.Graphics.FillRectangle(new SolidBrush(Color.Black), new Rectangle(spaceSize * 6, spaceSize * 6, spaceSize * 38, spaceSize));

            DrawBall(e, ballPosition.X, ballPosition.Y, spaceSize);

            if (gameActive)
            {
                int prevAX = ballPosition.X / spaceSize;
                int prevBX = prevAX + 1;

                int prevAY = ballPosition.Y / spaceSize;
                int prevBY = prevAY + 1;

                ballPosition.X += ballVelocity.X;
                ballPosition.Y += ballVelocity.Y;

                Point bounceFactor = Bounce(e, prevAX, prevBX, prevAY, prevBY);
                ballVelocity.X *= bounceFactor.X;
                ballVelocity.Y *= bounceFactor.Y;



                Thread.Sleep(refreshRate);
                this.Invalidate();
            }
            else
            {
                write(e, "press button to start", 100, 100, "Bold", 20);
                //write(e, filledSpace[1][1].ToString(), 100, 150, "Bold", 20);
            }


        }

        static List<List<bool>> GenerateInitialSpace(Point spaceDimensions, int spaceSize)
        {
            List<List<bool>> space = new List<List<bool>> { };

            int columnCount = (spaceDimensions.X / spaceSize);
            int rowCount = (spaceDimensions.Y / spaceSize);

            Console.WriteLine("Grid: " + columnCount + " x " + rowCount);

            for (int i = 0; i < columnCount; i++)
            {
                List<bool> column = new List<bool> { };
                for (int j = 0; j < rowCount; j++)
                {
                    if (j == 0 || j == rowCount - 1 || i == 0 || i == columnCount - 1)
                    {
                        column.Add(true);
                    }
                    else
                    {
                        column.Add(false);
                    }
                }
                space.Add(column);
            }


            for (int i = 6; i < 44; i++)
            {
                space[i][6] = true;
            }
            return space;
        }

        Point Bounce(System.Windows.Forms.PaintEventArgs e, int prevAX, int prevBX, int prevAY, int prevBY)
        {
            int AX = ballPosition.X / spaceSize;
            int BX = AX + 1;

            int AY = ballPosition.Y / spaceSize;
            int BY = AY + 1;

            if (ContactCount(AX, BX, AY, BY) > 0)
            {
                if (ContactCount(AX, BX, AY, BY) == 3)
                {
                    Console.WriteLine("CORNER HIT");
                    return new Point(-1, -1);
                }
                else if ((filledSpace[AX][AY] && filledSpace[AX][BY]) || (filledSpace[BX][AY] && filledSpace[BX][BY]))
                {
                    Console.WriteLine("L/R HIT");
                    return new Point(-1, 1);
                }
                else if ((filledSpace[AX][AY] && filledSpace[BX][AY]) || (filledSpace[AX][BY] && filledSpace[BX][BY]))
                {
                    Console.WriteLine("U/D HIT");
                    return new Point(1, -1);
                }

                int diff = 0;
                if (filledSpace[AX][AY])            //intersect top left corner
                {
                    if (ballVelocity.X >= 0)
                    {
                        return new Point(1, -1);
                    }
                    else if (ballVelocity.Y >= 0)
                    {
                        return new Point(-1, 1);
                    }

                    Point diffPoint = new Point(ballPosition.X - (spaceSize * AX), ballPosition.Y - (spaceSize * AY));
                    diff = diffPoint.Y - diffPoint.X;
                    Console.WriteLine("1 diff: " + diff);
                }
                else if (filledSpace[BX][AY])       // top right corner
                {
                    if (ballVelocity.X <= 0)
                    {
                        return new Point(1, -1);
                    }
                    else if (ballVelocity.Y >= 0)
                    {
                        return new Point(-1, 1);
                    }

                    Point diffPoint = new Point((spaceSize * BX) - ballPosition.X, ballPosition.Y - (spaceSize * AY));
                    diff = diffPoint.Y - diffPoint.X;
                    Console.WriteLine("2 diff: " + diff);
                }
                else if (filledSpace[AX][BY])       // bottom left corner
                {
                    if (ballVelocity.X >= 0)
                    {
                        return new Point(1, -1);
                    }
                    else if (ballVelocity.Y <= 0)
                    {
                        return new Point(-1, 1);
                    }

                    Point diffPoint = new Point(ballPosition.X - (spaceSize * AX), (spaceSize * BY) - ballPosition.Y);
                    diff = diffPoint.Y - diffPoint.X;
                    Console.WriteLine("3 diff: " + diff);
                }
                else if (filledSpace[BX][BY])       // bottom right corner
                {
                    if (ballVelocity.X <= 0)
                    {
                        return new Point(1, -1);
                    }
                    else if (ballVelocity.Y <= 0)
                    {
                        return new Point(-1, 1);
                    }

                    Point diffPoint = new Point((spaceSize * BX) - ballPosition.X, (spaceSize * BY) - ballPosition.Y);
                    diff = diffPoint.Y - diffPoint.X;
                    Console.WriteLine("4 diff: " + diff);
                }
                if (diff > 0)
                {
                    return new Point(1, -1);
                }
                else if (diff < 0)
                {
                    return new Point(-1, 1);
                }
                else
                {
                    return new Point(-1, -1);
                }
            }

            return new Point(1, 1);
        }

        public int ContactCount(int AX, int BX, int AY, int BY)
        {
            return Convert.ToInt32(filledSpace[AX][AY]) + Convert.ToInt32(filledSpace[AX][BY]) + Convert.ToInt32(filledSpace[BX][AY]) + Convert.ToInt32(filledSpace[BX][BY]);
        }

        public bool PrevPositionWasClear(int prevAX, int prevBX, int prevAY, int prevBY)
        {
            if (!filledSpace[prevAX][prevAY] && !filledSpace[prevAX][prevBY] && !filledSpace[prevBX][prevAY] && !filledSpace[prevBX][prevBY])
            {
                return true;
            }
            return false;
        }

        void DrawBall(System.Windows.Forms.PaintEventArgs e, int x, int y, int size)
        {
            e.Graphics.FillEllipse(new SolidBrush(Color.Red), new Rectangle(x, y, size, size));
            e.Graphics.DrawEllipse(new Pen(new SolidBrush(Color.Black), 2), new Rectangle(x, y, size, size));
        }

        void DrawBorder(System.Windows.Forms.PaintEventArgs e, int width, int height, int thickness)
        {
            SolidBrush black = new SolidBrush(Color.Black);
            e.Graphics.FillRectangle(black, new Rectangle(0, 0, thickness, height));
            e.Graphics.FillRectangle(black, new Rectangle(0, height - thickness, width, thickness));
            e.Graphics.FillRectangle(black, new Rectangle(0, 0, width, thickness));
            e.Graphics.FillRectangle(black, new Rectangle(width - thickness, 0, thickness, height));
        }

        void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            MessageBox.Show("Form.KeyPress: '" +
                e.KeyChar.ToString() + "' pressed.");

            switch (e.KeyChar)
            {
                case (char)49:
                case (char)52:
                case (char)55:
                    MessageBox.Show("Form.KeyPress: '" +
                        e.KeyChar.ToString() + "' consumed.");
                    e.Handled = true;
                    break;
            }

        }

        List<string> SwapStrings(List<string> list, int index1, int index2)
        {
            string temp = list[index1];
            list[index1] = list[index2];
            list[index2] = temp;

            return list;
        }

        void write(System.Windows.Forms.PaintEventArgs e, string str, int x, int y, string fontType, int fontSize)
        {

            Font[] fonts = { new Font("Arial", fontSize, FontStyle.Regular),
            new Font("Arial", fontSize, FontStyle.Bold),
            new Font("Arial", fontSize, FontStyle.Italic) };

            int fontTypeIndex = 0;

            if (fontType == "Bold") { fontTypeIndex = 1; }
            else if (fontType == "Italic") { fontTypeIndex = 2; }

            TextFormatFlags flags = TextFormatFlags.Bottom | TextFormatFlags.EndEllipsis;

            TextRenderer.DrawText(e.Graphics, str, fonts[fontTypeIndex],
            new Point(x, y),
            SystemColors.ControlText, flags);

        }

        private void tnrAppTimer_Tick(object sender, EventArgs e)
        {
            this.Refresh();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!gameActive)
            {
                gameActive = true;
            }
            else
            {
                gameActive = false;
            }
        }
    }
}
