using System.Data.Common;
using System.Data.SqlClient;
using PrzykladowyKolosFinal.DTOs;
using PrzykladowyKolosFinal.Exceptions;

namespace PrzykladowyKolosFinal.Repositories;

public class PrescriptionRepository : IPrescriptionRepository
{
    private readonly string connectionstring;

    public PrescriptionRepository(IConfiguration configuration)
    {
        connectionstring = configuration.GetConnectionString("DefaultConnection");
    }

    public async Task<List<PrescriptionDto>> GetPrescriptionsAsync(string lastname)
    {
        List<PrescriptionDto> prescriptions = new();
        string query;
        using var con = new SqlConnection(connectionstring);
        await con.OpenAsync();
        using var cmd = new SqlCommand();
        cmd.Connection = con;
        if (string.IsNullOrWhiteSpace(lastname))
        {
            cmd.CommandText= @"Select pr.IdPrescription, pr.Date, pr.DueDate, p.LastName as PatientLastName, d.LastName as DoctorLastName
                        from prescription pr
                        join Patient p on p.IdPatient = pr.IdPatient
                        join Doctor d on d.IdDoctor = pr.IdDoctor
                        order by pr.Date DESC";
            
        }
        else
        {
            cmd.CommandText = @"Select pr.IdPrescription, pr.Date, pr.DueDate, p.LastName as PatientLastName, d.LastName as DoctorLastName
                        from prescription pr
                        join Patient p on p.IdPatient = pr.IdPatient
                        join Doctor d on d.IdDoctor = pr.IdDoctor
                        where d.LastName = @doctorlastName
                        order by pr.Date DESC";
            cmd.Parameters.AddWithValue("@doctorLastName", lastname);
        }

        
            var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                PrescriptionDto prescriptionDto = new()
                {
                    IdPrescription = int.Parse(reader["IdPrescription"].ToString()),
                    Date = DateTime.Parse(reader["Date"].ToString()),
                    DueDate = DateTime.Parse(reader["DueDate"].ToString()),
                    PatientLastName = reader["PatientLastName"].ToString(),
                    DoctorLastName = reader["DoctorLastName"].ToString()
                };
                prescriptions.Add(prescriptionDto);
                Console.WriteLine(cmd.CommandText.ToString());
                return prescriptions;
            }
            else
            {
                throw new Exception("wystąpił błąd podczas wyswietlania");
            }
        
    }

    public async Task<bool> DoctorExist(string lastname)
    {
        using var con = new SqlConnection(connectionstring);
        await con.OpenAsync();
        using var cmd = new SqlCommand();
        cmd.Connection = con;
        cmd.CommandText = "Select count(*) from Doctor where LastName = @lastname";
        cmd.Parameters.AddWithValue("@lastname", lastname);
        var docCount = (int)await cmd.ExecuteScalarAsync();
        if (docCount == 0)
        {
            throw new NotFoundException("nie ma takiego doktora");
        }

        return true;
    }

    public async Task<PrescriptionAfterAddDto> AddPrescription(PrescriptionDtoToAdd prescriptionDtoToAdd)
    {
        PrescriptionAfterAddDto newprescription = new();
        using var con = new SqlConnection(connectionstring);
        await con.OpenAsync();
        
        // sprawdzamy czy dueDate po Date
        if (prescriptionDtoToAdd.Date > prescriptionDtoToAdd.DueDate)
        {
            throw new ConflictException("Zła data ważności");
        }


        using SqlTransaction transaction = (SqlTransaction)await con.BeginTransactionAsync();

        try
        {
        // sprawdzam czy doktor o podanym id isntieje
        var query1 = "Select count(*) from Doctor where IdDoctor = @idDoctor";
        using (var cmd = new SqlCommand(query1, con, transaction))
        {
            cmd.Parameters.AddWithValue("@IdDoctor", prescriptionDtoToAdd.IdDoctor);
            var docCount = (int)await cmd.ExecuteScalarAsync();
            if (docCount == 0)
            {
                throw new NotFoundException("nie ma takiego doktora");
            }
        }
        // sprawdzamy czy pacjent o podanym id istnieje
        var query2 = "Select count(*) from Patient where IdPatient = @idPatient";
        using (var cmd = new SqlCommand(query2,con,transaction))
        {
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@idPatient", prescriptionDtoToAdd.IdPatient);
            var patientCount = (int)await cmd.ExecuteScalarAsync();
            if (patientCount == 0)
            {
                throw new NotFoundException("nie ma takiego pacjenta");
            }
        }
        
        // dodajemy recepte
        int idPresc;
        var query3 =
            "Insert Into Prescription(Date, DueDate, IdPatient, IdDoctor) output inserted.IdPrescription values (@date, @dueDate, @idPatient, @idDoctor)";
        using (var cmd = new SqlCommand(query3, con, transaction))
        {
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@date", prescriptionDtoToAdd.Date);
            cmd.Parameters.AddWithValue("@dueDate", prescriptionDtoToAdd.DueDate);
            cmd.Parameters.AddWithValue("@idPatient", prescriptionDtoToAdd.IdPatient);
            cmd.Parameters.AddWithValue("@idDoctor", prescriptionDtoToAdd.IdDoctor);
            using (var reader = await cmd.ExecuteReaderAsync())
            {
                if (await reader.ReadAsync())
                {
                    idPresc = reader.GetInt32(0); // Assuming IdPrescription is the first column
                }
                else
                {
                    throw new Exception("Failed to retrieve inserted IdPrescription.");
                }
            }
        }
        //skladamy i zwracamy obiekt
        newprescription = new()
        {
            IdPrescription = idPresc,
            Date = prescriptionDtoToAdd.Date,
            DueDate = prescriptionDtoToAdd.DueDate,
            IdDoctor = prescriptionDtoToAdd.IdDoctor,
            IdPatient = prescriptionDtoToAdd.IdPatient
        };

        await transaction.CommitAsync();
        return newprescription;
        }
        catch (NotFoundException e)
        {
            await transaction.RollbackAsync();
            throw new NotFoundException(e.Message);
        }
        
    }
}