using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 视差效果管理器
/// 用于管理多个背景层的视差效果，可以一次性设置多个背景层
/// </summary>
public class ParallaxManager : MonoBehaviour
{
    [System.Serializable]
    public class ParallaxLayer
    {
        public Transform layerTransform; // 背景层的变换组件
        [Range(0f, 1f)]
        public float parallaxFactorX = 0.5f; // X轴视差因子
        [Range(0f, 1f)]
        public float parallaxFactorY = 0.5f; // Y轴视差因子
        public bool infiniteHorizontal = false; // 是否水平无限重复

        [HideInInspector]
        public Vector3 startPosition; // 初始位置
        [HideInInspector]
        public float textureUnitSizeX; // 纹理单位大小X
    }

    [Header("视差层设置")]
    [Tooltip("所有需要视差效果的背景层")]
    public List<ParallaxLayer> parallaxLayers = new List<ParallaxLayer>(); // 视差层列表

    [Header("全局设置")]
    [Tooltip("是否使用平滑过渡")]
    public bool smoothTransition = true; // 是否使用平滑过渡

    [Tooltip("平滑过渡速度")]
    public float smoothSpeed = 5f; // 平滑过渡速度

    [Tooltip("是否自动查找并添加子对象作为视差层")]
    public bool autoAddChildrenAsLayers = false; // 是否自动添加子对象作为视差层

    [Tooltip("自动添加时的视差因子递减值")]
    [Range(0.05f, 0.5f)]
    public float autoFactorDecrement = 0.1f; // 自动添加时的视差因子递减值

    private Transform cameraTransform; // 相机变换组件
    private Vector3 lastCameraPosition; // 上一帧相机位置
    private Dictionary<Transform, Vector3> targetPositions = new Dictionary<Transform, Vector3>(); // 目标位置字典

    void Start()
    {
        // 获取主相机的变换组件
        cameraTransform = Camera.main.transform;
        // 记录相机初始位置
        lastCameraPosition = cameraTransform.position;

        // 如果启用了自动添加子对象作为视差层
        if (autoAddChildrenAsLayers)
        {
            AutoAddChildrenAsLayers();
        }

        // 初始化所有视差层
        InitializeParallaxLayers();
    }

    /// <summary>
    /// 自动添加子对象作为视差层
    /// </summary>
    private void AutoAddChildrenAsLayers()
    {
        // 清空现有的视差层列表
        parallaxLayers.Clear();

        // 获取所有子对象
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);

            // 创建新的视差层
            ParallaxLayer layer = new ParallaxLayer
            {
                layerTransform = child,
                // 视差因子随着层数递减，越靠后的层移动越慢
                parallaxFactorX = 1.0f - (i * autoFactorDecrement),
                parallaxFactorY = 1.0f - (i * autoFactorDecrement),
                infiniteHorizontal = false // 默认不启用无限重复
            };

            // 确保视差因子不小于0
            layer.parallaxFactorX = Mathf.Max(0.1f, layer.parallaxFactorX);
            layer.parallaxFactorY = Mathf.Max(0.1f, layer.parallaxFactorY);

            // 添加到视差层列表
            parallaxLayers.Add(layer);
        }
    }

    /// <summary>
    /// 初始化所有视差层
    /// </summary>
    private void InitializeParallaxLayers()
    {
        foreach (ParallaxLayer layer in parallaxLayers)
        {
            if (layer.layerTransform != null)
            {
                // 记录初始位置
                layer.startPosition = layer.layerTransform.position;
                targetPositions[layer.layerTransform] = layer.startPosition;

                // 如果需要无限重复，计算纹理单位大小
                if (layer.infiniteHorizontal)
                {
                    SpriteRenderer spriteRenderer = layer.layerTransform.GetComponent<SpriteRenderer>();
                    if (spriteRenderer != null && spriteRenderer.sprite != null)
                    {
                        Sprite sprite = spriteRenderer.sprite;
                        Texture2D texture = sprite.texture;
                        layer.textureUnitSizeX = (texture.width / sprite.pixelsPerUnit) * layer.layerTransform.localScale.x;
                    }
                    else
                    {
                        Debug.LogWarning("ParallaxManager: 无法获取SpriteRenderer或Sprite组件，无限重复功能可能无法正常工作: " + layer.layerTransform.name);
                        layer.infiniteHorizontal = false;
                    }
                }
            }
            else
            {
                Debug.LogWarning("ParallaxManager: 视差层的变换组件为空");
            }
        }
    }

    void LateUpdate()
    {
        // 计算相机移动的增量
        Vector3 deltaMovement = cameraTransform.position - lastCameraPosition;

        // 更新每个视差层
        foreach (ParallaxLayer layer in parallaxLayers)
        {
            if (layer.layerTransform != null)
            {
                // 计算视差效果的移动量
                float parallaxX = deltaMovement.x * layer.parallaxFactorX;
                float parallaxY = deltaMovement.y * layer.parallaxFactorY;

                // 更新目标位置
                Vector3 targetPosition = targetPositions[layer.layerTransform];
                targetPosition += new Vector3(parallaxX, parallaxY, 0);
                targetPositions[layer.layerTransform] = targetPosition;

                // 如果启用了平滑过渡
                if (smoothTransition)
                {
                    // 使用插值平滑过渡到目标位置
                    layer.layerTransform.position = Vector3.Lerp(
                        layer.layerTransform.position,
                        targetPosition,
                        Time.deltaTime * smoothSpeed
                    );
                }
                else
                {
                    // 直接设置位置
                    layer.layerTransform.position = targetPosition;
                }

                // 如果启用了无限水平重复
                if (layer.infiniteHorizontal && layer.textureUnitSizeX > 0)
                {
                    // 计算相机和背景在X轴上的距离
                    float distanceX = cameraTransform.position.x - layer.layerTransform.position.x;

                    // 如果距离超过了一个纹理单位，重新定位背景
                    if (Mathf.Abs(distanceX) >= layer.textureUnitSizeX)
                    {
                        // 计算需要移动的纹理单位数量
                        float offsetPositionX = (distanceX > 0) ? layer.textureUnitSizeX : -layer.textureUnitSizeX;

                        // 更新目标位置和当前位置
                        Vector3 newTargetPos = targetPositions[layer.layerTransform];
                        newTargetPos.x += offsetPositionX;
                        targetPositions[layer.layerTransform] = newTargetPos;

                        Vector3 newPos = layer.layerTransform.position;
                        newPos.x += offsetPositionX;
                        layer.layerTransform.position = newPos;
                    }
                }
            }
        }

        // 更新上一帧相机位置
        lastCameraPosition = cameraTransform.position;
    }

    /// <summary>
    /// 添加一个新的视差层
    /// </summary>
    public void AddParallaxLayer(Transform layerTransform, float parallaxFactorX, float parallaxFactorY, bool infiniteHorizontal = false)
    {
        ParallaxLayer newLayer = new ParallaxLayer
        {
            layerTransform = layerTransform,
            parallaxFactorX = parallaxFactorX,
            parallaxFactorY = parallaxFactorY,
            infiniteHorizontal = infiniteHorizontal,
            startPosition = layerTransform.position
        };

        parallaxLayers.Add(newLayer);
        targetPositions[layerTransform] = layerTransform.position;

        // 如果需要无限重复，计算纹理单位大小
        if (infiniteHorizontal)
        {
            SpriteRenderer spriteRenderer = layerTransform.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null && spriteRenderer.sprite != null)
            {
                Sprite sprite = spriteRenderer.sprite;
                Texture2D texture = sprite.texture;
                newLayer.textureUnitSizeX = (texture.width / sprite.pixelsPerUnit) * layerTransform.localScale.x;
            }
            else
            {
                Debug.LogWarning("ParallaxManager: 无法获取SpriteRenderer或Sprite组件，无限重复功能可能无法正常工作: " + layerTransform.name);
                newLayer.infiniteHorizontal = false;
            }
        }
    }

    /// <summary>
    /// 移除一个视差层
    /// </summary>
    public void RemoveParallaxLayer(Transform layerTransform)
    {
        for (int i = 0; i < parallaxLayers.Count; i++)
        {
            if (parallaxLayers[i].layerTransform == layerTransform)
            {
                parallaxLayers.RemoveAt(i);
                if (targetPositions.ContainsKey(layerTransform))
                {
                    targetPositions.Remove(layerTransform);
                }
                return;
            }
        }
    }

    /// <summary>
    /// 重置所有视差层到初始位置
    /// </summary>
    public void ResetAllLayers()
    {
        foreach (ParallaxLayer layer in parallaxLayers)
        {
            if (layer.layerTransform != null)
            {
                layer.layerTransform.position = layer.startPosition;
                targetPositions[layer.layerTransform] = layer.startPosition;
            }
        }
    }
}