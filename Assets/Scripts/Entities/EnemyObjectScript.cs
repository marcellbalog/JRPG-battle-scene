using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChristmasBattle
{
	public class EnemyObjectScript : BattleEntityObjectScript
	{
		public Enemy thisEnemy;

		public GameObject EnemyImage;		
		public GameObject WhiteMask;
				

		public void SetImage(Enemy e)
		{
			thisEnemy = e;
			EnemyImage.GetComponent<SpriteRenderer>().sprite = e.Image;
			Vector2 S = Vector2.Scale(EnemyImage.GetComponent<SpriteRenderer>().sprite.bounds.size, EnemyImage.transform.localScale);
			GetComponent<BoxCollider2D>().size = S;
			GetComponent<SpriteMask>().sprite = e.Image;
		}		

		protected override void OnMouseEnter()
		{
			base.OnMouseEnter();	
		}

		public override void PlayDamageAnimAndSound()
		{
			GetComponent<Animator>().Play("EnemyDamage");
			MediaManager.PlaySound(MediaManager.Sound.Enemy_damage);
			print("enemy damaged");
		}

        public override void ReturnToIdle()
        {
            throw new System.NotImplementedException();
        }
    }
}