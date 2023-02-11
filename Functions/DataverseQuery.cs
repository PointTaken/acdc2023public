using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using PirateApi.Functions.Services;

namespace PirateApi.Functions.Functions
{
    public class DateverseQuery
    {
        private readonly ILogger<DateverseQuery> _logger;
        private readonly IDataverseService _dataverseService;

        public DateverseQuery(ILogger<DateverseQuery> log, IDataverseService dataverseService)
        {
            _logger = log;
            _dataverseService = dataverseService;
        }

        [FunctionName("DataverseQuery")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "DataverseQuery/{entity}/{*query}")] HttpRequest req, string entity, string query)
        {
            _logger.LogInformation("Dataverse HTTP trigger function triggered.");

            var json = await _dataverseService.Query(entity, query);

            return new OkObjectResult(json);
        }
    }
}

