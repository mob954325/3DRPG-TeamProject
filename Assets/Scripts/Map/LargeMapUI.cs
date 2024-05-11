using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

/// <summary>
/// MapUI�� �̺�Ʈ ���� Ŭ����
/// </summary>
public class LargeMapUI : MonoBehaviour, 
    IPointerClickHandler, IPointerMoveHandler, IDragHandler, 
    IBeginDragHandler, IEndDragHandler, IScrollHandler
{
    Vector2 mousePos;

    public Vector2 MousePos => mousePos;

    float screenWidth => Screen.width;

    /// <summary>
    /// ������ Ŭ������ �� �����ϴ� ��������Ʈ
    /// </summary>
    public Action<PointerEventData.InputButton, Vector2> onClick;

    /// <summary>
    /// �����ȿ� Mark ������Ʈ�� Pointer�� ���� ����Ǵ� ��������Ʈ
    /// </summary>
    public Action<Vector2> onPointerInMark;

    //public Action<Vector2> onPointerExitMark;

    public Action<Vector2> onPointerDragBegin;
    public Action<Vector2> onPointerDraging;
    public Action<Vector2> onPointerDragEnd;
    public Action<Vector2> onScroll;

    public void OnPointerClick(PointerEventData eventData)
    {
        onClick?.Invoke(eventData.button, eventData.position);
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        if(eventData.IsPointerMoving())
        {
            onPointerInMark?.Invoke(eventData.position);
        }
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        onPointerDragBegin?.Invoke(eventData.position);
    }

    public void OnDrag(PointerEventData eventData)
    {
        onPointerDraging?.Invoke(eventData.position);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        onPointerDragEnd?.Invoke(eventData.position);
    }

    public void OnScroll(PointerEventData eventData)
    {
        onScroll?.Invoke(eventData.scrollDelta);
    }
}