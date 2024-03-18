using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class DesignPlacementManager : MonoBehaviour
{
    public GameObject SpawnableDesignProp;
    public XROrigin sessionOrigin;
    public ARRaycastManager RaycastManager;
    public ARPlaneManager PlaneManager;

    private List<ARRaycastHit> raycastHitList = new List<ARRaycastHit>();

    private void Update()
    {
        if(Input.touchCount > 0)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                bool collision = RaycastManager.Raycast(Input.GetTouch(0).position,raycastHitList, TrackableType.PlaneWithinPolygon);

                if (collision && isUiButtonPressed() == false)
                {
                    GameObject _gO = Instantiate(SpawnableDesignProp);
                    _gO.transform.position = raycastHitList[0].pose.position;
                    _gO.transform.rotation = raycastHitList[0].pose.rotation;

                    //Hide drawn planes
                    foreach (var plane in PlaneManager.trackables)
                    {
                        plane.gameObject.SetActive(false);
                    }
                    PlaneManager.enabled = false;
                }
            }

        }

    }

    public bool isUiButtonPressed()
    {
        if(EventSystem.current.currentSelectedGameObject?.GetComponent<Button>() == null)
        {
            return false;
        }
        else { return true; }
    }

    public void SwitchDesignProp(GameObject designProp)
    {
        SpawnableDesignProp = designProp;
    }

}
