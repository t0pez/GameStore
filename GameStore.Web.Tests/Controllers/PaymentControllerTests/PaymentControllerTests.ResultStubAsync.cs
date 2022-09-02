using FluentAssertions;
using GameStore.Core.Models.Payment;
using GameStore.Tests.Infrastructure.Attributes;
using GameStore.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace GameStore.Web.Tests.Controllers.PaymentControllerTests;

public partial class PaymentControllerTests
{
    [Theory]
    [AutoMoqData]
    public async void ResultStubAsync_ReturnsRedirect(
        PaymentController sut)
    {
        var actualResult = await sut.ResultStubAsync(new VisaPaymentGetaway());

        actualResult.Should().BeOfType<RedirectToActionResult>();
    }
}