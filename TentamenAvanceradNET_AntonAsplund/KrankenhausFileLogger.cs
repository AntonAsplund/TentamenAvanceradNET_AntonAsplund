using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using TentamenAvanceradNET_AntonAsplund.Database;

namespace TentamenAvanceradNET_AntonAsplund
{
    /// <summary>
    /// Holds methods used to print information to log file
    /// </summary>
    class KrankenhausFileLogger
    {
        /// <summary>
        /// Logs the number of patients moved to and from a hospital ward to a textfile location in the program catalogue.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal static void LogPatientsMovedToConsole(object sender, KrankenhausMovedPatientsEventArgs e)
        {
            string path = $"DetailedLogOfSimulations.txt";

            using (StreamWriter streamWriter = new StreamWriter(path, true))
            {

                streamWriter.WriteLine($"----------------");
                streamWriter.WriteLine($"{e.LogTime.ToString("MM/dd/yyyy HH:mm:ss")}:");
                streamWriter.WriteLine($"----------------");
                streamWriter.WriteLine($"Number of patients admitted to ICU = { e.NumberOfPatientsFromQueueToICU + e.NumberOfPatientsFromSanatoriumToICU} whereof {e.NumberOfPatientsFromSanatoriumToICU} patient(s) from sanatorium and {e.NumberOfPatientsFromQueueToICU} patient(s) from the queue");
                streamWriter.WriteLine($"Number of patients admitted to Sanatorium from the queue =  {e.NumberOfPatientsFromQueueToSanatorium}");
                streamWriter.WriteLine($"Number of newly deceased patients =  {e.NumberOfDeceasedPatients}");
                streamWriter.WriteLine($"Number of newly recovered patients =  {e.NumberOfRecoveredPatients}");
            }
        }
        /// <summary>
        /// Writes to the detailed log file which simulation is being logged right now
        /// </summary>
        /// <param name="simulationNumber"></param>
        internal static void LogStartOfSimulationNumber(int simulationNumber)
        {
            string path = $"DetailedLogOfSimulations.txt";

            using (StreamWriter streamWriter = new StreamWriter(path, true))
            {
                streamWriter.WriteLine($"----------------");
                streamWriter.WriteLine($"----------------");
                streamWriter.WriteLine($"Start of simulation number: {simulationNumber}");
                streamWriter.WriteLine($"----------------");
                streamWriter.WriteLine($"----------------");

            }
        }

        /// <summary>
        /// Logs all client requested info from the simluations in a separate file.
        /// </summary>
        internal static void LogAllSimulationsInfoToFile()
        {
            using (var db = new KrankenhausContext())
            {
                string path = $"SummaryLogOfAllSimulations-{ DateTime.UtcNow.ToString("MM dd yyyy / HH/mm")}.txt";
                path = path.Replace(" ", "");

                using (StreamWriter streamWriter = new StreamWriter(path, true))
                {
                    for (int i = 0; i < 10; i++)
                    {
                        int simulationNumber = i + 1;
                        var patientInSimulation = db.PatientHistories.Where(P => P.SimulationNumber == simulationNumber).ToList();

                        streamWriter.WriteLine("---------------------");
                        streamWriter.WriteLine($"Simulation number {i+1}");
                        streamWriter.WriteLine("---------------------");
                        streamWriter.WriteLine($"Patients in simulation: {patientInSimulation.Count}");
                        streamWriter.WriteLine($"Number of patients recovered: {patientInSimulation.Where(P => P.Status == "Recovered").Count()}");
                        streamWriter.WriteLine($"Number of patients deceased: {patientInSimulation.Where(P => P.Status == "Deceased").Count()}");
                        streamWriter.WriteLine($"Number of patients deceased while in queue: {patientInSimulation.Where(P => P.Status == "Deceased" && P.AssingedToHopsitalBed == null).Count()}");

                        TimeSpan difference = new TimeSpan(0);
                        for (int j = 0; j < patientInSimulation.Count; j++)
                        {
                            if (patientInSimulation[i].AssingedToHopsitalBed != null)
                            {
                                DateTime nonNullableDateTime = (DateTime)patientInSimulation[i].AssingedToHopsitalBed;

                                TimeSpan averageTime = nonNullableDateTime - patientInSimulation[i].ArrivalAtHospital;
                                difference += averageTime;
                            }
                            else
                            {
                                DateTime nonNullableDateTime = (DateTime)patientInSimulation[i].SignedOut;

                                TimeSpan averageTime = nonNullableDateTime - patientInSimulation[i].ArrivalAtHospital;
                                difference += averageTime;
                            }
                        }

                        TimeSpan averageInQueue = new TimeSpan(difference.Ticks / patientInSimulation.Count);



                        streamWriter.WriteLine($"Average time for a patient in queue: {averageInQueue.Days} days, {averageInQueue.Hours} hr, {averageInQueue.Minutes} min, {averageInQueue.Seconds} sec, {averageInQueue.Milliseconds} milisec ");

                        var patientFirstIn = db.PatientHistories.OrderBy(P => P.PatientHistoryID).FirstOrDefault();
                        var lastPatientTreated = db.PatientHistories.OrderByDescending(P => P.SignedOut).FirstOrDefault();

                        TimeSpan treatmentTimeForAllPatients = (DateTime)patientFirstIn.SignedOut - (DateTime)patientFirstIn.ArrivalAtHospital;

                        streamWriter.WriteLine($"Total time for the handling of all patients: {treatmentTimeForAllPatients.Days} days, {treatmentTimeForAllPatients.Hours} hr, {treatmentTimeForAllPatients.Minutes} min, {treatmentTimeForAllPatients.Seconds} sec, {treatmentTimeForAllPatients.Milliseconds} milisec "); 
                    }

                    streamWriter.WriteLine("---------------------");
                    streamWriter.WriteLine("Detailed list of all patients result:");
                    streamWriter.WriteLine("---------------------");
                    var allPatientInSimulation = db.PatientHistories.ToList();

                    for (int i = 0; i < allPatientInSimulation.Count; i++)
                    {
                        streamWriter.WriteLine("---------------------");
                        streamWriter.WriteLine($"{allPatientInSimulation[i].Name}");
                        streamWriter.WriteLine($"Age: {allPatientInSimulation[i].Age}");
                        streamWriter.WriteLine($"Status: {allPatientInSimulation[i].Status}");
                        streamWriter.WriteLine("---------------------");
                    }
                }
            }
        }
        /// <summary>
        /// Renames the detailed log simulation file so next simulation will not continue to add data to the same text file
        /// </summary>
        internal static void RenameDetailedFile()
        {
            string path = $"DetailedLogOfAllSimulations-{ DateTime.UtcNow.ToString("MM dd yyyy / HH/mm")}.txt";
            path = path.Replace(" ", "");

            System.IO.File.Move("DetailedLogOfSimulations.txt", path);

        }
    }
}
