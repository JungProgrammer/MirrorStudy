using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitSelectionHandler : MonoBehaviour
{
    [SerializeField] 
    private RectTransform _unitSelectionArea = null;
    
    
    [SerializeField] 
    private LayerMask _layerMask;


    private Vector2 _startPosition;
    
    private RTSPlayer _rtsPlayer;
    private Camera _mainCamera;

    
    public List<Unit> SelectedUnits { get; } = new List<Unit>();


    private void Start()
    {
        _mainCamera = Camera.main;

        Unit.AuthorityOnUnitDespawned += AuthorityHandleUnitDespawned;
    }


    private void OnDestroy()
    {
        Unit.AuthorityOnUnitDespawned -= AuthorityHandleUnitDespawned;
    }


    private void Update()
    {
        if (_rtsPlayer == null)
        {
            _rtsPlayer = NetworkClient.connection.identity.GetComponent<RTSPlayer>();
        }
        
        
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            StartSelectionArea();
        }
        else if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            ClearSelectionArea();
        }
        else if (Mouse.current.leftButton.isPressed)
        {
            UpdateSelectionArea();
        }
    }


    private void StartSelectionArea()
    {
        if (!Keyboard.current.leftShiftKey.isPressed)
        {
            UnselectUnits();
        }
        
        _unitSelectionArea.gameObject.SetActive(true);

        _startPosition = Mouse.current.position.ReadValue();
        UpdateSelectionArea();
    }


    private void UpdateSelectionArea()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();

        float areaWidth = mousePosition.x - _startPosition.x;
        float areaHeight = mousePosition.y - _startPosition.y;

        _unitSelectionArea.sizeDelta = new Vector2(Mathf.Abs(areaWidth), Mathf.Abs(areaHeight));
        _unitSelectionArea.anchoredPosition = _startPosition + new Vector2(areaWidth / 2, areaHeight / 2);
    }
    

    private void ClearSelectionArea()
    {
        _unitSelectionArea.gameObject.SetActive(false);

        if (_unitSelectionArea.sizeDelta.magnitude < 100)
        {
            Ray ray = _mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _layerMask))
                return;
        
            if (!hit.collider.TryGetComponent(out Unit unit))
                return;
        
            if (!unit.hasAuthority)
                return;
        
        
            SelectedUnits.Add(unit);
            SelectUnits();   
            
            return;
        }


        Vector2 min = _unitSelectionArea.anchoredPosition - (_unitSelectionArea.sizeDelta / 2);
        Vector2 max = _unitSelectionArea.anchoredPosition + (_unitSelectionArea.sizeDelta / 2);

        foreach (Unit unit in _rtsPlayer.GetUnits())
        {
            if (SelectedUnits.Contains(unit))
                continue;
            
            
            Vector3 unitScreenPosition = _mainCamera.WorldToScreenPoint(unit.transform.position);

            if (unitScreenPosition.x > min.x
                && unitScreenPosition.x < max.x
                && unitScreenPosition.y > min.y
                && unitScreenPosition.y < max.y)
            {
                SelectedUnits.Add(unit);
                unit.Select();
            }
        }
    }


    private void SelectUnits()
    {
        foreach (Unit selectedUnit in SelectedUnits)
        {
            selectedUnit.Select();
        }
    }


    private void UnselectUnits()
    {
        foreach (Unit selectedUnit in SelectedUnits)
        {
            selectedUnit.Deselect();
        }
        
        SelectedUnits.Clear();
    }


    private void AuthorityHandleUnitDespawned(Unit unit)
    {
        SelectedUnits.Remove(unit);
    }
}
