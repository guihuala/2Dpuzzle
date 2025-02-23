using System;

[Serializable]
public class MechanismState
{
    public string mechanismID;
    public bool isTriggered;
    
    public bool isActivated;    // 机关是否被激活（灯）
    public bool isTaken;        // 机关是否被取走（可以被取走的物品）
    
    // 如果有更多状态可以继续在这里添加
}

public interface ISaveableMechanism
{
    // 返回机关当前的状态
    MechanismState SaveState();

    // 根据传入的状态恢复机关
    void LoadState(MechanismState state);
}
