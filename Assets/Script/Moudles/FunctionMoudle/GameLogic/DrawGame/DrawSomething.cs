using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DrawSomething : SingletonTemplateMon<DrawSomething> {

    enum GameState
    {
        None,
        PaintOutline,
        FinishOutline,
        PiantBody,
        FinishBody
    }

    public RectTransform prefab;

    private RectTransform drawRect;
    private RawImage drawImage;

    private RectTransform detectRect;
    private PaintingDetector2D detector;

    private RectTransform maskRect;
    private RawImage maskImage;

    public RawImage mini;
    public Texture2D outlineMask;
    public Texture2D outline;
    public Texture2D body;
    public Texture2D blink;
    public string[] pics;

    Vector2 penOffset = new Vector2(-80,-60);
    Vector2 brushOffset = new Vector2(0,0);
    int brushSize = 30;
    
    public RectTransform indicator;
    public RectTransform palette;
    public Button win;

    Texture2D paintTexture;
   

    int totalCount;
    int cur;

    GameState state;

    float time;

    [HideInInspector]
    public Color color = Color.clear;

    Canvas canvas;
    
    void Awake()
    {
        canvas = FindObjectOfType<Canvas>();
        if (_instance != null)
        {
            Destroy(this);
            return;
        }
        _instance = this;
    }

    // Use this for initialization
    void Start () {      
        InitPic();
        InitData();
        InitBrush();
        if (AppManager.Instance == null)
        {
            GameStart();
        }
    }

    public void GameStart()
    {
        BeginOutline();
    }

    int GetRandomPos()
    {
        int[] input = new int[pics.Length];
        int[] output = new int[pics.Length];
        for (int i = 0; i < pics.Length; i++)
        {
            input[i] = i;
        }
        for (int i = 0; i < pics.Length; i++)
        {
            int index = Random.Range(0, pics.Length - i);
            output[i] = input[index];
            input[index] = input[pics.Length - i - 1];
        }
        int num = -1;
        for (int i = 0; i < output.Length; i++)
        {
            if (!PlayerPrefs.HasKey("draw" + output[i].ToString()))
            {
                num = output[i];
                PlayerPrefs.SetInt("draw" + output[i].ToString(), 1);
                break;
            }
        }
        return num;
    }

    void ClearRandomPos()
    {
        for(int i = 0; i < pics.Length; i++)
        {
            PlayerPrefs.DeleteKey("draw"+i.ToString());
        }
    }

    void InitPic()
    {
        int pos = GetRandomPos();
        if(pos < 0)
        {
            ClearRandomPos();
            pos = GetRandomPos();
        }

        var pic = pics[pos];
        mini.texture = Load(pic+"cankaotu");
        outlineMask = Load(pic+"xuxian");
        outline = Load(pic+"miaoxian");
        body = Load(pic + "tu");
        blink = Load(pic+"gaoliang");
    }

    Texture2D Load(string name)
    {
        if(ResourceManager.Instance == null)
        {
            new GameObject("ResourceManager",typeof( ResourceManager));
            ResourceManager.Instance.Initialize();
        }
        return ResourceManager.Instance.LoadBuildInResource<Texture2D>("Draw/"+ name, AssetType.Texture);
    }

    void InitData()
    {
        var rad = Screen.height * 0.75f;
        Vector2 size = new Vector2((int)rad, (int)rad);

        detector = DrawUtility.CreateDetector(prefab);

        drawRect = DrawUtility.CreateRectTransform(prefab, size);
        drawImage = drawRect.GetComponent<RawImage>();


        maskRect = DrawUtility.CreateRectTransform(prefab, size);
        maskImage = maskRect.GetComponent<RawImage>();


        prefab.gameObject.SetActive(false);
    }

    void InitBrush()
    {
        if (PlayerPrefs.HasKey("penOffsetX"))
        {
            penOffset.x = PlayerPrefs.GetInt("penOffsetX");
        }
        if (PlayerPrefs.HasKey("penOffsetY"))
        {
            penOffset.y = PlayerPrefs.GetInt("penOffsetY");
        }
        if (PlayerPrefs.HasKey("brushOffsetX"))
        {
            brushOffset.x = PlayerPrefs.GetInt("brushOffsetX");
        }
        if (PlayerPrefs.HasKey("brushOffsetY"))
        {
            brushOffset.y = PlayerPrefs.GetInt("brushOffsetY");
        }
        if (PlayerPrefs.HasKey("brushOffsetY"))
        {
            brushSize = PlayerPrefs.GetInt("bSize");
        }  
    }

    private void OnStartPaint(PositionData posData)
    {
        indicator.gameObject.SetActive(true);
        indicator.SetAsLastSibling();
        Vector3 pos;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(canvas.GetComponent<RectTransform>(), posData.worldPos - penOffset, Camera.main, out pos);
        indicator.position = pos;
        //indicator.position = posData.worldPos - penOffset;
    }

    private void OnPaintMovement(PositionData posData)
    {
        //indicator.position = posData.worldPos - penOffset;
        Vector3 pos;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(canvas.GetComponent<RectTransform>(), posData.worldPos - penOffset, Camera.main, out pos);
        indicator.position = pos;

        var zoomX = paintTexture.width / drawImage.rectTransform.rect.width;
        var zoomY = paintTexture.height / drawImage.rectTransform.rect.height;

        Vector2 stPos = posData.preWorldPos;
        Vector2 endPos = posData.worldPos;

        stPos -= brushOffset;
        endPos -= brushOffset;

        var s = RectTransformUtility.WorldToScreenPoint(Camera.main, drawRect.position);
        //Debug.Log(posData.localPos + "   "+ posData.worldPos);
        //Rect rect = new Rect(drawRect.position.x + drawRect.rect.x,drawRect.position.y + drawRect.rect.y,drawRect.rect.width,drawRect.rect.height);
        Rect rect = new Rect(s.x + drawRect.rect.x, s.y + drawRect.rect.y, drawRect.rect.width, drawRect.rect.height);
        if (!rect.Contains(stPos) ||! rect.Contains(endPos))
        {
            return;
        }

        //var stPos = posData.preLocalPos;
        //var endPos = posData.localPos;
        stPos = stPos - rect.position;
        endPos = endPos - rect.position;

        stPos.x = stPos.x * zoomX;
        stPos.y = stPos.y * zoomY;
        endPos.x = endPos.x * zoomX;
        endPos.y = endPos.y * zoomY;

        if(state == GameState.PaintOutline)
        {
            Brush(stPos, endPos);
            if (cur > totalCount * 0.8f)
            {
                FinishOutline();
            }
        }
        else
        {
            if(color != Color.clear)
            {
                BrushInColor(stPos, endPos);
                if (cur > totalCount * 0.8f)
                {
                    //FinishBody();
                }
            }
        }
    }

    private void OnPaintEnd(PositionData posData)
    {
        indicator.gameObject.SetActive(false);
    }

    void BeginOutline()
    {
        paintTexture = DrawUtility.CreateCanvas(outline.width, outline.height);
        drawImage.texture = paintTexture;
        //drawImage.SetNativeSize();

        //detector = drawImage.GetComponent<PaintingDetector2D>();
        if (detector == null)
        {
            Debug.LogError("no paint detector!");
        }
        detector.OnStartPaint += new PaintingDetector2D.DelegatePaint(OnStartPaint);
        detector.OnPaintMovement += new PaintingDetector2D.DelegatePaint(OnPaintMovement);
        detector.OnPaintEnd += new PaintingDetector2D.DelegatePaint(OnPaintEnd);

        maskImage.texture = outlineMask;
        //outlineMaskImage.SetNativeSize();

        var pixels = outline.GetPixels();
        foreach (var pix in pixels)
        {
            if (pix.a > 0)
            {
                totalCount++;
            }
        }

        palette.gameObject.SetActive(false);
        state = GameState.PaintOutline;
    }

    void FinishOutline()
    {
        state = GameState.FinishOutline;

        detector.OnStartPaint -= new PaintingDetector2D.DelegatePaint(OnStartPaint);
        detector.OnPaintMovement -= new PaintingDetector2D.DelegatePaint(OnPaintMovement);
        detector.OnPaintEnd -= new PaintingDetector2D.DelegatePaint(OnPaintEnd);

        indicator.gameObject.SetActive(false);

        maskImage.texture = outline;
        maskRect.SetSiblingIndex(drawRect.GetSiblingIndex());
        outline = body;
        paintTexture = DrawUtility.CreateCanvas(outline.width, outline.height);
        drawImage.texture = paintTexture;

        var bRect =  DrawUtility.CreateRectTransform(maskRect, maskRect.sizeDelta);
        var image = bRect.GetComponent<RawImage>();
        image.texture = blink;

        float time = 1.5f;
        Hashtable hash = new Hashtable();
        hash.Add("a", 0);
        hash.Add("time", time);
        iTween.ColorFrom(bRect.gameObject, hash);
        Hashtable hash2 = new Hashtable();
        hash2.Add("a", 0);
        hash2.Add("time", time);
        hash2.Add("delay", time);
        hash2.Add("oncomplete", "FinishBlink");
        hash2.Add("oncompletetarget", gameObject);
        hash2.Add("oncompleteparams",image.gameObject);
        iTween.ColorTo(bRect.gameObject, hash2);
    }

    void FinishBlink(GameObject it)
    {
        Destroy(it);
        BeginBody();
    }

    void BeginBody()
    {
        var pixels = body.GetPixels();
        cur = 0;
        totalCount = 0;
        foreach (var pix in pixels)
        {
            if (pix.a == 1)
            {
                totalCount++;
            }
        }
        detector.OnStartPaint += new PaintingDetector2D.DelegatePaint(OnStartPaint);
        detector.OnPaintMovement += new PaintingDetector2D.DelegatePaint(OnPaintMovement);
        detector.OnPaintEnd += new PaintingDetector2D.DelegatePaint(OnPaintEnd);
        palette.gameObject.SetActive(true);
        state = GameState.PiantBody;
    }

    void FinishBody()
    {
        state = GameState.FinishBody;
        detector.OnStartPaint -= new PaintingDetector2D.DelegatePaint(OnStartPaint);
        detector.OnPaintMovement -= new PaintingDetector2D.DelegatePaint(OnPaintMovement);
        detector.OnPaintEnd -= new PaintingDetector2D.DelegatePaint(OnPaintEnd);

        indicator.gameObject.SetActive(false);
        Debug.Log("Finish");
        Win();
    }

    void Win()
    {
        win.gameObject.SetActive(true);
    }

    void Brush(Vector2 p1, Vector2 p2)
    {
        //Debug.Log(p1 + ","+p2);
        int count;
        Drawing.PaintLineInMask(p1, p2, brushSize, paintTexture, outline, out count);
        cur += count;
        //Drawing.PaintLine(p1, p2, 10, Color.red, 1f, paintTexture);
        paintTexture.Apply();
    }

    void BrushInColor(Vector2 p1,Vector2 p2)
    {
        int count;
        Drawing.PaintLineInMaskColor(p1, p2, brushSize, paintTexture, outline, color, out count);
        cur += count;
        paintTexture.Apply();
    }
}
