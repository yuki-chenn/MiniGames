using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MiniGames.BigWatermelon
{
    public class Fruit : MonoBehaviour
    {
        public float limitX;
        public bool released = false;
        public int fruitType;

        public float originScale;
        public bool isExpand = false;

        private void Awake()
        {
            originScale = transform.localScale.x;
        }

        private void Update()
        {
            if(transform.localScale.x < originScale)
            {
                if(transform.localScale.x > originScale * 0.8)
                {
                    isExpand = false;
                }
                transform.localScale += new Vector3(10f, 10f, 0) * Time.deltaTime;
            }
            else
            {
                isExpand = false;
                transform.localScale = originScale * Vector3.one;
            }
        }

        public void SetScale()
        {
            transform.localScale = Vector3.zero;
            isExpand = true;
        }


        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (!released) return;
            if (collision.transform.tag == "BW_Fruit")
            {
                var collisionFruit = collision.transform.GetComponent<Fruit>();
                if (collisionFruit == null || !collisionFruit.released || isExpand) return;
                if (fruitType == collisionFruit.fruitType)
                {
                    if (gameObject.transform.position.y < collision.transform.position.y)
                    {
                        // 位置更低的合并，防止调用两次
                        BWGameManager.Instance.MergeFruit(gameObject, collision.gameObject);
                    }
                    else if(gameObject.transform.position.y == collision.transform.position.y)
                    {
                        if(gameObject.transform.position.x < collision.transform.position.x)
                        {
                            // 位置更左的合并，防止调用两次
                            BWGameManager.Instance.MergeFruit(gameObject, collision.gameObject);
                        }
                    }
                }
            }
        }

    }
}

