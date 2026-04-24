using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartStay.API.DTOs.Common;
using SmartStay.API.DTOs.Offer;
using SmartStay.API.Repositories.Interfaces;

namespace SmartStay.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OfferController : ControllerBase
    {
        private readonly IOfferRepository _offerRepository;

        public OfferController(IOfferRepository offerRepository)
        {
            _offerRepository = offerRepository;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(CreateOfferDto dto)
        {
            var result = await _offerRepository.CreateOfferAsync(dto);

            return result.Success
                ? Ok(ApiResponse<string>.Ok(null, result.Message))
                : BadRequest(ApiResponse<string>.Fail(result.Message));
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetActive()
        {
            var data = await _offerRepository.GetActiveOffersAsync();
            return Ok(ApiResponse<object>.Ok(data));
        }
    }
}