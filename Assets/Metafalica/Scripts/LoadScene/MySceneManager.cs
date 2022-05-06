using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

namespace Metafalica.RPG
{
    public class MySceneManager : Singleton<MySceneManager>
    {
        private string id = "";

        public void LoadScene(string sceneName,string id)
        {
            this.id = id;
            m_Mono.StartCoroutine(WaitLoadScene(sceneName));
        
        }
    
        private IEnumerator WaitLoadScene(string sceneName)
        {
            FadeStart();
            yield return new WaitForSeconds(1f);
            AsyncOperation async = SceneManager.LoadSceneAsync(sceneName);
            async.allowSceneActivation = false;
        
            while (!async.isDone)
            {
                if (async.progress >= 0.9f)
                {
                    yield return new WaitForSeconds(1f);
                    async.allowSceneActivation = true;
                }
        
                yield return new WaitForSeconds(0.5f);
            }
            FadeEnd();
        }
    
        private void FadeStart()
        {
            UIManager.Instance.m_UIRoot.FadeStart();
        }

        private void FadeEnd()
        {
            UIManager.Instance.m_UIRoot.FadeEnd();
        }
    
    }
}

