using Godot;
using System;

public partial class ShopScene : Node2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		GetNode<Label>("Label").Text = $"HP: {FullGameSystem.PlayerHealth}/{FullGameSystem.PlayerMaxHealth}";
	}

	private void _on_finish_button_pressed() {
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
			GetTree().ChangeSceneToFile("res://combat/Main.tscn");
		}
		else if (FullGameSystem.Map[FullGameSystem.NextNode-1] == 7) {
			GetTree().ChangeSceneToFile("res://win.tscn");
		}
	}
}
