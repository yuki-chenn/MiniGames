using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MiniGames.DoodleJump
{
    public class BasePlatform : MonoBehaviour
    {
        public float jumpForce = 20f;
        public Animator animator;
        public BoxCollider2D boxCollider;

        protected virtual void Awake()
        {
            animator = GetComponent<Animator>();
            boxCollider = GetComponent<BoxCollider2D>();
        }

        protected virtual void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.transform.CompareTag("DJ_Player"))
            {
                // 刷新当前高度
                DJGameManager.Instance.UpdateHeight(transform.position.y);
                DJGameManager.Instance.UpdateDifficulty();
            }
        }

        public virtual void Reset()
        {
            if (animator != null) animator.Rebind();
            if(boxCollider != null) boxCollider.enabled = true;
        }


    }

    public enum PlatformType
    {
        NomalPlatform,
        MovePlatform,
        OncePlatform,
        BoomPlatform,
        BrokenPlatform,
    }

}
