using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EasyClick
{
    public class Minecart : MonoBehaviour
    {
        [SerializeField] WheelJoint2D _JointLeft;
        [SerializeField] WheelJoint2D _JointRight;
        [SerializeField] float _SpeedLeft;
        [SerializeField] float _SpeedRight;

        JointMotor2D _MotorLeft;
        JointMotor2D _MotorRight;

        void Start()
        {
            _MotorLeft.maxMotorTorque = 10_000f;
            _MotorLeft.motorSpeed = _SpeedLeft;

            _MotorRight.maxMotorTorque = 10_000f;
            _MotorRight.motorSpeed = _SpeedRight;
        }

        private void OnTriggerEnter2D(Collider2D collider)
        {
            if (collider.TryGetComponent<IBody>(out var body))
            {
                _JointLeft.motor = _MotorLeft;
                _JointRight.motor = _MotorRight;
            }
        }
    }
}