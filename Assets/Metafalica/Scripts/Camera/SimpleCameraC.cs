using UnityEngine;

namespace Metafalica.RPG
{
    public class SimpleCameraC : MonoBehaviour
    {
        public Transform target;
        public float targetHeight = 1.2f;
        public float targetSide = -0.15f;
        public float distance = 4.0f;
        public float maxDistance = 6;
        public float minDistance = 1;
        public float xSpeed = 250.0f;
        public float ySpeed = 120.0f;
        public float yMinLimit = -10;
        public float yMaxLimit = 70;
        public float zoomRate = 80;
        private float x = 15;
        private float y = 0;
        
        private Vector2 tempDir;
        private Quaternion tempRotation;
        RaycastHit hit;
        private Vector3 tempV3;

        void Start()
        {
            if (!target)
            {
                target = PlayerManager.Instance.Player.transform;
            }

            Vector3 angles = transform.eulerAngles;
            x = angles.y;
            y = angles.x;
            
            if (GetComponent<Rigidbody>())
                GetComponent<Rigidbody>().freezeRotation = true;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void LateUpdate()
        {
            if (!target || GlobalConditionC.freezeCam )
            {
                return;
            }

            if (Time.timeScale == 0)
            {
                return;
            }

            x += InputManager.GetAxisMouseX() * xSpeed * 0.02f;
            y -= InputManager.GetAxisMouseY() * ySpeed * 0.02f;

            if (!GlobalConditionC.freezeCamZoom)
            {
                distance -= (InputManager.GetAxisMouseScrollWheel() * Time.deltaTime) * zoomRate * Mathf.Abs(distance);
                distance = Mathf.Clamp(distance, minDistance, maxDistance);
            }

            y = ClampAngle(y, yMinLimit, yMaxLimit);
            //Rotate Camera
            tempRotation = Quaternion.Euler(y, x, 0);
            transform.rotation = tempRotation;

            //运动时鼠标控制角色移动方向，静止时鼠标控制转动视角
            PlayerManager.Instance.Motion.Motor.Rotate(target,x);


            //Camera Position
            tempV3 = target.position -
                     (tempRotation * new Vector3(targetSide, 0, 1) * distance + new Vector3(0, -targetHeight, 0));
            transform.position = tempV3;
            
            tempV3 = Vector3.zero;
            tempV3 = target.position - new Vector3(targetSide, -targetHeight, 0);
            if (Physics.Linecast(tempV3, transform.position, out hit))
            {
                //todo 待补充
                if (hit.transform.tag == "Wall")
                {
                    transform.position = hit.point + hit.normal * 0.1f;
                }
            }
        }

      

        static float ClampAngle(float angle, float min, float max)
        {
            if (angle < -360)
                angle += 360;
            if (angle > 360)
                angle -= 360;
            return Mathf.Clamp(angle, min, max);
        }
    }
}