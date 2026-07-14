using Sandbox;
using System;

public sealed class Pendulum : Component
{
    [Property] public Vector3 Axis { get; set; } = Vector3.Right;

    [Property] public float AmplitudeDegrees { get; set; } = 90f;

    [Property] public float PeriodSeconds { get; set; } = 3f;

    [Property] public float PhaseOffsetSeconds { get; set; } = 0f;

    Rigidbody _rigidbody;
    Rotation _startLocalRotation;

    protected override void OnStart()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _startLocalRotation = LocalRotation;
    }

    protected override void OnFixedUpdate()
    {
        Vector3 localAxis = Axis.Normal;
        float t = Time.Now + PhaseOffsetSeconds;
        float omega = MathF.Tau / MathF.Max( PeriodSeconds, 0.0001f );
        float amplitudeRad = AmplitudeDegrees * MathF.PI / 180f;

        if ( _rigidbody?.PhysicsBody != null )
        {
            float angularSpeed = amplitudeRad * omega * MathF.Cos( omega * t );
            Vector3 worldAxis = WorldRotation * localAxis;
            _rigidbody.PhysicsBody.AngularVelocity = worldAxis * angularSpeed;
            return;
        }

        float angleDeg = AmplitudeDegrees * MathF.Sin( omega * t );
        LocalRotation = _startLocalRotation * Rotation.FromAxis( localAxis, angleDeg );
    }
}
