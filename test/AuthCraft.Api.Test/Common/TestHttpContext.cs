using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace AuthCraft.Api.Test.Common
{
    public class TestHttpContext : HttpContext
    {
        public override IFeatureCollection Features { get; }

        public override HttpRequest Request { get; }

        public override HttpResponse Response { get; }

        public override ConnectionInfo Connection { get; }

        public override WebSocketManager WebSockets { get; }

        public override ClaimsPrincipal User { get; set; }

        public override IDictionary<object, object> Items { get; set; }

        public override IServiceProvider RequestServices { get; set; }

        public override CancellationToken RequestAborted { get; set; }

        public override string TraceIdentifier { get; set; }

        public override ISession Session { get; set; }

        public override void Abort()
        {
            return;
        }
    }
}
