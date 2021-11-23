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
    public class PermisosCirculacionServicios
    {
        private readonly ProveedorConexion _proveedor;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<PermisosCirculacionServicios> _log;

        public PermisosCirculacionServicios(ProveedorConexion proveedor, ILogger<PermisosCirculacionServicios> log, IHttpContextAccessor httpContextAccessor)
        {
            _proveedor = proveedor;
            _httpContextAccessor = httpContextAccessor;
            _log = log;
        }

        public PermisoCirculacionModel SelPorId(int id_permisosCirculacion)
        {
            string sp = "PA_Sel_Permisos_Circulacion_Por_Id";

            _log.LogInformation($"Ejecutando {sp} {id_permisosCirculacion}");

            try
            {
                using var db = _proveedor.GetDbConnection();
                db.Open();
                var values = new { id_permisosCirculacion };
                var respuesta = db.QueryFirst<PermisoCirculacionModel>(sp, values, null, null, CommandType.StoredProcedure);

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
