using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.IO;

namespace StockTrading
{
    public partial class Infobox : Form
    {
        private Label[] labels;
        private TextBox[] textBox;
        private ComboBox combo;
        private Label[] btn = new Label[2];
        private ToolTip[] toolTip;
        public Crypt[] crypt = new Crypt[4];
        public int MAX;
        public string key = "1a2b3c4d5e";

        public Infobox()
        {
            InitializeComponent();
        }

        private void Infobox_Load(object sender, EventArgs e)
        {
            makelabels();
            makecombo();
            maketextbox();
            makebutton();
            filemain();
            readvalue();
        }

        public void makelabels()
        {
            string[] name = new string[] {"ID", "로그인 비밀번호", "공인인증서 비밀번호", "계좌 비밀번호", "투자 종류", "매수종목 갯수",
                        "매수조건 최소 거래량", "매수조건 최소등락율",  "매수조건 최대등락율", "매수전 등락율 한계점", "원하는 수익율", "원하는 손절율", "매수 시작시간", "매매 종료시간"};

            MAX = name.Length;
            labels = new Label[MAX];

            for (int i = 0; i < MAX; i++)
            {
                labels[i] = new Label();
                labels[i].Parent = this;
                labels[i].Text = name[i];
                labels[i].Bounds = new Rectangle(30, 20 + i * 44, 200, 28);
                labels[i].BackColor = Color.FromArgb(240, 248, 255);
                labels[i].Font = new Font(Statics.글꼴, 12, FontStyle.Bold);
                labels[i].TextAlign = ContentAlignment.MiddleCenter;
                labels[i].Image = Image.FromFile(Statics.dirpath + "\\button\\item.png");
            }
        }

        public void makecombo()
        {
            string[] name = new string[] { "실투자", "모의투자" };

            combo = new ComboBox();
            combo.Parent = this;
            combo.Bounds = new Rectangle(300, 196, 198, 30);
            combo.Font = new Font(Statics.글꼴, 14, FontStyle.Bold);
            combo.BackColor = Color.FromArgb(240, 248, 255);
            combo.DrawMode = DrawMode.OwnerDrawFixed;
            combo.DropDownStyle = ComboBoxStyle.DropDownList;
            combo.DrawItem += new DrawItemEventHandler(Main.stock.drawitem);
            combo.Items.AddRange(name);
            combo.SelectedIndex = 0;
            combo.SelectedIndexChanged += selcombo;
        }

        private void selcombo(object sender, EventArgs e)
        {
            if (combo.SelectedIndex == 0)
                Statics.secretdata.url = "hts.ebestsec.co.kr";
            else
                Statics.secretdata.url = "demo.ebestsec.co.kr";
        }

        public void maketextbox()
        {
            textBox = new TextBox[MAX];
            toolTip = new ToolTip[MAX - 5];

            for (int i = 0; i < MAX; i++)
            {
                if (i == 4) continue;
                textBox[i] = new TextBox();
                if (i > 4)
                {
                    toolTip[i - 5] = new ToolTip();
                    toolTip[i - 5].InitialDelay = 0;
                    toolTip[i - 5].AutoPopDelay = 10000;
                }
                else
                {
                    textBox[i].PasswordChar = '*';
                }
                textBox[i].Parent = this;
                textBox[i].Multiline = true;
                textBox[i].Bounds = new Rectangle(300, 20 + i * 44, 200, 30);
                textBox[i].BackColor = Color.FromArgb(240, 248, 255);
                textBox[i].Font = new Font(Statics.글꼴, 14, FontStyle.Bold);
                textBox[i].TextAlign = HorizontalAlignment.Right;

                textBox[i].Click += new EventHandler(moneyclick);
                textBox[i].Leave += new EventHandler(moneyleave);
                textBox[i].Enter += new EventHandler(moneyfocus);
                if (i > 4) textBox[i].MouseHover += mousetooltip;
            }
        }

        public void moneyclick(object sender, EventArgs e)
        {
            ((TextBox)sender).BackColor = Color.Gold;
        }


        public void moneyleave(object sender, EventArgs e)
        {
            ((TextBox)sender).BackColor = Color.FromArgb(240, 248, 255);
        }

        public void moneyfocus(object sender, EventArgs e)
        {
            ((TextBox)sender).BackColor = Color.Gold;
        }

        string[] texttip = new string[] {"매수하기 위한 종목의 갯수", "매수할 시점에 최소한의 거래량", "매수할 시점에 최소 등락율(초과율 부터 매수)(부동소수점)", "매수할 시점에 최대 등락율(미만율 까지만 매수)(부동소수점)" ,
                  "매수하기 전에 현재 등락율 보다 더 높은 등락율이 있으면 매수 금지하는 한계 등락율(부동소수점)", " 매수할 때 등락율 보다 얼마나 더높은 수익을 낼지 결정할 수익율(부동소수점)",
                  "매수한 등락율 보다 얼마나 더 낮은 등락율이면 손절할 지 결정하는 등락율(부동소수점)", "매수하기 위한 시간(예 => 09:10:00 이면 91000, 10:00:00 이면 100000)", "종료할 시간(현재 매도 안된 종목들 전부 하한가로 매도}"};
        private void mousetooltip(object sender, EventArgs e)
        {
            int i = 0;
            for (; i < MAX; i++)
                if ((TextBox)sender == textBox[i]) break;
            toolTip[i - 5].IsBalloon = true;
            toolTip[i - 5].SetToolTip(textBox[i], texttip[i - 5]);

        }

        public void makebutton()
        {
            string[] name = { "저      장", "종      료" };
            for (int i = 0; i < 2; i++)
            {
                btn[i] = new Label();
                btn[i].Parent = this;
                btn[i].Text = name[i];
                btn[i].Bounds = new Rectangle(70 + i * 270, 640, 120, 38);
                btn[i].Font = new Font(Statics.글꼴, 16, FontStyle.Bold);
                btn[i].TextAlign = ContentAlignment.MiddleCenter;
                btn[i].Image = Image.FromFile(Statics.dirpath + "\\button\\normal.png");
                btn[i].MouseHover += new EventHandler(Main.stock.starthov);
                btn[i].MouseLeave += new EventHandler(Main.stock.startlve);
                btn[i].Click += new EventHandler(buttonclk);
            }
        }

        private void buttonclk(object sender, EventArgs e)
        {
            ((Label)sender).Image = Image.FromFile(Statics.dirpath + "\\button\\click.png");
            if((Label)sender == btn[0])
            {
                string temp = null;

                StreamWriter sw = new StreamWriter("secret.dat");

                for (int i = 0; i < 4; i++)
                {
                    temp = crypt[i].encrypt(textBox[i].Text.Trim(), key);
                    sw.WriteLine(temp);
                }
                sw.Close();

                sw = new StreamWriter("secret.ini");
                for (int i = 0; i < MAX - 1; i++)
                {
                    if (i == 0)
                    {
                        sw.Write("거래");
                        sw.Write(",");
                    }
                    if (i < 4) continue;
                    if (i == 4)
                    {
                        if (combo.SelectedIndex == 0)
                            sw.Write("hts.ebestsec.co.kr");
                        else
                            sw.Write("demo.ebestsec.co.kr");
                        sw.Write(",");
                    }
                    else
                    {
                        sw.Write(textBox[i].Text.Trim());
                        sw.Write(",");
                    }
                }
                sw.Write(textBox[MAX - 1].Text.Trim());
                sw.Close();
                readsecret();
            }
            this.Hide();
        }

        private void readvalue()
        {
            textBox[0].Text = Statics.secretdata.id;
            textBox[1].Text = Statics.secretdata.logpass;
            textBox[2].Text = Statics.secretdata.activepass;
            textBox[3].Text = Statics.secretdata.password;
            textBox[5].Text = Statics.secretdata.max.ToString();
            textBox[6].Text = Statics.secretdata.volume.ToString();
            textBox[7].Text = Statics.secretdata.sratio.ToString();
            textBox[8].Text = Statics.secretdata.eratio.ToString();
            textBox[9].Text = Statics.secretdata.limit.ToString();
            textBox[10].Text = Statics.secretdata.yield.ToString();
            textBox[11].Text = Statics.secretdata.loss.ToString();
            textBox[12].Text = Statics.secretdata.starttime.ToString();
            textBox[13].Text = Statics.secretdata.endtime.ToString();
            combo.SelectedIndex = (Statics.secretdata.url == "hts.ebestsec.co.kr") ? 0 : 1;
        }

        // ID, 비번등 기본자료 읽어오기

        public void filemain()
        {
            FileInfo fileInfo = new FileInfo("secret.ini");

            if (!fileInfo.Exists)
            {
                StreamWriter sw = new StreamWriter("secret.ini");
                sw.Write("거래,demo.ebestsec.co.kr,3,0,4.5,5.5,6.0,3.0,2.0,91000,150000");
                sw.Close();
            }
            fileInfo = new FileInfo("secret.dat");
            if (!fileInfo.Exists)
            {
                StreamWriter sw = new StreamWriter("secret.dat");
                sw.Write("");
                sw.Close();
                Main.stock.makeinfo();
            }
            readsecret();
        }

        public void readsecret()
        {
            for (int i = 0; i < 4; i++)
                crypt[i] = new Crypt();
            string key = "1a2b3c4d5e";

            string[] value = File.ReadAllLines("secret.ini"), temp, pw = new string[] { "", "", "", "" };
            for (int i = 0; i < value.Length; i++)
            {
                temp = value[i].Split(',');
                if (temp[0].Trim() == "거래")                 // temp[0]에 "거래"라고 써있는 것 만 읽어 온다
                {
                    StreamReader sr = new StreamReader("secret.dat");
                    string pass = "";
                    for (int j = 0; j < 4; j++)
                    {
                        if (!String.IsNullOrEmpty(pass = sr.ReadLine()))
                            pw[j] = crypt[i].decrypt(pass, key);
                    }
                    sr.Close();
                    Statics.secretdata =new Secret(pw[0], pw[1], pw[2], pw[3], temp[1], Int32.Parse(temp[2]), Int32.Parse(temp[3]), float.Parse(temp[4]),
                          float.Parse(temp[5]), float.Parse(temp[6]), float.Parse(temp[7]), float.Parse(temp[8]), Int32.Parse(temp[9]), Int32.Parse(temp[10]));
                }
            }
        }
    }

    public class Crypt
    {
        public String encrypt(String text, String password)
        {
            RijndaelManaged RijndaelCipher = new RijndaelManaged();

            byte[] plainText = Encoding.UTF8.GetBytes(text);
            byte[] salt = Encoding.UTF8.GetBytes(password);

            PasswordDeriveBytes secretKey = new PasswordDeriveBytes(password, salt);
            ICryptoTransform Encryptor = RijndaelCipher.CreateEncryptor(secretKey.GetBytes(32), secretKey.GetBytes(16));

            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(memoryStream, Encryptor, CryptoStreamMode.Write);
            cryptoStream.Write(plainText, 0, plainText.Length);     // 암호화 시작
            cryptoStream.FlushFinalBlock();

            byte[] cryptBytes = memoryStream.ToArray();

            // 스트림을 닫습니다.
            memoryStream.Close();
            cryptoStream.Close();

            String cryptResult = Convert.ToBase64String(cryptBytes);
            return cryptResult;     // 암호화 문자열 리턴.
        }

        public String decrypt(String text, String password)
        {
            RijndaelManaged RijndaelCipher = new RijndaelManaged();

            byte[] encryptData = Convert.FromBase64String(text);
            byte[] salt = Encoding.UTF8.GetBytes(password);

            PasswordDeriveBytes SecretKey = new PasswordDeriveBytes(password, salt);

            ICryptoTransform Decryptor = RijndaelCipher.CreateDecryptor(SecretKey.GetBytes(32), SecretKey.GetBytes(16));

            MemoryStream memoryStream = new MemoryStream(encryptData);

            CryptoStream cryptoStream = new CryptoStream(memoryStream, Decryptor, CryptoStreamMode.Read);

            byte[] PlainText = new byte[encryptData.Length];

            // 복호화 시작
            int DecryptedCount = cryptoStream.Read(PlainText, 0, PlainText.Length);

            memoryStream.Close();
            cryptoStream.Close();

            // 복호화된 데이터를 문자열로 바꾼다.
            string DecryptedData = Encoding.UTF8.GetString(PlainText, 0, DecryptedCount);

            // 최종 결과 리턴
            return DecryptedData;
        }
    }
}
