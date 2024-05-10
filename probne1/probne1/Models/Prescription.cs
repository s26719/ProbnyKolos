using System.ComponentModel.DataAnnotations;

namespace probne1.Models;

public class Prescription
{
    [Required]
    public int IdPrescription { get; set; }
    public DateTime Date { get; set; }
    public DateTime DueDate { get; set; }
    public string PatientLastName { get; set; }
    public string DoctorLastName { get; set; }
    
}