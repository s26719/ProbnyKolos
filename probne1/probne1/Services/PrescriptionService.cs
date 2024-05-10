using System.Globalization;
using probne1.DTOs;
using probne1.Models;
using probne1.Repositories;

namespace probne1.Services;

public class PrescriptionService : IPrescriptionService
{
    private readonly IPrescriptionRepository _prescriptionRepository;

    public PrescriptionService(IPrescriptionRepository prescriptionRepository)
    {
        _prescriptionRepository = prescriptionRepository;
    }
    
    public async Task<List<Prescription>> GetPrescriptionsByLastNameAsync(string lastname)
    {
        return await _prescriptionRepository.GetPrescriptionsByLastNameAsync(lastname);
    }

    public Task<bool> ExistDoctorById(PrescriptionDto prescriptionDto)
    {
        return _prescriptionRepository.ExistDoctorById(prescriptionDto);
    }

    public Task<bool> ExistPatientById(PrescriptionDto prescriptionDto)
    {
        return _prescriptionRepository.ExistPatientById(prescriptionDto);
    }


    public async Task<PrescriptionOutputDto> AddPrescription(PrescriptionDto prescriptionDto)
    {
        var patientExist = await ExistPatientById(prescriptionDto);
        var doctorExist = await ExistDoctorById(prescriptionDto);
        if (!patientExist)
        {
            throw new NotFoundException("Nie znaleziono pacjenta");
        }

        if (!doctorExist)
        {
            throw new NotFoundException("Nie znaleziono Doktora");
        }

        if (prescriptionDto.DueDate < prescriptionDto.Date)
        {
            throw new ConflictException("Zła data ważności");
        }
        var idPrescription = await _prescriptionRepository.AddPrescription(prescriptionDto);
        var result = new PrescriptionOutputDto()
        {
            IdPrescription = idPrescription,
            Date = prescriptionDto.Date,
            DueDate = prescriptionDto.DueDate,
            IdDoctor = prescriptionDto.IdDoctor,
            IdPatient = prescriptionDto.IdPatient
        };
        return result;
    }
}