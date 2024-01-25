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
    public partial class ActivateAccount : System.Web.UI.Page
    {
        SqlConnection connect = new SqlConnection(ConfigurationManager.ConnectionStrings["conString"].ConnectionString);
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void verifyBtn_Click(object sender, EventArgs e)
        {
            string email = Request.QueryString["email"].ToString();
            string activation_code = Request.QueryString["activation_code"].ToString();

            if(connect.State == ConnectionState.Closed)
            {
                try
                {
                    connect.Open();

                    string checkActivation = "SELECT id FROM users WHERE email=@email AND activation_code=@activation_code AND is_active!=1";

                    using (SqlCommand cmd = new SqlCommand(checkActivation, connect))
                    {
                        cmd.Parameters.AddWithValue("@email", email);
                        cmd.Parameters.AddWithValue("@activation_code", activation_code);

                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataTable table = new DataTable();
                        adapter.Fill(table);

                        if (table.Rows.Count >= 1)
                        {
                            string updateAcc = "UPDATE users SET is_active=1, activation_code=0 WHERE email=@email";
                            connect.Close();
                            connect.Open();
                            SqlCommand cmdUpdate = new SqlCommand(updateAcc, connect);
                            cmdUpdate.Parameters.AddWithValue("@email", email);
                            cmdUpdate.ExecuteNonQuery();

                            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Your Account is verified.')", true);
                        }
                        else
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Your link is expired.')", true);
                        }
                    }
                }
                catch(Exception ex)
                {

                }
                finally
                {
                    connect.Close();
                }
            }
        }
    }
}