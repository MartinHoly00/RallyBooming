using UnityEngine;

public class MenuCarDriver : MonoBehaviour
{
    public float speed = 10f;
    public Transform[] waypoints;

    private int currentIndex = 0;

    void Update()
    {
        if (waypoints.Length == 0) return;

        Transform target = waypoints[currentIndex];
        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
        transform.LookAt(target);

        if (Vector3.Distance(transform.position, target.position) < 0.2f)
        {
            currentIndex = (currentIndex + 1) % waypoints.Length;
        }
    }
}
