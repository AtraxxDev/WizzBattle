using UnityEngine;
using Unity.Netcode;

public class ScoreManager : NetworkBehaviour
{
    private NetworkVariable<int> score = new NetworkVariable<int>();

    private void Start()
    {
        if (IsServer)
        {
            score.Value = 0; // Inicializa el score en el servidor
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
