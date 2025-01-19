using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSC.Server.Units
{
    internal class PlayerUnit : Unit
    {
        public override void NotifyStep()
        {
            Step();
            SessionServer.Socket.Emit("your_step", null);
        }
    }
}
