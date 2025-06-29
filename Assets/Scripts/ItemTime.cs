public class ItemTime
{
    public bool isEquivalence;

    public bool isInfinity;

    public short idIcon;

    public int second;

    public int minute;

    private long curr;

    private long last;

    public bool isText;

    private bool dontClear;

    public string text;

    private bool isPaint_coolDownBar;

    public int time;

    public int coutTime;

    private int per = 100;

    public ItemTime()
    {
    }

    public ItemTime(short idIcon, int time, bool isEquivalence)
        : this(idIcon, time)
    {
        this.isEquivalence = isEquivalence;
    }

    public ItemTime(short idIcon, bool isInfinity)
    {
        this.idIcon = idIcon;
        this.isInfinity = isInfinity;
    }

    public ItemTime(short idIcon, int s)
    {
        this.idIcon = idIcon;
        minute = s / 60;
        second = s % 60;
        time = s;
        coutTime = s;
        curr = last = mSystem.currentTimeMillis();
        isPaint_coolDownBar = idIcon == 14;
    }

    public void initTimeText(sbyte id, string text, int time)
    {
        dontClear = time == -1;
        isText = true;
        minute = time / 60;
        second = time % 60;
        idIcon = id;
        this.time = time;
        coutTime = time;
        this.text = text;
        curr = last = mSystem.currentTimeMillis();
        isPaint_coolDownBar = idIcon == 14;
    }

    public void initTime(int time, bool isText)
    {
        minute = time / 60;
        second = time % 60;
        this.time = time;
        coutTime = time;
        this.isText = isText;
        curr = last = mSystem.currentTimeMillis();
    }

    public static bool isExistItem(int id)
    {
        for (int i = 0; i < Char.vItemTime.size(); i++)
        {
            if (((ItemTime)Char.vItemTime.elementAt(i)).idIcon == id)
            {
                return true;
            }
        }
        return false;
    }

    public static ItemTime getMessageById(int id)
    {
        for (int i = 0; i < GameScr.textTime.size(); i++)
        {
            ItemTime itemTime = (ItemTime)GameScr.textTime.elementAt(i);
            if (itemTime.idIcon == id)
            {
                return itemTime;
            }
        }
        return null;
    }

    public static bool isExistMessage(int id)
    {
        for (int i = 0; i < GameScr.textTime.size(); i++)
        {
            if (((ItemTime)GameScr.textTime.elementAt(i)).idIcon == id)
            {
                return true;
            }
        }
        return false;
    }

    public static ItemTime getItemById(int id)
    {
        for (int i = 0; i < Char.vItemTime.size(); i++)
        {
            ItemTime itemTime = (ItemTime)Char.vItemTime.elementAt(i);
            if (itemTime.idIcon == id)
            {
                return itemTime;
            }
        }
        return null;
    }

    public void initTime(int time)
    {
        minute = time / 60;
        second = time % 60;
        coutTime = time;
        curr = last = mSystem.currentTimeMillis();
    }

    public void paint(mGraphics g, int x, int y)
    {
        SmallImage.drawSmallImage(g, idIcon, x, y, 0, 3);
        if (!isInfinity)
        {
            string text = minute + "'" + second + "s";
            if (minute == 0)
            {
                text = second + "s";
            }
            if (isEquivalence)
            {
                text = "~" + text;
            }
            mFont.tahoma_7b_white.drawString(g, text, x, y + 15, 2, mFont.tahoma_7b_dark);
        }
        else
        {
            g.drawImage(ModImages.infinitySymbol, x, y + 21, mGraphics.VCENTER | mGraphics.HCENTER);
        }
    }

    public void paintText(mGraphics g, int x, int y)
    {
        if (isPaint_coolDownBar)
        {
            if (Char.myCharz() != null)
            {
                int num = 80;
                int x2 = (GameCanvas.w / 2) - (num / 2);
                int y2 = GameCanvas.h - 80;
                g.setColor(8421504);
                g.fillRect(x2, y2, num, 2);
                g.setColor(16777215);
                if (per > 0)
                {
                    g.fillRect(x2, y2, num * per / 100, 2);
                }
            }
        }
        else
        {
            string text = minute + "'" + second + "s";
            if (minute < 1)
            {
                text = second + "s";
            }
            if (minute < 0)
            {
                text = string.Empty;
            }
            if (dontClear)
            {
                text = string.Empty;
            }
            mFont.tahoma_7b_white.drawString(g, this.text + " " + text, x, y, mFont.LEFT, mFont.tahoma_7b_dark);
        }
    }

    public void update()
    {
        if (isInfinity)
        {
            return;
        }
        curr = mSystem.currentTimeMillis();
        if (curr - last >= 1000)
        {
            last = mSystem.currentTimeMillis();
            second--;
            coutTime--;
            if (second == -1)
            {
                second = 59;
                minute--;
            }
            if (time > 0)
            {
                per = coutTime * 100 / time;
            }
        }
        if (minute < 0 && !isText)
        {
            Char.vItemTime.removeElement(this);
        }
        if (minute < 0 && isText && !dontClear)
        {
            GameScr.textTime.removeElement(this);
        }
    }
}
