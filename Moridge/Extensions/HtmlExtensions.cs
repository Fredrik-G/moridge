using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Moridge.Extensions
{
    public static class HtmlExtensions
    {
        public static MvcHtmlString MaterialDesignInput<TModel, TValue>(this HtmlHelper<TModel> helper,
            Expression<Func<TModel, TValue>> expression, string labelText, bool isRequired, string type = "text", object htmlAttributes = null)
        {
            IDictionary<string, object> attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            attributes.Add("type", type);
            attributes.Add("data-role", "none");
            if (isRequired)
            {
                attributes.Add("required", string.Empty);
            }
            else
            {
                //get expression value
                var value = (ModelMetadata.FromLambdaExpression(expression, helper.ViewData).Model ?? string.Empty).ToString();
                //add not-empty class if not empty, otherwise add empty
                attributes.Add("class", (string.IsNullOrEmpty(value) ? "empty" : "not-empty") + " not-required");
            }
            var content = "<div data-role='none' class='group'>" +
                          helper.TextBoxFor(expression, attributes) +
                          "<span data-role='none' class='highlight'></span>" +
                          "<span data-role='none' class='bar'></span>" +
                          "<label data-role='none'>" + labelText + "</label>" +
                          "</div>";
            return new MvcHtmlString(content);
        }
    }
}