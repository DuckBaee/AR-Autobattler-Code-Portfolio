using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

public class ARObjectPlacement : MonoBehaviour
{
    [SerializeField]
    private GameObject _world;

    [SerializeField] 
    private GameObject _ConfirmUI;
    
    GameObject _instanceMap;
    
    private GameManager _gameManager;
    private ARRaycastManager _raycastManager;
    private List<ARRaycastHit> hits = new List<ARRaycastHit>();
    public bool _mapHasBeenSpawned;
    public bool _mapConfirm;
    private void Start()
    {
        _mapHasBeenSpawned = false;
        _mapConfirm = false;
        _gameManager = FindAnyObjectByType<GameManager>();
        _raycastManager = GetComponent<ARRaycastManager>();
    }

    private void Update()
    {
        if (_gameManager.GetCurrentGameState() == GameManager.GameState.MapPlace)
        {
            if (Touchscreen.current.touches.Count > 0 && Touchscreen.current.touches[0].phase.value == TouchPhase.Began && !_mapHasBeenSpawned)
            {
                Vector2 touchPosition = Touchscreen.current.touches[0].position.ReadValue();

                // 터치한 위치에서 평면 감지
                if (_raycastManager.Raycast(touchPosition, hits, TrackableType.Planes))
                {
                    Pose hitPose = hits[0].pose;

                    // 오브젝트를 설치
                    _instanceMap = Instantiate(_world, hitPose.position + (Vector3.up * -0.7f), hitPose.rotation);
                    _mapHasBeenSpawned = true;
                    _ConfirmUI.SetActive(true);
                }
            }
        }
    }

    public void Confirm()
    {
        _mapConfirm = true;
        _ConfirmUI.SetActive(false);
        ARPlaneManager _aRPlaneManager = FindAnyObjectByType<ARPlaneManager>();
        _aRPlaneManager.requestedDetectionMode = PlaneDetectionMode.None;
        _aRPlaneManager.SetTrackablesActive(false);
    }

    public void Cancel()
    {
        Destroy(_instanceMap);
        _mapHasBeenSpawned = false;
        _ConfirmUI.SetActive(false);
    }
}
