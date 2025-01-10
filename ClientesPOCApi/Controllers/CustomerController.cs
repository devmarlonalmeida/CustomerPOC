using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClientesPOCApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var customers = await _customerService.GetAllCustomersAsync();
            return Ok(customers);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var customer = await _customerService.GetCustomerByIdAsync(id);
            if (customer == null)
                return NotFound();
            return Ok(customer);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CustomerDTO customer)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                                .Select(e => e.ErrorMessage)
                                                .ToList();
                return BadRequest(new { Message = "Validation Failed", Errors = errors });
            }

            await _customerService.AddCustomerAsync(customer);
            return CreatedAtAction(nameof(GetById), new { id = customer.Id }, customer);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromForm] CustomerDTO customer)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            customer.Id = id;
            await _customerService.UpdateCustomerAsync(customer);
            return Ok();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _customerService.RemoveCustomerAsync(id);
            return NoContent();
        }
    }
}
