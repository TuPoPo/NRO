public class SplashScr : mScreen
{
    public static int splashScrStat;

    private bool isCheckConnect;

    private readonly bool isSwitchToLogin;

    public static int nData = -1;

    public static int maxData = -1;

    public static SplashScr instance;

    public static Image imgLogo;


    public long TIMEOUT;

    public SplashScr()
    {
        instance = this;
    }

    public static void loadSplashScr()
    {
        splashScrStat = 0;
    }

    public override void update()
    {
        if (splashScrStat == 30 && !isCheckConnect)
        {
            isCheckConnect = true;
            if (Rms.loadRMSInt("serverchat") != -1)
            {
                GameScr.isPaintChatVip = Rms.loadRMSInt("serverchat") == 0;
            }

            if (Rms.loadRMSInt("isPlaySound") != -1)
            {
                GameCanvas.isPlaySound = Rms.loadRMSInt("isPlaySound") == 1;
            }

            if (GameCanvas.isPlaySound)
            {
                SoundMn.gI().loadSound(TileMap.mapID);
            }

            SoundMn.gI().getStrOption();
            if (Rms.loadRMSInt("svselect") == -1)
            {
                ServerListScreen.getServerList(ServerListScreen.linkDefault);
                GameCanvas.serverScr.switchToMe();
            }
            else
            {
                ServerListScreen.loadIP();
            }
        }
        splashScrStat++;
        ServerListScreen.updateDeleteData();
        if (splashScrStat >= 150)
        {
            Res.outz("cho man hinh nay qa lau");
            if (Session_ME.gI().isConnected())
            {
                ServerListScreen.loadScreen = true;
                GameCanvas.serverScreen.switchToMe();
            }
            else
            {
                mSystem.onDisconnected();
            }
        }
    }

    public static void loadIP()
    {
        if (Rms.loadRMSInt("svselect") == -1)
        {
            Res.err(">>>loadIP:  svselect == -1");
            int num = 0;
            if (mResources.language > 0)
            {
                for (int i = 0; i < mResources.language; i++)
                {
                    num += ServerListScreen.lengthServer[i];
                }
            }
            ServerListScreen.ipSelect = ServerListScreen.serverPriority == -1
                ? num + Res.random(0, ServerListScreen.lengthServer[mResources.language])
                : ServerListScreen.serverPriority;
            Rms.saveRMSInt("svselect", ServerListScreen.ipSelect);
            GameMidlet.IP = ServerListScreen.address[ServerListScreen.ipSelect];
            GameMidlet.PORT = ServerListScreen.port[ServerListScreen.ipSelect];
            mResources.loadLanguague(ServerListScreen.language[ServerListScreen.ipSelect]);
            LoginScr.serverName = ServerListScreen.nameServer[ServerListScreen.ipSelect];
            GameCanvas.connect();
        }
        else
        {
            ServerListScreen.ipSelect = Rms.loadRMSInt("svselect");
            Res.err(">>>loadIP:  ipSelect == " + ServerListScreen.ipSelect);
            if (ServerListScreen.ipSelect > ServerListScreen.nameServer.Length - 1)
            {
                ServerListScreen.ipSelect = ServerListScreen.serverPriority;
                Rms.saveRMSInt("svselect", ServerListScreen.ipSelect);
            }
            GameMidlet.IP = ServerListScreen.address[ServerListScreen.ipSelect];
            GameMidlet.PORT = ServerListScreen.port[ServerListScreen.ipSelect];
            mResources.loadLanguague(ServerListScreen.language[ServerListScreen.ipSelect]);
            LoginScr.serverName = ServerListScreen.nameServer[ServerListScreen.ipSelect];
            GameCanvas.connect();
        }
    }

    public override void paint(mGraphics g)
    {
        if (imgLogo != null && splashScrStat < 30)
        {
            g.setColor(16777215);
            g.fillRect(0, 0, GameCanvas.w, GameCanvas.h);
            g.drawImage(imgLogo, GameCanvas.w / 2, GameCanvas.h / 2, 3);
        }
        if (nData != -1)
        {
            g.setColor(11584734);
            g.fillRect(0, 0, GameCanvas.w, GameCanvas.h);
            g.drawImage(LoginScr.imgTitle, GameCanvas.w / 2, (GameCanvas.h / 2) - 24, StaticObj.BOTTOM_HCENTER);
            GameCanvas.paintShukiren(GameCanvas.hw, (GameCanvas.h / 2) + 24, g);
            mFont.bigNumber_blue.drawString(g, mResources.downloading_data + (nData * 100 / maxData) + "%", GameCanvas.w / 2, GameCanvas.h / 2, 2);
        }
        else if (splashScrStat >= 30)
        {
            g.setColor(11584734);
            g.fillRect(0, 0, GameCanvas.w, GameCanvas.h);
            GameCanvas.paintShukiren(GameCanvas.hw, GameCanvas.hh, g);
            if (ServerListScreen.cmdDeleteRMS != null)
            {
                mFont.bigNumber_blue.drawString(g, mResources.xoadulieu, GameCanvas.w - 2, GameCanvas.h - 15, 1, mFont.tahoma_7_grey);
            }
        }
    }

    public static void loadImg()
    {
        imgLogo = GameCanvas.loadImage("/gamelogo.png");
    }
}
