using UnityEngine;

/// <summary>
/// 连接点类：表示可以相互连接的点
/// 处理点击、连接状态和与其他点的连接关系
/// </summary>
public class ConnectionPoint : MonoBehaviour
{
    // 是否是起始连接点
    public bool isStartPoint = false;

    // 当前点是否已经被连接
    public bool isConnected = false;

    // 与当前点相连的另一个连接点
    public ConnectionPoint connectedTo;

    // 当前点是否可以被连接
    public bool canConnect = true;

    /// <summary>
    /// 当点击此连接点时，开始一个新的连接
    /// </summary>
    private void OnMouseDown()
    {
        // 只有起始点或者当前正在拖拽时才能响应点击
        if (isStartPoint || LineController.instance.isDragging)
        {
            LineController.instance.StartConnection(this);
        }
    }

    /// <summary>
    /// 当鼠标进入此连接点时，如果正在拖动连线则尝试建立连接
    /// </summary>
    private void OnMouseEnter()
    {
        if (LineController.instance.isDragging)
        {
            LineController.instance.TryConnect(this);
        }
    }

    /// <summary>
    /// 与另一个连接点建立连接
    /// </summary>
    /// <param name="other">要连接的目标点</param>
    public void Connect(ConnectionPoint other)
    {
        if (!canConnect || isConnected) return;

        isConnected = true;
        connectedTo = other;
    }

    /// <summary>
    /// 断开当前连接
    /// 重置连接状态和引用
    /// </summary>
    public void Disconnect()
    {
        isConnected = false;
        connectedTo = null;
    }
}