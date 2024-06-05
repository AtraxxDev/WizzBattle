using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreUI : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;

    public void UpdateScoreText(int score)
    {
        scoreText.text = " " + score;
    }
}
