using SimpleSC.Server.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSC.Server.Effects
{
    internal class EverystepEffect : Effect
    {
        public override void StepProcess(Unit unit)
        {
            base.StepProcess(unit);
            if (activeStepsCurrent > 0)
                Step(unit);
        }
        public virtual void Step(Unit unit)
        {
        }
    }
}
