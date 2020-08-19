using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Extensions.Localization;

namespace AspnetcoreLocalizationDemo.Models.Services
{
    public class CustomLocalizer : IStringLocalizer
    {
        private readonly IStringLocalizer localizer;
        public CustomLocalizer(IStringLocalizerFactory localizerFactory)
        {
            this.localizer = localizerFactory.Create("Shared", System.Reflection.Assembly.GetExecutingAssembly().FullName);
        }

        //Il nostro CustomLocalizer è di fatto un wrapper attorno all'IStringLocalizer creato dalla factory
        public LocalizedString this[string name] => localizer[name];
        public LocalizedString this[string name, params object[] arguments] => localizer[name, arguments];
        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures) => localizer.GetAllStrings(includeParentCultures);
        [Obsolete]
        public IStringLocalizer WithCulture(CultureInfo culture) => localizer.WithCulture(culture);
    }
}