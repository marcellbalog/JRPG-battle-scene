using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace ChristmasBattle
{
	public class BattleEntityAnimator : MonoBehaviour
	{
		private IAttackStrategy attackStrategy;

		Vector3 BasePosition;
		bool isAlly = true;
		Enemy enemyTarget;
		List<Enemy> enemyTargets;
		Ally allyTarget;
		List<Ally> allyTargets = new List<Ally>();
		BattleEntity entityTarget;
		List<BattleEntity> entityTargets = new List<BattleEntity>();

		Transform attackTarget;
		List<Transform> attackTargets = new List<Transform>();

		AttackType activeAttack;
		
		public GameObject Bullet;
		GameObject activeBullet;
		
		int damage;

		public GameObject TextHolder;
		public GameObject DamageText;

		Vector3 SwitchPoint;
		bool isSwitching;

		List<GameObject> particleEffects = new List<GameObject>();

		private void Start()
		{
			isAlly = CompareTag("Ally");
			TextHolder = BattleManager.instance.TextHolder;
			DamageText = BattleManager.instance.DamageText;
		}

		private void Update()
		{
			if (isSwitching)
			{
				transform.position = Vector3.MoveTowards(transform.position, SwitchPoint, 20 * Time.deltaTime);
			}
		}

		public void Attack(AttackType at, int pow, float castTime = 0)
		{
			BasePosition = transform.position;
			activeAttack = at;
			damage = pow;
			if (isAlly)
			{
				transform.GetChild(3).gameObject.SetActive(true);
			}

			UnityAction delayedAttack = ExecuteAttack;
			Invoke(delayedAttack.Method.Name, castTime);				
		}

		private void ExecuteAttack()
		{
			print("attack invoke called");

			var attackContext = new AttackContext
			{
				battleEntityAnimator = this,
				transform = transform,
				attackTarget = attackTarget,
				entityTarget = entityTarget,
				Bullet = Bullet,
				activeBullet = activeBullet,
				damage = damage,
				enemyTargets = enemyTargets,
				isAlly = isAlly,
				BasePosition = BasePosition,

			};

			switch (activeAttack)
			{
				case AttackType.Punch:
					attackStrategy = new PunchAttackStrategy();
					break;
				case AttackType.Shoot:					
					attackStrategy = new ShootAttackStrategy();									
					break;
				case AttackType.Magic:
					attackStrategy = new MagicAttackStrategy();
					break;
				case AttackType.Slowhit:
					attackStrategy = new SlowHitAttackStrategy();
					break;
				case AttackType.ChainShoot:
					attackStrategy = new ChainShootAttackStrategy();
					break;
				case AttackType.BigPunch:
					attackStrategy = new BigPunchAttackStrategy();
					break;
			}

			attackStrategy.ExecuteAttack(attackContext);
		}


		public void GetEnemyTarget(Enemy e)
		{
			enemyTarget = e;
			attackTarget = e.EnemyObject.transform;
			entityTarget = e;
		}

		public void GetEnemyTarget(List<Enemy> e)
		{
			enemyTargets = new List<Enemy>(e);
			enemyTargets.RemoveAll(x => x.isDead);
			entityTargets = e.ConvertAll(x => (BattleEntity)x); ;
		}

		public void GetAllyTarget(Ally a)
		{
			allyTarget = a;
			attackTarget = a.EntityObject.transform;
			entityTarget = a;
		}

		public void GetAllyTarget(List<Ally> a)
		{
			allyTargets = a;
			entityTargets = a.ConvertAll(x => (BattleEntity)x);
		}

		public void GetEntityTarget(BattleEntity e)
        {			
        }

		public void GetEntityTargets(List<BattleEntity> e)
        {
        }


		public void ShowDamageNumbers()
		{

			var t = Instantiate(DamageText, TextHolder.transform);
			t.GetComponent<RectTransform>().anchoredPosition = attackTarget.position;
			t.transform.GetChild(0).GetComponent<TextMeshPro>().text = damage + "";
			Destroy(t, 2);
		}


		public void Switch(bool isOut, Vector3 point)
		{
			isSwitching = true;
			if (isOut)
			{
				SwitchPoint = new Vector3(transform.position.x - 10, transform.position.y, transform.position.z);
			}
			else
			{
				SwitchPoint = point;
				transform.position = new Vector3(SwitchPoint.x - 10, SwitchPoint.y, SwitchPoint.z);
			}
		}
	}
}