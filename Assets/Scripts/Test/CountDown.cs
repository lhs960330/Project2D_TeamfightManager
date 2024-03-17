using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class CountDown : MonoBehaviour
{
    float setTime = 60;
    [SerializeField] TMP_Text gameTime;
    public UnityEvent unityEvent;


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
            if (setTime < 0)
            {
                Manager.Game.GameEnd();
            }
            gameTime.text = ((int)setTime).ToString();
        }
    }


}
