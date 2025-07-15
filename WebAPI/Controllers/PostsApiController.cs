using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessObjects.Dtos;
using BusinessObjects.Dtos.ChatBot;
using DataAccessObjects.UntilHelpers;
using Microsoft.AspNetCore.Mvc;
using RentNest.Core.DTO;
using RentNest.Core.Enums;
using Service.Interfaces;
using Services.Interfaces;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostsApiController : ControllerBase
    {
        private readonly IPostService _postService;
        private readonly IPackagePricingService _packagePricingService;

        private readonly IAzureOpenAIService _azureOpenAIService;
        private readonly IAccountService _accountService;
        public PostsApiController(IPostService postService,
                                IPackagePricingService packagePricingService,
                                IAzureOpenAIService azureOpenAIService,
                                IAccountService accountService)
        {
            _postService = postService;
            _packagePricingService = packagePricingService;
            _azureOpenAIService = azureOpenAIService;
            _accountService = accountService;
        }
        [HttpGet("package-types/{timeUnitId}")]
        public async Task<IActionResult> GetPackageTypesByTimeUnit(int timeUnitId)
        {
            var result = await _packagePricingService.GetPackageTypes(timeUnitId);
            return Ok(result);
        }

        [HttpGet("durations")]
        public async Task<IActionResult> GetDurations(int timeUnitId, int packageTypeId)
        {
            var result = await _packagePricingService.GetDurationsAndPrices(timeUnitId, packageTypeId);
            return Ok(result);
        }

        [HttpPost("get-pricing")]
        public async Task<IActionResult> GetPricingId([FromBody] PricingLookupDto dto)
        {
            var pricingId = await _packagePricingService.GetPricingIdAsync(dto.TimeUnitId, dto.PackageTypeId, dto.DurationValue);
            if (pricingId == null)
                return NotFound(new { message = "Không tìm thấy gói tương ứng." });

            return Ok(new { pricingId });
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreatePost([FromBody] LandlordPostDto dto)
        {
            dto.OwnerId = User.GetUserId();
            var postId = await _postService.SavePost(dto);
            return Ok(new { success = true, postId = postId, amount = dto.TotalPrice });
        }

        [HttpPost("generate-ai")]
        public async Task<IActionResult> GeneratePostWithAI([FromBody] PostDataAIDto model)
        {
            var content = await _azureOpenAIService.GenerateDataPost(model);
            return Ok(new { content });
        }

        [HttpGet("manage")]
        public async Task<IActionResult> GetPosts([FromQuery] string? status = null)
        {
            if (string.IsNullOrEmpty(status))
            {
                status = PostStatusHelper.ToDbValue(PostStatus.Pending);
            }

            var userId = User.GetUserId();
            var allPosts = await _postService.GetAllPostsByUserAsync(userId.Value);
            var filteredPosts = allPosts.Where(p => p.CurrentStatus == status).ToList();

            return Ok(filteredPosts); // optionally transform to DTOs
        }

    }
}