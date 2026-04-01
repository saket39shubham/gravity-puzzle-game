using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Timer")]
    public float timeRemaining = 120f;
    public TextMeshProUGUI timerText;

    [Header("Game State")]
    public bool isGameOver = false;

    [Header("Cubes (Assign in Inspector)")]
    public GameObject[] cubes;   
    private int totalCubes;
    private int collectedCubes = 0;

    void Start()
    {
        totalCubes = cubes.Length;  // auto count
    }

    void Update()
    {
        if (isGameOver) return;

        HandleTimer();
    }

    void HandleTimer()
    {
        timeRemaining -= Time.deltaTime;

        if (timeRemaining <= 0)
        {
            timeRemaining = 0;
            GameOver("Time Up!");
        }

        UpdateTimerUI();
    }

    void UpdateTimerUI()
    {
        int minutes = Mathf.FloorToInt(timeRemaining / 60);
        int seconds = Mathf.FloorToInt(timeRemaining % 60);

        timerText.text = minutes.ToString("00") + ":" + seconds.ToString("00");
    }

    public void GameOver(string reason)
    {
        if (isGameOver) return;

        isGameOver = true;
        Debug.Log("GAME OVER: " + reason);

        Time.timeScale = 0f;
    }

    public void CollectCube(GameObject cube)
    {
        collectedCubes++;

        Debug.Log("Collected: " + collectedCubes + "/" + totalCubes);

        if (collectedCubes >= totalCubes)
        {
            Debug.Log("YOU WIN!");
            Time.timeScale = 0f;
        }
    }
}