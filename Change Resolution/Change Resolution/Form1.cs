using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Change_Resolution
{
      public partial class Form1 : Form
      {
            public Label[] label = new Label[4];
            public TextBox[] text = new TextBox[2];
            public string font = "굴림", dirpath = Directory.GetCurrentDirectory(), imgfolder = null, savefolder = null, filename;
            public int MAX = 0, check = 0;
            public Size resize;
            public Bitmap bitmap = null;

            public Form1()
            {
                  InitializeComponent();
            }

            private void Form1_Load(object sender, EventArgs e)
            {
                  progressBar1.Style = ProgressBarStyle.Continuous;
                  progressBar1.Minimum = 0;
                  progressBar1.Step = 1;
                  trackBar1.Maximum = 100;
                  trackBar1.Minimum = 0;
                  trackBar1.TickFrequency = 1;
                  trackBar1.LargeChange = 1;
                  trackBar1.SmallChange = 1;
                  trackBar1.Value = 10;
                  label2.Text = trackBar1.Value.ToString();
                  trackBar1.ValueChanged += trackbar_change;
                  make_label();
                  make_text();
            }

            public void trackbar_change(object sender, EventArgs e)
            {
                  check = (trackBar1.Value == 10) ? 0 : 1;
                  if (bitmap != null)
                  {
                        AdjustBrightness(bitmap, (float)trackBar1.Value / 10, 0);
                        label2.Text = ((float)trackBar1.Value).ToString();
                  }
            }


            public void make_label()
            {
                  string[] labeltext = new string[] { "그림 폴더", "저장 폴더", "변경한 그림", "모두 변경" };

                  for (int i = 0; i < 4; i++)
                  {
                        label[i] = new Label();
                        label[i].Text = labeltext[i];
                        label[i].Parent = this;
                        label[i].TextAlign = ContentAlignment.MiddleCenter;
                        label[i].Font = new Font(font, 14, FontStyle.Bold);
                        label[i].Bounds = new Rectangle(180, 35 + 70 * i, 124, 42);

                        label[i].MouseHover += label_hover;
                        label[i].MouseLeave += label_leave;
                        label[i].MouseClick += label_click;

                        label[i].Image = Image.FromFile(dirpath + "\\normal.png");
                  }
            }

            public void make_text()
            {
                  Label[] lb = new Label[2];
                  string[] labeltext = new string[] { "가로 길이", "세로 길이" };

                  for (int i = 0; i < 2; i++)
                  {
                        text[i] = new TextBox();
                        text[i].Parent = this;
                        text[i].TextAlign = HorizontalAlignment.Right;
                        text[i].Font = new Font(font, 14, FontStyle.Bold);
                        text[i].Bounds = new Rectangle(100 + 200 * i, 350, 100, 50);

                        lb[i] = new Label();
                        lb[i].Text = labeltext[i];
                        lb[i].Parent = this;
                        lb[i].TextAlign = ContentAlignment.MiddleCenter;
                        lb[i].Font = new Font(font, 14, FontStyle.Bold);
                        lb[i].Bounds = new Rectangle(100 + 200 * i, 310, 100, 50);
                  }
                  text[0].Text = "0";
                  text[1].Text = "0";
            }

            public void changesize(string fname, int sel)
            {
                  Bitmap img = new Bitmap(fname);
                  int width = img.Width, height = img.Height;
                  int rewidth = Int32.Parse(text[0].Text), reheight = Int32.Parse(text[1].Text);
                  Bitmap result = new Bitmap(width, height);

                  if ((text[0].Text == "0" && text[1].Text == "0") || (text[0].Text.Trim() == width.ToString() && text[1].Text.Trim() == height.ToString()))
                  {
                        text[0].Text = width.ToString();
                        text[1].Text = height.ToString();
                        result = img;
                  }
                  else
                  {
                        resize = new Size(Int32.Parse(text[0].Text), Int32.Parse(text[1].Text));
                        Bitmap reimg = new Bitmap(img, resize);
                        resize = new Size(rewidth, reheight);
                        result = new Bitmap(reimg, resize);
                        reimg.Dispose();
                  }
                  bitmap = new Bitmap(result);

                  if (sel == 0)
                        pictureBox2.Image = result.GetThumbnailImage(width, height, null, IntPtr.Zero);
                  else
                  {
                        Bitmap temp = new Bitmap(result); ;
                        if (check == 1)
                              temp = AdjustBrightness(result, (float)trackBar1.Value / 10, 1);
                        filename = savefolder + "\\" + Path.GetFileNameWithoutExtension(fname) + ".jpg";
                        temp.Save(filename, System.Drawing.Imaging.ImageFormat.Jpeg);
                        temp.Dispose();
                  }
                  img.Dispose();
                  result.Dispose();
            }

            public void Filenumber(string path)
            {
                  try
                  {
                        string[] dirs = Directory.GetDirectories(path);
                        DirectoryInfo di = new DirectoryInfo(path);

                        MAX += di.GetFiles().Length;

                        if (dirs.Length > 0)
                        {
                              foreach (string dir in dirs)
                              {
                                    DirFileSearch(dir);
                              }
                        }
                  }
                  catch (Exception ex)
                  {
                        MessageBox.Show(ex.Message);
                  }
            }

            public void DirFileSearch(string path)
            {
                  try
                  {
                        string[] dirs = Directory.GetDirectories(path);
                        DirectoryInfo di = new DirectoryInfo(path);
                        foreach (FileInfo f in di.GetFiles())
                        {
                              changesize(di.FullName + "\\" + f.Name, 1);
                              progressBar1.PerformStep();
                        }
                        if (dirs.Length > 0)
                        {
                              foreach (string dir in dirs)
                              {
                                    DirFileSearch(dir);
                              }
                        }
                  }
                  catch (Exception ex)
                  {
                        MessageBox.Show(ex.Message);
                  }
            }

            public void findfile(string path)
            {
                  try
                  {
                        string[] dirs = Directory.GetDirectories(path);
                        DirectoryInfo di = new DirectoryInfo(path);
                        foreach (FileInfo f in di.GetFiles())
                        {
                              changesize(di.FullName + "\\" + f.Name, 0);
                              trackBar1.Value = 10;
                              return;
                        }
                        if (dirs.Length > 0)
                        {
                              foreach (string dir in dirs)
                                    DirFileSearch(dir);
                        }
                  }
                  catch (Exception ex)
                  {
                        MessageBox.Show(ex.Message);
                  }
            }

            private void label_hover(object sender, EventArgs e)
            {
                  ((Label)sender).Image = Image.FromFile(dirpath + "\\hover.png");
            }

            private void label_leave(object sender, EventArgs e)
            {
                  ((Label)sender).Image = Image.FromFile(dirpath + "\\normal.png");
            }

            private void label_click(object sender, MouseEventArgs e)
            {
                  ((Label)sender).Image = Image.FromFile(dirpath + "\\click.png");
                  if ((Label)sender == label[0])
                  {
                        FolderBrowserDialog fbd = new FolderBrowserDialog();
                        if (fbd.ShowDialog() == DialogResult.OK)
                              imgfolder = fbd.SelectedPath;
                  }
                  else if ((Label)sender == label[1])
                  {
                        FolderBrowserDialog fbd = new FolderBrowserDialog();
                        if (fbd.ShowDialog() == DialogResult.OK)
                              savefolder = fbd.SelectedPath;
                  }
                  else if ((Label)sender == label[2])
                  {
                        if (imgfolder == null || savefolder == null)
                        {
                              MessageBox.Show("그림폴더와 저장폴더가 먼저 지정되어야 합니다!!");
                              return;
                        }
                        findfile(imgfolder);
                  }
                  else
                  {
                        if (imgfolder == null || savefolder == null)
                        {
                              MessageBox.Show("그림폴더와 저장폴더가 먼저 지정되어야 합니다!!");
                              return;
                        }
                        Filenumber(imgfolder);
                        progressBar1.Maximum = MAX;
                        DirFileSearch(imgfolder);
                  }
            }

            private Bitmap AdjustBrightness(Bitmap image, float brightness, int sel)
            {
                  // Make the ColorMatrix.
                  float b = brightness;
                  ColorMatrix cm = new ColorMatrix(new float[][]
                      {
                    new float[] {b, 0, 0, 0, 0},
                    new float[] {0, b, 0, 0, 0},
                    new float[] {0, 0, b, 0, 0},
                    new float[] {0, 0, 0, 1, 0},
                    new float[] {0, 0, 0, 0, 1},
                      });
                  ImageAttributes attributes = new ImageAttributes();
                  attributes.SetColorMatrix(cm);

                  // Draw the image onto the new bitmap while applying the new ColorMatrix.
                  Point[] points =
                  {
                new Point(0, 0),
                new Point(image.Width, 0),
                new Point(0, image.Height),
            };
                  Rectangle rect = new Rectangle(0, 0, image.Width, image.Height);

                  // Make the result bitmap.
                  Bitmap bm = new Bitmap(image.Width, image.Height);
                  using (Graphics gr = Graphics.FromImage(bm))
                  {
                        gr.DrawImage(image, points, rect, GraphicsUnit.Pixel, attributes);
                  }
                  if (sel == 0)
                        pictureBox2.Image = bm.GetThumbnailImage(bm.Width, bm.Height, null, IntPtr.Zero);
                  return bm;
            }
      }
}
