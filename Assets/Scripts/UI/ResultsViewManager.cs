using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class ResultsViewManager : MonoBehaviour
{

    public TMP_InputField searchInput;
    [SerializeField] private GameObject contents;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(this.isActiveAndEnabled && searchInput.text.Equals(string.Empty))
        {
            this.gameObject.SetActive(false);

            if (contents.transform.childCount > 0 )
            {
                DataHandler.Instance._debug.GetComponent<TextMeshProUGUI>().text += "\nDeleting...";
                // Clear existing children
                foreach (Transform child in contents.transform)
                {
                    Destroy(child.gameObject);
                }
            }
        }

    }

    public void CleanContents()
    {
        if (contents.transform.childCount > 0)
        {
            DataHandler.Instance._debug.GetComponent<TextMeshProUGUI>().text += "\nDeleting...";
            // Clear existing children
            foreach (Transform child in contents.transform)
            {
                Destroy(child.gameObject);
            }
        }
    }
}
