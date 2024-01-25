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
    public partial class RegisterationForm : System.Web.UI.Page
    {
        SqlConnection connect = new SqlConnection(ConfigurationManager.ConnectionStrings["conString"].ConnectionString);
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void signupBtn_Click(object sender, EventArgs e)
        {
            string username = Request.Form["username"];
            string email = Request.Form["email"];
            string password = Request.Form["password"];

            if(String.IsNullOrEmpty(email) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                // message box
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('All fields are required to be filled.", true);
            } else
            {
                if (connect.State == ConnectionState.Closed)
                {
                    try
                    {
                        connect.Open();

                        string selectEmail = "SELECT * FROM users WHERE email = @email";

                        using(SqlCommand cmd = new SqlCommand(selectEmail, connect))
                        {
                            cmd.Parameters.AddWithValue("@email", email);

                            SqlDataAdapter adatper = new SqlDataAdapter(cmd);
                            DataTable table = new DataTable();
                            adatper.Fill(table);

                            if(table.Rows.Count > 0)
                            {
                                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('This email is already exist.')", true);
                            }
                            else if (password.Length < 8)
                            {
                                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Invalid password, at least 8 characters are needed.')", true);
                            }
                            else
                            {
                                DateTime today = DateTime.Today;
                                string insertData = "INSERT INTO users (username, email, pass, date, activation_code, is_active) VALUES(@username, @email, @pass, @date, @activation_code, @is_active)";

                                using (SqlCommand insertD = new SqlCommand(insertData, connect))
                                {
                                    Random random = new Random();
                                    int myRandom = random.Next(1000000, 9999999);
                                    string activation_code = myRandom.ToString();

                                    insertD.Parameters.AddWithValue("@username", username);
                                    insertD.Parameters.AddWithValue("@email", email);
                                    insertD.Parameters.AddWithValue("@pass", password);
                                    insertD.Parameters.AddWithValue("@date", today);
                                    insertD.Parameters.AddWithValue("@activation_code", activation_code);
                                    insertD.Parameters.AddWithValue("@is_active", 0);

                                    insertD.ExecuteNonQuery();

                                    MailMessage mail = new MailMessage();
                                    mail.To.Add(email);
                                    mail.From = new MailAddress("johndevis112@outlook.com");
                                    mail.Subject = "Thank you for joining us.";

                                    string mailBody = "";
                                    mailBody += "<h1>Hello " + username + "</h1>";
                                    mailBody += "Click Below link to activate your account.</br>";
                                    mailBody += "<p><a href='" + "https://localhost:44319/activate?activation_code=" + activation_code + "&email=" + email + "'>Click here to activate.</a></p>";
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

                                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('You registered successfully. Please check your email Inbox/spam folder for activation.')", true);

                                    Response.Redirect("/login");
                                }
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
}