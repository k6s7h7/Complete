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

namespace account_book
{
    public partial class Search : Form
    {
        public string type;
        public ComboBox combobox;
        public TextBox text;
        public DataGridView dgv;
        public DateTimePicker sdtp, edtp;
        public Label sumlabel = new Label();
        public Collectdata coldata;
        public List<Data> sortlist;
        public BindingList<Data> bindlist;

        public Search(string type)
        {
            InitializeComponent();
            this.type = type;
        }

        private void Search_Load(object sender, EventArgs e)
        {
            Find.search = this;
            this.BackColor = Color.FromArgb(218, 223, 232);
            this.AutoScroll = true;
            sortlist = new List<Data>();
            bindlist = new BindingList<Data>(sortlist);
            coldata = new Collectdata();
            makedgv();
            makedtp();
            makelabel();
            searchtype();
            this.FormClosing += new FormClosingEventHandler(closeform);
        }

        public void closeform(object sender, FormClosingEventArgs e)
        {
            this.Dispose();
        }

        public void makedgv()
        {
            dgv = new DataGridView();
            dgv.Bounds = new Rectangle(30, 120, 1480, 800);
            dgv.RowTemplate.Height = 25;
            dgv.ColumnHeadersHeight = 35;
            this.Controls.Add(dgv);
            ExtensionMethods.DoubleBuffered(dgv, true);

            dgv.DataSource = bindlist;
            dgv.Font = new Font(Statics.font, 12, FontStyle.Bold);
            for (int i = 0; i < Statics.header.Length; i++) dgv.Columns[i].HeaderText = Statics.header[i];
            for (int i = 0; i < Statics.width.Length; i++) dgv.Columns[i].Width = Statics.width[i];
            Globals.house.basicsetdgv(dgv, 8);
            dgv.Columns[0].Visible = false;
        }

        public void makedtp()
        {
            sdtp = new DateTimePicker(); 
            edtp = new DateTimePicker();

            sdtp.Parent = this;
            sdtp.Font = new Font(Statics.font, 14, FontStyle.Bold);
            sdtp.Bounds = new Rectangle(110, 60, 180, 30);
            sdtp.DropDownAlign = LeftRightAlignment.Left;
            sdtp.CustomFormat = "yyyy-MM-dd";
            sdtp.Format = DateTimePickerFormat.Custom;

            edtp.Parent = this;
            edtp.Font = new Font(Statics.font, 14, FontStyle.Bold);
            edtp.Bounds = new Rectangle(360, 60, 180, 30);
            edtp.DropDownAlign = LeftRightAlignment.Left;
            edtp.CustomFormat = "yyyy-MM-dd";
            edtp.Format = DateTimePickerFormat.Custom;
        }

        public void makelabel()
        {
            Label[] label = new Label[6];
            string[] name = new string[] { "검색 시작일", "검색 마지막일", type, "검    색", "합    계" };
            int[,] rectangle = new int[,] { { 120, 20, 150, 30 }, { 370, 20, 150, 30 }, { 600, 20, 200, 30 }, { 900, 50, 140, 45, }, { 1100, 50, 140, 40 } };

            for(int i = 0; i < 5; i++)
            {
                label[i] = new Label();
                label[i].Parent = this;
                label[i].Text = name[i];
                label[i].Bounds = new Rectangle(rectangle[i, 0], rectangle[i, 1], rectangle[i, 2], rectangle[i, 3]);
                label[i].Font = new Font(Statics.font, 14, FontStyle.Bold);
                label[i].TextAlign = ContentAlignment.MiddleCenter;
            }
            label[3].Image = Image.FromFile(Statics.dirpath + "\\button\\normal.png");
            label[3].Font = new Font(Statics.font, 16, FontStyle.Bold);
            label[4].Image = Image.FromFile(Statics.dirpath + "\\button\\item.png");
            sumlabel.Parent = this;
            sumlabel.Bounds = new Rectangle(1250, 50, 250, 40);
            sumlabel.Font = new Font(Statics.font, 14, FontStyle.Bold);
            sumlabel.TextAlign = ContentAlignment.MiddleLeft;
            label[3].MouseHover += new EventHandler(Globals.house.savedelhov);
            label[3].MouseLeave += new EventHandler(Globals.house.savedellve);
            label[3].Click += new EventHandler(clicksearch);
        }

        public void clicksearch(object sender, EventArgs e)
        {
            ((Label)sender).Image = Image.FromFile(Statics.dirpath + "\\button\\click.png");
            dgv.Rows.Clear();
            bindlist.Clear();
            coldata.numlist = 0;
            coldata.searchfile();
            sortlist.Sort(delegate (Data x, Data y) {
                int rtn = y.date.CompareTo(x.date);
                if (rtn == 0) return y.num.CompareTo(x.num);
                else return rtn;
            });
        }
        
        public void makecombo()
        {
            combobox = new ComboBox();
            combobox.Parent = this;
            combobox.Font = new Font(Statics.font, 14, FontStyle.Bold);
            combobox.BackColor = Color.FromArgb(209, 220, 245);
            combobox.Bounds = new Rectangle(600, 60, 200, 40);
            combobox.DrawMode = DrawMode.OwnerDrawFixed;
            combobox.DropDownStyle = ComboBoxStyle.DropDownList;
            combobox.DrawItem += new DrawItemEventHandler(Globals.house.inputgroup.drawitem);
            combobox.DroppedDown = true;
        }

        public void maketextbox()
        {
            text = new TextBox();
            text.Parent = this;
            text.Font = new Font(Statics.font, 14, FontStyle.Bold);
            text.BackColor = Color.FromArgb(209, 220, 245);
            text.Bounds = new Rectangle(600, 60, 200, 40);
            text.ImeMode = ImeMode.Hangul;
            text.Enter += new EventHandler(Globals.house.inputgroup.moneyfocus);
            this.ActiveControl = text;
        }
        public void searchtype()
        {
            switch(type)
            {
                case "유형별":
                    makecombo();
                    combobox.Items.AddRange(Globals.house.titemlist.ToArray());
                    break;
                case "계정이름별":
                    makecombo();
                    for (int i = 0; i < Globals.house.banklist.Count; i++) combobox.Items.Add(Globals.house.banklist[i].name);
                    for (int i = 0; i < Globals.house.cardlist.Count; i++) combobox.Items.Add(Globals.house.cardlist[i].name);
                    break;
                case "큰항목별":
                    makecombo();
                    combobox.Sorted = true;
                    combobox.Items.AddRange(Globals.house.mnglist.pickuparray(Globals.house.oitemlist));
                    break;
                case "작은항목별":
                    makecombo();
                    combobox.Sorted = true;
                    combobox.Items.AddRange(Globals.house.mnglist.pickuparray(Globals.house.sitemlist));
                    break;
                case "금액별":
                    string[] money = { "만원미만", "만원이상", "2만원이상", "3만원이상", "4만원이상", "5만원이상", "10만원이상", "100만원이상" };
                    makecombo();
                    combobox.Items.AddRange(money);
                    break;
                case "상세내용별":
                case "메모별":
                    maketextbox();
                    text.Focus();
                    break;
                default:
                    break;
            }
        }   
    }

    public class Find
    {
        public static Search search;
    }

    public class Collectdata
    {
        public string sday, eday;
        public double sum;
        public int numlist = 0;

        // 검색 시작일자와 끝날일자 사이 검색
        public void searchfile()
        {
            string startmonth, endmonth;
            int syear, smonth, eyear, emonth; 
            // 기간 시작 날짜
            startmonth = Find.search.sdtp.Value.ToString("yyyy MM");
            syear = Int32.Parse(startmonth.Substring(0, 4));
            smonth = Int32.Parse(startmonth.Substring(5, 2));
            sday = Find.search.sdtp.Value.ToString("yyyy-MM-dd");
            // 기간 끝나는 날짜
            endmonth = Find.search.edtp.Value.ToString("yyyy MM");
            eyear = Int32.Parse(endmonth.Substring(0, 4));
            emonth = Int32.Parse(endmonth.Substring(5, 2));
            eday = Find.search.edtp.Value.ToString("yyyy-MM-dd");
            // 시작년도가 끝나는연도 보다 작으면 
            sum = 0;
            while (syear < eyear)
            {
                while (smonth <= 12)
                {
                    startmonth = syear.ToString() + smonth.ToString(" 00");
                    string selectfile = Statics.dirpath + @"\data\" + startmonth + ".dat";
                    System.IO.FileInfo tr = new System.IO.FileInfo(selectfile);
                    if (tr.Exists) searchrecoder(selectfile);
                    smonth++;
                }
                smonth = 1;
                syear++;
            }
            // 시작년도와 끝날년도가 같을 때 시작월이 끝날월 보다 작거나 같으면
            if (syear == eyear) while (smonth <= emonth)
            {
                startmonth = syear.ToString() + smonth.ToString(" 00");
                string selectfile = Statics.dirpath + @"\data\" + startmonth + ".dat";
                System.IO.FileInfo tr = new System.IO.FileInfo(selectfile);
                // 기간 내의 월과 일치하는 파일이 있으면 데이터 읽어 오기
                if (tr.Exists) searchrecoder(selectfile);
                smonth++;
            }
            if (sum == 0)
            {
                MessageBox.Show("자료가 없습니다");
                return;
            }
            Find.search.sumlabel.Text = sum.ToString("#,##0원");
        }

        // 조건에 맞는 파일을 찾아 기간 내의 날짜를 sclist 에 넣기
        public void searchrecoder(string filename)
        {
            string line = " ";
            string[] tmp = new string[9];

            StreamReader sr = new StreamReader(filename);
            while ((line = sr.ReadLine()) != null)
            {
                tmp = line.Split('|');
                if (String.Compare(tmp[0], sday) >= 0 && String.Compare(eday, tmp[0]) >= 0 && selectdata(tmp))
                {
                    Find.search.bindlist.Add(new Data(numlist++, tmp[0], tmp[1], tmp[2], tmp[3], Globals.house.mnglist.findname(Globals.house.oitemlist, Int32.Parse(tmp[4])),
                    Globals.house.mnglist.findname(Globals.house.sitemlist, Int32.Parse(tmp[5])), tmp[6], tmp[7], double.Parse(tmp[8])));
                    sum += double.Parse(tmp[8]);
                }
            }
            sr.Close();
        }

        // 레코드가 조건에 맞으면 True 반환
        public bool selectdata(string[] dat)
        {
            switch (Find.search.type)
            {
                case "유형별":
                    if (Find.search.combobox.SelectedItem.ToString() == dat[1]) return true;
                    break;
                case "계정이름별":
                    if (Find.search.combobox.SelectedItem.ToString() == dat[2]) return true;
                    break;
                case "큰항목별":
                    if (Find.search.combobox.SelectedItem.ToString() == Globals.house.mnglist.findname(Globals.house.oitemlist, Int32.Parse(dat[4]))) return true;
                    break;
                case "작은항목별":
                    if (Find.search.combobox.SelectedItem.ToString() == Globals.house.mnglist.findname(Globals.house.sitemlist, Int32.Parse(dat[5]))) return true;
                    break;
                case "금액별":
                    if (Find.search.combobox.SelectedIndex == 0)
                    {
                        if (double.Parse(dat[8]) < 10000) return true;
                    }
                    else
                    {
                        int[] selmoney = { 10000, 20000, 30000, 40000, 50000, 100000, 1000000 };
                        if (double.Parse(dat[8]) > selmoney[Find.search.combobox.SelectedIndex - 1]) return true;
                    }
                    break;
                case "상세내용별":
                    if (dat[6].Contains(Find.search.text.Text)) return true;
                    break;
                case "메모별":
                    if (dat[7].Contains(Find.search.text.Text)) return true;
                    break;
                default: break;
            }
            return false;
        }
    }
}
