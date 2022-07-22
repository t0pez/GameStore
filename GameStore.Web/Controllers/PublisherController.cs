using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using GameStore.Core.Interfaces;
using GameStore.Core.Models.ServiceModels.Publishers;
using GameStore.Web.Models.Publisher;
using GameStore.Web.ViewModels.Publisher;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.Web.Controllers;

[Route("publisher")]
public class PublisherController : Controller
{
    private readonly IMapper _mapper;
    private readonly IPublisherService _publisherService;

    public PublisherController(IPublisherService publisherService, IMapper mapper)
    {
        _publisherService = publisherService;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PublisherListViewModel>>> GetAllAsync()
    {
        var publishers = await _publisherService.GetAllAsync();
        var result = _mapper.Map<IEnumerable<PublisherListViewModel>>(publishers);

        return View(result);
    }

    [HttpGet("{companyName}")]
    public async Task<ActionResult<PublisherViewModel>> GetWithDetailsAsync([FromRoute] string companyName)
    {
        var publisher = await _publisherService.GetByCompanyNameAsync(companyName);
        var result = _mapper.Map<PublisherViewModel>(publisher);

        return View(result);
    }

    [HttpGet("new")]
    public async Task<ActionResult> CreateAsync()
    {
        return View(new PublisherCreateRequestModel());
    }

    [HttpPost("new")]
    public async Task<ActionResult> CreateAsync(PublisherCreateRequestModel request)
    {
        if (await _publisherService.IsCompanyNameAlreadyExists(request.Name))
        {
            ModelState.AddModelError(nameof(request.Name), "Company name already exists");
        }

        if (ModelState.IsValid == false)
        {
            return View(request);
        }

        var createModel = _mapper.Map<PublisherCreateModel>(request);

        var publisher = await _publisherService.CreateAsync(createModel);

        return RedirectToAction("GetWithDetails", "Publisher", new { companyName = publisher.Name });
    }

    [HttpGet("{companyName}/update")]
    public async Task<ActionResult<PublisherUpdateRequestModel>> UpdateAsync([FromRoute] string companyName)
    {
        var publisherToUpdate = await _publisherService.GetByCompanyNameAsync(companyName);
        var mapped = _mapper.Map<PublisherUpdateRequestModel>(publisherToUpdate);

        return View(mapped);
    }

    [HttpPost("{companyName}/update")]
    public async Task<ActionResult> UpdateAsync(PublisherUpdateRequestModel request, string companyName)
    {
        if (await _publisherService.IsCompanyNameAlreadyExists(request.Name))
        {
            ModelState.AddModelError(nameof(request.Name), "Company name already exists");
        }

        if (ModelState.IsValid == false)
        {
            return View(request);
        }

        var updateModel = _mapper.Map<PublisherUpdateModel>(request);
        updateModel.OldName = companyName;

        await _publisherService.UpdateAsync(updateModel);

        return RedirectToAction("GetWithDetails", "Publisher", new { companyName = request.Name });
    }

    [HttpPost("{companyName}/delete")]
    public async Task<ActionResult> DeleteAsync(string companyName)
    {
        await _publisherService.DeleteAsync(companyName);

        return RedirectToAction("GetAll", "Publisher");
    }
}