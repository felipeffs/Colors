using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableObjectAction : ConnectorAction
{
    [SerializeField] private Transform targetObject;

    public override void Execute()
    {
        if (targetObject)
            targetObject.gameObject.SetActive(false);
    }

    public override void Undo()
    {
        if (targetObject)
            targetObject.gameObject.SetActive(true);
    }
}
