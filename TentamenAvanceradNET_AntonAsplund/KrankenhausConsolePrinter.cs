using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TentamenAvanceradNET_AntonAsplund.Database;

namespace TentamenAvanceradNET_AntonAsplund
{
    /// <summary>
    /// Holds methods used to print information to console
    /// </summary>
    class KrankenhausConsolePrinter
    {
        /// <summary>
        /// Prints the number of patients moved to and from a hospital ward to console window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal static void PrintPatientsMovedToConsole(object sender, KrankenhausMovedPatientsEventArgs e)
        {
            Console.WriteLine($"----------------");
            Console.WriteLine($"{e.LogTime.ToString("MM/dd/yyyy HH:mm:ss")}:");
            Console.WriteLine($"----------------");
            Console.WriteLine($"Number of patients admitted to ICU = { e.NumberOfPatientsFromQueueToICU + e.NumberOfPatientsFromSanatoriumToICU} whereof {e.NumberOfPatientsFromSanatoriumToICU} patient(s) from sanatorium and {e.NumberOfPatientsFromQueueToICU} patient(s) from the queue");
            Console.WriteLine($"Number of patients admitted to Sanatorium from the queue =  {e.NumberOfPatientsFromQueueToSanatorium}");
            Console.WriteLine($"Number of newly deceased patients =  {e.NumberOfDeceasedPatients}");
            Console.WriteLine($"Number of newly recovered patients =  {e.NumberOfRecoveredPatients}");
        }
    }
}
