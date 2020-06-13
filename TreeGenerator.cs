using Godot;
using System;

public class TreeGenerator : Spatial
{
    [Export]
    public int Count = 10;
    [Export]
    public float BoundsX = 30;
    [Export]
    public float BoundsZ = 30;

    public void AddTree()
    {
        Tree tree = new Tree();
        AddChild(tree);
        float translateX = (float) GD.RandRange(-BoundsX, BoundsX);
        float translateZ = (float) GD.RandRange(-BoundsZ, BoundsZ);
        tree.Translate(new Vector3(translateX, 0, translateZ));
    }

    public override void _Ready()
    {
        ulong newSeed = 0;
        GD.RandSeed((ulong) DateTime.Now.Ticks, out newSeed);
        for (int i = 0; i < Count; i++)
        {
            AddTree();
        }
    }
    
}
