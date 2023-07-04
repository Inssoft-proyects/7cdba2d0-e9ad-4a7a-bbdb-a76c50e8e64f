using GuanajuatoAdminUsuarios.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using System.IO;

namespace GuanajuatoAdminUsuarios.Controllers
{
    public partial class BaseController : Controller
    {
        public readonly IViewRenderService _viewRenderService;
        public BaseController(IViewRenderService viewRenderService)
        {
            _viewRenderService = viewRenderService;
        }

        
        //protected string RenderPartialViewToString(string viewName, object model)
        //{
        //    if (string.IsNullOrEmpty(viewName))
        //        viewName = ControllerContext.RouteData.GetRequiredString("action");

        //    ViewData.Model = model;

        //    using (StringWriter sw = new StringWriter())
        //    {
        //        ViewEngineResult viewResult =
        //        ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
        //        ViewContext viewContext = new ViewContext
        //        (ControllerContext, viewResult.View, ViewData, TempData, sw);
        //        viewResult.View.Render(viewContext, sw);

        //        return sw.GetStringBuilder().ToString();
        //    }
        //}
    }
}
