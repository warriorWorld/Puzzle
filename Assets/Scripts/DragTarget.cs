using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragTarget : MonoBehaviour
{
    private Collider2D currentCollision;
    private Dragable currentCollisionDragItem;
    private AudioSource audioSource;
    private BoxCollider2D boxCollider2D;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        boxCollider2D = GetComponent<BoxCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("OnTriggerEnter2D:" + collision.name);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision != currentCollision)
        {
            currentCollision = collision;
            currentCollisionDragItem = collision.GetComponent<Dragable>();
        }
        if (null == currentCollisionDragItem)
        {
            return;
        }
        if (currentCollisionDragItem.isCollidable())
        {
            Debug.Log("OnTriggerStay2D:" + collision.name);
            audioSource.Play();
            currentCollision.transform.position = transform.position+currentCollisionDragItem.getPositionOffset();
            currentCollisionDragItem.setAttachedDragTarget(this);
            boxCollider2D.enabled = false;
        }
    }

    public void releaseDragItem() {
        boxCollider2D.enabled = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log("OnTriggerExit2D:" + collision.name);
    }
}
