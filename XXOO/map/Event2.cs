using Godot;
using System;
using System.Collections.Generic;

public partial class Event2 : Area2D
{
	private Vector2 MousePosition;
	private Vector2 InitialPosition;
	private Vector2 StopPosition;
	private Vector2[] EmptyPositions = new Vector2[5];
	private bool IsBeingClicked = false;
	private Map Parent;
	private bool IsInEmpty = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		this.InitialPosition = this.Position;
		this.StopPosition = this.InitialPosition;
		this.EmptyPositions[0] = GetNode<Sprite2D>("../Empty1").Position;
		this.EmptyPositions[1] = GetNode<Sprite2D>("../Empty2").Position;
		this.EmptyPositions[2] = GetNode<Sprite2D>("../Empty3").Position;
		this.EmptyPositions[3] = GetNode<Sprite2D>("../Empty4").Position;
		this.EmptyPositions[4] = GetNode<Sprite2D>("../Empty5").Position;
		this.Parent = GetNode<Map>("..");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		this.MousePosition = GetViewport().GetMousePosition();
		if (this.IsBeingClicked && Input.IsActionPressed("left mouse")) {
			this.Position = this.MousePosition;
			this.ZIndex = 1;
		}
		else {
			this.IsBeingClicked = false;
			this.Position -= (this.Position - this.StopPosition) * ((float) (delta * 20.0));
			if (this.Position.DistanceTo(this.StopPosition) <= 0.01) {
				this.Position = this.StopPosition;
			}
			this.ZIndex = 0;
		}
		this.IsInEmpty = false;
		for (int i = 0; i < 5; i++) {
			if (
				this.Position.DistanceTo(EmptyPositions[i]) <= 90 &&
				(this.Parent.IsOccupied[i] == 0 || this.Parent.IsOccupied[i] == 5) &&
				(Input.IsActionPressed("left mouse") || this.Parent.IsOccupied[i] == 5)
			) {
				GetNode<Sprite2D>("Sprite2D").SelfModulate = new Color((float) 0.0, (float) 1.0, (float) 0.0, (float) 1.0);
				this.StopPosition = this.EmptyPositions[i];
				this.Parent.IsOccupied[i] = 5;
				this.IsInEmpty = true;
			}
		}
		if (!IsInEmpty) {
			GetNode<Sprite2D>("Sprite2D").SelfModulate = new Color((float) 1.0, (float) 1.0, (float) 1.0, (float) 1.0);
			this.StopPosition = this.InitialPosition;
			for (int i = 0; i < 5; i++) {
				if (this.Parent.IsOccupied[i] == 5) {
					this.Parent.IsOccupied[i] = 0;
				}
			}
		}
	}

	private void _on_input_event(Node viewport, InputEvent @event, long shape_idx)
	{
		if (Input.IsActionJustPressed("left mouse"))
		{
			this.IsBeingClicked = true;
		}
	}
}
