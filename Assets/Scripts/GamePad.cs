
using System;

public class GamePad
{
    private int xC;

    private int yC;

    private int xM;

    private int yM;

    private int xMLast;

    private int yMLast;

    private readonly int R;

    private readonly int r;

    private int d;

    private int xTemp;

    private int yTemp;

    private int deltaX;

    private int deltaY;

    private int delta;

    private int angle;

    public int xZone;

    public int yZone;

    public int wZone;

    public int hZone;

    private bool isGamePad;

    public bool isSmallGamePad;

    public bool isMediumGamePad;

    public bool isLargeGamePad;

    public GamePad()
    {
        R = 28;
        if (GameCanvas.w < 300)
        {
            isSmallGamePad = true;
            isMediumGamePad = false;
            isLargeGamePad = false;
        }
        if (GameCanvas.w is >= 300 and <= 380)
        {
            isSmallGamePad = false;
            isMediumGamePad = true;
            isLargeGamePad = false;
        }
        if (GameCanvas.w > 380)
        {
            isSmallGamePad = false;
            isMediumGamePad = false;
            isLargeGamePad = true;
        }
        if (!isLargeGamePad)
        {
            xZone = 0;
            wZone = GameCanvas.hw;
            yZone = GameCanvas.hh >> 1;
            hZone = GameCanvas.h - 80;
        }
        else
        {
            xZone = 0;
            wZone = (GameCanvas.hw / 4 * 3) - 20;
            yZone = GameCanvas.hh >> 1;
            hZone = GameCanvas.h;
        }
    }

    public void update()
    {
        try
        {
            if (GameScr.isAnalog == 0)
            {
                return;
            }
            if (GameCanvas.isPointerDown && !GameCanvas.isPointerJustRelease)
            {
                xTemp = GameCanvas.pxFirst;
                yTemp = GameCanvas.pyFirst;
                if (xTemp < xZone || xTemp > wZone || yTemp < yZone || yTemp > hZone)
                {
                    return;
                }
                if (!isGamePad)
                {
                    xC = xM = xTemp;
                    yC = yM = yTemp;
                }
                isGamePad = true;
                deltaX = GameCanvas.px - xC;
                deltaY = GameCanvas.py - yC;
                delta = Math.pow(deltaX, 2) + Math.pow(deltaY, 2);
                d = Res.sqrt(delta);
                if (Math.abs(deltaX) <= 4 && Math.abs(deltaY) <= 4)
                {
                    return;
                }
                angle = Res.angle(deltaX, deltaY);
                if (!GameCanvas.isPointerHoldIn(xC - R, yC - R, 2 * R, 2 * R))
                {
                    if (d != 0)
                    {
                        yM = deltaY * R / d;
                        xM = deltaX * R / d;
                        xM += xC;
                        yM += yC;
                        if (!Res.inRect(xC - R, yC - R, 2 * R, 2 * R, xM, yM))
                        {
                            xM = xMLast;
                            yM = yMLast;
                        }
                        else
                        {
                            xMLast = xM;
                            yMLast = yM;
                        }
                    }
                    else
                    {
                        xM = xMLast;
                        yM = yMLast;
                    }
                }
                else
                {
                    xM = GameCanvas.px;
                    yM = GameCanvas.py;
                }
                resetHold();
                if (checkPointerMove(2))
                {
                    if (angle is (<= 360 and >= 340) or (>= 0 and <= 20))
                    {
                        GameCanvas.keyHold[(!Main.isPC) ? 6 : 24] = true;
                        GameCanvas.keyPressed[(!Main.isPC) ? 6 : 24] = true;
                    }
                    else if (angle is > 40 and < 70)
                    {
                        GameCanvas.keyHold[(!Main.isPC) ? 6 : 24] = true;
                        GameCanvas.keyPressed[(!Main.isPC) ? 6 : 24] = true;
                    }
                    else if (angle is >= 70 and <= 110)
                    {
                        GameCanvas.keyHold[(!Main.isPC) ? 8 : 22] = true;
                        GameCanvas.keyPressed[(!Main.isPC) ? 8 : 22] = true;
                    }
                    else if (angle is > 110 and < 120)
                    {
                        GameCanvas.keyHold[(!Main.isPC) ? 4 : 23] = true;
                        GameCanvas.keyPressed[(!Main.isPC) ? 4 : 23] = true;
                    }
                    else if (angle is >= 120 and <= 200)
                    {
                        GameCanvas.keyHold[(!Main.isPC) ? 4 : 23] = true;
                        GameCanvas.keyPressed[(!Main.isPC) ? 4 : 23] = true;
                    }
                    else if (angle is > 200 and < 250)
                    {
                        GameCanvas.keyHold[(!Main.isPC) ? 2 : 21] = true;
                        GameCanvas.keyPressed[(!Main.isPC) ? 2 : 21] = true;
                        GameCanvas.keyHold[(!Main.isPC) ? 4 : 23] = true;
                        GameCanvas.keyPressed[(!Main.isPC) ? 4 : 23] = true;
                    }
                    else if (angle is >= 250 and <= 290)
                    {
                        GameCanvas.keyHold[(!Main.isPC) ? 2 : 21] = true;
                        GameCanvas.keyPressed[(!Main.isPC) ? 2 : 21] = true;
                    }
                    else if (angle is > 290 and < 340)
                    {
                        GameCanvas.keyHold[(!Main.isPC) ? 2 : 21] = true;
                        GameCanvas.keyPressed[(!Main.isPC) ? 2 : 21] = true;
                        GameCanvas.keyHold[(!Main.isPC) ? 6 : 24] = true;
                        GameCanvas.keyPressed[(!Main.isPC) ? 6 : 24] = true;
                    }
                }
                else
                {
                    resetHold();
                }
            }
            else
            {
                xM = xC = 45;
                yM = !isLargeGamePad ? (yC = GameCanvas.h - 90) : (yC = GameCanvas.h - 45);
                isGamePad = false;
                resetHold();
            }
        }
        catch (Exception)
        {
        }
    }

    private bool checkPointerMove(int distance)
    {
        if (GameScr.isAnalog == 0)
        {
            return false;
        }
        if (Char.myCharz().statusMe == 3)
        {
            return true;
        }
        try
        {
            for (int num = 2; num > 0; num--)
            {
                int i = GameCanvas.arrPos[num].x - GameCanvas.arrPos[num - 1].x;
                int i2 = GameCanvas.arrPos[num].y - GameCanvas.arrPos[num - 1].y;
                if (Res.abs(i) > distance && Res.abs(i2) > distance)
                {
                    return false;
                }
            }
        }
        catch (Exception)
        {
        }
        return true;
    }

    private void resetHold()
    {
        GameCanvas.clearKeyHold();
    }

    public void paint(mGraphics g)
    {
        if (GameScr.isAnalog != 0)
        {
            g.drawImage(GameScr.imgAnalog1, xC, yC, mGraphics.HCENTER | mGraphics.VCENTER);
            g.drawImage(GameScr.imgAnalog2, xM, yM, mGraphics.HCENTER | mGraphics.VCENTER);
        }
    }

    public bool disableCheckDrag()
    {
        return GameScr.isAnalog != 0 && isGamePad;
    }

    public bool disableClickMove()
    {
        try
        {
            if (GameScr.isAnalog == 0)
            {
                return false;
            }
            //  bool flag = false;
            return (GameCanvas.px >= xZone && GameCanvas.px <= wZone && GameCanvas.py >= yZone && GameCanvas.py <= hZone) || GameCanvas.px >= GameCanvas.w - 50;
        }
        catch (Exception)
        {
            return false;
        }
    }
}
