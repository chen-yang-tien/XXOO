using Godot;
using System;

public enum Result {NONE, X, O, DRAW}

public static class U
{
    public static Random rand=new Random();
	public static int boardSizeX=8;
	public static int boardSizeY=8;
    public static ulong LeftMost=1|1<<8|1<<16|1<<24|(ulong)1<<32|(ulong)1<<40|(ulong)1<<48|(ulong)1<<56;  
	public static ulong RightMost=LeftMost<<7;
	public static ulong UpMost=(1<<8)-1;
	public static ulong DownMost=UpMost<<56;
	public static ulong Edge1=LeftMost|RightMost|UpMost|DownMost;
	public static ulong Edge2=Spread(Edge1);
	
	public static ulong Center4=1<<27|1<<28|(ulong)1<<35|(ulong)1<<36;
	public static ulong Center12=Spread(Center4);
	public static ulong Corners=1|1<<7|(ulong)1<<56|(ulong)1<<63;

	static int Coin=0;

	public static ulong[] L=new ulong[]{
		LeftMost,LeftMost*3,LeftMost*7,LeftMost*15,
		LeftMost*31,LeftMost*63,LeftMost*127
	};

	public static ulong[] Up=new ulong[]{
		UpMost,
		UpMost<<8|UpMost,
		UpMost<<16|UpMost<<8|UpMost,
		UpMost<<24|UpMost<<16|UpMost<<8|UpMost,
		UpMost<<32|UpMost<<24|UpMost<<16|UpMost<<8|UpMost,
		UpMost<<40|UpMost<<32|UpMost<<24|UpMost<<16|UpMost<<8|UpMost,
		UpMost<<48|UpMost<<40|UpMost<<32|UpMost<<24|UpMost<<16|UpMost<<8|UpMost,
	};
	

	public static ulong Slice(ulong a, int pos){
		return (a>>pos)|(a<<(64-pos));
	}
	public static ulong Spread(ulong a){
		return a|(a<<8)|(a>>8)|((a<<1)&(~LeftMost))|((a>>1)&(~RightMost));
	}

	public static ulong Push(ulong a, int x, int y){
		if (x>0){
			a&=L[7-x];
			a<<=x;
		}
		else if (x<0){
			a&=(~L[-x-1]);
			a>>=-x;
		}
		
		if (y>0){return a<<(y*8);}
		return a>>(y*-8);
	}

	public static Result Last(Result current){
		if(current==Result.X){return Result.O;}
		else if(current==Result.O){return Result.X;}
		
		return Result.NONE;
	}
	public static void BinaryShow(ulong board){
		
		GD.Print("<Board>");
		int follow=0;int gogo=0;string output="";
		while(gogo<boardSizeX*boardSizeY){
			if((board&1)==1){output+="1";}
			else{output+="0";}
			follow++;
			if(follow==boardSizeX){
				GD.Print(output);
				follow=0;
				output="";
			}
			board>>=1;
			gogo++;
		}
		GD.Print("==================");
	}

	public static int BinCount(ulong board){
		int i=0;
		while(board!=0){
			if((board&UpMost)==0){board>>=8;continue;}
			if((board&15)==0){board>>=4;continue;}
			if((board&15)==15){board>>=4;i+=4;continue;}
			if((board&1)==1){i++;}
			board>>=1;
		}
		return i;
	}

	public static ulong WeirdMult(ulong board, ulong mult){
		int id=0;
		ulong empty=0;
		while(mult!=0){
			if((mult&U.UpMost)==0){mult>>=8;id+=8;continue;}
			if((mult&15)==0){mult>>=4;id+=4;continue;}
			if((mult&1)==0){mult>>=1;id++;continue;}
			empty|=board<<id;
			mult>>=1;
			id++;
		}
		if(empty==0){GD.PrintErr($"invalid weirdmult, {board} {mult}");}
		return empty;
	}

	public static bool Coinflip(){

		Coin>>=1;
		if (Coin==0){Coin=rand.Next();}
		
		return (Coin&1)==0;
	}

}
