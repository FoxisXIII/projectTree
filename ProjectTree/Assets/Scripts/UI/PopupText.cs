using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;

public class PopupText : MonoBehaviour
{
    private TextMeshProUGUI textMesh;
    private void Awake()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
    }

    public void Setup(string text)
    {
        if (textMesh.enableAutoSizing)
        {
            textMesh.text = text;
            StartCoroutine(destroyText(1f));
        }
    }

    private IEnumerator destroyText(float time)
    {
        yield return new WaitForSeconds(time);
        textMesh.text = "";
        gameObject.SetActive(false);
    }
}
