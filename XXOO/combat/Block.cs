using Godot;
using System;

public partial class Block : Area2D
{
	[Signal]
	public delegate void IsClickedEventHandler(int x, int y, Block b);
	
	public int posx, posy;
	
	public override void _Ready()
	{
		
	}

	public override void _Process(double delta)
	{
		
	}
	
	public void Cross()
	{
		GetNode<Sprite2D>("Sprite2D").Texture = GD.Load<Texture2D>(Main.LOAD+"Cross.png");
	}
	
	public void Circle()
	{
		GetNode<Sprite2D>("Sprite2D").Texture = GD.Load<Texture2D>(Main.LOAD+"Circle.png");
	}

	public void Nope()
	{
		GetNode<Sprite2D>("Sprite2D").Texture = GD.Load<Texture2D>(Main.LOAD+"Nope.png");
	}
	
	private void _on_input_event(Node viewport, InputEvent @event, long shape_idx)
	{
		if (Input.IsActionJustPressed("left mouse"))
		{
			EmitSignal(SignalName.IsClicked, posx, posy, this);
		}
	}
}
