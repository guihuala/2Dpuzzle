using System;
using UnityEngine;

public static class EVENTMGR
{
    // 游戏需要用到的主要事件

    #region 进入可交互物体的UI提示

    // 进入可交互物体
    public static event Action<string> OnEnterInteractive;

    public static void TriggerEnterInteractive(string eventText)
    {
        OnEnterInteractive?.Invoke(eventText);
    }

    // 离开可交互物体
    public static event Action OnExitInteractive;

    public static void TriggerExitInteractive()
    {
        OnExitInteractive?.Invoke();
    }

    #endregion

}
