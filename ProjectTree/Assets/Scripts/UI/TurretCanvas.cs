using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TurretCanvas : MonoBehaviour
{
    public GameObject TooltipBox;
    public TextMeshProUGUI[] costsTexts;
    private void OnEnable()
    {
        TooltipBox.SetActive(true);
    }

    public void UpdateCosts(int[] costs)
    {
        for (int i = 0; i < costs.Length; i++)
        {
            costsTexts[i].text = costs[i].ToString();
        }
    }
}
