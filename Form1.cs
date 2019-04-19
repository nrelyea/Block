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
        public bool gameActive = false;
        public List<List<bool>> filledSpace = new List<List<bool>> { };

        public Point spaceDimensions = new Point(1000, 600);
        public int spaceSize = 20;

        public Point ballPosition = new Point(300, 300);
        public Point ballVelocity = new Point(-3, 1);

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

            DrawBall(e, ballPosition.X, ballPosition.Y, spaceSize);

            if (gameActive)
            {

                ballPosition.X += ballVelocity.X;
                ballPosition.Y += ballVelocity.Y;

                ballVelocity.X *= BounceX(e);
                //ballVelocity.Y *= BounceY(e);

                /*
                if(ballPosition.X > 960 || ballPosition.X < 20)
                {
                    ballVelocity.X *= -1;
                }
                if (ballPosition.Y > 560 || ballPosition.Y < 20)
                {
                    ballVelocity.Y *= -1;
                }
                */

                Thread.Sleep(15);
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

            return space;
        }

        int BounceX(System.Windows.Forms.PaintEventArgs e)
        {
            //write(e, "press button to start", 100, 150, "Bold", 20);
            int testAX = ballPosition.X / spaceSize;
            int testBX = testAX + 1;

            int testAY = ballPosition.Y / spaceSize;
            int testBY = testAY + 1;

            

            Console.Write("\nX: " + ballPosition.X + "(between " + testAX + " & " + testBX + ")");
            Console.Write("\tY: " + ballPosition.Y + "(between " + testAY + " & " + testBY + ")");

            return 1;
        }

        int BounceY(Point position)
        {
            return 1;
        }

        void DrawBall(System.Windows.Forms.PaintEventArgs e, int x, int y, int size)
        {
            e.Graphics.FillEllipse(new SolidBrush(Color.Red), new Rectangle(x, y, size, size));
            e.Graphics.DrawEllipse(new Pen(new SolidBrush(Color.Black),2), new Rectangle(x, y, size, size));
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
