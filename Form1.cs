/*
    Autor: Julio Bill Schvenger
    Date 24/08/2020
    Objetivo: Capturar o exception da trigger no C#

--criar a tabela
IF OBJECT_ID ('dbo.tabela_0001') IS NOT NULL
	DROP TABLE dbo.tabela_0001
GO

CREATE TABLE dbo.tabela_0001
	(
	campo1 INT NULL,
	campo2 INT NULL,
	campo3 INT NULL
	)
GO

--criar a trigger
ALTER TRIGGER tI_tabela_0001 ON tabela_0001
FOR INSERT
AS
BEGIN
  DECLARE @ierrno INT,
    @vcerrmsg VARCHAR(255)

  IF EXISTS (
      SELECT 1
      FROM inserted
      WHERE ltrim(campo1) IS NULL
      )
  BEGIN
    SELECT @ierrno = 100001,
      @vcerrmsg = 'Nome da pessoa inválida...............'

    GOTO error
  END

  RETURN

  error:;

  throw @ierrno,
    @vcerrmsg,
    1
END
GO

*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProjetoMessageTrigger
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string msg_except = "";
            string queryString = "insert into tabela_0001 (campo1) values (null)";

            StringBuilder errorMessages = new StringBuilder();
            using (SqlConnection connection1 = new SqlConnection("Data Source=localhost;Initial Catalog=master;Integrated Security=True"))
            {
                SqlCommand command1 = new SqlCommand(queryString, connection1);
                try
                {
                    command1.Connection.Open();
                    command1.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    for (int i = 0; i < ex.Errors.Count; i++)
                    {
                        errorMessages.Append("Index #" + i + "\n" +
                            "Message: " + ex.Errors[i].Message + "\n" +
                            "LineNumber: " + ex.Errors[i].LineNumber + "\n" +
                            "Source: " + ex.Errors[i].Source + "\n" +
                            "Procedure: " + ex.Errors[i].Procedure + "\n");

                        if (ex.Errors[i].Procedure == "tI_tabela_0001")
                        {
                            msg_except =
                                "Mensagem da trigger: " +
                                ex.Errors[i].Message;
                        }
                    }
                    MessageBox.Show(msg_except, "Erro", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                }
            }
        }
    }
}

