using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Net.Mail;
using System.Configuration;

namespace TaskAuth
{
    public partial class ForgetPasswordForm : System.Web.UI.Page
    {
        SqlConnection connect = new SqlConnection(ConfigurationManager.ConnectionStrings["conString"].ConnectionString);
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void submitBtn_Click(object sender, EventArgs e)
        {
            string email = Request.Form["email"];
            
            if(connect.State == ConnectionState.Closed)
            {
                try
                {
                    connect.Open();

                    string selectData = "SELECT id FROM users WHERE email=@email AND is_active=1";

                    using (SqlCommand cmd = new SqlCommand(selectData, connect))
                    {
                        cmd.Parameters.AddWithValue("@email", email);

                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataTable table = new DataTable();
                        adapter.Fill(table);

                        if(table.Rows.Count >= 1)
                        {
                            Random random = new Random();
                            int myRandom = random.Next(1000000, 9999999);
                            string forgot_otp = myRandom.ToString();
                            
                            //insert forgot_otp into userinfo
                            connect.Close();
                            connect.Open();
                            string updateAcc = "UPDATE users SET forgot_otp=" + forgot_otp + " WHERE email=@email";
                            SqlCommand cmdUpdate = new SqlCommand(updateAcc, connect);
                            cmdUpdate.Parameters.AddWithValue("@email", email);
                            cmdUpdate.ExecuteNonQuery();

                            MailMessage mail = new MailMessage();
                            mail.To.Add(email);
                            mail.From = new MailAddress("johndevis112@outlook.com");
                            mail.Subject = "Did you forget your password?";

                            string mailBody = "";
                            mailBody += "<h1>Hello " + User + "</h1>";
                            mailBody += "Click Below link to reset your password.</br>";
                            mailBody += "<p><a href='" + "https://localhost:44319/resetpwd?forgot_otp=" + forgot_otp + "&email=" + email + "'>Click here to reset your password.</a></p>";
                            mailBody += "Thank you...";

                            mail.Body = mailBody;
                            mail.IsBodyHtml = true;

                            SmtpClient smtp = new SmtpClient();
                            smtp.Port = 587;
                            smtp.EnableSsl = true;
                            smtp.UseDefaultCredentials = false;
                            smtp.Host = "smtp.office365.com";
                            smtp.Credentials = new NetworkCredential("johndevis112@outlook.com", "Richardivory112!!");
                            smtp.Send(mail);

                            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Please check your email inbox/spam folder to reset your password.')", true);
                        }
                        else
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('You are not associated with us.')", true);
                        }
                    }
                }
                catch (Exception ex)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Connection failed.')", true);
                }
                finally
                {
                    connect.Close();
                }
            }
        }
    }
}