using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using AuthCraft.Api.Health;
using Xunit;

namespace AuthCraft.Api.Test.Controllers.Health
{
    /// <summary>
    /// Tests of HealthCheckModel
    /// </summary>
    public class HealthCheckModelTest
    {
        [Fact]
        public void HealthCheckModel_Constructor_Success()
        {
            var dateBefore = DateTime.UtcNow;
            var instance = new HealthCheckModel();

            // Status hasnt default value
            Assert.Equal(0, (int)instance.Status);

            // Current Date from DateTime.UtcNow;
            Assert.InRange(instance.CurrentTime, dateBefore, DateTime.UtcNow);

            // HealthChecks is not empty collection. Just null
            Assert.Null(instance.HealthChecks);

            instance.ExecutionTimeInMilliseconds = 42;

            Assert.Equal(42, instance.ExecutionTimeInMilliseconds);
        }

        [Fact]
        public void HealthCheckModel_ConstructorWithIndicators_Success()
        {
            var indicators = new List<HealthIndicatorModel>()
            {
                new HealthIndicatorModel("Name1", HttpStatusCode.Accepted, "Details1") { ExecutionTimeInMilliseconds = 54 },
                new HealthIndicatorModel("Name2", HttpStatusCode.Gone, "Details2") { ExecutionTimeInMilliseconds = 1 },
                new HealthIndicatorModel("Name3", HttpStatusCode.BadRequest, "Details3") { ExecutionTimeInMilliseconds = 342 },
                new HealthIndicatorModel("Name4", HttpStatusCode.OK, "Details4") { ExecutionTimeInMilliseconds = 4 },
                new HealthIndicatorModel("Name5", HttpStatusCode.Ambiguous, "Details5") { ExecutionTimeInMilliseconds = 5 },
            };

            var instance = new HealthCheckModel();
            instance.HealthChecks = indicators;

            Assert.NotNull(instance.HealthChecks);

            Assert.Equal(instance.HealthChecks.Count(), indicators.Count());
            Assert.Equal(0, (int)instance.Status);
            Assert.Equal(0, instance.ExecutionTimeInMilliseconds);
        }
    }
}
