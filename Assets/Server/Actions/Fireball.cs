using SimpleSC.Server.Effects;
using SimpleSC.Server.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSC.Server.Actions
{
    internal class Fireball : UnitAction
    {
        public int attack, time, selfAttack;
        public Fireball(int cooldown, int attack, int time, int selfAttack)
        {
            Cooldown = cooldown;
            this.attack = attack;
            this.time = time;
            this.selfAttack = selfAttack;
            code = "fireball";
        }

        public override void Activate(Unit self, Unit enemy)
        {
            base.Activate(self, enemy);

            enemy.ProcessDamage(selfAttack);
            enemy.AddEffect(new Burning(time, attack));
        }
    }
}
