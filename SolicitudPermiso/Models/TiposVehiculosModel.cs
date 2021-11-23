using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SolicitudPermiso.Models
{
    public class TiposVehiculosModel
    {
        public int id_codigo_tipo_vehiculo { get; set; }

        public int? grupo { get; set; }

        public string nombre { get; set; }

        public int activo { get; set; }

    }
}
