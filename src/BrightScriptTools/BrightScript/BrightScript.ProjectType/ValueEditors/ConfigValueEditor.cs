using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using BrightScript.ValueEditorsUI;
using Microsoft.VisualStudio.ProjectSystem.Designers.Properties;
using Microsoft.VisualStudio.ProjectSystem.Properties;
using Microsoft.VisualStudio.ProjectSystem.Utilities;

namespace BrightScript.ValueEditors
{
    [Export(typeof(IPropertyPageUIValueEditor))]
    [ExportMetadata("Name", "ConfigValueEditor")]
    [AppliesTo(MyUnconfiguredProject.UniqueCapability)]
    public class ConfigValueEditor : IPropertyPageUIValueEditor
    {
        /// <summary>
        /// Invokes the editor.
        /// </summary>
        /// <param name="serviceProvider">The set of potential services the component can query for, mainly for access back to the host itself.</param>
        /// <param name="ruleProperty">the property being edited</param>
        /// <param name="currentValue">the current value of the property (may be different than property.Value - for example if host UI caches the new values until Apply button)</param>
        /// <returns>The new value.  May be <paramref name="currentValue"/> if no change is intended.</returns>
        public async Task<object> EditValueAsync(IServiceProvider serviceProvider, IProperty ruleProperty, object currentValue)
        {
            var editor = new ConfigValueEditorWindow();
            editor.Text = ruleProperty.Description;

            var configDt = new DataTable();
            configDt.Columns.Add(new DataColumn("Key"));
            configDt.Columns.Add(new DataColumn("Value"));
            var replacesDt = new DataTable();
            replacesDt.Columns.Add(new DataColumn("Key"));
            replacesDt.Columns.Add(new DataColumn("Value"));
            replacesDt.Columns.Add(new DataColumn("Enable", typeof(bool)));

            var value = currentValue as string;
            if (value != null)
            {
                try
                {
                    var tables = value.Split('|');
                    if (tables.Length == 2)
                    {
                        tables[0].Split(';')
                            .ToList()
                            .ForEach(s =>
                            {
                                var parts = s.Split(',');
                                if (parts.Length == 2)
                                    configDt.Rows.Add(parts[0], parts[1]);
                            });

                        tables[1].Split(';')
                            .ToList()
                            .ForEach(s =>
                            {
                                var parts = s.Split(',');
                                if (parts.Length == 3)
                                    replacesDt.Rows.Add(parts[0], parts[1], bool.Parse(parts[2]));
                            });
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            editor.SetData(configDt, replacesDt);

            if (editor.ShowDialog() == DialogResult.OK)
            {
                var configList = new List<string>();
                foreach (DataRow row in configDt.Rows)
                {
                    var key = row["Key"].ToString();
                    var val = row["Value"].ToString();
                    if (!string.IsNullOrWhiteSpace(key) && !string.IsNullOrWhiteSpace(val))
                        configList.Add(string.Join(",", key, val));
                }

                var replaceList = new List<string>();
                foreach (DataRow row in replacesDt.Rows)
                {
                    var key = row["Key"].ToString();
                    var val = row["Value"].ToString();
                    var enable = row["Enable"].ToString();
                    if (!string.IsNullOrWhiteSpace(key) && !string.IsNullOrWhiteSpace(val) && !string.IsNullOrWhiteSpace(enable))
                        replaceList.Add(string.Join(",", key, val, enable));
                }

                return string.Join("|", string.Join(";", configList), string.Join(";", replaceList));
            }

            return currentValue;
        }
    }
}