using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DrawUtility : MonoBehaviour {

	public static PaintingDetector2D CreateDetector(RectTransform prefab)
    {
        var detectRect = Instantiate(prefab);
        detectRect.name = "detect";
        detectRect.SetParent(prefab.parent);
        detectRect.localPosition = prefab.localPosition;
        detectRect.localScale = Vector3.one;
        detectRect.SetSiblingIndex(prefab.GetSiblingIndex() + 1);
        //detectRect.localPosition = new Vector3(indicator.rect.width, indicator.rect.height,0);
        detectRect.sizeDelta = new Vector2(Screen.width, Screen.height);
        detectRect.GetComponent<RawImage>().color = Color.clear;
        var detector = detectRect.gameObject.AddComponent<PaintingDetector2D>();
        return detector;
    }

    public static RectTransform CreateRectTransform(RectTransform prefab,Vector2 size)
    {
        var rect = Instantiate(prefab);
        rect.SetParent(prefab.parent);
        rect.localPosition = prefab.localPosition;
        rect.localScale = Vector3.one;
        rect.SetSiblingIndex(prefab.GetSiblingIndex() + 1);
        rect.sizeDelta = size;
        return rect;
    }

    public static Texture2D CreateCanvas(int width, int height)
    {
        var tex = new Texture2D(width, height);
        Color[] col = new Color[width * height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                tex.SetPixel(x, y, Color.clear);
            }
        }
        tex.Apply();
        return tex;
    }
}
