using System.Reflection;
using System.Text;
using LitJson;
using UnityEngine;

namespace ModHelper.Menu
{
    public static class Utilities
    {
        internal static long GetLastTimePress()
        {
            return GameCanvas.lastTimePress;
        }
        internal static Char FindCharInMap(string name)
        {
            for (int i = 0; i < GameScr.vCharInMap.size(); i++)
            {
                Char @char = (Char)GameScr.vCharInMap.elementAt(i);
                if (@char.getNameWithoutClanTag() == name)
                {
                    return @char;
                }
            }
            return null;
        }
        public static int getWidth(GUIStyle gUIStyle, string s)
        {
            return (int)(gUIStyle.CalcSize(new GUIContent(s)).x * 1.025f / mGraphics.zoomLevel);
        }
        public static System.Random random = new();
        internal static string FormatWithSIPrefix(double number)
        {
            string[] array = new string[5] { "", "k", "M", "B", "T" };
            int num = System.Math.Max(0, System.Math.Min((int)System.Math.Floor(System.Math.Log10(System.Math.Abs(number)) / 3.0), array.Length - 1));
            double num2 = number * System.Math.Pow(1000.0, -num);
            return $"{num2:0.##}{array[num]}";
        }

        internal static bool isMeWearingTXHSet()
        {
            return Char.myCharz().cgender == 0 && isMeWearingActivationSet(127);
        }

        internal static bool isMeWearingPikkoroDaimaoSet()
        {
            return Char.myCharz().cgender == 1 && isMeWearingActivationSet(132);
        }

        internal static bool isMeWearingCadicSet()
        {
            return Char.myCharz().cgender == 2 && isMeWearingActivationSet(134);
        }
        internal static bool isMeWearingActivationSet(int idSet)
        {
            int num = 0;
            for (int i = 0; i < 5; i++)
            {
                Item item = Char.myCharz().arrItemBody[i];
                if (item == null)
                {
                    return false;
                }
                if (item.itemOption == null)
                {
                    return false;
                }
                for (int j = 0; j < item.itemOption.Length; j++)
                {
                    if (item.itemOption[j].optionTemplate.id == idSet)
                    {
                        num++;
                        break;
                    }
                }
            }
            return num == 5;
        }

        public static string GetFullInfo(this Item item)
        {
            string text = item.template.name;
            if (item.itemOption != null)
            {
                for (int i = 0; i < item.itemOption.Length; i++)
                {
                    if (item.itemOption[i].optionTemplate.id == 72)
                    {
                        text = text + " [+" + item.itemOption[i].param.ToString() + "]";
                        break;
                    }
                }
            }

            if (item.itemOption != null)
            {
                for (int j = 0; j < item.itemOption.Length; j++)
                {
                    if (item.itemOption[j].optionTemplate.name.StartsWith("$"))
                    {
                        string optionColor = item.itemOption[j].getOptiongColor();
                        if (item.itemOption[j].param == 1)
                        {
                            text = text + "\n" + optionColor;
                        }

                        if (item.itemOption[j].param == 0)
                        {
                            text = text + "\n" + optionColor;
                        }
                    }
                    else
                    {
                        string optionString = item.itemOption[j].getOptionString();
                        if (!optionString.Equals(string.Empty) && item.itemOption[j].optionTemplate.id != 72)
                        {
                            text = text + "\n" + optionString;
                        }
                    }
                }
            }

            if (item.template.strRequire > 1)
            {
                text += "\n" + mResources.pow_request + ": " + item.template.strRequire.ToString();
            }

            return text + "\n" + item.template.description;
        }
        public static int GetPetGender()
        {
            string skill1Pet = Char.myPetz().arrPetSkill[0].template.name;
            return skill1Pet == GameScr.nClasss[0].skillTemplates[0].name
                ? GameScr.nClasss[0].classId
                : skill1Pet == GameScr.nClasss[1].skillTemplates[0].name
                ? GameScr.nClasss[1].classId
                : skill1Pet == GameScr.nClasss[2].skillTemplates[0].name ? GameScr.nClasss[2].classId : 3;
        }
        public static string status = "Đã kết nối";
        public static string username = "";
        public static string password = "";
        public static JsonData server = null;
        public static JsonData sizeData = null;
        public static readonly short ID_ITEM_CAPSULE_VIP = 194;

        public static readonly short ID_ITEM_CAPSULE_NORMAL = 193;

        public static readonly int ID_MAP_HOME_BASE = 21;

        public static readonly int ID_MAP_LANG_BASE = 7;

        public static readonly int ID_MAP_TTVT_BASE = 24;

        public static int mapCapsuleReturn = -1;

        public static readonly short ID_ICON_ITEM_TDLT = 4387;

        public static int getYGround(int x)
        {
            int y = 50;
            for (int i = 0; i < 30; i++)
            {
                y += 24;
                if (TileMap.tileTypeAt(x, y, 2))
                {
                    if (y % 24 != 0)
                    {
                        y -= y % 24;
                    }

                    return y;
                }
            }
            return -1;
        }


        public static bool canBuffMe(out Skill skillBuff)
        {
            skillBuff = Char.myCharz().
                getSkill(new SkillTemplate { id = ID_SKILL_BUFF });

            if (skillBuff == null)
            {
                return false;
            }

            return true;
        }
        public static MyVector getMyVectorMe()
        {
            var vMe = new MyVector();
            vMe.addElement(Char.myCharz());
            return vMe;
        }

        public static readonly sbyte ID_SKILL_BUFF = 7;
        public static void buffMe()
        {
            if (!canBuffMe(out Skill skillBuff))
            {
                GameScr.info1.addInfo("Không tìm thấy kỹ năng Trị thương", 0);
                return;
            }

            // Đổi sang skill hồi sinh
            Service.gI().selectSkill(ID_SKILL_BUFF);

            // Tự tấn công vào bản thân
            Service.gI().sendPlayerAttack(new MyVector(), getMyVectorMe(), -1);

            // Trả về skill cũ
            Service.gI().selectSkill(Char.myCharz().myskill.template.id);

            // Đặt thời gian hồi cho skill
            skillBuff.lastTimeUseThisSkill = mSystem.currentTimeMillis();
        }
        public static bool isMyCharDied()
        {
            Char @char = Char.myCharz();
            return @char.statusMe == 14 || @char.cHP <= 0;
        }

        public static Waypoint findWaypoint(int idMap)
        {
            for (int i = 0; i < TileMap.vGo.size(); i++)
            {
                Waypoint waypoint = (Waypoint)TileMap.vGo.elementAt(i);
                string textPopup = getTextPopup(waypoint.popup);
                if (textPopup.Equals(TileMap.mapNames[idMap]))
                {
                    return waypoint;
                }
            }
            return null;
        }

        public static string getTextPopup(PopUp popUp)
        {
            StringBuilder stringBuilder = new();
            for (int i = 0; i < popUp.says.Length; i++)
            {
                _ = stringBuilder.Append(popUp.says[i]);
                _ = stringBuilder.Append(" ");
            }
            return stringBuilder.ToString().Trim();
        }

        public static int getIdMapHome(int cgender)
        {
            return ID_MAP_HOME_BASE + cgender;
        }

        public static int getIdMapLang(int cgender)
        {
            return ID_MAP_LANG_BASE * cgender;
        }

        public static int getMapIdFromName(string mapName)
        {
            int cgender = Char.myCharz().cgender;
            if (mapName.Equals("Về nhà"))
            {
                return ID_MAP_HOME_BASE + cgender;
            }
            if (mapName.Equals("Trạm tàu vũ trụ"))
            {
                return ID_MAP_TTVT_BASE + cgender;
            }
            if (mapName.Contains("Về chỗ cũ: "))
            {
                mapName = mapName.Replace("Về chỗ cũ: ", "");
                if (TileMap.mapNames[mapCapsuleReturn].Equals(mapName))
                {
                    return mapCapsuleReturn;
                }
                if (mapName.Equals("Rừng đá"))
                {
                    return -1;
                }
            }
            for (int i = 0; i < TileMap.mapNames.Length; i++)
            {
                if (mapName.Equals(TileMap.mapNames[i]))
                {
                    return i;
                }
            }
            return -1;
        }

        public static void changeMap(Waypoint waypoint)
        {
            if (waypoint != null)
            {
                teleportMyChar(getXWayPoint(waypoint), getYWayPoint(waypoint));
                requestChangeMap(waypoint);
            }
        }

        public static int getXWayPoint(Waypoint waypoint)
        {
            return waypoint.maxX >= 60
            ? waypoint.minX <= TileMap.pxw - 60 ? waypoint.minX + ((waypoint.maxX - waypoint.minX) / 2) : TileMap.pxw - 15: 15;
        }

        public static int getYWayPoint(Waypoint waypoint)
        {
            return waypoint.maxY;
        }

        public static void requestChangeMap(Waypoint waypoint)
        {
            if (waypoint.isOffline)
            {
                Service.gI().getMapOffline();
            }
            else
            {
                Service.gI().requestChangeMap();
            }
        }

        public static bool hasItemCapsuleVip()
        {
            Item[] arrItemBag = Char.myCharz().arrItemBag;
            for (int i = 0; i < arrItemBag.Length; i++)
            {
                if (arrItemBag[i] != null && arrItemBag[i].template.id == ID_ITEM_CAPSULE_VIP)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool hasItemCapsuleNormal()
        {
            Item[] arrItemBag = Char.myCharz().arrItemBag;
            for (int i = 0; i < arrItemBag.Length; i++)
            {
                if (arrItemBag[i] != null && arrItemBag[i].template.id == ID_ITEM_CAPSULE_NORMAL)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool canNextMap()
        {
            return !Char.isLoadingMap && !Char.ischangingMap && !Controller.isStopReadMessage;
        }

        public static void teleportMyChar(int x)
        {
            teleportMyChar(x, getYGround(x));
        }

        public static void teleportMyChar(IMapObject obj)
        {
            teleportMyChar(obj.getX(), obj.getY());
        }

        public static bool isUsingTDLT()
        {
            return ItemTime.isExistItem(ID_ICON_ITEM_TDLT);
        }
        public static void ResetTF()
        {
            ChatTextField.gI().strChat = "Chat";
            ChatTextField.gI().tfChat.name = "chat";
            ChatTextField.gI().isShow = false;
        }
        public static void teleportMyChar(int x, int y)
        {
            Char.myCharz().currentMovePoint = null;
            Char.myCharz().cx = x;
            Char.myCharz().cy = y;
            Service.gI().charMove();
            if (!isUsingTDLT())
            {
                Char.myCharz().cx = x;
                Char.myCharz().cy = y + 1;
                Service.gI().charMove();
                Char.myCharz().cx = x;
                Char.myCharz().cy = y;
                Service.gI().charMove();
            }
        }

        public static void ResetTF(this ChatTextField tf)
        {
            tf.strChat = "Chat";
            tf.tfChat.name = "chat";
            tf.to = "";
            tf.tfChat.setIputType(TField.INPUT_TYPE_ANY);
            tf.isShow = false;
        }

        public static short getNRSDId()
        {
            return isMeInNRDMap() ? (short)(2400 - TileMap.mapID) : (short)0;
        }
        public static bool isMeInNRDMap()
        {
            return TileMap.mapID is >= 85 and <= 91;
        }

        public static T getValueProperty<T>(this object obj, string name)
        {
            return (T)obj.GetType().GetProperty(name).GetValue(obj, null);
        }
        internal static double Distance(double x1, double y1, double x2, double y2)
        {
            return System.Math.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));
        }
        public static int getDistance(IMapObject mapObject1, IMapObject mapObject2)
        {
            return Res.distance(mapObject1.getX(), mapObject1.getY(), mapObject2.getX(), mapObject2.getY());
        }

        public static void DoDoubleClickToObj(IMapObject mapObject)
        {
            _ = typeof(GameScr).GetMethod("doDoubleClickToObj", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.InvokeMethod).Invoke(GameScr.gI(), new object[1] { mapObject });
        }
    }
}