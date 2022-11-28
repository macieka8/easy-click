using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticPositionAndRotation : MonoBehaviour
{
    [SerializeField] Vector3 _positionOffset;
    [SerializeField] Quaternion _rotation = Quaternion.identity;
    Transform _transform;

    void Awake()
    {
        _transform = transform;
    }

    void LateUpdate()
    {
        _transform.position = transform.parent.position + _positionOffset;
        _transform.rotation = _rotation;
    }
}
