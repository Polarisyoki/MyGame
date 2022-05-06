using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace  Metafalica.RPG
{
    public class BattleManager : Singleton<BattleManager>
    {
        public AttackSelector AttackSelector { get; private set; }

        public override void Init(MonoBehaviour mono)
        {
            base.Init(mono);
            AttackSelector = new AttackSelector();
        }
        
    }
}

