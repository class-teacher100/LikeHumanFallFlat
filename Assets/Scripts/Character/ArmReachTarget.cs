using UnityEngine;

public class ArmReachTarget : MonoBehaviour
{
    [SerializeField] Transform _cameraTransform;
    [SerializeField] RagdollCharacter _character;
    [SerializeField] float _reachDistance = 1.5f;
    [SerializeField] float _armSpread = 0.25f;

    public Vector3 GetTargetPosition(bool isLeft)
    {
        Vector3 origin = _character.torso.transform.position;
        Vector3 forward = _cameraTransform.forward;
        Vector3 right = _cameraTransform.right;

        float side = isLeft ? -1f : 1f;
        return origin + forward * _reachDistance + right * (_armSpread * side);
    }

    public Vector3 GetReachDirection(bool isLeft)
    {
        Vector3 shoulder = isLeft
            ? _character.leftUpperArm.transform.position
            : _character.rightUpperArm.transform.position;

        return (GetTargetPosition(isLeft) - shoulder).normalized;
    }
}
