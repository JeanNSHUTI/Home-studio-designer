using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.AR;
using TMPro;
using UnityEngine.SceneManagement;

public class InputManager : ARBaseGestureInteractable
{
    [SerializeField] private Camera arCam;
    [SerializeField] private GameObject crosshairIndicator;
    [SerializeField] private XROrigin sessionOrigin;
    [SerializeField] private ARRaycastManager raycastManager;
    [SerializeField] private ARPlaneManager planeManager;
    [SerializeField] private XRSocketInteractor socket;

    [SerializeField] private GameObject _debug;
    [SerializeField] private GameObject buttonContainer;

    private List<ARRaycastHit> raycastHitList = new List<ARRaycastHit>();
    private Touch touch;
    private Pose pose;
    private bool planeState;
    private const string homeScene = "StartUpScene";

    // Start is called before the first frame update
    void Start()
    {
        planeState = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        CrosshairCalculation();
    }


    bool IsPointerOverUI(TapGesture touch)
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = new Vector2(touch.startPosition.x, touch.startPosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        return results.Count > 0;
    }

    void CrosshairCalculation()
    {
        Vector3 origin = arCam.ViewportToScreenPoint(new Vector3(0.5f, 0.5f, 0));
        Ray ray = arCam.ScreenPointToRay(touch.position);         

        if (GestureTransformationUtility.Raycast(origin, raycastHitList, sessionOrigin, TrackableType.PlaneWithinPolygon))
        {
            pose = raycastHitList[0].pose;

            // Calculate the angle between the camera forward vector and the world up vector
            float angle = Vector3.Angle(arCam.transform.forward, Vector3.up);

            // Adjust crosshair rotation based on angle
            if (angle > 125f)
            {
                crosshairIndicator.transform.eulerAngles = new Vector3(90f, 0f, 0f); // Face the floor
            }
            else
            {
                Vector3 hitNormal = raycastHitList[0].pose.rotation * Vector3.up;
                Quaternion rotationToWall = Quaternion.LookRotation(hitNormal, Vector3.up);
                crosshairIndicator.transform.rotation = rotationToWall;
                //crosshairIndicator.transform.eulerAngles = new Vector3(0f, 0f, 90f);
            }

            crosshairIndicator.transform.position = pose.position;
            //crosshairIndicator.transform.eulerAngles = new Vector3(90,0,0);
        }
    }

    protected override bool CanStartManipulationForGesture(TapGesture gesture)
    {
        if(gesture.targetObject == null) 
            return true;
        return false;
    }

    protected override void OnEndManipulation(TapGesture gesture)
    {
        if(gesture.isCanceled) 
            return;
        if(gesture.targetObject != null || IsPointerOverUI(gesture))
        {
            return;
        }
        if(GestureTransformationUtility.Raycast(gesture.startPosition, raycastHitList, xrOrigin))
        {
            GameObject placedObj = Instantiate(DataHandler.Instance.GetFurniture(), pose.position, pose.rotation);

            var anchorObject = new GameObject("PlacementAnchor");
            anchorObject.tag = "ArObject";
            anchorObject.transform.tag = "ArObject";
            anchorObject.transform.position = pose.position;
            anchorObject.transform.rotation = pose.rotation;

            placedObj.transform.parent = anchorObject.transform;
            //_debug.GetComponent<TextMeshProUGUI>().SetText("\nTag of: " + anchorObject.name + " is " + anchorObject.tag);

            Transform resultsView = GameObject.FindGameObjectWithTag("SearchResultsView").transform;
            if ( resultsView != null && resultsView.gameObject.activeInHierarchy)
            {
                resultsView.gameObject.SetActive(false);
            }
        }
    }


    public void DeleteARObjects()
    {
        int counter = 0;
        //_debug.GetComponent<TextMeshProUGUI>().SetText("Deleting ");
        GameObject[] arObjects = GameObject.FindGameObjectsWithTag("ArObject");
        foreach (GameObject arObject in arObjects)
        {
            Destroy(arObject);
            counter++;
            //_debug.GetComponent<TextMeshProUGUI>().SetText("\nDeleting " + counter + " : " + arObject.name);
        }
        //counter = arObjects.Length;
        //_debug.GetComponent<TextMeshProUGUI>().SetText("Deleted: " + counter);
    }

    public void TogglePlanes()
    {
        //_debug.GetComponent<TextMeshProUGUI>().SetText("Toggle");
        planeState = !planeState;
        bool state = planeState;
        ChangeStatePlanes(state);
    }

    public void ChangeStatePlanes(bool state)
    {
        foreach (var plane in planeManager.trackables)
        {
            plane.gameObject.SetActive(state);
        }
        planeManager.enabled = state;
    }

    public bool isUiButtonPressed()
    {
        if (EventSystem.current.currentSelectedGameObject?.GetComponent<Button>() == null)
        {
            return false;
        }
        else { return true; }
    }

    public void LoadStartUpScene()
    {
        SceneManager.LoadScene(homeScene);
    }


}
