using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using System.Data.SqlClient;

namespace Connected_Son_Proje
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        SqlConnection conn = new SqlConnection(Tools.ConnectionString);
        SqlCommand cmd;
        private void btnListele_Click(object sender, EventArgs e)
        {
            if (conn.State==ConnectionState.Closed)
            {
                conn.Open();
            }
            listView1.Items.Clear();
            listele();
            
            if (conn.State==ConnectionState.Open)
            {
                conn.Close();
            }
            
        }

        private void btnEkle_Click(object sender, EventArgs e)
        {
            if (txtUrunAdiEkle.Text != "" && txtUrunFiyatEkle.Text != "" && txtUrunStockEkle.Text != "")
            {
                int donenDeger = 0;
                try
                {
                    if (txtUrunAdiEkle.Text != "")
                    {
                        conn.Open();
                        cmd = new SqlCommand("INSERT INTO Products (ProductName,UnitPrice,UnitsInStock) VALUES (@proName,@price,@stock) SELECT CAST (scope_identity() AS int)", conn);

                        //INSERT INTO Categories (CategoryName) VALUES ('TEST') SELECT @@IDENTITY
                        cmd.Parameters.AddWithValue("@proName", txtUrunAdiEkle.Text);
                        cmd.Parameters.AddWithValue("@price", txtUrunFiyatEkle.Text);
                        cmd.Parameters.AddWithValue("@stock", txtUrunStockEkle.Text);

                        donenDeger = cmd.ExecuteNonQuery();
                        MessageBox.Show(donenDeger != 0 ? "İŞLEM BAŞARILI" : "İŞLEM BAŞARISIZ");
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
                listView1.Items.Clear();
                listele();
                conn.Close();
            }
            else
            {
                MessageBox.Show("EKLEME İÇİN İLGİLİ ALANLARA DEĞER GİRİNİZ.");
            }
            
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                ListViewItem item = listView1.SelectedItems[0];
                txtUrunAdiGuncelle.Text = item.SubItems[0].Text;
                txtUrunFiyatGuncelle.Text = item.SubItems[1].Text;
                txtUrunStockGuncelle.Text = item.SubItems[2].Text;
                label7.Text = item.SubItems[3].Text;
            }
            
        }

        private void btnGuncelle_Click(object sender, EventArgs e)
        {
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }
            if (txtUrunAdiGuncelle.Text != "" && txtUrunFiyatGuncelle.Text != "" && txtUrunStockGuncelle.Text != "")
            {
                //ListViewItem item = listView1.SelectedItems[0];
                cmd = new SqlCommand("UPDATE Products SET ProductName=@proName , UnitPrice=@price , UnitsInStock=@stock WHERE ProductID=@product", conn);
                cmd.Parameters.AddWithValue("@proName", txtUrunAdiGuncelle.Text);
                cmd.Parameters.AddWithValue("@price", txtUrunFiyatGuncelle.Text);
                cmd.Parameters.AddWithValue("@stock", txtUrunStockGuncelle.Text);
                cmd.Parameters.AddWithValue("@product", label7.Text);

                int etkilenen = cmd.ExecuteNonQuery();
                if (etkilenen > 0)
                {
                    listView1.Items.Clear();
                    listele();
                }
                MessageBox.Show(etkilenen != 0 ? "İŞLEM BAŞARILI" : "İŞLEM BAŞARISIZ");
                conn.Close();
            }
            else
            {
                MessageBox.Show("GÜNCELLEME İÇİN İLGİLİ ALANLARA DEĞER GİRİNİZ.");
            }
            

        }

        internal void listele()
        {
            
            cmd = new SqlCommand("SELECT   ProductName , UnitPrice , UnitsInStock ,ProductID  FROM Products", conn);

            SqlDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                ListViewItem item = new ListViewItem(rdr["ProductName"].ToString());
                item.SubItems.Add(rdr["UnitPrice"].ToString());
                item.SubItems.Add(rdr["UnitsInStock"].ToString());
                item.SubItems.Add(rdr["ProductID"].ToString());
                listView1.Items.Add(item);
            }
        }

        private void btnAra_Click(object sender, EventArgs e)
        {
            listView1.Items.Clear();
            cmd = new SqlCommand("SELECT ProductName , UnitPrice , UnitsInStock FROM Products WHERE ProductName like '%" + txtUrunAra.Text + "%'", conn);
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            SqlDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                ListViewItem item = new ListViewItem(rdr["ProductName"].ToString());
                item.SubItems.Add(rdr["UnitPrice"].ToString());
                item.SubItems.Add(rdr["UnitsInStock"].ToString());
                listView1.Items.Add(item);
            }
            conn.Close();
        }

        private void btnSil_Click(object sender, EventArgs e)
        {
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            SqlCommand cmd = new SqlCommand("DELETE FROM Products WHERE ProductID=@product", conn);
            cmd.Parameters.AddWithValue("@product", label7.Text);

            int etkilenen = cmd.ExecuteNonQuery();
            if (etkilenen > 0)
            {
                MessageBox.Show("Silme Başarılı");
                listView1.Items.Clear();
                listele();
            }
            else
            {
                MessageBox.Show("Silme Başarısız");
            }
            txtUrunAdiGuncelle.Text = "";
            txtUrunFiyatGuncelle.Text= "";
            txtUrunStockGuncelle.Text = "";
            conn.Close();
        }
    }
}
