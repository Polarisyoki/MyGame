using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Metafalica.RPG
{
    public class StoneConsoleC : MonoBehaviour
    {


        private void RotateMagicCubeUp()
        {
            DungeonMain9010001.Instance.RotateMagicCube(WorldDirection.North);
        }
        private void RotateMagicCubeDown()
        {
            DungeonMain9010001.Instance.RotateMagicCube(WorldDirection.South);
        }
        private void RotateMagicCubeLeft()
        {
            DungeonMain9010001.Instance.RotateMagicCube(WorldDirection.West);
        }
        private void RotateMagicCubeRight()
        {
            DungeonMain9010001.Instance.RotateMagicCube(WorldDirection.East);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == ParameterC.PlayerTagName)
            {
                UIManager.Instance.PushInteractiveOption(-21,"上旋",RotateMagicCubeUp);
                UIManager.Instance.PushInteractiveOption(-22,"下旋",RotateMagicCubeDown);
                UIManager.Instance.PushInteractiveOption(-23,"左旋",RotateMagicCubeLeft);
                UIManager.Instance.PushInteractiveOption(-24,"右旋",RotateMagicCubeRight);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.tag == ParameterC.PlayerTagName)
            {
                UIManager.Instance.PopInteractiveOption(-21);
                UIManager.Instance.PopInteractiveOption(-22);
                UIManager.Instance.PopInteractiveOption(-23);
                UIManager.Instance.PopInteractiveOption(-24);
            }
        
        }
    
    }
}

