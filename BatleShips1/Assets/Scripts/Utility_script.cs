using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utility_script
{


    public static Vector2 round_pos(Vector2 pos)
    {
        return new Vector2(Mathf.Round(pos.x), Mathf.Round(pos.y));
    }

    public static int random_num(int num1, int num2)
    {
        return Random.Range(num1, num2);
    }

}
