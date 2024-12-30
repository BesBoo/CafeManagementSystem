using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;

namespace CafeManagement.Model
{
    public partial class frmSet : Form
    {
        public frmSet()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Application.Exit();

        }

        private void cbMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedMode = cbMode.SelectedItem.ToString();
            ThemeManager.SetTheme(selectedMode);
            foreach (Form openForm in Application.OpenForms)
            {
                MainClass.ApplyTheme(openForm, selectedMode);
            }
        }

        private void frmSet_Load(object sender, EventArgs e)
        {
            cbMode.SelectedItem = ThemeManager.CurrentTheme;

            
            cbLanguage.Items.AddRange(new string[] { "English", "Vietnamese", "French", "Spanish", "Japanese" });
            //cbLanguage.SelectedIndex = 0;
        }
        private async Task TranslateFormControls(Control control, string languageCode)
        {
            foreach (Control childControl in control.Controls)
            {
                if (childControl is TextBox || childControl is Label || childControl is Button)
                {
                    string translatedText = await TranslateText(childControl.Text, languageCode);
                    childControl.Text = translatedText;
                }
                else if (childControl is DataGridView dgv)
                {
                    foreach (DataGridViewColumn column in dgv.Columns)
                    {
                        string translatedHeader = await TranslateText(column.HeaderText, languageCode);
                        column.HeaderText = translatedHeader;
                    }
                }

                if (childControl.HasChildren)
                {
                    await TranslateFormControls(childControl, languageCode);
                }
            }
        }

        private async void cbLanguage_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedLanguage = cbLanguage.SelectedItem.ToString();
            string languageCode = GetLanguageCode(selectedLanguage);

            if (!string.IsNullOrEmpty(languageCode))
            {
                try
                {
                    foreach (Form openForm in Application.OpenForms)
                    {
                        await TranslateFormControls(openForm, languageCode);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Translation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private string GetLanguageCode(string language)
        {
            var languageMapping = new Dictionary<string, string>
            {
                { "English", "en" },
                { "Vietnamese", "vi" },
                { "French", "fr" },
                { "Spanish", "es" },
                { "Japanese", "ja" }
            };

            return languageMapping.ContainsKey(language) ? languageMapping[language] : string.Empty;
        }
        

        private async Task<string> TranslateText(string inputText, string languageCode)
        {
            if (string.IsNullOrWhiteSpace(inputText))
            {
                return inputText;
            }

            string apiKey = "AIzaSyCqmtoKjNCDEf2b6rLn_FoGpL0NVQ565yg"; 
            string url = $"https://translation.googleapis.com/language/translate/v2?key={apiKey}";

            using (HttpClient client = new HttpClient())
            {
                var requestBody = new
                {
                    q = inputText,
                    target = languageCode
                };

                string jsonBody = Newtonsoft.Json.JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(url, content);

                if (!response.IsSuccessStatusCode)
                {
                    string errorDetails = await response.Content.ReadAsStringAsync();
                    throw new Exception($"API Error: {response.StatusCode}\n{errorDetails}");
                }

                string result = await response.Content.ReadAsStringAsync();
                JObject jsonResult = JObject.Parse(result);
                return jsonResult["data"]["translations"][0]["translatedText"].ToString();
            }
        }
    }
}
