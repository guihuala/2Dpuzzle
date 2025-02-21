using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;
using Cinemachine;

public class CameraBreathingEffect : MonoBehaviour
{
    [Header("呼吸参数")]
    [SerializeField] private float breathSpeed = 1f;
    [SerializeField] private float breathAmount = 5f;
    [SerializeField] private float positionAmount = 0.1f;

    private Vector3 defaultPosition;
    private CinemachineVirtualCamera virtualCamera;
    private float defaultFOV;

    private void Start()
    {
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
        if (virtualCamera != null)
        {
            defaultFOV = virtualCamera.m_Lens.FieldOfView;
            defaultPosition = transform.localPosition;
        }
    }


    private void Update()
    {
        float fov = defaultFOV + Mathf.Sin(Time.time * breathSpeed) * breathAmount;
        virtualCamera.m_Lens.FieldOfView = fov;
        
        float positionOffset = Mathf.Sin(Time.time * breathSpeed) * positionAmount;
        transform.localPosition = defaultPosition + new Vector3(0, positionOffset, 0);
    }
}




