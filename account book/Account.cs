using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace account_book
{
    public partial class Account : Form
    {
        public Label savelabel = new Label(), dellabel1 = new Label(), dellabel2 = new Label();
        public DataGridView bankdgv = new DataGridView(), carddgv = new DataGridView();
        public int bankrow = 0, cardrow = 0;
        public Boolean chkdgv = false;
        public Accontgroup accontgroup;
        public TextBox money;
        public Account()
        {
            InitializeComponent();
        }

        private void Account_Load(object sender, EventArgs e)
        {
            Acnt.account = this;
            
            this.BackColor = Color.FromArgb(218, 223, 232);
            this.AutoScroll = true;
            this.Text = "계정관리";
            this.Font = new Font(Statics.font, 14, FontStyle.Bold);
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.Controls.Add(bankdgv);
            this.Controls.Add(carddgv);
            accontgroup = new Accontgroup();
            makebutton();
            makecash();
            makedgv();
            this.FormClosing += new FormClosingEventHandler(closeform);
        }

        public void closeform(object sender, FormClosingEventArgs e)
        {
            if (chkdgv)
            {
                if (MessageBox.Show("변경 내용이 있습니다. 저장하시겠습니까?", "변경내용 저장", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    Statics.cash = Globals.house.strchn.strtodub(money.Text);
                    Globals.house.fm.writefile();
                }
                else Globals.house.fm.readfile();
            }
            Globals.house.calcu.showsum();
            this.Dispose();
        }

        public void makecash()
        {
            Label label = new Label();
            money = new TextBox();
            label.Parent = this;
            label.Text = "현    금";
            label.Bounds = new Rectangle(500, 30, 90, 30);

            money.Parent = this;
            money.Bounds = new Rectangle(620, 24, 250, 40);
            money.Text = Statics.cash.ToString("#,##0원");
            money.TextAlign = HorizontalAlignment.Right;
            money.Click += new EventHandler(Globals.house.inputgroup.moneyclick);
            money.Leave += new EventHandler(Globals.house.inputgroup.moneyleave);
            money.KeyUp += new KeyEventHandler(Globals.house.inputgroup.textkeyup);
            money.KeyDown += new KeyEventHandler(Globals.house.inputgroup.textkeydown);
            money.TextChanged += changedgv;
        }

        public void makebutton()
        {
            savelabel.Parent = this;
            savelabel.Bounds = new Rectangle(140, 20, 130, 40);
            savelabel.Text = "저    장";
            savelabel.Font = new Font(Statics.font, 16, FontStyle.Bold);
            savelabel.TextAlign = ContentAlignment.MiddleCenter;
            savelabel.Image = Image.FromFile(Statics.dirpath + "\\button\\normal.png");
            savelabel.MouseHover += new EventHandler(Globals.house.savedelhov);
            savelabel.MouseLeave += new EventHandler(Globals.house.savedellve);
            savelabel.Click += new EventHandler(saveclk);

            dellabel1.Parent = this;
            dellabel1.Text = "삭    제";
            dellabel1.Bounds = new Rectangle(650, 410, 130, 40);
            dellabel1.Font = new Font(Statics.font, 16, FontStyle.Bold);
            dellabel1.TextAlign = ContentAlignment.MiddleCenter;
            dellabel1.Image = Image.FromFile(Statics.dirpath + "\\button\\normal.png");
            dellabel1.MouseHover += new EventHandler(Globals.house.savedelhov);
            dellabel1.MouseLeave += new EventHandler(Globals.house.savedellve);
            dellabel1.Click += new EventHandler(delclk);

            dellabel2.Parent = this;
            dellabel2.Text = "삭    제";
            dellabel2.Bounds = new Rectangle(650, 810, 130, 40);
            dellabel2.Font = new Font(Statics.font, 16, FontStyle.Bold);
            dellabel2.TextAlign = ContentAlignment.MiddleCenter;
            dellabel2.Image = Image.FromFile(Statics.dirpath + "\\button\\normal.png");
            dellabel2.MouseHover += new EventHandler(Globals.house.savedelhov);
            dellabel2.MouseLeave += new EventHandler(Globals.house.savedellve);
            dellabel2.Click += new EventHandler(delclk);
        }

        public void saveclk(object sender, EventArgs e)
        {
            savelabel.Image = Image.FromFile(Statics.dirpath + "\\button\\click.png");

            if (chkdgv)
            {
                Statics.cash = Globals.house.strchn.strtodub(money.Text);
                Globals.house.fm.writefile();
            }
            chkdgv = false;
        }

        public void delclk(object sender, EventArgs e)
        {
            ((Label)sender).MouseLeave -= Globals.house.savedellve;
            ((Label)sender).Image = Image.FromFile(Statics.dirpath + "\\button\\click.png");
            if (MessageBox.Show("삭제합니다. 확실하십니까 ?", "삭제합니다", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                if (sender == dellabel1) bankdgv.Rows.Remove(bankdgv.Rows[bankrow]);
                else carddgv.Rows.Remove(carddgv.Rows[cardrow]);
            }
            ((Label)sender).MouseLeave += Globals.house.savedellve;
            ((Label)sender).Image = Image.FromFile(Statics.dirpath + "\\button\\normal.png");
        }

        public void settingdgv(DataGridView dgv, string[] header, int[] width, int sel, int num)
        {
            ExtensionMethods.DoubleBuffered(dgv, true);
            dgv.Font = new Font(Statics.font, 14, FontStyle.Bold);
            for (int i = 0; i < header.Length; i++) dgv.Columns[i].HeaderText = header[i];
            for (int i = 0; i < width.Length; i++) dgv.Columns[i].Width = width[i];
            dgv.ColumnHeadersHeight = 40;
            Globals.house.basicsetdgv(dgv, num);
            Globals.house.clickcell(dgv, 0);
            dgv.DataBindingComplete += new DataGridViewBindingCompleteEventHandler(changedgv);
        }

        public void makedgv()
        {
            string[] bankheader = new string[] { "통장 이름", "잔     액" };
            string[] cardheader = new string[] { "카드 이름", "결재 통장", "미결재금액" };
            int[] bankwidth = new int[] { 220, 310 };
            int[] cardwidth = new int[] { 180, 180, 240 };
            bankdgv.Bounds = new Rectangle(450, 100, 550, 300);
            carddgv.Bounds = new Rectangle(410, 500, 620, 300);
            bankdgv.RowTemplate.Height = 30;
            carddgv.RowTemplate.Height = 30;

            bankdgv.DataSource = Globals.house.banklist;
            carddgv.DataSource = Globals.house.cardlist;
            settingdgv(bankdgv, bankheader, bankwidth, 0, 1);
            settingdgv(carddgv, cardheader, cardwidth, 1, 2);

            bankdgv.CellClick += new DataGridViewCellEventHandler(bankclick);
            carddgv.CellClick += new DataGridViewCellEventHandler(cardclick);
        }

        public void bankclick(object sender, EventArgs e)
        {
            bankrow = bankdgv.CurrentCell.RowIndex;
            accontgroup.radiobank.Checked = accontgroup.radiomend.Checked = true;
            accontgroup.textname.Text = bankdgv[0, bankrow].Value.ToString();
            accontgroup.combo.Enabled = false;
            accontgroup.textmoney.Text = double.Parse(bankdgv[1, bankrow].Value.ToString()).ToString("#,##0원");
        }

        public void cardclick(object sender, EventArgs e)
        {
            cardrow = carddgv.CurrentCell.RowIndex;
            accontgroup.radiocard.Checked = accontgroup.radiomend.Checked = true;
            accontgroup.textname.Text = carddgv[0, cardrow].Value.ToString();
            accontgroup.addcombo();
            accontgroup.textmoney.Text = double.Parse(carddgv[2, cardrow].Value.ToString()).ToString("#,##0원");
        }

        public void changedgv(object sender, EventArgs e)
        {
            chkdgv = true;
        }
    }
    public class Accontgroup
    {
        public WATGroupBox acbox = new WATGroupBox();
        public RadioButton radiobank = new RadioButton(), radiocard = new RadioButton(), radioadd = new RadioButton(), radiomend = new RadioButton();
        public Label namelabel = new Label(), input = new Label(), mend = new Label();
        public ComboBox combo = new ComboBox();
        public TextBox textname = new TextBox(), textmoney = new TextBox();

        public Accontgroup()
        {
            acbox.BorderColor = Color.Blue;
            acbox.Parent = Acnt.account;
            acbox.Bounds = new Rectangle(20, 100, 360, 300);
            acbox.BackColor = Color.FromArgb(240, 248, 255);
            acbox.Font = new Font(Statics.font, 14, FontStyle.Bold);
            makeradio();
            makelabel();
            textcombo();
            radiobank.CheckedChanged += new EventHandler(bankcard);
            radiobank.Checked = true;
        }

        public void bankcard(object sender, EventArgs e)
        {
            if(radiobank.Checked)
            {
                namelabel.Text = "통장 이름";
                combo.Enabled = false;
            }
            else
            {
                namelabel.Text = "카드 이름";
                combo.Enabled = true;
                combo.Items.Clear();
                for (int i = 0; i < Globals.house.banklist.Count; i++) combo.Items.Add(Globals.house.banklist[i].name);
            }
        }

        public void makeradio()
        {
            radiobank.Parent = acbox;
            radiobank.Bounds = new Rectangle(100, 10, 70, 30);
            radiobank.Text = "통장";

            radiocard.Parent = acbox;
            radiocard.Bounds = new Rectangle(220, 10, 70, 30);
            radiocard.Text = "카드";
        }

        public void makelabel()
        {
            Label paylabel = new Label(), moneylabel = new Label();

            namelabel.Parent = acbox;
            namelabel.Bounds = new Rectangle(10, 80, 100, 30);

            paylabel.Parent = acbox;
            paylabel.Text = "결재 통장";
            paylabel.Bounds = new Rectangle(10, 130, 100, 30);

            moneylabel.Parent = acbox;
            moneylabel.Text = "잔    액";
            moneylabel.Bounds = new Rectangle(10, 180, 100, 30);

            input.Parent = acbox;
            input.Bounds = new Rectangle(30, 240, 130, 40);
            input.Text = "추    가";
            input.Font = new Font(Statics.font, 16, FontStyle.Bold);
            input.TextAlign = ContentAlignment.MiddleCenter;
            input.Image = Image.FromFile(Statics.dirpath + "\\button\\normal.png");
            input.MouseHover += new EventHandler(Globals.house.savedelhov);
            input.MouseLeave += new EventHandler(Globals.house.savedellve);
            input.Click += new EventHandler(inputclk);

            mend.Parent = acbox;
            mend.Bounds = new Rectangle(200, 240, 130, 40);
            mend.Text = "수    정";
            mend.Font = new Font(Statics.font, 16, FontStyle.Bold);
            mend.TextAlign = ContentAlignment.MiddleCenter;
            mend.Image = Image.FromFile(Statics.dirpath + "\\button\\normal.png");
            mend.MouseHover += new EventHandler(Globals.house.savedelhov);
            mend.MouseLeave += new EventHandler(Globals.house.savedellve);
            mend.Click += new EventHandler(mendclk);
        }

        public void inputclk(object sender, EventArgs e)
        {
            input.Image = Image.FromFile(Statics.dirpath + "\\button\\click.png");
            if (radiobank.Checked) Globals.house.banklist.Add(new Bank(textname.Text, Globals.house.strchn.strtodub(textmoney.Text)));
            else Globals.house.cardlist.Add(new Card(textname.Text, combo.Text, Globals.house.strchn.strtodub(textmoney.Text)));
            cleargroup();
        }

        public void mendclk(object sender, EventArgs e)
        {
            mend.Image = Image.FromFile(Statics.dirpath + "\\button\\click.png");
            if (radiobank.Checked)
            {
                Globals.house.banklist[Acnt.account.bankrow].name = textname.Text;
                Globals.house.banklist[Acnt.account.bankrow].money = Globals.house.strchn.strtodub(textmoney.Text);
                Acnt.account.bankdgv.Refresh();
            }
            else
            {
                Globals.house.cardlist[Acnt.account.cardrow].name = textname.Text;
                Globals.house.cardlist[Acnt.account.cardrow].payment = combo.Text;
                Globals.house.cardlist[Acnt.account.cardrow].money = Globals.house.strchn.strtodub(textmoney.Text);
                Acnt.account.carddgv.Refresh();
                combo.Items.Clear();
                for (int i = 0; i < Globals.house.banklist.Count; i++) combo.Items.Add(Globals.house.banklist[i].name);
            }
            Acnt.account.changedgv(sender, e);
            cleargroup();
        }

        public void textcombo()
        {
            textname.Parent = acbox;
            textname.Bounds = new Rectangle(120, 75, 220, 30);
            textname.BackColor = Color.FromArgb(209, 220, 245);
            textname.ImeMode = ImeMode.Hangul;

            combo.Parent = acbox;
            combo.Bounds = new Rectangle(120, 125, 220, 30);
            combo.BackColor = Color.FromArgb(209, 220, 245);

            textmoney.Parent = acbox;
            textmoney.Bounds = new Rectangle(120, 175, 220, 30);
            textmoney.BackColor = Color.FromArgb(209, 220, 245);
            textmoney.TextAlign = HorizontalAlignment.Right;
            textmoney.Click += new EventHandler(Globals.house.inputgroup.moneyclick);
            textmoney.Leave += new EventHandler(Globals.house.inputgroup.moneyleave);
            textmoney.KeyUp += new KeyEventHandler(Globals.house.inputgroup.textkeyup);
            textmoney.KeyDown += new KeyEventHandler(Globals.house.inputgroup.textkeydown);
        }

        public void addcombo()
        {
            combo.Enabled = true;
            combo.Items.Clear();
            for (int i = 0; i < Globals.house.banklist.Count; i++) combo.Items.Add(Globals.house.banklist[i].name);
            combo.SelectedItem = Acnt.account.carddgv[1, Acnt.account.cardrow].Value.ToString();
        }

        public void cleargroup()
        {
            textname.Text = "";
            combo.Text = "";
            textmoney.Text = "";
        }
    }

    public class Acnt
    {
        public static Account account;
    }
}
