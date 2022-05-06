using System;
using System.Collections;
using System.Collections.Generic;
using Metafalica.RPG.Skill;
using UnityEngine;

namespace Metafalica.RPG
{
    public class AttackSelector
    {

        private Collider[] firstSelect;
        private Collider[] secondSelect;
        /// <summary>
        /// 选择区域内目标的碰撞器
        /// </summary>
        /// <param name="data">技能数据</param>
        /// <param name="attackSource">攻击源的参考点</param>
        /// <returns></returns>
        public GameObject[] SeletctSphereTarget(SkillData data, Transform attackSource)
        {
            firstSelect = Physics.OverlapSphere(attackSource.position, data.AttackDistance);
            if (firstSelect == null || firstSelect.Length == 0)
            {
                return null;
            }

            //找到被标记的所有物体
            secondSelect = Array.FindAll(firstSelect, c =>
                Array.IndexOf(data.AttackTargetTags, c.tag) > 0
                && SkillHelper.RangeFind(attackSource, c.transform, data.AttackAngle));
            if (secondSelect == null || secondSelect.Length == 0)
            {
                return null;
            }

            switch (data.AttackType)
            {
                case SkillAttackType.Group:
                    return SkillHelper.TransCollider2GameObject(secondSelect);
                case SkillAttackType.Single:
                    return SkillHelper.FindDistanceMin(attackSource.position, secondSelect);
            }

            return null;
        }
    }
}