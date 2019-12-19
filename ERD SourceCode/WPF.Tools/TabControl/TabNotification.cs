using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF.Tools.TabControl
{
  public class TabNotification
  {
    public string TabHeaderToNotify {get; set;}

    public string NotificationMessage {get; set;}

    public TabNotificationTypeEnum NotificationType {get; set;}
  }
}
