using System;
public class BigBoss2 : Mob, IMapObject
{
    public static Image shadowBig;

    public static EffectData data;

    public int xTo;

    public int yTo;

    public bool haftBody;

    public bool change;

    private readonly Mob mob1;

    public new int xSd;

    public new int ySd;

    private bool isOutMap;

    private int wCount;

    public new bool isShadown = true;

    private int tick;

    private int frame;

    public static new Image imgHP = GameCanvas.loadImage("/mainImage/myTexture2dmobHP.png");

    private readonly bool wy;

    private readonly int wt;

    private readonly int fy;

    private readonly int ty;

    public new int typeSuperEff;

    private readonly Char focus;

    private readonly int timeDead;

    private bool flyUp;

    private bool flyDown;

    private int dy;

    public bool changePos;

    private int tShock;

    public new bool isBusyAttackSomeOne = true;

    private readonly int tA;

    private Char[] charAttack;

    private int[] dameHP;

    private sbyte type;

    public new int[] stand = new int[12]
    {
        0, 0, 0, 0, 0, 0, 1, 1, 1, 1,
        1, 1
    };

    public new int[] move = new int[15]
    {
        1, 1, 1, 1, 2, 2, 2, 2, 3, 3,
        3, 3, 2, 2, 2
    };

    public new int[] moveFast = new int[7] { 1, 1, 2, 2, 3, 3, 2 };

    public new int[] attack1 = new int[12]
    {
        0, 0, 0, 7, 7, 7, 8, 8, 8, 9,
        9, 9
    };

    public new int[] attack2 = new int[12]
    {
        0, 0, 0, 10, 10, 10, 11, 11, 11, 12,
        12, 12
    };

    public int[] attack3 = new int[24]
    {
        0, 0, 1, 1, 4, 4, 6, 6, 8, 8,
        25, 25, 26, 26, 28, 28, 30, 30, 32, 32,
        2, 2, 1, 1
    };

    public int[] fly = new int[21]
    {
        4, 4, 4, 5, 5, 5, 6, 6, 6, 6,
        6, 6, 3, 3, 3, 2, 2, 2, 1, 1,
        1
    };

    public int[] hitground = new int[12]
    {
        6, 6, 6, 3, 3, 3, 2, 2, 2, 1,
        1, 1
    };

    private bool shock;

    private readonly sbyte[] cou = new sbyte[2] { -1, 1 };

    public new Char injureBy;

    public new bool injureThenDie;

    public new Mob mobToAttack;

    public new int forceWait;

    public new bool blindEff;

    public new bool sleepEff;

    public BigBoss2(int id, short px, short py, int templateID, int hp, int maxHp, int s)
    {
        shadowBig ??= GameCanvas.loadImage("/mainImage/shadowBig.png");
        mobId = id;
        xTo = x = px + 20;
        yTo = y = py;
        yFirst = py;
        base.hp = hp;
        base.maxHp = maxHp;
        templateId = templateID;
        w_hp_bar = 100;
        h_hp_bar = 6;
        len = w_hp_bar;
        updateHp_bar();
        getDataB();
        status = 2;
    }

    public void getDataB()
    {
        data = null;
        data = new EffectData();
        string patch = "/x" + mGraphics.zoomLevel + "/effectdata/" + 109 + "/data";
        try
        {
            data.readData2(patch);
            data.img = GameCanvas.loadImage("/effectdata/" + 109 + "/img.png");
        }
        catch (Exception)
        {
            Service.gI().requestModTemplate(templateId);
        }
        w = data.width;
        h = data.height;
    }

    public override void setBody(short id)
    {
        changBody = true;
        smallBody = id;
    }

    public override void clearBody()
    {
        changBody = false;
    }

    public static new bool isExistNewMob(string id)
    {
        for (int i = 0; i < Mob.newMob.size(); i++)
        {
            string text = (string)Mob.newMob.elementAt(i);
            if (text.Equals(id))
            {
                return true;
            }
        }
        return false;
    }

    public new void checkFrameTick(int[] array)
    {
        tick++;
        if (tick > array.Length - 1)
        {
            tick = 0;
        }
        frame = array[tick];
    }

    private void updateShadown()
    {
        int num = TileMap.size;
        xSd = x;
        wCount = 0;
        if (ySd <= 0 || TileMap.tileTypeAt(xSd, ySd, 2))
        {
            return;
        }
        if (TileMap.tileTypeAt(xSd / num, ySd / num) == 0)
        {
            isOutMap = true;
        }
        else if (TileMap.tileTypeAt(xSd / num, ySd / num) != 0 && !TileMap.tileTypeAt(xSd, ySd, 2))
        {
            xSd = x;
            ySd = y;
            isOutMap = false;
        }
        while (isOutMap && wCount < 10)
        {
            wCount++;
            ySd += 24;
            if (TileMap.tileTypeAt(xSd, ySd, 2))
            {
                if (ySd % 24 != 0)
                {
                    ySd -= ySd % 24;
                }
                break;
            }
        }
    }

    private void paintShadow(mGraphics g)
    {
        g.drawImage(shadowBig, xSd, yFirst, 3);
        g.setClip(GameScr.cmx, GameScr.cmy - GameCanvas.transY, GameScr.gW, GameScr.gH + (2 * GameCanvas.transY));
    }

    public new void updateSuperEff()
    {
    }

    public override void update()
    {
        if (!isUpdate())
        {
            return;
        }
        updateShadown();
        switch (status)
        {
            case 2:
                updateMobStandWait();
                break;
            case 4:
                timeStatus = 0;
                updateMobFly();
                break;
            case 3:
                updateMobAttack();
                break;
            case 5:
                timeStatus = 0;
                updateMobWalk();
                break;
            case 6:
                timeStatus = 0;
                p1++;
                y += p1;
                if (y >= yFirst)
                {
                    y = yFirst;
                    p1 = 0;
                    status = 5;
                }
                break;
            case 7:
                updateInjure();
                break;
            case 0:
            case 1:
                updateDead();
                break;
        }
    }

    private void updateDead()
    {
        checkFrameTick(stand);
        if (GameCanvas.gameTick % 5 == 0)
        {
            ServerEffect.addServerEffect(167, Res.random(x - (getW() / 2), x + (getW() / 2)), Res.random(getY() + (getH() / 2), getY() + getH()), 1);
        }
        if (x != xTo || y != yTo)
        {
            x += (xTo - x) / 4;
            y += (yTo - y) / 4;
        }
    }

    private void updateMobFly()
    {
        if (flyUp)
        {
            dy++;
            y -= dy;
            checkFrameTick(fly);
            if (y <= -500)
            {
                flyUp = false;
                flyDown = true;
                dy = 0;
            }
        }
        if (flyDown)
        {
            x = xTo;
            dy += 2;
            y += dy;
            checkFrameTick(hitground);
            if (y > yFirst)
            {
                y = yFirst;
                flyDown = false;
                dy = 0;
                status = 2;
                GameScr.shock_scr = 10;
                shock = true;
            }
        }
    }

    public new void setInjure()
    {
    }

    public new void setAttack(Char cFocus)
    {
        isBusyAttackSomeOne = true;
        mobToAttack = null;
        base.cFocus = cFocus;
        p1 = 0;
        p2 = 0;
        status = 3;
        tick = 0;
        dir = (cFocus.cx > x) ? 1 : (-1);
        int cx = cFocus.cx;
        int cy = cFocus.cy;
        if (Res.abs(cx - x) < w * 2 && Res.abs(cy - y) < h * 2)
        {
            x = x < cx ? cx - w : cx + w;
            p3 = 0;
        }
        else
        {
            p3 = 1;
        }
    }

    private bool isSpecial()
    {
        return templateId is (>= 58 and <= 65) or 67 or 68;
    }

    private void updateInjure()
    {
    }

    private void updateMobStandWait()
    {
        checkFrameTick(stand);
        if (x != xTo || y != yTo)
        {
            x += (xTo - x) / 4;
            y += (yTo - y) / 4;
        }
    }

    public void setFly()
    {
        status = 4;
        flyUp = true;
    }

    public void setAttack(Char[] cAttack, int[] dame, sbyte type)
    {
        status = 3;
        charAttack = cAttack;
        dameHP = dame;
        this.type = type;
        tick = 0;
    }

    public new void updateMobAttack()
    {
        if (type == 0)
        {
            if (tick == attack1.Length - 1)
            {
                status = 2;
            }
            dir = (x < charAttack[0].cx) ? 1 : (-1);
            checkFrameTick(attack1);
            x += (charAttack[0].cx - x) / 4;
            y += (charAttack[0].cy - y) / 4;
            xTo = x;
            if (tick == 8)
            {
                for (int i = 0; i < charAttack.Length; i++)
                {
                    charAttack[i].doInjure(dameHP[i], 0, false, false);
                    ServerEffect.addServerEffect(102, charAttack[i].cx, charAttack[i].cy, 1);
                }
            }
        }
        if (type == 1)
        {
            if (tick == attack2.Length - 1)
            {
                status = 2;
            }
            dir = (x < charAttack[0].cx) ? 1 : (-1);
            checkFrameTick(attack2);
            if (tick == 8)
            {
                for (int j = 0; j < charAttack.Length; j++)
                {
                    MonsterDart.addMonsterDart(x + ((dir != 1) ? (-45) : 45), y - 25, true, dameHP[j], 0, charAttack[j], 24);
                }
            }
        }
        if (type != 2)
        {
            return;
        }
        if (tick == fly.Length - 1)
        {
            status = 2;
        }
        dir = (x < charAttack[0].cx) ? 1 : (-1);
        checkFrameTick(fly);
        x += (charAttack[0].cx - x) / 4;
        xTo = x;
        yTo = y;
        if (tick == 12)
        {
            for (int k = 0; k < charAttack.Length; k++)
            {
                charAttack[k].doInjure(dameHP[k], 0, false, false);
                ServerEffect.addServerEffect(102, charAttack[k].cx, charAttack[k].cy, 1);
            }
        }
    }

    public new void updateMobWalk()
    {
    }

    public new bool isPaint()
    {
        if (x < GameScr.cmx)
        {
            return false;
        }
        if (x > GameScr.cmx + GameScr.gW)
        {
            return false;
        }
        return y >= GameScr.cmy && y <= GameScr.cmy + GameScr.gH + 30 && status != 0;
    }

    public new bool isUpdate()
    {
        return status != 0;
    }

    public new bool checkIsBoss()
    {
        return isBoss || levelBoss > 0;
    }

    public override void paint(mGraphics g)
    {
        if (data == null || isHide)
        {
            return;
        }
        if (isMafuba)
        {
            if (!changBody)
            {
                data.paintFrame(g, frame, xMFB, yMFB, (dir != 1) ? 1 : 0, 2);
            }
            else
            {
                SmallImage.drawSmallImage(g, smallBody, xMFB, yMFB, (dir != 1) ? 2 : 0, mGraphics.BOTTOM | mGraphics.HCENTER);
            }
            return;
        }
        if (isShadown && status != 0)
        {
            paintShadow(g);
        }
        g.translate(0, GameCanvas.transY);
        if (!changBody)
        {
            data.paintFrame(g, frame, x, y + fy, (dir != 1) ? 1 : 0, 2);
        }
        else
        {
            SmallImage.drawSmallImage(g, smallBody, x, y + fy - 9, (dir != 1) ? 2 : 0, mGraphics.BOTTOM | mGraphics.HCENTER);
        }
        g.translate(0, -GameCanvas.transY);
        int imageWidth = mGraphics.getImageWidth(imgHPtem);
        int imageHeight = mGraphics.getImageHeight(imgHPtem);
        int num = imageWidth;
        int num3 = x - imageWidth;
        int num4 = y - h - 5;
        int num5 = imageWidth * 2 * per / 100;
        int num2;
        if (num5 > num)
        {
            num2 = num5 - num;
            if (num2 <= 0)
            {
                num2 = 0;
            }
        }
        else
        {
            num = num5;
            num2 = 0;
        }
        g.drawImage(GameScr.imgHP_tm_xam, num3, num4, mGraphics.TOP | mGraphics.LEFT);
        g.drawImage(GameScr.imgHP_tm_xam, num3 + imageWidth, num4, mGraphics.TOP | mGraphics.LEFT);
        g.drawRegion(imgHPtem, 0, 0, num, imageHeight, 0, num3, num4, mGraphics.TOP | mGraphics.LEFT);
        g.drawRegion(imgHPtem, 0, 0, num2, imageHeight, 0, num3 + imageWidth, num4, mGraphics.TOP | mGraphics.LEFT);
        if (shock)
        {
            tShock++;
            Effect me = new((type != 2) ? 22 : 19, x + (tShock * 50), y + 25, 2, 1, -1);
            EffecMn.addEff(me);
            Effect me2 = new((type != 2) ? 22 : 19, x - (tShock * 50), y + 25, 2, 1, -1);
            EffecMn.addEff(me2);
            if (tShock == 50)
            {
                tShock = 0;
                shock = false;
            }
        }
    }

    public new int getHPColor()
    {
        return 16711680;
    }

    public new void startDie()
    {
        hp = 0;
        injureThenDie = true;
        hp = 0;
        status = 1;
        p1 = -3;
        p2 = -dir;
        p3 = 0;
    }

    public new void attackOtherMob(Mob mobToAttack)
    {
        this.mobToAttack = mobToAttack;
        isBusyAttackSomeOne = true;
        cFocus = null;
        p1 = 0;
        p2 = 0;
        status = 3;
        tick = 0;
        dir = (mobToAttack.x > x) ? 1 : (-1);
        int num = mobToAttack.x;
        int num2 = mobToAttack.y;
        if (Res.abs(num - x) < w * 2 && Res.abs(num2 - y) < h * 2)
        {
            x = x < num ? num - w : num + w;
            p3 = 0;
        }
        else
        {
            p3 = 1;
        }
    }

    public new int getX()
    {
        return x;
    }

    public new int getY()
    {
        return y - 50;
    }

    public new int getH()
    {
        return 40;
    }

    public new int getW()
    {
        return 50;
    }

    public new void stopMoving()
    {
        if (status == 5)
        {
            status = 2;
            p1 = p2 = p3 = 0;
            forceWait = 50;
        }
    }

    public new bool isInvisible()
    {
        return status is 0 or 1;
    }

    public new void removeHoldEff()
    {
        if (holdEffID != 0)
        {
            holdEffID = 0;
        }
    }

    public new void removeBlindEff()
    {
        blindEff = false;
    }

    public new void removeSleepEff()
    {
        sleepEff = false;
    }
}