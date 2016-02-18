using UnityEngine;
using System.Collections;

public class TestLightmap : MonoBehaviour {

    public GameObject obj;

	// Use this for initialization
	void Start () {
        var inTerrains = obj.GetComponentsInChildren<Terrain>();
        var inRenderers = obj.GetComponentsInChildren<Renderer>();
        var go = Instantiate(obj, obj.transform.position + new Vector3(0, 0, 50),Quaternion.identity) as GameObject;
        //go.transform.position = obj.transform.position + new Vector3(0,0,50);
        var outTerrains = go.GetComponentsInChildren<Terrain>();
        var outRenderers = go.GetComponentsInChildren<Renderer>();
        for(int i = 0; i < outTerrains.Length; i++)
        {
            outTerrains[i].lightmapIndex = inTerrains[i].lightmapIndex;
            outTerrains[i].lightmapScaleOffset = inTerrains[i].lightmapScaleOffset;
        }
        for(int i = 0; i < outRenderers.Length; i++)
        {
            outRenderers[i].lightmapIndex = inRenderers[i].lightmapIndex;
            outRenderers[i].lightmapScaleOffset = inRenderers[i].lightmapScaleOffset;
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
