using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Threading.Tasks;

namespace ApiLanguageWireAdan.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class TranslateController : ControllerBase
	{
		private readonly IStringLocalizer _localizer;

		public TranslateController(IStringLocalizer<Resource> localizer)
		{
			_localizer = localizer;
		}


		/// <summary>
		/// It translates. 
		/// </summary>
		/// <param name="textToTransform">the text to transform. It only works with "Hello, how are you?" in a supported language, unless you select Jerigonza and are in Spain</param>
		/// <param name="languageFrom">de-DE,en-US,es-ES,fr-FR,it-IT</param>
		/// <param name="languageTo">de-DE,en-US,es-ES,fr-FR,it-IT,Jerigonza</param>
		/// <example>
		/// languageFrom= es-ES, languageTo= en-US. textToTransform= "Hola, ¿cómo estás?" will result in "Hello, how are you?
		/// </example>
		/// <returns></returns>
		[HttpGet]
		public string Get(string textToTransform, string languageFrom, string languageTo)
		{
			if (languageFrom == languageTo)
				return textToTransform;

			if(languageTo.ToLower() == "jerigonza" && RegionInfo.CurrentRegion.DisplayName=="España")
			{
				return Jerigonzar(textToTransform);
			}

			try
			{
				CultureInfo cFrom = new CultureInfo(languageFrom);
				CultureInfo cTo = new CultureInfo(languageTo);

				ResourceManager resourceManager = new ResourceManager(typeof(Resource));
				var entry =
				resourceManager.GetResourceSet(cFrom, true, true)
				  .OfType<DictionaryEntry>()
				  .FirstOrDefault(e => e.Value.ToString() == textToTransform);

				var key = entry.Key;
				if (key != null)
				{
					string bodyResource = resourceManager.GetString(key.ToString(), cTo);
					return bodyResource;
				}
				else
				{
					return "Sorry, we still can't translate that, try to write the Hello, how are you? in a supported language and write that language as languageFrom.";
				}
			}
			catch (Exception ex)
			{
				return ex.Message;
			}
		}

		private string Jerigonzar(string textToTransform)
		{
			textToTransform= textToTransform
				.Replace("a", "apa")
				.Replace("e", "epe")
				.Replace("i", "ipi")
				.Replace("o", "opo")
				.Replace("u", "upu");
			return textToTransform;
		}


	}
}
