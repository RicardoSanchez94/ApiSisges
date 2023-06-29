using System.Net.Mail;

namespace ServicioApi.Model.Clases
{
    public class CredencialesCorreo
    {


        public string Email { get; set; }
        public string Pass { get; set; }
        public bool EnableSsl { get; set; }
        public int Timeout { get; set; }
        public SmtpDeliveryMethod DeliveryMethod { get; set; }
        public bool UseDefaultCredentials { get; set; }

        public CredencialesCorreo()
        {
            this.DeliveryMethod = SmtpDeliveryMethod.Network;
        }
    }
}
