using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 线条控制器：负责处理连线的创建、更新和销毁
/// 管理鼠标拖拽绘制线条的整个过程
/// 使用单例模式确保全局只有一个实例
/// </summary>
public class LineController : MonoBehaviour
{
    // 单例实例
    public static LineController instance;

    // 是否正在进行拖拽绘制
    public bool isDragging = false;

    // 线条宽度
    public float lineWidth = 0.1f;

    // 线条材质
    public Material lineMaterial;

    // 当前连线的起始点
    private ConnectionPoint startPoint;

    // 当前正在绘制的线条
    private LineRenderer currentLine;

    // 存储当前连接序列中的所有点
    private List<ConnectionPoint> connectedPoints = new List<ConnectionPoint>();

    // 主相机引用
    private Camera mainCamera;

    /// <summary>
    /// 初始化时设置单例实例和获取主相机引用
    /// </summary>
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        mainCamera = Camera.main;
    }

    /// <summary>
    /// 开始创建新的连接线
    /// </summary>
    /// <param name="point">起始连接点</param>
    public void StartConnection(ConnectionPoint point)
    {
        // 如果不是起始点且没有正在进行的连接，则不允许开始
        if (!point.isStartPoint && !isDragging) return;

        // 如果正在拖拽中，则尝试连接到新点
        if (isDragging)
        {
            TryConnect(point);
            return;
        }

        startPoint = point;
        isDragging = true;
        connectedPoints.Clear();
        connectedPoints.Add(point);

        // 创建新的线条游戏对象并设置基本属性
        GameObject lineObj = new GameObject("Line");
        currentLine = lineObj.AddComponent<LineRenderer>();
        currentLine.material = lineMaterial;
        currentLine.startWidth = lineWidth;
        currentLine.endWidth = lineWidth;
        currentLine.positionCount = 2;
        currentLine.useWorldSpace = true;

        // 设置线条的起始点位置
        currentLine.SetPosition(0, startPoint.transform.position);
    }

    /// <summary>
    /// 每帧更新线条终点位置到鼠标位置
    /// </summary>
    private void Update()
    {
        if (isDragging && currentLine != null)
        {
            // 将鼠标屏幕坐标转换为世界坐标
            Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0;
            currentLine.SetPosition(currentLine.positionCount - 1, mousePosition);

            // 检测鼠标释放，结束连线
            if (Input.GetMouseButtonUp(0))
            {
                EndConnection();
            }
        }
    }

    /// <summary>
    /// 尝试连接到目标点
    /// </summary>
    /// <param name="endPoint">目标连接点</param>
    public void TryConnect(ConnectionPoint endPoint)
    {
        // 验证连接条件
        if (!endPoint.canConnect || endPoint.isConnected || connectedPoints.Contains(endPoint)) return;

        // 更新线条位置
        int posCount = currentLine.positionCount;
        currentLine.positionCount = posCount + 1;
        currentLine.SetPosition(posCount - 1, endPoint.transform.position);

        // 添加到已连接点列表
        connectedPoints.Add(endPoint);
        endPoint.isConnected = true;

        // 如果不是起始点，建立与前一个点的连接
        if (connectedPoints.Count > 1)
        {
            ConnectionPoint prevPoint = connectedPoints[connectedPoints.Count - 2];
            prevPoint.Connect(endPoint);
            endPoint.Connect(prevPoint);
        }

        // 检查整个解谜是否完成
        PuzzleManager.instance?.CheckPuzzleCompletion();
    }

    /// <summary>
    /// 结束连线操作
    /// 如果没有完成有效连接，则销毁当前线条并重置所有点的状态
    /// </summary>
    private void EndConnection()
    {
        if (currentLine != null)
        {
            // 如果只有起始点或者连接无效，则重置所有状态
            if (connectedPoints.Count <= 1)
            {
                foreach (var point in connectedPoints)
                {
                    point.Disconnect();
                }
                Destroy(currentLine.gameObject);
            }
        }

        isDragging = false;
        startPoint = null;
        currentLine = null;
        connectedPoints.Clear();
    }
}