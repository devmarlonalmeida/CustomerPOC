using ClientesPOCUI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using Shared.Utils;
using System.Net.Http.Headers;
using System.Text;

namespace ClientesPOCUI.Controllers
{
    public class CustomerController : Controller
    {
        private readonly HttpClient _httpClient;

        public CustomerController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
        }

        public IActionResult Create() => View(new CustomerViewModel());

        public async Task<IActionResult> Edit(Guid customerId, CustomerViewModel? invalidViewModel = null)
        {
            if(invalidViewModel != null && invalidViewModel.Id != Guid.Empty)
            {
                return View(invalidViewModel);
            }

            AddAuthorizationHeader();

            var response = await _httpClient.GetAsync($"api/customer/{customerId}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var customer = JsonConvert.DeserializeObject<CustomerViewModel>(content);

            if (customer == null)
            {
                return NotFound();
            }

            if(customer.LogoBytes?.Length > 0)
            {
                customer.Logo = FileUtils.ConvertByteArrayToIFormFile(customer.LogoBytes, customer.LogoFileName, customer.LogoContentType);
            }

            ModelState.Clear();

            return View(customer);
        }

        private void AddAuthorizationHeader()
        {
            var token = HttpContext.Session.GetString("JwtToken");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        public async Task<IActionResult> Index()
        {
            AddAuthorizationHeader();

            var response = await _httpClient.GetAsync("api/customer");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var customers = JsonConvert.DeserializeObject<List<CustomerViewModel>>(content);

            foreach(var customer in customers)
            {
                if (customer.LogoBytes?.Length > 0)
                {
                    customer.Logo = FileUtils.ConvertByteArrayToIFormFile(customer.LogoBytes, customer.LogoFileName, customer.LogoContentType);
                }
            }

            return View(customers);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CustomerViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                    if (model.Addresses == null || model.Addresses.Count <= 0)
                    {
                        ModelState.AddModelError("Addresses", "O cliente deve ter pelo menos um endereço.");
                    }

                    return View(model);
                }

                if (model.Addresses == null || model.Addresses.Count <= 0)
                {
                    ModelState.AddModelError("Addresses", "O cliente deve ter pelo menos um endereço.");
                    return View(model);
                }

                AddAuthorizationHeader();

                var content = new MultipartFormDataContent
            {
                { new StringContent(model.Name), "Name" },
                { new StringContent(model.Email), "Email" }
            };

                if (model.Logo != null)
                {
                    var memoryStream = new MemoryStream();
                    await model.Logo.CopyToAsync(memoryStream);
                    memoryStream.Position = 0;

                    var streamContent = new StreamContent(memoryStream);
                    streamContent.Headers.ContentType = new MediaTypeHeaderValue(model.Logo.ContentType);
                    content.Add(streamContent, "Logo", model.Logo.FileName);
                }

                if (model.Addresses != null && model.Addresses.Any())
                {
                    int index = 0;
                    foreach (var address in model.Addresses)
                    {
                        content.Add(new StringContent(address.Street ?? string.Empty), $"Addresses[{index}].Street");
                        content.Add(new StringContent(address.City ?? string.Empty), $"Addresses[{index}].City");
                        content.Add(new StringContent(address.State ?? string.Empty), $"Addresses[{index}].State");
                        index++;
                    }
                }

                HttpResponseMessage response = await _httpClient.PostAsync("api/customer", content);

                if (response.IsSuccessStatusCode)
                {
                    ViewData["SuccessMessage"] = "Cliente atualizado com sucesso!";
                    return RedirectToAction("Index");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    var errorResponse = JsonConvert.DeserializeObject<dynamic>(errorContent);

                    string errorMessage = errorResponse?.message;

                    ModelState.AddModelError(string.Empty, $"Erro: {errorMessage}");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Ocorreu um erro inesperado: {ex.Message}");
            }
            
            return View(model);
        }

        public async Task<IActionResult> Update(CustomerViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                    if (model.Addresses == null || model.Addresses.Count <= 0)
                    {
                        ModelState.AddModelError("Addresses", "O cliente deve ter pelo menos um endereço.");
                    }

                    return View("Edit", model);
                }

                if (model.Addresses == null || model.Addresses.Count <= 0)
                {
                    ModelState.AddModelError("Addresses", "O cliente deve ter pelo menos um endereço.");
                    return View(model);
                }

                AddAuthorizationHeader();

                using var content = new MultipartFormDataContent
            {
                { new StringContent(model.Name), "Name" },
                { new StringContent(model.Email), "Email" }
            };

                if (model.Addresses != null && model.Addresses.Any())
                {
                    int index = 0;
                    foreach (var address in model.Addresses)
                    {
                        content.Add(new StringContent(address.Street ?? string.Empty), $"Addresses[{index}].Street");
                        content.Add(new StringContent(address.City ?? string.Empty), $"Addresses[{index}].City");
                        content.Add(new StringContent(address.State ?? string.Empty), $"Addresses[{index}].State");
                        index++;
                    }
                }

                if (model.Logo != null)
                {
                    var streamContent = new StreamContent(model.Logo.OpenReadStream());
                    streamContent.Headers.ContentType = new MediaTypeHeaderValue(model.Logo.ContentType);
                    content.Add(streamContent, "Logo", model.Logo.FileName);
                }

                HttpResponseMessage response = await _httpClient.PutAsync($"api/customer/{model.Id}", content);

                if (response.IsSuccessStatusCode)
                {
                    ViewData["SuccessMessage"] = "Cliente atualizado com sucesso!";
                    return RedirectToAction("Index");
                }
                else
                {
                    // If not successful, read the error response content
                    var errorContent = await response.Content.ReadAsStringAsync();
                    var errorResponse = JsonConvert.DeserializeObject<dynamic>(errorContent);

                    // You can access the error message like this:
                    string errorMessage = errorResponse?.message;

                    ModelState.AddModelError(string.Empty, $"Erro: {errorMessage}");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Ocorreu um erro inesperado: {ex.Message}");
            }

            return View("Edit", model);
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            AddAuthorizationHeader();

            HttpResponseMessage response = await _httpClient.DeleteAsync($"api/customer/{id}");

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

            return View();
        }

        public async Task<IActionResult> Details(Guid id)
        {
            AddAuthorizationHeader();

            var response = await _httpClient.GetAsync($"api/customer/{id}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var customer = JsonConvert.DeserializeObject<CustomerViewModel>(content);

            return View(customer);
        }
    }
}
