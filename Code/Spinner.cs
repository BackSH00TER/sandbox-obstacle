using Sandbox;
using System;

public sealed class Spinner : Component
{
    [Property] public float SpeedRpm { get; set; } = 30f;

    [Property] public Vector3 Axis { get; set; } = Vector3.Up;

    Rigidbody _rigidbody;

    protected override void OnStart()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    protected override void OnFixedUpdate()
    {
        Vector3 localAxis = Axis.Normal;

        if ( _rigidbody?.PhysicsBody != null )
        {
            Vector3 worldAxis = WorldRotation * localAxis;
            _rigidbody.PhysicsBody.AngularVelocity = worldAxis * (SpeedRpm * MathF.Tau / 60f);
            return;
        }

        LocalRotation *= Rotation.FromAxis( localAxis, SpeedRpm * 6f * Time.Delta );
    }
}
