using UnityEngine;
using UnityEngine.UI;

public class FinalScoreUI : MonoBehaviour
{
    public Text finalScoreText;
    public Text finalTimeText;

    void Start()
    {
        if (ScoreManager.Instance != null)
        {
            finalScoreText.text = "Final Score: " + ScoreManager.Instance.score;
            finalTimeText.text = "Time Used: " + ScoreManager.Instance.timeElapsed.ToString("F2");
        }
    }
}
