﻿using System.Web;
using System.Web.Mvc;

namespace Raizen.UniCad.Web
{
    public static class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}