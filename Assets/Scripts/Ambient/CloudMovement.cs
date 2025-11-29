using UnityEngine;

public class CloudMovement : MonoBehaviour
{
    private Transform position;    
    [Header("Movement Values")]
    public float speed=0.005f;
    public float restartPosition=13f;
    void Start()
    {
        position = gameObject.GetComponent<Transform>();
    }

    void FixedUpdate()
    {
        position.position = new Vector3(position.position.x-speed, position.position.y, position.position.z);
        if (position.position.x < -restartPosition)
        {
            position.position = new Vector3(restartPosition, position.position.y, position.position.z);
        }
    }
}
