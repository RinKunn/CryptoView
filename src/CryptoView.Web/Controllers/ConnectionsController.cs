using CryptoView.Web.Interfaces;
using CryptoView.Web.Models.Connections;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CryptoView.Web.Controllers
{
    [Authorize]
    public class ConnectionsController : Controller
    {
        private readonly IConnectionsService _exConnectorsService;

        public ConnectionsController(IConnectionsService exConnectorsService)
        {
            _exConnectorsService = exConnectorsService;
        }

        [TempData]
        public string StatusMessage { get; set; }


        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var connectors = await _exConnectorsService.GetConnectionsForUser(userId);
            var viewModel = connectors?.ToList();
            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var viewModel = new ConnectionViewModel
            {
                Exchanges = await _exConnectorsService.GetExchanges()
            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ConnectionViewModel entryModel)
        {
            if (ModelState.IsValid)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var model = new Connection
                {
                    ApiKey = entryModel.ApiKey,
                    ApiSecret = entryModel.ApiSecret,
                    ExchangeId = entryModel.ExchangeId,
                    Created = DateTime.UtcNow,
                    UserId = userId,
                    IsTradingLocked = true,
                };

                if (await _exConnectorsService.CreateConnection(model))
                {
                    StatusMessage = $"New API created successfully!";
                    return RedirectToAction(nameof(Index));
                }
            }
            entryModel.Exchanges = await _exConnectorsService.GetExchanges();
            return View(entryModel);
        }


        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var conn = await _exConnectorsService.GetConnectionById(id);
            if(conn == null)
            {
                return NotFound(id);
            }
            if(conn.UserId != userId)
            {
                return Forbid();
            }

            var viewModel = new ConnectionViewModel
            {
                ApiKey = conn.ApiKey,
                ApiSecret = conn.ApiSecret,
                ExchangeId = conn.ExchangeId,
                Exchanges = await _exConnectorsService.GetExchanges(),
            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string id, ConnectionViewModel entryModel)
        {
            if (ModelState.IsValid)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var conn = await _exConnectorsService.GetConnectionById(id);
                if (conn == null)
                {
                    return NotFound(id);
                }
                if(conn.UserId != userId)
                {
                    return Forbid();
                }
                conn.ExchangeId = entryModel.ExchangeId;
                conn.ApiKey = entryModel.ApiKey;
                conn.ApiSecret = entryModel.ApiSecret;
                if(await _exConnectorsService.UpdateConnection(conn))
                {
                    StatusMessage = "Connection changed successfully!";
                    return RedirectToAction(nameof(Index));
                }
            }
            entryModel.Exchanges = await _exConnectorsService.GetExchanges();
            return View(entryModel);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var conn = await _exConnectorsService.GetConnectionById(id);
            if (conn == null)
            {
                return NotFound(id);
            }
            if (conn.UserId != userId)
            {
                return Forbid();
            }

            var viewModel = new ConnectionViewModel
            {
                ApiKey = conn.ApiKey,
                ApiSecret = conn.ApiSecret,
                ExchangeId = conn.ExchangeId,
                Exchanges = await _exConnectorsService.GetExchanges(),
            };
            return View(viewModel);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirm(string id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var conn = await _exConnectorsService.GetConnectionById(id);
            if (conn == null || id == null)
            {
                return NotFound(id);
            }
            if (conn.UserId != userId)
            {
                return Forbid();
            }
            await _exConnectorsService.DeleteConnection(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
