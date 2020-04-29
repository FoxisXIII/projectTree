using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    [SerializeField]
    private TeleportManager TPManager;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
        print(other.name);
        if (other.CompareTag("Teleporter") && Input.GetKeyDown(KeyCode.T))
        {
            print("Teleporting");
            TPManager.teleport();
        }
    }
}
