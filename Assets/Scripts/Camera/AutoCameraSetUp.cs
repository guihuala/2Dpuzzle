using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class AutoCameraSetup : MonoBehaviour
{
    public bool autoSetupCameraFollow = true;

    
    public string cameraFollowGameObjectName = "Player";

    void Awake ()
    {
        if(!autoSetupCameraFollow)
            return;

        CinemachineVirtualCamera cam = GetComponent<CinemachineVirtualCamera>();
        
        
        if(cam == null)
            throw new UnityException("Virtual Camera was not found, default follow cannot be assigned.");
        
        var rootObj = gameObject.scene.GetRootGameObjects();
        GameObject cameraFollowGameObject = null;
        
        foreach (var go in rootObj)
        {
            if (go.name == cameraFollowGameObjectName)
                cameraFollowGameObject = go;
            else
            {
                var t = go.transform.Find(cameraFollowGameObjectName);
                if (t != null)
                    cameraFollowGameObject = t.gameObject;
            }

            if (cameraFollowGameObject != null) break;
        }
        
        if(cameraFollowGameObject == null)
            throw new UnityException("GameObject called " + cameraFollowGameObjectName + " was not found, default follow cannot be assigned.");

        if (cam.Follow == null)
        {
            cam.Follow = cameraFollowGameObject.transform;
        }
    }
}