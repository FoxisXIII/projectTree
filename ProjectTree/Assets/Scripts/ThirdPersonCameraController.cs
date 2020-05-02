using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

public class ThirdPersonCameraController : MonoBehaviour
{

    public float rotationSpeed = 1;
    public Transform Target, Player;
    private Transform Obst;
    private float zoombSpeed = 2f;

    private float mauseX, mauseY;
    // Start is called before the first frame update
    void Start()
    {
        Obst = Target;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    
    private void LateUpdate()
    {
        CameraCon();
        viewObs();
    }

    void CameraCon()
    {
        mauseX += Input.GetAxis("Mouse X") * rotationSpeed;
        mauseY += Input.GetAxis("Mouse Y") * rotationSpeed;
        mauseY = Mathf.Clamp(mauseY, -35, 60);
        
        transform.LookAt(Target);

        if (Input.GetKey(KeyCode.LeftShift))
        {
            Target.rotation=Quaternion.Euler(mauseY,mauseX,0);
        }
        else
        {
            Target.rotation=Quaternion.Euler(mauseY,mauseX,0);
            Player.rotation= Quaternion.Euler(0,mauseX,0);
        }
        
    }
    
    
    void viewObs()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position,Target.position-transform.position,out hit,4.5f))
        {
            if (hit.collider.gameObject.tag!="Player")
            {
                Obst = hit.transform;
                Obst.gameObject.GetComponent<MeshRenderer>().shadowCastingMode =
                    UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
                if (Vector3.Distance(Obst.position,transform.position)>=3f && Vector3.Distance(transform.position, Target.position)>=1.5f)
                {
                    transform.Translate(Vector3.forward*zoombSpeed*Time.deltaTime);
                }
            }
            else
            {
                if(Obst!=Target)
                    Obst.gameObject.GetComponent<MeshRenderer>().shadowCastingMode =
                    UnityEngine.Rendering.ShadowCastingMode.On;
                if (Vector3.Distance(transform.position,Target.position)<4.5f)
                {
                    transform.Translate(Vector3.back*zoombSpeed*Time.deltaTime);
                }
            }
            
        }
    }
}
