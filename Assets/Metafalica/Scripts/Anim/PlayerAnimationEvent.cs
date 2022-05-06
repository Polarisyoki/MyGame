using System.Collections;
using System.Collections.Generic;
using Metafalica.RPG.Skill;
using UnityEngine;

namespace Metafalica.RPG
{
    public class PlayerAnimationEvent : MonoBehaviour
    {
        private SkillData _skillData = new SkillData()
        {
            AttackDistance = 1,
            AttackTargetTags = new string[]{"Enemy","Monster"},
            AttackType = SkillAttackType.Group,
            AttackAngle = 90,
        };

        private GameObject[] _objs;
        
        void Hit()
        {
            _objs = BattleManager.Instance.AttackSelector.SeletctSphereTarget(_skillData,
                PlayerManager.Instance.Player.transform);
            if (_objs == null || _objs.Length == 0)
                return;
            for (int i = 0; i < _objs.Length; i++)
            {
                _objs[i].SendMessage("OnDamage",PlayerManager.Instance.PlayerData.ATK);
            }
        }
        
    
    }

}
