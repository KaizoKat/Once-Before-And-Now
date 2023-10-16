using Godot;
using System;

public partial class PlayerActions : Node
{
	[Export] PlayerActionSystem pacs;

	public override void _Ready()
	{
	}

	public override void _Process(double delta)
	{
		if(pacs.case_InsideOfRange(5.0f))
		{
            pacs.ApplyBleedTake(Input.IsActionJustPressed("Mouse Left"),false,20.0f,1.0f,10,delta);
            pacs.act_CnockBackEntity(Input.IsActionJustPressed("Mouse Left"), 10.0f);
        }

        pacs.ApplyBleedTake(Input.IsActionJustPressed("Enter"), false, 20.0f, 1.0f, 10, delta);
    }
}
