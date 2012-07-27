using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml;

namespace SRP
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            this.getProductsFileNameFromUser();
        }

        private void getProductsFileNameFromUser()
        {
            this.openFileDialog1.Filter = "XML Document (*.xml)|*.xml|All Files (*.*)|*.*";
            var result = this.openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                this.txtFileName.Text = this.openFileDialog1.FileName;
                this.btnLoad.Enabled = true;
            }
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            this.loadProductsFromFileName();
        }

        private void loadProductsFromFileName()
        {
            this.listView1.Items.Clear();
            var fileName = this.txtFileName.Text;
            var productReader = new ProductXMLReader(fileName);
            var products = productReader.ReadProducts();
            this.AddProductsToListView(products);
        }

        private void AddProductsToListView(IEnumerable<string[]> products)
        {
            foreach (var product in products)
            {
                var listViewItem = new ListViewItem(product);
                this.listView1.Items.Add(listViewItem);
            }
        }
    }

    public class ProductXMLReader
    {
        private readonly string fileName;

        private readonly IList<string[]> products = new List<string[]>();

        public ProductXMLReader(string fileName)
        {
            this.fileName = fileName;
        }

        public IEnumerable<string[]> ReadProducts()
        {
            using (var fs = new FileStream(fileName, FileMode.Open))
            {
                var reader = XmlReader.Create(fs);
                while (reader.Read())
                {
                    readProductIfValidNode(reader);
                }
            }
            return products;
        }

        private void readProductIfValidNode(XmlReader reader)
        {
            if (reader.Name != "product")
            {
                return;
            }
            var productArray = readProductFromNodeAttributes(reader);
            products.Add(productArray);
        }

        private string[] readProductFromNodeAttributes(XmlReader reader)
        {
            var id = reader.GetAttribute("id");
            var name = reader.GetAttribute("name");
            var unitPrice = reader.GetAttribute("unitPrice");
            var discontinued = reader.GetAttribute("discontinued");
            return new[] { id, name, unitPrice, discontinued };
        }
    }
}
