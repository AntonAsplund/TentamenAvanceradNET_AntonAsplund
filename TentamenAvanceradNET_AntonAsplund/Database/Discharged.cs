using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;


namespace TentamenAvanceradNET_AntonAsplund.Database
{
    class Discharged
    {
        public Discharged()
        {

            this.Patients = new List<Patient>();
        }

        public int DischargedID { get; set; }
        public virtual List<Patient> Patients { get; set; }
    }
}
