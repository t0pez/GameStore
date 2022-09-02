using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FluentAssertions;
using GameStore.Core.Models.Mongo.Suppliers;
using GameStore.Core.Models.Server.Publishers;
using GameStore.Core.Models.Server.Publishers.Specifications;
using GameStore.Core.Services;
using GameStore.SharedKernel.Interfaces.DataAccess;
using GameStore.Tests.Infrastructure.Attributes;
using Moq;
using Xunit;

namespace GameStore.Core.Tests.Services.PublisherServiceTests;

public partial class PublisherServiceTests
{
    [Theory]
    [AutoMoqData]
    public async Task GetAllAsync_ReturnsCorrectValues(
        List<Publisher> publishers,
        List<Supplier> suppliers,
        [Frozen] Mock<IUnitOfWork> unitOfWorkMock,
        PublisherService sut)
    {
        unitOfWorkMock.Setup(repository =>
                                 repository.GetEfRepository<Publisher>().GetBySpecAsync(It.IsAny<PublishersSpec>()))
                      .ReturnsAsync(publishers);

        unitOfWorkMock.Setup(repository => repository.GetMongoRepository<Supplier>().GetBySpecAsync(null))
                      .ReturnsAsync(suppliers);

        var actualResult = await sut.GetAllAsync();

        var expectedCount = publishers.Count + suppliers.Count;
        actualResult.Should().HaveCount(expectedCount);
    }
}