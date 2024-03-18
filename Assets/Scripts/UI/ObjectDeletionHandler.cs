using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ObjectDeletionHandler : MonoBehaviour
{
    [SerializeField] private GameObject _debug;
    private GameObject selectedObject;

    private void OnEnable()
    {
        // Subscribe to the selectEntered and selectExited events.
        GetComponent<XRBaseInteractable>().selectEntered.AddListener(OnSelectEntered);
        GetComponent<XRBaseInteractable>().selectExited.AddListener(OnSelectExited);
    }

    private void OnDisable()
    {
        // Unsubscribe from the events to prevent memory leaks.
        GetComponent<XRBaseInteractable>().selectEntered.RemoveListener(OnSelectEntered);
        GetComponent<XRBaseInteractable>().selectExited.RemoveListener(OnSelectExited);
    }

    private void OnSelectEntered(SelectEnterEventArgs args)
    {
        selectedObject = args.interactable.gameObject;
        Debug.Log("Selected: " + selectedObject.name);
    }

    private void OnSelectExited(SelectExitEventArgs args)
    {
        if (args.interactable.gameObject == selectedObject)
        {
            selectedObject = null;
            Debug.Log("Deselected: " + args.interactable.gameObject.name);
        }
    }

    public void DeleteSelectedObject()
    {
        if (selectedObject != null)
        {
            _debug.GetComponent<TextMeshProUGUI>().SetText("\nArrived");
            // Delete or remove the selectedObject as needed.
            // Example: Destroy(selectedObject);
        }
    }
}

