using CinemaBookingSystemBLL.DTO.Payment;
using CinemaBookingSystemBLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CinemaBookingSystemAPI.Controllers
{
    [Route("api/payments")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class PaymentController : ControllerBase
    {
        private IPaymentService paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            this.paymentService = paymentService;
        }

        /// <summary>
        /// Get all payments.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet]
        [ProducesResponseType(typeof(List<PaymentResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllAsync(CancellationToken cancellationToken)
        {
            List<PaymentResponseDTO> payments = await paymentService.GetAllAsync(cancellationToken);
            return Ok(payments);
        }

        /// <summary>
        /// Get payments by session ID.
        /// </summary>
        /// <param name="sessionId">Session ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet("session/{sessionId:Guid}")]
        [ProducesResponseType(typeof(List<PaymentResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBySessionIdAsync(Guid sessionId, CancellationToken cancellationToken)
        {
            List<PaymentResponseDTO> payments = await paymentService.GetBySessionIdAsync(sessionId, cancellationToken);
            return Ok(payments);
        }

        /// <summary>
        /// Get payments by user ID.
        /// </summary>
        /// <param name="userId">User ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet("user/{userId}")]
        [ProducesResponseType(typeof(List<PaymentResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByUserIdAsync(string userId, CancellationToken cancellationToken)
        {
            List<PaymentResponseDTO> payments = await paymentService.GetByUserIdAsync(userId, cancellationToken);
            return Ok(payments);
        }

    }
}
