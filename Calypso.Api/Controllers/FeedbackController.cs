using System;
using System.Threading.Tasks;
using Calypso.Api.Common;
using Calypso.Api.Config;
using Calypso.Api.Dtos;
using Calypso.Api.Models;
using Calypso.Api.Repositories;
using Calypso.Api.Services;
using Calypso.Api.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Calypso.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FeedbackController : ControllerBase
    {
        private readonly IFeedbackRepository _feedbackRepository;
        private readonly IFeedbackImageRepository _feedbackImageRepository;
        private readonly IPlannerService _plannerService;

        public FeedbackController(
            IFeedbackRepository feedbackRepository,
            IFeedbackImageRepository feedbackImageRepository, 
            IPlannerService plannerService)
        {
            _feedbackRepository = feedbackRepository;
            _feedbackImageRepository = feedbackImageRepository;
            _plannerService = plannerService;
        }
        [HttpGet]
        public async Task<IActionResult> GetPaged(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int itemsPerPage = 10,
            [FromQuery] string searchString = null)
        {
            var feedbacks = await _feedbackRepository.GetAsync(pageNumber, itemsPerPage, searchString);
            return Ok(feedbacks);
        }

        [HttpGet]
        [Route("{rowKey}")]
        public async Task<IActionResult> Get(string rowKey)
        {
            var feedback = await _feedbackRepository.GetAsync(rowKey);

            if (feedback == null)
                return NotFound();

            return Ok(feedback.Map<FeedbackDto>());
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CreateFeedback req, 
            [FromServices] IOptions<PlannerOptions> plannerOptions)
        {
            var feedback = req.Map<Feedback>();
            feedback.RowKey = Guid.NewGuid().ToString();
            feedback.PartitionKey = _feedbackRepository.PartitionKey;
            if (req.File != null)
            {
                var fileName = Guid.NewGuid().ToString();
                await _feedbackImageRepository.UploadImageAsync(fileName, await req.File.ToStreamAsync());
                feedback.FileName = fileName;
            }
            await _feedbackRepository.CreateAsync(feedback);
            await _plannerService.CreateTask(HttpContext.Request.Headers.GetAuthorizationHeaderValue(), feedback.Subject);
            return Created("", feedback);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromForm] UpdateFeedback req)
        {
            var feedback = await _feedbackRepository.GetAsync(req.RowKey);
            if (feedback == null)
                return NotFound();

            feedback.Subject = req.Subject;
            feedback.Date = req.Date;
            feedback.Factory = req.Factory;
            feedback.Location = req.Location;
            feedback.Machine = req.Machine;
            feedback.ProductName = req.ProductName;
            feedback.ProjectName = req.ProjectName;
            feedback.Reporter = req.Reporter;
            feedback.Role = req.Role;
            feedback.Sbu = req.Sbu;
            if (req.File != null)
            {
                //delete existing file
                await _feedbackImageRepository.DeleteImageAsync(feedback.FileName);
                //add new file
                var fileName = Guid.NewGuid().ToString();
                await _feedbackImageRepository.UploadImageAsync(fileName, await req.File.ToStreamAsync());
                feedback.FileName = fileName;
            }

            await _feedbackRepository.UpdateAsync(feedback);
            return NoContent();
        }

        [HttpDelete]
        [Route("{rowKey}")]
        public async Task<IActionResult> Delete(string rowKey)
        {
            var feedback = await _feedbackRepository.GetAsync(rowKey);
            if (feedback == null)
                return NotFound();
            await _feedbackRepository.DeleteAsync(feedback);
            if(feedback.FileName != null)
                await _feedbackImageRepository.DeleteImageAsync(feedback.FileName);
            return NoContent();
        }
    }
}
