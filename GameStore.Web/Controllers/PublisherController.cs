using System;
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
    private readonly IPublisherService _publisherService;
    private readonly IMapper _mapper;

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
        if (ModelState.IsValid == false)
        {
            return View("Error");
        }
        
        var createModel = _mapper.Map<PublisherCreateModel>(request);

        var publisher = await _publisherService.CreateAsync(createModel);

        return RedirectToAction("GetWithDetails", "Publisher", new { companyName = publisher.Name });
    }

    [HttpGet("update/{companyName}")]
    public async Task<ActionResult<PublisherUpdateRequestModel>> UpdateAsync([FromRoute] string companyName)
    {
        var publisherToUpdate = await _publisherService.GetByCompanyNameAsync(companyName);
        var mapped = _mapper.Map<PublisherUpdateRequestModel>(publisherToUpdate);

        return View(mapped);
    }
    
    [HttpPost("update/{companyName}")]
    public async Task<ActionResult> UpdateAsync(PublisherUpdateRequestModel request)
    {
        var updateModel = _mapper.Map<PublisherUpdateModel>(request);
        await _publisherService.UpdateAsync(updateModel);

        return RedirectToAction("GetWithDetails", "Publisher", new { companyName = request.Name });
    }
    
    [HttpPost("delete")]
    public async Task<ActionResult> DeleteAsync(Guid id)
    {
        await _publisherService.DeleteAsync(id);

        return RedirectToAction("GetAll", "Publisher");
    }
}