using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(PathManager))]
public class PathEditor : Editor {

    PathManager _target;

    void OnEnable()
    {
        _target = (PathManager)target;
    }

    public override void OnInspectorGUI()
    {
        _target.pathVisible = EditorGUILayout.Toggle("Visible ",_target.pathVisible);
        if (GUILayout.Button("add", GUILayout.Width(100f)))
        {
            GameObject go = new GameObject();
            go.AddComponent<TransformDisplayNode>();
            go.transform.parent = _target.transform;
            _target.path.Add(go.transform);
            
        }

        for (int i = 0; i < _target.path.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            _target.path[i].position = EditorGUILayout.Vector3Field("Node" + (i + 1),_target.path[i].position);
            if (GUILayout.Button("X", GUILayout.Width(20f)))
            {
                Debug.Log(_target.path[i]);
                DestroyImmediate(_target.path[i].gameObject);
                _target.path.RemoveAt(i);
            }
            EditorGUILayout.EndHorizontal();
        }
    }

    void OnSceneGUI()
    {
        if (_target.pathVisible)
        {
            if (_target.path.Count > 0)
            {
                //node handle display:
                for (int i = 0; i < _target.path.Count; i++)
                {
                    Handles.Label(_target.path[i].position," "+(i+1));
                    _target.path[i].position = Handles.PositionHandle(_target.path[i].position, Quaternion.identity);
                }
            }
        }
    }
}
