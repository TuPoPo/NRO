using UnityEngine;

public class Skill
{
    public const sbyte ATT_STAND = 0;

    public const sbyte ATT_FLY = 1;

    public const sbyte SKILL_AUTO_USE = 0;

    public const sbyte SKILL_CLICK_USE_ATTACK = 1;

    public const sbyte SKILL_CLICK_USE_BUFF = 2;

    public const sbyte SKILL_CLICK_NPC = 3;

    public const sbyte SKILL_CLICK_LIVE = 4;

    public SkillTemplate template;

    public short skillId;

    public int point;

    public long powRequire;

    public int coolDown;

    public long lastTimeUseThisSkill;

    public int dx;

    public int dy;

    public int maxFight;

    public int manaUse;

    public SkillOption[] options;

    public bool paintCanNotUseSkill;

    public short damage;

    public string moreInfo;

    public short price;

    public short curExp;

    public string strCurExp()
    {
        if (curExp / 10 >= 100)
        {
            return "MAX";
        }
        if (curExp % 10 == 0)
        {
            return (curExp / 10) + "%";
        }
        int num = curExp % 10;
        return (curExp / 10) + "." + (num % 10) + "%";
    }

    public string strTimeReplay()
    {
        if (coolDown % 1000 == 0)
        {
            return (coolDown / 1000) + string.Empty;
        }
        int num = coolDown % 1000;
        return (coolDown / 1000) + "." + ((num % 100 != 0) ? (num / 10) : (num / 100));
    }

    public void paint(int x, int y, mGraphics g)
    {
        SmallImage.drawSmallImage(g, template.iconId, x, y, 0, StaticObj.VCENTER_HCENTER);
        long num = mSystem.currentTimeMillis() - lastTimeUseThisSkill;
        if (num < coolDown)
        {
            float num2 = num * 360 / coolDown;
            Texture2D texture2D = new(20 * mGraphics.zoomLevel, 20 * mGraphics.zoomLevel, TextureFormat.RGBA32, mipChain: false);
            for (int i = 0; i < 20 * mGraphics.zoomLevel; i++)
            {
                for (int j = 0; j < 20 * mGraphics.zoomLevel; j++)
                {
                    texture2D.SetPixel(i, j, new Color(0f, 0f, 0f, 0f));
                }
            }
            for (int k = 0; k < 10 * mGraphics.zoomLevel; k++)
            {
                texture2D.SetPixel(10 * mGraphics.zoomLevel, k + (10 * mGraphics.zoomLevel), new Color(0f, 0f, 0f, 0.4f));
            }
            if (num2 < 270f)
            {
                for (int l = 0; l <= 10 * mGraphics.zoomLevel; l++)
                {
                    for (int m = 0; m <= 10 * mGraphics.zoomLevel; m++)
                    {
                        texture2D.SetPixel(l, m + (10 * mGraphics.zoomLevel), new Color(0f, 0f, 0f, 0.4f));
                    }
                }
            }
            if (num2 < 180f)
            {
                for (int n = 0; n <= 10 * mGraphics.zoomLevel; n++)
                {
                    for (int num3 = 0; num3 <= 10 * mGraphics.zoomLevel; num3++)
                    {
                        texture2D.SetPixel(n, num3, new Color(0f, 0f, 0f, 0.4f));
                    }
                }
            }
            if (num2 < 90f)
            {
                for (int num4 = 0; num4 <= 10 * mGraphics.zoomLevel; num4++)
                {
                    for (int num5 = 0; num5 <= 10 * mGraphics.zoomLevel; num5++)
                    {
                        texture2D.SetPixel(num4 + (10 * mGraphics.zoomLevel), num5, new Color(0f, 0f, 0f, 0.4f));
                    }
                }
            }
            for (int num6 = 0; num6 < 10 * mGraphics.zoomLevel; num6++)
            {
                if (num2 <= 90f)
                {
                    int num7 = (int)(Mathf.Tan((float)System.Math.PI / 180f * num2) * num6);
                    if (num7 < 10 * mGraphics.zoomLevel && num7 > 0)
                    {
                        texture2D.SetPixel(num7 + (10 * mGraphics.zoomLevel), num6 - (10 * mGraphics.zoomLevel), new Color(1f, 0f, 0f, 1f));
                    }
                }
                else if (num2 <= 180f)
                {
                    int num8 = (int)(Mathf.Tan((float)System.Math.PI / 180f * (180f - num2)) * ((10 * mGraphics.zoomLevel) - num6));
                    if (num8 < 10 * mGraphics.zoomLevel && num8 > 0)
                    {
                        texture2D.SetPixel(num8 + (10 * mGraphics.zoomLevel), num6, new Color(1f, 0f, 0f, 1f));
                    }
                }
                else if (num2 <= 270f)
                {
                    int num9 = (int)(Mathf.Tan((float)System.Math.PI / 180f * (num2 - 180f)) * ((10 * mGraphics.zoomLevel) - num6));
                    if (num9 < 10 * mGraphics.zoomLevel && num9 > 0)
                    {
                        texture2D.SetPixel((10 * mGraphics.zoomLevel) - num9, num6, new Color(1f, 0f, 0f, 1f));
                    }
                }
                else
                {
                    int num10 = (int)(Mathf.Tan((float)System.Math.PI / 180f * (360f - num2)) * num6);
                    if (num10 < 10 * mGraphics.zoomLevel && num10 > 0)
                    {
                        texture2D.SetPixel((10 * mGraphics.zoomLevel) - num10, num6 - (10 * mGraphics.zoomLevel), new Color(1f, 0f, 0f, 1f));
                    }
                }
            }
            for (int num11 = 0; num11 < 10 * mGraphics.zoomLevel; num11++)
            {
                int num12;
                for (num12 = 0; num12 < 10 * mGraphics.zoomLevel; num12++)
                {
                    if (num2 < 90f)
                    {
                        if (texture2D.GetPixel(num12 + (10 * mGraphics.zoomLevel), num11 + (10 * mGraphics.zoomLevel)).r == 1f)
                        {
                            for (int num13 = num12; num13 <= 10 * mGraphics.zoomLevel; num13++)
                            {
                                texture2D.SetPixel(num13 + (10 * mGraphics.zoomLevel), num11 + (10 * mGraphics.zoomLevel), new Color(0f, 0f, 0f, 0.4f));
                            }
                            break;
                        }
                    }
                    else if (num2 < 180f)
                    {
                        if (texture2D.GetPixel(num12 + (10 * mGraphics.zoomLevel), num11).r == 1f)
                        {
                            for (int num14 = 0; num14 <= num12; num14++)
                            {
                                texture2D.SetPixel(num14 + (10 * mGraphics.zoomLevel), num11, new Color(0f, 0f, 0f, 0.4f));
                            }
                            break;
                        }
                    }
                    else if (num2 < 270f)
                    {
                        if (texture2D.GetPixel(num12, num11).r == 1f)
                        {
                            for (int num15 = 0; num15 <= num12; num15++)
                            {
                                texture2D.SetPixel(num15, num11, new Color(0f, 0f, 0f, 0.4f));
                            }
                            break;
                        }
                    }
                    else if (num2 < 360f && texture2D.GetPixel(num12, num11 + (10 * mGraphics.zoomLevel)).r == 1f)
                    {
                        for (int num16 = num12; num16 <= 10 * mGraphics.zoomLevel; num16++)
                        {
                            texture2D.SetPixel(num16, num11 + (10 * mGraphics.zoomLevel), new Color(0f, 0f, 0f, 0.4f));
                        }
                        break;
                    }
                }
                if (num12 < 10 * mGraphics.zoomLevel)
                {
                    continue;
                }
                for (num12 = 0; num12 <= 10 * mGraphics.zoomLevel; num12++)
                {
                    if (num2 < 45f)
                    {
                        texture2D.SetPixel(num12 + (10 * mGraphics.zoomLevel), num11 + (10 * mGraphics.zoomLevel), new Color(0f, 0f, 0f, 0.4f));
                    }
                    else if (num2 < 135f)
                    {
                        texture2D.SetPixel(num12 + (10 * mGraphics.zoomLevel), num11, new Color(0f, 0f, 0f, 0.4f));
                    }
                    else if (num2 < 225f)
                    {
                        texture2D.SetPixel(num12, num11, new Color(0f, 0f, 0f, 0.4f));
                    }
                    else if (num2 < 315f && num11 != 0)
                    {
                        texture2D.SetPixel(num12, num11 + (10 * mGraphics.zoomLevel), new Color(0f, 0f, 0f, 0.4f));
                    }
                }
            }
            texture2D.Apply();
            Graphics.DrawTexture(new Rect((x - 10) * mGraphics.zoomLevel, (y - 10) * mGraphics.zoomLevel, 20 * mGraphics.zoomLevel, 20 * mGraphics.zoomLevel), texture2D);
            _ = $"{(coolDown - num) / 1000f:#.0}";
        }
        else
        {
            paintCanNotUseSkill = false;
        }
    }
}
