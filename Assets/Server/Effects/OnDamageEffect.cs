using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSC.Server.Effects
{
    internal class OnDamageEffect : Effect
    {
        public virtual int ProcessDamage(int damage)
        {
            return damage;
        }
    }
}
