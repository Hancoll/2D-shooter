using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GradualLoadingObject : MonoBehaviour
{
    public bool IsActive { get { return isActive; } }
    bool isActive = true;

    public virtual void ChangeActive(bool value)
    {
        isActive = value;
        gameObject.SetActive(value);
    }

    public void OnDestroyObject() =>
        GameManager.GradualLoadingMapManager.RemoveGraduaLoadingObjects(this);

}
