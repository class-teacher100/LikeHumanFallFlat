using UnityEngine;

public class Platform : MonoBehaviour
{
    [SerializeField] bool _isMoving;
    [SerializeField] Vector3 _moveDirection = Vector3.right;
    [SerializeField] float _moveDistance = 3f;
    [SerializeField] float _moveSpeed = 1f;

    Vector3 _startPos;
    Rigidbody _rb;

    void Awake()
    {
        _startPos = transform.position;
        if (_isMoving)
        {
            _rb = gameObject.AddComponent<Rigidbody>();
            _rb.isKinematic = true;
            _rb.interpolation = RigidbodyInterpolation.Interpolate;
        }
    }

    void FixedUpdate()
    {
        if (!_isMoving || _rb == null) return;

        float t = Mathf.PingPong(Time.time * _moveSpeed, 1f);
        Vector3 target = _startPos + _moveDirection.normalized * _moveDistance * t;
        _rb.MovePosition(target);
    }
}
