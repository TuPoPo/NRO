﻿using System.Net.NetworkInformation;
using System.Threading;
using UnityEngine;
using Vietpad.InputMethod;

public class Main : MonoBehaviour
{
    public static Main main;

    public static mGraphics g;

    public static GameMidlet midlet;

    public static string res = "res";

    public static string mainThreadName;

    public static bool started;

    public static bool isIpod;

    public static bool isIphone4;

    public static bool isPC;

    public static bool isWindowsPhone;

    public static bool isIPhone;

    public static bool IphoneVersionApp;

    public static string IMEI;

    public static int versionIp;

    public static int numberQuit = 1;

    public static int typeClient = 4;

    public const sbyte PC_VERSION = 4;

    public const sbyte IP_APPSTORE = 5;

    public const sbyte WINDOWSPHONE = 6;

    private int level;

    public const sbyte IP_JB = 3;

    private int updateCount;

    private int paintCount;

    private int count;

    private int fps;

    private int max;

    private int up;

    private int upmax;

    private long timefps;

    private long timeup;

    private bool isRun;

    public static int waitTick;

    public static int f;

    public static bool isResume;

    public static bool isMiniApp = true;

    public static bool isQuitApp;

    private Vector2 lastMousePos;

    public static int a = 1;

    public static bool isCompactDevice = true;

    private void Start()
    {
        Time.timeScale = 2f;
        Server.LoadResources();
        if (started)
        {
            return;
        }
        if (Thread.CurrentThread.Name != "Main")
        {
            Thread.CurrentThread.Name = "Main";
        }
        mainThreadName = Thread.CurrentThread.Name;
        if (ActionPerform.IsMobile())
        {
            isPC = false;
        }
        else
        {
            isPC = true;
            VietKeyHandler.SmartMark = true;

        }
        started = true;
        Bomong.loadData();
        if (isPC)
        {
            level = Rms.loadRMSInt("levelScreenKN");
            if (level == 1)
            {
                Screen.SetResolution(720, 320, fullscreen: false);
            }
            else
            {
                Screen.SetResolution(1024, 600, fullscreen: false);
            }
        }
    }
    private void OnGUI()
    {
        if (count >= 10)
        {
            if (fps == 0)
            {
                timefps = mSystem.currentTimeMillis();
            }
            else if (mSystem.currentTimeMillis() - timefps > 1000)
            {
                max = fps;
                fps = 0;
                timefps = mSystem.currentTimeMillis();
            }
            fps++;
            checkInput();
            Session_ME.update();
            Session_ME2.update();
            if (Event.current.type.Equals(EventType.Repaint) && paintCount <= updateCount)
            {

                GameMidlet.gameCanvas.paint(g);
                mFont.bigNumber_orange.drawString(g, $"{System.Math.Round(1f / Time.smoothDeltaTime * Time.timeScale, 1):0.#}", 2, 0, 0);
                paintCount++;
                g.reset();
            }
        }
    }

    public void setsizeChange()
    {
        if (!isRun)
        {
           
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            Screen.orientation = ScreenOrientation.LandscapeLeft;
            Application.runInBackground = true;
            Application.targetFrameRate = 60;
            base.useGUILayout = false;
            if (main == null)
            {
                main = this;
            }
            isRun = true;
            ScaleGUI.initScaleGUI();
            IMEI = isPC ? SystemInfo.deviceUniqueIdentifier : GetMacAddress();
            isPC = true;
            if (isPC)
            {
                Screen.fullScreen = false;
            }
            if (isWindowsPhone)
            {
                typeClient = 6;
            }
            if (isPC)
            {
                typeClient = 4;
            }
            if (IphoneVersionApp)
            {
                typeClient = 5;
            }
            g = new mGraphics();
            midlet = new GameMidlet();
            TileMap.loadBg();
            Paint.loadbg();
            PopUp.loadBg();
            GameScr.loadBg();
            InfoMe.gI().loadCharId();
            Panel.loadBg();
            Menu.loadBg();
            Key.mapKeyPC();
            SoundMn.gI().loadSound(TileMap.mapID);
        }
    }
    public string GetMacAddress()
    {
        _ = string.Empty;
        NetworkInterface[] allNetworkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
        for (int i = 0; i < allNetworkInterfaces.Length; i++)
        {
            PhysicalAddress physicalAddress = allNetworkInterfaces[i].GetPhysicalAddress();
            if (physicalAddress.ToString() != string.Empty)
            {
                return physicalAddress.ToString();
            }
        }
        return string.Empty;
    }

    public void doClearRMS()
    {
        if (isPC && Rms.loadRMSInt("lastZoomlevel") != mGraphics.zoomLevel)
        {
            Rms.clearAll();
            Rms.saveRMSInt("lastZoomlevel", mGraphics.zoomLevel);
            Rms.saveRMSInt("levelScreenKN", level);
        }
    }
    public static void closeKeyBoard()
    {
        if (TouchScreenKeyboard.visible)
        {
            TField.kb.active = false;
            TField.kb = null;
        }
    }

    private void FixedUpdate()
    {
        Rms.update();
        count++;

        if (count >= 10)
        {
            if (up == 0)
            {
                timeup = mSystem.currentTimeMillis();
            }
            else if (mSystem.currentTimeMillis() - timeup > 1000)
            {
                upmax = up;
                up = 0;
                timeup = mSystem.currentTimeMillis();
            }
            up++;
            setsizeChange();
            updateCount++;
            GameMidlet.gameCanvas.update();
            Image.update();
            MainThreadDispatcher.update();
           
            DataInputStream.update();
            f++;
            if (f > 8)
            {
                f = 0;
            }
            if (!isPC)
            {
                _ = 1 / a;
            }
        }
    }

    private void checkInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePosition = Input.mousePosition;
            GameMidlet.gameCanvas.pointerPressed((int)(mousePosition.x / mGraphics.zoomLevel),
                (int)((Screen.height - mousePosition.y) / mGraphics.zoomLevel) +
                mGraphics.addYWhenOpenKeyBoard);
            lastMousePos.x = mousePosition.x / mGraphics.zoomLevel;
            lastMousePos.y = (mousePosition.y / mGraphics.zoomLevel) + mGraphics.addYWhenOpenKeyBoard;
        }

        if (Input.GetMouseButton(0))
        {
            Vector3 mousePosition2 = Input.mousePosition;
            GameMidlet.gameCanvas.pointerDragged((int)(mousePosition2.x / mGraphics.zoomLevel),
                (int)((Screen.height - mousePosition2.y) / mGraphics.zoomLevel) +
                mGraphics.addYWhenOpenKeyBoard);
            lastMousePos.x = mousePosition2.x / mGraphics.zoomLevel;
            lastMousePos.y = (mousePosition2.y / mGraphics.zoomLevel) + mGraphics.addYWhenOpenKeyBoard;
        }

        if (Input.GetMouseButtonUp(0))
        {
            Vector3 mousePosition3 = Input.mousePosition;
            lastMousePos.x = mousePosition3.x / mGraphics.zoomLevel;
            lastMousePos.y = (mousePosition3.y / mGraphics.zoomLevel) + mGraphics.addYWhenOpenKeyBoard;
            GameMidlet.gameCanvas.pointerReleased((int)(mousePosition3.x / mGraphics.zoomLevel),
                (int)((Screen.height - mousePosition3.y) / mGraphics.zoomLevel) +
                mGraphics.addYWhenOpenKeyBoard);
        }

        if (Input.anyKeyDown && Event.current.type == EventType.KeyDown)
        {
            int num = MyKeyMap.map(Event.current.keyCode);
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                switch (Event.current.keyCode)
                {
                    case KeyCode.Alpha2:
                        num = 64;
                        break;
                    case KeyCode.Minus:
                        num = 95;
                        break;
                }
            }

            if (num != 0)
            {
                GameMidlet.gameCanvas.keyPressedz(num);
            }
        }

        if (Event.current.type == EventType.KeyUp)
        {
            int num2 = MyKeyMap.map(Event.current.keyCode);
            if (num2 != 0)
            {
                GameMidlet.gameCanvas.keyReleasedz(num2);
            }
        }
        if (isPC)
        {
            GameMidlet.gameCanvas.scrollMouse((int)(Input.GetAxis("Mouse ScrollWheel") * 10f));
            float x = Input.mousePosition.x;
            float y = Input.mousePosition.y;
            int x2 = (int)x / mGraphics.zoomLevel;
            int y2 = (Screen.height - (int)y) / mGraphics.zoomLevel;
            GameMidlet.gameCanvas.pointerMouse(x2, y2);
        }
    }

    private void OnApplicationQuit()
    {
        if (TeleportMenu.close())
        {
            return;
        }
        Debug.LogWarning("APP QUIT");
        GameCanvas.bRun = false;
        Session_ME.gI().close();
        Session_ME2.gI().close();
        if (isPC)
        {
            Application.Quit();
        }
    }

    private void OnApplicationPause(bool paused)
    {
        isResume = false;
        if (paused)
        {
            if (GameCanvas.isWaiting())
            {
                isQuitApp = true;
            }
        }
        else
        {
            isResume = true;
        }
        if (TouchScreenKeyboard.visible)
        {
            TField.kb.active = false;
            TField.kb = null;
        }
        if (isQuitApp)
        {
            Application.Quit();
        }
    }

    public static void exit()
    {
        if (isPC)
        {
            main.OnApplicationQuit();
        }
        else
        {
            a = 0;
        }
    }
}
