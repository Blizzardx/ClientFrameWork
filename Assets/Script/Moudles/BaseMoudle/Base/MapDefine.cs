using UnityEngine;
using System;
using System.Collections;

public enum MapId
{
    TestProject1Map,
    TestProject2Map,
}

public class MapDefiner
{
    public static void RegisterMap()
    {
        MapManager.Instance.RegisterMap(MapId.TestProject1Map, new MapIndexStruct("map"));
		MapManager.Instance.RegisterMap(MapId.TestProject2Map, new MapIndexStruct("map"));
    }
}