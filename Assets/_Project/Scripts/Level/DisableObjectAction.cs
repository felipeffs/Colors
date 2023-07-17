using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableObjectAction : ConnectorAction
{
    [SerializeField] private Transform[] targetObjects;

    public override void Execute()
    {
        foreach (var tObject in targetObjects)
        {
            if (tObject)
                tObject.gameObject.SetActive(!tObject.gameObject.activeSelf);
        }
    }
}
