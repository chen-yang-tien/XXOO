using Godot;
using System;
using System.Collections.Generic;

public class AI
{

	static public double HP_REWARD_WEIGHT=5; // hp or vs
	static public double X_O_HP_EXCHANGE_WEIGHT=1; //0.5 ~ 1.5 //x or o
	static public double DEATH_WEIGHT=1.2;

	static public double HP_AREA_RATIO=0.8; // hp or area

	static public double FUTURE_REWARD_RATIO=0.96;

	static public int SIMULATION_LIMIT=17;//32
	static public double MEMORY_WEIGHT=1.1;
	
	//!
	static public bool ENABLE_GOOD_MOVES=true;
	static public bool ENABLE_APCS=true;

	static public int SimLowerLim=25000;
	
	public static AI X, O;
	public int startingHp;
	public Result self;

	public AI(int _hp, Result _self){
		startingHp=_hp;
		self=_self;
	}
	
	static public double Evaluate(State state){
		double reward=HpEval(state)+HP_AREA_RATIO*AreaEval(state);
		reward=2*Math.Atan(reward)/Math.PI;
		return reward;
	}

    static double HP_REWARD_INV=1/(1+HP_REWARD_WEIGHT);
	//! k represents the conflict bonus points
	public static double HpEval(State state){
		double dXhp=X.startingHp-state.Xhp;
		double dOhp=O.startingHp-state.Ohp;
		double k=state.Conflict+HP_REWARD_WEIGHT*(X_O_HP_EXCHANGE_WEIGHT*dOhp-dXhp);
		k*=HP_REWARD_INV;
		return k;
	}
	
	public static double AreaEval(State state){
		
		double k=0;
		foreach(Pattern patt in state.XPatterns){
			k+=state.SkillMaskValue(patt,Result.X);
		}
		foreach(Pattern patt in state.OPatterns){
			k-=state.SkillMaskValue(patt,Result.O);
		}
		return k;
	}
}
