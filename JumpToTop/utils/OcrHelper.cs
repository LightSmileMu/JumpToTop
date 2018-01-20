using JumpToTop.utils;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using Tesseract;

namespace JumpToTop
{
    internal class OcrHelper
    {
        public static string GetOcrText(string startPath,string fileName)
        {
            var result = string.Empty;

            if (string.IsNullOrEmpty(fileName))
            {
                return string.Empty;
            }

            try
            {
                
                using (var engine = new TesseractEngine(startPath + @"/tessdata", "chi_sim", EngineMode.Default))
                {
                    using (var img = Pix.LoadFromFile(fileName))
                    {
                        using (var page = engine.Process(img))
                        {
                            result = page.GetText();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.Message);
            }
            
            return result;
        }

        
    }
}