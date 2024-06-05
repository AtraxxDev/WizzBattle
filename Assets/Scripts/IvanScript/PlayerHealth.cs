using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class PlayerHealth : NetworkBehaviour
{
    [SerializeField] private int maxLives = 2;
    private int currentLives;
    private float _healthAmount;
    [SerializeField] private int _timeforRegen = 16;
    private Coroutine regenCoroutine;
    [SerializeField] private PlayerMovement _playerRef;
    [SerializeField] private Image healthBar;

    private void Start()
    {
        if (IsOwner)
        {
            currentLives = maxLives;
            _playerRef = GetComponent<PlayerMovement>();
            _healthAmount = Mathf.Clamp(_healthAmount, 0, maxLives);

            // Inicializa la barra de salud en el dueño del objeto
            UpdateHealthBarClientRpc(currentLives);
        }
    }

    public void TakeDamage(int damage)
    {
        if (IsServer)
        {
            currentLives -= damage;
            currentLives = Mathf.Clamp(currentLives, 0, maxLives);
            Debug.Log($"Player took damage. Current lives: {currentLives}");

            UpdateHealthBarClientRpc(currentLives);

            if (currentLives <= 0)
            {
                DeactivatePlayer();
                if (regenCoroutine != null)
                {
                    StopCoroutine(regenCoroutine);
                }
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
    }

    private IEnumerator RegenerateLife()
    {
        yield return new WaitForSeconds(_timeforRegen);

        if (IsServer)
        {
            currentLives += 1;
            currentLives = Mathf.Clamp(currentLives, 0, maxLives);
            Debug.Log($"Player regenerated a life. Current lives: {currentLives}");

            UpdateHealthBarClientRpc(currentLives);

            if (currentLives < maxLives)
            {
                regenCoroutine = StartCoroutine(RegenerateLife());
            }
        }
    }

    private void DeactivatePlayer()
    {
        Debug.Log("Player has been deactivated.");
        _playerRef.playerDed = true;
        _playerRef.playerAnim.SetTrigger("Die");
        _playerRef._lineRenderer.enabled = false;
    }

    [ClientRpc]
    private void UpdateHealthBarClientRpc(int newLives)
    {
        // Actualiza la barra de salud en todos los clientes
        _healthAmount = newLives;
        healthBar.fillAmount = (float)_healthAmount / maxLives;
    }

    public int GetCurrentLives()
    {
        return currentLives;
    }
}
