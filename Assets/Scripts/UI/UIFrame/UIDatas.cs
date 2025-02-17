using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UIData
{
    public string uiName;

    public string uiPath;

}


[CreateAssetMenu(fileName = "New UIDataListSO", menuName = "CustomizedSO/UIDataListSO")]
public class UIDatas : ScriptableObject
{
    public List<UIData> uiDataList;

}
