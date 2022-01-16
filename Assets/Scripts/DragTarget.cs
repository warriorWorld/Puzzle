using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragTarget : MonoBehaviour
{
    private int id;
    private Collider2D currentCollision;
    private Dragable currentCollisionDragItem;
    private AudioSource audioSource;
    private BoxCollider2D boxCollider2D;
    private PuzzleManager puzzleManager;

    public void setPuzzleManager(PuzzleManager puzzleManager) { this.puzzleManager = puzzleManager; }

    public void setId(int id) { this.id = id; }

    public int getId() { return id; }

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
            if (ShareKeys.isSoundOpen())
            {
                audioSource.Play();
            }
            else
            {
                NativeCaller.vibrate();
            }
            currentCollision.transform.position = transform.position+currentCollisionDragItem.getPositionOffset();
            currentCollisionDragItem.setAttachedDragTarget(this);
            boxCollider2D.enabled = false;
            puzzleManager.solvedOne();
        }
    }

    public void releaseDragItem() {
        boxCollider2D.enabled = true;
        puzzleManager.unsolvedOne();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log("OnTriggerExit2D:" + collision.name);
    }
}
