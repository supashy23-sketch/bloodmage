using UnityEngine;
using UnityEngine.SceneManagement;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    public int score = 0;
    public float timeElapsed = 0f;
    private bool isTiming = false; // คุมว่าจะนับเวลาไหม

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (isTiming)
        {
            timeElapsed += Time.deltaTime;
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Stage1") // ซีนเกมหลัก
        {
            ResetScoreAndTime();
            StartTimer();
        }
        else if (scene.name == "EndScene") // ซีนจบ
        {
            StopTimer();
        }
        else // ซีนเมนูหลัก
        {
            StopTimer();
        }
    }

    public void AddScore(int value)
    {
        score += value;
    }

    public void ResetScoreAndTime()
    {
        score = 0;
        timeElapsed = 0f;
    }

    public void StartTimer()
    {
        timeElapsed = 0f;
        isTiming = true;
    }

    public void StopTimer()
    {
        isTiming = false;
    }
}
