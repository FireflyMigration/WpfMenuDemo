using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Firefly.Wpf.MenuDemo
{
    public class SubMenu : MenuItem
    {
        public SubMenu()
        {
            Select = new MenuCommand(() =>
            {
                _notifySelected(this);
            });
        }

        public string Name { get; set; }
        ObservableCollection<MenuItem> _items = new ObservableCollection<MenuItem>();
        public ObservableCollection<MenuItem> Items
        {
            get { return _items; }
        }

        public void Add(MenuItem mi)
        {
            var sm = mi as SubMenu;
            if (sm != null)
            {
                sm.NotifySectedTo(z => { _notifySelected(z); }, _main);
            }
            Items.Add(mi);
        }

        public SubMenu AddMenuWithChildren(string name, Action what = null)
        {
            SubMenu grandParent;
            if (what != null)
                grandParent = new AnotherTypeOfSubMenu(name.Trim());
            else
                grandParent = new SubMenu() { Name = name.Trim() };
            _main.SetActionToMenu(grandParent, what);
            this.Add(grandParent);
            return grandParent;
        }
        public void AddMenuButton(string name, Action what = null)
        {
            var grandParent = new MenuButton(name.Trim());
            _main.SetActionToMenu(grandParent, what);
            this.Add(grandParent);
        }

        Action<SubMenu> _notifySelected = delegate { };

        public override string ToString()
        {
            return "Menu " + Name;
        }

        public virtual byte[] ScreenShot
        {
            get { return null; }
        }

        public virtual void SetAction(Action what)
        {

        }

        public virtual void SetScreenShot(byte[] image)
        {
            throw new NotImplementedException();
        }

        public ICommand Select { get; set; }
        Main _main;
        public void NotifySectedTo(Action<SubMenu> func, Main main)
        {
            _main = main;
            _notifySelected = func;
            foreach (var item in Items)
            {
                var si = item as SubMenu;
                if (si != null)
                    si.NotifySectedTo(func, main);

            }

        }
    }
    public class AnotherTypeOfSubMenu : SubMenu
    {
        public ICommand Command { get; set; }

        public AnotherTypeOfSubMenu(string name)
        {
            Name = name;

        }
        byte[] _screenShot = null;
        public override byte[] ScreenShot
        {
            get { return _screenShot; }
        }

        public override void SetAction(Action what)
        {
            Command = new MenuCommand(what);
        }

        public override void SetScreenShot(byte[] image)
        {
            _screenShot = image;
        }


    }
    public interface MenuItem
    {
        string Name { get; set; }
        byte[] ScreenShot { get; }
        void SetAction(Action what);
        void SetScreenShot(byte[] image);
    }
    public class MenuButton : MenuItem
    {
        byte[] _screenShot = null;
        public byte[] ScreenShot
        {
            get { return _screenShot; }
        }

        public void SetAction(Action what)
        {
            Command = new MenuCommand(what);
        }

        public void SetScreenShot(byte[] image)
        {
            _screenShot = image;
        }


        public string Name { get; set; }
        public event Action IAmSelected;
        public ICommand Command { get; set; }
        public override string ToString()
        {
            return "Menu " + Name;
        }
        public MenuButton(string name)
        {
            Name = name;

        }

    }
    public class TheMenu : INotifyPropertyChanged
    {

        public TheMenu()
        {
            _leftMenu = RootMenu;
        }
        string _color = "#FFE91E63";
        public string TitleColor
        {
            get
            {
                return _color;
            }
            set
            {
                _color = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("TitleColor"));
            }
        }
        public void MenuUsed(MenuItem x)
        {
            if (x.ScreenShot != null)
            {
                if (Mru.Contains(x))
                {
                    Mru.Move(Mru.IndexOf(x), 0);
                }
                else
                    Mru.Insert(0, x);
            }
        }
        public int Level { get; set; }
        SubMenu _currentMenu, _currentChildMenu;
        public SubMenu _leftMenu;
        public SubMenu RootMenu = new SubMenu();
        public SubMenu MiddleMenu
        {
            get { return _currentMenu; }
            set
            {
                _currentMenu = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("MiddleMenu"));
            }
        }
        public Stack<SubMenu> _stack = new Stack<SubMenu>();
        public SubMenu RightMenu
        {
            get { return _currentChildMenu; }
            set
            {
                _currentChildMenu = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("RightMenu"));
            }
        }

        ObservableCollection<MenuItem> _mru = new ObservableCollection<MenuItem>();
        public void SetLeftMenu(SubMenu m)
        {
            _leftMenu = m;
            FireLeftChange();
        }
        public void FireLeftChange()
        {

            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("LeftMenu"));
                PropertyChanged(this, new PropertyChangedEventArgs("_leftMenu"));
            }
        }
        public SubMenu LeftMenu { get { return _leftMenu; } }
        public ObservableCollection<MenuItem> Mru { get { return _mru; } }
        public event PropertyChangedEventHandler PropertyChanged;

        public void Clear()
        {
            Mru.Clear();
        }

        internal void BackButton()
        {
            Level--;

        }

        internal void EnterSubMenu(SubMenu y)
        {
            if (Level == 0)
            {
                MiddleMenu = y;
                Level++;
            }
            else
            {

                RightMenu = y;
                Level++;
                AfterMiddleScrollToRight = () =>
                {
                    _stack.Push(LeftMenu);
                    SetLeftMenu(MiddleMenu);
                    MiddleMenu = RightMenu;
                };
            }
        }

        internal bool MiddleScrollToLeftAndReturnTrueToReset()
        {
            if (Level == 0)
                return false;
            MiddleMenu = LeftMenu;
            SetLeftMenu(_stack.Pop());
            return true;

        }

        internal Action AfterMiddleScrollToRight = () => { };

    }

    class MenuCommand : ICommand
    {
        Action _what;

        public MenuCommand(Action what)
        {
            _what = what;
        }

        public void Execute(object parameter)
        {
            try
            {
                _what();
            }
            catch
            {
            }
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;
    }
}
