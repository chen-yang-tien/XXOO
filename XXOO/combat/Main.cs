using Godot;
using System;
using System.Collections.Generic;

public partial class Main : Node2D
{
	public const string LOAD="res://combat/";
	
	public readonly PackedScene Block = ResourceLoader.Load<PackedScene>(LOAD+"Block.tscn");
	
	//public State state;
	
	static public List<Block> blocks=new List<Block>();

	bool IsRunning=false;
	bool IsAutoX=false;
	bool IsAutoO=true;
	
	public override void _Ready()
	{
		int enemyId=
		FullGameSystem.EnemyFightingList[FullGameSystem.NextEnemy];
		FullGameSystem.NextEnemy += 1;

		List<Pattern> xSet=FullGameSystem.PlayerSkillSets;

		if (FullGameSystem.serum) {
			GetNode<Sprite2D>("PlayerNoSerum").Texture = GD.Load<Texture2D>("res://player_yes_serum.png");
		}
		
		List<Pattern> oSet=FullGameSystem.EnemySkillSets[enemyId];

		GetNode<Sprite2D>("EnemySkill").Texture = GD.Load<Texture2D>(FullGameSystem.EnemySkillsLocations[enemyId]);
		
		//ai x load in skill
		AI.X=new AI(FullGameSystem.PlayerHealth, Result.X);

		//ai O load in skill
		AI.O=new AI(FullGameSystem.EnemyHealth[enemyId], Result.O);

		//create base state
		
		//end of loading in combat;

		//prethink about an empty board
		GD.Print(MctsNode.InitializeMCTS(new State(0, 0, Result.X, xSet, oSet), 10000));
		
		//setup display
		GetNode<Label>("Conflict").Text = $"Conflict: {MctsNode._state.Conflict}";
		GetNode<Label>("Cross").Text = $"Cross: {MctsNode._state.Xhp}";
		GetNode<Label>("Circle").Text = $"Circle: {MctsNode._state.Ohp}";

		//!
		GetNode<Sprite2D>("EnemySprite").Texture = FullGameSystem.LoadEnemy(enemyId);
		GetNode<Label>("EnemyName").Text = FullGameSystem.EnemyNames[enemyId];


		for (int i = 0; i < 8; ++i)
		{
			for (int j = 0; j < 8; ++j) 
			{
				Block TheBlockInstance = Block.Instantiate<Block>();
				TheBlockInstance.posx = j;
				TheBlockInstance.posy = i;
				TheBlockInstance.Position = new Vector2(550 + j * 100, 150 + i * 100);
				AddChild(TheBlockInstance);
				TheBlockInstance.IsClicked += IsClicked;
				blocks.Add(TheBlockInstance);
			}
		}
		

		RequestReady();
	}

	double simTime=0;
	int slowdown=0;
	bool IsAutoRunning=false;
	
	public override void _Process(double delta)
	{
		if (
			(MctsNode._state.symbol==Result.X&&IsAutoX)|| 
			(MctsNode._state.symbol==Result.O&&IsAutoO)
			)
		{
			if(!IsAutoRunning){
				IsAutoRunning=true;
				simTime=0;
				GD.Print("slowdown: "+slowdown);
				slowdown=0;
			}
			
			MctsNode.MCTS(1000, 40000, false);
			simTime+=delta;

			if(simTime>0.5){
				int shouldMovePos;
				if(MctsNode.totalSimCount>AI.SimLowerLim-1000){
					shouldMovePos=MctsNode.MCTS(1000, 40000, true);
				}
				else{
					shouldMovePos=MctsNode.MCTS(AI.SimLowerLim-MctsNode.totalSimCount, 40000, true);
				}
				
				MctsNode.ActualPlace(shouldMovePos);
				

				Display();
				IsAutoRunning=false;
			}
			
		}
		
		if (MctsNode._state.END)
		{
			if (MctsNode._state.symbol==Result.X){
				FullGameSystem.PlayerHealth = MctsNode._state.Xhp + (FullGameSystem.PlayerMaxHealth + 2 - MctsNode._state.Xhp) / 3;
				// RequestReady();
				foreach (Block b in blocks) {
					b.QueueFree();
				}
				blocks = new List<Block>();
				GetTree().ChangeSceneToFile("res://Winning.tscn");
				GetNode<Label>("Conflict").Text = "X WON";
			}
			if (MctsNode._state.symbol==Result.O){
				GetTree().ChangeSceneToFile("res://Losing.tscn");
				GetNode<Label>("Conflict").Text = "O WON";
			}
			if (MctsNode._state.symbol==Result.DRAW){GetNode<Label>("Conflict").Text = "DRAW";}
		}
		
		if(slowdown<2000){
			MctsNode.MCTS(500-slowdown/4, 20000-5*slowdown, false);
			slowdown+=1;
		}
		
		
	}
	
	public void IsClicked(int x, int y, Block b)
	{
		if (IsRunning) {GD.Print("blocked");return;}
		IsRunning=true;
		
		if (MctsNode._state.symbol==Result.X&&IsAutoX){IsRunning=false;return;}
		if (MctsNode._state.symbol==Result.O&&IsAutoO){IsRunning=false;return;}
		if (MctsNode._state.END) {IsRunning=false;return;}
		
		if (!MctsNode.ActualPlace(x+y*U.boardSizeX)){IsRunning=false;return;}
		
		Display();
		
		
		IsRunning=false;
	}

	public void Display(){
		GetNode<Label>("Conflict").Text = $"Conflict: {MctsNode._state.Conflict}";
		GetNode<Label>("Cross").Text = $"Cross: {MctsNode._state.Xhp}";
		GetNode<Label>("Circle").Text = $"Circle: {MctsNode._state.Ohp}";

		GD.Print($"AI {AI.AreaEval(MctsNode._state)}");
		//MctsNode._state.Show();
	}
}
