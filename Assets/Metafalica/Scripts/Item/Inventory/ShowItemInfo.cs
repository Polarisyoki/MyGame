using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Metafalica.RPG
{
    public class ShowItemInfo : MonoBehaviour
    {
        public void UpdateInfo(object o, ItemType itemType)
        {
            switch (itemType)
            {
                case ItemType.Equip:
                    UpdateInfo(o as Equipment);
                    break;
                case ItemType.Mat:
                    UpdateInfo(o as Item_Mat);
                    break;
                case ItemType.Task:
                    UpdateInfo(o as Item_Task);
                    break;
                default:
                    break;
            }
        }

        public void NoInfo()
        {
            RectTransform content = this.GetComponent<ScrollRect>().content;
            content.gameObject.SetActive(false);
        }
        
        public void UpdateInfo(Equipment wp)
        {
            RectTransform content = this.GetComponent<ScrollRect>().content;
            content.gameObject.SetActive(true);

            content.Find("Img/Image").GetComponent<Image>().sprite = wp.Icon;
            content.Find("Txt/Name").GetComponent<Text>().text = wp.Name;
            string propertyTxt = "";

            propertyTxt = wp.m_Attr.ShowInfo();

            content.Find("Txt/Property").GetComponent<Text>().text = propertyTxt;
            content.Find("Txt/Intro").GetComponent<Text>().text = wp.Description;

            content.Find("Txt/Property").GetComponent<ContentSizeFitter>().SetLayoutVertical();
            content.Find("Txt/Intro").GetComponent<ContentSizeFitter>().SetLayoutVertical();
            content.GetComponent<ContentSizeFitter>().SetLayoutVertical();
        }

        public void UpdateInfo(Item_Mat item)
        {
            RectTransform content = this.GetComponent<ScrollRect>().content;
            content.gameObject.SetActive(true);

            content.Find("Img/Image").GetComponent<Image>().sprite = item.Icon;
            content.Find("Txt/Name").GetComponent<Text>().text = item.Name;

            content.Find("Txt/Property").GetComponent<Text>().text = "";
            content.Find("Txt/Intro").GetComponent<Text>().text = item.Description;

            content.Find("Txt/Property").GetComponent<ContentSizeFitter>().SetLayoutVertical();
            content.Find("Txt/Intro").GetComponent<ContentSizeFitter>().SetLayoutVertical();
            content.GetComponent<ContentSizeFitter>().SetLayoutVertical();
        }
        
        public void UpdateInfo(Item_Task item)
        {
            RectTransform content = this.GetComponent<ScrollRect>().content;
            content.gameObject.SetActive(true);

            content.Find("Img/Image").GetComponent<Image>().sprite = item.Icon;
            content.Find("Txt/Name").GetComponent<Text>().text = item.Name;

            content.Find("Txt/Property").GetComponent<Text>().text = "";
            content.Find("Txt/Intro").GetComponent<Text>().text = item.Description;

            content.Find("Txt/Property").GetComponent<ContentSizeFitter>().SetLayoutVertical();
            content.Find("Txt/Intro").GetComponent<ContentSizeFitter>().SetLayoutVertical();
            content.GetComponent<ContentSizeFitter>().SetLayoutVertical();
        }
    }
}