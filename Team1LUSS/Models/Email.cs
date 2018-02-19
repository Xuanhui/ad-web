using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Team1LUSS.Models
{
    public class Email
    {
        String body;
        String subject;
        String emailaddress;

        public String Emailaddress
        {
            get
            {
                return this.emailaddress;
            }
            set
            {
                this.emailaddress = value;
            }
        }
        public String Body
        {
            get
            {
                return this.body;
            }
            set
            {
                this.body = value;
            }
        }
        public String Subject
        {
            get
            {
                return this.subject;
            }
            set
            {
                this.subject = value;
            }
        }
        public Email(String emailaddress, String subjectText, String bodyText)
        {
            this.emailaddress = emailaddress;
            this.subject = subjectText;
            this.body = bodyText;            
        }
    }
}