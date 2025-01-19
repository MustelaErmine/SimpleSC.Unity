using SimpleSC.Server.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSC.Server.Effects
{
    internal class RegenerationEffect : EverystepEffect
    {
        public int healthIncrease;
        public override void Step(Unit unit)
        {
            base.Step(unit);
            unit.Health = Math.Min(unit.Health + healthIncrease, Unit.MaxHealth);
        }
        public RegenerationEffect(int time, int increse)
        {
            ActiveSteps = time;
            healthIncrease = increse;
            code = "regeneration";
        }
    }
}
