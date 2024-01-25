using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;

namespace TaskAuth
{
    public partial class LoginForm : System.Web.UI.Page
    {
        SqlConnection connect = new SqlConnection(ConfigurationManager.ConnectionStrings["conString"].ConnectionString);
        protected void Page_Load(object sender, EventArgs e)
        {
            Session.Clear();
        }

        protected void loginBtn_Click(object sender, EventArgs e)
        {
            string email = Request.Form["email"];
            string password = Request.Form["password"];

            //Check if email or password is null or empty
            if(connect.State == ConnectionState.Closed)
            {
                try
                {
                    connect.Open();

                    string selectData = "SELECT * FROM users WHERE email = @email AND pass = @pass";

                    using (SqlCommand cmd = new SqlCommand(selectData, connect))
                    {
                        cmd.Parameters.AddWithValue("@email", email);
                        cmd.Parameters.AddWithValue("@pass", password);

                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataTable table = new DataTable();
                        adapter.Fill(table);

                        if(table.Rows.Count >= 1)
                        {
                            int is_active = int.Parse(table.Rows[0].ItemArray[6].ToString());
                            if(is_active == 0)
                            {
                                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Your account is not verified.')", true);
                            }
                            else 
                            {
                                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Login succssfully.')", true);
                                string username = table.Rows[0].ItemArray[1].ToString();
                                Session["email"] = email;
                                Session["username"] = username;
                                Response.Redirect("/home");
                            }
                        }
                        else
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Incorrect Email or Password.')", true);
                        }
                    }
                }
                catch (Exception ex)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Connection failed.')", true);

                }
                finally
                {

                }
            }
        }
    }
}