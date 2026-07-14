using Sandbox;
using System;

public sealed class LooseSpinner : Component
{
    [Property] public Vector3 Axis { get; set; } = Vector3.Up;

    /// <summary>
    /// Ambient RPM the spinner drifts toward when nothing is pushing it. Set to 0 for a spinner that comes to rest on its own.
    /// </summary>
    [Property] public float IdleSpeedRpm { get; set; } = 5f;

    /// <summary>
    /// Hard cap on spin speed in RPM so a big hit can't fling the spinner uncontrollably fast.
    /// </summary>
    [Property] public float MaxSpeedRpm { get; set; } = 120f;

    /// <summary>
    /// How quickly current spin speed relaxes back toward IdleSpeedRpm, in 1/seconds. 0 = never relaxes.
    /// </summary>
    [Property] public float SettleRate { get; set; } = 0.5f;

    Rigidbody _rigidbody;

    protected override void OnStart()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    protected override void OnFixedUpdate()
    {
        if ( _rigidbody?.PhysicsBody == null ) return;

        Vector3 worldAxis = (WorldRotation * Axis.Normal).Normal;

        // Project current angular velocity onto our axis, discarding off-axis wobble.
        Vector3 angVel = _rigidbody.PhysicsBody.AngularVelocity;
        float spinRadPerSec = Vector3.Dot( angVel, worldAxis );

        float idleRadPerSec = IdleSpeedRpm * MathF.Tau / 60f;
        float maxRadPerSec = MaxSpeedRpm * MathF.Tau / 60f;

        // Frame-rate independent exponential decay toward idle speed.
        float alpha = 1f - MathF.Exp( -MathF.Max( SettleRate, 0f ) * Time.Delta );
        spinRadPerSec = MathX.Lerp( spinRadPerSec, idleRadPerSec, alpha );
        spinRadPerSec = Math.Clamp( spinRadPerSec, -maxRadPerSec, maxRadPerSec );

        _rigidbody.PhysicsBody.AngularVelocity = worldAxis * spinRadPerSec;
    }
}
