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

        private bool _isTargetable = false;

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
            _isTargetable = setTo;
            if (!setTo)
                Anim.SetBool("hover", false);
        }

        public void AnimateMarker(bool on = true)
        {
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
            if (!_isTargetable)
                return;
            AnimateMarker();
            if (BattleManager.instance.currentChar.selectedAction.multitarget)
                AnimateMarkers(BattleManager.instance.currentChar.selectedAction.targetOptions);

            foreach (Transform item in BattleManager.instance.QueuePanel.transform)
            {
                if (item.GetSiblingIndex() < 8 && item.name == thisEntity.ID)
                    item.transform.GetChild(1).gameObject.SetActive(true);
            }            
        }

        private void OnMouseExit()
        {
            if (!_isTargetable)
                return;
            AnimateMarker(false);

            // If the ability is multitarget
            if (BattleManager.instance.currentChar == null || BattleManager.instance.currentChar.selectedAction == null)
                return;
            if (BattleManager.instance.currentChar.selectedAction.multitarget)
                AnimateMarkers(BattleManager.instance.currentChar.selectedAction.targetOptions, false);
            
            foreach (Transform q in BattleManager.instance.QueuePanel.transform)
            {
                if (q.name == thisEntity.ID)
                    q.transform.GetChild(1).gameObject.SetActive(false);
            }
        }

        private void OnMouseDown()
        {
            if (!_isTargetable)
                return;

            BattleManager.instance.currentChar.selectedAction.UseOnTarget(this);
        }

        public abstract void PlayDamageAnimAndSound();

        public abstract void ReturnToIdle();
    }
}
