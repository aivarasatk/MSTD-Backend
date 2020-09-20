using Microsoft.AspNetCore.Mvc;
using Moq;
using MSTD_Backend.Controllers;
using MSTD_Backend.Data;
using MSTD_Backend.Enums;
using MSTD_Backend.Interfaces;
using MSTD_Backend.Models.Response;
using MSTD_Backend.Services;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace MSTD_Baackend.Tests
{
    public class MstdControllerTests
    {
        //Arrange
        //Act
        //Assert
        [Fact]
        public async Task WithCacheItem_GETSources_ReturnsOK()
        {
            //Arrange
            var controller = ArrangeController();

            //Act
            var resp = await controller.GetSourcesAsync();

            //Assert
            Assert.True(resp is OkObjectResult);
        }


        [Fact]
        public async Task WithValidParams_GETTorrents_ReturnsOK()
        {
            //Arrange
            var mockTpb = new Mock<IThePirateBaySource>();
            mockTpb.Setup(moq => moq.GetTorrentsAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<Sorting>()))
                .ReturnsAsync(new TorrentQueryResult());
            mockTpb.Setup(moq => moq.GetSources()).Returns(new[] { "https://tpb.party/" });
            var controller = ArrangeController(mockTpb: mockTpb);

            //Act
            var resp = await controller.GetTorrentsAsync(new[] { "https://tpb.party/" }, Sorting.LeechersAsc, 
                TorrentCategory.All, "", 1);

            //Assert
            Assert.True(resp is OkObjectResult);
        }

        [Fact]
        public async Task WithInvalidUrl_GETTorrents_ReturnsBadRequest()
        {
            //Arrange
            var mockTpb = new Mock<IThePirateBaySource>();
            mockTpb.Setup(moq => moq.GetTorrentsAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<Sorting>()))
                .ReturnsAsync(new TorrentQueryResult());
            mockTpb.Setup(moq => moq.GetSources()).Returns(new[] { "https://tpb.party/" });
            var controller = ArrangeController(mockTpb: mockTpb);

            //Act
            var resp = await controller.GetTorrentsAsync(new[] { "https://thepiratebay.org/" }, Sorting.LeechersAsc,
                TorrentCategory.All, "", 1);

            //Assert
            Assert.True(resp is BadRequestObjectResult);
        }

        [Fact]
        public async Task WithAtLeastOneValidUrl_GETTorrents_ReturnsOkWithWarning()
        {
            //Arrange
            var mockTpb = new Mock<IThePirateBaySource>();
            mockTpb.Setup(moq => moq.GetTorrentsAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<Sorting>()))
                .ReturnsAsync(new TorrentQueryResult());
            mockTpb.Setup(moq => moq.GetSources()).Returns(new[] { "https://tpb.party/", "https://tpbonline.org/" });
            var controller = ArrangeController(mockTpb: mockTpb);

            //Act
            var resp = await controller.GetTorrentsAsync(new[] { "https://tpb.party/", "https://thepiratebay.org/" }, Sorting.LeechersAsc,
                TorrentCategory.All, "", 1);
            var respValue = ((OkObjectResult)resp).Value as TorrentSearchResult;

            //Assert
            Assert.True(resp is OkObjectResult);
            Assert.NotEmpty(respValue.Warnings);
        }

        [Fact]
        public async Task WithValidArguments_GETMagnet_ReturnsOk()
        {
            //Arrange
            var mockLeet = new Mock<ILeetxSource>();
            mockLeet.Setup(moq => moq.GetTorrentMagnetAsync(It.IsAny<string>()))
                .ReturnsAsync("");
            mockLeet.Setup(moq => moq.GetSources()).Returns(new[] { "https://leet.com/"});
            var controller = ArrangeController(mockLeet: mockLeet);

            //Act
            var resp = await controller.GetMagnetAsync("https://leet.com/", "path", TorrentSource.Leetx);

            //Assert
            Assert.True(resp is OkObjectResult);
        }

        private MstdController ArrangeController(Mock<IThePirateBaySource> mockTpb = null,
            Mock<ILeetxSource> mockLeet = null, Mock<IKickassSource> mockKickass = null)
        {
            mockTpb ??= new Mock<IThePirateBaySource>();
            mockLeet ??= new Mock<ILeetxSource>();
            mockKickass ??= new Mock<IKickassSource>();

            var sourceHelper = new SourcesHelper(mockTpb.Object, mockLeet.Object, mockKickass.Object);

            var mockCache = new Mock<IStateCache>();
            mockCache.Setup(moq => moq.SourceStatesAsync())
                .ReturnsAsync(new Dictionary<TorrentSource, IEnumerable<SourceState>> 
                {
                    {TorrentSource.ThePirateBay, Enumerable.Empty<SourceState>()}
                });

            var mockLogger = new Mock<ILogger>();

            return new MstdController(sourceHelper, mockCache.Object, mockLogger.Object);
        }
    }
}
