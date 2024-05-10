using probne1.DTOs;
using probne1.Models;

namespace probne1.Repositories;

public interface IPrescriptionRepository
{
    Task<List<Prescription>> GetPrescriptionsByLastNameAsync(string lastname);
    Task<bool> ExistPatientById(PrescriptionDto prescriptionDto);
    Task<bool> ExistDoctorById(PrescriptionDto prescriptionDto);
    Task<int> AddPrescription(PrescriptionDto prescriptionDto);
}