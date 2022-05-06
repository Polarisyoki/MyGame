using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Metafalica.RPG
{
    public class UIBaseWindow : MonoBehaviour
    {
        /// <summary>
        /// 关闭窗口，目前调用UIManager中的方法
        /// </summary>
        public void CloseWindow()
        {
            UIManager.Instance.CloseWindow();
        }
    }
}

