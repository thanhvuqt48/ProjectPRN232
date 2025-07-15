using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessObjects.Configs
{
    public class AzureOpenAISettings
    {
        public required string Endpoint { get; set; }
        public required string DeploymentName { get; set; }
        public required string ApiKey { get; set; }
    }
}