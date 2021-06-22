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
            await _feedbackImageRepository.UploadImageAsync(fileName, await req.File.ToStreamAsync());
            feedback.FileName = fileName;

            await _feedbackRepository.CreateAsync(feedback);
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
