using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Runtime.InteropServices.JavaScript;
using probne1.DTOs;
using probne1.Models;

namespace probne1.Repositories;

public class PrescriptionRepository : IPrescriptionRepository
{
    private readonly string connectionString;

    public PrescriptionRepository(IConfiguration configuration)
    {
        connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    private List<Prescription> prescription = new();



    public async Task<List<Prescription>> GetPrescriptionsByLastNameAsync(string lastname)
    {
        using var con = new SqlConnection(connectionString);
        await con.OpenAsync();
        using var cmd = new SqlCommand();
        cmd.Connection = con;

        if (string.IsNullOrWhiteSpace(lastname))
        {
            cmd.CommandText = "SELECT pr.IdPrescription,pr.Date, pr.DueDate, d.LastName, p.LastName as PatientLastName from Prescription pr, Doctor d, Patient p WHERE p.IdPatient = pr.IdPatient and d.IdDoctor = pr.IdDoctor";
        }
        else
        {
            cmd.CommandText = "SELECT pr.IdPrescription,pr.Date, pr.DueDate, d.LastName, p.LastName as PatientLastName from Prescription pr, Doctor d, Patient p WHERE p.IdPatient = pr.IdPatient and d.IdDoctor = pr.IdDoctor and d.LastName = @lastname ";
            cmd.Parameters.AddWithValue("@lastname", lastname);
        }

      using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            Prescription prescriptions = new()
            {
                IdPrescription = int.Parse(reader["IdPrescription"].ToString()),
                Date = DateTime.Parse(reader["Date"].ToString()),
                DueDate = DateTime.Parse(reader["DueDate"].ToString()),
                PatientLastName = reader["PatientLastName"].ToString()!, // ! mowi ze ta wartosc nie jest nullem
                DoctorLastName = reader["LastName"].ToString()!
            };
            prescription.Add(prescriptions);
        }

        return prescription;
    }

    public async Task<bool> ExistPatientById(PrescriptionDto prescriptionDto)
    {
        using var con = new SqlConnection(connectionString);
        await con.OpenAsync();
        using var cmd = new SqlCommand();
        cmd.Connection = con;
        cmd.CommandText = ("SELECT * from Patient where IdPatient = @IdPatient");
        cmd.Parameters.AddWithValue("@IdPatient", prescriptionDto.IdPatient);
        var result = await cmd.ExecuteScalarAsync();
        if (result != null && result != DBNull.Value)
        {
            return Convert.ToInt32(result) > 0;
        }

        return false;
    }

    public async Task<bool> ExistDoctorById(PrescriptionDto prescriptionDto)
    {
        using var con = new SqlConnection(connectionString);
        await con.OpenAsync();
        using var cmd = new SqlCommand();
        cmd.Connection = con;
        cmd.CommandText = ("SELECT * from Doctor where IdDoctor = @IdDoctor");
        cmd.Parameters.AddWithValue("@IdDoctor", prescriptionDto.IdDoctor);
        var result = await cmd.ExecuteScalarAsync();
        if (result != null && result != DBNull.Value)
        {
            return Convert.ToInt32(result) > 0;
        }

        return false;
    }

    public async Task<int> AddPrescription(PrescriptionDto prescriptionDto)
    {
        using var con = new SqlConnection(connectionString);
        await con.OpenAsync();
        using var cmd = new SqlCommand();
        cmd.Connection = con;
        

        cmd.CommandText =
            @"insert into Prescription(Date, DueDate, IdPatient, IdDoctor)
            OUTPUT Inserted.IdPrescription
            values (@Date, @DueDate, @IdPatient, @IdDoctor)";
        cmd.Parameters.AddWithValue("@Date", prescriptionDto.Date);
        cmd.Parameters.AddWithValue("@DueDate", prescriptionDto.DueDate);
        cmd.Parameters.AddWithValue("@IdPatient", prescriptionDto.IdPatient);
        cmd.Parameters.AddWithValue("@IdDoctor", prescriptionDto.IdDoctor);
        
        var result = (int)await cmd.ExecuteScalarAsync()!; // zwraca to jaki jest output. W naszym przypadku Inserted.IdPrescription. jezeli by nie bylo inserted to pewnie zwraca rows affected czyli 0,1,2... itp

        return result;
    }
    
}