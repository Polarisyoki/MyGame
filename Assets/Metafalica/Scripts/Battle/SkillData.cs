using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace  Metafalica.RPG.Skill
{
    public enum SkillAttackType
    {
        Group,//群攻
        Single,//单体攻击
    }
    public class SkillData
    {
        public float AttackDistance;
        public float AttackAngle;
        public string[] AttackTargetTags;
        public SkillAttackType AttackType;
        
    }
}

