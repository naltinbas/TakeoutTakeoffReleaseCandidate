using System;
using UnityEngine;
using UnityEngine.UI;

public class QuitOnAnyKeyPress : MonoBehaviour
{
    private void Update()
    {
        if(Input.anyKey)
            Application.Quit();
    }
}
