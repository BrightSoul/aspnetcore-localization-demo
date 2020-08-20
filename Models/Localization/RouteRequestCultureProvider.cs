using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;

namespace AspnetcoreLocalizationDemo.Models.Localization
{
    public class RouteRequestCultureProvider : IRequestCultureProvider
    {
        private readonly IEnumerable<CultureInfo> supportedCultures;

        public RouteRequestCultureProvider(IEnumerable<CultureInfo> supportedCultures)
        {
            this.supportedCultures = supportedCultures;
        }

        public Task<ProviderCultureResult> DetermineProviderCultureResult(HttpContext httpContext)
        {
            var languageRouteValue = httpContext.Request.RouteValues.Where(r => r.Key == "language").ToList();
            CultureInfo selectedCulture = null;
            if (languageRouteValue.Any())
            {
                var selectedLanguage = languageRouteValue.First().Value as string;
                selectedCulture = supportedCultures.FirstOrDefault(culture => culture.TwoLetterISOLanguageName.Equals(selectedLanguage, StringComparison.InvariantCultureIgnoreCase));
            }
            selectedCulture = selectedCulture ?? supportedCultures.First();
            var result = new ProviderCultureResult(selectedCulture.TwoLetterISOLanguageName);
            return Task.FromResult(result);
        }
    }
}