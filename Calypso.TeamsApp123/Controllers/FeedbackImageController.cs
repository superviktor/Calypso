using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.Net.Http.Headers;
using Calypso.TeamsApp123.Repositories;

namespace Calypso.TeamsApp123.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedbackImageController : ControllerBase
    {
        private readonly IFeedbackImageRepository _feedbackImageRepository;

        public FeedbackImageController(IFeedbackImageRepository feedbackImageRepository)
        {
            _feedbackImageRepository = feedbackImageRepository;
        }

        [HttpGet]
        [Route("{name}")]
        public async Task<IActionResult> Get(string name)
        {
            var content = await _feedbackImageRepository.DownloadImageAsync(name);
            return new FileStreamResult(content, new MediaTypeHeaderValue("application/octet-stream"))
            {
                FileDownloadName = $"{name}.jpg"
            };
        }
    }
}
