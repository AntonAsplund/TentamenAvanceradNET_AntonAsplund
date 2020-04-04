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
    class IntensiveCareUnit
    {

        public int IntensiveCareUnitID { get; set; }
        public bool AvailableBed { get; set; }
        public virtual ICollection<Patient> Patient { get; set; }
        public int? DoctorID { get; set; }
        public virtual Doctor Doctor { get; set; }
    }
}
