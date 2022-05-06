using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Metafalica.RPG
{
    public class MessageArea : MonoBehaviour
    {
        private RectTransform _panel1;
        private CanvasGroup _panel1_canvasGroup;
        private Text _panel1_text;
        private RectTransform _panel2;
        private CanvasGroup _panel2_canvasGroup;
        private Text _panel2_text;

        private void GetField()
        {
            _panel1 = transform.Find("Area/Panel1").GetComponent<RectTransform>();
            _panel1_canvasGroup = _panel1.GetComponent<CanvasGroup>();
            _panel1_text = _panel1.Find("Text").GetComponent<Text>();
            
            _panel2 = transform.Find("Area/Panel2").GetComponent<RectTransform>();
            _panel2_canvasGroup = _panel2.GetComponent<CanvasGroup>();
            _panel2_text = _panel2.Find("Text").GetComponent<Text>();
        }

        private void Awake()
        {
            GetField();
        }

        public void SetPanelAlpha(int index ,float alpha)
        {
            switch (index)
            {
                case 0:
                    _panel1_canvasGroup.alpha = alpha;
                    break;
                case 1:
                    _panel2_canvasGroup.alpha = alpha;
                    break;
            }
        }
        public void SetPanelAlphaAnim(int index ,float targetAlpha,float time)
        {
            switch (index)
            {
                case 0:
                    _panel1_canvasGroup.DOFade(targetAlpha, time);
                    break;
                case 1:
                    _panel2_canvasGroup.DOFade(targetAlpha, time);
                    break;
            }
        }
        
        public void SetPanelPos(int index ,Vector2 pos)
        {
            switch (index)
            {
                case 0:
                    _panel1.localPosition = pos;
                    break;
                case 1:
                    _panel2.localPosition = pos;
                    break;
            }
        }
        
        public void SetPanelPosYAnim(int index ,float targetY,float time)
        {
            switch (index)
            {
                case 0:
                    _panel1.DOAnchorPosY(targetY, time);
                    break;
                case 1:
                    _panel2.DOAnchorPosY(targetY, time);
                    break;
            }
        }
        
        public void SetPanelText(int index ,string str)
        {
            switch (index)
            {
                case 0:
                    _panel1_text.text = str;
                    break;
                case 1:
                    _panel2_text.text = str;
                    break;
            }
        }
    }
}