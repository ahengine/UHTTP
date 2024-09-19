using UnityEngine;

namespace UHTTP
{
    public enum HTTPRequestMethod { GET,POST,PUT,HEAD,CREATE,DELETE }

    [System.Serializable]
    public class FormBinaryData
    {
        public string fieldName;
        public string filename;
        public byte[] data;
        public string mimeType;

        public FormBinaryData(string fieldName, byte[] data)
        {
            this.fieldName = fieldName;
            this.data = data;
        }

        public FormBinaryData(string fieldName, byte[] data, string filename)
        {
            this.fieldName = fieldName;
            this.data = data;
            this.filename = filename;
        }

        public FormBinaryData(string fieldName, byte[] data, string filename, string mimeType)
        {
            this.fieldName = fieldName;
            this.data = data;
            this.filename = filename;
            this.mimeType = mimeType;
        }

        public WWWForm FormWithBinaryData() 
        {
            var form = new WWWForm();

                if(!string.IsNullOrEmpty(mimeType))
                    form.AddBinaryData(fieldName, data, filename, mimeType);
            else 
                if(!string.IsNullOrEmpty(filename))
                    form.AddBinaryData(fieldName, data, filename);
            else 
                form.AddBinaryData(fieldName, data);

            return form;
        }
    }
}