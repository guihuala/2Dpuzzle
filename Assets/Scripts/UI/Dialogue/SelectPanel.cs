using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectPanel : BasePanel
{

    public void AddCell(DialogueCell cell,DialoguePanel panel)
    {
        GameObject selectCell = Instantiate(Resources.Load<GameObject>("UIcomponents/SelectCell"));

        selectCell.transform.SetParent(transform);

        selectCell.GetComponent<SelectCell>().Init(cell, panel);
    }
}
