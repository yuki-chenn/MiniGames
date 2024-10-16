using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MiniGames.DoodleJump
{
    public class CameraFollow : MonoBehaviour
    {
        public GameObject target;

        public float smoothTime = 0.3f;
        private Vector3 v = Vector3.zero;


        private void LateUpdate()
        {
            if (target == null) return;
            if(target.transform.position.y > transform.position.y)
            {
                Vector3 des = new Vector3(transform.position.x, target.transform.position.y, transform.position.z);
                transform.position = Vector3.SmoothDamp(transform.position, des, ref v, smoothTime);
            }
        }

    }
}
