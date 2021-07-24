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

namespace Calculator
{
     
    public partial class Form1 : Form
    {
        public string curpath = System.IO.Directory.GetCurrentDirectory();
        public string[] keyvalue = new string[16] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", ".", "←", "＋", "－", "×", "÷" };
        public static int prevalue, selvalue, symbol = 1, keynum, dif;
        public static double postval = 0;
        public int locationx = 600, locationy = 300;
        public bool flag = false, kflag = false;
        public Filemanager flmng;
        public Strchange strcng;
        public Input input;
        
        public Form1()
        {
            InitializeComponent();
        }

        public void Form1_Load(object sender, EventArgs e)
        {
            Global.form = this;
            flmng = new Filemanager();
            flmng.filemain();
            formsize();
            strcng = new Strchange();
            input = new Input();
            textBox1.Focus();
        }

        public void formsize()
        {
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(locationx, locationy);
            this.Size = new Size(430, 620);
        }

        public Point mousePoint;
        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            mousePoint = new Point(e.X, e.Y);
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
            {
                locationx = this.Left - (mousePoint.X - e.X);
                locationy = this.Top - (mousePoint.Y - e.Y);
                Location = new Point(locationx, locationy);
                flmng.creatinifile();
            }
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (textBox1.Text.Length > 18) return;
            switch (e.KeyChar)
            {
                case (char)8:
                    selvalue = 11;
                    break;
                case (char)42:
                    selvalue = 14;
                    break;
                case (char)43:
                    selvalue = 12;
                    break;
                case (char)45:
                    selvalue = 13;
                    break;
                case (char)46:
                    if (textBox1.Text.Length > 17) return;
                    selvalue = 10;
                    break;
                case (char)47:
                    selvalue = 15;
                    break;
                case (char)48:
                    if (textBox1.Text.Length > 17) return;
                    selvalue = 0;
                    break;
                case (char)49:
                    if (textBox1.Text.Length > 17) return;
                    selvalue = 1;
                    break;
                case (char)50:
                    if (textBox1.Text.Length > 17) return;
                    selvalue = 2;
                    break;
                case (char)51:
                    if (textBox1.Text.Length > 17) return;
                    selvalue = 3;
                    break;
                case (char)52:
                    if (textBox1.Text.Length > 17) return;
                    selvalue = 4;
                    break;
                case (char)53:
                    if (textBox1.Text.Length > 17) return;
                    selvalue = 5;
                    break;
                case (char)54:
                    if (textBox1.Text.Length > 17) return;
                    selvalue = 6;
                    break;
                case (char)55:
                    if (textBox1.Text.Length > 17) return;
                    selvalue = 7;
                    break;
                case (char)56:
                    if (textBox1.Text.Length > 17) return;
                    selvalue = 8;
                    break;
                case (char)57:
                    if (textBox1.Text.Length > 17) return;
                    selvalue = 9;
                    break;
                case (char)13:
                case (char)61:
                    label6_Click(sender, e);
                    return;
                default: break;
            }
            input.labelclick(sender, e);
        }
        
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox1.Text)) return;
            string str = strcng.makecomma(textBox1.Text, false);
            if (str != null) textBox1.Text = str;
            else textBox1.Clear();
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (string.IsNullOrEmpty(textBox1.Text)) return;
            if (e.KeyCode == Keys.Delete) input.curposition(1);
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox2.Text)) return;
            string str = strcng.makecomma(textBox2.Text, false);
            if (str != null) textBox2.Text = str;
            else textBox2.Clear();
        }

        private void label4_MouseHover(object sender, EventArgs e)
        {
            label4.Image = System.Drawing.Bitmap.FromFile(curpath + @"\keyhov.png");
        }

        private void label4_MouseLeave(object sender, EventArgs e)
        {
            label4.Image = System.Drawing.Bitmap.FromFile(curpath + @"\keynor.png");
        }

        private void label4_Click(object sender, EventArgs e)
        {
            input.restorekey();
            label4.Image = System.Drawing.Bitmap.FromFile(curpath + @"\keyclk.png");
            keynum = 1;
            if (string.IsNullOrEmpty(textBox1.Text)) return;
            input.curposition(1);
        }

        private void label5_MouseHover(object sender, EventArgs e)
        {
            label5.Image = System.Drawing.Bitmap.FromFile(curpath + @"\keyhov.png");
        }

        private void label5_MouseLeave(object sender, EventArgs e)
        {
            label5.Image = System.Drawing.Bitmap.FromFile(curpath + @"\keynor.png");
        }

        private void label5_Click(object sender, EventArgs e)
        {
            input.restorekey();
            label5.Image = System.Drawing.Bitmap.FromFile(curpath + @"\keyclk.png");
            keynum = 2;
            textBox1.Clear();
            textBox2.Clear();
            postval = 0;
            selvalue = 0;
            symbol = 1;
        }

        private void label6_MouseHover(object sender, EventArgs e)
        {
            label6.Image = System.Drawing.Bitmap.FromFile(curpath + @"\key1hov.png");
        }

        private void label6_MouseLeave(object sender, EventArgs e)
        {
            label6.Image = System.Drawing.Bitmap.FromFile(curpath + @"\key1nor.png");
        }

        public void label6_Click(object sender, EventArgs e)
        {
            input.restorekey();
            label6.Image = System.Drawing.Bitmap.FromFile(curpath + @"\key1clk.png");
            keynum = 3;
            if (string.IsNullOrEmpty(textBox1.Text)) return;
            input.calcurator(strcng.strtodig(textBox1.Text));
            textBox2.Text = postval.ToString();
            textBox1.Clear();
        }

        private void label7_MouseHover(object sender, EventArgs e)
        {
            label7.Image = System.Drawing.Bitmap.FromFile(curpath + @"\endhov.png");
        }

        private void label7_MouseLeave(object sender, EventArgs e)
        {
            label7.Image = System.Drawing.Bitmap.FromFile(curpath + @"\endnor.png");
        }

        private void label7_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
    }

    public class Input : Form1
    {
        static int KEYMAX = 16, POINTX = 60, POINTY = 190;
        Label[] labelkey = new Label[KEYMAX];
        
        public Input()
        {
            for (int i = 0; i < KEYMAX; i++)
            {
                labelkey[i] = new Label();
                labelkey[i].Name = keyvalue[i].ToString();
                labelkey[i].Text = keyvalue[i].ToString();
                labelkey[i].Parent = Global.form;
                labelkey[i].TextAlign = ContentAlignment.MiddleCenter;             // 컨트롤 Text 위치 지정
                labelkey[i].BackColor = Color.LightGray;                // 컨트롤 배경색상 RGB로 지정하기
                labelkey[i].Font = new Font("휴먼신그래픽", 24, FontStyle.Bold);
                labelkey[i].Location = new Point(POINTX + (i % 4) * 85, POINTY + (i / 4) * 85);
                labelkey[i].Size = new Size(70, 70);
                labelkey[i].MouseHover += new EventHandler(labelhover);
                labelkey[i].MouseLeave += new EventHandler(labelleave);
                labelkey[i].Click += new EventHandler(labelclick);
                labelkey[i].Image = System.Drawing.Bitmap.FromFile(curpath + @"\keynor.png");
            }
        }

        public void labelhover(object sender, EventArgs e)
        {
            string ctrlName = ((Label)sender).Name;
            for (selvalue = 0; selvalue < KEYMAX; selvalue++) if (string.Compare(ctrlName, keyvalue[selvalue].ToString()) == 0) break;
            labelkey[selvalue].Image = System.Drawing.Bitmap.FromFile(curpath + @"\keyhov.png");
        }

        public void labelleave(object sender, EventArgs e)
        {
            labelkey[selvalue].Image = System.Drawing.Bitmap.FromFile(curpath + @"\keynor.png");
        }

        public void labelclick(object sender, EventArgs e)
        {
            restorekey();
            labelkey[selvalue].Image = System.Drawing.Bitmap.FromFile(curpath + @"\keyclk.png");
            prevalue = selvalue;
            if (string.IsNullOrEmpty(Global.form.textBox1.Text) && string.IsNullOrEmpty(Global.form.textBox2.Text))
            {
                if (selvalue < 11) curposition(0);
                else if (selvalue > 11)
                {
                    Global.form.label3.Text = keyvalue[selvalue];
                    symbol = selvalue - 11;
                }
            }
            else if (selvalue == 11)
            {
                if (!string.IsNullOrEmpty(Global.form.textBox1.Text)) curposition(2);
            }
            else
            {
                if (selvalue < 11) curposition(0);
                else
                {
                    Global.form.label3.Text = keyvalue[selvalue];
                    if (!string.IsNullOrEmpty(Global.form.textBox1.Text))
                    {
                        calcurator(Global.form.strcng.strtodig(Global.form.textBox1.Text));
                        Global.form.textBox2.Text = postval.ToString();
                        Global.form.textBox1.Clear();
                    }
                    symbol = selvalue - 11;
                }
            }
        }
             
        public void curposition(int sel)
        {
            bool chk = false;
            Invalidate();

            int selstart = Global.form.textBox1.SelectionStart;
            int prelen = Global.form.textBox1.TextLength;
            if (sel == 0)
            {
                Global.form.textBox1.Text = Global.form.textBox1.Text.Insert(selstart, keyvalue[selvalue]);
                dif = Global.form.textBox1.TextLength - prelen;
            }
            else
            {
                if ((sel == 2 && selstart < 1) || (sel == 1 && selstart > prelen - 1)) return;
                Global.form.strcng.delchar(sel - 1, selstart, prelen, ref chk);
                if(sel == 1)
                {
                    if (prelen - Global.form.textBox1.TextLength == 1) dif = 1;
                    else dif = 0;
                    if (!chk) dif--;
                }
                else dif = Global.form.textBox1.TextLength - prelen;
            }
            if (selstart + dif < 0) Global.form.textBox1.SelectionStart = 0;
            else Global.form.textBox1.SelectionStart = selstart + dif;
        }
        
        public void calcurator(string str)
        {
            if (string.IsNullOrEmpty(Global.form.textBox2.Text) && selvalue > 11)
            {
                if (symbol == 2) postval = Double.Parse(str) * -1;
                else postval = Double.Parse(str);
            }
            else switch (symbol)
            {
                case 1:
                    postval += double.Parse(str);
                    break;
                case 2:
                    postval -= double.Parse(str);
                    break;
                case 3:
                    postval *= double.Parse(str);
                    break;
                case 4:
                    postval /= double.Parse(str);
                    break;
                default: break;
            }
        }

        public void restorekey()
        {
            switch (keynum)
            {
                case 1:
                    Global.form.label4.Image = System.Drawing.Bitmap.FromFile(curpath + @"\keynor.png");
                    break;
                case 2:
                    Global.form.label5.Image = System.Drawing.Bitmap.FromFile(curpath + @"\keynor.png");
                    break;
                case 3:
                    Global.form.label6.Image = System.Drawing.Bitmap.FromFile(curpath + @"\key1nor.png");
                    break;
                case 0:
                    labelkey[prevalue].Image = System.Drawing.Bitmap.FromFile(curpath + @"\keynor.png");
                    break;
                default: break;
            }
            keynum = 0;
        }

    }

    public class Strchange : Form1
    {
        public void delchar(int sel, int cur, int len, ref bool chk)        // backspace(sel=1) 와 delete(sel=0) 키 눌렀을 때 문자 삭제
        {
            char[] temp = new char[len];
            temp = Global.form.textBox1.Text.ToCharArray();
            if (sel == 1) delback(temp, cur, len--);
            else deldel(temp, cur, len--, ref chk);
            Array.Resize(ref temp, len);
            Global.form.textBox1.Text = new String(temp);
        }

        private void deldel(char[] temp, int cur, int len, ref bool chk) 
        {
            if (temp[cur] == ',')
            {
                cur++;
                chk = true;
            }
            for (; cur < len - 1; cur++) temp[cur] = temp[cur+1];
        }

        private void delback(char[] temp, int cur, int len)
        {
            if (temp[cur - 1] == ',') cur -= 2;
            else cur--;
            for (; cur < len -1; cur++) temp[cur] = temp[cur + 1];
        }
    
        public string strtodig(string str)
        {
            int len, i = 0, j = 0;
            bool chkpoint = false, chkzero = true;
            char[] value = new char[22];
            char[] temp = new char[22];

            value = str.ToCharArray();
            len = str.Length;
            if (len < 2) {
                if ((value[0] > 47 && value[0] < 58) || value[0] == 45 || value[0] == 46) return str;
                else return null;
            }
            if (value[0] == '-')
            {
                temp[j++] = '-';
                i++;
            }
            if (value[i] == '.' || (len > 1 && value[i] == '0' && value[i + 1] == '.'))
            {
                temp[j++] = '0';
                temp[j++] = '.';
                chkpoint = true;
                chkzero = false;
                i++;
                if (value[0] == '0') i++;
            }
            if (chkzero && value[i] == '0')
            {
                if (len < 2)
                {
                    temp[j++] = '0';
                    i++;
                }
                else
                {
                    if (value[i + 1] == '0') return null;
                    else
                    {
                        i++;
                        temp[j++] = value[i++];
                    }
                }
            }
            for (; i < len; i++)
            {
                if (!chkpoint && value[i] == '.')
                {
                    chkpoint = true;
                    temp[j++] = value[i];
                }
                else if (char.IsDigit(value[i])) temp[j++] = value[i];
            }
            Array.Resize(ref temp, j);
            return new String(temp);
        }

        public string makecomma(string str, bool sel)
        {
            int len, i, j = 0, k;
            char[] val = new char[21];
            char[] temp = new char[21];

            string sval = strtodig(str);
            if (string.IsNullOrEmpty(sval)) return null;
            len = sval.Length;
            if (len < 4) return sval;
            val = sval.ToCharArray();
            for (k = 0; k < len; k++) if (val[k] == '.') break;
            for (i = len - 1; i >= k; i--) temp[j++] = val[i];
            while (i >= 0)
            {
                for (k = 0; k < 3; k++)
                {
                    if (i < 0) break;
                    temp[j++] = val[i--];
                }
                if (i >= 0 && val[i] != '-') temp[j++] = ',';
            }
            k = j - 1;
            Array.Resize(ref val, j);
            for (i = 0; k >= 0; i++, k--) val[i] = temp[k];
            return new String(val);
        }
    }

    public class Filemanager : Form1
    {
        public void filemain()
        {
            FileInfo filestm = new FileInfo(curpath + @"\set.ini");
            if (!filestm.Exists)
            {
                FileStream fs = filestm.Create();
                fs.Close();
                creatinifile();
            }
            readinifile();
        }
        public void creatinifile()
        {
            StreamWriter sw = new StreamWriter(curpath + @"\set.ini");
            sw.Write(Global.form.locationx);
            sw.Write("\r\n");
            sw.Write(Global.form.locationy);
            sw.Close();
        }
        public void readinifile()          // 항목 파일(텍스트)을 읽는다  
        {
            string line = " ";

            StreamReader sr = new StreamReader(curpath + @"\set.ini");
            line = sr.ReadLine();
            Global.form.locationx = Int32.Parse(line);
            line = sr.ReadLine();
            Global.form.locationy = Int32.Parse(line);
            sr.Close();
        }
    }
    public class Global            // 폼에 있는 메서드를 다른 클래스에서 사용하기 위해
    {
        public static Form1 form;
    }
}
