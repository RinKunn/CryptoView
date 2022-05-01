using CryptoView.UnitTests.Helper;
using CryptoView.Web.Controllers;
using CryptoView.Web.Interfaces;
using CryptoView.Web.Models.ApiManagement;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace CryptoView.UnitTests.Web.Controllers
{
    public class ApiManagementControllerTests
    {
        private List<ApiInfo> _apiList;
        private List<ExchangeInfo> _exchangeList;

        public ApiManagementControllerTests()
        {
            _exchangeList = CreateExchanges();
            _apiList = CreateExchangeConnections(_exchangeList);
        }

        [Fact]
        public async Task Index_WithListOfApi_ReturnsViewResult()
        {
            var userId = "user1";
            var mockService = new Mock<IApiService>();
            mockService.Setup(s => s.GetApiList(userId)).ReturnsAsync(_apiList);
            mockService.Setup(s => s.ExchangesWithNoAPI(userId)).ReturnsAsync(_exchangeList.Where(e => e.Id == 3));
            var controller = new ApiManagementController(mockService.Object)
                .WithIdentity(userId, userId);

            var result = await controller.Index();

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IndexViewModel>(viewResult.Model);
            Assert.Equal(2, model.APIs.Count());
            Assert.Single(model.ExchangesWithNoAPI);
        }

        [Fact]
        public async Task Index_WithEmptyList_ReturnsViewResult()
        {
            var mockService = new Mock<IApiService>();
            mockService.Setup(s => s.GetApiList(It.IsAny<string>())).ReturnsAsync(null as IEnumerable<ApiInfo>);
            mockService.Setup(s => s.ExchangesWithNoAPI(It.IsAny<string>())).ReturnsAsync(_exchangeList);
            var controller = new ApiManagementController(mockService.Object)
                .WithAnonymousIdentity();

            var result = await controller.Index();

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IndexViewModel>(viewResult.Model);
            Assert.Null(model.APIs);
            Assert.Equal(3, model.ExchangesWithNoAPI.Count());
        }

        #region Creation
        [Fact]
        public async Task CreateGet_ExchangeNotExists_ReturnNotFoundResult()
        {
            int exchangeId = 4;
            var mockService = new Mock<IApiService>();
            mockService.Setup(s => s.GetExchangeById(It.IsAny<int>())).ReturnsAsync(null as ExchangeInfo);
            var controller = new ApiManagementController(mockService.Object).WithAnonymousIdentity();

            var result = await controller.Create(exchangeId);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task CreateGet_ApiAlreadyExists_ReturnRedirectToEditAction()
        {
            string userId = "user1";
            var api = _apiList[0];
            var exchange = api.ExchangeInfo;
            int exchangeId = exchange.Id;
            var mockService = new Mock<IApiService>();
            mockService.Setup(s => s.GetExchangeById(exchangeId)).ReturnsAsync(exchange);
            mockService.Setup(s => s.GetApiForExchange(exchangeId, userId)).ReturnsAsync(api);
            var controller = new ApiManagementController(mockService.Object).WithIdentity(userId, userId);

            var result = await controller.Create(exchangeId);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Edit", redirectResult.ActionName);
            Assert.NotNull(redirectResult.RouteValues?["exchangeId"]);
            int routeId = Assert.IsAssignableFrom<int>(redirectResult.RouteValues?["exchangeId"]);
            Assert.Equal(exchangeId, routeId);
        }

        [Fact]
        public async Task CreateGet_ApiNotExists_ReturnViewResult()
        {
            string userId = "user1";
            var exchange = _exchangeList[0];
            int exchangeId = exchange.Id;
            var mockService = new Mock<IApiService>();
            mockService.Setup(s => s.GetApiForExchange(exchangeId, userId)).ReturnsAsync(null as ApiInfo);
            mockService.Setup(s => s.GetExchangeById(exchangeId)).ReturnsAsync(exchange);
            var controller = new ApiManagementController(mockService.Object).WithIdentity(userId, userId);

            var result = await controller.Create(exchangeId);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<ApiInfo>(viewResult.Model);
            Assert.NotNull(model.ExchangeInfo);
            Assert.Equal(exchange, model.ExchangeInfo);
        }

        [Fact]
        public async Task CreatePost_InvalidModel_ReturnViewResult()
        {
            var exchange = _exchangeList[0];
            var exchangeId = exchange.Id;
            var entryModel = new ApiInfo { Key = "key", Secret = "pass" };
            var mockService = new Mock<IApiService>();
            mockService.Setup(m => m.GetExchangeById(exchangeId)).ReturnsAsync(exchange);
            var controller = new ApiManagementController(mockService.Object);
            controller.ModelState.AddModelError("some error", "some error nessage");
            
            var result = await controller.Create(exchangeId, entryModel);

            var viewModel = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ApiInfo>(viewModel.Model);
            Assert.Equal("key", model.Key);
            Assert.Equal("pass", model.Secret);
            Assert.NotNull(model.ExchangeInfo);
            Assert.Equal(exchange, model.ExchangeInfo);
            mockService.Verify(m => m.CreateApi(It.IsAny<ApiInfo>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task CreatePost_CreationThrowsException_ReturnViewResult()
        {
            var userId = "user1";
            var exchange = _exchangeList[0];
            var exchangeId = exchange.Id;
            var entryModel = new ApiInfo { Key = "key", Secret = "pass" };
            var mockService = new Mock<IApiService>();
            mockService.Setup(m => m.GetExchangeById(exchangeId)).ReturnsAsync(exchange);
            mockService.Setup(m => m.CreateApi(It.IsAny<ApiInfo>(), userId))
                .ThrowsAsync(new Exception());
            var controller = new ApiManagementController(mockService.Object).WithIdentity(userId, userId);
            
            var result = await controller.Create(exchangeId, entryModel);

            var viewModel = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ApiInfo>(viewModel.Model);
            Assert.Equal("key", model.Key);
            Assert.Equal("pass", model.Secret);
            Assert.NotNull(model.ExchangeInfo);
            Assert.Equal(exchange, model.ExchangeInfo);
            mockService.Verify(m => m.CreateApi(It.IsAny<ApiInfo>(), userId), Times.Once);
        }


        [Fact]
        public async Task Create_Succeed_ReturnRedirectToActionResult()
        {
            var userId = "user1";
            var exchange = _exchangeList[0];
            var exchangeId = exchange.Id;
            var entryModel = new ApiInfo { Key = "key", Secret = "pass" };
            var mockService = new Mock<IApiService>();
            mockService.Setup(s => s.GetExchangeById(exchangeId)).ReturnsAsync(exchange);
            var controller = new ApiManagementController(mockService.Object).WithIdentity(userId, userId);

            var result = await controller.Create(exchangeId, entryModel);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.NotNull(controller.StatusMessage);
            mockService.Verify(m => m.CreateApi(
                    It.Is<ApiInfo>(
                        c => c.ExchangeInfo != null
                        && c.ExchangeInfo.Id == exchangeId
                        && c.Key == "key"
                        && c.Secret == "pass"
                        && string.IsNullOrEmpty(c.Id)),
                    userId),
                Times.Once);
        }
        #endregion


        #region Edit
        [Fact]
        public async Task EditGet_ApiNotExists_ReturnNotFoundResult()
        {
            int exchangeId = 1;
            var mockService = new Mock<IApiService>();
            mockService.Setup(m => m.GetApiForExchange(exchangeId, It.IsAny<string>())).ReturnsAsync(null as ApiInfo);
            var controller = new ApiManagementController(mockService.Object).WithAnonymousIdentity();

            var result = await controller.Edit(exchangeId);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task EditGet_ApiExists_ReturnViewResult()
        {
            string userId = "user1";
            var api = _apiList[0];
            int exchangeId = api.ExchangeInfo.Id;
            var mockService = new Mock<IApiService>();
            mockService.Setup(m => m.GetApiForExchange(exchangeId, userId)).ReturnsAsync(api);
            var controller = new ApiManagementController(mockService.Object).WithIdentity(userId, userId);

            var result = await controller.Edit(exchangeId);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<ApiInfo>(viewResult.Model);
            Assert.NotNull(model);
            Assert.NotNull(model.ExchangeInfo);
            Assert.Equal(api, model);   
        }

        [Fact]
        public async Task EditPost_InvalidModel_ReturnViewResult()
        {
            string userId = "user1";
            var api = _apiList[0];
            int exchangeId = api.ExchangeInfo.Id;
            var mockService = new Mock<IApiService>();
            var entryModel = new ApiInfo { Key = "key", Secret = "pass" };
            mockService.Setup(m => m.GetApiForExchange(exchangeId, userId)).ReturnsAsync(api);
            var controller = new ApiManagementController(mockService.Object).WithIdentity(userId, userId);
            controller.ModelState.AddModelError("some error", "some error nessage");
            
            var result = await controller.Edit(exchangeId, entryModel);

            var viewModel = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ApiInfo>(viewModel.Model);
            Assert.Equal("key", model.Key);
            Assert.Equal("pass", model.Secret);
            Assert.NotNull(model.ExchangeInfo);
            Assert.Equal(exchangeId, model.ExchangeInfo.Id);
            mockService.Verify(m => m.UpdateApi(It.IsAny<ApiInfo>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task EditPost_NotExists_ReturnNotFoundResult()
        {
            string userId = "user1";
            int exchangeId = 1;
            var entryModel = new ApiInfo { Key = "key", Secret = "pass" };
            var mockService = new Mock<IApiService>();
            mockService.Setup(m => m.GetApiForExchange(It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync(null as ApiInfo);
            var controller = new ApiManagementController(mockService.Object).WithIdentity(userId, userId);

            var result = await controller.Edit(exchangeId, entryModel);

            var objResult = Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task EditPost_UpdateThrowsException_ReturnViewResult()
        {
            string userId = "user1";
            var api = _apiList[0];
            int exchangeId = api.ExchangeInfo.Id;
            var mockService = new Mock<IApiService>();
            mockService.Setup(m => m.GetApiForExchange(exchangeId, userId)).ReturnsAsync(api);
            mockService.Setup(m => m.UpdateApi(It.IsAny<ApiInfo>(), userId))
                .ThrowsAsync(new Exception());
            var entryModel = new ApiInfo { Key = "key", Secret = "pass" };
            var controller = new ApiManagementController(mockService.Object).WithIdentity(userId, userId);
            
            var result = await controller.Edit(exchangeId, entryModel);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ApiInfo>(viewResult.Model);
            Assert.Equal("key", model.Key);
            Assert.Equal("pass", model.Secret);
            Assert.NotNull(model.ExchangeInfo);
            Assert.Equal(api.ExchangeInfo.Id, model.ExchangeInfo.Id);
            mockService.Verify(m => m.UpdateApi(It.IsAny<ApiInfo>(), userId), Times.Once);
        }

        [Fact]
        public async Task EditPost_Succeed_ReturnRedirectToActionResult()
        {
            string userId = "user1";
            var api = _apiList[0];
            int exchangeId = api.ExchangeInfo.Id;
            var mockService = new Mock<IApiService>();
            mockService.Setup(m => m.GetApiForExchange(exchangeId, userId)).ReturnsAsync(api);
            var entryModel = new ApiInfo { Key = "key", Secret = "pass" };
            var controller = new ApiManagementController(mockService.Object).WithIdentity(userId, userId);

            var result = await controller.Edit(exchangeId, entryModel);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.NotNull(controller.StatusMessage);
            mockService.Verify(m => m.UpdateApi(
                    It.Is<ApiInfo>(c =>
                        !string.IsNullOrEmpty(c.Id)
                        && c.Key == "key"
                        && c.Secret == "pass"
                        && c.ExchangeInfo != null
                        && c.ExchangeInfo.Id == exchangeId),
                    userId),
                Times.Once);
        }
        #endregion


        #region Delete
        [Fact]
        public async Task DeleteGet_ApiNotExists_ReturnNotFoundResult()
        {
            int exchangeId = 1;
            var mockService = new Mock<IApiService>();
            mockService.Setup(m => m.GetApiForExchange(exchangeId, It.IsAny<string>())).ReturnsAsync(null as ApiInfo);
            var controller = new ApiManagementController(mockService.Object).WithAnonymousIdentity();

            var result = await controller.Delete(exchangeId);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteGet_ApiExists_ReturnViewResult()
        {
            string userId = "user1";
            var api = _apiList[0];
            int exchangeId = api.ExchangeInfo.Id;
            var mockService = new Mock<IApiService>();
            mockService.Setup(m => m.GetApiForExchange(exchangeId, userId)).ReturnsAsync(api);
            var controller = new ApiManagementController(mockService.Object).WithIdentity(userId, userId);

            var result = await controller.Delete(exchangeId);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<ApiInfo>(viewResult.Model);
            Assert.NotNull(model);
            Assert.NotNull(model.ExchangeInfo);
            Assert.Equal(api, model);
        }


        [Fact]
        public async Task DeletePost_DeletingThrowsException_ReturnViewResult()
        {
            string userId = "user1";
            var api = _apiList[0];
            int exchangeId = api.ExchangeInfo.Id;
            var mockService = new Mock<IApiService>();
            mockService.Setup(m => m.GetApiForExchange(exchangeId, userId)).ReturnsAsync(api);
            mockService.Setup(m => m.DeleteApi(api.Id, userId))
                .ThrowsAsync(new Exception());
            var controller = new ApiManagementController(mockService.Object).WithIdentity(userId, userId);

            var result = await controller.DeleteConfirm(exchangeId);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ApiInfo>(viewResult.Model);
            Assert.NotNull(model.ExchangeInfo);
            Assert.Equal(api, model);
            mockService.Verify(m => m.DeleteApi(api.Id, userId), Times.Once);
        }

        [Fact]
        public async Task DeletePost_Succeed_ReturnRedirectToActionResult()
        {
            string userId = "user1";
            var api = _apiList[0];
            int exchangeId = api.ExchangeInfo.Id;
            var mockService = new Mock<IApiService>();
            mockService.Setup(m => m.GetApiForExchange(exchangeId, userId)).ReturnsAsync(api);
            var controller = new ApiManagementController(mockService.Object).WithIdentity(userId, userId);

            var result = await controller.DeleteConfirm(exchangeId);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.NotNull(controller.StatusMessage);
            mockService.Verify(m => m.DeleteApi(api.Id, userId), Times.Once);
        } 
        #endregion



        private List<ApiInfo> CreateExchangeConnections(IList<ExchangeInfo> exchanges)
        {
            return new List<ApiInfo>
            {
                new ApiInfo{
                    Id = "id1",
                    Key = "Key11",
                    Secret = "Secret11",
                    ExchangeInfo = exchanges[0]
                },
                new ApiInfo{
                    Id = "id2",
                    Key = "Key12",
                    Secret = "Secret12",
                    ExchangeInfo = exchanges[1]
                },
            };
        }
        private List<ExchangeInfo> CreateExchanges()
        {
            return new List<ExchangeInfo>
            {
                new ExchangeInfo { Id = 1, Name = "Binance" },
                new ExchangeInfo { Id = 2, Name = "Kraken" },
                new ExchangeInfo { Id = 3, Name = "Coinbase"}
            };
        }
    }
}