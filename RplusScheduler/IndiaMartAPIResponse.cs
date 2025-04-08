using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RplusScheduler
{
    public class IndiaMartAPIResponse
    {
        public string CODE = "";
        public string STATUS = "";
        public string MESSAGE = "";
        public int TOTAL_RECORDS = 0;
        public List<IndiaMartAPILead> RESPONSE;
        /*
        public string QUERY_ID = "";
        public string QTYPE = "";
        public string SENDERNAME = "";
        public string SENDEREMAIL = "";
        public string SUBJECT = "";
        public DateTime DATE_TIME_RE;
        public string GLUSR_USR_COMPANYNAME = "";
        public string MOB = "";
        public string ENQ_MESSAGE = "";
        public string ENQ_ADDRESS = "";
        public string ENQ_CITY = "";
        public string ENQ_STATE = "";
        public string PRODUCT_NAME = "";
        public string PHONE = "";
        */
    }
    public class IndiaMartAPILead
    {
        public string UNIQUE_QUERY_ID = "";
        public string QUERY_TYPE = "";
        public string QUERY_TIME = "";
        public string SENDER_NAME = "";
        public string SENDER_MOBILE = "";
        public string SENDER_EMAIL = "";
        public string SUBJECT = "";
        public string SENDER_COMPANY = "";
        public string SENDER_ADDRESS = "";
        public string SENDER_CITY = "";
        public string SENDER_STATE = "";
        public string SENDER_COUNTRY_ISO = "";
        public string SENDER_MOBILE_ALT = "";
        public string SENDER_PHONE = "";
        public string SENDER_PHONE_ALT = "";
        public string SENDER_EMAIL_ALT = "";
        public string QUERY_PRODUCT_NAME = "";
        public string QUERY_MESSAGE = "";
        public string CALL_DURATION = "";
        public string RECEIVER_MOBILE = "";
    }
}
