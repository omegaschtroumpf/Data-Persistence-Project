using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MenuUIHandler : MonoBehaviour
{
    public void StartNew()
    {
        MainManager.Instance.m_PlayerName = GameObject.Find("PlayerNameText").GetComponent<Text>().text;
        SceneManager.LoadScene(1);
    }

    // Update is called once per frame
    public void ExitGame()
    {
        MainManager.Instance.SaveHighScore();
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }
}
