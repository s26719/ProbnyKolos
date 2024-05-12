namespace PrzykladowyKolosFinal.DTOs;

public class PrescriptionDtoToAdd
{
 
    public DateTime Date { get; set; }
    public DateTime DueDate { get; set; }
    public int IdPatient { get; set; }
    public int IdDoctor { get; set; }
}