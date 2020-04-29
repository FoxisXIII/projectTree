using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportManager : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> teleporters;

    [SerializeField]
    private GameObject canvasMenu, player;
    
    private GameObject tpDestination;
    public void teleport()
    {
        canvasMenu.SetActive(true);
        tpElection();
        player.transform.position = tpDestination.transform.position;
        canvasMenu.SetActive(false);
    }

    private void tpElection()
    {
        
        //tpDestination= whereClicked();
    }
}
