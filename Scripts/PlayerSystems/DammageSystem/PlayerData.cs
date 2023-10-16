using Godot;
using System.Collections.Generic;

public partial class PlayerData : Node
{
    public int hp = 100;
    public int maxHP = 100;
    public int shield = 0;
    public int luck = 0;
    public int dammage = 34;

    public List<StatusEffects> Buffs = new List<StatusEffects>();
    public List<StatusEffects> Debuffs = new List<StatusEffects>();

    public QMS.Data SpaceData;

    [Export] QMS PlayerQMS;

    public override void _Ready()
    {
        base._Ready();
        PlayerQMS.set_defaults();
        SpaceData = PlayerQMS.get_PlayerData();
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        SpaceData = PlayerQMS.get_PlayerData();

        if (hp <= 0)
            PlayerQMS.fun_KillMe();
    }
}
