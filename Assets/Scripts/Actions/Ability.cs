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
    public class Ability : Action
	{
		[Header("Ability stats")]
		public BattleEntity.AbilityOwner owner;		
		public int energy;
		public int power;		

		public float castTime;		
		public AttackType attackType;
	}
}
