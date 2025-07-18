using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;


namespace IMSS_SAM_Web
{
    public static class Recursos
    {
        public static string path = "";
        //static string key { get; set; } = "BIMSShi%XjjYY4YP3@Nob119X";

        //Conexción BD
        public static string strServer = "";
        public static string strBD = "";
        public static string strUsr = "";
        public static string strPassword = "";
        public static string strPathApp = "";
        public static string strPathFiles = "";
        public static string strRutaValmer = "";
        public static string strRutaPip = "";
        public static string strRutaAladdin = "";
        public static string strRutaLayouts = "";
        public static string strRutaAdjuntos = "";
        public static string strCorreoCodis = "";
        public static string strHoraInicio = "";
        public static string strHoraFin = "";


        public static string appfecha;
        public static string Pipfecha;
        public static string logfecha;
        public static string leyendalog;


        //public static string Encrypt(string text)
        //{
        //    using (var md5 = new MD5CryptoServiceProvider())
        //    {
        //        using (var tdes = new TripleDESCryptoServiceProvider())
        //        {
        //            tdes.Key = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
        //            tdes.Mode = CipherMode.ECB;
        //            tdes.Padding = PaddingMode.PKCS7;

        //            using (var transform = tdes.CreateEncryptor())
        //            {
        //                byte[] textBytes = UTF8Encoding.UTF8.GetBytes(text);
        //                byte[] bytes = transform.TransformFinalBlock(textBytes, 0, textBytes.Length);
        //                return Convert.ToBase64String(bytes, 0, bytes.Length);
        //            }
        //        }
        //    }
        //}

        //public static string Decrypt(string cipher)
        //{
        //    using (var md5 = new MD5CryptoServiceProvider())
        //    {
        //        using (var tdes = new TripleDESCryptoServiceProvider())
        //        {
        //            tdes.Key = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
        //            tdes.Mode = CipherMode.ECB;
        //            tdes.Padding = PaddingMode.PKCS7;

        //            using (var transform = tdes.CreateDecryptor())
        //            {
        //                byte[] cipherBytes = Convert.FromBase64String(cipher);
        //                byte[] bytes = transform.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
        //                return UTF8Encoding.UTF8.GetString(bytes);
        //            }
        //        }
        //    }
        //}

    }

}

