using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Minimap : MonoBehaviour
{
    [SerializeField] private WaveController waveController;
    [SerializeField] private ThirdPersonCharacterController player;

    [SerializeField] private List<Image> zones;

    private bool[] playerPos;

    private bool[] activeZones;

    private int lastPlace;

    // Start is called before the first frame update
    void Start()
    {
        activeZones = waveController.SpawnersActivated;
        playerPos = new bool[]{true,false,false,false};
        lastPlace = 1;
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < 4; i++)
        {
            if (activeZones[i])
            {
                if (playerPos[i])
                    zones[i].color = Color.blue;
                else
                    zones[i].color = Color.red;
            }
            else
            {
                if (playerPos[i])
                    zones[i].color = Color.green;
                else
                    zones[i].color = Color.white;
            }
        }
    }

    public void setPlayerPos(int index)
    {
        playerPos[index] = true;
        playerPos[lastPlace] = false;
        lastPlace = index;
    }
}