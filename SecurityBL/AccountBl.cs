using BLComponents;
using DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurityBL
{
    [AspectLogger()]
    public class AccountBl
    {
        public dynamic GetUserByName(dynamic data)
        {
            TransactionResult result = new TransactionResult();
            DataAccessObject ourDB = new DataAccessObject("DBModelsAWS");
            var a = ourDB.ExecuteReader(
           @"SELECT p.id, p.nombres , p.clave, p.documento, p.tipoafiliacion FROM
            agendamiento.afiliado p where p.email=@Email and p.clave=@Clave;", data,false );           
            result.DataObject = a;
            result.Message = "asdasdasd";
            return result;
        }

        public dynamic GetDetalleCita(dynamic data)
        {
            TransactionResult result = new TransactionResult();
            DataAccessObject ourDB = new DataAccessObject("DBModelsAWS");
            var a = ourDB.ExecuteReader(
           @"SELECT a.fecha, m.nombres as medico, con.nombre as consultorio, c.estado, esm.nombre as esm, e.nombres as especialidad, a.id as cita, c.motivocancelacion as motivo, c.justificacioncancelacion as justificacion
              FROM agendamiento.citas c
              INNER JOIN agendamiento.agenda a on (c.idagenda = a.id)
              INNER JOIN agendamiento.medico m on (a.idmedico=m.id)
              INNER JOIN agendamiento.consultorio con on (a.idconsultorio = con.id)
              INNER JOIN agendamiento.esm on (a.idesm = esm.id)
              INNER JOIN agendamiento.especialidad e on (a.idespecialidad = e.id)
              where c.id=@Cita;", data, false);
            result.DataObject = a;
            result.Message = "Detalle de cita";
            return result;
        }

        public dynamic GetCitasAsignadas(dynamic data)
        {
            TransactionResult result = new TransactionResult();
            DataAccessObject ourDB = new DataAccessObject("DBModelsAWS");
            var a = ourDB.ExecuteReader(
           @"SELECT c.id, m.nombres as medico, a.fecha ,  con.nombre as consultorio, e.nombre as esm, esp.nombres as especialidad  FROM agendamiento.citas c
                inner join agendamiento.agenda a on c.idagenda=a.id
                inner join agendamiento.medico m on a.idmedico=m.id
                inner join agendamiento.afiliado afi on c.idafiliado=afi.id
                inner join agendamiento.consultorio con on a.idconsultorio=con.id
                inner join agendamiento.esm e on (a.idesm=e.id)
                inner join agendamiento.especialidad esp on (a.idespecialidad=esp.id)
                where c.idafiliado = @Idafiliado;", data, true);
            result.DataObject = a;
            result.Message = "asdasdasd";
            return result;
        }


        public dynamic GetCitasDisponibles(dynamic data)
        {
            TransactionResult result = new TransactionResult();
            DataAccessObject ourDB = new DataAccessObject("DBModelsAWS");
            var a = ourDB.ExecuteReader(
           @"SELECT a.id as idAgenda, a.estado, m.nombres as medico, a.fecha, con.nombre as consultorio, e.nombre as esm, esp.nombres as especialidad 
             FROM agendamiento.agenda a               
                inner join agendamiento.medico m on a.idmedico=m.id
                inner join agendamiento.consultorio con on a.idconsultorio=con.id
                inner join agendamiento.esm e on (a.idesm=e.id)
                inner join agendamiento.especialidad esp on (a.idespecialidad=esp.id)
                where a.estado='Disponible' and e.id=@Esm and m.id=@Profesional and esp.id=@Especialidad;",data, true);
            result.DataObject = a;
            result.Message = "Citas disponibles";
            return result;
        }

        public dynamic GetCitasDisponiblesAutorizadas(dynamic data)
        {
            TransactionResult result = new TransactionResult();
            DataAccessObject ourDB = new DataAccessObject("DBModelsAWS");
            var a = ourDB.ExecuteReader(
           @"SELECT a.id as idAgenda, a.estado, m.nombres as medico, a.fecha ,  con.nombre as consultorio, e.nombre as esm, esp.nombres as especialidad 
            FROM agendamiento.agenda a               
                inner join agendamiento.medico m on a.idmedico=m.id
                inner join agendamiento.consultorio con on a.idconsultorio=con.id
                inner join agendamiento.esm e on (a.idesm=e.id)
                inner join agendamiento.especialidad esp on (a.idespecialidad=esp.id)
                inner join agendamiento.autorizacion aut on (e.id = aut.idesm and aut.idespecialidad=esp.id and aut.id=@Autorizacion)
                where a.estado='Disponible';", data, true);
            result.DataObject = a;
            result.Message = "Citas disponibles";
            return result;
        }
    }
}
