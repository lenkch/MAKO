using UnityEngine;

public class FallingRock : MonoBehaviour
{
    public float speed = 5f;
    public float resetY = -10f;
    public float startY = 10f;

    void Update()
    {
        transform.Translate(Vector3.down * speed * Time.deltaTime);

        if (transform.position.y <= resetY)
        {
            transform.position = new Vector3(
                transform.position.x,
                startY,
                transform.position.z
            );
        }
    }
}
