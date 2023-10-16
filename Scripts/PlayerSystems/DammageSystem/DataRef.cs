using Godot;
using System;

public partial class DataRef : Node3D
{
    [Export] public PlayerData conPlayerData;
    [Export] public EntityData conEntityData;
    [Export] public RigidBody3D conRig;
}
