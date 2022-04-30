using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using GameStore.Core.Interfaces;
using GameStore.Core.Models.ServiceModels.PlatformTypes;
using GameStore.Web.Models.PlatformType;
using GameStore.Web.ViewModels.PlatformTypes;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.Web.Controllers;

[Route("platforms")]
public class PlatformTypesController : Controller
{
    private readonly IPlatformTypeService _platformTypeService;
    private readonly IMapper _mapper;

    public PlatformTypesController(IPlatformTypeService platformTypeService, IMapper mapper)
    {
        _platformTypeService = platformTypeService;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PlatformTypeListViewModel>>> GetAllAsync()
    {
        var platforms = await _platformTypeService.GetAllAsync();
        var result = _mapper.Map<IEnumerable<PlatformTypeListViewModel>>(platforms);

        return View(result);
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<PlatformTypeViewModel>> GetWithDetailsAsync([FromRoute] Guid id)
    {
        var platform = await _platformTypeService.GetByIdAsync(id);
        var result = _mapper.Map<PlatformTypeViewModel>(platform);

        return View(result);
    }

    [HttpGet("new")]
    public async Task<ActionResult<PlatformTypeCreateRequestModel>> CreateAsync()
    {
        return View(new PlatformTypeCreateRequestModel());
    }

    [HttpPost("new")]
    public async Task<ActionResult> CreateAsync(PlatformTypeCreateRequestModel model)
    {
        var createModel = _mapper.Map<PlatformTypeCreateModel>(model);
        await _platformTypeService.CreateAsync(createModel);

        return RedirectToAction("GetAll", "PlatformTypes");
    }

    [HttpGet("update/{id}")]
    public async Task<ActionResult<PlatformTypeUpdateRequestModel>> UpdateAsync([FromRoute] Guid id)
    {
        return View(new PlatformTypeUpdateRequestModel { Id = id });
    }

    [HttpPost("update/{id}")]
    public async Task<ActionResult> UpdateAsync(PlatformTypeUpdateRequestModel model)
    {
        var updateModel = _mapper.Map<PlatformTypeUpdateModel>(model);
        await _platformTypeService.UpdateAsync(updateModel);

        return RedirectToAction("GetAll", "PlatformTypes");
    }

    [HttpPost("delete")]
    public async Task<ActionResult> DeleteAsync(Guid id)
    {
        await _platformTypeService.DeleteAsync(id);

        return RedirectToAction("GetAll", "PlatformTypes");
    }
}