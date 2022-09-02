using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using GameStore.Core.Interfaces;
using GameStore.Core.Models.Constants;
using GameStore.Core.Models.Server.Publishers;
using GameStore.Core.Models.ServiceModels.Users;
using GameStore.Web.Helpers;
using GameStore.Web.Infrastructure.Authorization;
using GameStore.Web.Interfaces;
using GameStore.Web.Models.User;
using GameStore.Web.ViewModels.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GameStore.Web.Controllers;

[Route("users")]
public class UsersController : Controller
{
    private readonly IMapper _mapper;
    private readonly IPublisherService _publisherService;
    private readonly IUserAuthHelper _userAuthHelper;
    private readonly IUserService _userService;

    public UsersController(IUserService userService, IPublisherService publisherService, IMapper mapper,
                           IUserAuthHelper userAuthHelper)
    {
        _userService = userService;
        _mapper = mapper;
        _userAuthHelper = userAuthHelper;
        _publisherService = publisherService;
    }

    [Authorize(Roles = ApiRoles.Administrator)]
    [HttpGet("")]
    public async Task<ActionResult<IEnumerable<UserListViewModel>>> GetAllAsync()
    {
        var users = await _userService.GetAllAsync();

        var viewModels = _mapper.Map<IEnumerable<UserListViewModel>>(users);

        return View(viewModels);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserViewModel>> GetWithDetailsAsync(Guid id)
    {
        var canView = await _userAuthHelper.CanViewAndEditAsync(id);

        if (canView == false)
        {
            throw new UnauthorizedAccessException("Not enough permissions for this page");
        }

        var user = await _userService.GetByIdAsync(id);

        var viewModel = _mapper.Map<UserViewModel>(user);

        var isCanDelete = await _userAuthHelper.CanDeleteAsync(id);

        ViewData[ViewKeys.Users.IsCanDelete] = isCanDelete;

        return View(viewModel);
    }

    [Authorize(Roles = ApiRoles.User)]
    [HttpGet("{id}/update")]
    public async Task<ActionResult> UpdateAsync(Guid id)
    {
        var canUpdate = await _userAuthHelper.CanViewAndEditAsync(id);

        if (canUpdate == false)
        {
            throw new UnauthorizedAccessException("Not enough permissions for this page");
        }

        var user = await _userService.GetByIdAsync(id);

        var requestModel = _mapper.Map<UserUpdateRequestModel>(user);

        await FillPublishersAsync();

        return View(requestModel);
    }

    [Authorize(Roles = ApiRoles.User)]
    [HttpPost("{id}/update")]
    public async Task<ActionResult> UpdateAsync(UserUpdateRequestModel requestModel, Guid id)
    {
        var canUpdate = await _userAuthHelper.CanViewAndEditAsync(id);

        if (canUpdate == false)
        {
            throw new UnauthorizedAccessException("Not enough permissions for this page");
        }

        var isEmailValid = await HasValidEmailAsync(requestModel);

        if (isEmailValid == false)
        {
            ModelState.AddModelError(nameof(requestModel.Email), "Such email already exists");
        }

        var isUserNameValid = await HasValidUserNameAsync(requestModel);

        if (isUserNameValid == false)
        {
            ModelState.AddModelError(nameof(requestModel.UserName), "Such username already exists");
        }

        if (requestModel.Role == Roles.Publisher && string.IsNullOrEmpty(requestModel.PublisherName))
        {
            ModelState.AddModelError(nameof(requestModel.PublisherName), "Publisher name must not be empty");
        }

        if (ModelState.IsValid == false)
        {
            await FillPublishersAsync();

            return View(requestModel);
        }

        var updateModel = _mapper.Map<UserUpdateModel>(requestModel);

        await _userService.UpdateAsync(updateModel);

        if (await _userAuthHelper.IsSameUserAsync(id))
        {
            return RedirectToAction("Logout", "Authorization");
        }

        return RedirectToAction("GetWithDetails", "Users", new { id = updateModel.Id });
    }

    [Authorize(Roles = ApiRoles.Administrator)]
    [HttpPost("{id}/delete")]
    public async Task<ActionResult> DeleteAsync(Guid id)
    {
        var canDelete = await _userAuthHelper.CanDeleteAsync(id);

        if (canDelete == false)
        {
            throw new UnauthorizedAccessException("Not enough permissions for this page");
        }

        await _userService.DeleteAsync(id);

        return RedirectToAction("GetAll", "Users");
    }

    private async Task<bool> HasValidEmailAsync(UserUpdateRequestModel requestModel)
    {
        var isUserEmailExists = await _userService.IsUserEmailExistsAsync(requestModel.Email);

        var isEmailNotChanged = requestModel.Email == requestModel.OldEmail;

        return isEmailNotChanged || isUserEmailExists == false;
    }

    private async Task<bool> HasValidUserNameAsync(UserUpdateRequestModel requestModel)
    {
        var isUserNameExists = await _userService.IsUserNameExistsAsync(requestModel.UserName);

        var isUserNameNotChanged = requestModel.UserName == requestModel.OldUserName;

        return isUserNameNotChanged || isUserNameExists == false;
    }

    private async Task FillPublishersAsync()
    {
        var publishers = await _publisherService.GetAllAsync();

        ViewData[ViewKeys.Users.Publishers] =
            new SelectList(publishers, nameof(Publisher.Name), nameof(Publisher.Name));
    }
}