using UnityEngine;

public class GroundDetector : MonoBehaviour
{
    RagdollCharacter _character;

    [SerializeField] float _rayLength = 0.15f;
    [SerializeField] LayerMask _groundLayer;

    public bool IsGrounded { get; private set; }
    public Vector3 GroundNormal { get; private set; }

    void Awake()
    {
        _character = GetComponent<RagdollCharacter>();
    }

    void FixedUpdate()
    {
        IsGrounded = false;
        GroundNormal = Vector3.up;

        CheckFoot(_character.leftFoot);
        CheckFoot(_character.rightFoot);
    }

    void CheckFoot(BodyPart foot)
    {
        if (foot == null) return;

        var origin = foot.transform.position;
        if (UnityEngine.Physics.Raycast(origin, Vector3.down, out var hit, _rayLength, _groundLayer))
        {
            IsGrounded = true;
            GroundNormal = hit.normal;
        }
    }
}
