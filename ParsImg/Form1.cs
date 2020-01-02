using AngleSharp.Html.Parser;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections.Generic;
using AngleSharp.Html.Dom;
using System.Net;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace ParsImg
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        public async Task<string> test()
        {
            string source = null;
            HttpClient client = new HttpClient();
            var respose = await client.GetAsync("https://clone-evolution.ru/cloneevo/maxevolves");
            if(respose!=null && respose.StatusCode == System.Net.HttpStatusCode.OK)
            {
                source = await respose.Content.ReadAsStringAsync();                
            }
            return source;
        }
        public async Task<IHtmlDocument> Parse()
        {
            var source = await test();//Получаем HTML код
            var domParser = new HtmlParser();
            var doc = await domParser.ParseDocumentAsync(source);

            var list = new List<string>();
            var items = doc.QuerySelectorAll("img");//Достаём все поля с img           //Достаём ссылки и обрезаем начало
            foreach (var item in items)
            {
                list.Add(item.OuterHtml.Substring(10));
            }

            //var a = from item in list where item.StartsWith("/media/cloneevo/images/clone/clone_") select item;
            //Почему то не работает((

            var list1 = new List<string>();

            //ФФильтруем м отрезаем не нужное в конце
            foreach (var item in list)
            {
                if (item.StartsWith("/media/cloneevo/images/clone/clone_"))
                    list1.Add(item.Substring(0, item.Length - 13));
            }

            string tmp = null;//Ссылка на картинку
            for (int i = 0; i < list1.Count; i++)
            {
                using (WebClient webClient = new WebClient()) //Веб клиент
                {
                    tmp = "https://clone-evolution.ru" + list1[i];//Формирование ссылки
                    byte[] data = webClient.DownloadData(tmp);

                    using (MemoryStream mem = new MemoryStream(data))
                    {                        
                        using (var yourImage = Image.FromStream(mem))
                        {                            
                                yourImage.Save("img/" + i + ".jpg", ImageFormat.Jpeg);///Сохранение картинки
                        }
                    }
                }
            }
            MessageBox.Show("All Ok");
            return doc;
        }
        
            private void button1_Click(object sender, EventArgs e)
        {
            Parse(); 
        }
    }
}
