using SimpleSC.Server.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSC.Server.Effects
{
    internal class Burning : EverystepEffect
    {
        public int healthDecrease;
        public override void Step(Unit unit)
        {
            base.Step(unit);
            unit.Health -= healthDecrease;
        }
        public Burning(int time, int decrese)
        {
            ActiveSteps = time;
            healthDecrease = decrese;
            code = "burning";
        }
    }
}
