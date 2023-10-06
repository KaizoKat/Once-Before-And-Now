using Godot;

public partial class Special : Node
{
    [Export] public byte generator;

    [Export] public byte heightOverwrite;
    [Export] public SceneTree FlorTilesOverwrite;
    [Export] public SceneTree WallTilesOverwrite;
    [Export] public SceneTree SmallWallTilesOverwrite;
    [Export] public SceneTree RoofTilesOverwrite;
    [Export] public SceneTree DoorTilesOverwrite;
    [Export] public SceneTree ProtalTilesOverwrite;
}
