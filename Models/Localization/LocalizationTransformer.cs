using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using Microsoft.Extensions.Caching.Memory;
using System.Reflection;
using System.Globalization;
using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace AspnetcoreLocalizationDemo.Models.Localization
{
    public class LocalizationTransformer : DynamicRouteValueTransformer
    {
        private readonly IMemoryCache memoryCache;
        public readonly IEnumerable<CultureInfo> supportedCultures;

        public LocalizationTransformer(IMemoryCache memoryCache, IEnumerable<CultureInfo> supportedCultures)
        {
            this.supportedCultures = supportedCultures;
            this.memoryCache = memoryCache;
        }
        public override ValueTask<RouteValueDictionary> TransformAsync(HttpContext httpContext, RouteValueDictionary values)
        {
            var controllerName = values["controller"] as string;
            var actionName = values["action"] as string;
            var language = values["language"] as string;
            var culture = supportedCultures.FirstOrDefault(culture => culture.TwoLetterISOLanguageName.Equals(language, StringComparison.InvariantCultureIgnoreCase)) ?? CultureInfo.CurrentCulture;
            var localizer = httpContext.RequestServices.GetService<IStringLocalizer>().WithCulture(culture);
            var controllerKey = MakeControllerKey(culture, controllerName);
            var actionKey = MakeActionKey(culture, controllerName, actionName);

            var reverseMap = memoryCache.GetOrCreate("LocalizationTransformerReverseMap", entry => CreateReverseMap(httpContext, entry));
            if (reverseMap.ContainsKey(controllerKey) && reverseMap.ContainsKey(actionKey))
            {
                // Riscrivi
                values["controller"] = reverseMap[controllerKey];
                values["action"] = reverseMap[actionKey];
            }
            else
            {
                //TODO: Scrivi riga di log per mancata riscrittura
                throw new KeyNotFoundException();
            }
            return new ValueTask<RouteValueDictionary>(values);
        }

        private Dictionary<string, string> CreateReverseMap(HttpContext httpContext, ICacheEntry arg)
        {
            arg.SlidingExpiration = TimeSpan.MaxValue;

            var localizer = httpContext.RequestServices.GetService<IStringLocalizer>();
            var supportedCultures = httpContext.RequestServices.GetService<IEnumerable<CultureInfo>>().ToList();

            // Ottengo l'elenco dei controller in questo assembly
            // ATTENZIONE: questo significa che non troverà i controller se si trovano in altri progetti referenziati da questo
            var controllerTypes = Assembly.GetExecutingAssembly().GetTypes().Where(type => typeof(ControllerBase).IsAssignableFrom(type)).ToList();
            // Mi creo un dizionario per la risoluzione inversa, cioè Libro => Book e Capitolo1 => Chapter1
            // perché il localizer riesce solo a tradurmi Book => Libro e Chapter1 => Capitolo1
            var reverseMap = new Dictionary<string, string>();
            // Aggiungo le risoluzioni inverse per le varie lingue
            // Questa è un'operazione che avrà un impatto sul tempo di avvio dell'applicazione (da misurare con uno Stopwatch, se l'applicazione ha dimensioni importanti e molte lingue supportate)
            supportedCultures.ForEach(culture =>
            {
                // Aggiungo la risoluzione inversa dei nomi dei controller per tutte le lingue
                // Per qualche motivo Microsoft ha reso obsoleto il metodo WithCulture perciò questo codice andrà sostituito in versioni successive alla 3.x
                var currentLocalizer = localizer.WithCulture(culture);
                controllerTypes.ForEach(type =>
                {

                    string controllerName = type.Name.Replace("Controller", "");
                    string localizedControllerName = currentLocalizer.GetString($"Routing.{controllerName}");
                    reverseMap.Add(MakeControllerKey(culture, localizedControllerName), controllerName);
                    // Aggiungo le sue action
                    var actions = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.InvokeMethod).Where(action => !action.IsSpecialName && !action.GetCustomAttributes().Any(attribute => attribute is NonActionAttribute)).ToList();
                    actions.ForEach(action =>
                    {
                        string actionName = action.Name;
                        string localizedActionName = currentLocalizer.GetString($"Routing.{controllerName}.{actionName}");
                        reverseMap.Add(MakeActionKey(culture, localizedControllerName, localizedActionName), actionName);
                    });
                });
            });

            return reverseMap;
        }

        private string MakeControllerKey(CultureInfo culture, string controllerName)
        {
            return $"{culture.TwoLetterISOLanguageName}.{controllerName}";
        }
        private string MakeActionKey(CultureInfo culture, string controllerName, string actionName)
        {
            string controllerKey = MakeControllerKey(culture, controllerName);
            return $"{controllerKey}.{actionName}";
        }
    }
}