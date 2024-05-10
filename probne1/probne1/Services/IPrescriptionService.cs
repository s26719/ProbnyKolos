using probne1.DTOs;
using probne1.Models;

namespace probne1.Services;

public interface IPrescriptionService
{
    Task<List<Prescription>> GetPrescriptionsByLastNameAsync(string lastname);
    Task<bool> ExistDoctorById(PrescriptionDto prescriptionDto);
    Task<bool> ExistPatientById(PrescriptionDto prescriptionDto);
    Task<PrescriptionOutputDto> AddPrescription(PrescriptionDto prescriptionDto);
}