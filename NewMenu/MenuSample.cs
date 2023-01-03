using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using ENV;
using Firefly.Box;
using ENV.Data;
using Firefly.Wpf.MenuDemo.UI;

namespace Firefly.Wpf.MenuDemo
{
    public class MenuSample : UIControllerBase
    {

        // TODO: Add members Here

        MenuSampleUI _form;
        static string menuName;
        static HashSet<string> _menuesToAvoid = new HashSet<string>(){"File","Edit","Options"};
        static string childMenu;

        public MenuSample(System.Windows.Forms.ToolStripItemCollection m,Action<Main> more)
            : this(
                delegate(Main main)
                {
                    var x = main.Menu.RootMenu;
                    AddChildItems(m, x);
                    more(main);
                })
        {

        }

        private static void AddChildItems(ToolStripItemCollection m, SubMenu x)
        {
            foreach (var ii in m)
            {
                var i = ii as ToolStripMenuItem;
                if (i != null)
                {
                    menuName = i.Text.Replace("&", "").Replace("...", "");
                    if (_menuesToAvoid.Contains(menuName) || !i.Available)
                        continue;
                    Action z = null;
                    if (i.DropDownItems.Count == 0)
                    {
                        x.AddMenuButton(menuName, () => CallClickEvent(i));
                    }
                    else
                    {

                        var s = x.AddMenuWithChildren(menuName);
                        AddChildItems(i.DropDownItems,s);
                    }
                }
            }
        }

        static void SendChildren(ToolStripMenuItem i, Action<string, Action, Action<Action<string, Action>>> y)
        {
            foreach (var jj in i.DropDownItems)
            {
                var j = jj as ToolStripMenuItem;
                if (j != null)
                {
                    Action w = null;
                    if (j.DropDownItems.Count == 0)
                        w = () => CallClickEvent(j);
                    childMenu = j.Text.Replace("&", "").Replace("...", "");
                    if (j.Available)
                        y(childMenu, w, t =>
                                        {
                                            SendChildren(j, (s, action, arg3) => { t(s, action); });
                                        });
                }
            }
        }

        static void CallClickEvent(ToolStripMenuItem i)
        {
            i.GetType().GetMethod("OnClick",
                                  System.Reflection.BindingFlags.Instance |
                                  System.Reflection.BindingFlags.NonPublic).Invoke(i,
                                                                                   new object
                                                                                       []   
                                                                                       {
                                                                                           new EventArgs
                                                                                               ()
                                                                                       });
        }

        public MenuSample(Action<Firefly.Wpf.MenuDemo.Main> populateMenu)
        {
            //TODO: Add task definitions here


            View = () =>
                       {
                           _form = new MenuSampleUI(this, populateMenu);
                           return _form;
                       };

        }
        public void Run()
        {
            try
            {
                ENV.MenuManager.IgnoreCloseAllTasks = true;
                Execute();
            }
            finally
            {
                ENV.MenuManager.IgnoreCloseAllTasks = false;

            }
        }

    }
}
