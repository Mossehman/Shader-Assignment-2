using UnityEngine;

public class FishScript : MonoBehaviour
{
    public float moveSpeed = 4.0f;

    void Update()
    {
        transform.position = new Vector3(transform.position.x + moveSpeed * Time.deltaTime, transform.position.y, transform.position.z);

        if (transform.position.x > 10)
        {

            transform.position = new Vector3(-9, transform.position.y, transform.position.z);
        }
        else if (transform.position.x < -10)
        {
            transform.position = new Vector3(9, transform.position.y, transform.position.z);
        }
    }
}
