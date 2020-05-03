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
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        canvasMenu.SetActive(true);

    }

    public void tpElected(GameObject election)
    {
        tpDestination = election;
        player.GetComponent<CharacterController>().enabled = false;
        var transformPosition = tpDestination.transform.position;
        transformPosition.y += 1;
        player.transform.position = transformPosition;
        player.GetComponent<CharacterController>().enabled = true;
        canvasMenu.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
