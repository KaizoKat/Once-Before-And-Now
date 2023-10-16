using Godot;
using System.Collections.Generic;

public partial class EntityData : Node
{
    public int hp = 100;
    public int maxHP = 100;
    public int shield = 0;
    public int dammage = 1;

    public List<StatusEffects> Buffs = new List<StatusEffects>();
    public List<StatusEffects> Debuffs = new List<StatusEffects>();

    public EQMS.Data SpaceData = new EQMS.Data();

    [Export] EQMS EntityQMS;

    public override void _Ready()
    {
        base._Ready();
        EntityQMS.set_defaults();
        SpaceData = EntityQMS.get_EntityData();
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        SpaceData = EntityQMS.get_EntityData();

        if (hp <= 0)
            EntityQMS.fun_KillMe();
    }
}
