using UnityEngine;

public class MovingGround : MonoBehaviour
{
    [SerializeField] private float speed = 2f;
    [SerializeField] private Transform[] points;
    private int i = 0;
    void Start()
    {
        transform.position = points[0].position;
    }
    void Update()
    {
        if (Vector2.Distance(transform.position, points[i].position)<0.1f)
        {
            i++;
            if (i >= points.Length)
            {
                i = 0;
            }
        }

        transform.position = Vector2.MoveTowards(transform.position, points[i].position, speed * Time.deltaTime);

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.transform.SetParent(null);
        }
    }
}
