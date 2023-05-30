using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AltaBancaApi.AltaBancaDB.Models;
using AltaBancaApi.AltaBancaDB;
using Microsoft.IdentityModel.Tokens;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Reflection;

namespace AltaBancaApi.Controllers
{
    [ApiController]
    //[Route("Api/AltaBanca")]
    public class ClientsController : Controller
    {
        private readonly AltabancaContext _dbContext;
        private readonly ILogger _logger;
        public ClientsController(AltabancaContext context, ILogger<ClientsController> logger)
        {
            _dbContext = context;
            _logger = logger;
        }

        /// <summary>
        /// Metodo GetAll Obtiene el listado de todos los clientes
        /// En caso que no encuentre registros retorna un code 404
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("Api/AltaBanca/GetAll")]
        public ActionResult<List<TblClient>> GetAll()
        {
            try
            {
                _logger.LogInformation("Iniciando Metodo..."+ MethodBase.GetCurrentMethod().Name);
                List<TblClient> tblClients = _dbContext.TblClients.ToList();
                if (tblClients.IsNullOrEmpty()) { throw new Exception("No se encontraron registros..."); }
                return tblClients;


            }
            catch (Exception ex)
            {
                _logger.LogError ("Error..." + ex.Message);
                return NotFound(ex.Message);
            }
            finally { _logger.LogInformation("Fin Metodo..." + MethodBase.GetCurrentMethod().Name); }
        }
        /// <summary>
        /// Metodo para consultar clientes por ID, retorna el cliente con el ID ingresado como parametro
        /// Se usa metodo Find para detectar match con el campo seteado como primaryKey
        /// En caso de no encontrar el registro retorna un 404.
        /// </summary>
        /// <param name="idClient"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Api/AltaBanca/Get")]
        public ActionResult<TblClient> Get(int idClient)
        {
            try
            {
                _logger.LogInformation("Iniciando Metodo..." + MethodBase.GetCurrentMethod().Name);
                var client = _dbContext.TblClients.Find(idClient);
                if (client == null) { throw new Exception("No se encontraron registros..."); }
                return client;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error..." + ex.Message);
                return NotFound(ex.Message);
            }
            finally { _logger.LogInformation("Fin Metodo..." + MethodBase.GetCurrentMethod().Name); }

        }

        /// <summary>
        /// Metodo Search para buscar registro por el campo Nombre
        /// Verifica si la cadena es contenida por el registro.
        /// Devuelve una lista de objetos con los resultados encontrados.
        /// Caso contrario devuelve un estatus 404.
        /// </summary>
        /// <param name="NameClient"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Api/AltaBanca/Search")]
        public ActionResult<List<TblClient>> Search(string NameClient)
        {
            try
            {
                _logger.LogInformation("Iniciando Metodo..." + MethodBase.GetCurrentMethod().Name);
                var client = _dbContext.TblClients.Where(ele => ele.Nombres.Contains(NameClient)).ToList();
                if (client.IsNullOrEmpty()) { throw new Exception("No se encontraron Clientes con el nombre: " + NameClient); }
                return client;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error..." + ex.Message);
                return NotFound(ex.Message);
            }
            finally { _logger.LogInformation("Fin Metodo..." + MethodBase.GetCurrentMethod().Name); }
        }
        /// <summary>
        /// Metodo Recibe Objeto con formato Client
        /// El proceso realiza un ingreso de nuevo registro, se usa metodos de EntityFramework ADD y SaveChanges
        /// Unica validación del campo mail por expresiones regulares El mismo acepta cadena con existencia de "@" y "." Ej-Valido->> Test@hola.com
        /// Caso Exitoso retorna un estatus 201 created con el objeto creado de lo contrario un estatus 400 badrequest indicando el mensaje del error
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Api/AltaBanca/Insert")]
        public ActionResult<TblClient> Insert(TblClient client)
        {
            try
            {
                _logger.LogInformation("Iniciando Metodo..." + MethodBase.GetCurrentMethod().Name);
                TblClient newClient = _dbContext.TblClients.Add(client).Entity;

                bool ValidEmail = Regex.IsMatch(client.Email, "^[^@\\s]+@[^@\\s]+\\.[^@\\s]+$");
                if (!ValidEmail) throw new Exception("Error al insertar el registro, campo Email: [" + client.Email + "] NO VALIDO");
                _dbContext.SaveChanges();
                _logger.LogInformation("Se actualizaron Cambios DB... Metodo..." + MethodBase.GetCurrentMethod().Name);

                return Created("Api/AltaBanca/Insert", newClient);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error..." + ex.Message);
                return BadRequest(ex.Message);
            }
            finally { _logger.LogInformation("Fin Metodo..." + MethodBase.GetCurrentMethod().Name); }

        }
        /// <summary>
        /// Metodo para actualizar campos de un registro.
        /// Se obtiene como parametro el ID y El Objeto a reemplazar
        /// Se verifica que el Id sea correspondiente al objeto a reemplazar, caso contrario retorna un 400 badrequest
        /// Se verifica la existencia del Id a buscar y reemplazar, de no encontrarlo retorna un 400
        /// Se obtiene las propiedades de los objetos a reemplazar.
        /// se recorre por cada propiedad y se le asigna el valor nuevo ingresado
        /// Se guarda los cambios y retorna un 200 con el objeto modificado como respuesta
        /// </summary>
        /// <param name="id"></param>
        /// <param name="UpdatedClient"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("Api/AltaBanca/Update")]
        public ActionResult<TblClient> Update(int id,TblClient UpdatedClient)
        {
            try
            {
                _logger.LogInformation("Iniciando Metodo..." + MethodBase.GetCurrentMethod().Name);
                var client = _dbContext.TblClients.Find(id);
                if (id != UpdatedClient.Id) throw new Exception("Error id ingresado no coincide con la informacion del cliente a modificar ");
                if (client == null) throw new Exception("No se encontraron registros para el id:" + id);

                PropertyInfo[] propertiesClient = client.GetType().GetProperties();
                PropertyInfo[] propertiesUpdated = UpdatedClient.GetType().GetProperties();
                foreach (PropertyInfo propertyClient in propertiesClient)
                {
                    foreach (PropertyInfo propertyUpdated in propertiesUpdated)
                    {
                        string asd = propertyClient.Name;
                        if (propertyClient.Name == propertyUpdated.Name)
                        {
                            propertyClient.SetValue(client, propertyUpdated.GetValue(UpdatedClient));
                        }
                    }

                }
                _dbContext.SaveChanges();
                _logger.LogInformation("Se actualizaron Cambios DB... Metodo..." + MethodBase.GetCurrentMethod().Name);
                return Ok(client);
            }
            
            catch (Exception ex)
            {
                _logger.LogError("Error..." + ex.Message);
                return BadRequest(ex.Message);
            }
            finally { _logger.LogInformation("Fin Metodo..." + MethodBase.GetCurrentMethod().Name); }
        }
        /// <summary>
        /// Metodo encargado de eliminar registros
        /// Recibe como parametro el ID del registro que se desea eliminar
        /// procede a validar la existencia del registro
        /// Caso OK elimina Registro y retorna el objeto eliminado
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("Api/AltaBanca/Delete")]
        public ActionResult<TblClient> Delete(int id)
        {
            try
            {
                _logger.LogInformation("Iniciando Metodo..." + MethodBase.GetCurrentMethod().Name);
                var client = _dbContext.TblClients.Find(id);
                if (client == null) throw new Exception("No se encontraron registros para el id:" + id);

                _dbContext.TblClients.Remove(client);
                _dbContext.SaveChanges();
                _logger.LogInformation("Se actualizaron Cambios DB... Metodo..." + MethodBase.GetCurrentMethod().Name);
                return Ok(client);
            }

            catch (Exception ex)
            {
                _logger.LogError("Error..." + ex.Message);
                return BadRequest(ex.Message);
            }
            finally { _logger.LogInformation("Fin Metodo..." + MethodBase.GetCurrentMethod().Name); }
        }

    }
}
