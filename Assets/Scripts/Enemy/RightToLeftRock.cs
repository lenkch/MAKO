using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class RightToLeftRock : MonoBehaviour
{
    public float speed = 3f;
    public Transform leftBound;
    public Transform rightBound;

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + Vector2.left * speed * Time.fixedDeltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform == leftBound)
        {
            rb.position = new Vector2(rightBound.position.x, rb.position.y);
        }
}
}
