
using System;

public class ipKeyboard
{
    private static UnityEngine.TouchScreenKeyboard tk;

    public static int TEXT;

    public static int NUMBERIC = 1;

    public static int PASS = 2;

    private static Command act;

    public static void openKeyBoard(string caption, int type, string text, Command action)
    {
        act = action;
        TouchScreenKeyboardType t = (type is 0 or 2) ? TouchScreenKeyboardType.ASCIICapable : TouchScreenKeyboardType.NumberPad;
        TouchScreenKeyboard.hideInput = false;
        tk = TouchScreenKeyboard.Open(text, t, false, false, type == 2, false, caption);
    }

    public static void update()
    {
        try
        {
            if (tk != null)
            {
                act?.perform(tk.text);
                tk.text = string.Empty;
                tk = null;
            }
        }
        catch (Exception)
        {
        }
    }
}
