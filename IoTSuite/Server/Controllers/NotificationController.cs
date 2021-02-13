using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using IoTSuite.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IoTSuite.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class NotificationsController : ControllerBase
    {
        // GET: Notifications
        [Obsolete]
        [HttpGet]
        [Authorize(Policy = "read")]
        public ActionResult GetNotification()
        {
            return StatusCode(StatusCodes.Status501NotImplemented);
        }

        // GET: Notifications/5
        [Obsolete]
        [HttpGet("{Id}")]
        [Authorize(Policy = "read")]
        public ActionResult GetNotification(long Id)
        {
            return StatusCode(StatusCodes.Status501NotImplemented);
        }

        // PUT: Notifications/5
        [Obsolete]
        [HttpPut("{Id}")]
        [Authorize(Policy = "write")]
        public ActionResult PutNotification(long Id)
        {
            return StatusCode(StatusCodes.Status501NotImplemented);
        }

        // POST: Notifications
        [Obsolete]
        [HttpPost]
        [Authorize(Policy = "write")]
        public ActionResult PostNotification()
        {
            return StatusCode(StatusCodes.Status501NotImplemented);
        }

        // DELETE: Notifications/5
        [Obsolete]
        [HttpDelete("{Id}")]
        [Authorize(Policy = "write")]
        public ActionResult DeleteNotification(long Id)
        {
            return StatusCode(StatusCodes.Status501NotImplemented);
        }

        // POST: Notifications/SMS/DeliveryReceipt
        [HttpPost("SMS/DeliveryReceipt")]
        [Authorize(AuthenticationSchemes = "Basic", Policy = "write")]
        [Consumes("application/x-www-form-urlencoded", MediaTypeNames.Application.Json)]
        public ActionResult PostSMSDeliveryConfirmation([FromForm, Required]SMSDeliveryReceipt deliveryReceipt)
        {
            foreach (var property in deliveryReceipt.GetType().GetProperties())
            {
                Console.WriteLine($"{property.Name}: {property.GetValue(deliveryReceipt)}");
            }

            return Ok();
        }
    }
}
