using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;

namespace Schedule_Manager
{
    public partial class ManagerAnni : Form
    {
        public DataGridView dgv = new DataGridView();
        public int currentrow = 0;
        public Boolean dgvchk = false;
        public Groupbox groupbox;
        public Label[] savedel = new Label[2];
        public ManagerAnni()
        {
            InitializeComponent();
        }

        private void ManagerAnni_Load(object sender, EventArgs e)
        {
            Globals.ma = this;
            Statics.curnum = Statics.dat[0].num;
            settingdgv();
            loaddgv();
            makebutton();
            groupbox = new Groupbox();
            this.FormClosing += new FormClosingEventHandler(closeform);
            this.FormClosed += new FormClosedEventHandler(closedform);
        }

        public void closeform(object sender, FormClosingEventArgs e)
        {
            if (dgvchk)
            {
                if (MessageBox.Show("변경 내용이 있습니다. 저장하시겠습니까?", "변경내용 저장", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    Globals.main.fmanager.filewrite();
            }
            this.Dispose();
        }

        public void closedform(object sender, FormClosedEventArgs e)
        {
            Globals.main.selectday(Globals.main.mframe.dtp.Value.ToString("yyyy-MM-dd"));
            Globals.main.mframe.makecalender();
            Globals.main.monmanager.bringdata();
        }

        public void basicsetdgv()
        {
            dgv.DefaultCellStyle.BackColor = Color.FromArgb(240, 248, 255);
            dgv.DefaultCellStyle.SelectionForeColor = Color.Yellow;
            dgv.RowHeadersVisible = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv.ReadOnly = true;
            dgv.AllowUserToAddRows = false;
            dgv.Font = new Font(Statics.font, 12, FontStyle.Bold);
        }

        public void settingdgv()
        {
            String[] header = new String[] { "양음력", "날짜", "시간", "주기", "휴일", "명칭", "메모" };
            int[] width = new int[] { 70, 100, 56, 70, 50, 300, 450 };

            dgv.Bounds = new Rectangle(440, 70, 1120, 820);
            dgv.ColumnCount = header.Length;
            for (int i = 0; i < header.Length; i++) dgv.Columns[i].HeaderText = header[i];
            for (int i = 0; i < width.Length; i++) dgv.Columns[i].Width = width[i];

            basicsetdgv();
            this.Controls.Add(dgv);
            ExtensionMethods.DoubleBuffered(dgv, true);
            dgv.CellClick += new DataGridViewCellEventHandler(dgvclick);
        }

        public void loaddgv()
        {
            string marklunar, cycle, holiday;
            dgv.Rows.Clear();
            sortdgv();
            for (int i = 0; i < Statics.dat.Count; i++)
            {
                marklunar = (Statics.dat[i].lunar == false) ? "양력" : "음력";
                cycle = (Statics.dat[i].cycle == 0) ? "매년" : "한번만";
                holiday = (Statics.dat[i].holiday == 0) ? "평일" : "휴일";
                dgv.Rows.Add(marklunar, Statics.dat[i].year + "." + Statics.dat[i].month.ToString("0#") + "." + Statics.dat[i].day.ToString("0#"),
                Statics.dat[i].hour.ToString("0#") + ":" + Statics.dat[i].minute.ToString("0#"), cycle, holiday, Statics.dat[i].title, Statics.dat[i].memo);
            }
            savecell();
        }

        public void savecell()
        {
            currentrow = Statics.dat.FindIndex(x => x.num == Statics.curnum);
            if (currentrow < 0) currentrow = 0;
            dgv.Columns[0].Selected = true;                           
            dgv.Rows[currentrow].Selected = true;                                           
            dgv.CurrentCell = dgv[0, currentrow];
        }

        public void sortdgv()
        {
            Statics.dat.Sort(delegate (Data y, Data x) {
                int rtn = y.year.CompareTo(x.year);
                if (rtn == 0)
                {
                    int rtn1 = y.month.CompareTo(x.month);
                    if (rtn1 == 0)
                    {
                        int rtn2 = y.day.CompareTo(x.day);
                        if (rtn2 == 0)
                        {
                            int rtn3 = y.hour.CompareTo(x.hour);
                            if (rtn3 == 0)
                            {
                                return y.minute.CompareTo(x.minute);
                            }
                            else return rtn3;
                        }
                        else return rtn2;
                    }
                    else return rtn1;
                }
                else return rtn;
            });
            dgv.Refresh();
        }

        public void dgvclick(object sender, DataGridViewCellEventArgs e)
        {
            String[] temp;
            currentrow = dgv.CurrentCell.RowIndex;
            Statics.curnum = Statics.dat[currentrow].num;
            if (dgv[0, currentrow].Value.ToString() == "양력") groupbox.radio[0].Checked = true;
            else groupbox.radio[1].Checked = true;
            temp = dgv[1, currentrow].Value.ToString().Split('.');
            groupbox.dtp.Value = new DateTime(Int32.Parse(temp[0]), Int32.Parse(temp[1]), Int32.Parse(temp[2]));
            temp = dgv[2, currentrow].Value.ToString().Split(':');
            groupbox.combo[0].SelectedIndex = Int32.Parse(temp[0]);
            groupbox.combo[1].SelectedIndex = Int32.Parse(temp[1]);
            groupbox.combo[2].SelectedIndex = (dgv[3, currentrow].Value.ToString() == "매년") ? 0 : 1;
            groupbox.text[0].Text = dgv[4, currentrow].Value.ToString().Replace("★", "");
            groupbox.text[1].Text = dgv[5, currentrow].Value.ToString();
        }

        public void makebutton()
        {
            String[] name = new String[] { "저    장", "삭    제" };
            for (int i = 0; i < savedel.Length; i++)
            {
                savedel[i] = new Label();
                savedel[i].Parent = this;
                savedel[i].Bounds = new Rectangle(700 + 400 * i, 10, 145, 45);
                savedel[i].Text = name[i];
                savedel[i].Font = new Font(Statics.font, 16, FontStyle.Bold);
                savedel[i].TextAlign = ContentAlignment.MiddleCenter;
                savedel[i].Image = Image.FromFile(Statics.imagepath + "normal.png");
                savedel[i].MouseHover += Globals.main.mframe.hoverbtn;
                savedel[i].MouseLeave += Globals.main.mframe.leavebtn;
                savedel[i].Click += savedelclk;
            }
        }

        public void savedelclk(object sender, EventArgs e)
        {
            if ((Label)sender == savedel[0])
            {
                Globals.main.fmanager.filewrite();
                dgvchk = false;
            }
            else
            {
                if (MessageBox.Show(dgv[4, currentrow].Value.ToString() +  "를 삭제합니다. 확실하십니까?", "지정한 행삭제", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    Statics.dat.RemoveAt(currentrow);
                    loaddgv();
                    dgvchk = true;
                    currentrow = 0;
                }
            }
        }
    }

    public class Groupbox
    {
        public WATGroupBox gbox = new WATGroupBox();
        public DateTimePicker dtp;
        public RadioButton[] radio = new RadioButton[2];
        public ComboBox[] combo  = new ComboBox[4];
        public TextBox[] text = new TextBox[2];
        public Label[] label = new Label[7];
        public Label[] button = new Label[2];

        public Groupbox()
        {
            makegroupbox();
            makeradio();
            makedtp();
            makecombo();
            makelabel();
            maketextbox();
            makebutton();
        }
        public void makegroupbox()
        {
            gbox.Name = "gbox";
            gbox.Parent = Globals.ma; 
            gbox.Font = new Font(Statics.font, 14, FontStyle.Bold);
            gbox.BackColor = Color.FromArgb(240, 248, 255);
            gbox.Bounds = new Rectangle(20, 70, 400, 360);
        }

        public void makeradio()
        {
            String[] name = new String[] { "양력", "음력" };
            for(int i = 0; i < radio.Length; i++)
            {
                radio[i] = new RadioButton();
                radio[i].Parent = gbox;
                radio[i].Text = name[i];
                radio[i].Bounds = new Rectangle(100 + i * 120, 20, 80, 26);
                radio[i].Font = new Font(Statics.font, 14, FontStyle.Bold);
            }
            radio[0].Checked = true;
        }

        public void makedtp()
        {
            dtp = new DateTimePicker();
            dtp.Parent = gbox;
            dtp.Font = new Font(Statics.font, 13, FontStyle.Bold);
            dtp.Bounds = new Rectangle(80, 60, 245, 26);
            dtp.DropDownAlign = LeftRightAlignment.Left;
        }

        public void makecombo()
        {
            int[,] cpmbolocation = new int[,] { { 80, 100 }, { 190, 100 }, { 80, 140}, { 250, 140 } };
            String[] cycle = new String[] { "매년", "한번만" }, holiday = new String[] { "평일", "휴일" };

            for (int i = 0; i < combo.Length; i++)
            {
                combo[i] = new ComboBox();
                combo[i].Parent = gbox;
                combo[i].Font = new Font(Statics.font, 14, FontStyle.Bold);
                combo[i].BackColor = Color.FromArgb(209, 220, 245);
                if( i < 2) combo[i].Bounds = new Rectangle(cpmbolocation[i, 0], cpmbolocation[i, 1], 55, 26 );
                else combo[i].Bounds = new Rectangle(cpmbolocation[i, 0], cpmbolocation[i, 1], 90, 26);
                combo[i].DrawMode = DrawMode.OwnerDrawFixed;
                combo[i].DropDownStyle = ComboBoxStyle.DropDownList;
                combo[i].DrawItem += Globals.main.mframe.drawitem;
            }
            for (int i = 0; i < 24; i++) combo[0].Items.Add(i.ToString("0#"));
            for (int i = 0; i < 60; i++) combo[1].Items.Add(i.ToString("0#"));
            combo[2].Items.AddRange(cycle);
            combo[3].Items.AddRange(holiday);
            for (int i = 0; i < combo.Length; i++) combo[i].SelectedIndex = 0;
        }

        public void makelabel()
        {
            int[,] labellocation = new int[,] { { 20, 100 }, { 120, 100 }, { 230, 100 }, { 20, 140 }, { 200, 140 }, { 20, 180 }, { 20, 220 } };
            string[] labeltext = new string[] { "시간", "시", "분", "주기", "휴일", "명칭", "메모" };

            for (int i = 0; i < label.Length; i++)
            {
                label[i] = new Label();
                label[i].Text = labeltext[i];
                label[i].Parent = gbox;
                label[i].TextAlign = ContentAlignment.MiddleCenter;
                label[i].Font = new Font(Statics.font, 14, FontStyle.Bold);
                label[i].Bounds = new Rectangle(labellocation[i, 0], labellocation[i, 1], 60, 30);
            }
        }

        public void maketextbox()
        {
            for (int i = 0; i < text.Length; i++)
            {
                text[i] = new TextBox();
                text[i].Parent = gbox;
                text[i].Font = new Font(Statics.font, 14, FontStyle.Bold);
                text[i].BackColor = Color.FromArgb(209, 220, 245);
                text[i].Bounds = new Rectangle(80, 180 + i * 40, 300, 30);
                text[i].TextAlign = HorizontalAlignment.Left;
                text[i].ImeMode = ImeMode.Hangul;
            }
        }

        public void makebutton()
        {
            String[] name = new String[] { "추    가", "수    정" };
            for (int i = 0; i < button.Length; i++)
            {
                button[i] = new Label();
                button[i].Parent = gbox;
                button[i].Text = name[i];
                button[i].Font = new Font(Statics.font, 14, FontStyle.Bold);
                button[i].Bounds = new Rectangle(20 + 200 * i, 300, 140, 45);
                button[i].TextAlign = ContentAlignment.MiddleCenter;
                button[i].Image = Image.FromFile(Statics.imagepath + "normal.png");
                button[i].MouseHover += Globals.main.mframe.hoverbtn;
                button[i].MouseLeave += Globals.main.mframe.leavebtn;
                button[i].Click += clickbtn;
            }
        }

        public void clickbtn(object sender, EventArgs e)
        {
            String[] data = new String[] { "양력", "매년", "평일" };
            Boolean lunar = false;
            int cycle = 0, holiday = 0;
            if (radio[1].Checked) 
            {
                data[0] = "음력";
                lunar = true;
            }
            String[] temp = dtp.Value.ToString("yyyy.MM.dd").Split('.');
            if (combo[2].SelectedIndex == 1)
            {
                data[1] = "한번만";
                cycle = 1;
            }
            if (combo[3].SelectedIndex == 1)
            {
                data[1] = "휴일";
                holiday = 1;
            }
            Globals.ma.dgvchk = true;

            if ((Label)sender == button[0])
            {
                Statics.curnum = Statics.num;
                Statics.dat.Add(new Data(Statics.num++, lunar, Int32.Parse(temp[0]), Int32.Parse(temp[1]), Int32.Parse(temp[2]),
                    Int32.Parse(combo[0].SelectedItem.ToString()), Int32.Parse(combo[1].SelectedItem.ToString()), cycle, holiday, "★" + text[0].Text, text[1].Text));
            }
            else
            {
                Statics.dat[Globals.ma.currentrow].lunar = lunar;
                Statics.dat[Globals.ma.currentrow].year = Int32.Parse(temp[0]);
                Statics.dat[Globals.ma.currentrow].month = Int32.Parse(temp[1]);
                Statics.dat[Globals.ma.currentrow].day = Int32.Parse(temp[2]);
                Statics.dat[Globals.ma.currentrow].hour = Int32.Parse(combo[0].SelectedItem.ToString());
                Statics.dat[Globals.ma.currentrow].minute = Int32.Parse(combo[1].SelectedItem.ToString());
                Statics.dat[Globals.ma.currentrow].cycle = cycle;
                Statics.dat[Globals.ma.currentrow].holiday = holiday;
                Statics.dat[Globals.ma.currentrow].title = "★" + text[0].Text;
                Statics.dat[Globals.ma.currentrow].memo = text[1].Text;
            }
            Globals.ma.loaddgv();
        }
    }

    // 그룹박스 테두리 색상 변경
    public class WATGroupBox : GroupBox
    {
        private Color borderColor;
        public Color BorderColor
        {
            get { return this.borderColor; }
            set
            {
                this.borderColor = value;
                this.Invalidate();
            }
        }

        public WATGroupBox()
        {
            this.borderColor = Color.Aqua;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Size tSize = TextRenderer.MeasureText(this.Text, this.Font);
            Rectangle borderRect = e.ClipRectangle;
            borderRect.Y += tSize.Height / 2;
            borderRect.Height -= tSize.Height / 2;
            ControlPaint.DrawBorder(e.Graphics, borderRect, this.borderColor, ButtonBorderStyle.Solid);

            Rectangle textRect = e.ClipRectangle;
            textRect.X += 6;
            textRect.Width = tSize.Width;
            textRect.Height = tSize.Height;
            e.Graphics.FillRectangle(new SolidBrush(this.BackColor), textRect);
            e.Graphics.DrawString(this.Text, this.Font, new SolidBrush(this.ForeColor), textRect);
        }
    }

    public static class ExtensionMethods
    {
        public static void DoubleBuffered(this DataGridView dgv, bool setting)
        {
            Type dgvType = dgv.GetType();
            PropertyInfo pi = dgvType.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            pi.SetValue(dgv, setting, null);
        }
    }
}
