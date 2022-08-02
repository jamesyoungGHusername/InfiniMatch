using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class PuzzleListManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void MainMenuTransition()
    {
        SceneManager.LoadScene("StartScreen");
    }
    public void TestLevelTransition()
    {
        SceneManager.LoadScene("TestPuzzle");
    }
}
