
using UnityEngine;

public class GameMidlet
{
    public static string IP = "112.213.94.23";

    public static int PORT = 14445;

    public static string IP2;

    public static int PORT2;

    public static sbyte PROVIDER;

    public static string VERSION = "2.4.1";
    public static int intVERSION = 241;

    public static string TuPoPo = "1.0.1";

    public static GameCanvas gameCanvas;

    public static GameMidlet instance;

    public static bool isConnect2;

    public static bool isBackWindowsPhone;
    public static int LANGUAGE;
    public GameMidlet()
    {
        initGame();
    }

    public void initGame()
    {
        instance = this;
        MotherCanvas.instance = new MotherCanvas();
        Session_ME.gI().setHandler(Controller.gI());
        Session_ME2.gI().setHandler(Controller.gI());
        Session_ME2.isMainSession = false;
        instance = this;
        gameCanvas = new GameCanvas();
        gameCanvas.start();
        SplashScr.loadImg();
        SplashScr.loadSplashScr();
        GameCanvas.currentScreen = new SplashScr();
    }

    public void exit()
    {
        if (Main.typeClient == 6)
        {
            mSystem.exitWP();
            return;
        }
        GameCanvas.bRun = false;
        mSystem.gcc();
        notifyDestroyed();
    }

    public void notifyDestroyed()
    {
        Main.exit();
    }

    public void platformRequest(string url)
    {
        Cout.LogWarning("PLATFORM REQUEST: " + url);
        Application.OpenURL(url);
    }
}
