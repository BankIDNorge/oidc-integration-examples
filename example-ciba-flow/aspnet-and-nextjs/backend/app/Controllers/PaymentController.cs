using System.Net.Mime;
using System.Security.Cryptography;
using System.Text.Json;
using ExampleAspnet.Exceptions;
using ExampleAspnet.Models;
using ExampleAspnet.Models.Oidc;
using ExampleAspnet.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace ExampleAspnet.Controllers;

/// <summary>
/// The main controller responsible for the endpoints used in the frontend.
/// </summary>
/// <see cref="UserCheck"/>
/// <see cref="ConfirmPayment"/>
/// <see cref="Poll"/>
[ApiController]
[Route("api")]
public class PaymentController : ControllerBase
{
    private readonly IBisOidcService _bisOidcService;

    private readonly ILogger<PaymentController> _logger;

    public PaymentController(
        ILogger<PaymentController> logger,
        IBisOidcService bisOidcService
    )
    {
        _bisOidcService = bisOidcService;
        _logger = logger;
    }

    /// <summary>
    /// This endpoint is responsible for initiating the CIBA flow. In this example, it is called from the front end when the user
    /// clicks the "Approve with BankID" button.
    /// </summary>
    /// <param name="request">The payment requested. See <see cref="ConfirmPaymentRequest"/>.</param>
    [HttpPost("confirmpayment")]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ConfirmPaymentResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
    public async Task<IActionResult> ConfirmPayment([FromBody] ConfirmPaymentRequest request)
    {
        // This example allows the user to specify the payment amount and currency. In a real application, user-provided
        // values should not be trusted. Instead, the amount and currency should be acquired from the merchant requesting
        // payment authentication. 

        var now = DateTimeOffset.UtcNow;

        var nonce = Base64UrlEncoder.Encode(RandomNumberGenerator.GetBytes(32));

        // Payment or basket id used for lookup at ASPSP. The user does not see the id or any derivative of the id.
        var id = Base64UrlEncoder.Encode(RandomNumberGenerator.GetBytes(32));

        var statement = new PaymentV1
        {
            Nonce = nonce,
            Id = id,
            Payments = new List<PaymentBasePermissionV1>
            {
                new()
                {
                    PaymentId = id,
                    Amount = request.Amount,
                    Currency = request.Currency,
                    CreditorName = "TopShop"
                }
            }
        };

        var encodedPermission = Base64UrlEncoder.Encode(JsonSerializer.Serialize(statement));

        var permissionRequest = new PermissionRequest
        {
            Type = "payment.v1",
            Iat = now.ToUnixTimeSeconds(),
            Exp = now.ToUnixTimeSeconds() + 600,
            Permission = encodedPermission,
            LoginHint = new List<LoginHint> { request.LoginHint },
            Intents = new List<string> { "ciba" },
            Loa = "sub",
            Signals = request.Signals
        };

        // 1. Register the permission
        var permissionResponse = await _bisOidcService.RegisterPermission(permissionRequest);

        // 2. Call the bc-authorize endpoint 
        var authorizationToken = await _bisOidcService.BackchannelAuthorize(
            permissionResponse.PermissionToken, permissionResponse.BindingMessage);

        var session = HttpContext.Session;
        session.SetString("permission_id", permissionResponse.PermissionId);
        session.SetString("binding_message", permissionResponse.BindingMessage);
        session.SetString("auth_req_id", authorizationToken.AuthReqId);

        return Ok(new ConfirmPaymentResponse(permissionResponse.BindingMessage));
    }

    /// <summary>
    /// This endpoint is used to poll for the authorization result. In this example, it is called from the frontend.
    /// </summary>
    [HttpPost("poll")]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthorizeResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
    public async Task<IActionResult> Poll([FromBody] AuthorizeRequest authorizeRequest,
        CancellationToken cancellationToken)
    {
        var permissionId = HttpContext.Session.GetString("permission_id");
        var authReqId = HttpContext.Session.GetString("auth_req_id");
        if (permissionId == null || authReqId == null)
        {
            throw new InvalidSessionException();
        }

        var tokenResponse = await _bisOidcService.GetToken(authReqId);

        var idTokenValidation = await _bisOidcService.ValidateIdTokenAsync(tokenResponse.IdToken);
        if (!idTokenValidation.IsValid)
        {
            _logger.LogWarning("Invalid id token received: {}: {}", tokenResponse.IdToken, idTokenValidation.Exception);
            throw new AuthFlowException();
        }

        var grant = await _bisOidcService.PollGrant(permissionId, cancellationToken);
        var grantValidation = await _bisOidcService.ValidateGrantTokenAsync(grant);
        if (!grantValidation.IsValid)
        {
            _logger.LogWarning("Invalid grant token received: {}: {}", grant, grantValidation.Exception);
            throw new AuthFlowException();
        }

        _logger.LogInformation("Authorization succeeded, id token: {}, grant: {}", tokenResponse.IdToken, grant);

        return Ok(new AuthorizeResponse(true));
    }

    /// <summary>
    /// Checks whether a user has BankID with biometrics.
    /// </summary>
    [HttpPost("usercheck")]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserCheckResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
    public async Task<IActionResult> UserCheck([FromBody] UserCheckRequest body)
    {
        var response = await _bisOidcService.UserExistsAsync(body.Value, body.Scheme);
        return Ok(response);
    }
}