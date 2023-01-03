using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace Firefly.Wpf.MenuDemo
{
    public class SubMenu : MenuItem
    {
        Main _parent;
        public SubMenu(Main parent)
        {
            _parent = parent;
            Select = new MenuCommand(() =>
            {
                if (IAmSelected != null)
                    IAmSelected();
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
                sm.NotifySectedTo(z => { _notifySelected(z); });
            }
            Items.Add(mi);
        }

        public SubMenu AddMenuWithChildren(string name, Action what = null)
        {
            SubMenu grandParent = null;
            if (what != null)
                grandParent = new AnotherTypeOfSubMenu(_parent, name.Trim());
            else
                grandParent = new SubMenu(_parent) { Name = name.Trim() };
            _parent.SetActionToMenu(grandParent, what);
            this.Add(grandParent);
            return grandParent;
        }

        Action<SubMenu> _notifySelected = delegate { };

        public override string ToString()
        {
            return "Menu " + Name;
        }

        public event Action IAmSelected;

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

        public void NotifySectedTo(Action<SubMenu> func)
        {
            _notifySelected = func;
            IAmSelected += () => { func(this); };
        }
    }
    public class AnotherTypeOfSubMenu : SubMenu
    {
        public ICommand Command { get; set; }

        public AnotherTypeOfSubMenu(Main parent, string name) : base(parent)
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
        event Action IAmSelected;
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
        public SubMenu CurrentMenu
        {
            get { return _currentMenu; }
            set
            {
                _currentMenu = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("CurrentMenu"));
            }
        }
        Stack<SubMenu> _stack = new Stack<SubMenu>();
        public SubMenu CurrentChildMenu
        {
            get { return _currentChildMenu; }
            set
            {
                _currentChildMenu = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("CurrentChildMenu"));
            }
        }

        public void Add(SubMenu m)
        {
            m.NotifySectedTo(y =>
                             {
                                 if (Level == 0)
                                 {
                                     CurrentMenu = y;
                                     Level++;
                                 }
                                 else
                                 {
                                     _stack.Push(CurrentChildMenu);
                                     CurrentChildMenu = y;
                                     Level++;
                                 }
                             });

            Menues.Add(m);
        }
        public void Add(MenuItem m)
        {

            var x = m as SubMenu;
            if (x != null)
                Add(x);
            else
                Menues.Add(m);
        }

        ObservableCollection<MenuItem> _menues = new ObservableCollection<MenuItem>(), _mru = new ObservableCollection<MenuItem>();
        public ObservableCollection<MenuItem> Menues { get { return _menues; } }
        public ObservableCollection<MenuItem> Mru { get { return _mru; } }
        public event PropertyChangedEventHandler PropertyChanged;

        public void Clear()
        {
            Menues.Clear();
            Mru.Clear();
        }

        internal void BackButton()
        {
            Level--;
            if (_stack.Count > 1)
            {
                CurrentChildMenu = _stack.Pop();
            }

        }
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
