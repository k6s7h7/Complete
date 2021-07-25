using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace account_book
{
    public partial class Category : Form
    {
        public ComboBox[] combo;
        public RadioButton[] radio;
        public ComboBox selcombo = new ComboBox();
        public TextBox textbox;
        public Label[] btnlabel;
        public Boolean chk = false;
        public Category()
        {
            InitializeComponent();
        }

        private void Category_Load(object sender, EventArgs e)
        {
            this.BackColor = Color.FromArgb(218, 223, 232);
            this.AutoScroll = true;
            makelabel();
            maketextbox();
            makecombo();
            makeradio();
            makebutton();
            this.FormClosing += new FormClosingEventHandler(closeform);
        }

        public void closeform(object sender, FormClosingEventArgs e)
        {
            if (chk) Globals.house.fm.writefile();
            Globals.house.inputgroup.combo[0].Items.Clear();
            Globals.house.inputgroup.combo[0].Items.AddRange(Globals.house.mnglist.pickuparray(Globals.house.oitemlist));
            Globals.house.inputgroup.combo[0].SelectedIndex = 0;
            Globals.house.inputgroup.combo[1].Items.Clear();
            Globals.house.inputgroup.combo[1].Items.AddRange(Globals.house.inputgroup.findsitem(combo[0]));
            if (Globals.house.inputgroup.combo[1].Items.Count > 0) Globals.house.inputgroup.combo[1].SelectedIndex = 0;
            this.Dispose();
        }

        public void makelabel()
        {
            string[] name = new string[] { "큰항목", "작은항목", "선택된 항목" };
            Label[] label = new Label[name.Length];
            int i = 0;
            for (; i < name.Length - 1; i++)
            {
                label[i] = new Label();
                label[i].Parent = this;
                label[i].Text = name[i];
                label[i].TextAlign = ContentAlignment.MiddleCenter;
                label[i].Font = new Font(Statics.font, 14, FontStyle.Bold);
                label[i].Bounds = new Rectangle(50 + 200 * i, 130, 160, 40);
            }
            label[i] = new Label();
            label[i].Parent = this;
            label[i].Text = name[i];
            label[i].TextAlign = ContentAlignment.MiddleCenter;
            label[i].Font = new Font(Statics.font, 14, FontStyle.Bold);
            label[i].Bounds = new Rectangle(50, 50, 142, 32);
            label[i].Image = Image.FromFile(Statics.dirpath + "\\button\\item.png");
        }

        public void makeradio()
        {
            radio = new RadioButton[2];
            for (int i = 0; i < 2; i++)
            {
                radio[i] = new RadioButton();
                radio[i].Parent = this;
                radio[i].Bounds = new Rectangle(130 + 200 * i, 110, 20, 20);
                radio[i].CheckedChanged += changeradio;
            }
            radio[0].Checked = true;
        }

        public void changeradio(object sender, EventArgs e)
        {
            if (radio[0].Checked)
            {
                selcombo = combo[0];
                selcombo.SelectedIndex = 0;
            }
            else
            {
                selcombo = combo[1];
                if(selcombo.Items.Count > 0) selcombo.SelectedIndex = 0;
            }
        }

        public void makecombo()
        {
            combo = new ComboBox[2];
            for (int i = 0; i < combo.Length; i++)
            {
                combo[i] = new ComboBox();
                combo[i].Parent = this;
                combo[i].Items.Clear();
                combo[i].Font = new Font(Statics.font, 14, FontStyle.Bold);
                combo[i].BackColor = Color.FromArgb(209, 220, 245);
                combo[i].Bounds = new Rectangle(50 + 200 * i, 180, 160, 40);
                combo[i].DrawMode = DrawMode.OwnerDrawFixed;
                combo[i].DropDownStyle = ComboBoxStyle.DropDownList;
                combo[i].DrawItem += new DrawItemEventHandler(Globals.house.inputgroup.drawitem);
            }
            combo[0].Items.AddRange(Globals.house.mnglist.pickuparray(Globals.house.oitemlist));
            combo[0].SelectedIndex = 0;
            textbox.Text = combo[0].SelectedItem.ToString();
            combo[0].Select();
            combo[1].Items.AddRange(Globals.house.inputgroup.findsitem(combo[0]));
            if(combo[1].Items.Count > 0) combo[1].SelectedIndex = 0;
            for (int i = 0; i < combo.Length; i++)
            {
                combo[i].SelectedIndexChanged += new System.EventHandler(changeindex);
            }
        }
        public void changeindex(object sender, EventArgs e)
        {
            textbox.BackColor = Color.Gold;
            if ((ComboBox)sender == combo[0])
            {
                combo[1].Items.Clear();
                combo[1].Items.AddRange(Globals.house.inputgroup.findsitem(combo[0]));
                if (combo[1].Items.Count > 0) combo[1].SelectedIndex = 0;
            }
            textbox.Text = ((ComboBox)sender).SelectedItem.ToString();
        }

        public void maketextbox()
        {
            textbox = new TextBox();
            textbox.Parent = this;
            textbox.TextAlign = HorizontalAlignment.Center;
            textbox.Font = new Font(Statics.font, 14, FontStyle.Bold);
            textbox.ImeMode = ImeMode.Hangul;
            textbox.Bounds = new Rectangle(200, 50, 200, 40);
            textbox.BackColor = Color.FromArgb(209, 220, 245);
        }

        public void makebutton()
        {
            string[] btnname = new string[] { "추    가", "수    정", "삭    제", "저    장" };
            btnlabel = new Label[btnname.Length];

            for (int i = 0; i < btnname.Length; i++)
            {
                btnlabel[i] = new Label();
                btnlabel[i].Parent = this;
                btnlabel[i].Text = btnname[i];
                btnlabel[i].TextAlign = ContentAlignment.MiddleCenter;
                btnlabel[i].Font = new Font(Statics.font, 16, FontStyle.Bold);
                if(i < 3)btnlabel[i].Bounds = new Rectangle(20 + 140 * i, 500, 130, 40);
                else btnlabel[i].Bounds = new Rectangle(20 + 140, 560, 130, 40);
                btnlabel[i].Image = Image.FromFile(Statics.dirpath + "\\button\\normal.png");
                btnlabel[i].MouseHover += new EventHandler(Globals.house.savedelhov);
                btnlabel[i].MouseLeave += new EventHandler(Globals.house.savedellve);
            }
            btnlabel[0].Click += new EventHandler(appendclk);
            btnlabel[1].Click += new EventHandler(mendclk);
            btnlabel[2].Click += new EventHandler(deleteclk);
            btnlabel[3].Click += new EventHandler(saveclk);
        }
        
        public void appendclk(object sender, EventArgs e)
        {
            int otindex, stindex;
            if (textbox.Text.Trim() == "")
            {
                MessageBox.Show("선택한 항목이 비어있습니다!!");
                return;
            }
            btnlabel[0].Image = Image.FromFile(Statics.dirpath + "\\button\\click.png");
            if (selcombo == combo[0])
            {
                otindex = Globals.house.oitemlist.FindIndex(x => x.name == textbox.Text);
                if (otindex >= 0)
                {
                    if (Globals.house.oitemlist[otindex].num < 10000)
                    {
                        MessageBox.Show("같은 항목이 있습니다");
                        return;
                    }
                    for (int i = 0; i < Globals.house.oitemlist.Count; i++) if (Globals.house.oitemlist[i].name == textbox.Text) Globals.house.oitemlist[i].name += i.ToString();
                }
                otindex = Globals.house.oitemlist.FindIndex(x => x.name == combo[0].SelectedItem.ToString());
                if (MessageBox.Show("큰항목에 " + textbox.Text + "를 추가합니다 확실하십니까?", "큰항목 추가", MessageBoxButtons.YesNo) == DialogResult.No) return;
                Globals.house.oitemlist.Insert(otindex, new Listdata(Globals.house.oitemlist.Count, textbox.Text));
                Globals.house.intervallist.Insert(otindex, 0);
            }
            else
            {
                stindex = Globals.house.sitemlist.FindIndex(x => x.name == textbox.Text);
                if (stindex >= 0)
                {
                    if (Globals.house.sitemlist[stindex].num < 10000)
                    {
                        MessageBox.Show("같은 항목이 있습니다");
                        return;
                    }
                    for (int i = 0; i < Globals.house.sitemlist.Count; i++) if (Globals.house.sitemlist[i].name == textbox.Text) Globals.house.sitemlist[i].name += i.ToString();
                }
                if (MessageBox.Show("큰항목 " + combo[0].SelectedItem.ToString() + " 작은항목에 " + textbox.Text + "를 추가합니다 확실하십니까?", "작은항목 추가", MessageBoxButtons.YesNo) == DialogResult.No) return;
                otindex = Globals.house.oitemlist.FindIndex(x => x.name == combo[0].SelectedItem.ToString());
                if (combo[1].Items.Count <= 0)
                {
                    stindex = 0;
                    for (int i = 0; i < otindex; i++)
                    {
                        if (Globals.house.intervallist[i] < 10000) stindex += Globals.house.intervallist[i];
                        else stindex += (Globals.house.intervallist[i] - 10000);
                    }
                }
                else stindex = Globals.house.sitemlist.FindIndex(x => x.name == combo[1].SelectedItem.ToString());
                Globals.house.sitemlist.Insert(stindex, new Listdata(Globals.house.sitemlist.Count, textbox.Text));
                otindex = Globals.house.oitemlist.FindIndex(x => x.name == combo[0].SelectedItem.ToString());
                Globals.house.intervallist[otindex]++;
            }
            chk = true;
            selcombo.Items.Clear();
            if (selcombo == combo[1]) selcombo.Items.AddRange(Globals.house.inputgroup.findsitem(combo[0]));
            else selcombo.Items.AddRange(Globals.house.mnglist.pickuparray(Globals.house.oitemlist));
            selcombo.SelectedIndex = selcombo.FindString(textbox.Text); 
        }

        public void mendclk(object sender, EventArgs e)
        {
            if (textbox.Text.Trim() == "")
            {
                MessageBox.Show("선택한 항목이 비어있습니다!!");
                return;
            }
            chk = true;
            btnlabel[1].Image = Image.FromFile(Statics.dirpath + "\\button\\click.png");
            if (selcombo == combo[0])
            {
                if (MessageBox.Show("큰항목 " + combo[0].SelectedItem.ToString() + "를 " + textbox.Text + "로 수정합니다 확실하십니까?", "큰항목 수정", MessageBoxButtons.YesNo) == DialogResult.No) return;
                int index = Globals.house.oitemlist.FindIndex(x => x.name == combo[0].SelectedItem.ToString());
                Globals.house.oitemlist[index].name = textbox.Text;
            }
            else
            {
                if (MessageBox.Show("작은항목 " + combo[1].SelectedItem.ToString() + "를 " + textbox.Text + "로 수정합니다 확실하십니까?", "작은항목 수정", MessageBoxButtons.YesNo) == DialogResult.No) return;
                int index = Globals.house.sitemlist.FindIndex(x => x.name == combo[1].SelectedItem.ToString());
                Globals.house.sitemlist[index].name = textbox.Text;
            }
            chk = true;
            selcombo.Items.Clear();
            if (selcombo == combo[1]) selcombo.Items.AddRange(Globals.house.inputgroup.findsitem(combo[0]));
            else selcombo.Items.AddRange(Globals.house.mnglist.pickuparray(Globals.house.oitemlist));
            selcombo.SelectedIndex = selcombo.FindString(textbox.Text);
        }

        public void deleteclk(object sender, EventArgs e)
        {
            int index;
            if (textbox.Text.Trim() == "")
            {
                MessageBox.Show("선택한 항목이 비어있습니다!!");
                return;
            }
            index = (selcombo == combo[0]) ? Globals.house.oitemlist.FindIndex(x => x.name == textbox.Text.Trim()) : Globals.house.sitemlist.FindIndex(x => x.name == textbox.Text.Trim());
            if (index < 0)
            {
                MessageBox.Show("선택한 항목과 같은 항목이 없습니다!!");
                return;
            }
            
            btnlabel[2].Image = Image.FromFile(Statics.dirpath + "\\button\\click.png");
            if (selcombo == combo[0])
            {
                if (MessageBox.Show("큰항목 " + textbox.Text.Trim() + "를 삭제합니다. 그에 딸려있는 작은항목도 모두 삭제됩니다 확실하십니까?", "큰항목 삭제", MessageBoxButtons.YesNo) == DialogResult.No) return;
                
                index = Globals.house.oitemlist.FindIndex(x => x.name == textbox.Text.Trim());
                Globals.house.oitemlist[index].num += 10000;
                int i = 0, start = 0;
                for (; i < index; i++) start += Globals.house.intervallist[i];
                for (i = 0; i < Globals.house.intervallist[index]; i++) if (Globals.house.sitemlist[start + i].num < 10000) Globals.house.sitemlist[start + i].num += 10000;
                Globals.house.intervallist[index] += 10000;
            }
            else
            {
                if (MessageBox.Show("큰항목 " + combo[0].SelectedItem.ToString() + "작은항목 " + textbox.Text.Trim() + "를 삭제합니다. 확실하십니까?", "작은항목 삭제", MessageBoxButtons.YesNo) == DialogResult.No) return;
                index = Globals.house.sitemlist.FindIndex(x => x.name == textbox.Text.Trim());
                Globals.house.sitemlist[index].num += 10000;
            }
            chk = true;
            selcombo.Items.Clear();
            if (selcombo == combo[1]) selcombo.Items.AddRange(Globals.house.inputgroup.findsitem(combo[0]));
            else selcombo.Items.AddRange(Globals.house.mnglist.pickuparray(Globals.house.oitemlist));
            combo[0].SelectedIndex = 0;
        }

        public void saveclk(object sender, EventArgs e)
        {
            Globals.house.fm.writefile();
            chk = false;
        }
    }
}