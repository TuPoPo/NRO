using System;
using System.Collections.Generic;

namespace ModHelper.Menu
{
    public class MenuItemCollection
    {
        public List<MenuItem> menuItems;

        public int Count => menuItems.Count;

        public MenuItem this[int index] => menuItems[index];

        public MenuItemCollection(Action<List<MenuItem>> action)
        {
            menuItems = new List<MenuItem>();
            action(menuItems);
        }
    }
}