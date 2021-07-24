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

namespace SearchWord
{
    public partial class Main : Form
    {
        public Managerfile mf;
        public Main()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Globals.main = this;
            startup();
            mf = new Managerfile();
            this.FormClosing += new FormClosingEventHandler(closeform);
        }
        
        public void startup()
        {
            Size sizescr = new Size();
            sizescr = Screen.PrimaryScreen.WorkingArea.Size;
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point((sizescr.Width - 780) / 3, (sizescr.Height - 340) / 3);
            this.Size = new Size(780, 340);
            this.BackColor = Color.FromArgb(209, 220, 245);
        }

        public void closeform(object sender, FormClosingEventArgs e)
        {
            mf.writefile();
            this.Dispose();
        }
    }

    public class Findword
    {
        public List<string> listfile = new List<string>();
        public List<string> listext = new List<string>();
        public string findword;
        public Label[] label = new Label[3];
        public Label btn;
        public ComboBox combo;
        public FolderBrowserDialog folderbrowser = new FolderBrowserDialog();
        public TextBox[] text = new TextBox[2];
        
        public Findword()
        {
            makelabel();
            makebutton();
            makecombobox();
            maketextbox();
        }

        private void initial()
        {
            listfile.Clear();
            listext.Clear();
            combo.Items.Clear();
            combo.Text = "";
        }

        public void makelabel()
        {
            String[] name = new String[] { "찾을 단어", "찾을 확장명", "찾은 파일" };
            for (int i = 0; i < 3; i++)
            {
                label[i] = new Label();
                label[i].Parent = Globals.main;
                label[i].Text = name[i]; 
                label[i].Font = new Font(Globals.font, 14, FontStyle.Bold);
                label[i].Bounds = new Rectangle(20, 50 + i * 70, 150, 40);
                label[i].TextAlign = ContentAlignment.MiddleCenter;
                label[i].Image = Image.FromFile(Globals.dirpath + "item.png");
            }
        }

        public void makebutton()
        {
            btn = new Label();
            btn.Parent = Globals.main;
            btn.Font = new Font(Globals.font, 14, FontStyle.Bold);
            btn.Text = "단어 찾기";
            btn.Bounds = new Rectangle(600, 50, 140, 45);
            btn.TextAlign = ContentAlignment.MiddleCenter;
            btn.Image = Image.FromFile(Globals.dirpath + "normal.png");
            btn.MouseHover += new EventHandler(btnhover);
            btn.MouseLeave += new EventHandler(btnleave);
            btn.Click += new EventHandler(btnclick);
        }

        public void btnhover(object sender, EventArgs e)
        {
            btn.Image = Image.FromFile(Globals.dirpath + "hover.png");
        }

        public void btnleave(object sender, EventArgs e)
        {
            btn.Image = Image.FromFile(Globals.dirpath + "normal.png");
        }

        public void btnclick(object sender, EventArgs e)
        {
            btn.MouseLeave -= btnleave;                     // 클릭할때 다른창이 뜨면 마우스가 이동을 해 버튼 이미지 표시가 안되므로 마우스리브 이벤트를 중지하였다가 창이 닫히면 이벤트 다시 실행
            btn.Image = Image.FromFile(Globals.dirpath + "click.png");
            findingword();                                      // 
            btn.MouseLeave += btnleave;
            btn.Image = Image.FromFile(Globals.dirpath + "normal.png");
        }

        public void makecombobox()
        {
            combo = new ComboBox();
            combo.Parent = Globals.main;
            combo.Font = new Font(Globals.font, 14, FontStyle.Bold);
            combo.BackColor = Color.FromArgb(209, 220, 245);
            combo.Bounds = new Rectangle(180, 195, 560, 50);
            combo.DrawMode = DrawMode.OwnerDrawFixed;
            combo.DropDownStyle = ComboBoxStyle.DropDownList;
            combo.DrawItem += new DrawItemEventHandler(drawitem);
            combo.SelectedIndexChanged += selectcountry;
        }

        public void selectcountry(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("Notepad.exe", combo.SelectedItem.ToString());     // 콤보박스에서 선택한 파일 메모장으로 열기
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
        public void maketextbox()
        {
            for (int i = 0; i < 2; i++)
            {
                text[i] = new TextBox();
                text[i].Parent = Globals.main;
                text[i].Bounds = new Rectangle(180, 55 + i * 70, 410, 50);
                text[i].TextAlign = HorizontalAlignment.Left;
                text[i].Font = new Font(Globals.font, 16, FontStyle.Bold);
                text[i].BackColor = Color.Gold;
                text[i].ImeMode = ImeMode.Hangul;
            }
        }

        private void searchingword(string path)
        {
            string[] dirs = Directory.GetDirectories(path);                     // path에 있는 폴더들을 dirs에 넣기
            foreach (string ext in listext)
            {
                string[] files = Directory.GetFiles(path, "*." + ext);           // path에 있는 파일들 중 ext 확장명을 가진 파일들을 files에 넣기
                foreach (string file in files)
                {
                    string temp = File.ReadAllText(file, Encoding.Default);    // file 파일의 내용을 모두 temp에 넣기(Encoding.Default는 한글 읽기 가능)
                    if (temp.Contains(findword)) listfile.Add(file);            // temp에 findword의 단어가 있으면 file을 listfile에 추가
                }
            }
            if (dirs.Length > 0)
            {
                foreach (string dir in dirs)
                {
                    searchingword(dir);                                      // dirs가 있으면 재귀호출 
                }
            }
        }

        public void findingword()
        {
            initial();
            if (string.IsNullOrEmpty(text[0].Text))
            {
                MessageBox.Show("검색어를 입력하지 않았습니다!!");
                return;
            }
            else findword = text[0].Text;
            Globals.startext = text[1].Text;
            if (!string.IsNullOrEmpty(Globals.startext))
            {
                string[] temp = Globals.startext.Split(',');           // startext의 문자열을 ','로 구분하여 temp 배열에 넣기
                foreach (string ext in temp)
                {
                    listext.Add(ext);                                      // ext를 listext에 추가하기
                }
            }
            else
            {
                Globals.startext = "txt";
                listext.Add("txt");
            }
            folderbrowser.SelectedPath = Globals.startdir;             // 폴더 브라우저의 시작 폴더 위치를 startdir로 정하기
            if (folderbrowser.ShowDialog() == DialogResult.OK)               // 폴더 브라우저에서 선택한 폴더가 정상이면
            {
                Globals.startdir = folderbrowser.SelectedPath;         // 폴더 브라우저에서 선택한 폴더를 startdir에 넣기
                searchingword(Globals.startdir);
                foreach (string filename in listfile)
                {
                    combo.Items.Add(filename);
                }
                if (combo.Items.Count > 0)
                {
                    combo.SelectedIndex = combo.Items.Count - 1;
                    combo.Text = combo.SelectedItem.ToString();
                }
                else
                {
                    MessageBox.Show("'" + findword + "' 라는 검색어를 가진 파일은 없습니다!!");
                }
            }
            Globals.main.mf.writefile();
        }
    }

    public class Managerfile
    {
        public string filename = "start.ini";
        public Findword fw = new Findword();

        public Managerfile()
        {
            if (!File.Exists(filename))
            {
                Globals.startdir = "C:\\";
                Globals.startext = "txt";
                writefile();
            }
            readfile();
            Form form = new Form();         // textbox에 한글모드더라도 기본값(포커스 포함)이 영어로 되어있는데 모달을 실행 후에는 기본값(포커스 포함)이 한글로 된다. 모달을 위해 폼 사용 
            form.Show();
            form.Dispose();
            fw.text[0].Select();
        }
        public void readfile()
        {
            StreamReader sr = new StreamReader(filename);
            Globals.startdir = sr.ReadLine();
            Globals.startext = sr.ReadLine();
            sr.Close();
            fw.text[1].Text = Globals.startext;
        }

        public void writefile()
        {
            StreamWriter sw = new StreamWriter(filename);
            sw.WriteLine(Globals.startdir);
            sw.WriteLine(Globals.startext);
            sw.Close();
        }
    }

    public class Globals
    {
        public static Main main;
        public static String font = "휴먼신그래픽", startdir, startext, dirpath = Directory.GetCurrentDirectory() + "\\";

    }
}
