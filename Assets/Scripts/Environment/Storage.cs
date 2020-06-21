using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Storage : InteractiveObject
{
    public ItemInspector[] items = new ItemInspector[12];

    public override bool Execute()
    {
        GameManager.GameUIManager.OpenStoragePanel(items);
        return true;
    }
}
