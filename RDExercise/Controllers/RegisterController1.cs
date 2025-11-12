using Microsoft.AspNetCore.Mvc;
using RDExercise.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace RDExercise.Controllers
{
    public class RegisterController : Controller
    {
        private readonly HttpClient _httpClient;

        public RegisterController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7298/"); // <-- Replace with your API URL
        }

        public async Task<IActionResult> Index()
        {
            var users = await _httpClient.GetFromJsonAsync<List<RDExerciseModel>>("register");
            return View(users);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(RDExerciseModel model)
        {
            if (ModelState.IsValid)
            {
                var response = await _httpClient.PostAsJsonAsync("register", model);
                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "User registered successfully!";
                    return RedirectToAction("Index");
                }
                ModelState.AddModelError("", "Failed to register user.");
            }
            return View(model);
        }
    }
}
