using System;
using Assets.src.e;
using Assets.src.f;
using Assets.src.g;
using UnityEngine;
public class Controller : IMessageHandler
{
    protected static Controller me;

    protected static Controller me2;

    public Message messWait;

    public static bool isLoadingData;

    public static bool isConnectOK;

    public static bool isConnectionFail;

    public static bool isDisconnected;

    public static bool isMain;

    private float demCount;

    private readonly int move;

    private readonly int total;

    public static bool isStopReadMessage;

    public static MyHashTable frameHT_NEWBOSS = new();

    public const sbyte PHUBAN_TYPE_CHIENTRUONGNAMEK = 0;

    public const sbyte PHUBAN_START = 0;

    public const sbyte PHUBAN_UPDATE_POINT = 1;

    public const sbyte PHUBAN_END = 2;

    public const sbyte PHUBAN_LIFE = 4;

    public const sbyte PHUBAN_INFO = 5;

    public static Controller gI()
    {
        me ??= new Controller();
        return me;
    }

    public static Controller gI2()
    {
        me2 ??= new Controller();
        return me2;
    }

    public void onConnectOK(bool isMain1)
    {
        isMain = isMain1;
        mSystem.onConnectOK();
    }

    public void onConnectionFail(bool isMain1)
    {
        isMain = isMain1;
        mSystem.onConnectionFail();
    }

    public void onDisconnected(bool isMain1)
    {
        isMain = isMain1;
        mSystem.onDisconnected();
    }

    public void requestItemPlayer(Message msg)
    {
        try
        {
            int num = msg.reader().readUnsignedByte();
            Item item = GameScr.currentCharViewInfo.arrItemBody[num];
            item.saleCoinLock = msg.reader().readInt();
            item.sys = msg.reader().readByte();
            item.options = new MyVector();
            try
            {
                while (true)
                {
                    item.options.addElement(new ItemOption(msg.reader().readUnsignedByte(), msg.reader().readUnsignedShort()));
                }
            }
            catch (Exception ex)
            {
                Cout.println("Loi tairequestItemPlayer 1" + ex.ToString());
            }
        }
        catch (Exception ex2)
        {
            Cout.println("Loi tairequestItemPlayer 2" + ex2.ToString());
        }
    }

    public void onMessage(Message msg)
    {
        GameCanvas.debugSession.removeAllElements();
        GameCanvas.debug("SA1", 2);
        try
        {
            Res.outz("<<<Read cmd= " + msg.command);
            Char @char = null;
            Mob mob = null;
            MyVector myVector = new();
            int num = 0;
            Controller2.readMessage(msg);
            sbyte cmd = msg.command;
            //Debug.Log("Recive CMD: " + cmd);
            switch (cmd)
            {
                case 24:
                    read_opt(msg);
                    break;
                case 20:
                    phuban_Info(msg);
                    break;
                case 66:
                    readGetImgByName(msg);
                    break;
                case 65:
                    {
                        sbyte b61 = msg.reader().readSByte();
                        string text6 = msg.reader().readUTF();
                        short num151 = msg.reader().readShort();
                        if (ItemTime.isExistMessage(b61))
                        {
                            if (num151 != 0)
                            {
                                ItemTime.getMessageById(b61).initTimeText(b61, text6, num151);
                            }
                            else
                            {
                                GameScr.textTime.removeElement(ItemTime.getMessageById(b61));
                            }
                        }
                        else
                        {
                            ItemTime itemTime = new();
                            itemTime.initTimeText(b61, text6, num151);
                            GameScr.textTime.addElement(itemTime);
                        }
                        break;
                    }
                case 112:
                    {
                        sbyte b66 = msg.reader().readByte();
                        Res.outz("spec type= " + b66);
                        if (b66 == 0)
                        {
                            Panel.spearcialImage = msg.reader().readShort();
                            Panel.specialInfo = msg.reader().readUTF();
                        }
                        else
                        {
                            if (b66 != 1)
                            {
                                break;
                            }
                            sbyte b67 = msg.reader().readByte();
                            Char.myCharz().infoSpeacialSkill = new string[b67][];
                            Char.myCharz().imgSpeacialSkill = new short[b67][];
                            GameCanvas.panel.speacialTabName = new string[b67][];
                            for (int num159 = 0; num159 < b67; num159++)
                            {
                                GameCanvas.panel.speacialTabName[num159] = new string[2];
                                string[] array17 = Res.split(msg.reader().readUTF(), "\n", 0);
                                if (array17.Length == 2)
                                {
                                    GameCanvas.panel.speacialTabName[num159] = array17;
                                }
                                if (array17.Length == 1)
                                {
                                    GameCanvas.panel.speacialTabName[num159][0] = array17[0];
                                    GameCanvas.panel.speacialTabName[num159][1] = string.Empty;
                                }
                                int num160 = msg.reader().readByte();
                                Char.myCharz().infoSpeacialSkill[num159] = new string[num160];
                                Char.myCharz().imgSpeacialSkill[num159] = new short[num160];
                                for (int num161 = 0; num161 < num160; num161++)
                                {
                                    Char.myCharz().imgSpeacialSkill[num159][num161] = msg.reader().readShort();
                                    Char.myCharz().infoSpeacialSkill[num159][num161] = msg.reader().readUTF();
                                }
                            }
                            GameCanvas.panel.tabName[25] = GameCanvas.panel.speacialTabName;
                            GameCanvas.panel.setTypeSpeacialSkill();
                            GameCanvas.panel.show();
                        }
                        break;
                    }
                case -98:
                    {
                        sbyte b62 = msg.reader().readByte();
                        GameCanvas.menu.showMenu = false;
                        if (b62 == 0)
                        {
                            GameCanvas.startYesNoDlg(msg.reader().readUTF(), new Command(mResources.YES, GameCanvas.instance, 888397, msg.reader().readUTF()), new Command(mResources.NO, GameCanvas.instance, 888396, null));
                        }
                        break;
                    }
                case -97:
                    Char.myCharz().cNangdong = msg.reader().readInt();
                    break;
                case -96:
                    {
                        sbyte typeTop = msg.reader().readByte();
                        GameCanvas.panel.vTop.removeAllElements();
                        string topName = msg.reader().readUTF();
                        sbyte b29 = msg.reader().readByte();
                        for (int num55 = 0; num55 < b29; num55++)
                        {
                            int rank = msg.reader().readInt();
                            int pId = msg.reader().readInt();
                            short headID = msg.reader().readShort();
                            short headICON = msg.reader().readShort();
                            short body = msg.reader().readShort();
                            short leg = msg.reader().readShort();
                            string name = msg.reader().readUTF();
                            string info = msg.reader().readUTF();
                            TopInfo topInfo = new()
                            {
                                rank = rank,
                                headID = headID,
                                headICON = headICON,
                                body = body,
                                leg = leg,
                                name = name,
                                info = info,
                                info2 = msg.reader().readUTF(),
                                pId = pId
                            };
                            GameCanvas.panel.vTop.addElement(topInfo);
                        }
                        GameCanvas.panel.topName = topName;
                        GameCanvas.panel.setTypeTop(typeTop);
                        GameCanvas.panel.show();
                        break;
                    }
                case -94:
                    while (msg.reader().available() > 0)
                    {
                        short num82 = msg.reader().readShort();
                        int num83 = msg.reader().readInt();
                        for (int num84 = 0; num84 < Char.myCharz().vSkill.size(); num84++)
                        {
                            Skill skill = (Skill)Char.myCharz().vSkill.elementAt(num84);
                            if (skill != null && skill.skillId == num82)
                            {
                                if (num83 < skill.coolDown)
                                {
                                    skill.lastTimeUseThisSkill = mSystem.currentTimeMillis() - (skill.coolDown - num83);
                                }
                                Res.outz("1 chieu id= " + skill.template.id + " cooldown= " + num83 + " curr cool down= " + skill.coolDown);
                            }
                        }
                    }
                    break;
                case -95:
                    {
                        sbyte b48 = msg.reader().readByte();
                        Res.outz("type= " + b48);
                        if (b48 == 0)
                        {
                            int num117 = msg.reader().readInt();
                            short templateId = msg.reader().readShort();
                            int num118 = msg.readInt3Byte();
                            SoundMn.gI().explode_1();
                            if (num117 == Char.myCharz().charID)
                            {
                                Char.myCharz().mobMe = new Mob(num117, false, false, false, false, false, templateId, 1, num118, 0, num118, (short)(Char.myCharz().cx + ((Char.myCharz().cdir != 1) ? (-40) : 40)), (short)Char.myCharz().cy, 4, 0)
                                {
                                    isMobMe = true
                                };
                                EffecMn.addEff(new Effect(18, Char.myCharz().mobMe.x, Char.myCharz().mobMe.y, 2, 10, -1));
                                Char.myCharz().tMobMeBorn = 30;
                                GameScr.vMob.addElement(Char.myCharz().mobMe);
                            }
                            else
                            {
                                @char = GameScr.findCharInMap(num117);
                                if (@char != null)
                                {
                                    Mob mob6 = new(num117, false, false, false, false, false, templateId, 1, num118, 0, num118, (short)@char.cx, (short)@char.cy, 4, 0)
                                    {
                                        isMobMe = true
                                    };
                                    @char.mobMe = mob6;
                                    GameScr.vMob.addElement(@char.mobMe);
                                }
                                else
                                {
                                    Mob mob7 = GameScr.findMobInMap(num117);
                                    if (mob7 == null)
                                    {
                                        mob7 = new Mob(num117, false, false, false, false, false, templateId, 1, num118, 0, num118, -100, -100, 4, 0)
                                        {
                                            isMobMe = true
                                        };
                                        GameScr.vMob.addElement(mob7);
                                    }
                                }
                            }
                        }
                        if (b48 == 1)
                        {
                            int num119 = msg.reader().readInt();
                            int mobId = msg.reader().readByte();
                            Res.outz("mod attack id= " + num119);
                            if (num119 == Char.myCharz().charID)
                            {
                                if (GameScr.findMobInMap(mobId) != null)
                                {
                                    Char.myCharz().mobMe.attackOtherMob(GameScr.findMobInMap(mobId));
                                }
                            }
                            else
                            {
                                @char = GameScr.findCharInMap(num119);
                                if (@char != null && GameScr.findMobInMap(mobId) != null)
                                {
                                    @char.mobMe.attackOtherMob(GameScr.findMobInMap(mobId));
                                }
                            }
                        }
                        if (b48 == 2)
                        {
                            int num120 = msg.reader().readInt();
                            int num121 = msg.reader().readInt();
                            int num122 = msg.readInt3Byte();
                            int cHPNew = msg.readInt3Byte();
                            if (num120 == Char.myCharz().charID)
                            {
                                Res.outz("mob dame= " + num122);
                                @char = GameScr.findCharInMap(num121);
                                if (@char != null)
                                {
                                    @char.cHPNew = cHPNew;
                                    if (Char.myCharz().mobMe.isBusyAttackSomeOne)
                                    {
                                        @char.doInjure(num122, 0, false, true);
                                    }
                                    else
                                    {
                                        Char.myCharz().mobMe.dame = num122;
                                        Char.myCharz().mobMe.setAttack(@char);
                                    }
                                }
                            }
                            else
                            {
                                mob = GameScr.findMobInMap(num120);
                                if (mob != null)
                                {
                                    if (num121 == Char.myCharz().charID)
                                    {
                                        Char.myCharz().cHPNew = cHPNew;
                                        if (mob.isBusyAttackSomeOne)
                                        {
                                            Char.myCharz().doInjure(num122, 0, false, true);
                                        }
                                        else
                                        {
                                            mob.dame = num122;
                                            mob.setAttack(Char.myCharz());
                                        }
                                    }
                                    else
                                    {
                                        @char = GameScr.findCharInMap(num121);
                                        if (@char != null)
                                        {
                                            @char.cHPNew = cHPNew;
                                            if (mob.isBusyAttackSomeOne)
                                            {
                                                @char.doInjure(num122, 0, false, true);
                                            }
                                            else
                                            {
                                                mob.dame = num122;
                                                mob.setAttack(@char);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        if (b48 == 3)
                        {
                            int num123 = msg.reader().readInt();
                            int mobId2 = msg.reader().readInt();
                            int hp = msg.readInt3Byte();
                            int num124 = msg.readInt3Byte();
                            @char = null;
                            @char = (Char.myCharz().charID != num123) ? GameScr.findCharInMap(num123) : Char.myCharz();
                            if (@char != null)
                            {
                                mob = GameScr.findMobInMap(mobId2);
                                @char.mobMe?.attackOtherMob(mob);
                                if (mob != null)
                                {
                                    mob.hp = hp;
                                    mob.updateHp_bar();
                                    if (num124 == 0)
                                    {
                                        mob.x = mob.xFirst;
                                        mob.y = mob.yFirst;
                                        GameScr.startFlyText(mResources.miss, mob.x, mob.y - mob.h, 0, -2, mFont.MISS);
                                    }
                                    else
                                    {
                                        GameScr.startFlyText("-" + num124, mob.x, mob.y - mob.h, 0, -2, mFont.ORANGE);
                                    }
                                }
                            }
                        }
                        if (b48 == 4)
                        {
                        }
                        if (b48 == 5)
                        {
                            int num125 = msg.reader().readInt();
                            sbyte b49 = msg.reader().readByte();
                            int mobId3 = msg.reader().readInt();
                            int num126 = msg.readInt3Byte();
                            int hp2 = msg.readInt3Byte();
                            @char = null;
                            @char = (num125 != Char.myCharz().charID) ? GameScr.findCharInMap(num125) : Char.myCharz();
                            if (@char == null)
                            {
                                return;
                            }
                            if ((TileMap.tileTypeAtPixel(@char.cx, @char.cy) & 2) == 2)
                            {
                                @char.setSkillPaint(GameScr.sks[b49], 0);
                            }
                            else
                            {
                                @char.setSkillPaint(GameScr.sks[b49], 1);
                            }
                            Mob mob8 = GameScr.findMobInMap(mobId3);
                            @char.cdir = @char.cx <= mob8.x ? 1 : -1;
                            @char.mobFocus = mob8;
                            mob8.hp = hp2;
                            mob8.updateHp_bar();
                            GameCanvas.debug("SA83v2", 2);
                            if (num126 == 0)
                            {
                                mob8.x = mob8.xFirst;
                                mob8.y = mob8.yFirst;
                                GameScr.startFlyText(mResources.miss, mob8.x, mob8.y - mob8.h, 0, -2, mFont.MISS);
                            }
                            else
                            {
                                GameScr.startFlyText("-" + num126, mob8.x, mob8.y - mob8.h, 0, -2, mFont.ORANGE);
                            }
                        }
                        if (b48 == 6)
                        {
                            int num127 = msg.reader().readInt();
                            if (num127 == Char.myCharz().charID)
                            {
                                Char.myCharz().mobMe.startDie();
                            }
                            else
                            {
                                @char = GameScr.findCharInMap(num127);
                                @char?.mobMe.startDie();
                            }
                        }
                        if (b48 != 7)
                        {
                            break;
                        }
                        int num128 = msg.reader().readInt();
                        if (num128 == Char.myCharz().charID)
                        {
                            Char.myCharz().mobMe = null;
                            for (int num129 = 0; num129 < GameScr.vMob.size(); num129++)
                            {
                                if (((Mob)GameScr.vMob.elementAt(num129)).mobId == num128)
                                {
                                    GameScr.vMob.removeElementAt(num129);
                                }
                            }
                            break;
                        }
                        @char = GameScr.findCharInMap(num128);
                        for (int num130 = 0; num130 < GameScr.vMob.size(); num130++)
                        {
                            if (((Mob)GameScr.vMob.elementAt(num130)).mobId == num128)
                            {
                                GameScr.vMob.removeElementAt(num130);
                            }
                        }
                        if (@char != null)
                        {
                            @char.mobMe = null;
                        }
                        break;
                    }
                case -92:
                    Main.typeClient = msg.reader().readByte();
                    if (Rms.loadRMSString("ResVersion") == null)
                    {
                        Rms.clearAll();
                    }

                    Rms.saveRMSInt("clienttype", Main.typeClient);
                    Rms.saveRMSInt("lastZoomlevel", mGraphics.zoomLevel);
                    if (Rms.loadRMSString("ResVersion") == null)
                    {
                        GameCanvas.startOK(mResources.plsRestartGame, 8885, null);
                    }

                    break;
                case -91:
                    {
                        sbyte b12 = msg.reader().readByte();
                        GameCanvas.panel.mapNames = new string[b12];
                        GameCanvas.panel.planetNames = new string[b12];
                        for (int m = 0; m < b12; m++)
                        {
                            GameCanvas.panel.mapNames[m] = msg.reader().readUTF();
                            GameCanvas.panel.planetNames[m] = msg.reader().readUTF();
                        }
                        GameCanvas.panel.setTypeMapTrans();
                        GameCanvas.panel.show();
                        Pk9rXmap.showPanelMapTrans();
                        break;
                    }
                case -90:
                    {
                        sbyte b50 = msg.reader().readByte();
                        int num134 = msg.reader().readInt();
                        Res.outz("===> UPDATE_BODY:    type = " + b50);
                        @char = (Char.myCharz().charID != num134) ? GameScr.findCharInMap(num134) : Char.myCharz();
                        if (b50 != -1)
                        {
                            short num135 = msg.reader().readShort();
                            short num136 = msg.reader().readShort();
                            short num137 = msg.reader().readShort();
                            sbyte b51 = msg.reader().readByte();
                            Res.err("====> Cmd: -90 UPDATE_BODY   \n  isMonkey= " + b51 + " head=  " + num135 + " body= " + num136 + " legU= " + num137);
                            if (@char != null)
                            {
                                if (@char.charID == num134)
                                {
                                    @char.isMask = true;
                                    @char.isMonkey = b51;
                                    if (@char.isMonkey != 0)
                                    {
                                        @char.isWaitMonkey = false;
                                        @char.isLockMove = false;
                                    }
                                }
                                else if (@char != null)
                                {
                                    @char.isMask = true;
                                    @char.isMonkey = b51;
                                }
                                if (num135 != -1)
                                {
                                    @char.head = num135;
                                }
                                if (num136 != -1)
                                {
                                    @char.body = num136;
                                }
                                if (num137 != -1)
                                {
                                    @char.leg = num137;
                                }
                            }
                        }
                        if (b50 == -1 && @char != null)
                        {
                            @char.isMask = false;
                            @char.isMonkey = 0;
                        }
                        if (@char == null)
                        {
                        }
                        break;
                    }
                case -88:
                    GameCanvas.endDlg();
                    GameCanvas.serverScreen.switchToMe();
                    break;
                case -87:
                    {
                        Res.outz("GET UPDATE_DATA " + msg.reader().available() + " bytes");
                        msg.reader().mark(100000);
                        createData(msg.reader(), true);
                        msg.reader().reset();
                        sbyte[] data3 = new sbyte[msg.reader().available()];
                        msg.reader().readFully(ref data3);
                        sbyte[] data4 = new sbyte[1] { GameScr.vcData };
                        Rms.saveRMS("NRdataVersion", data4);
                        LoginScr.isUpdateData = false;
                        if (GameScr.vsData == GameScr.vcData && GameScr.vsMap == GameScr.vcMap && GameScr.vsSkill == GameScr.vcSkill && GameScr.vsItem == GameScr.vcItem)
                        {
                            Res.outz(GameScr.vsData + "," + GameScr.vsMap + "," + GameScr.vsSkill + "," + GameScr.vsItem);
                            GameScr.gI().readDart();
                            GameScr.gI().readEfect();
                            GameScr.gI().readArrow();
                            GameScr.gI().readSkill();
                            Service.gI().clientOk();
                            return;
                        }
                        break;
                    }
                case -86:
                    {
                        sbyte b21 = msg.reader().readByte();
                        Res.outz("server gui ve giao dich action = " + b21);
                        if (b21 == 0)
                        {
                            int playerID = msg.reader().readInt();
                            GameScr.gI().giaodich(playerID);
                        }
                        if (b21 == 1)
                        {
                            int num35 = msg.reader().readInt();
                            Char char5 = GameScr.findCharInMap(num35);
                            if (char5 == null)
                            {
                                return;
                            }
                            GameCanvas.panel.setTypeGiaoDich(char5);
                            GameCanvas.panel.show();
                            Service.gI().getPlayerMenu(num35);
                        }
                        if (b21 == 2)
                        {
                            sbyte b22 = msg.reader().readByte();
                            for (int num36 = 0; num36 < GameCanvas.panel.vMyGD.size(); num36++)
                            {
                                Item item = (Item)GameCanvas.panel.vMyGD.elementAt(num36);
                                if (item.indexUI == b22)
                                {
                                    GameCanvas.panel.vMyGD.removeElement(item);
                                    break;
                                }
                            }
                        }
                        if (b21 == 5)
                        {
                        }
                        if (b21 == 6)
                        {
                            GameCanvas.panel.isFriendLock = true;
                            if (GameCanvas.panel2 != null)
                            {
                                GameCanvas.panel2.isFriendLock = true;
                            }
                            GameCanvas.panel.vFriendGD.removeAllElements();
                            GameCanvas.panel2?.vFriendGD.removeAllElements();
                            int friendMoneyGD = msg.reader().readInt();
                            sbyte b23 = msg.reader().readByte();
                            Res.outz("item size = " + b23);
                            for (int num37 = 0; num37 < b23; num37++)
                            {
                                Item item2 = new()
                                {
                                    template = ItemTemplates.get(msg.reader().readShort()),
                                    quantity = msg.reader().readInt()
                                };
                                int num38 = msg.reader().readUnsignedByte();
                                if (num38 != 0)
                                {
                                    item2.itemOption = new ItemOption[num38];
                                    for (int num39 = 0; num39 < item2.itemOption.Length; num39++)
                                    {
                                        int num40 = msg.reader().readUnsignedByte();
                                        int param2 = msg.reader().readUnsignedShort();
                                        if (num40 != -1)
                                        {
                                            item2.itemOption[num39] = new ItemOption(num40, param2);
                                            item2.compare = GameCanvas.panel.getCompare(item2);
                                        }
                                    }
                                }
                                if (GameCanvas.panel2 != null)
                                {
                                    GameCanvas.panel2.vFriendGD.addElement(item2);
                                }
                                else
                                {
                                    GameCanvas.panel.vFriendGD.addElement(item2);
                                }
                            }
                            if (GameCanvas.panel2 != null)
                            {
                                GameCanvas.panel2.setTabGiaoDich(false);
                                GameCanvas.panel2.friendMoneyGD = friendMoneyGD;
                            }
                            else
                            {
                                GameCanvas.panel.friendMoneyGD = friendMoneyGD;
                                if (GameCanvas.panel.currentTabIndex == 2)
                                {
                                    GameCanvas.panel.setTabGiaoDich(false);
                                }
                            }
                        }
                        if (b21 == 7)
                        {
                            InfoDlg.hide();
                            if (GameCanvas.panel.isShow)
                            {
                                GameCanvas.panel.hide();
                            }
                        }
                        break;
                    }
                case -85:
                    {
                        Res.outz("CAP CHAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");
                        sbyte b8 = msg.reader().readByte();
                        if (b8 == 0)
                        {
                            int num12 = msg.reader().readUnsignedShort();
                            Res.outz("lent =" + num12);
                            sbyte[] data = new sbyte[num12];
                            msg.reader().read(ref data, 0, num12);
                            GameScr.imgCapcha = Image.createImage(data, 0, num12);
                            GameScr.gI().keyInput = "-----";
                            GameScr.gI().strCapcha = msg.reader().readUTF();
                            GameScr.gI().keyCapcha = new int[GameScr.gI().strCapcha.Length];
                            GameScr.gI().mobCapcha = new Mob();
                            GameScr.gI().right = null;
                        }
                        if (b8 == 1)
                        {
                            MobCapcha.isAttack = true;
                        }
                        if (b8 == 2)
                        {
                            MobCapcha.explode = true;
                            GameScr.gI().right = GameScr.gI().cmdFocus;
                        }
                        break;
                    }
                case -112:
                    {
                        sbyte b68 = msg.reader().readByte();
                        if (b68 == 0)
                        {
                            sbyte mobIndex = msg.reader().readByte();
                            GameScr.findMobInMap(mobIndex).clearBody();
                        }
                        if (b68 == 1)
                        {
                            sbyte mobIndex2 = msg.reader().readByte();
                            GameScr.findMobInMap(mobIndex2).setBody(msg.reader().readShort());
                        }
                        break;
                    }
                case -84:
                    {
                        int index2 = msg.reader().readUnsignedByte();
                        Mob mob5 = null;
                        try
                        {
                            mob5 = (Mob)GameScr.vMob.elementAt(index2);
                        }
                        catch (Exception)
                        {
                        }
                        if (mob5 != null)
                        {
                            mob5.maxHp = msg.reader().readInt();
                        }
                        break;
                    }
                case -83:
                    {
                        sbyte b38 = msg.reader().readByte();
                        if (b38 == 0)
                        {
                            int num91 = msg.reader().readShort();
                            int bgRID = msg.reader().readShort();
                            int num92 = msg.reader().readUnsignedByte();
                            int num93 = msg.reader().readInt();
                            string text3 = msg.reader().readUTF();
                            int num94 = msg.reader().readShort();
                            int num95 = msg.reader().readShort();
                            sbyte b39 = msg.reader().readByte();
                            GameScr.gI().isRongNamek = b39 == 1;
                            GameScr.gI().xR = num94;
                            GameScr.gI().yR = num95;
                            Res.outz("xR= " + num94 + " yR= " + num95 + " +++++++++++++++++++++++++++++++++++++++");
                            if (Char.myCharz().charID == num93)
                            {
                                GameCanvas.panel.hideNow();
                                GameScr.gI().activeRongThanEff(true);
                            }
                            else if (TileMap.mapID == num91 && TileMap.zoneID == num92)
                            {
                                GameScr.gI().activeRongThanEff(false);
                            }
                            else if (mGraphics.zoomLevel > 1)
                            {
                                GameScr.gI().doiMauTroi();
                            }
                            GameScr.gI().mapRID = num91;
                            GameScr.gI().bgRID = bgRID;
                            GameScr.gI().zoneRID = num92;
                        }
                        if (b38 == 1)
                        {
                            Res.outz("map RID = " + GameScr.gI().mapRID + " zone RID= " + GameScr.gI().zoneRID);
                            Res.outz("map ID = " + TileMap.mapID + " zone ID= " + TileMap.zoneID);
                            if (TileMap.mapID == GameScr.gI().mapRID && TileMap.zoneID == GameScr.gI().zoneRID)
                            {
                                GameScr.gI().hideRongThanEff();
                            }
                            else
                            {
                                GameScr.gI().isRongThanXuatHien = false;
                                if (GameScr.gI().isRongNamek)
                                {
                                    GameScr.gI().isRongNamek = false;
                                }
                            }
                        }
                        if (b38 != 2)
                        {
                        }
                        break;
                    }
                case -82:
                    {
                        sbyte b40 = msg.reader().readByte();
                        TileMap.tileIndex = new int[b40][][];
                        TileMap.tileType = new int[b40][];
                        for (int num96 = 0; num96 < b40; num96++)
                        {
                            sbyte b41 = msg.reader().readByte();
                            TileMap.tileType[num96] = new int[b41];
                            TileMap.tileIndex[num96] = new int[b41][];
                            for (int num97 = 0; num97 < b41; num97++)
                            {
                                TileMap.tileType[num96][num97] = msg.reader().readInt();
                                sbyte b42 = msg.reader().readByte();
                                TileMap.tileIndex[num96][num97] = new int[b42];
                                for (int num98 = 0; num98 < b42; num98++)
                                {
                                    TileMap.tileIndex[num96][num97][num98] = msg.reader().readByte();
                                }
                            }
                        }
                        break;
                    }
                case -81:
                    {
                        sbyte b35 = msg.reader().readByte();
                        if (b35 == 0)
                        {
                            string src = msg.reader().readUTF();
                            string src2 = msg.reader().readUTF();
                            GameCanvas.panel.setTypeCombine();
                            GameCanvas.panel.combineInfo = mFont.tahoma_7b_blue.splitFontArray(src, Panel.WIDTH_PANEL);
                            GameCanvas.panel.combineTopInfo = mFont.tahoma_7.splitFontArray(src2, Panel.WIDTH_PANEL);
                            GameCanvas.panel.show();
                        }
                        if (b35 == 1)
                        {
                            GameCanvas.panel.vItemCombine.removeAllElements();
                            sbyte b36 = msg.reader().readByte();
                            for (int num78 = 0; num78 < b36; num78++)
                            {
                                sbyte b37 = msg.reader().readByte();
                                for (int num79 = 0; num79 < Char.myCharz().arrItemBag.Length; num79++)
                                {
                                    Item item3 = Char.myCharz().arrItemBag[num79];
                                    if (item3 != null && item3.indexUI == b37)
                                    {
                                        item3.isSelect = true;
                                        GameCanvas.panel.vItemCombine.addElement(item3);
                                    }
                                }
                            }
                            if (GameCanvas.panel.isShow)
                            {
                                GameCanvas.panel.setTabCombine();
                            }
                        }
                        if (b35 == 2)
                        {
                            GameCanvas.panel.combineSuccess = 0;
                            GameCanvas.panel.setCombineEff(0);
                        }
                        if (b35 == 3)
                        {
                            GameCanvas.panel.combineSuccess = 1;
                            GameCanvas.panel.setCombineEff(0);
                        }
                        if (b35 == 4)
                        {
                            short iconID = msg.reader().readShort();
                            GameCanvas.panel.iconID3 = iconID;
                            GameCanvas.panel.combineSuccess = 0;
                            GameCanvas.panel.setCombineEff(1);
                        }
                        if (b35 == 5)
                        {
                            short iconID2 = msg.reader().readShort();
                            GameCanvas.panel.iconID3 = iconID2;
                            GameCanvas.panel.combineSuccess = 0;
                            GameCanvas.panel.setCombineEff(2);
                        }
                        if (b35 == 6)
                        {
                            short iconID3 = msg.reader().readShort();
                            short iconID4 = msg.reader().readShort();
                            GameCanvas.panel.combineSuccess = 0;
                            GameCanvas.panel.setCombineEff(3);
                            GameCanvas.panel.iconID1 = iconID3;
                            GameCanvas.panel.iconID3 = iconID4;
                        }
                        if (b35 == 7)
                        {
                            short iconID5 = msg.reader().readShort();
                            GameCanvas.panel.iconID3 = iconID5;
                            GameCanvas.panel.combineSuccess = 0;
                            GameCanvas.panel.setCombineEff(4);
                        }
                        if (b35 == 8)
                        {
                            GameCanvas.panel.iconID3 = -1;
                            GameCanvas.panel.combineSuccess = 1;
                            GameCanvas.panel.setCombineEff(4);
                        }
                        short num80 = 21;
                        try
                        {
                            num80 = msg.reader().readShort();
                        }
                        catch (Exception)
                        {
                        }
                        for (int num81 = 0; num81 < GameScr.vNpc.size(); num81++)
                        {
                            Npc npc3 = (Npc)GameScr.vNpc.elementAt(num81);
                            if (npc3.template.npcTemplateId == num80)
                            {
                                GameCanvas.panel.xS = npc3.cx - GameScr.cmx;
                                GameCanvas.panel.yS = npc3.cy - GameScr.cmy;
                                GameCanvas.panel.idNPC = num80;
                                break;
                            }
                        }
                        break;
                    }
                case -80:
                    {
                        sbyte b46 = msg.reader().readByte();
                        InfoDlg.hide();
                        if (b46 == 0)
                        {
                            GameCanvas.panel.vFriend.removeAllElements();
                            int num101 = msg.reader().readUnsignedByte();
                            for (int num102 = 0; num102 < num101; num102++)
                            {
                                Char char8 = new()
                                {
                                    charID = msg.reader().readInt(),
                                    head = msg.reader().readShort(),
                                    headICON = msg.reader().readShort(),
                                    body = msg.reader().readShort(),
                                    leg = msg.reader().readShort(),
                                    bag = msg.reader().readUnsignedByte(),
                                    cName = msg.reader().readUTF()
                                };
                                bool isOnline = msg.reader().readBoolean();
                                InfoItem infoItem = new(mResources.power + ": " + msg.reader().readUTF())
                                {
                                    charInfo = char8,
                                    isOnline = isOnline
                                };
                                GameCanvas.panel.vFriend.addElement(infoItem);
                            }
                            GameCanvas.panel.setTypeFriend();
                            GameCanvas.panel.show();
                        }
                        if (b46 == 3)
                        {
                            MyVector vFriend = GameCanvas.panel.vFriend;
                            int num103 = msg.reader().readInt();
                            Res.outz("online offline id=" + num103);
                            for (int num104 = 0; num104 < vFriend.size(); num104++)
                            {
                                InfoItem infoItem2 = (InfoItem)vFriend.elementAt(num104);
                                if (infoItem2.charInfo != null && infoItem2.charInfo.charID == num103)
                                {
                                    Res.outz("online= " + infoItem2.isOnline);
                                    infoItem2.isOnline = msg.reader().readBoolean();
                                    break;
                                }
                            }
                        }
                        if (b46 != 2)
                        {
                            break;
                        }
                        MyVector vFriend2 = GameCanvas.panel.vFriend;
                        int num105 = msg.reader().readInt();
                        for (int num106 = 0; num106 < vFriend2.size(); num106++)
                        {
                            InfoItem infoItem3 = (InfoItem)vFriend2.elementAt(num106);
                            if (infoItem3.charInfo != null && infoItem3.charInfo.charID == num105)
                            {
                                vFriend2.removeElement(infoItem3);
                                break;
                            }
                        }
                        if (GameCanvas.panel.isShow)
                        {
                            GameCanvas.panel.setTabFriend();
                        }
                        break;
                    }
                case -99:
                    {
                        InfoDlg.hide();
                        sbyte b47 = msg.reader().readByte();
                        if (b47 == 0)
                        {
                            GameCanvas.panel.vEnemy.removeAllElements();
                            int num113 = msg.reader().readUnsignedByte();
                            for (int num114 = 0; num114 < num113; num114++)
                            {
                                Char char9 = new()
                                {
                                    charID = msg.reader().readInt(),
                                    head = msg.reader().readShort(),
                                    headICON = msg.reader().readShort(),
                                    body = msg.reader().readShort(),
                                    leg = msg.reader().readShort(),
                                    bag = msg.reader().readShort(),
                                    cName = msg.reader().readUTF()
                                };
                                InfoItem infoItem4 = new(msg.reader().readUTF());
                                bool flag8 = msg.reader().readBoolean();
                                infoItem4.charInfo = char9;
                                infoItem4.isOnline = flag8;
                                Res.outz("isonline = " + flag8);
                                GameCanvas.panel.vEnemy.addElement(infoItem4);
                            }
                            GameCanvas.panel.setTypeEnemy();
                            GameCanvas.panel.show();
                        }
                        break;
                    }
                case -79:
                    {
                        InfoDlg.hide();
                        int num108 = msg.reader().readInt();
                        Char charMenu = GameCanvas.panel.charMenu;
                        if (charMenu == null)
                        {
                            return;
                        }
                        charMenu.cPower = msg.reader().readLong();
                        charMenu.currStrLevel = msg.reader().readUTF();
                        break;
                    }
                case -93:
                    {
                        short num109 = msg.reader().readShort();
                        BgItem.newSmallVersion = new sbyte[num109];
                        for (int num110 = 0; num110 < num109; num110++)
                        {
                            BgItem.newSmallVersion[num110] = msg.reader().readByte();
                        }
                        break;
                    }
                case -77:
                    {
                        short num89 = msg.reader().readShort();
                        SmallImage.newSmallVersion = new sbyte[num89];
                        SmallImage.maxSmall = num89;
                        SmallImage.imgNew = new Small[num89];
                        for (int num90 = 0; num90 < num89; num90++)
                        {
                            SmallImage.newSmallVersion[num90] = msg.reader().readByte();
                        }
                        break;
                    }
                case -76:
                    {
                        sbyte b60 = msg.reader().readByte();
                        if (b60 == 0)
                        {
                            sbyte b61 = msg.reader().readByte();
                            if (b61 <= 0)
                            {
                                return;
                            }

                            Char.myCharz().arrArchive = new Archivement[b61];
                            for (int num154 = 0; num154 < b61; num154++)
                            {
                                Char.myCharz().arrArchive[num154] = new Archivement
                                {
                                    info1 = num154 + 1 + ". " + msg.reader().readUTF(),
                                    info2 = msg.reader().readUTF(),
                                    money = msg.reader().readShort(),
                                    isFinish = msg.reader().readBoolean(),
                                    isRecieve = msg.reader().readBoolean()
                                };
                            }
                            GameCanvas.panel.setTypeArchivement();
                            GameCanvas.panel.show();
                        }
                        else if (b60 == 1)
                        {
                            int num155 = msg.reader().readUnsignedByte();
                            if (Char.myCharz().arrArchive[num155] != null)
                            {
                                Char.myCharz().arrArchive[num155].isRecieve = true;
                            }
                        }
                        break;
                    }
                case -74:
                    {
                        if (ServerListScreen.stopDownload)
                        {
                            return;
                        }
                        if (!GameCanvas.isGetResourceFromServer())
                        {
                            Service.gI().getResource(3, null);
                            SmallImage.loadBigRMS();
                            SplashScr.imgLogo = null;
                            if (Rms.loadRMSString("acc") != null || Rms.loadRMSString("userAo" + ServerListScreen.ipSelect) != null)
                            {
                                LoginScr.isContinueToLogin = true;
                            }
                            GameCanvas.loginScr = new LoginScr();
                            GameCanvas.loginScr.switchToMe();
                            return;
                        }
                        bool flag5 = true;
                        sbyte b25 = msg.reader().readByte();
                        Res.outz("action = " + b25);
                        if (b25 == 0)
                        {
                            int num42 = msg.reader().readInt();
                            string text2 = Rms.loadRMSString("ResVersion");
                            int num43 = (text2 == null || !(text2 != string.Empty)) ? (-1) : int.Parse(text2);
                            if (num43 == -1 || num43 != num42)
                            {
                                ServerListScreen.loadScreen = false;
                                GameCanvas.serverScreen.show2();
                            }
                            else
                            {
                                Res.outz("login ngay");
                                SmallImage.loadBigRMS();
                                SplashScr.imgLogo = null;
                                ServerListScreen.loadScreen = true;
                                if (GameCanvas.currentScreen != GameCanvas.loginScr)
                                {
                                    GameCanvas.serverScreen.switchToMe();
                                }
                            }
                        }
                        if (b25 == 1)
                        {
                            ServerListScreen.strWait = mResources.downloading_data;
                            short nBig = msg.reader().readShort();
                            ServerListScreen.nBig = nBig;
                            Service.gI().getResource(2, null);
                        }
                        if (b25 == 2)
                        {
                            try
                            {
                                isLoadingData = true;
                                GameCanvas.endDlg();
                                ServerListScreen.demPercent++;
                                ServerListScreen.percent = ServerListScreen.demPercent * 100 / ServerListScreen.nBig;
                                string original = msg.reader().readUTF();
                                string[] array4 = Res.split(original, "/", 0);
                                string filename = "x" + mGraphics.zoomLevel + array4[^1];
                                int num44 = msg.reader().readInt();
                                sbyte[] data2 = new sbyte[num44];
                                msg.reader().read(ref data2, 0, num44);
                                Rms.saveRMS(filename, data2);
                            }
                            catch (Exception)
                            {
                                GameCanvas.startOK(mResources.pls_restart_game_error, 8885, null);
                            }
                        }
                        if (b25 == 3 && flag5)
                        {
                            isLoadingData = false;
                            int num45 = msg.reader().readInt();
                            Res.outz("last version= " + num45);
                            Rms.saveRMSString("ResVersion", num45 + string.Empty);
                            Service.gI().getResource(3, null);
                            GameCanvas.endDlg();
                            SplashScr.imgLogo = null;
                            SmallImage.loadBigRMS();
                            mSystem.gcc();
                            ServerListScreen.bigOk = true;
                            ServerListScreen.loadScreen = true;
                            GameScr.gI().loadGameScr();
                            if (GameCanvas.currentScreen != GameCanvas.loginScr)
                            {
                                GameCanvas.serverScreen.switchToMe();
                            }
                        }
                        break;
                    }
                case -43:
                    {
                        sbyte itemAction = msg.reader().readByte();
                        sbyte where = msg.reader().readByte();
                        sbyte index = msg.reader().readByte();
                        string info3 = msg.reader().readUTF();
                        GameCanvas.panel.itemRequest(itemAction, info3, where, index);
                        break;
                    }
                case -59:
                    {
                        sbyte typePK = msg.reader().readByte();
                        GameScr.gI().player_vs_player(msg.reader().readInt(), msg.reader().readInt(), msg.reader().readUTF(), typePK);
                        break;
                    }
                case -62:
                    {
                        int num21 = msg.reader().readUnsignedByte();
                        sbyte b16 = msg.reader().readByte();
                        if (b16 <= 0)
                        {
                            break;
                        }
                        ClanImage clanImage2 = ClanImage.getClanImage((short)num21);
                        if (clanImage2 == null)
                        {
                            break;
                        }
                        clanImage2.idImage = new short[b16];
                        for (int num22 = 0; num22 < b16; num22++)
                        {
                            clanImage2.idImage[num22] = msg.reader().readShort();
                            if (clanImage2.idImage[num22] > 0)
                            {
                                SmallImage.vKeys.addElement(clanImage2.idImage[num22] + string.Empty);
                            }
                        }
                        break;
                    }
                case -65:
                    {
                        Res.outz("TELEPORT ...................................................");
                        InfoDlg.hide();
                        int num100 = msg.reader().readInt();
                        sbyte b44 = msg.reader().readByte();
                        if (b44 == 0)
                        {
                            break;
                        }
                        if (Char.myCharz().charID == num100)
                        {
                            isStopReadMessage = true;
                            GameScr.lockTick = 500;
                            GameScr.gI().center = null;
                            if (b44 is 0 or 1 or 3)
                            {
                                Teleport p2 = new(Char.myCharz().cx, Char.myCharz().cy, Char.myCharz().head, Char.myCharz().cdir, 0, true, (b44 != 1) ? b44 : Char.myCharz().cgender);
                                Teleport.addTeleport(p2);
                            }
                            if (b44 == 2)
                            {
                                GameScr.lockTick = 50;
                                Char.myCharz().hide();
                            }
                        }
                        else
                        {
                            Char char6 = GameScr.findCharInMap(num100);
                            if ((b44 == 0 || b44 == 1 || b44 == 3) && char6 != null)
                            {
                                char6.isUsePlane = true;
                                Teleport teleport = new(char6.cx, char6.cy, char6.head, char6.cdir, 0, false, (b44 != 1) ? b44 : char6.cgender)
                                {
                                    id = num100
                                };
                                Teleport.addTeleport(teleport);
                            }
                            if (b44 == 2)
                            {
                                char6.hide();
                            }
                        }
                        break;
                    }
                case -64:
                    {
                        int num24 = msg.reader().readInt();
                        int num25 = msg.reader().readUnsignedByte();
                        @char = null;
                        @char = (num24 != Char.myCharz().charID) ? GameScr.findCharInMap(num24) : Char.myCharz();
                        @char.bag = num25;
                        if (@char.bag is >= 201 and < 255)
                        {
                            Effect effect = new(@char.bag, @char, 2, -1, 10, 1)
                            {
                                typeEff = 5
                            };
                            @char.addEffChar(effect);
                        }
                        else
                        {
                            for (int num26 = 0; num26 < 54; num26++)
                            {
                                @char.removeEffChar(0, 201 + num26);
                            }
                        }
                        Res.outz("cmd:-64 UPDATE BAG PLAER = " + ((@char != null) ? @char.cName : string.Empty) + num24 + " BAG ID= " + num25);
                        break;
                    }
                case -63:
                    {
                        Res.outz("GET BAG");
                        int num11 = msg.reader().readUnsignedByte();
                        sbyte b7 = msg.reader().readByte();
                        ClanImage clanImage = new()
                        {
                            ID = num11
                        };
                        if (b7 > 0)
                        {
                            clanImage.idImage = new short[b7];
                            for (int j = 0; j < b7; j++)
                            {
                                clanImage.idImage[j] = msg.reader().readShort();
                                Res.outz("ID=  " + num11 + " frame= " + clanImage.idImage[j]);
                            }
                            ClanImage.idImages.put(num11 + string.Empty, clanImage);
                        }
                        break;
                    }
                case -57:
                    {
                        string strInvite = msg.reader().readUTF();
                        int clanID = msg.reader().readInt();
                        int code = msg.reader().readInt();
                        GameScr.gI().clanInvite(strInvite, clanID, code);
                        break;
                    }
                case -51:
                    InfoDlg.hide();
                    readClanMsg(msg, 0);
                    if (GameCanvas.panel.isMessage && GameCanvas.panel.type == 5)
                    {
                        GameCanvas.panel.initTabClans();
                    }
                    break;
                case -53:
                    {
                        InfoDlg.hide();
                        bool flag7 = false;
                        int num85 = msg.reader().readInt();
                        Res.outz("clanId= " + num85);
                        if (num85 == -1)
                        {
                            flag7 = true;
                            Char.myCharz().clan = null;
                            ClanMessage.vMessage.removeAllElements();
                            GameCanvas.panel.member?.removeAllElements();
                            GameCanvas.panel.myMember?.removeAllElements();
                            if (GameCanvas.currentScreen == GameScr.gI())
                            {
                                GameCanvas.panel.setTabClans();
                            }
                            return;
                        }
                        GameCanvas.panel.tabIcon = null;
                        if (Char.myCharz().clan == null)
                        {
                            Char.myCharz().clan = new Clan();
                        }
                        Char.myCharz().clan.ID = num85;
                        Char.myCharz().clan.name = msg.reader().readUTF();
                        Char.myCharz().clan.slogan = msg.reader().readUTF();
                        Char.myCharz().clan.imgID = msg.reader().readUnsignedByte();
                        Char.myCharz().clan.powerPoint = msg.reader().readUTF();
                        Char.myCharz().clan.leaderName = msg.reader().readUTF();
                        Char.myCharz().clan.currMember = msg.reader().readUnsignedByte();
                        Char.myCharz().clan.maxMember = msg.reader().readUnsignedByte();
                        Char.myCharz().role = msg.reader().readByte();
                        Char.myCharz().clan.clanPoint = msg.reader().readInt();
                        Char.myCharz().clan.level = msg.reader().readByte();
                        GameCanvas.panel.myMember = new MyVector();
                        for (int num86 = 0; num86 < Char.myCharz().clan.currMember; num86++)
                        {
                            Member member5 = new()
                            {
                                ID = msg.reader().readInt(),
                                head = msg.reader().readShort(),
                                headICON = msg.reader().readShort(),
                                leg = msg.reader().readShort(),
                                body = msg.reader().readShort(),
                                name = msg.reader().readUTF(),
                                role = msg.reader().readByte(),
                                powerPoint = msg.reader().readUTF(),
                                donate = msg.reader().readInt(),
                                receive_donate = msg.reader().readInt(),
                                clanPoint = msg.reader().readInt(),
                                curClanPoint = msg.reader().readInt(),
                                joinTime = NinjaUtil.getDate(msg.reader().readInt())
                            };
                            GameCanvas.panel.myMember.addElement(member5);
                        }
                        int num87 = msg.reader().readUnsignedByte();
                        for (int num88 = 0; num88 < num87; num88++)
                        {
                            readClanMsg(msg, -1);
                        }
                        if (GameCanvas.panel.isSearchClan || GameCanvas.panel.isViewMember || GameCanvas.panel.isMessage)
                        {
                            GameCanvas.panel.setTabClans();
                        }
                        if (flag7)
                        {
                            GameCanvas.panel.setTabClans();
                        }
                        Res.outz("=>>>>>>>>>>>>>>>>>>>>>> -537 MY CLAN INFO");
                        break;
                    }
                case -52:
                    {
                        sbyte b19 = msg.reader().readByte();
                        if (b19 == 0)
                        {
                            Member member2 = new()
                            {
                                ID = msg.reader().readInt(),
                                head = msg.reader().readShort(),
                                headICON = msg.reader().readShort(),
                                leg = msg.reader().readShort(),
                                body = msg.reader().readShort(),
                                name = msg.reader().readUTF(),
                                role = msg.reader().readByte(),
                                powerPoint = msg.reader().readUTF(),
                                donate = msg.reader().readInt(),
                                receive_donate = msg.reader().readInt(),
                                clanPoint = msg.reader().readInt(),
                                joinTime = NinjaUtil.getDate(msg.reader().readInt())
                            };
                            GameCanvas.panel.myMember ??= new MyVector();
                            GameCanvas.panel.myMember.addElement(member2);
                            GameCanvas.panel.initTabClans();
                        }
                        if (b19 == 1)
                        {
                            GameCanvas.panel.myMember.removeElementAt(msg.reader().readByte());
                            GameCanvas.panel.currentListLength--;
                            GameCanvas.panel.initTabClans();
                        }
                        if (b19 == 2)
                        {
                            Member member3 = new()
                            {
                                ID = msg.reader().readInt(),
                                head = msg.reader().readShort(),
                                headICON = msg.reader().readShort(),
                                leg = msg.reader().readShort(),
                                body = msg.reader().readShort(),
                                name = msg.reader().readUTF(),
                                role = msg.reader().readByte(),
                                powerPoint = msg.reader().readUTF(),
                                donate = msg.reader().readInt(),
                                receive_donate = msg.reader().readInt(),
                                clanPoint = msg.reader().readInt(),
                                joinTime = NinjaUtil.getDate(msg.reader().readInt())
                            };
                            for (int num27 = 0; num27 < GameCanvas.panel.myMember.size(); num27++)
                            {
                                Member member4 = (Member)GameCanvas.panel.myMember.elementAt(num27);
                                if (member4.ID == member3.ID)
                                {
                                    if (Char.myCharz().charID == member3.ID)
                                    {
                                        Char.myCharz().role = member3.role;
                                    }
                                    Member o = member3;
                                    GameCanvas.panel.myMember.removeElement(member4);
                                    GameCanvas.panel.myMember.insertElementAt(o, num27);
                                    return;
                                }
                            }
                        }
                        Res.outz("=>>>>>>>>>>>>>>>>>>>>>> -52  MY CLAN UPDSTE");
                        break;
                    }
                case -50:
                    {
                        InfoDlg.hide();
                        GameCanvas.panel.member = new MyVector();
                        sbyte b15 = msg.reader().readByte();
                        for (int num20 = 0; num20 < b15; num20++)
                        {
                            Member member = new()
                            {
                                ID = msg.reader().readInt(),
                                head = msg.reader().readShort(),
                                headICON = msg.reader().readShort(),
                                leg = msg.reader().readShort(),
                                body = msg.reader().readShort(),
                                name = msg.reader().readUTF(),
                                role = msg.reader().readByte(),
                                powerPoint = msg.reader().readUTF(),
                                donate = msg.reader().readInt(),
                                receive_donate = msg.reader().readInt(),
                                clanPoint = msg.reader().readInt(),
                                joinTime = NinjaUtil.getDate(msg.reader().readInt())
                            };
                            GameCanvas.panel.member.addElement(member);
                        }
                        GameCanvas.panel.isViewMember = true;
                        GameCanvas.panel.isSearchClan = false;
                        GameCanvas.panel.isMessage = false;
                        GameCanvas.panel.currentListLength = GameCanvas.panel.member.size() + 2;
                        GameCanvas.panel.initTabClans();
                        break;
                    }
                case -47:
                    {
                        InfoDlg.hide();
                        sbyte b6 = msg.reader().readByte();
                        Res.outz("clan = " + b6);
                        if (b6 == 0)
                        {
                            GameCanvas.panel.clanReport = mResources.cannot_find_clan;
                            GameCanvas.panel.clans = null;
                        }
                        else
                        {
                            GameCanvas.panel.clans = new Clan[b6];
                            Res.outz("clan search lent= " + GameCanvas.panel.clans.Length);
                            for (int i = 0; i < GameCanvas.panel.clans.Length; i++)
                            {
                                GameCanvas.panel.clans[i] = new Clan
                                {
                                    ID = msg.reader().readInt(),
                                    name = msg.reader().readUTF(),
                                    slogan = msg.reader().readUTF(),
                                    imgID = msg.reader().readUnsignedByte(),
                                    powerPoint = msg.reader().readUTF(),
                                    leaderName = msg.reader().readUTF(),
                                    currMember = msg.reader().readUnsignedByte(),
                                    maxMember = msg.reader().readUnsignedByte(),
                                    date = msg.reader().readInt()
                                };
                            }
                        }
                        GameCanvas.panel.isSearchClan = true;
                        GameCanvas.panel.isViewMember = false;
                        GameCanvas.panel.isMessage = false;
                        if (GameCanvas.panel.isSearchClan)
                        {
                            GameCanvas.panel.initTabClans();
                        }
                        break;
                    }
                case -46:
                    {
                        InfoDlg.hide();
                        sbyte b65 = msg.reader().readByte();
                        if (b65 is 1 or 3)
                        {
                            GameCanvas.endDlg();
                            ClanImage.vClanImage.removeAllElements();
                            int num157 = msg.reader().readUnsignedByte();
                            for (int num158 = 0; num158 < num157; num158++)
                            {
                                ClanImage clanImage3 = new()
                                {
                                    ID = msg.reader().readUnsignedByte(),
                                    name = msg.reader().readUTF(),
                                    xu = msg.reader().readInt(),
                                    luong = msg.reader().readInt()
                                };
                                if (!ClanImage.isExistClanImage(clanImage3.ID))
                                {
                                    ClanImage.addClanImage(clanImage3);
                                    continue;
                                }
                                ClanImage.getClanImage((short)clanImage3.ID).name = clanImage3.name;
                                ClanImage.getClanImage((short)clanImage3.ID).xu = clanImage3.xu;
                                ClanImage.getClanImage((short)clanImage3.ID).luong = clanImage3.luong;
                            }
                            if (Char.myCharz().clan != null)
                            {
                                GameCanvas.panel.changeIcon();
                            }
                        }
                        if (b65 == 4)
                        {
                            Char.myCharz().clan.imgID = msg.reader().readUnsignedByte();
                            Char.myCharz().clan.slogan = msg.reader().readUTF();
                        }
                        break;
                    }
                case -61:
                    {
                        int num144 = msg.reader().readInt();
                        if (num144 != Char.myCharz().charID)
                        {
                            if (GameScr.findCharInMap(num144) != null)
                            {
                                GameScr.findCharInMap(num144).clanID = msg.reader().readInt();
                                if (GameScr.findCharInMap(num144).clanID == -2)
                                {
                                    GameScr.findCharInMap(num144).isCopy = true;
                                }
                            }
                        }
                        else if (Char.myCharz().clan != null)
                        {
                            Char.myCharz().clan.ID = msg.reader().readInt();
                        }
                        break;
                    }
                case -42:
                    Char.myCharz().cHPGoc = msg.readInt3Byte();
                    Char.myCharz().cMPGoc = msg.readInt3Byte();
                    Char.myCharz().cDamGoc = msg.reader().readInt();
                    Char.myCharz().cHPFull = msg.readInt3Byte();
                    Char.myCharz().cMPFull = msg.readInt3Byte();
                    Char.myCharz().cHP = msg.readInt3Byte();
                    Char.myCharz().cMP = msg.readInt3Byte();
                    Char.myCharz().cspeed = msg.reader().readByte();
                    Char.myCharz().hpFrom1000TiemNang = msg.reader().readByte();
                    Char.myCharz().mpFrom1000TiemNang = msg.reader().readByte();
                    Char.myCharz().damFrom1000TiemNang = msg.reader().readByte();
                    Char.myCharz().cDamFull = msg.reader().readInt();
                    Char.myCharz().cDefull = msg.reader().readInt();
                    Char.myCharz().cCriticalFull = msg.reader().readByte();
                    Char.myCharz().cTiemNang = msg.reader().readLong();
                    Char.myCharz().expForOneAdd = msg.reader().readShort();
                    Char.myCharz().cDefGoc = msg.reader().readShort();
                    Char.myCharz().cCriticalGoc = msg.reader().readByte();

                    Char.myCharz().cGiamST = msg.reader().readByte();
                    Char.myCharz().cCritDameFull = msg.reader().readShort(); InfoDlg.hide();
                    break;
                case 1:
                    {
                        bool flag9 = msg.reader().readBool();
                        Res.outz("isRes= " + flag9);
                        if (!flag9)
                        {
                            GameCanvas.startOKDlg(msg.reader().readUTF());
                            break;
                        }
                        GameCanvas.loginScr.isLogin2 = false;
                        Rms.saveRMSString("userAo" + ServerListScreen.ipSelect, string.Empty);
                        GameCanvas.endDlg();
                        GameCanvas.loginScr.doLogin();
                        break;
                    }
                case 2:
                    Char.isLoadingMap = true;
                    LoginScr.isLoggingIn = false;
                    if (!GameScr.isLoadAllData)
                    {
                        GameScr.gI().initSelectChar();
                    }
                    BgItem.clearHashTable();
                    GameCanvas.endDlg();
                    CreateCharScr.isCreateChar = true;
                    CreateCharScr.gI().switchToMe();
                    break;
                case -107:
                    {
                        sbyte b31 = msg.reader().readByte();
                        if (b31 == 0)
                        {
                            Char.myCharz().havePet = false;
                        }
                        if (b31 == 1)
                        {
                            Char.myCharz().havePet = true;
                        }
                        if (b31 != 2)
                        {
                            break;
                        }
                        InfoDlg.hide();
                        Char.myPetz().head = msg.reader().readShort();
                        Char.myPetz().setDefaultPart();
                        int num56 = msg.reader().readUnsignedByte();
                        Res.outz("num body = " + num56);
                        Char.myPetz().arrItemBody = new Item[num56];
                        for (int num57 = 0; num57 < num56; num57++)
                        {
                            short num58 = msg.reader().readShort();
                            Res.outz("template id= " + num58);
                            if (num58 == -1)
                            {
                                continue;
                            }
                            Res.outz("1");
                            Char.myPetz().arrItemBody[num57] = new Item
                            {
                                template = ItemTemplates.get(num58)
                            };
                            int num59 = Char.myPetz().arrItemBody[num57].template.type;
                            Char.myPetz().arrItemBody[num57].quantity = msg.reader().readInt();
                            Res.outz("3");
                            Char.myPetz().arrItemBody[num57].info = msg.reader().readUTF();
                            Char.myPetz().arrItemBody[num57].content = msg.reader().readUTF();
                            int num60 = msg.reader().readUnsignedByte();
                            Res.outz("option size= " + num60);
                            if (num60 != 0)
                            {
                                Char.myPetz().arrItemBody[num57].itemOption = new ItemOption[num60];
                                for (int num61 = 0; num61 < Char.myPetz().arrItemBody[num57].itemOption.Length; num61++)
                                {
                                    int num62 = msg.reader().readUnsignedByte();
                                    int param4 = msg.reader().readUnsignedShort();
                                    if (num62 != -1)
                                    {
                                        Char.myPetz().arrItemBody[num57].itemOption[num61] = new ItemOption(num62, param4);
                                    }
                                }
                            }
                            switch (num59)
                            {
                                case 0:
                                    Char.myPetz().body = Char.myPetz().arrItemBody[num57].template.part;
                                    break;
                                case 1:
                                    Char.myPetz().leg = Char.myPetz().arrItemBody[num57].template.part;
                                    break;
                            }
                        }
                        Char.myPetz().cHP = msg.readInt3Byte();
                        Char.myPetz().cHPFull = msg.readInt3Byte();
                        Char.myPetz().cMP = msg.readInt3Byte();
                        Char.myPetz().cMPFull = msg.readInt3Byte();
                        Char.myPetz().cDamFull = msg.readInt3Byte();
                        Char.myPetz().cName = msg.reader().readUTF();
                        Char.myPetz().currStrLevel = msg.reader().readUTF();
                        Char.myPetz().cPower = msg.reader().readLong();
                        Char.myPetz().cTiemNang = msg.reader().readLong();
                        Char.myPetz().petStatus = msg.reader().readByte();
                        Char.myPetz().cStamina = msg.reader().readShort();
                        Char.myPetz().cMaxStamina = msg.reader().readShort();
                        Char.myPetz().cCriticalFull = msg.reader().readByte();
                        Char.myPetz().cDefull = msg.reader().readShort();
                        Char.myPetz().arrPetSkill = new Skill[msg.reader().readByte()];
                        Res.outz("SKILLENT = " + Char.myPetz().arrPetSkill);
                        for (int num63 = 0; num63 < Char.myPetz().arrPetSkill.Length; num63++)
                        {
                            short num64 = msg.reader().readShort();
                            if (num64 != -1)
                            {
                                Char.myPetz().arrPetSkill[num63] = Skills.get(num64);
                                continue;
                            }
                            Char.myPetz().arrPetSkill[num63] = new Skill
                            {
                                template = null,
                                moreInfo = msg.reader().readUTF()
                            };
                        }
                        break;
                    }
                case -37:
                    {
                        sbyte b16 = msg.reader().readByte();
                        Res.outz("cAction= " + b16);
                        if (b16 != 0)
                        {
                            break;
                        }
                        Char.myCharz().head = msg.reader().readShort();
                        Char.myCharz().setDefaultPart();
                        int num30 = msg.reader().readUnsignedByte();
                        Res.outz("num body = " + num30);
                        Char.myCharz().arrItemBody = new Item[num30];
                        for (int num31 = 0; num31 < num30; num31++)
                        {
                            short num32 = msg.reader().readShort();
                            if (num32 == -1)
                            {
                                continue;
                            }
                            Char.myCharz().arrItemBody[num31] = new Item
                            {
                                template = ItemTemplates.get(num32)
                            };
                            int num33 = Char.myCharz().arrItemBody[num31].template.type;
                            Char.myCharz().arrItemBody[num31].quantity = msg.reader().readInt();
                            Char.myCharz().arrItemBody[num31].info = msg.reader().readUTF();
                            Char.myCharz().arrItemBody[num31].content = msg.reader().readUTF();
                            int num34 = msg.reader().readUnsignedByte();
                            if (num34 != 0)
                            {
                                Char.myCharz().arrItemBody[num31].itemOption = new ItemOption[num34];
                                for (int num35 = 0; num35 < Char.myCharz().arrItemBody[num31].itemOption.Length; num35++)
                                {
                                    int num36 = msg.reader().readUnsignedByte();
                                    int param2 = msg.reader().readUnsignedShort();
                                    if (num36 != -1)
                                    {
                                        Char.myCharz().arrItemBody[num31].itemOption[num35] = new ItemOption(num36, param2);
                                    }
                                }
                            }
                            switch (num33)
                            {
                                case 0:
                                    Char.myCharz().body = Char.myCharz().arrItemBody[num31].template.part;
                                    break;
                                case 1:
                                    Char.myCharz().leg = Char.myCharz().arrItemBody[num31].template.part;
                                    break;
                            }
                        }
                        break;
                    }
                case -36:
                    {
                        sbyte b26 = msg.reader().readByte();
                        Res.outz("cAction= " + b26);
                        if (b26 == 0)
                        {
                            int num46 = msg.reader().readUnsignedByte();
                            Char.myCharz().arrItemBag = new Item[num46];
                            GameScr.hpPotion = 0;
                            Res.outz("numC=" + num46);
                            for (int num47 = 0; num47 < num46; num47++)
                            {
                                short num48 = msg.reader().readShort();
                                if (num48 == -1)
                                {
                                    continue;
                                }
                                Char.myCharz().arrItemBag[num47] = new Item
                                {
                                    template = ItemTemplates.get(num48),
                                    quantity = msg.reader().readInt(),
                                    info = msg.reader().readUTF(),
                                    content = msg.reader().readUTF(),
                                    indexUI = num47
                                };
                                int num49 = msg.reader().readUnsignedByte();
                                if (num49 != 0)
                                {
                                    Char.myCharz().arrItemBag[num47].itemOption = new ItemOption[num49];
                                    for (int num50 = 0; num50 < Char.myCharz().arrItemBag[num47].itemOption.Length; num50++)
                                    {
                                        int num51 = msg.reader().readUnsignedByte();
                                        int param3 = msg.reader().readUnsignedShort();
                                        if (num51 != -1)
                                        {
                                            Char.myCharz().arrItemBag[num47].itemOption[num50] = new ItemOption(num51, param3);
                                        }
                                    }
                                    Char.myCharz().arrItemBag[num47].compare = GameCanvas.panel.getCompare(Char.myCharz().arrItemBag[num47]);
                                }
                                if (Char.myCharz().arrItemBag[num47].template.type == 11)
                                {
                                }
                                if (Char.myCharz().arrItemBag[num47].template.type == 6)
                                {
                                    GameScr.hpPotion += Char.myCharz().arrItemBag[num47].quantity;
                                }
                            }
                        }
                        if (b26 == 2)
                        {
                            sbyte b27 = msg.reader().readByte();
                            int quantity = msg.reader().readInt();
                            int quantity2 = Char.myCharz().arrItemBag[b27].quantity;
                            Char.myCharz().arrItemBag[b27].quantity = quantity;
                            if (Char.myCharz().arrItemBag[b27].quantity < quantity2 && Char.myCharz().arrItemBag[b27].template.type == 6)
                            {
                                GameScr.hpPotion -= quantity2 - Char.myCharz().arrItemBag[b27].quantity;
                            }
                            if (Char.myCharz().arrItemBag[b27].quantity == 0)
                            {
                                Char.myCharz().arrItemBag[b27] = null;
                            }
                        }
                        break;
                    }
                case -35:
                    {
                        sbyte b69 = msg.reader().readByte();
                        Res.outz("cAction= " + b69);
                        if (b69 == 0)
                        {
                            int num165 = msg.reader().readUnsignedByte();
                            Char.myCharz().arrItemBox = new Item[num165];
                            GameCanvas.panel.hasUse = 0;
                            for (int num166 = 0; num166 < num165; num166++)
                            {
                                short num167 = msg.reader().readShort();
                                if (num167 == -1)
                                {
                                    continue;
                                }
                                Char.myCharz().arrItemBox[num166] = new Item
                                {
                                    template = ItemTemplates.get(num167),
                                    quantity = msg.reader().readInt(),
                                    info = msg.reader().readUTF(),
                                    content = msg.reader().readUTF()
                                };
                                int num168 = msg.reader().readUnsignedByte();
                                if (num168 != 0)
                                {
                                    Char.myCharz().arrItemBox[num166].itemOption = new ItemOption[num168];
                                    for (int num169 = 0; num169 < Char.myCharz().arrItemBox[num166].itemOption.Length; num169++)
                                    {
                                        int num170 = msg.reader().readUnsignedByte();
                                        int param6 = msg.reader().readUnsignedShort();
                                        if (num170 != -1)
                                        {
                                            Char.myCharz().arrItemBox[num166].itemOption[num169] = new ItemOption(num170, param6);
                                        }
                                    }
                                }
                                GameCanvas.panel.hasUse++;
                            }
                        }
                        if (b69 == 1)
                        {
                            bool isBoxClan = false;
                            try
                            {
                                sbyte b70 = msg.reader().readByte();
                                if (b70 == 1)
                                {
                                    isBoxClan = true;
                                }
                            }
                            catch (Exception)
                            {
                            }
                            GameCanvas.panel.setTypeBox();
                            GameCanvas.panel.isBoxClan = isBoxClan;
                            GameCanvas.panel.show();
                        }
                        if (b69 == 2)
                        {
                            sbyte b71 = msg.reader().readByte();
                            int quantity3 = msg.reader().readInt();
                            Char.myCharz().arrItemBox[b71].quantity = quantity3;
                            if (Char.myCharz().arrItemBox[b71].quantity == 0)
                            {
                                Char.myCharz().arrItemBox[b71] = null;
                            }
                        }
                        break;
                    }
                case -45:
                    {
                        sbyte b53 = msg.reader().readByte();
                        int num145 = msg.reader().readInt();
                        short num146 = msg.reader().readShort();
                        Res.outz(">.SKILL_NOT_FOCUS  skill type= " + b53 + "   player use= " + num145);
                        if (b53 == 20)
                        {
                            sbyte typeFrame = msg.reader().readByte();
                            sbyte b54 = msg.reader().readByte();
                            short timeGong = msg.reader().readShort();
                            bool isFly = msg.reader().readByte() != 0;
                            sbyte typePaint = msg.reader().readByte();
                            sbyte typeItem = -1;
                            try
                            {
                                typeItem = msg.reader().readByte();
                            }
                            catch (Exception)
                            {
                            }
                            Res.outz(">.SKILL_NOT_FOCUS  skill playerDir= " + b54);
                            @char = (Char.myCharz().charID != num145) ? GameScr.findCharInMap(num145) : Char.myCharz();
                            @char.SetSkillPaint_NEW(num146, isFly, typeFrame, typePaint, b54, timeGong, typeItem);
                        }
                        if (b53 == 21)
                        {
                            Point point = new()
                            {
                                x = msg.reader().readShort(),
                                y = msg.reader().readShort()
                            };
                            short timeDame = msg.reader().readShort();
                            short rangeDame = msg.reader().readShort();
                            sbyte typePaint2 = 0;
                            sbyte typeItem2 = -1;
                            Point[] array13 = null;
                            @char = (Char.myCharz().charID != num145) ? GameScr.findCharInMap(num145) : Char.myCharz();
                            try
                            {
                                typePaint2 = msg.reader().readByte();
                                sbyte b55 = msg.reader().readByte();
                                array13 = new Point[b55];
                                for (int num147 = 0; num147 < array13.Length; num147++)
                                {
                                    array13[num147] = new Point
                                    {
                                        type = msg.reader().readByte()
                                    };
                                    array13[num147].id = array13[num147].type == 0 ? msg.reader().readByte() : msg.reader().readInt();
                                }
                            }
                            catch (Exception)
                            {
                            }
                            try
                            {
                                typeItem2 = msg.reader().readByte();
                            }
                            catch (Exception)
                            {
                            }
                            Res.outz(">.SKILL_NOT_FOCUS  skill targetDame= " + point.x + ":" + point.y + "    c:" + @char.cx + ":" + @char.cy + "   cdir:" + @char.cdir);
                            @char.SetSkillPaint_STT(1, num146, point, timeDame, rangeDame, typePaint2, array13, typeItem2);
                        }
                        if (b53 == 0)
                        {
                            Res.outz("id use= " + num145);
                            if (Char.myCharz().charID != num145)
                            {
                                @char = GameScr.findCharInMap(num145);
                                if ((TileMap.tileTypeAtPixel(@char.cx, @char.cy) & 2) == 2)
                                {
                                    @char.setSkillPaint(GameScr.sks[num146], 0);
                                }
                                else
                                {
                                    @char.setSkillPaint(GameScr.sks[num146], 1);
                                    @char.delayFall = 20;
                                }
                            }
                            else
                            {
                                Char.myCharz().saveLoadPreviousSkill();
                                Res.outz("LOAD LAST SKILL");
                            }
                            sbyte b56 = msg.reader().readByte();
                            Res.outz("npc size= " + b56);
                            for (int num148 = 0; num148 < b56; num148++)
                            {
                                sbyte b57 = msg.reader().readByte();
                                sbyte b58 = msg.reader().readByte();
                                Res.outz("index= " + b57);
                                if (num146 is >= 42 and <= 48)
                                {
                                    ((Mob)GameScr.vMob.elementAt(b57)).isFreez = true;
                                    ((Mob)GameScr.vMob.elementAt(b57)).seconds = b58;
                                    ((Mob)GameScr.vMob.elementAt(b57)).last = ((Mob)GameScr.vMob.elementAt(b57)).cur = mSystem.currentTimeMillis();
                                }
                            }
                            sbyte b59 = msg.reader().readByte();
                            for (int num149 = 0; num149 < b59; num149++)
                            {
                                int num150 = msg.reader().readInt();
                                sbyte b60 = msg.reader().readByte();
                                Res.outz("player ID= " + num150 + " my ID= " + Char.myCharz().charID);
                                if (num146 is < 42 or > 48)
                                {
                                    continue;
                                }
                                if (num150 == Char.myCharz().charID)
                                {
                                    if (!Char.myCharz().isFlyAndCharge && !Char.myCharz().isStandAndCharge)
                                    {
                                        GameScr.gI().isFreez = true;
                                        Char.myCharz().isFreez = true;
                                        Char.myCharz().freezSeconds = b60;
                                        Char.myCharz().lastFreez = Char.myCharz().currFreez = mSystem.currentTimeMillis();
                                        Char.myCharz().isLockMove = true;
                                    }
                                }
                                else
                                {
                                    @char = GameScr.findCharInMap(num150);
                                    if (@char != null && !@char.isFlyAndCharge && !@char.isStandAndCharge)
                                    {
                                        @char.isFreez = true;
                                        @char.seconds = b60;
                                        @char.freezSeconds = b60;
                                        @char.lastFreez = GameScr.findCharInMap(num150).currFreez = mSystem.currentTimeMillis();
                                    }
                                }
                            }
                        }
                        if (b53 == 1 && num145 != Char.myCharz().charID)
                        {
                            GameScr.findCharInMap(num145).isCharge = true;
                        }
                        if (b53 == 3)
                        {
                            if (num145 == Char.myCharz().charID)
                            {
                                Char.myCharz().isCharge = false;
                                SoundMn.gI().taitaoPause();
                                Char.myCharz().saveLoadPreviousSkill();
                            }
                            else
                            {
                                GameScr.findCharInMap(num145).isCharge = false;
                            }
                        }
                        if (b53 == 4)
                        {
                            if (num145 == Char.myCharz().charID)
                            {
                                Char.myCharz().seconds = msg.reader().readShort() - 1000;
                                Char.myCharz().last = mSystem.currentTimeMillis();
                                Res.outz("second= " + Char.myCharz().seconds + " last= " + Char.myCharz().last);
                            }
                            else if (GameScr.findCharInMap(num145) != null)
                            {
                                switch (GameScr.findCharInMap(num145).cgender)
                                {
                                    case 0:
                                        GameScr.findCharInMap(num145).useChargeSkill(false);
                                        break;
                                    case 1:
                                        GameScr.findCharInMap(num145).useChargeSkill(true);
                                        break;
                                }
                                GameScr.findCharInMap(num145).skillTemplateId = num146;
                                GameScr.findCharInMap(num145).isUseSkillAfterCharge = true;
                                GameScr.findCharInMap(num145).seconds = msg.reader().readShort();
                                GameScr.findCharInMap(num145).last = mSystem.currentTimeMillis();
                            }
                        }
                        if (b53 == 5)
                        {
                            if (num145 == Char.myCharz().charID)
                            {
                                Char.myCharz().stopUseChargeSkill();
                            }
                            else
                            {
                                GameScr.findCharInMap(num145)?.stopUseChargeSkill();
                            }
                        }
                        if (b53 == 6)
                        {
                            if (num145 == Char.myCharz().charID)
                            {
                                Char.myCharz().setAutoSkillPaint(GameScr.sks[num146], 0);
                            }
                            else if (GameScr.findCharInMap(num145) != null)
                            {
                                GameScr.findCharInMap(num145).setAutoSkillPaint(GameScr.sks[num146], 0);
                                SoundMn.gI().gong();
                            }
                        }
                        if (b53 == 7)
                        {
                            if (num145 == Char.myCharz().charID)
                            {
                                Char.myCharz().seconds = msg.reader().readShort();
                                Res.outz("second = " + Char.myCharz().seconds);
                                Char.myCharz().last = mSystem.currentTimeMillis();
                            }
                            else if (GameScr.findCharInMap(num145) != null)
                            {
                                GameScr.findCharInMap(num145).useChargeSkill(true);
                                GameScr.findCharInMap(num145).seconds = msg.reader().readShort();
                                GameScr.findCharInMap(num145).last = mSystem.currentTimeMillis();
                                SoundMn.gI().gong();
                            }
                        }
                        if (b53 == 8 && num145 != Char.myCharz().charID && GameScr.findCharInMap(num145) != null)
                        {
                            GameScr.findCharInMap(num145).setAutoSkillPaint(GameScr.sks[num146], 0);
                        }
                        break;
                    }
                case -44:
                    {
                        bool flag6 = false;
                        if (GameCanvas.w > 2 * Panel.WIDTH_PANEL)
                        {
                            flag6 = true;
                        }
                        sbyte b32 = msg.reader().readByte();
                        int num65 = msg.reader().readUnsignedByte();
                        Char.myCharz().arrItemShop = new Item[num65][];
                        GameCanvas.panel.shopTabName = new string[num65 + ((!flag6) ? 1 : 0)][];
                        for (int num66 = 0; num66 < GameCanvas.panel.shopTabName.Length; num66++)
                        {
                            GameCanvas.panel.shopTabName[num66] = new string[2];
                        }
                        if (b32 == 2)
                        {
                            GameCanvas.panel.maxPageShop = new int[num65];
                            GameCanvas.panel.currPageShop = new int[num65];
                        }
                        if (!flag6)
                        {
                            GameCanvas.panel.shopTabName[num65] = mResources.inventory;
                        }
                        for (int num67 = 0; num67 < num65; num67++)
                        {
                            string[] array5 = Res.split(msg.reader().readUTF(), "\n", 0);
                            if (b32 == 2)
                            {
                                GameCanvas.panel.maxPageShop[num67] = msg.reader().readUnsignedByte();
                            }
                            if (array5.Length == 2)
                            {
                                GameCanvas.panel.shopTabName[num67] = array5;
                            }
                            if (array5.Length == 1)
                            {
                                GameCanvas.panel.shopTabName[num67][0] = array5[0];
                                GameCanvas.panel.shopTabName[num67][1] = string.Empty;
                            }
                            int num68 = msg.reader().readUnsignedByte();
                            Char.myCharz().arrItemShop[num67] = new Item[num68];
                            Panel.strWantToBuy = mResources.say_wat_do_u_want_to_buy;
                            if (b32 == 1)
                            {
                                Panel.strWantToBuy = mResources.say_wat_do_u_want_to_buy2;
                            }
                            for (int num69 = 0; num69 < num68; num69++)
                            {
                                short num70 = msg.reader().readShort();
                                if (num70 == -1)
                                {
                                    continue;
                                }
                                Char.myCharz().arrItemShop[num67][num69] = new Item
                                {
                                    template = ItemTemplates.get(num70)
                                };
                                Res.outz("name " + num67 + " = " + Char.myCharz().arrItemShop[num67][num69].template.name + " id templat= " + Char.myCharz().arrItemShop[num67][num69].template.id);
                                if (b32 == 8)
                                {
                                    Char.myCharz().arrItemShop[num67][num69].buyCoin = msg.reader().readInt();
                                    Char.myCharz().arrItemShop[num67][num69].buyGold = msg.reader().readInt();
                                    Char.myCharz().arrItemShop[num67][num69].quantity = msg.reader().readInt();
                                }
                                else if (b32 == 4)
                                {
                                    Char.myCharz().arrItemShop[num67][num69].reason = msg.reader().readUTF();
                                }
                                else if (b32 == 0)
                                {
                                    Char.myCharz().arrItemShop[num67][num69].buyCoin = msg.reader().readInt();
                                    Char.myCharz().arrItemShop[num67][num69].buyGold = msg.reader().readInt();
                                }
                                else if (b32 == 1)
                                {
                                    Char.myCharz().arrItemShop[num67][num69].powerRequire = msg.reader().readLong();
                                }
                                else if (b32 == 2)
                                {
                                    Char.myCharz().arrItemShop[num67][num69].itemId = msg.reader().readShort();
                                    Char.myCharz().arrItemShop[num67][num69].buyCoin = msg.reader().readInt();
                                    Char.myCharz().arrItemShop[num67][num69].buyGold = msg.reader().readInt();
                                    Char.myCharz().arrItemShop[num67][num69].buyType = msg.reader().readByte();
                                    Char.myCharz().arrItemShop[num67][num69].quantity = msg.reader().readInt();
                                    Char.myCharz().arrItemShop[num67][num69].isMe = msg.reader().readByte();
                                }
                                else if (b32 == 3)
                                {
                                    Char.myCharz().arrItemShop[num67][num69].isBuySpec = true;
                                    Char.myCharz().arrItemShop[num67][num69].iconSpec = msg.reader().readShort();
                                    Char.myCharz().arrItemShop[num67][num69].buySpec = msg.reader().readInt();
                                }
                                int num71 = msg.reader().readUnsignedByte();
                                if (num71 != 0)
                                {
                                    Char.myCharz().arrItemShop[num67][num69].itemOption = new ItemOption[num71];
                                    for (int num72 = 0; num72 < Char.myCharz().arrItemShop[num67][num69].itemOption.Length; num72++)
                                    {
                                        int num73 = msg.reader().readUnsignedByte();
                                        int param5 = msg.reader().readUnsignedShort();
                                        if (num73 != -1)
                                        {
                                            Char.myCharz().arrItemShop[num67][num69].itemOption[num72] = new ItemOption(num73, param5);
                                            Char.myCharz().arrItemShop[num67][num69].compare = GameCanvas.panel.getCompare(Char.myCharz().arrItemShop[num67][num69]);
                                        }
                                    }
                                }
                                sbyte b33 = msg.reader().readByte();
                                Char.myCharz().arrItemShop[num67][num69].newItem = b33 != 0;
                                sbyte b34 = msg.reader().readByte();
                                if (b34 == 1)
                                {
                                    int headTemp = msg.reader().readShort();
                                    int bodyTemp = msg.reader().readShort();
                                    int legTemp = msg.reader().readShort();
                                    int bagTemp = msg.reader().readShort();
                                    Char.myCharz().arrItemShop[num67][num69].setPartTemp(headTemp, bodyTemp, legTemp, bagTemp);
                                }
                                if (b32 == 2)
                                {
                                    Char.myCharz().arrItemShop[num67][num69].nameNguoiKyGui = msg.reader().readUTF();
                                    Res.err("nguoi ki gui  " + Char.myCharz().arrItemShop[num67][num69].nameNguoiKyGui);
                                }
                            }
                        }
                        if (flag6)
                        {
                            if (b32 != 2)
                            {
                                GameCanvas.panel2 = new Panel();
                                GameCanvas.panel2.tabName[7] = new string[1][] { new string[1] { string.Empty } };
                                GameCanvas.panel2.setTypeBodyOnly();
                                GameCanvas.panel2.show();
                            }
                            else
                            {
                                GameCanvas.panel2 = new Panel();
                                GameCanvas.panel2.setTypeKiGuiOnly();
                                GameCanvas.panel2.show();
                            }
                        }
                        GameCanvas.panel.tabName[1] = GameCanvas.panel.shopTabName;
                        if (b32 == 2)
                        {
                            string[][] array6 = GameCanvas.panel.tabName[1];
                            GameCanvas.panel.tabName[1] = flag6
                                ? (new string[4][]
                                {
                            array6[0],
                            array6[1],
                            array6[2],
                            array6[3]
                                })
                                : (new string[5][]
                                {
                            array6[0],
                            array6[1],
                            array6[2],
                            array6[3],
                            array6[4]
                                });
                        }
                        GameCanvas.panel.setTypeShop(b32);
                        GameCanvas.panel.show();
                        break;
                    }
                case -41:
                    {
                        sbyte b24 = msg.reader().readByte();
                        Char.myCharz().strLevel = new string[b24];
                        for (int num41 = 0; num41 < b24; num41++)
                        {
                            string text = msg.reader().readUTF();
                            Char.myCharz().strLevel[num41] = text;
                        }
                        Res.outz("---   xong  level caption cmd : " + msg.command);
                        break;
                    }
                case -34:
                    {
                        sbyte b17 = msg.reader().readByte();
                        Res.outz("act= " + b17);
                        if (b17 == 0 && GameScr.gI().magicTree != null)
                        {
                            Res.outz("toi duoc day");
                            MagicTree magicTree = GameScr.gI().magicTree;
                            magicTree.id = msg.reader().readShort();
                            magicTree.name = msg.reader().readUTF();
                            magicTree.name = Res.changeString(magicTree.name);
                            magicTree.x = msg.reader().readShort();
                            magicTree.y = msg.reader().readShort();
                            magicTree.level = msg.reader().readByte();
                            magicTree.currPeas = msg.reader().readShort();
                            magicTree.maxPeas = msg.reader().readShort();
                            Res.outz("curr Peas= " + magicTree.currPeas);
                            magicTree.strInfo = msg.reader().readUTF();
                            magicTree.seconds = msg.reader().readInt();
                            magicTree.timeToRecieve = magicTree.seconds;
                            sbyte b18 = msg.reader().readByte();
                            magicTree.peaPostionX = new int[b18];
                            magicTree.peaPostionY = new int[b18];
                            for (int num23 = 0; num23 < b18; num23++)
                            {
                                magicTree.peaPostionX[num23] = msg.reader().readByte();
                                magicTree.peaPostionY[num23] = msg.reader().readByte();
                            }
                            magicTree.isUpdate = msg.reader().readBool();
                            magicTree.last = magicTree.cur = mSystem.currentTimeMillis();
                            GameScr.gI().magicTree.isUpdateTree = true;
                        }
                        if (b17 == 1)
                        {
                            myVector = new MyVector();
                            try
                            {
                                while (msg.reader().available() > 0)
                                {
                                    string caption = msg.reader().readUTF();
                                    myVector.addElement(new Command(caption, GameCanvas.instance, 888392, null));
                                }
                            }
                            catch (Exception ex7)
                            {
                                Cout.println("Loi MAGIC_TREE " + ex7.ToString());
                            }
                            GameCanvas.menu.startAt(myVector, 3);
                        }
                        if (b17 == 2)
                        {
                            GameScr.gI().magicTree.remainPeas = msg.reader().readShort();
                            GameScr.gI().magicTree.seconds = msg.reader().readInt();
                            GameScr.gI().magicTree.last = GameScr.gI().magicTree.cur = mSystem.currentTimeMillis();
                            GameScr.gI().magicTree.isUpdateTree = true;
                            GameScr.gI().magicTree.isPeasEffect = true;
                        }
                        break;
                    }
                case 11:
                    {
                        GameCanvas.debug("SA9", 2);
                        int num13 = msg.reader().readByte();
                        sbyte b9 = msg.reader().readByte();
                        if (b9 != 0)
                        {
                            Mob.arrMobTemplate[num13].data.readDataNewBoss(NinjaUtil.readByteArray(msg), b9);
                        }
                        else
                        {
                            Mob.arrMobTemplate[num13].data.readData(NinjaUtil.readByteArray(msg));
                        }
                        for (int k = 0; k < GameScr.vMob.size(); k++)
                        {
                            mob = (Mob)GameScr.vMob.elementAt(k);
                            if (mob.templateId == num13)
                            {
                                mob.w = Mob.arrMobTemplate[num13].data.width;
                                mob.h = Mob.arrMobTemplate[num13].data.height;
                            }
                        }
                        sbyte[] array3 = NinjaUtil.readByteArray(msg);
                        Image img = Image.createImage(array3, 0, array3.Length);
                        Mob.arrMobTemplate[num13].data.img = img;
                        int num14 = msg.reader().readByte();
                        Mob.arrMobTemplate[num13].data.typeData = num14;
                        if (num14 is 1 or 2)
                        {
                            readFrameBoss(msg, num13);
                        }
                        break;
                    }
                case -69:
                    Char.myCharz().cMaxStamina = msg.reader().readShort();
                    break;
                case -68:
                    Char.myCharz().cStamina = msg.reader().readShort();
                    break;
                case -67:
                    {
                        Res.outz("RECIEVE ICON");
                        demCount += 1f;
                        int num174 = msg.reader().readInt();
                        sbyte[] array18 = null;
                        try
                        {
                            array18 = NinjaUtil.readByteArray(msg);
                            Res.outz("request hinh icon = " + num174);
                            if (num174 == 3896)
                            {
                                Res.outz("SIZE CHECK= " + array18.Length);
                            }
                            SmallImage.imgNew[num174].img = createImage(array18);
                        }
                        catch (Exception)
                        {
                            array18 = null;
                            SmallImage.imgNew[num174].img = Image.createRGBImage(new int[1], 1, 1, true);
                        }
                        if (array18 != null)
                        {
                            Rms.saveRMS(mGraphics.zoomLevel + "Small" + num174, array18);
                        }
                        break;
                    }
                case -66:
                    {
                        short num153 = msg.reader().readShort();
                        sbyte[] data5 = NinjaUtil.readByteArray(msg);
                        EffectData effDataById = Effect.getEffDataById(num153);
                        sbyte b63 = msg.reader().readSByte();
                        if (b63 == 0)
                        {
                            effDataById.readData(data5);
                        }
                        else
                        {
                            effDataById.readDataNewBoss(data5, b63);
                        }
                        sbyte[] array15 = NinjaUtil.readByteArray(msg);
                        effDataById.img = Image.createImage(array15, 0, array15.Length);
                        Res.outz("err5 ");
                        if (num153 != 78)
                        {
                            break;
                        }
                        sbyte b64 = msg.reader().readByte();
                        short[][] array16 = new short[b64][];
                        for (int num154 = 0; num154 < b64; num154++)
                        {
                            int num155 = msg.reader().readUnsignedByte();
                            array16[num154] = new short[num155];
                            for (int num156 = 0; num156 < num155; num156++)
                            {
                                array16[num154][num156] = msg.reader().readShort();
                            }
                        }
                        effDataById.anim_data = array16;
                        break;
                    }
                case -32:
                    {
                        short num141 = msg.reader().readShort();
                        int num142 = msg.reader().readInt();
                        sbyte[] array12 = null;
                        Image image = null;
                        try
                        {
                            array12 = new sbyte[num142];
                            for (int num143 = 0; num143 < num142; num143++)
                            {
                                array12[num143] = msg.reader().readByte();
                            }
                            image = Image.createImage(array12, 0, num142);
                            BgItem.imgNew.put(num141 + string.Empty, image);
                        }
                        catch (Exception)
                        {
                            array12 = null;
                            BgItem.imgNew.put(num141 + string.Empty, Image.createRGBImage(new int[1], 1, 1, true));
                        }
                        if (array12 != null)
                        {
                            if (mGraphics.zoomLevel > 1)
                            {
                                Rms.saveRMS(mGraphics.zoomLevel + "bgItem" + num141, array12);
                            }
                            BgItemMn.blendcurrBg(num141, image);
                        }
                        break;
                    }
                case 92:
                    {
                        if (GameCanvas.currentScreen == GameScr.instance)
                        {
                            GameCanvas.endDlg();
                        }
                        string text4 = msg.reader().readUTF();
                        string str2 = msg.reader().readUTF();
                        str2 = Res.changeString(str2);
                        string empty = string.Empty;
                        Char char7 = null;
                        sbyte b45 = 0;
                        if (!text4.Equals(string.Empty))
                        {
                            char7 = new Char
                            {
                                charID = msg.reader().readInt(),
                                head = msg.reader().readShort(),
                                headICON = msg.reader().readShort(),
                                body = msg.reader().readShort(),
                                bag = msg.reader().readShort(),
                                leg = msg.reader().readShort()
                            };
                            b45 = msg.reader().readByte();
                            char7.cName = text4;
                        }
                        empty += str2;
                        InfoDlg.hide();
                        if (text4.Equals(string.Empty))
                        {
                            GameScr.info1.addInfo(empty, 0);
                            break;
                        }
                        GameScr.info2.addInfoWithChar(empty, char7, b45 == 0);
                        if (GameCanvas.panel.isShow && GameCanvas.panel.type == 8)
                        {
                            GameCanvas.panel.initLogMessage();
                        }
                        break;
                    }
                case -26:
                    ServerListScreen.testConnect = 2;
                    GameCanvas.debug("SA2", 2);
                    GameCanvas.startOKDlg(msg.reader().readUTF());
                    InfoDlg.hide();
                    LoginScr.isContinueToLogin = false;
                    Char.isLoadingMap = false;
                    if (GameCanvas.currentScreen == GameCanvas.loginScr)
                    {
                        GameCanvas.serverScreen.switchToMe();
                    }
                    break;
                case -25:
                    GameCanvas.debug("SA3", 2);
                    GameScr.info1.addInfo(msg.reader().readUTF(), 0);
                    break;
                case 94:
                    GameCanvas.debug("SA3", 2);
                    GameScr.info1.addInfo(msg.reader().readUTF(), 0);
                    break;
                case 47:
                    GameCanvas.debug("SA4", 2);
                    GameScr.gI().resetButton();
                    break;
                case 81:
                    {
                        GameCanvas.debug("SXX4", 2);
                        Mob mob4 = (Mob)GameScr.vMob.elementAt(msg.reader().readUnsignedByte());
                        mob4.isDisable = msg.reader().readBool();
                        break;
                    }
                case 82:
                    {
                        GameCanvas.debug("SXX5", 2);
                        Mob mob4 = (Mob)GameScr.vMob.elementAt(msg.reader().readUnsignedByte());
                        mob4.isDontMove = msg.reader().readBool();
                        break;
                    }
                case 85:
                    {
                        GameCanvas.debug("SXX5", 2);
                        Mob mob4 = (Mob)GameScr.vMob.elementAt(msg.reader().readUnsignedByte());
                        mob4.isFire = msg.reader().readBool();
                        break;
                    }
                case 86:
                    {
                        GameCanvas.debug("SXX5", 2);
                        Mob mob4 = (Mob)GameScr.vMob.elementAt(msg.reader().readUnsignedByte());
                        mob4.isIce = msg.reader().readBool();
                        if (!mob4.isIce)
                        {
                            ServerEffect.addServerEffect(77, mob4.x, mob4.y - 9, 1);
                        }
                        break;
                    }
                case 87:
                    {
                        GameCanvas.debug("SXX5", 2);
                        Mob mob4 = (Mob)GameScr.vMob.elementAt(msg.reader().readUnsignedByte());
                        mob4.isWind = msg.reader().readBool();
                        break;
                    }
                case 56:
                    {
                        GameCanvas.debug("SXX6", 2);
                        @char = null;
                        int num9 = msg.reader().readInt();
                        if (num9 == Char.myCharz().charID)
                        {
                            bool flag3 = false;
                            @char = Char.myCharz();
                            @char.cHP = msg.readInt3Byte();
                            int num16 = msg.readInt3Byte();
                            Res.outz("dame hit = " + num16);
                            if (num16 != 0)
                            {
                                @char.doInjure();
                            }
                            int num17 = 0;
                            try
                            {
                                flag3 = msg.reader().readBoolean();
                                sbyte b13 = msg.reader().readByte();
                                if (b13 != -1)
                                {
                                    Res.outz("hit eff= " + b13);
                                    EffecMn.addEff(new Effect(b13, @char.cx, @char.cy, 3, 1, -1));
                                }
                            }
                            catch (Exception)
                            {
                            }
                            num16 += num17;
                            if (Char.myCharz().cTypePk != 4)
                            {
                                if (num16 == 0)
                                {
                                    GameScr.startFlyText(mResources.miss, @char.cx, @char.cy - @char.ch, 0, -3, mFont.MISS_ME);
                                }
                                else
                                {
                                    GameScr.startFlyText("-" + num16, @char.cx, @char.cy - @char.ch, 0, -3, flag3 ? mFont.FATAL : mFont.RED);
                                }
                            }
                            break;
                        }
                        @char = GameScr.findCharInMap(num9);
                        if (@char == null)
                        {
                            return;
                        }
                        @char.cHP = msg.readInt3Byte();
                        bool flag4 = false;
                        int num18 = msg.readInt3Byte();
                        if (num18 != 0)
                        {
                            @char.doInjure();
                        }
                        int num19 = 0;
                        try
                        {
                            flag4 = msg.reader().readBoolean();
                            sbyte b14 = msg.reader().readByte();
                            if (b14 != -1)
                            {
                                Res.outz("hit eff= " + b14);
                                EffecMn.addEff(new Effect(b14, @char.cx, @char.cy, 3, 1, -1));
                            }
                        }
                        catch (Exception)
                        {
                        }
                        num18 += num19;
                        if (@char.cTypePk != 4)
                        {
                            if (num18 == 0)
                            {
                                GameScr.startFlyText(mResources.miss, @char.cx, @char.cy - @char.ch, 0, -3, mFont.MISS);
                            }
                            else
                            {
                                GameScr.startFlyText("-" + num18, @char.cx, @char.cy - @char.ch, 0, -3, flag4 ? mFont.FATAL : mFont.ORANGE);
                            }
                        }
                        break;
                    }
                case 83:
                    {
                        GameCanvas.debug("SXX8", 2);
                        int num9 = msg.reader().readInt();
                        @char = (num9 != Char.myCharz().charID) ? GameScr.findCharInMap(num9) : Char.myCharz();
                        if (@char == null)
                        {
                            return;
                        }
                        Mob mobToAttack = (Mob)GameScr.vMob.elementAt(msg.reader().readUnsignedByte());
                        @char.mobMe?.attackOtherMob(mobToAttack);
                        break;
                    }
                case 84:
                    {
                        int num9 = msg.reader().readInt();
                        if (num9 == Char.myCharz().charID)
                        {
                            @char = Char.myCharz();
                        }
                        else
                        {
                            @char = GameScr.findCharInMap(num9);
                            if (@char == null)
                            {
                                return;
                            }
                        }
                        @char.cHP = @char.cHPFull;
                        @char.cMP = @char.cMPFull;
                        @char.cx = msg.reader().readShort();
                        @char.cy = msg.reader().readShort();
                        @char.liveFromDead();
                        break;
                    }
                case 46:
                    GameCanvas.debug("SA5", 2);
                    Cout.LogWarning("Controler RESET_POINT  " + Char.ischangingMap);
                    Char.isLockKey = false;
                    Char.myCharz().setResetPoint(msg.reader().readShort(), msg.reader().readShort());
                    break;
                case -29:
                    messageNotLogin(msg);
                    break;
                case -28:
                    messageNotMap(msg);
                    break;
                case -30:
                    messageSubCommand(msg);
                    break;
                case 62:
                    GameCanvas.debug("SZ3", 2);
                    @char = GameScr.findCharInMap(msg.reader().readInt());
                    if (@char != null)
                    {
                        @char.killCharId = Char.myCharz().charID;
                        Char.myCharz().npcFocus = null;
                        Char.myCharz().mobFocus = null;
                        Char.myCharz().itemFocus = null;
                        Char.myCharz().charFocus = @char;
                        Char.isManualFocus = true;
                        GameScr.info1.addInfo(@char.cName + mResources.CUU_SAT, 0);
                    }
                    break;
                case 63:
                    GameCanvas.debug("SZ4", 2);
                    Char.myCharz().killCharId = msg.reader().readInt();
                    Char.myCharz().npcFocus = null;
                    Char.myCharz().mobFocus = null;
                    Char.myCharz().itemFocus = null;
                    Char.myCharz().charFocus = GameScr.findCharInMap(Char.myCharz().killCharId);
                    Char.isManualFocus = true;
                    break;
                case 64:
                    GameCanvas.debug("SZ5", 2);
                    @char = Char.myCharz();
                    try
                    {
                        @char = GameScr.findCharInMap(msg.reader().readInt());
                    }
                    catch (Exception ex2)
                    {
                        Cout.println("Loi CLEAR_CUU_SAT " + ex2.ToString());
                    }
                    @char.killCharId = -9999;
                    break;
                case 39:
                    GameCanvas.debug("SA49", 2);
                    GameScr.gI().typeTradeOrder = 2;
                    if (GameScr.gI().typeTrade >= 2 && GameScr.gI().typeTradeOrder >= 2)
                    {
                        InfoDlg.showWait();
                    }
                    break;
                case 57:
                    {
                        GameCanvas.debug("SZ6", 2);
                        MyVector myVector2 = new();
                        myVector2.addElement(new Command(msg.reader().readUTF(), GameCanvas.instance, 88817, null));
                        GameCanvas.menu.startAt(myVector2, 3);
                        break;
                    }
                case 58:
                    {
                        GameCanvas.debug("SZ7", 2);
                        int num9 = msg.reader().readInt();
                        Char char10 = (num9 != Char.myCharz().charID) ? GameScr.findCharInMap(num9) : Char.myCharz();
                        char10.moveFast = new short[3];
                        char10.moveFast[0] = 0;
                        short num172 = msg.reader().readShort();
                        short num173 = msg.reader().readShort();
                        char10.moveFast[1] = num172;
                        char10.moveFast[2] = num173;
                        try
                        {
                            num9 = msg.reader().readInt();
                            Char char11 = (num9 != Char.myCharz().charID) ? GameScr.findCharInMap(num9) : Char.myCharz();
                            char11.cx = num172;
                            char11.cy = num173;
                        }
                        catch (Exception ex24)
                        {
                            Cout.println("Loi MOVE_FAST " + ex24.ToString());
                        }
                        break;
                    }
                case 88:
                    {
                        string info4 = msg.reader().readUTF();
                        short num171 = msg.reader().readShort();
                        GameCanvas.inputDlg.show(info4, new Command(mResources.ACCEPT, GameCanvas.instance, 88818, num171), TField.INPUT_TYPE_ANY);
                        break;
                    }
                case 27:
                    {
                        myVector = new MyVector();
                        string text7 = msg.reader().readUTF();
                        int num162 = msg.reader().readByte();
                        for (int num163 = 0; num163 < num162; num163++)
                        {
                            string caption4 = msg.reader().readUTF();
                            short num164 = msg.reader().readShort();
                            myVector.addElement(new Command(caption4, GameCanvas.instance, 88819, num164));
                        }
                        GameCanvas.menu.startWithoutCloseButton(myVector, 3);
                        break;
                    }
                case 33:
                    {
                        GameCanvas.debug("SA51", 2);
                        InfoDlg.hide();
                        GameCanvas.clearKeyHold();
                        GameCanvas.clearKeyPressed();
                        myVector = new MyVector();
                        try
                        {
                            while (true)
                            {
                                string caption3 = msg.reader().readUTF();
                                myVector.addElement(new Command(caption3, GameCanvas.instance, 88822, null));
                            }
                        }
                        catch (Exception ex22)
                        {
                            Cout.println("Loi OPEN_UI_MENU " + ex22.ToString());
                        }
                        if (Char.myCharz().npcFocus == null)
                        {
                            return;
                        }
                        for (int num152 = 0; num152 < Char.myCharz().npcFocus.template.menu.Length; num152++)
                        {
                            string[] array14 = Char.myCharz().npcFocus.template.menu[num152];
                            myVector.addElement(new Command(array14[0], GameCanvas.instance, 88820, array14));
                        }
                        GameCanvas.menu.startAt(myVector, 3);
                        break;
                    }
                case 40:
                    {
                        GameCanvas.debug("SA52", 2);
                        GameCanvas.taskTick = 150;
                        short taskId = msg.reader().readShort();
                        sbyte index3 = msg.reader().readByte();
                        string str3 = msg.reader().readUTF();
                        str3 = Res.changeString(str3);
                        string str4 = msg.reader().readUTF();
                        str4 = Res.changeString(str4);
                        string[] array9 = new string[msg.reader().readByte()];
                        string[] array10 = new string[array9.Length];
                        GameScr.tasks = new int[array9.Length];
                        GameScr.mapTasks = new int[array9.Length];
                        short[] array11 = new short[array9.Length];
                        short count = -1;
                        for (int num139 = 0; num139 < array9.Length; num139++)
                        {
                            string str5 = msg.reader().readUTF();
                            str5 = Res.changeString(str5);
                            GameScr.tasks[num139] = msg.reader().readByte();
                            GameScr.mapTasks[num139] = msg.reader().readShort();
                            string str6 = msg.reader().readUTF();
                            str6 = Res.changeString(str6);
                            array11[num139] = -1;
                            if (!str5.Equals(string.Empty))
                            {
                                array9[num139] = str5;
                                array10[num139] = str6;
                            }
                        }
                        try
                        {
                            count = msg.reader().readShort();
                            for (int num140 = 0; num140 < array9.Length; num140++)
                            {
                                array11[num140] = msg.reader().readShort();
                            }
                        }
                        catch (Exception ex17)
                        {
                            Cout.println("Loi TASK_GET " + ex17.ToString());
                        }
                        Char.myCharz().taskMaint = new Task(taskId, index3, str3, str4, array9, array11, count, array10);
                        if (Char.myCharz().npcFocus != null)
                        {
                            Npc.clearEffTask();
                        }
                        Char.taskAction(false);
                        break;
                    }
                case 41:
                    GameCanvas.debug("SA53", 2);
                    GameCanvas.taskTick = 100;
                    Res.outz("TASK NEXT");
                    Char.myCharz().taskMaint.index++;
                    Char.myCharz().taskMaint.count = 0;
                    Npc.clearEffTask();
                    Char.taskAction(true);
                    break;
                case 50:
                    {
                        sbyte b52 = msg.reader().readByte();
                        Panel.vGameInfo.removeAllElements();
                        for (int num138 = 0; num138 < b52; num138++)
                        {
                            GameInfo gameInfo = new()
                            {
                                id = msg.reader().readShort(),
                                main = msg.reader().readUTF(),
                                content = msg.reader().readUTF()
                            };
                            Panel.vGameInfo.addElement(gameInfo);
                            bool hasRead = Rms.loadRMSInt(gameInfo.id + string.Empty) != -1;
                            gameInfo.hasRead = hasRead;
                        }
                        break;
                    }
                case 43:
                    GameCanvas.taskTick = 50;
                    GameCanvas.debug("SA55", 2);
                    Char.myCharz().taskMaint.count = msg.reader().readShort();
                    if (Char.myCharz().npcFocus != null)
                    {
                        Npc.clearEffTask();
                    }
                    try
                    {
                        short num131 = msg.reader().readShort();
                        short num132 = msg.reader().readShort();
                        Char.myCharz().x_hint = num131;
                        Char.myCharz().y_hint = num132;
                        Res.outz("CMD   TASK_UPDATE:43_mapID =    x|y " + num131 + "|" + num132);
                        for (int num133 = 0; num133 < TileMap.vGo.size(); num133++)
                        {
                            Res.outz("===> " + TileMap.vGo.elementAt(num133));
                        }
                    }
                    catch (Exception)
                    {
                    }
                    break;
                case 90:
                    GameCanvas.debug("SA577", 2);
                    requestItemPlayer(msg);
                    break;
                case 29:
                    GameCanvas.debug("SA58", 2);
                    GameScr.gI().openUIZone(msg);
                    break;
                case -21:
                    {
                        GameCanvas.debug("SA60", 2);
                        short itemMapID = msg.reader().readShort();
                        for (int num116 = 0; num116 < GameScr.vItemMap.size(); num116++)
                        {
                            if (((ItemMap)GameScr.vItemMap.elementAt(num116)).itemMapID == itemMapID)
                            {
                                GameScr.vItemMap.removeElementAt(num116);
                                break;
                            }
                        }
                        break;
                    }
                case -20:
                    {
                        GameCanvas.debug("SA61", 2);
                        Char.myCharz().itemFocus = null;
                        short itemMapID = msg.reader().readShort();
                        for (int num115 = 0; num115 < GameScr.vItemMap.size(); num115++)
                        {
                            ItemMap itemMap2 = (ItemMap)GameScr.vItemMap.elementAt(num115);
                            if (itemMap2.itemMapID != itemMapID)
                            {
                                continue;
                            }
                            itemMap2.setPoint(Char.myCharz().cx, Char.myCharz().cy - 10);
                            string text5 = msg.reader().readUTF();
                            num = 0;
                            try
                            {
                                num = msg.reader().readShort();
                                if (itemMap2.template.type == 9)
                                {
                                    num = msg.reader().readShort();
                                    Char.myCharz().xu += num;
                                    Char.myCharz().xuStr = mSystem.numberTostring(Char.myCharz().xu);
                                }
                                else if (itemMap2.template.type == 10)
                                {
                                    num = msg.reader().readShort();
                                    Char.myCharz().luong += num;
                                    Char.myCharz().luongStr = mSystem.numberTostring(Char.myCharz().luong);
                                }
                                else if (itemMap2.template.type == 34)
                                {
                                    num = msg.reader().readShort();
                                    Char.myCharz().luongKhoa += num;
                                    Char.myCharz().luongKhoaStr = mSystem.numberTostring(Char.myCharz().luongKhoa);
                                }
                            }
                            catch (Exception)
                            {
                            }
                            if (text5.Equals(string.Empty))
                            {
                                if (itemMap2.template.type == 9)
                                {
                                    GameScr.startFlyText(((num >= 0) ? "+" : string.Empty) + num, Char.myCharz().cx, Char.myCharz().cy - Char.myCharz().ch, 0, -2, mFont.YELLOW);
                                    SoundMn.gI().getItem();
                                }
                                else if (itemMap2.template.type == 10)
                                {
                                    GameScr.startFlyText(((num >= 0) ? "+" : string.Empty) + num, Char.myCharz().cx, Char.myCharz().cy - Char.myCharz().ch, 0, -2, mFont.GREEN);
                                    SoundMn.gI().getItem();
                                }
                                else if (itemMap2.template.type == 34)
                                {
                                    GameScr.startFlyText(((num >= 0) ? "+" : string.Empty) + num, Char.myCharz().cx, Char.myCharz().cy - Char.myCharz().ch, 0, -2, mFont.RED);
                                    SoundMn.gI().getItem();
                                }
                                else
                                {
                                    GameScr.info1.addInfo(mResources.you_receive + " " + ((num <= 0) ? string.Empty : (num + " ")) + itemMap2.template.name, 0);
                                    SoundMn.gI().getItem();
                                }
                                if (num > 0 && Char.myCharz().petFollow != null && Char.myCharz().petFollow.smallID == 4683)
                                {
                                    ServerEffect.addServerEffect(55, Char.myCharz().petFollow.cmx, Char.myCharz().petFollow.cmy, 1);
                                    ServerEffect.addServerEffect(55, Char.myCharz().cx, Char.myCharz().cy, 1);
                                }
                            }
                            else if (text5.Length == 1)
                            {
                                Cout.LogError3("strInf.Length =1:  " + text5);
                            }
                            else
                            {
                                GameScr.info1.addInfo(text5, 0);
                            }
                            break;
                        }
                        break;
                    }
                case -19:
                    {
                        GameCanvas.debug("SA62", 2);
                        short itemMapID = msg.reader().readShort();
                        @char = GameScr.findCharInMap(msg.reader().readInt());
                        for (int num112 = 0; num112 < GameScr.vItemMap.size(); num112++)
                        {
                            ItemMap itemMap = (ItemMap)GameScr.vItemMap.elementAt(num112);
                            if (itemMap.itemMapID != itemMapID)
                            {
                                continue;
                            }
                            if (@char == null)
                            {
                                return;
                            }
                            itemMap.setPoint(@char.cx, @char.cy - 10);
                            if (itemMap.x < @char.cx)
                            {
                                @char.cdir = -1;
                            }
                            else if (itemMap.x > @char.cx)
                            {
                                @char.cdir = 1;
                            }
                            break;
                        }
                        break;
                    }
                case -18:
                    {
                        GameCanvas.debug("SA63", 2);
                        int num111 = msg.reader().readByte();
                        GameScr.vItemMap.addElement(new ItemMap(msg.reader().readShort(), Char.myCharz().arrItemBag[num111].template.id, Char.myCharz().cx, Char.myCharz().cy, msg.reader().readShort(), msg.reader().readShort()));
                        Char.myCharz().arrItemBag[num111] = null;
                        break;
                    }
                case 68:
                    {
                        Res.outz("ADD ITEM TO MAP --------------------------------------");
                        GameCanvas.debug("SA6333", 2);
                        short itemMapID = msg.reader().readShort();
                        short itemTemplateID = msg.reader().readShort();
                        int x = msg.reader().readShort();
                        int y = msg.reader().readShort();
                        int num107 = msg.reader().readInt();
                        short r = 0;
                        if (num107 == -2)
                        {
                            r = msg.reader().readShort();
                        }
                        ItemMap o2 = new(num107, itemMapID, itemTemplateID, x, y, r);
                        GameScr.vItemMap.addElement(o2);
                        break;
                    }
                case 69:
                    SoundMn.IsDelAcc = msg.reader().readByte() != 0;
                    break;
                case -14:
                    GameCanvas.debug("SA64", 2);
                    @char = GameScr.findCharInMap(msg.reader().readInt());
                    if (@char == null)
                    {
                        return;
                    }
                    GameScr.vItemMap.addElement(new ItemMap(msg.reader().readShort(), msg.reader().readShort(), @char.cx, @char.cy, msg.reader().readShort(), msg.reader().readShort()));
                    break;
                case -22:
                    GameCanvas.debug("SA65", 2);
                    Char.isLockKey = true;
                    Char.ischangingMap = true;
                    GameScr.gI().timeStartMap = 0;
                    GameScr.gI().timeLengthMap = 0;
                    Char.myCharz().mobFocus = null;
                    Char.myCharz().npcFocus = null;
                    Char.myCharz().charFocus = null;
                    Char.myCharz().itemFocus = null;
                    Char.myCharz().focus.removeAllElements();
                    Char.myCharz().testCharId = -9999;
                    Char.myCharz().killCharId = -9999;
                    GameCanvas.resetBg();
                    GameScr.gI().resetButton();
                    GameScr.gI().center = null;
                    break;
                case -70:
                    {
                        Res.outz("BIG MESSAGE .......................................");
                        GameCanvas.endDlg();
                        int avatar2 = msg.reader().readShort();
                        string chat3 = msg.reader().readUTF();
                        Npc npc6 = new(-1, 0, 0, 0, 0, 0)
                        {
                            avatar = avatar2
                        };
                        ChatPopup.addBigMessage(chat3, 100000, npc6);
                        sbyte b43 = msg.reader().readByte();
                        if (b43 == 0)
                        {
                            ChatPopup.serverChatPopUp.cmdMsg1 = new Command(mResources.CLOSE, ChatPopup.serverChatPopUp, 1001, null)
                            {
                                x = (GameCanvas.w / 2) - 35,
                                y = GameCanvas.h - 35
                            };
                        }
                        if (b43 == 1)
                        {
                            string p = msg.reader().readUTF();
                            string caption2 = msg.reader().readUTF();
                            ChatPopup.serverChatPopUp.cmdMsg1 = new Command(caption2, ChatPopup.serverChatPopUp, 1000, p)
                            {
                                x = (GameCanvas.w / 2) - 75,
                                y = GameCanvas.h - 35
                            };
                            ChatPopup.serverChatPopUp.cmdMsg2 = new Command(mResources.CLOSE, ChatPopup.serverChatPopUp, 1001, null)
                            {
                                x = (GameCanvas.w / 2) + 11,
                                y = GameCanvas.h - 35
                            };
                        }
                        break;
                    }
                case 38:
                    {
                        GameCanvas.debug("SA67", 2);
                        InfoDlg.hide();
                        int num74 = msg.reader().readShort();
                        Res.outz("OPEN_UI_SAY ID= " + num74);
                        string str = msg.reader().readUTF();
                        str = Res.changeString(str);
                        for (int num99 = 0; num99 < GameScr.vNpc.size(); num99++)
                        {
                            Npc npc4 = (Npc)GameScr.vNpc.elementAt(num99);
                            Res.outz("npc id= " + npc4.template.npcTemplateId);
                            if (npc4.template.npcTemplateId == num74)
                            {
                                ChatPopup.addChatPopupMultiLine(str, 100000, npc4);
                                GameCanvas.panel.hideNow();
                                return;
                            }
                        }
                        Npc npc5 = new(num74, 0, 0, 0, num74, GameScr.info1.charId[Char.myCharz().cgender][2]);
                        if (npc5.template.npcTemplateId == 5)
                        {
                            npc5.charID = 5;
                        }
                        try
                        {
                            npc5.avatar = msg.reader().readShort();
                        }
                        catch (Exception)
                        {
                        }
                        ChatPopup.addChatPopupMultiLine(str, 100000, npc5);
                        GameCanvas.panel.hideNow();
                        break;
                    }
                case 32:
                    {
                        GameCanvas.debug("SA68", 2);
                        int num74 = msg.reader().readShort();
                        for (int num75 = 0; num75 < GameScr.vNpc.size(); num75++)
                        {
                            Npc npc = (Npc)GameScr.vNpc.elementAt(num75);
                            if (npc.template.npcTemplateId == num74 && npc.Equals(Char.myCharz().npcFocus))
                            {
                                string chat = msg.reader().readUTF();
                                string[] array7 = new string[msg.reader().readByte()];
                                for (int num76 = 0; num76 < array7.Length; num76++)
                                {
                                    array7[num76] = msg.reader().readUTF();
                                }
                                GameScr.gI().createMenu(array7, npc);
                                _ = ChatPopup.addChatPopup(chat, 100000, npc);
                                return;
                            }
                        }
                        Npc npc2 = new(num74, 0, -100, 100, num74, GameScr.info1.charId[Char.myCharz().cgender][2]);
                        Res.outz((Char.myCharz().npcFocus == null) ? "null" : "!null");
                        string chat2 = msg.reader().readUTF();
                        string[] array8 = new string[msg.reader().readByte()];
                        for (int num77 = 0; num77 < array8.Length; num77++)
                        {
                            array8[num77] = msg.reader().readUTF();
                        }
                        try
                        {
                            short avatar = msg.reader().readShort();
                            npc2.avatar = avatar;
                        }
                        catch (Exception)
                        {
                        }
                        Res.outz((Char.myCharz().npcFocus == null) ? "null" : "!null");
                        GameScr.gI().createMenu(array8, npc2);
                        _ = ChatPopup.addChatPopup(chat2, 100000, npc2);
                        break;
                    }
                case 7:
                    {
                        sbyte type = msg.reader().readByte();
                        short id = msg.reader().readShort();
                        string info2 = msg.reader().readUTF();
                        GameCanvas.panel.saleRequest(type, info2, id);
                        break;
                    }
                case 6:
                    GameCanvas.debug("SA70", 2);
                    Char.myCharz().xu = msg.reader().readLong();
                    Char.myCharz().luong = msg.reader().readInt();
                    Char.myCharz().luongKhoa = msg.reader().readInt();
                    Char.myCharz().xuStr = mSystem.numberTostring(Char.myCharz().xu);
                    Char.myCharz().luongStr = mSystem.numberTostring(Char.myCharz().luong);
                    Char.myCharz().luongKhoaStr = mSystem.numberTostring(Char.myCharz().luongKhoa);
                    GameCanvas.endDlg();
                    break;
                case -24:
                    Char.isLoadingMap = true;
                    Cout.println("GET MAP INFO");
                    GameScr.gI().magicTree = null;
                    GameCanvas.isLoading = true;
                    GameCanvas.debug("SA75", 2);
                    GameScr.resetAllvector();
                    GameCanvas.endDlg();
                    TileMap.vGo.removeAllElements();
                    PopUp.vPopups.removeAllElements();
                    mSystem.gcc();
                    TileMap.mapID = msg.reader().readUnsignedByte();
                    TileMap.planetID = msg.reader().readByte();
                    TileMap.tileID = msg.reader().readByte();
                    TileMap.bgID = msg.reader().readByte();
                    Cout.println("load planet from server: " + TileMap.planetID + "bgType= " + TileMap.bgType + ".............................");
                    TileMap.typeMap = msg.reader().readByte();
                    TileMap.mapName = msg.reader().readUTF();
                    TileMap.zoneID = msg.reader().readByte();
                    GameCanvas.debug("SA75x1", 2);
                    try
                    {
                        TileMap.loadMapFromResource(TileMap.mapID);
                    }
                    catch (Exception)
                    {
                        Service.gI().requestMaptemplate(TileMap.mapID);
                        messWait = msg;
                        return;
                    }
                    loadInfoMap(msg);
                    try
                    {
                        sbyte b30 = msg.reader().readByte();
                        TileMap.isMapDouble = b30 != 0;
                    }
                    catch (Exception)
                    {
                    }
                    GameScr.cmx = GameScr.cmtoX;
                    GameScr.cmy = GameScr.cmtoY;
                    break;
                case -31:
                    {
                        TileMap.vItemBg.removeAllElements();
                        short num52 = msg.reader().readShort();
                        Cout.LogError2("nItem= " + num52);
                        for (int num53 = 0; num53 < num52; num53++)
                        {
                            BgItem bgItem = new()
                            {
                                id = num53,
                                idImage = msg.reader().readShort(),
                                layer = msg.reader().readByte(),
                                dx = msg.reader().readShort(),
                                dy = msg.reader().readShort()
                            };
                            sbyte b28 = msg.reader().readByte();
                            bgItem.tileX = new int[b28];
                            bgItem.tileY = new int[b28];
                            for (int num54 = 0; num54 < b28; num54++)
                            {
                                bgItem.tileX[num53] = msg.reader().readByte();
                                bgItem.tileY[num53] = msg.reader().readByte();
                            }
                            TileMap.vItemBg.addElement(bgItem);
                        }
                        break;
                    }
                case -4:
                    {
                        GameCanvas.debug("SA76", 2);
                        @char = GameScr.findCharInMap(msg.reader().readInt());
                        if (@char == null)
                        {
                            return;
                        }
                        GameCanvas.debug("SA76v1", 2);
                        if ((TileMap.tileTypeAtPixel(@char.cx, @char.cy) & 2) == 2)
                        {
                            @char.setSkillPaint(GameScr.sks[msg.reader().readUnsignedByte()], 0);
                        }
                        else
                        {
                            @char.setSkillPaint(GameScr.sks[msg.reader().readUnsignedByte()], 1);
                        }
                        GameCanvas.debug("SA76v2", 2);
                        @char.attMobs = new Mob[msg.reader().readByte()];
                        for (int n = 0; n < @char.attMobs.Length; n++)
                        {
                            Mob mob3 = (Mob)GameScr.vMob.elementAt(msg.reader().readByte());
                            @char.attMobs[n] = mob3;
                            if (n == 0)
                            {
                                @char.cdir = @char.cx <= mob3.x ? 1 : -1;
                            }
                        }
                        GameCanvas.debug("SA76v3", 2);
                        @char.charFocus = null;
                        @char.mobFocus = @char.attMobs[0];
                        Char[] array = new Char[10];
                        num = 0;
                        try
                        {
                            for (num = 0; num < array.Length; num++)
                            {
                                int num9 = msg.reader().readInt();
                                Char char4 = array[num] = (num9 != Char.myCharz().charID) ? GameScr.findCharInMap(num9) : Char.myCharz();
                                if (num == 0)
                                {
                                    @char.cdir = @char.cx <= char4.cx ? 1 : -1;
                                }
                            }
                        }
                        catch (Exception ex4)
                        {
                            Cout.println("Loi PLAYER_ATTACK_N_P " + ex4.ToString());
                        }
                        GameCanvas.debug("SA76v4", 2);
                        if (num > 0)
                        {
                            @char.attChars = new Char[num];
                            for (num = 0; num < @char.attChars.Length; num++)
                            {
                                @char.attChars[num] = array[num];
                            }
                            @char.charFocus = @char.attChars[0];
                            @char.mobFocus = null;
                        }
                        GameCanvas.debug("SA76v5", 2);
                        break;
                    }
                case 54:
                    {
                        @char = GameScr.findCharInMap(msg.reader().readInt());
                        if (@char == null)
                        {
                            return;
                        }
                        int num10 = msg.reader().readUnsignedByte();
                        if ((TileMap.tileTypeAtPixel(@char.cx, @char.cy) & 2) == 2)
                        {
                            @char.setSkillPaint(GameScr.sks[num10], 0);
                        }
                        else
                        {
                            @char.setSkillPaint(GameScr.sks[num10], 1);
                        }
                        Mob[] array2 = new Mob[10];
                        num = 0;
                        try
                        {
                            for (num = 0; num < array2.Length; num++)
                            {
                                Mob mob2 = array2[num] = (Mob)GameScr.vMob.elementAt(msg.reader().readByte());
                                if (num == 0)
                                {
                                    @char.cdir = @char.cx <= mob2.x ? 1 : -1;
                                }
                            }
                        }
                        catch (Exception ex3)
                        {
                            Cout.println("Loi PLAYER_ATTACK_NPC " + ex3.ToString());
                        }
                        if (num > 0)
                        {
                            @char.attMobs = new Mob[num];
                            for (num = 0; num < @char.attMobs.Length; num++)
                            {
                                @char.attMobs[num] = array2[num];
                            }
                            @char.charFocus = null;
                            @char.mobFocus = @char.attMobs[0];
                        }
                        break;
                    }
                case -60:
                    {
                        GameCanvas.debug("SA7666", 2);
                        int num2 = msg.reader().readInt();
                        int num3 = -1;
                        if (num2 != Char.myCharz().charID)
                        {
                            Char char2 = GameScr.findCharInMap(num2);
                            if (char2 == null)
                            {
                                return;
                            }
                            if (char2.currentMovePoint != null)
                            {
                                char2.createShadow(char2.cx, char2.cy, 10);
                                char2.cx = char2.currentMovePoint.xEnd;
                                char2.cy = char2.currentMovePoint.yEnd;
                            }
                            int num4 = msg.reader().readUnsignedByte();
                            Res.outz("player skill ID= " + num4);
                            if ((TileMap.tileTypeAtPixel(char2.cx, char2.cy) & 2) == 2)
                            {
                                char2.setSkillPaint(GameScr.sks[num4], 0);
                            }
                            else
                            {
                                char2.setSkillPaint(GameScr.sks[num4], 1);
                            }
                            sbyte b = msg.reader().readByte();
                            Res.outz("nAttack = " + b);
                            Char[] array = new Char[b];
                            for (num = 0; num < array.Length; num++)
                            {
                                num3 = msg.reader().readInt();
                                Char char3;
                                if (num3 == Char.myCharz().charID)
                                {
                                    char3 = Char.myCharz();
                                    if (!GameScr.isChangeZone && GameScr.isAutoPlay && GameScr.canAutoPlay)
                                    {
                                        Service.gI().requestChangeZone(-1, -1);
                                        GameScr.isChangeZone = true;
                                    }
                                }
                                else
                                {
                                    char3 = GameScr.findCharInMap(num3);
                                }
                                array[num] = char3;
                                if (num == 0)
                                {
                                    char2.cdir = char2.cx <= char3.cx ? 1 : -1;
                                }
                            }
                            if (num > 0)
                            {
                                char2.attChars = new Char[num];
                                for (num = 0; num < char2.attChars.Length; num++)
                                {
                                    char2.attChars[num] = array[num];
                                }
                                char2.mobFocus = null;
                                char2.charFocus = char2.attChars[0];
                            }
                        }
                        else
                        {
                            sbyte b2 = msg.reader().readByte();
                            sbyte b3 = msg.reader().readByte();
                            num3 = msg.reader().readInt();
                        }
                        try
                        {
                            sbyte b4 = msg.reader().readByte();
                            Res.outz("isRead continue = " + b4);
                            if (b4 != 1)
                            {
                                break;
                            }
                            sbyte b5 = msg.reader().readByte();
                            Res.outz("type skill = " + b5);
                            if (num3 == Char.myCharz().charID)
                            {
                                bool flag = false;
                                @char = Char.myCharz();
                                int num5 = msg.readInt3Byte();
                                Res.outz("dame hit = " + num5);
                                @char.isDie = msg.reader().readBoolean();
                                if (@char.isDie)
                                {
                                    Char.isLockKey = true;
                                }
                                Res.outz("isDie=" + @char.isDie + "---------------------------------------");
                                int num6 = 0;
                                flag = @char.isCrit = msg.reader().readBoolean();
                                @char.isMob = false;
                                num5 = @char.damHP = num5 + num6;
                                if (b5 == 0)
                                {
                                    @char.doInjure(num5, 0, flag, false);
                                }
                            }
                            else
                            {
                                @char = GameScr.findCharInMap(num3);
                                if (@char == null)
                                {
                                    return;
                                }
                                bool flag2 = false;
                                int num7 = msg.readInt3Byte();
                                Res.outz("dame hit= " + num7);
                                @char.isDie = msg.reader().readBoolean();
                                Res.outz("isDie=" + @char.isDie + "---------------------------------------");
                                int num8 = 0;
                                flag2 = @char.isCrit = msg.reader().readBoolean();
                                @char.isMob = false;
                                num7 = @char.damHP = num7 + num8;
                                if (b5 == 0)
                                {
                                    @char.doInjure(num7, 0, flag2, false);
                                }
                            }
                        }
                        catch (Exception)
                        {
                        }
                        break;
                    }
            }
            switch (msg.command)
            {
                case -2:
                    {
                        GameCanvas.debug("SA77", 22);
                        int num197 = msg.reader().readInt();
                        Char.myCharz().yen += num197;
                        GameScr.startFlyText((num197 <= 0) ? (string.Empty + num197) : ("+" + num197), Char.myCharz().cx, Char.myCharz().cy - Char.myCharz().ch - 10, 0, -2, mFont.YELLOW);
                        break;
                    }
                case 95:
                    {
                        GameCanvas.debug("SA77", 22);
                        int num184 = msg.reader().readInt();
                        Char.myCharz().xu += num184;
                        Char.myCharz().xuStr = mSystem.numberTostring(Char.myCharz().xu);
                        GameScr.startFlyText((num184 <= 0) ? (string.Empty + num184) : ("+" + num184), Char.myCharz().cx, Char.myCharz().cy - Char.myCharz().ch - 10, 0, -2, mFont.YELLOW);
                        break;
                    }
                case 96:
                    GameCanvas.debug("SA77a", 22);
                    Char.myCharz().taskOrders.addElement(new TaskOrder(msg.reader().readByte(), msg.reader().readShort(), msg.reader().readShort(), msg.reader().readUTF(), msg.reader().readUTF(), msg.reader().readByte(), msg.reader().readByte()));
                    break;
                case 97:
                    {
                        sbyte b77 = msg.reader().readByte();
                        for (int num190 = 0; num190 < Char.myCharz().taskOrders.size(); num190++)
                        {
                            TaskOrder taskOrder = (TaskOrder)Char.myCharz().taskOrders.elementAt(num190);
                            if (taskOrder.taskId == b77)
                            {
                                taskOrder.count = msg.reader().readShort();
                                break;
                            }
                        }
                        break;
                    }
                case -1:
                    {
                        GameCanvas.debug("SA77", 222);
                        int num196 = msg.reader().readInt();
                        Char.myCharz().xu += num196;
                        Char.myCharz().xuStr = mSystem.numberTostring(Char.myCharz().xu);
                        Char.myCharz().yen -= num196;
                        GameScr.startFlyText("+" + num196, Char.myCharz().cx, Char.myCharz().cy - Char.myCharz().ch - 10, 0, -2, mFont.YELLOW);
                        break;
                    }
                case -3:
                    {
                        GameCanvas.debug("SA78", 2);
                        sbyte b73 = msg.reader().readByte();
                        int num181 = msg.reader().readInt();
                        if (b73 == 0)
                        {
                            Char.myCharz().cPower += num181;
                        }
                        if (b73 == 1)
                        {
                            Char.myCharz().cTiemNang += num181;
                        }
                        if (b73 == 2)
                        {
                            Char.myCharz().cPower += num181;
                            Char.myCharz().cTiemNang += num181;
                        }
                        Char.myCharz().applyCharLevelPercent();
                        if (Char.myCharz().cTypePk != 3)
                        {
                            GameScr.startFlyText(((num181 <= 0) ? string.Empty : "+") + num181, Char.myCharz().cx, Char.myCharz().cy - Char.myCharz().ch, 0, -4, mFont.GREEN);
                            if (num181 > 0 && Char.myCharz().petFollow != null && Char.myCharz().petFollow.smallID == 5002)
                            {
                                ServerEffect.addServerEffect(55, Char.myCharz().petFollow.cmx, Char.myCharz().petFollow.cmy, 1);
                                ServerEffect.addServerEffect(55, Char.myCharz().cx, Char.myCharz().cy, 1);
                            }
                        }
                        break;
                    }
                case -73:
                    {
                        sbyte b79 = msg.reader().readByte();
                        for (int num195 = 0; num195 < GameScr.vNpc.size(); num195++)
                        {
                            Npc npc7 = (Npc)GameScr.vNpc.elementAt(num195);
                            if (npc7.template.npcTemplateId == b79)
                            {
                                sbyte b80 = msg.reader().readByte();
                                npc7.isHide = b80 == 0;
                                break;
                            }
                        }
                        break;
                    }
                case -5:
                    {
                        GameCanvas.debug("SA79", 2);
                        int charID = msg.reader().readInt();
                        int num186 = msg.reader().readInt();
                        Char char15 = num186 != -100
                            ? new Char
                            {
                                charID = charID,
                                clanID = num186
                            }
                            : new Mabu
                            {
                                charID = charID,
                                clanID = num186
                            };
                        if (char15.clanID == -2)
                        {
                            char15.isCopy = true;
                        }
                        if (readCharInfo(char15, msg))
                        {
                            sbyte b75 = msg.reader().readByte();
                            if (char15.cy <= 10 && b75 != 0 && b75 != 2)
                            {
                                Res.outz("nhân vật bay trên trời xuống x= " + char15.cx + " y= " + char15.cy);
                                Teleport teleport2 = new(char15.cx, char15.cy, char15.head, char15.cdir, 1, false, (b75 != 1) ? b75 : char15.cgender)
                                {
                                    id = char15.charID
                                };
                                char15.isTeleport = true;
                                Teleport.addTeleport(teleport2);
                            }
                            if (b75 == 2)
                            {
                                char15.show();
                            }
                            for (int num187 = 0; num187 < GameScr.vMob.size(); num187++)
                            {
                                Mob mob10 = (Mob)GameScr.vMob.elementAt(num187);
                                if (mob10 != null && mob10.isMobMe && mob10.mobId == char15.charID)
                                {
                                    Res.outz("co 1 con quai");
                                    char15.mobMe = mob10;
                                    char15.mobMe.x = char15.cx;
                                    char15.mobMe.y = char15.cy - 40;
                                    break;
                                }
                            }
                            if (GameScr.findCharInMap(char15.charID) == null)
                            {
                                GameScr.vCharInMap.addElement(char15);
                            }
                            char15.isMonkey = msg.reader().readByte();
                            short num188 = msg.reader().readShort();
                            Res.outz("mount id= " + num188 + "+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
                            if (num188 != -1)
                            {
                                char15.isHaveMount = true;
                                switch (num188)
                                {
                                    case 346:
                                    case 347:
                                    case 348:
                                        char15.isMountVip = false;
                                        break;
                                    case 349:
                                    case 350:
                                    case 351:
                                        char15.isMountVip = true;
                                        break;
                                    case 396:
                                        char15.isEventMount = true;
                                        break;
                                    case 532:
                                        char15.isSpeacialMount = true;
                                        break;
                                    default:
                                        if (num188 >= Char.ID_NEW_MOUNT)
                                        {
                                            char15.idMount = num188;
                                        }
                                        break;
                                }
                            }
                            else
                            {
                                char15.isHaveMount = false;
                            }
                        }
                        sbyte b76 = msg.reader().readByte();
                        Res.outz("addplayer:   " + b76);
                        char15.cFlag = b76;
                        char15.isNhapThe = msg.reader().readByte() == 1;
                        try
                        {
                            char15.idAuraEff = msg.reader().readShort();
                            char15.idEff_Set_Item = msg.reader().readSByte();
                            char15.idHat = msg.reader().readShort();
                            if (char15.bag is >= 201 and < 255)
                            {
                                Effect effect2 = new(char15.bag, char15, 2, -1, 10, 1)
                                {
                                    typeEff = 5
                                };
                                char15.addEffChar(effect2);
                            }
                            else
                            {
                                for (int num189 = 0; num189 < 54; num189++)
                                {
                                    char15.removeEffChar(0, 201 + num189);
                                }
                            }
                        }
                        catch (Exception ex37)
                        {
                            Res.outz("cmd: -5 err: " + ex37.StackTrace);
                        }
                        GameScr.gI().getFlagImage(char15.charID, char15.cFlag);
                        Res.outz("Cmd: -5 PLAYER_ADD: cID| cName| cFlag| cBag|    " + @char.charID + " | " + @char.cName + " | " + @char.cFlag + " | " + @char.bag);
                        break;
                    }
                case -7:
                    {
                        GameCanvas.debug("SA80", 2);
                        int num179 = msg.reader().readInt();
                        Cout.println("RECEVED MOVE OF " + num179);
                        for (int num182 = 0; num182 < GameScr.vCharInMap.size(); num182++)
                        {
                            Char char14 = null;
                            try
                            {
                                char14 = (Char)GameScr.vCharInMap.elementAt(num182);
                            }
                            catch (Exception ex29)
                            {
                                Cout.println("Loi PLAYER_MOVE " + ex29.ToString());
                            }
                            if (char14 == null)
                            {
                                break;
                            }
                            if (char14.charID == num179)
                            {
                                GameCanvas.debug("SA8x2y" + num182, 2);
                                char14.moveTo(msg.reader().readShort(), msg.reader().readShort(), 0);
                                char14.lastUpdateTime = mSystem.currentTimeMillis();
                                break;
                            }
                        }
                        GameCanvas.debug("SA80x3", 2);
                        break;
                    }
                case -6:
                    {
                        GameCanvas.debug("SA81", 2);
                        int num179 = msg.reader().readInt();
                        for (int num180 = 0; num180 < GameScr.vCharInMap.size(); num180++)
                        {
                            Char char13 = (Char)GameScr.vCharInMap.elementAt(num180);
                            if (char13 != null && char13.charID == num179)
                            {
                                if (!char13.isInvisiblez && !char13.isUsePlane)
                                {
                                    ServerEffect.addServerEffect(60, char13.cx, char13.cy, 1);
                                }
                                if (!char13.isUsePlane)
                                {
                                    GameScr.vCharInMap.removeElementAt(num180);
                                }
                                return;
                            }
                        }
                        break;
                    }
                case -13:
                    {
                        GameCanvas.debug("SA82", 2);
                        int num191 = msg.reader().readUnsignedByte();
                        if (num191 > GameScr.vMob.size() - 1 || num191 < 0)
                        {
                            return;
                        }
                        Mob mob9 = (Mob)GameScr.vMob.elementAt(num191);
                        mob9.sys = msg.reader().readByte();
                        mob9.levelBoss = msg.reader().readByte();
                        if (mob9.levelBoss != 0)
                        {
                            mob9.typeSuperEff = Res.random(0, 3);
                        }
                        mob9.x = mob9.xFirst;
                        mob9.y = mob9.yFirst;
                        mob9.status = 5;
                        mob9.injureThenDie = false;
                        mob9.hp = msg.reader().readInt();
                        mob9.maxHp = mob9.hp;
                        mob9.updateHp_bar();
                        ServerEffect.addServerEffect(60, mob9.x, mob9.y, 1);
                        break;
                    }
                case -75:
                    {
                        Mob mob9 = null;
                        try
                        {
                            mob9 = (Mob)GameScr.vMob.elementAt(msg.reader().readUnsignedByte());
                        }
                        catch (Exception)
                        {
                        }
                        if (mob9 != null)
                        {
                            mob9.levelBoss = msg.reader().readByte();
                            if (mob9.levelBoss > 0)
                            {
                                mob9.typeSuperEff = Res.random(0, 3);
                            }
                        }
                        break;
                    }
                case -9:
                    {
                        GameCanvas.debug("SA83", 2);
                        Mob mob9 = null;
                        try
                        {
                            mob9 = (Mob)GameScr.vMob.elementAt(msg.reader().readUnsignedByte());
                        }
                        catch (Exception)
                        {
                        }
                        GameCanvas.debug("SA83v1", 2);
                        if (mob9 != null)
                        {
                            mob9.hp = msg.readInt3Byte();
                            mob9.updateHp_bar();
                            int num183 = msg.readInt3Byte();
                            if (num183 == 1)
                            {
                                return;
                            }
                            if (num183 > 1)
                            {
                                mob9.setInjure();
                            }
                            bool flag10 = false;
                            try
                            {
                                flag10 = msg.reader().readBoolean();
                            }
                            catch (Exception)
                            {
                            }
                            sbyte b74 = msg.reader().readByte();
                            if (b74 != -1)
                            {
                                EffecMn.addEff(new Effect(b74, mob9.x, mob9.getY(), 3, 1, -1));
                            }
                            GameCanvas.debug("SA83v2", 2);
                            if (flag10)
                            {
                                GameScr.startFlyText("-" + num183, mob9.x, mob9.getY() - mob9.getH(), 0, -2, mFont.FATAL);
                            }
                            else if (num183 == 0)
                            {
                                mob9.x = mob9.xFirst;
                                mob9.y = mob9.yFirst;
                                GameScr.startFlyText(mResources.miss, mob9.x, mob9.getY() - mob9.getH(), 0, -2, mFont.MISS);
                            }
                            else if (num183 > 1)
                            {
                                GameScr.startFlyText("-" + num183, mob9.x, mob9.getY() - mob9.getH(), 0, -2, mFont.ORANGE);
                            }
                        }
                        GameCanvas.debug("SA83v3", 2);
                        break;
                    }
                case 45:
                    {
                        GameCanvas.debug("SA84", 2);
                        Mob mob9 = null;
                        try
                        {
                            mob9 = (Mob)GameScr.vMob.elementAt(msg.reader().readUnsignedByte());
                        }
                        catch (Exception ex28)
                        {
                            Cout.println("Loi tai NPC_MISS  " + ex28.ToString());
                        }
                        if (mob9 != null)
                        {
                            mob9.hp = msg.reader().readInt();
                            mob9.updateHp_bar();
                            GameScr.startFlyText(mResources.miss, mob9.x, mob9.y - mob9.h, 0, -2, mFont.MISS);
                        }
                        break;
                    }
                case -12:
                    {
                        Res.outz("SERVER SEND MOB DIE");
                        GameCanvas.debug("SA85", 2);
                        Mob mob9 = null;
                        try
                        {
                            mob9 = (Mob)GameScr.vMob.elementAt(msg.reader().readUnsignedByte());
                        }
                        catch (Exception)
                        {
                            Cout.println("LOi tai NPC_DIE cmd " + msg.command);
                        }
                        if (mob9 == null || mob9.status == 0 || mob9.status == 0)
                        {
                            break;
                        }
                        mob9.startDie();
                        try
                        {
                            int num192 = msg.readInt3Byte();
                            if (msg.reader().readBool())
                            {
                                GameScr.startFlyText("-" + num192, mob9.x, mob9.y - mob9.h, 0, -2, mFont.FATAL);
                            }
                            else
                            {
                                GameScr.startFlyText("-" + num192, mob9.x, mob9.y - mob9.h, 0, -2, mFont.ORANGE);
                            }
                            sbyte b78 = msg.reader().readByte();
                            for (int num193 = 0; num193 < b78; num193++)
                            {
                                ItemMap itemMap4 = new(msg.reader().readShort(), msg.reader().readShort(), mob9.x, mob9.y, msg.reader().readShort(), msg.reader().readShort());
                                int num194 = itemMap4.playerId = msg.reader().readInt();
                                Res.outz("playerid= " + num194 + " my id= " + Char.myCharz().charID);
                                GameScr.vItemMap.addElement(itemMap4);
                                if (Res.abs(itemMap4.y - Char.myCharz().cy) < 24 && Res.abs(itemMap4.x - Char.myCharz().cx) < 24)
                                {
                                    Char.myCharz().charFocus = null;
                                }
                            }
                        }
                        catch (Exception ex39)
                        {
                            Cout.println("LOi tai NPC_DIE " + ex39.ToString() + " cmd " + msg.command);
                        }
                        break;
                    }
                case 74:
                    {
                        GameCanvas.debug("SA85", 2);
                        Mob mob9 = null;
                        try
                        {
                            mob9 = (Mob)GameScr.vMob.elementAt(msg.reader().readUnsignedByte());
                        }
                        catch (Exception)
                        {
                            Cout.println("Loi tai NPC CHANGE " + msg.command);
                        }
                        if (mob9 != null && mob9.status != 0 && mob9.status != 0)
                        {
                            mob9.status = 0;
                            ServerEffect.addServerEffect(60, mob9.x, mob9.y, 1);
                            ItemMap itemMap3 = new(msg.reader().readShort(), msg.reader().readShort(), mob9.x, mob9.y, msg.reader().readShort(), msg.reader().readShort());
                            GameScr.vItemMap.addElement(itemMap3);
                            if (Res.abs(itemMap3.y - Char.myCharz().cy) < 24 && Res.abs(itemMap3.x - Char.myCharz().cx) < 24)
                            {
                                Char.myCharz().charFocus = null;
                            }
                        }
                        break;
                    }
                case -11:
                    {
                        GameCanvas.debug("SA86", 2);
                        Mob mob9 = null;
                        try
                        {
                            int index4 = msg.reader().readUnsignedByte();
                            mob9 = (Mob)GameScr.vMob.elementAt(index4);
                        }
                        catch (Exception ex26)
                        {
                            Res.outz("Loi tai NPC_ATTACK_ME " + msg.command + " err= " + ex26.StackTrace);
                        }
                        if (mob9 != null)
                        {
                            Char.myCharz().isDie = false;
                            Char.isLockKey = false;
                            int num176 = msg.readInt3Byte();
                            int num177;
                            try
                            {
                                num177 = msg.readInt3Byte();
                            }
                            catch (Exception)
                            {
                                num177 = 0;
                            }
                            if (mob9.isBusyAttackSomeOne)
                            {
                                Char.myCharz().doInjure(num176, num177, false, true);
                                break;
                            }
                            mob9.dame = num176;
                            mob9.dameMp = num177;
                            mob9.setAttack(Char.myCharz());
                        }
                        break;
                    }
                case -10:
                    {
                        GameCanvas.debug("SA87", 2);
                        Mob mob9 = null;
                        try
                        {
                            mob9 = (Mob)GameScr.vMob.elementAt(msg.reader().readUnsignedByte());
                        }
                        catch (Exception)
                        {
                        }
                        GameCanvas.debug("SA87x1", 2);
                        if (mob9 != null)
                        {
                            GameCanvas.debug("SA87x2", 2);
                            @char = GameScr.findCharInMap(msg.reader().readInt());
                            if (@char == null)
                            {
                                return;
                            }
                            GameCanvas.debug("SA87x3", 2);
                            int num185 = msg.readInt3Byte();
                            mob9.dame = @char.cHP - num185;
                            @char.cHPNew = num185;
                            GameCanvas.debug("SA87x4", 2);
                            try
                            {
                                @char.cMP = msg.readInt3Byte();
                            }
                            catch (Exception)
                            {
                            }
                            GameCanvas.debug("SA87x5", 2);
                            if (mob9.isBusyAttackSomeOne)
                            {
                                @char.doInjure(mob9.dame, 0, false, true);
                            }
                            else
                            {
                                mob9.setAttack(@char);
                            }
                            GameCanvas.debug("SA87x6", 2);
                        }
                        break;
                    }
                case -17:
                    GameCanvas.debug("SA88", 2);
                    Char.myCharz().meDead = true;
                    Char.myCharz().cPk = msg.reader().readByte();
                    Char.myCharz().startDie(msg.reader().readShort(), msg.reader().readShort());
                    try
                    {
                        Char.myCharz().cPower = msg.reader().readLong();
                        Char.myCharz().applyCharLevelPercent();
                    }
                    catch (Exception)
                    {
                        Cout.println("Loi tai ME_DIE " + msg.command);
                    }
                    Char.myCharz().countKill = 0;
                    break;
                case 66:
                    Res.outz("ME DIE XP DOWN NOT IMPLEMENT YET!!!!!!!!!!!!!!!!!!!!!!!!!!");
                    break;
                case -8:
                    GameCanvas.debug("SA89", 2);
                    @char = GameScr.findCharInMap(msg.reader().readInt());
                    if (@char == null)
                    {
                        return;
                    }
                    @char.cPk = msg.reader().readByte();
                    @char.waitToDie(msg.reader().readShort(), msg.reader().readShort());
                    break;
                case -16:
                    GameCanvas.debug("SA90", 2);
                    if (Char.myCharz().wdx != 0 || Char.myCharz().wdy != 0)
                    {
                        Char.myCharz().cx = Char.myCharz().wdx;
                        Char.myCharz().cy = Char.myCharz().wdy;
                        Char.myCharz().wdx = Char.myCharz().wdy = 0;
                    }
                    Char.myCharz().liveFromDead();
                    Char.myCharz().isLockMove = false;
                    Char.myCharz().meDead = false;
                    break;
                case 44:
                    {
                        GameCanvas.debug("SA91", 2);
                        int num178 = msg.reader().readInt();
                        string text8 = msg.reader().readUTF();
                        Res.outz("user id= " + num178 + " text= " + text8);
                        @char = (Char.myCharz().charID != num178) ? GameScr.findCharInMap(num178) : Char.myCharz();
                        if (@char == null)
                        {
                            return;
                        }
                        @char.addInfo(text8);
                        break;
                    }
                case 18:
                    {
                        sbyte b72 = msg.reader().readByte();
                        for (int num175 = 0; num175 < b72; num175++)
                        {
                            int charId = msg.reader().readInt();
                            int cx = msg.reader().readShort();
                            int cy = msg.reader().readShort();
                            int cHPShow = msg.readInt3Byte();
                            Char char12 = GameScr.findCharInMap(charId);
                            if (char12 != null)
                            {
                                char12.cx = cx;
                                char12.cy = cy;
                                char12.cHP = char12.cHPShow = cHPShow;
                                char12.lastUpdateTime = mSystem.currentTimeMillis();
                            }
                        }
                        break;
                    }
                case 19:
                    Char.myCharz().countKill = msg.reader().readUnsignedShort();
                    Char.myCharz().countKillMax = msg.reader().readUnsignedShort();
                    break;
            }
            GameCanvas.debug("SA92", 2);
        }
        catch (Exception ex40)
        {
            Res.outz("Controller = " + ex40.StackTrace);
        }
        finally
        {
            msg?.cleanup();
        }
    }

    private void createItem(myReader d)
    {
        GameScr.vcItem = d.readByte();
        ItemTemplates.itemTemplates.clear();
        GameScr.gI().iOptionTemplates = new ItemOptionTemplate[d.readUnsignedByte()];
        for (int i = 0; i < GameScr.gI().iOptionTemplates.Length; i++)
        {
            GameScr.gI().iOptionTemplates[i] = new ItemOptionTemplate
            {
                id = i,
                name = d.readUTF(),
                type = d.readByte()
            };
        }
        int num = d.readShort();
        for (int j = 0; j < num; j++)
        {
            ItemTemplate it = new((short)j, d.readByte(), d.readByte(), d.readUTF(), d.readUTF(), d.readByte(), d.readInt(), d.readShort(), d.readShort(), d.readBool());
            ItemTemplates.add(it);
        }
    }

    private void createSkill(myReader d)
    {
        GameScr.vcSkill = d.readByte();
        GameScr.gI().sOptionTemplates = new SkillOptionTemplate[d.readByte()];
        for (int i = 0; i < GameScr.gI().sOptionTemplates.Length; i++)
        {
            GameScr.gI().sOptionTemplates[i] = new SkillOptionTemplate
            {
                id = i,
                name = d.readUTF()
            };
        }
        GameScr.nClasss = new NClass[d.readByte()];
        for (int j = 0; j < GameScr.nClasss.Length; j++)
        {
            GameScr.nClasss[j] = new NClass
            {
                classId = j,
                name = d.readUTF(),
                skillTemplates = new SkillTemplate[d.readByte()]
            };
            for (int k = 0; k < GameScr.nClasss[j].skillTemplates.Length; k++)
            {
                GameScr.nClasss[j].skillTemplates[k] = new SkillTemplate
                {
                    id = d.readByte(),
                    name = d.readUTF(),
                    maxPoint = d.readByte(),
                    manaUseType = d.readByte(),
                    type = d.readByte(),
                    iconId = d.readShort(),
                    damInfo = d.readUTF()
                };
                int lineWidth = 130;
                if (GameCanvas.w == 128 || GameCanvas.h <= 208)
                {
                    lineWidth = 100;
                }
                GameScr.nClasss[j].skillTemplates[k].description = mFont.tahoma_7_green2.splitFontArray(d.readUTF(), lineWidth);
                GameScr.nClasss[j].skillTemplates[k].skills = new Skill[d.readByte()];
                for (int l = 0; l < GameScr.nClasss[j].skillTemplates[k].skills.Length; l++)
                {
                    GameScr.nClasss[j].skillTemplates[k].skills[l] = new Skill
                    {
                        skillId = d.readShort(),
                        template = GameScr.nClasss[j].skillTemplates[k],
                        point = d.readByte(),
                        powRequire = d.readLong(),
                        manaUse = d.readShort(),
                        coolDown = d.readInt(),
                        dx = d.readShort(),
                        dy = d.readShort(),
                        maxFight = d.readByte(),
                        damage = d.readShort(),
                        price = d.readShort(),
                        moreInfo = d.readUTF()
                    };
                    Skills.add(GameScr.nClasss[j].skillTemplates[k].skills[l]);
                }
            }
        }
    }

    private void createMap(myReader d)
    {
        GameScr.vcMap = d.readByte();
        TileMap.mapNames = new string[d.readUnsignedByte()];
        for (int i = 0; i < TileMap.mapNames.Length; i++)
        {
            TileMap.mapNames[i] = d.readUTF();
        }
        Npc.arrNpcTemplate = new NpcTemplate[d.readByte()];
        for (sbyte b = 0; b < Npc.arrNpcTemplate.Length; b = (sbyte)(b + 1))
        {
            Npc.arrNpcTemplate[b] = new NpcTemplate
            {
                npcTemplateId = b,
                name = d.readUTF(),
                headId = d.readShort(),
                bodyId = d.readShort(),
                legId = d.readShort(),
                menu = new string[d.readByte()][]
            };
            for (int j = 0; j < Npc.arrNpcTemplate[b].menu.Length; j++)
            {
                Npc.arrNpcTemplate[b].menu[j] = new string[d.readByte()];
                for (int k = 0; k < Npc.arrNpcTemplate[b].menu[j].Length; k++)
                {
                    Npc.arrNpcTemplate[b].menu[j][k] = d.readUTF();
                }
            }
        }
        Mob.arrMobTemplate = new MobTemplate[d.readByte()];
        for (sbyte b2 = 0; b2 < Mob.arrMobTemplate.Length; b2 = (sbyte)(b2 + 1))
        {
            Mob.arrMobTemplate[b2] = new MobTemplate
            {
                mobTemplateId = b2,
                type = d.readByte(),
                name = d.readUTF(),
                hp = d.readInt(),
                rangeMove = d.readByte(),
                speed = d.readByte(),
                dartType = d.readByte()
            };
        }
    }

    private void createData(myReader d, bool isSaveRMS)
    {
        GameScr.vcData = d.readByte();
        if (isSaveRMS)
        {
            Rms.saveRMS("NR_dart", NinjaUtil.readByteArray(d));
            Rms.saveRMS("NR_arrow", NinjaUtil.readByteArray(d));
            Rms.saveRMS("NR_effect", NinjaUtil.readByteArray(d));
            Rms.saveRMS("NR_image", NinjaUtil.readByteArray(d));
            Rms.saveRMS("NR_part", NinjaUtil.readByteArray(d));
            Rms.saveRMS("NR_skill", NinjaUtil.readByteArray(d));
            Rms.DeleteStorage("NRdata");
        }
    }

    private Image createImage(sbyte[] arr)
    {
        try
        {
            return Image.createImage(arr, 0, arr.Length);
        }
        catch (Exception)
        {
        }
        return null;
    }

    public int[] arrayByte2Int(sbyte[] b)
    {
        int[] array = new int[b.Length];
        for (int i = 0; i < b.Length; i++)
        {
            int num = b[i];
            if (num < 0)
            {
                num += 256;
            }
            array[i] = num;
        }
        return array;
    }

    public void readClanMsg(Message msg, int index)
    {
        try
        {
            ClanMessage clanMessage = new();
            sbyte b = msg.reader().readByte();
            clanMessage.type = b;
            clanMessage.id = msg.reader().readInt();
            clanMessage.playerId = msg.reader().readInt();
            clanMessage.playerName = msg.reader().readUTF();
            clanMessage.role = msg.reader().readByte();
            clanMessage.time = msg.reader().readInt() + 1000000000;
            bool flag = false;
            GameScr.isNewClanMessage = false;
            if (b == 0)
            {
                string text = msg.reader().readUTF();
                GameScr.isNewClanMessage = true;
                if (mFont.tahoma_7.getWidth(text) > Panel.WIDTH_PANEL - 60)
                {
                    clanMessage.chat = mFont.tahoma_7.splitFontArray(text, Panel.WIDTH_PANEL - 10);
                }
                else
                {
                    clanMessage.chat = new string[1];
                    clanMessage.chat[0] = text;
                }
                clanMessage.color = msg.reader().readByte();
            }
            else if (b == 1)
            {
                clanMessage.recieve = msg.reader().readByte();
                clanMessage.maxCap = msg.reader().readByte();
                flag = msg.reader().readByte() == 1;
                if (flag)
                {
                    GameScr.isNewClanMessage = true;
                }
                if (clanMessage.playerId != Char.myCharz().charID)
                {
                    clanMessage.option = clanMessage.recieve < clanMessage.maxCap ? (new string[1] { mResources.donate }) : null;
                }
                if (GameCanvas.panel.cp != null)
                {
                    GameCanvas.panel.updateRequest(clanMessage.recieve, clanMessage.maxCap);
                }
            }
            else if (b == 2 && Char.myCharz().role == 0)
            {
                GameScr.isNewClanMessage = true;
                clanMessage.option = new string[2]
                {
                    mResources.CANCEL,
                    mResources.receive
                };
            }
            if (GameCanvas.currentScreen != GameScr.instance)
            {
                GameScr.isNewClanMessage = false;
            }
            else if (GameCanvas.panel.isShow && GameCanvas.panel.type == 0 && GameCanvas.panel.currentTabIndex == 3)
            {
                GameScr.isNewClanMessage = false;
            }
            ClanMessage.addMessage(clanMessage, index, flag);
        }
        catch (Exception)
        {
            Cout.println("LOI TAI CMD -= " + msg.command);
        }
    }

    public void loadCurrMap(sbyte teleport3)
    {
        Res.outz("is loading map = " + Char.isLoadingMap);
        GameScr.gI().auto = 0;
        GameScr.isChangeZone = false;
        CreateCharScr.instance = null;
        GameScr.info1.isUpdate = false;
        GameScr.info2.isUpdate = false;
        GameScr.lockTick = 0;
        GameCanvas.panel.isShow = false;
        SoundMn.gI().stopAll();
        if (!GameScr.isLoadAllData && !CreateCharScr.isCreateChar)
        {
            GameScr.gI().initSelectChar();
        }
        GameScr.loadCamera(false, (teleport3 != 1) ? (-1) : Char.myCharz().cx, (teleport3 == 0) ? (-1) : 0);
        TileMap.loadMainTile();
        TileMap.loadMap(TileMap.tileID);
        Res.outz("LOAD GAMESCR 2");
        Char.myCharz().cvx = 0;
        Char.myCharz().statusMe = 4;
        Char.myCharz().currentMovePoint = null;
        Char.myCharz().mobFocus = null;
        Char.myCharz().charFocus = null;
        Char.myCharz().npcFocus = null;
        Char.myCharz().itemFocus = null;
        Char.myCharz().skillPaint = null;
        Char.myCharz().setMabuHold(false);
        Char.myCharz().skillPaintRandomPaint = null;
        GameCanvas.clearAllPointerEvent();
        if (Char.myCharz().cy >= TileMap.pxh - 100)
        {
            Char.myCharz().isFlyUp = true;
            Char.myCharz().cx += Res.abs(Res.random(0, 80));
            Service.gI().charMove();
        }
        GameScr.gI().loadGameScr();
        GameCanvas.loadBG(TileMap.bgID);
        Char.isLockKey = false;
        Res.outz("cy= " + Char.myCharz().cy + "---------------------------------------------");
        for (int i = 0; i < Char.myCharz().vEff.size(); i++)
        {
            EffectChar effectChar = (EffectChar)Char.myCharz().vEff.elementAt(i);
            if (effectChar.template.type == 10)
            {
                Char.isLockKey = true;
                break;
            }
        }
        GameCanvas.clearKeyHold();
        GameCanvas.clearKeyPressed();
        GameScr.gI().dHP = Char.myCharz().cHP;
        GameScr.gI().dMP = Char.myCharz().cMP;
        Char.ischangingMap = false;
        GameScr.gI().switchToMe();
        if (Char.myCharz().cy <= 10 && teleport3 != 0 && teleport3 != 2)
        {
            Teleport p = new(Char.myCharz().cx, Char.myCharz().cy, Char.myCharz().head, Char.myCharz().cdir, 1, true, (teleport3 != 1) ? teleport3 : Char.myCharz().cgender);
            Teleport.addTeleport(p);
            Char.myCharz().isTeleport = true;
        }
        if (teleport3 == 2)
        {
            Char.myCharz().show();
        }
        if (GameScr.gI().isRongThanXuatHien)
        {
            if (TileMap.mapID == GameScr.gI().mapRID && TileMap.zoneID == GameScr.gI().zoneRID)
            {
                GameScr.gI().callRongThan(GameScr.gI().xR, GameScr.gI().yR);
            }
            if (mGraphics.zoomLevel > 1)
            {
                GameScr.gI().doiMauTroi();
            }
        }
        InfoDlg.hide();
        InfoDlg.show(TileMap.mapName, mResources.zone + " " + TileMap.zoneID, 30);
        GameCanvas.endDlg();
        GameCanvas.isLoading = false;
        Hint.clickMob();
        Hint.clickNpc();
        GameCanvas.debug("SA75x9", 2);
    }

    public void loadInfoMap(Message msg)
    {
        try
        {
            if (mGraphics.zoomLevel == 1)
            {
                SmallImage.clearHastable();
            }
            Char.myCharz().cx = Char.myCharz().cxSend = Char.myCharz().cxFocus = msg.reader().readShort();
            Char.myCharz().cy = Char.myCharz().cySend = Char.myCharz().cyFocus = msg.reader().readShort();
            Char.myCharz().xSd = Char.myCharz().cx;
            Char.myCharz().ySd = Char.myCharz().cy;
            Res.outz("head= " + Char.myCharz().head + " body= " + Char.myCharz().body + " left= " + Char.myCharz().leg + " x= " + Char.myCharz().cx + " y= " + Char.myCharz().cy + " chung toc= " + Char.myCharz().cgender);
            if (Char.myCharz().cx is >= 0 and <= 100)
            {
                Char.myCharz().cdir = 1;
            }
            else if (Char.myCharz().cx >= TileMap.tmw - 100 && Char.myCharz().cx <= TileMap.tmw)
            {
                Char.myCharz().cdir = -1;
            }
            GameCanvas.debug("SA75x4", 2);
            int num = msg.reader().readByte();
            Res.outz("vGo size= " + num);
            if (!GameScr.info1.isDone)
            {
                GameScr.info1.cmx = Char.myCharz().cx - GameScr.cmx;
                GameScr.info1.cmy = Char.myCharz().cy - GameScr.cmy;
            }
            for (int i = 0; i < num; i++)
            {
                Waypoint waypoint = new(msg.reader().readShort(), msg.reader().readShort(), msg.reader().readShort(), msg.reader().readShort(), msg.reader().readBoolean(), msg.reader().readBoolean(), msg.reader().readUTF());
                if ((TileMap.mapID != 21 && TileMap.mapID != 22 && TileMap.mapID != 23) || waypoint.minX < 0 || waypoint.minX <= 24)
                {
                }
            }
            _ = Resources.UnloadUnusedAssets();
            GC.Collect();
            GameCanvas.debug("SA75x5", 2);
            num = msg.reader().readByte();
            Mob.newMob.removeAllElements();
            for (sbyte b = 0; b < num; b = (sbyte)(b + 1))
            {
                Mob mob = new(b, msg.reader().readBoolean(), msg.reader().readBoolean(), msg.reader().readBoolean(), msg.reader().readBoolean(), msg.reader().readBoolean(), msg.reader().readByte(), msg.reader().readByte(), msg.reader().readInt(), msg.reader().readByte(), msg.reader().readInt(), msg.reader().readShort(), msg.reader().readShort(), msg.reader().readByte(), msg.reader().readByte());
                mob.xSd = mob.x;
                mob.ySd = mob.y;
                mob.isBoss = msg.reader().readBoolean();
                if (Mob.arrMobTemplate[mob.templateId].type != 0)
                {
                    mob.dir = b % 3 == 0 ? -1 : 1;
                    mob.x += 10 - (b % 20);
                }
                mob.isMobMe = false;
                BigBoss bigBoss = null;
                BachTuoc bachTuoc = null;
                BigBoss2 bigBoss2 = null;
                NewBoss newBoss = null;
                if (mob.templateId == 70)
                {
                    bigBoss = new BigBoss(b, (short)mob.x, (short)mob.y, 70, mob.hp, mob.maxHp, mob.sys);
                }
                if (mob.templateId == 71)
                {
                    bachTuoc = new BachTuoc(b, (short)mob.x, (short)mob.y, 71, mob.hp, mob.maxHp, mob.sys);
                }
                if (mob.templateId == 72)
                {
                    bigBoss2 = new BigBoss2(b, (short)mob.x, (short)mob.y, 72, mob.hp, mob.maxHp, 3);
                }
                if (mob.isBoss)
                {
                    newBoss = new NewBoss(b, (short)mob.x, (short)mob.y, mob.templateId, mob.hp, mob.maxHp, mob.sys);
                }
                if (newBoss != null)
                {
                    GameScr.vMob.addElement(newBoss);
                }
                else if (bigBoss != null)
                {
                    GameScr.vMob.addElement(bigBoss);
                }
                else if (bachTuoc != null)
                {
                    GameScr.vMob.addElement(bachTuoc);
                }
                else if (bigBoss2 != null)
                {
                    GameScr.vMob.addElement(bigBoss2);
                }
                else
                {
                    GameScr.vMob.addElement(mob);
                }
            }
            for (int j = 0; j < Mob.lastMob.size(); j++)
            {
                string text = (string)Mob.lastMob.elementAt(j);
                if (!Mob.isExistNewMob(text))
                {
                    Mob.arrMobTemplate[int.Parse(text)].data = null;
                    Mob.lastMob.removeElementAt(j);
                    j--;
                }
            }
            if (Char.myCharz().mobMe != null && GameScr.findMobInMap(Char.myCharz().mobMe.mobId) == null)
            {
                Char.myCharz().mobMe.getData();
                Char.myCharz().mobMe.x = Char.myCharz().cx;
                Char.myCharz().mobMe.y = Char.myCharz().cy - 40;
                GameScr.vMob.addElement(Char.myCharz().mobMe);
            }
            num = msg.reader().readByte();
            for (byte b2 = 0; b2 < num; b2 = (byte)(b2 + 1))
            {
            }
            GameCanvas.debug("SA75x6", 2);
            num = msg.reader().readByte();
            Res.outz("NPC size= " + num);
            for (int k = 0; k < num; k++)
            {
                sbyte b3 = msg.reader().readByte();
                short cx = msg.reader().readShort();
                short num2 = msg.reader().readShort();
                sbyte b4 = msg.reader().readByte();
                short num3 = msg.reader().readShort();
                if (b4 != 6 && ((Char.myCharz().taskMaint.taskId >= 7 && (Char.myCharz().taskMaint.taskId != 7 || Char.myCharz().taskMaint.index > 1)) || (b4 != 7 && b4 != 8 && b4 != 9)) && (Char.myCharz().taskMaint.taskId >= 6 || b4 != 16))
                {
                    if (b4 == 4)
                    {
                        GameScr.gI().magicTree = new MagicTree(k, b3, cx, num2, b4, num3);
                        Service.gI().magicTree(2);
                        GameScr.vNpc.addElement(GameScr.gI().magicTree);
                    }
                    else
                    {
                        Npc o = new(k, b3, cx, num2 + 3, b4, num3);
                        GameScr.vNpc.addElement(o);
                    }
                }
            }
            GameCanvas.debug("SA75x7", 2);
            num = msg.reader().readByte();
            Res.outz("item size = " + num);
            for (int l = 0; l < num; l++)
            {
                short itemMapID = msg.reader().readShort();
                short itemTemplateID = msg.reader().readShort();
                int x = msg.reader().readShort();
                int y = msg.reader().readShort();
                int num4 = msg.reader().readInt();
                short r = 0;
                if (num4 == -2)
                {
                    r = msg.reader().readShort();
                }
                ItemMap itemMap = new(num4, itemMapID, itemTemplateID, x, y, r);
                bool flag = false;
                for (int m = 0; m < GameScr.vItemMap.size(); m++)
                {
                    ItemMap itemMap2 = (ItemMap)GameScr.vItemMap.elementAt(m);
                    if (itemMap2.itemMapID == itemMap.itemMapID)
                    {
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                {
                    GameScr.vItemMap.addElement(itemMap);
                }
            }
            TileMap.vCurrItem.removeAllElements();
            if (mGraphics.zoomLevel == 1)
            {
                BgItem.clearHashTable();
            }
            BgItem.vKeysNew.removeAllElements();
            if (!GameCanvas.lowGraphic || (GameCanvas.lowGraphic && TileMap.isVoDaiMap()) || TileMap.mapID == 45 || TileMap.mapID == 46 || TileMap.mapID == 47 || TileMap.mapID == 48)
            {
                short num5 = msg.reader().readShort();
                Res.outz("nItem= " + num5);
                for (int n = 0; n < num5; n++)
                {
                    short id = msg.reader().readShort();
                    short num6 = msg.reader().readShort();
                    short num7 = msg.reader().readShort();
                    if (TileMap.getBIById(id) == null)
                    {
                        continue;
                    }
                    BgItem bIById = TileMap.getBIById(id);
                    BgItem bgItem = new()
                    {
                        id = id,
                        idImage = bIById.idImage,
                        dx = bIById.dx,
                        dy = bIById.dy,
                        x = num6 * TileMap.size,
                        y = num7 * TileMap.size,
                        layer = bIById.layer
                    };
                    if (TileMap.isExistMoreOne(bgItem.id))
                    {
                        bgItem.trans = (n % 2 != 0) ? 2 : 0;
                        if (TileMap.mapID == 45)
                        {
                            bgItem.trans = 0;
                        }
                    }
                    Image image = null;
                    if (!BgItem.imgNew.containsKey(bgItem.idImage + string.Empty))
                    {
                        if (mGraphics.zoomLevel == 1)
                        {
                            image = GameCanvas.loadImage("/mapBackGround/" + bgItem.idImage + ".png");
                            if (image == null)
                            {
                                image = Image.createRGBImage(new int[1], 1, 1, true);
                                Service.gI().getBgTemplate(bgItem.idImage);
                            }
                            BgItem.imgNew.put(bgItem.idImage + string.Empty, image);
                        }
                        else
                        {
                            bool flag2 = false;
                            sbyte[] array = Rms.loadRMS(mGraphics.zoomLevel + "bgItem" + bgItem.idImage);
                            if (array != null)
                            {
                                if (BgItem.newSmallVersion != null)
                                {
                                    Res.outz("Small  last= " + (array.Length % 127) + "new Version= " + BgItem.newSmallVersion[bgItem.idImage]);
                                    if (array.Length % 127 != BgItem.newSmallVersion[bgItem.idImage])
                                    {
                                        flag2 = true;
                                    }
                                }
                                if (!flag2)
                                {
                                    image = Image.createImage(array, 0, array.Length);
                                    if (image != null)
                                    {
                                        BgItem.imgNew.put(bgItem.idImage + string.Empty, image);
                                    }
                                    else
                                    {
                                        flag2 = true;
                                    }
                                }
                            }
                            else
                            {
                                flag2 = true;
                            }
                            if (flag2)
                            {
                                image = GameCanvas.loadImage("/mapBackGround/" + bgItem.idImage + ".png");
                                if (image == null)
                                {
                                    image = Image.createRGBImage(new int[1], 1, 1, true);
                                    Service.gI().getBgTemplate(bgItem.idImage);
                                }
                                BgItem.imgNew.put(bgItem.idImage + string.Empty, image);
                            }
                        }
                        BgItem.vKeysLast.addElement(bgItem.idImage + string.Empty);
                    }
                    if (!BgItem.isExistKeyNews(bgItem.idImage + string.Empty))
                    {
                        BgItem.vKeysNew.addElement(bgItem.idImage + string.Empty);
                    }
                    bgItem.changeColor();
                    TileMap.vCurrItem.addElement(bgItem);
                }
                for (int num8 = 0; num8 < BgItem.vKeysLast.size(); num8++)
                {
                    string text2 = (string)BgItem.vKeysLast.elementAt(num8);
                    if (!BgItem.isExistKeyNews(text2))
                    {
                        BgItem.imgNew.remove(text2);
                        if (BgItem.imgNew.containsKey(text2 + "blend" + 1))
                        {
                            BgItem.imgNew.remove(text2 + "blend" + 1);
                        }
                        if (BgItem.imgNew.containsKey(text2 + "blend" + 3))
                        {
                            BgItem.imgNew.remove(text2 + "blend" + 3);
                        }
                        BgItem.vKeysLast.removeElementAt(num8);
                        num8--;
                    }
                }
                BackgroudEffect.isFog = false;
                BackgroudEffect.nCloud = 0;
                EffecMn.vEff.removeAllElements();
                BackgroudEffect.vBgEffect.removeAllElements();
                Effect.newEff.removeAllElements();
                short num9 = msg.reader().readShort();
                for (int num10 = 0; num10 < num9; num10++)
                {
                    string key = msg.reader().readUTF();
                    string value = msg.reader().readUTF();
                    keyValueAction(key, value);
                }
                for (int num11 = 0; num11 < Effect.lastEff.size(); num11++)
                {
                    string text3 = (string)Effect.lastEff.elementAt(num11);
                    if (!Effect.isExistNewEff(text3))
                    {
                        Effect.removeEffData(int.Parse(text3));
                        Effect.lastEff.removeElementAt(num11);
                        num11--;
                    }
                }
            }
            else
            {
                short num12 = msg.reader().readShort();
                for (int num13 = 0; num13 < num12; num13++)
                {
                    short num14 = msg.reader().readShort();
                    short num15 = msg.reader().readShort();
                    short num16 = msg.reader().readShort();
                }
                short num17 = msg.reader().readShort();
                for (int num18 = 0; num18 < num17; num18++)
                {
                    string text4 = msg.reader().readUTF();
                    string text5 = msg.reader().readUTF();
                }
            }
            TileMap.bgType = msg.reader().readByte();
            sbyte teleport = msg.reader().readByte();
            loadCurrMap(teleport);
            Char.isLoadingMap = false;
            GameCanvas.debug("SA75x8", 2);
            _ = Resources.UnloadUnusedAssets();
            GC.Collect();
            Cout.LogError("----------DA CHAY XONG LOAD INFO MAP");
        }
        catch (Exception ex)
        {
            Pk9rXmap.fixBlackScreen();
            Res.err("LOI TAI LOADMAP INFO " + ex.StackTrace);
        }
    }

    public void keyValueAction(string key, string value)
    {
        if (key.Equals("eff"))
        {
            if (Panel.graphics > 0)
            {
                return;
            }
            string[] array = Res.split(value, ".", 0);
            int id = int.Parse(array[0]);
            int layer = int.Parse(array[1]);
            int x = int.Parse(array[2]);
            int y = int.Parse(array[3]);
            int loop;
            int loopCount;
            if (array.Length <= 4)
            {
                loop = -1;
                loopCount = 1;
            }
            else
            {
                loop = int.Parse(array[4]);
                loopCount = int.Parse(array[5]);
            }
            Effect effect = new(id, x, y, layer, loop, loopCount);
            if (array.Length > 6)
            {
                effect.typeEff = int.Parse(array[6]);
                if (array.Length > 7)
                {
                    effect.indexFrom = int.Parse(array[7]);
                    effect.indexTo = int.Parse(array[8]);
                }
            }
            EffecMn.addEff(effect);
        }
        else if (key.Equals("beff") && Panel.graphics <= 1)
        {
            BackgroudEffect.addEffect(int.Parse(value));
        }
    }

    public void messageNotMap(Message msg)
    {
        GameCanvas.debug("SA6", 2);
        try
        {
            sbyte b = msg.reader().readByte();
            Res.outz("---messageNotMap : " + b);
            switch (b)
            {
                case 16:
                    MoneyCharge.gI().switchToMe();
                    break;
                case 17:
                    GameCanvas.debug("SYB123", 2);
                    Char.myCharz().clearTask();
                    break;
                case 18:
                    {
                        GameCanvas.isLoading = false;
                        GameCanvas.endDlg();
                        int num2 = msg.reader().readInt();
                        GameCanvas.inputDlg.show(mResources.changeNameChar, new Command(mResources.OK, GameCanvas.instance, 88829, num2), TField.INPUT_TYPE_ANY);
                        break;
                    }
                case 20:
                    Char.myCharz().cPk = msg.reader().readByte();
                    GameScr.info1.addInfo(mResources.PK_NOW + " " + Char.myCharz().cPk, 0);
                    break;
                case 35:
                    GameCanvas.endDlg();
                    GameScr.gI().resetButton();
                    GameScr.info1.addInfo(msg.reader().readUTF(), 0);
                    break;
                case 36:
                    GameScr.typeActive = msg.reader().readByte();
                    Res.outz("load Me Active: " + GameScr.typeActive);
                    break;
                case 4:
                    {
                        GameCanvas.debug("SA8", 2);
                        GameCanvas.loginScr.savePass();
                        GameScr.isAutoPlay = false;
                        GameScr.canAutoPlay = false;
                        LoginScr.isUpdateAll = true;
                        LoginScr.isUpdateData = true;
                        LoginScr.isUpdateMap = true;
                        LoginScr.isUpdateSkill = true;
                        LoginScr.isUpdateItem = true;
                        GameScr.vsData = msg.reader().readByte();
                        GameScr.vsMap = msg.reader().readByte();
                        GameScr.vsSkill = msg.reader().readByte();
                        GameScr.vsItem = msg.reader().readByte();
                        sbyte b3 = msg.reader().readByte();
                        if (GameCanvas.loginScr.isLogin2)
                        {
                            Rms.saveRMSString("acc", string.Empty);
                            Rms.saveRMSString("pass", string.Empty);
                        }
                        else
                        {
                            Rms.saveRMSString("userAo" + ServerListScreen.ipSelect, string.Empty);
                        }
                        if (GameScr.vsData != GameScr.vcData)
                        {
                            GameScr.isLoadAllData = false;
                            Service.gI().updateData();
                        }
                        else
                        {
                            try
                            {
                                LoginScr.isUpdateData = false;
                            }
                            catch (Exception)
                            {
                                GameScr.vcData = -1;
                                Service.gI().updateData();
                            }
                        }
                        if (GameScr.vsMap != GameScr.vcMap)
                        {
                            GameScr.isLoadAllData = false;
                            Service.gI().updateMap();
                        }
                        else
                        {
                            try
                            {
                                if (!GameScr.isLoadAllData)
                                {
                                    DataInputStream dataInputStream = new(Rms.loadRMS("NRmap"));
                                    createMap(dataInputStream.r);
                                }
                                LoginScr.isUpdateMap = false;
                            }
                            catch (Exception)
                            {
                                GameScr.vcMap = -1;
                                Service.gI().updateMap();
                            }
                        }
                        if (GameScr.vsSkill != GameScr.vcSkill)
                        {
                            GameScr.isLoadAllData = false;
                            Service.gI().updateSkill();
                        }
                        else
                        {
                            try
                            {
                                if (!GameScr.isLoadAllData)
                                {
                                    DataInputStream dataInputStream2 = new(Rms.loadRMS("NRskill"));
                                    createSkill(dataInputStream2.r);
                                }
                                LoginScr.isUpdateSkill = false;
                            }
                            catch (Exception)
                            {
                                GameScr.vcSkill = -1;
                                Service.gI().updateSkill();
                            }
                        }
                        if (GameScr.vsItem != GameScr.vcItem)
                        {
                            GameScr.isLoadAllData = false;
                            Service.gI().updateItem();
                        }
                        else
                        {
                            try
                            {
                                DataInputStream dataInputStream3 = new(Rms.loadRMS("NRitem0"));
                                loadItemNew(dataInputStream3.r, 0, false);
                                DataInputStream dataInputStream4 = new(Rms.loadRMS("NRitem1"));
                                loadItemNew(dataInputStream4.r, 1, false);
                                DataInputStream dataInputStream5 = new(Rms.loadRMS("NRitem2"));
                                loadItemNew(dataInputStream5.r, 2, false);
                                DataInputStream dataInputStream6 = new(Rms.loadRMS("NRitem100"));
                                loadItemNew(dataInputStream6.r, 100, false);
                                LoginScr.isUpdateItem = false;
                            }
                            catch (Exception)
                            {
                                GameScr.vcItem = -1;
                                Service.gI().updateItem();
                            }
                        }
                        if (GameScr.vsData == GameScr.vcData && GameScr.vsMap == GameScr.vcMap && GameScr.vsSkill == GameScr.vcSkill && GameScr.vsItem == GameScr.vcItem)
                        {
                            if (!GameScr.isLoadAllData)
                            {
                                GameScr.gI().readDart();
                                GameScr.gI().readEfect();
                                GameScr.gI().readArrow();
                                GameScr.gI().readSkill();

                            }
                            Service.gI().clientOk();
                        }

                        sbyte b4 = msg.reader().readByte();
                        Res.outz("CAPTION LENT= " + b4);
                        GameScr.exps = new long[b4];
                        for (int j = 0; j < GameScr.exps.Length; j++)
                        {
                            GameScr.exps[j] = msg.reader().readLong();
                        }
                        break;
                    }
                case 6:
                    {
                        Res.outz("GET UPDATE_MAP " + msg.reader().available() + " bytes");
                        msg.reader().mark(100000);
                        createMap(msg.reader());
                        msg.reader().reset();
                        sbyte[] data3 = new sbyte[msg.reader().available()];
                        msg.reader().readFully(ref data3);
                        Rms.saveRMS("NRmap", data3);
                        sbyte[] data4 = new sbyte[1] { GameScr.vcMap };
                        Rms.saveRMS("NRmapVersion", data4);
                        LoginScr.isUpdateMap = false;
                        if (GameScr.vsData == GameScr.vcData && GameScr.vsMap == GameScr.vcMap && GameScr.vsSkill == GameScr.vcSkill && GameScr.vsItem == GameScr.vcItem)
                        {
                            GameScr.gI().readDart();
                            GameScr.gI().readEfect();
                            GameScr.gI().readArrow();
                            GameScr.gI().readSkill();
                            Service.gI().clientOk();
                        }
                        break;
                    }
                case 7:
                    {
                        Res.outz("GET UPDATE_SKILL " + msg.reader().available() + " bytes");
                        msg.reader().mark(100000);
                        createSkill(msg.reader());
                        msg.reader().reset();
                        sbyte[] data = new sbyte[msg.reader().available()];
                        msg.reader().readFully(ref data);
                        Rms.saveRMS("NRskill", data);
                        sbyte[] data2 = new sbyte[1] { GameScr.vcSkill };
                        Rms.saveRMS("NRskillVersion", data2);
                        LoginScr.isUpdateSkill = false;
                        if (GameScr.vsData == GameScr.vcData && GameScr.vsMap == GameScr.vcMap && GameScr.vsSkill == GameScr.vcSkill && GameScr.vsItem == GameScr.vcItem)
                        {
                            GameScr.gI().readDart();
                            GameScr.gI().readEfect();
                            GameScr.gI().readArrow();
                            GameScr.gI().readSkill();
                            Service.gI().clientOk();
                        }
                        break;
                    }
                case 8:
                    Res.outz("GET UPDATE_ITEM " + msg.reader().available() + " bytes");
                    createItemNew(msg.reader());
                    break;
                case 10:
                    try
                    {
                        Char.isLoadingMap = true;
                        Res.outz("REQUEST MAP TEMPLATE");
                        GameCanvas.isLoading = true;
                        TileMap.maps = null;
                        TileMap.types = null;
                        mSystem.gcc();
                        GameCanvas.debug("SA99", 2);
                        TileMap.tmw = msg.reader().readByte();
                        TileMap.tmh = msg.reader().readByte();
                        TileMap.maps = new int[TileMap.tmw * TileMap.tmh];
                        Res.outz("   M apsize= " + (TileMap.tmw * TileMap.tmh));
                        for (int i = 0; i < TileMap.maps.Length; i++)
                        {
                            int num = msg.reader().readByte();
                            if (num < 0)
                            {
                                num += 256;
                            }
                            TileMap.maps[i] = (ushort)num;
                        }
                        TileMap.types = new int[TileMap.maps.Length];
                        msg = messWait;
                        loadInfoMap(msg);
                        try
                        {
                            sbyte b2 = msg.reader().readByte();
                            TileMap.isMapDouble = b2 != 0;
                        }
                        catch (Exception)
                        {
                        }
                    }
                    catch (Exception ex2)
                    {
                        Cout.LogError("LOI TAI CASE REQUEST_MAPTEMPLATE " + ex2.ToString());
                    }
                    msg.cleanup();
                    messWait.cleanup();
                    msg = messWait = null;
                    break;
                case 12:
                    GameCanvas.debug("SA10", 2);
                    break;
                case 9:
                    GameCanvas.debug("SA11", 2);
                    break;
            }
        }
        catch (Exception)
        {
            Cout.LogError("LOI TAI messageNotMap + " + msg.command);
        }
        finally
        {
            msg?.cleanup();
        }
    }

    public void messageNotLogin(Message msg)
    {
        try
        {
            sbyte b = msg.reader().readByte();
            Res.outz("---messageNotLogin : " + b);
            if (b != 2)
            {
                return;
            }
            string text = msg.reader().readUTF();
            if (mSystem.isTest)
            {
                text = "88:192.168.1.88:20000:0,53:112.213.85.53:20000:0," + text;
            }
            if (Rms.loadRMSInt("AdminLink") == 1)
            {
                return;
            }
            ServerListScreen.linkDefault = mSystem.clientType == 1 ? text : text;
            ServerListScreen.getServerList(ServerListScreen.linkDefault);
            try
            {
                sbyte b2 = msg.reader().readByte();
                Panel.CanNapTien = b2 == 1;
                sbyte b3 = msg.reader().readByte();
                Rms.saveRMSInt("AdminLink", b3);
            }
            catch (Exception)
            {
            }
        }
        catch (Exception)
        {
        }
        finally
        {
            msg?.cleanup();
        }
    }

    public void messageSubCommand(Message msg)
    {
        try
        {
            GameCanvas.debug("SA12", 2);
            sbyte b = msg.reader().readByte();
            Res.outz("---messageSubCommand : " + b);
            switch (b)
            {
                case 63:
                    {
                        sbyte b7 = msg.reader().readByte();
                        if (b7 > 0)
                        {
                            GameCanvas.panel.vPlayerMenu_id.removeAllElements();
                            InfoDlg.showWait();
                            MyVector vPlayerMenu = GameCanvas.panel.vPlayerMenu;
                            for (int num17 = 0; num17 < b7; num17++)
                            {
                                string caption = msg.reader().readUTF();
                                string caption2 = msg.reader().readUTF();
                                short num18 = msg.reader().readShort();
                                GameCanvas.panel.vPlayerMenu_id.addElement(num18 + string.Empty);
                                Char.myCharz().charFocus.menuSelect = num18;
                                Command command = new(caption, 11115, Char.myCharz().charFocus)
                                {
                                    caption2 = caption2
                                };
                                vPlayerMenu.addElement(command);
                            }
                            InfoDlg.hide();
                            GameCanvas.panel.setTabPlayerMenu();
                        }
                        break;
                    }
                case 1:
                    GameCanvas.debug("SA13", 2);
                    Char.myCharz().nClass = GameScr.nClasss[msg.reader().readByte()];
                    Char.myCharz().cTiemNang = msg.reader().readLong();
                    Char.myCharz().vSkill.removeAllElements();
                    Char.myCharz().vSkillFight.removeAllElements();
                    Char.myCharz().myskill = null;
                    break;
                case 2:
                    {
                        GameCanvas.debug("SA14", 2);
                        if (Char.myCharz().statusMe is not 14 and not 5)
                        {
                            Char.myCharz().cHP = Char.myCharz().cHPFull;
                            Char.myCharz().cMP = Char.myCharz().cMPFull;
                            Cout.LogError2(" ME_LOAD_SKILL");
                        }
                        Char.myCharz().vSkill.removeAllElements();
                        Char.myCharz().vSkillFight.removeAllElements();
                        sbyte b2 = msg.reader().readByte();
                        for (sbyte b3 = 0; b3 < b2; b3 = (sbyte)(b3 + 1))
                        {
                            short skillId = msg.reader().readShort();
                            Skill skill2 = Skills.get(skillId);
                            useSkill(skill2);
                        }
                        GameScr.gI().sortSkill();
                        if (GameScr.isPaintInfoMe)
                        {
                            GameScr.indexRow = -1;
                            GameScr.gI().left = GameScr.gI().center = null;
                        }
                        break;
                    }
                case 19:
                    GameCanvas.debug("SA17", 2);
                    Char.myCharz().boxSort();
                    break;
                case 21:
                    {
                        GameCanvas.debug("SA19", 2);
                        int num3 = msg.reader().readInt();
                        Char.myCharz().xuInBox -= num3;
                        Char.myCharz().xu += num3;
                        Char.myCharz().xuStr = mSystem.numberTostring(Char.myCharz().xu);
                        break;
                    }
                case 0:
                    {
                        GameCanvas.debug("SA21", 2);
                        RadarScr.list = new MyVector();
                        Teleport.vTeleport.removeAllElements();
                        GameScr.vCharInMap.removeAllElements();
                        GameScr.vItemMap.removeAllElements();
                        Char.vItemTime.removeAllElements();
                        GameScr.loadImg();
                        GameScr.currentCharViewInfo = Char.myCharz();
                        Char.myCharz().charID = msg.reader().readInt();
                        Char.myCharz().ctaskId = msg.reader().readByte();
                        Char.myCharz().cgender = msg.reader().readByte();
                        Char.myCharz().head = msg.reader().readShort();
                        Char.myCharz().cName = msg.reader().readUTF();
                        Char.myCharz().cPk = msg.reader().readByte();
                        Char.myCharz().cTypePk = msg.reader().readByte();
                        Char.myCharz().cPower = msg.reader().readLong();
                        Char.myCharz().applyCharLevelPercent();
                        Char.myCharz().eff5BuffHp = msg.reader().readShort();
                        Char.myCharz().eff5BuffMp = msg.reader().readShort();
                        Char.myCharz().nClass = GameScr.nClasss[msg.reader().readByte()];
                        Char.myCharz().vSkill.removeAllElements();
                        Char.myCharz().vSkillFight.removeAllElements();
                        GameScr.gI().dHP = Char.myCharz().cHP;
                        GameScr.gI().dMP = Char.myCharz().cMP;
                        sbyte b2 = msg.reader().readByte();
                        for (sbyte b6 = 0; b6 < b2; b6 = (sbyte)(b6 + 1))
                        {
                            Skill skill3 = Skills.get(msg.reader().readShort());
                            useSkill(skill3);
                        }
                        GameScr.gI().sortSkill();
                        GameScr.gI().loadSkillShortcut();
                        Char.myCharz().xu = msg.reader().readLong();
                        Char.myCharz().luongKhoa = msg.reader().readInt();
                        Char.myCharz().luong = msg.reader().readInt();
                        Char.myCharz().xuStr = NinjaUtil.getMoneys(Char.myCharz().xu);
                        Char.myCharz().luongStr = mSystem.numberTostring(Char.myCharz().luong);
                        Char.myCharz().luongKhoaStr = mSystem.numberTostring(Char.myCharz().luongKhoa);
                        Char.myCharz().arrItemBody = new Item[msg.reader().readByte()];
                        try
                        {
                            Char.myCharz().setDefaultPart();
                            for (int k = 0; k < Char.myCharz().arrItemBody.Length; k++)
                            {
                                short num5 = msg.reader().readShort();
                                if (num5 == -1)
                                {
                                    continue;
                                }
                                ItemTemplate itemTemplate = ItemTemplates.get(num5);
                                int num6 = itemTemplate.type;
                                Char.myCharz().arrItemBody[k] = new Item
                                {
                                    template = itemTemplate,
                                    quantity = msg.reader().readInt(),
                                    info = msg.reader().readUTF(),
                                    content = msg.reader().readUTF()
                                };
                                int num7 = msg.reader().readUnsignedByte();
                                if (num7 != 0)
                                {
                                    Char.myCharz().arrItemBody[k].itemOption = new ItemOption[num7];
                                    for (int l = 0; l < Char.myCharz().arrItemBody[k].itemOption.Length; l++)
                                    {
                                        int num8 = msg.reader().readUnsignedByte();
                                        int param = msg.reader().readUnsignedShort();
                                        if (num8 != -1)
                                        {
                                            Char.myCharz().arrItemBody[k].itemOption[l] = new ItemOption(num8, param);
                                        }
                                    }
                                }
                                switch (num6)
                                {
                                    case 0:
                                        Res.outz("toi day =======================================" + Char.myCharz().body);
                                        Char.myCharz().body = Char.myCharz().arrItemBody[k].template.part;
                                        break;
                                    case 1:
                                        Char.myCharz().leg = Char.myCharz().arrItemBody[k].template.part;
                                        Res.outz("toi day =======================================" + Char.myCharz().leg);
                                        break;
                                }
                            }
                        }
                        catch (Exception)
                        {
                        }
                        Char.myCharz().arrItemBag = new Item[msg.reader().readByte()];
                        GameScr.hpPotion = 0;
                        for (int m = 0; m < Char.myCharz().arrItemBag.Length; m++)
                        {
                            short num9 = msg.reader().readShort();
                            if (num9 == -1)
                            {
                                continue;
                            }
                            Char.myCharz().arrItemBag[m] = new Item
                            {
                                template = ItemTemplates.get(num9),
                                quantity = msg.reader().readInt(),
                                info = msg.reader().readUTF(),
                                content = msg.reader().readUTF(),
                                indexUI = m
                            };
                            sbyte b7 = msg.reader().readByte();
                            if (b7 != 0)
                            {
                                Char.myCharz().arrItemBag[m].itemOption = new ItemOption[b7];
                                for (int n = 0; n < Char.myCharz().arrItemBag[m].itemOption.Length; n++)
                                {
                                    int num10 = msg.reader().readUnsignedByte();
                                    int param2 = msg.reader().readUnsignedShort();
                                    if (num10 != -1)
                                    {
                                        Char.myCharz().arrItemBag[m].itemOption[n] = new ItemOption(num10, param2);
                                        Char.myCharz().arrItemBag[m].getCompare();
                                    }
                                }
                            }
                            if (Char.myCharz().arrItemBag[m].template.type == 6)
                            {
                                GameScr.hpPotion += Char.myCharz().arrItemBag[m].quantity;
                            }
                        }
                        Char.myCharz().arrItemBox = new Item[msg.reader().readByte()];
                        GameCanvas.panel.hasUse = 0;
                        for (int num11 = 0; num11 < Char.myCharz().arrItemBox.Length; num11++)
                        {
                            short num12 = msg.reader().readShort();
                            if (num12 == -1)
                            {
                                continue;
                            }
                            Char.myCharz().arrItemBox[num11] = new Item
                            {
                                template = ItemTemplates.get(num12),
                                quantity = msg.reader().readInt(),
                                info = msg.reader().readUTF(),
                                content = msg.reader().readUTF(),
                                itemOption = new ItemOption[msg.reader().readByte()]
                            };
                            for (int num13 = 0; num13 < Char.myCharz().arrItemBox[num11].itemOption.Length; num13++)
                            {
                                int num14 = msg.reader().readUnsignedByte();
                                int param3 = msg.reader().readUnsignedShort();
                                if (num14 != -1)
                                {
                                    Char.myCharz().arrItemBox[num11].itemOption[num13] = new ItemOption(num14, param3);
                                    Char.myCharz().arrItemBox[num11].getCompare();
                                }
                            }
                            GameCanvas.panel.hasUse++;
                        }
                        Char.myCharz().statusMe = 4;
                        int num15 = Rms.loadRMSInt(Char.myCharz().cName + "vci");
                        GameScr.isViewClanInvite = num15 >= 1;
                        short num16 = msg.reader().readShort();
                        Char.idHead = new short[num16];
                        Char.idAvatar = new short[num16];
                        for (int num17 = 0; num17 < num16; num17++)
                        {
                            Char.idHead[num17] = msg.reader().readShort();
                            Char.idAvatar[num17] = msg.reader().readShort();
                        }
                        for (int num18 = 0; num18 < GameScr.info1.charId.Length; num18++)
                        {
                            GameScr.info1.charId[num18] = new int[3];
                        }
                        GameScr.info1.charId[Char.myCharz().cgender][0] = msg.reader().readShort();
                        GameScr.info1.charId[Char.myCharz().cgender][1] = msg.reader().readShort();
                        GameScr.info1.charId[Char.myCharz().cgender][2] = msg.reader().readShort();
                        Char.myCharz().isNhapThe = msg.reader().readByte() == 1;
                        Res.outz("NHAP THE= " + Char.myCharz().isNhapThe);
                        GameScr.deltaTime = mSystem.currentTimeMillis() - (msg.reader().readInt() * 1000L);
                        GameScr.isNewMember = msg.reader().readByte();
                        Service.gI().updateCaption((sbyte)Char.myCharz().cgender);
                        Service.gI().androidPack();
                        try
                        {
                            Char.myCharz().idAuraEff = msg.reader().readShort();
                            Char.myCharz().idEff_Set_Item = msg.reader().readSByte();
                            Char.myCharz().idHat = msg.reader().readShort();
                            break;
                        }
                        catch (Exception)
                        {
                            break;
                        }
                    }
                case 4:
                    GameCanvas.debug("SA23", 2);
                    Char.myCharz().xu = msg.reader().readLong();
                    Char.myCharz().luong = msg.reader().readInt();
                    Char.myCharz().cHP = msg.readInt3Byte();
                    Char.myCharz().cMP = msg.readInt3Byte();
                    Char.myCharz().luongKhoa = msg.reader().readInt();
                    Char.myCharz().xuStr = mSystem.numberTostring(Char.myCharz().xu);
                    Char.myCharz().luongStr = mSystem.numberTostring(Char.myCharz().luong);
                    Char.myCharz().luongKhoaStr = mSystem.numberTostring(Char.myCharz().luongKhoa);
                    break;
                case 5:
                    {
                        GameCanvas.debug("SA24", 2);
                        int cHP = Char.myCharz().cHP;
                        Char.myCharz().cHP = msg.readInt3Byte();
                        if (Char.myCharz().cHP > cHP && Char.myCharz().cTypePk != 4)
                        {
                            GameScr.startFlyText("+" + (Char.myCharz().cHP - cHP) + " " + mResources.HP, Char.myCharz().cx, Char.myCharz().cy - Char.myCharz().ch - 20, 0, -1, mFont.HP);
                            SoundMn.gI().HP_MPup();
                            if (Char.myCharz().petFollow != null && Char.myCharz().petFollow.smallID == 5003)
                            {
                                MonsterDart.addMonsterDart(Char.myCharz().petFollow.cmx + ((Char.myCharz().petFollow.dir != 1) ? (-10) : 10), Char.myCharz().petFollow.cmy + 10, true, -1, -1, Char.myCharz(), 29);
                            }
                        }
                        if (Char.myCharz().cHP < cHP)
                        {
                            GameScr.startFlyText("-" + (cHP - Char.myCharz().cHP) + " " + mResources.HP, Char.myCharz().cx, Char.myCharz().cy - Char.myCharz().ch - 20, 0, -1, mFont.HP);
                        }
                        GameScr.gI().dHP = Char.myCharz().cHP;
                        if (GameScr.isPaintInfoMe)
                        {
                        }
                        break;
                    }
                case 6:
                    {
                        GameCanvas.debug("SA25", 2);
                        if (Char.myCharz().statusMe is 14 or 5)
                        {
                            break;
                        }
                        int cMP = Char.myCharz().cMP;
                        Char.myCharz().cMP = msg.readInt3Byte();
                        if (Char.myCharz().cMP > cMP)
                        {
                            GameScr.startFlyText("+" + (Char.myCharz().cMP - cMP) + " " + mResources.KI, Char.myCharz().cx, Char.myCharz().cy - Char.myCharz().ch - 23, 0, -2, mFont.MP);
                            SoundMn.gI().HP_MPup();
                            if (Char.myCharz().petFollow != null && Char.myCharz().petFollow.smallID == 5001)
                            {
                                MonsterDart.addMonsterDart(Char.myCharz().petFollow.cmx + ((Char.myCharz().petFollow.dir != 1) ? (-10) : 10), Char.myCharz().petFollow.cmy + 10, true, -1, -1, Char.myCharz(), 29);
                            }
                        }
                        if (Char.myCharz().cMP < cMP)
                        {
                            GameScr.startFlyText("-" + (cMP - Char.myCharz().cMP) + " " + mResources.KI, Char.myCharz().cx, Char.myCharz().cy - Char.myCharz().ch - 23, 0, -2, mFont.MP);
                        }
                        Res.outz("curr MP= " + Char.myCharz().cMP);
                        GameScr.gI().dMP = Char.myCharz().cMP;
                        if (GameScr.isPaintInfoMe)
                        {
                        }
                        break;
                    }
                case 7:
                    {
                        Char @char = GameScr.findCharInMap(msg.reader().readInt());
                        if (@char == null)
                        {
                            break;
                        }
                        @char.clanID = msg.reader().readInt();
                        if (@char.clanID == -2)
                        {
                            @char.isCopy = true;
                        }
                        _ = readCharInfo(@char, msg);
                        try
                        {
                            @char.idAuraEff = msg.reader().readShort();
                            @char.idEff_Set_Item = msg.reader().readSByte();
                            @char.idHat = msg.reader().readShort();
                            if (@char.bag >= 201)
                            {
                                Effect effect = new(@char.bag, @char, 2, -1, 10, 1)
                                {
                                    typeEff = 5
                                };
                                @char.addEffChar(effect);
                            }
                            else
                            {
                                @char.removeEffChar(0, 201);
                            }
                            break;
                        }
                        catch (Exception)
                        {
                            break;
                        }
                    }
                case 8:
                    {
                        GameCanvas.debug("SA26", 2);
                        Char @char = GameScr.findCharInMap(msg.reader().readInt());
                        if (@char != null)
                        {
                            @char.cspeed = msg.reader().readByte();
                        }
                        break;
                    }
                case 9:
                    {
                        GameCanvas.debug("SA27", 2);
                        Char @char = GameScr.findCharInMap(msg.reader().readInt());
                        if (@char != null)
                        {
                            @char.cHP = msg.readInt3Byte();
                            @char.cHPFull = msg.readInt3Byte();
                        }
                        break;
                    }
                case 10:
                    {
                        GameCanvas.debug("SA28", 2);
                        Char @char = GameScr.findCharInMap(msg.reader().readInt());
                        if (@char != null)
                        {
                            @char.cHP = msg.readInt3Byte();
                            @char.cHPFull = msg.readInt3Byte();
                            @char.eff5BuffHp = msg.reader().readShort();
                            @char.eff5BuffMp = msg.reader().readShort();
                            @char.wp = msg.reader().readShort();
                            if (@char.wp == -1)
                            {
                                @char.setDefaultWeapon();
                            }
                        }
                        break;
                    }
                case 11:
                    {
                        GameCanvas.debug("SA29", 2);
                        Char @char = GameScr.findCharInMap(msg.reader().readInt());
                        if (@char != null)
                        {
                            @char.cHP = msg.readInt3Byte();
                            @char.cHPFull = msg.readInt3Byte();
                            @char.eff5BuffHp = msg.reader().readShort();
                            @char.eff5BuffMp = msg.reader().readShort();
                            @char.body = msg.reader().readShort();
                            if (@char.body == -1)
                            {
                                @char.setDefaultBody();
                            }
                        }
                        break;
                    }
                case 12:
                    {
                        GameCanvas.debug("SA30", 2);
                        Char @char = GameScr.findCharInMap(msg.reader().readInt());
                        if (@char != null)
                        {
                            @char.cHP = msg.readInt3Byte();
                            @char.cHPFull = msg.readInt3Byte();
                            @char.eff5BuffHp = msg.reader().readShort();
                            @char.eff5BuffMp = msg.reader().readShort();
                            @char.leg = msg.reader().readShort();
                            if (@char.leg == -1)
                            {
                                @char.setDefaultLeg();
                            }
                        }
                        break;
                    }
                case 13:
                    {
                        GameCanvas.debug("SA31", 2);
                        int num2 = msg.reader().readInt();
                        Char @char = (num2 != Char.myCharz().charID) ? GameScr.findCharInMap(num2) : Char.myCharz();
                        if (@char != null)
                        {
                            @char.cHP = msg.readInt3Byte();
                            @char.cHPFull = msg.readInt3Byte();
                            @char.eff5BuffHp = msg.reader().readShort();
                            @char.eff5BuffMp = msg.reader().readShort();
                        }
                        break;
                    }
                case 14:
                    {
                        GameCanvas.debug("SA32", 2);
                        Char @char = GameScr.findCharInMap(msg.reader().readInt());
                        if (@char == null)
                        {
                            break;
                        }
                        @char.cHP = msg.readInt3Byte();
                        sbyte b4 = msg.reader().readByte();
                        Res.outz("player load hp type= " + b4);
                        if (b4 == 1)
                        {
                            ServerEffect.addServerEffect(11, @char, 5);
                            ServerEffect.addServerEffect(104, @char, 4);
                        }
                        if (b4 == 2)
                        {
                            @char.doInjure();
                        }
                        try
                        {
                            @char.cHPFull = msg.readInt3Byte();
                            break;
                        }
                        catch (Exception)
                        {
                            break;
                        }
                    }
                case 15:
                    {
                        GameCanvas.debug("SA33", 2);
                        Char @char = GameScr.findCharInMap(msg.reader().readInt());
                        if (@char != null)
                        {
                            @char.cHP = msg.readInt3Byte();
                            @char.cHPFull = msg.readInt3Byte();
                            @char.cx = msg.reader().readShort();
                            @char.cy = msg.reader().readShort();
                            @char.statusMe = 1;
                            @char.cp3 = 3;
                            ServerEffect.addServerEffect(109, @char, 2);
                        }
                        break;
                    }
                case 35:
                    {
                        GameCanvas.debug("SY3", 2);
                        int num4 = msg.reader().readInt();
                        Res.outz("CID = " + num4);
                        if (TileMap.mapID == 130)
                        {
                            GameScr.gI().starVS();
                        }
                        if (num4 == Char.myCharz().charID)
                        {
                            Char.myCharz().cTypePk = msg.reader().readByte();
                            if (GameScr.gI().isVS() && Char.myCharz().cTypePk != 0)
                            {
                                GameScr.gI().starVS();
                            }
                            Res.outz("type pk= " + Char.myCharz().cTypePk);
                            Char.myCharz().npcFocus = null;
                            if (!GameScr.gI().isMeCanAttackMob(Char.myCharz().mobFocus))
                            {
                                Char.myCharz().mobFocus = null;
                            }
                            Char.myCharz().itemFocus = null;
                        }
                        else
                        {
                            Char @char = GameScr.findCharInMap(num4);
                            if (@char != null)
                            {
                                Res.outz("type pk= " + @char.cTypePk);
                                @char.cTypePk = msg.reader().readByte();
                                if (@char.isAttacPlayerStatus())
                                {
                                    Char.myCharz().charFocus = @char;
                                }
                            }
                        }
                        for (int i = 0; i < GameScr.vCharInMap.size(); i++)
                        {
                            Char char2 = GameScr.findCharInMap(i);
                            if (char2 != null && char2.cTypePk != 0 && char2.cTypePk == Char.myCharz().cTypePk)
                            {
                                if (!Char.myCharz().mobFocus.isMobMe)
                                {
                                    Char.myCharz().mobFocus = null;
                                }
                                Char.myCharz().npcFocus = null;
                                Char.myCharz().itemFocus = null;
                                break;
                            }
                        }
                        Res.outz("update type pk= ");
                        break;
                    }
                case 61:
                    {
                        string text = msg.reader().readUTF();
                        sbyte[] data = new sbyte[msg.reader().readInt()];
                        _ = msg.reader().read(ref data);
                        if (data.Length == 0)
                        {
                            data = null;
                        }
                        if (text.Equals("KSkill"))
                        {
                            GameScr.gI().onKSkill(data);
                        }
                        else if (text.Equals("OSkill"))
                        {
                            GameScr.gI().onOSkill(data);
                        }
                        else if (text.Equals("CSkill"))
                        {
                            GameScr.gI().onCSkill(data);
                        }
                        break;
                    }
                case 23:
                    {
                        short num = msg.reader().readShort();
                        Skill skill = Skills.get(num);
                        useSkill(skill);
                        if (num is not 0 and not 14 and not 28)
                        {
                            GameScr.info1.addInfo(mResources.LEARN_SKILL + " " + skill.template.name, 0);
                        }
                        break;
                    }
                case 62:
                    Res.outz("ME UPDATE SKILL");
                    read_UpdateSkill(msg);
                    break;
            }
        }
        catch (Exception ex5)
        {
            Cout.println("Loi tai Sub : " + ex5.ToString());
        }
        finally
        {
            msg?.cleanup();
        }
    }

    private void useSkill(Skill skill)
    {
        if (Char.myCharz().myskill == null)
        {
            Char.myCharz().myskill = skill;
        }
        else if (skill.template.Equals(Char.myCharz().myskill.template))
        {
            Char.myCharz().myskill = skill;
        }
        Char.myCharz().vSkill.addElement(skill);
        if ((skill.template.type == 1 || skill.template.type == 4 || skill.template.type == 2 || skill.template.type == 3) && (skill.template.maxPoint == 0 || (skill.template.maxPoint > 0 && skill.point > 0)))
        {
            if (skill.template.id == Char.myCharz().skillTemplateId)
            {
                Service.gI().selectSkill(Char.myCharz().skillTemplateId);
            }
            Char.myCharz().vSkillFight.addElement(skill);
        }
    }

    public bool readCharInfo(Char c, Message msg)
    {
        try
        {
            c.clevel = msg.reader().readByte();
            c.isInvisiblez = msg.reader().readBoolean();
            c.cTypePk = msg.reader().readByte();
            Res.outz("ADD TYPE PK= " + c.cTypePk + " to player " + c.charID + " @@ " + c.cName);
            c.nClass = GameScr.nClasss[msg.reader().readByte()];
            c.cgender = msg.reader().readByte();
            c.head = msg.reader().readShort();
            c.cName = msg.reader().readUTF();
            c.cHP = msg.readInt3Byte();
            c.dHP = c.cHP;
            if (c.cHP == 0)
            {
                c.statusMe = 14;
            }
            c.cHPFull = msg.readInt3Byte();
            if (c.cy >= TileMap.pxh - 100)
            {
                c.isFlyUp = true;
            }
            c.body = msg.reader().readShort();
            c.leg = msg.reader().readShort();
            c.bag = msg.reader().readUnsignedByte();
            Res.outz(" body= " + c.body + " leg= " + c.leg + " bag=" + c.bag + "BAG ==" + c.bag + "*********************************");
            c.isShadown = true;
            sbyte b = msg.reader().readByte();
            if (c.wp == -1)
            {
                c.setDefaultWeapon();
            }
            if (c.body == -1)
            {
                c.setDefaultBody();
            }
            if (c.leg == -1)
            {
                c.setDefaultLeg();
            }
            c.cx = msg.reader().readShort();
            c.cy = msg.reader().readShort();
            c.xSd = c.cx;
            c.ySd = c.cy;
            c.eff5BuffHp = msg.reader().readShort();
            c.eff5BuffMp = msg.reader().readShort();
            int num = msg.reader().readByte();
            for (int i = 0; i < num; i++)
            {
                EffectChar effectChar = new(msg.reader().readByte(), msg.reader().readInt(), msg.reader().readInt(), msg.reader().readShort());
                c.vEff.addElement(effectChar);
                if (effectChar.template.type is 12 or 11)
                {
                    c.isInvisiblez = true;
                }
            }
            return true;
        }
        catch (Exception ex)
        {
            _ = ex.StackTrace.ToString();
        }
        return false;
    }

    private void readGetImgByName(Message msg)
    {
        try
        {
            string text = msg.reader().readUTF();
            sbyte nFrame = msg.reader().readByte();
            sbyte[] array = null;
            array = NinjaUtil.readByteArray(msg);
            Image img = createImage(array);
            ImgByName.SetImage(text, img, nFrame);
            if (array != null)
            {
                ImgByName.saveRMS(text, nFrame, array);
            }
        }
        catch (Exception)
        {
        }
    }

    private void createItemNew(myReader d)
    {
        try
        {
            loadItemNew(d, -1, true);
        }
        catch (Exception)
        {
        }
    }

    private void loadItemNew(myReader d, sbyte type, bool isSave)
    {
        try
        {
            d.mark(100000);
            GameScr.vcItem = d.readByte();
            type = d.readByte();
            if (type == 0)
            {
                GameScr.gI().iOptionTemplates = new ItemOptionTemplate[d.readUnsignedByte()];
                for (int i = 0; i < GameScr.gI().iOptionTemplates.Length; i++)
                {
                    GameScr.gI().iOptionTemplates[i] = new ItemOptionTemplate
                    {
                        id = i,
                        name = d.readUTF(),
                        type = d.readByte()
                    };
                }
                if (isSave)
                {
                    d.reset();
                    sbyte[] data = new sbyte[d.available()];
                    d.readFully(ref data);
                    Rms.saveRMS("NRitem0", data);
                }
            }
            else if (type == 1)
            {
                ItemTemplates.itemTemplates.clear();
                int num = d.readShort();
                for (int j = 0; j < num; j++)
                {
                    ItemTemplate it = new((short)j, d.readByte(), d.readByte(), d.readUTF(), d.readUTF(), d.readByte(), d.readInt(), d.readShort(), d.readShort(), d.readBoolean());
                    ItemTemplates.add(it);
                }
                if (isSave)
                {
                    d.reset();
                    sbyte[] data2 = new sbyte[d.available()];
                    d.readFully(ref data2);
                    Rms.saveRMS("NRitem1", data2);
                }
            }
            else if (type == 2)
            {
                int num2 = d.readShort();
                int num3 = d.readShort();
                for (int k = num2; k < num3; k++)
                {
                    ItemTemplate it2 = new((short)k, d.readByte(), d.readByte(), d.readUTF(), d.readUTF(), d.readByte(), d.readInt(), d.readShort(), d.readShort(), d.readBoolean());
                    ItemTemplates.add(it2);
                }
                if (isSave)
                {
                    d.reset();
                    sbyte[] data3 = new sbyte[d.available()];
                    d.readFully(ref data3);
                    Rms.saveRMS("NRitem2", data3);
                    sbyte[] data4 = new sbyte[1] { GameScr.vcItem };
                    Rms.saveRMS("NRitemVersion", data4);
                    LoginScr.isUpdateItem = false;
                    if (GameScr.vsData == GameScr.vcData && GameScr.vsMap == GameScr.vcMap && GameScr.vsSkill == GameScr.vcSkill && GameScr.vsItem == GameScr.vcItem)
                    {
                        GameScr.gI().readDart();
                        GameScr.gI().readEfect();
                        GameScr.gI().readArrow();
                        GameScr.gI().readSkill();
                        Service.gI().clientOk();
                    }
                }
            }
            else if (type == 100)
            {
                Char.Arr_Head_2Fr = readArrHead(d);
                if (isSave)
                {
                    d.reset();
                    sbyte[] data5 = new sbyte[d.available()];
                    d.readFully(ref data5);
                    Rms.saveRMS("NRitem100", data5);
                }
            }
        }
        catch (Exception ex)
        {
            _ = ex.ToString();
        }
    }

    private void readFrameBoss(Message msg, int mobTemplateId)
    {
        try
        {
            int num = msg.reader().readByte();
            int[][] array = new int[num][];
            for (int i = 0; i < num; i++)
            {
                int num2 = msg.reader().readByte();
                array[i] = new int[num2];
                for (int j = 0; j < num2; j++)
                {
                    array[i][j] = msg.reader().readByte();
                }
            }
            frameHT_NEWBOSS.put(mobTemplateId + string.Empty, array);
        }
        catch (Exception)
        {
        }
    }

    private int[][] readArrHead(myReader d)
    {
        int[][] array = new int[1][] { new int[2] { 542, 543 } };
        try
        {
            int num = d.readShort();
            array = new int[num][];
            for (int i = 0; i < array.Length; i++)
            {
                int num2 = d.readByte();
                array[i] = new int[num2];
                for (int j = 0; j < num2; j++)
                {
                    array[i][j] = d.readShort();
                }
            }
        }
        catch (Exception)
        {
        }
        return array;
    }

    public void phuban_Info(Message msg)
    {
        try
        {
            sbyte b = msg.reader().readByte();
            if (b == 0)
            {
                readPhuBan_CHIENTRUONGNAMEK(msg, b);
            }
        }
        catch (Exception)
        {
        }
    }

    private void readPhuBan_CHIENTRUONGNAMEK(Message msg, int type_PB)
    {
        try
        {
            sbyte b = msg.reader().readByte();
            if (b == 0)
            {
                short idmapPaint = msg.reader().readShort();
                string nameTeam = msg.reader().readUTF();
                string nameTeam2 = msg.reader().readUTF();
                int maxPoint = msg.reader().readInt();
                short timeSecond = msg.reader().readShort();
                int maxLife = msg.reader().readByte();
                GameScr.phuban_Info = new InfoPhuBan(type_PB, idmapPaint, nameTeam, nameTeam2, maxPoint, timeSecond)
                {
                    maxLife = maxLife
                };
                GameScr.phuban_Info.updateLife(type_PB, 0, 0);
            }
            else if (b == 1)
            {
                int pointTeam = msg.reader().readInt();
                int pointTeam2 = msg.reader().readInt();
                GameScr.phuban_Info?.updatePoint(type_PB, pointTeam, pointTeam2);
            }
            else if (b == 2)
            {
                sbyte b2 = msg.reader().readByte();
                short type = 0;
                short num = -1;
                if (b2 == 1)
                {
                    type = 1;
                    num = 3;
                }
                else if (b2 == 2)
                {
                    type = 2;
                }
                num = -1;
                GameScr.phuban_Info = null;
                GameScr.addEffectEnd(type, num, 0, GameCanvas.hw, GameCanvas.hh, 0, 0, -1, null);
            }
            else if (b == 5)
            {
                short timeSecond2 = msg.reader().readShort();
                GameScr.phuban_Info?.updateTime(type_PB, timeSecond2);
            }
            else if (b == 4)
            {
                int lifeTeam = msg.reader().readByte();
                int lifeTeam2 = msg.reader().readByte();
                GameScr.phuban_Info?.updateLife(type_PB, lifeTeam, lifeTeam2);
            }
        }
        catch (Exception)
        {
        }
    }

    public void read_opt(Message msg)
    {
        try
        {
            sbyte b = msg.reader().readByte();
            if (b == 0)
            {
                short idHat = msg.reader().readShort();
                Char.myCharz().idHat = idHat;
                SoundMn.gI().getStrOption();
            }
            else if (b == 2)
            {
                int num = msg.reader().readInt();
                sbyte b2 = msg.reader().readByte();
                short num2 = msg.reader().readShort();
                string v = num2 + "," + b2;
                MainImage imagePath = ImgByName.getImagePath("banner_" + num2, ImgByName.hashImagePath);
                GameCanvas.danhHieu.put(num + string.Empty, v);
            }

        }
        catch (Exception)
        {
        }
    }

    public void read_UpdateSkill(Message msg)
    {
        try
        {
            short num = msg.reader().readShort();
            sbyte b = -1;
            try
            {
                b = msg.reader().readSByte();
            }
            catch (Exception)
            {
            }
            if (b == 0)
            {
                short curExp = msg.reader().readShort();
                for (int i = 0; i < Char.myCharz().vSkill.size(); i++)
                {
                    Skill skill = (Skill)Char.myCharz().vSkill.elementAt(i);
                    if (skill.skillId == num)
                    {
                        skill.curExp = curExp;
                        break;
                    }
                }
            }
            else if (b == 1)
            {
                sbyte b2 = msg.reader().readByte();
                for (int j = 0; j < Char.myCharz().vSkill.size(); j++)
                {
                    Skill skill2 = (Skill)Char.myCharz().vSkill.elementAt(j);
                    if (skill2.skillId == num)
                    {
                        for (int k = 0; k < 20; k++)
                        {
                            string nameImg = "Skills_" + skill2.template.id + "_" + b2 + "_" + k;
                            MainImage imagePath = ImgByName.getImagePath(nameImg, ImgByName.hashImagePath);
                        }
                        break;
                    }
                }
            }
            else
            {
                if (b != -1)
                {
                    return;
                }
                Skill skill3 = Skills.get(num);
                for (int l = 0; l < Char.myCharz().vSkill.size(); l++)
                {
                    Skill skill4 = (Skill)Char.myCharz().vSkill.elementAt(l);
                    if (skill4.template.id == skill3.template.id)
                    {
                        Char.myCharz().vSkill.setElementAt(skill3, l);
                        break;
                    }
                }
                for (int m = 0; m < Char.myCharz().vSkillFight.size(); m++)
                {
                    Skill skill5 = (Skill)Char.myCharz().vSkillFight.elementAt(m);
                    if (skill5.template.id == skill3.template.id)
                    {
                        Char.myCharz().vSkillFight.setElementAt(skill3, m);
                        break;
                    }
                }
                for (int n = 0; n < GameScr.onScreenSkill.Length; n++)
                {
                    if (GameScr.onScreenSkill[n] != null && GameScr.onScreenSkill[n].template.id == skill3.template.id)
                    {
                        GameScr.onScreenSkill[n] = skill3;
                        break;
                    }
                }
                for (int num2 = 0; num2 < GameScr.keySkill.Length; num2++)
                {
                    if (GameScr.keySkill[num2] != null && GameScr.keySkill[num2].template.id == skill3.template.id)
                    {
                        GameScr.keySkill[num2] = skill3;
                        break;
                    }
                }
                if (Char.myCharz().myskill.template.id == skill3.template.id)
                {
                    Char.myCharz().myskill = skill3;
                }
                GameScr.info1.addInfo(mResources.hasJustUpgrade1 + skill3.template.name + mResources.hasJustUpgrade2 + skill3.point, 0);
            }
        }
        catch (Exception)
        {
        }
    }
}
