using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [SerializeField] PlayerInputHandler _input;
    [SerializeField] RagdollCharacter _target;
    [SerializeField] float _distance = 5f;
    [SerializeField] float _sensitivity = 8f;
    [SerializeField] float _minVerticalAngle = -30f;
    [SerializeField] float _maxVerticalAngle = 70f;
    [SerializeField] Vector3 _offset = new Vector3(0f, 1.0f, 0f);
    [SerializeField] float _smoothSpeed = 10f;

    float _yaw;
    float _pitch = 15f;

    void LateUpdate()
    {
        if (_target == null || _target.pelvis == null) return;

        // Yaw follows character's facing direction
        _yaw = _target.pelvis.Rb.transform.eulerAngles.y;

        // Pitch still controlled by look input
        Vector2 look = _input.LookInput;
        _pitch -= look.y * _sensitivity * Time.deltaTime;
        _pitch = Mathf.Clamp(_pitch, _minVerticalAngle, _maxVerticalAngle);

        Quaternion rotation = Quaternion.Euler(_pitch, _yaw, 0f);
        Vector3 targetPos = _target.pelvis.transform.position + _offset;
        Vector3 desiredPos = targetPos - rotation * Vector3.forward * _distance;

        transform.position = Vector3.Lerp(transform.position, desiredPos, _smoothSpeed * Time.deltaTime);
        transform.LookAt(targetPos);
    }
}
