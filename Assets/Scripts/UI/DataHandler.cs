using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.InputSystem.Android;
using UnityEngine.UI;

public class DataHandler : MonoBehaviour
{
    private GameObject furniture;

    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private GameObject buttonContainer;
    [SerializeField] private List<DesignItem> items;
    [SerializeField] private string label;

    [SerializeField] public GameObject _debug;
    public int counter = 1;

    private static DataHandler instance;
    private IList<UnityEngine.ResourceManagement.ResourceLocations.IResourceLocation> locations;
    private int numberOfLocations = 0;
    private int startDownloadIndex = 0;
    private int endDownloadIndex = 0;
    private const int NUMBER_OF_MODELS_PER_DOWNLOAD = 9;

    public static DataHandler Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<DataHandler>();
            }
            return instance;
        }
    }

    public GameObject GetFurniture() { return furniture; }

    public void SetFurniture(string id)
    {
        foreach(DesignItem item in items)
        {
            if(item.prefabId == id)
            {
                furniture = item.designItemPrefab;
            }
        }
    }

    private async void Start()
    {
        items = new List<DesignItem>();
        await GetLocations();  
        await Get(label);
        //_debug.GetComponent<TextMeshProUGUI>().SetText("executed: " + counter);
        CreateButtons();
        //Rebuild ML graph
        FurnitureGraph.Instance.RebuildGraph();
    }

    public async void LoadMoreModelButtons()
    {
        startDownloadIndex += NUMBER_OF_MODELS_PER_DOWNLOAD;
        await Get(label);
        CreateButtons();

        //Rebuild ML graph
        FurnitureGraph.Instance.RebuildGraph();
    }

    public List<DesignItem> GetItemsCopy()
    {
        List<DesignItem> copy = new List<DesignItem>(items);
        return copy;
    }

    void CreateButtons()
    {
        //_debug.GetComponent<TextMeshProUGUI>().SetText("Count: " + buttonContainer.transform.childCount);
        /*foreach (DesignItem item in items)
        {
            GameObject b = Instantiate(buttonPrefab, buttonContainer.transform);
            b.GetComponent<ButtonManager>().PrefabId = item.prefabId;
            b.GetComponent<ButtonManager>().ButtonTexture = item.designItemImage;
            buttonContainer.GetComponent<MyContentFitter>().Fit();
            counter++;
        }*/

        if (startDownloadIndex + NUMBER_OF_MODELS_PER_DOWNLOAD < endDownloadIndex)
        {
            for (int i = startDownloadIndex; i < startDownloadIndex + NUMBER_OF_MODELS_PER_DOWNLOAD; i++)
            {
                GameObject b = Instantiate(buttonPrefab, buttonContainer.transform);
                b.GetComponent<ButtonManager>().PrefabId = items[i].prefabId;
                b.GetComponent<ButtonManager>().ButtonTexture = items[i].designItemImage;
                buttonContainer.GetComponent<MyContentFitter>().Fit();
                counter++;
            }
            return;
        }
        else if (endDownloadIndex - startDownloadIndex > 0)
        {
            for (int x = startDownloadIndex; x <= endDownloadIndex; x++)
            {
                GameObject b = Instantiate(buttonPrefab, buttonContainer.transform);
                b.GetComponent<ButtonManager>().PrefabId = items[x].prefabId;
                b.GetComponent<ButtonManager>().ButtonTexture = items[x].designItemImage;
                buttonContainer.GetComponent<MyContentFitter>().Fit();
                counter++;
            }
            return;
        }
        else { return; }



    }

    public async Task GetLocations()
    {
        locations = await Addressables.LoadResourceLocationsAsync(label).Task;
        numberOfLocations = locations.Count;
        endDownloadIndex = numberOfLocations - 1;
        _debug.GetComponent<TextMeshProUGUI>().SetText("Loading... " + numberOfLocations);
    }

    public async Task Get(string label)
    {
        /*foreach (var location in locations)
        {
            await Addressables.InstantiateAsync(location).Task;
            DesignItem obj = await Addressables.LoadAssetAsync<DesignItem>(location).Task;
            items.Add(obj);
        }*/

        if(startDownloadIndex + NUMBER_OF_MODELS_PER_DOWNLOAD < endDownloadIndex)
        {
            //load 9 new models
            for(int i = startDownloadIndex; i < startDownloadIndex + NUMBER_OF_MODELS_PER_DOWNLOAD; i++)
            {
                await Addressables.InstantiateAsync(locations[i]).Task;
                DesignItem obj = await Addressables.LoadAssetAsync<DesignItem>(locations[i]).Task;
                items.Add(obj);
            }
            _debug.GetComponent<TextMeshProUGUI>().SetText("Loaded files: " + items.Count);
            //_debug.GetComponent<TextMeshProUGUI>().SetText(UserDataHandler.Instance.UserData.ToString());
       
        }
        else if(endDownloadIndex - startDownloadIndex > 0)
        {
            //Load remaining models
            for(int x =  startDownloadIndex; x <= endDownloadIndex; x++)
            {
                await Addressables.InstantiateAsync(locations[x]).Task;
                DesignItem obj = await Addressables.LoadAssetAsync<DesignItem>(locations[x]).Task;
                items.Add(obj);
            }
            //_debug.GetComponent<TextMeshProUGUI>().SetText("All files loaded: " + items.Count);

        }
        else
        {
            //All models have been loaded
            _debug.GetComponent<TextMeshProUGUI>().SetText("All files loaded: " + items.Count);

        }
        
    }

}
