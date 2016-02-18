using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MusicGameTool_BackgroundSetting : MonoBehaviour {
    [Range(0f,3f)]
    public float DistancePerUnit = 2.65f;

    public List<GameObject> LaunchPos = new List<GameObject>(); 
    public List<GameObject> NoteKey = new List<GameObject>();
    
	// Use this for initialization
	void Start () {
        // Get Root
        GameObject launchRoot = GameObject.Find("LaunchPositions");
        if (launchRoot == null)
        {
            Debug.LogError("LaunchPositions not found");
            return;
        }
        for (int i = 0; i < launchRoot.transform.childCount; i++)
        {
            LaunchPos.Add(launchRoot.transform.GetChild(i).gameObject);
        }
        GameObject noteRoot = GameObject.Find("NoteKey");
        if (noteRoot == null)
        {
            Debug.LogError("NoteKey not found");
            return;
        }
        for (int i = 0; i < noteRoot.transform.childCount; i++)
        {
            NoteKey.Add(noteRoot.transform.GetChild(i).gameObject);
        }
        // Set Pos
        int lunchCount = LaunchPos.Count;
        for (int i = 0; i < lunchCount; i++)
        {
            LaunchPos[i].transform.position = new Vector3((i - lunchCount / 2 + 0.5f) * DistancePerUnit, LaunchPos[i].transform.position.y, LaunchPos[i].transform.position.z);
        }
        int noteCount = NoteKey.Count;
        for (int i = 0; i < noteCount; i++)
        {
            NoteKey[i].transform.position = new Vector3((i - noteCount / 2 + 0.5f) * DistancePerUnit, NoteKey[i].transform.position.y, NoteKey[i].transform.position.z);
        }
	}
	
	// Update is called once per frame
	void Update () {
        // Set Pos
        //int lunchCount = LaunchPos.Count;
        //for (int i = 0; i < lunchCount; i++)
        //{
        //    LaunchPos[i].transform.position = new Vector3((i - lunchCount / 2 + 0.5f) * DistancePerUnit, LaunchPos[i].transform.position.y, LaunchPos[i].transform.position.z);
        //}
        //int noteCount = NoteKey.Count;
        //for (int i = 0; i < noteCount; i++)
        //{
        //    NoteKey[i].transform.position = new Vector3((i - noteCount / 2 + 0.5f) * DistancePerUnit, NoteKey[i].transform.position.y, NoteKey[i].transform.position.z);
        //}
	}
}
