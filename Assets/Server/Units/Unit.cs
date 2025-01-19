using SimpleSC.Server.Actions;
using SimpleSC.Server.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleSC.Server.Effects;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace SimpleSC.Server.Units
{
    public class Unit
    {
        public const int MaxHealth = 50;
        public int Health { get; set; } = MaxHealth;
        public List<UnitAction> PossibleActions { get; set; } = new List<UnitAction>()
        {
            new Attack(0, 8),
            new BarrierAction(4,5,2),
            new Clearence(5),
            new Fireball(6,1,5,5),
            new RegenerationAction(5,2,3),
        };
        public SessionServer SessionServer { get; set; }
        public List<Effect> Effects { get; set; } = new List<Effect>();

        public void ProcessDamage(int damage)
        {
            foreach(var effect in Effects)
            {
                if (effect is OnDamageEffect && effect.activeStepsCurrent > 0)
                {
                    damage = ((OnDamageEffect)effect).ProcessDamage(damage);
                }
            }
            Health -= damage;
        }
        public void Step()
        {
            foreach(UnitAction unitAction in  PossibleActions)
            {
                unitAction.Step();
            }
            foreach(Effect effect in Effects)
            {
                effect.StepProcess(this);
            }
        }
        public void AddEffect(Effect effect)
        {
            Effects.Add(effect);
            effect.Activate();
        }
        public void ActivateAction(string code, Unit enemy)
        {
            foreach (UnitAction action in PossibleActions)
            {
                if (action.code == code)
                {
                    action.Activate(this, enemy);
                    return;
                }
            }
            Debug.Log("Invalid actions");
        }
        public virtual void NotifyStep()
        {
            Step();
        }
    }
}
