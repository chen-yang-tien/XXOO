using Godot;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;


public class State {
	
	//8x8 board
	public ulong Xboard;
	public ulong Oboard;
	ulong ResetBranchCheck;
	public Result symbol;

	public int Count;
	public bool END;
	public int Conflict;
	public int Xhp;
	
	public int Ohp;

	public readonly List<Pattern> XPatterns;
	public readonly List<Pattern> OPatterns;

	public State(State s){
		Xboard=s.Xboard;
		Oboard=s.Oboard;
		symbol=s.symbol;
		Count=s.Count;
		END=s.END;
		Conflict=s.Conflict;
		Xhp=s.Xhp;
		Ohp=s.Ohp;
		XPatterns = s.XPatterns;
		OPatterns = s.OPatterns;

		ResetBranchCheck=s.ResetBranchCheck;
	}
	
	public State(ulong _Xboard, ulong _Oboard, Result _result, List<Pattern> X, List<Pattern> O){
		Xboard=_Xboard;
		Oboard=_Oboard;
		symbol=_result;
		Count=U.boardSizeX*U.boardSizeY;
		END=false;
		Conflict=0;

		XPatterns = X;
		OPatterns = O;

		Xhp=AI.X.startingHp;
		Ohp=AI.O.startingHp;
		
		ResetBranchCheck=1;
	}

	//make it so that both 1 var and 2 var is alr,
	public bool Place(int pos, bool Display=false){
		if (pos<0||pos>U.boardSizeX*U.boardSizeY)
		{GD.Print($"U FUCKED UP (In Place - State");return false;}
		int x=pos%U.boardSizeX;
		int y=pos/U.boardSizeX;
		return GameRound(x, y, Display);
	}
	public bool Place(int x, int y, bool Display=false){
		return GameRound(x, y, Display);
	}

	//the actual function, with dmg calc and status effects
	private bool GameRound(int x,int y,bool Display){
		
		if(!PutOnBoard(x, y, Display)){return false;}
		//!Show();
		//BinaryShow(MaskPriority(Result.O));
		
		if (symbol==Result.X)
		{
			foreach (Pattern p in XPatterns) {

				if (Check(x, y, symbol, p)) {
					Conflict += p.Point;
				}
			}
			//! update here
			//if (ChainCheck(x, y, symbol, 5)){Conflict += 2;}
			
			// if anyone is watching this, I'm really having a stroke if I see more than one semicolon
			// in one line except for loops.
			if (Conflict < 0) {
				Xhp -= 1;
				Conflict += 1;
			}
			
		}
		else if (symbol==Result.O)
		{
			foreach (Pattern p in OPatterns) {
				if (Check(x, y, symbol, p)) {
					Conflict -= p.Point;
				}
			}
			if (Conflict > 0) {Ohp -= 1;Conflict -= 1;}
		}

		//! update here
		if(ClearBoard()){
			Xboard=0;Oboard=0;Count=64;
			if (Display){
				for (int i = 0; i < 8; ++i){
				for (int j = 0; j < 8; ++j){
					Main.blocks[i+j*U.boardSizeX].Nope();
				}
				}
			}
		}

		if (Xhp <= 0 || Ohp <= 0){END=true;}

		if (symbol==Result.X){symbol=Result.O;}
		else if (symbol==Result.O){symbol=Result.X;}

		return true;
	}
	//only the placing symbol section
	private bool PutOnBoard(int x, int y,bool Display=false){
		ulong locate=(ulong)1<<(x+y*U.boardSizeX);
		//im dum and so r u
		if((Xboard&Oboard)>0){GD.Print($"U FUCKED UP (In PutOnBoard - State");return false;}
		if((Xboard&locate)>0){GD.Print($"X symbol already here at ({x},{y})");return false;}
		if((Oboard&locate)>0){GD.Print($"O symbol already here at ({x},{y})");return false;}
		//if actuall placed successfully
		Count--;
		if(symbol==Result.X){Xboard|=locate;if (Display){Main.blocks[x+y*U.boardSizeX].Cross();}return true;}
		if(symbol==Result.O){Oboard|=locate;if (Display){Main.blocks[x+y*U.boardSizeX].Circle();}return true;}
		//if draw
		
		GD.PrintErr($"symbol {symbol} is unknown - (PutOnBoard - State");
		Count++;
		return false;
	}

	public void Show(){
		//GD.Print($"symbol {symbol} end {END} count {Count}");
		//GD.Print($"con {Conflict} XHp {Xhp} Ohp {Ohp}");
		GD.Print("<X Board>");
		ulong tmp=Xboard;int follow=0;int gogo=0;string output="";
		while(gogo<U.boardSizeX*U.boardSizeY){
			if((tmp&1)==1){output+="X";}
			else{output+="=";}
			follow++;
			if(follow==U.boardSizeX){
				GD.Print(output);
				follow=0;
				output="";
			}
			tmp=tmp>>1;
			gogo++;
		}
		GD.Print("==================");
		GD.Print("<O Board>");
		tmp=Oboard;follow=0;gogo=0;output="";
		while(gogo<U.boardSizeX*U.boardSizeY){
			if((tmp&1)==1){output+="O";}
			else{output+="=";}
			follow++;
			if(follow==U.boardSizeX){
				GD.Print(output);
				follow=0;
				output="";
			}
			tmp=tmp>>1;
			gogo++;
		}
		GD.Print("==================");
	}

	public bool ClearBoard(){
		if(Count>42){return false;}
		ulong board=Xboard|Oboard;
		if((board&U.Corners)!=U.Corners){return false;}
		ulong grow=ResetBranchCheck;
		do{
			ResetBranchCheck=grow;
			grow=U.Spread(ResetBranchCheck)&board;
			if((grow&U.Corners)==U.Corners){return true;}
		}
		while(grow!=ResetBranchCheck);
		return false;
	}

	public bool ChainCheck(int x, int y, Result symbol, int goal){
		ulong board=0;
		if(symbol==Result.X){board=Xboard;}
		if(symbol==Result.O){board=Oboard;}
		ulong locate;
		ulong grow=(ulong)1<<(x+y*U.boardSizeX);
		int step=0;
		do{
			locate=grow;
			grow=U.Spread(locate)&board;
			step+=1;
			if (step>=goal){return true;}
		}
		while(grow!=locate);
		return false;
	}

	public bool Check(int x, int y, Result symbol, Pattern patt){
		ulong locate;
		ulong moveIt=patt.ThePattern;

		int back=0;
		while(moveIt>0){
			if(((moveIt&1)==1)&&(x+patt.Xsize<U.boardSizeX)&&(y+patt.Ysize<U.boardSizeY)){
				locate=patt.ThePattern<<(x+y*U.boardSizeX);
				if(symbol==Result.X&&((locate&Xboard)==locate)){return true;}
				if(symbol==Result.O&&((locate&Oboard)==locate)){return true;}
			}
			moveIt>>=1;
			back+=1;
			x-=1;
			if(x==-1){x=U.boardSizeX-1;y-=1;}
			if(y==-1){return false;}
		}
		return false;
	}

	public bool IsDraw() {
		return symbol == Result.DRAW && END;
	}
	public static int NullMove() {
		return -1;
	}
	public static ulong FullBoard(){
		return ~(ulong)0;
	}
	public ulong PossibleMoves(){
		return ~(Xboard|Oboard);
	}
	
	public void QuickRandMove(bool apcs){
		ulong board=GoodMoves(apcs);
		if(board==0){board=PossibleMoves();}
		
		
		int pos= U.rand.Next(0, 64);
		board = U.Slice(board, pos);
		

		while(board>0){
			if(pos>=64){pos-=64;}
			
			if ((board&15)==0){
				pos+=4;
				board>>=4;
				continue;
			}
			if ((board&1)==1){
				Place(pos);return;
			}
			pos++;
			board>>=1;
		}
		GD.Print("U fucked up in State-QuickRandMove");
		Place(NullMove());
		return;
	}

	public ulong GoodMoves(bool ENABLE_GOOD_MOVES){
		if (!ENABLE_GOOD_MOVES){ return 0;}
		ulong quickBoard=0;
			foreach(Pattern p in XPatterns){
				quickBoard|=SkillMaskMoves(p,Result.X);
			}
			foreach(Pattern p in OPatterns){
				quickBoard|=SkillMaskMoves(p,Result.O);
			}
			return quickBoard&PossibleMoves();
	}
	public ulong[] Mask(Pattern patt, Result side){
		//edge ban
		ulong keep=FullBoard();
		if(patt.Xsize>0){
			keep&=U.L[7-patt.Xsize];	
		}
		if(patt.Ysize>0){
			keep&=U.Up[7-patt.Ysize];
		}

		ulong pattClone=patt.ThePattern;
		ulong[] boards = new ulong[patt.num+1];
		ulong selfBoard = 0;
		ulong enemyBoard = 0;
		

		if(side==Result.X){selfBoard=Xboard;enemyBoard=Oboard;}
		else if(side==Result.O){selfBoard=Oboard;enemyBoard=Xboard;}
		else{GD.PrintErr($"invalid symbol {side}");}

		int find=0;
		int found=0;
		ulong checkEmptyBan = 0;
		ulong checkFullBan = FullBoard();
		ulong tempNum;
		while(pattClone>0){
			if((pattClone&15)==0){
				pattClone>>=4;
				find+=4;
				continue;
			}

			if((pattClone&1)==0){
				pattClone>>=1;
				find++;
				continue;
			}
			keep&=~(enemyBoard>>find);
			tempNum=selfBoard>>find;
			boards[found]=tempNum;
			checkEmptyBan|=tempNum;
			checkFullBan&=tempNum;
			found++;
			
			//last
			pattClone>>=1;
			find++;
			
		}
		
		//ban empty, ban full
		keep&=checkEmptyBan;
		keep&=~checkFullBan;

		for(int i=0; i<boards.Length-1;i++){
			boards[i]&=keep;
		}
		boards[patt.num]=keep;
		return boards;
	}
	public double SkillMaskValue(Pattern patt, Result side){

		ulong[] boards=Mask(patt,side);
		ulong keep=boards[patt.num];

		int collide;
		int sigma=0;

		//fast count
		ulong bin;
		if(patt.num==3){
			bin=
			(boards[0]&boards[1])|
			(boards[0]&boards[2])|
			(boards[1]&boards[1]);
			sigma+=U.BinCount(bin)<<2;
			keep&=~bin;
			bin=(boards[0]|boards[1]|boards[2])&keep;
			sigma+=U.BinCount(bin);
			return sigma*patt.Point/4;
		}

		//fast count 2
		if(patt.num==4){
			bin=
			(boards[0]&boards[1]&boards[2])|
			(boards[0]&boards[1]&boards[3])|
			(boards[0]&boards[3]&boards[2])|
			(boards[3]&boards[1]&boards[2]);
			sigma+=U.BinCount(bin)<<4;
			keep&=~bin;

			bin=keep;
			bin&=
			(boards[0]&boards[1])|
			(boards[0]&boards[2])|
			(boards[0]&boards[3])|
			(boards[1]&boards[2])|
			(boards[1]&boards[3])|
			(boards[2]&boards[3]);
			sigma+=U.BinCount(bin)<<2;
			keep&=~bin;

			bin=keep;
			bin&=boards[0]|boards[1]|boards[2]|boards[3];
			sigma+=U.BinCount(bin);

			return sigma*patt.Point/16;
		}

		
		//normal count
		int delta=0;
		while(keep>0){

			//dash
			if((keep&U.UpMost)==0){
				keep>>=8;
				delta+=8;
				continue;
			}
			if((keep&1)==0){
				keep>>=1;
				delta++;
				continue;
			}

			//push individual board
			for (int i = 0; i < patt.num; i++)
			{boards[i]>>=delta;}
			delta=0;

			//count individual board
			collide=0;
			for (int i = 0; i < patt.num; i++)
			{
				if((boards[i]&1)==1){collide++;}
			}
			sigma+=1<<(collide<<1);

			keep>>=1;
			delta++;
		}

		return sigma*patt.Point/(double)(1<<(2*patt.num-2));
	}

	public ulong SkillMaskMoves(Pattern patt, Result side){

		ulong[] boards=Mask(patt,side);
		ulong keep=boards[patt.num];

		//fast count
		ulong bin;
		if(patt.num==3){
			bin=
			(boards[0]&boards[1])|
			(boards[0]&boards[2])|
			(boards[1]&boards[1]);
			if (bin>0){return U.WeirdMult(bin,patt.ThePattern);}

			bin=boards[0]|boards[1]|boards[2];
			if (bin>0){return U.WeirdMult(bin,patt.ThePattern);}
		}

		//fast count 2
		if(patt.num==4){
			bin=
			(boards[0]&boards[1]&boards[2])|
			(boards[0]&boards[1]&boards[3])|
			(boards[0]&boards[3]&boards[2])|
			(boards[3]&boards[1]&boards[2]);
			if (bin>0){return U.WeirdMult(bin,patt.ThePattern);}

			bin=
			(boards[0]&boards[1])|
			(boards[0]&boards[2])|
			(boards[0]&boards[3])|
			(boards[1]&boards[2])|
			(boards[1]&boards[3])|
			(boards[2]&boards[3]);
			if (bin>0){return U.WeirdMult(bin,patt.ThePattern);}

			bin=boards[0]|boards[1]|boards[2]|boards[3];
			if (bin>0){return U.WeirdMult(bin,patt.ThePattern);}
		}

		int collide;
		int maxN=0;
		int pos=0;
		int delta=0;
		ulong final=0;
		while(keep>0){
			if((keep&U.UpMost)==0){
				keep>>=8;
				pos+=8;
				delta+=8;
				continue;
			}
			if((keep&1)==0){
				keep>>=1;
				pos++;
				delta++;
				continue;
			}

			for (int i = 0; i < patt.num; i++)
			{boards[i]>>=delta;}
			delta=0;

			collide=0;
			for (int i = 0; i < patt.num; i++)
			{
				if((boards[i]&1)==1){collide++;}
			}
			
			if(collide>maxN){maxN=collide;final=0;}
			if(collide==maxN){final|=patt.ThePattern<<pos;}

			keep>>=1;
			pos++;
			delta++;
		}

		/*patt.Point/(1<<(2*patt.num-2-2*maxN))*/
		return final;
	}
}
