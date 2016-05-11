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
            Expression<Func<TModel, TValue>> expression, string labelText, bool isRequired, string value = null, object htmlAttributes = null)
        {
            IDictionary<string, object> attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            attributes.Add("type", "text");
            attributes.Add("data-role", "none");
            if (isRequired)
            {
                attributes.Add("required", string.Empty);
            }
            if (!string.IsNullOrEmpty(value))
            {
                attributes.Add("value", value);
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