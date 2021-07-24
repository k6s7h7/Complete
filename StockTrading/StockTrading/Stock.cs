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
using System.Threading;
using System.Timers;
using System.Reflection;
using Microsoft.Win32;
using XA_SESSIONLib;
using XA_DATASETLib;

namespace StockTrading
{
    public partial class Stock : Form
    {
        public Size sizescr = new Size();
        public Label[] btn = new Label[4];
        public Label[] labels = new Label[6];
        public Label message = new Label();
        public ComboBox combo = new ComboBox();
        public TextBox textBox = new TextBox();
        public Table table = new Table();
        public Infobox infobox = new Infobox();
        public Audit audit = new Audit();

        public Change_string chstr = new Change_string();
        public Loginout logio = new Loginout();
        public Timemanager tm = new Timemanager();
        public CSPAQ12300 cspaq12300 = new CSPAQ12300();
        public T8430 t8430 = new T8430();
        public T1102 t1102 = new T1102();
        public T1442 t1442 = new T1442();
        public CSPAT00600 cspat00600 = new CSPAT00600();
        public CSPAT00700 cspat00700 = new CSPAT00700();
        public CSPAT00800 cspat00800 = new CSPAT00800();
        public Realdata realdata = new Realdata();
        public int len, cur;

        public Stock()
        {
            InitializeComponent();
        }

        private void Stock_Load(object sender, EventArgs e)
        {
            Main.stock = this;
            autostart();

            housescreen();
            makelabels();
            makedgv();
            makebutton();
            makecombo();
            maketextbox();
            logio.makeimage();
            makemsg();

            infobox.filemain();
            table.settingdgv();
            startstock();
        }

        private void Stock_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("정말로 종료하시겠습니까?", "프로그램 종료", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                realdata.realend();
                Statics.XingSession.Logout();
                Statics.XingSession.DisconnectServer();
                if(infobox != null) infobox.Dispose();
                System.Diagnostics.Process.GetCurrentProcess().Kill();
            }
            else e.Cancel = true;
        }

        public void autostart()
        {
            if (Directory.GetCurrentDirectory() == "C:\\Windows\\system32")
            {
                RegistryKey rkey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run");   // Registry 에서 Sub Key를 가져온다
                Statics.dirpath = rkey.GetValue("주식자동매매").ToString();
                Statics.dirpath = Statics.dirpath.Replace("\"", "");
                Statics.dirpath = Statics.dirpath.Substring(0, Statics.dirpath.LastIndexOf("신고가"));
                Directory.SetCurrentDirectory(Statics.dirpath);
            }
        }

        public void housescreen()
        {
            sizescr = Screen.PrimaryScreen.WorkingArea.Size;
            this.WindowState = FormWindowState.Normal;
            this.Location = new Point(0, 100);
            ClientSize = new Size(sizescr.Width, sizescr.Height / 2 + 120);
            this.BackColor = Color.FromArgb(218, 223, 232);
        }

        public void makedgv()
        {
            table.dgv.Location = new Point(10, 150);
            table.dgv.Size = new Size(sizescr.Width - 30, sizescr.Height / 2 - 50);
            table.dgv.RowTemplate.Height = 25;
            table.dgv.ColumnHeadersHeight = 35;
            this.Controls.Add(table.dgv);
            ExtensionMethods.DoubleBuffered(table.dgv, true);
        }

        public void makelabels()
        {
            string[] name = new string[] { "서버 상태", "로그인상태", "계좌번호", "주문가능 금액", "사용할 금액" };

            for (int i = 0; i < name.Length + 1; i++)
            {
                labels[i] = new Label();
                labels[i].Parent = this;

                if (i == 5)
                    labels[5].Bounds = new Rectangle(1410, 34, 200, 20);
                else
                {
                    labels[i].Text = name[i];
                    labels[i].Bounds = new Rectangle(600 + i * 260, 10, 240, 20);
                }
                labels[i].TextAlign = ContentAlignment.MiddleCenter;
                labels[i].BackColor = Color.FromArgb(218, 223, 232);
                labels[i].Font = new Font(Statics.글꼴, 12, FontStyle.Bold);
            }
        }

        public void makecombo()
        {
            combo.Parent = this;
            combo.Bounds = new Rectangle(1160, 34, 180, 32);
            combo.Font = new Font(Statics.글꼴, 12, FontStyle.Bold);
            combo.BackColor = Color.FromArgb(240, 248, 255);
            combo.DrawMode = DrawMode.OwnerDrawFixed;
            combo.DropDownStyle = ComboBoxStyle.DropDownList;
            combo.DrawItem += new DrawItemEventHandler(drawitem);
            combo.SelectedIndexChanged += showcombo;
        }

        private void showcombo(object sender, EventArgs e)
        {
            Statics.accountnum = combo.SelectedIndex;
            Statics.계좌번호 = combo.SelectedItem.ToString();
            cspaq12300.cspaq12300in();
            tm.Delay(1000);
        }

        public void drawitem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;

            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
                e.Graphics.FillRectangle(new SolidBrush(Color.Gold), e.Bounds);
            else
                e.Graphics.FillRectangle(new SolidBrush(((ComboBox)sender).BackColor), e.Bounds);
            e.Graphics.DrawString(((ComboBox)sender).Items[e.Index].ToString(), e.Font, Brushes.Black, e.Bounds, StringFormat.GenericDefault);
        }

        public void maketextbox()
        {
            textBox.Parent = this;
            textBox.Bounds = new Rectangle(1660, 34, 220, 32);
            textBox.Font = new Font(Statics.글꼴, 12, FontStyle.Bold);
            textBox.BackColor = Color.FromArgb(240, 248, 255);
            textBox.TextAlign = HorizontalAlignment.Right;

            textBox.Click += new EventHandler(moneyclick);
            textBox.Leave += new EventHandler(moneyleave);
            textBox.KeyUp += new KeyEventHandler(textkeyup);
            textBox.KeyDown += new KeyEventHandler(textkeydown);
            textBox.Enter += new EventHandler(moneyfocus);
        }

        public void moneyclick(object sender, EventArgs e)
        {
            ((TextBox)sender).BackColor = Color.Gold;
        }

        public void moneyleave(object sender, EventArgs e)
        {
            ((TextBox)sender).BackColor = Color.FromArgb(240, 248, 255);
            Statics.money = chstr.strtodouble(textBox.Text);
        }

        public void moneyfocus(object sender, EventArgs e)
        {
            ((TextBox)sender).BackColor = Color.Gold;
        }

        // 숫자를 입력 받아 천 단위에 콤마찍고 커서는 제위치에 놓기
        public void textkeyup(object sender, KeyEventArgs e)
        {
            ((TextBox)sender).Text = chstr.makecomma(((TextBox)sender).Text);
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

        public void makebutton()
        {
            string[] name = new string[] { "로 그 인", "다시 시작", "설      정" };

            for (int i = 0; i < name.Length; i++)
            {
                btn[i] = new Label();
                btn[i].Parent = this;
                btn[i].Text = name[i];
                btn[i].Bounds = new Rectangle(50 + i * 170, 20, 120, 38);
                btn[i].Font = new Font(Statics.글꼴, 16, FontStyle.Bold);
                btn[i].TextAlign = ContentAlignment.MiddleCenter;
                btn[i].Image = Image.FromFile(Statics.dirpath + "\\button\\normal.png");
                btn[i].MouseHover += new EventHandler(starthov);
                btn[i].MouseLeave += new EventHandler(startlve);
                btn[i].Click += new EventHandler(startclk);
            }
        }

        public void starthov(object sender, EventArgs e)
        {
            ((Label)sender).Image = Image.FromFile(Statics.dirpath + "\\button\\hover.png");
        }

        public void startlve(object sender, EventArgs e)
        {
            ((Label)sender).Image = Image.FromFile(Statics.dirpath + "\\button\\normal.png");
        }

        public void startclk(object sender, EventArgs e)
        {
            ((Label)sender).Image = Image.FromFile(Statics.dirpath + "\\button\\click.png");
            if ((Label)sender == btn[0])
            {
                if (String.IsNullOrEmpty(Statics.secretdata.id))
                {
                    MessageBox.Show("ID, 비밀번호 등 기본자료가 없습니다. 설정을 구성하세요");
                    ((Label)sender).Image = Image.FromFile(Statics.dirpath + "\\button\\normal.png");
                    return;
                }
                logio.ConnectToServer();
                if (!Statics.loginchk) return;
                t8430.t8430in();
                tm.Delay(500);
            }
            else if ((Label)sender == btn[1])
            {
                restartapp();
            }
            else if ((Label)sender == btn[2])
                makeinfo();
        }

        public void restartapp()
        {
            realdata.realstart();
            for (int i = 0; i < Statics.sblist.Count; i++)
                realdata.send_price(Statics.sblist[i].종목코드, Statics.sblist[i].구분);
            if (Int32.Parse(DateTime.Now.ToString("HHmmss")) > 153000)
            {
                MessageBox.Show("주식매매를 할 시간이 끝났습니다. 15:30까지 입니다!!");
                Statics.END = true;
            }
            else 
                starttrading();
        }

        public void condition()
        {
            if (!Statics.loginchk)
            {
                MessageBox.Show("먼저 로그인이 되어 있어야 합니다!!");
                return;
            }
            if (Int32.Parse(DateTime.Now.ToString("HHmmss")) > Statics.secretdata.endtime || Int32.Parse(DateTime.Now.ToString("HHmmss")) > 153000)
            {
                if (Statics.secretdata.endtime >= 153000)
                    MessageBox.Show("주식매매를 할 시간이 끝났습니다. 15:30까지 입니다!!");
                else
                    MessageBox.Show("주식매매를 할 시간이 끝났습니다. " + Statics.secretdata.endtime + "까지 입니다!!");
            }
            else
            {
                Statics.money = chstr.strtodouble(textBox.Text);
                if (Statics.money < 1000000)
                    MessageBox.Show("사용할 금액이 너무 적습니다. 1,000,000원 이상을 입력하십시요!!");
                else if (Statics.money > chstr.strtodouble(Main.stock.labels[5].Text))
                    MessageBox.Show("사용할 금액이 계좌잔고 보다 많습니다. 다시 입력하십시요!!");
                else starttrading();
            }
        }
        // 자동 실행 메서드
        public void startstock()
        {
            if (String.IsNullOrEmpty(Statics.secretdata.id))
            {
                MessageBox.Show("ID, 비밀번호 등 기본자료가 없습니다. 설정을 구성하세요");
                return;
            }
            logio.ConnectToServer();
            if (!Statics.loginchk) return;
            tm.Delay(1000);
            t8430.t8430in();
            tm.Delay(500);
            realdata.realstart();
            Statics.money = 30000000;
            textBox.Text = "30,000,000";
            condition();
        }

        public void makemsg()
        {
            message.Parent = this;
            message.Bounds = new Rectangle(200, 100, 1000, 32);
            message.Font = new Font(Statics.글꼴, 12, FontStyle.Bold);
            message.BackColor = Color.FromArgb(240, 248, 255);
            message.TextAlign = ContentAlignment.MiddleCenter;
        }

        public void makeinfo()
        {
            infobox.StartPosition = FormStartPosition.Manual;
            infobox.FormBorderStyle = FormBorderStyle.None;
            infobox.Bounds = new Rectangle(200, 200, 540, 700);
            infobox.BackColor = Color.FromArgb(176, 196, 222);
            infobox.TopMost = true;
            infobox.Show();
        }

        public void makeaudit()
        {
            audit.StartPosition = FormStartPosition.Manual;
            audit.Bounds = new Rectangle(50, 50, 1820, 760);
            audit.BackColor = Color.FromArgb(176, 196, 222);
            audit.TopMost = true;
            audit.Show();
        }

        public void starttrading()
        {
            tm.starttime();
            while (!Statics.END)
            {
                tm.endtime();
                if (Statics.sblist.Count < Statics.secretdata.max)
                {
                    t1442.t1442in();
                    tm.Delay(1000);
                }
                order();
                checkserver();
                if (!Statics.loginchk) return;
                tm.Delay(1);
            }
            tm.Delay(1000);
            makeaudit();
        }

        private void checkserver()
        {
            if (!Statics.XingSession.IsConnected())                      // 서버 연결 체크
            {
                logio.labels[0].Image = Image.FromFile(Statics.dirpath + "\\button\\wifix.png");
                logio.labels[1].Image = Image.FromFile(Statics.dirpath + "\\button\\engry.png");
                message.Text = "서버와의 연결이 끊겼습니다!! 잠시 기다려 주십시요. 자동으로 다시연결합니다";
                Statics.loginchk = false;
                realdata.realend();
                logio.ConnectToServer();
                if (Statics.loginchk) Main.stock.restartapp();
                else
                    message.Text = "로그인 오류로 자동연결에 실패했습니다. 수동으로 연결하십시요";
            }
        }

        private void checkmend(int no)
        {
            if(Statics.sblist[no].매수미체결 > 0)
            {
                cspat00800.cspat00800in(no);
                if (Statics.sblist[no].매수수량 > 0)
                {
                    cspat00600.cspat00600in(no, "3");
                    tm.Delay(1000);
                }
            }
            else
            {
                if(Statics.sblist[no].체결여부 == "매도중")
                {
                    cspat00700.cspat00700in(no);
                    tm.Delay(1000);
                }
                else if(Statics.sblist[no].체결여부 == "매수완")
                {
                    cspat00600.cspat00600in(no, "3");
                    tm.Delay(1000);
                }
            }
        }

        public void order()
        {
            int end = 0;
            for (int i = 0; i < Statics.sblist.Count; i++)
            {
                switch (Statics.sblist[i].주문여부)
                {
                    case "":
                        cspat00600.cspat00600in(i, "2");
                        tm.Delay(1000);
                        break;
                    case "매수":
                        if (Statics.sblist[i].체결여부 == "매수완")
                        {
                            cspat00600.cspat00600in(i, "1");
                            tm.Delay(1000);
                        }
                        break;
                    case "매도":
                        if (Statics.sblist[i].체결여부 == "매도완") end++;
                        break;
                    case "정정":
                        if (Statics.sblist[i].체결여부 == "")
                        {
                            tm.Delay(1000);
                            if (Statics.sblist[i].체결여부 == "")
                            {
                                Statics.sblist[i].체결여부 = "매수없음";
                                end++;
                            }
                        }
                        else if (Statics.sblist[i].체결여부 == "매수없음" || Statics.sblist[i].체결여부 == "정정완")
                            end++;
                        else checkmend(i);
                        break;
                    default:
                        break;
                }
                table.dgv.Refresh();
            }
            if (end >= Statics.secretdata.max) Statics.END = true;
        }
    }

    public class Timemanager
    {
        public DateTime Delay(int MS)
        {
            DateTime ThisMoment = DateTime.Now;
            TimeSpan duration = new TimeSpan(0, 0, 0, 0, MS);
            DateTime AfterWards = ThisMoment.Add(duration);

            while (AfterWards >= ThisMoment)
            {
                System.Windows.Forms.Application.DoEvents();
                ThisMoment = DateTime.Now;
            }
            return DateTime.Now;
        }

        public void starttime()
        {
            while (Statics.secretdata.starttime > Int32.Parse(DateTime.Now.ToString("HHmmss")))
                Thread.Sleep(1000);
        }

        public void endtime()
        {
            if (Int32.Parse(DateTime.Now.ToString("HHmmss")) > Statics.secretdata.endtime)
            {
                Statics.secretdata.max = Statics.sblist.Count;
                for (int i = 0; i < Statics.sblist.Count; i++)
                    if (Statics.sblist[i].체결여부 != "매도완") Statics.sblist[i].주문여부 = "정정";
            }
        }
    }

    // 서버 연결과 로그인
    public class Loginout
    {
        int XingPort = 20001;
        public Label[] labels = new Label[2];

        public void makeimage()
        {
            for (int i = 0; i < 2; i++)
            {
                labels[i] = new Label();
                labels[i].Parent = Main.stock;
                labels[i].Bounds = new Rectangle(690 + i * 260, 30, 60, 60);
            }
        }

        public void ConnectToServer()
        {
            while (true)
            {
                bool connected = Statics.XingSession.ConnectServer(Statics.secretdata.url, XingPort);

                if (connected)
                {
                    labels[0].Image = Image.FromFile(Statics.dirpath + "\\button\\wifi.gif");
                    Main.stock.message.Text = "서버가 연결되었습니다!";
                    login();
                    break;
                }
                else
                {
                    labels[0].Image = Image.FromFile(Statics.dirpath + "\\button\\wifix.png");
                    labels[1].Image = Image.FromFile(Statics.dirpath + "\\button\\engry.png");
                    int ErrCode = Statics.XingSession.GetLastError();
                    Main.stock.message.Text = String.Format("{0}({1})", Statics.XingSession.GetErrorMessage(ErrCode), ErrCode);
                    Main.stock.tm.Delay(1000);
                }
            }
        }

        public void login()
        {
            ((XA_SESSIONLib._IXASessionEvents_Event)Statics.XingSession).Login += Main.stock.logio.XingSession_Login;
            bool valid = ((XA_SESSIONLib.IXASession)Statics.XingSession).Login(Statics.secretdata.id, Statics.secretdata.logpass, Statics.secretdata.activepass, 0, false);

            if (!valid)
            {
                labels[1].Image = Image.FromFile(Statics.dirpath + "\\button\\engry.png");
                int ErrCode = Statics.XingSession.GetLastError();
                MessageBox.Show(String.Format("{0}({1})", Statics.XingSession.GetErrorMessage(ErrCode), ErrCode));
                Statics.XingSession.DisconnectServer();
                labels[0].Image = Image.FromFile(Statics.dirpath + "\\button\\wifix.png");
                ((XA_SESSIONLib._IXASessionEvents_Event)Statics.XingSession).Login -= Main.stock.logio.XingSession_Login;
            }
            Main.stock.tm.Delay(1000);
        }

        public void XingSession_Login(string code, string msg)
        {
            ((XA_SESSIONLib._IXASessionEvents_Event)Statics.XingSession).Login -= Main.stock.logio.XingSession_Login;

            if (code == "0000")     // 로그인에 성공했으면
            {
                labels[1].Image = Image.FromFile(Statics.dirpath + "\\button\\smile.png");
                if (Statics.XingSession.GetAccountListCount() > 0)
                    for (int i = 0; i < Statics.XingSession.GetAccountListCount(); i++)
                        Main.stock.combo.Items.Add(Statics.XingSession.GetAccountList(i));
                Main.stock.message.Text = String.Format("{0}({1})", msg, code);
                Statics.loginchk = true;
                Main.stock.combo.SelectedIndex = 0;
            }
            else
            {
                labels[1].Image = Image.FromFile(Statics.dirpath + "\\button\\engry.png");
                Main.stock.message.Text = String.Format("{0}({1})", msg, code);
                Statics.loginchk = false;
                Statics.XingSession.DisconnectServer();
                labels[0].Image = Image.FromFile(Statics.dirpath + "\\button\\wifix.png");
            }
        }
    }

    public class CSPAQ12300
    {
        public void cspaq12300in()
        {
            Statics.event12300.ReceiveData += this.cspaq12300out;
            Statics.event12300.ResFileName = "/res/CSPAQ12300.res";

            Statics.event12300.SetFieldData("CSPAQ12300InBlock1", "AcntNo", 0, Statics.계좌번호);
            Statics.event12300.SetFieldData("CSPAQ12300InBlock1", "Pwd", 0, Statics.secretdata.password);
            Statics.event12300.SetFieldData("CSPAQ12300InBlock1", "BalCreTp", 0, "1");
            Statics.event12300.SetFieldData("CSPAQ12300InBlock1", "CmsnAppTpCode", 0, "1");
            Statics.event12300.SetFieldData("CSPAQ12300InBlock1", "D2balBaseQryTp", 0, "0");

            int result = Statics.event12300.Request(false);
            if (result < 0)
            {
                Statics.event12300.ReceiveData -= this.cspaq12300out;
                MessageBox.Show(Statics.XingSession.GetErrorMessage(result));
            }
        }

        public void cspaq12300out(string trno)
        {
            Statics.event12300.ReceiveData -= this.cspaq12300out;

            Statics.event12300.GetBlockCount("CSPAQ12300OutBlock1");
            if (Statics.event12300.GetFieldData("CSPAQ12300OutBlock1", "AcntNo", 0) != Statics.계좌번호)
            {
                MessageBox.Show("틀린 계좌번호가 수신되었습니다. 프로그램을 다시 실행하십시요!!");
                return;
            }

            int chk = Statics.event12300.GetBlockCount("CSPAQ12300OutBlock2");
            if (chk > 0)
                Statics.balance = long.Parse(Statics.event12300.GetFieldData("CSPAQ12300OutBlock2", "MnyOrdAbleAmt", 0));
            else
                Statics.balance = 0;
            Main.stock.labels[5].Text = Statics.balance.ToString("#,##0원");
        }
    }

    public class T8430
    {
        public void t8430in()
        {
            Statics.event8430.ReceiveData += this.t8430out;
            Statics.event8430.ResFileName = "/res/t8430.res";

            Statics.event8430.SetFieldData("t8430inblock", "gubun", 0, "0");

            int result = Statics.event8430.Request(false);
            if (result < 0 && Statics.END == false)
            {
                Statics.event8430.ReceiveData -= this.t8430out;
                MessageBox.Show(Statics.XingSession.GetErrorMessage(result));
            }
        }

        public void t8430out(string trno)
        {
            Statics.event8430.ReceiveData -= this.t8430out;

            int count = Statics.event8430.GetBlockCount("t8430outblock");
            for (int i = 0; i < count; i++)
            {
                string 종목이름 = Statics.event8430.GetFieldData("t8430OutBlock", "hname", i).Trim();
                string 종목코드 = Statics.event8430.GetFieldData("t8430OutBlock", "shcode", i).Trim();
                string 긴종목코드 = Statics.event8430.GetFieldData("t8430OutBlock", "expcode", i).Trim();
                string 구분 = Statics.event8430.GetFieldData("t8430OutBlock", "gubun", i);

                Statics.basiclist.Add(new Basicdata(종목이름, 종목코드, 긴종목코드, 구분));
            }
        }
    }

    public class T1442
    {
        public T1302 t1302 = new T1302();
        public string[] exception = { "KODEX", "코스피", "ETN", "KRX", "TIGER", "중소형", "ARIRANG", "팩", "S&P", "KBSTAR", "KINDEX", "대형주", "중형주", "레버리지" };

        public void t1442in()
        {
            Statics.event1442.ReceiveData += this.t1442out;
            Statics.event1442.ResFileName = "/res/t1442.res";

            Statics.event1442.SetFieldData("t1442InBlock", "idx", 0, Statics.nextkey);
            Statics.event1442.SetFieldData("t1442InBlock", "gubun", 0, "0");
            Statics.event1442.SetFieldData("t1442InBlock", "type1", 0, "0");
            Statics.event1442.SetFieldData("t1442InBlock", "type2", 0, "6");
            Statics.event1442.SetFieldData("t1442InBlock", "type3", 0, "0");

            int result = Statics.event1442.Request(true);         // 연속조회이면 true, 단일조회이면 false
            if (result < 0)
            {
                Statics.event1442.ReceiveData -= this.t1442out;
                MessageBox.Show(Statics.XingSession.GetErrorMessage(result));
            }
        }

        public void t1442out(string trno)
        {
            Statics.event1442.ReceiveData -= this.t1442out;

            int chk = Statics.event1442.GetBlockCount("t1442OutBlock");
            if (chk > 0)
                Statics.nextkey = Statics.event1442.GetFieldData("t1442OutBlock", "idx", 0);
            int count = Statics.event1442.GetBlockCount("t1442OutBlock1");

            for (int idx = 0, i = 0; idx < count; idx++, i = 0)
            {
                string 종목이름 = Statics.event1442.GetFieldData("t1442OutBlock1", "hname", idx);
                for (; i < exception.Length; i++)
                    if (종목이름.Contains(exception[i])) break;
                if (i < exception.Length) continue;

                string 종목코드 = Statics.event1442.GetFieldData("t1442OutBlock1", "shcode", idx);
                for (i = 0; i < Statics.sblist.Count; i++)
                    if (Statics.sblist[i].종목코드 == 종목코드) break;
                if (i < Statics.sblist.Count) continue;

                string 표시 = Statics.event1442.GetFieldData("t1442OutBlock1", "sign", idx);
                float 등락율 = float.Parse(Statics.event1442.GetFieldData("t1442OutBlock1", "diff", idx));
                int 거래량 = Int32.Parse(Statics.event1442.GetFieldData("t1442OutBlock1", "volume", idx));
                if (표시 == "2" && 등락율 >= Statics.secretdata.sratio && 등락율 <= Statics.secretdata.eratio && 거래량 > Statics.secretdata.volume)
                {
                    int 전일대비 = Int32.Parse(Statics.event1442.GetFieldData("t1442OutBlock1", "change", idx));
                    int 현재가 = Int32.Parse(Statics.event1442.GetFieldData("t1442OutBlock1", "price", idx));
                    Main.stock.tm.Delay(1000);
                    t1302.t1302in(종목코드);
                    Main.stock.tm.Delay(1000);
                    if (Statics.event1302pass == true)
                    {
                        for (i = 0; i < Statics.basiclist.Count; i++)
                            if (Statics.basiclist[i].종목코드 == 종목코드) break;
                        Statics.sblist.Add(new Data(Statics.basiclist[i].구분, Statics.basiclist[i].긴종목코드, Statics.basiclist[i].종목코드, 종목이름,
                            현재가, 전일대비, 등락율, Math.Abs((int)(전일대비 / 등락율)), 0f, "", "", 0, 0, 0, 0, "", "", 0, 0, 0, 0, "", "", "", 0, 0, 0, 0, 0));
                        Main.stock.realdata.send_price(종목코드, Statics.basiclist[i].구분);
                        Main.stock.cspat00600.cspat00600in(Statics.sblist.Count - 1, "2");
                        Main.stock.tm.Delay(50);
                        if (Statics.sblist.Count >= Statics.secretdata.max) return;
                    }
                }
            }
        }

        public class T1302
        {
            public void t1302in(string 종목코드)
            {
                Statics.event1302.ReceiveData += this.t1302out;
                Statics.event1302.ResFileName = "/res/t1302.res";

                Statics.event1302.SetFieldData("t1302InBlock", "shcode", 0, 종목코드);
                Statics.event1302.SetFieldData("t1302InBlock", "gubun", 0, "0");
                Statics.event1302.SetFieldData("t1302InBlock", "time", 0, "");
                Statics.event1302.SetFieldData("t1302InBlock", "cnt", 0, "20");

                int result = Statics.event1302.Request(false);
                if (result < 0 && Statics.END == false)
                {
                    Statics.event1302.ReceiveData -= this.t1302out;
                    MessageBox.Show(Statics.XingSession.GetErrorMessage(result));
                }
            }

            public void t1302out(string trno)
            {
                Statics.event1302.ReceiveData -= this.t1302out;

                int count = Statics.event1302.GetBlockCount("t1302OutBlock1"), i;

                for (i = 0; i < count; i++)
                    if (float.Parse(Statics.event1302.GetFieldData("t1302OutBlock1", "diff", i)) > Statics.secretdata.limit) break;
                Statics.event1302pass = (i < count) ? false : true;
            }
        }
    }

    // 주식 현재가(시세) 조회
    public class T1102
    {
        int no = 0;
        public void t1102in(int no)
        {
            this.no = no;
            Statics.event1102.ReceiveData += t1102out;

            Statics.event1102.ResFileName = "/res/t1102.res";
            Statics.event1102.SetFieldData("t1102InBlock", "shcode", 0, Statics.sblist[no].종목코드);

            int result = Statics.event1102.Request(false);
            if (result < 0)
            {
                MessageBox.Show(Statics.XingSession.GetErrorMessage(result));
                Statics.event1102.ReceiveData -= t1102out;
            }
        }

        public void t1102out(string trno)
        {
            Statics.event1102.ReceiveData -= t1102out;

            Statics.event1102.GetBlockCount("t1102OutBlock");
            Statics.sblist[no].현재가 = Int32.Parse(Statics.event1102.GetFieldData("t1102OutBlock", "price", 0));
            Statics.sblist[no].전일대비 = Int32.Parse(Statics.event1102.GetFieldData("t1102OutBlock", "change", 0));
            Statics.sblist[no].등락율 = float.Parse(Statics.event1102.GetFieldData("t1102OutBlock", "diff", 0));
        }
    }

    public class CSPAT00600
    {
        int no;
        string gubun;
        public void cspat00600in(int no, string gubun)
        {
            if (Statics.check[0] == "2") return;
            Statics.check[0] = "2";
            this.no = no;
            this.gubun = gubun;

            Statics.event00600.ReceiveData += this.cspat00600out;
            Statics.event00600.ResFileName = "/res/CSPAT00600.res";

            Statics.event00600.SetFieldData("CSPAT00600InBlock1", "AcntNo", 0, Statics.계좌번호);
            Statics.event00600.SetFieldData("CSPAT00600InBlock1", "InptPwd", 0, Statics.secretdata.password);
            Statics.event00600.SetFieldData("CSPAT00600InBlock1", "IsuNo", 0, Statics.sblist[no].긴종목코드);

            int qunt = 0, price = 0;
            if (gubun == "2")
            {
                Main.stock.tm.Delay(50);
                Main.stock.t1102.t1102in(no);
                Main.stock.tm.Delay(200);
                qunt = Statics.sblist[no].매수미체결 = (int)(((Statics.money * 9) / (Statics.secretdata.max * 10)) / Statics.sblist[no].현재가);
                while(qunt > (int)((Statics.money / Statics.secretdata.max) / Statics.sblist[no].현재가))
                    qunt = Statics.sblist[no].매수미체결 = (int)(((Statics.money * 9) / (Statics.secretdata.max * 10)) / Statics.sblist[no].현재가);
                Statics.event00600.SetFieldData("CSPAT00600InBlock1", "OrdprcPtnCode", 0, "03");
            }
            else
            {
                qunt = Statics.sblist[no].매도미체결 = Statics.sblist[no].매수수량;
                if (gubun == "1")
                {
                    price = orderprice(no);
                    Statics.event00600.SetFieldData("CSPAT00600InBlock1", "OrdprcPtnCode", 0, "00");
                }
                else
                {
                    Statics.event00600.SetFieldData("CSPAT00600InBlock1", "OrdprcPtnCode", 0, "03");
                    gubun = "1";
                }
            }
            Statics.event00600.SetFieldData("CSPAT00600InBlock1", "OrdPrc", 0, price.ToString());
            Statics.event00600.SetFieldData("CSPAT00600InBlock1", "OrdQty", 0, qunt.ToString());
            Statics.event00600.SetFieldData("CSPAT00600InBlock1", "BnsTpCode", 0, gubun);
            Statics.event00600.SetFieldData("CSPAT00600InBlock1", "MgntrnCode", 0, "000");
            Statics.event00600.SetFieldData("CSPAT00600InBlock1", "LoanDt", 0, "");
            Statics.event00600.SetFieldData("CSPAT00600InBlock1", "OrdCndiTpCode", 0, "0");

            int result = Statics.event00600.Request(false);
            if (result < 0)
            {
                Statics.event00600.ReceiveData -= this.cspat00600out;
                MessageBox.Show(Statics.XingSession.GetErrorMessage(result));
            }
            else
            {
                int t = 0;
                while (Statics.check[0] == "2")
                {
                    Main.stock.tm.Delay(1);
                    if (t++ > 1000) break;
                }
                if (t < 1000) Statics.sblist[no].주문여부 = (gubun == "2") ? "매수" :  "매도";
            }
        }
        // 매도가 계산
        private int orderprice(int no)
        {
            int x = 500;
            for (int i = 0; i < 10; i++)
            {
                if (i % 2 == 0)
                    x *= 2;
                else
                    x *= 5;
                if (Statics.sblist[no].현재가 / x < 1)
                    break;
            }
            return (int)(Statics.sblist[no].율값 * Statics.secretdata.yield) / (x / 1000) * (x / 1000) + Statics.sblist[no].현재가;
        }

        public void cspat00600out(string trno)
        {
            Statics.event00600.ReceiveData -= this.cspat00600out;

            Statics.event00600.GetBlockCount("CSPAT00600OutBlock2");
            if (gubun == "2")
            {
                if (Statics.sblist[no].체결여부 != "매수완") Statics.sblist[no].체결여부 = "매수중";
                Statics.sblist[no].매수주문번호 = Statics.event00600.GetFieldData("CSPAT00600OutBlock2", "OrdNo", 0);
            }
            else
            {
                if (Statics.sblist[no].체결여부 != "매도완") Statics.sblist[no].체결여부 = "매도중";
                Statics.sblist[no].매도주문번호 = Statics.event00600.GetFieldData("CSPAT00600OutBlock2", "OrdNo", 0);
            }
            Statics.check[0] = "1";
        }
    }

    public class CSPAT00700
    {
        int no;
        public void cspat00700in(int no)
        {
            if (Statics.check[1] == "2") return;
            Statics.check[1] = "2";
            this.no = no;
            Statics.event00700.ReceiveData += this.cspat00700out;
            Statics.event00700.ResFileName = "/res/CSPAT00700.res";

            Statics.event00700.SetFieldData("CSPAT00700InBlock1", "OrgOrdNo", 0, Statics.sblist[no].매도주문번호);
            Statics.event00700.SetFieldData("CSPAT00700InBlock1", "AcntNo", 0, Statics.계좌번호);
            Statics.event00700.SetFieldData("CSPAT00700InBlock1", "InptPwd", 0, Statics.secretdata.password);
            Statics.event00700.SetFieldData("CSPAT00700InBlock1", "IsuNo", 0, Statics.sblist[no].긴종목코드);
            Statics.event00700.SetFieldData("CSPAT00700InBlock1", "OrdQty", 0, Statics.sblist[no].매도미체결.ToString());
            Statics.event00700.SetFieldData("CSPAT00700InBlock1", "OrdprcPtnCode", 0, "03");
            Statics.event00700.SetFieldData("CSPAT00700InBlock1", "OrdCndiTpCode", 0, "0");
            Statics.event00700.SetFieldData("CSPAT00700InBlock1", "OrdPrc", 0, "0");

            int result = Statics.event00700.Request(false);
            if (result < 0)
            {
                Statics.event00700.ReceiveData -= this.cspat00700out;
                MessageBox.Show(Statics.XingSession.GetErrorMessage(result));
            }
            else
            {
                int t = 0;
                while (Statics.check[1] == "2")
                {
                    Main.stock.tm.Delay(1);
                    if (t++ > 1000) break;
                }
            }
        }

        public void cspat00700out(string trno)
        {
            Statics.event00700.ReceiveData -= this.cspat00700out;

            Statics.sblist[no].원번호 = Statics.sblist[no].매도주문번호;
            Statics.event00700.GetBlockCount("CSPAT00700OutBlock2");
            Statics.sblist[no].매도주문번호 = Statics.event00700.GetFieldData("CSPAT00700OutBlock2", "OrdNo", 0);
            if (Statics.sblist[no].체결여부 != "정정완") Statics.sblist[no].체결여부 = "정정중";
            Statics.check[1] = "1";
        }
    }

    public class CSPAT00800
    {
        int no;
        public void cspat00800in(int no)
        {
            if (Statics.check[2] == "2") return;
            Statics.check[2] = "2";
            this.no = no;
            Statics.event00800.ReceiveData += this.cspat00800out;
            Statics.event00800.ResFileName = "/res/CSPAT00800.res";

            Statics.event00800.SetFieldData("CSPAT00800InBlock1", "OrgOrdNo", 0, Statics.sblist[no].매수주문번호);
            Statics.event00800.SetFieldData("CSPAT00800InBlock1", "AcntNo", 0, Statics.계좌번호);
            Statics.event00800.SetFieldData("CSPAT00800InBlock1", "InptPwd", 0, Statics.secretdata.password);
            Statics.event00800.SetFieldData("CSPAT00800InBlock1", "IsuNo", 0, Statics.sblist[no].긴종목코드);
            Statics.event00800.SetFieldData("CSPAT00800InBlock1", "OrdQty", 0, Statics.sblist[no].매수미체결.ToString());

            int result = Statics.event00800.Request(false);
            if (result < 0)
            {
                Statics.event00800.ReceiveData -= this.cspat00800out;
                MessageBox.Show(Statics.XingSession.GetErrorMessage(result));
            }
            else
            {
                int t = 0;
                while (Statics.check[2] == "2")
                {
                    Main.stock.tm.Delay(1);
                    if (t++ > 1000) break;
                }
            }
        }

        public void cspat00800out(string trno)
        {
            Statics.event00800.ReceiveData -= this.cspat00800out;

            if (Statics.sblist[no].체결여부 != "정정완") Statics.sblist[no].체결여부 = "정정중";
            Statics.check[2] = "1";
        }
    }

    public class Realdata
    {
        public void realstart()
        {
            Statics.eventsc1.ReceiveRealData += receive_sc1;
            Statics.eventsc1.ResFileName = "/res/SC1.res";
            Statics.eventsc1.AdviseRealData();
            Statics.eventsc2.ReceiveRealData += receive_sc2;
            Statics.eventsc2.ResFileName = "/res/SC2.res";
            Statics.eventsc2.AdviseRealData();
            Statics.eventsc3.ReceiveRealData += receive_sc3;
            Statics.eventsc3.ResFileName = "/res/SC3.res";
            Statics.eventsc3.AdviseRealData();

            Statics.eventS3_.ReceiveRealData += receive_price;
            Statics.eventK3_.ReceiveRealData += receive_price;
        }

        public void receive_sc1(string trno)
        {
            int no;

            Statics.eventsc1.GetBlockData("OutBlock");
            string 긴종목코드 = Statics.eventsc1.GetFieldData("OutBlock", "Isuno");
            for (no = 0; no < Statics.sblist.Count; no++) if (Statics.sblist[no].긴종목코드 == 긴종목코드) break;
            if (no >= Statics.sblist.Count) return;

            if (Statics.eventsc1.GetFieldData("OutBlock", "bnstp") == "2")
            {
                Statics.sblist[no].매수미체결 = Int32.Parse(Statics.eventsc1.GetFieldData("OutBlock", "unercqty"));
                Statics.sblist[no].매수주문번호 = Statics.eventsc1.GetFieldData("OutBlock", "ordno");
                Statics.sblist[no].매수체결시간 = Statics.eventsc1.GetFieldData("OutBlock", "exectime");
                Statics.sblist[no].매수수량 += Int32.Parse(Statics.eventsc1.GetFieldData("OutBlock", "execqty"));
                Statics.sblist[no].매수금액 += (Int32.Parse(Statics.eventsc1.GetFieldData("OutBlock", "execqty")) * Int32.Parse(Statics.eventsc1.GetFieldData("OutBlock", "execprc")));
                Statics.sblist[no].매수단가 = (int)(Statics.sblist[no].매수금액 / (long)Statics.sblist[no].매수수량);

                Statics.sblist[no].체결여부 = (Statics.sblist[no].매수미체결 > 0)  ? "매수중" : "매수완";
                Statics.check[0] = "1";
            }
            else
            {
                Statics.sblist[no].매도미체결 = Int32.Parse(Statics.eventsc1.GetFieldData("OutBlock", "unercqty"));
                Statics.sblist[no].매도주문번호 = Statics.eventsc1.GetFieldData("OutBlock", "ordno");
                Statics.sblist[no].매도체결시간 = Statics.eventsc1.GetFieldData("OutBlock", "exectime");
                Statics.sblist[no].매도수량 += Int32.Parse(Statics.eventsc1.GetFieldData("OutBlock", "execqty"));
                Statics.sblist[no].매도금액 += (Int32.Parse(Statics.eventsc1.GetFieldData("OutBlock", "execqty")) * Int32.Parse(Statics.eventsc1.GetFieldData("OutBlock", "execprc")));
                Statics.sblist[no].매도단가 = (int)(Statics.sblist[no].매도금액 / (long)Statics.sblist[no].매도수량);

                if (Statics.sblist[no].주문여부 == "정정")
                {
                    if (Statics.sblist[no].매도미체결 > 0)
                        Statics.sblist[no].체결여부 = "정정중";
                    else
                    {
                        Statics.sblist[no].체결여부 = "정정완";
                        Statics.sblist[no].차익실현 = Statics.sblist[no].매도금액 - Statics.sblist[no].매수금액;
                    }
                    Statics.check[1] = "1";
                }
                else
                {
                    if (Statics.sblist[no].매도미체결 > 0)
                        Statics.sblist[no].체결여부 = "매도중";
                    else
                    {
                        Statics.sblist[no].체결여부 = "매도완";
                        Statics.sblist[no].차익실현 = Statics.sblist[no].매도금액 - Statics.sblist[no].매수금액;
                    }
                    Statics.check[0] = "1";
                }
            }
        }

        public void receive_sc2(string trno)
        {
            int no;

            Statics.eventsc2.GetBlockData("OutBlock");
            string 긴종목코드 = Statics.eventsc2.GetFieldData("OutBlock", "Isuno");
            for (no = 0; no < Statics.sblist.Count; no++) if (Statics.sblist[no].긴종목코드 == 긴종목코드) break;
            if (no >= Statics.sblist.Count) return;

            string 원주문번호 = Statics.eventsc2.GetFieldData("OutBlock", "orgordno");
            if (Statics.sblist[no].매도주문번호 == "" || Statics.sblist[no].매도주문번호 != 원주문번호) return;

            if (Statics.sblist[no].체결여부 != "정정완") Statics.sblist[no].체결여부 = "정정중";
            Statics.check[1] = "1";
        }

        public void receive_sc3(string trno)
        {
            int no;

            Statics.eventsc3.GetBlockData("OutBlock");
            string 긴종목코드 = Statics.eventsc3.GetFieldData("OutBlock", "Isuno");
            for (no = 0; no < Statics.sblist.Count; no++) if (Statics.sblist[no].긴종목코드 == 긴종목코드) break;
            if (no >= Statics.sblist.Count) return;

            string 원주문번호 = Statics.eventsc3.GetFieldData("OutBlock", "orgordno");
            if (Statics.sblist[no].매수주문번호 == "" || Statics.sblist[no].매수주문번호 != 원주문번호) return;

            Statics.sblist[no].매수미체결 = Int32.Parse(Statics.eventsc3.GetFieldData("OutBlock", "unercqty"));
            Statics.sblist[no].체결여부 = (Statics.sblist[no].매수미체결 == 0 && Statics.sblist[no].매수수량 == Statics.sblist[no].매도수량) ? "정정완" : "정정중";
            Statics.check[2] = "1";
        }

        public void send_price(string code, string gubun)
        {
            if (gubun == "1")
            {
                Statics.eventS3_.ResFileName = "/res/S3_.res";
                Statics.eventS3_.SetFieldData("InBlock", "shcode", code);
                Statics.eventS3_.AdviseRealData();
            }
            else
            {
                Statics.eventK3_.ResFileName = "/res/K3_.res";
                Statics.eventK3_.SetFieldData("InBlock", "shcode", code);
                Statics.eventK3_.AdviseRealData();
            }
        }

        public void receive_price(string trno)
        {
            int no = 0;
            if (trno == "S3_")
            {
                Statics.eventS3_.GetBlockData("OutBlock");
                for (; no < Statics.sblist.Count; no++)
                    if (Statics.eventS3_.GetFieldData("OutBlock", "shcode") == Statics.sblist[no].종목코드) break;
                if (no > Statics.sblist.Count) return;
                Statics.sblist[no].현재가 = Int32.Parse(Statics.eventS3_.GetFieldData("OutBlock", "price"));
                if (Statics.sblist[no].주문여부 == "매도" || Statics.sblist[no].주문여부 == "정정")
                    Statics.sblist[no].매수대비율 = (float)(Statics.sblist[no].현재가 - Statics.sblist[no].매수단가) / Statics.sblist[no].율값;
                Statics.sblist[no].전일대비 = Int32.Parse(Statics.eventS3_.GetFieldData("OutBlock", "change"));
                Statics.sblist[no].등락율 = float.Parse(Statics.eventS3_.GetFieldData("OutBlock", "drate"));
            }
            else if (trno == "K3_")
            {
                Statics.eventK3_.GetBlockData("OutBlock");
                for (; no < Statics.sblist.Count; no++)
                    if (Statics.eventK3_.GetFieldData("OutBlock", "shcode") == Statics.sblist[no].종목코드) break;
                if (no > Statics.sblist.Count) return;
                Statics.sblist[no].현재가 = Int32.Parse(Statics.eventK3_.GetFieldData("OutBlock", "price"));
                if (Statics.sblist[no].주문여부 == "매도" || Statics.sblist[no].주문여부 == "정정")
                    Statics.sblist[no].매수대비율 = (float)(Statics.sblist[no].현재가 - Statics.sblist[no].매수단가) / Statics.sblist[no].율값;
                Statics.sblist[no].전일대비 = Int32.Parse(Statics.eventK3_.GetFieldData("OutBlock", "change"));
                Statics.sblist[no].등락율 = float.Parse(Statics.eventK3_.GetFieldData("OutBlock", "drate"));
            }
            if (Statics.sblist[no].체결여부 == "매도중" && (Statics.sblist[no].매수대비율 <= (Statics.secretdata.loss * -1)))
                Statics.sblist[no].주문여부 = "정정";
        }

        public void realend()
        {
            Statics.eventsc1.UnadviseRealData();
            Statics.eventsc1.ReceiveRealData -= receive_sc1;
            Statics.eventsc2.UnadviseRealData();
            Statics.eventsc2.ReceiveRealData -= receive_sc2;
            Statics.eventsc3.UnadviseRealData();
            Statics.eventsc3.ReceiveRealData -= receive_sc3;

            Statics.eventS3_.UnadviseRealData();
            Statics.eventK3_.UnadviseRealData();
            Statics.eventS3_.ReceiveRealData -= receive_price;
            Statics.eventK3_.ReceiveRealData -= receive_price;
        }
    }

    // DataGridView 설정
    public class Table
    {
        public DataGridView dgv = new DataGridView();
        // dgv 설정
        public void basicsetdgv()
        {
            int[] colnum = { 0, 1, 6, 7 }, money = { 2, 3, 9, 10, 13, 14, 16 }, numb = { 8, 11, 12, 15 };
            ExtensionMethods.DoubleBuffered(dgv, true);
            dgv.DefaultCellStyle.BackColor = Color.FromArgb(240, 248, 255);
            dgv.DefaultCellStyle.SelectionForeColor = Color.Yellow;
            dgv.RowHeadersVisible = false;
            dgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            dgv.ColumnHeadersHeight = 40;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            foreach (int i in colnum)
                dgv.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            foreach (int i in money)
                dgv.Columns[i].DefaultCellStyle.Format = "#,##0원";
            foreach (int i in numb)
                dgv.Columns[i].DefaultCellStyle.Format = "#,##0";
            dgv.Columns[4].DefaultCellStyle.Format = "0.#0";
            dgv.Columns[5].DefaultCellStyle.Format = "0.#0";
            dgv.ReadOnly = true;
            dgv.AllowUserToAddRows = false;
        }

        public void settingdgv()
        {
            dgv.RowTemplate.Height = 50;                                                 // 행높이 설정하려면 dgv 바인딩하기 전에 지정해야함
            dgv.Font = new Font(Statics.글꼴, 11, FontStyle.Bold);
            dgv.DataSource = Statics.sblist;
            for (int i = 0; i < Statics.width.Length; i++)
                dgv.Columns[i].Width = Statics.width[i];
            basicsetdgv();
            clickcell(0);
        }

        public void clickcell(int currow)
        {
            if (dgv.RowCount < 1) return;
            dgv.Columns[1].Selected = true;
            dgv.Rows[currow].Selected = true;
            dgv.CurrentCell = dgv[1, currow];
        }
    }

    public class Statics
    {
        public static Boolean END = false, event1302pass = true, loginchk = true;
        public static double totmoney = 0, money = 0, balance = 0;
        // public static SortableBindingList<Data> sblist = new SortableBindingList<Data>();
        public static BindingList<Data> sblist = new BindingList<Data>();
        public static int[] width = new int[] { 90, 200, 130, 100, 80, 80, 70, 70, 90, 130, 150, 90, 90, 130, 150, 90, 145 };
        public static int accountnum = 0, testnum = 0;
        public static Secret secretdata;
        public static List<Basicdata> basiclist = new List<Basicdata>();

        public static List<Medata> med = new List<Medata>();
        public static List<string> except = new List<string>();
        public static string 계좌번호, 글꼴 = "굴림", dirpath = Directory.GetCurrentDirectory(), nextkey = "";
        public static string[] check = new string[] { "0", "0", "0" };

        public static XASessionClass XingSession = new XASessionClass();
        public static XAQueryClass event12300 = new XAQueryClass();
        public static XAQueryClass event8430 = new XAQueryClass();
        public static XAQueryClass event1102 = new XAQueryClass();
        public static XAQueryClass event1442 = new XAQueryClass();
        public static XAQueryClass event1302 = new XAQueryClass();
        public static XAQueryClass event00600 = new XAQueryClass();
        public static XAQueryClass event00700 = new XAQueryClass();
        public static XAQueryClass event00800 = new XAQueryClass();
        public static XAQueryClass event0150 = new XAQueryClass();

        public static XARealClass eventsc1 = new XARealClass();
        public static XARealClass eventsc2 = new XARealClass();
        public static XARealClass eventsc3 = new XARealClass();
        public static XARealClass eventS3_ = new XARealClass();
        public static XARealClass eventK3_ = new XARealClass();
    }

    public class SortableBindingList<T> : BindingList<T>
    {
        private bool isSortedValue;
        ListSortDirection sortDirectionValue;
        PropertyDescriptor sortPropertyValue;

        public SortableBindingList()
        {
        }

        public SortableBindingList(IList<T> list)
        {
            foreach (object o in list)
            {
                this.Add((T)o);
            }
        }

        protected override void ApplySortCore(PropertyDescriptor prop,
            ListSortDirection direction)
        {
            Type interfaceType = prop.PropertyType.GetInterface("IComparable");

            if (interfaceType == null && prop.PropertyType.IsValueType)
            {
                Type underlyingType = Nullable.GetUnderlyingType(prop.PropertyType);

                if (underlyingType != null)
                {
                    interfaceType = underlyingType.GetInterface("IComparable");
                }
            }

            if (interfaceType != null)
            {
                sortPropertyValue = prop;
                sortDirectionValue = direction;

                IEnumerable<T> query = base.Items;

                if (direction == ListSortDirection.Ascending)
                {
                    query = query.OrderBy(i => prop.GetValue(i));
                }
                else
                {
                    query = query.OrderByDescending(i => prop.GetValue(i));
                }

                int newIndex = 0;
                foreach (object item in query)
                {
                    this.Items[newIndex] = (T)item;
                    newIndex++;
                }

                isSortedValue = true;
                this.OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
            }
            else
            {
                throw new NotSupportedException("Cannot sort by " + prop.Name +
                    ". This" + prop.PropertyType.ToString() +
                    " does not implement IComparable");
            }
        }

        protected override PropertyDescriptor SortPropertyCore
        {
            get { return sortPropertyValue; }
        }

        protected override ListSortDirection SortDirectionCore
        {
            get { return sortDirectionValue; }
        }

        protected override bool SupportsSortingCore
        {
            get { return true; }
        }

        protected override bool IsSortedCore
        {
            get { return isSortedValue; }
        }
    }

    public class Data
    {
        [Browsable(false)]
        public string 구분 { get; set; }
        [Browsable(false)]
        public string 긴종목코드 { get; set; }
        public string 종목코드 { get; set; }
        public string 종목이름 { get; set; }
        public int 현재가 { get; set; }
        public int 전일대비 { get; set; }
        public float 등락율 { get; set; }
        [Browsable(false)]
        public int 율값 { get; set; }
        public float 매수대비율 { get; set; }
        public string 주문여부 { get; set; }
        public string 체결여부 { get; set; }
        public int 매수수량 { get; set; }
        public int 매수단가 { get; set; }
        public long 매수금액 { get; set; }
        public int 매수미체결 { get; set; }
        [Browsable(false)]
        public string 매수주문번호 { get; set; }
        [Browsable(false)]
        public string 매수체결시간 { get; set; }
        public int 매도수량 { get; set; }
        public int 매도단가 { get; set; }
        public long 매도금액 { get; set; }
        public int 매도미체결 { get; set; }
        [Browsable(false)]
        public string 매도주문번호 { get; set; }
        [Browsable(false)]
        public string 원번호 { get; set; }
        [Browsable(false)]
        public string 매도체결시간 { get; set; }
        [Browsable(false)]
        public int 수수료 { get; set; }
        [Browsable(false)]
        public int 거래세 { get; set; }
        [Browsable(false)]
        public int 농특세 { get; set; }
        [Browsable(false)]
        public int 세금합계 { get; set; }
        public long 차익실현 { get; set; }

        public Data(string 구분, string 긴종목코드, string 종목코드, string 종목이름,
            int 현재가, int 전일대비, float 등락율, int 율값, float 매수대비율, string 주문여부, string 체결여부,
            int 매수수량, int 매수단가, long 매수금액, int 매수미체결, string 매수주문번호, string 매수체결시간,
            int 매도수량, int 매도단가, long 매도금액, int 매도미체결, string 매도주문번호, string 원번호, string 매도체결시간,
            int 수수료, int 거래세, int 농특세, int 세금합계, long 차익실현)
        {
            this.구분 = 구분;
            this.긴종목코드 = 긴종목코드;
            this.종목코드 = 종목코드;
            this.종목이름 = 종목이름;

            this.현재가 = 현재가;
            this.전일대비 = 전일대비;
            this.등락율 = 등락율;
            this.율값 = 율값;
            this.매수대비율 = 매수대비율;
            this.주문여부 = 주문여부;
            this.체결여부 = 체결여부;

            this.매수수량 = 매수수량;
            this.매수단가 = 매수단가;
            this.매수금액 = 매수금액;
            this.매수미체결 = 매수미체결;
            this.매수주문번호 = 매수주문번호;
            this.매수체결시간 = 매수체결시간;

            this.매도수량 = 매도수량;
            this.매도단가 = 매도단가;
            this.매도금액 = 매도금액;
            this.매도미체결 = 매도미체결;
            this.매도주문번호 = 매도주문번호;
            this.원번호 = 원번호;
            this.매도체결시간 = 매도체결시간;

            this.수수료 = 수수료;
            this.거래세 = 거래세;
            this.농특세 = 농특세;
            this.세금합계 = 세금합계;
            this.차익실현 = 차익실현;
        }
    }

    // 모든 종목에서 추출할 정보
    public class Basicdata
    {
        public string 종목이름, 종목코드, 긴종목코드, 구분;

        public Basicdata(string 종목이름, string 종목코드, string 긴종목코드, string 구분)
        {
            this.종목이름 = 종목이름;
            this.종목코드 = 종목코드;
            this.긴종목코드 = 긴종목코드;
            this.구분 = 구분;
        }
    }

    public class Secret
    {
        public string id, logpass, activepass, password, url;
        public int max, volume, starttime, endtime;
        public float sratio, eratio, limit, yield, loss;

        public Secret(string id, string logpass, string activepass, string password, string url, int max, int volume, float sratio, float eratio, float limit, float yield, float loss, int starttime, int endtime)
        {
            this.id = id;
            this.logpass = logpass;
            this.activepass = activepass;
            this.password = password;
            this.url = url;
            this.max = max;
            this.volume = volume;
            this.sratio = sratio;
            this.eratio = eratio;
            this.limit = limit;
            this.yield = yield;
            this.loss = loss;
            this.starttime = starttime;
            this.endtime = endtime;
        }
    }

    // 문자열과 숫자 변환 클래스
    public class Change_string
    {
        // 문자열을 숫자와 - 부호를 제외한 모든 값을 제거하고 더블값으로 변환하여 반환
        public double strtodouble(string str)
        {
            string rtn = strtostr(str);
            if (string.IsNullOrEmpty(rtn)) return 0;
            else return double.Parse(rtn);
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
            while (value[i] < 49 || value[i] > 57)
            {
                i++;
                if (i >= temp.Length) break;
            }
            for (; i < temp.Length; i++) if (char.IsDigit(value[i])) tmp[j++] = value[i];
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
            char[] temp = new char[sval.Length + sval.Length / 3 + 2];

            value = sval.ToCharArray();
            int k = 0;
            for (int i = sval.Length - 1, j = 0; i > -1; i--, j++, k++)
            {
                if (j == 3 && value[i] != '-')
                {
                    temp[k++] = ',';
                    j = 0;
                }
                temp[k] = value[i];
            }
            char swap;
            for (int i = 0, j = k - 1; i < k / 2; i++, j--)
            {
                swap = temp[i];
                temp[i] = temp[j];
                temp[j] = swap;
            }
            temp[k] = '원';
            return new String(temp);
        }
    }
    // dgv 속도 향상 클래스
    public static class ExtensionMethods
    {
        public static void DoubleBuffered(this DataGridView dgv, bool setting)
        {
            Type dgvType = dgv.GetType();
            PropertyInfo pi = dgvType.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            pi.SetValue(dgv, setting, null);
        }
    }

    public class Main
    {
        public static Stock stock;
    }
}