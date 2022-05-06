using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Metafalica.RPG
{
    public class UIItemCellView : MonoBehaviour
    {
        public Image _imgItemIcon;
        public Image _imgMask;
        public Text _txtTip;
        public Text _txtAmount;
        public Button _btnClickCell;
        
        private void GetFeild()
        {
            _imgItemIcon = this.transform.Find("Img_ItemIcon").GetComponent<Image>();
            _imgMask = this.transform.Find("Img_Mask").GetComponent<Image>();
            _txtTip = this.transform.Find("Txt_Tip").GetComponent<Text>();
            _txtAmount = this.transform.Find("Txt_Amount").GetComponent<Text>();
            _btnClickCell = this.transform.Find("Btn_ClickCell").GetComponent<Button>();
        }

        private Action clickAction;

        private void SetAllBtn()
        {
            _btnClickCell.onClick.AddListener(() =>
            {
                clickAction?.Invoke();
            });
        }
        
        private void Awake()
        {
            GetFeild();
            SetAllBtn();
        }

        private void OnEnable()
        {
            
        }

        public void Init(UIItemCellParam param)
        {
            _imgItemIcon.sprite = param.icon;
            _imgMask.enabled = false;
            _txtTip.text = param.isUse ? "已装备" : string.Empty;
            _txtAmount.enabled = param.itemType != ItemType.Equip;
            _txtAmount.text = param.amount.ToString();
            this.clickAction = param.clickAction;
        }

        public void OnSelect(bool isSelect = false)
        {
            _imgMask.enabled = isSelect;
        }
    }
}

