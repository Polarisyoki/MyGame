using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Metafalica.RPG
{
    public class CocoaAnimation : BaseAnimation
    {
        private int _idelParam = Animator.StringToHash("IdleParam");
        
        protected override void Start()
        {
            base.Start();
            InvokeRepeating("ChangeIdle",15f,30f);
        }

        private void ChangeIdle()
        {
            int value = Random.Range(0, 5);
            ChangeAnim(_idelParam,value, () =>
            {
                ChangeAnim(_idelParam, 0);
            });
        }
        
    }
}


