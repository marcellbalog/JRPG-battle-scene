using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ChristmasBattle
{
	// Shoots a projectile at the target
	public class ShootAttackStrategy : MonoBehaviour, IAttackStrategy
	{
		public void ExecuteAttack(AttackContext context)
		{
			if (context.activeBullet == null)
			{
				Vector3 shootPosition = context.transform.position;
				if (context.isAlly) shootPosition = BattleManager.instance.currentChar.FindAttackPoint();

				context.activeBullet = Instantiate(context.Bullet, transform);
				context.activeBullet.transform.position = shootPosition;
				MediaManager.PlaySound(MediaManager.Sound.Snowman_A1);
				MediaManager.PlayEffect(MediaManager.Effect.Shot, shootPosition);
				context.activeBullet.transform.right = context.attackTarget.transform.position - shootPosition;

				context.activeBullet.transform.DOMove(context.attackTarget.position, 1f / 50f * Vector3.Distance(context.activeBullet.transform.position, context.attackTarget.position))
					.SetEase(Ease.Linear)
					.OnComplete(() =>
					{
						Destroy(context.activeBullet.gameObject);
						context.battleEntityAnimator.ShowDamageNumbers();
						MediaManager.PlayEffect(MediaManager.Effect.BasicDamage, context.attackTarget.position);

						BattleManager.instance.EndEntityTurn();
						context.entityTarget.entityScript.PlayDamageAnimAndSound();
						context.entityTarget.LoseHealth(context.damage);
						BattleManager.instance.currentEntity.entityScript.ReturnToIdle();
					});
			}

		}
	}

}
