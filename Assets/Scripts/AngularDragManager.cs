using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngularDragManager : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private float angularDrag = 1f;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // if (collision.gameObject.layer == layerMask)
        rb.freezeRotation = true;
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        // if (collision.gameObject.layer == layerMask)
        rb.freezeRotation = true;
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        // if (collision.gameObject.layer == layerMask)
        rb.freezeRotation = false;
        rb.angularDrag = angularDrag;
    }
}
