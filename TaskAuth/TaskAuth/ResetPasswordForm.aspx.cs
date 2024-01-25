using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace TaskAuth
{
    public partial class ResetPasswordForm : System.Web.UI.Page
    {
        SqlConnection connect = new SqlConnection(ConfigurationManager.ConnectionStrings["conString"].ConnectionString);
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void resetPasswordBtn_Click(object sender, EventArgs e)
        {
            string password = Request.Form["password"];
            string cpassword = Request.Form["cpassword"];
            string email = Request.QueryString["email"];
            string forgot_otp = Request.QueryString["forgot_otp"];
            if (password != cpassword)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Please confirm your password.')", true);
            }
            else if (password.Length < 8)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Invalid password, at least 8 characters.')", true);
            }
            else if (connect.State == ConnectionState.Closed)
            {
                try
                {
                    connect.Open();

                    string checkActivation = "SELECT id FROM users WHERE email=@email AND forgot_otp=@forgot_otp AND is_active=1";

                    using (SqlCommand cmd = new SqlCommand(checkActivation, connect))
                    {
                        cmd.Parameters.AddWithValue("@email", email);
                        cmd.Parameters.AddWithValue("@forgot_otp", forgot_otp);

                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataTable table = new DataTable();
                        adapter.Fill(table);

                        if (table.Rows.Count >= 1)
                        {
                            string updateAcc = "UPDATE users SET pass=" + password + ", forgot_otp=0 WHERE email=@email";
                            connect.Close();
                            connect.Open();
                            SqlCommand cmdUpdate = new SqlCommand(updateAcc, connect);
                            cmdUpdate.Parameters.AddWithValue("@email", email);
                            cmdUpdate.ExecuteNonQuery();

                            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Your password is updated.')", true);
                        }
                        else
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Your link is expired.')", true);
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