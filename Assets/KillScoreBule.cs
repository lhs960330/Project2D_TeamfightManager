using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class KillScoreBule : MonoBehaviour
{
    [SerializeField] TMP_Text Blue;
    private void Update()
    {
       Blue.text = Manager.Game.GetBuleScore().ToString();
    }
}
