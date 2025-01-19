using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSC.Server.Effects
{
    internal class BarrierEffect : OnDamageEffect
    {
        public int blockDamage;
        public override int ProcessDamage(int damage)
        {
            return Math.Max(0, damage - blockDamage);
        }
        public BarrierEffect(int blockDamage, int activeSteps)
        {
            ActiveSteps = activeSteps;
            this.blockDamage = blockDamage;
            code = "barrier";
        }
    }
}
