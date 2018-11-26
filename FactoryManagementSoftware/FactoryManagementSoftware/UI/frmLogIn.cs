﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FactoryManagementSoftware.UI
{
    public partial class frmLogIn : Form
    {
        public frmLogIn()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private bool validation()
        {
            bool result = true;

            if(string.IsNullOrEmpty(textBox1.Text))
            {
                errorProvider1.SetError(textBox1, "Username required");
                result = false;
            }

            if (string.IsNullOrEmpty(textBox2.Text))
            {
                errorProvider2.SetError(textBox2, "Password required");
                result = false;
            }

            return result;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            string username = textBox1.Text;
            string password = textBox2.Text;

            if (validation())
            {
                if (username.Equals("junong") && password.Equals("123456"))
                {
                    MainDashboard frm = new MainDashboard();
                    frm.Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("username or password invalid. please try again.");
                } 
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if(!string.IsNullOrEmpty(textBox1.Text))
            {
                errorProvider1.Clear();
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textBox2.Text))
            {
                errorProvider2.Clear();
            }
        }

        private void frmLogIn_KeyDown(object sender, KeyEventArgs e)
        {
            MessageBox.Show("haha");
            if (e.KeyCode == Keys.Enter)
            {
                MessageBox.Show("enter");
            }
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button1_Click(sender, e);
            }
        }

        private void frmLogIn_Load(object sender, EventArgs e)
        {
            //checking whether database exist or not
            if (!CheckDatabaseExist())
            {
                
                MessageBox.Show("Database not exist");
                //GenerateDatabase();
            }
        }

        private bool CheckDatabaseExist()
        {
            bool result;
            string myconnstrng = ConfigurationManager.ConnectionStrings["connstrng"].ConnectionString;
            //Sql connection for user defined database
            SqlConnection conn = new SqlConnection(myconnstrng);

            try
            {
                conn.Open();
                result = true;             
            }
            catch
            {
                result = false;
            }

            conn.Close();
            return result;
        }

        private void GenerateDatabase()
        {
            List<string> cmds = new List<string>();
            if (File.Exists(Application.StartupPath + "\\script.sql"))
            {
                TextReader tr = new StreamReader(Application.StartupPath + "\\script.sql");
                string line = "";
                string cmd = "";
                while((line = tr.ReadLine()) != null)
                {
                    if(line.Trim().ToUpper() == "GO")
                    {
                        cmds.Add(cmd);
                        cmd = "";
                    }
                    else
                    {
                        cmd += line + "\r\n";
                    }
                }

                if(cmd.Length > 0)
                {
                    cmds.Add(cmd);
                    cmd = "";
                }

                tr.Close();
            }

            if(cmds.Count > 0)
            {
                SqlCommand command = new SqlCommand();

                command.Connection = new SqlConnection(@"Data Source=
           .\SQLEXPRESS;Initial Catalog=Factory;Integrated Security=True");

                command.CommandType = System.Data.CommandType.Text;
                command.Connection.Open();
                
                for(int i = 0; i < cmds.Count; i++)
                {
                    command.CommandText = cmds[i];
                    command.ExecuteNonQuery();
                }
            }
        }

    }
    
}
