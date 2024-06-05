using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;


public class PlayerScore : NetworkBehaviour
{
    private int score = 0;

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

}
