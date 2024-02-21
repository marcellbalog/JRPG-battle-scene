using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ChristmasBattle
{
	// Shoots at multiple targets in a single attack
	public class ChainShootAttackStrategy : MonoBehaviour, IAttackStrategy
	{
		public void ExecuteAttack(AttackContext context)
		{
			Sequence chainSequence = DOTween.Sequence();

			foreach (Enemy enemy in context.enemyTargets)
			{
				Transform enemyTransform = enemy.EnemyObject.transform;

				chainSequence.AppendCallback(() =>
				{
					context.attackTarget = enemyTransform;

					Vector3 shootPosition = transform.position;
					if (context.isAlly) shootPosition = BattleManager.instance.currentChar.FindAttackPoint();

					context.activeBullet = Instantiate(context.Bullet, transform);
					context.activeBullet.transform.position = shootPosition;
					MediaManager.PlaySound(MediaManager.Sound.Snowman_A1);
					MediaManager.PlayEffect(MediaManager.Effect.Shot, shootPosition);
					context.activeBullet.transform.right = enemyTransform.position - shootPosition;

				}).AppendCallback(() =>
				{
					// Ensure activeBullet is not null before attempting to use it
					if (context.activeBullet != null)
					{
						chainSequence.Append(context.activeBullet.transform.DOMove(enemyTransform.position, 1f / 50f * Vector3.Distance(context.activeBullet.transform.position, enemyTransform.position))
							.SetEase(Ease.Linear)
							.OnComplete(() =>
							{
								Destroy(context.activeBullet.gameObject);
								context.battleEntityAnimator.ShowDamageNumbers();
								MediaManager.PlayEffect(MediaManager.Effect.BasicDamage, enemyTransform.position);
								enemy.LoseHealth(context.damage);
								enemy.entityScript.PlayDamageAnimAndSound();
							}));
					}
				});
			}

			chainSequence.AppendCallback(() =>
			{
				BattleManager.instance.EndEntityTurn();
				BattleManager.instance.currentEntity.entityScript.ReturnToIdle();
			});
		}

	}

}
