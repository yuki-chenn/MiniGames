using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MiniGames.DoodleJump
{
    public class MovePlatform : BasePlatform
    {
        public float speed = 1f;

        public float moveRange = 2f;

        private float originalX;

        protected override void Awake()
        {
            base.Awake();
            originalX = transform.position.x;
        }


        private void Update()
        {
            Vector3 pos = transform.position;
            pos.x += speed * Time.deltaTime;
            if (pos.x > originalX + moveRange)
            {
                pos.x = originalX + moveRange;
                speed = -speed;
            }
            else if (pos.x < originalX - moveRange)
            {
                pos.x = originalX - moveRange;
                speed = -speed;
            }
            transform.position = pos;
        }


        protected override void OnTriggerEnter2D(Collider2D collision)
        {
            base.OnTriggerEnter2D(collision);
            if (collision.transform.CompareTag("DJ_Player"))
            {
                Rigidbody2D rigidbody = collision.attachedRigidbody;
                // 获取刚体的速度
                Vector2 objectVelocity = rigidbody.velocity;

                if (rigidbody != null && objectVelocity.y <= Constants.VELOCITY_EPS)
                {
                    //Debug.Log(collision.collider);
                    DJAudioManager.Instance.PlayJumpPlatformEffect();
                    Vector2 v = rigidbody.velocity;
                    v.y = jumpForce;
                    rigidbody.velocity = v;
                    
                }
            }
        }
    }
}