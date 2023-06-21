using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ChristmasBattle
{
    public abstract class BattleEntityObjectScript : MonoBehaviour
    {
        public BattleEntity thisEntity;

        public GameObject Marker;
        public Animator Anim;

        public bool isTargetable = false;

        private void Start()
        {
            Marker = transform.GetChild(0).gameObject;
            SetMarkerVisible(false);
            Anim = GetComponent<Animator>();
        }

        public void SetMarkerVisible(bool toVisible)
        {
            Marker.SetActive(toVisible);
        }

        public void SetTargetable(bool setTo)
        {
            SetMarkerVisible(setTo);
            isTargetable = setTo;
            if (!setTo)
                Anim.SetBool("hover", false);
        }

        public void AnimateMarker(bool on = true)
        {
            Debug.Log("marker animated");
            Anim.SetBool("hover", on);            
        }

        public void AnimateMarkers(List<BattleEntity> entities, bool on = true)
        {
            foreach (var e in entities)
            {
                e.entityScript.AnimateMarker(on);
            }
        }

        protected virtual void OnMouseEnter()
        {
            if (!isTargetable)
                return;
            print("mouse on entity");
            AnimateMarker();
            if (BattleManager.S.currentChar.selectedAction.multitarget)
                AnimateMarkers(BattleManager.S.currentChar.selectedAction.targetOptions);

            foreach (Transform item in BattleManager.S.QueuePanel.transform)
            {
                if (item.GetSiblingIndex() < 8 && item.name == thisEntity.ID)
                    item.transform.GetChild(1).gameObject.SetActive(true);
            }            
        }

        private void OnMouseExit()
        {
            if (!isTargetable)
                return;
            AnimateMarker(false);

            //ha multitarget az ability
            if (BattleManager.S.currentChar == null || BattleManager.S.currentChar.selectedAction == null)
                return;
            if (BattleManager.S.currentChar.selectedAction.multitarget)
                AnimateMarkers(BattleManager.S.currentChar.selectedAction.targetOptions, false);
            
            foreach (Transform q in BattleManager.S.QueuePanel.transform)
            {
                if (q.name == thisEntity.ID)
                    q.transform.GetChild(1).gameObject.SetActive(false);
            }
        }

        private void OnMouseDown()
        {
            if (!isTargetable)
                return;

            BattleManager.S.currentChar.selectedAction.UseOnTarget(this);
        }

        public abstract void PlayDamageAnimAndSound();

        public abstract void ReturnToIdle();
    }
}
