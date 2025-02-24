using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class BaseInteractableObject : MonoBehaviour, Interactable, ISaveableMechanism
{
    private bool canInteract = true;
    private bool isLit = false;
    protected bool isActivated = false; // 机关是否被激活 比如灯是否被打开
    protected bool isTaken = false;    // 是否被取走 比如物品和收藏品

    private SpriteRenderer spriteRenderer;

    [SerializeField] [TextArea] protected string interactInfo;
    [SerializeField] protected Material interactOutlineMaterial;
    [SerializeField] protected Material defaultMaterial;
    [SerializeField] private bool requiresLightToActivate = true;

    protected string uniqueID;  // 每个BaseInteractableObject的唯一标识

    protected virtual void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // 生成一个全局唯一的标识符
        uniqueID = gameObject.name;

        // 注册机关到游戏进度管理器
        GameProgressManager.Instance.RegisterMechanism(uniqueID, this);

        // 加载物体的状态
        LoadStateFromManager();
    }

    private void Update()
    {
        Light2D[] lights = FindObjectsOfType<Light2D>();
        isLit = false;

        foreach (var light in lights)
        {
            if (IsInLightRange(light))
            {
                isLit = true;
                break;
            }
        }

        if (requiresLightToActivate)
        {
            canInteract = isLit;
        }
    }

    private bool IsInLightRange(Light2D light)
    {
        float distanceToLight = Vector2.Distance(transform.position, light.transform.position);
        return distanceToLight <= light.pointLightOuterRadius;
    }

    public void Interact()
    {
        if (canInteract)
            Apply();
        else
            Debug.Log("Interaction is disabled, either no light detected or light is required.");
    }

    protected virtual void Apply()
    {
        Debug.Log("Interacted with " + gameObject.name);
    }

    public virtual void Enter()
    {
        if (!canInteract)
            return;

        EVENTMGR.TriggerEnterInteractive(interactInfo);

        if (interactOutlineMaterial != null)
            spriteRenderer.material = interactOutlineMaterial;
    }

    public virtual void Exit()
    {
        if (!canInteract)
            return;

        EVENTMGR.TriggerExitInteractive();

        if (interactOutlineMaterial != null)
            spriteRenderer.material = defaultMaterial;
    }
    

    // 加载状态
    public void LoadState(MechanismState state)
    {
        isActivated = state.isActivated;
        isTaken = state.isTaken;
    }

    // 自动从GameProgressManager中加载状态
    private void LoadStateFromManager()
    {
        var state = GameProgressManager.Instance.GetMechanismStateByID(uniqueID);
        if (state != null)
        {
            LoadState(state);
        }
    }
    
    public MechanismState SaveState()
    {
        return new MechanismState { isActivated = isActivated, isTaken = isTaken };
    }

    // 获取物体的唯一ID
    public string GetUniqueID()
    {
        return uniqueID;
    }
}
