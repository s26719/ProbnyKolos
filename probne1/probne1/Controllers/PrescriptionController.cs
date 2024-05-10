using Microsoft.AspNetCore.Mvc;
using probne1.DTOs;
using probne1.Models;
using probne1.Services;

namespace probne1.Controllers;
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
    public async Task<IActionResult> GetPrescriptionsByLastNameAsync([FromQuery] string lastname = "")
    {
        IList<Prescription> prescriptions = await _prescriptionService.GetPrescriptionsByLastNameAsync(lastname);
        return Ok(prescriptions);
    }

    [HttpPost]
    public async Task<IActionResult> AddPrescription(PrescriptionDto prescriptionDto)
    {

        try
        {
            var result = _prescriptionService.AddPrescription(prescriptionDto).Result;
            return Ok(result);
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
        catch (ConflictException e)
        {
            return Conflict(e.Message);
        }
        
    }
}