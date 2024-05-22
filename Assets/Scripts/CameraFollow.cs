using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform followPlayer;
    private Transform cameraTransform;

    public Vector3 playerOffset;

    public float MoveSpeed = 400f;
    
        
    void Start()
    {
        cameraTransform = transform;    
    }


    public void SetTarget(Transform newTransformTarget) //actializa el transform del player
    {
        followPlayer = newTransformTarget;
    }

    private void LateUpdate()
    {
        if(followPlayer != null)
        {
            cameraTransform.position = Vector3.Lerp(cameraTransform.position, followPlayer.position + playerOffset, MoveSpeed * Time.deltaTime); //Interpola entre los dos vectores.
        }
    }
}
