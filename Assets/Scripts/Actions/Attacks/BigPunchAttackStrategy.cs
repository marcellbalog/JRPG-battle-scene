using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ChristmasBattle
{
	// Variation of the punch attack 
	public class BigPunchAttackStrategy : MonoBehaviour, IAttackStrategy
	{
		public void ExecuteAttack(AttackContext context)
		{
			float punchDuration = 1.0f;

			transform.DOMove(context.attackTarget.position, punchDuration)
				.SetEase(Ease.Linear)
				.OnComplete(() =>
				{
					// Callback when the punch is complete
					transform.position = context.BasePosition;
					context.battleEntityAnimator.ShowDamageNumbers();
					MediaManager.PlayEffect(MediaManager.Effect.RudolfBigPunch, context.attackTarget.position);
					BattleManager.instance.EndEntityTurn();
					context.entityTarget.entityScript.PlayDamageAnimAndSound();
					context.entityTarget.LoseHealth(context.damage);
					BattleManager.instance.currentEntity.entityScript.ReturnToIdle();
				});

		}
	}

}
