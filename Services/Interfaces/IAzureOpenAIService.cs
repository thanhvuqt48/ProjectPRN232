using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IAzureOpenAIService
    {
        Task<string> GenerateDataPost(PostDataAIDto model);
        Task<string> ChatWithAIAsync(string userMessage);
    }
}