using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePACSLoans.Utilities
{
    public class AccessInputsFromJSON
    {
        public string GetData(string filePath, string jsonObjectName, string inputKey)
        {
            try
            {
                var json = File.ReadAllText(filePath);
                var jObj = JObject.Parse(json);
                var obj = jObj[jsonObjectName];
                if (obj == null)
                    throw new Exception($"JSON object '{jsonObjectName}' not found.");
                var value = obj[inputKey]?.ToString();

                if (value == null)
                    throw new Exception($"Input key '{inputKey}' not found inside '{jsonObjectName}'.");

                return value;
            }
            catch(Exception ex) 
            {
                return "0";
            }
        }

        public void UpdateData(string filePath, string jsonObjectName, string attribute, string valueToBeUpdated)
        {
            const int maxRetries = 5;
            const int delay = 300; // milliseconds
            int retryCount = 0;

            while (true)
            {
                try
                {
                    // Try to read the file
                    string jsonContent = File.ReadAllText(filePath);

                    // Parse JSON
                    JObject jsonObj = JObject.Parse(jsonContent);

                    // Validate target JSON object
                    if (jsonObj[jsonObjectName] == null)
                        throw new Exception($"Object '{jsonObjectName}' not found in JSON.");

                    // Get the object (must be JObject)
                    JObject targetObj = jsonObj[jsonObjectName] as JObject;

                    if (targetObj == null)
                        throw new Exception($"'{jsonObjectName}' is not a JSON object.");

                    // Validate attribute
                    if (targetObj[attribute] == null)
                        throw new Exception($"Attribute '{attribute}' not found under '{jsonObjectName}'.");

                    // Update the value
                    targetObj[attribute] = valueToBeUpdated;

                    // Write back to file
                    File.WriteAllText(filePath, jsonObj.ToString());

                    break; // Success → exit retry loop
                }
                catch (IOException)
                {
                    // File is locked → retry
                    retryCount++;

                    if (retryCount >= maxRetries)
                        throw new IOException("Unable to update JSON file. The file is locked by another process.");

                    Thread.Sleep(delay); // wait and retry
                }
                catch (Exception ex)
                {
                    // Other exceptions (parse error, missing key, etc.)
                    throw new Exception($"Error updating JSON: {ex.Message}", ex);
                }
            }
        }

    }
}
