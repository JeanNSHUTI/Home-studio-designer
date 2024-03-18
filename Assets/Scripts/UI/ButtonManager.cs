using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ButtonManager : MonoBehaviour
{
    //[SerializeField] private RawImage buttonImage;
    private RawImage buttonImage;

    private Button btn;
    private string prefabId;
    private Sprite buttonTexture;

    public Sprite ButtonTexture 
    {
        set 
        {
            buttonTexture = value;
            buttonImage = GetComponent<RawImage>();
            buttonImage.texture = buttonTexture.texture;
        }
        
    }
    public string PrefabId { get { return prefabId; } set => prefabId = value; }


    // Start is called before the first frame update
    void Start()
    {
        btn = GetComponent<Button>();
        btn.onClick.AddListener(SelectObject);

    }

    // Update is called once per frame
    void Update()
    {
        if(MenuOptionController.Instance.OnEntered(gameObject))
        {
            transform.DOScale(Vector3.one * 2, 0.3f);
            
        }
        else
        {
            transform.DOScale(Vector3.one, 0.3f);
        }
    }

    void SelectObject()
    {
        //DataHandler.Instance.furniture = furniture;
        DataHandler.Instance.SetFurniture(PrefabId);
    }
}
