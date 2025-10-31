namespace ASP.net_8_Entity_Framework_RESTful_API.Classes.Models
{
    public class PrinterItem
    {
        public string Name { get; set; } = string.Empty;
        public string IP { get; set; } = string.Empty;
        public int Port { get; set; }
    }
    public class SettingsConfiguration
    {        
        public List<PrinterItem> PrinterSettings { get; set; } = new List<PrinterItem>();
    }
}
