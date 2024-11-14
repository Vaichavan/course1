using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectCoins : MonoBehaviour
{
    [Tooltip("The particles that appear after the player collects a coin.")]
    public GameObject coinParticles;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            PlayerMovement playerMovementScript = other.GetComponent<PlayerMovement>();
            playerMovementScript.soundManager.PlayCoinSound();
            ScoreManager.score += 10;

            // Update the coin count in PlayerMovement
            playerMovementScript.CollectCoin();

            GameObject particles = Instantiate(coinParticles, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
