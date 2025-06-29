
public class MotherCanvas
{
    public static MotherCanvas instance;

    public GameCanvas tCanvas;

    public int zoomLevel = 1;

    public Image imgCache;

    private readonly int[] imgRGBCache;

    private readonly int newWidth;

    private readonly int newHeight;

    private readonly int[] output;

    //  private int OUTPUTSIZE = 20;

    public MotherCanvas()
    {
        checkZoomLevel(getWidth(), getHeight());
    }

    public void checkZoomLevel(int w, int h)
    {
        if (Main.isWindowsPhone)
        {
            mGraphics.zoomLevel = 2;
            if (w * h >= 2073600)
            {
                mGraphics.zoomLevel = 4;
            }
            else if (w * h > 384000)
            {
                mGraphics.zoomLevel = 3;
            }
        }
        else if (!Main.isPC)
        {
            if (Main.isIpod)
            {
                mGraphics.zoomLevel = 2;
            }
            else if (w * h >= 2073600)
            {
                mGraphics.zoomLevel = 4;
            }
            else if (w * h >= 691200)
            {
                mGraphics.zoomLevel = 3;
            }
            else if (w * h > 153600)
            {
                mGraphics.zoomLevel = 2;
            }
        }
        else
        {
            if (!Main.isPC)
            {
                if (w * h >= 2073600)
                {
                    mGraphics.zoomLevel = 4;
                }
                else if (w * h >= 691200)
                {
                    mGraphics.zoomLevel = 3;
                }
            }
            else
            {
                mGraphics.zoomLevel = 2;
                if (w * h >= 2073600)
                {
                    mGraphics.zoomLevel = 4;
                }
                else if (w * h >= 691200)
                {
                    mGraphics.zoomLevel = 3;
                }
            }
        }
    }

    public int getWidth()
    {
        return (int)ScaleGUI.WIDTH;
    }

    public int getHeight()
    {
        return (int)ScaleGUI.HEIGHT;
    }

    public void setChildCanvas(GameCanvas tCanvas)
    {
        this.tCanvas = tCanvas;
    }

    protected void paint(mGraphics g)
    {
        tCanvas.paint(g);
    }

    protected void keyPressed(int keyCode)
    {
        tCanvas.keyPressedz(keyCode);
    }

    protected void keyReleased(int keyCode)
    {
        tCanvas.keyReleasedz(keyCode);
    }

    protected void pointerDragged(int x, int y)
    {
        x /= mGraphics.zoomLevel;
        y /= mGraphics.zoomLevel;
        tCanvas.pointerDragged(x, y);
    }

    protected void pointerPressed(int x, int y)
    {
        x /= mGraphics.zoomLevel;
        y /= mGraphics.zoomLevel;
        tCanvas.pointerPressed(x, y);
    }

    protected void pointerReleased(int x, int y)
    {
        x /= mGraphics.zoomLevel;
        y /= mGraphics.zoomLevel;
        tCanvas.pointerReleased(x, y);
    }

    public int getWidthz()
    {
        int width = getWidth();
        return (width / mGraphics.zoomLevel) + (width % mGraphics.zoomLevel);
    }

    public int getHeightz()
    {
        int height = getHeight();
        return (height / mGraphics.zoomLevel) + (height % mGraphics.zoomLevel);
    }
}
