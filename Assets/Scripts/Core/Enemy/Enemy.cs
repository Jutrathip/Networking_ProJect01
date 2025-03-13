using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed = 2f;
    public Transform target; // ตั้งให้เป็น Base

    private void Start()
    {
        target = GameObject.FindWithTag("Base").transform;
    }

    private void Update()
    {
        if (target)
        {
            transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
        }
    }
}