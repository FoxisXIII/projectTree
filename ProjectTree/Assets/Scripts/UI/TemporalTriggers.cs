using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemporalTriggers : MonoBehaviour
{
    [SerializeField] private int ID;


    private Minimap minimap;
    // Start is called before the first frame update
    void Start()
    {
        minimap = GameObject.Find("Minimap").GetComponent<Minimap>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            minimap.setPlayerPos(ID);
        }
    }
}