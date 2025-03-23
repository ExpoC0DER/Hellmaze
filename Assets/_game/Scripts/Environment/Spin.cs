using UnityEngine;

public class Spin : MonoBehaviour
{
    [SerializeField] float speed = 50;
    void Update()
    {
        transform.Rotate(transform.forward * speed * Time.deltaTime);
    }
}