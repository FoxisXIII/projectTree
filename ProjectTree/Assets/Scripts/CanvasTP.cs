using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CanvasTP : MonoBehaviour
{
    private TeleportManager tpManager;

    [SerializeField] private GameObject myTP;

    // Start is called before the first frame update
    void Start()
    {
        tpManager = GameObject.Find("Teleport Manager").GetComponent<TeleportManager>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void clickedTP()
    {
        tpManager.tpElected(myTP);
    }
}