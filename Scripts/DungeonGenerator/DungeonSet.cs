using Godot;

public partial class DungeonSet : Node
{
    [Export] public Node3D[] Doors;
    [Export] public Node3D[] SpecialDoors;
    [Export] public Node3D prefab;
    [Export] public Special[] Specialgeneration;
    [Export] byte MaxHeightmapHeight;

    [Export] public SceneTree[] FlorTiles = new SceneTree[0]; 
    [Export] public SceneTree[] WallTiles = new SceneTree[0];
    [Export] public SceneTree[] SmallWallTiles = new SceneTree[0];
    [Export] public SceneTree[] RoofTiles = new SceneTree[0];
    [Export] public SceneTree[] DoorTiles = new SceneTree[0];
    [Export] public SceneTree[] ProtalTiles = new SceneTree[0];
}
