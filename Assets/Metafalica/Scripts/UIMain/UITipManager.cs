using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEditor;
using UnityEngine.UI;

namespace Metafalica.RPG
{
    public class UITipManager : Singleton<UITipManager>
    {
        private LinkedList<string> msgList;

        private MessageArea _messageArea;
        private int preMsg;
        private int curMsg;
        private bool isShow = false;

        public override void Init(MonoBehaviour mono)
        {
            base.Init(mono);
            
            _messageArea = ObjectManager.Instance.InstantiateObject(GamePath.Res_Prefab_File_MessageArea,
                UIManager.Instance.m_UIRoot.m_Canvas).GetComponent<MessageArea>();
            msgList = new LinkedList<string>();
            preMsg = 0;
            curMsg = 1;
            _messageArea.SetPanelAlpha(preMsg,0);
            _messageArea.SetPanelAlpha(curMsg,0);
        }
        
        public void PushMessage(string msg, float time = 1f)
        {
            msgList.AddLast(msg);
            m_Mono.StartCoroutine(ShowMessage(time));
        }

        private void PopMessage()
        {
            msgList.RemoveFirst();
        }

        IEnumerator ShowMessage(float time = 1f)
        {
            if (isShow) yield break;
            isShow = true;
            WaitForSeconds wait = new WaitForSeconds(time);
            while (msgList.Count > 0)
            {
                if (GlobalConditionC.freezeUI)
                {
                    yield return wait;
                    continue;
                }

                _messageArea.SetPanelText(curMsg,msgList.First.Value);
                _messageArea.SetPanelPos(curMsg,new Vector2(0,-30));
                _messageArea.SetPanelAlpha(curMsg,1);
                yield return wait;

                int t = curMsg;
                curMsg = preMsg;
                preMsg = t;
                
                _messageArea.SetPanelPosYAnim(preMsg,30f,time);
                _messageArea.SetPanelAlphaAnim(preMsg,0,time);
                PopMessage();
            }

            isShow = false;
        }

        void SetAlpha(int index, bool isClear)
        {
            _messageArea.SetPanelAlpha(index,isClear ? 0 : 1);
        }
    }
}