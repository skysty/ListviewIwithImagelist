using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Forms;

namespace howto_listview_db_pictures
{
    public static class ListViewExtensions
    {
        // ListView бағанының тақырыптарын жасау.
        // ParamArray жазбалары арасында кезектесу керек
        // жолдар және HorizontalAlignment мәндері.
        public static void MakeColumnHeaders(
            this ListView lvw, params object[] header_info)
        {
            // Бар тақырыптарды жойыңыз..
            lvw.Columns.Clear();

            // Баған тақырыптарын жасау цикл құрамыз.
            for (int i = header_info.GetLowerBound(0);
                     i <= header_info.GetUpperBound(0);
                     i += 3)
            {
                lvw.Columns.Add(
                    (string)header_info[i],
                    (int)header_info[i + 1],
                    (HorizontalAlignment)header_info[i + 2]);
            }
        }

        // ListViewге қатар қосу.
        public static void AddRow(this ListView lvw, string key,
            string item_title, params string[] subitem_titles)
        {
            // / Элемент жасау.
            ListViewItem new_item = lvw.Items.Add(item_title, key);

            // Ішкі элементтерді жасау.
            for (int i = subitem_titles.GetLowerBound(0);
                     i <= subitem_titles.GetUpperBound(0);
                     i++)
            {
                new_item.SubItems.Add(subitem_titles[i]);
            }
        }
    }
}
