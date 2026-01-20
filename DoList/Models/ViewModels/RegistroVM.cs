namespace DoList.Models.ViewModels
{
 
        public class RegistroVM
        {
        //Usado para registrar
            public CUsuario Usuario { get; set; }
            
        //Usado para poner el mensaje de error
            public string? MensajeError { get; set; }
        }
    
}
