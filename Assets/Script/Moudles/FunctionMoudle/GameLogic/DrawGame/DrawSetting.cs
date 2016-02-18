using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class DrawSetting : MonoBehaviour {

    public RectTransform prefab;
    public InputField penX;
    public InputField penY;
    public InputField brushX;
    public InputField brushY;
    public InputField bSize;
    public Button btn;

    public RectTransform indicator;

    RectTransform drawRect;
    RawImage drawImage;
    Texture2D paintTexture;

    Vector2 penOffset;
    public Vector2 brushOffset;
    int brushSize = 16;

    // Use this for initialization
    void Start () {
        penX.onValueChange.AddListener(ChangePenX);
        penY.onValueChange.AddListener(ChangePenY);
        brushX.onValueChange.AddListener(ChangeBrushX);
        brushY.onValueChange.AddListener(ChangeBrushY);
        bSize.onValueChange.AddListener(ChangeBSize);

        penX.text = PlayerPrefs.GetInt("penOffsetX", 0).ToString();
        penY.text = PlayerPrefs.GetInt("penOffsetY", 0).ToString();
        brushX.text = PlayerPrefs.GetInt("brushOffsetX", 0).ToString();
        brushY.text = PlayerPrefs.GetInt("brushOffsetY", 0).ToString();
        bSize.text = PlayerPrefs.GetInt("bSize", 10).ToString();

        var detector = DrawUtility.CreateDetector(prefab);

        detector.OnStartPaint += new PaintingDetector2D.DelegatePaint(OnStartPaint);
        detector.OnPaintMovement += new PaintingDetector2D.DelegatePaint(OnPaintMovement);
        detector.OnPaintEnd += new PaintingDetector2D.DelegatePaint(OnPaintEnd);


        drawRect = DrawUtility.CreateRectTransform(prefab, new Vector2(Screen.width, Screen.height));
        drawImage = drawRect.GetComponent<RawImage>();
        paintTexture = DrawUtility.CreateCanvas(Screen.width, Screen.height);
        drawImage.texture = paintTexture;

        btn.onClick.AddListener(Click);
    }

    

    private void OnStartPaint(PositionData posData)
    {
        indicator.gameObject.SetActive(true);
        indicator.SetAsLastSibling();
        int x = PlayerPrefs.GetInt("penOffsetX");
        int y = PlayerPrefs.GetInt("penOffsetY");
        indicator.position = posData.worldPos - new Vector2(x,y);
    }

    private void OnPaintMovement(PositionData posData)
    {
        int x = PlayerPrefs.GetInt("penOffsetX",0);
        int y = PlayerPrefs.GetInt("penOffsetY",0);
        indicator.position = posData.worldPos - new Vector2(x, y);

        Vector2 stPos = posData.preWorldPos;
        Vector2 endPos = posData.worldPos;

        brushOffset = new Vector2(PlayerPrefs.GetInt("brushOffsetX", 0),PlayerPrefs.GetInt("brushOffsetY", 0));

        stPos -= brushOffset;
        endPos -= brushOffset;

        Brush(stPos, endPos);
    }

    private void OnPaintEnd(PositionData posData)
    {
        indicator.gameObject.SetActive(false);
    }


    void Brush(Vector2 p1, Vector2 p2)
    {
        brushSize = PlayerPrefs.GetInt("bSize", 10);
        Drawing.PaintLine(p1, p2, brushSize, Color.black,1, paintTexture);
        //Drawing.PaintLine(p1, p2, 10, Color.red, 1f, paintTexture);
        paintTexture.Apply();
    }

    void ChangePenX(string s)
    {
        int res;
        if(int.TryParse(s,out res))
        {
            PlayerPrefs.SetInt("penOffsetX", res);
        }
    }

    void ChangePenY(string s)
    {
        int res;
        if (int.TryParse(s, out res))
        {
            PlayerPrefs.SetInt("penOffsetY", res);
        }
    }

    void ChangeBrushX(string s)
    {
        int res;
        if (int.TryParse(s, out res))
        {
            PlayerPrefs.SetInt("brushOffsetX", res);
        }
    }

    void ChangeBrushY(string s)
    {
        int res;
        if (int.TryParse(s, out res))
        {
            PlayerPrefs.SetInt("brushOffsetY", res);
        }
    }

    private void ChangeBSize(string s)
    {
        int res;
        if (int.TryParse(s, out res))
        {
            PlayerPrefs.SetInt("bSize", res);
        }
    }

    void Click()
    {
        Application.LoadLevel("DrawSomething");
    }
}
