using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;
using WavePlayer.Configuration;

namespace WavePlayer.Localization
{
    public class LocalizationService : ILocalizationService
    {
        private const string ToolName = "System.Resources.Tools.StronglyTypedResourceBuilder";
        private const BindingFlags PropertyFlags = BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;

        private readonly IConfigurationService _configurationService;
        private readonly List<Type> _resourceTypes = new List<Type>();
        private readonly List<CultureInfo> _cultures = new List<CultureInfo>();
        private readonly Lazy<List<CultureInfo>> _allCultures = new Lazy<List<CultureInfo>>(InitializeCultures, true);

        public LocalizationService(IConfigurationService configurationService)
        {
            _configurationService = configurationService;
        }

        public event EventHandler<EventArgs> CurrentCultureChanged;

        public CultureInfo CurrentCulture { get; private set; }

        public IEnumerable<CultureInfo> AvailableCultures
        {
            get
            {
                return _cultures;
            }
        }

        public void SetCurrentCulture(int lcid, bool fallback = false)
        {
            if (CurrentCulture != null &&
                CurrentCulture.LCID == lcid)
            {
                return;
            }

            var culture = _cultures.SingleOrDefault(c => c.LCID == lcid);

            if (culture == null)
            {
                if (fallback)
                {
                    culture = CultureInfo.InvariantCulture;

                    Debug.WriteLine("Culture with lcid {0} was not found. Setup default culture: {1}", lcid, culture.EnglishName);
                }
                else
                {
                    var errorMessage = string.Format(WavePlayerLocalization.Culture, WavePlayerLocalization.LanguageNotFound);

                    throw new InvalidOperationException(errorMessage);
                }
            }

            foreach (var resourceType in _resourceTypes)
            {
                var prop = resourceType.GetProperty("Culture", PropertyFlags);
                prop.SetValue(null, culture, null);
            }

            CurrentCulture = culture;

            _configurationService.CultureId = culture.LCID;

            OnCurrentCultureChanged();
        }

        public void RegisterResource(Type resourceType)
        {
            if (resourceType == null)
            {
                throw new ArgumentNullException("resourceType");
            }

            var atttributes = resourceType.GetCustomAttributes<GeneratedCodeAttribute>(true);

            if (atttributes.All(atttribute => atttribute.Tool != ToolName))
            {
                var errorMessage = string.Format(WavePlayerLocalization.Culture, WavePlayerLocalization.TypeIsNotResource, resourceType);

                throw new InvalidOperationException(errorMessage);
            }

            if (_resourceTypes.Contains(resourceType))
            {
                return;
            }

            _resourceTypes.Add(resourceType);

            RegisterCultures(resourceType);
        }

        private void RegisterCultures(Type resourceType)
        {
            var resourceManager = (ResourceManager)resourceType.GetProperty("ResourceManager", PropertyFlags).GetValue(null, null);

            var cultures = from culture in _allCultures.Value
                           where !_cultures.Contains(culture) && resourceManager.GetResourceSet(culture, true, false) != null
                           select culture;

            _cultures.AddRange(cultures);
        }

        private void OnCurrentCultureChanged()
        {
            var handler = CurrentCultureChanged;

            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        private static List<CultureInfo> InitializeCultures()
        {
            var lcid = CultureInfo.InvariantCulture.LCID;

            return CultureInfo.GetCultures(CultureTypes.AllCultures).Where(culture => culture.LCID != lcid).ToList();
        }
    }
}
