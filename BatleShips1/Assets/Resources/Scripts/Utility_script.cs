using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utility_script
{


    public static Vector2Int round_pos(Vector2 pos)
    {
        return new Vector2Int((int)Mathf.Round(pos.x), (int)Mathf.Round(pos.y));
    }

    public static int random_num(int num1, int num2)
    {
        return Random.Range(num1, num2);
    }

}
