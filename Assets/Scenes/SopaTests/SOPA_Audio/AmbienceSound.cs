using UnityEngine;

public class AmbienceSound : MonoBehaviour
{
    public Collider Area;
    public GameObject Player;

    private void Update()
    {
        Vector3 closestPoint = Area.ClosestPoint(Player.transform.position);
        transform.position = closestPoint;
    }
}
