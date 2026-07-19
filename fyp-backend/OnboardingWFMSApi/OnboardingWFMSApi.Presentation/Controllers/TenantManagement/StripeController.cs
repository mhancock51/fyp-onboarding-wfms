// using Microsoft.AspNetCore.Mvc;
// using Stripe;
// using Stripe.Checkout;

// namespace OnboardingWFMSApi.Presentation.Controllers.TenantManagement
// {
//     [ApiController]
//     [Route("api/[controller]")]
//     public class StripeController : ControllerBase
//     {
//         private const string YourDomain = "http://localhost:3000"; // Frontend domain

//         [HttpPost("create-checkout-session")]
//         public async Task<IActionResult> CreateCheckoutSession([FromBody] CheckoutRequest request)
//         {
//             if (string.IsNullOrEmpty(request.PriceId))
//             {
//                 return BadRequest(new { error = "Price ID is required." });
//             }

//             try
//             {
//                 var options = new SessionCreateOptions
//                 {
//                     LineItems = new List<SessionLineItemOptions>
//                     {
//                         new SessionLineItemOptions
//                         {
//                             // Provide the exact Price ID from your Stripe Dashboard
//                             Price = request.PriceId,
//                             Quantity = 1,
//                         },
//                     },
//                     // Use "subscription" if this price is a recurring plan
//                     Mode = "payment", 
//                     SuccessUrl = $"{YourDomain}/success.html",
//                     CancelUrl = $"{YourDomain}/cancel.html",
//                 };

//                 var service = new SessionService();
//                 Session session = await service.CreateAsync(options);

//                 // Return the checkout URL to your frontend
//                 return Ok(new { url = session.Url });
//             }
//             catch (StripeException e)
//             {
//                 // Log error internally
//                 return StatusCode(500, new { error = e.Message });
//             }
//         }

//         // Simple model to capture incoming JSON from the frontend
//         public class CheckoutRequest
//         {
//             public string SubscriptionTierId { get; set; }
//         }
//     }
// }