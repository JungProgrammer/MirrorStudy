using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Minimap : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    [SerializeField] 
    private RectTransform _minimapRect;


    [SerializeField] 
    private float _mapScale = 50f;


    [SerializeField] 
    private float _offset = -6f;
    

    private Transform _playerCameraTransform;


    private void Update()
    {
        if (_playerCameraTransform != null)
            return;
        
        if (NetworkClient.connection.identity == null)
            return;

        _playerCameraTransform = NetworkClient.connection.identity.GetComponent<RTSPlayer>().GetCameraTransform();
    }


    private void MoveCamera()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _minimapRect,
                mousePosition,
                null,
                out Vector2 localPoint
            ))
            return;


        Vector2 lerp = new Vector2(
            (localPoint.x - _minimapRect.rect.x) / _minimapRect.rect.width,
            (localPoint.y - _minimapRect.rect.y) / _minimapRect.rect.height);

        Vector3 newCameraPosition = new Vector3(
            Mathf.Lerp(-_mapScale, _mapScale, lerp.x),
            _playerCameraTransform.position.y,
            Mathf.Lerp(-_mapScale, _mapScale, lerp.y));

        _playerCameraTransform.position = newCameraPosition + new Vector3(0, 0, _offset);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        MoveCamera();
    }

    public void OnDrag(PointerEventData eventData)
    {
        MoveCamera();
    }
}
