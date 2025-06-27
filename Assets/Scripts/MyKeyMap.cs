using System.Collections;
using UnityEngine;

public class MyKeyMap
{
    private static readonly Hashtable h;

    static MyKeyMap()
    {
        h = new Hashtable
        {
            { KeyCode.A, 97 },
            { KeyCode.B, 98 },
            { KeyCode.C, 99 },
            { KeyCode.D, 100 },
            { KeyCode.E, 101 },
            { KeyCode.F, 102 },
            { KeyCode.G, 103 },
            { KeyCode.H, 104 },
            { KeyCode.I, 105 },
            { KeyCode.J, 106 },
            { KeyCode.K, 107 },
            { KeyCode.L, 108 },
            { KeyCode.M, 109 },
            { KeyCode.N, 110 },
            { KeyCode.O, 111 },
            { KeyCode.P, 112 },
            { KeyCode.Q, 113 },
            { KeyCode.R, 114 },
            { KeyCode.S, 115 },
            { KeyCode.T, 116 },
            { KeyCode.U, 117 },
            { KeyCode.V, 118 },
            { KeyCode.W, 119 },
            { KeyCode.X, 120 },
            { KeyCode.Y, 121 },
            { KeyCode.Z, 122 },
            { KeyCode.Alpha0, 48 },
            { KeyCode.Alpha1, 49 },
            { KeyCode.Alpha2, 50 },
            { KeyCode.Alpha3, 51 },
            { KeyCode.Alpha4, 52 },
            { KeyCode.Alpha5, 53 },
            { KeyCode.Alpha6, 54 },
            { KeyCode.Alpha7, 55 },
            { KeyCode.Alpha8, 56 },
            { KeyCode.Alpha9, 57 },
            { KeyCode.Space, 32 },
            { KeyCode.F1, -21 },
            { KeyCode.F2, -22 },
            { KeyCode.Equals, -25 },
            { KeyCode.Minus, 45 },
            { KeyCode.F3, -23 },
            { KeyCode.UpArrow, -1 },
            { KeyCode.DownArrow, -2 },
            { KeyCode.LeftArrow, -3 },
            { KeyCode.RightArrow, -4 },
            { KeyCode.Backspace, -8 },
            { KeyCode.Return, -5 },
            { KeyCode.Period, 46 },
            { KeyCode.At, 64 },
            { KeyCode.Tab, -26 }
        };
    }

    public static int map(KeyCode k)
    {
        object obj = h[k];
        return obj == null ? 0 : (int)obj;
    }
}
