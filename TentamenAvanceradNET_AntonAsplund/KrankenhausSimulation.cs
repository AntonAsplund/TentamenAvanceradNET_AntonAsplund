using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using TentamenAvanceradNET_AntonAsplund.Database;

namespace TentamenAvanceradNET_AntonAsplund
{
    /// <summary>
    /// Holds all required methods to run ONE simulation
    /// </summary>
    class KrankenhausSimulation
    {
        public bool SimulationOver { get; set; }
        public void RunSimulation()
        {
            var krankenhausMain = new KrankenhausMain();

            krankenhausMain.PatientsMoved += KrankenhausConsolePrinter.PrintPatientsMovedToConsole;
            krankenhausMain.PatientsMoved += KrankenhausFileLogger.LogPatientsMovedToConsole;

            krankenhausMain.SimulationOver += CancelSimulation;

            var threadOne = new Thread(krankenhausMain.ThreadOne);      //Adds 30 patients
            threadOne.Start();

            var threadFive = new Thread(krankenhausMain.ThreadFive);    //Adds 10 doctors
            threadFive.Start();

            Thread simulationPartOne = new Thread(() => PartOne(krankenhausMain));  //Starts threads which runs every 3 seconds
            simulationPartOne.Start();

            Thread simulationPartTwo = new Thread(() => PartTwo(krankenhausMain));  //Starts threads which runs every 5 seconds
            simulationPartTwo.Start();

            Thread runCheckIfSimulationIsOverLoop = new Thread(() => RunCheckIfSimulationIsOverLoop(krankenhausMain));//Starts a thread which continuously checks if simulation is over
            runCheckIfSimulationIsOverLoop.Start();
            
        }
        /// <summary>
        /// Logs the start of a new simulation in the detailed log text file
        /// </summary>
        /// <param name="simulationNumber"></param>
        internal void LogStartOfSimulation(int simulationNumber)
        {
            KrankenhausFileLogger.LogStartOfSimulationNumber(simulationNumber);
        }

        /// <summary>
        /// Calls the thread which checks if the simulation is over. If all patietns have been moved to afterlife or being discharged for hospital.
        /// </summary>
        /// <param name="krankenhausMain"></param>
        private void RunCheckIfSimulationIsOverLoop(KrankenhausMain krankenhausMain)
        {
            while (true)
            {
                Thread checkIfSimulationIsOver = new Thread(krankenhausMain.ThreadCheckIfSimulationOver);   //Starts a thread which checks if simulation is over
                checkIfSimulationIsOver.Start();
                if (SimulationOver)
                {
                    break;
                }

                Thread.Sleep(1000);
            }
        }

        /// <summary>
        /// Moves all records in the patient table to a patientHistory table
        /// </summary>
        /// <param name="simulationNumber"></param>
        internal void MovePatientInfoToHistory(int simulationNumber)
        {
            using (var db = new KrankenhausContext())
            {
                var patients = db.Patients.ToList();

                for (int i = 0; i < patients.Count; i++)
                {
                    var patientsHistory = KrankenhausHelpMethods.GetAPatientHistory(patients[i], simulationNumber);
                    db.Patients.Remove(patients[i]);
                    db.PatientHistories.Add(patientsHistory);
                    db.SaveChanges();
                }
            }
        }
        /// <summary>
        /// Removes all info from the database, making sure it is a "clean slate" for next simulation
        /// </summary>
        internal void RemoveAllCurrentInfoFromSimulator()
        {
            KrankenhausMain krankenhausMain = new KrankenhausMain();
            krankenhausMain.RemoveAllFromDatabase();
        }
        /// <summary>
        /// Method which gets fired by the simulation over event. Changes bool to true which cancels all running threads.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelSimulation(object sender, KrankenhausEndOfSimlationEventArgs e)
        {
            SimulationOver = e.SimulationOver;
        }
        /// <summary>
        /// Starts all threads running on a 5 second interval and checks if KrankenhausSimulation property "simulationOver" is true. Then the threads stops getting started.
        /// </summary>
        /// <param name="krankenhausMain"></param>
        private void PartTwo(KrankenhausMain krankenhausMain)
        {
            while (true)
            {
                var threadTwo = new Thread(krankenhausMain.ThreadTwo);
                threadTwo.Start();
                var threadFour = new Thread(krankenhausMain.ThreadFour);
                threadFour.Start();
                var threadSix = new Thread(krankenhausMain.ThreadSix);
                threadSix.Start();
                Thread.Sleep(500);

                if(SimulationOver)
                { 
                break;
                }
            }
        }
        /// <summary>
        /// Starts all threads running on a 3 second interval and checks if KrankenhausSimulation property "simulationOver" is true. Then the threads stops getting started.
        /// </summary>
        /// <param name="krankenhausMain"></param>
        private void PartOne(KrankenhausMain krankenhausMain)
        {
            while (true)
            {
                var threadThree = new Thread(krankenhausMain.ThreadThree);
                threadThree.Start();
                Thread.Sleep(300);

                if (SimulationOver)
                {
                    break;
                }
            }
        }
        /// <summary>
        /// Calls a method used to log information about all simulations the program has made during this run.
        /// </summary>
        internal void PrintAllSimulationsInfoToTextFile()
        {
            KrankenhausFileLogger.LogAllSimulationsInfoToFile();
        }

        internal void RenameDetailedLogFile()
        {
            KrankenhausFileLogger.RenameDetailedFile();
        }
    }
}
