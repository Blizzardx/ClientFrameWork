using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PathManager : MonoBehaviour {

    public bool pathVisible;
    public List<Transform> path = new List<Transform>();

    void OnDrawGizmosSelected()
    {
        if (pathVisible)
        {
            if (path.Count > 0)
            {
                iTween.DrawPath(path.ToArray(), Color.red);
            }
        }
    }
    void OnDrawGizmos()
    {
        if (!pathVisible)
            return;
        if (path == null || path.Count <= 1)
            return;
        iTween.DrawPath(path.ToArray(),Color.red);
    }
}
