using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace MD_cut {
    class Program {
        static void Main(string[] args) {
            try {
                string path = Console.ReadLine();
                FileStream file = File.Open(path, FileMode.Open);
                PdfDocument pdfDocument = PdfReader.Open(file);
                int pgNum = int.MaxValue;
                Regex rx = new Regex(@"\([0-9]*\)", RegexOptions.Compiled);
                List<PdfPage> pagesToDelete = new List<PdfPage>();
                for (int i = pdfDocument.Pages.Count - 1; i >= 0; i--) {
                    for (int j = 0; j < pdfDocument.Pages[i].Contents.Elements.Count; j++) {
                        byte[] arr = pdfDocument.Pages[i].Contents.Elements.GetDictionary(j).Stream.Value;
                        string converted = Encoding.UTF8.GetString(arr, 0, arr.Length);
                        string[] lines = converted.Split('\n');
                        foreach (string line in lines) {
                            if (line.StartsWith("/F19 ") && (line.EndsWith(")]TJ"))) {
                                MatchCollection matches = rx.Matches(line);
                                if (matches.Count == 2) {
                                    string mVal0 = matches[0].Value;
                                    int currPgNum = int.Parse(mVal0.Substring(1, mVal0.IndexOf(')') - 1));
                                    if (currPgNum < pgNum) {
                                        pgNum = currPgNum;
                                    }
                                    else if (currPgNum == pgNum) {
                                        pagesToDelete.Add(pdfDocument.Pages[i]);
                                    }
                                }
                            }
                        }
                    }
                }
                foreach (PdfPage page in pagesToDelete) {
                    pdfDocument.Pages.Remove(page);
                }
                pdfDocument.Save(Path.GetDirectoryName(path) + "\\" + Path.GetFileNameWithoutExtension(path) + "cut" + ".pdf");
            }
            catch (Exception e) {
                Console.Error.WriteLine(e.Message);
                return;
            }
        }
    }
}
