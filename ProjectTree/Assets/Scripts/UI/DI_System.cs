using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DI_System : MonoBehaviour
{


    public DamageIndicator damageIndicatorPrefab = null;
    public RectTransform holder;
    public Camera camer;
    public Transform player;
    
    private Dictionary<Transform,DamageIndicator> Indicators=new Dictionary<Transform, DamageIndicator>();

    #region delegates
    public static Action<Transform> createInidcator=delegate{  };
    public static Func<Transform, bool> checkIfObjectInSinght = null;
    #endregion

    private void OnEnable()
    {
        createInidcator += create;
        checkIfObjectInSinght += inSisght;
    }


    private void OnDisable()
    {
        createInidcator -= create;
        checkIfObjectInSinght -= inSisght;
    }


    void create(Transform target)
    {
        if (Indicators.ContainsKey(target))
        {
            Indicators[target].Restart();
            return;
            
        }

        DamageIndicator newIndicator = Instantiate(damageIndicatorPrefab, holder);
        newIndicator.Register(target,player, new Action(() => { Indicators.Remove(target);} ));
        
        Indicators.Add(target,newIndicator);
    }

    bool inSisght(Transform t)
    {
        Vector3 screenpoint = camer.WorldToViewportPoint(t.position);
        return screenpoint.z > 0 && screenpoint.x > 0 && screenpoint.x < 1 && screenpoint.y > 0 && screenpoint.y < 1;
    }
    
    
}
