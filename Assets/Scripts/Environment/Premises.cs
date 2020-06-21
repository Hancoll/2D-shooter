using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Premises : MonoBehaviour
{
    [SerializeField] GameObject forwardWall;
    bool visibleStatus = true;

    public void ChangeForwardWallVisible(bool visibleStatus)
    {
        if(this.visibleStatus != visibleStatus)
            forwardWall.SetActive(visibleStatus);

        this.visibleStatus = visibleStatus;
    }
}
