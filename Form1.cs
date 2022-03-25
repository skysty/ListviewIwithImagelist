using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

// Жобалау кезінде:
// ImageList ImageSize сипаттарын дұрыс мәндерге орнатыңыз:
//      imlSmallIcons.ImageSize = 32,32
//      imlLargeIcons.ImageSize = 64,64
//   Set the ImageList's ColorDepth properties to the correct values:
//      imlSmallIcons.ColorDepth = Depth32bit
//      imlLargeIcons.ColorDepth = Depth32bit

using System.Data.OleDb;
using System.IO;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

// Add the database to the project and set its
// МАҢЫЗДЫ: Access бағдарламасында немесе оның ішінде дерекқорды ашпаңыз
// барлық кескін деректерін өшіруі мүмкін.

namespace howto_listview_db_pictures
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Бірінші стильді таңдаңыз.
            cboStyle.SelectedIndex = 0;
            // ListView инициализациясы.
            lvwBooks.SmallImageList = imlSmallIcons;
            lvwBooks.LargeImageList = imlLargeIcons;

            // // Баған тақырыптарын .
            lvwBooks.MakeColumnHeaders(
                "Тақырырып", 230, HorizontalAlignment.Left,
                "URL", 220, HorizontalAlignment.Left,
                "ISBN", 130, HorizontalAlignment.Left,
                "Сурет", 230, HorizontalAlignment.Left,
                "Бет саны", 50, HorizontalAlignment.Right,
                "Жыл", 60, HorizontalAlignment.Right);

            //Деррекқор аты.
            string db_name = Application.StartupPath +
                "\\books_with_images.mdb";

            // Дерекқорға қосылу жолы access үшін
            using (OleDbConnection conn =
                new OleDbConnection(
                    "Provider=Microsoft.ACE.OLEDB.12.0;" +
                    "Data Source=" + db_name + ";" +
                    "Mode=Share Deny None"))
            {
                // Кітап туралы ақпарат.
                OleDbCommand cmd = new OleDbCommand(
                    "SELECT Title, URL, ISBN, CoverUrl, " +
                    "Pages, Year, CoverImage FROM Books ORDER BY Year DESC",
                    conn);
                conn.Open();
                using (OleDbDataReader reader = cmd.ExecuteReader())
                {
                    lvwBooks.Items.Clear();
                    imlLargeIcons.Images.Clear();
                    imlSmallIcons.Images.Clear();
                    while (reader.Read())
                    {
                        // Суретті алу үшін.
                        if (!reader.IsDBNull(6))
                        {
                            // Суретті алу.
                            Bitmap bm = BytesToImage((byte[])reader.GetValue(6));
                            float source_aspect = bm.Width / (float)bm.Height;

                            // Үлекен суретті алу.
                            AddImageToImageList(imlLargeIcons,
                                bm, reader[0].ToString(),
                                imlLargeIcons.ImageSize.Width,
                                imlLargeIcons.ImageSize.Height);

                            // Кішкентай суреттерді алу.
                            AddImageToImageList(imlSmallIcons,
                                bm, reader[0].ToString(),
                                imlLargeIcons.ImageSize.Width,
                                imlLargeIcons.ImageSize.Height);
                        }

                        // Алған деректерімізді қосамыз
                        lvwBooks.AddRow(
                            reader[0].ToString(),   // сурет байт кодтары
                            reader[0].ToString(),   // Тақырып 
                            reader[1].ToString(),   // сілтеме
                            reader[2].ToString(),   // ISBN
                            reader[3].ToString(),   // Беткі сілтемесі
                            reader[4].ToString(),   // бет саны
                            reader[5].ToString());  // Жылы
                    }
                }
            }
        }

        // Кескінді ImageList ішіне сыйғызу үшін масштабын қосамыз.
        private void AddImageToImageList(ImageList iml, Bitmap bm,
            string key, float wid, float hgt)
        {
            // bitmap жасаймыз.
            Bitmap iml_bm = new Bitmap(
                iml.ImageSize.Width,
                iml.ImageSize.Height);
            using (Graphics gr = Graphics.FromImage(iml_bm))
            {
                gr.Clear(Color.Transparent);
                gr.InterpolationMode = InterpolationMode.High;

                // Суретті дұрыс масштабтап саламыз.
                RectangleF source_rect = new RectangleF(
                    0, 0, bm.Width, bm.Height);
                RectangleF dest_rect = new RectangleF(
                    0, 0, iml_bm.Width, iml_bm.Height);
                dest_rect = ScaleRect(source_rect, dest_rect);

                // Суретті саламыз.
                gr.DrawImage(bm, dest_rect, source_rect,
                    GraphicsUnit.Pixel);
            }

            //Суретті ImageList ішіне қосамыз.
            iml.Images.Add(key, iml_bm);
        }

        // Байт массивін суретке конвертация жасаймыз.
        private Bitmap BytesToImage(byte[] bytes)
        {
            using (MemoryStream image_stream =
                new MemoryStream(bytes))
            {
                Bitmap bm = new Bitmap(image_stream);
                return bm;
            }
        }

        // Кескінді бұрмаламай масштабтаймыз.
        // Бізге берілген аумақта центрленген тіктөртбұрышты қайтарамыз.
        private RectangleF ScaleRect(
            RectangleF source_rect, RectangleF dest_rect)
        {
            float source_aspect =
                source_rect.Width / source_rect.Height; //түпнұсқа өлшемі
            float wid = dest_rect.Width;//ені
            float hgt = dest_rect.Height;//,Биіктігі
            float dest_aspect = wid / hgt; //экаранға шығаратын өлшем

            if (source_aspect > dest_aspect)
            {
                // Сурет салыстырмалы түрде қысқа және кең болса.
                // Енін түпнұсқа өлшемінен  бөліп біктігін шығарамыз
                hgt = wid / source_aspect;
            }
            else
            {
                // Сурет салыстырмалы түрде биік және жіңішке болса.
                // биіктігін түпнұсқа өлшемінен  бөліп біктігін шығарамыз.
                wid = hgt * source_aspect;
            }

            // Ортаға қоямыз
            float x = dest_rect.Left + (dest_rect.Width - wid) / 2;
            float y = dest_rect.Top + (dest_rect.Height - hgt) / 2;
            return new RectangleF(x, y, wid, hgt);
        }

        // ListView көрсету стилін өзгерту.
        private void cboStyle_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (cboStyle.Text)
            {
                case "Большие иконки":
                    lvwBooks.View = View.LargeIcon;
                    break;
                case "Маленькие значки":
                    lvwBooks.View = View.SmallIcon;
                    break;
                case "Список":
                    lvwBooks.View = View.List;
                    break;
                case "Плитка":
                    lvwBooks.View = View.Tile;
                    break;
                case "Подробности":
                    lvwBooks.View = View.Details;
                    break;
            }
        }
    }
}
