using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ActionEditor;

public class MoveFrameHelper : MonoBehaviour {

    List<TransformDisplayNode> nodeList;
	public void Init(List<Common.Auto.ThriftVector3> path)
    {
        Clear();

        nodeList = new List<TransformDisplayNode>();
        foreach (var pos in path)
        {
            var node = CreateNode();
            nodeList.Add(node);
            node.transform.position = pos.GetVector3();
        }
    }

    public void Clear()
    {
        if (nodeList != null)
        {
            foreach (var node in nodeList)
            {
                Destroy(node.gameObject);
            }
        }
        nodeList = null;
    }

    TransformDisplayNode CreateNode()
    {
        var go = new GameObject();
        var node = go.AddComponent<TransformDisplayNode>();
        return node;
    }

    public void AddObj(int index,Vector3 pos)
    {
        var node = CreateNode();
        nodeList.Insert(index, node);
        node.transform.position = pos;
    }

    public void RemoveObj(int index)
    {
        Destroy(nodeList[index].gameObject);
        nodeList.RemoveAt(index);
    }

    public void RefershPos(int index,Vector3 pos)
    {
        nodeList[index].transform.position = pos;
    }

    public TransformDisplayNode GetNode(int index)
    {
        return nodeList[index];
    }

    public int GetIndex(GameObject go)
    {
        var dn = go.GetComponent<TransformDisplayNode>();
        if (dn != null)
        {
            return nodeList.IndexOf(dn);
        }
        return -1;
    }

    void Update()
    {

        //iTween.DrawPath();
    }

    void OnDrawGizmos()
    {
        if (nodeList == null || nodeList.Count <= 1)
            return;
        var path = new Vector3[nodeList.Count];
        for (int i=0;i<nodeList.Count;i++)
        {
            path[i] = nodeList[i].transform.position;
        }
        iTween.DrawPath(path);
    }
}
