using Godot;
using System;

public partial class Title : Node2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void _on_start_button_pressed() {
		// GD.Print("hello, world");
		Random rand = new Random();
		int randint = rand.Next(6);
		if (randint == 0) {
			FullGameSystem.EnemyFightingList[0] = 0;
			FullGameSystem.EnemyFightingList[1] = 1;
			FullGameSystem.EnemyFightingList[2] = 2;
		}
		else if (randint == 1) {
			FullGameSystem.EnemyFightingList[0] = 0;
			FullGameSystem.EnemyFightingList[1] = 2;
			FullGameSystem.EnemyFightingList[2] = 1;
		}
		else if (randint == 2) {
			FullGameSystem.EnemyFightingList[0] = 1;
			FullGameSystem.EnemyFightingList[1] = 0;
			FullGameSystem.EnemyFightingList[2] = 2;
		}
		else if (randint == 3) {
			FullGameSystem.EnemyFightingList[0] = 1;
			FullGameSystem.EnemyFightingList[1] = 2;
			FullGameSystem.EnemyFightingList[2] = 0;
		}
		else if (randint == 4) {
			FullGameSystem.EnemyFightingList[0] = 2;
			FullGameSystem.EnemyFightingList[1] = 0;
			FullGameSystem.EnemyFightingList[2] = 1;
		}
		else {
			FullGameSystem.EnemyFightingList[0] = 2;
			FullGameSystem.EnemyFightingList[1] = 1;
			FullGameSystem.EnemyFightingList[2] = 0;
		}
		GetTree().ChangeSceneToFile("res://Map/Map.tscn");
	}

	private void _on_exit_button_pressed() {
		// GD.Print("hello, world2");
		GetTree().Quit();
	}
}
