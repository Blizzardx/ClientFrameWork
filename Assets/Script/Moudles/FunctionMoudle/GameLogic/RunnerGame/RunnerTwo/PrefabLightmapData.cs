﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PrefabLightmapData : MonoBehaviour
{
	[System.Serializable]
	struct RendererInfo
	{
		public Renderer 	renderer;
        //start
        public Terrain      terrain;
		public int 			lightmapIndex;
		public Vector4 		lightmapOffsetScale;
	}
	
	[SerializeField]
	RendererInfo[]	m_RendererInfo;
	[SerializeField]
	Texture2D[] 	m_Lightmaps;

    [SerializeField]
    bool     m_applied = false;
	
	void Awake ()
	{
		if (m_RendererInfo == null || m_RendererInfo.Length == 0)
			return;

        ApplyLightmaps();
	}

    void ApplyLightmaps()
    {
        if (m_applied)
            return;

        var lightmaps = LightmapSettings.lightmaps;
        var combinedLightmaps = new LightmapData[lightmaps.Length + m_Lightmaps.Length];

        lightmaps.CopyTo(combinedLightmaps, 0);
        for (int i = 0; i < m_Lightmaps.Length;i++)
        {
            combinedLightmaps[i+lightmaps.Length] = new LightmapData();
            combinedLightmaps[i+lightmaps.Length].lightmapFar = m_Lightmaps[i];
        }

        ApplyRendererInfo(m_RendererInfo, lightmaps.Length);
        LightmapSettings.lightmaps = combinedLightmaps;

        m_applied = true;
    }
	
	static void ApplyRendererInfo (RendererInfo[] infos, int lightmapOffsetIndex)
	{
		for (int i=0;i<infos.Length;i++)
		{
			var info = infos[i];
			
            //start
            if(info.renderer != null)
            {
                #if UNITY_EDITOR
			    if (UnityEditor.GameObjectUtility.AreStaticEditorFlagsSet(info.renderer.gameObject, UnityEditor.StaticEditorFlags.BatchingStatic))
			    {
				    Debug.LogWarning("The renderer " + info.renderer.name + " is marked Batching Static. The static batch is created when building the player. " +
				                 "Setting the lightmap scale and offset will not affect lightmapping UVs as they have the scale and offset already burnt in.", info.renderer);
			    }
			    #endif
			
			    info.renderer.lightmapIndex = info.lightmapIndex + lightmapOffsetIndex;
			    info.renderer.lightmapScaleOffset = info.lightmapOffsetScale;
            }
            else
            {
                if(info.terrain != null)
                {
                    info.terrain.lightmapIndex = info.lightmapIndex + lightmapOffsetIndex;
                    info.terrain.lightmapScaleOffset = info.lightmapOffsetScale;
                }
            }
            /*
			#if UNITY_EDITOR
			if (UnityEditor.GameObjectUtility.AreStaticEditorFlagsSet(info.renderer.gameObject, UnityEditor.StaticEditorFlags.BatchingStatic))
			{
				Debug.LogWarning("The renderer " + info.renderer.name + " is marked Batching Static. The static batch is created when building the player. " +
				                 "Setting the lightmap scale and offset will not affect lightmapping UVs as they have the scale and offset already burnt in.", info.renderer);
			}
			#endif
			
			info.renderer.lightmapIndex = info.lightmapIndex + lightmapOffsetIndex;
			info.renderer.lightmapScaleOffset = info.lightmapOffsetScale;
            */
		}
	}
	
	#if UNITY_EDITOR
	[UnityEditor.MenuItem("Assets/Bake Prefab Lightmaps")]
	static void GenerateLightmapInfo ()
	{
		if (UnityEditor.Lightmapping.giWorkflowMode != UnityEditor.Lightmapping.GIWorkflowMode.OnDemand)
		{
			Debug.LogError("ExtractLightmapData requires that you have baked you lightmaps and Auto mode is disabled.");
			return;
		}
		UnityEditor.Lightmapping.Bake();
		
		PrefabLightmapData[] prefabs = FindObjectsOfType<PrefabLightmapData>();
		
		foreach (var instance in prefabs)
		{
			var gameObject = instance.gameObject;
			var rendererInfos = new List<RendererInfo>();
			var lightmaps = new List<Texture2D>();
			
			GenerateLightmapInfo(gameObject, rendererInfos, lightmaps);
			
			instance.m_RendererInfo = rendererInfos.ToArray();
			instance.m_Lightmaps = lightmaps.ToArray();
			
			var targetPrefab = UnityEditor.PrefabUtility.GetPrefabParent(gameObject) as GameObject;
			if (targetPrefab != null)
			{
				//UnityEditor.Prefab
				UnityEditor.PrefabUtility.ReplacePrefab(gameObject, targetPrefab);
			}
		}
	}
    [UnityEditor.MenuItem("Assets/Generate Prefab Lightmaps")]
    static void GenerateLightmapInfoNoBake()
    {
        if (UnityEditor.Lightmapping.giWorkflowMode != UnityEditor.Lightmapping.GIWorkflowMode.OnDemand)
        {
            Debug.LogError("ExtractLightmapData requires that you have baked you lightmaps and Auto mode is disabled.");
            return;
        }

        PrefabLightmapData[] prefabs = FindObjectsOfType<PrefabLightmapData>();

        foreach (var instance in prefabs)
        {
            var gameObject = instance.gameObject;
            var rendererInfos = new List<RendererInfo>();
            var lightmaps = new List<Texture2D>();

            GenerateLightmapInfo(gameObject, rendererInfos, lightmaps);

            instance.m_RendererInfo = rendererInfos.ToArray();
            instance.m_Lightmaps = lightmaps.ToArray();

            var targetPrefab = UnityEditor.PrefabUtility.GetPrefabParent(gameObject) as GameObject;
            if (targetPrefab != null)
            {
                //UnityEditor.Prefab
                UnityEditor.PrefabUtility.ReplacePrefab(gameObject, targetPrefab);
            }
        }
    }


    static void GenerateLightmapInfo (GameObject root, List<RendererInfo> rendererInfos, List<Texture2D> lightmaps)
	{
		var renderers = root.GetComponentsInChildren<MeshRenderer>();
		foreach (MeshRenderer renderer in renderers)
		{
			if (renderer.lightmapIndex != -1)
			{
				RendererInfo info = new RendererInfo();
				info.renderer = renderer;
				info.lightmapOffsetScale = renderer.lightmapScaleOffset;
				
				Texture2D lightmap = LightmapSettings.lightmaps[renderer.lightmapIndex].lightmapFar;
				
				info.lightmapIndex = lightmaps.IndexOf(lightmap);
				if (info.lightmapIndex == -1)
				{
					info.lightmapIndex = lightmaps.Count;
					lightmaps.Add(lightmap);
				}
				
				rendererInfos.Add(info);
			}
		}

        //start
        var terrains = root.GetComponentsInChildren<Terrain>();
        foreach(Terrain terrain in terrains)
        {
            if(terrain.lightmapIndex != -1)
            {
                RendererInfo info = new RendererInfo();
                info.terrain = terrain;
                info.lightmapOffsetScale = terrain.lightmapScaleOffset;

                Texture2D lightmap = LightmapSettings.lightmaps[terrain.lightmapIndex].lightmapFar;

                info.lightmapIndex = lightmaps.IndexOf(lightmap);
                if(info.lightmapIndex == -1)
                {
                    info.lightmapIndex = lightmaps.Count;
                    lightmaps.Add(lightmap);
                }

                rendererInfos.Add(info);
            }
        }
	}

    [UnityEditor.MenuItem("Assets/Apply Lightmapped Prefabs")]
    static void RefreshLightmapInfo ()
    {
        PrefabLightmapData[] prefabs = FindObjectsOfType<PrefabLightmapData>();

        foreach (var instance in prefabs)
        {
            instance.ApplyLightmaps();
        }
    }
	#endif
	
}