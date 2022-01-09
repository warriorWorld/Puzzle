using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Dragable : MonoBehaviour,IDragHandler,IBeginDragHandler,IEndDragHandler
{
    public Image image;

    public void OnBeginDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
