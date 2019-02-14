using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public enum SceneBuildIndex
    {
        MainMenu   = 0,
        Game       = 1,
        LevelMaker = 2
    }

    public void PlayGame()
    {
        SceneManager.LoadScene((int)SceneBuildIndex.Game);
    }

    public void MakeLevel()
    {
        SceneManager.LoadScene((int)SceneBuildIndex.LevelMaker);
    }
}
