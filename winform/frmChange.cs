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

namespace proto
{
    public partial class frmChange : Form
    {
        string beforeID, beforePWD, changeID, changePWD;
        string db = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Owner\Desktop\proto_\test.mdf;Integrated Security=True;Connect Timeout=30";
        SqlConnection con = new SqlConnection();
        SqlCommand cmd = new SqlCommand();
        SqlDataReader sr;

        public frmChange()
        {
            InitializeComponent();
            con.ConnectionString = db;
            cmd.Connection = con;
        }

        private void btnChange_Click(object sender, EventArgs e)
        {
            //db사용을 어떻게 해야하는지 생각해봐야할 것 같습니다
            if(EmptyCheck())
            {
                SqlDataAdapter sda = new SqlDataAdapter($"Select count(*) from adminInfo where Id='{tbBeforeID.Text}'and Password='{tbBeforePWD.Text}'", con);
                
                 DataTable newtable = new DataTable();
                 sda.Fill(newtable);

                 if(newtable.Rows[0][0].ToString() == "1")
                 {
                     beforeID = tbBeforeID.Text;    beforePWD = tbBeforePWD.Text;
                     changeID = tbChangeID.Text;    changePWD = tbChangePWD.Text;

                     con.Open();

                    if (changeID != beforeID && changePWD != beforePWD)
                    {
                        string sql = $"UPDATE adminInfo SET Id='{changeID}', Password='{changePWD}' ";
                        cmd.CommandText = sql;
                        cmd.ExecuteNonQuery();
                        //SqlCommand cmd = new SqlCommand(sql, con);

                         MessageBox.Show("정보 수정 완료");
                         this.DialogResult = DialogResult.OK;
                         this.Close();
                     }

                   else if (tbBeforeID.Text == admin.id && tbBeforePWD.Text != admin.pwd)
                     {
                         MessageBox.Show("비밀번호만 변경합니다.");
                         admin.pwd = tbBeforePWD.Text;
                         this.DialogResult = DialogResult.OK;
                         this.Close();
                     }
                     else if (tbBeforeID.Text != admin.id && tbBeforePWD.Text == admin.pwd)
                     {
                         MessageBox.Show("사용자명만 변경합니다.");
                         admin.id = tbBeforeID.Text;
                         this.DialogResult = DialogResult.OK;
                         this.Close();
                     }   
                     else
                         MessageBox.Show("사용자명과 아이디가 이전과 같습니다.");
                    con.Close();

                 }


            }
        }

        public bool EmptyCheck()
        {
            if (String.IsNullOrEmpty(tbBeforeID.Text))
            {
                MessageBox.Show("변경 전 사용자명을 입력해 주세요");
                tbBeforeID.Focus();
                return false;
            }
            else if (String.IsNullOrEmpty(tbBeforePWD.Text))
            {
                MessageBox.Show("변경 전 비밀번호를 입력해 주세요");
                tbBeforePWD.Focus();
                return false;
            }
            else if (String.IsNullOrEmpty(tbChangeID.Text))
            {
                MessageBox.Show("변경할 사용자명을 입력해 주세요\n (변경을 원하지 않을 시 이전 사용자명을 입력해 주세요)");
                tbChangeID.Focus();
                return false;
            }
            else if (String.IsNullOrEmpty(tbChangePWD.Text))
            {
                MessageBox.Show("변경할 비밀번호를 입력해 주세요\n (변경을 원하지 않을 시 이전 비밀번호를 입력해 주세요)");
                tbChangePWD.Focus();
                return false;
            }

            return true;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void tbBeforeID_KeyDown(object sender, KeyEventArgs e)
        {
            //ID 에 Enter Key가 입력되면 PWD로 넘기기
            if (e.KeyCode == Keys.Enter)
            {
                tbBeforePWD.Focus();
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void frmChange_Load(object sender, EventArgs e)
        {

        }

        private void tbBeforePWD_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                tbChangeID.Focus();
            }
        }

        private void tbChangeID_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                tbChangePWD.Focus();
            }
        }

        private void tbChangePWD_KeyDown_1(object sender, KeyEventArgs e)
        {
            //PWD 에 Enter Key가 입력되면 btnChange_Click을 호출
            if (e.KeyCode == Keys.Enter)
            {
                btnChange_Click(sender, e);
                btnChange.Select();
            }
        }
    }
}
