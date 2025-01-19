using SimpleSC.Server.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSC.Server.Actions
{
    public class UnitAction
    {
        public int Cooldown { get; set; }
        public int currentCooldown;
        public string code = "null";

        public virtual void Activate(Unit self, Unit enemy)
        {
            currentCooldown = Cooldown;
        }
        public void Step()
        {
            currentCooldown--;
        }
    }
}
