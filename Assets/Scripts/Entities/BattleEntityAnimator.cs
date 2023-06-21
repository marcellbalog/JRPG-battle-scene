using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace ChristmasBattle
{
	public class BattleEntityAnimator : MonoBehaviour
	{
		Vector3 BasePosition;
		bool isAlly = true;
		Enemy enemyTarget;
		List<Enemy> enemyTargets;
		Ally allyTarget;
		List<Ally> allyTargets = new List<Ally>();
		BattleEntity entityTarget;
		List<BattleEntity> entityTargets = new List<BattleEntity>();

		bool isAttack = false;
		bool attackEnded = false; //true, amikor vége az attacknak, hagy idõt, hogy lemenjen egy cooldown utána
		Transform attackTarget;
		List<Transform> attackTargets = new List<Transform>();

		//public enum AttackType { punch, shoot, hack, slash, chainShoot, bigPunch }
		AttackType activeAttack;
		
		public GameObject Bullet;
		GameObject activeBullet;

		float timer;
		int damage;

		public GameObject TextHolder;
		public GameObject DamageText;

		Vector3 SwitchPoint;
		bool isSwitching;

		List<GameObject> particleEffects = new List<GameObject>();

		private void Start()
		{
			isAlly = CompareTag("Ally");
			TextHolder = BattleManager.S.TextHolder;
			DamageText = BattleManager.S.DamageText;
		}

		private void Update()
		{

			if (timer > 0)
				timer -= Time.deltaTime;

			if (isAttack)
			{
				if (timer > 0)
					return;

				switch (activeAttack)
				{
					case AttackType.Punch:
						transform.position = Vector3.MoveTowards(transform.position, attackTarget.position, 60 * Time.deltaTime);
						if (transform.position == attackTarget.position)
						{
							isAttack = false;
							transform.position = BasePosition;
							ShowDamageNumbers();
							MediaManager.PlayEffect(MediaManager.Effect.BasicDamage, attackTarget.position);

							attackEnded = true;
							timer = 1;
							entityTarget.entityScript.PlayDamageAnimAndSound();
							entityTarget.LoseHealth(damage);
							BattleManager.S.currentEntity.entityScript.ReturnToIdle();
							
						}
						break;
					case AttackType.Shoot:

						if (activeBullet == null)
						{
							Vector3 shootPostition = transform.position;
							if (isAlly) shootPostition = BattleManager.S.currentChar.FindAttackPoint();


							activeBullet = Instantiate(Bullet, transform);
							activeBullet.transform.position = shootPostition;
							MediaManager.PlaySound(MediaManager.Sound.Snowman_A1);
							MediaManager.PlayEffect(MediaManager.Effect.Shot, shootPostition);
							activeBullet.transform.right = attackTarget.transform.position - shootPostition;
						}

						activeBullet.transform.position = Vector3.MoveTowards(activeBullet.transform.position, attackTarget.position, 50 * Time.deltaTime);

						if (activeBullet.transform.position == attackTarget.position)
						{
							isAttack = false;
							Destroy(activeBullet.gameObject);
							ShowDamageNumbers();
							MediaManager.PlayEffect(MediaManager.Effect.BasicDamage, attackTarget.position);

							attackEnded = true;
							timer = 1;
							entityTarget.entityScript.PlayDamageAnimAndSound();
							entityTarget.LoseHealth(damage);
							BattleManager.S.currentEntity.entityScript.ReturnToIdle();
							
						}
						break;
					case AttackType.Magic:

						isAttack = false;
						ShowDamageNumbers();

						attackEnded = true;
						timer = 1;
						entityTarget.entityScript.PlayDamageAnimAndSound();
						entityTarget.LoseHealth(damage);
						BattleManager.S.currentEntity.entityScript.ReturnToIdle();

						break;
					case AttackType.Slowhit:
						transform.position = Vector3.MoveTowards(transform.position, attackTarget.position, 30 * Time.deltaTime);
						if (transform.position == attackTarget.position)
						{
							isAttack = false;
							transform.position = BasePosition;
							ShowDamageNumbers();
							MediaManager.PlayEffect(MediaManager.Effect.BasicDamage, attackTarget.position);

							attackEnded = true;
							timer = 1;
							entityTarget.entityScript.PlayDamageAnimAndSound();
							entityTarget.LoseHealth(damage);
							BattleManager.S.currentEntity.entityScript.ReturnToIdle();
							
						}
						break;
					case AttackType.ChainShoot:

						attackTarget = enemyTargets[0].enemyObject.transform;

						if (activeBullet == null)
						{
							Vector3 shootPostition = transform.position;
							if (isAlly) shootPostition = BattleManager.S.currentChar.FindAttackPoint();

							activeBullet = Instantiate(Bullet, transform);
							activeBullet.transform.position = shootPostition;
							MediaManager.PlaySound(MediaManager.Sound.Snowman_A1);
							MediaManager.PlayEffect(MediaManager.Effect.Shot, shootPostition);
							activeBullet.transform.right = attackTarget.transform.position - shootPostition;
						}

						activeBullet.transform.position = Vector3.MoveTowards(activeBullet.transform.position, attackTarget.position, 50 * Time.deltaTime);

						if (activeBullet.transform.position == attackTarget.position)
						{
							Destroy(activeBullet.gameObject);
							ShowDamageNumbers();
							MediaManager.PlayEffect(MediaManager.Effect.BasicDamage, attackTarget.position);
							enemyTargets[0].LoseHealth(damage);
							enemyTargets[0].entityScript.PlayDamageAnimAndSound();


							if (enemyTargets.Count > 1)
							{
								enemyTargets.RemoveAt(0);
								attackTarget = enemyTargets[0].enemyObject.transform;
							}
							else
							{
								isAttack = false;
								attackEnded = true;
								timer = 1;

								BattleManager.S.currentEntity.entityScript.ReturnToIdle();
							}
						}
						break;
					case AttackType.BigPunch:
						transform.position = Vector3.MoveTowards(transform.position, attackTarget.position, 60 * Time.deltaTime);
						if (transform.position == attackTarget.position)
						{
							isAttack = false;
							transform.position = BasePosition;
							ShowDamageNumbers();
							MediaManager.PlayEffect(MediaManager.Effect.RudolfBigPunch, attackTarget.position);
							//MediaManager.PlayEffect(MediaManager.Effect.BasicDamage, attackTarget.position);

							attackEnded = true;
							timer = 1;
							entityTarget.entityScript.PlayDamageAnimAndSound();
							entityTarget.LoseHealth(damage);
							BattleManager.S.currentEntity.entityScript.ReturnToIdle();
							
						}
						break;
				}
			}

			if (isSwitching)
			{
				transform.position = Vector3.MoveTowards(transform.position, SwitchPoint, 20 * Time.deltaTime);
			}

			if (attackEnded)
			{
				//if (timer > 0)
				//	return;
				BattleManager.S.EndEntityTurn();
				attackEnded = false;
			}
		}

		public void Attack(AttackType at, int pow, float castTime = 0)
		{
			BasePosition = transform.position;
			activeAttack = at;
			isAttack = true;
			damage = pow;
			if (isAlly)
			{
				transform.GetChild(3).gameObject.SetActive(true);
			}

			if (castTime != 0)
				timer = castTime;

			print(at.ToString());

			switch (activeAttack)
			{
				case AttackType.Punch:
					MediaManager.PlaySound(MediaManager.Sound.Rudolf_A1);
					break;
				case AttackType.Shoot:
					break;
				case AttackType.Magic:
					MediaManager.PlaySound(MediaManager.Sound.Santa_A1);
					MediaManager.PlayEffect(MediaManager.Effect.Magic, transform.position);
					break;
				case AttackType.Slowhit:
					MediaManager.PlaySound(MediaManager.Sound.Elf_A1);
					break;
			}
		}

		public void GetEnemyTarget(Enemy e)
		{
			enemyTarget = e;
			attackTarget = e.enemyObject.transform;
			entityTarget = e;
		}

		public void GetEnemyTarget(List<Enemy> e)
		{
			enemyTargets = new List<Enemy>(e);
			print("targets: " + enemyTargets.Count);
			enemyTargets.RemoveAll(x => x.isDead);
			print("targets: " + enemyTargets.Count);
			//attackTarget = e.enemyObject.transform;			
			entityTargets = e.ConvertAll(x => (BattleEntity)x); ;
		}

		public void GetAllyTarget(Ally c)
		{
			allyTarget = c;
			attackTarget = c.EntityObject.transform;
			entityTarget = c;
		}

		public void GetAllyTarget(List<Ally> c)
		{
			allyTargets = c;
			//attackTargets = c.EntityObject.transform;
			entityTargets = c.ConvertAll(x => (BattleEntity)x);
		}

		public void GetEntityTarget(BattleEntity e)
        {			
        }

		public void GetEntityTargets(List<BattleEntity> e)
        {
        }


		void ShowDamageNumbers()
		{

			var t = Instantiate(DamageText, TextHolder.transform);
			t.GetComponent<RectTransform>().anchoredPosition = attackTarget.position;
			print(t.GetComponent<RectTransform>().anchoredPosition + " " + attackTarget.position);
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