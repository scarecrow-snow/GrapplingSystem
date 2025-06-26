using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGrappleDrawer
{
    public bool IsGrappling();
    public Vector3 GetGrapplePoint();
    public Vector3 GetGunTipPosition();
}
