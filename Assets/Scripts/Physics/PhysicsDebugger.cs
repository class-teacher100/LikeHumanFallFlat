using UnityEngine;

public class PhysicsDebugger : MonoBehaviour
{
    [SerializeField] RagdollCharacter _character;
    [SerializeField] GroundDetector _groundDetector;
    [SerializeField] bool _showJoints = true;
    [SerializeField] bool _showForces = true;
    [SerializeField] bool _showGroundRays = true;

    void OnDrawGizmos()
    {
        if (_character == null) return;

        if (_showJoints)
            DrawJoints();

        if (_showForces)
            DrawVelocities();

        if (_showGroundRays)
            DrawGroundDetection();
    }

    void DrawJoints()
    {
        foreach (var part in _character.AllParts)
        {
            if (part == null || part.Joint == null) continue;

            Gizmos.color = Color.yellow;
            Vector3 anchor = part.transform.TransformPoint(part.Joint.anchor);
            Gizmos.DrawWireSphere(anchor, 0.03f);

            if (part.Joint.connectedBody != null)
            {
                Vector3 connectedAnchor = part.Joint.connectedBody.transform.TransformPoint(part.Joint.connectedAnchor);
                Gizmos.color = Color.green;
                Gizmos.DrawLine(anchor, connectedAnchor);
            }
        }
    }

    void DrawVelocities()
    {
        foreach (var part in _character.AllParts)
        {
            if (part == null || part.Rb == null) continue;

            Gizmos.color = Color.cyan;
            Gizmos.DrawRay(part.transform.position, part.Rb.linearVelocity * 0.2f);
        }
    }

    void DrawGroundDetection()
    {
        if (_groundDetector == null) return;

        Gizmos.color = _groundDetector.IsGrounded ? Color.green : Color.red;

        if (_character.leftFoot != null)
            Gizmos.DrawRay(_character.leftFoot.transform.position, Vector3.down * 0.15f);
        if (_character.rightFoot != null)
            Gizmos.DrawRay(_character.rightFoot.transform.position, Vector3.down * 0.15f);
    }
}
