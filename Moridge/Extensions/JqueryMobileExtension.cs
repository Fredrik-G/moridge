using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Moridge.Extensions
{
    public static class JqueryMobileExtension
    {
        /// <summary>
        /// Creates a flip switch.
        /// http://stackoverflow.com/a/16853459/4660537
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static MvcHtmlString FlipSwitchFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
            var value = (bool)(metadata.Model ?? false);

            var items = new List<SelectListItem>()
                    {
                        new SelectListItem() { Text = "", Value = "False", Selected = (!value) },
                        new SelectListItem() { Text = "", Value = "True", Selected = (value) }
                    };
            return htmlHelper.DropDownListFor(expression, items, new { @data_role = "slider", style = "width:50px" });
        }
    }
}