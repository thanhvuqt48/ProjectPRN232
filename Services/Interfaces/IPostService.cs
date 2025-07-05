using BusinessObjects.Domains;
using RentNest.Core.DTO;

namespace Service.Interfaces
{
    public interface IPostService
    {
        Task<List<Post>> GetAllPostsWithAccommodation();
    }
}
