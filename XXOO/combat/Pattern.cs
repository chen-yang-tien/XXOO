using Godot;
using System;
using System.Collections.Generic;


public partial class Pattern
{
	public ulong ThePattern;
	public int Xsize;
	public int Ysize;
	public int num;


	public int Point;
	
	//normal pattern
	public Pattern(List<(int x, int y)> GivenPattern, int GivenPoint)
	{
		Xsize=0;Ysize=0;num=0;
		foreach ((int x, int y) in GivenPattern){
			if(x<0||y<0){GD.Print("u fucked up (in Pattern");}
			ThePattern|=(ulong)1<<(x+y*U.boardSizeX);
			Xsize=(x>Xsize)? x:Xsize;
			Ysize=(y>Ysize)? y:Ysize;
			num++;
		}
		Point = GivenPoint;
	}
	
}
