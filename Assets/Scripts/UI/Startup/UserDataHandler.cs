using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class UserDataHandler : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown selectedRoomTypePref;
    [SerializeField] private TMP_Dropdown selectedPriceRangePref;
    [SerializeField] private GameObject selectedStylePrefs;
    [SerializeField] private GameObject selectedPlacementPrefs;
    [SerializeField] private GameObject selectedPopularityPref;

    private static UserDataHandler instance;
    private const string mainScene = "SampleScene";

    public static UserDataHandler Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<UserDataHandler>();
            }
            return instance;
        }
    }

    public UserData UserData { get; set; } = new UserData(8, 4);

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SubmitUserPrefsAndLoadNewScene()
    {
        //Save user preferences
        UserData.UserTypePref = selectedRoomTypePref.options[selectedRoomTypePref.value].text;
        UserData.UserPriceRangePref = selectedPriceRangePref.options[selectedPriceRangePref.value].text;
        
        for(int i = 0; i < selectedStylePrefs.transform.childCount; i++)
        {
            if(selectedStylePrefs.transform.GetChild(i).GetComponent<Toggle>().isOn)
            {
                UserData.UserStylePrefs.Add(selectedStylePrefs.transform.GetChild(i).name);
            }
        }

        UserData.UserPlacementPrefs.Add("room");
        for(int i = 0; i < selectedPlacementPrefs.transform.childCount; i++)
        {
            if (selectedPlacementPrefs.transform.GetChild(i).GetComponent<Toggle>().isOn)
            {
                UserData.UserPlacementPrefs.Add(selectedPlacementPrefs.transform.GetChild(i).name);
            }
        }

        if (selectedPopularityPref.transform.GetChild(0).GetComponent<Toggle>().isOn)
        {
            UserData.UserPopularityInterest = true;
        }
        else
        {
            UserData.UserPopularityInterest = false;
        }
        
        //Load main scene
        SceneManager.LoadScene(mainScene);
    }
}
