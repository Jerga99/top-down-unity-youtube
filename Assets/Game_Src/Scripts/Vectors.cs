
using Unity.VisualScripting;
using UnityEngine;

public class Vectors : MonoBehaviour
{
    // public Vector3 SpawnPoint = new Vector3(3, 0.25f, 3);
    // public Vector3 Direction = new Vector3(1, 0, 0);
    public float Speed = 2f;

    public Transform Enemy;

    void Start()
    {
        // transform.position = SpawnPoint;
    }


    void Update()
    {
        // transform.position += Direction.normalized * Time.deltaTime;
        // transform.position += Enemy.position.normalized * Time.deltaTime;

        var direction = Enemy.position - transform.position;
        var distance = direction.magnitude;

        if (distance < 2f)
        {
            return;
        }

        transform.position += direction.normalized * Speed * Time.deltaTime;

        var directionToPlayer = (transform.position - Enemy.position).normalized;

        float dotProduct = Vector3.Dot(Enemy.forward, directionToPlayer);

        Debug.Log(dotProduct);
    }
}
