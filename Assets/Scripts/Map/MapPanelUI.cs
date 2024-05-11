using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using static UnityEngine.EventSystems.PointerEventData;

/// <summary>
/// Map �г� UI�� �����ϴ� Ŭ����
/// </summary>
public class MapPanelUI : MonoBehaviour
{
    /// <summary>
    /// Map�� ���� ī�޶�
    /// </summary>
    Camera mapCamera;

    LargeMapUI mapUI;

    /// <summary>
    /// �� �� ������
    /// </summary>
    public GameObject mapPingPrefab;

    /// <summary>
    /// �巡�� ���� Vector��
    /// </summary>
    Vector3 startDragVector = Vector3.zero;

    /// <summary>
    /// �巡�� ���� ������ Vector ��
    /// </summary>
    Vector3 onDragingVector = Vector3.zero;

    /// <summary>
    /// �巡�׸� �ϴ��� Ȯ���ϴ� �÷��� ����
    /// </summary>
    bool isDrag = false;

    private void Awake()
    {
        // Map UI �ʱ�ȭ
        mapUI = GetComponentInChildren<LargeMapUI>();

        mapUI.onClick += OnClickInput;

        mapUI.onPointerInMark += OnCheckMark;
        mapUI.onPointerDragBegin += OnDragEnter;
        mapUI.onPointerDraging += OnDraging;
        mapUI.onPointerDragEnd += OnDragEnd;
        mapUI.onScroll += OnScroll;
    }

    private void OnScroll(Vector2 scrollDelta)
    {
        float scroll = -scrollDelta.y;

        mapCamera.orthographicSize += scroll;
        mapCamera.orthographicSize = Mathf.Clamp(mapCamera.orthographicSize, 50, 100);
    }

    private void Start()
    {
        mapCamera = MapManager.Instance.MapCamera;
    }

    /// <summary>
    /// �巡�׸� ������ �� �����ϴ� �Լ�
    /// </summary>
    /// <param name="vector"></param>
    private void OnDragEnter(Vector2 vector)
    {
        startDragVector = new Vector3(vector.x, 0, vector.y);
        startDragVector += mapCamera.transform.position;
        MapManager.Instance.SetCameraPosition(startDragVector);
    }

    /// <summary>
    /// �巡�װ� �������� �� �����ϴ� �Լ�
    /// </summary>
    /// <param name="vector"></param>
    private void OnDraging(Vector2 vector)
    {
        isDrag = true;

        onDragingVector = new Vector3(vector.x, 0, vector.y);

        Vector3 result = startDragVector - onDragingVector;

        MapManager.Instance.SetCameraPosition(result);
    }

    /// <summary>
    /// �巡�װ� ������ �����ϴ� �Լ�
    /// </summary>
    /// <param name="vector"></param>
    private void OnDragEnd(Vector2 vector)
    {
        isDrag = false;
    }

    /// <summary>
    /// �ʿ� Ŭ������ �� ����Ǵ� �Լ�
    /// </summary>
    /// <param name="vector"></param>
    private void OnClickInput(InputButton button, Vector2 vector)
    {
        RaycastHit hit = GetObjectScreenToWorld(vector);
        Vector3 instantiateVector = hit.point;
        instantiateVector.y = 0;

        if(button == InputButton.Left)
        {
        }
        else if(button == InputButton.Right)
        {
            MapPointMark mark = hit.transform.gameObject?.GetComponent<MapPointMark>(); // ���� ������Ʈ�� Mark ������Ʈ���� Ȯ��

            if (mark != null)   // ���� ���� Mark�� �ִ�.
            {
                mark.DestoryMark();
            }
            else // Mark�� ���� : Mark ����
            {
                Instantiate(mapPingPrefab, instantiateVector, Quaternion.identity);  // PointObject
            }
        }
    }

    MapPointMark lastMark = null;

    /// <summary>
    /// �� �ȿ��� Mark�� �����Ͱ� ������ ����Ǵ� �Լ�
    /// </summary>
    /// <param name="pointObject">���� ������Ʈ</param>
    private void OnCheckMark(Vector2 pointVector)
    {
        RaycastHit hit = GetObjectScreenToWorld(pointVector);

        if (isDrag || hit.collider == null)
            return;

        MapPointMark mark = hit.transform.gameObject?.GetComponent<MapPointMark>(); // ���� ������Ʈ�� Mark ������Ʈ���� Ȯ��
        if (mark != null)
        {
            mark.EnableHighlightMark();
            lastMark = mark;
        }
        else if (lastMark != null)
        {
            lastMark.DisableHighlightMark();
        }
    }

    /// <summary>
    /// ��ũ������ ���� ������Ʈ�� ������ ���ϴ� �Լ�
    /// </summary>
    /// <param name="vector"></param>
    /// <returns></returns>
    private RaycastHit GetObjectScreenToWorld(Vector3 vector)
    {
        Ray ray = mapCamera.ScreenPointToRay(vector);   // ray
        RaycastHit hit;                                 // rayHit ����

        if (Physics.Raycast(ray, out hit, 1000f, LayerMask.GetMask("Map Object"))) // Map Object Ž��
        {
            Debug.DrawRay(ray.origin, ray.direction * 100, Color.red, 5f);
        }

        return hit;
    }
}