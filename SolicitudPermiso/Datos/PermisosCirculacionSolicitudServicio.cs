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
    public class PermisosCirculacionSolicitudServicio
    {
        private readonly ProveedorConexion _proveedor;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<PermisosCirculacionServicios> _log;

        public PermisosCirculacionSolicitudServicio(ProveedorConexion proveedor, ILogger<PermisosCirculacionServicios> log, IHttpContextAccessor httpContextAccessor)
        {
            _proveedor = proveedor;
            _httpContextAccessor = httpContextAccessor;
            _log = log;
        }

        public PermisoCirculacionSolicitudModel SelPorId(int id_permisosCirculacion_solicitud)
        {
            string sp = "PA_Sel_Permisos_Circulacion_Solicitud_Por_Id";

            _log.LogInformation($"Ejecutando {sp} {id_permisosCirculacion_solicitud}");

            try
            {
                using var db = _proveedor.GetDbConnection();
                db.Open();
                var values = new { id_permisosCirculacion_solicitud };
                var respuesta = db.QueryFirst<PermisoCirculacionSolicitudModel>(sp, values, null, null, CommandType.StoredProcedure);

                return respuesta;
            }
            catch (Exception e)
            {
                _log.LogError($"{e}");
                return null;
            }

        }

        public string Ins(PermisoCirculacionSolicitudModel solicitud)
        {
            string sp = "PA_Ins_Permiso_Circulacion";

            try
            {
                using var db = _proveedor.GetDbConnection();
                db.Open();
                var values = valuesIns(solicitud);

                _log.LogInformation($"Ejecutando {sp} {values}");

                var respuesta = db.QueryFirst<string>(sp, values, null, null, CommandType.StoredProcedure);

                return respuesta;
            }
            catch (Exception e)
            {
                _log.LogError($"{e}");
                return null;
            }

        }

        private static object valuesIns(PermisoCirculacionSolicitudModel solicitud)
        {
            return new
            {
                solicitud.id_permisosCirculacion,
                solicitud.tipo,
                solicitud.rut,
                solicitud.patente,
                solicitud.monto_neto,
                solicitud.fecha_vencimiento,
                solicitud.digito_verificador,
                solicitud.Marca,
                solicitud.Modelo,
                solicitud.ano,
                solicitud.tipo_vehiculo,
                solicitud.motor,
                solicitud.color,
                solicitud.chasis,
                solicitud.n_puertas,
                solicitud.n_asiento,
                solicitud.tara,
                solicitud.codigo_sii,
                solicitud.tasacion,
                solicitud.cilindrada,
                solicitud.combustible,
                solicitud.transmision,
                solicitud.equipamiento,
                solicitud.nombre_propietario,
                solicitud.domicilio_propietario,
                solicitud.comuna_propietario,
                solicitud.telefono_propietario,
                solicitud.pago_total,
                solicitud.sello_verde,
                solicitud.comuna_anterior,
                solicitud.zona_franca,
                solicitud.carga,
                solicitud.multa,
                solicitud.ipc,
                solicitud.valor_total,
                solicitud.interes,
                solicitud.total_neto,
                solicitud.total_pagado,
                solicitud.cuota,
                solicitud.empresa,
                solicitud.usuario
            };
        }

    }
}
