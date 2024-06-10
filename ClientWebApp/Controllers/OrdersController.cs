using ClientWebApp.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClientWebApp.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;

        public OrdersController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(OrderViewModel model)
        {
            if (ModelState.IsValid)
            {
                var client = _clientFactory.CreateClient();
                client.BaseAddress = new Uri("http://localhost:5212");
                var response = await client.PostAsJsonAsync("/api/orders", model);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError(string.Empty, "An error occurred while creating the order.");
            }
            return View(model);
        }
    }
}
