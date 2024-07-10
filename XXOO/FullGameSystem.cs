using Godot;
using System;
using System.Collections.Generic;

public partial class FullGameSystem : Node
{
    /*
    1 = treasure, 2 = heal, 3 = shop, 4 = event, 5 = combat, 6 = boss, 7 = win
    */
    public static int[] Map = {0, 5, 0, 5, 0, 5, 0, 0, 6, 7};
    public static int NextNode = 0;
    public static int NextEnemy = 0;
    /*
        initial player skill set: ({(0, 0), (1, 0), (2, 0)}, 3), ({(0, 0), (0, 1), (0, 2)}, 1)
    */
    public static List<Pattern> PlayerSkillSets =
        new List<Pattern> {
            new Pattern(new List<(int, int)>{(0, 0), (1, 0), (2, 0)}, 3),
            new Pattern(new List<(int, int)>{(0, 0), (0, 1), (0, 2)}, 1)
        }
    ;
    public static int PlayerHealth = 12;
    public static int PlayerMaxHealth = 12;
    public static bool serum = false;
    /*
    0 = {(0, 0), (-1, 2), (1, 2), 2}, {(0, 0), (-1, 1), (-1, 2), (-2, 3), 4}
    */
    public static int[] EnemyFightingList = {0, 0, 0, 3};
    public static List<List<Pattern>> EnemySkillSets = 
        new List<List<Pattern>> {
            new List<Pattern> {
                new Pattern(new List<(int, int)>{(0, 0), (1, 1), (2, 2)}, 3),
                new Pattern(new List<(int, int)>{(0, 0), (0, 1), (1, 0), (1, 1)}, 2)
            },
            new List<Pattern> {
                new Pattern(new List<(int, int)>{(0, 1), (1, 0), (2, 1)}, 2),
                new Pattern(new List<(int, int)>{(0, 0), (1, 1), (2, 0)}, 1)
            },
            new List<Pattern> {
                new Pattern(new List<(int, int)>{(1, 0), (2, 0), (1, 1), (0, 2)}, 3),
                new Pattern(new List<(int, int)>{(0, 0), (0, 1), (1, 1), (0, 2)}, 3)
            },
            new List<Pattern> {
                new Pattern(new List<(int, int)>{(1, 0), (0, 2), (2, 2)}, 2),
                new Pattern(new List<(int, int)>{(2, 0), (1, 1), (1, 2), (0, 3)}, 4)
            }
        }
    ;
    public static List<int> EnemyHealth = new List<int>{3, 5, 4, 6};
    public static List<string> EnemyNames = new List<string>{
        "Ancient Robot", 
        "Transparent Slime", 
        "Geometry Monster", 
        "Spinning Blackhole"
    };
    public static List<string> EnemySpriteLocations = new List<string>{
        "ancient_robot.png",
        "transparent_slime.png",
        "geometry_monster.png",
        "spinning_blackhole.png"
    };
    public static List<string> EnemySkillsLocations = new List<string>{
        "ancient_robot_skills.png",
        "transparent_slime_skills.png",
        "geometry_monster_skills.png",
        "spinning_blackhole_skills.png"
    };
    static public Texture2D LoadEnemy(int enemyId){
        string location="res://enemy sprites/"+EnemySpriteLocations[enemyId];
        return GD.Load<Texture2D>(location);
    }
    
}
