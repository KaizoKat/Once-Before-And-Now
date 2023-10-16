using Godot;
using System;

public partial class FollowNode : Node3D
{
	[Export] Node3D Holder;

	public override void _Ready()
	{
	}

	public override void _Process(double delta)
	{
		GlobalPosition = GlobalPosition.Lerp(Holder.GlobalPosition,0.2f);
	}
}
