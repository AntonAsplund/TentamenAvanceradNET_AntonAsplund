using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TentamenAvanceradNET_AntonAsplund.Database;
using System.Threading;

namespace TentamenAvanceradNET_AntonAsplund
{
    class Program
    {

        static void Main(string[] args)
        {
            KrankenhausSimulation krankenhausSimulation = new KrankenhausSimulation() { SimulationOver = false };

            for (int i = 0; i < 10; i++)
            {
                krankenhausSimulation.RunSimulation();

                while (krankenhausSimulation.SimulationOver == false)
                {
                    Thread.Sleep(1000);
                }

                krankenhausSimulation.MovePatientInfoToHistory(i + 1);
                krankenhausSimulation.RemoveAllCurrentInfoFromSimulator();

                Console.WriteLine($"Simulation number {i + 1} has completed.");

                if (i < 9)
                {
                    Console.WriteLine("Press any key to continue to next simulation");
                    krankenhausSimulation.SimulationOver = false;
                    Console.ReadKey();
                }
                else
                {
                    Console.WriteLine("The last simluation has completed. Please see \"LogOfAllSimulations{Date & Time}\" textfile in the program folder. ");
                }


            }

            krankenhausSimulation.PrintAllSimulationsInfoToTextFile();

            Console.WriteLine("Press any key to exit program...");
            Console.ReadKey();


        }

        /// <summary>
        /// **ONLY FOR USE BY LEAD DEVELOPER or Paul T ** Removes all records from every table in the database
        /// </summary>
        public static void RemoveEverythingFromAllTable()
        {
            var kranken = new KrankenhausMain();
            kranken.RemoveAllFromDatabase();

            using (var db = new KrankenhausContext())
            {

                var patientHistory = db.PatientHistories.ToList();
                for (int i = 0; i < patientHistory.Count; i++)
                { 
                    db.PatientHistories.Remove(patientHistory[i]);
                    db.SaveChanges();
                }

            }
        }
    }
}
