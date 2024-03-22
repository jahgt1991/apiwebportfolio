using API.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data;
using System.Data.SqlClient;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FormController : ControllerBase
    {
        public readonly IConfiguration _config;
        public FormController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet]
        [Route("GetAllForms")]
        public string GetForm()
        {
            SqlConnection con = new SqlConnection(_config.GetConnectionString("WebFormCon").ToString());
            SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM FormWeb", con);
            DataTable dt = new DataTable();
            da.Fill(dt);
            List<WebForm> webFormList = new List<WebForm>();
            Response response = new Response();

            if(dt.Rows.Count>0)
            {
                for(int i=0; i < dt.Rows.Count; i++)
                {
                    WebForm webForm = new WebForm();
                    webForm.Nombre = Convert.ToString(dt.Rows[i]["Nombre"]);
                    webForm.Telefono = Convert.ToString(dt.Rows[i]["Telefono"]);
                    webForm.Email = Convert.ToString(dt.Rows[i]["Email"]);
                    webForm.Tema = Convert.ToString(dt.Rows[i]["Tema"]);
                    webForm.Mensaje = Convert.ToString(dt.Rows[i]["Mensaje"]);
                    webFormList.Add(webForm);
                }
            }
            if(webFormList.Count > 0)
            {
                return JsonConvert.SerializeObject(webFormList);
            }
            else
            {
                response.StatusCode = 100;
                response.ErrorMessage = "No data found";
                return JsonConvert.SerializeObject(response);
            }
        }

        [HttpPost]
        [Route("SaveForm")]
        public string SetForm([FromBody] WebForm formData)
        {
            Response response = new Response();

            try
            {
                string nombre = formData.Nombre;
                string telefono = formData.Telefono;
                string email = formData.Email;
                string tema = formData.Tema;
                string mensaje = formData.Mensaje;

                SqlConnection con = new SqlConnection(_config.GetConnectionString("WebFormCon").ToString());
                SqlCommand cmd = new SqlCommand("INSERT INTO FormWeb (Nombre, Telefono, Email, Tema, Mensaje) VALUES (@Nombre, @Telefono, @Email, @Tema, @Mensaje)", con);
                cmd.Parameters.AddWithValue("@Nombre", nombre);
                cmd.Parameters.AddWithValue("@Telefono", telefono);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@Tema", tema);
                cmd.Parameters.AddWithValue("@Mensaje", mensaje);

                con.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                con.Close();

                if (rowsAffected > 0)
                {
                    response.StatusCode = 200;
                    response.ErrorMessage = "Formulario agregado exitosamente";
                    return JsonConvert.SerializeObject(response);
                }
                else
                {
                    response.StatusCode = 100;
                    response.ErrorMessage = "Error al agregar el formulario";
                    return JsonConvert.SerializeObject(response);
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.ErrorMessage = "Error interno del servidor: " + ex.Message;
                return JsonConvert.SerializeObject(response);
            }
        }
    }
}
