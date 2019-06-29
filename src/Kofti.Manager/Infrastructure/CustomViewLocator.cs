using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Razor;

namespace Kofti.Manager.Infrastructure
{
    public class CustomViewLocator : IViewLocationExpander
    {
        public void PopulateValues(ViewLocationExpanderContext context)
        {
        }

        public virtual IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context,
            IEnumerable<string> viewLocations)
        {
            return viewLocations.Select(f => f.Replace("/Views/", "/Pages/"));
        }
    }
}