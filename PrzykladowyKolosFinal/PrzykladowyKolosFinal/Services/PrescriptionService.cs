using PrzykladowyKolosFinal.DTOs;
using PrzykladowyKolosFinal.Exceptions;
using PrzykladowyKolosFinal.Repositories;

namespace PrzykladowyKolosFinal.Services;

public class PrescriptionService : IPrescriptionService
{
    private readonly IPrescriptionRepository _prescriptionRepository;

    public PrescriptionService(IPrescriptionRepository prescriptionRepository)
    {
        _prescriptionRepository = prescriptionRepository;
    }

    public async Task<List<PrescriptionDto>> GetPrescriptionsAsync(string lastname)
    {
        if (string.IsNullOrWhiteSpace(lastname))
        {
           return await _prescriptionRepository.GetPrescriptionsAsync(lastname);
        }
        else
        {
            try
            {
                await _prescriptionRepository.DoctorExist(lastname);
                return await _prescriptionRepository.GetPrescriptionsAsync(lastname);
            }
            catch (NotFoundException e)
            {
                throw new NotFoundException(e.Message);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }

    public async Task<PrescriptionAfterAddDto> AddPrescription(PrescriptionDtoToAdd prescriptionDtoToAdd)
    {
        try
        {
            return await _prescriptionRepository.AddPrescription(prescriptionDtoToAdd);
        }
        catch (NotFoundException e)
        {
            throw new NotFoundException(e.Message);
        }
    }
}