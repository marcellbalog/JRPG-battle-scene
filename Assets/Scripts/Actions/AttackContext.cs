using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ChristmasBattle
{
	public class AttackContext
	{
		public BattleEntityAnimator battleEntityAnimator { get; set; }
		public Transform transform { get; set; }
		public Transform attackTarget { get; set; }
		public BattleEntity entityTarget { get; set; }
		public GameObject Bullet { get; set; }		
		public GameObject activeBullet { get; set; }
		public int damage { get; set; }
		public List<Enemy> enemyTargets { get; set; }
		public bool isAlly { get; set; }
		public Vector3 BasePosition { get; set; }
	}
}
