using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuOptionController : MonoBehaviour
{
    public Transform selectionPoint;
    //public Transform selectionPointV;

    private static MenuOptionController instance;
    private PointerEventData pEventData;
    private EventSystem eventSystem;
    private GraphicRaycaster raycaster;

    public static MenuOptionController Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<MenuOptionController>();
            }
            return instance;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        raycaster = GetComponent<GraphicRaycaster>();
        eventSystem = GetComponent<EventSystem>();
        pEventData = new PointerEventData(eventSystem);

        pEventData.position = selectionPoint.position;
        //Transform resultsView = GameObject.FindGameObjectWithTag("SearchResultsView").transform;
        /*if (resultsView != null && resultsView.gameObject.activeInHierarchy)
        {
            pEventData.position = selectionPointV.position;
        }*/
        

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool OnEntered(GameObject button)
    {
        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(pEventData, results);

        foreach(var result in results)
        {
            if(result.gameObject == button)
            {
                return true;
            }
        }
        return false;
    }
}
