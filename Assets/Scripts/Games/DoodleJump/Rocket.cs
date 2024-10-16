using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

namespace MiniGames.DoodleJump
{
    public class Rocket : MonoBehaviour
    {
        public float rocketForce = 10f;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.transform.CompareTag("DJ_Player"))
            {
                Player player = collision.GetComponent<Player>();
                if (player != null) 
                {
                    DJAudioManager.Instance.PlayRocketEffect();
                    player.UseRocket(rocketForce);
                    gameObject.SetActive(false);
                }
            }
        }
    }
}

