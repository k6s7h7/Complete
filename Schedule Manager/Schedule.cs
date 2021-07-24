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
using System.Runtime.Serialization.Formatters.Binary;

namespace Schedule_Manager
{
    public partial class Schedule : Form
    {
        public Makeframe mframe;
        public ManagerAnni ma;
        public Filemanager fmanager;
        public Monthdatamanager monmanager;
        public Schedule()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Globals.main = this;
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = Color.Beige;
            Statics.calendar = new List<String>();
            Statics.korcal = new List<String>();
            Statics.dat = new List<Data>();
            Statics.mlist = new List<Memo>();
            Statics.mondat = new Monthdata[31];
            for (int i = 0; i < 31; i++)
            {
                Statics.mondat[i] = new Monthdata(0, new string[6], "");
                Statics.anniversary[i] = new Label();
            }
            Statics.calendar = File.ReadLines(Statics.dirpath + "korcal.dat").ToList();
            fmanager = new Filemanager();
            fmanager.creatfile();
            mframe = new Makeframe();
            selectday(DateTime.Now.ToString("yyyy-MM-dd"));
            mframe.makecalender();
            monmanager = new Monthdatamanager();
            monmanager.bringdata();
        }
        
        // 달력에 표시할 년월일을 구한다
        public void selectday(String today)
        {
            String[] temp = today.Split('-');
            Statics.selyear = Int32.Parse(temp[0]);
            checkyear();
            Statics.selmonth = Int32.Parse(temp[1]);
            Statics.selday = Int32.Parse(temp[2]);
            Statics.startdate = calcudate("1900.01.01", Statics.selyear.ToString() + "." + Statics.selmonth.ToString("0#") + ".01");
            Statics.startday = (int)(new DateTime(Statics.selyear, Statics.selmonth, 1)).DayOfWeek;
            Statics.endday = DateTime.DaysInMonth(Statics.selyear, Statics.selmonth);
            Statics.korcal = Statics.calendar.Skip(Statics.startdate).Take(Statics.endday).ToList();
            fmanager.accessmemo();
            mframe.edgeday(Statics.startday + Statics.selday - 1);
            cleardata();
        }
        // 해당월의 일수를 구한다
        public int calcudate(string startdate, string enddate)
        {
            DateTime T1 = DateTime.Parse(startdate);
            DateTime T2 = DateTime.Parse(enddate);
            TimeSpan TS = T2.Subtract(T1);
            return TS.Days;  //날짜의 차이 구하기
        }
        // 데이터 범위내의 년인지 검사한다
        public void checkyear()
        {
            if (Statics.selyear < 1900 || Statics.selyear > 2100)
            {
                MessageBox.Show("데이터가 1900.1.1 부터 2100.12.31 까지 밖에 없습니다. 범위 내의 날짜를 선택하십시요");
                if (Statics.selyear < 1900)
                {
                    mframe.dtp.Value = new DateTime(int.Parse(DateTime.Now.ToString("1900")), 1, 1);
                    Statics.selyear = 1900;
                }
                else
                {
                    mframe.dtp.Value = new DateTime(int.Parse(DateTime.Now.ToString("2100")), 12, 31);
                    Statics.selyear = 2100;
                }
            }
        }
        // 달력에 표시할 기념일등을 지워 초기화 한다
        public void cleardata()
        {
            for (int i = 0; i < 31; i++)
            {
                Statics.mondat[i].count = 0;
                Statics.mondat[i].memo = "";
                for (int j = 0; j < 6; j++) Statics.mondat[i].title[j] = "";
                Statics.anniversary[i].Text = "";
            }
        }
    }

    public class Makeframe
    {
        public DateTimePicker dtp = new DateTimePicker(), dtp1 = new DateTimePicker();
        public Label[] tlabel = new Label[8], weeklabel = new Label[7], label = new Label[42], anniversary = new Label[31], btn = new Label[7];
        public Label overlabel = new Label();
        public ComboBox combo = new ComboBox();
        public TextBox[] text = new TextBox[2];

        public Makeframe()
        {
            makedtp();
            makelabel();
            makebutton();
            makecombo();
            maketext();
            makeweeklabel();
            makedays();
        }
        // DateTimePicker 만든다
        public void makedtp()
        {
            dtp.Parent = Globals.main;
            dtp.Font = new Font(Statics.font, 13, FontStyle.Bold);
            dtp.Bounds = new Rectangle(365, 90, 240, 26);
            dtp.Value = DateTime.Now;
            dtp.Format = DateTimePickerFormat.Long;
            dtp.CloseUp += changedtp;

            dtp1.Parent = Globals.main;
            dtp1.Font = new Font(Statics.font, 12, FontStyle.Bold);
            dtp1.Bounds = new Rectangle(124, 700, 120, 26);
            dtp1.Value = DateTime.Now;
            dtp1.Format = DateTimePickerFormat.Custom;
            dtp1.CustomFormat = "yyyy.MM.dd";
        }
        // DateTimePicker의 선택된 날짜의 달력을 나타낸다
        public void changedtp(object sender, EventArgs e)
        {
            Globals.main.selectday(dtp.Value.ToString("yyyy-MM-dd"));
            makecalender();
            Globals.main.monmanager.bringdata();
        }
        // 양력, 음력 콤보박스를 만든다
        public void makecombo()
        {
            String[] name = new String[] { "양력", "음력" };
            combo.Items.Clear();
            combo.Parent = Globals.main;
            combo.Font = new Font(Statics.font, 12, FontStyle.Bold);
            combo.BackColor = Color.FromArgb(209, 220, 245);
            combo.Bounds = new Rectangle(60, 700, 62, 26);
            combo.Items.AddRange(name);
            combo.SelectedIndex = 0;
            combo.DrawMode = DrawMode.OwnerDrawFixed;
            combo.DropDownStyle = ComboBoxStyle.DropDownList;
            combo.DrawItem += new DrawItemEventHandler(drawitem);
        }
        // 콤보박스 색상 지정
        public void drawitem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;
            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
            {
                e.Graphics.FillRectangle(new SolidBrush(Color.Gold), e.Bounds);
            }
            else
            {
                e.Graphics.FillRectangle(new SolidBrush(((ComboBox)sender).BackColor), e.Bounds);
            }
            e.Graphics.DrawString(((ComboBox)sender).Items[e.Index].ToString(), e.Font, Brushes.Black, e.Bounds, StringFormat.GenericDefault);
        }
        // 년월등 라벨들을 만든다
        public void makelabel()
        {
            int[] startx = new int[] { 805, 900, 1060, 1115, 62, 100, 62, 12 }, starty = new int[] { 80, 80, 80, 80, 780, 200, 240, 280 }, 
                sizex = new int[] { 98, 59, 60, 60, 186, 120, 180, 48 }, sizey = new int[] { 40, 40, 40, 40, 30, 40, 30, 30 }; 
            for (int i = 0; i < 8; i++)
            {
                tlabel[i] = new Label();
                tlabel[i].Parent = Globals.main;
                tlabel[i].Bounds = new Rectangle(startx[i], starty[i], sizex[i], sizey[i]);
                if(i < 4) tlabel[i].Font = new Font(Statics.font, 24, FontStyle.Bold);
                else tlabel[i].Font = new Font(Statics.font, 14, FontStyle.Bold);
                tlabel[i].TextAlign = (i == 7) ? ContentAlignment.MiddleLeft : ContentAlignment.MiddleCenter;
                if(i > 5) tlabel[i].BackColor = Color.FromArgb(218, 223, 232);
            }
            tlabel[1].Text = "년";
            tlabel[3].Text = "월";
            tlabel[5].Text = "일    정";
            tlabel[7].Text = "제목";
            tlabel[4].ForeColor = Color.FromArgb(30, 144, 255);
        }
        // 일정을 메모할 텍스트박스를 만든다
        public void maketext()
        {
            for (int i = 0; i < 2; i++)
            {
                text[i] = new TextBox();
                text[i].Parent = Globals.main;
                text[i].Font = new Font(Statics.font, 12, FontStyle.Bold);
                text[i].TextAlign = HorizontalAlignment.Left;
                text[i].BackColor = Color.FromArgb(218, 223, 232);
                text[i].ForeColor = Color.Blue;
                text[i].ImeMode = ImeMode.Hangul;
            }
            text[0].Bounds = new Rectangle(70, 280, 220, 30);
            text[1].Bounds = new Rectangle(12, 320, 280, 270);
            text[1].Multiline = true;
            
        }
        // 버튼들을 만든다
        public void makebutton()
        {
            String[] name = new String[] { "오늘", "기념일 관리", "삭   제", "저   장", "양음력 변환" };
            int[] startx = new int[] { 630, 1225, 1415, 85, 10, 150, 85 }, starty = new int[] { 85, 85, 85, 125, 600, 600, 735 };

            for (int i = 0; i < 7; i++)
            {
                btn[i] = new Label();
                btn[i].Parent = Globals.main;
                btn[i].Font = new Font(Statics.font, 14, FontStyle.Bold);
                btn[i].TextAlign = ContentAlignment.MiddleCenter;
                if (i > 1)
                {
                    btn[i].Text = name[i - 2];
                    btn[i].Bounds = new Rectangle(startx[i], starty[i], 140, 45);
                    btn[i].Image = Bitmap.FromFile(Statics.imagepath + "normal.png");
                    btn[i].MouseHover += hoverbtn;
                    btn[i].MouseLeave += leavebtn;
                    btn[i].Click += clickbtn;
                }
                else
                {
                    btn[i].Bounds = new Rectangle(startx[i], starty[i], 100, 36);
                    btn[i].MouseHover += hoverarrow;
                    btn[i].MouseLeave += leavearrow;
                    btn[i].Click += clickarrow;
                }
            }
            btn[0].Image = Bitmap.FromFile(Statics.imagepath + "arrowl.png");
            btn[1].Image = Bitmap.FromFile(Statics.imagepath + "arrowr.png");
        }
        // 버튼에 마우스가 올라갈 때 변경되는 그림을 그린다
        public void hoverbtn(object sender, EventArgs e)
        {
            ((Label)sender).Image = Image.FromFile(Statics.imagepath + "hover.png");
        }
        // 버튼에서 마우스가 벗어날 때 변경되는 그림을 그린다
        public void leavebtn(object sender, EventArgs e)
        {
            ((Label)sender).Image = Image.FromFile(Statics.imagepath + "normal.png");
        }
        // 버튼들이 클릭될 때 처리하는 메서드
        public void clickbtn(object sender, EventArgs e)
        {
            ((Label)sender).Image = Image.FromFile(Statics.imagepath + "click.png");
            if ((Label)sender == btn[2]) currentdate();
            else if ((Label)sender == btn[3]) annimanager();
            else if ((Label)sender == btn[4]) delmemo();
            else if ((Label)sender == btn[5]) savememo();
            else changelunarsolar();
        }
        // 양력과 음력 바꿔주는 함수
        public void changelunarsolar()          
        {
            string[] temp = new string[29];
            tlabel[4].Text = "";
            Statics.startdate = Globals.main.calcudate("1900.01.01", dtp1.Value.ToString("yyyy.MM.dd"));
            Statics.korcal = Statics.calendar.Skip(Statics.startdate).Take(100).ToList();
            if (combo.SelectedIndex == 0)
            {
                temp = Statics.korcal[0].Split('|');
                tlabel[4].Text = "음력 " + temp[5] + "." + temp[6] + "." + temp[7];
            }
            else
            {
                int i = 0;
                temp = Statics.korcal[i++].Split('|');
                while (temp[5] != dtp1.Value.ToString("yyyy") && i < 100) temp = Statics.korcal[i++].Split('|');
                while (temp[6].PadLeft(2, '0') != dtp1.Value.ToString("MM") && i < 100) temp = Statics.korcal[i++].Split('|');
                while (temp[7].PadLeft(2, '0') != dtp1.Value.ToString("dd") && i < 100) temp = Statics.korcal[i++].Split('|');
                if (i > 99) MessageBox.Show("그런 음력일자는 없습니다. 다시 확인하시기 바랍니다");
                else tlabel[4].Text = "양력 " + temp[2] + "." + temp[3] + "." + temp[4];
            }
        }
        // 화살표에 마우스가 올라갈 때 변경되는 그림을 그린다
        public void hoverarrow(object sender, EventArgs e)
        {
            if((Label)sender == btn[0]) ((Label)sender).Image = Image.FromFile(Statics.imagepath + "arrowlh.png");
            else ((Label)sender).Image = Image.FromFile(Statics.imagepath + "arrowrh.png");
        }
        // 화살표에서 마우스가 벗어날 때 변경되는 그림을 그린다
        public void leavearrow(object sender, EventArgs e)
        {
            if ((Label)sender == btn[0]) ((Label)sender).Image = Image.FromFile(Statics.imagepath + "arrowl.png");
            else ((Label)sender).Image = Image.FromFile(Statics.imagepath + "arrowr.png");
        }
        // 화살표가 클릭될 때 처리하는 함수
        public void clickarrow(object sender, EventArgs e)
        {
            if ((Label)sender == btn[0])
            {
                ((Label)sender).Image = Image.FromFile(Statics.imagepath + "arrowlc.png");
                dtp.Value = dtp.Value.AddMonths(-1);
                changedtp(sender, e);
            }
            else
            {
                ((Label)sender).Image = Image.FromFile(Statics.imagepath + "arrowrc.png");
                dtp.Value = dtp.Value.AddMonths(1);
                changedtp(sender, e);
            }
        }
        // 기념일 관리 창을 띄운다
        public void annimanager()
        {
            Globals.main.ma = new ManagerAnni();
            Globals.main.ma.StartPosition = FormStartPosition.Manual;
            Globals.main.ma.Bounds = new Rectangle((Globals.main.Width - 1600) / 2, (Globals.main.Height - 950) / 2, 1600, 950);
            Globals.main.ma.Show();
        }
        // 오늘 날짜의 달력을 그린다
        public void currentdate()
        {
            dtp.Value = DateTime.Now;
            Globals.main.selectday(DateTime.Now.ToString("yyyy-MM-dd"));
            if (Statics.selyear > 2100) MessageBox.Show("데이터가 1900.1.1 부터 2100.12.31 까지 밖에 없습니다. 이제는 사용할 수 없습니다");
            makecalender();
            Globals.main.monmanager.bringdata();
        }
        // memo를 삭제한다
        public void delmemo()
        {
            String temp = Statics.selmonth.ToString("0#") + (Statics.selvalue + 1).ToString("0#");
            int delnum = -1;
            if ((delnum = Statics.mlist.FindIndex(x => x.date == temp)) >= 0) Statics.mlist.RemoveAt(delnum);
            Statics.mondat[Statics.selvalue].title[4] = "";
            Statics.mondat[Statics.selvalue].memo = "";
            text[0].Text = text[1].Text = "";
            Globals.main.fmanager.mwritefile();
            Globals.main.monmanager.outanniversary(Statics.selvalue, Statics.startday + Statics.selvalue);
        }
        // memo를 저장한다
        public void savememo()
        {
            if (text[0].Text.Trim() == "") MessageBox.Show("제목 란이 빈칸이면 안됩니다!!");
            else if (tlabel[6].Text.Trim() == "") MessageBox.Show("날짜를 지정하십시요!!");
            else
            {
                String temp = Statics.selmonth.ToString("0#") + (Statics.selvalue + 1).ToString("0#");
                int delnum = -1;
                if ((delnum = Statics.mlist.FindIndex(x => x.date == temp)) >= 0) Statics.mlist.RemoveAt(delnum);
                Statics.mlist.Add(new Memo(temp, text[0].Text, text[1].Text));
                Statics.mondat[Statics.selvalue].title[4] = text[0].Text;
                Statics.mondat[Statics.selvalue].memo = text[1].Text;
                Globals.main.fmanager.mwritefile();
                Globals.main.monmanager.outanniversary(Statics.selvalue, Statics.startday + Statics.selvalue);
            }
        }
        // 각 요일들의 제목을 그린다 
        public void makeweeklabel()
        {
            String[] weekday = new String[] { "일요일", "월요일", " 화요일", "수요일", "목요일", "금요일", "토요일" };

            for (int i = 0; i < 7; i++)
            {
                weeklabel[i] = new Label();
                weeklabel[i].Parent = Globals.main;
                weeklabel[i].Text = weekday[i];
                weeklabel[i].Bounds = new Rectangle(334 + 216 * i, 130, 140, 32);
                weeklabel[i].Font = new Font(Statics.font, 14, FontStyle.Bold);
                weeklabel[i].TextAlign = ContentAlignment.MiddleCenter;
                weeklabel[i].Image = System.Drawing.Bitmap.FromFile(Statics.imagepath + "week.png");
            }
        }
        // 달력에 날짜를 쓴다
        public void makecalender()
        {
            int i, j;

            tlabel[0].Text = Statics.selyear.ToString();
            tlabel[2].Text = Statics.selmonth.ToString();
            for (i = 0; i < 6; i++)
            {
                for (j = 0; j < 7; j++) colordate(i * 7 + j);
                if (i * 7 + j >= Statics.endday + Statics.startday) break;
            }
            for (i++; i < 6; i++) for (j = 0; j < 7; j++) label[i * 7 + j].Visible = false;
        }
        // 달력의 날짜칸 이미지를 그린다
        public void makedays()
        {
            int i, j;
            
            for (i = 0; i < 6; i++)
            {
                for (j = 0; j < 7; j++)
                {
                    label[i * 7 + j] = new Label();
                    label[i * 7 + j].Parent = Globals.main;
                    label[i * 7 + j].Name = (i * 7 + j).ToString();
                    label[i * 7 + j].Bounds = new Rectangle(300 + 216 * j, 180 + 126 * i, 210, 120);
                    label[i * 7 + j].Font = new Font(Statics.font, 28, FontStyle.Bold);
                    label[i * 7 + j].TextAlign = ContentAlignment.MiddleCenter;
                    label[i * 7 + j].Image = System.Drawing.Bitmap.FromFile(Statics.imagepath + "calday.jpg");
                }
            }
            makecalender();
        }
        // 평일, 토요일, 일요일의 색상을 지정하여 날짜를 쓴다 
        public void colordate(int number)
        {
            label[number].Visible = true;
            if (number < Statics.startday || number > Statics.endday + Statics.startday - 1) label[number].Text = "";
            else
            {
                if (number % 7 == 0) label[number].ForeColor = Color.Red;
                else if (number % 7 == 6) label[number].ForeColor = Color.Aqua;
                else label[number].ForeColor = Color.White;
                label[number].Text = "      " + (number - Statics.startday + 1).ToString();
            }
        }
        // 주어진 날짜에 테두리가 있는 그림을 덮어쓴다(라벨의 부모를 지정하여 부모 위에 덮어쓴다)
        public void edgeday(int sel)
        {
            overlabel.Parent = label[sel];                                 
            overlabel.Bounds = new Rectangle(0, 0, 210, 120);
            overlabel.Image = Bitmap.FromFile(Statics.imagepath + "edge.png");
            overlabel.BackColor = Color.Transparent;
            overlabel.Visible = true;
        }
        // 선택된 날짜를 표시한다
        public void amarkday(object sender, EventArgs e)
        {
            for (Statics.selvalue = 0; Statics.selvalue < 31; Statics.selvalue++) if (string.Compare(((Label)sender).Name, Statics.selvalue.ToString()) == 0) break;
            edgeday(Statics.selvalue + Statics.startday);
            dtp.Value = new DateTime(Statics.selyear, Statics.selmonth, Statics.selvalue + 1);
            tlabel[6].Text = Statics.selyear.ToString() + "년 " + Statics.selmonth.ToString() + "월 " + (Statics.selvalue + 1).ToString() + "일";
            writememo();

        }
        // 일정을 일정 란에 쓴다
        public void writememo()
        {
            if (Statics.mondat[Statics.selvalue].title[4].Trim() != "")
            {
                text[0].Text = Statics.mondat[Statics.selvalue].title[4];
                text[1].Text = Statics.mondat[Statics.selvalue].memo;
            }
            else
            {
                text[0].Text = "";
                text[1].Text = "";
            }
        }
    }

    public class Monthdatamanager 
    {
        public void bringdata()
        {
            int i, num;
            for (i = 0; i < Statics.dat.Count; i++)
            {
                num = 10000;
                if (Statics.dat[i].cycle == 1)
                {
                    if (Statics.dat[i].year == Statics.selyear && Statics.dat[i].month == Statics.selmonth) num = Statics.dat[i].day;
                }
                else
                {
                    if (Statics.dat[i].lunar) num = lunartosolar(i);
                    else
                    {
                        if (Statics.dat[i].month == Statics.selmonth) num = Statics.dat[i].day;
                    }
                    
                }
                if (num < Statics.korcal.Count + 1)
                {
                    Statics.mondat[num - 1].title[Statics.mondat[num - 1].count++] = Statics.dat[i].title;
                    if (Statics.dat[i].holiday == 1) Statics.mondat[num - 1].title[5] = "1";
                }
            }
            overdata();
        }

        public void overdata()
        {
            string[] temp = new string[10];

            readkorfile();
            for (int i = 0; i < Statics.endday; i++)
            {
                outanniversary(i, Statics.startday + i);
                if (Statics.mondat[i].title[5] == "1")
                {
                    Globals.main.mframe.label[Statics.startday + i].ForeColor = Color.Red;
                    Globals.main.mframe.label[Statics.startday + i].Text = "      " + (i + 1).ToString();
                }
            }
        }

        public void readkorfile()
        {
            string[] temp;
            int[] num = new int[] { 22, 24, 26, 27 };
            for (int i = 0; i < Statics.endday; i++)
            {
                temp = Statics.korcal[i].Split('|');
                if(temp[28] == "1") Statics.mondat[i].title[5] = "1";
                String date = Int32.Parse(temp[3]).ToString("0#") + Int32.Parse(temp[4]).ToString("0#");
                int idx = Statics.mlist.FindIndex(x => x.date == date);
                if (idx >= 0)
                {
                    Statics.mondat[i].title[4] = Statics.mlist[idx].title;
                    Statics.mondat[i].memo = Statics.mlist[idx].memo;
                }
                for (int j = 0; j < num.Length; j++)
                {
                    if(Statics.mondat[i].count < 4 && temp[num[j]] != "NULL")
                    {
                        Statics.mondat[i].title[Statics.mondat[i].count++] = temp[num[j]];
                    }
                }
            }
        }
        // 데이터로 찾아서 음력을 양력으로 바꾸는거
        public int lunartosolar(int numb)                  
        {
            int i;
            String[] temp;
            
            for (i = 0; i < Statics.korcal.Count; i++)
            {
                temp = Statics.korcal[i].Split('|');
                if (Int32.Parse(temp[6]) == Statics.dat[numb].month && Int32.Parse(temp[7]) == Statics.dat[numb].day) break;
            }
            return i + 1;
        }
        // 라벨의 부모를 지정하여 기념일들을 부모 위에 덮어쓴다
        public void outanniversary(int sday, int numb)
        {
            Statics.anniversary[sday].Parent = Globals.main.mframe.label[numb];                                 
            Statics.anniversary[sday].Name = sday.ToString();
            Statics.anniversary[sday].Location = new Point(18, 20);
            Statics.anniversary[sday].Size = new Size(160, 82);
            Statics.anniversary[sday].Image = Bitmap.FromFile(Statics.imagepath + "blank.png");
            Statics.anniversary[sday].Click += Globals.main.mframe.amarkday;
            Statics.anniversary[sday].PointToClient(new Point(Control.MousePosition.X, Control.MousePosition.Y));
            Statics.anniversary[sday].Paint += (sender, e) => paintlabel(sender, e, sday);
            Statics.anniversary[sday].Font = new Font("휴먼신그래픽", 11, FontStyle.Bold);
            Statics.anniversary[sday].TextAlign = ContentAlignment.TopLeft;
            Statics.anniversary[sday].BackColor = Color.Transparent;                   // 부모의 배경색을 백컬러로
            Statics.anniversary[sday].BringToFront();                                  // 표시 순서를 가장 앞으로 올린다
        }

        public void paintlabel(object sender, PaintEventArgs e, int num)
        {
            for (int j = 0; j < Statics.mondat[num].count; j++)
            {
                if (Statics.mondat[num].title[j].Substring(0, 1) == "★")
                {
                    Font normalFont = new Font("휴먼신그래픽", 12, FontStyle.Bold);
                    Size normalSize = TextRenderer.MeasureText("12345678901234567890", normalFont);
                    Rectangle normalRect = new Rectangle(
                            0, j * 20, normalSize.Width, normalSize.Height);
                    e.Graphics.DrawString(Statics.mondat[num].title[j], normalFont, Brushes.Yellow, normalRect);
                }
                else
                {
                    Font smallFont = new Font("휴먼신그래픽", 11, FontStyle.Bold);
                    Size smallSize = TextRenderer.MeasureText("12345678901234567890", smallFont);
                    Rectangle smallRect = new Rectangle(0, j * 20, smallSize.Width, smallSize.Height);
                    e.Graphics.DrawString(Statics.mondat[num].title[j], smallFont, Brushes.White, smallRect);
                }
            }
            if(Statics.mondat[num].title[4].Trim() != "")
            {
                Font normalFont = new Font("휴먼신그래픽", 12, FontStyle.Bold);
                Size normalSize = TextRenderer.MeasureText("12345678901234567890", normalFont);
                Rectangle normalRect = new Rectangle(10, 66, normalSize.Width, normalSize.Height);
                e.Graphics.DrawString(Statics.mondat[num].title[4], normalFont, Brushes.LawnGreen, normalRect);
            }
        }
    }

    public class Filemanager
    {
        string filename = Statics.dirpath + "data.dat", mfilename;
        BinaryFormatter bf = new BinaryFormatter();
        Stream stream;

        public void creatfile()
        {
            FileInfo fn = new FileInfo(filename);
            if (!fn.Exists) filewrite();
            fileread();
        }

        public void filewrite()
        {
            StreamWriter sw = new StreamWriter(filename);
            for (int i = 0; i < Statics.dat.Count; i++)
            {
                sw.Write(Statics.dat[i].lunar);
                sw.Write('|');
                sw.Write(Statics.dat[i].year);
                sw.Write('|');
                sw.Write(Statics.dat[i].month);
                sw.Write('|');
                sw.Write(Statics.dat[i].day);
                sw.Write('|');
                sw.Write(Statics.dat[i].hour);
                sw.Write('|');
                sw.Write(Statics.dat[i].minute);
                sw.Write('|');
                sw.Write(Statics.dat[i].cycle);
                sw.Write('|');
                sw.Write(Statics.dat[i].holiday);
                sw.Write('|');
                sw.Write(Statics.dat[i].title);
                sw.Write('|');
                sw.Write(Statics.dat[i].memo);
                sw.Write("\r\n");
            }
            sw.Close();
        }

        public void fileread()
        {
            string line;
            string[] tmp = new string[12];

            StreamReader sr = new StreamReader(filename);
            while ((line = sr.ReadLine()) != null)
            {
                tmp = line.Split('|');
                Statics.dat.Add(new Data(Statics.num++, Convert.ToBoolean(tmp[0]), Int32.Parse(tmp[1]), Int32.Parse(tmp[2]), Int32.Parse(tmp[3]),
                    Int32.Parse(tmp[4]), Int32.Parse(tmp[5]), Int32.Parse(tmp[6]), Int32.Parse(tmp[7]), tmp[8], tmp[9]));
            }
            sr.Close();
        }
        
        public void accessmemo()
        {
            mfilename = Statics.dirpath + Statics.selyear.ToString() + "memo.dat";
            try
            {
                FileInfo fi = new FileInfo(mfilename);
                if (!fi.Exists) mwritefile();
                mreadfile();
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show("에러 매세지 = " + ex.Message);
            }
        }

        public void mwritefile()
        {
            mfilename = Statics.dirpath + Statics.selyear.ToString() + "memo.dat";
            stream = File.Open(mfilename, FileMode.Create);

            bf.Serialize(stream, Statics.mlist);
            stream.Close();
        }

        public void mreadfile()
        {
            Statics.mlist.Clear();
            stream = File.Open(mfilename, FileMode.Open);

            Statics.mlist = (List<Memo>)bf.Deserialize(stream);
            stream.Close();
        }
    }

    public class Statics
    {
        public static String font = "휴먼신그래픽", dirpath = Directory.GetCurrentDirectory() + "\\Data\\", imagepath = Directory.GetCurrentDirectory() + "\\Image\\";
        public static int selyear, selmonth, selday, startdate, startday, endday, selvalue = 0, mouse = 0, num = 0, curnum = 0;
        public static List<String> korcal, calendar;
        public static List<Data> dat;
        public static List<Memo> mlist;
        public static Monthdata[] mondat;
        public static Label[] anniversary = new Label[31];
    }

    public class Data
    {
        public int num;
        public Boolean lunar;
        public int year, month, day, hour, minute, cycle, holiday;
        public String title, memo;

        public Data(int num, Boolean lunar, int year, int month, int day, int hour, int minute, int cycle, int holiday, String title, String memo)
        {
            this.num = num;
            this.lunar = lunar;
            this.year = year;
            this.month = month;
            this.day = day;
            this.hour = hour;
            this.minute = minute;
            this.cycle = cycle;
            this.holiday = holiday;
            this.title = title;
            this.memo = memo;
        }
    }
    public class Monthdata
    {
        public int count;
        public String[] title;
        public String memo;
        public Monthdata(int count, String[] title, String memo)
        {
            this.count = count;
            this.title = title;
            this.memo = memo;
        }
    }

    [Serializable]
    public class Memo
    {
        public String date;
        public String title;
        public String memo;

        public Memo(String date, String title, String memo)
        {
            this.date = date;
            this.title = title;
            this.memo = memo;
        }
    }

    public class Globals            // 폼에 있는 메서드를 다른 클래스에서 사용하기 위해
    {
        public static Schedule main;
        public static ManagerAnni ma;
    }
}