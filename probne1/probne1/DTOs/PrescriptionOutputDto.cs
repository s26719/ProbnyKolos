namespace probne1.DTOs;

public class PrescriptionOutputDto
{
    public int IdPrescription { get; set; }
    public DateTime Date{ get; set; }
    public DateTime DueDate{ get; set; }
    public int IdPatient{ get; set; }
    public int IdDoctor{ get; set; }
}
