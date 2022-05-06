using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Metafalica.RPG
{
    [RequireComponent(typeof(Animator))]
    public class BaseAnimation : MonoBehaviour
    {
        private Animator _animator;
        public Animator m_Animator => _animator;
        

        #region 视线跟随
        //整体权重
        [Range(0, 1)] public float weight = 1;
        [Range(0, 1)] public float bodyWeight = 0.2f;
        [Range(0, 1)] public float headWeight = 1f;
        [Range(0, 1)] public float eyesWeight = 0.6f;
        [Range(0, 90)] public float maxSightAnglr = 50f;
        public float followSpeed = 2f;
        public string targetTag = "Player";
        
        //注视目标
        [HideInInspector]public Transform target;
        //目标位置
        [HideInInspector]public Vector3 targetPos;
        //当前权重
        private float currentWeight = 0;
        //目标与自身夹角
        private float angle;
        
        #endregion
        
        protected virtual void Start()
        {
            _animator = this.GetComponent<Animator>();
        }

        public void SetTargetTag(string targetTag)
        {
            this.targetTag = targetTag;
        }

        public void ChangeAnimForce(string name,float time)
        {
            _animator.CrossFade(name,time);
        }
        public void ChangeAnim(int id,int value,Action onEnd = null)
        {
            _animator.SetInteger(id,value);
            ToNextAction(onEnd);
        }
        public void ChangeAnim(string name,int value,Action onEnd = null)
        {
            _animator.SetInteger(name,value);
            ToNextAction(onEnd);
        }
        
        public void ChangeAnim(int id,float value,Action onEnd = null)
        {
            _animator.SetFloat(id,value);
            ToNextAction(onEnd);
        }
        public void ChangeAnim(string name,float value,Action onEnd = null)
        {
            _animator.SetFloat(name,value);
            ToNextAction(onEnd);
        }
        public void ChangeAnim(int id,bool value,Action onEnd = null)
        {
            _animator.SetBool(id,value);
            ToNextAction(onEnd);
        }
        public void ChangeAnim(string name,bool value,Action onEnd = null)
        {
            _animator.SetBool(name,value);
            ToNextAction(onEnd);
        }
        public void ChangeAnim(int id)
        {
            _animator.SetTrigger(id);
        }
        public void ChangeAnim(string name)
        {
            _animator.SetTrigger(name);
        }

        public bool IsAnimState(string name,out float time)
        {
            if (_animator.GetCurrentAnimatorStateInfo(0).IsName(name))
            {
                time = _animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
                return true;
            }
            time = 0;
            return false;
        }
        
        private void ToNextAction(Action action)
        {
            StartCoroutine(NextAction(action));
        }
        IEnumerator NextAction(Action action)
        {
            yield return new WaitForEndOfFrame();
            action?.Invoke();
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(ParameterC.PlayerTagName))
            {
                target = other.transform.Find("GazeTarget");
                
                PlayerManager.Instance.PlayerAnim.target = this.transform.parent.parent.Find("GazeTarget");
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag(ParameterC.PlayerTagName))
            {
                if (target != null)
                    targetPos = target.position;
                target = null;

                if (PlayerManager.Instance.PlayerAnim.target != null)
                    PlayerManager.Instance.PlayerAnim.targetPos = PlayerManager.Instance.PlayerAnim.target.position;
                PlayerManager.Instance.PlayerAnim.target = null;
            }
        }
        
        private void OnAnimatorIK(int layerIndex)
        {
            if (target != null)
            {
                angle = Vector3.Angle(target.position - transform.position, transform.forward);
                if (angle > maxSightAnglr)
                {
                    targetPos = target.position;
                }
            }

            if (target != null && angle <= maxSightAnglr)
            {
                currentWeight = Mathf.Clamp01(currentWeight + followSpeed * Time.deltaTime) * weight;
                _animator.SetLookAtWeight(currentWeight,bodyWeight,headWeight,eyesWeight);
                _animator.SetLookAtPosition(target.position);
            }
            else
            {
                currentWeight = Mathf.Clamp01(currentWeight - followSpeed * Time.deltaTime) * weight;
                _animator.SetLookAtWeight(currentWeight,bodyWeight,headWeight,eyesWeight);
                _animator.SetLookAtPosition(targetPos);
            }
        }
    }
}

