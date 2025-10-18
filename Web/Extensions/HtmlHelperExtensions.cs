using Helpers.Extensions;

using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Web.Extensions
{
    public static class HtmlHelperExtensions
    {
        public static IEnumerable<SelectListItem> GetEnumSelectListWithDefaultValue<TEnum>(this IHtmlHelper htmlHelper, TEnum? defaultValue, string? defaultLabel = null)
            where TEnum : struct
        {
            var selectList = htmlHelper.GetEnumSelectList<TEnum>().ToList();
            selectList.Insert(0, new SelectListItem { Text = (defaultLabel ?? "Select All"), Value = "-1" });
            selectList.Single(x => x.Value == "-1").Selected = true;
            return selectList;
        }

        public static SelectList GetEnumSelectListWithStrings<TEnum>() where TEnum : Enum
        {
            var values = Enum.GetValues(typeof(TEnum))
                             .Cast<TEnum>()
                             .Select(e => new SelectListItem
                             {
                                 Value = e.GetEnumDescription(), // Convert enum value to string
                                 Text = e.ToString()
                             });

            return new SelectList(values, "Value", "Text");
        }

        public static Task<IHtmlContent> PartialAsync(this IHtmlHelper htmlHelper, string partialViewName, object model, string prefix)
        {
            var viewData = new ViewDataDictionary(htmlHelper.ViewData);
            var htmlPrefix = viewData.TemplateInfo.HtmlFieldPrefix;
            viewData.TemplateInfo.HtmlFieldPrefix += !Equals(htmlPrefix, string.Empty) ? $".{prefix}" : prefix;
            return htmlHelper.PartialAsync(partialViewName, model, viewData);
        }

        public static Task<IHtmlContent> PartialAsync(this IHtmlHelper htmlHelper, string partialViewName, object model, string prefix, ViewDataDictionary viewDataDictionary)
        {
            var viewData = new ViewDataDictionary(viewDataDictionary);
            var htmlPrefix = viewData.TemplateInfo.HtmlFieldPrefix;
            viewData.TemplateInfo.HtmlFieldPrefix += !Equals(htmlPrefix, string.Empty) ? $".{prefix}" : prefix;
            return htmlHelper.PartialAsync(partialViewName, model, viewData);
        }

        public static async Task<string> RenderViewAsync<TModel>(this Controller controller, string viewName, TModel model, bool partial = false)
        {
            controller.ViewData.Model = model;
            using (var writer = new StringWriter())
            {
                var viewEngine = controller.HttpContext.RequestServices.GetService(typeof(ICompositeViewEngine)) as ICompositeViewEngine;
                var viewResult = viewEngine.FindView(controller.ControllerContext, viewName, !partial);

                if (!viewResult.Success)
                    throw new FileNotFoundException($"View {viewName} not found.");

                var viewContext = new ViewContext(
                    controller.ControllerContext,
                    viewResult.View,
                    controller.ViewData,
                    controller.TempData,
                    writer,
                    new HtmlHelperOptions()
                );

                await viewResult.View.RenderAsync(viewContext);
                return writer.GetStringBuilder().ToString();
            }
        }
    }
}
