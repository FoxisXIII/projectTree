using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class TowerButtons : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
{
    public GameObject textToSet;
    private TextMeshProUGUI _textMeshPro;
    private string previousText;
    public string explanationText;

    private void Start()
    {
        _textMeshPro = textToSet.GetComponent<TextMeshProUGUI>();
    }

    private void OnDisable()
    {
        _textMeshPro.text = "Press C to place turrets";
        previousText = "";
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        previousText = _textMeshPro.text;
        if (_textMeshPro.enableAutoSizing)
            _textMeshPro.text = explanationText;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _textMeshPro.text = previousText;
        previousText = "";
    }
}
