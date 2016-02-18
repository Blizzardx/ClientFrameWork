using System;
using Config;
using UnityEngine;
using System.Collections.Generic;

public class Target_1_INCIRCLERANGE:TargetMethodBase
{
    public Target_1_INCIRCLERANGE(int id) : base(id)
    {
    }

    public override List<Ilife> GetTargetList(Ilife thisUnit, TargetData data, FuncContext context)
    {
        List<Ilife> res = new List<Ilife>();

        if (thisUnit == null)
        {
            return res;
        }
        Vector3 point = ((ITransformBehaviour) (thisUnit)).GetTransformData().GetPosition();
        float r = ((ITransformBehaviour)(thisUnit)).GetTransformData().GetScale().x;
        var lifeList = LifeManager.GetLifeList();
        foreach (var elem in lifeList)
        {
            if (elem.Key == thisUnit.GetInstanceId())
            {
                continue;
            }
           Ilife otherLifes = elem.Value;
           Vector3 point1 = ((ITransformBehaviour)(otherLifes)).GetTransformData().GetPosition();
           if (Vector3.Distance(point, point1) < r)
           {
               // add to target list
               res.Add(otherLifes);
           }
        }
        return res;
    }
}