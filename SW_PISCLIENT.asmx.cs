using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;


using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Services;

using System.Text.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using RestSharp.Authenticators;
using System.Data;
using System.Globalization;

namespace SW_PISCLIENT
{
    /// <summary>
    /// Descripción breve de SW_PISCLIENT
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // Para permitir que se llame a este servicio web desde un script, usando ASP.NET AJAX, quite la marca de comentario de la línea siguiente. 
    // [System.Web.Script.Services.ScriptService]
    public class SW_PISCLIENT : System.Web.Services.WebService
    {
       //public PISSecurityResponse iobj_response;

        [WebMethod]
        public string HelloWorld()
        {
            return "Hola a todos";
        }

        [WebMethod]
        //public string GetToken()

        public PISSecurityResponse GetToken()  
        {
            string _url = "";
            string _user = "";
            string _password = "";
            string _guid = "";
            int _idAPI = 0;

            PISSecurityRequest lobj_Request = new PISSecurityRequest();
               
            _url = ConfigurationManager.AppSettings["url_pislogin"];
            _user = ConfigurationManager.AppSettings["usuario"];
            _password = ConfigurationManager.AppSettings["password"];
            _guid= ConfigurationManager.AppSettings["guid"];
            if (  int.TryParse(ConfigurationManager.AppSettings["guid"].ToString(), out _idAPI)== false)
            {
                _idAPI = 0;
            }

            lobj_Request.usuario = _user;
            lobj_Request.password = _password;
            lobj_Request.guid = _guid;

            var client = new RestClient(_url);
           
            var request = new RestRequest(_url, Method.Post);

            request.AddHeader("content-type", "application/json");
            //request.AddParameter("application/json", "{ \"grant_type\":\"client_credentials\" }", ParameterType.RequestBody);
            
            string jsonString;
            jsonString = System.Text.Json.JsonSerializer.Serialize(lobj_Request);
            //request.AddParameter("application/json", jsonString, ParameterType.RequestBody);
            request.AddParameter("application/json", jsonString, ParameterType.RequestBody);

            var responseJson = client.Execute(request).Content;

            var token = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseJson)["mensaje"].ToString();
            token = token;

            PISSecurityResponse iobj_response = new PISSecurityResponse();
            iobj_response = new PISSecurityResponse();
            iobj_response = JsonConvert.DeserializeObject<PISSecurityResponse>(responseJson);

            return iobj_response;
         //   return responseJson.ToString();
        }

        
        //////
        ///

        [WebMethod]
        public string GetContainersTypes()
        {
            string _url = "";
            string lstr_temp = "";

            PISSecurityResponse iobj_response = new PISSecurityResponse();
            //lstr_temp = GetToken();
            iobj_response = GetToken();

            //PISSecurityRequest lobj_Request = new PISSecurityRequest();


            _url = ConfigurationManager.AppSettings["url_pistipoContenedor"];

            var client = new RestClient(_url);
    
            //client.Authenticator = new HttpBasicAuthenticator(_user, _password);
            //var request = new RestRequest("api/users/login", Method.POST);
            var request = new RestRequest(_url, Method.Get);

            //request.AddHeader("content-type", "application/json");
            request.AddHeader("authorization", "Bearer " + iobj_response.valor);
            //request.AddParameter("application/json", "{ \"grant_type\":\"client_credentials\" }", ParameterType.RequestBody);

            //  string jsonString;
            //  jsonString = System.Text.Json.JsonSerializer.Serialize(lobj_Request);

            //request.AddParameter("application/json", jsonString, ParameterType.RequestBody);

            //request.AddParameter("application/json", jsonString, ParameterType.RequestBody);

            var responseJson = client.Execute(request).Content;

            //var token = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseJson)["mensaje"].ToString();

            //List<PISTipoContendorResponse> listresult = JsonConvert.DeserializeObject<List<PISTipoContendorResponse>>(responseJson);
            PISTipoContendorResponse objTiposContResponse = JsonConvert.DeserializeObject<PISTipoContendorResponse>(responseJson);
            

            //List<ShippingInfo> shippingInfo = JsonConvert.DeserializeObject<List<ShippingInfo>>(json);

            //token = token;

            //PISSecurityResponse iobj_response = new PISSecurityResponse();
            //iobj_response = new PISSecurityResponse();
            //iobj_response = JsonConvert.DeserializeObject<PISSecurityResponse>(responseJson);


            return responseJson.ToString();
        }

        [WebMethod]
        public string AssingVisitToPIS(long alng_VisitId)
        {
            string _url = "";
            string lstr_temp = "";
            int lint_obj = 0;
            int lint_hasupdate = 0;
            long llong_numeric = 0;
            int lint_isExpiratedDate = 0;
            int lint_isToday = 0;
            int lint_hour = 0;

            string lstr_DateAppoint = "";
            DateTime ldtm_appointment;
            DateTime ldtm_now = DateTime.Now;

            SolicitarCitaRequest lobj_Visita= new SolicitarCitaRequest();
            PISSecurityResponse iobj_sec = new PISSecurityResponse();
            SW_PISSource.SW_PISSource lswref = new SW_PISSource.SW_PISSource();

            DataTable ldtb_Result = new DataTable();// ' la tabla que obtiene el resultado
            ldtb_Result.TableName = "result";
            iobj_sec = GetToken();

     
            // obtener informacion
            try
            {
                ldtb_Result = lswref.GetPISInfo(alng_VisitId);
                lobj_Visita.codigoCita = ldtb_Result.Rows[0]["strVisitCodigo"].ToString();

                lint_obj = 0;
                if (int.TryParse(ldtb_Result.Rows[0]["idTipoOperacion"].ToString(), out lint_obj) == false)
                    lint_obj = 0;

                lobj_Visita.idTipoOperacion = lint_obj;


             
                //--
                lint_obj = 0;
                if (int.TryParse(ldtb_Result.Rows[0]["idTipo"].ToString(), out lint_obj) == false)
                    lint_obj = 0;

                lobj_Visita.idTipo= lint_obj;
                //--
                lint_obj = 0;
                if (int.TryParse(ldtb_Result.Rows[0]["intMotivo"].ToString(), out lint_obj) == false)
                    lint_obj = 0;

                lobj_Visita.idMotivo= lint_obj;

                //--
                lint_obj = 0;
                if (int.TryParse(ldtb_Result.Rows[0]["idRecintoOrigen"].ToString(), out lint_obj) == false)
                    lint_obj = 0;

                lobj_Visita.idRecintoOrigen = lint_obj;

                //--
                lint_obj = 0;
                if (int.TryParse(ldtb_Result.Rows[0]["idRecintoDestino"].ToString(), out lint_obj) == false)
                    lint_obj = 0;

                lobj_Visita.idRecintoDestino= lint_obj;

                //
                lobj_Visita.imo = ldtb_Result.Rows[0]["strimo"].ToString();
                lobj_Visita.nombreBuque = ldtb_Result.Rows[0]["strBuque"].ToString();

                //--
                lint_obj = 0;
                if (int.TryParse(ldtb_Result.Rows[0]["idSolicitante"].ToString(), out lint_obj) == false)
                    lint_obj = 0;

                lobj_Visita.idSolicitante = lint_obj;

                //
                //--
                lint_obj = 0;
                if (int.TryParse(ldtb_Result.Rows[0]["idAgenteAduanal"].ToString(), out lint_obj) == false)
                    lint_obj = 0;

                lobj_Visita.idAgenteAduanal = lint_obj;

                //
                //--
                lint_obj = 0;
                if (int.TryParse(ldtb_Result.Rows[0]["idTipoProducto"].ToString(), out lint_obj) == false)
                    lint_obj = 0;

                lobj_Visita.idTipoProducto= lint_obj;

                //
                //--
                lint_obj = 0;
                if (int.TryParse(ldtb_Result.Rows[0]["idManiobra"].ToString(), out lint_obj) == false)
                    lint_obj = 0;

                lobj_Visita.idManiobra = lint_obj;

                //
                //--
                lint_obj = 0;
                if (int.TryParse(ldtb_Result.Rows[0]["idTipoDespacho"].ToString(), out lint_obj) == false)
                    lint_obj = 0;

                lobj_Visita.idTipoDespacho = lint_obj;

                               
                //--
                lint_obj = 0;
                if (int.TryParse(ldtb_Result.Rows[0]["idEstadoContenedor"].ToString(), out lint_obj) == false)
                    lint_obj = 0;

                lobj_Visita.idEstadoContenedor= lint_obj;

                //
               
                lobj_Visita.fechaInicio = ldtb_Result.Rows[0]["strfechaInicio"].ToString();
                //--
                lint_obj = 0;
                if (int.TryParse(ldtb_Result.Rows[0]["idHoraInicia"].ToString(), out lint_obj) == false)
                    lint_obj = 0;

                lobj_Visita.idHoraInicia= lint_obj;

                // 
                lobj_Visita.fechaFin= ldtb_Result.Rows[0]["strfechaFin"].ToString();
                //--
                lint_obj = 0;
                if (int.TryParse(ldtb_Result.Rows[0]["idHoraFinaliza"].ToString(), out lint_obj) == false)
                    lint_obj = 0;

                lobj_Visita.idHoraFinaliza= lint_obj;

                // 
                //--
                lint_obj = 0;
                if (int.TryParse(ldtb_Result.Rows[0]["inttotalTractos"].ToString(), out lint_obj) == false)
                    lint_obj = 0;

                lobj_Visita.totalTractos= lint_obj;

                // 

                //--
                lint_obj = 0;
                if (int.TryParse(ldtb_Result.Rows[0]["inttotalManiobras"].ToString(), out lint_obj) == false)
                    lint_obj = 0;

                //-- ciclo de recorrido de elementos 
                string lstr_comma = "";
                int lint_count = 0;
                int lint_idx = 0;
                string lstr_BL = "";
                string lstr_Pedimento = "";
                string lstr_idtipocont = "";
                string lstr_DATA = "";

                lobj_Visita.bl = "";
                lobj_Visita.noPedimento = "";
                lobj_Visita.idTipoContenedor = "";
                lobj_Visita.noContenedor = "";
                lobj_Visita.ium = "";

                foreach ( DataRow litemrow in ldtb_Result.Rows)
                {
                    // revisar bl
                    lstr_DATA= litemrow["strBL"].ToString();

                    // si no existe bl agregarlo 
                    if (lobj_Visita.bl.IndexOf(lstr_DATA) < 0)
                    {
                        // si la longitud del campo es mayor a 1 , poner con coma 
                        if (lobj_Visita.bl.Length > 1)
                        {
                            lobj_Visita.bl = lobj_Visita.bl + "," + lstr_DATA;
                        }
                        else
                        {
                           lobj_Visita.bl = lstr_DATA;
                        } // if (lobj_Visita.bl.Length > 1)
                        
                        //lobj_Visita.bl = ldtb_Result.Rows[lint_count]["strBL"].ToString();

                    } //if (lobj_Visita.bl.IndexOf(lstr_DATA) < 0) // si no existe agregarlo


                    // revisar pedimento 
                    lstr_DATA = "";
                    lstr_DATA = litemrow["strPedimento"].ToString();

                    //  si no existe pedimento agregarlo 
                    if (lobj_Visita.noPedimento.IndexOf(lstr_DATA) < 0)
                    {
                        // si la longitud del campo es mayor a 1 , poner con coma 
                        if (lobj_Visita.noPedimento.Length > 1)
                        {
                            lobj_Visita.noPedimento = lobj_Visita.noPedimento + "," + lstr_DATA;
                        }
                        else
                        {
                            lobj_Visita.noPedimento = lstr_DATA;
                        } // if (lobj_Visita.bl.Length > 1)


                    } //if (lobj_Visita.noPedimento.IndexOf(lstr_DATA) < 0)
                    //lobj_Visita.noPedimento = ldtb_Result.Rows[lint_count]["strPedimento"].ToString();

                    // revisar contenedor
                    lstr_DATA = "";
                    lstr_DATA = litemrow["noContenedor"].ToString();
                     // si tiene mas de un caracter
                     if (lobj_Visita.noContenedor.Length > 1)
                     {
                        lobj_Visita.noContenedor = lobj_Visita.noContenedor + "," + lstr_DATA;
                     }
                     else
                     {
                        lobj_Visita.noContenedor = lstr_DATA;
                     } //if (lobj_Visita.noContenedor.Length > 1)

                    //lobj_Visita.noContenedor = ldtb_Result.Rows[lint_count]["noContenedor"].ToString();

                    // revisar tipo de contenedor 
                    lstr_DATA = "";
                    lstr_DATA = litemrow["idTipoContenedor"].ToString();
                    // si tiene mas de un caracter
                    if (lobj_Visita.idTipoContenedor.Length > 1)
                    {
                        lobj_Visita.idTipoContenedor = lobj_Visita.idTipoContenedor + "," + lstr_DATA;
                    }
                    else
                    {
                        lobj_Visita.idTipoContenedor = lstr_DATA;
                    } //if (lobj_Visita.noContenedor.Length > 1)



                    //lobj_Visita.idTipoContenedor = ldtb_Result.Rows[lint_count]["idTipoContenedor"].ToString();


                    
                    // revisar el ium 
                    lstr_DATA = "";
                    lstr_DATA = litemrow["strium"].ToString();
                    // si tiene mas de un caracter
                    if (lobj_Visita.ium.Length > 1)
                    {
                        lobj_Visita.ium = lobj_Visita.ium + "," + lstr_DATA;
                    }
                    else
                    {
                        lobj_Visita.ium = lstr_DATA;
                    } //if (lobj_Visita.noContenedor.Length > 1)
                    //lobj_Visita.ium = ldtb_Result.Rows[0]["strium"].ToString();


                } /// for each 


                lobj_Visita.totalManiobras= lint_obj;


                // 
                clsdistribucion liobjitem = new clsdistribucion();

                liobjitem.fecha = lobj_Visita.fechaInicio;
                liobjitem.idHora = lobj_Visita.idHoraInicia;
                liobjitem.noManiobras = 1;

                lobj_Visita.distribucion = new List<clsdistribucion>();

                lobj_Visita.distribucion.Add(liobjitem);

                /////
                /// ver si hay id de piss
                try
                {
                    lint_obj = 0;
                    if (int.TryParse(ldtb_Result.Rows[0]["PISID"].ToString(), out lint_obj) == false)
                    {
                        lint_obj = 0;
                    }

                    if ( lint_obj >0)
                    {
                        lint_hasupdate = lint_obj;
                    }
                    ///  ldtb_Result = lswref.GetPISInfo(alng_VisitId);
                }
                catch (Exception ex)
                {
                }

                //<<<  verificacion id de piss

              

                


                liobjitem = new clsdistribucion();

                liobjitem.fecha = lobj_Visita.fechaInicio;
                liobjitem.idHora = lobj_Visita.idHoraInicia;
                liobjitem.noManiobras = 1;

                lobj_Visita.distribucion = new List<clsdistribucion>();

                lobj_Visita.distribucion.Add(liobjitem);

                /////////// <<< pruebas
                ///

            } ///   try     {ldtb_Result = lswref.GetPISInfo(alng_VisitId);...........

            catch (Exception ext)
            {
                string lst = ext.Message;
                lst = lst;
                return "";
            }

            string jsonString;
            _url = ConfigurationManager.AppSettings["url_pisSolicitaCita"];
            var request = new RestRequest(_url, Method.Post);
            var client = new RestClient(_url);

            /// si no tiene id de piss que sea nuevo registro
            if ( lint_hasupdate ==0) // si no tiene id de piss
            {

                _url = ConfigurationManager.AppSettings["url_pisSolicitaCita"];

                 client = new RestClient(_url);
                request = new RestRequest(_url, Method.Post);

                //request.AddHeader("content-type", "application/json");
                request.AddHeader("authorization", "Bearer " + iobj_sec.valor);
                //request.AddParameter("application/json", "{ \"grant_type\":\"client_credentials\" }", ParameterType.RequestBody);
                
                ///  
                /////


                jsonString = System.Text.Json.JsonSerializer.Serialize(lobj_Visita);

            }
            else // else tiene id pis
            {

                _url = ConfigurationManager.AppSettings["url_pisupdateA"];
                 client = new RestClient(_url);
                request = new RestRequest(_url, Method.Put);                                
                request.AddHeader("authorization", "Bearer " + iobj_sec.valor);
                UpdateCitaRequestA lobj_updateA = new UpdateCitaRequestA();

                
                lobj_updateA.distribucion = new List<clsdistribucion>();
                lobj_updateA.distribucion = lobj_Visita.distribucion;

                lobj_updateA.codigoCita = lobj_Visita.codigoCita;
                lobj_updateA.idRecinto = lobj_Visita.idRecintoOrigen;
                lobj_updateA.fechaInicio = lobj_Visita.fechaInicio;
                lobj_updateA.idHoraInicia = lobj_Visita.idHoraInicia;
                lobj_updateA.fechaFin = lobj_Visita.fechaFin;
                lobj_updateA.idHoraFinaliza = lobj_Visita.idHoraFinaliza;
                lobj_updateA.totalTractos = lobj_Visita.totalTractos;
                lobj_updateA.totalManiobras = lobj_Visita.totalManiobras;

                jsonString = System.Text.Json.JsonSerializer.Serialize(lobj_updateA);

            } // else tiene id pis

            ///
            //UpdateCitaRequestA
            //request.AddParameter("application/json", jsonString, ParameterType.RequestBody);
            request.AddParameter("application/json", jsonString, ParameterType.RequestBody);


            //  string jsonString;
            //  jsonString = System.Text.Json.JsonSerializer.Serialize(lobj_Request);

            //request.AddParameter("application/json", jsonString, ParameterType.RequestBody);

            //request.AddParameter("application/json", jsonString, ParameterType.RequestBody);

            var responseJson = client.Execute(request).Content;

            //var token = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseJson)["mensaje"].ToString();

            //List<PISTipoContendorResponse> listresult = JsonConvert.DeserializeObject<List<PISTipoContendorResponse>>(responseJson);


            //List<ShippingInfo> shippingInfo = JsonConvert.DeserializeObject<List<ShippingInfo>>(json);

            //token = token;

            //PISSecurityResponse iobj_response = new PISSecurityResponse();
            //iobj_response = new PISSecurityResponse();
            //iobj_response = JsonConvert.DeserializeObject<PISSecurityResponse>(responseJson);


            PISRegisterVResponseInfo objCItaResponse = JsonConvert.DeserializeObject<PISRegisterVResponseInfo>(responseJson);
            try
            {
                string lstridpis = "";
                string lstrMessage = "";
                try
                {
                    objCItaResponse = JsonConvert.DeserializeObject<PISRegisterVResponseInfo>(responseJson);

                    try
                    {
                        lstridpis = objCItaResponse.valor.idpis.ToString();
                    }
                    catch (Exception ex)
                    { }
                    
                    lstrMessage = objCItaResponse.mensaje;
                    lstrMessage = lstrMessage + lstridpis;
                       
                    // si el mensaje es por caducidad
                    if( lstrMessage.IndexOf("ya paso") >=0  )
                    {
                        return ReAppointVisit(alng_VisitId, ldtb_Result, lobj_Visita);
                    }
                    //<< si el mensaje es por caducidad
                }
                catch (Exception ex)
                {
                    lstrMessage = ex.Message;
                }


                //lswref.LogPISInfo(alng_VisitId, objTiposContResponse.mensaje.ToString() + "--" + lstridpis);
                //lswref.LogPISInfo(alng_VisitId, lstrMessage);
                llong_numeric = 0;

                // primero probar con el id pis
                if (lstridpis.Length > 0)
                {
                    if (long.TryParse(lstridpis, out llong_numeric) == false)
                    {
                        llong_numeric = 0;
                    }

                } //if (lstridpis.Length > 0)

                // si el valor numerico sigue siendo 0 probar con el menssaje de texto
                if (llong_numeric == 0)
                {

                    if (long.TryParse(objCItaResponse.mensaje.ToString(), out llong_numeric) == false)
                    {
                        llong_numeric = 0;
                    }
                                       
                } //if (llong_numeric > 0)

                lswref.LogPISInfo(alng_VisitId, lstrMessage, llong_numeric,"");
                return "";
            }
            catch(Exception ex )
            {

            }
            //  return responseJson.ToString();
            lswref.LogPISInfo(alng_VisitId, "intento llamada",0,"");
            return "";
        }


        public string ReAppointVisit(long alng_Visit, DataTable adtb_Result, SolicitarCitaRequest aobj_vist)
        {

            string _url = "";
            string lstr_temp = "";
            int lint_obj = 0;
            string jsonString;

            PISRegisterVResponseInfo lobjResponse = new PISRegisterVResponseInfo();
            PISSecurityResponse iobj_sec = new PISSecurityResponse();
            SW_PISSource.SW_PISSource lswref = new SW_PISSource.SW_PISSource();
                        
            iobj_sec = GetToken();


            // obtener informacion
            try
            {
                aobj_vist.codigoCita = adtb_Result.Rows[0]["strVisitUP"].ToString();

                // preparar, ejecutar
                
                _url = ConfigurationManager.AppSettings["url_pisSolicitaCita"];
                var lrequest = new RestRequest(_url, Method.Post);
                var lclient = new RestClient(_url);
                lrequest.AddHeader("authorization", "Bearer " + iobj_sec.valor);
                jsonString = System.Text.Json.JsonSerializer.Serialize(aobj_vist);

                lrequest.AddParameter("application/json", jsonString, ParameterType.RequestBody);

                var lresponseJson = lclient.Execute(lrequest).Content;

                
                try
                {
                    string lstridpis = "";
                    string lstrMessage = "";
                    try
                    {
                        lobjResponse = JsonConvert.DeserializeObject<PISRegisterVResponseInfo>(lresponseJson);
                        //lobjTiposContResponse = JsonConvert.DeserializeObject<PISRegisterVResponseInfo>(lresponseJson);

                        try
                        {
                            lstridpis = lobjResponse.valor.idpis.ToString();
                        }
                        catch (Exception ex)
                        { }

                        lstrMessage = lobjResponse.mensaje;
                        lstrMessage = lstrMessage + lstridpis;

                    }
                    catch (Exception ex)
                    {
                        lstrMessage = ex.Message;
                    }

                    long llong_numeric = 0;

                    // primero probar con el id pis
                    if (lstridpis.Length > 0)
                    {
                        if (long.TryParse(lstridpis, out llong_numeric) == false)
                        {
                            llong_numeric = 0;
                        }

                    } //if (lstridpis.Length > 0)

                    // si el valor numerico sigue siendo 0 probar con el menssaje de texto
                    if (llong_numeric == 0)
                    {

                        if (long.TryParse(lobjResponse.mensaje.ToString(), out llong_numeric) == false)
                        {
                            llong_numeric = 0;
                        }

                    } //if (llong_numeric > 0)

                    lswref.LogPISInfo( alng_Visit, lstrMessage, llong_numeric, aobj_vist.codigoCita.ToString());
                    return "";
                }
                catch (Exception ex)
                {

                }
                //  return responseJson.ToString();
                lswref.LogPISInfo(alng_Visit, "intento llamada", 0, "");
                return "";
            }
            catch (Exception ex)
            {

            }
            //  return responseJson.ToString();
            lswref.LogPISInfo(alng_Visit, "intento llamada", 0, "");
            return "";
        } // reappoint 

    }


    public class PISSecurityRequest
    {
        public string usuario { get; set; }
        public string password { get; set; }
        public string guid { get; set; }
        public int idAPI { get; set; }

    }

    public class PISSecurityResponse
    {
        public bool error { get; set; }
        public string mensaje { get; set; }  
        public string valor  { get; set; }
  
    }

    public class PISTipoContendorResponse
    {
        public string error { get; set; }
        public string mensaje { get; set; }
        public IList<PISTipoContenedorItem> valor { get; set; }        
    }

     public class PISTipoContenedorItem
    {
       public int id { get; set; }
       public string nombre { get; set; }
       public string nombreCorto { get; set; }
       public bool activo { get; set; }

     }

    public class clsdistribucion
    {
        public  string fecha { get; set; }
        public int idHora { get; set; }
        public int noManiobras { get; set; }
    }

    public class SolicitarCitaRequest
    {
        public string codigoCita { get; set; }
        public int   idTipoOperacion { get; set; }
        public string bl { get; set; }
        public string noPedimento { get; set; }
        public int idTipo { get; set; }
        public int idMotivo { get; set; }
        public int idRecintoOrigen { get; set; }
        public int idRecintoDestino { get; set; }
        public string imo { get; set; }
        public string nombreBuque { get; set; }
        public int idSolicitante { get; set; }
        public int idAgenteAduanal { get; set; }
        public int idTipoProducto { get; set; }
        public int idManiobra { get; set; }
        public int idTipoDespacho { get; set; }
        public string noContenedor { get; set; }
        public string idTipoContenedor { get; set; }
        public int idEstadoContenedor { get; set; }
        public string ium { get; set; }
        public string fechaInicio { get; set; }
        public int idHoraInicia { get; set; }
        public string fechaFin { get; set; }
        public int idHoraFinaliza { get; set; }
        public int totalTractos { get; set; }
        public int totalManiobras { get; set; }
        public IList<clsdistribucion> distribucion { get; set; }            

    }

    public class PISRegisterVResponse
    {
        public bool error { get; set; }
        public string mensaje { get; set; }
        public IList<ClsValor> valor { get; set; }

    }

    public class PISRegisterVResponseInfo
    {
        public bool error { get; set; }
        public string mensaje { get; set; }
        public ClsValor valor { get; set; }

    }

    public class ClsValor
    {
        public int idpis { get; set; }
    }


    public class UpdateCitaRequestA
    {
        public string codigoCita { get; set; }
        public int idRecinto { get; set; }
        public string fechaInicio { get; set; }
        public int idHoraInicia { get; set; }
        public string fechaFin { get; set; }
        public int idHoraFinaliza { get; set; }
        public int totalTractos { get; set; }
        public int totalManiobras { get; set; }
        public IList<clsdistribucion> distribucion { get; set; }

    }

    //error\":false,\"mensaje\":\"\",\"valor\":{\"idpis\":\"48775\"}}"
}
