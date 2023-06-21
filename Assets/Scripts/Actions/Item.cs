using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using UnityEngine;

namespace ChristmasBattle
{
    [CreateAssetMenu]    
    public class Item : Action
    {
        [Header("Item stats")]
        public ItemEffect itemEffect;
        public int value;

        private List<BattleEntity> targets;

        public void SetTargeting()
        {

            Debug.Log("Select target(s) with item - " + name);
        }

        public override void UseItem(List<BattleEntity> Targets)
        {
            Debug.Log("Use item - " + name);
            targets = Targets;
            Inventory.S.DepleteItem(this);

            switch (itemEffect)
            {
                case ItemEffect.Heal:
                    Heal();
                    break;
                case ItemEffect.EnergyRestore:
                    EnergyRestore();
                    break;
                default:
                    break;
            }

            BattleManager.S.UpdateStats();
        }

        void Heal()
        {
            foreach (var t in targets)
            {
                t.Heal((float)value/100);
                MediaManager.PlayEffect(MediaManager.Effect.Heal, t.EntityObject.transform.position);
            }
        }

        void EnergyRestore() {
            foreach (var t in targets)
            {
                t.GainEnergy((float)value / 100);
                MediaManager.PlayEffect(MediaManager.Effect.Energy, t.EntityObject.transform.position);
            }
        }
    }
}
