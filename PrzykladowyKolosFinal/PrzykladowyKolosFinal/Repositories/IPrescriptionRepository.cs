using PrzykladowyKolosFinal.DTOs;

namespace PrzykladowyKolosFinal.Repositories;

public interface IPrescriptionRepository
{
    Task<List<PrescriptionDto>> GetPrescriptionsAsync(string lastname);
    Task<bool> DoctorExist(string lastname);
    Task<PrescriptionAfterAddDto> AddPrescription(PrescriptionDtoToAdd prescriptionDtoToAdd);
}