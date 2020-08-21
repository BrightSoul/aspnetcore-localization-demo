using System.Globalization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.Linq;

namespace AspnetcoreLocalizationDemo.Models.Localization
{
    public class LocalizedLinkGenerator : LinkGenerator
    {
        private readonly LinkGenerator linkGenerator;
        public IHttpContextAccessor Accessor { get; }
        private readonly IEnumerable<CultureInfo> supportedCultures;
        private readonly IStringLocalizer stringLocalizer;

        public LocalizedLinkGenerator(LinkGenerator linkGenerator, IStringLocalizer stringLocalizer, IEnumerable<CultureInfo> supportedCultures)
        {
            this.stringLocalizer = stringLocalizer;
            this.supportedCultures = supportedCultures;
            this.linkGenerator = linkGenerator;
        }

        private void RewriteValuesDictionary(RouteValueDictionary values)
        {
            var language = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
            if (values.ContainsKey("language"))
            {
                language = values["language"] as string;
            }
            var culture = supportedCultures.FirstOrDefault(culture => culture.TwoLetterISOLanguageName == language as string) ?? CultureInfo.CurrentCulture;
            var currentLocalizer = stringLocalizer.WithCulture(culture);
            var controllerKey = $"Routing.{values["controller"]}";
            var actionKey = $"{controllerKey}.{values["action"]}";
            var localizedController = currentLocalizer.GetString(controllerKey);
            var localizedAction = currentLocalizer.GetString(actionKey);
            if (localizedController.ResourceNotFound || localizedAction.ResourceNotFound)
            {
                return;
            }
            values["controller"] = localizedController.Value;
            values["action"] = localizedAction.Value;
        }

        public override string GetPathByAddress<TAddress>(HttpContext httpContext, TAddress address, RouteValueDictionary values, RouteValueDictionary ambientValues = null, PathString? pathBase = null, FragmentString fragment = default, LinkOptions options = null)
        {
            RewriteValuesDictionary(values);
            return linkGenerator.GetPathByAddress(httpContext, address, values, ambientValues, pathBase, fragment, options);
        }

        public override string GetPathByAddress<TAddress>(TAddress address, RouteValueDictionary values, PathString pathBase = default, FragmentString fragment = default, LinkOptions options = null)
        {
            RewriteValuesDictionary(values);
            return linkGenerator.GetPathByAddress(address, values, pathBase, fragment, options);
        }

        public override string GetUriByAddress<TAddress>(HttpContext httpContext, TAddress address, RouteValueDictionary values, RouteValueDictionary ambientValues = null, string scheme = null, HostString? host = null, PathString? pathBase = null, FragmentString fragment = default, LinkOptions options = null)
        {
            RewriteValuesDictionary(values);
            return linkGenerator.GetUriByAddress(httpContext, address, values, ambientValues, scheme, host, pathBase, fragment, options);
        }

        public override string GetUriByAddress<TAddress>(TAddress address, RouteValueDictionary values, string scheme, HostString host, PathString pathBase = default, FragmentString fragment = default, LinkOptions options = null)
        {
            RewriteValuesDictionary(values);
            return linkGenerator.GetUriByAddress(address, values, scheme, host, pathBase, fragment, options);
        }
    }
}