using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MiniGames.BigWatermelon
{
    public class RedLine : MonoBehaviour
    {

        public float stayTime = 0;
        private SpriteRenderer spriteRenderer;
        private bool blink = false;

        public List<GameObject> touched;

        private void Awake()
        {
            touched = new List<GameObject>();
            stayTime = Constants.RED_LINE_TIME;
            spriteRenderer = transform.GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            if (!BWGameManager.Instance.isStart)
            {
                blink = false;
                return;
            }

            // 时间减少
            if(touched.Count > 0)
            {
                stayTime -= Time.deltaTime;
            }
            else
            {
                stayTime = Constants.RED_LINE_TIME;
            }

            // 当stayTime小于3s时，红线闪烁
            if (stayTime > 0) {
                if (stayTime < 3)
                {
                    if (!blink)
                    {
                        blink = true;
                        StartCoroutine(Coroutine_Blink());
                    }
                }
                else
                {
                    blink = false;
                    StopCoroutine(Coroutine_Blink());
                    spriteRenderer.enabled = true;
                }
            }
            else
            {
                blink = false;
                spriteRenderer.enabled = true;
                // 游戏结束
                touched = new List<GameObject>();
                stayTime = Constants.RED_LINE_TIME;
                BWGameManager.Instance.GameOver();
            }
        }

        IEnumerator Coroutine_Blink()
        {
            // 闪烁
            while (blink)
            {
                spriteRenderer.enabled = !spriteRenderer.enabled;
                yield return new WaitForSeconds(0.2f);
            }
            spriteRenderer.enabled = true;
        }

        //private void OnTriggerStay2D(Collider2D collision)
        //{
        //    if (!BWGameManager.Instance.isStart) return;
        //    if (collision.tag == "BW_Fruit")
        //    {
        //        stayTime -= Time.deltaTime;
        //    }

        //}

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!BWGameManager.Instance.isStart) return;
            if (collision.tag == "BW_Fruit")
            {
                touched.Add(collision.gameObject);
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (!BWGameManager.Instance.isStart) return;
            if (collision.tag == "BW_Fruit")
            {
                touched.Remove(collision.gameObject);
            }
        }
    }
}

