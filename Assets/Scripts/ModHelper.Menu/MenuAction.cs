using System;

namespace ModHelper.Menu
{

    public class MenuAction
    {
        public Action<int, string, string[]> action;

        public MenuAction(Action<int, string, string[]> action)
        {
            this.action = action;
        }

        public MenuAction(Action<int, string> action)
        {
            this.action = delegate (int selected, string caption, string[] _)
            {
                action(selected, caption);
            };
        }

        public MenuAction(Action<int> action)
        {
            this.action = delegate (int selected, string _, string[] _)
            {
                action(selected);
            };
        }

        public MenuAction(Action action)
        {
            this.action = delegate
            {
                action();
            };
        }

        public void Invoke(int selected, string caption, string[] captions)
        {
            action(selected, caption, captions);
        }
    }
}