using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/*
 * TO DO: Add vibrations and sound! End of game scoring.
 */
public class GameManager : MonoBehaviour
{

    [HideInInspector] public GameObject selectedBlock;
    [HideInInspector] public bool hasSelected;
    public int currentLevel;
    public bool hasNextLevel;
    public GameObject board;
    public GameObject veil;
    public int score;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI gameOverText;
    public TextMeshProUGUI timeRemainingText;
    public Button gameOverBackground;
    public Button restartButton;
    public TextMeshProUGUI addedTimeText;
    public TextMeshProUGUI bestScoreText;
    [HideInInspector] public float timeRemaining=20.0f;
    private bool timerRunning = false;
    [HideInInspector] public bool active;
    private int savedHighScore=0;
    public Button timerBar;
    public Button startButton;
    [HideInInspector] public float barAnimatingValue;
    [HideInInspector] public bool readyForEndScreen=false;
    private AudioSource timerAudio;
    public bool isTutorial = false;
    public int ScoreGoal = 300;
    public bool scored = true;
    public bool won = false;
    public Button nextLevel;
    public bool isZen = false;
    private bool passed = false;
    // Start is called before the first frame update
    void Start()
    {
        LoadPrefs();
        if (!isZen)
        {
            bestScoreText.text = "Level " + currentLevel;
        }
    }

    public void StartButtonPressed()
    {
        startButton.gameObject.SetActive(false);
        veil.gameObject.SetActive(false);
        active = true;
        timeRemainingText.text = timeRemaining.ToString("F1") + "s";
        
        
        timerBar.GetComponent<Image>().color = (Color.green);
        barAnimatingValue = timeRemaining;
        if (!isTutorial || !isZen)
        {
            timerRunning = true;
        }
        else
        {

        }
        timerAudio = GetComponent<AudioSource>();
        //Debug.Log(timerAudio.isPlaying);
    }
    // Update is called once per frame
    void Update()
    {
        
        if (won && scored && active)
        {
            Debug.Log("Win condition detected.");

            timerAudio.Stop();
            timerRunning = false;
            active = false;
            hasSelected = false;
            if (selectedBlock != null)
            {
                selectedBlock.GetComponent<Block>().selected = false;
                selectedBlock.transform.localScale = new Vector3(1, 1, 1);
            }
            board.GetComponent<Board>().EndTurn();
            while (score < board.GetComponent<Board>().score)
            {
                updateScore();
                scoreText.text = "Score: " + score;
            }
            StartCoroutine(winLevel());
        }
        else if (isTutorial && board.GetComponent<Board>().firstMoveCompleted && board.GetComponent<Board>().blocks[4,3]!=selectedBlock && !passed)
        {

            timerAudio.Stop();
            timerRunning = false;
            //timeRemainingText.text = "0.0s";
            active = false;
            hasSelected = false;
            
            board.GetComponent<Board>().EndTurn();
            
            gameOverText.text = "Try Again.";
                
            
            StartCoroutine(failLevel());



        }
        else if (board.GetComponent<Board>().firstMoveCompleted && board.GetComponent<Board>().blocks[4, 3] == selectedBlock)
        {
            passed = true;
        }
        else
        {
            if (timerRunning && !isZen)
            {
                if (timeRemaining < 5.0f && timeRemaining > 0.0f && !timerAudio.isPlaying)
                {
                    timerAudio.Play();
                }
            }
            updateScore();
            if (scored)
            {
                scoreText.text = "Score: " + score + "/" + ScoreGoal;
            }
            else
            {
                scoreText.text = "Score: " + score;
            }

            if (active)
            {
                if (timeRemaining <= 10.0f)
                {
                    timerBar.GetComponent<Image>().color = Color.yellow;
                }
                else if (timeRemaining <= 5)
                {
                    timerBar.GetComponent<Image>().color = Color.red;

                }
                else
                {
                    timerBar.GetComponent<Image>().color = Color.green;
                }

                if (timeRemaining <= 2.0f)
                {
                    timeRemainingText.text = timeRemaining.ToString("F1") + "s";
                    //timeRemainingText.color = Color.red;

                }
                else
                {
                    timeRemainingText.text = timeRemaining.ToString("F1") + "s";
                }
            }

            if (timerRunning && timeRemaining > 0f)
            {
                timeRemaining -= Time.deltaTime;
                barAnimatingValue = timeRemaining;
                if (timeRemaining > 5)
                {
                    timerBar.GetComponent<RectTransform>().sizeDelta = new Vector2(barAnimatingValue * 25, 50f);
                }
                else
                {
                    timerBar.GetComponent<Image>().color = Color.red;
                }

            }

            else
            {
                if (barAnimatingValue < timeRemaining)
                {
                    barAnimatingValue += 0.1f;
                    if (timeRemaining > 5)
                    {
                        timerBar.GetComponent<RectTransform>().sizeDelta = new Vector2(barAnimatingValue * 25, 50f);
                    }
                }

            }
            if (timeRemaining <= 0.0f && !isZen)
            {
                timerAudio.Stop();
                timerRunning = false;
                timeRemainingText.text = "0.0s";
                active = false;
                hasSelected = false;
                if (selectedBlock != null)
                {
                    selectedBlock.GetComponent<Block>().selected = false;
                    selectedBlock.transform.localScale = new Vector3(1, 1, 1);
                }
                board.GetComponent<Board>().EndTurn();
                while (score < board.GetComponent<Board>().score)
                {
                    updateScore();
                    scoreText.text = "Score: " + score;
                }
                if (board.GetComponent<Board>().score > savedHighScore)
                {
                    gameOverText.text = "NEW HIGH SCORE!";
                    SaveNewHighScore(board.GetComponent<Board>().score);
                }
                StartCoroutine(endGame());

            }
        }

        
    }
    IEnumerator failLevel()
    {
        yield return new WaitForSeconds(1);
        restartButton.gameObject.SetActive(true);
        gameOverText.gameObject.SetActive(true);
        gameOverBackground.gameObject.SetActive(true);
    }
    IEnumerator endGame()
    {
        
        yield return new WaitUntil(() => readyForEndScreen && score == board.GetComponent<Board>().score);
        //yield return new WaitForSeconds(5.0f);
        restartButton.gameObject.SetActive(true);
        gameOverText.gameObject.SetActive(true);
        gameOverBackground.gameObject.SetActive(true);
    }
    IEnumerator winLevel()
    {
        yield return new WaitUntil(() => readyForEndScreen && score == board.GetComponent<Board>().score);
        if (hasNextLevel)
        {
            gameOverText.text = "LEVEL COMPLETE";
        }
        else
        {
            gameOverText.text = "YOU WIN";
            nextLevel.GetComponentInChildren<TextMeshProUGUI>().text = "Main Menu";
        }
        gameOverText.gameObject.SetActive(true);
        nextLevel.gameObject.SetActive(true);
        gameOverBackground.gameObject.SetActive(true);
    }
    public void MoveToNextLevel()
    {
        if (hasNextLevel) {
            PlayerPrefs.SetInt("LastLevel", currentLevel + 1);
            PlayerPrefs.Save();
            SceneManager.LoadScene("Level" + (currentLevel + 1));
        }
        else
        {
            SceneManager.LoadScene("StartScreen");
        }
    }
    void updateScore()
    {
        
        if (score < board.GetComponent<Board>().score)
        {
            if (board.GetComponent<Board>().score - score >= 100)
            {
                score += 10;
            }
            else
            {
                score += 1;
            }
        }
    }
    public void restartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void BackAction()
    {
        SceneManager.LoadScene("StartScreen");
    }
    public void startTimer()
    {
        timerRunning = true;
        
    }
    public void stopTimer()
    {
        timerRunning = false;
        //timeRemaining = 10.0f;
        timeRemainingText.text = timeRemaining.ToString("F1") + "s";
        if (timerAudio.isPlaying)
        {
            timerAudio.Stop();
        }
    }
    public void addTime(float t)
    {
        timeRemaining += t;
        timeRemainingText.text = timeRemaining.ToString("F1") + "s";
    }
    public void SaveNewHighScore(int s)
    {
        if (isZen)
        {
            PlayerPrefs.SetInt("BestScore", s);
            PlayerPrefs.Save();
        }
    }

    public void LoadPrefs()
    {
        savedHighScore = PlayerPrefs.GetInt("BestScore", 0);
    }
}
