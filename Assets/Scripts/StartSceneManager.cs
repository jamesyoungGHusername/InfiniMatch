using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class StartSceneManager : MonoBehaviour
{
    // Start is called before the first frame update
    public TextMeshProUGUI levelProgressText,bestZenScore;
    public Button startButton, restartButton, settingsButton,zenButton;
    public int lastLevel;
    void Start()
    {
        lastLevel = PlayerPrefs.GetInt("LastLevel", 1);
       
   
        levelProgressText.text = "Lv: "+lastLevel;
   
        bestZenScore.text = "Best: "+PlayerPrefs.GetInt("BestScore", 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void RestartAction()
    {
        PlayerPrefs.SetInt("LastLevel", 1);
    
        SceneManager.LoadScene("Level1");
    }
    public void PlayAction()
    {
        SceneManager.LoadScene("Level" + lastLevel);
    }
    public void SettingsAction()
    {
        SceneManager.LoadScene("Settings");
    }
    public void ZenAction()
    {
        SceneManager.LoadScene("GameScene");
    }
}
