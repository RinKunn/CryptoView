using CryptoView.Web.Interfaces;
using CryptoView.Web.Models.ApiManagement;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Security.Claims;

namespace CryptoView.Web.Controllers
{
    [Authorize]
    public class ApiManagementController : Controller
    {
        // Access to the API by the exchange ID is implemented here
        // because each exchange can only have one API connection per user

        private readonly IApiService _apiService;

        public ApiManagementController(IApiService apiService)
        {
            _apiService = apiService ?? throw new ArgumentNullException(nameof(apiService));
        }

        [TempData]
        public string StatusMessage { get; set; }


        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = GetUserId(User);
            var viewModel = new IndexViewModel
            {
                APIs = await _apiService.GetApiList(userId),
                ExchangesWithNoAPI = await _apiService.ExchangesWithNoAPI(userId),
            };
            return View(viewModel);
        }

        
        [HttpGet]
        public async Task<IActionResult> Create(int exchangeId)
        {
            var exchange = await _apiService.GetExchangeById(exchangeId);
            if (exchange == null)
            {
                return NotFound();
            }
            // If a connection to the exchange already exists for the user, then redirect to the Edit action
            var userId = GetUserId(User);
            var api = await _apiService.GetApiForExchange(exchangeId, userId);
            if (api != null)
            {
                StatusMessage = $"The API already exists for the {api.ExchangeInfo.Name} you wanted to create";
                return RedirectToAction(nameof(Edit), new { exchangeId = exchange.Id });
            }

            var viewModel = new ApiInfo
            {
                ExchangeInfo = exchange
            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(int exchangeId,
            [Bind("Key", "Secret")] ApiInfo entryModel)
        {
            entryModel.ExchangeInfo = await _apiService.GetExchangeById(exchangeId);

            if (ModelState.IsValid)
            {
                var userId = GetUserId(User);

                // TODO: check connection by ping pong
                try
                {
                    await _apiService.CreateApi(entryModel, userId);
                    StatusMessage = $"New API for '{entryModel.ExchangeInfo.Name}' is created successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Creation of API for '{entryModel.ExchangeInfo.Name}' is failed: {ex.Message}");
                }
            }
            return View(entryModel);
        }


        [HttpGet]
        public async Task<IActionResult> Edit(int exchangeId)
        {
            var userId = GetUserId(User);
            var api = await _apiService.GetApiForExchange(exchangeId, userId);
            if (api == null)
            {
                return NotFound();
            }
            if (!string.IsNullOrEmpty(StatusMessage))
                ModelState.AddModelError("", StatusMessage);
            return View(api);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int exchangeId, 
            [Bind("Key", "Secret")] ApiInfo entryModel)
        {
            var userId = GetUserId(User);
            var api = await _apiService.GetApiForExchange(exchangeId, userId);
            if (api == null)
            {
                return NotFound();
            }
            api.Key = entryModel.Key;
            api.Secret = entryModel.Secret;

            if (ModelState.IsValid)
            {
                try
                {
                    await _apiService.UpdateApi(api, userId);
                    StatusMessage = $"API for '{api.ExchangeInfo.Name}' is changed successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Updating of API for '{api.ExchangeInfo.Name}' is failed: {ex.Message}");
                }
            }
            return View(api);
        }


        [HttpGet]
        public async Task<IActionResult> Delete(int exchangeId)
        {
            var userId = GetUserId(User);
            var api = await _apiService.GetApiForExchange(exchangeId, userId);
            if (api == null)
            {
                return NotFound();
            }
            return View(api);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirm(int exchangeId)
        {
            var userId = GetUserId(User);
            var api = await _apiService.GetApiForExchange(exchangeId, userId);
            try
            {
                await _apiService.DeleteApi(api.Id, userId);
                StatusMessage = $"API for '{api.ExchangeInfo.Name}' is deleted successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Deleting of API for '{api.ExchangeInfo.Name}' is failed: {ex.Message}");
            }
            return View(api);
        }

        private string GetUserId(ClaimsPrincipal user) 
            => user.FindFirstValue(ClaimTypes.NameIdentifier);
    }

}
