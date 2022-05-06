using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

namespace Metafalica.RPG
{
    public class DialogueC : SingletonMono<DialogueC>
    {
        private DialogueView _dialogue;

        private const UIState _state = UIState.Dialogue;

        private LinkedList<DialogueData> _sq;

        //0:无对话; 1:正在对话中,且不能跳过; 2:正在对话中,且可以跳过; 3:对话结束,等待其他操作
        private int Flag = 0;

        // private Tween _tween; //对话的动画

        private Action _onEnd;


        private LinkedList<DialogueData> Sequeue
        {
            get
            {
                if (_sq == null)
                {
                    _sq = new LinkedList<DialogueData>();
                }

                return _sq;
            }
        }

        private void Update()
        {
            if (Flag == 2)
            {
                if (Input.GetMouseButton(0) || Input.GetKeyDown(KeyCode.Space))
                {
                    ShowNextSentence();
                }
            }
        }

        public void PushSentence(DialogueData data, Action onEnd = null)
        {
            PushSentence(new []{data}, onEnd);
        }

        public void PushSentence(DialogueData[] datas, Action onEnd = null)
        {
            if (datas == null || datas.Length == 0) return;

            this._onEnd = onEnd;
            for (int i = 0; i < datas.Length; ++i)
            {
                Sequeue.AddLast(datas[i]);
            }

            if (Flag == 0)
            {
                StartDialogue();
            }
            else if (Flag == 3)
            {
                ShowSentence();
            }
        }
        public void PushSentence(List<DialogueData> datas, Action onEnd = null)
        {
            PushSentence(datas.ToArray(), onEnd);
        }

        private void PopSentence()
        {
            Sequeue.RemoveFirst();
        }

        public void ClearSq()
        {
            Sequeue.Clear();
        }

        public void StartDialogue()
        {
            _dialogue = UIManager.Instance.OpenWindow(GamePath.Res_Prefab_File_DialogueView, _state).GetComponent<DialogueView>();
            ShowSentence();
        }

        public void FinishDialogue()
        {
            _onEnd = null;
            ClearSq();
            UIManager.Instance.CloseWindow();
            Flag = 0;
        }

        private void ShowSentence()
        {
            if (Sequeue.Count == 0) return;
            Flag = 1;
            _dialogue.SetSentence(Sequeue.First.Value, () => { Flag = 2;});
        }

        private void ShowNextSentence()
        {
            PopSentence();
            if (Sequeue.Count == 0)
            {
                Flag = 3;
                if (_onEnd != null)
                    _onEnd();
                else
                    FinishDialogue();
            }
            else
            {
                ShowSentence();
            }
        }
    }

    public struct DialogueData
    {
        public string npcName;
        public string sentence;

        public DialogueData(string npcName, string sentence)
        {
            this.npcName = npcName;
            this.sentence = sentence;
        }
    }
}