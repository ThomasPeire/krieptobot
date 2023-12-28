using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace KrieptoBot.Infrastructure.Bitvavo;

public class BadRequestLoggingHandler : DelegatingHandler
{
    private readonly ILogger<BadRequestLoggingHandler> _logger;
    public BadRequestLoggingHandler(ILogger<BadRequestLoggingHandler> logger)
    {
        _logger = logger;
    }
    
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogError("Bad request: {Content}", content);
        }

        return response;
    }
    
}
