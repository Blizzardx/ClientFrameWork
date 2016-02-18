using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class PositionData
{
    public PositionData() { }
    public PositionData(Vector2 localPos,Vector2 worldPos,Vector2 preLocalPos,Vector2 preWorldPos)
    {
        this.localPos = localPos;
        this.worldPos = worldPos;
        this.preLocalPos = preLocalPos;
        this.preWorldPos = preWorldPos;
    }
    public Vector2 localPos;
    public Vector2 worldPos;
    public Vector2 preLocalPos;
    public Vector2 preWorldPos;
}

public class PaintingDetector2D : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{

    RectTransform rectT;
    Vector2 lastPos;
    Vector2 lastWorldPos;

    public delegate void DelegatePaint(PositionData posData);
    public event DelegatePaint OnStartPaint;
    public event DelegatePaint OnPaintMovement;
    public event DelegatePaint OnPaintEnd;

    // Use this for initialization
    void Start () {
        rectT = GetComponent<RectTransform>();
	}

    public void OnPointerDown(PointerEventData eventData)
    {
        Vector2 localCursor;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectT, eventData.position, eventData.pressEventCamera, out localCursor);
        Vector2 offset = rectT.rect.position;
        Vector2 pos = localCursor - offset;

        //DrawPoint(pos);
        var posdata = new PositionData(pos,eventData.position,lastPos,lastWorldPos);
        if(OnStartPaint != null)
        {
            OnStartPaint(posdata);
        }
        
        lastPos = pos;
        lastWorldPos = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (lastPos != eventData.position)
        {
            Vector2 localCursor;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectT, eventData.position, eventData.pressEventCamera, out localCursor);
            Vector2 offset = rectT.rect.position;
            Vector2 pos = localCursor - offset;

            //DrawLine(lastPos, pos);
            var posdata = new PositionData(pos,eventData.position,lastPos,lastWorldPos);

            if (OnPaintMovement != null)
            {
                OnPaintMovement(posdata);
            }
            
            lastPos = pos;
            lastWorldPos = eventData.position;
        }
    }
    public void OnPointerUp(PointerEventData eventData)
    {
            Vector2 localCursor;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectT, eventData.position, eventData.pressEventCamera, out localCursor);
            Vector2 offset = rectT.rect.position;
            Vector2 pos = localCursor - offset;

            //DrawLine(lastPos, pos);
            var posdata = new PositionData(pos, eventData.position, lastPos, lastWorldPos);

            if (OnPaintEnd != null)
            {
                OnPaintEnd(posdata);
            }

            lastPos = pos;
            lastWorldPos = eventData.position;
    }
}
