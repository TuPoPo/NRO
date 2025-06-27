
using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class QLTKPanel : mScreen, IActionListener
{
    public static myCommand qltkCmd;

    private bool isInputAccount;

    private bool isSelectSv;

    private bool isEditAccount;

    private readonly Scroll scroll;

    private readonly int w;

    private readonly int h;

    private readonly int x;

    private readonly int y;

    private readonly int itemH = 30;

    public int bColor = 15115520;

    public List<Account> accounts = new();

    private readonly TField userName;

    private readonly TField password;

    private readonly myCommand back;

    private readonly myCommand ok;

    public myCommand server;

    private readonly myCommand btnAdd;

    private readonly myCommand btnEdit;

    private readonly myCommand btnBack;

    private int mainSelect;

    private int numw;

    private int numh;

    private readonly MyVector vecServer = new();

    private int accountSelect = -1;

    private readonly WaitTimer wait = new();

    private static QLTKPanel instance;

    static QLTKPanel()
    {
        qltkCmd = new myCommand("Auto Manager", GameCanvas.serverScreen, 999, null)
        {
            w = 76,
            x = 5
        };
        qltkCmd.y = GameCanvas.h - qltkCmd.h - 5;
    }

    public static QLTKPanel gI()
    {
        instance ??= new QLTKPanel();
        return instance;
    }

    public QLTKPanel()
    {
        scroll = new Scroll();
        userName = new TField();
        userName.setIputType(TField.INPUT_TYPE_ANY);
        userName.name = "Account";
        userName.width = 160;
        userName.height = mScreen.ITEM_HEIGHT + 2;
        password = new TField();
        password.setIputType(TField.INPUT_TYPE_PASSWORD);
        password.name = "Password";
        password.width = 160;
        password.height = mScreen.ITEM_HEIGHT + 2;
        back = new myCommand("Back", this, 1, null)
        {
            w = 76
        };
        ok = new myCommand("OK", this, 2, null)
        {
            w = 76
        };
        server = new myCommand("Server", this, 3, "");
        server.setType();
        w = GameCanvas.w - 50;
        h = GameCanvas.h - 70;
        x = (GameCanvas.w - w) / 2;
        y = GameCanvas.hh - (h / 2);
        load();
        scroll.setStyle(accounts.Count, itemH, x, y, w, h, styleUPDOWN: true, 1);
        btnAdd = new myCommand("Add", this, 5, null)
        {
            w = 76,
            x = x
        };
        btnAdd.y = GameCanvas.h - btnAdd.h;
        btnEdit = new myCommand("Edit", this, 6, null)
        {
            w = 76
        };
        btnEdit.x = GameCanvas.hw - (btnEdit.w / 2);
        btnEdit.y = GameCanvas.h - btnAdd.h;
        btnBack = new myCommand("Back", this, 7, null)
        {
            w = 76,
            x = x + w - btnEdit.w,
            y = GameCanvas.h - btnAdd.h
        };
    }

    public override void paint(mGraphics g)
    {
        GameCanvas.paintBGGameScr(g);
        g.translate(-g.getTranslateX(), -g.getTranslateY());
        if (isSelectSv)
        {
            for (int i = 0; i < vecServer.size(); i++)
            {
                if (vecServer.elementAt(i) != null)
                {
                    ((myCommand)vecServer.elementAt(i)).paint(g);
                }
            }
            back.x = btnBack.x;
            back.y = btnBack.y;
            back.paint(g);
        }
        else if (isInputAccount)
        {
            paintInputAccount(g);
        }
        else
        {
            paintListAccount(g);
        }
        base.paint(g);
    }

    private int paintButton(mGraphics g, string text, int x, int y, int size, int alignText, int alignBtn = 0, bool isFocus = false)
    {
        Image[] array = isFocus ? myCommand.btnsF : myCommand.btns;
        int width = array[1].getWidth();
        int num = Mathf.Max(size, width);
        int num2 = num % width;
        int num3 = (width * 2) + num2 + Mathf.Clamp(num - (width * 2), 0, Mathf.Abs(num - (width * 2)));
        if (alignBtn == mFont.RIGHT)
        {
            x -= num3;
        }
        else if (alignBtn == mFont.CENTER)
        {
            x -= num3 / 2;
        }
        g.drawImage(array[0], x, y, 0);
        for (int i = width; i < num - width; i += width)
        {
            g.drawImage(array[1], x + i, y, 0);
        }
        if (num2 > 0)
        {
            g.drawRegion(array[1], 0, 0, num2, array[1].getHeight(), 0, x + num - num2, y, 0);
        }
        g.drawImage(array[2], x + num, y, 0);
        int num4 = (alignText == mFont.CENTER) ? (num3 / 2) : 10;
        mFont.tahoma_7b_white.drawString(g, text, x + num4, y + 6, alignText);
        return num3;
    }

    private void paintListAccount(mGraphics g)
    {
        PopUp.paintPopUp(g, x, y - 5, w, h + 5, -1, isButton: true);
        g.translate(x, y);
        g.setClip(0, 0, w, h);
        g.translate(scroll.cmx, -scroll.cmy);
        for (int i = 0; i < accounts.Count; i++)
        {
            int num = i * itemH;
            int size = w * 4 / 7;
            string text = accounts[i].ipserver.Split(":")[0];
            _ = paintButton(g, accounts[i].username + " - " + text, 12, num, size, mFont.CENTER, 0, accountSelect == i);
            _ = paintButton(g, "Delete", w - 140, num, 32, mFont.CENTER);
            _ = paintButton(g, "Login", w - 75, num, 32, mFont.CENTER);
        }
        g.reset();
        btnAdd.paint(g);
        btnEdit.paint(g);
        btnBack.paint(g);
    }

    private void paintInputAccount(mGraphics g)
    {
        int num = 170;
        int num2 = 250;
        int num3 = (GameCanvas.w / 2) - (num2 / 2);
        int num4 = GameCanvas.hh - (num / 2);
        PopUp.paintPopUp(g, num3, num4, num2, num, -1, isButton: true);
        userName.x = (GameCanvas.w / 2) - (userName.width / 2);
        userName.y = num4 + 50;
        password.x = (GameCanvas.w / 2) - (password.width / 2);
        password.y = userName.y + userName.height + 10;
        server.x = userName.x;
        server.y = password.y + password.height + 10;
        back.x = userName.x + userName.width - back.w;
        back.y = num4 + num - 30;
        ok.x = userName.x;
        ok.y = num4 + num - 30;
        userName.paint(g);
        password.paint(g);
        GameCanvas.resetTrans(g);
        back.paint(g);
        ok.paint(g);
        server.paint(g);
    }

    public override void update()
    {
        GameScr.cmx++;
        if (GameScr.cmx > (GameCanvas.w * 3) + 100)
        {
            GameScr.cmx = 100;
        }
        if (isSelectSv)
        {
            for (int i = 0; i < vecServer.size(); i++)
            {
                Command command = (Command)vecServer.elementAt(i);
                if (command != null && command.isPointerPressInside())
                {
                    command.performAction();
                }
            }
        }
        else if (isInputAccount)
        {
            userName.update();
            password.update();
        }
        else
        {
            scroll.updatecm();
            base.update();
        }
    }

    public override void updateKey()
    {
        if (isSelectSv)
        {
            if (back.isPointerPressInside())
            {
                isSelectSv = false;
            }
            updateSelectSv();
            return;
        }
        if (isInputAccount)
        {
            if (GameCanvas.keyPressed[Key.FIRE])
            {
                GameCanvas.clearKeyPressed();
                if (userName.isFocus)
                {
                    userName.setFocus(isFocus: false);
                    password.setFocusWithKb(isFocus: true);
                }
                else if (password.isFocus)
                {
                    password.setFocus(isFocus: false);
                    userName.setFocusWithKb(isFocus: true);
                }
            }
            if (back.isPointerPressInside())
            {
                back.performAction();
            }
            else if (ok.isPointerPressInside())
            {
                ok.performAction();
            }
            else if (server.isPointerPressInside())
            {
                server.performAction();
            }
            GameCanvas.clearKeyPressed();
            return;
        }
        if (btnAdd.isPointerPressInside())
        {
            btnAdd.performAction();
        }
        if (btnEdit.isPointerPressInside())
        {
            btnEdit.performAction();
        }
        if (btnBack.isPointerPressInside())
        {
            btnBack.performAction();
        }
        if (GameCanvas.isPointerJustRelease && GameCanvas.isPointerClick && !GameCanvas.isPointerMove)
        {
            accountSelect = (scroll.cmtoY + GameCanvas.py - y) / itemH;
            if (accountSelect >= accounts.Count)
            {
                accountSelect = -1;
            }
            if (accountSelect != -1)
            {
                if (GameCanvas.isPointerHoldIn(x + w - 140, y + (accountSelect * itemH) - scroll.cmy, 64, 24))
                {
                    GameCanvas.clearAllPointerEvent();
                    SoundMn.gI().buttonClick();
                    accounts.RemoveAt(accountSelect);
                    save();
                }
                else if (GameCanvas.isPointerHoldIn(x + w - 75, y + (accountSelect * itemH) - scroll.cmy, 64, 24))
                {
                    GameCanvas.clearAllPointerEvent();
                    SoundMn.gI().buttonClick();
                    Account account = accounts[accountSelect];
                    AutoLogin.acc = account.username;
                    AutoLogin.pass = account.password;
                    AutoLogin.server = 0;
                    perform(8, null);
                }
            }
            return;
        }
        if (scroll.cmyLim > 0)
        {
            ScrollResult scrollResult = scroll.updateKey();
            if (scroll.cmy < 0 && !scrollResult.isDowning)
            {
                scroll.cmy -= scroll.cmy / 2;
            }
            else if (scroll.cmy > scroll.cmyLim && !scrollResult.isDowning)
            {
                scroll.cmy -= (scroll.cmy - scroll.cmyLim + 6) / 2;
            }
        }
        GameCanvas.clearKeyPressed();
    }

    public override void keyPress(int keyCode)
    {
        if (userName.isFocus)
        {
            _ = userName.keyPressed(keyCode);
        }
        if (password.isFocus)
        {
            _ = password.keyPressed(keyCode);
        }
    }

    private void updateSelectSv()
    {
        int num = mainSelect % numw;
        int num2 = mainSelect / numw;
        if (GameCanvas.keyPressed[4])
        {
            if (num > 0)
            {
                mainSelect--;
            }
            GameCanvas.keyPressed[4] = false;
        }
        else if (GameCanvas.keyPressed[6])
        {
            if (num < numw - 1)
            {
                mainSelect++;
            }
            GameCanvas.keyPressed[6] = false;
        }
        else if (GameCanvas.keyPressed[2])
        {
            if (num2 > 0)
            {
                mainSelect -= numw;
            }
            GameCanvas.keyPressed[2] = false;
        }
        else if (GameCanvas.keyPressed[8])
        {
            if (num2 < numh - 1)
            {
                mainSelect += numw;
            }
            GameCanvas.keyPressed[8] = false;
        }
        if (mainSelect < 0)
        {
            mainSelect = 0;
        }
        if (mainSelect >= vecServer.size())
        {
            mainSelect = vecServer.size() - 1;
        }
        if (GameCanvas.keyPressed[5])
        {
            ((Command)vecServer.elementAt(num)).performAction();
            GameCanvas.keyPressed[5] = false;
        }
        GameCanvas.clearKeyPressed();
    }

    public override void switchToMe()
    {
        SoundMn.gI().stopAll();
        vecServer.removeAllElements();
        for (int i = 0; i < ServerListScreen.nameServer.Length; i++)
        {
            string p = ServerListScreen.nameServer[i] + ":" + ServerListScreen.address[i] + ":" + ServerListScreen.port[i] + ":0,0,0";
            myCommand myCommand2 = new(ServerListScreen.nameServer[i], this, 100 + i, p)
            {
                w = 76
            };
            vecServer.addElement(myCommand2);
        }
        sort();
        base.switchToMe();
    }

    private void sort()
    {
        mainSelect = 0;
        string text = (string)server.p;
        for (int i = 0; i < ServerListScreen.nameServer.Length; i++)
        {
            if (ServerListScreen.nameServer[i] != null && text.StartsWith(ServerListScreen.nameServer[i]))
            {
                mainSelect = i;
                break;
            }
        }
        int num = 5;
        int num2 = 76;
        int num3 = mScreen.cmdH;
        numw = 2;
        if (GameCanvas.w > 3 * (num2 + num))
        {
            numw = 3;
        }
        if (vecServer.size() < 3)
        {
            numw = 2;
        }
        numh = (vecServer.size() / numw) + ((vecServer.size() % numw != 0) ? 1 : 0);
        for (int j = 0; j < vecServer.size(); j++)
        {
            Command command = (Command)vecServer.elementAt(j);
            if (command != null)
            {
                int num4 = GameCanvas.hw - (numw * (num2 + num) / 2) + (j % numw * (num2 + num));
                int num5 = GameCanvas.hh - (numh * (num3 + num) / 2) + (j / numw * (num3 + num));
                command.x = num4;
                command.y = num5;
            }
        }
    }

    private void save()
    {
        try
        {
            JArray jArray = new();
            for (int i = 0; i < accounts.Count; i++)
            {
                Account account = accounts[i];
                JObject jObject = new()
                {
                    { "id", account.id },
                    { "username", account.username },
                    { "password", account.password },
                    { "ip", account.ipserver }
                };
                jArray.Add(jObject);
            }
            Rms.saveRMSString("listAccount", jArray.ToString());
        }
        catch (Exception ex)
        {
            Debug.LogError("err save acc " + ex.Message);
        }
    }

    private void load()
    {
        try
        {
            accounts.Clear();
            JArray jArray = JArray.Parse(Rms.loadRMSString("listAccount") ?? "[]");
            for (int i = 0; i < jArray.Count; i++)
            {
                JObject jObject = (JObject)jArray[i];
                accounts.Add(new Account(jObject.Value<int>("id"), jObject.Value<string>("username"), jObject.Value<string>("password"), jObject.Value<string>("ip")));
            }
        }
        catch (Exception exception)
        {
            Debug.LogException(exception);
        }
    }

    public void perform(int idAction, object p)
    {
        if (idAction == 1)
        {
            isEditAccount = false;
            isInputAccount = false;
        }
        else if (idAction == 2)
        {
            isInputAccount = false;
            try
            {
                if (isEditAccount)
                {
                    if (accountSelect >= 0)
                    {
                        accounts[accountSelect].username = userName.getText();
                        accounts[accountSelect].password = password.getText();
                        accounts[accountSelect].ipserver = (string)server.p;
                    }
                    isEditAccount = false;
                }
                else
                {
                    int id = (accounts.Count != 0) ? (accounts[^1].id + 1) : 0;
                    string text = (string)server.p;
                    Debug.Log("ip =>>>>>>>>>>" + text);
                    accounts.Add(new Account(id, userName.getText(), password.getText(), text));
                }
                save();
                Debug.Log("okokok");
                scroll.clear();
                scroll.setStyle(accounts.Count, itemH, x, y, w, h, styleUPDOWN: true, 1);
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
            }
        }
        else if (idAction == 3)
        {
            isSelectSv = true;
        }
        else if (idAction >= 100)
        {
            string text2 = (string)p;
            Debug.Log(text2);
            server.caption = text2.Split(":")[0];
            server.p = text2;
            isSelectSv = false;
        }
        if (idAction == 5)
        {
            isInputAccount = true;
        }
        if (idAction == 6 && accountSelect >= 0)
        {
            Account account = accounts[accountSelect];
            isInputAccount = true;
            isEditAccount = true;
            userName.setText(account.username);
            password.setText(account.password);
            server.caption = account.ipserver.Split(':')[0];
            server.p = account.ipserver;
        }
        if (idAction == 7)
        {
            GameCanvas.serverScreen.switchToMe();
        }
        if (idAction == 8 && accountSelect >= 0)
        {
            AutoLogin.waitToLogin = true;
            AutoLogin.tWaitToLogin = 3;
            Session_ME.gI().close();
            ServerListScreen.getServerList(accounts[accountSelect].ipserver);
            Rms.saveRMSInt("svselect", 0);
            ServerListScreen.ipSelect = 0;
            GameCanvas.connect();
            _ = Main.main.StartCoroutine(AutoLogin.loadAccount());
        }
    }
}
