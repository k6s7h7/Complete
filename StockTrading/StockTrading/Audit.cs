using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StockTrading
{
    public partial class Audit : Form
    {
        public Dgv dgv = new Dgv();
        public T0150 t0150 = new T0150();

        public Audit()
        {
            InitializeComponent();
        }

        private void Audit_Load(object sender, EventArgs e)
        {
            this.Controls.Add(dgv.dgv);
            t0150.t0150in();
            Main.stock.tm.Delay(1000);
            dgv.settingdgv();
            writedgv();
        }

        private void writedgv()
        {
            for (int i = 0; i < Statics.med.Count; i++)
                dgv.dgv.Rows.Add(Statics.med[i].code, Statics.med[i].name, Statics.med[i].msqunt, Statics.med[i].msprice, Statics.med[i].msamount,
                Statics.med[i].mdqunt, Statics.med[i].mdprice, Statics.med[i].mdamount, Statics.med[i].fee, Statics.med[i].tax, Statics.med[i].argtax, Statics.med[i].adjamt);

            dgv.dgv.Rows.Add("", "총계", "", "", t0150.sumsu, "", "", t0150.sumdo, t0150.tfee, t0150.ttax, t0150.targtax, t0150.adjust);
            dgv.clickcell(dgv.dgv.RowCount - 1);
            label1.Text = "오늘 주식매매는 끝났습니다. 총 " + Statics.med.Count.ToString() + "건에 총수익은 " + t0150.adjust.ToString("#,##0원") + "입니다";
        }
    }

    public class T0150
    {
        public long sumsu, sumdo, adjust;
        public int tfee, ttax, targtax;

        public void t0150in()
        {
            Statics.event0150.ReceiveData += t0150out;

            Statics.event0150.ResFileName = "/res/t0150.res";
            Statics.event0150.SetFieldData("t0150InBlock", "accno", 0, Statics.계좌번호);

            int result = Statics.event0150.Request(false);
            if (result < 0)
            {
                MessageBox.Show(Statics.XingSession.GetErrorMessage(result));
                Statics.event0150.ReceiveData -= t0150out;
            }
        }

        public string searchname(string code)
        {
            int i;
            for (i = 0; i < Statics.basiclist.Count; i++)
                if (Statics.basiclist[i].종목코드 == code) break;
            if (i < Statics.basiclist.Count)
                return Statics.basiclist[i].종목이름;
            return "";
        }

        public void t0150out(string trno)
        {
            string code = "", name = "", pregubun = "", gubun = "";
            int qunt = 0, price = 0, fee = 0, tax = 0, argtax = 0;
            long amount = 0, adjamt = 0;

            Statics.event0150.ReceiveData -= t0150out;

            Statics.event0150.GetBlockCount("t0150OutBlock");
            sumsu = long.Parse(Statics.event0150.GetFieldData("t0150OutBlock", "msamt", 0));
            sumdo = long.Parse(Statics.event0150.GetFieldData("t0150OutBlock", "mdamt", 0));
            tfee = Int32.Parse(Statics.event0150.GetFieldData("t0150OutBlock", "tfee", 0));
            ttax = Int32.Parse(Statics.event0150.GetFieldData("t0150OutBlock", "tottax", 0));
            targtax = Int32.Parse(Statics.event0150.GetFieldData("t0150OutBlock", "targtax", 0));
            adjust = long.Parse(Statics.event0150.GetFieldData("t0150OutBlock", "tadjamt", 0));


            int count = Statics.event0150.GetBlockCount("t0150OutBlock1");

            for (int i = 0; i < count; i++)
            {
                gubun = Statics.event0150.GetFieldData("t0150OutBlock1", "medosu", i).Trim();
                if (gubun == "종목소계")
                {
                    code = Statics.event0150.GetFieldData("t0150OutBlock1", "expcode", i - 1);
                    qunt = Int32.Parse(Statics.event0150.GetFieldData("t0150OutBlock1", "qty", i));
                    price = Int32.Parse(Statics.event0150.GetFieldData("t0150OutBlock1", "price", i));
                    amount = long.Parse(Statics.event0150.GetFieldData("t0150OutBlock1", "amt", i));
                    fee = Int32.Parse(Statics.event0150.GetFieldData("t0150OutBlock1", "fee", i));
                    tax = Int32.Parse(Statics.event0150.GetFieldData("t0150OutBlock1", "tax", i));
                    argtax = Int32.Parse(Statics.event0150.GetFieldData("t0150OutBlock1", "argtax", i));
                    adjamt = long.Parse(Statics.event0150.GetFieldData("t0150OutBlock1", "adjamt", i));

                    if (pregubun == "매도")
                    {
                        int cnt = Statics.med.Count - 1;
                        if (cnt >= 0 && Statics.med[cnt].code == code)
                        {
                            Statics.med[cnt].mdqunt += qunt;
                            Statics.med[cnt].mdamount += amount;
                            Statics.med[cnt].fee += fee;
                            Statics.med[cnt].tax += tax;
                            Statics.med[cnt].argtax += argtax;
                            Statics.med[cnt].mdadjamt += adjamt;
                        }
                        else
                        {
                            name = searchname(code);
                            Statics.med.Add(new Medata(code, name, 0, 0, 0, 0, qunt, price, amount, adjamt, fee, tax, argtax, 0));
                        }
                    }
                    else if(pregubun == "매수")
                    {
                        int j;
                        for (j = 0; j < Statics.med.Count; j++)
                            if (Statics.med[j].code == code) break;
                        if (j >= Statics.med.Count)
                        {
                            name = searchname(code);
                            Statics.med.Add(new Medata(code, name, qunt, price, amount, adjamt, 0, 0, 0, 0, fee, tax, argtax, 0));
                        }
                        else
                        {
                            Statics.med[j].msqunt += qunt;
                            Statics.med[j].msprice = price;
                            Statics.med[j].msamount += amount;
                            Statics.med[j].fee += fee;
                            Statics.med[j].tax += tax;
                            Statics.med[j].argtax += argtax;
                            Statics.med[j].msadjamt = adjamt;
                            Statics.med[j].adjamt = Statics.med[j].mdadjamt - Statics.med[j].msadjamt;
                        }
                    }
                }
                pregubun = gubun;
            }
        }
    }

    public class Dgv
    {
        public DataGridView dgv = new DataGridView();

        public void settingdgv()
        {
            dgv.RowTemplate.Height = 50;                                                 // 행높이 설정하려면 dgv 바인딩하기 전에 지정해야함
            basicsetdgv();
            clickcell(0);
        }

        public void basicsetdgv()
        {
            string[] name = new string[] { "종목코드", "종목명", "매수수량", "매수단가", "매수금액", "매도수량", "매도단가", "매도금액", "수수료", "거래세", "농특세", "차익실현" };
            int[] width = { 180, 200, 110, 120, 180, 110, 120, 180, 120, 120, 120, 180 };
            dgv.ColumnCount = name.Length;
            for (int i = 0; i < name.Length; i++)
            {
                dgv.Columns[i].HeaderText = name[i];
                dgv.Columns[i].Width = width[i];
            }
            ExtensionMethods.DoubleBuffered(dgv, true);
            dgv.Bounds = new Rectangle(20, 70, 1760, 600);
            dgv.DefaultCellStyle.BackColor = Color.FromArgb(240, 248, 255);
            dgv.DefaultCellStyle.SelectionForeColor = Color.Yellow;
            dgv.RowHeadersVisible = false;
            dgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            dgv.ColumnHeadersHeight = 40;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv.RowTemplate.Height = 50;
            dgv.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgv.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            for (int i = 2; i < name.Length; i++)
            {
                if (i == 2 || i == 5)
                    dgv.Columns[i].DefaultCellStyle.Format = "#,##0";
                else
                    dgv.Columns[i].DefaultCellStyle.Format = "#,##0원";
            }
            dgv.ReadOnly = true;
            dgv.AllowUserToAddRows = false;
            dgv.Font = new Font(Statics.글꼴, 12, FontStyle.Bold);
        }

        public void clickcell(int currow)
        {
            if (dgv.RowCount < 1) return;
            dgv.Columns[1].Selected = true;
            dgv.Rows[currow].Selected = true;
            dgv.CurrentCell = dgv[1, currow];
        }
    }

    public class Medata
    {
        public string code, name;
        public int msqunt, msprice, mdqunt, mdprice, fee, tax, argtax;
        public long msamount, mdamount, msadjamt, mdadjamt, adjamt;

        public Medata(string code, string name, int msqunt, int msprice, long msamount, long msadjamt, int mdqunt, int mdprice, long mdamount, long mdadjamt, int fee, int tax, int argtax, long adjamt)
        {
            this.code = code;
            this.name = name;
            this.msqunt = msqunt;
            this.msprice = msprice;
            this.msamount = msamount;
            this.msadjamt = msadjamt;
            this.mdqunt = mdqunt;
            this.mdprice = mdprice;
            this.mdamount = mdamount;
            this.mdadjamt = mdadjamt;
            this.fee = fee;
            this.tax = tax;
            this.argtax = argtax;
            this.adjamt = adjamt;
        }
    }
}
