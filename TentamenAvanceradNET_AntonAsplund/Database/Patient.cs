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
    class Patient
    {
        public int PatientID { get; set; }
        public string Name { get; set; }
        public double SSN { get; set; }
        public int Age { get; set; }
        public int ConditionLevel { get; set; }
        public DateTime ArrivalAtHospital { get; set; }
        public DateTime? AssingedToHopsitalBed { get; set; }
        public DateTime? SignedOut { get; set; }

        public int? AfterLifeID { get; set; }
        [ForeignKey("AfterLifeID")]
        public virtual AfterLife Afterlife { get; set; }

        public int? DischargedID { get; set; }
        [ForeignKey("DischargedID")]
        public virtual Discharged Discharged { get; set; }

        public int? IntensiveCareUnitID { get; set; }
        public virtual IntensiveCareUnit IntensiveCareUnit { get; set; }

        public int? PatientQueueID { get; set; }
        public virtual PatientQueue PatientQueue { get; set; }

        public int? SanatoriumID { get; set; }
        public virtual Sanatorium Sanatorium { get; set; }
    }
}
