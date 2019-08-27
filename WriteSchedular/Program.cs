using WriteScheduler.Schedulers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WriteScheduler
{
    class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine("please select an implementation:");
            Console.WriteLine("1) Round Robin");
            Console.WriteLine("2) Write Scheduler");

            int selection = 0;
            Console.Write("Select option an: ");
            if(int.TryParse(Console.ReadLine(),out selection))
            {
               if(selection==1)
                {
                    Console.WriteLine("Starting round robin scheduler\n");
                    RoundRobinScheduler rr = new RoundRobinScheduler();
                    rr.doRoundRobin();

                }
                else if (selection == 2)
                {
                    Console.WriteLine("Starting write scheduler\n");
                  Schedulers.WriteScheduler  ws = new Schedulers.WriteScheduler();
                    ws.RunWriteScheduler();


                }
                else
                {
                    Console.WriteLine("Invalid input");
                }

            }
            else
            {
                Console.WriteLine("Invalid input");
            }
            
            Console.ReadLine();
        }
    }
}
