using Unity.VisualScripting;
using UnityEngine;

public class FallingGround : MonoBehaviour
{
    [SerializeField] private float timeBeforeFall = 1f;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            Invoke("Fall", timeBeforeFall);
        }
    }
    private void Fall()
    {
        transform.AddComponent<Rigidbody2D>();
        Destroy(transform.parent.gameObject, 2f);
    }
}
