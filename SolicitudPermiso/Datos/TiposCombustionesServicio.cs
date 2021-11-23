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
    public class TiposCombustionesServicio
    {
        private readonly ProveedorConexion _proveedor;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<TiposCombustionesServicio> _log;

        public TiposCombustionesServicio(ProveedorConexion proveedor, ILogger<TiposCombustionesServicio> log, IHttpContextAccessor httpContextAccessor)
        {
            _proveedor = proveedor;
            _httpContextAccessor = httpContextAccessor;
            _log = log;
        }

        public TiposCombustionesModel SelPorId(int id)
        {
            string sp = "PA_Sel_Tipos_Combustiones_Por_Id";

            _log.LogInformation($"Ejecutando {sp} {id}");

            try
            {
                using var db = _proveedor.GetDbConnection();
                db.Open();
                var values = new { id };
                var respuesta = db.QueryFirst<TiposCombustionesModel>(sp, values, null, null, CommandType.StoredProcedure);

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
