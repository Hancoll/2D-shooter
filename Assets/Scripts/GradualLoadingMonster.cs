using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GradualLoadingMonster : GradualLoadingObject
{
    public override void ChangeActive(bool value)
    {
        base.ChangeActive(value);

        if(value)
            gameObject.GetComponent<MonsterBaseAI>().ContinuePreActiveCoroutines();
    }
}
