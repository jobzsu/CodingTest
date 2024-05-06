using CodingTestApp.Controllers;
using Common.Models;
using Common.Models.WebAPI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using RainfallReadingService.Abstractions;

namespace RainfallReadingService.Tests;

[TestFixture]
public class RainfallControllerTests
{
    [TestCase("123")]
    [TestCase("321")]
    [TestCase("00000")]
    [TestCase("11111")]
    public async Task GetReadingByStationId_StationIdIsValid_Returns200RainfallReadingResponse(string stationId)
    {
        // Arrange
        var mockedRainfallReadingFactory = new Mock<IRainfallReadingService>();
        var mockedLogger = new Mock<ILogger<RainfallController>>();
        mockedRainfallReadingFactory.Setup(x => x.GetReadingByStationId(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Result<RainfallReadingResponse>().Success(new RainfallReadingResponse() { Readings = new() }));

        var readingController = new RainfallController(mockedRainfallReadingFactory.Object, mockedLogger.Object);

        // Act
        var response = await readingController.GetReadingsByStationId(stationId, 10);

        // Assert
        Assert.That(response, Is.TypeOf<OkObjectResult>());
        Assert.That(((OkObjectResult)response).Value, Is.Not.Null);
        Assert.That(((OkObjectResult)response).Value, Is.TypeOf(typeof(RainfallReadingResponse)));
        mockedRainfallReadingFactory.Verify(x => x.GetReadingByStationId(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [TestCase(" ")]
    [TestCase("")]
    public async Task GetReadingByStationId_NoStationId_ReturnsBadRequestErrorResult(string stationId)
    {
        // Arrange
        var expectedErrorDetail = new ErrorDetail
        {
            PropertyName = "stationId",
            Message = "StationId is required"
        };
        var expectedError = new ErrorResponse() { Details = new() { expectedErrorDetail }, Message = "Bad Request" };

        var mockedRainfallReadingFactory = new Mock<IRainfallReadingService>();
        var mockedLogger = new Mock<ILogger<RainfallController>>();
        mockedRainfallReadingFactory.Setup(x => x.GetReadingByStationId(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Result<RainfallReadingResponse>().Error(expectedError, 400))
            .Verifiable();

        var readingController = new RainfallController(mockedRainfallReadingFactory.Object, mockedLogger.Object);

        // Act
        var response = await readingController.GetReadingsByStationId(stationId, 10);

        // Assert
        Assert.That(response, Is.TypeOf<BadRequestObjectResult>());
        Assert.That(((BadRequestObjectResult)response).Value, Is.Not.Null);
        Assert.That(((BadRequestObjectResult)response).Value, Is.TypeOf(typeof(ErrorResponse)));
        Assert.That(((ErrorResponse)((BadRequestObjectResult)response).Value).Details.Count(d => d.Message == expectedErrorDetail.Message), Is.EqualTo(1));
        mockedRainfallReadingFactory.Verify(x => x.GetReadingByStationId(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [TestCase("AnotherOne")]
    [TestCase("AndAnotherOne")]
    public async Task GetReadingByStationId_StationIdInvalidFormat_ReturnsBadRequestErrorResult(string stationId)
    {
        // Arrange
        var expectedErrorDetail = new ErrorDetail
        {
            PropertyName = "stationId",
            Message = "Invalid StationId format"
        };
        var expectedError = new ErrorResponse() { Details = new() { expectedErrorDetail }, Message = "Bad Request" };

        var mockedRainfallReadingFactory = new Mock<IRainfallReadingService>();
        var mockedLogger = new Mock<ILogger<RainfallController>>();
        mockedRainfallReadingFactory.Setup(x => x.GetReadingByStationId(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Result<RainfallReadingResponse>().Error(expectedError, 400))
            .Verifiable();

        var readingController = new RainfallController(mockedRainfallReadingFactory.Object, mockedLogger.Object);

        // Act
        var response = await readingController.GetReadingsByStationId(stationId, 10);

        // Assert
        Assert.That(response, Is.TypeOf<BadRequestObjectResult>());
        Assert.That(((BadRequestObjectResult)response).Value, Is.Not.Null);
        Assert.That(((BadRequestObjectResult)response).Value, Is.TypeOf(typeof(ErrorResponse)));
        Assert.That(((ErrorResponse)((BadRequestObjectResult)response).Value).Details.Count(d => d.Message == expectedErrorDetail.Message), Is.EqualTo(1));
        mockedRainfallReadingFactory.Verify(x => x.GetReadingByStationId(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task GetReadingByStationId_StationIdNoReadingsFound_ReturnsNotFoundErrorResult()
    {
        // Arrange
        var expectedError = new ErrorResponse() { Details = new(), Message = "No readings found" };

        var mockedRainfallReadingFactory = new Mock<IRainfallReadingService>();
        var mockedLogger = new Mock<ILogger<RainfallController>>();
        mockedRainfallReadingFactory.Setup(x => x.GetReadingByStationId(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Result<RainfallReadingResponse>().Error(expectedError, 404))
            .Verifiable();

        var readingController = new RainfallController(mockedRainfallReadingFactory.Object, mockedLogger.Object);

        // Act
        var response = await readingController.GetReadingsByStationId("321", 10);

        // Assert
        Assert.That(response, Is.TypeOf<NotFoundObjectResult>());
        Assert.That(((NotFoundObjectResult)response).Value, Is.Not.Null);
        Assert.That(((NotFoundObjectResult)response).Value, Is.TypeOf(typeof(ErrorResponse)));
        Assert.That(((ErrorResponse)((NotFoundObjectResult)response).Value).Message, Is.EqualTo(expectedError.Message));
        mockedRainfallReadingFactory.Verify(x => x.GetReadingByStationId(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
