using SimpleSC.Server.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSC.Server.Units
{
    internal class AIEnemy : Unit
    {
        public Unit player;
        public override void NotifyStep()
        {
            Step();
            UnitAction action;
            do
            {
                action = PossibleActions[new Random().Next(PossibleActions.Count)];
            } while (action.currentCooldown > 0);
            action.Activate(this, player);
            SessionServer.NextStep();
        }
    }
}
