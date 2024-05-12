using Microsoft.AspNetCore.Mvc;
using PrzykladowyKolosFinal.DTOs;
using PrzykladowyKolosFinal.Exceptions;
using PrzykladowyKolosFinal.Services;

namespace PrzykladowyKolosFinal.Controllers;
[Route("api/prescriptions")]
[ApiController]
public class PrescriptionController : ControllerBase
{
    private readonly IPrescriptionService _prescriptionService;

    public PrescriptionController(IPrescriptionService prescriptionService)
    {
        _prescriptionService = prescriptionService;
    }

    [HttpGet]
    public async Task<IActionResult> GetPrescriptionsAsync([FromQuery] string lastname = "")
    {
        try
        {
            return Ok(await _prescriptionService.GetPrescriptionsAsync(lastname));
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPost]
    public async Task<IActionResult> AddPrescription(PrescriptionDtoToAdd prescriptionDtoToAdd)
    {
        try
        {
            return Ok(await _prescriptionService.AddPrescription(prescriptionDtoToAdd));
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}