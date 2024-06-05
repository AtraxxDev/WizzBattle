using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;


public class PlayerHealth : NetworkBehaviour
{
    [SerializeField] private int maxLives = 2;
    public int currentLives;
    private float _healthAmount;
    [SerializeField] private int _timeforRegen=16;
    private Coroutine regenCoroutine;
    [SerializeField]private PlayerMovement _playerRef;
    [SerializeField] private Image healthBar;

    private void Start()
    {
        if(IsOwner)
        {
            currentLives = maxLives;
            _playerRef = GetComponent<PlayerMovement>();
            _healthAmount = Mathf.Clamp(_healthAmount, 0, maxLives);
        }

    }

    public void TakeDamage(int damage)
    {
        currentLives -= damage;
        currentLives = Mathf.Clamp(currentLives, 0, maxLives);
        //Actualiza vida en el UI
        _healthAmount = currentLives;
        healthBar.fillAmount = (float)_healthAmount/ maxLives;
        Debug.Log($"Player took damage. Current lives: {currentLives}");

        if (currentLives <= 0)
        {
            DeactivatePlayer();
            StopCoroutine(regenCoroutine);
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
        _healthAmount = currentLives;
        healthBar.fillAmount=(float)_healthAmount/ maxLives;           
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
        _playerRef.playerAnim.SetTrigger("Die");
        _playerRef._lineRenderer.enabled=false;
        
        //gameObject.SetActive(false);
    }

    public int GetCurrentLives()
    {
        return currentLives;
    }
}
