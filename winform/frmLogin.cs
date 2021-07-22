using Caffe_Manager;
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


    public partial class frmLogin : Form
    {

        public frmLogin()
        {
            InitializeComponent();
            tbID.Focus();
        }
        private void btnLogin_Click(object sender, EventArgs e)
        {
            SqlConnection con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Owner\Desktop\proto_\test.mdf;Integrated Security=True;Connect Timeout=30");
            SqlDataAdapter sda = new SqlDataAdapter("Select Count(*) from adminInfo where Id='" + tbID.Text + "'and Password='" + tbPWD.Text + "'", con);

            DataTable newtable = new DataTable();
            sda.Fill(newtable);

            if(EmptyCheck())
            {
                if (newtable.Rows[0][0].ToString() == "1")
                {
                    MessageBox.Show("로그인 성공");
                    //정보선택 폼으로 넘어감녀 될것 같습니당
                    this.Visible = false;
                    ClimateInfo ci = new ClimateInfo();
                    ci.ShowDialog();
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                    MessageBox.Show("사용자명과 비밀번호를 확인해주세요.");

            /*    if (tbID.Text == admin.id && tbPWD.Text == admin.pwd)
                {
                    MessageBox.Show("로그인 성공");
                    //정보선택 폼으로 넘어감녀 될것 같습니당
                    this.Visible = false;
                    SelectPage sf = new SelectPage();
                    sf.ShowDialog();
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                } 

                else if (tbID.Text == admin.id && tbPWD.Text != admin.pwd)
                    MessageBox.Show("비밀번호가 올바르지 않습니다.");
                else if (tbID.Text != admin.id && tbPWD.Text == admin.pwd)
                    MessageBox.Show("사용자명이 올바르지 않습니다.");
                else
                    MessageBox.Show("사용자명과 아이디가 올바르지 않습니다."); */
            }

        }

        public bool EmptyCheck()
        {
            if (String.IsNullOrEmpty(tbID.Text))
            {
                MessageBox.Show("사용자명을 입력해 주세요");
                tbID.Focus();
                return false;
            }
            else if (String.IsNullOrEmpty(tbPWD.Text))
            {
                MessageBox.Show("비밀번호를 입력해 주세요");
                tbPWD.Focus();
                return false;
            }

            return true;
        }
       private void tbID_KeyDown(object sender, KeyEventArgs e)
        {
            //ID에 Enter Key가 입력되면 PWD 창으로 넘기기
            if(e.KeyCode == Keys.Enter)
            {
                tbPWD.Focus();
            }
        }

        private void tbPWD_KeyDown(object sender, KeyEventArgs e)
        {
            //PWD 에 Enter Key가 입력되면 btnLogin_Click을 호출
            if(e.KeyCode == Keys.Enter)
            {
                btnLogin_Click(sender, e);
                btnLogin.Select();
            }
        }

        private void lbChange_Click(object sender, EventArgs e)
        {
            frmChange frmChange = new frmChange();
            frmChange.ShowDialog();
        }

        private void frmLogin_Load_1(object sender, EventArgs e)
        {
            tbID.Focus();
        }
    }

    class admin
    {
        //db에 정보 넣어야 아이디,비번 변경했을 떄 대처 가능할 것 같아용,, 
        public static string id;
        public static string pwd;
    }
}
