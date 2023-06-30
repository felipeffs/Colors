using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface IGrabbable
{
    public Action<Collider2D> Grab(Transform grabPoint, Collider2D performerCollider, Action<bool> changeGrabStatus);
}
