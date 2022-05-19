using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class BuildingButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] 
    private Building _building;


    [SerializeField] 
    private Image _iconImage;


    [SerializeField] 
    private TMP_Text _priceText;


    [SerializeField] 
    private LayerMask _floorMask = new LayerMask();


    private Camera _mainCamera;
    private RTSPlayer _rtsPlayer;
    private GameObject _buildingPreviewInstance;
    private Renderer _buildingRendererInstance;


    private void Start()
    {
        _mainCamera = Camera.main;

        _iconImage.sprite = _building.GetIcon();
        _priceText.text = _building.GetPrice().ToString();
    }


    private void Update()
    {
        if (_rtsPlayer == null)
        {
            _rtsPlayer = NetworkClient.connection.identity.GetComponent<RTSPlayer>();
        }
        
        
        if (_buildingPreviewInstance == null)
            return;
        
        
        UpdateBuildingPreview();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;


        _buildingPreviewInstance = Instantiate(_building.GetBuildingPreview());
        _buildingRendererInstance = _buildingPreviewInstance.GetComponentInChildren<Renderer>();
        
        _buildingPreviewInstance.SetActive(false);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (_buildingPreviewInstance == null)
            return;


        Ray ray = _mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _floorMask))
        {
            // place building
            
            
        }
        
        Destroy(_buildingPreviewInstance);
    }


    private void UpdateBuildingPreview()
    {
        Ray ray = _mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _floorMask))
            return;


        _buildingPreviewInstance.transform.position = hit.point;

        if (!_buildingPreviewInstance.activeSelf)
        {
            _buildingPreviewInstance.SetActive(true);
        }
    }
}