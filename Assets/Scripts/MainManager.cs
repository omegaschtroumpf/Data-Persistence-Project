using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

public class MainManager : MonoBehaviour
{
    public Brick BrickPrefab;
    public int LineCount = 6;
    private Rigidbody Ball;

    public Text ScoreText;
    public Text HighScoreText;
    public GameObject GameOverText;
    
    private bool m_Started = false;
    public string m_PlayerName;
    private int m_Points;

    private bool m_GameSetUp = false;
    private bool m_GameOver = false;
    private int m_HighScore = 0;
    private string m_HighScoreName;

    public static MainManager Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
        // Load this info for any scene
        // StartNew will load a new game if appropriate
        if (HighScoreText == null) HighScoreText = GameObject.Find("HighScoreText").GetComponent<Text>();
        LoadHighScore();
    }

    public void StartNew()
    {
        if (HighScoreText == null) HighScoreText = GameObject.Find("HighScoreText").GetComponent<Text>();
        if (m_HighScoreName != null)
        {
            HighScoreText.text = "High Score: " + m_HighScoreName + ": " + m_HighScore;
        }
        if (ScoreText == null) ScoreText = GameObject.Find("ScoreText").GetComponent<Text>();
        if (GameOverText == null) GameOverText = GameObject.Find("GameOverText");
        GameOverText.SetActive(false);
        if (Ball == null) Ball = GameObject.Find("Ball").GetComponent<Rigidbody>();
        const float step = 0.6f;
        int perLine = Mathf.FloorToInt(4.0f / step);

        int[] pointCountArray = new[] { 1, 1, 2, 2, 5, 5 };
        for (int i = 0; i < LineCount; ++i)
        {
            for (int x = 0; x < perLine; ++x)
            {
                Vector3 position = new Vector3(-1.5f + step * x, 2.5f + i * 0.3f, 0);
                var brick = Instantiate(BrickPrefab, position, Quaternion.identity);
                brick.PointValue = pointCountArray[i];
                brick.onDestroyed.AddListener(AddPoint);
            }
        }
    }

    private void Update()
    {
        if ((SceneManager.GetActiveScene().buildIndex == 1))
        {
            if (!m_GameOver && !m_Started)
            {
                if (!m_GameSetUp)
                {
                    m_GameSetUp = true;
                    StartNew();
                }
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    m_Started = true;
                    float randomDirection = Random.Range(-1.0f, 1.0f);
                    Vector3 forceDir = new Vector3(randomDirection, 1, 0);
                    forceDir.Normalize();

                    Ball.transform.SetParent(null);
                    Ball.AddForce(forceDir * 2.0f, ForceMode.VelocityChange);
                }
            }
            else if (m_GameOver)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    ReloadGame();
                }
            }
        }
    }

    void AddPoint(int point)
    {
        m_Points += point;
        ScoreText.text = $"Score : {m_Points}";
        if (m_Points > m_HighScore)
        {
            m_HighScore = m_Points;
            m_HighScoreName = m_PlayerName;
            HighScoreText.text = "High Score: " + m_HighScoreName + ": " + m_HighScore;
            SaveHighScore();
        }
    }

    public void GameOver()
    {
        m_GameOver = true;
        GameOverText.SetActive(true);
    }
    void ReloadGame()
    {
        m_GameOver = false;
        m_Started = false;
        m_GameSetUp = false;
        m_Points = 0;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    [System.Serializable]
    public class SaveData
    {
        public string highScoreName;
        public int highScore;
    }
    public void SaveHighScore()
    {
        SaveData data = new SaveData();
        data.highScoreName = m_HighScoreName;
        data.highScore = m_HighScore;
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(Application.persistentDataPath + "/savefile.json", json);
    }
    public void LoadHighScore()
    {
        string path = Application.persistentDataPath + "/savefile.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            m_HighScoreName = data.highScoreName;
            m_HighScore = data.highScore;
            HighScoreText.text = $"High Score: {m_HighScoreName} : {m_HighScore}";
        }
    }

}
