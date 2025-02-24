using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

/// <summary>
/// 解谜管理器：负责管理整个解谜游戏的状态和进度
/// 使用单例模式确保全局只有一个实例
/// </summary>
public class PuzzleManager : MonoBehaviour
{
    // 单例实例
    public static PuzzleManager instance;

    // 存储所有连接点的列表
    public List<ConnectionPoint> connectionPoints;

    // 指定的起始连接点
    public ConnectionPoint startingPoint;

    // 当解谜完成时触发的事件
    public UnityEvent onPuzzleComplete;

    /// <summary>
    /// 初始化时设置单例实例和起始点
    /// </summary>
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        if (startingPoint != null)
        {
            startingPoint.isStartPoint = true;
        }
    }

    /// <summary>
    /// 检查解谜是否完成
    /// 遍历所有连接点，如果所有可连接的点都已连接，则触发完成事件
    /// </summary>
    public void CheckPuzzleCompletion()
    {
        bool isComplete = true;
        foreach (ConnectionPoint point in connectionPoints)
        {
            // 如果存在可连接但未连接的点，则解谜未完成
            if (!point.isConnected && point.canConnect)
            {
                isComplete = false;
                break;
            }
        }

        // 如果所有点都已正确连接，触发完成事件
        if (isComplete)
        {
            onPuzzleComplete?.Invoke();
            Debug.Log("解谜完成！");
        }
    }

    /// <summary>
    /// 重置解谜状态
    /// 断开所有连接点的连接并清除所有连线
    /// </summary>
    public void ResetPuzzle()
    {
        // 断开所有连接点
        foreach (ConnectionPoint point in connectionPoints)
        {
            point.Disconnect();
        }

        // 销毁场景中所有的线条对象
        LineRenderer[] lines = FindObjectsOfType<LineRenderer>();
        foreach (LineRenderer line in lines)
        {
            Destroy(line.gameObject);
        }
    }
}