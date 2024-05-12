using PrzykladowyKolosFinal.DTOs;

namespace PrzykladowyKolosFinal.Services;

public interface IPrescriptionService
{
    Task<List<PrescriptionDto>> GetPrescriptionsAsync(string lastname);
    Task<PrescriptionAfterAddDto> AddPrescription(PrescriptionDtoToAdd prescriptionDtoToAdd);
}