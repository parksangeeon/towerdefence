using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Life : MonoBehaviour
{
    public static Life instance;
    
    [Header("Life Settings")]
    public int startLives = 3;
    private int lives;
    
    [Header("UI")]
    public TMP_Text livesText;
    public TMP_Text gameOverText;
    public GameObject gameOverPanel;
    public GameObject reButton;

    private bool gameOver = false;
    
    void Awake()
    {
        if(instance != null)
        {
            Debug.LogError("More than one Life in scene!");
            return;
        }
        instance = this;
    }
    
    void Start()
    {
        lives = startLives;
        UpdateLivesText();
        
        if(gameOverText != null)
            gameOverText.gameObject.SetActive(false);
            
        if(gameOverPanel != null)
            gameOverPanel.SetActive(false);
        if (reButton != null)
            reButton.SetActive(false);
    }
    
    public void LoseLife()
    {
        if(gameOver)
            return;
            
        lives--;
        UpdateLivesText();
        
        if(lives <= 0)
        {
            GameOver();
        }
    }
    
    void UpdateLivesText()
    {
        if(livesText != null)
        {
            livesText.text = "Lives: " + lives.ToString();
        }
    }
    
    void GameOver()
    {
        gameOver = true;
        
        if(gameOverText != null)
        {
            gameOverText.text = "GAME OVER";
            gameOverText.gameObject.SetActive(true);
        }
        
        if(gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
        if (reButton != null)
            reButton.SetActive(true);
        Debug.Log("Game Over!");
        Time.timeScale = 0f; // 게임 일시정지
    }
    public void RestartStage()
    {
        Time.timeScale = 1f; // 게임 재시작 시 시간 복구
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // 현재 씬 다시 로드
    }
    public bool IsGameOver()
    {
        return gameOver;
    }
    
    public void ShowMessage(string message, float duration = 0f)
    {
        if(gameOverText != null)
        {
            gameOverText.text = message;
            gameOverText.gameObject.SetActive(true);
            
            if(duration > 0f)
            {
                StartCoroutine(HideMessageAfterDelay(duration));
            }
        }
    }
    
    IEnumerator HideMessageAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if(gameOverText != null)
        {
            gameOverText.gameObject.SetActive(false);
        }
    }
}
