using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MiniGames.DoodleJump
{
    public class BoomPlatform : BasePlatform
    {
        public float timer = 0;

        public bool startTurn = false;
        public bool isBoom = false;
        public float turnTime = 3.0f;
        public float boomTime = 1.5f;
        public float turnDis = 5f;

        protected override void Awake()
        {
            base.Awake();
            timer = 0;
        }

        public override void Reset()
        {
            base.Reset();
            timer = 0;
            startTurn = false;
            isBoom = false;
        }

        private void Update()
        {
            if (!startTurn)
            {
                if (Camera.main.transform.position.y > transform.position.y - turnDis)
                {
                    startTurn = true;
                }
            }
            
            if (!startTurn || isBoom) return;

            if (gameObject.activeSelf)
            {
                timer += Time.deltaTime;
            }

            if(timer > turnTime)
            {
                animator.SetTrigger("turn");
            }

            if(timer > turnTime + boomTime)
            {
                isBoom = true;
                DJAudioManager.Instance.PlayPlatformBoomEffect();
                animator.SetTrigger("boom");
                boxCollider.enabled = false;
                Invoke("Hide", 1.0f);
            }

        }


        protected override void OnTriggerEnter2D(Collider2D collision)
        {
            base.OnTriggerEnter2D(collision);
            if (collision.transform.CompareTag("DJ_Player"))
            {
                //Debug.Log(collision.relativeVelocity.ToString("E"));
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

        private void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}