using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TiteScene : BaseScene
{
    [SerializeField] GameObject SettingButton;

    public override IEnumerator LoadingRoutine()
    {
        yield return null;

    }
    public void GameSceneLoad()
    {
        Manager.Scene.LoadScene("battlefieldScene");
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    public void Seting()
    {
        SettingButton.SetActive(true);
    }

    public void close()
    {
        SettingButton.SetActive(false);
    }

    public void OnClose(InputValue value)
    {
        if (SettingButton.active)
        {
            close();
        }
        else
        {
            ExitGame();
        }
    }
}

