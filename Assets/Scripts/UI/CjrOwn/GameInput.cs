using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : Singleton<GameInput>
{
    private const string PLAYER_PREFS_BINDINGS = "InputBindings";

    public event Action OnInteractAction;
    public event Action OnOpenBag;
    
    public event Action OnPauseAction;
    public event EventHandler OnBindingRebind;

    public enum Binding
    {
        Move_Up,
        Move_Down,
        Move_Left,
        Move_Right,
        Interact,
        Pause,
        Gamepad_Interact,
        Gamepad_Pause
    }

    private PlayerInputSystem playerInputAction;

    protected override void Awake()
    {
        base.Awake();

        playerInputAction = new PlayerInputSystem();

        if (PlayerPrefs.HasKey(PLAYER_PREFS_BINDINGS))
        {
            playerInputAction.LoadBindingOverridesFromJson(PlayerPrefs.GetString(PLAYER_PREFS_BINDINGS));
        }

        playerInputAction.Player.Enable();

        playerInputAction.Player.Interact.performed += Interact_Performed;
        playerInputAction.Player.Inventory.performed += OpenBag_performed;
        playerInputAction.Player.Pause.performed += Pause_performed;
    }

    private void OnDestroy()
    {
        playerInputAction.Player.Interact.performed -= Interact_Performed;
        playerInputAction.Player.Inventory.performed -= OpenBag_performed;
        playerInputAction.Player.Pause.performed -= Pause_performed;

        playerInputAction.Dispose();
    }

    private void Pause_performed(InputAction.CallbackContext obj)
    {
        OnPauseAction?.Invoke();

        UIManager.Instance.OpenPanel("SettingPanel");
    }

    private void OpenBag_performed(InputAction.CallbackContext obj)
    {
        OnOpenBag?.Invoke();
    }

    private void Interact_Performed(InputAction.CallbackContext obj)
    {
        OnInteractAction?.Invoke();
    }

    /// <summary>
    /// 获取归一化的移动向量
    /// </summary>
    public Vector2 GetMovementVectorNormalized()
    {
        Vector2 inputVector = playerInputAction.Player.Move.ReadValue<Vector2>();
        return inputVector.normalized;  // 仅返回二维向量
    }

    #region 重新绑定按键

    public string GetBindingText(Binding binding)
    {
        switch (binding)
        {
            default:
            case Binding.Move_Up:
                return playerInputAction.Player.Move.bindings[1].ToDisplayString();
            case Binding.Move_Down:
                return playerInputAction.Player.Move.bindings[2].ToDisplayString();
            case Binding.Move_Left:
                return playerInputAction.Player.Move.bindings[3].ToDisplayString();
            case Binding.Move_Right:
                return playerInputAction.Player.Move.bindings[4].ToDisplayString();
            case Binding.Interact:
                return playerInputAction.Player.Interact.bindings[0].ToDisplayString();
            case Binding.Pause:
                return playerInputAction.Player.Pause.bindings[0].ToDisplayString();
            case Binding.Gamepad_Interact:
                return playerInputAction.Player.Interact.bindings[1].ToDisplayString();
            case Binding.Gamepad_Pause:
                return playerInputAction.Player.Pause.bindings[1].ToDisplayString();
        }
    }

    public void RebindBinding(Binding binding, Action onActionRebind)
    {
        playerInputAction.Player.Disable();

        InputAction inputAction;
        int bindingIndex;

        switch (binding)
        {
            default:
            case Binding.Move_Up:
                inputAction = playerInputAction.Player.Move;
                bindingIndex = 1;
                break;
            case Binding.Move_Down:
                inputAction = playerInputAction.Player.Move;
                bindingIndex = 2;
                break;
            case Binding.Move_Left:
                inputAction = playerInputAction.Player.Move;
                bindingIndex = 3;
                break;
            case Binding.Move_Right:
                inputAction = playerInputAction.Player.Move;
                bindingIndex = 4;
                break;
            case Binding.Interact:
                inputAction = playerInputAction.Player.Interact;
                bindingIndex = 0;
                break;
            case Binding.Pause:
                inputAction = playerInputAction.Player.Pause;
                bindingIndex = 0;
                break;
        }

        inputAction.PerformInteractiveRebinding(bindingIndex)
            .OnComplete((callback) =>
            {
                callback.Dispose();
                playerInputAction.Player.Enable();
                onActionRebind();

                PlayerPrefs.SetString(PLAYER_PREFS_BINDINGS, playerInputAction.SaveBindingOverridesAsJson());
                PlayerPrefs.Save();

                OnBindingRebind?.Invoke(this, EventArgs.Empty);
            })
            .Start();
    }

    #endregion
}