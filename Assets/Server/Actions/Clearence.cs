using SimpleSC.Server.Effects;
using SimpleSC.Server.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSC.Server.Actions
{
    internal class Clearence : UnitAction
    {
        public Clearence(int cooldown)
        {
            Cooldown = cooldown;
            code = "clearence";
        }
        public override void Activate(Unit self, Unit enemy)
        {
            base.Activate(self, enemy);
            self.Effects.RemoveAll((Effect effect) => effect.code == "burning");
        }
    }
}
