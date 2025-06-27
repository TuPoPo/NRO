
using System.Collections;
using System.Threading;
using UnityEngine;

public class AutoLogin
{
    public static string acc;

    public static string pass;

    public static int server = -1;
    public static int tWaitToLogin;
    public static bool waitToLogin;

    public static IEnumerator loadAccount()
    {
        while (waitToLogin && tWaitToLogin >= 0) // Lặp từ 5 về 0
        {
            Debug.Log($"Đếm ngược: {tWaitToLogin}");
            yield return new WaitForSecondsRealtime(1f);
            tWaitToLogin--;
            if (tWaitToLogin == 2)
            {
                if (!Session_ME.gI().isConnected())
                {
                    GameCanvas.connect();
                }
                while (GameCanvas.currentScreen != GameCanvas.serverScreen)
                {
                    yield return new WaitForSecondsRealtime(3f);
                }
                GameCanvas.currentDialog?.center.performAction();
                if (!ServerListScreen.loadScreen)
                {
                    GameCanvas.serverScreen.perform(2, null);
                    yield return new WaitForSecondsRealtime(1f);
                }
                while (!ServerListScreen.loadScreen || GameCanvas.currentScreen != GameCanvas.serverScreen)
                {
                    Debug.Log("Chờ tải...");
                    yield return null;
                }
                Debug.Log("Bắt đầu đăng nhập");
                GameCanvas.loginScr ??= new LoginScr();
            }

            if (tWaitToLogin == 0) // Khi đếm xuống 0, tiến hành đăng nhập
            {
                waitToLogin = false;
                tWaitToLogin = 3;
                Debug.Log("Loginnnnnnnnnnn"); acc = (acc != null && acc != string.Empty) ? acc : GameCanvas.loginScr.tfUser.getText();
                pass = (pass != null && pass != string.Empty) ? pass : GameCanvas.loginScr.tfPass.getText();
                server = (server != -1) ? server : ServerListScreen.ipSelect;
                if (acc != string.Empty && pass != string.Empty && server != -1)
                {
                    Rms.saveRMSString("acc", acc);
                    Rms.saveRMSString("pass", pass);
                    yield return new WaitForSecondsRealtime(2f);
                    if (GameCanvas.currentScreen == GameCanvas.loginScr || GameCanvas.currentScreen == GameCanvas.serverScreen)
                    {
                        Thread.Sleep(100);
                        if (server != ServerListScreen.ipSelect)
                        {
                            GameCanvas.serverScr.perform(100 + server, null);
                            yield return new WaitForSecondsRealtime(0.5f);
                        }
                        if (!Session_ME.gI().isConnected())
                        {
                            GameCanvas.connect();
                        }
                        GameCanvas.loginScr ??= new LoginScr();
                        GameCanvas.loginScr.switchToMe();
                        Service.gI().login(acc, pass, GameMidlet.VERSION, 0);
                        if (Session_ME.connected)
                        {
                            GameCanvas.startWaitDlg();
                        }
                        else
                        {
                            GameCanvas.startOKDlg(mResources.maychutathoacmatsong);
                        }
                    }
                }
                else if ((GameCanvas.currentScreen == GameCanvas.loginScr || GameCanvas.currentScreen == GameCanvas.serverScreen) && Rms.loadRMSString("acc") != string.Empty)
                {
                    GameCanvas.serverScreen.perform(3, null);
                }

            }
        }
    }

}