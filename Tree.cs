using Godot;
using System;
using System.Collections.Generic;

public class Tree : Spatial
{
    [Export]
    public Material material = null;

    public static Material mat;

    public float CrossSectionalRadius;
    public float MinLength;
    public float MaxLength;
    public int DepthRemaining;
    public List<Tree> Children;

    public float MainMaxOffsetDegrees = 20;
    public float SubMaxOffsetDegrees = 40;

    public float SwayFrequency = 2.0f;
    public float SwayAmplitude = 0.05f;
    public float SwayPhase;
    private float TimeElapsed = 0;

    public static SpatialMaterial BranchMaterial = new SpatialMaterial()
    {
        AlbedoColor = new Color(0.54f, 0.38f, 0.34f)
    };

    public static SpatialMaterial LeafMaterial = new SpatialMaterial()
    {
        AlbedoColor = new Color(0.2f, 0.6f, 0.04f)
    };

    public static ShaderMaterial LeafMaterial2 = ResourceLoader.Load("Leaf.tres") as ShaderMaterial;

    public Tree()
    {
        CrossSectionalRadius = 0.1f * 2;
        MinLength = 1.5f * 2;
        MaxLength = 2.0f * 2;
        DepthRemaining = 6;
        Children = new List<Tree>();
        SwayPhase = (float) GD.RandRange(-Mathf.Pi, Mathf.Pi);
    }

    public Tree(int depthRemaining, float radius, 
                float minLength, float maxLength)
    {
        CrossSectionalRadius = radius;
        DepthRemaining = depthRemaining;
        MinLength = minLength;
        MaxLength = maxLength;
        Children = new List<Tree>();
        SwayPhase = (float) GD.RandRange(-Mathf.Pi, Mathf.Pi);
    }

    public void AddBranch(int reduceDepth, float length, float downScale, float slantLowerbound, float slantUpperbound)
    {
        Tree branch = new Tree(
            DepthRemaining - reduceDepth,
            CrossSectionalRadius * downScale,
            MinLength * downScale,
            MaxLength * downScale);
        AddChild(branch);
        Children.Add(branch);
        branch.Translate(Transform.basis.y * length);
        branch.Rotate(Transform.basis.y, (float) GD.RandRange(-Mathf.Pi, Mathf.Pi));
        branch.Rotate(branch.Transform.basis.x, (float) GD.RandRange(slantLowerbound, slantUpperbound));
    }

    public void CreateTree()
    {
        if (DepthRemaining <= 0)
        {
            CreateLeaf();
            return;
        }
        Rotate(Transform.basis.y, (float) GD.RandRange(0, 2 * Mathf.Pi));

        CylinderMesh mesh = new CylinderMesh();
        mesh.TopRadius = CrossSectionalRadius;
        mesh.BottomRadius = CrossSectionalRadius;
        mesh.RadialSegments = 8;
        mesh.Rings = 1;
        float length = (float) GD.RandRange(MinLength, MaxLength);
        mesh.Height = length;
        MeshInstance meshInstance = new MeshInstance();
        meshInstance.Mesh = mesh;
        meshInstance.Translate(Transform.basis.y * length / 2);
        meshInstance.SetSurfaceMaterial(0, BranchMaterial);
        AddChild(meshInstance);

        float subOffsetDegrees = Mathf.Deg2Rad(SubMaxOffsetDegrees);
        
        // Main Branch
        AddBranch(1, length, 0.9f, 0, Mathf.Deg2Rad(MainMaxOffsetDegrees));
        AddBranch(1, length, 0.7f, subOffsetDegrees / 2, subOffsetDegrees);
        /*if (DepthRemaining > 1) 
        {
            
            // Sub Branches
            if (GD.RandRange(0, 1) > 0.5)
            {
                AddBranch(1, length, 0.8f, subOffsetDegrees / 2, subOffsetDegrees);
                AddBranch(1, length, 0.8f, subOffsetDegrees / 2, subOffsetDegrees);
            }
            else 
            {
                AddBranch(1, length, 0.8f, subOffsetDegrees / 2, subOffsetDegrees);
                AddBranch(1, length, 0.8f, subOffsetDegrees / 2, subOffsetDegrees);
                // AddBranch(1, length, 0.7f, subOffsetDegrees / 2, subOffsetDegrees);
                //AddBranch(2, length, 0.7f, subOffsetDegrees / 2, subOffsetDegrees);
            }
        }*/
    }

    public void CreateLeaf()
    {
        SphereMesh mesh = new SphereMesh();
        // mesh.Radius = 0.4f * 5;
        mesh.Radius = 1;
        mesh.Height = mesh.Radius * 2;
        mesh.RadialSegments = 64;
        mesh.Rings = 32;
        MeshInstance meshInstance = new MeshInstance();
        meshInstance.Mesh = mesh;
        meshInstance.SetSurfaceMaterial(0, LeafMaterial2);
        AddChild(meshInstance);
        meshInstance.RotateX((float) GD.RandRange(-Mathf.Pi, Mathf.Pi));
        meshInstance.RotateY((float) GD.RandRange(-Mathf.Pi, Mathf.Pi));
        meshInstance.RotateZ((float) GD.RandRange(-Mathf.Pi, Mathf.Pi));
    }


    public override void _Ready()
    {
        if (material != null)
            mat = material;
        CreateTree();
    }

    public override void _PhysicsProcess(float delta)
    {
        TimeElapsed += delta;
        // Add swaying in the wind
        Rotate(Vector3.Right, 
            SwayAmplitude * Mathf.Cos(SwayFrequency * TimeElapsed + SwayPhase) * delta / (100 * CrossSectionalRadius * CrossSectionalRadius));
    }
    
}
