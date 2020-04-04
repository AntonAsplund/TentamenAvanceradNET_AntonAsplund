using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TentamenAvanceradNET_AntonAsplund.Database
{
    class PatientHistory
    {
        public int PatientHistoryID { get; set; }
        public string Name { get; set; }
        public double SSN { get; set; }
        public int Age { get; set; }
        public string Status { get; set; }
        public int SimulationNumber { get; set; }
        public DateTime ArrivalAtHospital { get; set; }
        public DateTime? AssingedToHopsitalBed { get; set; }
        public DateTime? SignedOut { get; set; }
    }
}
