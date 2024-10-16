using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MiniGames.DoodleJump
{
    public class OncePlatform : BasePlatform
    {

        protected override void Awake()
        {
            base.Awake();
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
                    
                    boxCollider.enabled = false;
                    animator.SetTrigger("disappear");
                    Invoke("Hide", 1.0f);
                }
            }
        }

        private void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}