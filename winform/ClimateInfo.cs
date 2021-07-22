using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Caffe_Manager
{
    public partial class ClimateInfo : Form
    {
        private static string Tname;
        private static string QueryLogin = "SERVER = 192.168.0.10; DATABASE = kosta; UID = root; PASSWORD = 12341234;";
        private static string QueryTable = "SELECT * FROM " + Tname + " WHERE 1=1 AND TIME=(SELECT MAX(TIME) FROM " + Tname + ")";
       
        private bool TimerStatus;
        /*
        public void RefreshOn2()
        {
            TimerStatus = true;
            timer1.Start();
            btnClimateStart.Text = "그래프 정지";
        }
        public void RefreshOff2()
        {
            TimerStatus = false;
            timer1.Stop();
            btnClimateStart.Text = "시작";
        }

        public void RefreshToggle2()
        {
            if (false == TimerStatus)//타이머 꺼져있을 때
            {
                RefreshOn2();
            }
            else
            {
                RefreshOff2();
            }
        }
        */
        public void RefreshOn()
        {
            TimerStatus = true;
            timer1.Start();
            
        }
        public void RefreshOff()
        {
            TimerStatus = false;
            timer1.Stop();
         
        }

        public void RefreshToggle()
        {
            if (false == TimerStatus)//타이머 꺼져있을 때
            {
                RefreshOn();
            }
            else
            {
                RefreshOff();
            }
        }
        


        public ClimateInfo()
        {
            InitializeComponent();
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }



        private void UpdateChart()
        {
            DataSet aDataSet = new DataSet();
            using (MySqlConnection amySqlConnection = new MySqlConnection(QueryLogin))
            {
                MySqlDataAdapter mySqlDataAdapter = new MySqlDataAdapter(QueryTable, amySqlConnection);
                mySqlDataAdapter.Fill(aDataSet, Tname);

            }
            DataRow Temp = aDataSet.Tables[Tname].Rows[0];

            ClimateChart.Series[0].Points.AddXY(Temp["TIME"].ToString().Substring(11, 10), int.Parse(Temp["TEMP"].ToString()));

            ClimateChart.Series[1].Points.AddXY(Temp["TIME"].ToString().Substring(11, 10), int.Parse(Temp["HUM"].ToString()));

            if (ClimateChart.Series[0].Points.Count > 3000)
            {
                ClimateChart.Series[0].Points.RemoveAt(0);
                ClimateChart.Series[1].Points.RemoveAt(0);
            }
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            if (Tname == "sensorvalue")
                UpdateChart();
            else if (Tname == "berrystatus")
                UpdateChart2();
        }

        
        private void TabPage2_Enter(object sender, EventArgs e)
        {
            Tname = "sensorvalue";
            string tempsql = "SELECT * FROM " + Tname;
            using (MySqlConnection amySqlConnection = new MySqlConnection(QueryLogin))
            {
                MySqlDataAdapter mySqlDataAdapter = new MySqlDataAdapter(tempsql, amySqlConnection);
                DataSet ds = new DataSet();
                mySqlDataAdapter.Fill(ds);
                dataGridView1.DataSource = ds.Tables[0];
            }
        }

        private void TabPage3_Enter(object sender, EventArgs e)
        {

        }

        private void UpdateChart2()
        {
            //QueryTable = "SELECT * FROM " + Tname;

            DataSet aDataSet = new DataSet();
            using (MySqlConnection amySqlConnection = new MySqlConnection(QueryLogin))
            {
                MySqlDataAdapter mySqlDataAdapter = new MySqlDataAdapter(QueryTable, amySqlConnection);
                mySqlDataAdapter.Fill(aDataSet, Tname);

            }
            DataRow Temp = aDataSet.Tables[Tname].Rows[0];
            chart2.Series[0].IsValueShownAsLabel = true;
            chart2.Series[1].IsValueShownAsLabel = true;
            

            chart2.Series[0].Points.AddXY(Temp["SECTOR"].ToString(), int.Parse(Temp["UNRIPE"].ToString()));
            chart2.Series[1].Points.AddXY(Temp["SECTOR"].ToString(), int.Parse(Temp["RIPE"].ToString()));
            if (chart2.Series[0].Points.Count > 10)
            {
                chart2.Series[0].Points.RemoveAt(0);
                chart2.Series[1].Points.RemoveAt(0);
            }
        }


        private void button3_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnClimateStart_Click(object sender, EventArgs e)
        {
            Tname = "sensorvalue";
            QueryTable = "SELECT * FROM " + Tname + " WHERE 1=1 AND TIME=(SELECT MAX(TIME) FROM " + Tname + ")";

            if (btnClimateStart.Text == "그래프 정지")
            {
                btnClimateStart.Text = "스타트";
                RefreshToggle();
            }
            else
            {
                btnClimateStart.Text = "그래프 정지";
                RefreshToggle();
                RefreshOn();

                DataSet aDataSet = new DataSet();
                using (MySqlConnection amySqlConnection = new MySqlConnection(QueryLogin))
                {
                    MySqlDataAdapter mySqlDataAdapter = new MySqlDataAdapter(QueryTable, amySqlConnection);
                    mySqlDataAdapter.Fill(aDataSet, Tname);

                    List<int> TempData = new List<int>();
                    List<int> HumiData = new List<int>();
                    List<string> Time = new List<string>();
                    foreach (DataRow item in aDataSet.Tables[Tname].Rows)
                    {
                        TempData.Add(int.Parse(item["TEMP"].ToString()));
                        HumiData.Add(int.Parse(item["HUM"].ToString()));
                        Time.Add(item["TIME"].ToString().Substring(11, 10));
                    }

                    Time.Reverse();

                    ClimateChart.Series[0].Points.DataBindXY(Time, TempData);
                    ClimateChart.ChartAreas[0].AxisX.Minimum = 0;
                    ClimateChart.ChartAreas[0].AxisY.Maximum = 100;
                    ClimateChart.Series[0].LegendText = "온도 ºC";
                    ClimateChart.Series[1].Points.DataBindXY(Time, HumiData);
                    ClimateChart.Series[1].LegendText = "습도 %";
                }
            }
        }

        

        private void btnBerryStart_Click_1(object sender, EventArgs e)
        {
            Tname = "berrystatus";//여기에 그 음...딸기의 데이터베이스 이름
            QueryTable = "SELECT * FROM " + Tname + " WHERE 1=1 AND TIME=(SELECT MAX(TIME) FROM " + Tname + ")";


            //딸기의 그래프 시작 페이지
            if (btnBerryStart.Text == "그래프 정지")
            {
                btnBerryStart.Text = "스타트";
                RefreshToggle();
            }
            else
            {
                btnBerryStart.Text = "그래프 정지";
                RefreshToggle();
                RefreshOn();

                DataSet aDataSet = new DataSet();
                using (MySqlConnection amySqlConnection = new MySqlConnection(QueryLogin))
                {
                    MySqlDataAdapter mySqlDataAdapter = new MySqlDataAdapter(QueryTable, amySqlConnection);
                    mySqlDataAdapter.Fill(aDataSet, Tname);
                    //덜익은 딸기, 잘익은 딸기, 섹터, 시간
                    List<int> LessRipe = new List<int>();
                    List<int> WellRipe = new List<int>();
                    List<int> Sector = new List<int>();
                    List<string> Time = new List<string>();
                    foreach (DataRow item in aDataSet.Tables[Tname].Rows)
                    {
                        LessRipe.Add(int.Parse(item["RIPE"].ToString()));
                        WellRipe.Add(int.Parse(item["UNRIPE"].ToString()));
                        Sector.Add(int.Parse(item["SECTOR"].ToString()));
                                                
                        //Time.Add(item["TIME"].ToString().Substring(11, 10));
                    }
                    Time.Reverse();

                    chart2.Series[0].Points.DataBindXY(Sector, LessRipe);
                    chart2.ChartAreas[0].AxisX.Minimum = 0;
                    chart2.ChartAreas[0].AxisY.Maximum = 10;
                    chart2.Series[0].LegendText = "덜익은 딸기";
                    chart2.Series[1].Points.DataBindXY(Sector, WellRipe);
                    chart2.Series[1].LegendText = "잘익은 딸기";

                }
            }
        }

        private void btnBerry_Click(object sender, EventArgs e)
        {
            Tname = "berrystatus";//여기에 그 음...딸기의 데이터베이스 이름
            QueryTable = "SELECT * FROM " + Tname;


            //딸기의 그래프 시작 페이지
            if (btnBerry.Text == "그래프 정지")
            {
                RefreshToggle();
            }
            else
            {
                RefreshToggle();
                RefreshOn();

                DataSet aDataSet = new DataSet();
                using (MySqlConnection amySqlConnection = new MySqlConnection(QueryLogin))
                {
                    MySqlDataAdapter mySqlDataAdapter = new MySqlDataAdapter(QueryTable, amySqlConnection);
                    mySqlDataAdapter.Fill(aDataSet, Tname);
                    //덜익은 딸기, 잘익은 딸기, 섹터, 시간
                    /*List<int> LessRipe = new List<int>();
                    List<int> WellRipe = new List<int>();
                    List<int> Sector = new List<int>();
                    List<string> Time = new List<string>();

                    foreach (DataRow item in aDataSet.Tables[Tname].Rows)
                    {
                        int sec = int.Parse(item["SECTOR"].ToString()); 
                        

                        LessRipe.Add(int.Parse(item["RIPE"].ToString()));
                        WellRipe.Add(int.Parse(item["UNRIPE"].ToString()));
                        Sector.Add(int.Parse(item["SECTOR"].ToString()));

                        //Time.Add(item["TIME"].ToString().Substring(11, 10));
                    }
                    Time.Reverse();*/


                    //2차원배열

                    //2차원배열
                    int[] unripeArray = new int[4];
                    int[] ripeArray = new int[4];
                    int sector, lessripe, wellripe;

                    foreach (DataRow item in aDataSet.Tables[Tname].Rows)
                    {
                        sector = int.Parse(item["SECTOR"].ToString());
                        lessripe = int.Parse(item["UNRIPE"].ToString());
                        wellripe = int.Parse(item["RIPE"].ToString());

                        unripeArray[sector - 1] += lessripe;
                        ripeArray[sector - 1] += wellripe;
                    }

                    chart1.Series[0].Name = "UNRIPE";
                    chart1.Series[1].Name = "RIPE";
                    chart1.Series[0].Points.DataBindY(unripeArray);
                    chart1.Series[1].Points.DataBindY(ripeArray);
                    
                }
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void TabPage1_Leave(object sender, EventArgs e)
        {
            RefreshOff();
            btnClimateStart.Text = "그래프 정지";
        }

        private void BtnBerryStart_Leave(object sender, EventArgs e)
        {
            RefreshOff();
            btnBerryStart.Text = "그래프 정지";
        }
    }
}
