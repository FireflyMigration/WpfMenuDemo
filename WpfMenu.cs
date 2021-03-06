﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Firefly.Wpf.MenuDemo
{
    public class WpfMenu
    {
        public static void Run(ToolStrip theMenu, string title = null, string image = null, string logoImage = null)
        {
            var x = new Firefly.Wpf.MenuDemo.MenuSample(theMenu, y =>
            {
                if (!string.IsNullOrEmpty(title))
                    y.MenuName = title;
                if (!string.IsNullOrEmpty(image) && System.IO.File.Exists(image))
                    y.SetBackgroundImage(image);
                if (!string.IsNullOrEmpty(logoImage))
                    y.SetLogoInage(logoImage);

            });


            x.Run();
        }
        public static void Run(Action<Action<string, Action, Action<Action<string, Action, Action<Action<string, Action>>>>>> addItems, string title = null, string image = null, string logoImage = null)
        {
            var x = new Firefly.Wpf.MenuDemo.MenuSample(y =>
            {
                if (!string.IsNullOrEmpty(title))
                    y.MenuName = title;
                if (!string.IsNullOrEmpty(image) && System.IO.File.Exists(image))
                    y.SetBackgroundImage(image);
                if (!string.IsNullOrEmpty(logoImage))
                    y.SetLogoInage(logoImage);
                addItems(y.AddMenu);
            });
            x.Run();
        }

        public static void Test()
        {
            new Firefly.Wpf.MenuDemo.MenuSample(y =>
                                                {
                                                    y.AddMenu("GrandFather",
                                                        () => MessageBox.Show("GrandFather"),
                                                        z =>
                                                        {
                                                            z("Father", () => MessageBox.Show("Father"), yz =>
                                                                                                       {
                                                                                                           yz("child",
                                                                                                               () =>
                                                                                                                   MessageBox
                                                                                                                       .Show
                                                                                                                       ("child")
                                                                                                           );
                                                                                                       });
                                                            z("mother2",
                                                            () => { MessageBox.Show("Mother2"); },
                                                            delegate { })
                                                            ;
                                                        });
                                                    y.AddMenu("Grand Button", () => MessageBox.Show("grand Button"),
                                                        delegate { });
                                                    y.AddMenu("Grand Mother", null, yy =>
                                                                                    {
                                                                                        yy("mother", null, delegate
                                                                                        {
                                                                                        });
                                                                                    });
                                                }).Run();
        }
    }
}
