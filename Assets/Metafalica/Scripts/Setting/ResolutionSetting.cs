using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.InteropServices;

namespace Metafalica.RPG
{
    public class ResolutionSetting : MonoBehaviour
    {
        //安卓端不需要此功能
#if UNITY_STANDALONE_WIN

        #region 这部分是直接抄的

        [StructLayout(LayoutKind.Sequential)]
        public struct DEVMODE
        {
            private const int CCHDEVICENAME = 0x20;
            private const int CCHFORMNAME = 0x20;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x20)]
            public string dmDeviceName;

            public short dmSpecVersion;
            public short dmDriverVersion;
            public short dmSize;
            public short dmDriverExtra;
            public int dmFields;
            public int dmPositionX;
            public int dmPositionY;
            public ScreenOrientation dmDisplayOrientation;
            public int dmDisplayFixedOutput;
            public short dmColor;
            public short dmDuplex;
            public short dmYResolution;
            public short dmTTOption;
            public short dmCollate;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x20)]
            public string dmFormName;

            public short dmLogPixels;
            public int dmBitsPerPel;
            public int dmPelsWidth;
            public int dmPelsHeight;
            public int dmDisplayFlags;
            public int dmDisplayFrequency;
            public int dmICMMethod;
            public int dmICMIntent;
            public int dmMediaType;
            public int dmDitherType;
            public int dmReserved1;
            public int dmReserved2;
            public int dmPanningWidth;
            public int dmPanningHeight;
        }

        [DllImport("user32.dll")]
        public static extern bool EnumDisplaySettings(string deviceName, int modeNum, ref DEVMODE devMode);

        #endregion

        public Dropdown _dropdown;
        private List<string> tempOption = new List<string>();
        private string curOption;
        private bool flag;

        void Start()
        {
            InitList();

            curOption = PlayerPrefs.GetString("ResolutionOption", "-");
            flag = false; //标记是否被匹配

            AddDropdownKey();

            if (!flag)
            {
                _dropdown.value = 0;
                ChangeSolution(0);
            }

            //监听分辨率选项
            _dropdown.onValueChanged.AddListener((int i) => { ChangeSolution(i); });
        }

        //获取分辨率列表
        void InitList()
        {
            DEVMODE devmode = new DEVMODE();
            int index = 0;
            List<string> list = new List<string>();
            while (EnumDisplaySettings(null, index, ref devmode))
            {
                string temp = devmode.dmPelsWidth + "*" + devmode.dmPelsHeight;
                if (!list.Contains(temp))
                {
                    list.Add(temp);
                }

                index++;
            }

            for (int i = list.Count - 1; i >= list.Count / 3; i--)
            {
                tempOption.Add(list[i]);
            }
        }

        //添加选项,字体等可提前修改模板
        void AddDropdownKey()
        {
            _dropdown.options.Clear();
            for (int i = 0; i < tempOption.Count; i++)
            {
                string tempTip_CN = i == 0 ? "全屏" : "窗口"; //这里考虑从外部填入
                string temp = tempOption[i] + " " + tempTip_CN;
                _dropdown.options.Add(new Dropdown.OptionData(temp));

                //修改默认值
                if (curOption == temp)
                {
                    _dropdown.value = i;
                    flag = true;
                }
            }
        }

        //调整分辨率，并记录
        void ChangeSolution(int i)
        {
            string temp = _dropdown.options[i].text;
            int tempW = int.Parse(temp.Split('*', ' ')[0]);
            int tempH = int.Parse(temp.Split('*', ' ')[1]);
            bool tempFullScreen = i == 0;
            Screen.SetResolution(tempW, tempH, tempFullScreen);
            PlayerPrefs.SetString("ResolutionOption", temp);
        }
#endif
    }
}