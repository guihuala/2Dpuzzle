using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveObject : BaseInteractableObject
{
    BasePanel savePanel;
    
    protected override void Apply()
    {
        base.Apply();

        savePanel = UIManager.Instance.OpenPanel("SaveButtonGroup");
    }
    
    public override void Exit()
    {
        base.Exit();
        
        if(savePanel != null)
            UIManager.Instance.ClosePanel("SaveButtonGroup");
    }
}
