using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Firefly.Wpf.MenuDemo
{
    public class WpfMenu
    {
        public enum imgStrech
        {
            None = 0,
            Fill = 1,
            Uniform = 2,
            UniformToFill = 3
        }

        public static void Run(ToolStrip theMenu, string title = null, string image = null, string logoImage = null, imgStrech imgStyle=0)
        {
            Run(theMenu.Items, title, image, logoImage, imgStyle);
        }
        public static void Run(ToolStripMenuItem theMenu, string title = null, string image = null, string logoImage = null, imgStrech imgStyle = 0)
        {
            Run(theMenu.DropDownItems, title, image, logoImage, imgStyle);
        }
        public static void Run(System.Windows.Forms.ToolStripItemCollection theMenu, string title = null, string image = null, string logoImage = null, imgStrech imgStyle = 0)
        {
            var x = new Firefly.Wpf.MenuDemo.MenuSample(theMenu, y =>
            {
                if (!string.IsNullOrEmpty(title))
                    y.MenuName = title;
                if (!string.IsNullOrEmpty(image) && System.IO.File.Exists(image))
                    y.SetBackgroundImage(image, (System.Windows.Media.Stretch)imgStyle);
                if (!string.IsNullOrEmpty(logoImage))
                    y.SetLogoImage(logoImage);

            });


            x.Run();
        }
        public static void Run(Action<Action<string, Action, Action<Action<string, Action, Action<Action<string, Action>>>>>> addItems, string title = null, string image = null, string logoImage = null, imgStrech imgStyle = 0)
        {
            var x = new Firefly.Wpf.MenuDemo.MenuSample(y =>
            {
                if (!string.IsNullOrEmpty(title))
                    y.MenuName = title;
                if (!string.IsNullOrEmpty(image) && System.IO.File.Exists(image))
                    y.SetBackgroundImage(image, (System.Windows.Media.Stretch)imgStyle);
                if (!string.IsNullOrEmpty(logoImage))
                    y.SetLogoImage(logoImage);
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
