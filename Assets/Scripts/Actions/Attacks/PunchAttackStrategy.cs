using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ChristmasBattle
{
	public class PunchAttackStrategy : MonoBehaviour, IAttackStrategy
	{
		public void ExecuteAttack(AttackContext context)
		{
			transform.DOMove(context.attackTarget.position, 1f / 60f * Vector3.Distance(transform.position, context.attackTarget.position))
						.SetEase(Ease.Linear)
						.OnComplete(() =>
						{
							transform.position = context.BasePosition;
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
