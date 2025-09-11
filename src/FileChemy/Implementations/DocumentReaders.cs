using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.IO;

namespace FileChemy.Implementations
{
	public class ExcelDocumentReader : IDocumentReader
	{

        Application application = new Application();
        Workbook workbook;

        public DocumentType SupportedType => DocumentType.Excel;

		public DocumentContent Read(Stream input)
		{
            Dictionary<int, Worksheet> keyValuePairs = new Dictionary<int, Worksheet>();

            // 1. Stream �� �ӽ����� ���� 
            string tempFile = Path.GetTempFileName();
            
            try
            {
                using (var fileStream = File.Create(tempFile))
                {
                    // .NET Framework 3.5������ CopyTo�� �����Ƿ� ���� ����
                    byte[] buffer = new byte[4096];
                    int bytesRead;
                    while ((bytesRead = input.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        fileStream.Write(buffer, 0, bytesRead);
                    }
                }

                // Excel ���ø����̼��� ���� ó�� (����ڿ��� ������ �ʰ�)
                application.Visible = false;
                application.ScreenUpdating = false;

                // 2. �ӽ����� �� Excel ���μ���
                workbook = application.Workbooks.Open(tempFile);

                // 3. Excel ������ �б� - ��� ��ũ��Ʈ ���� ����
                System.Console.WriteLine($"�� ��ũ��Ʈ ��: {workbook.Worksheets.Count}");

                // ��ȯ�� ��ũ��Ʈ ã��
                findSheetToExport(keyValuePairs);


                // �����͸� ����Ʈ �迭�� ��ȯ
                byte[] resultData = System.Text.Encoding.UTF8.GetBytes(keyValuePairs[keyValuePairs.Count].ToString());

                // 4. DocumentContent ��ȯ
                return new DocumentContent
                {
                    FilePath = tempFile,
                    Data = resultData, // ���� Excel ������ ����
                    Type = DocumentType.Excel
                };
            }
            catch (System.Exception ex)
            {
                // ���� �߻� �� ���� �޽����� ������ ������ ��ȯ
                string errorMessage = $"Excel �б� ����: {ex.Message}";
                byte[] errorData = System.Text.Encoding.UTF8.GetBytes(errorMessage);
                
                return new DocumentContent
                {
                    FilePath = tempFile,
                    Data = errorData,
                    Type = DocumentType.Excel
                };
            }
            finally
            {
                // 5. ���ҽ� ����
                if (workbook != null)
                {
                    workbook.Close(false);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(workbook);
                }
                
                if (application != null)
                {
                    application.Quit();
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(application);
                }
                
                if (File.Exists(tempFile))
                {
                    File.Delete(tempFile);
                }
            }
		}

        /// <summary>
        /// PDF�� �̹����� ��ȯ�� ��ũ��Ʈ ã��
        /// Rule: 1. DATA��� �̸��� ���Ե� ��Ʈ�� ������ �� ���
        ///         => ��� ��Ʈ�� �˻��Ͽ� DATA��� �̸��� ���Ե� ��Ʈ �� 2�� A������ J������ �˻��Ͽ� "Sample ID"�� �ִ� �� ã�� 
        ///             -> ���� Sample ID�� �ִ� ���� 4���� ���� �ִ� ������ ��Ʈ�� ¦�� ��ȯ 
        ///       2. DATA��� �̸��� ���Ե� ��Ʈ�� �ϳ��� ���
        ///         => �ش� ��Ʈ���� Sample ID���� �ִ� ���� ������ ���� ã�� �ش� ǥ�� �̸��� ���� ��Ʈ�� ��ȯ
        /// </summary>
        /// <param name="keyValuePairs"></param>
        private void findSheetToExport(Dictionary<int, Worksheet> keyValuePairs)
        {
            int dataSheetCount = 0;
            // ��ü ��Ʈ�� ����
            foreach (Worksheet sheet in workbook.Worksheets)
            {
                if (sheet.Name.ToUpper().Contains("DATA"))
                {
                    dataSheetCount++;
                }
                if (dataSheetCount > 1)
                {
                    break;
                }
            }


            if(dataSheetCount > 1)
            {
                // �������� ���
                findSheetWhenMultiple(keyValuePairs);
            }
            else if(dataSheetCount == 1)
            {
                // �ϳ��� ���
                findSheetWhenSingle(keyValuePairs);
            }
            else
            {
                System.Console.WriteLine("DATA��� �̸��� ���Ե� ��Ʈ�� �����ϴ�.");
            }
            
        }

        /// <summary>
        /// DATA ��Ʈ�� �ϳ��� �� ��ȯ�� ��ũ��Ʈ�� ã�� �޼���
        /// </summary>
        /// <param name="keyValuePairs">��ȯ�� ��ũ��Ʈ�� ������ ��ųʸ� (Ű: �ε���, ��: ��ũ��Ʈ)</param>
        /// <remarks>
        /// ó�� ����:
        /// 1. ù ��° ��ũ��Ʈ���� 2�� A~J���� �˻��Ͽ� "Sample ID" ��� ���� ã��
        /// 2. Sample ID ������ 10�� �������� �����ϸ鼭 ������ ������ ���� ã�� (row+3 ��ġ�� �� �� Ȯ��)
        /// 3. ������ ������ �࿡�� 3�� ��(lastRow-3)�� A�� ���� �׽�Ʈ �̸����� ���
        /// 4. �ش� �׽�Ʈ �̸��� ��ġ�ϴ� ��ũ��Ʈ�� ã�� keyValuePairs�� ����
        /// </remarks>
        private void findSheetWhenSingle(Dictionary<int, Worksheet> keyValuePairs)
        {
            var ws = (Worksheet)workbook.Worksheets[1];

            int sampleIdColumnIndex = -1;

            //Sample Id �� ã��
            for (int col = 1; col <= 10; col++) // A��(1)���� J��(10)����
            {
                var headerCell = (Range)ws.Cells[2, col];
                string headerValue = headerCell.Value2?.ToString() ?? "";
                if (headerValue.ToUpper().Contains("SAMPLE ID"))
                {
                    sampleIdColumnIndex = col;
                    break;
                }
            }

            int lastRow = -1;

            // Sample ID ���� ã�Ҵٸ� sample ID ���� �ִ� ������ �� ã��
            if (sampleIdColumnIndex != -1)
            {
                
                for(int row = 1; row < ws.UsedRange.Rows.Count; row += 10)
                {
                    var cell = (Range)ws.Cells[row+3, sampleIdColumnIndex];
                    string cellValue = cell.Value2?.ToString() ?? "";
                    if (string.IsNullOrEmpty(cellValue))
                    {
                        break;
                    }
                    lastRow = cell.Row;
                }

         

                var sampleIdCell = (Range)ws.Cells[lastRow, sampleIdColumnIndex];
                var lastTestCell = (Range)ws.Cells[lastRow-3, 1];
                string sampleIdValue = sampleIdCell.Value2?.ToString() ?? "";
                string lastTestValue = lastTestCell.Value2?.ToString() ?? "";


                if (sampleIdValue == "")
                {
                    System.Console.WriteLine("Sample ID ���� �����ϴ�.");
                }
                System.Console.WriteLine($"��ũ��Ʈ '{lastTestValue}'�� Sample ID: {sampleIdValue}");

                // Sample ID ���� ���� �̸��� ��Ʈ ã��
                foreach (Worksheet sheet in workbook.Worksheets)
                {
                    if (sheet.Name.Equals(lastTestValue, StringComparison.OrdinalIgnoreCase))
                    {
                        keyValuePairs[1] = sheet;
                        break;
                    }
                }
            }
            else
            {
                System.Console.WriteLine("Sample ID ���� ã�� ���߽��ϴ�.");
            }
        }

        private void findSheetWhenMultiple(Dictionary<int, Worksheet> keyValuePairs)
        {
            for (int i = 1; i <= workbook.Worksheets.Count; i++)
            {
                var ws = (Worksheet)workbook.Worksheets[i];
                if (ws.Name.ToUpper().Contains("DATA"))
                {
                    // 2�� A������ J������ �˻��Ͽ� "Sample ID"�� �ִ� �� ã��
                    int sampleIdColumnIndex = -1;
                    for (int col = 1; col <= 10; col++) // A��(1)���� J��(10)����
                    {
                        var headerCell = (Range)ws.Cells[2, col];
                        string headerValue = headerCell.Value2?.ToString() ?? "";
                        if (headerValue.ToUpper().Contains("SAMPLE ID"))
                        {
                            sampleIdColumnIndex = col;
                            break;
                        }
                    }

                    // Sample ID ���� ã�Ҵٸ� 3���� ���� ��������
                    if (sampleIdColumnIndex != -1)
                    {
                        var sampleIdCell = (Range)ws.Cells[4, sampleIdColumnIndex];
                        string sampleIdValue = sampleIdCell.Value2?.ToString() ?? "";
                        if (sampleIdValue == "")
                        {
                            break;
                        }
                        System.Console.WriteLine($"��ũ��Ʈ '{ws.Name}'�� Sample ID: {sampleIdValue}");
                        keyValuePairs[i] = (Worksheet)workbook.Worksheets[i + 1];
                    }
                }
            }
        }

        public DocumentMetadata GetMetadata(Stream input)
		{
			// TODO: Excel ��Ÿ������ �б� ����
			return new DocumentMetadata
			{
				Title = "Excel Document",
				Author = "Unknown",
				PageCount = 1 // Excel�� ��� ��ũ��Ʈ ���� ���� �ʿ�
			};
		}
	}

	public class WordDocumentReader : IDocumentReader
	{
		public DocumentType SupportedType => DocumentType.Word;

		public DocumentContent Read(Stream input)
		{
			// TODO: Word ���� �б� ����
			// �����δ� OpenXML SDK �Ǵ� Word Interop�� ����� ����
			
			var buffer = new byte[input.Length];
			input.Read(buffer, 0, buffer.Length);
			
			return new DocumentContent
			{
				Data = buffer,
				Type = DocumentType.Word
			};
		}

		public DocumentMetadata GetMetadata(Stream input)
		{
			// TODO: Word ��Ÿ������ �б� ����
			return new DocumentMetadata
			{
				Title = "Word Document",
				Author = "Unknown",
				PageCount = 1 // ���� ������ ���� ���� �ʿ�
			};
		}
	}
}