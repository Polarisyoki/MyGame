using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Metafalica.RPG
{
    public class DialogueView : MonoBehaviour
    {
        public Image _imgBg;
        public Text _txtName;
        public Text _txtDes;

        private void GetFeild()
        {
            _imgBg = transform.Find("Img_Bg").GetComponent<Image>();
            _txtName = transform.Find("Txt_Name").GetComponent<Text>();
            _txtDes = transform.Find("Txt_Des").GetComponent<Text>();
        }

        private Tween _tween;
        
        private void Awake()
        {
            GetFeild();
        }
        
        public void SetSentence(DialogueData data,Action onEnd = null)
        {
            if(_tween != null)
                _tween.Kill();
            
            _txtName.text = data.npcName;
            _txtDes.text = "";
            float time = data.sentence.Length;
            _tween = _txtDes.DOText(data.sentence, time / 10f)
                .SetEase(Ease.Linear).OnComplete(
                    () => { onEnd?.Invoke(); });
        }
    }
}

