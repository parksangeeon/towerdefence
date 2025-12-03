using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class Wave
{
    public int enemyCount;
    public Transform enemyPrefab;
}

public class WaveSpawner : MonoBehaviour
{
    public Transform spawnPoint;
    public float timeBetweenWaves = 5f;
    public float startDelay = 3f; // 게임 시작 딜레이
    public float healthIncreasePerWave = 5f;
    
    public Wave[] waves;
    
    private float countdown = 0f;
    private int waveIndex = 0;
    private bool gameStarted = false;
    private bool allWavesComplete = false;
    private bool checkingForEnemies = false;
    
    private void Start()
    {
        // 웨이브 설정: 3, 4, 5, 6
        if(waves == null || waves.Length == 0)
        {
            waves = new Wave[4];
            for(int i = 0; i < 4; i++)
            {
                waves[i] = new Wave();
                waves[i].enemyCount = 3 + i; // 3, 4, 5, 6
            }
        }
        
        // 게임 시작 딜레이
        countdown = startDelay;
        gameStarted = false;
        
        // 시작 카운트다운 표시
        StartCoroutine(StartCountdown());
    }
    
    IEnumerator StartCountdown()
    {
        if(Life.instance != null && Life.instance.gameOverText != null)
        {
            Life.instance.gameOverText.gameObject.SetActive(true);
            
            for(int i = (int)startDelay; i > 0; i--)
            {
                Life.instance.gameOverText.text = "Wave Starting in " + i.ToString() + "...";
                yield return new WaitForSeconds(1f);
            }
            
            Life.instance.gameOverText.text = "Wave 1 Start!";
            yield return new WaitForSeconds(0.5f);
            Life.instance.gameOverText.gameObject.SetActive(false);
        }
        else
        {
            yield return new WaitForSeconds(startDelay);
        }
        
        gameStarted = true;
        countdown = timeBetweenWaves;
    }
    
    private void Update()
    {
        if(!gameStarted)
            return;
            
        if (allWavesComplete)
        {
            return;
        }
        
        if (waveIndex >= waves.Length)
        {
            // 모든 웨이브 스폰 완료 - 이제 모든 적이 처치될 때까지 대기
            if(!allWavesComplete && !checkingForEnemies)
            {
                allWavesComplete = true;
                checkingForEnemies = true;
                StartCoroutine(WaitForAllEnemiesDefeated());
            }
            return;
        }
        
        if (countdown <= 0f)
        {
            StartCoroutine(SpawnWave());
            countdown = timeBetweenWaves;
        }
        countdown -= Time.deltaTime;
    }
    
    IEnumerator WaitForAllEnemiesDefeated()
    {
        // 씬에 Enemy가 모두 없어질 때까지 대기
        while(true)
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            
            if(enemies.Length == 0)
            {
                // 모든 적이 처치됨 - Stage Clear 표시
                StartCoroutine(StageClear());
                break;
            }
            
            // 0.5초마다 체크
            yield return new WaitForSeconds(0.5f);
        }
    }
    
    IEnumerator StageClear()
    {
        // 현재 씬 이름 가져오기
        string currentScene = SceneManager.GetActiveScene().name;
        int currentStage = 0;
        
        // 씬 이름에서 스테이지 번호 추출 (예: "1Stage" -> 1)
        if(currentScene.Contains("Stage"))
        {
            string stageNum = currentScene.Replace("Stage", "");
            int.TryParse(stageNum, out currentStage);
        }
        
        // 3스테이지면 게임 클리어, 아니면 스테이지 클리어
        if(currentStage == 3)
        {
            // 게임 클리어
            StartCoroutine(GameClear());
        }
        else
        {
            // Stage Clear 표시
            if(Life.instance != null && Life.instance.gameOverText != null)
            {
                Life.instance.gameOverText.text = "STAGE CLEAR!";
                Life.instance.gameOverText.gameObject.SetActive(true);
            }
            
            // 3초 대기
            yield return new WaitForSeconds(3f);
            
            // 다음 스테이지로 이동
            LoadNextStage();
        }
    }
    
    IEnumerator GameClear()
    {
        // 게임 클리어 표시
        if(Life.instance != null && Life.instance.gameOverText != null)
        {
            Life.instance.gameOverText.text = "GAME CLEAR!";
            Life.instance.gameOverText.gameObject.SetActive(true);
        }
        
        // 3초 대기
        yield return new WaitForSeconds(3f);
        
        // 종료 메시지 표시
        if(Life.instance != null && Life.instance.gameOverText != null)
        {
            Life.instance.gameOverText.text = "This game will be turn off 10 seconds later\ncongraturation!";
        }
        
        // 10초 대기
        yield return new WaitForSeconds(10f);
        
        // 게임 종료
        Application.Quit();
        
        #if UNITY_EDITOR
        // 에디터에서는 Quit이 작동하지 않으므로 플레이 모드 종료
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
    
    void LoadNextStage()
    {
        // 현재 씬 이름 가져오기
        string currentScene = SceneManager.GetActiveScene().name;
        
        // 씬 이름에서 스테이지 번호 추출 (예: "1Stage" -> 1)
        if(currentScene.Contains("Stage"))
        {
            string stageNum = currentScene.Replace("Stage", "");
            if(int.TryParse(stageNum, out int currentStage))
            {
                int nextStage = currentStage + 1;
                string nextSceneName = nextStage + "Stage";
                
                // 다음 스테이지 씬이 있는지 확인하고 로드
                if(Application.CanStreamedLevelBeLoaded(nextSceneName))
                {
                    SceneManager.LoadScene(nextSceneName);
                }
                else
                {
                    Debug.Log("Next stage scene not found: " + nextSceneName);
                    // 다음 스테이지가 없으면 타이틀로 돌아가거나 게임 종료
                    SceneManager.LoadScene("Title");
                }
            }
        }
        else
        {
            // 씬 이름 형식이 다르면 타이틀로 이동
            SceneManager.LoadScene("Title");
        }
    }
    
    IEnumerator SpawnWave() 
    {
        Wave wave = waves[waveIndex];
        
        for(int i = 0; i < wave.enemyCount; i++)
        {
            SpawnEnemy(wave.enemyPrefab);
            yield return new WaitForSeconds(0.5f);
        }
        
        waveIndex++;
    }
    
    private void SpawnEnemy(Transform enemyPrefab)
{
    if (enemyPrefab != null)
    {
        // 적 생성
        Transform enemyTf = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);

        // Enemy 컴포넌트 가져오기
        Enemy enemy = enemyTf.GetComponent<Enemy>();
        if (enemy != null)
        {
            float extraHp = healthIncreasePerWave * waveIndex;

            enemy.startHealth += extraHp;

        }
    }
}
}
