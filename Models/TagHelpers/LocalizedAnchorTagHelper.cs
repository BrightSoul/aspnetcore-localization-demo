using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace AspnetcoreLocalizationDemo.Models.Services
{
    [HtmlTargetElement("a", TagStructure = TagStructure.NormalOrSelfClosing)]
    public class LocalizedAnchorTagHelper : AnchorTagHelper
    {
        public LocalizedAnchorTagHelper(IHtmlGenerator generator) : base(generator)
        {
        }

/*        [ViewContext]
 [HtmlAttributeNotBound]
 public ViewContext ViewContext { get; set; }*/

        public override int Order => int.MinValue;

        //http://ziyad.info/en/articles/12-Configuring_Culture_Route_Model
        //http://ziyad.info/en/articles/13-Localizing_Request


        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (!string.IsNullOrEmpty(Action))
            {
                Action = "Fooo";
            }
            base.Process(context, output);
        }
    }
}