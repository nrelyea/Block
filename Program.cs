using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Block_Game
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            /*
            Point spaceDimensions = new Point(1000, 600);
            int spaceSize = 100;

            List<List<bool>> space = GenerateInitialSpace(spaceDimensions, spaceSize);

            PrintBoolListList(space);

            Console.Read();
            */

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());



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

        static void PrintBoolListList(List<List<bool>> lstlst)
        {
            for(int i=0; i<lstlst.Count; i++)
            {
                for(int j=0; j<lstlst[i].Count; j++)
                {
                    Console.Write(lstlst[i][j] + " | ");
                }
                Console.WriteLine();
            }
        }
    }
}
