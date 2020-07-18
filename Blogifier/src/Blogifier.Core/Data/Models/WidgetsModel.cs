using System.Collections.Generic;

namespace Blogifier.Core.Data
{
    public class WidgetsModel
    {
        public List<WidgetItem> Widgets { get; set; }
    }

    public class WidgetItem
    {
        public string Widget { get; set; }
        public string Title { get; set; }
    }
}