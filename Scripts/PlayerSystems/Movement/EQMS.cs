using Godot;

public partial class EQMS : Node
{
    float maxSpeed;
    float maxWaterSpeed;
    float maxAirSpeed = 12.0f;
    float maxAccel;
    float clFwrdSpd = 1.0f;
    float clSideSpd = 1.0f;
    float jumpForce;
    float sensetivity = 5.0f;

    [Export] RigidBody3D rig;
    [Export] Node3D orientation;
    [Export] Node3D head;
    [Export] Node3D body;
    [Export] RayCast3D groundCast;
    [Export] RayCast3D slopeCast;
    [Export] ShapeCast3D waterBoxCast;

    bool jumped;
    float startYscale;
    float drag;
    float groundDrag;
    float waterDrag;
    float wait0;
    Vector3 dir;
    GodotObject oid;

    public bool readyFlag = false;

    public override void _Ready()
    {
        set_defaults();
        startYscale = rig.Scale.Y;

        readyFlag = true;
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        maxAccel = maxSpeed * 10;

        if (!phy_Sloped()) dir = fun_CalculateWishDir();
        else dir = phy_GetSlopeDir();

        fun_DragCalculations();
        fun_UpdateVelocity(delta);
    }

    void fun_DragCalculations()
    {
        if (phy_Grounded() || phy_Sloped()) drag = groundDrag;
        else if (waterBoxCast.IsColliding()) drag = waterDrag;
        else drag = 0;

        if (waterBoxCast.IsColliding() || phy_Sloped())
            rig.GravityScale = 0;
        else 
            rig.GravityScale = 6;
    }
    void fun_UpdateVelocity(double frame)
    {
        if (phy_Grounded() || phy_Sloped() && !waterBoxCast.IsColliding())
            rig.LinearVelocity = phy_UpdateVelGround(dir, rig.LinearVelocity, frame);
        else if (!waterBoxCast.IsColliding())
            rig.LinearVelocity = phy_UpdateVelAir(dir, rig.LinearVelocity, frame);
        else
            rig.LinearVelocity = phy_UpdateVelWater(dir, rig.LinearVelocity, frame);
    }
    void phy_Jump(double frame)
    {
        if (jumped == false && !waterBoxCast.IsColliding())
        {
            if (phy_Grounded() && !waterBoxCast.IsColliding())
            {
                rig.ApplyCentralImpulse(jumpForce * Vector3.Up);
                jumped = true;
            }
        }
        else if (waterBoxCast.IsColliding())
        {
            rig.ApplyCentralForce(rig.Basis.Y * jumpForce * 1.5f);
        }

        if (jumped)
        {
            wait0 += (float)frame;

            if (wait0 > 0.6f && phy_Grounded())
            {
                jumped = false;
                wait0 = 0;
            }
        }

        rig.ForceUpdateTransform();
    }
    void fun_CalculateFrinction(ref Vector3 vel, float dt)
    {
        float spd = vel.Length();

        if (spd <= 0.00001f) return;

        float downLimit = Mathf.Max(spd, 0.5f);
        float dampAmmount = spd - (downLimit * drag * dt);

        if(dampAmmount <0) dampAmmount = 0;

        vel *= dampAmmount / spd;
    }
    public void fun_KillMe()
    {
        QueueFree();
    }

    bool phy_Grounded()
    {
        return groundCast.IsColliding();
    }
    bool phy_Sloped()
    {
        if(slopeCast.IsColliding())
        {
            float angle = Vector3.Up.AngleTo(slopeCast.GetCollisionNormal());
            return angle < 45 && angle != 0;
        }

        return false;
    }

    Vector3 fun_CalculateWishDir()
    {
        Vector3 dirc = act_DircFromPos(Vector3.Zero);
        return (
            -orientation.Transform.Basis.Z * dirc.X * clFwrdSpd
            + orientation.Transform.Basis.X * dirc.Z * clSideSpd
            ).Normalized();
    }
    Vector3 phy_UpdateVelGround(Vector3 wishDir, Vector3 vel, double frame)
    {
        fun_CalculateFrinction(ref vel, (float)frame);

        float currSpd = vel.Dot(wishDir);
        float addSpd = (float)Mathf.Clamp(maxSpeed - currSpd, 0, maxAccel * frame);

        return vel + addSpd * wishDir;
    }
    Vector3 phy_UpdateVelAir(Vector3 wishDir, Vector3 vel, double frame)
    {

        float currSpd = vel.Dot(wishDir);
        float addSpd = (float)Mathf.Clamp(maxAirSpeed - currSpd, 0, maxAccel * frame);

        return vel + addSpd * wishDir;
    }
    Vector3 phy_UpdateVelWater(Vector3 wishDir, Vector3 vel, double frame)
    {
        fun_CalculateFrinction(ref vel, (float)frame);

        float currSpd = vel.Dot(wishDir);
        float addSpd = (float)Mathf.Clamp(maxWaterSpeed - currSpd, 0, maxAccel * frame);

        return vel + addSpd * wishDir;
    }
    Vector3 phy_GetSlopeDir()
    {
        return ProjectOnPlane(fun_CalculateWishDir(), slopeCast.GetCollisionNormal()).Normalized();
    }
    Vector3 ProjectOnPlane(Vector3 vector, Vector3 planeNormal)
    {
        float num = planeNormal.Dot(planeNormal);
        if (num < Mathf.Epsilon)
        {
            return vector;
        }

        float num2 = vector.Dot(planeNormal);
        return new Vector3(
            vector.X - planeNormal.X * num2 / num, 
            vector.Y - planeNormal.Y * num2 / num, 
            vector.Z - planeNormal.Z * num2 / num);
    }
    Vector3 act_DircFromPos(Vector3 globalPoint)
    {
        Vector3 direc = body.GlobalPosition - globalPoint;
        return direc.Normalized();
    }

    public void set_EntityData(Data playerData)
    {
        groundDrag = playerData._groundDrag;
        waterDrag = playerData._waterDrag;
        jumpForce = playerData._jumpFroce;
        maxSpeed = playerData._walkSpeed;
        maxWaterSpeed = playerData._walkSpeed;
    }
    public void set_defaults()
    {
        maxSpeed = 0.0f;
        maxWaterSpeed = 0.0f;
        maxAccel = maxSpeed * 10;
        jumpForce = 15.0f;
        groundDrag = 6.0f;
        waterDrag = 8.0f;
    }
    public Data get_EntityData()
    {
        return new Data()
        {
            _velocity = rig.AngularVelocity,
            _position = body.GlobalPosition,
            _groundDrag = groundDrag,
            _waterDrag = waterDrag,
            _jumpFroce = jumpForce,
            _walkSpeed = maxSpeed,
            _waterSpeed = maxAirSpeed
        };
    }
    public struct Data
    {
        public Vector3 _position;
        public Vector3 _velocity;
        public float _groundDrag;
        public float _waterDrag;
        public float _jumpFroce;
        public float _waterSpeed;
        public float _walkSpeed;
    }
}