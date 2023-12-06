using System.Net;
using AuthCraft.Api.Controllers;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace AuthCraft.Api.Test.Controllers
{
    public class BaseControllerTest : BaseController
    {
        [Fact]
        public void StatusCode_Test()
        {
            var code = HttpStatusCode.Found;
            var content = new object();
            var status = StatusCode(code, content);

            Assert.NotNull(status);
            Assert.IsType<ObjectResult>(status);
        }

        [Fact]
        public void BaseController_CheckAttributes()
        {
            // TODO: Investigate how to check attributes without Castle.Core;
            // Assert.True(typeof(BaseController).GetAttributes<ModelValidatorAttribute>().Any());

            // Assert.True(typeof(BaseController).GetAttributes<NormalizationFilterAttribute>().Any());

            // Assert.True(typeof(BaseController).GetAttributes<ProducesAttribute>().Any());
        }
    }
}
