using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics; 

public partial class MctsNode
{
	private double V=0;
	private int N=0;
	private double V_avg=0;
	private readonly MctsNode parent;
	private readonly int move;
	
	private int nextMove;
	private int moveCount;
	private ulong moves;

	private int nextGoodMove;
	private ulong goodMoves=0;
	//private ulong badMoves;
	private List<MctsNode> children;
	private Result side;

	static MctsNode root;
	 
	public static State _state;
	static State state;
	public static int totalSimCount=0;
	

	public MctsNode(State _state) {
		V = 0;
		N = 0;
		parent = null;
		move = State.NullMove(); //default to an illegal move
		side = _state.symbol;
		
		//!
		
		moves = _state.PossibleMoves();//get_moves();
		moveCount=U.BinCount(moves);

		goodMoves=_state.GoodMoves(AI.ENABLE_GOOD_MOVES);
		moves=moves&(~goodMoves);

		nextMove=U.rand.Next(0, 64);
		nextGoodMove=nextMove;
		
		moves=U.Slice(moves,nextMove);
		goodMoves=U.Slice(goodMoves,nextGoodMove);

		children=new List<MctsNode>();

	}

	public MctsNode(MctsNode _parent, int _move) {
		V = 0;
		N = 0;
		parent = _parent;
		move = _move;
		side = state.symbol;
		
		//!
		moves = state.PossibleMoves();//get_moves();
		moveCount=U.BinCount(moves);
		goodMoves=state.GoodMoves(AI.ENABLE_GOOD_MOVES);
		moves=moves&(~goodMoves);
		
		
		nextMove=U.rand.Next(0, 64);
		nextGoodMove=nextMove;
		moves=U.Slice(moves,nextMove);
		goodMoves=U.Slice(goodMoves,nextGoodMove);

		children=new List<MctsNode>();

	}

	private MctsNode Select() {
		MctsNode node = root;
		double uct;

		MctsNode best_child = null;
		double best_value;
		int is_negative;
		
		while(node.children.Count == node.moveCount) {
			if(node.moveCount==0){break;}
			if(node.moveCount==1){best_child=node.children[0];}
			else{
				
				is_negative=1;
				if(node.side==Result.O){
					is_negative=-1;
				}
				
				best_value = -1000.0;

				foreach(MctsNode child in node.children){

					uct = is_negative*child.V_avg + Math.Sqrt(2.0 * Math.Log(node.N) / child.N);

					if(uct > best_value) {
						best_value = uct;
						best_child = child;
					}
				}
				
				
			}

			if(best_child==null){
				break;
			}
			
			node = best_child;
			state.Place(node.move);//do move
		}
		return node;
	}

	private MctsNode Expansion() {
		MctsNode node;
		int _move=State.NullMove();

		while(goodMoves>0){
			if(nextGoodMove>=64){nextGoodMove=0;}
			if ((goodMoves&1)==1){
				_move=nextGoodMove;
				nextGoodMove++;
				goodMoves>>=1;
				break;
			}
			nextGoodMove++;
			goodMoves>>=1;
		}

		if(_move==State.NullMove()){
		while(moves>0){
			if(nextMove>=64){nextMove=0;}
			if ((moves&1)==1){
				_move=nextMove;
				nextMove++;
				moves>>=1;
				break;
			}
			nextMove++;
			moves>>=1;
		}
		}
		
		if(_move==State.NullMove()){GD.Print("goodmoves"+goodMoves);GD.Print("moves"+moves);}

		if(!state.Place(_move)){GD.Print("wtfff: "+_move);}//do move
		node = new MctsNode(this, _move);
		children.Add(node);
		return node;
	}

	
	private static double Simulate() {
		//Result player = U.Last(state.symbol);

		//AI ai;
		//if thisPlayer == Result.O
		//if(player==Result.X){ai=AI.X;}
		//else{ai=AI.O;}

		double reward=AI.Evaluate(state);
		double win;


		int movesLeft=0;
		while(!state.END) 
		{
			state.QuickRandMove(AI.ENABLE_APCS);
			movesLeft+=1;

			if(movesLeft==AI.SIMULATION_LIMIT/2){
				reward+=AI.Evaluate(state);
				reward*=0.5;
			}
			
			if(movesLeft>AI.SIMULATION_LIMIT){
				break;
			}
			
		}

		if (state.END) {
			if (state.symbol == Result.X) {win=AI.DEATH_WEIGHT;}
			else {win=-AI.DEATH_WEIGHT;}
		}
		else{
			win=AI.Evaluate(state);
		}
		double yeet=Math.Pow(AI.FUTURE_REWARD_RATIO,movesLeft);

		return reward*(1-yeet)+win*yeet;
		
	}


	void Update(double value) {
		MctsNode node = this;

		while(node is not null) {
			node.V += value;
			node.N += 1;
			node.V_avg=node.V/node.N;
			node=node.parent;
		}
	}

	void OneRound() {

		MctsNode node = Select();

		if (!state.END) {
			node = node.Expansion();
		}

		double result = 0.0;
		if (!state.IsDraw()) {
			result = Simulate();
		}

		node.Update(result);

	}

	
	int Round(int simCount, long timeLimit, bool show) {
		
		int initSimCount=simCount;
		int simLimit=simCount;
		long startTime = Stopwatch.GetTimestamp();//exp+=c-b;
		long currentTime;

		while (simLimit > 0){
			state=new State(_state);
			simLimit -= 1;
			OneRound();
			
			currentTime = Stopwatch.GetTimestamp();
			if (simLimit==0&&(currentTime-startTime)<timeLimit){
				simLimit+=initSimCount/5;
				simCount+=initSimCount/5;
			}
		}

		totalSimCount+=simCount;
		

		if (show){
			int best_move = State.NullMove(); 
			int best_n = -1000;
			double best_Vavg=0;
			int num;
			int numTotal = 0;
			foreach (MctsNode child in children) {
				num = child.N;
				numTotal+=num;

				//!
				GD.Print($"move {child.move} num {num} avg_val {child.V_avg}");
				if (num > best_n) {
					best_n = num;
					best_Vavg = child.V_avg;
					best_move = child.move;
				}
			}
			//!
			GD.Print("\nroot: "+root.side+" best_n: "+best_n+" best_v: "+best_Vavg+" num_total: "+numTotal+"\n");
			return best_move;
		}
		return 0;
	}

	public static int MCTS(int simLimit, long timeLimit, bool show) {
		


		int move = root.Round(simLimit, timeLimit, show);

		if (show){
			GD.Print($"AI {AI.AreaEval(_state)}");
			return move;
		}
		
		return 0;
		
	}

	public static int InitializeMCTS(State __state, int simLimit){

		_state=__state;
		root = new MctsNode(_state);
		
		return MCTS(simLimit, 0, true);
	}

	public static bool ActualPlace(int move){

		if(!_state.Place(move, true)){return false;}
		MemoryTree(move);
		return true;
	}
	public static void MemoryTree(int move){

		totalSimCount=0;
		foreach (MctsNode child in root.children)
		{
			if (child.move==move){
				root=child;
				root.WipeMemory();
				//MCTS(_state, 64, 0, false);
				return;
			}
		}
		GD.PrintErr("MemoryTreeBroke");
	}

	public void WipeMemory(){

		foreach (MctsNode child in children){child.WipeMemory();}
		V=2*AI.MEMORY_WEIGHT*Math.Atan(V/N)/Math.PI;
		N=1;
	}
}
