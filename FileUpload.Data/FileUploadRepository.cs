using System;
using System.Data.SqlClient;

namespace FileUpload.Data
{
    public class FileUploadRepository
    {
        private string _connectionString;

        public FileUploadRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public int Add(Image image)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO Images (ImagePath, Password, NumberOfViews) VALUES (@path, @password, 0)" +
                               "SELECT SCOPE_IDENTITY()";
           
            cmd.Parameters.AddWithValue("@path", image.ImagePath);
            cmd.Parameters.AddWithValue("@password", image.Password);
            cmd.Parameters.AddWithValue("@numOfViews", image.NumberOfViews);
            conn.Open();
            return (int)(decimal)cmd.ExecuteScalar();
        }
        public Image GetImageById(int Id)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM Images WHERE id=@id";
            cmd.Parameters.AddWithValue("@id", Id);
            conn.Open();
            SqlDataReader reader=cmd.ExecuteReader();
            reader.Read();

            Image image = new Image
            {
                Id = (int)reader["Id"],
                ImagePath = (string)reader["ImagePath"],
                Password=(string)reader["Password"],
                NumberOfViews=(int)reader["NumberOfViews"]
            };

            return image;
        }
        public void UpdateImages(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE Images SET NumberOfViews= NumberOfViews+1 WHERE id=@id";
            cmd.Parameters.AddWithValue("@id", id);
            conn.Open();
            cmd.ExecuteNonQuery();
        }
        public string GetPassword(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT password FROM Images WHERE id=@id";
            cmd.Parameters.AddWithValue("@id", id);
            conn.Open();
          
            return (string)cmd.ExecuteScalar();

        }
    }
}
