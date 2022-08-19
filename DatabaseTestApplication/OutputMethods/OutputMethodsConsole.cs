using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseTestApplicatie.OutputMethods
{
    public class OutputMethodsConsole
    {
        public void ShowColumnNames(string[] arr)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                Console.Write(arr[i] + "\t");

                if(i == 1)
                {
                    //Add extra tab.
                    Console.Write("\t");
                }
            }

            Console.WriteLine();
        }

        public void ShowData2D(string[,] arr)
        {
            if (arr != null)
            {
                for (int i = 0; i < arr.GetLength(0); i++)
                {
                    //Add extra tabs at some output.
                    string test1 = (arr[i, 1].Length <= 7) ? arr[i, 1] + "\t" : arr[i, 1];

                    string test4 = "\t" + arr[i, 4];

                    Console.WriteLine(string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t", arr[i, 0], test1, arr[i, 2], arr[i, 3], test4));
                }
            }
            else
            {
                Console.WriteLine("Nothing to Show!");
            }
        }

        public void ShowData(string[] arr)
        {
            if (arr != null)
            {
                for (int i = 0; i < arr.Length; i++)
                {
                    Console.WriteLine(arr[i]);
                }
            }
            else
            {
                Console.WriteLine("Nothing to Show!");
            }
        }

        public void ShowMenuItems(string[] arr)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                Console.WriteLine((i + 1) + " = " + arr[i]);
            }
        }

    }
}
