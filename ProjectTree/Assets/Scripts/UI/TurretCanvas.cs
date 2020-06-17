using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretCanvas : MonoBehaviour
{
    public GameObject TooltipBox;
    private void OnEnable()
    {
        TooltipBox.SetActive(true);
    }
}
