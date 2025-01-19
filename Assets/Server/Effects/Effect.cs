using SimpleSC.Server.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSC.Server.Effects
{
    public class Effect
    {
        public string code;
        public void Activate()
        {
            activeStepsCurrent = ActiveSteps;
        }
        public int ActiveSteps { get; set; }
        public int activeStepsCurrent;
        public virtual void StepProcess(Unit unit)
        {
            activeStepsCurrent--;
        }
    }
}
