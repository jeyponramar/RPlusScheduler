using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RplusScheduler
{
    public class TradeIndiaAPIResponse
    {
        public List<TradeIndiaAPILead> RESPONSE;
    }
    public class TradeIndiaAPILead
    {
        public string inquiry_type = "";
        public string sender_co = "";
        public string subject = "";
        public string generated_time = "";
        public string sender_name = "";
        public string receiver_mobile = "";
        public string ago_time = "";
        public string receiver_uid = "";
        public string sender_country = "";
        public string message = "";
        public string month_slot = "";
        public string sender = "";
        public string sender_state = "";
        public string receiver_name = "";
        public string generated_date = "";
        public string sender_uid = "";
        public string sender_mobile = "";
        public string sender_email = "";
        public string source = "";
        public string view_status = "";
        public string generated = "";
        public string receiver_co = "";
        public string sender_city = "";
        public string landline_number = "";
        public string address = "";
        public string rfi_id = "";

        public string product_name = "";
        public string product_id = "";
        public string sender_other_mobiles = "";
        public string website = "";

    }
}
