using Microsoft.AspNetCore.Mvc;
using TestTask.Services;

namespace TestTask.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FieldsController : ControllerBase
    {
        private readonly FieldService _fieldService;

        public FieldsController(FieldService fieldService)
        {
            _fieldService = fieldService;
        }

        [HttpGet]
        public IActionResult GetAllFields()
        {
            var fields = _fieldService.GetAllFields();
            return Ok(fields);
        }

        [HttpGet("{id}/size")]
        public IActionResult GetFieldSize(string id)
        {
            var size = _fieldService.GetFieldSize(id);
            if (size == null) return NotFound();
            return Ok(size);
        }

        [HttpGet("{id}/distance")]
        public IActionResult GetDistanceToCenter(string id, [FromQuery] double lat, [FromQuery] double lng)
        {
            var distance = _fieldService.GetDistanceToPoint(id, lat, lng);
            if (distance == null) return NotFound();
            return Ok(distance);
        }

        [HttpGet("contains")]
        public IActionResult CheckPointInFields([FromQuery] double lat, [FromQuery] double lng)
        {
            var result = _fieldService.FindFieldContainingPoint(lat, lng);
            if (result == null) return Ok(false);
            return Ok(result);
        }
    }
}
