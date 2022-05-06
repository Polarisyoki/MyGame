using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Metafalica.RPG.Skill
{
    public static class SkillHelper
    {
        /// <summary>
        /// 将碰撞器转换成其绑定的对象
        /// </summary>
        /// <param name="colliders"></param>
        /// <returns></returns>
        public static GameObject[] TransCollider2GameObject(params Collider[] colliders)
        {
            if (colliders == null || colliders.Length == 0)
                return null;
            GameObject[] objs = new GameObject[colliders.Length];
            for (int i = 0; i < colliders.Length; i++)
            {
                objs[i] = colliders[i].gameObject;
            }

            return objs;
        }

        /// <summary>
        /// 离源最近的碰撞器
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="colliders"></param>
        /// <returns></returns>
        public static GameObject[] FindDistanceMin(Vector3 origin, Collider[] colliders)
        {
            if (colliders == null || colliders.Length == 0)
                return null;
            int minIndex = 0;
            float minDis = Vector3.Distance(origin, colliders[0].transform.position);
            for (int i = 1; i < colliders.Length; i++)
            {
                float tempDis = Vector3.Distance(origin, colliders[1].transform.position);
                if (minDis > tempDis)
                {
                    minDis = tempDis;
                    minIndex = i;
                }
            }

            return TransCollider2GameObject(colliders[minIndex]);
        }

        /// <summary>
        /// 查询目标是否在源的特定角度内
        /// </summary>
        /// <returns></returns>
        public static bool RangeFind(Transform origin, Transform target, float angle)
        {
            if (Quaternion.Angle(origin.rotation, Quaternion.LookRotation(target.position - origin.position)) <= angle)
                return true;
            return false;
        }
    }
}