using System.Web;
using System.Web.Mvc;
using NewIIDIprogram.Filters;

namespace NewIIDIprogram
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new AuthorizeSessionAttribute()); // <-- Add this line
        }
    }
}
