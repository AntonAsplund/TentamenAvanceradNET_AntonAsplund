using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TentamenAvanceradNET_AntonAsplund.Database;

namespace TentamenAvanceradNET_AntonAsplund
{
    /// <summary>
    /// Holds a single bool value used to cancel a simulation
    /// </summary>
    class KrankenhausEndOfSimlationEventArgs : EventArgs
    {
        public KrankenhausEndOfSimlationEventArgs()
        {
            SimulationOver = true;
        }

        public bool SimulationOver { get; set; }
    }
}
