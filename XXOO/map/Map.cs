using Godot;
using System;

public partial class Map : Node2D
{
	public int[] IsOccupied = {0, 0, 0, 0, 0};
	public int NowMouse = 0;
	public bool IsFinished = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GetNode<Button>("FinishButton").Hide();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		/* GD.Print(
			$"{this.IsOccupied[0]} {this.IsOccupied[1]} {this.IsOccupied[2]} {this.IsOccupied[3]} {this.IsOccupied[4]}"
		); */
		FullGameSystem.Map[0] = IsOccupied[0] < 5 ? IsOccupied[0] : 4;
		FullGameSystem.Map[2] = IsOccupied[1] < 5 ? IsOccupied[1] : 4;
		FullGameSystem.Map[4] = IsOccupied[2] < 5 ? IsOccupied[2] : 4;
		FullGameSystem.Map[6] = IsOccupied[3] < 5 ? IsOccupied[3] : 4;
		FullGameSystem.Map[7] = IsOccupied[4] < 5 ? IsOccupied[4] : 4;
		/* GD.Print(
			$"{MapSystem.Map[0]} {MapSystem.Map[2]} {MapSystem.Map[4]} {MapSystem.Map[6]} {MapSystem.Map[7]}"
		); */
		IsFinished = true;
		for (int i = 0; i < 5; i++) {
			if (IsOccupied[i] == 0) {
				IsFinished = false;
			}
		}
		if (IsFinished) {
			GetNode<Button>("FinishButton").Show();
		}
		else {
			GetNode<Button>("FinishButton").Hide();
		}
	}

	private void _on_finish_button_pressed() {
		if(IsFinished) {
			FullGameSystem.NextNode += 1;
			if (FullGameSystem.Map[FullGameSystem.NextNode-1] == 1) {
				GetTree().ChangeSceneToFile("res://Treasure.tscn");
			}
			else if (FullGameSystem.Map[FullGameSystem.NextNode-1] == 2) {
				GetTree().ChangeSceneToFile("res://Heal.tscn");
			}
			else if (FullGameSystem.Map[FullGameSystem.NextNode-1] == 3) {
				GetTree().ChangeSceneToFile("res://Shop.tscn");
			}
			else if (FullGameSystem.Map[FullGameSystem.NextNode-1] == 4) {
				GetTree().ChangeSceneToFile("res://Event.tscn");
			}
			else if (FullGameSystem.Map[FullGameSystem.NextNode-1] == 5) {
				GetTree().ChangeSceneToFile("res://combat/Main.tscn");
			}
			else if (FullGameSystem.Map[FullGameSystem.NextNode-1] == 6) {
				
			}
		}
	}
}
