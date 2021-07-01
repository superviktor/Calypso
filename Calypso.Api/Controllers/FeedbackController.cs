using System;
using System.Threading.Tasks;
using Calypso.Api.Common;
using Calypso.Api.Config;
using Calypso.Api.Dtos;
using Calypso.Api.Models;
using Calypso.Api.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Microsoft.Graph.Auth;
using Microsoft.Identity.Client;

namespace Calypso.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedbackController : ControllerBase
    {
        private readonly IFeedbackRepository _feedbackRepository;
        private readonly IFeedbackImageRepository _feedbackImageRepository;
        private readonly GraphServiceClient _graphServiceClient;

        public FeedbackController(
            IFeedbackRepository feedbackRepository,
            IFeedbackImageRepository feedbackImageRepository,
            IOptions<AzureAdOptions> azureAdOptions)
        {
            var confidentialClientApplication = ConfidentialClientApplicationBuilder
                .Create(azureAdOptions.Value.ClientId)
                .WithTenantId(azureAdOptions.Value.TenantId)
                .WithClientSecret(azureAdOptions.Value.ClientSecret)
                .Build();

            var authProvider = new AuthorizationCodeProvider(confidentialClientApplication);
            _graphServiceClient = new GraphServiceClient(authProvider);

            _feedbackRepository = feedbackRepository;
            _feedbackImageRepository = feedbackImageRepository;
        }
        [HttpGet]
        public async Task<IActionResult> GetPaged([FromQuery] int pageNumber, [FromQuery] int itemsPerPage)
        {
            var feedbacks = await _feedbackRepository.GetAsync(pageNumber, itemsPerPage);
            return Ok(feedbacks);
        }

        [HttpGet]
        [Route("{rowKey}")]
        public async Task<IActionResult> Get(string rowKey)
        {
            var feedback = await _feedbackRepository.GetAsync(rowKey);
            return Ok(feedback.Map<FeedbackDto>());
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CreateFeedback req, 
            [FromServices] IOptions<PlannerOptions> plannerOptions)
        {
            var feedback = req.Map<Feedback>();
            feedback.RowKey = Guid.NewGuid().ToString();
            feedback.PartitionKey = _feedbackRepository.PartitionKey;

            var fileName = Guid.NewGuid().ToString();
            await _feedbackImageRepository.UploadImageAsync(fileName, await req.File.ToStreamAsync());
            feedback.FileName = fileName;

            await _feedbackRepository.CreateAsync(feedback);

            await _graphServiceClient.Planner.Tasks.Request().AddAsync(new PlannerTask
            {
                PlanId = plannerOptions.Value.PlanId,
                Title = "from backend"
            });

            return Created("", feedback);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromForm] UpdateFeedback req)
        {
            var feedback = await _feedbackRepository.GetAsync(req.RowKey);
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
            //delete existing file
            await _feedbackImageRepository.DeleteImageAsync(feedback.FileName);
            //add new file
            var fileName = Guid.NewGuid().ToString();
            await _feedbackImageRepository.UploadImageAsync(fileName, await req.File.ToStreamAsync());
            feedback.FileName = fileName;

            await _feedbackRepository.UpdateAsync(feedback);
            return NoContent();
        }

        [HttpDelete]
        [Route("{rowKey}")]
        public async Task<IActionResult> Delete(string rowKey)
        {
            var feedback = await _feedbackRepository.GetAsync(rowKey);
            await _feedbackRepository.DeleteAsync(feedback);
            await _feedbackImageRepository.DeleteImageAsync(feedback.FileName);
            return NoContent();
        }
    }
}
