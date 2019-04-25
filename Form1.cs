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
        public int refreshRate = 2;

        public bool gameActive = false;
        public List<List<bool>> filledSpace = new List<List<bool>> { };
        public List<List<Color>> colorMatrix = new List<List<Color>> { };

        public Color unbreakable = Color.Black;

        public Point spaceDimensions = new Point(1000, 600);
        public int spaceSize = 20;

        public Point ballPosition = new Point(490, 520);
        public Point ballVelocity = new Point(3, -3);

        public int barPosition;
        public Point mousePosition = new Point(0, 0);

        public int prevAX;
        public int prevBX;
        public int prevAY;
        public int prevBY;

        public Form1()
        {
            this.KeyPreview = false;

            prevAX = ballPosition.X / spaceSize;
            prevBX = prevAX + 1;

            prevAY = ballPosition.Y / spaceSize;
            prevBY = prevAY + 1;

            filledSpace = GenerateInitialSpace(spaceDimensions, spaceSize);
            colorMatrix = GenerateBlankColorMatrix(spaceDimensions, spaceSize);
            PopulateMatrices();

            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.DoubleBuffered = true;

            this.WindowState = FormWindowState.Maximized;

            this.Paint += new PaintEventHandler(Form1_Paint);

        }

        public void Form1_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {

            BreakBlock();

            //write(e, "Rina is the most beautiful girl in the world!", 225, 320, "Bold", 20);

            DrawGrid(e);

            DrawBall(e, ballPosition.X, ballPosition.Y, spaceSize);

            if (gameActive)
            {
                SetOptionChanging(false);

                DrawBar(e);

                MoveBall();



                Thread.Sleep(refreshRate);
                this.Invalidate();
            }
            else
            {
                SetOptionChanging(true);

                button1.Text = "start";
                label1.Text = refreshRate.ToString();

                write(e, "press 'start' to begin", 375, 325, "Bold", 20);

                DrawBar(e);



            }


        }

        public List<List<bool>> GenerateInitialSpace(Point spaceDimensions, int spaceSize)
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
                    column.Add(false);
                }
                space.Add(column);
            }

            return space;
        }

        public List<List<Color>> GenerateBlankColorMatrix(Point spaceDimensions, int spaceSize)
        {
            List<List<Color>> colorMatrix = new List<List<Color>> { };

            int columnCount = (spaceDimensions.X / spaceSize);
            int rowCount = (spaceDimensions.Y / spaceSize);

            for (int i = 0; i < columnCount; i++)
            {
                List<Color> column = new List<Color> { };
                for (int j = 0; j < rowCount; j++)
                {
                    column.Add(Color.Purple);
                }
                colorMatrix.Add(column);
            }

            return colorMatrix;
        }

        public void PopulateMatrices()
        {
            // Draw Border
            for (int i = 0; i < filledSpace.Count; i++)
            {
                for (int j = 0; j < filledSpace[i].Count; j++)
                {
                    if (j == 0 || j == filledSpace[i].Count - 1 || i == 0 || i == filledSpace.Count - 1)
                    {
                        filledSpace[i][j] = true;
                        colorMatrix[i][j] = unbreakable;
                    }
                }
            }

            for (int x = 4; x < 46; x += 6)
            {
                for (int i = x; i < x + 3; i++)
                {
                    filledSpace[i][13] = true;
                    colorMatrix[i][13] = Color.Red;
                }
                for (int i = x + 3; i < x + 6; i++)
                {
                    filledSpace[i][13] = true;
                    colorMatrix[i][13] = Color.Blue;
                }
            }
            for (int x = 4; x < 46; x += 6)
            {
                for (int i = x; i < x + 3; i++)
                {
                    filledSpace[i][14] = true;
                    colorMatrix[i][14] = Color.LawnGreen;
                }
                for (int i = x + 3; i < x + 6; i++)
                {
                    filledSpace[i][14] = true;
                    colorMatrix[i][14] = Color.Yellow;
                }
            }


        }

        public void DrawGrid(System.Windows.Forms.PaintEventArgs e)
        {
            for (int i = 0; i < filledSpace.Count; i++)
            {
                for (int j = 0; j < filledSpace[i].Count; j++)
                {
                    if (filledSpace[i][j])
                    {
                        e.Graphics.FillRectangle(new SolidBrush(colorMatrix[i][j]), new Rectangle(i * spaceSize, j * spaceSize, spaceSize, spaceSize));
                    }
                }
            }
        }

        public void DrawBar(System.Windows.Forms.PaintEventArgs e)
        {
            if (!gameActive)
            {
                barPosition = 440;
            }
            else if (mousePosition.X < 80)
            {
                barPosition = 20;
            }
            else if (mousePosition.X > 920)
            {
                barPosition = 860;
            }
            else
            {
                barPosition = mousePosition.X - 60;
            }
            e.Graphics.FillRectangle(new SolidBrush(Color.Red), new Rectangle(20, 540, 960, 40));
            e.Graphics.FillRectangle(new SolidBrush(Color.Green), new Rectangle(barPosition, 540, 120, 40));
            e.Graphics.DrawRectangle(new Pen(new SolidBrush(Color.Black), 2), new Rectangle(barPosition, 540, 120, 40));


        }

        public void MoveBall()
        {
            prevAX = ballPosition.X / spaceSize;
            prevBX = prevAX + 1;

            prevAY = ballPosition.Y / spaceSize;
            prevBY = prevAY + 1;

            ballPosition.X += ballVelocity.X;
            ballPosition.Y += ballVelocity.Y;

            Point bounceFactor = Bounce();
            ballVelocity.X *= bounceFactor.X;
            ballVelocity.Y *= bounceFactor.Y;

            if (ballPosition.Y > (540 - spaceSize))
            {
                if ((ballPosition.X + spaceSize) > barPosition && ballPosition.X < (barPosition + 120))
                {
                    ballVelocity.Y *= -1;
                }
                else
                {
                    ballPosition = new Point(490, 520);
                    ballVelocity = new Point(3, -3);
                    gameActive = false;
                    PopulateMatrices();
                }

            }
        }

        public void BreakBlock()
        {
            for (int i = prevAX; i < prevAX + 2; i++)
            {
                for (int j = prevAY; j < prevAY + 2; j++)
                {
                    if (filledSpace[i][j] && colorMatrix[i][j] != unbreakable)
                    {
                        for (int k = i - 2; k < i + 3; k++)
                        {
                            if (colorMatrix[k][j] == colorMatrix[i][j])
                            {
                                filledSpace[k][j] = false;
                            }
                        }
                    }
                }
            }
        }

        Point Bounce()
        {
            int AX = ballPosition.X / spaceSize;
            int BX = AX + 1;

            int AY = ballPosition.Y / spaceSize;
            int BY = AY + 1;

            if (ContactCount(AX, BX, AY, BY) > 0)
            {
                if (ContactCount(AX, BX, AY, BY) == 3)
                {
                    //Console.WriteLine("CORNER HIT EVERYBODY CELEBRATE IT ACTUALLY HAPPENED OMG!!!!!!!!!!!!!1");
                    return new Point(-1, -1);
                }
                else if ((filledSpace[AX][AY] && filledSpace[AX][BY]) || (filledSpace[BX][AY] && filledSpace[BX][BY]))
                {
                    //Console.WriteLine("L/R HIT");
                    return new Point(-1, 1);
                }
                else if ((filledSpace[AX][AY] && filledSpace[BX][AY]) || (filledSpace[AX][BY] && filledSpace[BX][BY]))
                {
                    //Console.WriteLine("U/D HIT");                                       
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
                    //Console.WriteLine("1 diff: " + diff);
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
                    //Console.WriteLine("2 diff: " + diff);
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
                    //Console.WriteLine("3 diff: " + diff);                    
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
                    //Console.WriteLine("4 diff: " + diff);                    
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

        public void SetOptionChanging(bool val)
        {
            button2.Enabled = val;
            button3.Enabled = val;
        }

        public int ContactCount(int AX, int BX, int AY, int BY)
        {
            return Convert.ToInt32(filledSpace[AX][AY]) + Convert.ToInt32(filledSpace[AX][BY]) + Convert.ToInt32(filledSpace[BX][AY]) + Convert.ToInt32(filledSpace[BX][BY]);
        }

        void DrawBall(System.Windows.Forms.PaintEventArgs e, int x, int y, int size)
        {
            //e.Graphics.FillEllipse(new SolidBrush(Color.Red), new Rectangle(x, y, size, size));
            //e.Graphics.DrawEllipse(new Pen(new SolidBrush(Color.Black), 2), new Rectangle(x, y, size, size));
            e.Graphics.FillRectangle(new SolidBrush(Color.Green), new Rectangle(x, y, size, size));
            e.Graphics.DrawRectangle(new Pen(new SolidBrush(Color.Black), 2), new Rectangle(x, y, size, size));
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
            gameActive = !gameActive;
            if (gameActive)
            {
                button1.Text = "pause";
            }
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            mousePosition.X = e.X;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            refreshRate++;
            this.Invalidate();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (refreshRate > 0)
            {
                refreshRate--;
                this.Invalidate();
            }
        }
    }
}
