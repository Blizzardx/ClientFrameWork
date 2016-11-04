using UnityEngine;
using System.Collections;

public class ModelPlayer : ModelBase
{
    public const int Index = 0;
    public override int GetIndex()
    {
        return Index;
    }
    protected override void OnCreate()
    {
        Debug.Log("init player model");
    }
}
