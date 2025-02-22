using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class BaseInteractableObject : MonoBehaviour, Interactable, ISaveableMechanism
{
    private bool canInteract = true;
    
    private bool isLit = false;
    private bool isActivated = false; // 机关是否被激活
    private bool isTaken = false;    // 机关是否被取走

    private SpriteRenderer spriteRenderer;

    [SerializeField] [TextArea] protected string interactInfo;
    [SerializeField] protected Material interactOutlineMaterial;
    [SerializeField] protected Material defaultMaterial;
    [SerializeField] private bool requiresLightToActivate = true;

    private string uniqueID;  // 每个BaseInteractableObject的唯一标识

    protected virtual void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        uniqueID = gameObject.name;
        GameProgressManager.Instance.RegisterMechanism(uniqueID, this);
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
        isLit = state.isTriggered;
    }

    MechanismState ISaveableMechanism.SaveState()
    {
        return new MechanismState { mechanismID = uniqueID, isTriggered = isLit };
    }
}
