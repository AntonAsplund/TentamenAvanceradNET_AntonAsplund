using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TentamenAvanceradNET_AntonAsplund.Database
{
    class Doctor
    {
        public int DoctorID { get; set; }
        public string Name { get; set; }
        public int SkillLevel { get; set; }
        public int NumberOfRotationsLeft { get; set; }
        public bool assignedToICU { get; set; }
        public bool assignedToSantorium { get; set; }

        public ICollection<Sanatorium> Sanatoriums { get; set; }

        public ICollection<IntensiveCareUnit> intensiveCareUnits { get; set; }
    }
}
