using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class MovingPlatform : MonoBehaviour
{
    public float speed = 2f;
    public Transform[] points;
    public float reachThreshold = 0.05f;
    private int index = 0;

    void Start()
    {
        if (points[0] != null)
            transform.position = points[0].position;
    }

    void Update()
    {
        if (points == null || points.Length == 0) return;
        if (points[index] == null) return;
        transform.position = Vector2.MoveTowards(transform.position, points[index].position, speed * Time.deltaTime);
        if (Vector2.Distance(transform.position, points[index].position) <= reachThreshold)
        {
            index++;
            if (index >= points.Length) index = 0;
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        TrySetPlayerParent(collision, true);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        TrySetPlayerParent(collision, true);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;
        StartCoroutine(DelayedUnparent(collision.transform));
    }

    private void TrySetPlayerParent(Collision2D collision, bool doParent)
    {
        if (!collision.gameObject.CompareTag("Player")) return;
        if (collision.contacts == null || collision.contacts.Length == 0) return;
        foreach (var contact in collision.contacts)
        {
            if (contact.normal.y > 0.5f)
            {
                StartCoroutine(DelayedParent(collision.transform));
                return;
            }
        }
    }

    private IEnumerator DelayedParent(Transform child)
    {
        yield return new WaitForEndOfFrame();
        child.SetParent(transform, true); 
    }

    private IEnumerator DelayedUnparent(Transform child)
    {
        yield return new WaitForEndOfFrame();

        if (child == null) yield break;
        if (child.parent == transform)
            child.SetParent(null, true);
    }
}
