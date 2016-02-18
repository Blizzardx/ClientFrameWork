using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SetFaces : MultiTex
{
    public static Dictionary<string, int> faceTable;
    public GameObject face;
    public void SetFace(string faceName)
    {
        if (faceTable == null)
            faceTable = new Dictionary<string, int> {
            { "zhengchang",0 },
            { "jingya",1 },
            { "xiee",2},
            { "shengqi",3 },
            { "jifen",4 },
            { "haipa",5 },
            { "guilian",6 },
            { "congbai",7 } };//看代码才知道还能这么初始化，学到了，谢谢
        if (faceTable.ContainsKey(faceName))
            SetTex(faceTable[faceName]);
    }
    void Start()
    {
        base.Init(face);
    }
}
