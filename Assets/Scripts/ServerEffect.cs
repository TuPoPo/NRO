
public class ServerEffect : Effect2
{
    public EffectCharPaint eff;

    private int i0;

    private readonly int dx0;

    private readonly int dy0;

    private int x;

    private int y;

    private Char c;

    private Mob m;

    private short loopCount;

    private long endTime;

    private int trans;

    public static void addServerEffect(int id, int cx, int cy, int loopCount)
    {
        ServerEffect serverEffect = new()
        {
            eff = GameScr.efs[id - 1],
            x = cx,
            y = cy,
            loopCount = (short)loopCount
        };
        Effect2.vEffect2.addElement(serverEffect);
    }

    public static void addServerEffect(int id, int cx, int cy, int loopCount, int trans)
    {
        ServerEffect serverEffect = new()
        {
            eff = GameScr.efs[id - 1],
            x = cx,
            y = cy,
            loopCount = (short)loopCount,
            trans = trans
        };
        Effect2.vEffect2.addElement(serverEffect);
    }

    public static void addServerEffect(int id, Mob m, int loopCount)
    {
        ServerEffect serverEffect = new()
        {
            eff = GameScr.efs[id - 1],
            m = m,
            loopCount = (short)loopCount
        };
        Effect2.vEffect2.addElement(serverEffect);
    }

    public static void addServerEffect(int id, Char c, int loopCount)
    {
        ServerEffect serverEffect = new()
        {
            eff = GameScr.efs[id - 1],
            c = c,
            loopCount = (short)loopCount
        };
        Effect2.vEffect2.addElement(serverEffect);
    }

    public static void addServerEffect(int id, Char c, int loopCount, int trans)
    {
        ServerEffect serverEffect = new()
        {
            eff = GameScr.efs[id - 1],
            c = c,
            loopCount = (short)loopCount,
            trans = trans
        };
        Effect2.vEffect2.addElement(serverEffect);
    }

    public static void addServerEffectWithTime(int id, int cx, int cy, int timeLengthInSecond)
    {
        ServerEffect serverEffect = new()
        {
            eff = GameScr.efs[id - 1],
            x = cx,
            y = cy,
            endTime = mSystem.currentTimeMillis() + (timeLengthInSecond * 1000)
        };
        Effect2.vEffect2.addElement(serverEffect);
    }

    public static void addServerEffectWithTime(int id, Char c, int timeLengthInSecond)
    {
        ServerEffect serverEffect = new()
        {
            eff = GameScr.efs[id - 1],
            c = c,
            endTime = mSystem.currentTimeMillis() + (timeLengthInSecond * 1000)
        };
        Effect2.vEffect2.addElement(serverEffect);
    }

    public override void paint(mGraphics g)
    {
        if (mGraphics.zoomLevel == 1)
        {
            GameScr.countEff++;
        }
        if (GameScr.countEff < 8)
        {
            if (c != null)
            {
                x = c.cx;
                y = c.cy + GameCanvas.transY;
            }
            if (m != null)
            {
                x = m.x;
                y = m.y + GameCanvas.transY;
            }
            int num = x + dx0 + eff.arrEfInfo[i0].dx;
            int num2 = y + dy0 + eff.arrEfInfo[i0].dy;
            if (GameCanvas.isPaint(num, num2))
            {
                SmallImage.drawSmallImage(g, eff.arrEfInfo[i0].idImg, num, num2, trans, mGraphics.VCENTER | mGraphics.HCENTER);
            }
        }
    }

    public override void update()
    {
        if (endTime != 0)
        {
            i0++;
            if (i0 >= eff.arrEfInfo.Length)
            {
                i0 = 0;
            }
            if (mSystem.currentTimeMillis() - endTime > 0)
            {
                Effect2.vEffect2.removeElement(this);
            }
        }
        else
        {
            i0++;
            if (i0 >= eff.arrEfInfo.Length)
            {
                loopCount--;
                if (loopCount <= 0)
                {
                    Effect2.vEffect2.removeElement(this);
                }
                else
                {
                    i0 = 0;
                }
            }
        }
        if (GameCanvas.gameTick % 11 == 0 && c != null && c != Char.myCharz() && !GameScr.vCharInMap.contains(c))
        {
            Effect2.vEffect2.removeElement(this);
        }
    }
}
