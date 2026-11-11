using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Framing")]
    [SerializeField] private float height = 14f;
    [SerializeField] private float pitch = 58f;
    [SerializeField] private float focusHeight = 0f;
    
    [Header("Smoothing")]
    [SerializeField] private float smoothTime = 0.15f;

    private Vector3 velocity;

    void Start()
    {
        if(target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if(player != null ) target = player.transform;
        }

        transform.rotation = Quaternion.Euler(pitch, 0f, 0f);
        if (target != null) transform.position = GetDesiredPosition();
    }

    void LateUpdate()
    {
        if (target == null) return;

        transform.rotation = Quaternion.Euler(pitch, 0f, 0f);
        transform.position = Vector3.SmoothDamp(transform.position, GetDesiredPosition(), ref velocity, smoothTime);
    }

    private Vector3 GetDesiredPosition()
    {
        float backDistance = height / Mathf.Tan(pitch *  Mathf.Deg2Rad);
        Vector3 focus = new Vector3(target.position.x, focusHeight, target.position.z);
        return focus + new Vector3(0f, height, -backDistance);
    }
}
