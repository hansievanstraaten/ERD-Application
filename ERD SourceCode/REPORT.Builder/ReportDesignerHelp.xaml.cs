using MdXaml;
using System.Windows;
using System.Windows.Documents;

namespace REPORT.Builder
{
    /// <summary>
    /// Interaction logic for ReportDesignerHelp.xaml
    /// </summary>
    public partial class ReportDesignerHelp : Window
    {
        public ReportDesignerHelp()
        {
            this.InitializeComponent();

            this.DataContext = this;

            Markdown markdown = new Markdown();

            this.uxHelpViewer.Document = markdown.Transform(this.MarkDownText);
        }

        private string MarkDownText
        {
            get
            {
                return @"#**Quick Guide**

## To the left

- **Report** - Under Report you can change the report properties. **Active Version** is for implementing version control if you would like to implement a third-party print option.

- **Tools** - Provides some basic tool options.

- **Data Options** - Click the **Checkbox** icon on the left to display the options.
  - Select the **Main Table**. This should be the top-most table in your data structure.
  - Select all the tables that you would like to use in your report. Ensure that foreign-key constraints are considered to allow the correct linking of SQL queries.

## Canvas

- **Header** - This section is used for report header information such as the report name and column headings.
- **Data** - This section carries the report data.
  - This is the only section that will accept data objects from the **Data Options** section.
- **Footer** - This section carries footer details such as the print date.

## How to

- **Add objects to the canvas** - Drag an object from the left (**Tools** or **Data Options**) onto the canvas.
- **Access object properties** - Clicking on an object on the canvas will display its properties on the right.
- **Add Data sections** - Dragging a data object onto a canvas that it does not belong to will create a data section for that table. Care must be taken to drag these data objects in the correct order as the data flows. Sections can be deleted by right-clicking on the canvas, but cannot be moved.
- **Link section data together** - Drag all the required IDs onto their respective data canvases. If you don't want to see the IDs on the report, set **Is Suppressed** to **Checked**.
  - Click on the **Child Data Canvas** to display the canvas properties.
  - In the **Parent Section**, select the section that can be used to get the parent IDs from. This will provide the following drop-down options:
    - The Child canvas data objects
    - The Parent canvas data objects
    - The `WHERE` `AND/OR` clause
  - Select the appropriate values. Selecting **AND** or **OR** will create a new list for entering additional conditions.

**Save the report before testing.**

**Hints**
- Use arrow keys to move selected object.
- Use **Cover Page**, **Page Header and Footer**, or **Final Page** templates with company information. Reference the template when creating a report, and it will automatically print with the report.
- Click on the ruler to add markers.
- Resizing an data object will cause it to wrap its text. By default it will overflow.
---";
            }
        }
    }
}
