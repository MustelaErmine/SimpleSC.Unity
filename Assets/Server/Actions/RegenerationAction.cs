using SimpleSC.Server.Effects;
using SimpleSC.Server.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSC.Server.Actions
{
    internal class RegenerationAction : UnitAction
    {
        public int regen, time;
        public RegenerationAction(int cooldown, int regen, int time)
        {
            Cooldown = cooldown;
            this.regen = regen;
            this.time = time;
            code = "regeneration";
        }

        public override void Activate(Unit self, Unit enemy)
        {
            base.Activate(self, enemy);
            self.AddEffect(new RegenerationEffect(time, regen));
        }
    }
}
