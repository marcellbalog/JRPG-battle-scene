using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ChristmasBattle
{
    public abstract class Action : ScriptableObject
    {
        new public string name;
        [MultilineAttribute]
        public string description;
        public bool selfonly;
        public bool multitarget;
        public enum TargetGroup { enemy_only, ally_only, any }
        public TargetGroup targetGroup;

        public List<BattleEntity> targetOptions = new List<BattleEntity>();

        public void SetTargetMarkers()
        {
            FindTargets();

            foreach (var e in BattleManager.instance.BattleEntities)
            {
                e.entityScript.SetTargetable(false);
            }

            foreach (var t in targetOptions)
            {
                t.entityScript.SetTargetable(true);
            }
        }

        public void FindTargets()
        {
            List<BattleEntity> newTargetOptions = new List<BattleEntity>();

            if (selfonly)
            {
                newTargetOptions.Add(BattleManager.instance.currentChar);
            }
            else
            {
                switch (targetGroup)
                {
                    case TargetGroup.ally_only:
                        newTargetOptions.AddRange(BattleManager.instance.activeChars.ToList());
                        break;
                    case TargetGroup.enemy_only:
                        newTargetOptions.AddRange(BattleManager.instance.Enemies);
                        break;
                    case TargetGroup.any:
                        newTargetOptions.AddRange(BattleManager.instance.BattleEntities);
                        break;
                    default:
                        break;
                }
            }
            targetOptions = newTargetOptions;            
        }

        public void UseOnTarget(BattleEntityObjectScript target)
        {
            List<BattleEntity> targets = new List<BattleEntity>();            

            if (multitarget)
            {
                if (targetGroup == TargetGroup.enemy_only)
                {
                    targets.AddRange(BattleManager.instance.Enemies);
                }
                if (targetGroup == TargetGroup.ally_only)
                {
                    targets.AddRange(BattleManager.instance.activeChars);                    
                }
            } else
            {
                targets.Add(target.thisEntity);
            }
            

            if (BattleManager.instance.currentChar.selectedAction is Item)
                UseItem(targets);
            else if (BattleManager.instance.currentChar.selectedAction is Ability)
                BattleManager.instance.PrepareAttackEnemy(!multitarget ? target.gameObject : null);

            //^^^^ EZT LEHET HOGY A CHILD CLASSOKBA KÉNE SZEDNI, MERT AZ ITEM/ABILITY KÖZÖTT IS KÜLÖNBSÉGET KELL TENNI
        }

        public virtual void UseItem(List<BattleEntity> targets) { }
    }
}
