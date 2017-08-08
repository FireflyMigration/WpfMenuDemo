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
	/// Interaction logic for ButtonSubMenu.xaml
	/// </summary>
	public partial class ButtonSubMenu : UserControl
	{
		public ButtonSubMenu()
		{
			this.InitializeComponent();
			
			
		}
		[System.ComponentModel.BindableAttribute(true)]
		public ICommand ClickCommand{get{return ClickButton.Command;}set{ClickButton.Command=value;}}
		[System.ComponentModel.BindableAttribute(true)]
		public ICommand SelectCommand{get{return SubMenuButton.Command;}set{SubMenuButton.Command=value;}}
		
	}
}