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
using System.IO.Compression;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Drawing.Printing;

namespace account_book
{
    public partial class House : Form
    {
        public Size sizescr = new Size();
        public Stringchange strchn;
        public DataGridView mdgv;
        public InputGroup inputgroup;
        public Autocomp auto;
        public Recovery recovery;

        public List<Listdata> oitemlist, sitemlist;
        public List<string> titemlist, month = new List<string>();
        public List<int> intervallist;
        public BindingList<Bank> banklist;
        public BindingList<Card> cardlist;
        public BindingList<Data> blist;
        public List<Data> list;

        public Filemanager fm;
        public Datafilemanager dfm;
        public Makemenu mkm;
        public Account account;
        public Search search;
        public Category category;
        public Statistics stat;
        public Calcu calcu;
        public ManageList mnglist;
        public ComboBox moncombo;

        public House()
        {
            InitializeComponent();
        }

        private void House_Load(object sender, EventArgs e)
        {
            Globals.house = this;

            
            intervallist = new List<int>();
            oitemlist = new List<Listdata>();
            sitemlist = new List<Listdata>();
            titemlist = new List<string>();
            banklist = new BindingList<Bank>();
            cardlist = new BindingList<Card>();
            list = new List<Data>();
            blist = new BindingList<Data>(list);
            housescreen();
            makedgv();
            savedel();
            inputgroup = new InputGroup();
            strchn = new Stringchange();
            recovery = new Recovery();
            fm = new Filemanager();
            dfm = new Datafilemanager();
            mnglist = new ManageList();
            calcu = new Calcu();
            fm.basicfilemain();
            dfm.datafilemain();
            auto = new Autocomp();
            monthcombo();
            settingdgv();
            new Makemenu();
            inputgroup.makecombo();
            this.FormClosing += new FormClosingEventHandler(closeform);
        }

        public void closeform(object sender, FormClosingEventArgs e)
        {
            endprogram();
        }

        public void endprogram()
        { 
            recovery.backup();
            if (Statics.dgvchk)
            {
                if (MessageBox.Show("변경 내용이 있습니다. 저장하시겠습니까?", "변경내용 저장", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    Globals.house.fm.writefile();
                    Globals.house.dfm.writefile();
                }
            }
            this.Dispose();
        }

        // 해상도를 구하여 전체화면으로
        public void housescreen()
        {
            sizescr = Screen.PrimaryScreen.WorkingArea.Size;
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = Color.FromArgb(218, 223, 232);
            this.AutoScroll = true;
        }

        public void makedgv()
        {
            mdgv = new DataGridView();
            mdgv.Location = new Point(410, 50);
            mdgv.Size = new Size(sizescr.Width - 440, sizescr.Height - 102);
            mdgv.RowTemplate.Height = 25;
            mdgv.ColumnHeadersHeight = 35;
            this.Controls.Add(mdgv);
            ExtensionMethods.DoubleBuffered(mdgv, true);
        }

        // 계정관리 생성, 설정하고 Account 열기
        public void makeaccount()
        {
            account = new Account();
            account.StartPosition = FormStartPosition.Manual;
            account.Location = new Point(0, 0);
            account.Size = new Size(720, 610);
            account.Show();
        }
        // 검색창 생성하고 열기
        public void makesearch(string type)
        {
            search = new Search(type);
            search.StartPosition = FormStartPosition.Manual;
            search.Location = new Point(0, 0);
            search.Size = new Size(1550, 1000);
            search.Show();
        }
        // 항목 관리 창 생성하고 열기
        public void makecategory()
        {
            category = new Category();
            category.StartPosition = FormStartPosition.Manual;
            category.Location = new Point(0, 0);
            category.Size = new Size(470, 700);
            category.Show();
        }
        // 통계창 생성
        public void makestatistics()
        {
            stat = new Statistics();
            stat.StartPosition = FormStartPosition.Manual;
            stat.Location = new Point(0, 0);
            stat.Size = new Size(1920, 1080);
            stat.Show();
        }

        // dgv 설정
        public void basicsetdgv(DataGridView dgv, int num)
        {
            dgv.DefaultCellStyle.BackColor = Color.FromArgb(240, 248, 255);
            dgv.DefaultCellStyle.SelectionForeColor = Color.Yellow;
            dgv.RowHeadersVisible = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv.ReadOnly = true;
            dgv.Columns[num].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgv.Columns[num].DefaultCellStyle.Format = "#,##0원";
            dgv.AllowUserToAddRows = false;
        }

        public void settingdgv()
        {
            // 컬럼명 쓰는 것이 바인딩하는 것 보다 위에 있으면 컬럼명 쓸 때 인덱스 오류남 
            mdgv.DataSource = blist;
            mdgv.Font = new Font(Statics.font, 12, FontStyle.Bold);
            for (int i = 0; i < Statics.header.Length; i++) mdgv.Columns[i].HeaderText = Statics.header[i];
            for (int i = 0; i < Statics.width.Length; i++) mdgv.Columns[i].Width = Statics.width[i];

            basicsetdgv(mdgv, 8);
            mdgv.Columns[0].Visible = false;
            mdgv.CellClick += new DataGridViewCellEventHandler(dgvclick);
            mdgv.DataBindingComplete += new DataGridViewBindingCompleteEventHandler(changedgv);
            clickcell(mdgv, 0);
        }

        public void clickcell(DataGridView dgv, int currow)
        {
            if (dgv.RowCount < 1) return;
            dgv.Columns[1].Selected = true;
            dgv.Rows[currow].Selected = true;
            dgv.CurrentCell = dgv[1, currow];
            Statics.currentrow = currow;
            if(dgv == mdgv)Statics.curnum = blist[currow].num;
        }

        public void sortdgv()
        {
            list.Sort(delegate (Data x, Data y) {
                int rtn = y.date.CompareTo(x.date);
                if (rtn == 0) return y.num.CompareTo(x.num);
                else return rtn;
            });
            mdgv.Refresh();
        }

        public void searchcell()
        {
            int i = 0;
            foreach (DataGridViewRow row in mdgv.Rows)
            {
                if ((int)row.Cells[0].Value == Statics.curnum) break;
                i++;
            }
            Statics.currentrow = i;
            clickcell(mdgv, Statics.currentrow);
        }

        public void dgvclick(object sender, EventArgs e)
        {
            Statics.currentrow = mdgv.CurrentCell.RowIndex;
            Statics.curnum = (int)mdgv[0, Statics.currentrow].Value;
            inputgroup.dtp.Text = blist[Statics.currentrow].date;
            for (int i = 0; i < 4; i++) inputgroup.combo[i].DropDownStyle = ComboBoxStyle.Simple;
            inputgroup.combo[0].Text = blist[Statics.currentrow].bigitem;
            inputgroup.combo[1].Text = blist[Statics.currentrow].smallitem;
            inputgroup.combo[2].Text = blist[Statics.currentrow].type;
            inputgroup.combo[3].Text = blist[Statics.currentrow].cntname;
            if (inputgroup.combo[2].SelectedIndex == 4) for (int i = 0; i < Globals.house.banklist.Count; i++) inputgroup.combo[4].Items.Add(Globals.house.banklist[i].name);
            inputgroup.combo[4].Text = blist[Statics.currentrow].deposit;
            for (int i = 0; i < 4; i++) inputgroup.combo[i].DropDownStyle = ComboBoxStyle.DropDownList;
            inputgroup.moneytb.Text = blist[Statics.currentrow].money.ToString("#,##0원");
            inputgroup.commenttb.Text = blist[Statics.currentrow].detail;
            inputgroup.memotb.Text = blist[Statics.currentrow].memo;
            inputgroup.moneytb.Select(inputgroup.moneytb.TextLength - 1, 0);
        }

        public void changedgv(object sender, EventArgs e)
        {
            Statics.dgvchk = true;
        }

        public void savedel()
        {
            Label save = new Label(), del = new Label();

            save.Parent = Globals.house;
            save.Location = new Point(1000, 5);
            save.Size = new Size(140, 45);
            save.Text = "저    장";
            save.Font = new Font(Statics.font, 16, FontStyle.Bold);
            save.TextAlign = ContentAlignment.MiddleCenter;
            save.Image = Image.FromFile(Statics.dirpath + "\\button\\normal.png");
            save.MouseHover += new EventHandler(savedelhov);
            save.MouseLeave += new EventHandler(savedellve);
            save.Click += new EventHandler(saveclk);

            del.Parent = Globals.house;
            del.Location = new Point(1400, 5);
            del.Size = new Size(140, 45);
            del.Text = "삭    제";
            del.Font = new Font(Statics.font, 16, FontStyle.Bold);
            del.TextAlign = ContentAlignment.MiddleCenter;
            del.Image = Image.FromFile(Statics.dirpath + "\\button\\normal.png");
            del.MouseHover += new EventHandler(savedelhov);
            del.MouseLeave += new EventHandler(savedellve);
            del.Click += new EventHandler(delclk);
        }

        public void savedelhov(object sender, EventArgs e)
        {
            ((Label)sender).Image = Image.FromFile(Statics.dirpath + "\\button\\hover.png");
        }

        public void savedellve(object sender, EventArgs e)
        {
            ((Label)sender).Image = Image.FromFile(Statics.dirpath + "\\button\\normal.png");
        }

        public void saveclk(object sender, EventArgs e)
        {
            ((Label)sender).Image = Image.FromFile(Statics.dirpath + "\\button\\click.png");

            fm.writefile();
            dfm.writefile();
            Statics.dgvchk = false;
        }

        public void delclk(object sender, EventArgs e)
        {
            if (blist.Count < 1) return;
            ((Label)sender).MouseLeave -= savedellve;
            ((Label)sender).Image = Image.FromFile(Statics.dirpath + "\\button\\click.png");
            dgvclick(sender, e);
            if (MessageBox.Show("삭제합니다. 확실하십니까?", "지정한 행삭제", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                inputgroup.finddelitem();
                mdgv.Rows.Remove(mdgv.Rows[Statics.currentrow]);
                calcu.showsum();
                if (blist.Count < 1)
                {
                    ((Label)sender).MouseLeave += savedellve;
                    return;
                }
                if (blist.Count <= Statics.currentrow) Statics.currentrow = blist.Count - 1;
                clickcell(mdgv, Statics.currentrow);
            }
            ((Label)sender).MouseLeave += savedellve;
            ((Label)sender).Image = Image.FromFile(Statics.dirpath + "\\button\\normal.png");
        }

        public void monthcombo()
        {
            DirectoryInfo di = new DirectoryInfo(Statics.dirpath + "\\Data");
            foreach (FileInfo File in di.GetFiles())
            {
                if (File.Extension.ToLower().CompareTo(".dat") == 0)
                    month.Add(File.Name.Substring(0, File.Name.Length - 4));
            }
            moncombo = new ComboBox();
            moncombo.Items.Clear();
            moncombo.Parent = this;
            moncombo.Font = new Font(Statics.font, 14, FontStyle.Bold);
            moncombo.BackColor = Color.FromArgb(209, 220, 245);
            moncombo.Location = new Point(600, 10);
            moncombo.Size = new Size(120, 40);
            moncombo.Items.AddRange(month.ToArray());
            moncombo.Sorted = true;
            moncombo.SelectedItem = Statics.filename;
            moncombo.SelectedIndexChanged += new System.EventHandler(monthdgv);
            moncombo.DrawMode = DrawMode.OwnerDrawFixed;
            moncombo.DropDownStyle = ComboBoxStyle.DropDownList;
            moncombo.DrawItem += new DrawItemEventHandler(inputgroup.drawitem);
        }

        public void monthdgv(object sender, EventArgs e)
        {
            if (changemonth(((ComboBox)sender).Text) == 1) clickcell(mdgv, 0);
        }

        public int changemonth(string mon)
        {
            if (mon == Statics.filename) return 0;
            if (Statics.dgvchk)
            {
                if (MessageBox.Show("변경 내용이 있습니다. 저장하시겠습니까?", "변경내용 저장", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    Globals.house.fm.writefile();
                    Globals.house.dfm.writefile();
                }
            }
            blist.Clear();
            Statics.filename = mon;
            fm.basicfilemain();
            dfm.datafilemain();
            Statics.dgvchk = false;
            if (!month.Contains(Statics.filename)) {
                month.Add(Statics.filename);
                moncombo.Items.Clear();
                moncombo.Items.AddRange(month.ToArray());
            }
            moncombo.SelectedItem = Statics.filename;
            return 1;
        }
    }

    public class Globals
    {
        public static House house;
    }
    // dvg 로딩 속도 개선하기 위해 DoubleBuffered 실행 함수
    public static class ExtensionMethods
    {
        public static void DoubleBuffered(this DataGridView dgv, bool setting)
        {
            Type dgvType = dgv.GetType();
            PropertyInfo pi = dgvType.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            pi.SetValue(dgv, setting, null);
        }
    }

    public class Calcu
    {
        public Label[] money;
        public ComboBox[] combo;

        public Calcu()
        {
            makelabel();
            makecombo();
        }
        public void makelabel()
        {
            string[] name = new string[] { "통장 잔액", "카드잔액", "현금 잔액", "월 수입", "월 지출", "월 차액" };
            Label[] label = new Label[name.Length];

            money = new Label[name.Length];
            for (int i = 0; i < name.Length; i++)
            {
                label[i] = new Label();
                label[i].Parent = Globals.house;
                label[i].Text = name[i];
                label[i].Bounds = new Rectangle(20, 500 + i * 80 + (i / 3) * 20, 142, 32);
                label[i].Font = new Font(Statics.font, 16, FontStyle.Bold);
                label[i].TextAlign = ContentAlignment.MiddleCenter;
                label[i].Image = Image.FromFile(Statics.dirpath + "\\button\\item.png");

                money[i] = new Label();
                money[i].Parent = Globals.house;
                money[i].Bounds = new Rectangle(170, 500 + i * 80 + (i / 3) * 20, 220, 32);
                money[i].Font = new Font(Statics.font, 16, FontStyle.Bold);
                money[i].TextAlign = ContentAlignment.MiddleRight;
            }
        }

        public void makecombo()
        {
            combo = new ComboBox[2];

            for (int i = 0; i < 2; i++)
            {
                combo[i] = new ComboBox();
                combo[i].Parent = Globals.house;
                combo[i].Font = new Font(Statics.font, 14, FontStyle.Bold);
                combo[i].BackColor = Color.FromArgb(209, 220, 245);
                combo[i].Bounds = new Rectangle(180, 500 + i * 80 + (i / 3) * 20 + 40, 210, 40);
                combo[i].DrawMode = DrawMode.OwnerDrawFixed;
                combo[i].DropDownStyle = ComboBoxStyle.DropDownList;
                combo[i].DrawItem += new DrawItemEventHandler(Globals.house.inputgroup.drawitem);
            }
            combo[0].SelectedIndexChanged += new System.EventHandler(bankcombo);
            combo[1].SelectedIndexChanged += new System.EventHandler(cardcombo);
            sumitem();
        }

        public void bankcombo(object sender, EventArgs e)
        {
            double sum = 0;
            if (((ComboBox)sender).Text == "합    계")
            {
                for (int i = 0; i < Globals.house.banklist.Count; i++) sum += Globals.house.banklist[i].money;
                money[0].Text = sum.ToString("#,##0원");
            }
            else money[0].Text = Globals.house.banklist[combo[0].FindStringExact(((ComboBox)sender).Text) - 1].money.ToString("#,##0원");
        }

        public void cardcombo(object sender, EventArgs e)
        {
            double sum = 0;
            if (((ComboBox)sender).Text == "합    계")
            {
                for (int i = 0; i < Globals.house.cardlist.Count; i++) sum += Globals.house.cardlist[i].money;
                money[1].Text = sum.ToString("#,##0원");
            }
            else money[1].Text = Globals.house.cardlist[combo[1].FindStringExact(((ComboBox)sender).Text) - 1].money.ToString("#,##0원");
        }

        public void sumitem()
        {
            combo[0].Items.Clear();
            combo[0].Items.Add("합    계");
            for (int i = 0; i < Globals.house.banklist.Count; i++) combo[0].Items.Add(Globals.house.banklist[i].name);
            combo[0].SelectedIndex = 0;

            combo[1].Items.Clear();
            combo[1].Items.Add("합    계");
            for (int i = 0; i < Globals.house.cardlist.Count; i++) combo[1].Items.Add(Globals.house.cardlist[i].name);
            combo[1].SelectedIndex = 0;

            money[2].Text = Statics.cash.ToString("#,##0원");
        }

        public void showcalcu()
        {
            money[3].Text = Statics.income.ToString("#,##0원");
            money[4].Text = Statics.expend.ToString("#,##0원");
            money[5].Text = (Statics.income - Statics.expend).ToString("#,##0원");
        }



        public void summonth()
        {
            Statics.income = Statics.expend = 0;
            for (int i = 0; i < Globals.house.blist.Count; i++)
            {
                if (Globals.house.blist[i].type == "현금수입" || Globals.house.blist[i].type == "통장수입" || Globals.house.blist[i].type == "카드수입")
                    Statics.income += Globals.house.blist[i].money;
                else if (Globals.house.blist[i].type == "현금지출" || Globals.house.blist[i].type == "통장지출" || Globals.house.blist[i].type == "카드지출")
                    Statics.expend += Globals.house.blist[i].money;
            }
            showcalcu();
        }

        public void showsum()
        {
            sumitem();
            summonth();
        }

    }
    public class Statics
    {
        public static int numlist = 0, currentrow = 0, curnum = 0;
        public static string font = "휴먼신그래픽", dirpath = Directory.GetCurrentDirectory(), filename = DateTime.Now.ToString("yyyy MM");
        public static double cash = 0, income = 0, expend = 0;
        public static Boolean dgvchk = false;
        public static string[] header = new string[] { "번호", "날짜", "유형", "계정이름", "입금통장", "큰항목", "작은항목", "상세내용", "금    액", "메        모" };
        public static int[] width = new int[] { 50, 140, 100, 110, 110, 130, 130, 200, 170, 368 };
    }

    public class Menulist
    {
        public string[] mainitem = new string[] { "파일", "검색", "관리", "도구", "도움말" },
            fileitem = new string[] { "저장", "인쇄", "종료" },
            searchitem = new string[] { "유형별", "계정이름별", "큰항목별", "작은항목별", "금액별", "상세내용별", "메모별" },
            manageitem = new string[] { "계정관리", "항목관리" },
            toolitem = new string[] { "복구", "통계" };
    }

    public class Items
    {
        public int[] iteminterval = { 6, 6, 7, 2, 5, 2, 7, 8, 4, 4, 3, 7, 5, 6, 5, 5, 7, 4, 4, 2, 8, 7, 2, 3, 3 };
        public string[] objitem = new string[] { "근로소득", "금융소득", "기타소득", "돈거래수입", "내부거래", "돈거래지출", "식비", "패션/미용", "교통비", "통신비", "은행", "저축", "문화생활비",
            "자기계발/투자", "경조사비", "의료비", "내구소비재", "보험료", "차량유지비", "주거비", "세금&공과금", "육아/교육", "사고팔고", "종교관련", "기타" },
        subitem = new string[] { "급여", "수당", "상여금", "퇴직금", "아르바이트", "기타근로소득",     "이자소득", "임대수입", "보험금", "배당금", "투자이익", "기타금융소득",
            "현금수입", "할인소득", "상품권", "상금", "환불", "잡소득", "잠시보관",  "빛받은돈", "빌린돈",   "통장간이체", "현금인출", "현금입금", "카드결재", "현금서비스",  "빌려준돈", "빛갚은돈",
            "재료비", "외식", "배달/포장", "커피/음료", "술/유흥", "간식", "건강식품",      "의류", "신발", "화장품", "헤어", "엑세서리", "잡화", "피부/바디", "세탁/수선",
            "대중교통", "택시", "기차", "기타교통비",      "핸드폰", "인터넷", "유선방송", "집전화",      "대출이자", "원금상환", "수수료",
            "청약저축", "예금", "적금", "펀드", "주식", "연금", "기타저축",      "관람료", "음반", "레저", "여행", "기타문화생활",
            "복권", "도서", "시험응시료", "강의", "운동", "기타투자",      "용돈", "경조사비", "회비", "선물", "후원금",      "진료비", "약값", "수술비", "검사비", "기타의료비",
            "가전제품", "가구", "주방/욕실", "생활용품", "인테리어", "일용품", "잡화",      "생명보험", "손해보험", "자동차보험", "운전자보험",
            "유류비", "주차비", "수리비", "통행료",      "집세", "관리비",      "전기요금", "수도요금", "가스요금", "재산세", "주민세", "자동차세", "배당금세금", "기타세금",
            "분유/기저귀", "아기용품 ", "보육비", "학비", "학원비", "교재비", "기타교육비",      "배송비", "환불",      "봉헌금", "교무금", "기타종교비용",   "이웃돕기", "잡비", "불분명" },
        typeitem = new string[] { "현금수입", "현금지출", "통장수입", "통장지출", "통장간이체", "현금입금", "현금인출", "카드수입", "카드지출", "카드결재", "현금서비스" };

        public Items()
        {
            int i = 0;
            foreach (int item in iteminterval) Globals.house.intervallist.Add(item);
            foreach (string item in objitem) Globals.house.oitemlist.Add(new Listdata(i++, item));
            i = 0;
            foreach (string item in subitem) Globals.house.sitemlist.Add(new Listdata(i++, item));
            Globals.house.titemlist.AddRange(typeitem);
        }
    }

    public class Listdata
    {
        public int num;
        public string name;

        public Listdata() { }
        public Listdata(int num, string name)
        {
            this.num = num;
            this.name = name;
        }
    }

    public class Bank
    {
        public string name;
        public double money;

        public Bank(string name, double money)
        {
            this.name = name;
            this.money = money;
        }
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public double Money
        {
            get { return money; }
            set { money = value; }
        }
    }

    public class Card
    {
        public string name;
        public string payment;
        public double money;

        public Card(string name, string payment, double money)
        {
            this.name = name;
            this.payment = payment;
            this.money = money;
        }
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public string Payment
        {
            get { return payment; }
            set { payment = value; }
        }
        public double Money
        {
            get { return money; }
            set { money = value; }
        }
    }

    public class Data
    {
        public int num;
        public string date, type, cntname, deposit, bigitem, smallitem, detail, memo;
        public double money;

        public Data(int num, string date, string type, string cntname, string deposit, string bigitem, string smallitem, string detail, string memo, double money)
        {
            this.num = num;
            this.date = date;
            this.type = type;
            this.cntname = cntname;
            this.deposit = deposit;
            this.bigitem = bigitem;
            this.smallitem = smallitem;
            this.detail = detail;
            this.memo = memo;
            this.money = money;
        }
        public int Num
        {
            get { return num; }
            set { num = value; }
        }
        public string Date
        {
            get { return date; }
            set { date = value; }
        }
        public string Type
        {
            get { return type; }
            set { type = value; }
        }
        public string Cntname
        {
            get { return cntname; }
            set { cntname = value; }
        }
        public string Deposit
        {
            get { return deposit; }
            set { deposit = value; }
        }
        public string Bigitem
        {
            get { return bigitem; }
            set { bigitem = value; }
        }
        public string Smallitem
        {
            get { return smallitem; }
            set { smallitem = value; }
        }
        public string Detail
        {
            get { return detail; }
            set { detail = value; }
        }
        public double Money
        {
            get { return money; }
            set { money = value; }
        }
        public string Memo
        {
            get { return memo; }
            set { memo = value; }
        }
    }

    public class Makemenu
    {
        public Panel panel = new Panel();
        public Menulist menulist = new Menulist();
        public MenuStrip mainmenu = new MenuStrip();
        public ToolStripMenuItem[] filemenu, searchmenu, managemenu, toolmenu;
        public Printmanager print = new Printmanager();

        public Makemenu()
        {
            ToolStripMenuItem[] fitem = new ToolStripMenuItem[menulist.fileitem.Length];
            ToolStripMenuItem[] sitem = new ToolStripMenuItem[menulist.searchitem.Length];
            ToolStripMenuItem[] mngitem = new ToolStripMenuItem[menulist.manageitem.Length];
            ToolStripMenuItem[] titem = new ToolStripMenuItem[menulist.toolitem.Length];
            ToolStripMenuItem[] stitem = new ToolStripMenuItem[Globals.house.titemlist.Count];
            ToolStripMenuItem[] mitem = new ToolStripMenuItem[menulist.mainitem.Length];

            var verticalPadding = 14 - TextRenderer.MeasureText(" ", new Font(Statics.font, 13)).Height / 2;
            ToolStripManager.Renderer = new MyRenderer
            {
                VerticalPadding = verticalPadding
            };

            for (int i = 0; i < menulist.mainitem.Length; i++)
            {
                mitem[i] = new ToolStripMenuItem();
                mitem[i].Text = menulist.mainitem[i];
                mitem[i].AutoSize = false;
                mitem[i].Size = new Size(100, 34);
                mitem[i].TextAlign = ContentAlignment.TopCenter;
                mainmenu.Items.Add(mitem[i]);
                mitem[i].BackColor = Color.FromArgb(218, 223, 232);
                mitem[i].Font = new Font(Statics.font, 13, FontStyle.Bold);
            }

            for (int i = 0; i < menulist.fileitem.Length; i++)
            {
                fitem[i] = new ToolStripMenuItem();
                fitem[i].Text = menulist.fileitem[i];
                fitem[i].AutoSize = false;
                fitem[i].Size = new Size(162, 30);
                mitem[0].DropDownItems.Add(fitem[i]);
                fitem[i].Click += new EventHandler(file_click);
                fitem[i].BackColor = Color.FromArgb(209, 220, 245);
                fitem[i].Font = new Font(Statics.font, 13, FontStyle.Bold);
            }

            for (int i = 0; i < menulist.searchitem.Length; i++)
            {
                sitem[i] = new ToolStripMenuItem();
                sitem[i].Text = menulist.searchitem[i];
                sitem[i].AutoSize = false;
                sitem[i].Size = new Size(170, 30);
                mitem[1].DropDownItems.Add(sitem[i]);
                sitem[i].Click += new EventHandler(searchitem_click);
                sitem[i].BackColor = Color.FromArgb(209, 220, 245);
                sitem[i].Font = new Font(Statics.font, 13, FontStyle.Bold);
            }

            for (int i = 0; i < menulist.manageitem.Length; i++)
            {
                mngitem[i] = new ToolStripMenuItem();
                mngitem[i].Text = menulist.manageitem[i];
                mngitem[i].AutoSize = false;
                mngitem[i].Size = new Size(162, 30);
                mitem[2].DropDownItems.Add(mngitem[i]);
                mngitem[i].Click += new EventHandler(mngitem_click);
                mngitem[i].BackColor = Color.FromArgb(209, 220, 245);
                mngitem[i].Font = new Font(Statics.font, 13, FontStyle.Bold);
            }

            for (int i = 0; i < menulist.toolitem.Length; i++)
            {
                titem[i] = new ToolStripMenuItem();
                titem[i].Text = menulist.toolitem[i];
                titem[i].AutoSize = false;
                titem[i].Size = new Size(162, 30);
                mitem[3].DropDownItems.Add(titem[i]);
                titem[i].Click += new EventHandler(tools_click);
                titem[i].BackColor = Color.FromArgb(209, 220, 245);
                titem[i].Font = new Font(Statics.font, 13, FontStyle.Bold);
            }

            panel.Controls.Add(mainmenu);
            mainmenu.BackColor = Color.FromArgb(218, 223, 232);
            panel.Location = new Point(0, 0);
            panel.Size = new Size(520, 40);
            Globals.house.Controls.Add(panel);
        }

        public void file_click(object sender, EventArgs e)
        {
            if (((ToolStripMenuItem)sender).Text == menulist.fileitem[0])
            {
                Globals.house.fm.writefile();
                if (Globals.house.blist.Count > 0) Globals.house.dfm.writefile();
            }
            else if (((ToolStripMenuItem)sender).Text == menulist.fileitem[1])
                print.Print_GridView();
            else Globals.house.endprogram();
        }
     
        private void tools_click(object sender, EventArgs e)
        {
            if (((ToolStripMenuItem)sender).Text == menulist.toolitem[0]) Globals.house.recovery.makerecombo();
            else Globals.house.makestatistics();
        }

        public void searchitem_click(object sender, EventArgs e)
        {
            Globals.house.makesearch(sender.ToString());
        }

        private void mngitem_click(object sender, EventArgs e)
        {
            if (((ToolStripMenuItem)sender).Text == menulist.manageitem[0]) Globals.house.makeaccount();
            else Globals.house.makecategory();
        }
    }

    // 메뉴 텍스트의 위치와 색상 변경
    public class MyRenderer : ToolStripProfessionalRenderer
    {
        public MyRenderer() : base(new MyColors()) { }
        public int VerticalPadding { get; set; }
        protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
        {
            if (null == e)
            { return; }
            e.TextFormat &= ~TextFormatFlags.HidePrefix;
            e.TextFormat |= TextFormatFlags.VerticalCenter;
            var rect = e.TextRectangle;
            rect.Offset(0, VerticalPadding);
            e.TextRectangle = rect;
            base.OnRenderItemText(e);
        }
    }

    // 메뉴가 선택되었을 때 색상
    public class MyColors : ProfessionalColorTable
    {
        public override Color MenuItemSelected
        {
            get { return Color.Gold; }
        }

        public override Color MenuItemSelectedGradientBegin
        {
            get { return Color.Yellow; }
        }
        public override Color MenuItemSelectedGradientEnd
        {
            get { return Color.Gold; }
        }

        public override Color MenuItemPressedGradientBegin
        {
            get { return Color.Yellow; }
        }
        public override Color MenuItemPressedGradientEnd
        {
            get { return Color.Gold; }
        }
    }

    public class Recovery
    {
        public ComboBox rcombo = new ComboBox();
        public Label rlabel = new Label();
        public List<string> bklist = new List<string>();

        // 백업 파일 만들기
        public void backup()
        {
            string nowday = DateTime.Now.ToString("yyyy MM dd");
            string zippath = Statics.dirpath + "\\BackUp\\";
            string zipfilepath = zippath + nowday + ".bak";
            DirectoryInfo di = new DirectoryInfo(zippath);
            if (!di.Exists) di.Create();

            FileInfo fi = new FileInfo(zipfilepath);
            if (fi.Exists) File.Delete(zipfilepath);
            // Zip 압축파일 생성
            ZipFile.CreateFromDirectory(Statics.dirpath + "\\data\\", zipfilepath);
            foreach (string file in Directory.GetFiles(zippath))
            {
                fi = new FileInfo(file);
                if (fi.LastWriteTime <= DateTime.Now.AddMonths(-1)) fi.Delete();        // 한달이지난파일
            }
        }

        // 콤보박스 생성
        public void makerecombo()
        {
            if (rcombo.Visible == false)
            {
                bevisible();
                return;
            }
            rlabel.Parent = Globals.house;
            rlabel.Text = "복구일자";
            rlabel.Location = new Point(750, 20);
            rlabel.Size = new Size(74, 30);
            rlabel.Font = new Font(Statics.font, 12, FontStyle.Bold);
            rlabel.ForeColor = Color.Red;

            rcombo.Parent = Globals.house;
            rcombo.Location = new Point(830, 15);
            rcombo.Size = new Size(130, 30);
            rcombo.Font = new Font(Statics.font, 12, FontStyle.Bold);
            rcombo.BackColor = Color.FromArgb(209, 220, 245);
            rcombo.Items.Clear();
            DirectoryInfo di = new DirectoryInfo(Statics.dirpath + "\\BackUp\\");
            foreach (var item in di.GetFiles()) bklist.Add(Path.GetFileNameWithoutExtension(item.Name));
            rcombo.Items.AddRange(bklist.ToArray());
            rcombo.SelectedIndex = bklist.Count - 1;
            rcombo.DrawMode = DrawMode.OwnerDrawFixed;
            rcombo.DrawItem += new DrawItemEventHandler(Globals.house.inputgroup.drawitem);
            rcombo.DropDownStyle = ComboBoxStyle.DropDownList;
            rcombo.SelectionChangeCommitted += new System.EventHandler(recoverydata);
            rcombo.DropDownClosed += new System.EventHandler(nonevisible);
        }

        // 콤보박스 숨기기
        public void nonevisible(object sender, EventArgs e)
        {
            rcombo.Visible = false;
            rlabel.Visible = false;
        }
        // 콩보박스 보이기
        public void bevisible()
        {
            rcombo.Visible = true;
            rlabel.Visible = true;
        }

        // 기존파일은 삭제하고 원하는 백업파일 압축 풀어 읽기
        public void recoverydata(object sender, EventArgs e)
        {
            if (MessageBox.Show("지금 자료는 다 없어지고 " + bklist[rcombo.SelectedIndex] +
                " 일자로 자료가 복구됩니다. 계속 하시겠습니까?", "자료복구", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                Directory.Delete(Statics.dirpath + "\\data\\", true);
                ZipFile.ExtractToDirectory(Statics.dirpath + "\\BackUp\\" + bklist[rcombo.SelectedIndex] + ".bak", Statics.dirpath + "\\data\\");
                rcombo.Visible = false;
                readrecovery();
                MessageBox.Show(bklist[rcombo.SelectedIndex] + " 일자로 자료가 복구 되었습니다");
            }
        }

        // 압축 푼 파일 읽기
        public void readrecovery()
        {
            Statics.currentrow = 0;
            Globals.house.fm.readfile();
            Globals.house.dfm.readfile();
        }
    }

    // 그룹박스 클래스
    public class InputGroup
    {
     //   public WATGroupBox gbox = new WATGroupBox();
        public GroupBox gbox = new GroupBox();
        public DateTimePicker dtp;
        public Label[] label;
        public Label input, mend;
        public ComboBox[] combo;
        public TextBox moneytb, commenttb, memotb;
        public int len, cur;

        public InputGroup()
        {
            makegroupbox();
            makedtp();
            makelabel();
            maketextbox();
            makeinput();
        }

        public void makegroupbox()
        {
            gbox.Name = "gbox";
            gbox.Parent = Globals.house;
            gbox.Font = new Font(Statics.font, 14, FontStyle.Bold);
            gbox.BackColor = Color.FromArgb(240, 248, 255);
            gbox.Location = new Point(20, 50);
            gbox.Size = new Size(370, 420);
        }

        public void makedtp()
        {
            dtp = new DateTimePicker();
            dtp.Parent = gbox;
            dtp.Font = new Font(Statics.font, 16, FontStyle.Bold);
            dtp.Location = new Point(70, 10);
            dtp.Size = new Size(280, 30);
            dtp.DropDownAlign = LeftRightAlignment.Left;
        }

        public void makelabel()
        {
            int[,] labellocation = new int[,] { { 10, 10 }, { 10, 54 }, { 190, 54 }, { 10, 130 }, { 190, 130 }, { 10, 206 }, { 190, 206 }, { 10, 276 }, { 10, 314 } };
            string[] labeltext = new string[] { "오늘", "큰항목", "작은항목", "유형", "금액", "계정이름", "입금통장", "내용", "메모" };
            label = new Label[9];

            for (int i = 0; i < 9; i++)
            {
                label[i] = new Label();
                label[i].Text = labeltext[i];
                label[i].Parent = gbox;
                label[i].TextAlign = ContentAlignment.MiddleCenter;
                label[i].Font = new Font(Statics.font, 14, FontStyle.Bold);
                label[i].Location = new Point(labellocation[i, 0], labellocation[i, 1]);
                if (i == 0 || i == 7 || i == 8) label[i].Size = new Size(50, 30);
                else label[i].Size = new Size(170, 30);
            }
            label[0].BackColor = Color.FromArgb(209, 220, 245);
            label[0].MouseHover += new EventHandler(todayhover);
            label[0].MouseLeave += new EventHandler(todayleave);
            label[0].Click += new EventHandler(todayclick);
        }

        public void todayhover(object sender, EventArgs e)
        {
            label[0].BackColor = Color.Gold;
        }

        public void todayleave(object sender, EventArgs e)
        {
            label[0].BackColor = Color.FromArgb(209, 220, 245);
        }

        public void todayclick(object sender, EventArgs e)
        {
            dtp.Value = DateTime.Today;
        }

        public void maketextbox()
        {
            moneytb = new TextBox();            
            moneytb.Parent = gbox;
            moneytb.Font = new Font(Statics.font, 14, FontStyle.Bold);
            moneytb.BackColor = Color.FromArgb(209, 220, 245);
            moneytb.Location = new Point(190, 162);
            moneytb.Size = new Size(170, 30);
            moneytb.TextAlign = HorizontalAlignment.Right;

            moneytb.Click += new EventHandler(moneyclick);
            moneytb.Leave += new EventHandler(moneyleave);
            moneytb.KeyUp += new KeyEventHandler(textkeyup);
            moneytb.KeyDown += new KeyEventHandler(textkeydown);
            moneytb.Enter += new EventHandler(moneyfocus);

            commenttb = new TextBox();
            commenttb.Parent = gbox;
            commenttb.Font = new Font(Statics.font, 14, FontStyle.Bold);
            commenttb.BackColor = Color.FromArgb(209, 220, 245);
            commenttb.Location = new Point(70, 276);
            commenttb.Size = new Size(290, 30);
            commenttb.ImeMode = ImeMode.Hangul;

            memotb = new TextBox();
            memotb.Parent = gbox;
            memotb.Font = new Font(Statics.font, 14, FontStyle.Bold);
            memotb.BackColor = Color.FromArgb(209, 220, 245);
            memotb.Location = new Point(70, 314);
            memotb.Size = new Size(290, 30);
            memotb.ImeMode = ImeMode.Hangul;
        }

        public void moneyclick(object sender, EventArgs e)
        {
            ((TextBox)sender).BackColor = Color.Gold;
        }

        public void moneyleave(object sender, EventArgs e)
        {
            ((TextBox)sender).BackColor = Color.FromArgb(209, 220, 245);
        }

        public void moneyfocus(object sender, EventArgs e)
        {
            ((TextBox)sender).BackColor = Color.Gold;
        }
        
        // 숫자를 입력 받아 천 단위에 콤마찍고 커서는 제위치에 놓기
        public void textkeyup(object sender, KeyEventArgs e)
        {
            ((TextBox)sender).Text = Globals.house.strchn.makecomma(((TextBox)sender).Text);
            if (cur == 0)
            {
                if (e.KeyCode == Keys.Back || e.KeyCode == Keys.Delete || e.KeyCode == Keys.Left) ((TextBox)sender).SelectionStart = 0;
                else ((TextBox)sender).SelectionStart = 1;
            }
            else if (len - 1 == cur)
            {
                if (e.KeyCode == Keys.Left) ((TextBox)sender).SelectionStart = ((TextBox)sender).TextLength - 2;
                else ((TextBox)sender).SelectionStart = ((TextBox)sender).TextLength - 1;
            }
            else switch (e.KeyCode)
            {
                case Keys.Back:
                    if (cur == 1) ((TextBox)sender).SelectionStart = 0;
                    else ((TextBox)sender).SelectionStart = cur + ((TextBox)sender).TextLength - len;
                    break;
                case Keys.Delete:
                    ((TextBox)sender).SelectionStart = cur + ((TextBox)sender).TextLength - len + 1;
                    break;
                case Keys.Left:
                    ((TextBox)sender).SelectionStart = cur - 1;
                    break;
                case Keys.Right:
                    ((TextBox)sender).SelectionStart = cur + 1;
                    break;
                case Keys.OemMinus:
                case Keys.Subtract:
                case Keys.D0:
                case Keys.D1:
                case Keys.D2:
                case Keys.D3:
                case Keys.D4:
                case Keys.D5:
                case Keys.D6:
                case Keys.D7:
                case Keys.D8:
                case Keys.D9:
                case Keys.NumPad0:
                case Keys.NumPad1:
                case Keys.NumPad2:
                case Keys.NumPad3:
                case Keys.NumPad4:
                case Keys.NumPad5:
                case Keys.NumPad6:
                case Keys.NumPad7:
                case Keys.NumPad8:
                case Keys.NumPad9:
                    ((TextBox)sender).SelectionStart = cur + ((TextBox)sender).TextLength - len;
                    break;
                default:
                    e.Handled = true;
                    ((TextBox)sender).SelectionStart = cur;
                    break;
            }
        }

        public void textkeydown(object sender, KeyEventArgs e)
        {
            len = ((TextBox)sender).TextLength;
            cur = ((TextBox)sender).SelectionStart;
        }

        public void makeinput()
        {
            input = new Label();
            input.Parent = gbox;
            input.Text = "입    력";
            input.Font = new Font(Statics.font, 16, FontStyle.Bold);
            input.TextAlign = ContentAlignment.MiddleCenter;
            input.Image = Image.FromFile(Statics.dirpath + "\\button\\normal.png");
            input.Location = new Point(50, 360);
            input.Size = new Size(140, 45);
            input.MouseHover += new EventHandler(Globals.house.savedelhov);
            input.MouseLeave += new EventHandler(Globals.house.savedellve);
            input.Click += new EventHandler(inputclk);

            mend = new Label();
            mend.Parent = gbox;
            mend.Text = "수    정";
            mend.Font = new Font(Statics.font, 16, FontStyle.Bold);
            mend.TextAlign = ContentAlignment.MiddleCenter;
            mend.Image = Image.FromFile(Statics.dirpath + "\\button\\normal.png");
            mend.Location = new Point(200, 360);
            mend.Size = new Size(140, 45);
            mend.MouseHover += new EventHandler(Globals.house.savedelhov);
            mend.MouseLeave += new EventHandler(Globals.house.savedellve);
            mend.Click += new EventHandler(mendclk);
        }

        public void inputclk(object sender, EventArgs e)
        {
            input.Image = Image.FromFile(Statics.dirpath + "\\button\\click.png");
            Globals.house.auto.buttonclick();
            Globals.house.changemonth(dtp.Value.ToString("yyyy MM"));
            Globals.house.blist.Add(new Data(Statics.numlist, dtp.Value.ToString("yyyy-MM-dd"), combo[2].Text, combo[3].Text, combo[4].Text,
                combo[0].Text, combo[1].Text, commenttb.Text, memotb.Text, Globals.house.strchn.strtodub(moneytb.Text)));
            Statics.curnum = Statics.numlist++;
            findinsitem();
            Globals.house.sortdgv();
            Globals.house.searchcell();
            Globals.house.calcu.showsum();
            groupclear();
        }

        public void mendclk(object sender, EventArgs e)
        {
            mend.Image = Image.FromFile(Statics.dirpath + "\\button\\click.png");
            Globals.house.auto.buttonclick();
            Statics.curnum = (int)Globals.house.mdgv[0, Statics.currentrow].Value;
            List<Data> temp = new List<Data>();
            temp.Add(new Data(Statics.curnum, dtp.Value.ToString("yyyy-MM-dd"), combo[2].Text, combo[3].Text, combo[4].Text,
                combo[0].Text, combo[1].Text, commenttb.Text, memotb.Text, Globals.house.strchn.strtodub(moneytb.Text)));
            finddelitem();
            Globals.house.mdgv.Rows.Remove(Globals.house.mdgv.Rows[Statics.currentrow]);
            if (Globals.house.changemonth(dtp.Value.ToString("yyyy MM")) == 1) temp[0].num = Statics.curnum = Statics.numlist++;
            findinsitem();
            Globals.house.blist.Add(temp[0]);
            Globals.house.sortdgv();
            Globals.house.searchcell();
            Globals.house.calcu.showsum();
            groupclear();
        }

        public void finddelitem()
        {
            int[] num = new int[] { 0, 0 };
            int sel = 0;
            for (; sel < Globals.house.titemlist.Count; sel++) if (Globals.house.mdgv[2, Statics.currentrow].Value.ToString() == Globals.house.titemlist[sel]) break;
            if (sel > 1 && sel < 7)
            {
                for (; num[0] < Globals.house.banklist.Count; num[0]++) if (Globals.house.banklist[num[0]].name == Globals.house.mdgv[3, Statics.currentrow].Value.ToString()) break;
            }
            else if (sel > 6 && sel < 11)
            {
                for (; num[0] < Globals.house.cardlist.Count; num[0]++) if (Globals.house.cardlist[num[0]].name == Globals.house.mdgv[3, Statics.currentrow].Value.ToString()) break;
            }
            if (sel == 4)
                for (; num[1] < Globals.house.banklist.Count; num[1]++) if (Globals.house.banklist[num[1]].name == Globals.house.mdgv[4, Statics.currentrow].Value.ToString()) break;
            if (sel == 9)
                for (; num[1] < Globals.house.banklist.Count; num[1]++) if (Globals.house.banklist[num[1]].name == Globals.house.cardlist[num[0]].payment) break;
            calculator(sel, num, Globals.house.strchn.strtodub(Globals.house.mdgv[8, Statics.currentrow].Value.ToString()) * -1);
        }

        public void findinsitem()
        {
            int[] num = new int[] { 0, 0 };
            int sel = 0;
            for (; sel < Globals.house.titemlist.Count; sel++) if (combo[2].Text == Globals.house.titemlist[sel]) break;
            if (sel > 1 && sel < 7)
            {
                for (; num[0] < Globals.house.banklist.Count; num[0]++) if (Globals.house.banklist[num[0]].name == combo[3].Text) break;
            }
            else if (sel > 6 && sel < 11)
            {
                for (; num[0] < Globals.house.cardlist.Count; num[0]++) if (Globals.house.cardlist[num[0]].name == combo[3].Text) break;
            }
            if (sel == 4)
                for (; num[1] < Globals.house.banklist.Count; num[1]++) if (Globals.house.banklist[num[1]].name == combo[4].Text) break;
            if (sel == 9)
                for (; num[1] < Globals.house.banklist.Count; num[1]++) if (Globals.house.banklist[num[1]].name == Globals.house.cardlist[num[0]].payment) break;
            calculator(sel, num, Globals.house.strchn.strtodub(moneytb.Text));
        }

        public void calculator(int sel, int[] num, double money)
        {
            switch (sel)
            {
                case 0:
                    Statics.cash += money;
                    Statics.income += money;
                    break;
                case 1:
                    Statics.cash -= money;
                    Statics.expend += money;
                    break;
                case 2:
                    Globals.house.banklist[num[0]].money += money;
                    Statics.income += money;
                    break;
                case 3:
                    Globals.house.banklist[num[0]].money -= money;
                    Statics.expend += money;
                    break;
                case 4:
                    Globals.house.banklist[num[0]].money -= money;
                    Globals.house.banklist[num[1]].money += money;
                    break;
                case 5:
                    Globals.house.banklist[num[0]].money += money;
                    Statics.cash -= money;
                    break;
                case 6:
                    Globals.house.banklist[num[0]].money -= money;
                    Statics.cash += money;
                    break;
                case 7:
                    Globals.house.cardlist[num[0]].money += money;
                    Statics.income += money;
                    break;
                case 8:
                    Globals.house.cardlist[num[0]].money -= money;
                    Statics.expend += money;
                    break;
                case 9:
                    Globals.house.cardlist[num[0]].money += money;
                    Globals.house.banklist[num[1]].money -= money;
                    break;
                case 10:
                    Globals.house.cardlist[num[0]].money -= money;
                    Statics.cash += money;
                    break;
                default:
                    break;
            }
        }

        public void groupclear()
        {
            combo[4].Items.Clear();
            moneytb.Text = "";
            commenttb.Text = "";
            memotb.Text = "";
        }

        public void makecombo()
        {
            int[,] cpmbolocation = new int[,] { { 10, 86 }, { 190, 86 }, { 10, 162 }, { 10, 238 }, { 190, 238 } };
            combo = new ComboBox[5];

            for (int i = 0; i < 5; i++)
            {
                combo[i] = new ComboBox();
                combo[i].Parent = gbox;
                
                combo[i].Font = new Font(Statics.font, 14, FontStyle.Bold);
                combo[i].BackColor = Color.FromArgb(209, 220, 245);
                combo[i].Location = new Point(cpmbolocation[i, 0], cpmbolocation[i, 1]);
                combo[i].Size = new Size(170, 30);
                combo[i].DrawMode = DrawMode.OwnerDrawFixed;
                combo[i].DropDownStyle = ComboBoxStyle.DropDownList;
                combo[i].DrawItem += new DrawItemEventHandler(drawitem);
            }
            combo[0].Items.AddRange(Globals.house.mnglist.pickuparray(Globals.house.oitemlist));
            combo[0].SelectedIndex = 0;
            combo[1].Items.AddRange(findsitem(combo[0]));
            if (combo[1].Items.Count > 0) combo[1].SelectedIndex = 0; 
            combo[2].Items.AddRange(Globals.house.titemlist.ToArray());
            combo[2].SelectedIndex = 0;
            combo[0].SelectedIndexChanged += new System.EventHandler(showcombo);
            combo[1].SelectedIndexChanged += new System.EventHandler(showcombo1);
            combo[2].SelectedIndexChanged += new System.EventHandler(showcombo2);
        }

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

        public string[] findsitem(ComboBox combobox)
        {
            int i, j, start = 0, index;
            string[] rtn;

            index = Globals.house.oitemlist.FindIndex(x => x.name == combobox.SelectedItem.ToString());
            for (i = 0; i < index; i++)
            {
                if(Globals.house.intervallist[i] < 10000) start += Globals.house.intervallist[i];
                else start += (Globals.house.intervallist[i] - 10000);
            }
            rtn = new string[Globals.house.intervallist[index]];
            for (i = 0, j = 0; i < Globals.house.intervallist[index]; i++)
            {
                if(Globals.house.sitemlist[start + i].num < 10000) rtn[j++] = Globals.house.sitemlist[start + i].name;
            }
            Array.Resize(ref rtn, j);
            return rtn;
        }

        public void showcombo(object sender, EventArgs e)
        {
            combo[1].Items.Clear();
            combo[1].Items.AddRange(findsitem((ComboBox)sender));
            combo[1].DroppedDown = true;
        }

        public void showcombo1(object sender, EventArgs e)
        {
            combo[2].Items.Clear();
            combo[2].Items.AddRange(Globals.house.titemlist.ToArray());
            combo[2].DroppedDown = true;
        }
        public void showcombo2(object sender, EventArgs e)
        {
            combo[3].Enabled = true;
            combo[3].Items.Clear();
            combo[4].Items.Clear();
            combo[4].Enabled = false;
            label[5].Text = "계정이름";
            if(combo[2].SelectedIndex < 2)
            {
                combo[3].Enabled = false;
            }
            else if (combo[2].SelectedIndex > 1 && combo[2].SelectedIndex < 7)
            {
                if (Globals.house.banklist.Count < 1)
                {
                    MessageBox.Show("등록된 통장이 없습니다. 통장을 먼저 등록하십시요!!");
                    combo[2].SelectedIndex = 0;
                    return;
                }
                for (int i =0; i< Globals.house.banklist.Count; i++) combo[3].Items.Add(Globals.house.banklist[i].name);
                combo[3].SelectedIndex = 0;
                if (combo[2].SelectedIndex == 4)
                {
                    combo[4].Enabled = true;
                    label[5].Text = "송금통장";
                    for (int i = 0; i < Globals.house.banklist.Count; i++) combo[4].Items.Add(Globals.house.banklist[i].name);
                    combo[4].SelectedIndex = 0;
                }
            }
            else
            {
                if (Globals.house.cardlist.Count < 1)
                {
                    MessageBox.Show("등록된 카드가 없습니다. 카드를 먼저 등록하십시요!!");
                    combo[2].SelectedIndex = 0;
                    return;
                }
                else
                {
                    for (int i = 0; i < Globals.house.cardlist.Count; i++) combo[3].Items.Add(Globals.house.cardlist[i].name);
                    combo[3].SelectedIndex = 0;
                }
            }
            moneytb.Focus();
            moneytb.SelectionLength = 0;
            if (moneytb.Text != "") moneytb.Select(moneytb.TextLength - 1, 0);
            moneytb.BackColor = Color.Gold;
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
            this.borderColor = Color.Black;
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

    public class Printmanager
    {
        // Gridview 프린트 호출 함수
        public void Print_GridView()
        {
            PrintDocument printDoc = new PrintDocument();
            printDoc.PrintPage += new PrintPageEventHandler(printDoc_PrintPage);
            // 용지설정 ,true(가로) , false(세로)
            printDoc.DefaultPageSettings.Landscape = true;

            // 용지 여백 설정
            printDoc.DefaultPageSettings.Margins.Top = 10;
            printDoc.DefaultPageSettings.Margins.Left = 20;
            printDoc.DefaultPageSettings.Margins.Bottom = 10;
            printDoc.DefaultPageSettings.Margins.Right = 10;

            printDoc.Print();
        }

        // 프린트 이벤트 처리 함수
        private void printDoc_PrintPage(object sender, PrintPageEventArgs e)
        {
            PrintPage(Globals.house.mdgv, e);

            // Datagrid를 이미지로 비트맵으로 만들어서 출력하는 방식
            //using (Bitmap bmp = new Bitmap(data_ProjectList.Width, data_ProjectList.Height))
            //{
            //    data_ProjectList.DrawToBitmap(bmp, new Rectangle(0, 0, bmp.Width, bmp.Height));
            //    e.Graphics.DrawImage(bmp, 0, 0);
            //}

            //e.HasMorePages = false;
        }

        // DataGridView를 프린트 용지에 맞게 데이타 및 출력 크기를 설정해준다.
        private int mPrintRow;
        public void PrintPage(DataGridView dgv, PrintPageEventArgs e)
        {
            float ypos = e.MarginBounds.Top;
            ypos += printRow(iterHeader(dgv), ypos, true, dgv.ColumnHeadersDefaultCellStyle, e);
            for (; mPrintRow < dgv.Rows.Count; mPrintRow++)
            {
                var row = dgv.Rows[mPrintRow];
                if (ypos + row.Height > e.MarginBounds.Bottom)
                {
                    e.HasMorePages = true;
                    return;
                }
                ypos += printRow(iterRow(row), ypos, false, dgv.DefaultCellStyle, e);
            }
            mPrintRow = 0;
        }
        private IEnumerable<DataGridViewCell> iterRow(DataGridViewRow row)
        {
            foreach (DataGridViewCell cell in row.Cells) yield return cell;
        }
        private IEnumerable<DataGridViewCell> iterHeader(DataGridView dgv)
        {
            foreach (DataGridViewColumn column in dgv.Columns) yield return column.HeaderCell;
        }
        private float printRow(IEnumerable<DataGridViewCell> cells, float ypos, bool header, DataGridViewCellStyle style, PrintPageEventArgs e)
        {
            // 출력 폰트 지정
            Font font = new Font(Statics.font, 10);

            StringFormat fmt = new StringFormat(StringFormatFlags.LineLimit);
            fmt.Trimming = StringTrimming.EllipsisCharacter;
            fmt.LineAlignment = StringAlignment.Center;
            fmt.Alignment = StringAlignment.Center;
            float xpos = e.MarginBounds.Left;
            float height = 0;
            foreach (DataGridViewCell cell in cells)
            {
                //RectangleF rc = new RectangleF(xpos, ypos, cell.Size.Width, cell.Size.Height);
                RectangleF rc = new RectangleF(xpos, ypos, cell.Size.Width / 4 * 3, cell.Size.Height);  // 가로 사이즈가 용지를 넘치기 때문에 조정
                if (header) e.Graphics.FillRectangle(Brushes.LightGray, rc.Left, rc.Top, rc.Width, rc.Height);
                e.Graphics.DrawRectangle(Pens.Black, rc.Left, rc.Top, rc.Width, rc.Height);
                //e.Graphics.DrawString(cell.FormattedValue.ToString(), style.Font, Brushes.Black, rc, fmt);
                e.Graphics.DrawString(cell.FormattedValue.ToString(), font, Brushes.Black, rc, fmt); // 폰트 및 글자크기를 직접 지정함    
                height = Math.Max(height, rc.Height);
                xpos += rc.Width;
            }
            return height;
        }
    }

    public class Filemanager
    {
        FileInfo fn = new FileInfo(Statics.dirpath + "\\Data\\basic.ini");
        // 기본 데이터 파일 없으면 만들고 있으면 읽기
        public void basicfilemain()
        {
            if (!Directory.Exists(Statics.dirpath + "\\Data"))
            {
                Directory.CreateDirectory(Statics.dirpath + "\\Data");
            }
            if (!fn.Exists)
            {
                new Items();
                writefile();
            }
            readfile();
        }

        // 기본 데이터 파일 쓰기
        public void writefile()
        {
            int i;

            StreamWriter wr = new StreamWriter(Statics.dirpath + "\\Data\\basic.ini");
            for (i = 0; i < Globals.house.intervallist.Count - 1; i++)        
            {
                wr.Write(Globals.house.intervallist[i].ToString());
                wr.Write("|");
            }
            wr.Write(Globals.house.intervallist[i].ToString());
            wr.Write("\r\n");

            for (i = 0; i < Globals.house.oitemlist.Count - 1; i++)
            {
                wr.Write(Globals.house.oitemlist[i].num);
                wr.Write("|");
                wr.Write(Globals.house.oitemlist[i].name);
                wr.Write("|");
            }
            wr.Write(Globals.house.oitemlist[i].num);
            wr.Write("|");
            wr.Write(Globals.house.oitemlist[i].name);
            wr.Write("\r\n");

            for (i = 0; i < Globals.house.sitemlist.Count - 1; i++)
            {
                wr.Write(Globals.house.sitemlist[i].num);
                wr.Write("|");
                wr.Write(Globals.house.sitemlist[i].name);
                wr.Write("|");
            }
            wr.Write(Globals.house.sitemlist[i].num);
            wr.Write("|");
            wr.Write(Globals.house.sitemlist[i].name);
            wr.Write("\r\n");

            for (i = 0; i < Globals.house.titemlist.Count - 1; i++)
            {
                wr.Write(Globals.house.titemlist[i]);
                wr.Write("|");
            }
            wr.Write(Globals.house.titemlist[i]);
            wr.Write("\r\n");

            wr.Write(Statics.cash);
            wr.Write("\r\n");

            if (Globals.house.banklist.Count > 0)
            {
                for (i = 0; i < Globals.house.banklist.Count - 1; i++)
                {
                    wr.Write(Globals.house.banklist[i].name);
                    wr.Write("|");
                    wr.Write(Globals.house.banklist[i].money);
                    wr.Write("|");
                }
                wr.Write(Globals.house.banklist[i].name);
                wr.Write("|");
                wr.Write(Globals.house.banklist[i].money);
            }
            wr.Write("\r\n");

            if (Globals.house.cardlist.Count > 0)
            {
                for (i = 0; i < Globals.house.cardlist.Count - 1; i++)
                {
                    wr.Write(Globals.house.cardlist[i].name);
                    wr.Write("|");
                    wr.Write(Globals.house.cardlist[i].payment);
                    wr.Write("|");
                    wr.Write(Globals.house.cardlist[i].money);
                    wr.Write("|");
                }
                wr.Write(Globals.house.cardlist[i].name);
                wr.Write("|");
                wr.Write(Globals.house.cardlist[i].payment);
                wr.Write("|");
                wr.Write(Globals.house.cardlist[i].money);
            }
            wr.Close();
        }

        public Boolean readfile()
        {
            int i;
            string line;
            string[] tmp;

            Globals.house.intervallist.Clear();
            Globals.house.oitemlist.Clear();
            Globals.house.sitemlist.Clear();
            Globals.house.titemlist.Clear();
            Globals.house.banklist.Clear();
            Globals.house.cardlist.Clear();

            StreamReader sr = new StreamReader(Statics.dirpath + "\\Data\\basic.ini");

            line = sr.ReadLine();
            tmp = line.Split('|');
            for (i = 0; i < tmp.Length; i++) Globals.house.intervallist.Add(int.Parse(tmp[i]));

            line = sr.ReadLine();
            tmp = line.Split('|');
            for (i = 0; i < tmp.Length; i += 2) Globals.house.oitemlist.Add(new Listdata(Int32.Parse(tmp[i]), tmp[i + 1]));

            line = sr.ReadLine();
            tmp = line.Split('|');
            for (i = 0; i < tmp.Length; i += 2) Globals.house.sitemlist.Add(new Listdata(Int32.Parse(tmp[i]), tmp[i + 1]));

            line = sr.ReadLine();
            tmp = line.Split('|');
            for (i = 0; i < tmp.Length; i++) Globals.house.titemlist.Add(tmp[i]);

            line = sr.ReadLine();
            Statics.cash =double.Parse(line);

            line = sr.ReadLine();
            if (!String.IsNullOrEmpty(line))
            {
                tmp = line.Split('|');
                for (i = 0; i < tmp.Length; i+=2) Globals.house.banklist.Add(new Bank(tmp[i], double.Parse(tmp[i + 1])));
            }
            else
            {
                sr.Close();
                return false;
            }

            line = sr.ReadLine();
            if (!String.IsNullOrEmpty(line))
            {
                tmp = line.Split('|');
                for (i = 0; i < tmp.Length; i+=3) Globals.house.cardlist.Add(new Card(tmp[i], tmp[i + 1], double.Parse(tmp[i + 2])));
            }
            sr.Close();
            return true;
        }
    }

    public class Datafilemanager
    {
        // 데이터 파일 없으면 만들고 있으면 읽기
        public void datafilemain()
        {
            FileInfo fn = new FileInfo(Statics.dirpath + "\\Data\\" + Statics.filename + ".dat");
            if (!fn.Exists)
            {
                writefile();
            }
            readfile();
        }

        // 데이터 파일 쓰기
        public void writefile()
        {
            int num;
            Listdata listdata = new Listdata();
            StreamWriter sw = new StreamWriter(Statics.dirpath + "\\Data\\" + Statics.filename + ".dat");
            for (int i = 0; i < Globals.house.list.Count; i++)
            {
                sw.Write(Globals.house.list[i].date);
                sw.Write('|');
                sw.Write(Globals.house.list[i].type);
                sw.Write('|');
                sw.Write(Globals.house.list[i].cntname);
                sw.Write('|');
                sw.Write(Globals.house.list[i].deposit);
                sw.Write('|');
                listdata = Globals.house.oitemlist.Find(x => x.name == Globals.house.list[i].bigitem);
                num = (listdata.num < 10000) ? listdata.num : listdata.num - 10000;
                sw.Write(num);
                sw.Write('|');
                listdata = Globals.house.sitemlist.Find(x => x.name == Globals.house.list[i].smallitem);
                num = (listdata.num < 10000) ? listdata.num : listdata.num - 10000;
                sw.Write(num);
                sw.Write('|');
                sw.Write(Globals.house.list[i].detail);
                sw.Write('|');
                sw.Write(Globals.house.list[i].memo);
                sw.Write('|');
                sw.Write(Globals.house.list[i].money);
                sw.Write("\r\n");
            }
            sw.Close();
        }

        // 데이터 파일 읽기
        public void readfile()
        {
            string line = " ";
            string[] tmp = new string[9];
            
            Statics.numlist = 0;
            Globals.house.blist.Clear();
            StreamReader sr = new StreamReader(Statics.dirpath + "\\Data\\" + Statics.filename + ".dat");
            while ((line = sr.ReadLine()) != null)
            {
                tmp = line.Split('|');
                Globals.house.blist.Add(new Data(Statics.numlist++, tmp[0], tmp[1], tmp[2], tmp[3], Globals.house.mnglist.findname(Globals.house.oitemlist, Int32.Parse(tmp[4])),
                    Globals.house.mnglist.findname(Globals.house.sitemlist, Int32.Parse(tmp[5])), tmp[6], tmp[7], double.Parse(tmp[8])));
             //   MessageBox.Show(tmp[4] + " " + tmp[5]);
            }
            sr.Close();
            Globals.house.sortdgv();
            Globals.house.calcu.showsum();
        }
    }

    // 숫자 string 을 통화, 더블값 등으로 변경하여 반환 
    public class Stringchange
    {
        // 문자열을 숫자와 - 부호를 제외한 모든 값을 제거하고 더블값으로 변환하여 반환
        public double strtodub(string str)
        {
            string rtn = strtostr(str);
            if (string.IsNullOrEmpty(rtn)) return 0;
            else return Convert.ToDouble(rtn);
        }

        // 문자열을 숫자와 - 부호를 제외한 모든 값을 제거하고 문자열로 반환
        public string strtostr(string str)
        {
            string temp = str.Trim();
            int len = temp.Length;
            if (string.IsNullOrEmpty(temp)) return null;
            char[] value = new char[len];
            char[] tmp = new char[len];

            value = temp.ToCharArray();
            int i = 0, j = 0;
            if (value[0] == '-')
            {
                tmp[j++] = value[i++];
                if (len < 2) return new String(tmp);
            }
            while (value[i] < 49 || value[i] >57)
            {
                i++;
                if (i >= temp.Length) break;
            }
            for (; i < temp.Length; i++) if(char.IsDigit(value[i])) tmp[j++] = value[i];
            Array.Resize(ref tmp, j);
            if (string.IsNullOrEmpty(new String(tmp))) return null;
            return new String(tmp);
        }

        // 금액은 천단위 마다 컴마를 찍고 맨끝에 "원"자를 붙이는 함수
        public string makecomma(string str)
        {
            string sval = strtostr(str);
            if (string.IsNullOrEmpty(sval)) return "0원";
            char[] value = new char[sval.Length];
            char[] temp = new char[sval.Length + sval.Length/3 + 2];

            value = sval.ToCharArray();
            int k = 0;
            for (int i = sval.Length - 1, j = 0; i > -1 ; i--, j++, k++)
            {
                if (j == 3 && value[i] != '-')
                {
                    temp[k++] = ',';
                    j = 0;
                }
                temp[k] = value[i];
            }
            char swap;
            for(int i = 0, j = k - 1; i < k/2; i++, j--)
            {
                swap = temp[i];
                temp[i] = temp[j];
                temp[j] = swap;
            }
            temp[k] = '원';
            return new String(temp);
        }
    }

    public class ManageList
    {
        // 모든 값 리턴
        public string[] listtoarray(List<Listdata> list)
        {
            string[] rtn = new string[list.Count];
            for (int i = 0; i < list.Count; i++) rtn[i] = list[i].name;
            return rtn;
        }  
        // num 이 10,000 이하인 값만 리턴
        public string[] pickuparray(List<Listdata> list)
        {
            string[] rtn = new string[list.Count];
            int j = 0;
            for (int i = 0; i < list.Count; i++) if (list[i].num < 10000) rtn[j++] = list[i].name;
            Array.Resize(ref rtn, j);
            return rtn;
        }

        public string findname(List<Listdata> list, int num)
        {
            int i = 0, temp;
            for(; i < list.Count; i++)
            {
                temp = (list[i].num < 10000) ? list[i].num : list[i].num - 10000;
                if (temp == num) break;
            }
            return list[i].name;
        }
    }
    // 자동완성 클래스
    public class Autocomp
    {
        public TextBox txtbox = new TextBox();
        public ListBox listbox = new ListBox();
        public ProcessDB pdb;
        public DataTable dts;
        public List<Autodata> rtn = new List<Autodata>();

        public Autocomp()
        {
            maketextbox();
            makelistbox();
            pdb = new ProcessDB();
            pdb.accessdb();
            Globals.house.inputgroup.commenttb.KeyUp += processkeyup;
            Globals.house.inputgroup.memotb.KeyUp += processkeyup;
            Globals.house.inputgroup.commenttb.MouseClick += selectbox;
            Globals.house.inputgroup.memotb.MouseClick += selectbox;
        }

        public void maketextbox()
        {
            txtbox.Parent = Globals.house.inputgroup.gbox;
            txtbox.Size = new Size(0, 0);
            txtbox.ImeMode = ImeMode.Hangul;
        }

        public void selectbox(object sender, EventArgs e)
        {
            if (sender == Globals.house.inputgroup.commenttb) txtbox = Globals.house.inputgroup.commenttb;
            else txtbox = Globals.house.inputgroup.memotb;
        }

        public void makelistbox()
        {
            listbox.Size = new Size(200, 260);
            listbox.Font = new Font(Statics.font, 14, FontStyle.Bold);
            listbox.Hide();
            Globals.house.Controls.Add(listbox);
            listbox.BringToFront();
            listbox.MouseHover += new EventHandler(list_MouseHover);
            listbox.MouseLeave += new EventHandler(list_MouseLeave);
            listbox.KeyUp += new KeyEventHandler(list_KeyUp);
            listbox.Click += new EventHandler(list_Click);
        }

        // 키입력을 받아서 처리
        public void processkeyup(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                listbox.Hide();
                return;
            }
            if (e.KeyCode == Keys.Down)
            {
                if (listbox.Items.Count > 0)
                {
                    listbox.SelectedIndex = 0;
                    txtbox.Text = listbox.Text;
                    listbox.Focus();
                }
            }
            else
            {
                string str = txtbox.Text.Trim();
                if (str == "")
                {
                    listbox.DataSource = null;
                    listbox.Hide();
                    return;
                }
                else makelist(str);
            }
        }

        // 리스트박스에 마우스가 올라올 떄
        private void list_MouseHover(object sender, EventArgs e)
        {
            listbox.Show();
        }

        // 리스트박스에서 마우스가 떠날 때
        private void list_MouseLeave(object sender, EventArgs e)
        {
            listbox.Hide();
        }

        // 리스트의 항목을 마우스로 클릭하면 선택된 항목을 textbox에 출력
        private void list_Click(object sender, EventArgs e)
        {
            if (listbox.SelectedIndex >= 0)
            {
                txtbox.Text = listbox.Text;
                listbox.Hide();
            }
        }

        // 리스트의 항목을 탐색하거나 선택
        private void list_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Down || e.KeyCode == Keys.Up)
            {
                if (listbox.SelectedIndex >= 0)
                {
                    txtbox.Text = listbox.Text;
                }
            }
            else if (e.KeyCode == Keys.Enter) listbox.Hide();
        }

        // 리스트 만들기
        public void makelist(string str)
        {
            if (txtbox == Globals.house.inputgroup.commenttb) listbox.Location = new Point(110, 360);
            else listbox.Location = new Point(110, 400);

            listbox.Items.Clear();
            string val = pdb.ProcessHangul(str);
            comparelist(val);
            if (rtn.Count > 0)
            {
                if (rtn.Count < 2 && rtn[0].code.Trim() == val.Trim())
                {
                    listbox.Hide();
                }
                else
                {
                    sortlist();
                    for (int i = 0; i < rtn.Count; i++)
                    {
                        listbox.Items.Add(rtn[i].word);
                    }
                    listbox.SelectedIndex = -1;
                    listbox.Show();
                }
            }
            else
            {
                listbox.Items.Clear();
                listbox.Hide();
            }
        }

        public void sortlist()
        {
            rtn.Sort(delegate (Autodata x, Autodata y)
            {
                if (x.num < y.num) return 1;
                else if (x.num > y.num) return -1;
                return 0;
            });
        }

        // 입력, 수정 버튼을 눌렀을 때 자동완성 박스에 입력된 값 처리
        public void buttonclick()
        {
            txtbox = Globals.house.inputgroup.commenttb;
            processdata();
            txtbox = Globals.house.inputgroup.memotb;
            processdata();
            listbox.Hide();
            listbox.Items.Clear();
            txtbox.Focus();
            txtbox.Select(txtbox.Text.Length, 0);
        }

        // 데이터 20개 까지 listbox list에 저장
        public void comparelist(string val)
        {
            int k = 0;
            rtn.Clear();
            for (int i = 0; i < pdb.listdata.Count; i++)
            {
                if (pdb.listdata[i].code.Contains(val))
                {
                    rtn.Add(pdb.listdata[i]);
                    k++;
                }
                if (k > 20) break;
            }
        }

        // 입력한 데이터가 없으면 삽입하고 있으면 갯수에 1 더하기
        private void processdata()
        {
            int i = 0;
            if (txtbox.Text.Trim() == "") return;
            if (pdb.listdata.Count > 0)
            {
                for (i = 0; i < pdb.listdata.Count; i++) if (txtbox.Text.Trim() == pdb.listdata[i].word.Trim()) break;
                if (i < pdb.listdata.Count)
                {
                    pdb.updatedata(i);
                }
                else pdb.insertdata();
            }
            else pdb.insertdata();
        }
    }

    [Serializable]
    public class Autodata
    {
        public string word;
        public int num;
        public string code;

        public Autodata(string word, int num, string code)
        {
            this.word = word;
            this.num = num;
            this.code = code;
        }
    }

    public class ProcessDB
    {
        BinaryFormatter bf = new BinaryFormatter();
        Stream stream;
        public List<Autodata> listdata = new List<Autodata>();
        string filename = Statics.dirpath + "\\Data\\listdata.cmp";

        //  초성 자소
        int[] firstGroup
                = { 0x3131, 0x3132, 0x3134, 0x3137, 0x3138,
                    0x3139, 0x3141, 0x3142, 0x3143, 0x3145, 0x3146, 0x3147, 0x3148,
                    0x3149, 0x314a, 0x314b, 0x314c, 0x314d, 0x314e };

        // 중성 자소
        int[] middleGroup
            = { 0x314f, 0x3150, 0x3151, 0x3152, 0x3153,
                    0x3154, 0x3155, 0x3156, 0x3157, 0x3158, 0x3159, 0x315a, 0x315b,
                    0x315c, 0x315d, 0x315e, 0x315f, 0x3160, 0x3161, 0x3162, 0x3163 };

        // 종성 자소
        int[] lastGroup
            = { 0x0000, 0x3131, 0x3132, 0x3133, 0x3134,
                    0x3135, 0x3136, 0x3137, 0x3139, 0x313a, 0x313b, 0x313c, 0x313d,
                    0x313e, 0x313f, 0x3140, 0x3141, 0x3142, 0x3144, 0x3145, 0x3146,
                    0x3147, 0x3148, 0x314a, 0x314b, 0x314c, 0x314d, 0x314e };

        // DB 없으면 만들고 있으면 열고 datatable에 데이터 넘김
        public void accessdb()
        {
            try
            {
                FileInfo fi = new FileInfo(filename);
                if (!fi.Exists) writefile();
                readfile();
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show("에러 매세지 = " + ex.Message);
            }
        }

        public void writefile()
        {
            stream = File.Open(filename, FileMode.Create);

            bf.Serialize(stream, listdata);
            stream.Close();
        }

        public void readfile()
        {
            listdata.Clear();
            stream = File.Open(filename, FileMode.Open);

            listdata = (List<Autodata>)bf.Deserialize(stream);
            stream.Close();
        }

        // 데이터에 같은 단어가 없으면 데이터에 추가
        public void insertdata()
        {
            string code = ProcessHangul(Globals.house.auto.txtbox.Text.Trim());
            listdata.Add(new Autodata(Globals.house.auto.txtbox.Text.Trim(), 1, code));
            writefile();
            readfile();
        }

        // 데이터에 같은단어가 있으면 갯수에 1 더하기
        public void updatedata(int id)
        {
            listdata[id].num++;
            writefile();
            readfile();
        }

        // string을 int 값으로 반환
        public string ProcessHangul(string str)
        {
            int firstChar;
            int lastChar;
            int middleChar;
            string codes = "";
            for (int i = 0; i < str.Length; i++)
            {
                if (((str[i] >= 12593) && (str[i] <= 12686)) || ((str[i] >= 44032) && (str[i] <= 55203)))
                {
                    lastChar = str[i] - 0xAC00;
                    firstChar = lastChar / (21 * 28);
                    lastChar = lastChar % (21 * 28);
                    middleChar = lastChar / 28;
                    lastChar = lastChar % 28;
                    if (firstChar >= 0)
                    {
                        codes += firstGroup[firstChar] + " ";
                    }
                    else
                    {
                        // 초성이 없는경우
                        codes += System.Convert.ToInt32(str[i]) + " ";
                    }
                    if (middleChar >= 0)
                    {
                        codes += middleGroup[middleChar] + " ";
                    }
                    if (lastChar != 0x0000 && lastChar >= 0)
                    {
                        codes += lastGroup[lastChar] + " ";
                    }
                }
                else if (str[i] != ' ')
                {
                    codes += System.Convert.ToInt32(str[i]).ToString() + " ";
                }
            }
            return codes;
        }
    }
}