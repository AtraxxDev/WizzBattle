using UnityEngine;
using Unity.Netcode;

public class ScoreManager : NetworkBehaviour
{
    private NetworkVariable<int> score = new NetworkVariable<int>();

    private GameObject btnChild; // Referencia al hijo del GameObject btn

    private void Start()
    {
        GameObject btn = GameObject.FindGameObjectWithTag("btn");
        if (btn != null)
        {
            btnChild = btn.transform.GetChild(0).gameObject; // Obtener el primer hijo del GameObject btn
        }

        if (IsServer)
        {
            score.Value = 0; // Inicializa el score en el servidor
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (btnChild != null)
            {
                // Verificar si el GameObject hijo está activo
                if (btnChild.activeSelf)
                {
                    // Desactivar el GameObject hijo
                    btnChild.SetActive(false);
                }
                else
                {
                    // Activar el GameObject hijo
                    btnChild.SetActive(true);
                }
            }
            else
            {
                Debug.LogWarning("El botón hijo no fue encontrado.");
            }
        }
    }

    public void AddScore(int amount)
    {
        if (IsServer)
        {
            score.Value += amount; // Solo el servidor puede modificar el valor de la NetworkVariable
        }
    }

    public int GetScore()
    {
        return score.Value;
    }

    [ClientRpc]
    private void UpdateScoreClientRpc(int newScore)
    {
        score.Value = newScore; // Actualiza el score en todos los clientes
    }
}
