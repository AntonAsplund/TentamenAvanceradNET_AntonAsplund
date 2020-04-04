using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TentamenAvanceradNET_AntonAsplund
{
    /// <summary>
    /// EventArgs which holds information of how many patients are moved from each hospital ward and to where.
    /// </summary>
    class KrankenhausMovedPatientsEventArgs : EventArgs
    {
        public KrankenhausMovedPatientsEventArgs(int numberOfPatientsFromSanatoriumToICU = 0, int numberOfPatientsFromQueueToICU = 0, int numberOfPatientsFromQueueToSanatorium = 0, int numberOfDeceasedPatients = 0, int numberOfRecoveredPatients = 0)
        {
            NumberOfPatientsFromSanatoriumToICU = numberOfPatientsFromSanatoriumToICU;
            NumberOfPatientsFromQueueToICU = numberOfPatientsFromQueueToICU;
            NumberOfPatientsFromQueueToSanatorium = numberOfPatientsFromQueueToSanatorium;
            NumberOfDeceasedPatients = numberOfDeceasedPatients;
            NumberOfRecoveredPatients = numberOfRecoveredPatients;
            LogTime = DateTime.UtcNow;
        }
        public DateTime LogTime { get; set; }
        public int NumberOfPatientsFromSanatoriumToICU { get; set; }
        public int NumberOfPatientsFromQueueToICU { get; set; }
        public int NumberOfPatientsFromQueueToSanatorium { get; set; }

        public int NumberOfDeceasedPatients { get; set; }
        public int NumberOfRecoveredPatients { get; set; }
    }
}
