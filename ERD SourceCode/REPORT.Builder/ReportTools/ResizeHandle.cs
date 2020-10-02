using REPORT.Builder.ReportComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;

namespace REPORT.Builder.ReportTools
{
    public class ResizeHandle : Thumb
    {
        public ResizeHandle()
        {
            this.ElemntId = Guid.NewGuid();
        }

        public Guid ElemntId { get; set; }

        public ResizeHandlesEnum ResizeHandleType { get; set; }
    }
}
