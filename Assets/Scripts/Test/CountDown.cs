using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CountDown : MonoBehaviour
{
    float setTime = 5;
    [SerializeField] TMP_Text gameTime;
    public UnityEvent unityEvent;
    [SerializeField] GameObject Respawn;
    [SerializeField] List<Image> RedWin;
    [SerializeField] List<Image> BlueWin;
    Image roundWin;
    int indexBule = 0;
    int indexRed = 0;

    bool isGoldenTime = false;


    private void Start()
    {
        gameTime.text = setTime.ToString();

    }

    private void Update()
    {
        if (gameTime.IsActive() == true)
        {
            if (setTime > 0)
                setTime -= Time.deltaTime;
            if (setTime < 0 && isGoldenTime == false)
            {
                if (Manager.Game.GetBuleScore() > Manager.Game.GetRedScore() && indexBule < BlueWin.Count)
                {
                    WinnerBlue();
                    isGoldenTime = true;
                }
                else if (Manager.Game.GetBuleScore() < Manager.Game.GetRedScore() && indexRed < RedWin.Count)
                {
                    WinnerRed();
                    isGoldenTime = true;
                }
                else if (Manager.Game.GetBuleScore() == Manager.Game.GetRedScore())
                {
                    StartCoroutine(GoldenTime());
                }

            }
            gameTime.text = ((int)setTime).ToString();

        }
    }
    
    IEnumerator GoldenTime()
    {
        isGoldenTime = true;
        Debug.Log("°ñµç Å¸ÀÓ ½ÃÀÛ!");
        yield return new WaitUntil(() => (Manager.Game.GetBuleScore() != Manager.Game.GetRedScore()));

        if (Manager.Game.GetRedScore() > Manager.Game.GetBuleScore())
        {
            WinnerRed();
            Respawn.SetActive(false);

        }
        else
        {
            WinnerBlue();
            Respawn.SetActive(false);
        }
        Debug.Log("°ñµç Å¸ÀÓ ³¡. ½ÂÀÚ °áÁ¤");
        Manager.Game.GameEnd();
    }

    public void WinnerRed()
    {
        roundWin = RedWin[indexBule];
        roundWin.gameObject.SetActive(true);
        indexRed++;
        Manager.Game.GameEnd();
        Respawn.SetActive(false);

    }

    public void WinnerBlue()
    {
        roundWin = BlueWin[indexBule];
        roundWin.gameObject.SetActive(true);
        indexBule++;
        Manager.Game.GameEnd();
        Respawn.SetActive(false);

    }
}
