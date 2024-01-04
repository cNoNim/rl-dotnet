using System;
using System.Numerics;
using Box2D.NetStandard.Collision;
using Box2D.NetStandard.Collision.Shapes;
using Box2D.NetStandard.Dynamics.Bodies;
using Box2D.NetStandard.Dynamics.Contacts;
using Box2D.NetStandard.Dynamics.Fixtures;
using Box2D.NetStandard.Dynamics.Joints.Revolute;
using Box2D.NetStandard.Dynamics.World;
using Box2D.NetStandard.Dynamics.World.Callbacks;
using RL.Environments;
using RL.Environments.Spaces;
using RL.MDArrays;
using RL.Random;
using static System.MathF;
using static RL.Generators.Generator;
using Color = System.Drawing.Color;

namespace RL.Box2D;

public class LunarLanderEnvironment : EnvironmentBase<
    Box<Array1D<float>>,
    Discrete,
    Array1D<float>,
    int
>
{
    private const int Fps = 50;
    private const float Scale = 30;

    private const float MainEnginePower = 13;
    private const float SideEnginePower = 0.6f;

    private const float InitialRandom = 1000;

    private const float LegAway = 20;
    private const float LegDown = 18;
    private const float LegW = 2;
    private const float LegH = 8;
    private const float LegSpringTorque = 40;

    private const float SideEngineHeight = 14;
    private const float SideEngineAway = 12;
    private const float MainEngineYLocation = 4;

    private const int ViewportW = 600;
    private const int ViewportH = 400;

    private static readonly (float x, float y)[] LanderPoly =
    [
        (-14, +17),
        (-17, 0),
        (-17, -10),
        (+17, -10),
        (+17, 0),
        (+14, +17)
    ];

    private readonly Body?[] _legs = new Body?[2];

    private readonly World _world;
    private bool _gameOver;
    private float _helipadY;
    private Body? _lander;
    private Body? _moon;
    private float? _prevShaping;

    public LunarLanderEnvironment(float gravity = -10.0f) :
        base("LunarLander")
    {
        _world = new World(new Vector2(0, gravity));

        Array1D<float> high =
        [
            1.5f,
            1.5f,
            5.0f,
            5.0f,
            PI,
            5.0f,
            1.0f,
            1.0f,
        ];
        var low = -high;
        low[6] = 0.0f;
        low[7] = 0.0f;

        ObservationSpace = new Box<Array1D<float>>(low, high);
    }

    public IDebugDrawer? Drawer { get; set; }

    public World World => _world;

    public Body Lander => _lander ?? throw new InvalidOperationException();

    public override Box<Array1D<float>> ObservationSpace { get; }
    public override Discrete ActionSpace { get; } = new(4);
    public override int? MaxStepCount => 1000;

    protected override Array1D<float> DoReset()
    {
        Destroy();
        World.SetContactListener(new ContactDetector(this));
        _gameOver = false;
        _prevShaping = null;

        const float h = ViewportH / Scale;
        const float w = ViewportW / Scale;

        const int chunks = 11;

        var height = Random.Sequence(0, h / 2)
            .Take(chunks + 1)
            .ToMDArray();

        var chunkX = Range<int>(chunks)
            .Select(static i =>
                w / (chunks - 1) * i);

        _helipadY = h / 4;

        height[chunks / 2 - 2] = _helipadY;
        height[chunks / 2 - 1] = _helipadY;
        height[chunks / 2 + 0] = _helipadY;
        height[chunks / 2 + 1] = _helipadY;
        height[chunks / 2 + 2] = _helipadY;

        var smoothY = Range<int>(chunks)
            .Select(height, static (height, i) =>
            {
                var c = height.Count;
                var l = i == 0 ? c - 1 : i - 1;
                var r = i == c - 1 ? 0 : i + 1;
                return 0.33f * (height[l] + height[i] + height[r]);
            });

        var moonBodyDef = new BodyDef();
        _moon = _world.CreateBody(moonBodyDef);
        var moonShape = new EdgeShape(Vector2.Zero, new Vector2(w, 0));
        _moon.CreateFixture(moonShape);

        foreach (var i in Range<int>(chunks - 1))
        {
            var p1 = new Vector2(chunkX[i], smoothY[i]);
            var p2 = new Vector2(chunkX[i + 1], smoothY[i + 1]);
            var edgeShape = new EdgeShape(p1, p2);
            var fixtureDef = new FixtureDef { shape = edgeShape, density = 0.0f, friction = 0.1f };
            _moon.CreateFixture(fixtureDef);
        }

        const float initialY = ViewportH / Scale;
        const float initialX = ViewportW / Scale / 2;
        var landerBodyDef = new BodyDef
        {
            type = BodyType.Dynamic,
            position = new Vector2(initialX, initialY),
            angle = 0.0f
        };
        _lander = _world.CreateBody(landerBodyDef);

        var vertices = LanderPoly
            .AsGenerator()
            .Select(v => new Vector2(v.x, v.y) / Scale)
            .ToArray();

        var landerShape = new PolygonShape(vertices);
        var landerFixtureDef = new FixtureDef
        {
            shape = landerShape,
            density = 5.0f,
            friction = 0.1f,
            restitution = 0.0f,
            filter = new Filter
            {
                categoryBits = 0x0010,
                maskBits = 0x001
            }
        };
        _lander.CreateFixture(landerFixtureDef);

        var force = new Vector2(
            Random.Random(-InitialRandom, InitialRandom),
            Random.Random(-InitialRandom, InitialRandom)
        );
        _lander.ApplyForceToCenter(force);

        foreach (var index in Range<int>(_legs.Length))
        {
            var i = index == 0 ? -1 : 1;
            var legBodyDef = new BodyDef
            {
                type = BodyType.Dynamic,
                position = new Vector2(initialX - i * LegAway / Scale, initialY),
                angle = i * 0.05f,
                userData = false
            };
            var leg = _world.CreateBody(legBodyDef);
            var legShape = new PolygonShape(LegW / Scale, LegH / Scale);
            var legFixtureDef = new FixtureDef
            {
                shape = legShape,
                density = 1.0f,
                restitution = 0.0f,
                filter = new Filter
                {
                    categoryBits = 0x0020,
                    maskBits = 0x001
                }
            };
            leg.CreateFixture(legFixtureDef);

            var legJointDef = new RevoluteJointDef
            {
                bodyA = _lander,
                bodyB = leg,
                localAnchorA = Vector2.Zero,
                localAnchorB = new Vector2(i * LegAway / Scale, LegDown / Scale),
                enableMotor = true,
                enableLimit = true,
                maxMotorTorque = LegSpringTorque,
                motorSpeed = 0.3f * i,
                lowerAngle = index == 0 ? 0.9f - 0.5f : -0.9f,
                upperAngle = index == 0 ? 0.9f : 0.5f - 0.9f
            };
            _world.CreateJoint(legJointDef);
            _legs[index] = leg;
        }

        return Step(0).NextState;

        void Destroy()
        {
            if (_moon == null)
                return;
            _world.SetContactListener(null);
            _world.DestroyBody(_moon);
            _moon = null;
            _world.DestroyBody(_lander);
            _lander = null;
            foreach (var (leg, i) in _legs.AsGenerator().Index())
            {
                _world.DestroyBody(leg);
                _legs[i] = null;
            }
        }
    }

    protected override (Array1D<float> nextState, float reward, bool terminated)
        DoStep(Array1D<float> _, int action)
    {
        if (_lander == null)
            throw new InvalidOperationException();

        var angle = _lander.GetAngle();
        var position = _lander.Position;
        var (sin, cos) = SinCos(angle);
        var tip = new Vector2(sin, -cos);
        var side = new Vector2(-cos, -sin);

        var xRandom = Random.Random(-1f, 1f);
        var yRandom = Random.Random(-1f, 1f);
        var dispersion = new Vector2(xRandom, yRandom) / Scale;

        var mPower = 0.0f;
        if (action == 2)
        {
            mPower = 1.0f;

            var o = tip * (MainEngineYLocation / Scale + 2 * dispersion.X) + side * dispersion.Y;
            var impulsePos = position + o;
            var impulse = -o * MainEnginePower * mPower;
            _lander.ApplyLinearImpulse(impulse, impulsePos);
        }

        var sPower = 0.0f;
        if (action is 1 or 3)
        {
            var direction = action - 2;
            sPower = 1.0f;

            var o = tip * dispersion.X + side * (3 * dispersion.Y + direction * SideEngineAway / Scale);
            var impulsePos = position + o - tip * new Vector2(17, SideEngineHeight) / Scale;
            var impulse = -o * SideEnginePower * sPower;
            _lander.ApplyLinearImpulse(impulse, impulsePos);
        }

        _world.Step(1.0f / Fps, 6 * 30, 2 * 30);

        position = _lander.Position;
        var velocity = _lander.GetLinearVelocity();
        angle = _lander.GetAngle();
        var angularVelocity = _lander.GetAngularVelocity();

        var target = new Vector2(ViewportW / Scale / 2, _helipadY + LegDown / Scale);
        Drawer?.DrawSegment(position, target, Color.Red);
        var delta = position - target;

        const float wScale = ViewportW / Scale / 2;
        const float hScale = ViewportH / Scale / 2;
        Array1D<float> nextState =
        [
            delta.X / wScale,
            delta.Y / hScale,
            velocity.X * wScale / Fps,
            velocity.Y * hScale / Fps,
            angle,
            20.0f * angularVelocity / Fps,
            _legs[0]!.GetUserData<bool>() ? 1.0f : 0.0f,
            _legs[1]!.GetUserData<bool>() ? 1.0f : 0.0f
        ];

        var reward = 0.0f;

        var shaping =
            -100 * Sqrt(nextState[0] * nextState[0] + nextState[1] * nextState[1])
            - 100 * Sqrt(nextState[2] * nextState[2] + nextState[3] * nextState[3])
            - 100 * Abs(nextState[4])
            + 10 * nextState[6]
            + 10 * nextState[7];

        if (_prevShaping.HasValue)
            reward = shaping - _prevShaping.Value;
        _prevShaping = shaping;

        reward -= mPower * 0.3f;
        reward -= sPower * 0.03f;

        var terminated = false;
        if (_gameOver || Abs(nextState[0]) >= 1.0f)
        {
            terminated = true;
            reward = -100;
        }

        if (!_lander.IsAwake())
        {
            terminated = true;
            reward = +100;
        }

        return (nextState, reward, terminated);
    }

    public override int Heuristic(Array1D<float> state)
    {
        var angle = state[0] * 0.5f + state[2] * 1.0f;
        if (angle > 0.4f)
            angle = 0.4f;
        if (angle < -0.4f)
            angle = -0.4f;
        var hover = 0.55f * Abs(state[0]);
        var angleDelta = (angle - state[4]) * 0.5f - state[5] * 1.0f;
        var hoverDelta = (hover - state[1]) * 0.5f - state[3] * 0.5f;
        if (state[6] != 0.0f || state[7] != 0.0f)
        {
            angleDelta = 0;
            hoverDelta = -state[3] * 0.5f;
        }

        if (hoverDelta > Abs(angleDelta) && hoverDelta > 0.05f)
            return 2;
        if (angleDelta < -0.05f)
            return 3;
        if (angleDelta > +0.05f)
            return 1;
        return 0;
    }

    private class ContactDetector(LunarLanderEnvironment environment) : ContactListener
    {
        public override void BeginContact(in Contact contact)
        {
            var lander = environment._lander;
            if (lander == contact.FixtureA.Body || lander == contact.FixtureB.Body)
                environment._gameOver = true;
            foreach (var leg in environment._legs)
                if (leg == contact.FixtureA.Body || leg == contact.FixtureB.Body)
                    leg.SetUserData(true);
        }

        public override void EndContact(in Contact contact)
        {
            foreach (var leg in environment._legs)
                if (leg == contact.FixtureA.Body || leg == contact.FixtureB.Body)
                    leg.SetUserData(false);
        }

        public override void PreSolve(in Contact contact, in Manifold oldManifold)
        {
        }

        public override void PostSolve(in Contact contact, in ContactImpulse impulse)
        {
        }
    }
}
