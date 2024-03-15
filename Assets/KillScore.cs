using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class KillScore : MonoBehaviour
{
    [SerializeField] TMP_Text Red;
    private void Update()
    {
       Red.text = Manager.Game.GetRedScore().ToString();
    }
}
