using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SolicitudPermiso.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;


namespace SolicitudPermiso.Datos
{
    public class TiposCarroceriasServicio
    {
        private readonly ProveedorConexion _proveedor;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<TiposCarroceriasServicio> _log;

        public TiposCarroceriasServicio(ProveedorConexion proveedor, ILogger<TiposCarroceriasServicio> log, IHttpContextAccessor httpContextAccessor)
        {
            _proveedor = proveedor;
            _httpContextAccessor = httpContextAccessor;
            _log = log;
        }

        public TiposCarroceriasModel SelPorId(int id)
        {
            string sp = "PA_Sel_Tipos_Carrocerias_Por_Id";

            _log.LogInformation($"Ejecutando {sp} {id}");

            try
            {
                using var db = _proveedor.GetDbConnection();
                db.Open();
                var values = new { id };
                var respuesta = db.QueryFirst<TiposCarroceriasModel>(sp, values, null, null, CommandType.StoredProcedure);

                return respuesta;
            }
            catch (Exception e)
            {
                _log.LogError($"{e}");
                return null;
            }

        }

    }
}
