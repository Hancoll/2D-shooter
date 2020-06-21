using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractiveObject: MonoBehaviour
{
    public static LayerMask InteractiveLayerMask => LayerMask.NameToLayer("InteractiveObject");
    public abstract bool Execute();
}
