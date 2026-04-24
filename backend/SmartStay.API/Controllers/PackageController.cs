using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartStay.API.DTOs.Common;
using SmartStay.API.DTOs.Package;
using SmartStay.API.Repositories.Interfaces;

namespace SmartStay.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PackageController : ControllerBase
    {
        private readonly IPackageRepository _packageRepository;

        public PackageController(IPackageRepository packageRepository)
        {
            _packageRepository = packageRepository;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(CreatePackageDto dto)
        {
            var result = await _packageRepository.CreatePackageAsync(dto);

            return result.Success
                ? Ok(ApiResponse<string>.Ok(null, result.Message))
                : BadRequest(ApiResponse<string>.Fail(result.Message));
        }

        [HttpGet("hotel/{hotelId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetByHotel(int hotelId)
        {
            var data = await _packageRepository.GetPackagesByHotelAsync(hotelId);
            return Ok(ApiResponse<object>.Ok(data));
        }
    }
}