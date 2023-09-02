using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Extensions.Logging;
using Translumo.Infrastructure.Constants;
using Translumo.Infrastructure.Language;
using Translumo.Translation.Configuration;
using Translumo.Translation.Exceptions;
using Translumo.Utils.Http;

namespace Translumo.Translation.OneRing
{
    public class OneRingTranslator : BaseTranslator<OneRingContainer>
    {
        private const string TRANSLATE_URL = "http://127.0.0.1:4990/translate?from_lang={0}&to_lang={1}&text={2}";

        public OneRingTranslator(TranslationConfiguration translationConfiguration, LanguageService languageService, ILogger logger)
            : base(translationConfiguration, languageService, logger)
        {
        }

        protected override async Task<string> TranslateTextInternal(OneRingContainer container, string sourceText)
        {
            string url = string.Format(TRANSLATE_URL, SourceLangDescriptor.IsoCode, TargetLangDescriptor.IsoCode,
                HttpUtility.UrlEncode(sourceText));
            HttpResponse requestResult = await container.Reader.RequestWebDataAsync(url, HttpMethods.GET, true)
                .ConfigureAwait(false);
            dynamic res = Newtonsoft.Json.Linq.JObject.Parse(requestResult.Body);
            return res.result;
        }

        protected override IList<OneRingContainer> CreateContainers(TranslationConfiguration configuration)
        {
            var result = configuration.ProxySettings.Select(proxy => new OneRingContainer(proxy)).ToList();
            result.Add(new OneRingContainer(isPrimary: true));

            return result;
        }
    }
}
