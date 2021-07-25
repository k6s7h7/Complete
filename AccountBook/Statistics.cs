using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Windows.Forms.DataVisualization.Charting;

namespace account_book
{
    public partial class Statistics : Form
    {
        public Showchart showcht;
        public ComboBox chartcombo, divcombo, moncombo, monthcombo;
        public CheckedListBox listbox;
        public Chart chart = new Chart();
        public ChartArea chartarea = new ChartArea();
        public Legend legend = new Legend();
        public Title title = new Title();
        public Series[] series;
        public List<Gdata> glist = new List<Gdata>();
        public List<string> fnlist = new List<string>();
        public double[] itemvalue;
        public List<Yeardata> yearlist = new List<Yeardata>();
        public int END = 0;

        public Statistics()
        {
            InitializeComponent();
        }

        private void Statistics_Load(object sender, EventArgs e)
        {
            Gglobal.stat = this;
            this.BackColor = Color.FromArgb(218, 223, 232);
            this.AutoScroll = true;
            itemvalue = new double[Globals.house.oitemlist.Count];
            showcht = new Showchart();
            new Readfile();
            if (END == 0)
            {
                makecombo();
                makechart();
            }
            this.FormClosing += new FormClosingEventHandler(closeform);
        }

        public void closeform(object sender, FormClosingEventArgs e)
        {
            this.Dispose();
        }

        public void inoutgraph()
        {
            int i, j;
            double[] value;
            series = new Series[2];
            string[] name = new string[] { "수입", "지출" };

            basicsetting(2, name);
            switch (divcombo.SelectedIndex)
            {
                case 0:
                    value = new double[2] { 0, 0 };
                    for (i = 0; i < glist.Count; i++)
                    {
                        value[0] += glist[i].income;
                        value[1] += glist[i].expend;
                    }
                    series[0].Points.AddXY("총합계", value[0] / 1000);
                    series[1].Points.AddXY("", value[1] / 1000);
                    break;
                case 1:
                    value = new double[yearlist.Count * 2];
                    for (i = 0; i < yearlist.Count; i++)
                    {
                        value[i * 2] = 0;
                        value[i * 2 + 1] = 0;
                        for (j = 0; j < yearlist[i].max; j++)
                        {
                            value[i * 2] += glist[yearlist[i].start + j].income;
                            value[i * 2 + 1] += glist[yearlist[i].start + j].expend;
                        }
                    }
                    for (i = 0; i < yearlist.Count; i++)
                    {
                        series[0].Points.AddXY(yearlist[i].year, value[i * 2] / 1000);
                        series[1].Points.AddXY("", value[i * 2 + 1] / 1000);
                    }
                    break;
                case 2:
                    for (j = 0; j < yearlist.Count; j++) if (yearlist[j].year == moncombo.SelectedItem.ToString()) break;
                    for (i = 0; i < yearlist[j].max; i++)
                    {
                        series[0].Points.AddXY(fnlist[yearlist[j].start + i], glist[yearlist[j].start + i].income / 1000);
                        series[1].Points.AddXY("", glist[yearlist[j].start + i].expend / 1000);
                    }
                    break;
                default:
                    break;
            }
        }

        public void itemsgraph()
        {
            int i = 0, j, k;
            double[] value = new double[Globals.house.oitemlist.Count];
            string[] name = new string[] { "항목" };
            series = new Series[1];

            basicsetting(1, name);
            switch (divcombo.SelectedIndex)
            {
                case 0:
                    for (i = 0; i < Globals.house.oitemlist.Count; i++)
                    {
                        value[i] = 0;
                        for (j = 0; j < fnlist.Count; j++) value[i] += glist[j].value[i];
                    }
                    break;
                case 1:
                    for (j = 0; j < yearlist.Count; j++) if (yearlist[j].year == moncombo.SelectedItem.ToString()) break;
                    for (i = 0; i < Globals.house.oitemlist.Count; i++)
                    {
                        value[i] = 0;
                        for (k = 0; k < yearlist[j].max; k++) value[i] += glist[k].value[i];
                    }
                    break;
                case 2:
                    for (j = 0; j < yearlist.Count; j++) if (yearlist[j].year == moncombo.SelectedItem.ToString()) break;
                    for (k = 0; k < yearlist[j].max; k++) if (fnlist[yearlist[j].start + k].Substring(5, 2) == monthcombo.SelectedItem.ToString()) break;
                    for (i = 0; i < Globals.house.oitemlist.Count; i++) value[i] = glist[yearlist[j].start + k].value[i];
                    break;
                default:
                    break;
            }
            for (i = 0; i < Globals.house.oitemlist.Count; i++)
            {
                if (i == 4) continue;
                series[0].Points.AddXY(Globals.house.oitemlist[i].name, value[i] / 1000);
            }
        }

        public void multiitemgraph()
        {
            int i = 0, j, k;
            
            int[] item = new int[listbox.CheckedItems.Count];
            series = new Series[listbox.CheckedItems.Count];
            string[] name = new string[listbox.CheckedItems.Count];

            foreach (Object obj in listbox.CheckedItems)
            {
                item[i] = listbox.Items.IndexOf(obj);
                name[i++] = obj.ToString();
            }
            basicsetting(listbox.CheckedItems.Count, name);
            switch (divcombo.SelectedIndex)
            {
                case 0:
                    double[] value = new double[listbox.CheckedItems.Count];
                    for (i = 0; i < listbox.CheckedItems.Count; i++)
                    {
                        for (j = 0; j < fnlist.Count; j++) value[i] += glist[j].value[item[i] - 2];
                        series[i].Points.AddXY("총합계", value[i] / 1000);
                    }                                                                   
                    break;
                case 1:
                    double[,] value1 = new double[listbox.CheckedItems.Count, yearlist.Count];
                    for (i = 0; i < listbox.CheckedItems.Count; i++)
                    { 
                        for (j = 0; j < yearlist.Count; j++)
                        {
                            value1[i, j] = 0;
                            for (k = 0; k < yearlist[j].max; k++)
                            {
                                value1[i, j] += glist[yearlist[j].start + k].value[item[i] - 2];
                            }
                            series[i].Points.AddXY(yearlist[j].year, value1[i, j] / 1000);
                        }
                    }
                    break;
                case 2:
                    for (j = 0; j < yearlist.Count; j++) if (yearlist[j].year == moncombo.SelectedItem.ToString()) break;
                    for (k = 0; k < listbox.CheckedItems.Count; k++)
                    {
                        for (i = 0; i < yearlist[j].max; i++)
                        {
                            series[k].Points.AddXY(fnlist[yearlist[j].start + i], glist[yearlist[j].start + i].value[item[k] - 2] / 1000);
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        public void makechart()
        {
            chart = new Chart();
            chartarea = new ChartArea();
            legend = new Legend();

            chart.BorderlineWidth = 5;
            chart.BorderSkin.SkinStyle = BorderSkinStyle.FrameTitle1;
            chart.Bounds = new Rectangle(300, 30, 1600, 970);
            chart.TabIndex = 10;

            chart.Name = "차트";
            chartarea.Name = "차트영역";
            legend.Name = "범례";

            chart.ChartAreas.Add(chartarea);
            chart.Legends.Add(legend);
            Controls.Add(chart);

            chart.BackColor = Color.FromArgb(240, 248, 255);
            chartarea.BackColor = Color.FromArgb(218, 223, 232);
        }

        
        public void basicsetting(int max, string[] name)
        {
            // 모두 초기화
            chart.ChartAreas.Clear();
            chart.Titles.Clear();
            chart.Legends.Clear();
            chart.Series.Clear();
            // 모두 추가
            chart.ChartAreas.Add(chartarea);
            chart.Titles.Add(title);
            chart.Legends.Add(legend);
            for (int i = 0; i < max; i++)
            {
                series[i] = new Series();
                // 그래프 모양 설정
                if (chartcombo.SelectedIndex == -1) chartcombo.SelectedIndex = 10;
                series[i].ChartType = (SeriesChartType)chartcombo.SelectedIndex;
                series[i].Name = name[i];
                // 막대 차트가 실린더 모양
                series[i]["DrawingStyle"] = "Cylinder";
                // 선 두께
                series[i].BorderWidth = 12;
                // 점 크기
                series[i].MarkerSize = 10;
                // 표 안의 그래프 끝에 값 표시
                series[i].IsValueShownAsLabel = true;
                // 표 안에 글자 글꼴 및 통화표시
                series[i].Font = new Font(Statics.font, 11, FontStyle.Bold);
                series[i].LabelFormat = "C";
                chart.Series.Add(series[i]);
            }
            // Y축 기준글자 통화표시
            chartarea.AxisY.LabelStyle.Format = "C";
            // Y축 기준글자 제목
            chartarea.AxisY.Title = "단위 : 천원";
            chartarea.AxisY.TitleFont = new Font(Statics.font, 12, FontStyle.Bold);
            chartarea.RecalculateAxesScale();
            // X축 기준글자 간격 설정
            chartarea.AxisX.Interval = 1;
            // X축, Y축 기준글자 글꼴
            chartarea.AxisX.LabelStyle.Font = new Font(Statics.font, 12, FontStyle.Bold);
            chartarea.AxisY.LabelStyle.Font = new Font(Statics.font, 12, FontStyle.Bold);
            // 테두리 상단 중간에 글자
            if (divcombo.SelectedIndex == 2 && listbox.SelectedIndex == 1) title.Text = moncombo.SelectedItem.ToString() + " 년 " + monthcombo.SelectedItem.ToString() + " 월";
            else if (divcombo.SelectedIndex == 2 || (divcombo.SelectedIndex == 1 && listbox.SelectedIndex == 1)) title.Text = moncombo.SelectedItem.ToString() + " 년";
            else title.Text = divcombo.SelectedItem.ToString();
            title.Font = new Font(Statics.font, 20, FontStyle.Bold);
            title.ForeColor = Color.White;
            // 범례 글꼴
            legend.Font = new Font(Statics.font, 12, FontStyle.Bold);
        }

        public void makecombo()
        {
            string[] typename = new string[]{"요소 차트", "빠른 요소 차트", "거품형 차트", "꺾은선형 차트", "스플라인 차트", "계단식 꺾은선형 차트", "빠른 꺾은선형 차트", "가로 막대형 차트", "누적 가로 막대형 차트",
            "100% 기준 누적 가로 막대형 차트", "세로 막대형 차트", "누적 세로 막대형 차트", "100% 기준 누적 세로 막대형 차트", "영역 차트", "스플라인 영역 차트", "누적 영역형 차트", "100% 기준 누적 영역형 차트",
            "원형 차트", "도넛형 차트", "주식형 차트", "원통형 차트", "범위 차트", "스플라인 범위 차트", "범위 가로 막대형 차트", "범위 세로 막대형 차트", "방사형 차트", "극좌표형 차트", "오차 막대 차트",
            "상자 그림 차트", "렌코 차트", "삼선전환 차트", "카기 차트", "요소 및 그림 차트", "깔때기형 차트", "피라미드형 차트"},
            division = new string[] { "총합계", "년도별", "월별" };

            listbox = new CheckedListBox();
            listbox.Parent = this;
            listbox.Font = new Font(Statics.font, 13, FontStyle.Bold);
            listbox.BackColor = Color.FromArgb(209, 220, 245);
            listbox.Bounds = new Rectangle(10, 60, 200, 470);
            listbox.CheckOnClick = true;
            listbox.DrawMode = DrawMode.OwnerDrawFixed;
            listbox.DrawItem += new DrawItemEventHandler(Globals.house.inputgroup.drawitem);
            listbox.Items.Add("수입지출별");
            listbox.Items.Add("항목별");
            listbox.Items.AddRange(Globals.house.mnglist.listtoarray(Globals.house.oitemlist));
            listbox.SelectedIndex = 0;
            listbox.SelectedIndexChanged += changeitem;

            chartcombo = new ComboBox();
            chartcombo.Parent = this;
            chartcombo.Font = new Font(Statics.font, 13, FontStyle.Bold);
            chartcombo.BackColor = Color.FromArgb(209, 220, 245);
            chartcombo.Bounds = new Rectangle(10, 540, 280, 500);
            chartcombo.DrawMode = DrawMode.OwnerDrawFixed;
            chartcombo.DropDownStyle = ComboBoxStyle.Simple;
            chartcombo.DrawItem += new DrawItemEventHandler(Globals.house.inputgroup.drawitem);
            chartcombo.Items.AddRange(typename);
            chartcombo.SelectedIndex = 0;
            chartcombo.DroppedDown = true;

            divcombo = new ComboBox();
            divcombo.Parent = this;
            divcombo.Bounds = new Rectangle(214, 30, 84, 100);
            divcombo.Font = new Font(Statics.font, 13, FontStyle.Bold);
            divcombo.BackColor = Color.FromArgb(209, 220, 245);
            divcombo.DrawMode = DrawMode.OwnerDrawFixed;
            divcombo.DropDownStyle = ComboBoxStyle.Simple;
            divcombo.DrawItem += new DrawItemEventHandler(Globals.house.inputgroup.drawitem);
            foreach (string name in division) divcombo.Items.Add(name);
            divcombo.SelectedIndex = 0;
            divcombo.SelectedIndexChanged += divselect;
            divcombo.DroppedDown = true;

            moncombo = new ComboBox();
            moncombo.Parent = this;
            moncombo.Bounds = new Rectangle(214, 130, 74, (yearlist.Count + 1) * 25);
            moncombo.Font = new Font(Statics.font, 13, FontStyle.Bold);
            moncombo.BackColor = Color.FromArgb(209, 220, 245);
            moncombo.DrawMode = DrawMode.OwnerDrawFixed;
            moncombo.DropDownStyle = ComboBoxStyle.Simple;
            moncombo.DrawItem += new DrawItemEventHandler(Globals.house.inputgroup.drawitem);
            moncombo.DroppedDown = true;
            foreach(Yeardata data in yearlist) moncombo.Items.Add(data.year);
            moncombo.SelectedIndex = 0;
            moncombo.SelectedIndexChanged += monselect;
            moncombo.Visible = false;

            monthcombo = new ComboBox();
            monthcombo.Parent = this;
            monthcombo.Font = new Font(Statics.font, 13, FontStyle.Bold);
            monthcombo.BackColor = Color.FromArgb(209, 220, 245);
            monthcombo.DrawMode = DrawMode.OwnerDrawFixed;
            monthcombo.DropDownStyle = ComboBoxStyle.Simple;
            monthcombo.Items.Add("12");
            monthcombo.DrawItem += new DrawItemEventHandler(Globals.house.inputgroup.drawitem);
            monthcombo.DroppedDown = true;
            monthcombo.SelectedIndex = 0;
            monthcombo.Visible = false;
        }

        public void changeitem(object sender, EventArgs e)
        {
            moncombo.Visible = false;
            monthcombo.Visible = false;
            divcombo.SelectedIndex = 0;
            moncombo.SelectedIndex = 0;
            if (listbox.SelectedIndex < 2)
            {
                int sel = listbox.SelectedIndex;
                foreach (int index in listbox.CheckedIndices) listbox.SetItemChecked(index, false);
                listbox.SetItemChecked(sel, true);
            }
            else
            {
                listbox.SetItemChecked(0, false);
                listbox.SetItemChecked(1, false);
            }
        }

        public void divselect(object sender, EventArgs e)
        {
            monthcombo.Visible = false;
            moncombo.SelectedIndex = 0;
            monthcombo.Items.Add("12");
            monthcombo.SelectedIndex = 0;
            if (divcombo.SelectedIndex == 2 || (listbox.SelectedIndex == 1 && divcombo.SelectedIndex == 1)) moncombo.Visible = true;
            else
            {
                moncombo.Visible = false;
                monthcombo.Visible = false;
            }
        }

        public void monselect(object sender, EventArgs e)
        {
            monthcombo.Items.Clear();
            if (listbox.SelectedIndex == 1 && divcombo.SelectedIndex == 2)
            {
                int i = 0, j = 0;
                for (; i < yearlist.Count; i++) if (yearlist[i].year == moncombo.SelectedItem.ToString()) break;
                monthcombo.Bounds = new Rectangle(214, 230, 70, (yearlist[i].max + 1) * 25);
                for (; j < yearlist[i].max; j++) monthcombo.Items.Add(fnlist[yearlist[i].start + j].Substring(5, 2));
                monthcombo.Visible = true;
                monthcombo.SelectedIndex = 0;
            }
            else monthcombo.Visible = false;
        }
    }

    public class Showchart
    {
        public Label label;

        public Showchart()
        {
            label = new Label();
            label.Parent = Gglobal.stat;
            label.Text = "보    기";
            label.Font = new Font(Statics.font, 16, FontStyle.Bold);
            label.TextAlign = ContentAlignment.MiddleCenter;
            label.Image = Image.FromFile(Statics.dirpath + "\\button\\normal.png");
            label.Bounds = new Rectangle(60, 10, 140, 45);
            label.MouseHover += new EventHandler(Globals.house.savedelhov);
            label.MouseLeave += new EventHandler(Globals.house.savedellve);
            label.Click += new EventHandler(showclk);
        }

        public void showclk(object sender, EventArgs e)
        {
            int[] item = new int[Gglobal.stat.listbox.CheckedItems.Count];
            int i = 0;
            foreach (Object obj in Gglobal.stat.listbox.CheckedItems) item[i++] = Gglobal.stat.listbox.Items.IndexOf(obj);
            switch (item[0])
            {
                case 0:
                    Gglobal.stat.inoutgraph();
                    break;
                case 1:
                    Gglobal.stat.itemsgraph();
                    break;
                default:
                    Gglobal.stat.multiitemgraph();
                    break;
            }
        }
    }
    
    public class Readfile
    {
        public Readfile()
        {
            arranging();
            if (Gglobal.stat.END == 0) readfile();
        }
        public void arranging()
        {
            string line;
            
            DirectoryInfo di = new DirectoryInfo(Statics.dirpath + "\\Data\\");
            foreach (FileInfo fi in di.GetFiles())
            {
                if (Path.GetExtension(fi.FullName).Equals(".dat"))
                {
                    StreamReader sr = new StreamReader(fi.FullName);
                    line = sr.ReadLine();
                    if (line == null)
                    {
                        sr.Close();
                        File.Delete(fi.FullName);
                    }
                    else
                    {
                        Gglobal.stat.fnlist.Add(Path.GetFileNameWithoutExtension(fi.Name));
                        sr.Close();
                    }
                }
            }
            if(Gglobal.stat.fnlist.Count < 1)
            {
                MessageBox.Show("자료가 없습니다");
                Gglobal.stat.END = 1;
            }
            else
            {
                Gglobal.stat.fnlist.Sort();
                makeyear();
            }
        }

        public void makeyear()
        {
            string syear = Gglobal.stat.fnlist[0].Substring(0, 4);
            int start = 0, max = 0;
            
            foreach (string year in Gglobal.stat.fnlist)
            {
                if (year.Substring(0, 4) != syear)
                {
                    Gglobal.stat.yearlist.Add(new Yeardata(syear, start, max));
                    syear = year.Substring(0, 4);
                    start += max;
                    max = 1;
                } else max++;
            }
            Gglobal.stat.yearlist.Add(new Yeardata(syear, start, max));
        }

        public void readfile()
        {
            string line;
            string[] tmp;
            int benchmark = Globals.house.oitemlist.FindIndex(x => x.name == "내부거래"), index, i;
            double income, expend;
            StreamReader sr;

            foreach (string name in Gglobal.stat.fnlist)
            {
                sr = new StreamReader(Statics.dirpath + "\\Data\\" + name + ".dat");
                income = expend = 0;
                for (i = 0; i < Gglobal.stat.itemvalue.Length; i++) Gglobal.stat.itemvalue[i] = 0;
                while ((line = sr.ReadLine()) != null)
                {
                    tmp = line.Split('|');
                    index = Globals.house.oitemlist.FindIndex(x => x.num == Int32.Parse(tmp[4]));
                    Gglobal.stat.itemvalue[index] += double.Parse(tmp[8]);
                    if (index < benchmark) income += double.Parse(tmp[8]);
                    else if (index > benchmark) expend += double.Parse(tmp[8]);
                } 
                sr.Close();
                Gglobal.stat.glist.Add(new Gdata(income, expend, Gglobal.stat.itemvalue));
            }
        }
    }

    public class Gdata
    {
        public double income, expend;
        public double[] value;
        public Gdata(double income, double expend, params double[] data)
        {
            int i = 0;
            this.income = income;
            this.expend = expend;
            value = new double[data.Length];
            foreach (double item in data)
            {
                value[i++] = item;
            }
        }
    }

    public class Yeardata
    {
        public string year;
        public int start, max;

        public Yeardata(string year, int start, int max)
        {
            this.year = year;
            this.start = start;
            this.max = max;
        }
    }

    public class Gglobal            // 폼에 있는 메서드를 다른 클래스에서 사용하기 위해
    {
        public static Statistics stat;
    }
}