using UnityEngine;

public class Spawner : MonoBehaviour
{
    public bool isFiring;
    [SerializeField] private float fireRate;
    private float nextFireTime;
    public bool isRotating;
    [SerializeField] private float angleLeft;
    [SerializeField] private float angleRight;
    private float targetAngle;
    [SerializeField] private float rotationSpeed = 1f;

    void Start()
    {
        isFiring = false;
        nextFireTime = 0f;
        targetAngle = angleLeft;
        isRotating = true;
    }

    void Update()
    {
        if (isFiring)
        {
            nextFireTime += Time.deltaTime;
            if (nextFireTime >= fireRate)
            {
                Fire();
                nextFireTime = 0f;
            }
            if (isRotating)
            {
                Rotate();
            }
        }
    }
    void Rotate()
    {
        Quaternion targetRotation =
            Quaternion.Euler(0f, 0f, targetAngle);

        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );

        if (Quaternion.Angle(transform.rotation, targetRotation) < 0.5f)
        {
            targetAngle = Mathf.Approximately(targetAngle, angleLeft) ? angleRight : angleLeft;
        }
    }
    void Fire()
    {
        GameObject meteor =
            GameManager.Instance.MeteorPool.GetObject();

        if (meteor == null)
            return;

        meteor.transform.SetPositionAndRotation(
            transform.position,
            transform.rotation
        );

        meteor.GetComponent<EnemyProjectile>().Launch();
    }

    private void OnDrawGizmos()
    {
        float gizmoLength = 2f;
        Gizmos.color = Color.green;
        Gizmos.DrawRay(
            transform.position,
            Quaternion.Euler(0f, 0f, angleLeft) * Vector3.right * gizmoLength);

        Gizmos.color = Color.red;
        Gizmos.DrawRay(
            transform.position,
            Quaternion.Euler(0f, 0f, angleRight) * Vector3.right * gizmoLength);
    }
}
