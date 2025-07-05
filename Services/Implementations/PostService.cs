using BusinessObjects.Domains;
using RentNest.Core.DTO;
using Repositories.Interfaces;
using Service.Interfaces;

namespace Service.Implements
{
    public class PostService : IPostService
    {
        private readonly IPostRepository _postRepository;
        public PostService(IPostRepository postRepository)
        {
            _postRepository = postRepository;
        }

        public async Task<List<Post>> GetAllPostsWithAccommodation()
        {
            return await _postRepository.GetAllPostsWithAccommodation();
        }

    }
}
