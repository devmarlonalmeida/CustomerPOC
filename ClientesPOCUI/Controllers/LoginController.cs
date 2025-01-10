using ClientesPOCUI.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace ClientesPOCUI.Controllers
{
    public class LoginController : Controller
    {
        private readonly HttpClient _httpClient;

        public LoginController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Prepare login request payload
            var json = JsonConvert.SerializeObject(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Send login request to the API
            var response = await _httpClient.PostAsync("api/login", content);

            if (response.IsSuccessStatusCode)
            {
                // Get the token from the response
                var responseContent = await response.Content.ReadAsStringAsync();
                var token = JsonConvert.DeserializeObject<LoginResponseViewModel>(responseContent);

                // Store the token in localStorage
                HttpContext.Session.SetString("JwtToken", token.Token);

                // Redirect to the home page or customer list
                return RedirectToAction("Index", "Customer");
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                var errorResponse = JsonConvert.DeserializeObject<dynamic>(errorContent);

                string errorMessage = errorResponse?.message;

                ModelState.AddModelError(string.Empty, $"Erro: {errorMessage}");
            }

            return View(model);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("JwtToken");
            return RedirectToAction("Index", "Login");
        }
    }
}
