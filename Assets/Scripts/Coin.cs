using UnityEngine;

public class Coin : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerScore player = other.GetComponent<PlayerScore>();
            if (player != null && player.IsOwner)
            {
                player.AddScore(1);
                Destroy(gameObject); // Destruye la moneda al recogerla
            }
        }
    }
}
