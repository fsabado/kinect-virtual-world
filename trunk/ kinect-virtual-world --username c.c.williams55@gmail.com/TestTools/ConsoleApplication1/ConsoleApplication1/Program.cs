using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            HttpSlphp hsp = new HttpSlphp();
            var dic = hsp.GetPrimListPHP();
            foreach (var item in dic.Keys)
            {
                Console.WriteLine("Prim Name: " + item + " Url: " + dic[item]);
            }

            Console.WriteLine("Enter 'q' to quit or 'url' to change the object you want to reach, 'color' to send values. etc. etc...");

            var userInput = "";

            do 
            {
                String temp,x,y,z = "";
                
                switch (userInput)
                {
                    case "url":
                        Console.WriteLine("Enter url to contact:");
                        temp = Console.ReadLine();
                        temp = temp.Trim();
                        hsp.Url = temp;
                        Console.WriteLine("Url set to: " + hsp.Url);

                        break;
                    case "color":
                        Console.WriteLine("Enter color data Red: Ex: 0.3");
                        x = Console.ReadLine().Trim();
                        Console.WriteLine("Enter color data Green: Ex: 0.2");
                        y = Console.ReadLine().Trim();
                        Console.WriteLine("Enter color data Blue: Ex: 1.0");
                        z = Console.ReadLine().Trim();
                        Console.WriteLine("Sending Request");
                        hsp.ChangeColorSL(x, y, z);
                        Console.WriteLine("Request Sent");
                        break;

                    case "shape":

                        break;
                    case "size":

                        break;


                    default:
                        break;
                }


              userInput = Console.ReadLine().ToString();
            } while ( !userInput.Equals("q") );

        }
    }
}
