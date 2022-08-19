using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseTestApplicatie.OutputMethods
{
    public class RepeatingMenus
    {

        /*
         * Data fields, etc.
         * 
         */

        private OutputMethodsConsole outputMethodsConsole = new();

        string[] availableSpiciness = new string[] { "NONE", "HOT", "SPICY" };

        /*
         * Other methods.
         * 
         */

        public int RepeatMenu(string menuTitle, string[]? menuItems, int minimum, int maximum)
        {
            Console.WriteLine();
            Console.WriteLine(menuTitle);

            if (menuItems != null)
            {
                outputMethodsConsole.ShowMenuItems(menuItems);
                Console.WriteLine();
            }

            string? input = Console.ReadLine();
            int number;
            bool parsed = int.TryParse(input, out number);

            while (!parsed || !IsValidRange(number, minimum, maximum))
            {
                Console.WriteLine("Invalid Input");
                input = Console.ReadLine();
                parsed = int.TryParse(input, out number);
            }

            return number;
        }

        public object? RepeatInput(byte type)
        {
            string? input = "";
            object? data = null;

            if (type == 0 || type == 2)
            {
                input = Console.ReadLine();
            }

            if (type == 0)
            {
                //Test for string, not null.
                if (string.IsNullOrEmpty(input.Trim()))
                {
                    while (string.IsNullOrEmpty(input.Trim()))
                    {
                        Console.WriteLine("String can\'t be empty or null");
                        input = Console.ReadLine();
                    }

                    data = input;
                }
                else
                {
                    data = input;
                }
            }
            else if (type == 1)
            {
                //Choose item.
                int choice = RepeatMenu("Choose Spiciness:", availableSpiciness, 1, 3);
                data = availableSpiciness[choice - 1];
            }
            else if (type == 2)
            {
                //Test for decimal.
                decimal number;
                bool isAllowed = decimal.TryParse(input, out number);

                while (!isAllowed || number <= (decimal)0.0)
                {
                    Console.WriteLine("Price invalid or lower than 0.0");
                    input = Console.ReadLine();
                    isAllowed = decimal.TryParse(input, out number);
                }

                data = (decimal)number;
            }

            return data;
        }

        public bool IsValidRange(int num, int min, int max)
        {
            if (num < min || num > max)
            {
                return false;
            }

            return true;
        }

        public string DeriveValue(object input, byte type)
        {
            string s = "";

            if (type == 0 || type == 1)
            {
                //s = "\'" + input.ToString() + "\'";
                s = input.ToString();
            }
            else if (type == 2)
            {
                s = ((decimal)input).ToString().Replace(",", ".");
            }

            return s;
        }

    }
}
