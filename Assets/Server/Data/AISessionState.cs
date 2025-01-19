using SimpleSC.Server.Actions;
using SimpleSC.Server.Effects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSC.Server.Data
{
    public class AISessionState
    {
        public int health, enemyHealth;
        public Effect[] effects;
        public UnitAction[] actions;
    }
}
