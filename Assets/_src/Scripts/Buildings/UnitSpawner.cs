using Mirror;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitSpawner : NetworkBehaviour, IPointerClickHandler
{
    [SerializeField] 
    private GameObject _unitPrefab = null;


    [SerializeField] 
    private Transform _unitSpawnPoint;


    #region Server

    [Command]
    private void CmdSpawnUnit()
    {
        GameObject unitInstance = Instantiate(_unitPrefab, _unitSpawnPoint.position, _unitSpawnPoint.rotation);
        
        NetworkServer.Spawn(unitInstance, connectionToClient);
    }

    #endregion


    #region Client

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;
        if (!hasAuthority)
            return;
        
        
        CmdSpawnUnit();
    }

    #endregion
}
