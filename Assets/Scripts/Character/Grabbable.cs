using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Grabbable : MonoBehaviour
{
    public Rigidbody Rb { get; private set; }

    void Awake()
    {
        Rb = GetComponent<Rigidbody>();
    }
}
