using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

namespace MiniGames.DoodleJump
{
    public class Spring : MonoBehaviour
    {
        public Animator animator;
        public bool isUsed = false;

        public float jumpForce = 30f;

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        public void Reset()
        {
            isUsed = false;
            animator.Rebind();
        }


        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.transform.CompareTag("DJ_Player"))
            {

                Rigidbody2D rb = collision.attachedRigidbody;
                // 获取刚体的速度
                Vector2 objectVelocity = rb.velocity;
                if (!isUsed && objectVelocity.y < Constants.VELOCITY_EPS)
                {
                    DJAudioManager.Instance.PlayJumpSpringEffect();
                    animator.SetTrigger("release");
                    isUsed = true;
                    Vector2 v = objectVelocity;
                    v.y = jumpForce;
                    rb.velocity = v;
                }
            }
        }
    }
}

