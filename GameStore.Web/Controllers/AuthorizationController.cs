using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using GameStore.Core.Interfaces;
using GameStore.Core.Models.Constants;
using GameStore.Core.Models.Server.Users;
using GameStore.Core.Models.ServiceModels.Users;
using GameStore.Web.Infrastructure.Authorization;
using GameStore.Web.Interfaces;
using GameStore.Web.Models.User;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.Web.Controllers;

[Route("auth")]
public class AuthorizationController : Controller
{
    private static readonly DateTimeOffset AuthExpirationDateTimeOffset = DateTimeOffset.UtcNow.AddYears(2);

    private readonly IMapper _mapper;
    private readonly IOrderMergingService _orderMergingService;
    private readonly IOrderService _orderService;
    private readonly IUserCookieService _userCookieService;
    private readonly IUserService _userService;

    public AuthorizationController(IUserService userService, IOrderService orderService,
                                   IOrderMergingService orderMergingService, IUserCookieService userCookieService,
                                   IMapper mapper)
    {
        _userService = userService;
        _orderService = orderService;
        _orderMergingService = orderMergingService;
        _userCookieService = userCookieService;
        _mapper = mapper;
    }

    [HttpGet("register")]
    public async Task<ActionResult> RegisterAsync()
    {
        return View(new UserCreateRequestModel());
    }

    [HttpPost("register")]
    public async Task<ActionResult> RegisterAsync(UserCreateRequestModel request)
    {
        var isUserEmailExistsAsync = await _userService.IsUserEmailExistsAsync(request.Email);

        if (isUserEmailExistsAsync)
        {
            ModelState.AddModelError(nameof(request.Email), "This email already exists");
        }

        var isUserNameExistsAsync = await _userService.IsUserNameExistsAsync(request.UserName);

        if (isUserNameExistsAsync)
        {
            ModelState.AddModelError(nameof(request.UserName), "This username already exists");
        }

        if (ModelState.IsValid == false)
        {
            return View(request);
        }

        var customerId = _userCookieService.GetCookiesUserId();

        var createModel = _mapper.Map<UserCreateModel>(request);
        createModel.CookieUserId = customerId;

        var user = await _userService.CreateAsync(createModel);

        return RedirectToAction("Login", "Authorization", new { email = user.Email });
    }

    [HttpGet("login")]
    public async Task<ActionResult> LoginAsync(string email)
    {
        return View(new LoginRequestModel { Email = email });
    }

    [HttpPost("login")]
    public async Task<ActionResult> LoginAsync(LoginRequestModel request)
    {
        var loginModel = _mapper.Map<LoginModel>(request);

        var user = await _userService.GetByLoginModelOrDefaultAsync(loginModel);

        if (user is null)
        {
            ModelState.AddModelError(nameof(request.Email), "Wrong login or password");

            return View(request);
        }

        await MergeOrdersAsync(user.Id);

        var claims = GetClaimsForUser(user);

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        var authProperties = new AuthenticationProperties
        {
            AllowRefresh = true,
            IsPersistent = true,
            ExpiresUtc = AuthExpirationDateTimeOffset
        };

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                                      new ClaimsPrincipal(claimsIdentity), authProperties);

        _userCookieService.AppendUserId(user.Id);

        return RedirectToAction("GetAll", "Games");
    }

    [Authorize]
    [HttpGet("logout")]
    public async Task<ActionResult> LogoutAsync()
    {
        await HttpContext.SignOutAsync();

        _userCookieService.RemoveUserId();

        return RedirectToAction("GetAll", "Games");
    }

    private async Task MergeOrdersAsync(Guid userId)
    {
        var guestId = _userCookieService.GetCookiesUserId();

        var isGuestHasBasket = await _orderService.IsCustomerHasBasketAsync(guestId);

        if (isGuestHasBasket == false)
        {
            return;
        }

        var isUserHasActiveOrder = await _orderService.IsCustomerHasActiveOrderAsync(userId);

        if (isUserHasActiveOrder)
        {
            return;
        }

        await _orderMergingService.MergeOrdersAsync(guestId, userId);
    }

    private IEnumerable<Claim> GetClaimsForUser(User user)
    {
        var claims = new List<Claim>
        {
            new(Claims.Id, user.Id.ToString()),
            new(Claims.UserName, user.UserName),
            new(Claims.Role, user.Role)
        };

        if (user.Role == Roles.Publisher && string.IsNullOrEmpty(user.PublisherName) == false)
        {
            claims.Add(new Claim(Claims.PublisherName, user.PublisherName));
        }

        return claims;
    }
}