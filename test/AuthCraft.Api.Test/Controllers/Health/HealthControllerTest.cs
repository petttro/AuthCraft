using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AuthCraft.Api.Controllers.Health;
using AuthCraft.Api.Health;
using AuthCraft.Api.Test.Common;
using Microsoft.AspNetCore.Mvc;
using AuthCraft.Api.Controllers;
using Xunit;

namespace AuthCraft.Api.Test.Controllers.Health
{
    /// <summary>
    /// Test of HealthController
    /// </summary>
    public class HealthControllerTest
    {
        /// <summary>
        ///     Controller Test with indicator which arises an exception during GetHealth call
        /// </summary>
        [Fact]
        public async Task HealthController_GetHealth_AriseExceptionInIndicator()
        {
            // Add indicator which arises an exception
            var indicator = new ExceptionHealthIndicator();
            var indicators = new List<IHealthIndicator>() { indicator };
            var loggerFactory = TestLoggerFactory.Create();

            var controller = new HealthController(indicators, loggerFactory);

            var result = await controller.GetHealth();
            var okResult = Assert.IsType<ObjectResult>(result);
            var returnValue = Assert.IsAssignableFrom<HealthCheckModel>(okResult.Value);

            // Controller returns non null value
            Assert.NotNull(returnValue);

            // Indicators and check results have the same count of items.
            Assert.Equal(returnValue.HealthChecks.Count(), indicators.Count());

            // Status of HealthCheckModel
            Assert.Equal(returnValue.Status, returnValue.HealthChecks.Max(x => x.Status));

            var checkResult = returnValue.HealthChecks.First();

            // On exception check result should have:
            Assert.Equal(HttpStatusCode.ServiceUnavailable, checkResult.Status);
            Assert.Contains("Exception=", checkResult.Details);
            Assert.Contains("Exception:", checkResult.Details);
            Assert.Equal(checkResult.Name, indicator.Identifier);
        }

        [Fact]
        public async Task HealthController_GetHealth_EmptyIndicatorListReturnsResult()
        {
            var indicators = new List<IHealthIndicator>();
            var loggerFactory = TestLoggerFactory.Create();

            var controller = new HealthController(indicators, loggerFactory);

            var result = await controller.GetHealth();

            var okResult = Assert.IsType<ObjectResult>(result);
            var returnValue = Assert.IsAssignableFrom<HealthCheckModel>(okResult.Value);

            // Controller returns non null value
            Assert.NotNull(returnValue);

            // Indicators and check results have the same count of items.
            Assert.Equal(returnValue.HealthChecks.Count(), indicators.Count());
        }

        [Fact]
        public async Task HealthController_GetHealth_PopulatedIndicatorListReturnsResult()
        {
            var indicators = new List<IHealthIndicator>()
            {
                new TestHealthIndicator("Name1", HttpStatusCode.Accepted, "Details1", 34),
                new TestHealthIndicator("Name2", HttpStatusCode.Gone, "Details2", 15),
                new TestHealthIndicator("Name3", HttpStatusCode.BadRequest, "Details3", 342),
                new TestHealthIndicator("Name4", HttpStatusCode.OK, "Details4", 56),
                new TestHealthIndicator("Name5", HttpStatusCode.Ambiguous, "Details5", 1),
            };
            var loggerFactory = TestLoggerFactory.Create();

            var controller = new HealthController(indicators, loggerFactory);

            var result = await controller.GetHealth();

            var okResult = Assert.IsType<ObjectResult>(result);
            var returnValue = Assert.IsAssignableFrom<HealthCheckModel>(okResult.Value);

            // Controller returns non null value
            Assert.NotNull(returnValue);

            // Indicators and check results have the same count of items.
            Assert.Equal(returnValue.HealthChecks.Count(), indicators.Count());
            Assert.Equal(HttpStatusCode.Gone, returnValue.Status);
            Assert.Equal(342, returnValue.ExecutionTimeInMilliseconds);
        }
    }
}
