using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using WPF.Tools.BaseClasses;

namespace WPF.Tools.TabControl
{
  public class TabItemsCollection : ObservableCollection<UserControlBase>
  {
    public delegate void PropertyChangedEventHandler(object sender, NotifyCollectionChangedEventArgs e);

        public event PropertyChangedEventHandler OnPropertyChangedEvent;

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnCollectionChanged(e);

            if (this.OnPropertyChangedEvent != null)
            {
                this.OnPropertyChangedEvent(this, e);
            }
        }

        public void AddRange(UserControlBase[] columns)
        {
            foreach (UserControlBase col in columns)
            {
                this.Add(col);
            }
        }

        public void Add(Type objType, object[] para, Guid moduleId)
        {
            BaseInstance inst = new BaseInstance();

            UserControlBase controlBase = inst.CreateControlInstance(objType, moduleId, para);

            base.Add(controlBase);
        }

        public void Add(Type objType, Guid moduleId, object[] para, string title)
        {
            BaseInstance inst = new BaseInstance();

            UserControlBase controlBase = inst.CreateControlInstance(objType, moduleId, para);

            base.Add(controlBase);
        }

        new public UserControlBase this[int index]
        {
            get
            {
                return this.Items.Where(i => i.TabIndex == index).FirstOrDefault();
            }
        }
    
        new public void Clear()
        {

            base.Clear();
        }
  }
}
