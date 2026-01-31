using UnityEngine;

[RequireComponent(typeof(ThirdPersonCamera))]
public class CameraCollision : MonoBehaviour
{
    [SerializeField] RagdollCharacter _target;
    [SerializeField] float _sphereRadius = 0.2f;
    [SerializeField] LayerMask _collisionLayers;
    [SerializeField] Vector3 _offset = new Vector3(0f, 1.0f, 0f);

    void LateUpdate()
    {
        if (_target == null || _target.pelvis == null) return;

        Vector3 targetPos = _target.pelvis.transform.position + _offset;
        Vector3 direction = transform.position - targetPos;
        float distance = direction.magnitude;

        if (UnityEngine.Physics.SphereCast(targetPos, _sphereRadius, direction.normalized, out var hit, distance, _collisionLayers))
        {
            transform.position = targetPos + direction.normalized * (hit.distance - _sphereRadius * 0.5f);
        }
    }
}
