using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MiniGames.DoodleJump
{
    public class Player : MonoBehaviour
    {
        private Rigidbody2D rigidBody;
        private SpriteRenderer spriteRenderer;

        public float moveSensity = 5f;
        public float velocity = 0f;

        float scale = 0.9f;

        GameObject rocket;

        public bool isUsingRocket = false;
        public float rocketTimer = 0f;
        public float rocketForce = 0f;

        private void Awake()
        {
            scale = transform.localScale.x;
            rocket = transform.Find("Rocket").gameObject;
            rocket.SetActive(false);
            BindComponent();
        }


        private void BindComponent()
        {
            rigidBody = GetComponent<Rigidbody2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
        }


        private void Update()
        {
            if (!DJGameManager.Instance.isStart) return;
            Move();

            // 限制玩家在屏幕内移动
            if (transform.position.x < DJGameManager.Instance.leftBorder)
            {
                transform.position = new Vector3(DJGameManager.Instance.rightBorder, transform.position.y, transform.position.z);
            }
            if (transform.position.x > DJGameManager.Instance.rightBorder)
            {
                transform.position = new Vector3(DJGameManager.Instance.leftBorder, transform.position.y, transform.position.z);
            }

            RocketMove();

        }

        private void Move()
        {
            // 手机左右摇晃控制人物移动
            if (rigidBody != null)
            {
                float movement = 0f;
                if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
                {
                    movement = Input.acceleration.x * 2f;
                }
                else
                {
                    // 使用键盘箭头键或A/D键来移动
                    movement = Input.GetAxis("Horizontal");
                }

                velocity = movement * moveSensity;
                //Debug.Log("v:" + velocity);
            }
        }

        private void RocketMove()
        {
            if (!isUsingRocket) return;
            rocketTimer -= Time.deltaTime;
            if (rocketTimer < 0)
            {
                isUsingRocket = false;
                rocket.SetActive(false);
                rocketForce = 0;
                rigidBody.velocity = Vector2.zero;
            }
            else
            {
                Vector2 v = rigidBody.velocity;
                v.y = rocketForce;
                rigidBody.velocity = v;
            }

        }

        private void FixedUpdate()
        {
            if (!DJGameManager.Instance.isStart) return;
            Vector2 v = rigidBody.velocity;
            v.x = velocity;
            rigidBody.velocity = v;
            transform.localScale = new Vector3(velocity >= 0 ? scale : -scale, scale, 1);
        }

        public void UseRocket(float force)
        {
            isUsingRocket = true;
            rocket.SetActive(true);
            rocketTimer = Constants.ROCKET_TIME;
            rocketForce = force;
        }

    }
}

