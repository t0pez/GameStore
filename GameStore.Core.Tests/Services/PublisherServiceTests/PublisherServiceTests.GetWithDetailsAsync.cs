using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FluentAssertions;
using GameStore.Core.Interfaces;
using GameStore.Core.Models.Dto;
using GameStore.Core.Services;
using GameStore.Tests.Infrastructure.Attributes;
using Moq;
using Xunit;

namespace GameStore.Core.Tests.Services.PublisherServiceTests;

public partial class PublisherServiceTests
{
    [Theory]
    [AutoMoqData]
    public async Task GetWithDetailsAsync_ReturnsCorrectValues(
        PublisherDto publisher,
        [Frozen] Mock<ISearchService> searchServiceMock,
        PublisherService sut)
    {
        searchServiceMock.Setup(service => service.GetPublisherDtoByCompanyNameOrDefaultAsync(publisher.Name))
                         .ReturnsAsync(publisher);

        var actualResult = await sut.GetByCompanyNameAsync(publisher.Name);

        actualResult.Should().Be(publisher);
    }
}