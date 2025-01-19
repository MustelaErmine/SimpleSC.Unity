using SimpleSC.Server.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSC.Server.Actions
{
    internal class Attack : UnitAction
    {
        public int damage;
        public Attack(int cooldown, int damage)
        { 
            Cooldown = cooldown;
            this.damage = damage;
            code = "attack";
        }

        public override void Activate(Unit self, Unit enemy)
        {
            base.Activate(self, enemy);
            enemy.ProcessDamage(damage);
        }
    }
}
