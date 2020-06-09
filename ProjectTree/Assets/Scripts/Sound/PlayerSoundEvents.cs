using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSoundEvents : MonoBehaviour
{
    public string stepSoundPath;
    public string endJumpSoundPath;
    
    public void Step()
    {
        SoundManager.GetInstance().PlayOneShotSound(stepSoundPath, transform.position);
    }

    public void EndJump()
    {
        SoundManager.GetInstance().PlayOneShotSound(endJumpSoundPath, transform.position);
        Debug.Log("landing");
    }
}
