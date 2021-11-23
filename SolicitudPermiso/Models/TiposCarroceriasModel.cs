using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SolicitudPermiso.Models
{
    public class TiposCarroceriasModel
    {
        public int id_tipo_carroceria { get; set; }

        public int? grupo { get; set; }

        public string nombre { get; set; }

        public int activo { get; set; }

    }
}
