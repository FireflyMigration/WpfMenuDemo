using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Firefly.Wpf.MenuDemo
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow 
	{
		public MainWindow()
		{
			this.InitializeComponent();
            myMain.MenuName = "menu name";
            //myMain.SetLogoImage(@"C:\temp\Logo.bmp");
            //myMain.SetBackgroundImage(@"C:\temp\Logo.bmp",0);

            var m = myMain.AddMenuWithChildren("xxx", () => System.Windows.Forms.MessageBox.Show("Test"));
            m.AddMenuWithChildren("yy",
                () => System.Windows.Forms.MessageBox.Show("Test")).AddMenuWithChildren("zz",
                () => System.Windows.Forms.MessageBox.Show("Test")).AddMenuWithChildren("aa",null);
            var mm = myMain.AddMenuWithChildren("xxx");

            myMain.AddMenu("name", () => { }, x=> { });
            myMain.AddMenu("name", () => { }, x => {
                x("name", () => { },y => { });
            });
            myMain.AddMenu("name", null, x=>x("name", () => { }, y => {
                y("asdfasdfas", () => { });
            }));
            myMain.AddMenu("name", () => { }, x => { });
            myMain.AddMenu("name", () => { }, x => { });
            myMain.AddMenu("name", () => { }, x => { });
            
        }
	}
}