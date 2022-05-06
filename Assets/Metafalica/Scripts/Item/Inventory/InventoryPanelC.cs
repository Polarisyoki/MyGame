using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.VisualScripting;

namespace Metafalica.RPG
{
    public class InventoryPanelC : UIBaseWindow
    {
        public ScrollRect _srBagArea;
        public ScrollRect _srIntroArea;
        public Button _btnItemExAction;
        public Text _txtItemExAction;
        public Button _btnDelete;
        public Text _txtMoney;
        public Button _btnBagType_Equip;
        public Button _btnBagType_Mat;
        public Button _btnBagType_Task;
        public Button _btnClose;


        void GetFeild()
        {
            _srBagArea = this.transform.Find("T_Panel/Sr_BagArea").GetComponent<ScrollRect>();
            _srIntroArea = this.transform.Find("T_Panel/Sr_IntroArea").GetComponent<ScrollRect>();
            _btnItemExAction = this.transform.Find("T_Panel/Btn_ExAction").GetComponent<Button>();
            _txtItemExAction = this.transform.Find("T_Panel/Btn_ExAction/Text").GetComponent<Text>();
            _btnDelete = this.transform.Find("T_Panel/Btn_Delete").GetComponent<Button>();
            _txtMoney = this.transform.Find("T_Panel/Money").GetComponent<Text>();
            _btnBagType_Equip = this.transform.Find("T_Panel/T_BagType/Btn_Equip").GetComponent<Button>();
            _btnBagType_Mat = this.transform.Find("T_Panel/T_BagType/Btn_Mat").GetComponent<Button>();
            _btnBagType_Task = this.transform.Find("T_Panel/T_BagType/Btn_Task").GetComponent<Button>();
            _btnClose = this.transform.Find("Btn_Close").GetComponent<Button>();
        }

        private PlayerInventoryC _playerInventoryC;
        private List<GameObject> bagGoList = new List<GameObject>();

        private int curSelect = 0;
        private ItemType curBagItemType;

        private Action itemExAction;

        private void Awake()
        {
            GetFeild();
            SetBtnAction();
        }

        void Start()
        {
            curBagItemType = ItemType.Equip;
            _playerInventoryC = PlayerManager.Instance.Inventory;

            RefreshBag();
        }

        void SetBtnAction()
        {
            _btnClose.onClick.AddListener(() => { CloseWindow(); });

            _btnDelete.onClick.AddListener(() => { DeleteItem(); });

            _btnBagType_Equip.onClick.AddListener(() => { ChangeBag(ItemType.Equip); });
            _btnBagType_Mat.onClick.AddListener(() => { ChangeBag(ItemType.Mat); });
            _btnBagType_Task.onClick.AddListener(() => { ChangeBag(ItemType.Task); });

            _btnItemExAction.onClick.AddListener(() => { itemExAction?.Invoke(); });
        }

        void RefreshBag()
        {
            if (bagGoList.Count > 0)
            {
                foreach (var val in bagGoList)
                {
                    ObjectManager.Instance.ReleaseObject(val, -1);
                    // val.SetActive(false);
                }
            }

            bagGoList.Clear();

            int n = _playerInventoryC.GetBagCount(curBagItemType);
            if (n == 0)
            {
                curSelect = -1;
            }

            for (int i = 0; i < n; i++)
            {
                GameObject go;
                // if (i < _srBagArea.content.childCount)
                // {
                //     go = _srBagArea.content.GetChild(i).gameObject;
                //     go.SetActive(true);
                // }
                // else
                // {
                //     // go = Instantiate(prefab, _srBagArea.content);
                //     go = ObjectManager.Instance.InstantiateObject(GamePath.Res_Prefab_File_ItemCell,
                //         _srBagArea.content);
                //     bagGoList.Add(go);
                // }

                go = ObjectManager.Instance.InstantiateObject(GamePath.Res_Prefab_File_ItemCell, _srBagArea.content);
                bagGoList.Add(go);

                int index = i;
                if (curBagItemType == ItemType.Equip)
                {
                    Equipment item = _playerInventoryC.GetBagItem<Equipment>(index, curBagItemType);
                    go.GetComponent<UIItemCellView>().Init(new UIItemCellParam()
                    {
                        icon = item.Icon,
                        itemType = item.ItemType,
                        isUse = item.m_IsUse,
                        clickAction = () => { SelectOne(index); }
                    });
                }
                else if (curBagItemType == ItemType.Mat)
                {
                    Item_Mat item = _playerInventoryC.GetBagItem<Item_Mat>(index, curBagItemType);
                    go.GetComponent<UIItemCellView>().Init(new UIItemCellParam()
                    {
                        icon = item.Icon,
                        itemType = item.ItemType,
                        amount = item.CurCount,
                        clickAction = () => { SelectOne(index); }
                    });
                }
                else if (curBagItemType == ItemType.Task)
                {
                    Item_Task item = _playerInventoryC.GetBagItem<Item_Task>(index, curBagItemType);
                    go.GetComponent<UIItemCellView>().Init(new UIItemCellParam()
                    {
                        icon = item.Icon,
                        itemType = item.ItemType,
                        amount = item.CurCount,
                        clickAction = () => { SelectOne(index); }
                    });
                }
            }

            SelectOne(curSelect);
            RefreshMoney();
        }

        void RefreshMoney()
        {
            _txtMoney.text = "Mana:" + _playerInventoryC.GetMoney();
            _txtMoney.GetComponent<ContentSizeFitter>().SetLayoutVertical();
        }

        void ChangeBag(ItemType type)
        {
            if (curBagItemType == type)
                return;
            curBagItemType = type;
            curSelect = 0;
            RefreshBag();
        }

        void SelectOne(int index)
        {
            if (index == -1)
            {
                ChangeIntro(index);
                return;
            }

            curSelect = index;
            for (int i = 0; i < bagGoList.Count; i++)
            {
                bagGoList[i].GetComponent<UIItemCellView>().OnSelect(i == index);
            }

            if (curBagItemType == ItemType.Equip)
            {
                bool b = (_playerInventoryC.GetBagItem<Equipment>(curSelect, curBagItemType)).m_IsUse;
                _btnItemExAction.gameObject.SetActive(true);
                _txtItemExAction.text = b ? "卸下" : "装备";
                itemExAction = () =>
                {
                    var eq = _playerInventoryC.GetBagItem<Equipment>(curSelect, curBagItemType);
                    eq.SetUsingStatus(!b);
                    _playerInventoryC.SetUsingEquipments(eq);
                    RefreshBag();
                };
            }
            else
            {
                itemExAction = null;
                _btnItemExAction.gameObject.SetActive(false);
            }

            ChangeIntro(index);
        }

        void ChangeIntro(int index)
        {
            if (index == -1)
            {
                _srIntroArea.GetOrAddComponent<ShowItemInfo>().NoInfo();
            }
            else
            {
                object o;
                if (curBagItemType == ItemType.Equip)
                    o = _playerInventoryC.GetBagItem<Equipment>(curSelect, curBagItemType);
                else if (curBagItemType == ItemType.Mat)
                    o = _playerInventoryC.GetBagItem<Item_Mat>(curSelect, curBagItemType);
                else if (curBagItemType == ItemType.Task)
                    o = _playerInventoryC.GetBagItem<Item_Task>(curSelect, curBagItemType);
                else
                    o = null;
                _srIntroArea.GetOrAddComponent<ShowItemInfo>().UpdateInfo(o, curBagItemType);
            }
        }


        void DeleteItem()
        {
            if (curSelect == -1)
                return;

            if (curBagItemType == ItemType.Equip &&
                (_playerInventoryC.GetBagItem<Equipment>(curSelect, curBagItemType)).m_IsUse)
            {
                UITipManager.Instance.PushMessage(ParameterC.DeleteEquipmentError);
                return;
            }

            _playerInventoryC.DeleteItem(curSelect, curBagItemType);

            int count = _playerInventoryC.GetBagCount(curBagItemType);
            if (count <= curSelect)
                curSelect = count - 1;
            RefreshBag();
        }
    }
}