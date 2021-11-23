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
    public class TiposEquipamientosServicio
    {
        private readonly ProveedorConexion _proveedor;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<TiposEquipamientosServicio> _log;

        public TiposEquipamientosServicio(ProveedorConexion proveedor, ILogger<TiposEquipamientosServicio> log, IHttpContextAccessor httpContextAccessor)
        {
            _proveedor = proveedor;
            _httpContextAccessor = httpContextAccessor;
            _log = log;
        }

        public TiposEquipamientosModel SelPorId(int id)
        {
            string sp = "PA_Sel_Tipos_Equipamientos_Por_Id";

            _log.LogInformation($"Ejecutando {sp} {id}");

            try
            {
                using var db = _proveedor.GetDbConnection();
                db.Open();
                var values = new { id };
                var respuesta = db.QueryFirst<TiposEquipamientosModel>(sp, values, null, null, CommandType.StoredProcedure);

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
