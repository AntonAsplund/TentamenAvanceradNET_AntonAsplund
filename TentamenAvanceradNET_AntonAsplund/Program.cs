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
        
        /// <summary>
        /// If the program is not run properly from finish to start, then the log files might not be created in the correct way and old data cleared. Resulting in incorrect log files during the next simulation.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            //Simulation loop time can be changed in "partOne" and "partTwo" in KrankenhausSimulation class
            //Number of patients in the simulation can be changed in the ThreadOne method in KrankenhausMain class
            //Have in mind that if the simulation is cancel or exited incorrectly, the log file might contain corrupt data during the next simulation run.

            KrankenhausSimulation krankenhausSimulation = new KrankenhausSimulation() { SimulationOver = false };

            for (int i = 0; i < 10; i++)
            {
                krankenhausSimulation.LogStartOfSimulation(i);

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
                    Console.WriteLine("The last simluation has completed. Please see textfiles in the program folder for more information. ");
                }


            }

            krankenhausSimulation.PrintAllSimulationsInfoToTextFile();
            krankenhausSimulation.RenameDetailedLogFile();

            RemoveEverythingFromAllTable(); // Deletes every record and makes sure there is a clean slate for the next simulation. 
                                            // Next simulations log file will not be created correctly if this piece of code is not run.

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
