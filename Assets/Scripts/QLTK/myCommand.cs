
using UnityEngine;

public class myCommand : Command
{
    public static Image[] btns;

    public static Image[] btnsF;

    static myCommand()
    {
        Texture2D texture2D = Resources.Load<Texture2D>("btn1");
        int num = 24 * mGraphics.zoomLevel;
        int num2 = num * texture2D.width / texture2D.height;
        texture2D = Resize(texture2D, num2, num);
        Image src = Image.createImage(texture2D.EncodeToPNG());
        btns = new Image[3];
        for (int i = 0; i < 3; i++)
        {
            btns[i] = Image.createImage(src, i * (num2 / 3), 0, num2 / 3, num, 0);
        }
        texture2D = Resources.Load<Texture2D>("btn2");
        texture2D = Resize(texture2D, num2, num);
        src = Image.createImage(texture2D.EncodeToPNG());
        btnsF = new Image[3];
        for (int j = 0; j < 3; j++)
        {
            btnsF[j] = Image.createImage(src, j * (num2 / 3), 0, num2 / 3, num, 0);
        }
    }

    private static Texture2D Resize(Texture2D texture2D, int targetX, int targetY)
    {
        RenderTexture dest = RenderTexture.active = new RenderTexture(targetX, targetY, 24);
        Graphics.Blit(texture2D, dest);
        Texture2D texture2D2 = new(targetX, targetY);
        texture2D2.ReadPixels(new Rect(0f, 0f, targetX, targetY), 0, 0);
        texture2D2.Apply();
        return texture2D2;
    }

    public myCommand(string caption, IActionListener actionListener, int action, object p)
        : base(caption, actionListener, action, p)
    {
    }


    public new void paint(mGraphics g)
    {
        if (caption != string.Empty)
        {
            if (!isFocus)
            {
                paintBtn(btns[0], btns[1], btns[2], x, y, w, g);
            }
            else
            {
                paintBtn(btnsF[0], btnsF[1], btnsF[2], x, y, w, g);
            }
        }
        int num = (type != 1) ? (x + 38) : (x + hw);
        if (!isFocus)
        {
            mFont.bigNumber_red.drawString(g, caption, num, y + 7, 2, mFont.tahoma_7b_dark);
        }
        else
        {
            mFont.bigNumber_blue.drawString(g, caption, num, y + 7, 2, mFont.tahoma_7b_dark);
        }
    }

    public void paintBtn(Image img0, Image img1, Image img2, int x, int y, int size, mGraphics g)
    {
        for (int i = img0.getWidth(); i <= size - img0.getWidth() - img2.getWidth(); i += img1.getWidth())
        {
            g.drawImage(img1, x + i, y, 0);
        }
        int num = size % img1.getWidth();
        if (num > 0)
        {
            g.drawRegion(img1, 0, 0, num, 24, 0, x + size - img1.getWidth() - num, y, 0);
        }
        g.drawImage(img0, x, y, 0);
        g.drawImage(img2, x + size - img2.getWidth(), y, 0);
    }
}