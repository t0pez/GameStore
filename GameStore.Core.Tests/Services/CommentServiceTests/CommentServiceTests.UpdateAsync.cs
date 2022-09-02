using System.Threading.Tasks;
using AutoFixture.Xunit2;
using GameStore.Core.Models.Server.Comments;
using GameStore.Core.Models.Server.Comments.Specifications;
using GameStore.Core.Models.ServiceModels.Comments;
using GameStore.Core.Services;
using GameStore.SharedKernel.Interfaces.DataAccess;
using GameStore.Tests.Infrastructure.Attributes;
using Moq;
using Xunit;

namespace GameStore.Core.Tests.Services.CommentServiceTests;

public partial class CommentServiceTests
{
    [Theory]
    [AutoMoqData]
    public async Task UpdateAsync_CorrectValue(
        CommentUpdateModel updateModel,
        Comment comment,
        [Frozen] Mock<IUnitOfWork> unitOfWorkMock,
        CommentService sut)
    {
        unitOfWorkMock.Setup(repository =>
                                 repository.GetEfRepository<Comment>()
                                           .GetSingleOrDefaultBySpecAsync(It.IsAny<CommentsSpec>()))
                      .ReturnsAsync(comment);

        await sut.UpdateAsync(updateModel);

        unitOfWorkMock.Verify(repository => repository.GetEfRepository<Comment>().UpdateAsync(It.IsAny<Comment>()));
        unitOfWorkMock.Verify(unitOfWork => unitOfWork.SaveChangesAsync());
    }
}