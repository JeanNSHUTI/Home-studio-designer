using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName ="DesignItem", menuName ="AddDesignOption/DesignItem")]
public class DesignItem : ScriptableObject
{
    public string prefabId;
    public string description;
    public string type;
    public string placementTags;
    public string styleTags;
    public string packageTag;
    public string priceRangeTag;
    public string musicStyleTags;
    public int popularityScore;

    public GameObject designItemPrefab;
    public Sprite designItemImage;


}
