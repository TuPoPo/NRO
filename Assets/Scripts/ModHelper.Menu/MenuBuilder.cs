using System.Collections.Generic;
using System.Linq;

namespace ModHelper.Menu
{
    public class MenuBuilder : IActionListener
    {
        private string chatPopup;

        private bool isPosDefault = true;

        public int x;

        public int y;

        public List<MenuItem> menuItems = new();

        public MenuBuilder setChatPopup(string chatPopup)
        {
            this.chatPopup = chatPopup;
            return this;
        }

        public MenuBuilder setPos(int x, int y)
        {
            isPosDefault = false;
            this.x = x;
            this.y = y;
            return this;
        }

        public MenuBuilder addItem(string caption, MenuAction action)
        {
            menuItems.Add(new MenuItem(caption, action));
            return this;
        }

        public MenuBuilder addItem(bool ifCondition, string caption, MenuAction action)
        {
            if (ifCondition)
            {
                menuItems.Add(new MenuItem(caption, action));
            }
            return this;
        }

        public MenuBuilder map<T>(MyVector myVector, System.Func<T, MenuItem> func)
        {
            for (int i = 0; i < myVector.size(); i++)
            {
                T arg = (T)myVector.elementAt(i);
                menuItems.Add(func(arg));
            }
            return this;
        }

        public MenuBuilder map<T>(IEnumerable<T> values, System.Func<T, MenuItem> func)
        {
            foreach (T value in values)
            {
                menuItems.Add(func(value));
            }
            return this;
        }
        public static int avata;
        public void start()
        {
            MyVector myVectorStartMenu = getMyVectorStartMenu();
            if (myVectorStartMenu.size() > 0)
            {
                if (isPosDefault)
                {
                    GameCanvas.menu.startAt(myVectorStartMenu, 3);
                }
                else
                {
                    GameCanvas.menu.startAt(myVectorStartMenu, x, y);
                }
            }
            if (!string.IsNullOrEmpty(chatPopup))
            {
                if (Char.myCharz().cgender == 0)
                {
                    avata = 8025;
                }
                if (Char.myCharz().cgender == 1)
                {
                    avata = 8057;
                }
                if (Char.myCharz().cgender == 2)
                {
                    avata = 7992;
                }
                _ = ChatPopup.addChatPopup(chatPopup, 100000, new Npc(5, 0, -100, 100, 5, avata));
            }
        }

        private MyVector getMyVectorStartMenu()
        {
            IEnumerable<string> source = menuItems.Select((MenuItem menuItem) => menuItem.caption);
            MyVector myVector = new();
            for (int i = 0; i < menuItems.Count; i++)
            {
                MenuItem menuItem2 = menuItems[i];
                myVector.addElement(new Command(menuItem2.caption, this, 1, new
                {
                    selected = i,
                    menuItem2.action,
                    captions = source.ToArray()
                }));
            }
            return myVector;
        }

        public void perform(int idAction, object p)
        {
            if (idAction is not 0 and 1)
            {
                onMenuSelected(p);
            }
        }

        private static void onMenuSelected(object p)
        {

            int valueProperty = p.getValueProperty<int>("selected");
            MenuAction valueProperty2 = p.getValueProperty<MenuAction>("action");
            string[] valueProperty3 = p.getValueProperty<string[]>("captions");
            string caption = valueProperty3[valueProperty];
            if (Char.myCharz().cgender == 0)
            {
                avata = 8025;
            }
            if (Char.myCharz().cgender == 1)
            {
                avata = 8057;
            }
            if (Char.myCharz().cgender == 2)
            {
                avata = 7992;
            }
            if (Char.chatPopup != null && Char.chatPopup.c.avatar == avata)
            {
                Char.chatPopup = null;
            }
            valueProperty2.Invoke(valueProperty, caption, valueProperty3);
        }
    }
}