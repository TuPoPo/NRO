
public class BallInfo
{
    public int x;

    public int y;

    public int xTo = -999;

    public int yTo = -999;

    public int count;

    public int vy;

    public int vx;

    public int dir;

    public int idImg;

    public bool isPaint = true;

    public bool isDone;

    public bool isSetImg;

    public Char cFocus;

    public void SetChar()
    {
        cFocus = new Char
        {
            charID = Res.random(-999, -800),
            head = -1,
            body = -1,
            leg = -1,
            bag = -1,
            cName = string.Empty
        };
        cFocus.cHP = cFocus.cHPFull = 20;
    }

    public void UpdChar()
    {
        cFocus.cx = x;
        cFocus.cy = y;
    }
}
