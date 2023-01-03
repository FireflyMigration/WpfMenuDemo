using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Firefly.Box;
using Rectangle = System.Windows.Shapes.Rectangle;


namespace Firefly.Wpf.MenuDemo
{
    /// <summary>
    /// Interaction logic for Main.xaml
    /// </summary>
    public partial class Main : UserControl
    {
        public Main()
        {
            this.InitializeComponent();
            menu.DataContext = DataContext;
            mru.DataContext = DataContext;

        }
        public void SetBackgroundImage(string name, Stretch imageStrech)
        {
            backBorder.Background = new ImageBrush(new BitmapImage(new Uri(name))) { Stretch = imageStrech };
        }
        public void SetLogoImage(string name)
        {
            LogoImage.Source = new BitmapImage(new Uri(name));
        }

        public TheMenu Menu { get { return (TheMenu)DataContext; } }

        public SubMenu AddMenuWithChildren(string name, Action what = null)
        {
            SubMenu grandParent = null;
            if (what != null)
                grandParent = new AnotherTypeOfSubMenu(this,name.Trim());
            else
                grandParent = new SubMenu(this) { Name = name.Trim() };
            SetActionToMenu(grandParent, what);
            Menu.Add(grandParent);
            return grandParent;
        }

        public void AddMenu(string grandParentName, Action grandParentAction, Action<Action<string, Action, Action<Action<string, Action>>>> addSons)
        {
            {
                MenuItem grandParent = null;

                addSons((parentName, parentAction, addGrandSon) =>
                {

                    SubMenu tempGrandParent;
                    if (grandParent == null)
                    {
                        if (grandParentAction != null)
                            tempGrandParent = new AnotherTypeOfSubMenu(this,grandParentName.Trim());
                        else
                            tempGrandParent = new SubMenu(this) { Name = grandParentName.Trim() };
                        grandParent = tempGrandParent;

                    }
                    else
                        tempGrandParent = grandParent as SubMenu;
                    ;
                    MenuItem mb = null;
                    addGrandSon((grandsonName, grandwhat) =>
                    {

                        SubMenu yy;
                        if (mb == null)
                        {
                            if (grandParentAction != null)
                                yy = new AnotherTypeOfSubMenu(this,parentName.Trim());
                            else
                                yy = new SubMenu(this) { Name = parentName.Trim() };
                            mb = yy;

                        }
                        else
                            yy = mb as SubMenu;


                        var mmb = new MenuButton(grandsonName.Trim());
                        SetActionToMenu(mmb, grandwhat);
                        yy.Add(mmb);
                    });
                    if (mb == null)
                        mb = new MenuButton(parentName.Trim());
                    SetActionToMenu(mb, parentAction);
                    tempGrandParent.Add(mb);
                });
                if (grandParent == null)
                {
                    grandParent = new MenuButton(grandParentName.Trim());
                }
                SetActionToMenu(grandParent, grandParentAction);
                Menu.Add(grandParent);

            };
        }
        class RunActionClass : IDisposable
        {
            MenuItem _item;
            Main _parent;
            public RunActionClass(MenuItem item, Main parent)
            {
                _parent = parent;
                _item = item;

                ENV.AbstractUIController.StartOfInstance += AbstractUIController_StartOfInstance;
                //      ENV.BusinessProcessBase.StartOfInstance += AbstractUIController_StartOfInstance;
            }
            void AbstractUIController_StartOfInstance(Firefly.Box.BusinessProcess obj)
            {
                if (obj.View != null && obj.ShowView)
                {
                    Dispose();
                    obj.End += () =>
                                   {
                                       GetFormImage(obj.View);
                                   };
                    ENV.AbstractUIController.StartOfInstance -= AbstractUIController_StartOfInstance;
                }
            }

            void GetFormImage(Firefly.Box.UI.Form obj)
            {
                if (obj == null)
                    return;
                while (obj.Parent is Firefly.Box.UI.Form)
                    obj = (Firefly.Box.UI.Form)obj.Parent;
                if (_item.ScreenShot == null)
                {
                    var bp = new Bitmap(obj.Width, obj.Height);
                    obj.DrawToBitmap(bp,
                                          new System.Drawing.Rectangle(0, 0, bp.Width,
                                                                       bp.Height));

                    using (var ms = new MemoryStream())
                    {
                        bp.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                        _item.SetScreenShot(ms.ToArray());
                    }
                }

                _parent.Menu.MenuUsed(_item);
            }

            void AbstractUIController_StartOfInstance(Firefly.Box.UIController obj)
            {


                Dispose();
                obj.End += () =>
                               {
                                   Firefly.Box.Context.Current.InvokeUICommand(() => GetFormImage(obj.View));
                               };





            }

            bool _disposed;
            public void Dispose()
            {
                if (_disposed)
                    return;
                _disposed = true;
                //    ENV.AbstractUIController.StartOfInstance -= AbstractUIController_StartOfInstance;
                //          ENV.BusinessProcessBase.StartOfInstance -= AbstractUIController_StartOfInstance;
            }
        }

        public Action<Action> RunActionWrapper = what =>
        {
            ENV.MenuManager.DoOnMenuManagers(y => y.Run(null, what));
        };
        public string MenuName { get { return menu.MenuName; } set { menu.MenuName = value; } }

        public string BackButtonName { get { return menu.BackButtonName; } set { menu.BackButtonName = value; } }

        internal void SetActionToMenu(MenuItem item, Action what)
        {
            item.SetAction(() =>
                               {
                                   RunActionWrapper
                                       (() =>
                                            {
                                                using (new RunActionClass(item, this))
                                                {
                                                    if (what != null)
                                                        what();
                                                }
                                            });


                               });
        }
    }
}