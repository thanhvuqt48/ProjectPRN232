﻿using BusinessObjects.Domains;
using RentNest.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interfaces
{
    public interface IPostRepository
    {
        Task<List<Post>> GetAllPostsWithAccommodation();
        Task<Post?> GetPostDetailWithAccommodationDetailAsync(int postId);
        Task<List<Post>> GetAllPostsByUserAsync(int accountId);
        Task<List<Post>> GetTopVipPostsAsync();
        Task<int> SavePost(LandlordPostDto dto);
    }
}
