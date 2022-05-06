using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Metafalica.RPG.Character
{
    public class IKMotion : MonoBehaviour
    {
        private Animator anim;
        //射线检测IK位置
        private Vector3 LeftFootIK,RightFootIK;
        //IK位置
        private Vector3 LeftFootPos, RightFootPos;
        //IK旋转
        private Quaternion LeftFootRot, RightFootRot;
        
        //IK交互层
        public LayerMask EnvLayer;
        //脚部IK与实际射线检测位置的Y轴差
        [Range(0, 0.2f)] public float GroundOffset = 0.1f;
        //射线向下检测距离
        public float GroundDistance = 0.5f;
        public bool enableIK = true;

        private void Start()
        {
            anim = this.GetComponent<Animator>();
        }

        private void FixedUpdate()
        {
            Debug.DrawRay(LeftFootIK, Vector3.down,Color.red);
            if (Physics.Raycast(LeftFootIK + Vector3.up, Vector3.down, out RaycastHit hitL, GroundDistance + 1,
                EnvLayer))
            {
                LeftFootPos = hitL.point + Vector3.up * GroundOffset;
                LeftFootRot = Quaternion.FromToRotation(Vector3.up ,hitL.normal) * transform.rotation;
            }
            Debug.DrawRay(RightFootIK, Vector3.down,Color.red);
            if (Physics.Raycast(RightFootIK + Vector3.up, Vector3.down, out RaycastHit hitR, GroundDistance + 1,
                EnvLayer))
            {
                RightFootPos = hitR.point + Vector3.up * GroundOffset;
                RightFootRot = Quaternion.FromToRotation(Vector3.up ,hitR.normal) * transform.rotation;
            }
        }

        private void OnAnimatorIK(int layerIndex)
        {
            LeftFootIK = anim.GetIKPosition(AvatarIKGoal.LeftFoot);
            RightFootIK = anim.GetIKPosition(AvatarIKGoal.RightFoot);

            if (!enableIK)
                return;
            anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot,anim.GetFloat("LIK"));
            anim.SetIKRotationWeight(AvatarIKGoal.LeftFoot,anim.GetFloat("LIK"));
            anim.SetIKPosition(AvatarIKGoal.LeftFoot,LeftFootPos);
            anim.SetIKRotation(AvatarIKGoal.LeftFoot,LeftFootRot);
            
            anim.SetIKPositionWeight(AvatarIKGoal.RightFoot,anim.GetFloat("RIK"));
            anim.SetIKRotationWeight(AvatarIKGoal.RightFoot,anim.GetFloat("RIK"));
            anim.SetIKPosition(AvatarIKGoal.RightFoot,RightFootPos);
            anim.SetIKRotation(AvatarIKGoal.RightFoot,RightFootRot);
        }
    }
}

