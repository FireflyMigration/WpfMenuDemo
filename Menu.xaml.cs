using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace Firefly.Wpf.MenuDemo
{
    /// <summary>
    /// Interaction logic for Menu.xaml
    /// </summary>
    public partial class Menu : UserControl
    {
        public Menu()
        {

            this.InitializeComponent();
            BackButtonName = "<";
            MiddleScrollToRight = ((Storyboard)this.FindResource("EnterChild2"));
            MiddleScrollToRight.Completed += delegate
            {
                TheMenu.AfterMiddleScrollToRight();
                ((Storyboard)this.FindResource("ResetMiddle")).Begin();
            };
            MiddleScrollToLeft = ((Storyboard)this.FindResource("LeaveChild"));
            MiddleScrollToLeft.Completed += delegate
            {
                if (TheMenu.MiddleScrollToLeftAndReturnTrueToReset())
                    ((Storyboard)this.FindResource("ResetMiddle")).Begin();

            };
        }
        Storyboard MiddleScrollToRight,MiddleScrollToLeft;
        public TheMenu TheMenu { get { return (TheMenu)DataContext; } }


        private void SubMenuButton_Click(object sender, RoutedEventArgs e)
        {
            if (TheMenu.Level == 0)
                ((Storyboard)this.FindResource("EnterChild")).Begin();
            else
            {
                MiddleScrollToRight.Begin();
            }
        }

        string _backButtonName;
        string _menuName;
        public string MenuName
        {
            get { return _menuName; }
            set
            {
                _menuName = value; this.TheMenu.RootMenu.Name = value; TheMenu.FireLeftChange();
            }
        }

        public string BackButtonName
        {
            get { return _backButtonName; }
            set
            {
                _backButtonName = value;

            }
        }

        void backButton_Click(object sender, RoutedEventArgs e)
        {
            ((Storyboard)this.FindResource("LeaveChild")).Begin();
            TheMenu.BackButton();
        }

        internal void SetButtonClickedColor(string color)
        {
            ((SolidColorBrush)this.FindResource("PressedButton")).Color = (System.Windows.Media.Color)new ColorConverter().ConvertFrom(color);
        }
    }
}