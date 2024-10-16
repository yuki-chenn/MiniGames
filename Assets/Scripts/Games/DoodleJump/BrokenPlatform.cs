using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace MiniGames.DoodleJump
{
    public class BrokenPlatform : BasePlatform
    {
        private bool isDrop = false;

        protected override void Awake()
        {
            base.Awake();
        }

        public override void Reset()
        {
            base.Reset();
            isDrop = false;
            StopAllCoroutines();
        }


        protected override void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.transform.CompareTag("DJ_Player"))
            {
                Rigidbody2D rigidbody = collision.attachedRigidbody;
                // ��ȡ������ٶ�
                Vector2 objectVelocity = rigidbody.velocity;

                if (!isDrop && rigidbody != null && objectVelocity.y <= Constants.VELOCITY_EPS)
                {
                    //Debug.Log(collision.relativeVelocity.ToString("E"));
                    DJAudioManager.Instance.PlayBrokenPlatformEffect();
                    boxCollider.enabled = false;
                    animator.SetTrigger("break");
                    // ģ�����µ���
                    StartCoroutine(Drop());
                    isDrop = true;
                }
            }
        }

        IEnumerator Drop()
        {
            float time = 0;
            while (true)
            {
                if (time >= 3) break;
                time += Time.deltaTime;
                if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
                {
                    transform.position -= new Vector3(0, 0.1f, 0);
                }
                else
                {
                    // ʹ�ü��̼�ͷ����A/D�����ƶ�
                    transform.position -= new Vector3(0, 0.01f, 0);
                }
                yield return null;
            }
            gameObject.SetActive(false);
        }
    }
}