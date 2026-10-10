using System.Numerics;

namespace RL.Box2D;

public readonly record struct LunarLanderObservation(
    Vector2 Delta,
    Vector2 Velocity,
    float Angle,
    float AngularVelocity,
    bool LeftContact,
    bool RightContact
);
