using Godot;
using System.Collections.Generic;
using System.Linq;

public partial class DungeonCrafter : Node
{
	[Export] DungeonData Data;
	[Export] int portalsCount;
	[Export] int DataSetID;
	[Export] int RoomSize;
	[Export] int CellSize;
	[Export] ulong seed;

    HashSet<Vector2I> GenerateHashes(List<Node3D> _Doros, byte generator)
	{
        List<Vector3> Doors = new List<Vector3>();
		HashSet<Vector2I> RoomHash = new HashSet<Vector2I>();
		HashSet<Vector2I> CellHash = new HashSet<Vector2I>();

        HashSet<Vector2I> portals = new HashSet<Vector2I>();
        HashSet<HashOverlay2D> roomProtalOverlay = new HashSet<HashOverlay2D>();

        foreach (Node3D door in _Doros)
		{
            portals.Add(new Vector2I((int)door.Position.X, (int)door.Position.Z));
            Doors.Add(door.Position);
        }

		// calculate the center of the dungeon
		Vector2I center = Vector2I.Zero;
		foreach (Vector3 door in Doors)
		{
            center.X = center.X + (int)door.X;
			center.Y = center.Y + (int)door.Z;
        }

		center = center / Doors.Count;

        // "intantiante" walkers in the center of the dungeon (walkers count is equal to the number of doors) 
        List<Walker> Walkers = new List<Walker>();
		foreach (Vector3 door in Doors)
			Walkers.Add(new Walker(center));

		List<Vector2I> roomEnds = new List<Vector2I>();

        // influence each one's movment towards one of the doors
        // i.e. one will do drunk walking, but it will have a higher chance to move towards its designated door
        for (int i = 0; i < Doors.Count; i++)
		{
			Vector2I doorPos = new Vector2I((int)Doors[i].X, (int)Doors[i].Z);
            while (Doors[i].DistanceTo(new Vector3(Walkers[i].pos.X, Doors[i].Y, Walkers[i].pos.Y)) > RoomSize)
			{
				if(generator == 0)
					RoomHash.Add(Walkers[i].TakeStepInfluenced(Vector2I.One * RoomSize, doorPos, seed));
				else if(generator == 1)
                    RoomHash.Add(Walkers[i].TakeStep(Vector2I.One * RoomSize, seed));
            }
                

			roomEnds.Add(RoomHash.Last());
		}

		// create portals between each room
        for (int i = 0; i < RoomHash.Count; i++)
		{
            roomProtalOverlay.Add(new HashOverlay2D() { pos = RoomHash.ToArray()[i], data = 0 });

            if (RoomHash.ToArray()[i] == new Vector2I(RoomHash.ToArray()[i].X + RoomSize, RoomHash.ToArray()[i].Y))
			{
                portals.Add(new Vector2I(RoomHash.ToArray()[i].X + RoomSize / 2, RoomHash.ToArray()[i].Y));
				roomProtalOverlay.ToArray()[i].data++;
            }

			if(RoomHash.ToArray()[i] == new Vector2I(RoomHash.ToArray()[i].X - RoomSize, RoomHash.ToArray()[i].Y))
			{
                portals.Add(new Vector2I(RoomHash.ToArray()[i].X - RoomSize / 2, RoomHash.ToArray()[i].Y));
                roomProtalOverlay.ToArray()[i].data++;
            } 

            if (RoomHash.ToArray()[i] == new Vector2I(RoomHash.ToArray()[i].X, RoomHash.ToArray()[i].Y + RoomSize))
            {
                portals.Add(new Vector2I(RoomHash.ToArray()[i].X, RoomHash.ToArray()[i].Y + RoomSize / 2));
                roomProtalOverlay.ToArray()[i].data++;
            }

            if (RoomHash.ToArray()[i] == new Vector2I(RoomHash.ToArray()[i].X, RoomHash.ToArray()[i].Y - RoomSize))
			{
                portals.Add(new Vector2I(RoomHash.ToArray()[i].X, RoomHash.ToArray()[i].Y - RoomSize / 2));
                roomProtalOverlay.ToArray()[i].data++;
            }
        }

        // Generate the cell hash by creating walkers in each room and making them all move at the same time.
        // these walkers will try to go towards nearby doors and portals. (reason why we have portals in the first place.)
        int index = 0;
        for (int i = 0; i < RoomHash.Count; i++)
        {
			List<Walker> cellWalker = new List<Walker>();
            foreach (Vector2I re in roomEnds)
                cellWalker.Add(new Walker(re));

			foreach (HashOverlay2D ho2D in roomProtalOverlay)
				for (int j = 0; j < ho2D.data; j++)
					cellWalker.Add(new Walker(ho2D.pos));

            foreach (Walker walker in cellWalker)
			{
				while ((walker.pos - portals.ToArray()[index]).Length() > CellSize)
				{
					if(generator == 0)
						CellHash.Add(walker.TakeStepInfluenced(Vector2I.One * CellSize, portals.ToArray()[index], seed));
					else if(generator == 1)
                        CellHash.Add(walker.TakeStep(Vector2I.One * CellSize, seed));
                }

				index++;
            }
        }

		// return the CellHash data.
		return CellHash;
	}

	void GenerateRoom()
	{
		HashSet<Vector2I> CellSet = GenerateHashes(Data.Sets[DataSetID].Doors.ToList(),0);

        // create a 2d hash overlay for a base map. this hash overlay will hold data on:
        // - Grid Position
        // - Neibours (binary vector 2)
        // - ocupant index
        HashSet<AdvancedHashOverlay2D> SpecialDataSet = new HashSet<AdvancedHashOverlay2D>();

        // create a new cell set that will make the special tiles, like water, stairs, bridges, etc.
        // make the system be as customizable as possible to allow for anything at all. like lakes, structures, etc.
        HashSet<HashSet<Vector2I>> SpecialCellSet = new HashSet<HashSet<Vector2I>>();
        foreach (Special spc in Data.Sets[DataSetID].Specialgeneration)
            SpecialCellSet.Add(GenerateHashes(Data.Sets[DataSetID].SpecialDoors.ToList(), spc.generator));

        //i need separated generation for the room and cells.

        // finally, create a height set. this will decide a few things later on.
        //
        // combine the sets together.
        //
        // trace each cell and generate the data that corresponds, without any ocupants. instead, add all the
        // ocupants to an smaller array that can hold them for future creation.
        //
        // after the map has been generated, add ocupants.
        //
        // done. probably.|
        //
    }
}

public class HashOverlay2D
{
    public Vector2I pos;
    public int data;
}

public class HashOverlay3D
{
    public Vector3 pos;
    public int data;
}

public class HashOverlay2D_MegaData
{
    public Vector2I pos;
    public ByteVec2[] neibours = new ByteVec2[8];
    public Node3D prefab;
    public ByteVec2 direction;
    public byte height;
}

public class AdvancedHashOverlay2D
{
    public Vector2I pos;
    public ByteVec2[] neibours = new ByteVec2[8];
	public int ocupant;
}

public class ByteVec2
{
	byte x, y;
}

public class ByteVec3
{
    byte x, y, z;
}

public class ByteVec4
{
    byte x, y, z, w;
}

public class Walker
{
	public Vector2I pos;

	public Walker(Vector2I position) 
	{ 
		pos = position;
	}

	public Vector2I TakeStep(Vector2I distance, ulong seed)
	{
        RandomNumberGenerator rand = new RandomNumberGenerator();
		rand.Seed = seed;

        return new Vector2I
			(pos.X + rand.RandiRange(-1,1) * distance.X,
             pos.Y + rand.RandiRange(-1, 1) * distance.Y);
	}

	public Vector2I TakeStepInfluenced(Vector2I distance, Vector2I towards, ulong seed)
	{
        RandomNumberGenerator rand = new RandomNumberGenerator();
        rand.Seed = seed;

		towards.Clamp(new Vector2I(-1, -1), new Vector2I(1, 1));

		towards.X = pos.X + rand.RandiRange(pos.X, towards.X + rand.RandiRange(-1, 1)) * distance.X;
		towards.Y = pos.Y + rand.RandiRange(pos.Y, towards.Y + rand.RandiRange(-1, 1)) * distance.Y;

        towards.Clamp(new Vector2I(-1, -1), new Vector2I(1, 1));

		return towards;
    }
}

