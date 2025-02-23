using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class RecordUI : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Text indexText;         // 序号
    public Text recordName;        // 存档名称
    public GameObject auto;        // 自动存档的标识
    public Image rect;             // 边框
    [ColorUsage(true)]
    public Color enterColor;       // 鼠标悬停存档时的边框颜色

    // 事件回调
    public static System.Action<int> OnLeftClick;
    public static System.Action<int> OnRightClick;
    public static System.Action<int> OnEnter;
    public static System.Action OnExit;

    int id;

    private void Start()
    {
        id = transform.GetSiblingIndex();
    }

    // 鼠标点击事件
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (OnLeftClick != null)
                OnLeftClick(id);
        }

        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (OnRightClick != null)
                OnRightClick(id);
        }
    }

    // 鼠标进入事件
    public void OnPointerEnter(PointerEventData eventData)
    {
        // 改变边框颜色
        rect.color = enterColor;

        // 如果存档存在则显示提示信息（通过ID获取存档数据，ID为SiblingIndex）
        if (recordName.text != "空白")
        {
            if (OnEnter != null)
                OnEnter(id);
        }
    }

    // 鼠标离开事件
    public void OnPointerExit(PointerEventData eventData)
    {
        // 恢复离开时边框颜色
        rect.color = Color.white;

        // 触发离开事件
        if (OnExit != null)
            OnExit();
    }

    // 初始化存档列表时设置序号
    public void SetID(int i)
    {
        indexText.text = i.ToString();
    }

    // 设置存档名称
    public void SetName(int i)
    {
        // 如果为空则隐藏Auto标识（可能是删除时需要用到的）
        if (RecordData.Instance.recordName[i] == "")
        {
            recordName.text = "空白";
            auto.SetActive(false);
        }
        else
        {
            // 获取存档文件的完整名称，去掉扩展名
            string full = RecordData.Instance.recordName[i];
            // 获取日期（8位）
            string date = full.Substring(0, 8);
            // 获取时间（6位）
            string time = full.Substring(9, 6);
            // 格式化日期
            TIMEMGR.SetDate(ref date);
            // 格式化时间
            TIMEMGR.SetTime(ref time);
            // 显示到UI
            recordName.text = date + " " + time;

            // 根据存档名称设置是否显示Auto标识
            if (full.Substring(full.Length - 4) == "auto")
                auto.SetActive(true);
            else
                auto.SetActive(false);
        }
    }
}
