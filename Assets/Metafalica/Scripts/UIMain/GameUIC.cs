using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Metafalica.RPG
{
    public class GameUIC : MonoBehaviour
    {
        public Transform baseUI;
        public Transform playerState;

        public Transform monstorState;

        // public Transform otherPlayerState;
        public Transform taskState;
        // public Transform chatPenal;

        public Transform interactiveOption;
        private List<InteractiveOptionDat> interactiveOptionList;
        private int curSelect = 0;
        private Scrollbar interactiveScrollbar;


        private void Start()
        {
            baseUI.gameObject.SetActive(true);
            UIManager.Instance.SetBaseUI(baseUI.gameObject, this);
            interactiveOptionList = new List<InteractiveOptionDat>();
            interactiveOption.gameObject.SetActive(true);
            interactiveScrollbar = interactiveOption.Find("Scrollbar Vertical").GetComponent<Scrollbar>();
        }

        public void UpdatePlayerState(int hp, int maxHp)
        {
            playerState.Find("CurHP").GetComponent<Image>().fillAmount = 1f * hp / maxHp;
            playerState.Find("CurHP").GetComponentInChildren<Text>().text = hp + "/" + maxHp;
        }

        public void UpdateMonstorState(int lv, string name, int hp, int maxHp)
        {
            monstorState.Find("Name").GetComponentInChildren<Text>().text = "Lv" + lv + " " + name;
            monstorState.Find("CurHP").GetComponent<Image>().fillAmount = 1f * hp / maxHp;
        }

        #region 交互选项

        public class InteractiveOptionDat
        {
            public int id;
            public GameObject obj;
            public Action action;

            public InteractiveOptionDat(int id, GameObject obj, Action action)
            {
                this.id = id;
                this.obj = obj;
                this.action = action;
            }
        }

        //添加选项
        public void AddInteractiveOption(int id, string name, Action action)
        {
            if (interactiveOptionList.Find(c => c.id == id) != null)
                return;
            Transform content = interactiveOption.GetComponent<ScrollRect>().content;
            GameObject go =
                ObjectManager.Instance.InstantiateObject(GamePath.Res_Prefab_File_InteractiveOptionCell, content);
            go.GetComponentInChildren<Text>().text = name;
            
            interactiveOptionList.Add(new InteractiveOptionDat(id,go,action));
            
            if (interactiveOptionList.Count == 1)
            {
                curSelect = 0;
            }

            Refresh();
            content.GetComponent<ContentSizeFitter>().SetLayoutVertical();
        }

        //滚轮滚动选项
        public void ChangeSelect(bool plus)
        {
            int lastSelect = curSelect;
            if (plus)
            {
                if (curSelect < interactiveOptionList.Count - 1)
                    curSelect++;
            }
            else
            {
                if (curSelect > 0)
                    curSelect--;
            }

            if (lastSelect != curSelect)
            {
                Refresh(true);
            }
        }

        private void Refresh(bool isEase = false)
        {
            float res = 0;
            if (curSelect >= interactiveOptionList.Count - 1)
                res = 0;
            else
                res = 1 - curSelect * 1f / interactiveOptionList.Count;

            if (isEase)
            {
                DOTween.To(() => interactiveScrollbar.value, x => interactiveScrollbar.value = x, res, 0.1f);
            }
            else
            {
                interactiveScrollbar.value = res;
            }

            UpdateSelectMask(curSelect);
        }

        private void UpdateSelectMask(int index)
        {
            for (int i = 0; i < interactiveOptionList.Count; ++i)
            {
                interactiveOptionList[i].obj.transform.Find("mask").gameObject.SetActive(i == index);
            }
        }

        //执行选项绑定的方法
        public void ExecOption()
        {
            interactiveOptionList[curSelect].action?.Invoke();
        }

        private int GetDataIndex(int id)
        {
            for (int i = 0; i < interactiveOptionList.Count; ++i)
            {
                if (interactiveOptionList[i].id == id)
                    return i;
            }

            return -1;
        }

        //删除选项
        public void RemoveInteractiveOption(int id, Action action)
        {
            int index = GetDataIndex(id);
            if (index == -1) return;

            Destroy(interactiveOptionList[index].obj);
            if (interactiveOptionList.Count == 1)
            {
                interactiveOptionList.Clear();
                action();
            }
            else
            {
                interactiveOptionList.RemoveAt(index);
                if (curSelect >= interactiveOptionList.Count)
                {
                    curSelect = interactiveOptionList.Count - 1;
                    UpdateSelectMask(curSelect);
                }

                Transform content = interactiveOption.GetComponent<ScrollRect>().content;
                content.GetComponent<ContentSizeFitter>().SetLayoutVertical();
            }
        }

        //清除所有选项
        public void RemoveAllInteractiveOption(Action action)
        {
            for (int i = 0; i < interactiveOptionList.Count; i++)
            {
                Destroy(interactiveOptionList[i].obj);
            }
            interactiveOptionList.Clear();
            action();
        }

        #endregion
    }
}