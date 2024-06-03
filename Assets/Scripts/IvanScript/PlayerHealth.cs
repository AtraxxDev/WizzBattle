using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxLives = 2;
    public int currentLives;
    [SerializeField] private int _timeforRegen=16;
    private Coroutine regenCoroutine;
    [SerializeField]private PlayerMovement _playerRef;

    private void Start()
    {
        currentLives = maxLives;     
        _playerRef = GetComponent<PlayerMovement>();
    }

    public void TakeDamage(int damage)
    {
        currentLives -= damage;
        currentLives = Mathf.Clamp(currentLives, 0, maxLives);
        Debug.Log($"Player took damage. Current lives: {currentLives}");

        if (currentLives <=0 )
        {
            DeactivatePlayer();
        }
        else
        {
            if (regenCoroutine != null)
            {
                StopCoroutine(regenCoroutine);
            }
            regenCoroutine = StartCoroutine(RegenerateLife());
        }
    }

    private IEnumerator RegenerateLife()
    {
        yield return new WaitForSeconds(_timeforRegen);

        currentLives += 1;
        currentLives = Mathf.Clamp(currentLives, 0, maxLives);
        Debug.Log($"Player regenerated a life. Current lives: {currentLives}");

        if (currentLives < maxLives)
        {
            regenCoroutine = StartCoroutine(RegenerateLife());
        }
    }

    private void DeactivatePlayer()
    {
        Debug.Log("Player has been deactivated.");
        _playerRef.playerDed = true;
        //gameObject.SetActive(false);
    }

    public int GetCurrentLives()
    {
        return currentLives;
    }
}
