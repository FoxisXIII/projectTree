using System;
using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class Base : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] private float life;

    private float maxLife;

    //The base generates them by time?
    private int energyCreation;
    private int materialCreation;

    public Image lifeUI_1;
<<<<<<< HEAD
    public int lastDamageWave;

    [Header("FMOD paths")] public string baseDestroySoundPath;

=======
    
    [Header("FMOD paths")]
    public string baseDestroySoundPath;
    public string lowLifeSoundPath;
    private EventInstance lowLifeSoundEvent = new EventInstance();
>>>>>>> develop

    private void Awake()
    {
        GameController.GetInstance().Base = this;
        maxLife = life;
    }

    public void ReceiveDamage(int damage)
    {
        life -= damage;
        lifeUI_1.fillAmount = life / maxLife;
        if (lastDamageWave != GameController.GetInstance().WaveCounter)
        {
            lastDamageWave = GameController.GetInstance().WaveCounter;
            GameController.GetInstance().NoBaseDamage = false;
        }

        if (life <= 0)
        {
            SoundManager.GetInstance().PlayOneShotSound(baseDestroySoundPath, transform.position);
            GameController.GetInstance().gameOver("THEY HAVE ENT... BZZZ BZZZ BZZZ");
        }
    }

    public void Heal(int heal)
    {
        life = Mathf.Min(maxLife, life + heal);
        lifeUI_1.fillAmount = life / maxLife;

        if (life <= 0.1f)
        {
            SoundManager.GetInstance().PlayOneShotSound(baseDestroySoundPath, transform.position);
            GameController.GetInstance().gameOver("THEY HAVE ENT... BZZZ BZZZ BZZZ");
        }
        else if (life <= maxLife * 0.2f && !SoundManager.GetInstance().IsPlaying(lowLifeSoundEvent))
        {
           lowLifeSoundEvent = SoundManager.GetInstance().PlayEvent(lowLifeSoundPath, transform.position, 1f); 
           GameController.GetInstance().GetLowLifeSoundEvent(lowLifeSoundEvent);
        }
    }
}