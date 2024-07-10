using Godot;
using System;

public partial class TreasureScene : Node2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GetNode<Button>("FinishButton").Hide();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		GetNode<Label>("Hp").Text = $"HP: {FullGameSystem.PlayerHealth}/{FullGameSystem.PlayerMaxHealth}";
	}

	private void _on_button_serum_pressed() {
		FullGameSystem.PlayerMaxHealth = 1;
		FullGameSystem.PlayerHealth = Math.Min(FullGameSystem.PlayerHealth, FullGameSystem.PlayerMaxHealth);
		FullGameSystem.PlayerSkillSets[1].Point += 1;
		FullGameSystem.serum = true;
		GetNode<Label>("Prompt").Text = "You chose to inject the serum, it was uncomfortable,\nbut it's good for you, right?";
		GetNode<Button>("ButtonSerum").Hide();
		GetNode<Button>("ButtonPills").Hide();
		GetNode<Button>("ButtonNo").Hide();
		GetNode<Button>("FinishButton").Show();
	}

	private void _on_button_pills_pressed() {
		FullGameSystem.PlayerMaxHealth += 2;
		FullGameSystem.PlayerHealth += 2;
		GetNode<Label>("Prompt").Text = "You chose to take the pills, they taste like nothing,\nbut thats how pills work.";
		GetNode<Button>("ButtonSerum").Hide();
		GetNode<Button>("ButtonPills").Hide();
		GetNode<Button>("ButtonNo").Hide();
		GetNode<Button>("FinishButton").Show();
	}

	private void _on_button_no_pressed() {
		GetNode<Label>("Prompt").Text = "You chose to not use any of them, maybe that was a loss, but who knows?";
		GetNode<Button>("ButtonSerum").Hide();
		GetNode<Button>("ButtonPills").Hide();
		GetNode<Button>("ButtonNo").Hide();
		GetNode<Button>("FinishButton").Show();
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
