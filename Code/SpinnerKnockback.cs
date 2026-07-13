using Sandbox;

public sealed class SpinnerKnockback : Component, Component.ICollisionListener
{
    [Property] public float Multiplier { get; set; } = 2f;

    [Property] public float VerticalBoost { get; set; } = 300f;

    [Property] public float PreventGroundingTime { get; set; } = 0.3f;

    Rigidbody _rigidbody;

    protected override void OnStart()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    public void OnCollisionStart( Collision collision )
    {
        if ( _rigidbody?.PhysicsBody == null ) return;

        PlayerController player = FindPlayerController( collision.Other.GameObject );
        if ( player == null ) return;

        Vector3 fromPivot = collision.Contact.Point - WorldPosition;
        Vector3 tangential = Vector3.Cross( _rigidbody.PhysicsBody.AngularVelocity, fromPivot );

        Vector3 launch = tangential * Multiplier + Vector3.Up * VerticalBoost;

        player.PreventGrounding( PreventGroundingTime );
        player.Body.Velocity += launch;
    }

    static PlayerController FindPlayerController( GameObject start )
    {
        for ( GameObject cursor = start; cursor.IsValid(); cursor = cursor.Parent )
        {
            PlayerController controller = cursor.GetComponent<PlayerController>();
            if ( controller != null ) return controller;
        }
        return null;
    }
}
