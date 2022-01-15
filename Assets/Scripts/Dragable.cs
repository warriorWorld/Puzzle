using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class Dragable : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private int id;
    public Image image;
    private bool collidable = false;
    private DragTarget attachedDragTarget;
    public Vector2 positionOffset;
    private RectTransform solvedGroup, unsolvedGroup;
    public void setId(int id) { this.id = id; }

    public void setFutureParent(RectTransform solvedParent,RectTransform unsolvedParent)
    {
        this.solvedGroup = solvedParent;
        this.unsolvedGroup = unsolvedParent;
    }

    public Vector3 getPositionOffset() { return new Vector3(positionOffset.x,positionOffset.y,0); }
    public void setAttachedDragTarget(DragTarget attachedDragTarget)
    {
        this.attachedDragTarget = attachedDragTarget;
        transform.SetParent(solvedGroup);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        attachedDragTarget?.releaseDragItem();
        attachedDragTarget = null;
        transform.SetParent(unsolvedGroup);
        transform.position = eventData.position;
        collidable = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
        collidable = false;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        collidable = true;
    }

    public bool isCollidable() { return collidable; }

    // Start is called before the first frame update
    
    public bool isCorrect()
    {
        return id == attachedDragTarget?.getId();
    }
}
