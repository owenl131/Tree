using Godot;
using System;

public class CameraContainer : Spatial
{
    public override void _Ready()
    {
        
    }

    public override void _PhysicsProcess(float delta)
    {
        if (Input.IsActionPressed("ui_page_down"))
        {
            GetNode<Camera>("Camera").Translate(2 * Vector3.Back * delta);
        }
        if (Input.IsActionPressed("ui_page_up"))
        {
            GetNode<Camera>("Camera").Translate(2 * Vector3.Forward * delta);
        }
        if (Input.IsActionPressed("ui_left"))
        {
            RotateY(-2f * delta);
        }
        if (Input.IsActionPressed("ui_right"))
        {
            RotateY(2f * delta);
        }
        if (Input.IsActionPressed("ui_up"))
        {
            Translate(2 * Vector3.Up * delta);
        }
        if (Input.IsActionPressed("ui_down"))
        {
            Translate(2 * Vector3.Down * delta);
        }
        GetNode<Camera>("Camera").LookAt(Vector3.Up * 1, Vector3.Up);
    }
}
