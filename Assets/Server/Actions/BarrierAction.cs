using SimpleSC.Server.Effects;
using SimpleSC.Server.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSC.Server.Actions
{
    internal class BarrierAction : UnitAction
    {
        public int barrier, time;
        public BarrierAction(int cooldown, int barrier, int time)
        {
            Cooldown = cooldown;
            this.barrier = barrier;
            this.time = time;
            code = "barrier";
        }
        public override void Activate(Unit self, Unit enemy)
        {
            base.Activate(self, enemy);
            self.AddEffect(new BarrierEffect(barrier, time));
        }
    }
}
