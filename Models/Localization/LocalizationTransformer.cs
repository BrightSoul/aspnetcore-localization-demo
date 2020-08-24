using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using Microsoft.Extensions.Caching.Memory;
using System.Globalization;
using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Primitives;

namespace AspnetcoreLocalizationDemo.Models.Localization
{
    public class LocalizationTransformer : DynamicRouteValueTransformer
    {
        private readonly IMemoryCache memoryCache;
        public readonly IEnumerable<CultureInfo> supportedCultures;

        //TODO: Vedi se questi riesci a ottenerli in qualche altra maniera
        private const string controller = "controller";
        private const string action = "action";
        private const string language = "language";

        public LocalizationTransformer(IMemoryCache memoryCache, IEnumerable<CultureInfo> supportedCultures)
        {
            this.supportedCultures = supportedCultures;
            this.memoryCache = memoryCache;
        }
        public override ValueTask<RouteValueDictionary> TransformAsync(HttpContext httpContext, RouteValueDictionary values)
        {
            //httpContext.Request.Query = httpContext.Request.Query.Append(new KeyValuePair<string, StringValues>("pippo", new StringValues("puppo")));
            var languageName = values[language] as string;
            var culture = supportedCultures.FirstOrDefault(culture => culture.TwoLetterISOLanguageName.Equals(languageName, StringComparison.InvariantCultureIgnoreCase)) ?? CultureInfo.CurrentCulture;
            var localizer = httpContext.RequestServices.GetService<IStringLocalizer>().WithCulture(culture);
            var reverseMap = memoryCache.GetOrCreate("LocalizationTransformerReverseMap", entry => CreateReverseMap(httpContext, entry));

            TransformRouteValues(httpContext, reverseMap, culture, localizer, values);
            TransformQueryString(httpContext, reverseMap, culture, localizer);

            return new ValueTask<RouteValueDictionary>(values);
        }

        private void TransformRouteValues(HttpContext httpContext, Dictionary<string, string> reverseMap, CultureInfo culture, IStringLocalizer stringLocalizer, RouteValueDictionary values)
        {
            var controllerName = values[controller] as string;
            var actionName = values[action] as string;
            var controllerKey = MakeControllerKey(culture, controllerName);
            var actionKey = MakeActionKey(culture, controllerName, actionName);

            if (reverseMap.ContainsKey(controllerKey) && reverseMap.ContainsKey(actionKey))
            {
                // Riscrivi
                values[controller] = reverseMap[controllerKey];
                values[action] = reverseMap[actionKey];
            }
            else
            {
                //TODO: Scrivi riga di log per mancata riscrittura
                throw new KeyNotFoundException();
            }
        }

        private void TransformQueryString(HttpContext httpContext, Dictionary<string, string> reverseMap, CultureInfo culture, IStringLocalizer localizer)
        {
            var query = new QueryString();
            foreach (var parameter in httpContext.Request.Query)
            {
                var key = parameter.Key;
                var queryParameterKey = MakeQueryParameterKey(culture, key);
                if (reverseMap.ContainsKey(queryParameterKey))
                {
                    key = reverseMap[queryParameterKey];
                }
                query = query.Add(key, parameter.Value);
            }
            httpContext.Request.QueryString = query;
        }

        private Dictionary<string, string> CreateReverseMap(HttpContext httpContext, ICacheEntry arg)
        {
            arg.SlidingExpiration = TimeSpan.MaxValue;

            var localizer = httpContext.RequestServices.GetService<IStringLocalizer>();
            var supportedCultures = httpContext.RequestServices.GetService<IEnumerable<CultureInfo>>().ToList();
            var actionDescriptorProvider = httpContext.RequestServices.GetService<IActionDescriptorCollectionProvider>();
            // Ottengo l'elenco dei controller in questo assembly
            var actionDescriptorGroups = actionDescriptorProvider.ActionDescriptors.Items.OfType<ControllerActionDescriptor>().GroupBy(descriptor => descriptor.ControllerName).ToList();


            // Mi creo un dizionario per la risoluzione inversa, cioè Libro => Book e Capitolo1 => Chapter1
            // perché il localizer riesce solo a tradurmi Book => Libro e Chapter1 => Capitolo1
            var reverseMap = new Dictionary<string, string>();

            // Aggiungo le risoluzioni inverse per le varie lingue
            // Questa è un'operazione che avrà un impatto sul tempo di avvio dell'applicazione (da misurare con uno Stopwatch, se l'applicazione ha dimensioni importanti e molte lingue supportate)
            supportedCultures.ForEach(culture =>
            {
                // Aggiungo la risoluzione inversa dei nomi dei controller per tutte le lingue
                // Per qualche motivo Microsoft ha reso obsoleto il metodo WithCulture perciò questo codice andrà sostituito in versioni successive alla 3.x
                var specificCultureLocalizer = localizer.WithCulture(culture);
                AddRoutingEndpointsToReverseMap(reverseMap, culture, specificCultureLocalizer, actionDescriptorGroups);
                AddQueryParametersToReverseMap(reverseMap, culture, specificCultureLocalizer);
            });

            return reverseMap;
        }

        private void AddRoutingEndpointsToReverseMap(Dictionary<string, string> reverseMap, CultureInfo culture, IStringLocalizer localizer, List<IGrouping<string, ControllerActionDescriptor>> actionDescriptorGroups)
        {
            actionDescriptorGroups.ForEach(group =>
            {
                string controllerName = group.Key;
                string localizedControllerName = localizer.GetString($"Routing.{controllerName}");
                reverseMap.Add(MakeControllerKey(culture, localizedControllerName), controllerName);
                    // Aggiungo le sue action
                    group.Select(actionDescriptor => actionDescriptor.ActionName).Distinct().ToList().ForEach(actionName =>
                {
                    string localizedActionName = localizer.GetString($"Routing.{controllerName}.{actionName}");
                    reverseMap.Add(MakeActionKey(culture, localizedControllerName, localizedActionName), actionName);
                });
            });
        }

        private void AddQueryParametersToReverseMap(Dictionary<string, string> reverseMap, CultureInfo culture, IStringLocalizer localizer)
        {
            string queryParameterPrefix = "Query.";
            var queryParameterEntry = localizer.GetAllStrings(false).Where(key => key.Name.StartsWith(queryParameterPrefix)).ToList();
            queryParameterEntry.ForEach(entry => {
                var localizedQueryParameter = entry.Value;
                var originalQueryParameter = entry.Name.Substring(queryParameterPrefix.Length);
                var queryParameterKey = MakeQueryParameterKey(culture, localizedQueryParameter);
                reverseMap.Add(queryParameterKey, originalQueryParameter);
            });
        }

        private string MakeQueryParameterKey(CultureInfo culture, string key)
        {
            return $"Query.{culture.TwoLetterISOLanguageName}.{key}";
        }

        private string MakeControllerKey(CultureInfo culture, string controllerName)
        {
            return $"Routing.{culture.TwoLetterISOLanguageName}.{controllerName}";
        }
        private string MakeActionKey(CultureInfo culture, string controllerName, string actionName)
        {
            string controllerKey = MakeControllerKey(culture, controllerName);
            return $"{controllerKey}.{actionName}";
        }
    }
}