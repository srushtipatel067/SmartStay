using SmartStay.API.DTOs.Package;

namespace SmartStay.API.Repositories.Interfaces
{
    public interface IPackageRepository
    {
        Task<(bool Success, string Message)> CreatePackageAsync(CreatePackageDto dto);
        Task<IEnumerable<PackageDto>> GetPackagesByHotelAsync(int hotelId);
    }
}