using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Metafalica.RPG
{
    public enum WorldDirection
    {
        North,
        South,
        West,
        East,
    }
    
    public class DungeonMain9010001 : SingletonMono<DungeonMain9010001>
    {
        public Canvas roomHint;//房间提示
        
        //  从0开始共计6
        //  上下加减1，左右加减3
        private int[][][] map = new int[6][][];

        private int size = 3;

        private int x, y, k;//K为第几面，x,y为第几行第几列（从0 开始计数）
        void Awake()
        {
            InitMap();
            k = 0;
            y = x = 1;
            ChangeRoomHint(map[k][x][y]);
        }

        void InitMap()
        {
            for (int i = 0; i < map.Length; i++)
            {
                map[i] = new int[size][];
                for (int j = 0; j < size; j++)
                {
                    map[i][j] = new int[size];
                    for (int m = 0; m < size; m++)
                    {
                        map[i][j][m] = i;
                    }
                }
            }
        }

        public bool HidePortal(WorldDirection dir)
        {
            if (x == 0 && dir == WorldDirection.West) return true;
            if (x == size - 1 && dir == WorldDirection.East) return true;
            if (y == 0 && dir == WorldDirection.North) return true;
            if (y == size - 1 && dir == WorldDirection.South) return true;
            return false;
        }

        public void MovePlayer(WorldDirection dir)
        {
            Vector3 pos = PlayerManager.Instance.Player.transform.position;
            if (dir == WorldDirection.North)
            {
                if (y > 0)
                {
                    y--;
                    PlayerManager.Instance.MovePlayer(pos - new Vector3(0,0,2*pos.z));
                }
            }
            else if(dir == WorldDirection.South)
            {
                if (y < size - 1)
                {
                    y++;
                    PlayerManager.Instance.MovePlayer(pos - new Vector3(0,0,2*pos.z));
                }
            }
            else if(dir == WorldDirection.East)
            {
                if (x < size - 1)
                {
                    x++;
                    PlayerManager.Instance.MovePlayer(pos - new Vector3(2*pos.x,0,0));
                }
            }
            else if(dir == WorldDirection.West)
            {
                if (x > 0)
                {
                    x--;
                    PlayerManager.Instance.MovePlayer(pos - new Vector3(2*pos.x,0,0));
                }
            }

            ChangeRoomHint(map[k][x][y]);
            _action();
        }

        private Action _action;
        public void HideOrShowPortal(Action action)
        {
            _action += action;
        }


        public void RotateMagicCube(WorldDirection dir)
        {
            int t = k;
            if (dir == WorldDirection.North || dir == WorldDirection.South)
            {
                if (dir == WorldDirection.North) k++;
                else k--;

                k = (k + 6) % 6;
                
                //改纵向
                for (int i = 0; i < size; i++)
                {
                    int temp = map[t][x][i];
                    map[t][x][i] = map[k][x][i];
                    map[k][x][i] = temp;
                }
            }
            else
            {
                if (dir == WorldDirection.East) k+=2;
                else k-=2;
                
                k = (k + 6) % 6;
                
                //改横向
                for (int i = 0; i < size; i++)
                {
                    int temp = map[t][i][y];
                    map[t][i][y] = map[k][i][y];
                    map[k][i][y] = temp;
                }
            }
            ChangeRoomHint(map[k][x][y]);
        }

        private void ChangeRoomHint(int num)
        {
            roomHint.GetComponentInChildren<Image>().color = RoomNum2Color(num);
        }

        private Color RoomNum2Color(int num)
        {
            switch (num)
            {
                case 0:
                    return Color.red;
                case 1:
                    return Color.blue;
                case 2:
                    return Color.green;
                case 3:
                    return Color.yellow;
                case 4:
                    return Color.cyan;
                case 5:
                    return Color.white;
                default:
                    return Color.black;
                        
            }
        }
    }

}

