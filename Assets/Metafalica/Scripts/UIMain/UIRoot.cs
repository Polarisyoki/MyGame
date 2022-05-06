using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Metafalica.RPG
{
    public class UIRoot : MonoBehaviour
    {
        public Transform m_Canvas;
        public Camera m_UICamera;

        public GameObject _uiLoading;
        public Image _imgBg;
        public Text _txtLoad;

        private void GetFeild()
        {
            m_Canvas = transform.Find("Canvas");
            m_UICamera = transform.Find("UICamera").GetComponent<Camera>();

            _uiLoading = transform.Find("UILoading").gameObject;
            _imgBg = transform.Find("UILoading/Img_Bg").GetComponent<Image>();
            _txtLoad = transform.Find("UILoading/Txt_Load").GetComponent<Text>();
            _uiLoading.SetActive(false);
        }

        public void Init()
        {
            GetFeild();
        }

        private Tween loading;
        
        /// <summary>
        /// 开始切换场景，需要用FadeEnd结束
        /// </summary>
        public void FadeStart()
        {
            GlobalConditionC.freezeUI = true;

            _uiLoading.SetActive(true);
            _txtLoad.enabled = false;
            _imgBg.color = Color.clear;
            _imgBg.DOColor(Color.black, 1f).SetEase(Ease.Linear).OnComplete(() =>
            {
                _txtLoad.enabled = true;
                _txtLoad.text = "";
                loading = _txtLoad.DOText("Loading...", 3f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart);
            });
        }

        /// <summary>
        /// 结束切换场景
        /// </summary>
        public void FadeEnd()
        {
            _txtLoad.enabled = false;
            loading.Kill();
            _imgBg.DOColor(Color.clear, 0.5f).OnComplete(() =>
            {
                _uiLoading.SetActive(false);
                GlobalConditionC.freezeUI = false;
            });
        }

        /// <summary>
        /// 黑屏切场,变黑/保持黑/恢复
        /// </summary>
        public void FadeSingle(float time1, float time2, float time3)
        {
            _uiLoading.SetActive(true);
            _txtLoad.enabled = false;
            _imgBg.DOColor(Color.black, time1).SetEase(Ease.Linear).OnComplete(() =>
            {
                _imgBg.DOColor(Color.black, time2).OnComplete(() =>
                {
                    _imgBg.DOColor(Color.clear, time3).OnComplete(() => { _uiLoading.SetActive(false); });
                });
            });
        }
    }
}