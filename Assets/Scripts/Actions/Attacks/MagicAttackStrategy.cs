using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ChristmasBattle
{
	// Magic attack from a distance
	public class MagicAttackStrategy : MonoBehaviour, IAttackStrategy
	{
		public void ExecuteAttack(AttackContext context)
		{
			MediaManager.PlaySound(MediaManager.Sound.Santa_A1);
			MediaManager.PlayEffect(MediaManager.Effect.Magic, transform.position);
			context.battleEntityAnimator.ShowDamageNumbers();

			BattleManager.instance.EndEntityTurn();
			context.entityTarget.entityScript.PlayDamageAnimAndSound();
			context.entityTarget.LoseHealth(context.damage);
			BattleManager.instance.currentEntity.entityScript.ReturnToIdle();

		}
	}

}
