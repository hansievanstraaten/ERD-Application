using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using WPF.Tools.Collections;

namespace WPF.Tools.Stacks
{
  public class ButtonStack : StackPanel
  {
    public ButtonStack()
    {
      this.Orientation = Orientation.Vertical;

      this.Children = new ItemsCollection<Button>();

      this.Children.CollectionChanged += this.Button_Changed;

      //this.Children.Add()
    }

    public new ItemsCollection<Button> Children {get; set;}

    public Button this[string name]
    {
      get
      {
        foreach (Button item in base.Children)
        {
          if (item.Name == name)
          {
            return item;
          }
        }

        return null;
      }
    }

    private void Button_Changed(object sender, NotifyCollectionChangedEventArgs e)
    {
      switch (e.Action)
      {
          case NotifyCollectionChangedAction.Add:

            foreach (Button item in e.NewItems)
            {
              base.Children.Add(item);
            }

            break;

          case NotifyCollectionChangedAction.Reset:

            base.Children.Clear();

            break;
      }
    }

  }
}
