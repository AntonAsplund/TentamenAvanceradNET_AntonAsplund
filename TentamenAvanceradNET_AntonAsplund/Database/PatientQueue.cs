using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TentamenAvanceradNET_AntonAsplund.Database
{
    class PatientQueue
    {

        public int PatientQueueID { get; set; }

        public ICollection<Patient> Patient { get; set; }
    }
}
