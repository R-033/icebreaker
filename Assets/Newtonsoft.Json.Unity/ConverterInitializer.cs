namespace Newtonsoft.Json.Unity
{
    public static class ConverterInitializer
    {
		private static bool ms_initialized;

		public static void Initialize()
        {
			if (ms_initialized) return;
			
            JsonSerializerSettings currentSettings = JsonConvert.DefaultSettings?.Invoke() ?? new JsonSerializerSettings();

			currentSettings.Converters.Add(new JsonColorConverter());
            currentSettings.Converters.Add(new JsonVector2Converter());
            currentSettings.Converters.Add(new JsonVector3Converter());
            currentSettings.Converters.Add(new JsonVector4Converter());
            currentSettings.Converters.Add(new JsonQuaternionConverter());
			//currentSettings.TypeNameHandling = TypeNameHandling.All;

            JsonConvert.DefaultSettings = () => currentSettings;
            
            ms_initialized = true;
        }
    }
}
