using System;
using System.Threading.Tasks;
using Calypso.Api.Common;
using Calypso.Api.Dtos;
using Calypso.Api.Models;
using Calypso.Api.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Calypso.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedbackController : ControllerBase
    {
        private readonly IFeedbackRepository _feedbackRepository;
        private readonly IFeedbackImageRepository _feedbackImageRepository;

        public FeedbackController(IFeedbackRepository feedbackRepository, IFeedbackImageRepository feedbackImageRepository)
        {
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
        public async Task<IActionResult> Create([FromForm] CreateFeedback req)
        {
            var feedback = req.Map<Feedback>();
            feedback.RowKey = Guid.NewGuid().ToString();
            feedback.PartitionKey = _feedbackRepository.PartitionKey;

            var fileName = Guid.NewGuid().ToString();
            await _feedbackImageRepository.UploadImageAsync(fileName,  await req.File.ToStreamAsync());
            feedback.FileName = fileName; 

            await _feedbackRepository.CreateAsync(feedback);
            return Created("", feedback);
        }

        [HttpPut]
        public IActionResult Update()
        {
            return NoContent();
        }

        [HttpDelete]
        public IActionResult Delete()
        {
            return NoContent();
        }
    }
}
