using CryptoView.UnitTests.Helper;
using CryptoView.Web.Controllers;
using CryptoView.Web.Interfaces;
using CryptoView.Web.Models.Connections;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace CryptoView.UnitTests.Web.Controllers
{
    public class ConnectionsControllerTests
    {
        private List<Connection> _connections;
        private List<SelectListItem> _exchanges;

        public ConnectionsControllerTests()
        {
            _connections = CreateExchangeConnections();
            _exchanges = CreateExchanges();
        }

        [Fact]
        public async Task Index_ReturnsViewResult_WithListOfConnections()
        {
            var userId = "user1";
            var mockService = new Mock<IConnectionsService>();
            mockService
                .Setup(s => s.GetConnectionsForUser(It.IsAny<string>()))
                .ReturnsAsync(_connections)
                .Verifiable();
            var controller = new ConnectionsController(mockService.Object)
                .WithIdentity(userId, "user1");

            var result = await controller.Index();

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Connection>>(viewResult.Model);
            Assert.Equal(2, model.Count());
            mockService.Verify(m => m.GetConnectionsForUser(It.Is<string>(arg => arg == userId)));
        }

        [Fact]
        public async Task Index_ReturnsViewResult_WithEmptyList()
        {
            var mockService = new Mock<IConnectionsService>();
            mockService.Setup(s => s.GetConnectionsForUser(It.IsAny<string>()))
                .ReturnsAsync(null as IEnumerable<Connection>);
            var controller = new ConnectionsController(mockService.Object)
                .WithAnonymousIdentity();

            var result = await controller.Index();

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.Model);
        }


        #region Create
        [Fact]
        public async Task Create_InvalidModel_ReturnViewResult()
        {
            var mockService = new Mock<IConnectionsService>();
            mockService.Setup(m => m.GetExchanges()).ReturnsAsync(_exchanges);
            var controller = new ConnectionsController(mockService.Object);
            controller.ModelState.AddModelError("some error", "some error nessage");
            var entryModel = new ConnectionViewModel { ApiKey = "key", ApiSecret = "pass", ExchangeId = 1 };

            var result = await controller.Create(entryModel);

            var viewModel = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ConnectionViewModel>(viewModel.Model);
            Assert.Equal(entryModel.ApiKey, model.ApiKey);
            Assert.Equal(entryModel.ExchangeId, model.ExchangeId);
            Assert.NotEmpty(model.Exchanges);
            mockService.Verify(m => m.CreateConnection(It.IsAny<Connection>()), Times.Never);
        }

        [Fact]
        public async Task Create_CreationError_ReturnViewResult()
        {
            var mockService = new Mock<IConnectionsService>();
            mockService.Setup(m => m.CreateConnection(It.IsAny<Connection>())).ReturnsAsync(false);
            mockService.Setup(m => m.GetExchanges()).ReturnsAsync(_exchanges);
            var controller = new ConnectionsController(mockService.Object).WithAnonymousIdentity();
            var entryModel = new ConnectionViewModel { ApiKey = "key", ApiSecret = "pass", ExchangeId = 1 };

            var result = await controller.Create(entryModel);

            var viewModel = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ConnectionViewModel>(viewModel.Model);
            Assert.Equal(entryModel.ApiKey, model.ApiKey);
            Assert.Equal(entryModel.ExchangeId, model.ExchangeId);
            Assert.NotEmpty(model.Exchanges);
            mockService.Verify(m => m.CreateConnection(It.IsAny<Connection>()), Times.Once);
        }

        [Fact]
        public async Task Create_Succeed_ReturnRedirectToActionResult()
        {
            var mockService = new Mock<IConnectionsService>();
            mockService.Setup(m => m.CreateConnection(It.IsAny<Connection>())).ReturnsAsync(true);
            var controller = new ConnectionsController(mockService.Object).WithAnonymousIdentity();
            var entryModel = new ConnectionViewModel { ApiKey = "key", ApiSecret = "pass", ExchangeId = 1 };

            var result = await controller.Create(entryModel);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.NotNull(controller.StatusMessage);
        }
        #endregion


        #region Edit
        [Fact]
        public async Task EditGet_NotExists_ReturnNotFoundResult()
        {
            var mockService = new Mock<IConnectionsService>();
            mockService.Setup(m => m.GetConnectionById(It.IsAny<string>()))
                .ReturnsAsync(null as Connection);
            var controller = new ConnectionsController(mockService.Object)
                .WithAnonymousIdentity();

            var result = await controller.Edit("notExistsId");

            var objResult = Assert.IsType<NotFoundObjectResult>(result);
            var id = Assert.IsAssignableFrom<string>(objResult.Value);
            Assert.Equal("notExistsId", id);
        }

        [Fact]
        public async Task EditGet_RequestedNonOwnConnection_ReturnForbidResult()
        {
            var mockService = new Mock<IConnectionsService>();
            mockService.Setup(m => m.GetConnectionById(It.IsAny<string>()))
                .ReturnsAsync(_connections[0]);
            var controller = new ConnectionsController(mockService.Object)
                .WithIdentity("user2", "user2");

            var result = await controller.Edit("id1");

            Assert.IsType<ForbidResult>(result);
        }

        [Fact]
        public async Task EditGet_RequestedOwnConnection_ReturnViewResult()
        {
            var mockService = new Mock<IConnectionsService>();
            mockService.Setup(m => m.GetConnectionById(It.IsAny<string>()))
                .ReturnsAsync(_connections[0]);
            mockService.Setup(m => m.GetExchanges()).ReturnsAsync(_exchanges);
            var controller = new ConnectionsController(mockService.Object)
                .WithIdentity("user1", "user1");

            var result = await controller.Edit("id1");

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<ConnectionViewModel>(viewResult.Model);
            Assert.NotNull(model);
            Assert.Equal(_connections[0].ApiKey, model.ApiKey);
            Assert.Equal(_connections[0].ApiSecret, model.ApiSecret);
            Assert.Equal(_connections[0].ExchangeId, model.ExchangeId);
            Assert.NotEmpty(model.Exchanges);
        } 

        [Fact]
        public async Task EditPost_InvalidModel_ReturnViewResult()
        {
            var mockService = new Mock<IConnectionsService>();
            mockService.Setup(m => m.GetExchanges()).ReturnsAsync(_exchanges);
            var controller = new ConnectionsController(mockService.Object);
            controller.ModelState.AddModelError("some error", "some error nessage");
            var entryModel = new ConnectionViewModel { ApiKey = "key", ApiSecret = "pass", ExchangeId = 1 };

            var result = await controller.Edit("id1", entryModel);

            var viewModel = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ConnectionViewModel>(viewModel.Model);
            Assert.Equal(entryModel.ApiKey, model.ApiKey);
            Assert.Equal(entryModel.ExchangeId, model.ExchangeId);
            Assert.NotEmpty(model.Exchanges);
            mockService.Verify(m => m.UpdateConnection(It.IsAny<Connection>()), Times.Never);
        }
        [Fact]
        public async Task EditPost_NotExists_ReturnNotFoundObjectResult()
        {
            var mockService = new Mock<IConnectionsService>();
            mockService.Setup(m => m.GetConnectionById(It.IsAny<string>())).ReturnsAsync(null as Connection);
            mockService.Setup(m => m.GetExchanges()).ReturnsAsync(_exchanges);
            var controller = new ConnectionsController(mockService.Object).WithAnonymousIdentity();
            var entryModel = new ConnectionViewModel { ApiKey = "key", ApiSecret = "pass", ExchangeId = 1 };

            var result = await controller.Edit("notExistsId", entryModel);

            var objResult = Assert.IsType<NotFoundObjectResult>(result);
            var id = Assert.IsAssignableFrom<string>(objResult.Value);
            Assert.Equal("notExistsId", id);
            mockService.Verify(m => m.UpdateConnection(It.IsAny<Connection>()), Times.Never);
        }

        [Fact]
        public async Task EditPost_UpdateError_ReturnViewResult()
        {
            var mockService = new Mock<IConnectionsService>();
            mockService.Setup(m => m.UpdateConnection(It.IsAny<Connection>())).ReturnsAsync(false);
            mockService.Setup(m => m.GetConnectionById(It.IsAny<string>())).ReturnsAsync(_connections[0]);
            mockService.Setup(m => m.GetExchanges()).ReturnsAsync(_exchanges);
            var controller = new ConnectionsController(mockService.Object).WithIdentity("user1", "user1");
            var entryModel = new ConnectionViewModel { ApiKey = "key", ApiSecret = "pass", ExchangeId = 1 };

            var result = await controller.Edit("id1", entryModel);

            var viewModel = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ConnectionViewModel>(viewModel.Model);
            Assert.Equal(entryModel.ApiKey, model.ApiKey);
            Assert.Equal(entryModel.ExchangeId, model.ExchangeId);
            Assert.NotEmpty(model.Exchanges);
            mockService.Verify(m => m.UpdateConnection(It.IsAny<Connection>()), Times.Once);
        }

        [Fact]
        public async Task EditPost_IdSubstitution_ReturnForbidenResult()
        {
            var mockService = new Mock<IConnectionsService>();
            mockService.Setup(m => m.GetConnectionById(It.IsAny<string>())).ReturnsAsync(_connections[0]);
            mockService.Setup(m => m.GetExchanges()).ReturnsAsync(_exchanges);
            var controller = new ConnectionsController(mockService.Object).WithIdentity("user2", "user2");
            var entryModel = new ConnectionViewModel { ApiKey = "key", ApiSecret = "pass", ExchangeId = 1 };

            var result = await controller.Edit("notMyId", entryModel);

            Assert.IsType<ForbidResult>(result);
        }

        [Fact]
        public async Task EditPost_Succeed_ReturnRedirectToActionResult()
        {
            var mockService = new Mock<IConnectionsService>();
            mockService.Setup(m => m.UpdateConnection(It.IsAny<Connection>())).ReturnsAsync(true);
            mockService.Setup(m => m.GetConnectionById(It.IsAny<string>())).ReturnsAsync(_connections[0]);
            var controller = new ConnectionsController(mockService.Object).WithIdentity("user1", "user1");
            var entryModel = new ConnectionViewModel { ApiKey = "key", ApiSecret = "pass", ExchangeId = 1 };

            var result = await controller.Edit("id1", entryModel);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.NotNull(controller.StatusMessage);
            mockService.Verify(m => m.UpdateConnection(It.IsAny<Connection>()), Times.Once);
        }
        #endregion





        private List<Connection> CreateExchangeConnections()
        {
            return new List<Connection>
            {
                new Connection{
                    Id = "id1",
                    ExchangeName = "Binance",
                    ApiKey = "Key11",
                    ApiSecret = "Secret11",
                    Created = DateTime.Now.AddDays(-10),
                    UserId = "user1",
                },
                new Connection{
                    Id = "id2",
                    ExchangeName = "Kraken",
                    ApiKey = "Key12",
                    ApiSecret = "Secret12",
                    Created = DateTime.Now.AddDays(-9),
                    UserId = "user1",
                },
            };
        }
        private List<SelectListItem> CreateExchanges()
        {
            return new List<SelectListItem>
            {
                new SelectListItem("Binance", "1"),
                new SelectListItem("Kraken", "2"),
            };
        }
    }
}