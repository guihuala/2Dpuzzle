using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New DialogueDataListSO", menuName = "Dialogue/DialogueDataListSO")]
public class DialogueData : ScriptableObject
{
    public List<DialogueCell> Cells;
}
