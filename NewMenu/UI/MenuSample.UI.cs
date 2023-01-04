using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Firefly.Box;

namespace Firefly.Wpf.MenuDemo.UI
{
    partial class MenuSampleUI : Firefly.Box.UI.Form
    {
        MenuSample _task;
        public MenuSampleUI(MenuSample task,Action<Firefly.Wpf.MenuDemo.Main> populateMenu)
        {
            _task = task;
            InitializeComponent();

            var x = new Firefly.Wpf.MenuDemo.Main(){BackButtonName="Back"};
            
            x.RunActionWrapper=w => w();
            
            x.Menu.Clear();
            populateMenu(x);
            x.menu.SetButtonClickedColor(x.Menu.TitleColor);


            this.elementHost1.Child = x;
        }

    }
}