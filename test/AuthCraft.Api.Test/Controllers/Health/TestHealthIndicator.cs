using System.Net;
using System.Threading.Tasks;
using AuthCraft.Api.Health;

namespace AuthCraft.Api.Test.Controllers.Health
{
    public class TestHealthIndicator : IHealthIndicator
    {
        public TestHealthIndicator(string name, HttpStatusCode status, string details, long executionTimeInMilliseconds)
        {
            Name = name;
            Status = status;
            Details = details;
            ExecutionTimeInMilliseconds = executionTimeInMilliseconds;
        }

        public string Identifier => typeof(TestHealthIndicator).FullName;

        public string Name { get; set; }

        public HttpStatusCode Status { get; set; }

        public string Details { get; set; }

        public long ExecutionTimeInMilliseconds { get; set; }

        public Task<HealthIndicatorModel> CheckStatusAsync()
        {
            var model = new HealthIndicatorModel(Name, Status, Details)
            {
                ExecutionTimeInMilliseconds = ExecutionTimeInMilliseconds,
            };
            return Task.FromResult(model);
        }
    }
}
