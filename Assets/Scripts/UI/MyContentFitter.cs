using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MyContentFitter : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        /*HorizontalLayoutGroup horizontalLayoutGroup = GetComponent<HorizontalLayoutGroup>();
        int childCount = transform.childCount - 1;
        float childWidth = transform.GetChild(0).GetComponent<RectTransform>().rect.width;
        float width = horizontalLayoutGroup.spacing * childCount + childCount * childWidth + horizontalLayoutGroup.padding.left + childWidth;

        GetComponent<RectTransform>().sizeDelta = new Vector2 (width, 265);*/
    }


    public void Fit()
    {
        HorizontalLayoutGroup hg = GetComponent<HorizontalLayoutGroup>();
        int childCount = transform.childCount - 1;
        float childWidth = transform.GetChild(0).GetComponent<RectTransform>().rect.width;
        float width = hg.spacing * childCount +
                      childCount * childWidth +
                      hg.padding.left +
                      childWidth;

        Vector2 size = GetComponent<RectTransform>().sizeDelta;
        GetComponent<RectTransform>().sizeDelta = new Vector2(width, size.y);
    }

    public void FitVertical()
    {
        VerticalLayoutGroup lg = GetComponent<VerticalLayoutGroup>();
        int childCount = transform.childCount - 1;
        float childHeight = transform.GetChild(0).GetComponent <RectTransform>().rect.height;
        float height = lg.spacing * childCount +
            childCount * childHeight +
            lg.padding.top + childHeight;

        Vector2 size = GetComponent<RectTransform>().sizeDelta;
        GetComponent<RectTransform>().sizeDelta = new Vector2(size.x, height);
    }
}
