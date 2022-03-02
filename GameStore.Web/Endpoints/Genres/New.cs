using Ardalis.ApiEndpoints;
using GameStore.Core.Models.Games;
using GameStore.SharedKernel.Interfaces.DataAccess;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GameStore.Web.Endpoints.Genres
{
    public class New : EndpointBaseAsync.WithRequest<CreateGenreRequest>.WithActionResult
    {

        private readonly IUnitOfWork _unitOfWork;

        public New(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpPost("genres/new")]
        public override async Task<ActionResult> HandleAsync(CreateGenreRequest request, CancellationToken cancellationToken = default)
        {
            var result = await _unitOfWork.GetRepository<Genre>().AddAsync(new Genre { Id = Guid.NewGuid(), Name = request.Name, Games = new List<Game>(), IsDeleted = false, SubGenres = new List<Genre>() });

            await _unitOfWork.SaveChangesAsync();

            if (result != null)
                return Ok();

            return BadRequest();
        }
    }
}
