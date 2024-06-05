using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

public class PlayerScore : NetworkBehaviour
{
    private int score = 0;
    public string winPanelTag = "WinPanel";
    public string losePanelTag = "LosePanel";

    private GameObject[] winParentObjects;
    private GameObject[] loseParentObjects;

    public int GetScore()
    {
        return score;
    }

    public void AddScore(int amount)
    {
        if (IsOwner)
        {
            score += amount;
            UpdateScoreUI();

            // Verificar si el jugador ha alcanzado 7 monedas
            if (score >= 5)
            {
                // Activar el panel "Ganaste" solo para el propietario del jugador
                foreach (GameObject winParent in winParentObjects)
                {
                    if (winParent.GetComponent<NetworkObject>() != null && winParent.GetComponent<NetworkObject>().OwnerClientId == NetworkManager.Singleton.LocalClientId)
                    {
                        // Activar todos los hijos del objeto principal
                        foreach (Transform child in winParent.transform)
                        {
                            child.gameObject.SetActive(true);
                        }
                    }
                }

                // Activar el panel "Perdiste" para todos los jugadores excepto el propietario del jugador que ganó
                foreach (GameObject loseParent in loseParentObjects)
                {
                    if (loseParent.GetComponent<NetworkObject>() != null && loseParent.GetComponent<NetworkObject>().OwnerClientId != NetworkManager.Singleton.LocalClientId)
                    {
                        // Activar todos los hijos del objeto principal
                        foreach (Transform child in loseParent.transform)
                        {
                            child.gameObject.SetActive(true);
                        }
                    }
                }
            }
        }
    }

    private void UpdateScoreUI()
    {
        // Aquí deberías actualizar el UI del jugador
        ScoreUI scoreUI = FindObjectOfType<ScoreUI>();
        if (scoreUI != null)
        {
            scoreUI.UpdateScoreText(score);
        }
    }

    private void Start()
    {
        // Encontrar y almacenar referencias a los objetos principales con las etiquetas de victoria y derrota
        winParentObjects = FindParentObjectsWithTag(winPanelTag);
        loseParentObjects = FindParentObjectsWithTag(losePanelTag);

        // Desactivar los hijos de los objetos principales al inicio del juego
        DeactivateChildren(winParentObjects);
        DeactivateChildren(loseParentObjects);
    }

    private GameObject[] FindParentObjectsWithTag(string tag)
    {
        GameObject[] parentObjects = GameObject.FindGameObjectsWithTag(tag);
        List<GameObject> filteredParents = new List<GameObject>();

        foreach (GameObject parent in parentObjects)
        {
            // Asegurarse de que el objeto no sea un hijo de otro objeto con la misma etiqueta
            if (parent.transform.parent == null || parent.transform.parent.tag != tag)
            {
                filteredParents.Add(parent);
            }
        }

        return filteredParents.ToArray();
    }

    private void DeactivateChildren(GameObject[] parentObjects)
    {
        foreach (GameObject parent in parentObjects)
        {
            foreach (Transform child in parent.transform)
            {
                child.gameObject.SetActive(false);
            }
        }
    }
}
