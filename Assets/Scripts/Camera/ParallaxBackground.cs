using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 视差背景效果组件
/// 将此脚本附加到背景图层上，可以实现背景移动比角色慢的视差效果
/// </summary>
public class ParallaxBackground : MonoBehaviour
{
    [Header("视差效果参数")]
    [Tooltip("视差效果强度，值越小移动越慢，0表示不移动，1表示与相机同速移动")]
    [Range(0f, 1f)]
    public float parallaxEffectX = 0.5f; // X轴视差效果强度

    [Tooltip("Y轴视差效果强度，值越小移动越慢，0表示不移动，1表示与相机同速移动")]
    [Range(0f, 1f)]
    public float parallaxEffectY = 0.5f; // Y轴视差效果强度

    [Tooltip("是否无限重复背景")]
    public bool infiniteHorizontal = false; // 是否在水平方向无限重复

    [Tooltip("是否平滑过渡")]
    public bool smoothTransition = true; // 是否使用平滑过渡

    [Tooltip("平滑过渡速度")]
    public float smoothSpeed = 5f; // 平滑过渡速度

    private Transform cameraTransform; // 相机变换组件
    private Vector3 lastCameraPosition; // 上一帧相机位置
    private float textureUnitSizeX; // 背景纹理单位大小X
    private float textureUnitSizeY; // 背景纹理单位大小Y
    private Vector3 targetPosition; // 目标位置

    void Start()
    {
        // 获取主相机的变换组件
        cameraTransform = Camera.main.transform;
        // 记录相机初始位置
        lastCameraPosition = cameraTransform.position;
        // 记录初始位置
        targetPosition = transform.position;

        // 如果需要无限重复，计算纹理单位大小
        if (infiniteHorizontal)
        {
            Sprite sprite = GetComponent<SpriteRenderer>()?.sprite;
            if (sprite != null)
            {
                // 计算世界空间中的纹理单位大小
                Texture2D texture = sprite.texture;
                textureUnitSizeX = (texture.width / sprite.pixelsPerUnit) * transform.localScale.x;
                textureUnitSizeY = (texture.height / sprite.pixelsPerUnit) * transform.localScale.y;
            }
            else
            {
                Debug.LogWarning("ParallaxBackground: 无法获取SpriteRenderer或Sprite组件，无限重复功能可能无法正常工作");
                infiniteHorizontal = false;
            }
        }
    }

    void LateUpdate()
    {
        // 计算相机移动的增量
        Vector3 deltaMovement = cameraTransform.position - lastCameraPosition;

        // 计算视差效果的移动量
        float parallaxX = deltaMovement.x * parallaxEffectX;
        float parallaxY = deltaMovement.y * parallaxEffectY;

        // 更新目标位置
        targetPosition += new Vector3(parallaxX, parallaxY, 0);

        // 如果启用了平滑过渡
        if (smoothTransition)
        {
            // 使用插值平滑过渡到目标位置
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * smoothSpeed);
        }
        else
        {
            // 直接设置位置
            transform.position = targetPosition;
        }

        // 如果启用了无限水平重复
        if (infiniteHorizontal)
        {
            // 计算相机和背景在X轴上的距离
            float distanceX = cameraTransform.position.x - transform.position.x;

            // 如果距离超过了一个纹理单位，重新定位背景
            if (Mathf.Abs(distanceX) >= textureUnitSizeX)
            {
                // 计算需要移动的纹理单位数量
                float offsetPositionX = (distanceX > 0) ? textureUnitSizeX : -textureUnitSizeX;

                // 更新目标位置和当前位置
                targetPosition.x += offsetPositionX;
                transform.position = new Vector3(transform.position.x + offsetPositionX, transform.position.y, transform.position.z);
            }
        }

        // 更新上一帧相机位置
        lastCameraPosition = cameraTransform.position;
    }
}