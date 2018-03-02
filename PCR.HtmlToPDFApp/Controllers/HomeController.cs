using PCR.HtmlToPDFApp.Models;
using System;
using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace PCR.HtmlToPDFApp.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            return base.View();
        }

        [HttpPost, ValidateInput(false)]
        public async Task<ActionResult> Submit(FormCollection collection)
        {
            string htmlString = collection["htmlcontent"];
            string baseURL = ConfigurationManager.AppSettings["BaseURL"];
            htmlString = htmlString.Replace("&", "\\").Replace("#", "//");
            Helper helper = new Helper();
            helper.baseURL = baseURL;
            helper.htmlString = htmlString;
            HttpClient httpClient = new HttpClient();
            ActionResult result;
            try
            {
                byte[] array = null;
                httpClient.BaseAddress = new Uri(ConfigurationManager.AppSettings["WebAPIURL"]);
                httpClient.DefaultRequestHeaders.Accept.Clear();
                HttpResponseMessage httpResponseMessage = await httpClient.PostAsJsonAsync("api/HTMLCodeToPDF", helper);
                HttpResponseMessage Res = httpResponseMessage;
                if (Res.IsSuccessStatusCode)
                {
                    array = Res.Content.ReadAsByteArrayAsync().Result;
                }
                if (array != null)
                {
                    result = new FileContentResult(array, "application/pdf")
                    {
                        FileDownloadName = "Document.pdf"
                    };
                    return result;
                }
                array = null;
            }
            finally
            {
                if (httpClient != null)
                {
                    ((IDisposable)httpClient).Dispose();
                }
            }
            httpClient = null;
            result = null;
            return result;
        }
    }
}